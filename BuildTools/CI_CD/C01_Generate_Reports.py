# -*- coding: utf-8 -*-
"""
C01 - Generate all VECTO CI test reports (pass/fail summary, VSUM detail, MRF/CRF).

All three report types share the same OR report number so they form a coherent bundle:
  OR_{Version}_{Date}_{NNNN}_{Hash}.xlsx          -- pass/fail summary
  OR_{Version}_{Date}_{NNNN}_{Hash}_DETAIL.xlsx   -- VSUM detail
  OR_{Version}_{Date}_{NNNN}_{Hash}_CRF.xlsx      -- MRF/CRF fuel consumption

Usage:
    python C01_Generate_Reports.py <vecto_root>
        [--report-dir <path>]        Directory where Reports/ will be created (default: D:/Reports or C:/Reports)
        [--interim-report]           Write to Interim_Reports/ subfolder instead of Reports/
        [--reports-cache <path>]     Cache directory used to determine the next report number
        [--mr-reports-cache <path>]  Directory for MR OR reports (default: <reports-cache>/MR)
        [--ref-vecto-root <path>]    Path to REF_VECTO_ROOT; required with --is-mr-run
        [--is-mr-run]                Also generate the MR OR report and include the Intermediate Delta
"""

import argparse
import datetime
import glob
import json
import os
import re
import shutil
import sys

import numpy as np
import pandas as pd
from lxml import etree

from C44_Generate_Delta_Report import append_delta_to_report, generate_delta_report
from FUNCTIONS.REPORTS import (
    get_default_report_dir,
    get_latest_vecto_release,
    get_latest_vecto_releases_by_type,
    get_next_report_number,
    get_vecto_hash,
    get_vecto_version,
)
from FUNCTIONS.VECTO import get_vecto_git_root

# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

parser = argparse.ArgumentParser(
    description='Generate VECTO CI test reports (summary, VSUM detail, MRF/CRF).'
)
parser.add_argument('vecto_root', help='Path to the VECTO installation directory.')
parser.add_argument(
    '--report-dir',
    default=None,
    help='Directory where a Reports folder will be created (default: D:/Reports or C:/Reports).',
)
parser.add_argument(
    '--interim-report',
    action='store_true',
    default=False,
    help='Write reports to an Interim_Reports subfolder of the report directory.',
)
parser.add_argument(
    '--reports-cache',
    default=None,
    help='Reports cache directory to determine the next report number.',
)
parser.add_argument(
    '--mr-reports-cache',
    default=None,
    help='Directory for MR OR reports. Defaults to <reports-cache>/MR.',
)
parser.add_argument(
    '--ref-vecto-root',
    default=None,
    help='Path to REF_VECTO_ROOT. When supplied together with --is-mr-run, '
         'the MR vecto executable is taken from <ref-vecto-root>/MR.',
)
parser.add_argument(
    '--is-mr-run',
    action='store_true',
    default=False,
    help='When set, also generates the MR OR report (from <ref-vecto-root>/MR results) '
         'before generating the current report, so the Intermediate Delta is included '
         'in the same pipeline run.',
)
args = parser.parse_args()

if not os.path.exists(args.vecto_root):
    raise FileNotFoundError(f'VECTO path does not exist: {args.vecto_root}')

vecto_root = args.vecto_root
vecto_version = get_vecto_version(vecto_root)
print(f'VECTO version   : {repr(vecto_version)}')
result_dir = os.path.join(os.getcwd(), 'XX_RESULTS', vecto_version)
print(f'Result dir      : {result_dir}')
print(f'Result dir exists: {os.path.exists(result_dir)}')
if os.path.exists(os.path.join(os.getcwd(), 'XX_RESULTS')):
    print(f'XX_RESULTS contents: {os.listdir(os.path.join(os.getcwd(), "XX_RESULTS"))}')

_base = os.path.join(args.report_dir, 'Reports') if args.report_dir else get_default_report_dir()
report_base_dir = os.path.join(_base, 'Interim_Reports') if args.interim_report else _base
_reports_cache = args.reports_cache if args.reports_cache else report_base_dir
_mr_reports_cache = args.mr_reports_cache if args.mr_reports_cache else os.path.join(_reports_cache, 'MR')

