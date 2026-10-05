<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" type="text/css" href="../Harmonize.css?v=14">
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
            cursor: pointer;
            user-select: none;
        }
        .data-table th:hover { opacity: 0.85; }
        .data-table th .sort-arrow { margin-left: 4px; font-size: 11px; }
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
        .btn-chart  { background-color: #004381; color: white; }
        .btn-report { background-color: #8B1A1A; color: white; }
        .btn-csv    { background-color: #5a3e8a; color: white; }
        .btn-raw    { background-color: #888;    color: white; }
        .btn:hover  { opacity: 0.85; }
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
// Build domain list dynamically from subdirectories
$domains = [];
foreach (glob(__DIR__ . '/*', GLOB_ONLYDIR) as $dir) {
    $folder = basename($dir);
    $label  = ucwords(str_replace('_', ' ', $folder));
    $domains[$folder] = $label;
}
ksort($domains);

$domain_input = isset($_GET['domain']) ? $_GET['domain'] : '';

// Match domain case-insensitively, use the actual folder name
$domain = '';
foreach ($domains as $folder => $label) {
    if (strcasecmp($folder, $domain_input) === 0) {
        $domain = $folder;
        break;
    }
}

if ($domain === '') {
    $valid = implode(', ', array_keys($domains));
    http_response_code(400);
    echo "<title>Error</title></head><body><p>Invalid domain. Available: ?domain=" . htmlspecialchars($valid) . "</p></body></html>";
    exit;
}

$title = $domains[$domain];
echo "    <title>Harmonize – $title Series</title>\n";
?>
</head>
<body>

<div class="mobile-container">
    <div class="topnav">
        <a href="../index.php">Harmonize.no</a>
        <a href="index.php">DataBank</a>
        <a href="web_explorer.php">Explorer</a>
        <a href="../templates/demo.php">Demo</a>
    </div>
</div>

<main>
    <h1><?php echo htmlspecialchars($title); ?> – Individual Series</h1>
    <p>Individual series available for download and charting.</p>

    <div class="info-box">
        To download data as <strong>CSV or XLS</strong>, open a Chart and use the menu in the upper right corner.
        Click column headers to sort the table.
    </div>

    <table class="data-table">
        <thead>
            <tr>
                <th onclick="sortTable(0)">Name <span class="sort-arrow"></span></th>
                <th onclick="sortTable(1)">Description <span class="sort-arrow"></span></th>
                <th onclick="sortTable(2)">Updated <span class="sort-arrow"></span></th>
                <th>View / Download</th>
            </tr>
        </thead>
        <tbody>
<?php
$seriesDir   = __DIR__ . '/' . $domain . '/series/';
$catalogFile = $seriesDir . 'catalog.json';

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
        $jsonUrl   = 'getseries.php?domain=' . $domain . '&name=' . pathinfo($filename, PATHINFO_FILENAME);
        $chartUrl  = '../templates/Chart.html?filename=../data/' . $domain . '/series/' . $filename;
        $reportUrl  = '../templates/Report.html?filename=../data/' . $domain . '/series/' . $filename;
        $csvUrl     = 'csvdownload.php?file=' . $domain . '/series/' . $filename;
        $name       = !empty($entry['name']) ? $entry['name'] : pathinfo($filename, PATHINFO_FILENAME);

        echo "            <tr>\n";
        echo "                <td>" . htmlspecialchars($name, ENT_QUOTES, 'UTF-8') . "</td>\n";
        echo "                <td>" . htmlspecialchars($desc, ENT_QUOTES, 'UTF-8') . "</td>\n";
        echo "                <td class=\"published\">" . $timestamp . "</td>\n";
        echo "                <td>\n";
        echo "                    <a class=\"btn btn-chart\"  href=\"" . htmlspecialchars($chartUrl)  . "\">Chart</a>\n";
        echo "                    <a class=\"btn btn-report\" href=\"" . htmlspecialchars($reportUrl) . "\">Report</a>\n";
        echo "                    <a class=\"btn btn-raw\"    href=\"" . htmlspecialchars($jsonUrl)   . "\" target=\"_blank\">JSON</a>\n";
        echo "                    <a class=\"btn btn-csv\"    href=\"" . htmlspecialchars($csvUrl)    . "\">&#8595; CSV</a>\n";
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

<script>
let sortDir = {};
function sortTable(col) {
    const table = document.querySelector('.data-table');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    const dir = sortDir[col] === 'asc' ? 'desc' : 'asc';
    sortDir = {};
    sortDir[col] = dir;

    rows.sort((a, b) => {
        const aText = (a.cells[col] || {}).textContent || '';
        const bText = (b.cells[col] || {}).textContent || '';
        return dir === 'asc' ? aText.localeCompare(bText) : bText.localeCompare(aText);
    });

    rows.forEach(r => tbody.appendChild(r));

    // Update arrows
    table.querySelectorAll('th .sort-arrow').forEach(s => s.textContent = '');
    const arrow = table.querySelectorAll('th')[col].querySelector('.sort-arrow');
    if (arrow) arrow.textContent = dir === 'asc' ? '\u25B2' : '\u25BC';
}
</script>

<?php include __DIR__ . '/../includes/footer.php'; ?>

</body>
</html>
