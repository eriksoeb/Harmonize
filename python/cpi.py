from pyjstat import pyjstat
import requests
import pyodbc
import pandas as pd
import urllib
import datetime
import sys
import time





print("Starting ...")

storedProcName = 'UTILS_PutTime' ##the api proc to upsert data into an existing loadset
loadsetName = 'CPI_NO'

unitId= '20' #Index
  
                                                                      
# my public test user                 
#conn = pyodbc.connect('Driver={SQL Server};'
#                      'SERVER=localhost\\MSSQLSERVER2022;'
#                      'Database=Harmonize;'
#                      'Connection Timeout=5;'
#                      'UId=mypublic;'
#                      'PWD=Password2026;')
                      
                      
                      
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


#CPI data json stat statbank www.ssb.no
POST_URL = 'https://data.ssb.no/api/v0/en/table/03013'

# API query for selected items some obervations back in time, 
payload = {"query": [{"code": "Konsumgrp", "selection": 
{"filter": "item", "values": ["TOTAL","01","03","02","04","05","06","07","08","09","10","11","12","01.1.1.1_11111", "01.2.1.1_12111", "01.1.4.7_11471", "01.1.8.4_11843"] } },  
{"code": "ContentsCode", "selection": 
{"filter": "item", "values": ["KpiIndMnd"] } 
}, 
{"code": "Tid", "selection": {"filter": "top", "values": ["50"] } } ],  #last n observations 
"response": {"format": "json-stat"}
 }



resultat = requests.post(POST_URL, json=payload)
data = resultat.json()  # <-- original JSON


# --- Build label → code mapping once ---
labels = data['dataset']['dimension']['Konsumgrp']['category']['label']
label_to_code = {v: k for k, v in labels.items()}

# --- Convert JSON-stat to DataFrame ---
dataset = pyjstat.Dataset.read(resultat.text)
df = dataset.write('dataframe')
df = df.rename(columns={'consumption group': 'consumption_group'})

# --- Add a column with the code instead of label ---
df['konsum_code'] = df['consumption_group'].map(label_to_code)

# --- Convert month to timezone-aware datetime ---
df['naive'] = pd.to_datetime(df['month'], format="%YM%m")
df['tz_no'] = df.naive.dt.tz_localize('Europe/Oslo')

# --- Loop through rows and execute stored procedure ---
for index, row in df.iterrows():
    myVareGruppe = row['konsum_code']  # <-- now using the code

    myVal = str(row.value)
    valueDate = str(row.tz_no)[0:10] 
    

    # Remove spaces and prepare names for SQL
    mySname = str.replace('C'+myVareGruppe + '.IDX', ' ', '')
    myDesc = row['consumption_group']

    if myVal != 'nan':
       
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
            

        except Exception as e:
            print(f"\n❌ SQL error on row {index}")
            print(e)
            conn.rollback()
            sys.exit(1)
        
        
        
        #comitting for every 100, can be 1500 which is better
        if index % 100 == 0 and index > 0:            
            conn.commit()
            print( str(index) + ' Bulk committing rows.')
           

conn.commit()
print("A total of " + str(index) + " rows have been updated and commited")

# --- Call additional stored procedure ---
print(f"Updating the Stats for :  {loadsetName}")
cursor = conn.cursor()
conn.execute(f'EXEC UTILS_UpdateCurveInfo {loadsetName}')
conn.commit()
conn.close()
print('End')

