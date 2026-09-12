

#PPI ERIK

from pyjstat import pyjstat
import requests
import pyodbc
import pandas as pd
import urllib
from datetime import datetime
import math
import json
import datetime
import sys
import time
              

unitId= '20' #Indeks - takes ppi from stats norway sample some last rows of some series

storedProcName = 'UTILS_PutTime'
loadsetName = 'PPI'


print("Starting upserting loadset...: "+ loadsetName)

           
                              
                      
#sjd use ur built in ad login trusted connection                    
#conn = pyodbc.connect('Driver={SQL Server};'
 #                     'SERVER=localhost\\MSSQLSERVER2022;'
  #                    'Database=Harmonize;'
   #                   'Connection Timeout=5;'
    #                  'Integrated Security=True;')
                      
    

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


# PPI data from SSB API


POST_URL = "https://data.ssb.no/api/v0/en/table/12462"

#all   {"code": "NaringUtenriks", "selection": {"filter": "all", "values": ["*"]}},

payload = {
    "query": [
        {"code": "Marked", "selection": {"filter": "item", "values": ["00"]}},
        {"code": "NaringUtenriks", "selection": {"filter": "all", "values": ["*"]}},
        {
            "code": "ContentsCode",
            "selection": {"filter": "item", "values": ["Indeksnivo"]},
        },
        {"code": "Tid", "selection": {"filter": "top", "values": ["2"]}},  # last n observations/months 100 in start
    ],
    "response": {"format": "json-stat2"},
}

# --- FETCH DATA ---
response = requests.post(POST_URL, json=payload)
response.raise_for_status()
data = response.json()

# --- PARSE DIMENSIONS ---
dims = data["dimension"]
industries = dims["NaringUtenriks"]["category"]["label"]
periods = dims["Tid"]["category"]["label"]
values = data["value"]

# --- GENERATE EXEC LINES ---
start_time = time.time()
i = 0
for mySname, myDesc in industries.items():
    for period_code in periods.keys():
        myVal = values[i]
        i += 1
        valueDate = f"{period_code[:4]}-{period_code[5:7]}-01"

        #print(f"period code {period_code}")
        #print(f"valuedate debug  {valueDate}")

        # skip missing or NaN values
        if myVal != 'nan':
        
        
       
            try:
                cursor.execute(
                    sql,
                    loadsetName,
                    mySname.upper()+'.IDX',  #better name ? 
                    myDesc,
                    unitId,
                    valueDate,
                    myVal
                    )
            
            except Exception as e:
                print(f"\n❌ SQL error on row {i}")
                print(e)
                conn.rollback()
                sys.exit(1)
        
        
            #comitting for every 10, 100, can be 1500 which is better
            if i % 1000 == 0 and i > 0:
                conn.commit()
                elapsed = time.time() - start_time
                print(f"{i} Bulk committing rows. Elapsed: {elapsed:.1f}s")

# --- Commit updates ---
conn.commit()
elapsed = time.time() - start_time
print(f"A total of {i} rows have been updated and commited in {elapsed:.1f}s")  

# --- Call additional stored procedure ---
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End')

