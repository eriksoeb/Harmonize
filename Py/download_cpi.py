from pyjstat import pyjstat
import requests
import pandas as pd
import time

POST_URL = "https://data.ssb.no/api/v0/en/table/14700"

print("Downloading CPI data from SSB...")

unitId = 20
loadsetName = 'CPI'

payload = {
    "query": [
        {"code": "VareTjenesteGrp", "selection": {"filter": "all", "values": ["*"]}},
        {"code": "ContentsCode", "selection": {"filter": "item", "values": ["KpiIndMnd"]}},
        {"code": "Tid", "selection": {"filter": "top", "values": ["152"]}}
    ],
    "response": {"format": "json-stat"}
}

start_time = time.time()
resultat = requests.post(POST_URL, json=payload)
data = resultat.json()

# --- Build label to code mapping ---
labels = data['dataset']['dimension']['VareTjenesteGrp']['category']['label']
label_to_code = {v: k for k, v in labels.items()}

# --- Convert JSON-stat to DataFrame ---
dataset = pyjstat.Dataset.read(resultat.text)
df = dataset.write('dataframe')
df = df.rename(columns={'goods and services': 'consumption_group'})

# --- Add code column ---
df['konsum_code'] = df['consumption_group'].map(label_to_code)

# --- Convert month to date ---
df['naive'] = pd.to_datetime(df['month'], format="%YM%m")

# --- Build CSV rows in FFI format: name,unit,date,value,desc,doc ---
rows = []
for _, row in df.iterrows():
    val = row['value']
    if pd.isna(val):
        continue
    sname = ('C' + row['konsum_code'] + '.IDX').replace(' ', '').upper()
    date = row['naive'].strftime('%Y-%m-%d')
    desc = row['consumption_group']
    rows.append(f'{sname},{unitId},{date},{val},{desc},')

outfile = 'cpi.csv'
with open(outfile, 'w', encoding='utf-8') as f:
    f.write('name,unit,date,value,desc,doc\n')
    f.write('\n'.join(rows) + '\n')

elapsed = time.time() - start_time
print(f"Saved {len(rows)} rows to {outfile} in {elapsed:.1f}s")
