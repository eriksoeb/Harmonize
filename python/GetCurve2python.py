#Erik May 2025

import requests
import pyodbc
import urllib3
import getopt,sys
import pandas as pd
from datetime import datetime
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
#import matplotlib.dates
import json
#pip install matplotlib



#samples:
#StoredProc = "execute Harmonize..StpGetSeries 'CPI_NO:CTOTAL.IDX',2014,'Default',NULL,1,'json', 'NONE',1500, 'asc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'AllData',NULL,1,'json', 'NONE',1500, 'asc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'AllData',NULL,1,'json', 'NONE',1500, 'desc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2',NULL,1,'json', 'NONE',1500, 'desc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2',NULL,1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2',NULL,1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2','PCT(n)',1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"
#same as
#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2','[diff(n)]',1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"

#StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX',NULL,'YearsLast2','[diff(n)]',1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"

#baseyear and function
StoredProc = "execute StpGetSeries 'CPI_NO:CTOTAL.IDX','2022','YearsLast2','[diff(n)]',1,'json', 'AVG_QUARTER',1500, 'desc', 'NULL'"

    
                     
#sjd use ur built in ad login trusted connection                    
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost\\MSSQLSERVER2022;'
                      'Database=Harmonize;'
                      'Connection Timeout=5;'
                      'Integrated Security=True;')

                                 
                                                          
cursor = conn.cursor()

#print(StoredProc)
result = cursor.execute(StoredProc)

json_string = (cursor.fetchval())
conn.commit()
conn.close()
#print (json_string)

#OK
data = json.loads(json_string)
print(data)
MyCurveName =  (data[0]["Name"])

#print(data)
df = pd.json_normalize(data, max_level=0, record_path=['Obs'])

#print(df)

df.VDate = pd.to_datetime(df.VDate)
df.plot.line(x='VDate', y='Value',color='green', figsize=(18,7))

plt.xlabel('Time') 
plt.ylabel('Index') 
plt.title(MyCurveName)
plt.grid(True)

plt.ticklabel_format(style='plain', useOffset=False, axis='y')
plt.show(block=True);


