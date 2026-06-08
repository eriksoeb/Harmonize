import requests
import csv
import time

start = "2015"
end = "2025"

# ---- choose country mode ----

USE_ALL_COUNTRIES = False

countries = ["NO", "DK", "UG"]   # used if USE_ALL_COUNTRIES = False


# ---- indicators ----

indicators = {
    "IQ.SPI.OVRL": "Statistical Performance Indicators (SPI) Overall score",
    "SPI.INDEX.PIL1": "Statistical Performance Indicators (SPI) Pillar 1",
    "SPI.INDEX.PIL2": "Statistical Performance Indicators (SPI) Pillar 2",
    "SPI.INDEX.PIL3": "Statistical Performance Indicators (SPI) Pillar 3",
    "SPI.INDEX.PIL4": "Statistical Performance Indicators (SPI) Pillar 4",
    "SPI.INDEX.PIL5": "Statistical Performance Indicators (SPI) Pillar 5",
    "NY.GDP.PCAP.KD.ZG": "GDP per capita growth (annual %)",
    "FP.CPI.TOTL": "Consumer price index (2010 = 100)",
    "FP.CPI.TOTL.ZG": "Inflation, consumer prices (annual %)"
}


# ---- optionally download all countries ----

if USE_ALL_COUNTRIES:

    countries = []

    url = "https://api.worldbank.org/v2/country?format=json&per_page=400"

    r = requests.get(url)
    data = r.json()

    for c in data[1]:
        if c["region"]["id"] != "NA":   # remove aggregates like "World"
            countries.append(c["id"])


# ---- output file ----

outfile = open("datafile.csv", "w", newline="", encoding="utf-8")
writer = csv.writer(outfile)


# ---- main download loop ----

for iso2 in countries:

    print("Country:", iso2)

    for ind, label in indicators.items():

        page = 1

        while True:

            url = f"https://api.worldbank.org/v2/countries/{iso2}/indicators/{ind}?date={start}:{end}&format=json&page={page}"

            try:
                r = requests.get(url, timeout=10)
                r.raise_for_status()
                data = r.json()
            except Exception as e:
                print("API error:", e)
                break

            if len(data) < 2:
                break

            for row in data[1]:

                if row["value"] is None:
                    continue

                year = row["date"]
                date = f"{year}-01-01"

                writer.writerow([
                    f"{iso2}.{ind}",
                    20,
                    date,
                    row["value"],
                    label
                ])

            if page >= data[0]["pages"]:
                break

            page += 1
            time.sleep(0.2)


outfile.close()

print("Finished. Data saved to datafile.csv")