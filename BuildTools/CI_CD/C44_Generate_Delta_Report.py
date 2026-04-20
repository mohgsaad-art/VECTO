# -*- coding: utf-8 -*-
"""
Compare two C01 OR reports and produce a delta XLSX for CO2, FC, and EC.

Usage:
    python C44_Generate_Delta_Report.py <ref_report> <current_report> [--output <path>]

Arguments:
    ref_report  Path to the reference report (old).
    current_report  Path to the current report (new).
    --output  Output .xlsx path (default: DELTA_REPORT_{Date}_{Hash}.xlsx in CI_CD/Reports).
"""

import argparse
import datetime
import os
import re

import pandas as pd

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

_DELTA_VARIABLES = ['CO2 [g/km]', 'FC-Final [g/km]', 'EC_el_final [kWh/km]']
_DELTA_THRESHOLD = 0.0003  # 0.03%

_VAR_SHORT = {
    'CO2 [g/km]':           'CO2_delta_pct',
    'FC-Final [g/km]':      'FC_delta_pct',
    'EC_el_final [kWh/km]': 'EC_delta_pct',
}

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

def make_sheet_name(var: str) -> str:
    """Convert a variable name to a valid Excel sheet name (mirrors C01 logic)."""
    for ch in ('/', '[', ']'):
        var = var.replace(ch, ' ')
    return var.strip().replace(' ', '_')


def load_var_series(report_path: str, var: str) -> pd.Series | None:
    """Load a variable's per-case mean values from an OR report.

    Returns a Series indexed by 'case', or None if the sheet/column is absent.
    """
    sheet = make_sheet_name(var)
    try:
        df = pd.read_excel(report_path, sheet_name=sheet)
    except Exception:
        print(f'  Sheet "{sheet}" not found in {os.path.basename(report_path)} - skipping.')
        return None

    col = next((c for c in df.columns if var in c), None)
    if col is None:
        print(f'  Column matching "{var}" not found in sheet "{sheet}" - skipping.')
        return None

    if 'case' in df.columns:
        return df.set_index('case')[col].groupby(level=0).mean()
    return df[col]


def build_var_deltas(ref_report: str, current_report: str) -> dict[str, pd.DataFrame]:
    """Compute per-variable delta DataFrames between two OR reports.

    Returns a dict keyed by variable name. Each DataFrame has columns:
        reference, current, delta_abs, delta_pct
    indexed by 'case'. Only variables with data in both reports are included.
    """
    var_deltas: dict[str, pd.DataFrame] = {}
    for var in _DELTA_VARIABLES:
        ref_series = load_var_series(ref_report, var)
        cur_series = load_var_series(current_report, var)
        if ref_series is None or cur_series is None:
            continue

        df = (ref_series.rename('reference')
              .to_frame()
              .join(cur_series.rename('current'), how='inner')
              .dropna())

        df['delta_abs'] = df['current'] - df['reference']
        df['delta_pct'] = (df['delta_abs'] / df['reference'].replace(0, float('nan'))) * 100

        var_deltas[var] = df
        print(f'  {var}: {len(df)} matched test cases')

    return var_deltas


def build_delta_summary(var_deltas: dict[str, pd.DataFrame]) -> pd.DataFrame:
    """Build a summary DataFrame from per-variable delta results.

    Columns: CO2_delta_pct, FC_delta_pct, EC_delta_pct, status (OK/WARNING).
    """
    frames = [df['delta_pct'].rename(_VAR_SHORT[var]) for var, df in var_deltas.items()]
    if not frames:
        return pd.DataFrame()

    df_summary = pd.concat(frames, axis=1)
    df_summary['status'] = df_summary.apply(
        lambda row: 'OK' if all(abs(v) <= _DELTA_THRESHOLD * 100 for v in row.dropna()) else 'WARNING',
        axis=1,
    )
    return df_summary


def write_delta_report(output_path: str, var_deltas: dict[str, pd.DataFrame], df_summary: pd.DataFrame) -> None:
    """Write the delta report XLSX: Summary sheet first, then one sheet per variable."""
    with pd.ExcelWriter(output_path, engine='openpyxl') as writer:
        if not df_summary.empty:
            df_summary.to_excel(writer, sheet_name='Delta Summary')
        for var, df in var_deltas.items():
            df.to_excel(writer, sheet_name='Δ' + make_sheet_name(var))
    print(f'Written: {output_path}')


