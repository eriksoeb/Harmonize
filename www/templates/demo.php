<!DOCTYPE html>
<html lang="en">
<head>
<link rel="stylesheet" type="text/css" href="../Harmonize.css?v=14">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
	<link rel="icon" href="./../images/no.png" type="image/ico"/>
	
    <title>Demo</title>
</head>
<body>

    <header>
        <h1>Charts on web</h1>
        <p>A scalable and generic time-series platform consisting of an MSSQL database model, Windows application for analyzing, and API, supporting multiple statistical domains organized by time</p>
		<p>Charts created by Harmonize and copied (published) to web</p>
    </header>

    <nav>
	<a href="../index.php">Home</a>
	<a href="../data/index.php">DataBank</a>
	<a href="../data/web_explorer.php">Explorer</a>
    <a href="../install/install.php">Downloads</a>

		
    </nav>

<main>
    <h2>It's all about Time !</h2>

    <p><strong>Harmonize</strong> – Here you can see some charts samples created by Harmonize. This to illustrate some of the flexibility and how charts look like when copy to the web.
	 A sample article using Harmonize data and a Harmonize template looks like this – it can however be modified and customized: <a href="article.html">Article about consumer prices</a></p>

    <p>The 3 main templates can be reused for many charts, and you may want to customize your own additional templates or modify existing templates:</p>
	   <p>All charts are made by the use of <strong>www.Highcharts.com</strong></p>

    <ol class="demo-list">
        <li>
            <strong>Chart</strong> :
            <a href="Chart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_total.json">Consumer Price Index Totals</a>
        </li>

        <li>
            <strong>Chart</strong> :
            <a href="Chart.html?filename=../data/Prices_PPI_(2021)/charts/ppi.json">CPI & PPI with common base year 2021 - aggregated to quarter and year</a>
        </li>

        <li>
            <strong>Multichart</strong> :
            <a href="MultiChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_series.json">CPI Totals and some more indexes to scroll</a>
			  </li>

        <li>
			  <strong>Multichart</strong> :
            <a href="MultiChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_total.json">CPI Total & monthly and yearly percentage changes</a>
        </li>

        <li>
            <strong>YearChart</strong> :
            <a href="YearChart.html?filename=../data/Energy_Reservoirs/charts/waterlevel_year.json">Waterlevels in percentage, year over year</a>
		</li>

        <li>

			 <strong>YearChart</strong> :
            <a href="YearChart.html?filename=../data/Prices_CPI_(2025)/charts/cpi_transport.json">CPI Transport index, year over year</a>
        </li>

<!-- 
        <li>
            <strong>Report</strong> –
            <a href="Report.html?filename=CPI_chart.json">CPI Total & Changes</a> |
            <a href="Report.html?filename=cpi_series.json">CPI Series</a>
        </li>
-->		
		
    </ol>
	

	
	
	
</main>






    <?php include __DIR__ . '/../includes/footer.php'; ?>

</body>
</html>
