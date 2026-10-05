/**
 * harmonize-common.js — shared helpers for Harmonize chart templates
 */

// Auto-detect frequency from data point spacing
function detectFreq(data) {
    if (!data || data.length < 2) return 'Y';
    var gap = data[1][0] - data[0][0];
    if (gap < 0.5 * 86400000) return 'T';         // < 12 hours → minutely/hourly
    else if (gap < 3 * 86400000) return 'D';       // < 3 days → daily
    else if (gap < 50 * 86400000) return 'M';      // < 50 days → monthly
    else if (gap < 120 * 86400000) return 'Q';     // < 120 days → quarterly
    return 'Y';                                     // yearly
}

// Map frequency code to Highcharts date format string
function dateFmtForFreq(freq) {
    switch (freq) {
        case 'T': return '%Y-%m-%dT%H:%M';
        case 'D': return '%Y-%m-%d';
        case 'M': return '%Y-%m-%d';
        case 'Q': return '%Y Q';   // quarter placeholder, use formatDate for display
        case 'Y': return '%Y';
        default:  return '%Y-%m-%d';
    }
}

// Format a timestamp according to detected frequency (handles quarterly specially)
function formatDate(ts, freq) {
    if (freq === 'Q') {
        var d = new Date(ts);
        var q = Math.floor(d.getUTCMonth() / 3) + 1;
        return d.getUTCFullYear() + ' Q' + q;
    }
    return Highcharts.dateFormat(dateFmtForFreq(freq), ts);
}

// Frequency rank for comparing (lower = finer)
var freqRank = { 'T': 0, 'D': 1, 'M': 2, 'Q': 3, 'Y': 4 };

// Find the finest frequency among an array of freq codes
function finestOfFreqs(freqs) {
    return freqs.reduce(function(a, b) {
        return freqRank[a] <= freqRank[b] ? a : b;
    }, 'Y');
}
