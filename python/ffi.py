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
    print("  python ffi.py <LoadsetName> <InputFile.csv>")
    sys.exit(1)

loadsetName = sys.argv[1]
InFile = sys.argv[2]

print(f"Uploading to loadset: {loadsetName}")
print(f"Input file: {InFile}")

# ----------------------------
# SQL connection (Trusted)
# ----------------------------
conn = pyodbc.connect(
    'Driver={SQL Server};'
    'SERVER=localhost\\MSSQLSERVER2022;'
    'Database=Harmonize;'
    'Trusted_Connection=True;'
    'Connection Timeout=5;'
)

cursor = conn.cursor()

# ----------------------------
# SQL with parameter markers 
# ----------------------------
sql = f"""
EXEC {storedProcName}
     ?, ?, ?, ?, ?, ?;
"""

start_time = time.time()
rowcnt = 0

# ----------------------------
# CSV processing
# ----------------------------
with open(InFile, encoding='utf-8', newline='') as f:
    reader = csv.reader(f, delimiter=';')

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

            cursor.execute(
                sql,
                loadsetName,
                mySname.upper(),
                myDesc,
                unitId,
                valueDate,
                myVal
            )

        except Exception as e:
            print(f"\n❌ SQL error on row {rowcnt}")
            print(e)
            conn.rollback()
            sys.exit(1)

# ----------------------------
# Commit & finish
# ----------------------------
conn.commit()

elapsed = time.time() - start_time

print(f"\n✅ Completed successfully")
print(f"Rows processed: {rowcnt - 1}")
print(f"Elapsed time: {elapsed:.2f} seconds")


# --- Call additional stored procedure ---
print(f"Updating the Stats for :  {loadsetName}")
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End')





