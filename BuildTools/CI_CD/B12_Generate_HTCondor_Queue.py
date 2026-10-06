# -*- coding: utf-8 -*-
"""
B12 - Generate the HTCondor queue file (batch_arguments.txt).

For each combination of VECTO version and input directory found under
input_root (the test_pool_path argument), writes one line to batch_arguments.txt in the
format expected by the HTCondor submit script:

    <vecto_root> <input_dir> <output_filename>

When --ref-vecto-root is supplied, missing vehicles (those absent from the
latest OR report for the reference version) are appended as additional jobs
using the reference executable, with output filenames suffixed '_REF' (or
'_MR' for a <ref-vecto-root>/MR build) so B14 can later route each result
archive to its own VECTO version's folder. This enables Case 1 backward-
compatibility delta comparison via C44.
"""

import argparse
import importlib.util
import os

from FUNCTIONS.REPORTS import get_default_report_dir, get_vecto_version


def _load_extract_missing():
    """Dynamically import 00_Extract_Missing_Vehicles (name starts with digit)."""
    here = os.path.dirname(os.path.abspath(__file__))
    spec = importlib.util.spec_from_file_location(
        'extract_missing',
        os.path.join(here, '00_Extract_Missing_Vehicles.py'),
    )
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def get_missing_ids(em, test_pool_path, ref_vecto_root, reports_cache, release_tag=None):
    """Return the set of vehicle IDs present in the pool but absent from the latest reference OR report."""
    pool_ids = em.get_vehicle_ids_from_pool(test_pool_path)

    print(f'Reports cache: {reports_cache}')
    print(f'Reference VECTO root: {ref_vecto_root}')
    ref_report = em.find_latest_report(reports_cache, ref_vecto_root, release_tag)
    if ref_report is None:
        print(f'No OR report found in {reports_cache} for reference version - treating all pool vehicles as missing.')
        ref_report_ids = set()
    else:
        print(f'Reference OR report: {ref_report}')
        ref_report_ids = em.get_vehicle_ids_from_report(ref_report)

    return em.compute_missing_vehicles(pool_ids, ref_report_ids)


parser = argparse.ArgumentParser(
    description='Generate the HTCondor queue file (batch_arguments.txt) from the test pool.'
)
parser.add_argument(
    'vecto_root',
    help='Path to the VECTO installation directory (e.g. A00_VECTO/2026_02_12-VECTO-5.1.1-DEPLOY).'
)
parser.add_argument(
    'test_pool_path',
    help='Root directory of the test pool. Input files are expected under <test_pool_path>/A01_INPUT_FILES.'
)
parser.add_argument(
    '--force-rerun',
    action='store_true',
    default=False,
    help='Include jobs whose output archive already exists in Z00_VECTO_OUTPUTS. Default: skip already-completed jobs.'
)
parser.add_argument(
    '--no-hev',
    action='store_true',
    default=False,
    help='Exclude HEV vehicles from the job list.'
)
parser.add_argument(
    '--ref-vecto-root',
    default=None,
    help='Path to the reference (previous official) VECTO installation directory. '
         'When supplied, missing vehicles are also queued against this version for delta comparison.'
)
parser.add_argument(
    '--reports-cache',
    default=None,
    help='Directory containing OR_*.xlsx report files used to detect missing vehicles for the reference version. '
         'Defaults to the standard CI Reports directory.'
)
parser.add_argument(
    '--release-tag',
    default=None,
    help='Release tag (e.g. Release/v5.1.4-DEV) whose OR report is used as the reference report. '
         'Defaults to looking up the release tags in the git repository.'
)
args = parser.parse_args()

if not os.path.exists(args.vecto_root):
    raise FileNotFoundError(f'VECTO path does not exist: {args.vecto_root}')
if not os.path.exists(args.test_pool_path):
    raise FileNotFoundError(f'Test pool path does not exist: {args.test_pool_path}')
if args.ref_vecto_root and not os.path.exists(args.ref_vecto_root):
    raise FileNotFoundError(f'Reference VECTO path does not exist: {args.ref_vecto_root}')

vecto_root = args.vecto_root
test_pool_path = args.test_pool_path
force_rerun = args.force_rerun
ref_vecto_root = args.ref_vecto_root

# Collect input directories
input_root = test_pool_path

