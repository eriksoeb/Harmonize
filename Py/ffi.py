import pyodbc
import csv
import sys
import time

# ----------------------------
# Stored procedure (NO exec!)
# ----------------------------
storedProcName = 'UTILS_PutTime'

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


# -- file_path = "connection.txt"
file_path = r"C:\Harmonize\App\connection.txt"



try:
    with open(file_path, "r") as f:
        conn_str = f.readline().strip()
except FileNotFoundError:
    raise Exception("Connection file not found")

conn_str = conn_str.replace("User ID=", "UID=").replace("Password=", "PWD=").replace("Initial Catalog=", "Database=")

print ("Connection: " , conn_str)

# Decide driver based on content
if "Integrated" in conn_str or "Integrated Security" in conn_str:
    conn = pyodbc.connect(f"Driver={{SQL Server}};{conn_str}")
else:
    conn = pyodbc.connect(f"Driver={{ODBC Driver 17 for SQL Server}};{conn_str}")



cursor = conn.cursor()
cursor.fast_executemany = True  # sends full batch in one network call

# ----------------------------
# SQL with parameter markers
# ----------------------------
sql = f"""
EXEC {storedProcName}
     ?, ?, ?, ?, ?, ?;
"""

BATCH_SIZE = 500  # rows per executemany call

start_time = time.time()
rowcnt = 0
batch = []

# ----------------------------
# CSV processing
# ----------------------------

with open(InFile, encoding='utf-8', newline='') as f:
    reader = csv.reader(f, delimiter=',')

    for rowcnt, row in enumerate(reader, start=1):
        # Skip header
        if rowcnt == 1:
            continue

        try:
            mySname   = row[0].strip().replace('"', '')
            unitId    = int(row[1])
            valueDate = row[2]              # YYYY-MM-DD
            myVal     = float(row[3])
            myDesc    = row[4].strip().replace('"', '')

            batch.append((loadsetName, mySname.upper(), myDesc, unitId, valueDate, myVal))

            if len(batch) >= BATCH_SIZE:
                cursor.executemany(sql, batch)
                conn.commit()
                print(f'{rowcnt} rows committed.')
                batch = []

        except Exception as e:
            print(f"\n❌ SQL error on row {rowcnt}")
            print(e)
            conn.rollback()
            sys.exit(1)



# ----------------------------
# Flush remaining rows
# ----------------------------
if batch:
    cursor.executemany(sql, batch)
    conn.commit()

elapsed = time.time() - start_time

print(f"\n✅ Completed successfully")
print(f"Rows processed: {rowcnt - 1}")
print(f"Elapsed time: {elapsed:.2f} seconds")


# --- Call additional stored procedure ---
print(f"Updating the Statistics for :  {loadsetName}")
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End of ffi processing : fast flexiblefile interface')
