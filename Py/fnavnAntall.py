
from pyjstat import pyjstat
import requests
import pyodbc
import math
import json
import urllib


print("starting ...")

procnamedeci = 'UTILS_PutTime'

#loadsetid = '6' not needed
loadsetName = "'Fornavn'"
unit = 5 #pct
unit = 1 #antall


#update user must have access                      
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost;'
                      'Database=dw;'
                      'UId=ExeUser;'
                      'PWD=This1sN0ThePW;')
                                                                                    
cursor = conn.cursor()

 
#FNAVN
POST_URL = 'https://data.ssb.no/api/v0/no/table/10467'

# API spørring OK alle jenter
#payload = {"query": [{"code": "Fornavn", "selection": {"filter": "all", "values": ["1*" ] } }, {"code": "ContentsCode", "selection": {"filter": "item", "values": ["PersonerProsent"] } }, {"code": "Tid", "selection": {"filter": "top", "values": ["138"] } } ], "response": {"format": "json-stat"} }

# API spørring noen test jenter
#payload = {"query": [{"code": "Fornavn", "selection": {"filter": "all", "values": [ "1TRINE","1HANNE",  "1HILDE" ,"1Z3SE"] } }, {"code": "ContentsCode", "selection": {"filter": "item", "values": ["PersonerProsent"] } }, {"code": "Tid", "selection": {"filter": "top", "values": ["12"] } } ], "response": {"format": "json-stat"} }


payload = {"query": [{"code": "Fornavn", "selection": {"filter": "*", "values": [ "1KIM","2KIM","1JANNE","1ASTRID","2HANS","2ERIK", "1HILDE", "1CHRISTINA","2GEIR", "1MARIE","1TRINE","1ANETTE","1HELGA", "2JO", "2JON","1RAGNHILD", "1CAMILLA","2CHRISTIAN","2ARVID" ] } }, {"code": "ContentsCode", "selection": {"filter": "item", "values": ["Personer"] } }, {"code": "Tid", "selection": {"filter": "top", "values": ["5"] } } ], "response": {"format": "json-stat"} }



resultat = requests.post(POST_URL, json = payload)
#print(resultat.text)



#my_json = json.loads(resultat.text)



resultat = requests.post(POST_URL, json = payload)
#resultatkoden
#print(resultat.text)



my_json = json.loads(resultat.text)




dataset = pyjstat.Dataset.read(resultat.text) 
df = dataset.write('dataframe')



print(df)
print (my_json)


print('print start..: ')

print (str(my_json["dataset"]["label"]) )
print (str(my_json["dataset"]["updated"]) )


print (str(my_json["dataset"]["dimension"]["ContentsCode"]["category"]["unit"]))
#print (str(my_json["dataset"]["dimension"]["ContentsCode"]["category"]["unit"]["PersonerProsent"]["base"]))


print (str(my_json["dataset"]["dimension"]["Fornavn"]) )
print (str(my_json["dataset"]["dimension"]["Fornavn"]["label"]) )
print (str(my_json["dataset"]["dimension"]["Fornavn"]["category"] ))
print (str(my_json["dataset"]["dimension"]["Fornavn"]["category"] ["index"]))
#feiler print (str(my_json["dataset"]["dimension"]["Fornavn"]["category"] ["valueTexts"]))

#y = str(my_json["dataset"]["dimension"]["Fornavn"]["category"]["label"])

#print (str(my_json["dataset"]["dimension"]["Fornavn"]["category"]["label"]))

#key = '1HANNE'
#for x in (my_json["dataset"]["dimension"]["Fornavn"]["category"] ["label"])  :
	#print (str(y[key]))
#	print(str(x))  #1Hanne

#json_dict = (str(my_json["dataset"]["dimension"]["Fornavn"]["category"]["index"]))


#for key in my_json:  
#    print(key, ":", my_json[key])


print ( my_json["dataset"]["dimension"]["Fornavn"]["category"] ["label"])


mynames = json.loads(str(my_json["dataset"]["dimension"]["Fornavn"]["category"] ["label"]).replace("'",'"') )

print ('MyNames:')
print (mynames)

#mynamedict = json.loads(mynames)
#print (mynamedict)
#print(my_json["dataset"]["value"])
myyears = json.loads(str(my_json["dataset"]["dimension"]["Tid"]["category"]["label"]).replace("'",'"') )
print(myyears)


name=''
namedesc =''
cnt = 0
ycnt=0
for namecode in mynames:  
    #print(namecode, ":", mynames[namecode])
    name = "'"+namecode+".ANT"+"'"
    namedesc = "'"+mynames[namecode]+"'"
    cnt = cnt+1
    print(name)
    print(namedesc)
    for year in myyears:
 
        #print (str(myyears[year]))    
        #print (str(my_json["dataset"]["value"][ycnt]))  ##forste

        MyVal = str(my_json["dataset"]["value"][ycnt]).replace("None","NULL")
        
        
        StoredProc = f"exec {procnamedeci} {loadsetName},{name}, {namedesc},{unit},{year},{MyVal}, NULL"
        print(StoredProc)
        conn.execute(StoredProc)
        
        ycnt=ycnt+1   #tbc here bit tricky but correct, tbc



print (str(my_json["dataset"]["value"]))
print (str(my_json["dataset"]["value"][0]))  ##forste




conn.commit()		
print('stats..')


cursor = conn.cursor()
conn.execute('exec UTILS_UpdateCurveInfo '+loadsetName)
conn.commit()
print('end')





