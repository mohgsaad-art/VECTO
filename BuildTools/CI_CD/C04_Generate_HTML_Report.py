# -*- coding: utf-8 -*-
"""
Created on Wed Apr  2 2026
"""

import argparse
import datetime
import glob
import os
import re
import pandas as pd

from FUNCTIONS.REPORTS import get_default_report_dir

parser = argparse.ArgumentParser(description='Generate a static HTML dashboard from the C01 test report.')
parser.add_argument('--report-dir', default=None,
                    help='Directory inside which the Reports folder is located (default: D:/Reports or C:/Reports).')
parser.add_argument('--interim-report', action='store_true', default=False,
                    help='Read from and write to the Interim_Reports subfolder of the report directory.')
args = parser.parse_args()

_base = os.path.join(args.report_dir, 'Reports') if args.report_dir else get_default_report_dir()
report_base_dir = os.path.join(_base, 'Interim_Reports') if args.interim_report else _base

# find most recent C01 report (OR naming convention, no DETAIL or CRF suffix)
xlsx_files = sorted(glob.glob(os.path.join(report_base_dir, 'OR*.xlsx')))
xlsx_files = [f for f in xlsx_files if 'DETAIL' not in f and 'CRF' not in f]
if not xlsx_files:
    raise FileNotFoundError(f'No OR*.xlsx found in {report_base_dir}')

report_path = xlsx_files[-1]
print(f'Reading: {report_path}')

# extract date and hash from source filename: OR_{Version}_{Date}_{NNNN}_{Hash}[_suffix].xlsx
_m = re.match(r'OR_[^_]+_(\d{6})_\d{4}_([0-9a-f]+)', os.path.basename(report_path))
_report_date = _m.group(1) if _m else datetime.datetime.now().strftime('%y%m%d')
_commit_hash = _m.group(2) if _m else 'unknown'

df = pd.read_excel(report_path, sheet_name='Success', index_col=0)

# summary counts
total = len(df)
n_ok = (df['Success'] == 'OK').sum()
n_error = df['Success'].str.startswith('ERROR').sum()
n_not_run = (df['Success'] == 'not run').sum()

def row_color(status):
    if status == 'OK':
        return '#d4edda'
    if isinstance(status, str) and status.startswith('ERROR'):
        return '#f8d7da'
    return '#e2e3e5'

def badge_color(status):
    if status == 'OK':
        return 'background:#28a745;color:#fff'
    if isinstance(status, str) and status.startswith('ERROR'):
        return 'background:#dc3545;color:#fff'
    return 'background:#6c757d;color:#fff'

_CRITERIA_COLS = ['vsum_ok', 'log_ok', 'rslt_mfr_ok', 'complete_step_ok']
_CRITERIA_LABELS = {
    'vsum_ok':          'VSUM',
    'log_ok':           'Log 100%',
    'rslt_mfr_ok':      'MFR',
    'complete_step_ok': 'Complete step',
}

def bool_cell(value) -> str:
    if value is True or str(value).lower() == 'true':
        return '<td style="text-align:center;color:#28a745;font-size:15px">&#10003;</td>'
    return '<td style="text-align:center;color:#dc3545;font-size:15px">&#10007;</td>'

def delta_cell(value) -> str:
    if pd.isna(value) or value == '':
        return '<td style="text-align:center;color:#aaa">—</td>'
    if str(value).startswith('WARNING'):
        return (
            f'<td><span style="padding:2px 8px;border-radius:4px;font-size:11px;'
            f'background:#ffc107;color:#212529" title="{value}">WARNING</span></td>'
        )
    return '<td style="text-align:center;color:#28a745;font-size:15px">&#10003;</td>'

_has_delta = 'Delta Status' in df.columns
n_delta_warn = df['Delta Status'].str.startswith('WARNING').sum() if _has_delta else 0

# build table rows
rows_html = ''
for test_name, row in df.iterrows():
    status = row['Success']
    bg = row_color(status)
    badge = badge_color(status)
    criteria_cells = ''.join(
        bool_cell(row[col]) if col in row.index else '<td></td>'
        for col in _CRITERIA_COLS
    )
    delta_col = delta_cell(row.get('Delta Status', float('nan'))) if _has_delta else ''
    _delta_val = row.get('Delta Status', '') if _has_delta else ''
    _warn_icon = ' <span style="color:#856404" title="Delta warning">&#9888;</span>' if isinstance(_delta_val, str) and _delta_val.startswith('WARNING') else ''
    rows_html += (
        f'<tr style="background:{bg}">'
        f'<td>{test_name}{_warn_icon}</td>'
        f'<td><span style="padding:2px 8px;border-radius:4px;font-size:11px;{badge}">{status}</span></td>'
        f'{criteria_cells}'
        f'{delta_col}'
        f'</tr>\n'
    )

now = datetime.datetime.now()
source_file = os.path.basename(report_path)