file_list_TORUN = [
    os.path.join(root, file)
    for root, dummy, files in os.walk(input_root)
    for file in files
    if file == 'input.zip'
]
input_dirs = [os.path.dirname(f) for f in file_list_TORUN]

# Build (vecto_root, input_dir, output_filename) combinations for current version
job_args = []

for input_dir in input_dirs:
    # Derive the output name from the path relative to the input root,
    # using '__' as separator to keep it a flat filename.
    rel = os.path.normpath(os.path.relpath(input_dir, input_root))
    output_filename = rel.replace(os.sep, '__')
    job_args.append((vecto_root, input_dir, output_filename))

# Skip already-completed jobs (unless --force-rerun)
if not force_rerun:
    vecto_version = get_vecto_version(vecto_root)
    result_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS', vecto_version)
    os.makedirs(result_dir, exist_ok=True)
    existing = set(os.listdir(result_dir))
    job_args = [a for a in job_args if a[2] + '_out.zip' not in existing]

# Exclude HEV if requested
if args.no_hev:
    job_args = [a for a in job_args if 'HEV' not in a[2]]

def _build_missing_job_args(em, vecto_root, test_pool_path, reports_cache, input_root, force_rerun, no_hev, suffix,
                            release_tag=None):
    """Return job-arg tuples for vehicles missing from the given vecto_root's
    latest OR report. suffix ('_REF' or '_MR') is appended to each output
    filename so B14 can later tell which VECTO version produced it."""
    missing_ids = get_missing_ids(em, test_pool_path, vecto_root, reports_cache, release_tag)
    missing_dirs = em.resolve_input_dirs(missing_ids, test_pool_path)
    print(f'Identified {len(missing_ids)} missing vehicle(s) for VECTO version at {vecto_root}')

    version = get_vecto_version(vecto_root)
    result_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS', version)
    os.makedirs(result_dir, exist_ok=True)
    existing = set(os.listdir(result_dir))

    job_list = []
    for input_dir in missing_dirs:
        rel = os.path.normpath(os.path.relpath(input_dir, input_root))
        output_filename = rel.replace(os.sep, '__') + suffix
        if not force_rerun and output_filename + '_out.zip' in existing:
            continue
        print(f'Queuing job for missing vehicle: {output_filename}')
        job_list.append((vecto_root, input_dir, output_filename))

    if no_hev:
        job_list = [a for a in job_list if 'HEV' not in a[2]]

    return job_list, len(missing_ids)


# --- Reference version: append jobs for missing vehicles ---
ref_job_args = []
mr_job_args = []
if args.ref_vecto_root:
    em = _load_extract_missing()
    reports_cache = args.reports_cache or get_default_report_dir()

    print('--- ref-vecto-root supplied, extracting missing vehicles for reference jobs')
    ref_job_args, ref_missing_count = _build_missing_job_args(
        em, args.ref_vecto_root, test_pool_path,
        reports_cache, input_root, force_rerun, args.no_hev, '_REF', args.release_tag,
    )
    print(f'Missing from reference report: {ref_missing_count} vehicle(s)')
    print(f'Reference version jobs queued: {len(ref_job_args)}')

    # --- MR version: same logic, cache at <ref-vecto-root>/MR ---
    mr_vecto_root = os.path.join(args.ref_vecto_root, 'MR')
    if os.path.isdir(mr_vecto_root):
        print('--- MR vecto root found, extracting missing vehicles for MR jobs')
        mr_reports_cache = os.path.join(reports_cache, 'MR')
        mr_job_args, mr_missing_count = _build_missing_job_args(
            em, mr_vecto_root, test_pool_path,
            mr_reports_cache, input_root, force_rerun, args.no_hev, '_MR', args.release_tag,
        )
        print(f'Missing from MR report: {mr_missing_count} vehicle(s)')
        print(f'MR version jobs queued: {len(mr_job_args)}')
    else:
        print(f'No MR vecto root found at {mr_vecto_root} - skipping MR jobs')

# Write queue file
all_jobs = job_args + ref_job_args + mr_job_args
queue_file = 'batch_arguments.txt'
with open(queue_file, 'w') as f:
    for arg in all_jobs:
        f.write(f'{arg[0]} {arg[1]} {arg[2]}\n')

print(f'Queue file written: {queue_file}')
print(f'Total jobs queued: {len(all_jobs)} ({len(job_args)} current + {len(ref_job_args)} reference + {len(mr_job_args)} MR)')
