import csv
from datetime import datetime

MONTH_MAP = {
    'Jan':'01','Feb':'02','Mar':'03','Apr':'04','May':'05','Jun':'06',
    'Jul':'07','Aug':'08','Sep':'09','Oct':'10','Nov':'11','Dec':'12'
}

input_file  = r'C:\HarmonizeCode\Py\Nadabas.csv'
output_file = r'C:\HarmonizeCode\Py\HarmonizeNadabas.csv'

with open(input_file, encoding='utf-8', newline='') as f:
    reader = csv.reader(f)
    rows = list(reader)

# Row 0 = header: Code, Description, Jan-13, Feb-13, ...
header = rows[0]
date_cols = header[2:]  # list of 'Jan-13', 'Feb-13', ...

# Build date strings for each column
dates = []
for col in date_cols:
    col = col.strip()
    if not col:
        dates.append(None)
        continue
    parts = col.split('-')
    if len(parts) == 2:
        mon, yr = parts
        month_num = MONTH_MAP.get(mon[:3].capitalize())
        year = '20' + yr if int(yr) < 50 else '19' + yr
        dates.append(f"{month_num}/01/{year}")
    else:
        dates.append(None)

with open(output_file, 'w', newline='', encoding='utf-8') as out:
    writer = csv.writer(out)
    writer.writerow(['name', 'unit', 'date', 'value', 'desc', 'doc'])

    for row in rows[1:]:
        code = row[0].strip().strip('"') if row else ''
        if not code:
            continue  # skip section headers and empty rows
        desc = row[1].strip().strip('"') if len(row) > 1 else ''
        for i, val_str in enumerate(row[2:]):
            val_str = val_str.strip()
            if not val_str:
                continue
            if dates[i] is None:
                continue
            try:
                val = float(val_str)
            except ValueError:
                continue
            writer.writerow([code, 20, dates[i], val, desc, ''])

print(f"Done: {output_file}")
