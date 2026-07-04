<?php
// Allowed domains (prevents directory traversal)
$allowed_domains = ['cpi', 'ppi', 'energy'];

$file = isset($_GET['file']) ? $_GET['file'] : '';

// Validate: must start with an allowed domain and contain no ..
$valid = false;
foreach ($allowed_domains as $d) {
    if (strpos($file, $d . '/') === 0) { $valid = true; break; }
}

if (!$valid || strpos($file, '..') !== false || !preg_match('/^[\w\-\/\.]+\.json$/', $file)) {
    http_response_code(400);
    echo "Invalid file parameter.";
    exit;
}

$path = __DIR__ . '/' . $file;
if (!file_exists($path)) {
    http_response_code(404);
    echo "File not found.";
    exit;
}

$json = json_decode(file_get_contents($path), true);
if (!$json || !isset($json[0]['series'])) {
    http_response_code(500);
    echo "Invalid JSON structure.";
    exit;
}

$doc      = $json[0];
$series   = $doc['series'];
$decimals = isset($doc['decimals']) ? (int)$doc['decimals'] : 2;
$freq     = isset($doc['frequency']) ? $doc['frequency'] : 'MONTHLY';
$useLong  = ($freq === 'HOURLY' || $freq === 'DAILY');

function formatTs($ts_ms, $useLong) {
    $ts = (int)($ts_ms / 1000);
    return $useLong ? gmdate('Y-m-d\TH:i', $ts) : gmdate('Y-m-d', $ts);
}

// Build CSV rows
$rows = [];

// Header row
$header = ['Date'];
foreach ($series as $s) {
    $name = isset($s['name']) ? $s['name'] : '';
    $desc = isset($s['description']) ? $s['description'] : '';
    $header[] = $name . ($desc ? ' - ' . $desc : '');
}
$rows[] = $header;

// Collect all timestamps (sorted, duplicates preserved for DST)
$maxCount = [];
foreach ($series as $s) {
    $counts = [];
    foreach ($s['data'] as $point) {
        $ts = $point[0];
        $counts[$ts] = ($counts[$ts] ?? 0) + 1;
    }
    foreach ($counts as $ts => $c) {
        $maxCount[$ts] = max($maxCount[$ts] ?? 0, $c);
    }
}
ksort($maxCount);

$allDates = [];
foreach ($maxCount as $ts => $count) {
    for ($i = 0; $i < $count; $i++) {
        $allDates[] = ['ts' => $ts, 'occ' => $i];
    }
}

// Data rows
foreach ($allDates as ['ts' => $ts, 'occ' => $occ]) {
    $row = [formatTs($ts, $useLong)];
    foreach ($series as $s) {
        $matches = array_values(array_filter($s['data'], fn($p) => $p[0] == $ts));
        $val = isset($matches[$occ][1]) && is_numeric($matches[$occ][1])
            ? number_format((float)$matches[$occ][1], $decimals, '.', '')
            : '';
        $row[] = $val;
    }
    $rows[] = $row;
}

// Output CSV
$basename = pathinfo($file, PATHINFO_FILENAME);
header('Content-Type: text/csv; charset=utf-8');
header('Content-Disposition: attachment; filename="' . $basename . '.csv"');
header('Cache-Control: no-cache');

$out = fopen('php://output', 'w');
foreach ($rows as $row) {
    fputcsv($out, $row);
}
fclose($out);
