from pyjstat import pyjstat
import requests
import pyodbc
import pandas as pd
import urllib
import datetime
import sys
import time


POST_URL = "https://data.ssb.no/api/v0/en/table/14700"

metadata = requests.get(POST_URL).json()

#for var in metadata["variables"]:
#    print(var["code"])


#print(pyodbc.drivers())



print("Starting ...")

storedProcName = 'UTILS_PutTime' ##the api proc to upsert data into an existing loadset
loadsetName = 'CPI'

unitId= '20' #Indeks
  


file_path = r"C:\Harmonize\App\connection.txt"


try:
    with open(file_path, "r") as f:
        conn_str = f.readline().strip()
except FileNotFoundError:
    raise Exception("Connection file not found")

# Translate ADO.NET/C# keywords to ODBC keywords so pyodbc can parse them
conn_str = conn_str.replace("User ID=", "UID=").replace("Password=", "PWD=").replace("Initial Catalog=", "Database=")

print ("Connection: " , conn_str)

# Decide driver based on content
if "Integrated" in conn_str or "Integrated Security" in conn_str:
    driver = "SQL Server"
else:
    driver = "ODBC Driver 17 for SQL Server"

def open_connection():
    c = pyodbc.connect(f"Driver={{{driver}}};{conn_str}", timeout=30)
    c.timeout = 60
    return c

conn = open_connection()
print(f"Connected via {driver}")




# Add driver in front
#Trusted
#conn = pyodbc.connect(f"Driver={{SQL Server}};{conn_str}")

# Add driver in front
#cloud
#conn = pyodbc.connect(f"Driver={{ODBC Driver 17 for SQL Server}};{conn_str}")

                                                              
                                       
cursor = conn.cursor()

# ----------------------------
# SQL with parameter markers 
# ----------------------------
sql = f"""
EXEC {storedProcName}
     ?, ?, ?, ?, ?, ?;
"""


# API query for selected items some observations back in time,
    # // {"code": "VareTjenesteGrp", "selection": {"filter": "item", "values": ["00","01","03","02","04","05","06","07","08","09","10","11","12","01.1.1.1",  "01.1.4.7", "01.1.8.4"]}},


payload = {
    "query": [
        {"code": "VareTjenesteGrp", "selection": {"filter": "all", "values": ["*"]}},
        {"code": "ContentsCode", "selection": {"filter": "item", "values": ["KpiIndMnd"]}},
        {"code": "Tid", "selection": {"filter": "top", "values": ["2"]}}
    ],
    "response": {"format": "json-stat"}
}


resultat = requests.post(POST_URL, json=payload)
data = resultat.json()  # <-- original JSON

#print ("erik debug")
#print (resultat)
#print (data)

# --- Build label → code mapping once ---dataset failes
labels = data['dataset']['dimension']['VareTjenesteGrp']['category']['label']
label_to_code = {v: k for k, v in labels.items()}

# --- Convert JSON-stat to DataFrame ---
#print (resultat.text)

dataset = pyjstat.Dataset.read(resultat.text)
df = dataset.write('dataframe')
df = df.rename(columns={'goods and services': 'consumption_group'})

# --- Add a column with the code instead of label ---
df['konsum_code'] = df['consumption_group'].map(label_to_code)

# --- Convert month to timezone-aware datetime ---
df['naive'] = pd.to_datetime(df['month'], format="%YM%m")
df['tz_no'] = df.naive.dt.tz_localize('Europe/Oslo')

start_time = time.time()

# --- Loop through rows and execute stored procedure ---
for index, row in df.iterrows():
    myVareGruppe = row['konsum_code']  # <-- now using the code

    myVal = str(row.value)
    valueDate = str(row.tz_no)[0:10] 
    

    # Remove spaces and prepare names for SQL
    mySname = str.replace('C'+myVareGruppe + '.IDX', ' ', '')
    myDesc = row['consumption_group']

    if myVal != 'nan':

        for attempt in range(3):
            try:
                cursor.execute(
                    sql,
                    loadsetName,
                    mySname.upper(),
                    myDesc,
                    unitId,
                    valueDate,
                    myVal
                )
                break
            except pyodbc.Error as e:
                err = str(e)
                if ('08S01' in err or '40001' in err) and attempt < 2:
                    print(f"\n* Connection/deadlock error at row {index}, reconnecting (attempt {attempt+2})...")
                    time.sleep(2)
                    conn = open_connection()
                    cursor = conn.cursor()
                else:
                    print(f"\n* SQL error on row {index}, continuing...")
                    print(e)
                    break

        if index % 500 == 0 and index > 0:
            conn.commit()
            elapsed = time.time() - start_time
            print(f"{index} Bulk committing rows. Elapsed: {elapsed:.1f}s")

conn.commit()
elapsed = time.time() - start_time
print(f"A total of {index} rows have been updated and commited in {elapsed:.1f}s")

# --- Call additional stored procedure ---
print(f"Updating the Stats for :  {loadsetName}")
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('Done')

