<?php
// ----------------------------
// getseries.php  — static JSON API
// Usage: getseries.php?domain=cpi&name=c00.idx
// ----------------------------

// Build allowed domains dynamically from subdirectories
$allowed_domains = array_map('basename', glob(__DIR__ . '/*', GLOB_ONLYDIR));

$domain = isset($_GET['domain']) ? strtolower(trim($_GET['domain'])) : '';
$name   = isset($_GET['name'])   ? strtolower(trim($_GET['name']))   : '';

// Validate domain
if (!in_array($domain, $allowed_domains)) {
    http_response_code(400);
    header('Content-Type: application/json');
    echo json_encode(['error' => 'Invalid domain. Available: ' . implode(', ', $allowed_domains)]);
    exit;
}

// Validate name — only allow alphanumeric, dots, dashes, underscores
if (!preg_match('/^[a-z0-9._-]+$/', $name)) {
    http_response_code(400);
    header('Content-Type: application/json');
    echo json_encode(['error' => 'Invalid series name.']);
    exit;
}

// Ensure .json extension
$filename = preg_match('/\.json$/', $name) ? $name : $name . '.json';

$filepath = __DIR__ . '/' . $domain . '/series/' . $filename;

if (!file_exists($filepath)) {
    http_response_code(404);
    header('Content-Type: application/json');
    echo json_encode(['error' => 'Series not found: ' . $domain . '/' . $name]);
    exit;
}

// Serve the file
header('Content-Type: application/json');
header('Cache-Control: public, max-age=3600');
header('X-Series: ' . $domain . '/' . $filename);
readfile($filepath);
