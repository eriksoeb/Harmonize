#Erik  Sep 2023
#Imports descr TBCont

import requests
import pyodbc

import json
import urllib
import datetime
import sys
import traceback

from datetime import datetime,timedelta
from pytz import timezone
import pytz
import time
import math
import csv

def Myconvert(seconds): 	
	return time.strftime("%H:%M:%S", time.gmtime(seconds))


#update user must have access                      
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost;'
                      'Database=dw;'
                      'UId=ExeUser;'
                      'PWD=This1sN0ThePW;')

                                  
cursor = conn.cursor()

st = time.time()

rowcnt = 0
Sql = "StpPutLineDescr "
InFile='eviews_descr.txt'

with open(InFile, encoding='utf-8', newline='') as f:
	reader = csv.reader(f, delimiter=';')
	for row in reader:
		rowcnt=rowcnt+1
		

		print (row[0])

		#print (InsertSqlHeader + MyValuesStr) 
		try:
			#print('calling')
			conn.execute(Sql+"'"+row[0]+"'")
	
		except Exception as e: 
				
				print ("Problems processing file:", InFile, "Rownumber :",  rowcnt)
				print (Sql + row[0]) 
				print(e)
				et = time.time()
				# calculate the execution time
				elapsed_time = et - st
	
				print ("Forced Exit :",Myconvert(elapsed_time))
				exit(0)
			
			


print ('Final committe at row no',rowcnt)	
conn.commit()
conn.close()
print('End' , rowcnt-1, ' data rows processed, exl header')

time.sleep(0) #just test if more than 0 TBR
et = time.time()
elapsed_time = et - st
print(elapsed_time)
print("Successful Execution time:", Myconvert(elapsed_time))