

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
              

unitId= '20' #Index

storedProcName = 'UTILS_PutTime'
loadsetName = 'PPI'


print("Starting upserting loadset...: "+ loadsetName)

           
                              
                      
#sjd use ur built in ad login trusted connection                    
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost\\MSSQLSERVER2022;'
                      'Database=Harmonize;'
                      'Connection Timeout=5;'
                      'Integrated Security=True;')
                      
                      
                                                        
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


payload = {
    "query": [
        {"code": "Marked", "selection": {"filter": "item", "values": ["00"]}},
        {
            "code": "NaringUtenriks",
            "selection": {
                "filter": "item","values": [
                    "SNN0",
                    "SNN06_TOT",
                    "SNN10_33",
                    "SNN35_TOT",
                    "SPE4",
                    "E6_TOT",
                    "SNN06",
          "SNN08",
          "SNN09",
          "SNN10",
          "SNN11",
          "SNN16",
          "SNN17",
          "SNN19",
          "SNN20",
          "SNN22",
          "SNN23",
          "SNN24",
          "SNN25",
          "SNN26",
          "SNN27",
          "SNN28",
          "SNN31",
          "SNN33",
          "SNN35",
          "SNN36"                 
                ],
            },
        },
        {
            "code": "ContentsCode",
            "selection": {"filter": "item", "values": ["Indeksnivo"]},
        },
        {"code": "Tid", "selection": {"filter": "top", "values": ["2"]}},  # last n observations
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
            if i % 10 == 0 and i > 0:            
                conn.commit()
                print( str(i) + ' Bulk committing rows.')
             
# --- Commit updates ---
conn.commit()
print('Stats updated')
print("A total of " + str(i) + " rows have been updated and commited")  

# --- Call additional stored procedure ---
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End')