def generate_delta_report(ref_report: str, current_report: str, output_path: str | None = None) -> str:
    """Compare two OR reports and write a delta XLSX.

    Args:
        ref_report:    Path to the reference report (old).
        current_report:    Path to the current report (new).
        output_path: Output path. Defaults to DELTA_REPORT_{Date}_{Hash}.xlsx
                     in the CI_CD/Reports directory next to this script.

    Returns:
        The path of the written output file.
    """
    for p in (ref_report, current_report):
        if not os.path.isfile(p):
            raise FileNotFoundError(f'Report not found: {p}')

    if output_path is None:
        reports_dir = os.path.join(os.path.dirname(__file__), 'Reports')
        os.makedirs(reports_dir, exist_ok=True)
        m = re.match(r'OR_[^_]+_(\d{6})_\d{4}_([0-9a-f]+)', os.path.basename(current_report))
        date = m.group(1) if m else datetime.datetime.now().strftime('%y%m%d')
        hash_ = m.group(2) if m else 'unknown'
        output_path = os.path.join(reports_dir, f'DELTA_REPORT_{date}_{hash_}.xlsx')

    print(f'Reference : {ref_report}')
    print(f'Current   : {current_report}')
    print(f'Output    : {output_path}')

    var_deltas = build_var_deltas(ref_report, current_report)
    if not var_deltas:
        print('No matching variable sheets found - skipping delta report.')
        return None

    df_summary = build_delta_summary(var_deltas)
    write_delta_report(output_path, var_deltas, df_summary)

    return output_path


def append_delta_to_report(ref_report: str, current_report: str) -> None:
    """Compute delta between two OR reports and append the results to current_report.

    Appends to current_report:
      - 'Delta Summary' sheet with CO2/FC/EC delta_pct columns and a 'status' column
      - One 'Δ<var>' sheet per variable with reference, current, delta_abs, delta_pct
    Also adds a 'status' column to the existing 'Success' sheet in current_report.
    """
    import openpyxl

    var_deltas = build_var_deltas(ref_report, current_report)
    df_summary = build_delta_summary(var_deltas)

    if not var_deltas:
        print('No delta data - nothing appended.')
        return

    # Append delta sheets to the existing C01 report
    with pd.ExcelWriter(current_report, engine='openpyxl', mode='a', if_sheet_exists='replace') as writer:
        if not df_summary.empty:
            df_summary.to_excel(writer, sheet_name='Delta Summary')
        for var, df in var_deltas.items():
            df.to_excel(writer, sheet_name='Δ' + make_sheet_name(var))

    # Patch the Success sheet: add 'status' column from Delta Summary
    if not df_summary.empty and 'status' in df_summary.columns:
        wb = openpyxl.load_workbook(current_report)
        ws = wb['Success']

        # Build case -> status mapping
        status_map = df_summary['status'].to_dict()

        # Find the next empty column in the header row
        header_row = next(ws.iter_rows(min_row=1, max_row=1))
        next_col = len(header_row) + 1
        ws.cell(row=1, column=next_col, value='Delta Status')

        # Match rows by test name (column 1 = index)
        for row in ws.iter_rows(min_row=2):
            test_name = row[0].value
            ws.cell(row=row[0].row, column=next_col, value=status_map.get(test_name, ''))

        wb.save(current_report)

    print(f'Delta sheets appended to: {current_report}')


# ---------------------------------------------------------------------------
# CLI entry point
# ---------------------------------------------------------------------------

if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        description='Compare two C01 OR reports and write a delta XLSX.'
    )
    parser.add_argument('ref_report', help='Reference report path (old).')
    parser.add_argument('current_report', help='Current report path (new).')
    parser.add_argument(
        '--output', default=None,
        help='Output .xlsx path (default: DELTA_REPORT_{Date}_{Hash}.xlsx in CI_CD/Reports).',
    )
    args = parser.parse_args()
    generate_delta_report(args.ref_report, args.current_report, args.output)
