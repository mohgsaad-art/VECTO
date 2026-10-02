# -*- coding: utf-8 -*-
"""
Shared utilities for CI/CD report generation (C01, C02, C03).
"""

import glob
import os
import re
import subprocess

from FUNCTIONS.VECTO import get_vecto_git_root


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
    return result.stdout.decode('utf-8').strip() or 'unknown'


def get_default_report_dir():
    """Return the default Reports directory: D:/Reports if D:/ exists, otherwise C:/Reports."""
    drive = 'D:' if os.path.exists('D:/') else 'C:'
    default_report_path = os.path.join(drive, 'CI', 'Reports')
    print(f'No --report-dir specified, using default: {default_report_path}')
    return default_report_path


def _fetch_release_tags(vecto_root):
    """Fetch from remote and return all Release/v* tags sorted by creation date (newest first)."""
    subprocess.run(
        f'git -C "{vecto_root}" fetch --tags',
        stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, shell=True
    )
    result = subprocess.run(
        f'git -C "{vecto_root}" tag --sort=-creatordate',
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True
    )
    _tag_re = re.compile(r'^Release/v\d+\.\d+\.\d+(-RC|-DEV)?$')
    return [t for t in result.stdout.decode('utf-8').splitlines() if _tag_re.match(t)]


def get_latest_vecto_releases_by_type(vecto_root):
    """Return the latest official, RC, and DEV release tag names from the VECTO git repo.

    Returns a dict with keys 'official', 'rc', 'dev', each being the full tag
    name (e.g. 'Release/v5.1.1') of the highest version in that category,
    or None if no tag of that type exists.
    """
    tags = _fetch_release_tags(vecto_root)

    _tag_re = re.compile(r'^Release/v(\d+)\.(\d+)\.(\d+)(-RC|-DEV)?$')

    official, rc, dev = None, None, None

    for tag in tags:
        m = _tag_re.match(tag)
        if not m:
            continue
        version = (int(m.group(1)), int(m.group(2)), int(m.group(3)))
        suffix = m.group(4)  # '-RC', '-DEV', or None

        if suffix is None:
            if official is None or version > official[0]:
                official = (version, tag)
        elif suffix == '-RC':
            if rc is None or version > rc[0]:
                rc = (version, tag)
        elif suffix == '-DEV':
            if dev is None or version > dev[0]:
                dev = (version, tag)

    return {
        'official': official[1] if official else None,
        'rc':       rc[1]       if rc       else None,
        'dev':      dev[1]      if dev      else None,
    }


def get_latest_vecto_release(vecto_root):
    """Return the most recently published Release/v* tag, regardless of type (official, RC, or DEV)."""
    tags = _fetch_release_tags(vecto_root)
    return tags[0] if tags else None


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


def get_latest_release_report(reports_dir: str, vecto_root: str) -> str | None:
    """Return the path of the OR summary report matching the latest VECTO release tag.

    Tries the most recently published tag first (official, RC, or DEV). Falls back
    through the latest official, RC, and DEV releases in order. Returns the matching
    report with the highest NNNN, or None if no match is found for any release.
    """
    if not os.path.isdir(reports_dir):
        return None

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
