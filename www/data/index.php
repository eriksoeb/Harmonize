<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Harmonize – Data</title>
    <link rel="stylesheet" type="text/css" href="../Harmonize.css?v=14">
    <style>
        .data-table {
            width: 100%;
            max-width: 100%;
            margin: 2rem auto;
            border-collapse: collapse;
        }
        .data-table th {
            background-color: #00824d;
            color: white;
            padding: 10px 12px;
            text-align: left;
        }
        .data-table td {
            padding: 10px 12px;
            border: 1px solid #aaa;
            vertical-align: middle;
        }
        .data-table tr:hover td {
            background-color: #f0f8f4;
        }
        .btn {
            display: inline-block;
            padding: 6px 12px;
            margin: 2px 3px;
            border-radius: 4px;
            text-decoration: none;
            font-size: 13px;
            font-weight: bold;
            white-space: nowrap;
        }
        .btn-chart      { background-color: #004381; color: white; }
        .btn-multichart { background-color: #00824d; color: white; }
        .btn-report     { background-color: #8B1A1A; color: white; }
        .btn-csv        { background-color: #5a3e8a; color: white; }
        .btn-raw        { background-color: #888;    color: white; }
        .btn:hover      { opacity: 0.85; }
        .domain-header td {
            background-color: #e8f5f0;
            font-weight: bold;
            color: #00824d;
            padding: 8px 12px;
        }
        .published {
            white-space: nowrap;
            color: #555;
            font-size: 13px;
        }
        .info-box {
            max-width: 100%;
            margin: 1rem auto 0 auto;
            padding: 10px 16px;
            background-color: #f0f8f4;
            border-left: 4px solid #00824d;
            font-size: 14px;
        }
        main { max-width: 100%; padding: 1rem 12px; }
    </style>
</head>
<body>

<div class="mobile-container">
    <div class="topnav">
        <a href="../index.php" class="active">Harmonize.no</a>
        <a href="../templates/demo.php" class="active">Demo</a>
        <a href="web_explorer.php" class="active">Explorer</a>
    </div>
</div>

<main>
    <h1>Harmonized Databank</h1>
    <p>
        Time series data published by Harmonize. Each dataset can be viewed as an interactive chart,
        as a tabular report, or used directly as raw JSON.
    </p>

    <div class="info-box">
        To download data as <strong>XLS, JPG, PNG, PDF or SVG</strong>, open a Chart and use the menu in the upper right corner of the chart.
    </div>

    <table class="data-table">
        <thead>
            <tr>
                <th>Dataset</th>
                <th>Published</th>
                <th>View / Download</th>
            </tr>
        </thead>
        <tbody>

            <tr class="domain-header">
                <td colspan="3"><a href="series.php?domain=Prices_CPI_(2025)" style="color:#00824d;">Prices CPI (2025)</a></td>
            </tr>
            <tr>
                <td>Consumer Price Index – Total</td>
                <td class="published" data-json="Prices_CPI_(2025)/charts/cpi_total.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/Chart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_total.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_total.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Prices_CPI_(2025)/charts/cpi_total.json">Report</a>
                    <a class="btn btn-raw"        href="Prices_CPI_(2025)/charts/cpi_total.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Prices_CPI_(2025)/charts/cpi_total.json">&#8595; CSV</a>
                </td>
            </tr>
            <tr>
                <td>Consumer Price Index – Multiple series</td>
                <td class="published" data-json="Prices_CPI_(2025)/charts/cpi_series.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/Chart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_series.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_series.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Prices_CPI_(2025)/charts/cpi_series.json">Report</a>
                    <a class="btn btn-raw"        href="Prices_CPI_(2025)/charts/cpi_series.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Prices_CPI_(2025)/charts/cpi_series.json">&#8595; CSV</a>
                </td>
            </tr>
            <tr>
                <td>Consumer Price Index – Transport</td>
                <td class="published" data-json="Prices_CPI_(2025)/charts/cpi_transport.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/YearChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_transport.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_transport.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Prices_CPI_(2025)/charts/cpi_transport.json">Report</a>
                    <a class="btn btn-raw"        href="Prices_CPI_(2025)/charts/cpi_transport.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Prices_CPI_(2025)/charts/cpi_transport.json">&#8595; CSV</a>
                </td>
            </tr>

            <tr class="domain-header">
                <td colspan="3"><a href="series.php?domain=Prices_PPI_(2021)" style="color:#00824d;">Prices PPI (2021)</a></td>
            </tr>
            <tr>
                <td>Production Price Index Totals</td>
                <td class="published" data-json="Prices_PPI_(2021)/charts/ppi.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/Chart.html?filename=../data/Prices_PPI_(2021)/charts/ppi.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Prices_PPI_(2021)/charts/ppi.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Prices_PPI_(2021)/charts/ppi.json">Report</a>
                    <a class="btn btn-raw"        href="Prices_PPI_(2021)/charts/ppi.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Prices_PPI_(2021)/charts/ppi.json">&#8595; CSV</a>
                </td>
            </tr>

            <tr class="domain-header">
                <td colspan="3"><a href="series.php?domain=Energy_Reservoirs" style="color:#00824d;">Energy Reservoirs</a></td>
            </tr>
            <tr>
                <td>Waterlevels – Reservoir fill rate, year over year</td>
                <td class="published" data-json="Energy_Reservoirs/charts/waterlevel_year.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/YearChart.html?filename=../data/Energy_Reservoirs/charts/waterlevel_year.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Energy_Reservoirs/charts/waterlevel_year.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Energy_Reservoirs/charts/waterlevel_year.json">Report</a>
                    <a class="btn btn-raw"        href="Energy_Reservoirs/charts/waterlevel_year.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Energy_Reservoirs/charts/waterlevel_year.json">&#8595; CSV</a>
                </td>
            </tr>
			
			
			 <tr class="domain-header">
                <td colspan="3"><a href="series.php?domain=Satellites_FramSat-1" style="color:#00824d;">Satellites FramSat-1</a></td>
            </tr>
			 <tr>
                <td>Measures versus calculated</td>
                <td class="published" data-json="Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json">…</td>
                <td>
                    <a class="btn btn-chart"      href="../templates/Chart.html?filename=../data/Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json">Chart</a>
                    <a class="btn btn-multichart" href="../templates/MultiChart.html?filename=../data/Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json">MultiChart</a>
                    <a class="btn btn-report"     href="../templates/Report.html?filename=../data/Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json">Report</a>
                    <a class="btn btn-raw"        href="Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json" target="_blank">JSON</a>
                    <a class="btn btn-csv"        href="csvdownload.php?file=Satellites_FramSat-1/charts/azimuth_meas_vs_calc.json">&#8595; CSV</a>
                </td>
            </tr>
			
			
			
			
			
			
			

        </tbody>
    </table>
</main>

<?php include __DIR__ . '/../includes/footer.php'; ?>

<script>
document.querySelectorAll('td.published[data-json]').forEach(function(td) {
    fetch(td.dataset.json, { method: 'HEAD' })
        .then(function(r) {
            var lm = r.headers.get('Last-Modified');
            if (lm) {
                var d = new Date(lm);
                var yyyy = d.getFullYear();
                var mm   = String(d.getMonth() + 1).padStart(2, '0');
                var dd   = String(d.getDate()).padStart(2, '0');
                var hh   = String(d.getHours()).padStart(2, '0');
                var min  = String(d.getMinutes()).padStart(2, '0');
                td.textContent = yyyy + '-' + mm + '-' + dd + ' ' + hh + ':' + min;
            } else {
                td.textContent = '–';
            }
        })
        .catch(function() { td.textContent = '–'; });
});
</script>
</body>
</html>
