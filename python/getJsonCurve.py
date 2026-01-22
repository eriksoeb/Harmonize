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
    MyCurve = "'CPI_NO:CTOTAL.IDX'"
    #MyCurve = "'PPI_NO:SNN0yyy.IDX'"
    print("Using Curvename:", MyCurve)
else:
    MyCurve = "'" + sys.argv[1] + "'"
    print("Reading from:", sys.argv[1], "=>", MyCurve)




#settings
Interval = "'YearsLast4'"
BaseYear = "2024"
Func="NULL"
#Func="'PCT(n)'"
lag=1
Format = "'json'"
#Format = "'client'"
#Aggregate = "'NONE'"
Aggregate = "'AVG_YEAR'"
Sort="'asc'"


#dec2025
#cleaner
StoredProc = f"{storedProcName}{MyCurve},{BaseYear},{Interval},{Func},{lag},{Format},{Aggregate},{500},{Sort},{'NULL'}"


print(StoredProc)
                     
                      
                     
                    
                      
                      
#sjd use ur built in ad login trusted connection                    
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost\\MSSQLSERVER2022;'
                      'Database=Harmonize;'
                      'Connection Timeout=5;'
                      'Integrated Security=True;')

                      
                      
                                                                          
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

   