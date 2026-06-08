print("starting ...")
#definerer navn på lagret proc i sqldatabase med noen params for å finne den i gjen.
rowcnt = 0
cnt=0
procnamedeci = 'UTILS_PutTime'
loadsetName = "Ghana"


#loadsetidtxt = '149'
#loadsetNametxt = "'Shadowtxt'"

from pyjstat import pyjstat
import requests
import pyodbc

import json
import urllib
import datetime

from datetime import datetime,timedelta
from pytz import timezone
import pytz
import time
import csv
      


#update user must have access                      
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost;'
                      'Database=dw;'
                      'UId=ExeUser;'
                      'PWD=This1sN0ThePW;')      
                                                                  
cursor = conn.cursor()


Mcode = 'None'

unit = '31'  #unit amt tbc
Value = '0' #Value Volum
UTCDate ='mdate'
header = ['nu1ard','2bmoil']


#with open('dump_02_12_v2.txt', encoding='utf-8', newline='') as f:
#with open('p2_mini.csv', encoding='utf-8', newline='') as f:
with open('ghana_eviews.csv', encoding='utf-8', newline='') as f:
	reader = csv.reader(f, delimiter=';')
	for row in reader:
		rowcnt=rowcnt+1
		cnt=0
		#ignorerer 1 foerste radene i filen heading
		if (rowcnt ==1 ):
			for cell in row:
				print(cell)
			header=row
			print(header[1],header[2])

		elif (rowcnt >1 ):
			#print('radnr',rowcnt)
	
			#utctimestr = row[0]
			cnt = 0
			for cell in row:
				#print(cell)
				#print('celle i rad',cnt)
				#cnt = cnt+1
				if cnt ==0:
					UTCDate="'"+cell+"'"
				else:
					Value = cell  

					#print(type(Value))

					#print(row[0])  #time
					#print(row[1])  #market code
					Mcode = (header[cnt])  #serienavn
					#UTCDate = "'"+(row[0])+"'"  #datoen


					CName = "'"+Mcode+"'"
					#Value = (row[2])  #Activated quant
					FValue = Value.replace(",", ".")   
					#print(type(FValue))
					#print(isinstance(FValue,str))
					unit = 31 # type

					#StoredProc = f"exec {procnamedeci} {loadsetName},{CName}, {CName},{unit},{UTCDate},{FValue}, NULL"
					#print(StoredProc)
					if FValue not in [ '#I/T', 'NA','na']:
						Value=round(float(FValue),2)
						StoredProc = f"exec {procnamedeci} {loadsetName},{CName}, {CName},{unit},{UTCDate},{Value}, NULL"
						print(StoredProc)
                        #Value = round(FValue,4)
						conn.execute(StoredProc)
				cnt=cnt+1
            

				if ((rowcnt % 1500) == 0):
					conn.commit()
					print ('committer '+ StoredProc)

					#dytter inn rad for rad i sql database med upsert (overskriver dersom raden er der fr a før

		
	print('stats..')
	#oppretter litt statistikk på statistikken not needed
	cursor = conn.cursor()
	#conn.execute('exec UTILS_UpdateCurveInfo '+loadsetName)

conn.commit()
conn.close()
print('end')





