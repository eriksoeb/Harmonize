#Erik Oct 2025
#gets a curve with data from database
import requests
import pyodbc
import urllib3
import getopt,sys
import pandas as pd
from datetime import datetime
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
import json



storedProcName = 'execute Harmonize..StpGetSeries '



#MyCurve = "'CPI_NO:CTOTAL.IDX'"  // to be passed as a argument


if len(sys.argv) != 2:
    print("Need Curvename as parameter – using default.")
    MyCurve = "'CPI:C00.IDX'"
    #MyCurve = "'PPI_NO:SNN0yyy.IDX'"
    print("Using Curvename:", MyCurve)
else:
    MyCurve = "'" + sys.argv[1] + "'"
    print("Reading from:", sys.argv[1], "=>", MyCurve)




#settings
Interval = "'YearsLast4'"
BaseYear = "2024"
Func="NULL" #Func="'PCT(n)'"
lagn=1 #n above
Format = "'json'"
#Format = "'client'"
#Aggregate = "'NONE'"
Aggregate = "'AVG_YEAR'"
Convert = "'Q'"  #converting to Q, ANN, MONthly freq
Sort="'asc'"


#dec2025
#cleaner
StoredProc = f"{storedProcName}{MyCurve},{BaseYear},{Interval},{Func},{lagn},{Format},{Aggregate},{Convert},{Sort}, {500},{'NULL'}"


print(StoredProc)
                     
      

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
                     
                    
  







#print(StoredProc)
result = cursor.execute(StoredProc)


if Format == "'client'":
    #print(StoredProc)
    rows = result.fetchall()
    for row in rows:
        print(row)
else:
    json_string = cursor.fetchval()
    print(json_string)

conn.commit()
conn.close()



#df = pd.DataFrame(json.loads(json_string))
#print (df)
#df.plot.line(x='VDate', y='Value')
#plt.ticklabel_format(style='plain', useOffset=False, axis='y')
#plt.show(block=True);

   