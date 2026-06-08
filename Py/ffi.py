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


file_path = "connection.txt"



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

            cursor.execute(
                sql,
                loadsetName,
                mySname.upper(),
                myDesc,
                unitId,
                valueDate,
                myVal
            )

            #comitting for every 100, 100, can be 1500 which is better
            if rowcnt % 100 == 0:            
                conn.commit()
                print( str(rowcnt) + ' Bulk committing rows.')            
            

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
print(f"Updating the Statistics for :  {loadsetName}")
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End of ffi processing : fast flexiblefile interface')





