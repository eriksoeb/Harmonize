

import time

from datetime import datetime,timedelta

import json
import subprocess
import requests
import math
import pyodbc
import ssl #need ??
import urllib3

import pandas as pd
from pyjstat import pyjstat


import re
import getopt,sys

from datetime import datetime

MyComma =', '
MyPeriod ='.'
MyCountry = ''
MyVariabel = ''
MyDate =''
MyVal =''
StoredProc = ''
loadsetname = 'Waterlevel'

MySetName = f"'{loadsetname}'"
procname = 'UTILS_PutTime '
  


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
                      
 


headers = ""

#alle data first load only
#url = 'https://biapi.nve.no/magasinstatistikk/api/Magasinstatistikk/HentOffentligData'


#naar ikke full last/historie kjores siste uke
url='https://biapi.nve.no/magasinstatistikk/api/Magasinstatistikk/HentOffentligDataSisteUke'

r = requests.request("GET", url,  headers=headers, verify=True)


#Ok jsonformat print(json.dumps(dict, indent = 4, sort_keys=True))

loaded_json = json.loads(r.text) # d og dict samem same

line= 0
index = 0
dager = 0 # antall dager man kjøre tilbake
value = 0
unit = 5 #percent




for x in loaded_json:
	#print(x)   
	

	if x["omrType"] =='EL':

		#print(x["fyllingsgrad"], x["dato_Id"] , x["omrType"] , x["omrnr"])
		line = line +1
		cettime = "'"+x["dato_Id"]+"'"


		value = x["fyllingsgrad"]
		MyLatitude = 'NULL'
		MyLongitude = 'NULL'
		MyAltitude = 0
        
		sdesc = 'Na'

		snametmp = 'NO'+  str(x["omrnr"])+ '.PCT'
        	#tbc lon lat og sdesc - bare om url uke..
		if snametmp == 'NO1.PCT':
			sdesc = f"'Fyllingsgrad Oslo'"
			MyLatitude = 59.91
			MyLongitude = 10.75
		elif snametmp == 'NO2.PCT':
			sdesc = f"'Fyllingsgrad Kristiansand'"
			MyLatitude = 58.14
			MyLongitude = 7.99
		elif snametmp == 'NO3.PCT':
			sdesc = f"'Fyllingsgrad Trondheim'"
			MyLatitude = 63.44
			MyLongitude = 10.42
		elif snametmp == 'NO4.PCT':
			sdesc = f"'Fyllingsgrad Tromsø'"
			MyLatitude = 	69.64
			MyLongitude = 18.95
		elif snametmp == 'NO5.PCT':
			sdesc = f"'Fyllingsgrad Bergen'"
			MyLatitude = 60.39
			MyLongitude = 5.32

        
		sname =  f"'{snametmp}'"
		value = x["fyllingsgrad"] *100
		StoredProc = f"exec {procname} {MySetName},{sname}, {sdesc},{unit},{cettime},{value}"

		#print(StoredProc)
				
		conn.execute (StoredProc)
            
	#comitting for every 1000, 1000, can be 1500 ..
	if line % 1000 == 0:            
		conn.commit()
		print( str(line) + ' Bulk committing rows.')  
     
#comitiing rest        
conn.commit()

print("Loaded: " ,line, "rows")


print(loadsetname)
#oppretter litt statistikk på statistikken not needed

cursor = conn.cursor()

#all history sjekk post acc
conn.execute('exec UTILS_UpdateCurveInfo '+loadsetname) 
conn.commit()
conn.close()
#banker inn i basen
						
print("The End")





