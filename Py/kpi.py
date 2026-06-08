print("Starting ...")
#definerer navn på lagret proc i sqldatabase med noen params for å finne den i gjen.

#MyShortGrp =''
#MyShortVar = ''
MyFnutt = "'"
MyComma =', '
MyPeriod ='.'
unitId= '20'

storedProcName = 'exec UTILS_PutTime'
## not needed loadsetId = 8
loadsetName = "'CPI'"

from pyjstat import pyjstat
import requests
import pyodbc
import pandas as pd
import urllib
import datetime

#import time
                     

#update user must have access                      
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost\\MSSQLSERVER2022;'
                      'Database=dw;'
                      'UId=sa;'
                      'PWD=MyTime2025!!!;')
                                               
                                       
cursor = conn.cursor()


#CPI data json stat www.ssb.no
POST_URL = 'https://data.ssb.no/api/v0/no/table/03013'

# API spørring OK tar moen konsumgrupper 57 observasjoner mnd tilbake - dvs endel historien   "KpiVektMnd"
payload = {"query": [{"code": "Konsumgrp", "selection": 
{"filter": "item", "values": ["TOTAL","01","03","02","04","05","06","07","08","09","10","11","12","01.1.1.1_11111", "01.2.1.1_12111", "01.1.4.7_11471", "01.1.8.4_11843"] } }, 
#{"filter": "item", "values": ["01.1.8.4_11843"] } }, 
{"code": "ContentsCode", "selection": 
{"filter": "item", "values": ["KpiIndMnd"] } 
}, 
{"code": "Tid", "selection": {"filter": "top", "values": ["2"] } } ], 
"response": {"format": "json-stat"}
 }


resultat = requests.post(POST_URL, json = payload)


#resultatkoden
#print(resultat.text)


dataset = pyjstat.Dataset.read(resultat.text) 
df = dataset.write('dataframe')


#print(df)

df['naive'] = pd.to_datetime(df['måned'], format="%YM%m")
df['tz_no'] = df.naive.dt.tz_localize('Europe/Oslo')


#print (df['naive'] )
#print (df['tz_no'] )


for index, row in df.iterrows():
	myVareGruppe = row.konsumgruppe
	#myVariabel= row.statistikkvariabel # konsum pris 2015=100 nais for Documetation later
	myVal = str(row.value)
	valueDate= "'"+(str(row.tz_no)[0:10])+ "'"
    	#ta bort spacer?
	mySname = str.replace(MyFnutt+myVareGruppe+'.IDX'+MyFnutt, ' ','')
	myDoc = MyFnutt+myVareGruppe+MyFnutt#+myVariabel+MyFnutt
	if myVal != 'nan': 
		StoredProc = f"{storedProcName} {loadsetName}, {mySname.upper()}, {myDoc}, {unitId}, {valueDate}, {myVal}, NULL"
		print(StoredProc)
		conn.execute(StoredProc)
    	#dytter inn rad for rad i sql database med upsert (overskriver dersom raden er der fr a før


conn.commit()		
print('stats..')
cursor = conn.cursor()
conn.execute('exec UTILS_UpdateCurveInfo '+loadsetName)
conn.commit()
print('end')





