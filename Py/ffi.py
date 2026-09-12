import pyodbc
import csv
import sys
import time

# ----------------------------
# Command-line arguments
# ----------------------------
if len(sys.argv) < 3:
    print("Usage:")
    print("  python ffi.py <LoadsetName> <Inputfile.csv>")
    sys.exit(1)

loadsetName = sys.argv[1]
InFile = sys.argv[2]

print(f"Uploading to loadset: {loadsetName}")
print(f"Input file: {InFile}")

# ----------------------------
# Database connection
# ----------------------------
file_path = r"C:\Harmonize\App\connection.txt"

try:
    with open(file_path, "r") as f:
        conn_str = f.readline().strip()
except FileNotFoundError:
    raise Exception("Connection file not found")

conn_str = conn_str.replace("User ID=", "UID=").replace("Password=", "PWD=").replace("Initial Catalog=", "Database=")

if "Integrated" in conn_str or "Integrated Security" in conn_str:
    driver = "SQL Server"
else:
    driver = "ODBC Driver 17 for SQL Server"

conn = pyodbc.connect(f"Driver={{{driver}}};{conn_str}")
print(f"Connected via {driver}")

cursor = conn.cursor()
cursor.fast_executemany = True

start_time = time.time()

# ----------------------------
# Step 1: Clear old staging data
# ----------------------------
cursor.execute("EXEC dbo.UTILS_BulkStagingClear ?", loadsetName)
conn.commit()
print("Staging table cleared")

# ----------------------------
# Step 2: Bulk insert CSV into staging via proc
# ----------------------------
staging_sql = "EXEC dbo.UTILS_BulkStagingInsert ?, ?, ?, ?, ?, ?"

BATCH_SIZE = 5000
rowcnt = 0
batch = []

with open(InFile, encoding='utf-8', newline='') as f:
    reader = csv.reader(f, delimiter=',')

    for rowcnt, row in enumerate(reader, start=1):
        if rowcnt == 1:
            continue

        try:
            mySname   = row[0].strip().replace('"', '')
            unitId    = int(row[1])
            valueDate = row[2]
            myVal     = float(row[3])
            myDesc    = row[4].strip().replace('"', '')

            batch.append((loadsetName, mySname.upper(), myDesc, unitId, valueDate, myVal))

            if len(batch) >= BATCH_SIZE:
                cursor.executemany(staging_sql, batch)
                conn.commit()
                elapsed = time.time() - start_time
                print(f'{rowcnt-1} rows staged. Elapsed: {elapsed:.1f}s')
                batch = []

        except Exception as e:
            print(f"\n* Error parsing row {rowcnt}: {e}")
            continue

# Flush remaining rows
if batch:
    cursor.executemany(staging_sql, batch)
    conn.commit()

elapsed = time.time() - start_time
total_rows = rowcnt - 1
print(f"\nStaging complete: {total_rows} rows in {elapsed:.1f}s")

# ----------------------------
# Step 3: Bulk upsert from staging to production
# ----------------------------
print(f"Running bulk upsert for: {loadsetName}")
cursor.execute("EXEC dbo.UTILS_BulkUpsert ?", loadsetName)
conn.commit()
elapsed = time.time() - start_time
print(f"Bulk upsert done. Elapsed: {elapsed:.1f}s")

# ----------------------------
# Step 4: Update statistics
# ----------------------------
print(f"Updating statistics for: {loadsetName}")
cursor.execute(f"EXEC UTILS_UpdateCurveInfo {loadsetName}")
conn.commit()

elapsed = time.time() - start_time
print(f"\nCompleted: {total_rows} rows in {elapsed:.1f}s")
conn.close()
print('Done')