_failed_reports_drive = 'D:\\' if os.path.exists('D:\\') else 'C:\\'
_failed_reports_dir = os.path.join(_failed_reports_drive, 'CI', 'FailedReports')

os.makedirs(report_base_dir, exist_ok=True)
os.makedirs(_reports_cache, exist_ok=True)

# ---------------------------------------------------------------------------
# Shared: resolve report metadata (computed once, shared by all three reports)
# ---------------------------------------------------------------------------

date = datetime.datetime.now().strftime('%y%m%d')
nnnn = get_next_report_number(report_base_dir)
vecto_hash = get_vecto_hash(vecto_root)


def report_path() -> str:
    """Return the full output path for the combined report file."""
    filename = f'OR_{vecto_version}_{date}_{nnnn}_{vecto_hash}.xlsx'
    return os.path.join(report_base_dir, filename)


def get_latest_mr_report(mr_reports_dir: str) -> str | None:
    """Return the single OR report in the MR reports directory, or None if absent."""
    if not os.path.isdir(mr_reports_dir):
        return None
    candidates = [
        os.path.join(mr_reports_dir, f)
        for f in os.listdir(mr_reports_dir)
        if re.match(r'^OR_.+\.xlsx$', f)
    ]
    return candidates[0] if candidates else None


def get_latest_release_report(reports_dir: str) -> str | None:
    """Return the path of the summary report associated with the latest VECTO release.

    Tries the most recently published tag first (official, RC, or DEV). If no
    report matches that version, falls back through the latest official, RC, and
    DEV releases in that order.  Returns the matching report with the highest
    NNNN, or None if no match is found for any release.
    """
    _report_re = re.compile(r'^OR_(.+)_\d{6}_(\d{4})_[0-9a-f]+\.xlsx$')

    def _best_report_for_version(tag_version: str) -> str | None:
        best_nnnn, best_path = -1, None
        for fname in os.listdir(reports_dir):
            m = _report_re.match(fname)
            if not m or m.group(1) != tag_version:
                continue
            nnnn_val = int(m.group(2))
            if nnnn_val > best_nnnn:
                best_nnnn, best_path = nnnn_val, os.path.join(reports_dir, fname)
        return best_path

    # Build candidate list: most-recently-published first, then by type
    git_root = get_vecto_git_root(vecto_root) or vecto_root

    latest_tag = get_latest_vecto_release(git_root)
    by_type    = get_latest_vecto_releases_by_type(git_root)

    candidates = []
    if latest_tag:
        candidates.append(latest_tag)
    for key in ('official', 'rc', 'dev'):
        tag = by_type.get(key)
        if tag and tag not in candidates:
            candidates.append(tag)

    for tag in candidates:
        tag_version = tag.removeprefix('Release/v')
        result = _best_report_for_version(tag_version)
        if result:
            return result

    return None


# ---------------------------------------------------------------------------
# Shared: discover result directories
# ---------------------------------------------------------------------------

def find_result_dirs(result_dir: str) -> list[str]:
    """
    Walk result_dir and return the list of leaf directories that contain
    runnable test cases (.xml input files or .vecto files).

    Directories that contain a .vecto file are 'bus complete step' cases -
    they are included directly (via the .vecto file) rather than via their
    child .xml files.
    """
    file_list_runable = [
        os.path.join(root, f)
        for root, _dirs, files in os.walk(result_dir)
        for f in files
        if (f.endswith('.xml') and 'RSLT' not in f and 'VIF' not in f)
        or f.endswith('.vecto')
    ]

    dirs_with_vecto = {os.path.dirname(f) for f in file_list_runable if f.endswith('.vecto')}

    file_list_torun = [
        f for f in file_list_runable
        if os.path.dirname(f) not in dirs_with_vecto or f.endswith('.vecto')
    ]

    return [os.path.dirname(f) for f in file_list_torun]


def make_sheet_name(var: str) -> str:
    """Convert a variable name to a valid Excel sheet name."""
    for ch in ('/', '[', ']'):
        var = var.replace(ch, ' ')
    return var.strip().replace(' ', '_')


