# -*- coding: utf-8 -*-
"""
Shared utilities for CI/CD report generation (C01, C02, C03).
"""

import glob
import os
import re
import subprocess


def get_vecto_version(vecto_root):
    """Return the VECTO version string by calling vectocmd -V."""
    vecto_cmd_local = 'vectocmd.dll' if os.name == 'posix' else 'vectocmd.exe'
    vecto_cmd = os.path.join(vecto_root, vecto_cmd_local)
    if os.name == 'posix':
        version_cmd = f'dotnet "{vecto_cmd}" -V'
    else:
        version_cmd = f'"{vecto_cmd}" -V'
    result = subprocess.run(version_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)
    output = result.stdout.decode('utf-8')

    match = re.search(r'(\d+\.\d+\.\d+(?:-RC|-DEV)?)', output)
    if match:
        return match.group(1)

    return 'unknown'


def get_vecto_hash(vecto_root):
    """Return the short git commit hash of the VECTO repository."""
    result = subprocess.run(
        f'git -C "{vecto_root}" rev-parse --short HEAD',
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True
    )
    if result.returncode != 0:
        return 'unknown'
    return result.stdout.decode('utf-8').strip() or 'unknown'


def get_default_report_dir():
    """Return the default Reports directory: D:/Reports if D:/ exists, otherwise C:/Reports."""
    drive = 'D:' if os.path.exists('D:/') else 'C:'
    default_report_path = os.path.join(drive, 'CI', 'Reports')
    print(f'No --report-dir specified, using default: {default_report_path}')
    return default_report_path


def get_next_report_number(report_base_dir):
    """Return the next OR report number as a zero-padded 4-digit string."""
    existing = glob.glob(os.path.join(report_base_dir, 'OR_*.xlsx'))
    numbers = []
    for f in existing:
        m = re.search(r'_(\d{4})_', os.path.basename(f))
        if m:
            numbers.append(int(m.group(1)))
    return f'{max(numbers) + 1:04d}' if numbers else '0001'


def find_result_dirs(result_dir: str) -> list[str]:
    """Return leaf directories containing runnable test cases (.xml or .vecto files).

    Directories that contain a .vecto file are bus complete-step cases — they are
    included directly via the .vecto file rather than via their child .xml files.
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


# OR_{Version}[-RC|-DEV]_{YYMMDD}_{NNNN}_{Hash}.xlsx - summary reports only (no _DETAIL/_CRF suffix)
_OR_REPORT_RE = re.compile(
    r'^OR_(?P<ver>\d+\.\d+\.\d+)(?P<sfx>-RC|-DEV)?_(?P<date>\d{6})_(?P<nnnn>\d{4})_[0-9a-f]+\.xlsx$'
)
_RELEASE_TYPE_RANK = {None: 2, '-RC': 1, '-DEV': 0}  # official > RC > DEV


def get_latest_release_report(reports_dir: str, release_tag: str | None = None,
                              allow_prerelease: bool = False) -> str | None:
    """Return the path of the reference OR summary report in reports_dir.

    1. release_tag (e.g. 'Release/v5.1.4-DEV') given and reports exist for that exact
       version: the newest of them (date, then NNNN).
    2. Otherwise rank every report by numeric version, release type (official > RC > DEV),
       date and NNNN, and return the highest. RC/DEV reports are only considered when
       allow_prerelease is True (MR reports cache).
    Returns None if reports_dir is missing or no report qualifies.
    """
    if not os.path.isdir(reports_dir):
        return None

    reports = []
    for fname in os.listdir(reports_dir):
        m = _OR_REPORT_RE.match(fname)
        if not m:
            continue
        key = (
            tuple(int(p) for p in m['ver'].split('.')),
            _RELEASE_TYPE_RANK[m['sfx']],
            int(m['date']),
            int(m['nnnn']),
        )
        reports.append((key, m['ver'] + (m['sfx'] or ''), os.path.join(reports_dir, fname)))

    if release_tag:
        tag_version = release_tag.removeprefix('Release/v')
        tagged = [r for r in reports if r[1] == tag_version]
        if tagged:
            chosen = max(tagged)[2]
            print(f'Reference report (matches release tag {release_tag}): {chosen}')
            return chosen

    ranked = reports if allow_prerelease else [r for r in reports if r[0][1] == _RELEASE_TYPE_RANK[None]]
    if not ranked:
        return None
    chosen = max(ranked)[2]
    print(f'Reference report (highest version/date/NNNN'
          f'{"" if allow_prerelease else ", official releases only"}): {chosen}')
    return chosen
