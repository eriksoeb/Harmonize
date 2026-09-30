<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0">
<meta name="color-scheme" content="light">
<title>Harmonize – Explorer</title>
<link rel="icon" href="../images/no.png" type="image/png">
<link rel="stylesheet" type="text/css" href="../Harmonize.css?v=14">

<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
<script src="https://code.highcharts.com/stock/highstock.js"></script>
<script src="https://code.highcharts.com/modules/exporting.js"></script>
<script src="https://code.highcharts.com/modules/export-data.js"></script>
<script src="https://code.highcharts.com/modules/data.js"></script>

<style>
    .explorer-wrap {
        margin: 0 12px;
        padding: 0;
    }
    .picker-row {
        display: flex;
        gap: 16px;
        flex-wrap: wrap;
        margin: 10px 0;
        align-items: center;
    }
    .picker-col { flex: 1; min-width: 260px; }
    .picker-col label {
        display: block;
        font-weight: bold;
        margin-bottom: 4px;
        font-size: 14px;
    }
    .picker-col select, .picker-col input {
        width: 100%;
        padding: 7px 8px;
        font-size: 16px;
        border: 1px solid #aaa;
        border-radius: 4px;
        box-sizing: border-box;
    }
    .slot {
        background: #f0f8f4;
        border: 1px solid #ccc;
        border-radius: 6px;
        padding: 10px 14px;
        margin-bottom: 8px;
        display: flex;
        align-items: center;
        gap: 10px;
        font-size: 14px;
    }
    .slot .tag {
        color: #fff;
        padding: 2px 8px;
        border-radius: 3px;
        font-size: 12px;
        font-weight: bold;
        white-space: nowrap;
    }
    .slot .remove-btn {
        margin-left: auto;
        background: none;
        border: none;
        color: #c00;
        font-size: 18px;
        cursor: pointer;
        padding: 0 4px;
    }
    .slots-row {
        display: flex;
        gap: 12px;
        flex-wrap: wrap;
        margin-top: 10px;
    }
    .slots-row .slot { flex: 1; min-width: 250px; margin-bottom: 0; }
    .btn-compare {
        display: inline-block;
        padding: 7px 22px;
        background: #00824d;
        color: #fff;
        border: none;
        border-radius: 4px;
        font-size: 15px;
        font-weight: bold;
        cursor: pointer;
        margin: 0;
    }
    .btn-compare:hover { opacity: 0.85; }
    .btn-compare:disabled { opacity: 0.4; cursor: default; }
    @media (max-width: 600px) {
        .btn-compare { padding: 12px; font-size: 16px; }
        .slot { padding: 6px 10px; font-size: 12px; }
        .slot .tag { font-size: 11px; padding: 1px 6px; }
        .series-list { max-height: 150px; }
        .picker-col select, .picker-col input { font-size: 16px; padding: 6px; }
        .slots-row { gap: 6px; margin-top: 6px; }
        .slots-row .slot { min-width: 100%; }
        #explorer-chart { min-height: 300px !important; max-height: 400px; }
    }
    #explorer-chart { margin-top: 6px; width: 100%; }
    .slot-empty { color: #999; font-style: italic; }
    .series-list {
        max-height: 220px;
        overflow-y: auto;
        border: 1px solid #ccc;
        border-radius: 4px;
        background: #fff;
        margin-top: 2px;
    }
    .series-list-item {
        padding: 6px 10px;
        cursor: pointer;
        font-size: 13px;
        border-bottom: 1px solid #eee;
    }
    .series-list-item:hover { background: #e6f4ed; }
    .series-list-item .item-domain {
        color: #888;
        font-size: 11px;
        margin-left: 4px;
    }
    .series-list-item .item-desc {
        color: #555;
        font-size: 12px;
        display: block;
        margin-top: 1px;
    }
    .hidden { display: none; }
    html, body { height: 100%; margin: 0; }
    body { display: flex; flex-direction: column; min-height: 100vh; }
    #explorer-chart { min-height: 640px; }
</style>
</head>
<body>

<div class="mobile-container">
    <div class="topnav">
        <a href="../index.php">Harmonize.no</a>
        <a href="index.php">DataBank</a>
        <a href="../templates/demo.php">Demo</a>
    </div>
</div>

<div class="explorer-wrap">
    <h2>Explorer – Find and Compare</h2>

    <!-- Domain filter + search + Chart button -->
    <div class="picker-row">
        <div class="picker-col">
            <select id="domainFilter">
                <option value="">All domains</option>
            </select>
        </div>
        <div class="picker-col">
            <input type="text" id="searchBox" placeholder="Type series #1 to search for" autocomplete="off">
        </div>
    </div>

    <!-- Search results -->
    <div id="seriesList" class="series-list hidden"></div>

    <!-- Selected slots -->
    <div class="slots-row">
        <div id="slot0" class="slot">
            <span class="tag" style="background:#628DCB">1</span>
            <span class="slot-label slot-empty">No series selected</span>
            <button class="remove-btn hidden" title="Remove">&times;</button>
        </div>
        <div id="slot1" class="slot">
            <span class="tag" style="background:#C7B491">2</span>
            <span class="slot-label slot-empty">No series selected</span>
            <button class="remove-btn hidden" title="Remove">&times;</button>
        </div>
    </div>
    <div style="margin:6px 0;">
        <button id="btnCompare" class="btn-compare" style="width:100%; padding:7px;" disabled>Chart</button>
    </div>
</div>

<!-- Chart -->
<div id="explorer-chart"></div>
<div id="chart-series-names" style="text-align:center; margin-top:10px;"></div>

<?php
// Build catalog data server-side, output as JSON for JS
$catalog = [];
foreach (glob(__DIR__ . '/*', GLOB_ONLYDIR) as $dir) {
    $folder = basename($dir);
    $catFile = $dir . '/series/catalog.json';
    if (!file_exists($catFile)) continue;

    $label = ucwords(str_replace('_', ' ', $folder));
    $entries = json_decode(file_get_contents($catFile), true);
    if (!is_array($entries)) continue;

    foreach ($entries as $e) {
        $catalog[] = [
            'domain'      => $folder,
            'domainLabel' => $label,
            'file'        => $e['file'],
            'name'        => isset($e['name']) ? $e['name'] : pathinfo($e['file'], PATHINFO_FILENAME),
            'description' => isset($e['description']) ? $e['description'] : '',
        ];
    }
}
// Sort by domain label then name (so "Prices:" groups together)
usort($catalog, function($a, $b) {
    $c = strcmp($a['domainLabel'], $b['domainLabel']);
    return $c !== 0 ? $c : strcmp($a['name'], $b['name']);
});
?>

<script>
// ── Catalog injected from PHP ──
const CATALOG = <?php echo json_encode($catalog, JSON_UNESCAPED_UNICODE); ?>;

// ── Unique domains for the filter dropdown ──
const domains = [...new Set(CATALOG.map(c => c.domain))].sort((a, b) => {
    const la = CATALOG.find(c => c.domain === a)?.domainLabel || a;
    const lb = CATALOG.find(c => c.domain === b)?.domainLabel || b;
    return la.localeCompare(lb);
});
const domainLabels = {};
CATALOG.forEach(c => { domainLabels[c.domain] = c.domainLabel; });

const domainSel  = document.getElementById('domainFilter');
const searchBox  = document.getElementById('searchBox');
const listEl     = document.getElementById('seriesList');
const btnCompare = document.getElementById('btnCompare');

// Populate domain dropdown
domains.forEach(d => {
    const o = document.createElement('option');
    o.value = d;
    o.textContent = domainLabels[d] || d;
    domainSel.appendChild(o);
});

// ── Selection state (max 2 slots) ──
const slots = [null, null]; // { domain, file, name, description, domainLabel }

function renderSlots() {
    for (let i = 0; i < 2; i++) {
        const el   = document.getElementById('slot' + i);
        const lbl  = el.querySelector('.slot-label');
        const btn  = el.querySelector('.remove-btn');
        if (slots[i]) {
            lbl.textContent = slots[i].name + ' — ' + slots[i].description;
            lbl.classList.remove('slot-empty');
            btn.classList.remove('hidden');
        } else {
            lbl.textContent = 'No series selected';
            lbl.classList.add('slot-empty');
            btn.classList.add('hidden');
        }
    }
    btnCompare.disabled = !slots[0]; // need at least 1
    searchBox.placeholder = !slots[0] ? 'Type series #1 to search for' : !slots[1] ? 'Type series #2 to search for' : 'Type to replace series #2';
}

// Remove button handlers
document.querySelectorAll('.remove-btn').forEach((btn, i) => {
    btn.addEventListener('click', () => {
        slots[i] = null;
        // Collapse: if slot 0 empty but slot 1 filled, move up
        if (!slots[0] && slots[1]) { slots[0] = slots[1]; slots[1] = null; }
        renderSlots();
    });
});

// ── Search / list ──
function filterCatalog() {
    const q = searchBox.value.trim().toLowerCase();
    const d = domainSel.value;
    if (!q && !d) return [];
    return CATALOG.filter(c => {
        if (d && c.domain !== d) return false;
        if (q) {
            const hay = (c.name + ' ' + c.description).toLowerCase();
            return hay.includes(q);
        }
        return true;
    }).slice(0, 50); // cap visible results
}

function renderList() {
    const items = filterCatalog();
    if (items.length === 0 && !searchBox.value.trim() && !domainSel.value) {
        listEl.classList.add('hidden');
        return;
    }
    listEl.innerHTML = '';
    if (items.length === 0) {
        listEl.innerHTML = '<div class="series-list-item" style="color:#999">No matches</div>';
    } else {
        items.forEach(c => {
            const div = document.createElement('div');
            div.className = 'series-list-item';
            div.innerHTML = '<strong>' + escHtml(c.name) + '</strong>' +
                            '<span class="item-domain">' + escHtml(c.domainLabel) + '</span>' +
                            '<span class="item-desc">' + escHtml(c.description) + '</span>';
            div.addEventListener('click', () => addSeries(c));
            listEl.appendChild(div);
        });
    }
    listEl.classList.remove('hidden');
}

function escHtml(s) {
    return s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

searchBox.addEventListener('input', renderList);
domainSel.addEventListener('change', renderList);

// Close list on outside click
document.addEventListener('click', (e) => {
    if (!listEl.contains(e.target) && e.target !== searchBox && e.target !== domainSel) {
        listEl.classList.add('hidden');
    }
});
searchBox.addEventListener('focus', renderList);

// ── Add series to a slot ──
function addSeries(c) {
    // Already selected?
    if (slots.some(s => s && s.domain === c.domain && s.file === c.file)) return;

    if (!slots[0])      slots[0] = c;
    else if (!slots[1]) slots[1] = c;
    else                slots[1] = c; // replace second

    listEl.classList.add('hidden');
    searchBox.value = '';
    renderSlots();
}

// ── Compare button ──
btnCompare.addEventListener('click', loadAndChart);

function buildShareUrl() {
    const params = new URLSearchParams();
    slots.filter(Boolean).forEach(s => {
        params.append('s', s.domain + '/' + s.file.replace(/\.json$/, ''));
    });
    return location.pathname + '?' + params.toString();
}

async function loadAndChart() {
    const selected = slots.filter(Boolean);
    if (selected.length === 0) return;

    btnCompare.disabled = true;
    btnCompare.textContent = 'Loading...';

    try {
        const allSeries = [];
        let decimals = 1;
        let frequency = 'MONTHLY';

        for (const s of selected) {
            const url = s.domain + '/series/' + s.file;
            const resp = await fetch(url);
            if (!resp.ok) throw new Error('Failed to load ' + url);
            const json = await resp.json();
            const doc = Array.isArray(json) ? json[0] : json;

            decimals  = doc.decimals  ?? decimals;
            frequency = doc.frequency ?? frequency;

            doc.series.forEach(ser => {
                allSeries.push({
                    ...ser,
                    ...(ser.style || {}),
                    dashStyle: ser.style?.linestyle || 'solid',
                    data: [...ser.data].sort((a, b) => a[0] - b[0])
                });
            });
        }

        renderChart(allSeries, decimals, frequency);

        // Update URL for sharing
        history.replaceState(null, '', buildShareUrl());

    } catch (err) {
        alert(err.message);
    } finally {
        btnCompare.disabled = false;
        btnCompare.textContent = 'Chart';
    }
}

// ── Highcharts rendering ──
function renderChart(chartSeries, DECIMALS, FREQ) {

    let dateFmt;
    switch (FREQ) {
        case 'MONTHLY':    dateFmt = '%Y-%m-%d'; break;
        case 'QUARTERLY':  dateFmt = '%Y-%m-%d'; break;
        case 'DAILY':      dateFmt = '%Y-%m-%d'; break;
        case 'HOURLY':     dateFmt = '%Y-%m-%dT%H:%M'; break;
        case 'YEARLY':     dateFmt = '%Y'; break;
        default:           dateFmt = '%B %Y';
    }

    // Fixed colors for series #1 and #2 to ensure contrast
    const explorerColors = ['#628DCB', '#C7B491'];
    chartSeries.forEach((s, i) => {
        s.color = explorerColors[i % explorerColors.length];
    });

    // Detect distinct units — use dual y-axis when they differ
    const units = [...new Set(chartSeries.map(s => (s.unit || '').toUpperCase()))];
    const dualAxis = units.length > 1;

    if (dualAxis) {
        // Assign yAxis index: first unit → 0 (left), second unit → 1 (right)
        chartSeries.forEach(s => {
            const u = (s.unit || '').toUpperCase();
            s.yAxis = (u === units[0]) ? 0 : 1;
        });
    } else {
        chartSeries.forEach(s => { s.yAxis = 0; });
    }

    const yAxes = dualAxis
        ? [
            { gridLineWidth: 0, alternateGridColor: 'rgba(0,0,0,0.05)', opposite: false,
              title: { text: units[0] } },
            { gridLineWidth: 0, opposite: true,
              title: { text: units[1] } }
          ]
        : [
            { gridLineWidth: 0, alternateGridColor: 'rgba(0,0,0,0.05)', opposite: false }
          ];

    let chart = Highcharts.stockChart('explorer-chart', {
        chart: { spacingTop: 10, zoomType: 'x', animation: false, backgroundColor: '#ffffff' },
        navigator: { enabled: false },
        scrollbar: { enabled: false },
        rangeSelector: { enabled: true },

        exporting: {
            showTable: false,
            tableCaption: '',
            filename: 'DataExplorer',
            csv: { dateFormat: dateFmt },
            buttons: {
                contextButton: {
                    menuItems: [
                        'separator', 'viewFullscreen', 'separator',
                        'downloadCSV', 'downloadXLS', 'separator',
                        'downloadPNG', 'downloadJPEG', 'downloadPDF', 'downloadSVG',
                        'separator',
                        {
                            textKey: 'viewData',
                            onclick: function () {
                                this.toggleDataTable();
                                var namesDiv = document.getElementById('chart-series-names');
                                var tableDiv = document.querySelector('.highcharts-data-table');
                                if (tableDiv && namesDiv && tableDiv.parentNode) {
                                    tableDiv.parentNode.insertBefore(namesDiv, tableDiv);
                                }
                            }
                        },
                        'separator', 'printChart', 'separator'
                    ]
                }
            }
        },

        credits: { enabled: true, href: null },

        tooltip: {
            distance: 30,
            padding: 5,
            split: false,
            shared: true,
            formatter: function () {
                let s = '<b>' + Highcharts.dateFormat(dateFmt, this.x) + '</b><br/>';
                this.points.forEach(point => {
                    s += '<span style="color:' + point.series.color + '">&#9679;</span> ' +
                         (point.series.options.style?.function || '') + ' ' +
                         point.series.name + ': <b>' +
                         Highcharts.numberFormat(point.y, DECIMALS, '.', ' ') +
                         (point.series.tooltipOptions.valueSuffix || '') +
                         '</b><br/>';
                });
                return s;
            }
        },

        xAxis: {
            type: 'datetime',
            scrollbar: { enabled: true },
            alternateGridColor: 'rgba(0,0,0,0.05)'
        },

        yAxis: yAxes,

        plotOptions: {
            series: {
                marker: { enabled: false },
                states: { inactive: { opacity: 1 } },
                groupPadding: 0
            }
        },

        series: chartSeries
    });

    // Series name labels below chart
    let namesHtml = chartSeries.map((s, i) => {
        let color = chart.series[i].color;
        return `<span class="series-name" data-index="${i}"
                     style="margin-right:15px; cursor:pointer; display:inline-flex; align-items:center;">
                    <span style="display:inline-block;width:12px;height:12px;background-color:${color};margin-right:5px;"></span>
                    ${s.style?.function || ''} ${s.aggregation || ''} ${s.name} ${s.description || ''}
                </span>`;
    }).join('');
    $('#chart-series-names').html(namesHtml);

    // Toggle visibility
    $('#chart-series-names').off('click').on('click', '.series-name', function() {
        let index = $(this).data('index');
        let series = chart.series[index];
        if (series.visible) {
            series.hide();
            $(this).addClass('series-hidden');
        } else {
            series.show();
            $(this).removeClass('series-hidden');
        }
    });

    // Round exported data
    Highcharts.addEvent(chart, 'exportData', function (e) {
        const rows = e.dataRows || e.data || [];
        for (let r = 1; r < rows.length; r++) {
            for (let c = 1; c < rows[r].length; c++) {
                const num = parseFloat(rows[r][c]);
                if (!isNaN(num)) {
                    rows[r][c] = Highcharts.numberFormat(num, DECIMALS, '.', '');
                }
            }
        }
    });
}

// ── Load from URL params on page load ──
(function loadFromUrl() {
    const params = new URLSearchParams(location.search);
    const sList = params.getAll('s'); // e.g. ["cpi/c00.idx", "ppi/snn0.idx"]
    if (sList.length === 0) return;

    let loaded = 0;
    sList.slice(0, 2).forEach((raw, i) => {
        const parts = raw.split('/');
        if (parts.length < 2) return;
        const domain = parts[0];
        const fileBase = parts.slice(1).join('/');
        const file = fileBase.endsWith('.json') ? fileBase : fileBase + '.json';

        const match = CATALOG.find(c => c.domain === domain && c.file === file);
        if (match) {
            slots[i] = match;
            loaded++;
        }
    });

    if (loaded > 0) {
        renderSlots();
        loadAndChart();
    }
})();

// Mobile fix: bridge touchend → click for Highcharts menu items
document.addEventListener('touchend', function(e) {
    var target = e.target;
    while (target && target !== document.body) {
        if (target.className && typeof target.className === 'string' &&
                target.className.indexOf('highcharts-menu-item') !== -1) {
            e.preventDefault();
            target.dispatchEvent(new MouseEvent('click', { bubbles: false, cancelable: true }));
            return;
        }
        target = target.parentElement;
    }
}, { passive: false });
</script>


</body>
</html>
