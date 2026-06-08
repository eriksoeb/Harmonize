import datetime
import logging
import pyodbc
import requests
from pyjstat import pyjstat


loadsetId = 8
loadsetName = f"'Emissions-to-air'"

#Collecting data from the API of SSB with statistics on emissions to air:
apiURL = 'https://data.ssb.no/api/v0/en/table/08940/'

#Query sent to the URL above, asking for data on specified dimension values:
query = {
  "query": [
    {
      "code": "UtslpTilLuft",
      "selection": {
        "filter": "vs:UtslpKildeA01",
        "values": [
          "0",
          "1",
          "3"

        ]
      }
    },
    {
      "code": "UtslpEnergivare",
      "selection": {
        "filter": "item",
        "values": [
          "VT0"
        ]
      }
    },
    {
      "code": "UtslpKomp",
      "selection": {
        "filter": "item",
        "values": [
          "A10",
          "K12"

        ]
      }
    },
    {
      "code": "ContentsCode",
      "selection": {
        "filter": "item",
        "values": [
          "UtslippCO2ekvival"
        ]
      }
    },
    
 {   
"code": "Tid",
      "selection": {
        "filter": "item",
"values": [
        "2016",
        "2017",
        "2018"
        ]
      }
    },
    
    
  ],
  "response": {
    "format": "json-stat2"
  }
}

#Query request sent to SSB, store in a variable:
requestToSSB = requests.post(apiURL, json = query)

#Checking if response from SSB is okay (200 = okay, 400 = bad request, 404 = not found):
#print(requestToSSB)

#Converts json stat format to dataframe:
dataResults = pyjstat.Dataset.read(requestToSSB.text)

#Write to a dataframe: 
resultsDataFrame = dataResults.write('dataframe')

#Defining connection info to Azure db and how to define stored procedure: 

storedProcExec = 'exec UTILS_Put_GeoTime '
infoproc = 'UTILS_UpdateCurveInfo2 '
StoredProcMeteData = f"exec {infoproc} {loadsetId}"
print (StoredProcMeteData)

#Defining database connection

                     
                      
conn = pyodbc.connect('Driver={SQL Server};'
                      'SERVER=localhost;'
                      'Database=SBR_NOR;'
                      'UId=sa;'
                      'PWD=12qw!@QW;')
                      
                      
                      

cursor = conn.cursor()



#Defining parameters for stored procedure:
unitId = 12
geoPointId = 'Null'
mrId = 'Null'

for i, row in resultsDataFrame.iterrows():
  source_activity = row["source (activity)"]
  energy_product = row["energy product"]
  pollutant = row.pollutant
  contents = row.contents
  year = row.year

  value = row.value

  #Creating variables suitable for the table format in SQL db: 
  curveNameLong = f"'{source_activity}.{energy_product}.{pollutant}.{contents}'"
  #print(curveNameLong)
  #curveNameShort = f"{curveNameLong[:64]}'"                                       #To match criteria from db

  curveNameShort = f"'{source_activity}.{energy_product}.{pollutant}'"
  print(curveNameShort)
  #MyShortVar = str.replace(str.replace(str.replace(row.statistikkvariabel,' ',''),'(kr)',''),'(tonn)','')

  valDateTemp = str(datetime.datetime.strptime(year, "%Y"))
  valDate = f"'{valDateTemp}'"

  #Creating stored procedure. Format: StoredProcedure = loadsetid, loadsetname, curvename, description, unit, date, value, geopointid, mrid)
  StoredProc = f"{storedProcExec} {loadsetId}, {loadsetName}, {curveNameShort}, {curveNameLong}, {unitId}, {valDate}, {value}, {geoPointId}, {mrId}"

  #logging.info(StoredProc)

  #We don't insert NaN values from SSB into the database
  #if str(value) != 'nan':     
    #conn.execute(StoredProc)
    
print(i)
    





conn.execute(StoredProcMeteData)
conn.commit()

conn.close()