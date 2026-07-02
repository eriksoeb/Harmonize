<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" type="text/css" href="../Harmonize.css">
    <style>
        .data-table {
            width: 100%;
            max-width: 1400px;
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
        .btn-chart { background-color: #00824d; color: white; }
        .btn-raw   { background-color: #888;    color: white; }
        .btn:hover { opacity: 0.85; }
        .published {
            white-space: nowrap;
            color: #555;
            font-size: 13px;
        }
        .info-box {
            max-width: 1400px;
            margin: 1rem auto 0 auto;
            padding: 10px 16px;
            background-color: #f0f8f4;
            border-left: 4px solid #00824d;
            font-size: 14px;
        }
    </style>
<?php
// Whitelist of allowed domains → display names
$domains = [
    'cpi'    => 'Consumer Price Index',
    'ppi'    => 'Production Price Index',
    'energy' => 'Energy',
];

$domain = isset($_GET['domain']) ? $_GET['domain'] : '';

if (!array_key_exists($domain, $domains)) {
    http_response_code(400);
    echo "<title>Error</title></head><body><p>Invalid domain. Use ?domain=cpi, ?domain=ppi or ?domain=energy</p></body></html>";
    exit;
}

$title = $domains[$domain];
echo "    <title>Harmonize – $title Series</title>\n";
?>
</head>
<body>

<div class="mobile-container">
    <div class="topnav">
        <a href="../index.html">Harmonize.no</a>
        <a href="index.html">Data</a>
        <a href="../templates/demo.html">Demo</a>
    </div>
</div>

<main>
    <h1><?php echo htmlspecialchars($title); ?> – Individual Series</h1>
    <p>Individual series available for download and charting.</p>

    <div class="info-box">
        To download data as <strong>CSV or XLS</strong>, open a Chart and use the menu in the upper right corner.
    </div>

    <table class="data-table">
        <thead>
            <tr>
                <th>Name</th>
                <th>Description</th>
                <th>Updated</th>
                <th>View / Download</th>
            </tr>
        </thead>
        <tbody>
<?php
$catalogFile = __DIR__ . '/' . $domain . '/catalog.json';
$seriesDir   = __DIR__ . '/' . $domain . '/series/';

if (!file_exists($catalogFile)) {
    echo "            <tr><td colspan=\"4\">No catalog found for " . htmlspecialchars($domain) . ".</td></tr>\n";
} else {
    $catalog = json_decode(file_get_contents($catalogFile), true);
    $found = 0;

    foreach ($catalog as $entry) {
        $filename = $entry['file'];
        $desc     = $entry['description'];
        $path     = $seriesDir . $filename;

        if (!file_exists($path)) continue;  // skip if not yet published

        $found++;
        $timestamp = date('Y-m-d H:i', filemtime($path));
        $jsonUrl   = $domain . '/series/' . $filename;
        $chartUrl  = '../templates/Chart.html?filename=../data/' . $domain . '/series/' . $filename;
        $name      = !empty($entry['name']) ? $entry['name'] : pathinfo($filename, PATHINFO_FILENAME);

        echo "            <tr>\n";
        echo "                <td>" . htmlspecialchars($name, ENT_QUOTES, 'UTF-8') . "</td>\n";
        echo "                <td>" . htmlspecialchars($desc, ENT_QUOTES, 'UTF-8') . "</td>\n";
        echo "                <td class=\"published\">" . $timestamp . "</td>\n";
        echo "                <td>\n";
        echo "                    <a class=\"btn btn-chart\" href=\"" . htmlspecialchars($chartUrl) . "\">Chart</a>\n";
        echo "                    <a class=\"btn btn-raw\"   href=\"" . htmlspecialchars($jsonUrl) . "\" target=\"_blank\">JSON</a>\n";
        echo "                </td>\n";
        echo "            </tr>\n";
    }

    if ($found === 0) {
        echo "            <tr><td colspan=\"4\">No series published yet.</td></tr>\n";
    }
}
?>
        </tbody>
    </table>
</main>

<footer>
    <p><strong>Timed and Tailored &copy; 2026</strong></p>
    <small>erik.soeberg&#64;ssb&#46;no</small>
</footer>

</body>
</html>