# ---------------------------------------------------------------------------
# Report 1: pass/fail summary
# ---------------------------------------------------------------------------

def build_summary_dataframe(result_dir: str) -> tuple[pd.DataFrame, list[str]]:
    """
    Inspect each result directory and classify simulations as OK / ERROR / not run.

    Returns:
        df_all: DataFrame with columns Test_Name, Success, cpu_time
        failed_names: list of test names that contain an error
    """
    _num_pattern = re.compile(r'([0-9.]+)')

    res_dir_list = find_result_dirs(result_dir)
    dirs_with_vecto = {
        os.path.dirname(f)
        for root, _dirs, files in os.walk(result_dir)
        for f in files
        if f.endswith('.vecto')
        for f in [os.path.join(root, f)]  # rebuild full path
    }
    # Recompute cleanly to avoid the comprehension confusion above
    dirs_with_vecto = set()
    for root, _dirs, files in os.walk(result_dir):
        for f in files:
            if f.endswith('.vecto'):
                dirs_with_vecto.add(root)

    res_name_list = [
        '__'.join(os.path.relpath(d, result_dir).split(os.sep))
        for d in res_dir_list
    ]

    success_list:          list[str]  = []
    vsum_ok_list:          list[bool] = []
    log_ok_list:           list[bool] = []
    rslt_mfr_ok_list:      list[bool] = []
    complete_step_ok_list: list[bool] = []
    cpu_time:              list[float] = []

    for res_dir in res_dir_list:
        vsums = [os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.vsum')]
        logs  = [os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.log')]
        rslt_manufacturers = [
            os.path.join(res_dir, f) for f in os.listdir(res_dir) if 'RSLT_MANUFACTURER' in f
        ]

        if not vsums:
            success_list.append('not run' if not logs else 'ERROR no vsum')
            vsum_ok_list.append(False)
            log_ok_list.append(False)
            rslt_mfr_ok_list.append(False)
            complete_step_ok_list.append(False)
            cpu_time.append(0.0)
            continue

        vsum = vsums[0]
        df_vsum = pd.read_csv(vsum, skiprows=[0])[:-1]
        vsum_ok = (df_vsum['Status'] == 'Success').all()

        log = logs[0]
        with open(log) as fh:
            log_ok = any('[100.00%]' in line or '[100.000%]' in line for line in fh)
        with open(log) as fh:
            t_cpu = [_num_pattern.findall(line)[0] for line in fh if 'Simulation runs' in line]

        # Resolve RSLT_MANUFACTURER file
        if len(rslt_manufacturers) == 1:
            rslt_mfr = rslt_manufacturers[0]
        elif len(rslt_manufacturers) > 1:
            rslt_mfr = next(
                f for f in rslt_manufacturers if '.RSLT_MANUFACTURER.xml' in f
            )
        else:
            rslt_mfr = None

        # Bus complete-step check (tracked independently - does not modify vsum_ok)
        complete_step_ok = True
        if res_dir in dirs_with_vecto:
            vecto_file = next(
                os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.vecto')
            )
            try:
                with open(vecto_file) as fh:
                    file_version = json.load(fh)['Header']['FileVersion']
            except Exception:
                file_version = 0

            if file_version == 10:
                rslt_mfr_final = [
                    f for f in os.listdir(res_dir) if '.RSLT_MANUFACTURER.xml' in f
                ]
                if not rslt_mfr_final:
                    with open(log) as fh:
                        exempted = any('xempted' in line for line in fh)
                    if not exempted:
                        complete_step_ok = False

        # RSLT_MANUFACTURER error check
        rslt_mfr_ok = True
        if rslt_mfr is not None:
            with open(rslt_mfr) as fh:
                rslt_mfr_ok = not any('Error' in line for line in fh)

        vsum_ok_list.append(bool(vsum_ok))
        log_ok_list.append(log_ok)
        rslt_mfr_ok_list.append(rslt_mfr_ok)
        complete_step_ok_list.append(complete_step_ok)

        if vsum_ok and log_ok and rslt_mfr_ok and complete_step_ok:
            success_list.append('OK')
            cpu_time.append(float(t_cpu[0]))
        else:
            if 'timeout' in log:
                success_list.append('ERROR Timeout')
                cpu_time.append(float(_num_pattern.findall(log)[0]))
            elif vsum_ok and log_ok:
                success_list.append('ERROR VIF')
                cpu_time.append(0.0)
            else:
                success_list.append('ERROR')
                cpu_time.append(0.0)

    df_all = pd.DataFrame({
        'Test_Name':        res_name_list,
        'Success':          success_list,
        'vsum_ok':          vsum_ok_list,
        'log_ok':           log_ok_list,
        'rslt_mfr_ok':      rslt_mfr_ok_list,
        'complete_step_ok': complete_step_ok_list,
        'cpu_time':         cpu_time,
    })
    df_all['Test_Name'] = df_all['Test_Name'].astype(str).str.replace(' ', '_')

    failed_names = [
        name for name, status in zip(res_name_list, success_list)
        if 'error' in status.lower()
    ]
    return df_all, failed_names


def write_summary_report(
    writer: pd.ExcelWriter,
    df_all: pd.DataFrame,
    failed_names: list[str],
) -> None:
    success_cols = ['Success', 'vsum_ok', 'log_ok', 'rslt_mfr_ok', 'complete_step_ok']
    (df_all[['Test_Name'] + success_cols]
        .sort_values('Test_Name')
        .set_index('Test_Name')
        .to_excel(writer, sheet_name='Success'))

    (df_all[['Test_Name', 'cpu_time']]
        .sort_values('Test_Name')
        .set_index('Test_Name')
        .to_excel(writer, sheet_name='cpu_time'))

    OK_GREEN  = '\033[92m'
    FAIL_RED  = '\033[91m'
    END_COLOR = '\033[0m'
    print(f'Number of failed vehicles: {len(failed_names)}')
    if not failed_names:
        print(f'{OK_GREEN}Success{END_COLOR}')
    else:
        print(f'{FAIL_RED}Failed{END_COLOR}')


# ---------------------------------------------------------------------------
# Report 2: VSUM detail
# ---------------------------------------------------------------------------

_VSUM_VARIABLES = [
    'CO2 [g/km]', 'FC-Final [g/km]', 'EC_el_final [kWh/km]',
    'ΔE_REESS [kWh]', 'REESS Start SoC [%]', 'REESS End SoC [%]', 'f_equiv', 'Status',
]
_VSUM_KEYID_ALL = [
    'Cycle [-]', 'HDV CO2 vehicle class [-]', 'Corrected Actual Curb Mass [kg]',
    'Loading [kg]', 'Vehicle fuel type [-]', 'Ovc Mode [-]', 'Job [-]',
]


def build_vsum_dataframe(result_dir: str) -> pd.DataFrame:
    res_dir_list = find_result_dirs(result_dir)
    df_all = None

    for res_dir in res_dir_list:
        vsums = [os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.vsum')]
        if not vsums:
            continue

        case_name = '__'.join(os.path.relpath(res_dir, result_dir).split(os.sep))
        df_vsum = pd.read_csv(vsums[0], skiprows=[0])[:-1]

        col_sel  = [c for c in df_vsum.columns if c in _VSUM_VARIABLES]
        keyid    = [k for k in _VSUM_KEYID_ALL if k in df_vsum.columns]
        col_to_create = list(
            set(_VSUM_VARIABLES + [k for k in _VSUM_KEYID_ALL if k not in df_vsum.columns])
            - set(col_sel)
        )

        df_mod = df_vsum[keyid + col_sel].copy()
        if col_to_create:
            df_mod[col_to_create] = np.nan

        df_mod['case'] = case_name.replace(' ', '_')
        if '_ENG_' in case_name:
            df_mod['HDV CO2 vehicle class [-]'] = '-'

        df_mod = (
            df_mod
            .replace({'Job [-]': '-'},      '-0',  regex=False)
            .replace({'Job [-]': r'-\d+-'}, '-',   regex=True)
            .replace({'Job [-]': '-01'},    '-1',  regex=False)
            .replace({'Job [-]': '-02'},    '-2',  regex=False)
        )

        df_all = df_mod if df_all is None else pd.concat([df_all, df_mod])

    return df_all


def write_vsum_report(writer: pd.ExcelWriter, df_all: pd.DataFrame) -> None:
    keyid   = [k for k in _VSUM_KEYID_ALL if k in df_all.columns]
    key4cat = ['case'] + keyid

    ln, lm, lstd, lmin, lmax = [], [], [], [], []

    for var in _VSUM_VARIABLES:
        if var == 'Status':
            continue

        var2keep   = [c for c in df_all.columns if var in c]
        status_col = [c for c in df_all.columns if 'Status' in c]
        col2keep   = key4cat + status_col + var2keep

        df_var = (
            df_all[col2keep]
            .sort_values(by=key4cat)
            .set_index(key4cat)
            .dropna(subset=var2keep, how='all')
        )

        sel = df_var[status_col[0]] == 'Success'
        for i in range(1, len(var2keep)):
            diff_col = f'Diff. [%] with {var2keep[i]}'
            df_var[diff_col] = np.round(
                (df_var[var2keep[i]] / df_var[var2keep[0]] - 1) * 100.0 * 100
            ) / 100

            err  = (df_var[var2keep[i]] / df_var[var2keep[0]] - 1) * 100.0
            sel2 = abs(err) > 0.05
            sel3 = df_var[status_col[i]] == 'Success'
            err  = err[sel & sel2 & sel3]

            ln.append(diff_col)
            if len(err):
                print(np.count_nonzero(~np.isnan(err)))
                lm.append(np.nanmean(err))
                lmin.append(np.nanmin(err))
                lmax.append(np.nanmax(err))
                lstd.append(np.nanstd(err))
            else:
                lm.append(np.nan); lmin.append(np.nan)
                lmax.append(np.nan); lstd.append(np.nan)

        for i in range(1, len(var2keep)):
            diff_col = f'Diff. [%] with {var2keep[i]}'
            df_var[f'MOD - OK {var2keep[i]}'] = np.where(
                (
                    (abs(df_var[diff_col]) < 0.3)
                    & (df_var[status_col[i]] == df_var[status_col[0]])
                    & (df_var[status_col[i]] == 'Success')
                ) | (
                    (df_var[status_col[i]] == 'Success')
                    & (df_var[status_col[0]] != 'Success')
                ),
                True, False,
            )

        df_var.to_excel(writer, sheet_name=make_sheet_name(var))

    # Stats sheet
    dic = {
        name: [mean, mn, mx, std]
        for name, mean, mn, mx, std in zip(ln, lm, lmin, lmax, lstd)
    }
    df_stats = (
        pd.DataFrame(dic)
        .transpose()
        .rename(columns={
            0: 'mean of variations>0.05% (for successful simulations)',
            1: 'min of variations (for successful simulations)',
            2: 'max of variations (for successful simulations)',
            3: 'std of variations (for successful simulations)',
        })
    )
    df_stats.to_excel(writer, sheet_name='stats of non-zero variations')


# ---------------------------------------------------------------------------
# Report 3: MRF/CRF fuel consumption
# ---------------------------------------------------------------------------

_CRF_VARIABLES = ['FuelConsumption [g/km]', 'FuelConsumption [MJ/km]', 'CO2 [g/km]']
_CRF_KEY4CAT   = ['case', 'VehicleGroup', 'Model', 'TPMLM [kg]', 'avgPayload/Passenger [kg]', 'vocational']


def build_crf_dataframe(result_dir: str) -> pd.DataFrame | None:
    res_dir_list = find_result_dirs(result_dir)
    df_all = None

    for res_dir in res_dir_list:
        crf_files = [os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.RSLT_CUSTOMER.xml')]
        logs      = [os.path.join(res_dir, f) for f in os.listdir(res_dir) if f.endswith('.log')]

        if not crf_files or not logs:
            continue

        with open(logs[0]) as fh:
            log_ok = any('[100.000%]' in line or '[100.00%]' in line for line in fh)
        if not log_ok:
            continue

        case_name = '__'.join(os.path.relpath(res_dir, result_dir).split(os.sep)).replace(' ', '_')
        xml_tree = etree.parse(crf_files[0])
        df_crf = None

        for summary in xml_tree.findall('.//{*}Summary'):
            dico: dict = {}

            vocational_node = summary.find('.//{*}Vocational')
            is_vocational = vocational_node is not None and vocational_node.text == 'true'

            avg_payload_node  = summary.find('.//{*}AveragePayload')
            passenger_node    = summary.find('.//{*}MassPassengers')
            if avg_payload_node is not None:
                avg_payload = float(avg_payload_node.text) * 1000
            elif passenger_node is not None:
                avg_payload = float(passenger_node.text)
            else:
                avg_payload = 0.0

            for var_name in ('FuelConsumption', 'CO2'):
                for var_node in summary.findall(f'.//{"{*}"}{var_name}'):
                    label = f'{var_name} [{var_node.attrib["unit"]}]'
                    dico[label] = [float(var_node.text)]

            if not dico:
                continue

            df_i = pd.DataFrame.from_dict(dico)
            df_i['case'] = case_name
            df_i['vocational'] = is_vocational
            df_i['avgPayload/Passenger [kg]'] = avg_payload

            df_crf = df_i if df_crf is None else pd.concat([df_crf, df_i], ignore_index=False)

        if df_crf is None:
            continue

        tpmlm_node = xml_tree.find('.//{*}TechnicalPermissibleMaximumLadenMass')
        if tpmlm_node is not None:
            df_crf['TPMLM [kg]'] = int(tpmlm_node.text)

        for tag, col in (('VehicleGroup', 'VehicleGroup'), ('Model', 'Model')):
            node = xml_tree.find(f'.//{"{*}"}{tag}')
            if node is not None:
                df_crf[col] = node.text

        df_all = df_crf if df_all is None else pd.concat([df_all, df_crf])

    return df_all


def write_crf_report(writer: pd.ExcelWriter, df_all: pd.DataFrame) -> None:
    for var in _CRF_VARIABLES:
        if var == 'Status':
            continue

        var2keep = [c for c in df_all.columns if var in c]
        col2keep = _CRF_KEY4CAT + var2keep

        df_var = (
            df_all[col2keep]
            .sort_values(by=_CRF_KEY4CAT)
            .set_index(_CRF_KEY4CAT)
            .dropna(subset=var2keep, how='all')
        )

        for i in range(1, len(var2keep)):
            diff_col = f'Diff. [%] with {var2keep[i]}'
            df_var[diff_col] = np.round(
                (df_var[var2keep[i]] / df_var[var2keep[0]] - 1) * 100.0 * 100
            ) / 100

        for i in range(1, len(var2keep)):
            diff_col = f'Diff. [%] with {var2keep[i]}'
            df_var[f'MOD - OK {var2keep[i]}'] = np.where(
                (abs(df_var[diff_col]) < 0.3)
                | (df_var[var2keep[i]].notna() & df_var[var2keep[0]].isna()),
                True, False,
            )

        df_var.to_excel(writer, sheet_name='CRF_' + make_sheet_name(var))


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def write_report(result_dir: str, output_path: str) -> list[str]:
    """Build and write a single OR report. Returns the list of failed test names."""
    print('--- Building report data ---')
    df_summary, failed_names = build_summary_dataframe(result_dir)
    df_vsum = build_vsum_dataframe(result_dir)
    df_crf  = build_crf_dataframe(result_dir)

    print(f'--- Writing report: {output_path} ---')
    with pd.ExcelWriter(output_path, engine='openpyxl') as writer:
        write_summary_report(writer, df_summary, failed_names)
        if df_vsum is not None and not df_vsum.empty:
            write_vsum_report(writer, df_vsum)
        else:
            print('No VSUM data found, skipping DETAIL sheets.')
        if df_crf is not None and not df_crf.empty:
            write_crf_report(writer, df_crf)
        else:
            print('No CRF data found, skipping CRF sheets.')

    print(f'Written: {output_path}')
    return failed_names


def refresh_mr_report() -> None:
    """Generate a fresh MR OR report, replacing any existing one in _mr_reports_cache.

    Only called when --is-mr-run is set. Requires --ref-vecto-root so the MR
    vecto executable can be found at <ref-vecto-root>/MR.
    """
    if not args.ref_vecto_root:
        print('--ref-vecto-root not supplied - cannot generate MR report.')
        return

    mr_vecto_root = os.path.join(args.ref_vecto_root, 'MR')
    if not os.path.isdir(mr_vecto_root):
        print(f'MR vecto root not found at {mr_vecto_root} - skipping MR report.')
        return

    mr_version = get_vecto_version(mr_vecto_root)
    mr_result_dir = os.path.join(os.getcwd(), 'XX_RESULTS', mr_version)
    if not os.path.isdir(mr_result_dir):
        print(f'MR result dir not found at {mr_result_dir} - skipping MR report.')
        return

    os.makedirs(_mr_reports_cache, exist_ok=True)
    for _old in glob.glob(os.path.join(_mr_reports_cache, 'OR_*.xlsx')):
        os.remove(_old)
        print(f'Removed old MR report: {_old}')

    mr_hash = get_vecto_hash(mr_vecto_root)
    mr_nnnn = get_next_report_number(_mr_reports_cache)
    mr_output_path = os.path.join(_mr_reports_cache, f'OR_{mr_version}_{date}_{mr_nnnn}_{mr_hash}.xlsx')

    write_report(mr_result_dir, mr_output_path)
    print(f'MR report written: {mr_output_path}')


def purge_old_failed_reports(failed_reports_dir: str, max_age_days: int = 10) -> None:
    if not os.path.isdir(failed_reports_dir):
        return
    cutoff = datetime.datetime.now() - datetime.timedelta(days=max_age_days)
    for entry in os.scandir(failed_reports_dir):
        if entry.is_dir():
            mtime = datetime.datetime.fromtimestamp(entry.stat().st_mtime)
            if mtime < cutoff:
                shutil.rmtree(entry.path)
                print(f'  Purged old failed reports: {entry.name}')


def copy_failed_outputs(failed_names: list[str], src_result_dir: str, dest_dir: str) -> None:
    if not failed_names:
        return
    os.makedirs(dest_dir, exist_ok=True)
    for name in failed_names:
        rel_path = name.replace('__', os.sep)
        src = os.path.join(src_result_dir, rel_path)
        dst = os.path.join(dest_dir, name)
        if os.path.isdir(src):
            if os.path.exists(dst):
                shutil.rmtree(dst)
            shutil.copytree(src, dst)
            print(f'  Copied failed outputs: {name}')
        else:
            print(f'  WARNING: result dir not found for failed vehicle: {src}')


def main() -> None:
    # When --is-mr-run is set, regenerate the MR OR report first so the
    # Intermediate Delta reflects the latest MR build in this same pipeline run.
    if args.is_mr_run:
        print('--- Generating MR OR report ---')
        refresh_mr_report()

    output_path = report_path()
    failed_names = write_report(result_dir, output_path)

    purge_old_failed_reports(_failed_reports_dir)
    if failed_names:
        run_label = f'{vecto_version}_{date}_{nnnn}_{vecto_hash}'
        dest = os.path.join(_failed_reports_dir, run_label)
        print(f'Copying {len(failed_names)} failed vehicle(s) to: {dest}')
        copy_failed_outputs(failed_names, result_dir, dest)

    ref_report = get_latest_release_report(_reports_cache)
    if ref_report and ref_report != output_path:
        generate_delta_report(ref_report, output_path)
        append_delta_to_report(ref_report, output_path)
    else:
        print('No reference report found - skipping delta report.')

    # Intermediate Delta: always compare against whatever MR report is available.
    mr_report = get_latest_mr_report(_mr_reports_cache)
    if mr_report and mr_report != output_path:
        generate_delta_report(mr_report, output_path)
        append_delta_to_report(mr_report, output_path, sheet_suffix='_MR')
    else:
        print('No MR report found - skipping Intermediate Delta.')


if __name__ == '__main__':
    main()
