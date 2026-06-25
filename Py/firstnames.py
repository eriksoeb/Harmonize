import requests
import pyodbc
import sys
import time

print("Starting firstnames ...")

storedProcName = 'UTILS_PutTime'
loadsetName = sys.argv[1] if len(sys.argv) > 1 else 'FirstNames'
unitId = 1  # antall (count of people with the name)

# ----------------------------
# Connection
# ----------------------------
file_path = r"C:\Harmonize\App\connection.txt"
try:
    with open(file_path, "r") as f:
        conn_str = f.readline().strip()
except FileNotFoundError:
    raise Exception("Connection file not found")

conn_str = conn_str.replace("User ID=", "UID=").replace("Password=", "PWD=").replace("Initial Catalog=", "Database=")
print("Connection:", conn_str)

if "Integrated" in conn_str or "Integrated Security" in conn_str:
    conn = pyodbc.connect(f"Driver={{SQL Server}};{conn_str}")
else:
    conn = pyodbc.connect(f"Driver={{ODBC Driver 17 for SQL Server}};{conn_str}")

cursor = conn.cursor()
cursor.fast_executemany = True

sql = f"EXEC {storedProcName} ?, ?, ?, ?, ?, ?;"

# ----------------------------
# SSB API - Norwegian first names, table 10467
# ----------------------------
POST_URL = 'https://data.ssb.no/api/v0/no/table/10467'

payload = {
    "query": [
        {"code": "Fornavn", "selection": {"filter": "*", "values": [
            "1KIM","2KIM","1JANNE","1ASTRID","2HANS","2ERIK","1HILDE",
            "1CHRISTINA","2GEIR","1MARIE","1TRINE","1ANETTE","1HELGA",
            "2JO","2JON","1RAGNHILD","1CAMILLA","2CHRISTIAN","2ARVID"
        ]}},
        {"code": "ContentsCode", "selection": {"filter": "item", "values": ["Personer"]}},
        {"code": "Tid", "selection": {"filter": "top", "values": ["5"]}}
    ],
    "response": {"format": "json-stat"}
}

print("Fetching data from SSB ...")
resultat = requests.post(POST_URL, json=payload)
resultat.raise_for_status()

data = resultat.json()
mynames = data["dataset"]["dimension"]["Fornavn"]["category"]["label"]  # code → label
myyears = data["dataset"]["dimension"]["Tid"]["category"]["label"]       # code → label
values  = data["dataset"]["value"]

# ----------------------------
# Build batch and upload
# ----------------------------
BATCH_SIZE = 500
batch = []
start_time = time.time()
ycnt = 0

for namecode, namedesc in mynames.items():
    sname = (namecode + ".ANT").upper()
    for year in myyears:
        raw = values[ycnt]
        ycnt += 1
        if raw is None:
            continue
        batch.append((loadsetName, sname, namedesc, unitId, year, float(raw)))

        if len(batch) >= BATCH_SIZE:
            cursor.executemany(sql, batch)
            conn.commit()
            print(f"{ycnt} rows committed.")
            batch = []

# flush remaining
if batch:
    cursor.executemany(sql, batch)
    conn.commit()

elapsed = time.time() - start_time
print(f"\n✅ Completed: {ycnt} observations in {elapsed:.2f} seconds")

# ----------------------------
# Update statistics
# ----------------------------
print(f"Updating statistics for: {loadsetName}")
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print("End of firstnames processing")