html = f"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>VECTO CI Test Report</title>
<style>
  h1, h2, h3, h4, h5, h6, p, blockquote {{ margin: 0; padding: 0; }}
  body {{
    font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
    font-size: 13px;
    line-height: 18px;
    color: #737373;
    background-color: white;
    margin: 10px 13px 10px 13px;
  }}
  @media screen and (min-width: 914px) {{ body {{ width: 854px; margin: 10px auto; }} }}
  h1 {{ color: #404040; font-size: 24px; line-height: 36px; margin-bottom: 8px; }}
  h2 {{ color: #404040; font-size: 16px; line-height: 28px; margin: 16px 0 6px 0; }}
  table {{ margin: 10px 0 15px 0; border-collapse: collapse; width: 100%; }}
  td, th {{ border: 1px solid #ddd; padding: 4px 10px; }}
  th {{ padding: 6px 10px; background: #f5f5f5; color: #404040; text-align: left; cursor: pointer; user-select: none; }}
  th:hover {{ background: #e8e8e8; }}
  .summary {{
    display: flex; gap: 12px; margin: 12px 0;
  }}
  .card {{
    padding: 10px 18px; border-radius: 6px; font-size: 13px; font-weight: bold; min-width: 90px; text-align: center;
  }}
  .card .num {{ font-size: 28px; display: block; }}
  .card-total {{ background: #e9ecef; color: #495057; }}
  .card-ok    {{ background: #d4edda; color: #155724; }}
  .card-error {{ background: #f8d7da; color: #721c24; }}
  .card-skip  {{ background: #e2e3e5; color: #383d41; }}
  .card-warn  {{ background: #fff3cd; color: #856404; }}
  .meta {{ color: #aaa; font-size: 11px; margin-bottom: 14px; }}
  input#search {{
    padding: 5px 10px; font-size: 13px; border: 1px solid #ddd;
    border-radius: 4px; margin-bottom: 8px; width: 260px;
  }}
  @media print {{ body, h1, h2, th, td {{ color: black; }} table {{ page-break-inside: avoid; }} }}
</style>
</head>
<body>

<h1>VECTO CI Test Report</h1>
<p class="meta">Generated: {now.strftime("%Y-%m-%d %H:%M")} &nbsp;|&nbsp; Source: {source_file}</p>

<div class="summary">
  <div class="card card-total"><span class="num">{total}</span>Total</div>
  <div class="card card-ok"><span class="num">{n_ok}</span>Passed</div>
  <div class="card card-error"><span class="num">{n_error}</span>Failed</div>
  {'<div class="card card-warn"><span class="num">' + str(n_delta_warn) + '</span>Warnings</div>' if _has_delta else ''}
  <div class="card card-skip"><span class="num">{n_not_run}</span>Not run</div>
</div>

<h2>Results</h2>
<input type="text" id="search" placeholder="Filter by test name or status..." onkeyup="filterTable()">
<table id="resultsTable">
  <thead>
    <tr>
      <th onclick="sortTable(0)">Test Name &#8597;</th>
      <th onclick="sortTable(1)">Status &#8597;</th>
      <th onclick="sortTable(2)">VSUM &#8597;</th>
      <th onclick="sortTable(3)">Log 100% &#8597;</th>
      <th onclick="sortTable(4)">MFR &#8597;</th>
      <th onclick="sortTable(5)">Complete step &#8597;</th>
      {'<th onclick="sortTable(6)">Delta Diff &#8597;</th>' if _has_delta else ''}
    </tr>
  </thead>
  <tbody>
{rows_html}  </tbody>
</table>

<script>
function filterTable() {{
  var input = document.getElementById('search').value.toLowerCase();
  var rows = document.getElementById('resultsTable').getElementsByTagName('tr');
  for (var i = 1; i < rows.length; i++) {{
    var text = rows[i].innerText.toLowerCase();
    rows[i].style.display = text.includes(input) ? '' : 'none';
  }}
}}

function sortTable(col) {{
  var table = document.getElementById('resultsTable');
  var rows = Array.from(table.getElementsByTagName('tr')).slice(1);
  var asc = table.dataset.sortCol == col && table.dataset.sortDir == 'asc';
  rows.sort(function(a, b) {{
    var x = a.cells[col].innerText.trim();
    var y = b.cells[col].innerText.trim();
    return asc ? y.localeCompare(x) : x.localeCompare(y);
  }});
  var tbody = table.getElementsByTagName('tbody')[0];
  rows.forEach(function(r) {{ tbody.appendChild(r); }});
  table.dataset.sortCol = col;
  table.dataset.sortDir = asc ? 'desc' : 'asc';
}}
</script>

</body>
</html>
"""

html_dir = os.path.join(report_base_dir, 'HTML')
os.makedirs(html_dir, exist_ok=True)
output_path = os.path.join(html_dir, f'HTML_REPORT_{_report_date}_{_commit_hash}.html')
with open(output_path, 'w', encoding='utf-8') as f:
    f.write(html)

print(f'HTML report written to: {output_path}')
