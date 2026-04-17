# -*- coding: utf-8 -*-
"""
00_Extract_Missing_Vehicles.py

Compare the vehicles present in the test pool against the vehicles recorded in
the latest OR output report.  Write the input directory paths of any vehicles
that are missing from the report to an output file so they can be submitted to
HTCondor for execution.

Implements the "Case 2" check described in fuego_server_concept.md:
  run only new/un-run vehicles against the current VECTO version.
"""

import argparse
import os
import re
import sys

import pandas as pd

from FUNCTIONS.REPORTS import (
    get_default_report_dir,
    get_latest_vecto_release,
    get_latest_vecto_releases_by_type,
)
from FUNCTIONS.VECTO import get_vecto_git_root

# ---------------------------------------------------------------------------
# Report filename regex — matches OR_{Version}_{Date}_{NNNN}_{Hash}.xlsx
# ---------------------------------------------------------------------------
_REPORT_RE = re.compile(r'^OR_(.+)_\d{6}_(\d{4})_[0-9a-f]+\.xlsx$')


# ---------------------------------------------------------------------------
# Pool
# ---------------------------------------------------------------------------

def get_vehicle_ids_from_pool(test_pool_path: str) -> set[str]:
    """Return the set of vehicle IDs present in the test pool.

    Walks <test_pool_path>/A01_INPUT_FILES for input.zip files and derives
    each vehicle's unique ID using the same logic as B32 lines 67-71:
        rel   = os.path.normpath(os.path.relpath(input_dir, input_root))
        id    = rel.replace(os.sep, '__')
    """
    input_root = os.path.join(test_pool_path, 'A01_INPUT_FILES')
    if not os.path.isdir(input_root):
        raise FileNotFoundError(f'A01_INPUT_FILES not found under: {test_pool_path}')

    ids = set()
    for root, _dirs, files in os.walk(input_root):
        if 'input.zip' in files:
            rel = os.path.normpath(os.path.relpath(root, input_root))
            vehicle_id = rel.replace(os.sep, '__').replace(' ', '_')
            ids.add(vehicle_id)
    return ids


# ---------------------------------------------------------------------------
# Report
# ---------------------------------------------------------------------------

def find_latest_report(reports_cache: str, vecto_root: str) -> str | None:
    """Return the path of the most relevant OR summary report in reports_cache.

    Search order (matches C01.get_latest_release_report):
      1. Latest published Release/v* git tag
      2. Latest official, then RC, then DEV release tag
      3. Fallback: report with the highest NNNN in the cache
    Returns None if the cache directory is empty or missing.
    """
    if not os.path.isdir(reports_cache):
        return None

    def _best_for_version(tag_version: str) -> str | None:
        best_nnnn, best_path = -1, None
        for fname in os.listdir(reports_cache):
            m = _REPORT_RE.match(fname)
            if not m or m.group(1) != tag_version:
                continue
            nnnn_val = int(m.group(2))
            if nnnn_val > best_nnnn:
                best_nnnn, best_path = nnnn_val, os.path.join(reports_cache, fname)
        return best_path

    git_root = get_vecto_git_root(vecto_root) or vecto_root
    latest_tag = get_latest_vecto_release(git_root)
    by_type = get_latest_vecto_releases_by_type(git_root)

    candidates = []
    if latest_tag:
        candidates.append(latest_tag)
    for key in ('official', 'rc', 'dev'):
        tag = by_type.get(key)
        if tag and tag not in candidates:
            candidates.append(tag)

    for tag in candidates:
        result = _best_for_version(tag.removeprefix('Release/v'))
        if result:
            return result

    # Fallback: highest NNNN regardless of version
    best_nnnn, best_path = -1, None
    for fname in os.listdir(reports_cache):
        m = _REPORT_RE.match(fname)
        if not m:
            continue
        nnnn_val = int(m.group(2))
        if nnnn_val > best_nnnn:
            best_nnnn, best_path = nnnn_val, os.path.join(reports_cache, fname)
    return best_path


def get_vehicle_ids_from_report(report_path: str) -> set[str]:
    """Return the set of Test_Name values from the Success sheet of an OR report."""
    df = pd.read_excel(report_path, sheet_name='Success', index_col=0)
    return set(str(name) for name in df.index)


# ---------------------------------------------------------------------------
# Comparison
# ---------------------------------------------------------------------------

def compute_missing_vehicles(pool_ids: set[str], report_ids: set[str]) -> list[str]:
    """Return sorted list of vehicle IDs present in the pool but not the report."""
    return sorted(pool_ids - report_ids)


def resolve_input_dirs(missing_ids: list[str], test_pool_path: str) -> list[str]:
    """Convert vehicle IDs back to absolute input_dir paths under A01_INPUT_FILES."""
    input_root = os.path.join(test_pool_path, 'A01_INPUT_FILES')
    return [
        os.path.join(input_root, vid.replace('__', os.sep))
        for vid in missing_ids
    ]


# ---------------------------------------------------------------------------
# Output
# ---------------------------------------------------------------------------

def write_output(
    input_dirs: list[str],
    output_path: str,
    pool_total: int,
    report_total: int,
) -> None:
    """Write missing input_dir paths to output_path and print a summary."""
    with open(output_path, 'w') as f:
        for path in input_dirs:
            f.write(path + '\n')

    print(f'Pool vehicles   : {pool_total}')
    print(f'Report vehicles : {report_total}')
    print(f'Missing vehicles: {len(input_dirs)}')
    if input_dirs:
        print(f'Written to      : {output_path}')
    else:
        print('No missing vehicles — output file is empty.')


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Find vehicles in the test pool that are absent from the latest OR report.'
    )
    parser.add_argument(
        'vecto_root',
        help='Path to the VECTO installation directory (used to resolve git tags).',
    )
    parser.add_argument(
        'test_pool_path',
        help='Root directory of the test pool. Input files must be under <test_pool_path>/A01_INPUT_FILES.',
    )
    parser.add_argument(
        '--reports-cache',
        default=None,
        help='Directory containing OR_*.xlsx report files. Defaults to D:/CI/Reports or C:/CI/Reports.',
    )
    parser.add_argument(
        '--output',
        default='missing_vehicles.txt',
        help='Output file path for missing vehicle input directories. Default: ./missing_vehicles.txt.',
    )
    args = parser.parse_args()

    if not os.path.exists(args.vecto_root):
        raise FileNotFoundError(f'VECTO path does not exist: {args.vecto_root}')
    if not os.path.exists(args.test_pool_path):
        raise FileNotFoundError(f'Test pool path does not exist: {args.test_pool_path}')

    reports_cache = args.reports_cache or get_default_report_dir()

    pool_ids = get_vehicle_ids_from_pool(args.test_pool_path)

    report_path = find_latest_report(reports_cache, args.vecto_root)
    if report_path is None:
        print(f'No OR report found in: {reports_cache}')
        print('All pool vehicles are considered missing.')
        report_ids = set()
    else:
        print(f'Reference report: {report_path}')
        report_ids = get_vehicle_ids_from_report(report_path)

    missing_ids = compute_missing_vehicles(pool_ids, report_ids)
    missing_dirs = resolve_input_dirs(missing_ids, args.test_pool_path)

    write_output(missing_dirs, args.output, len(pool_ids), len(report_ids))
    sys.exit(len(missing_ids))


if __name__ == '__main__':
    main()
