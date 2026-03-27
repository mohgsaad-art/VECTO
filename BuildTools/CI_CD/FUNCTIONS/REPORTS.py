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
    return result.stdout.decode('utf-8').strip() or 'unknown'


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
