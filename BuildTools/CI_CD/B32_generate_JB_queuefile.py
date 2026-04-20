# -*- coding: utf-8 -*-
"""
B32 - Generate the HTCondor queue file (batch_arguments.txt).

For each combination of VECTO version and input directory found under
test_pool_path/A01_INPUT_FILES, writes one line to batch_arguments.txt in the
format expected by the HTCondor submit script:

    <vecto_root> <input_dir> <output_filename>

When --ref-vecto-root is supplied, missing vehicles (those absent from the
latest OR report for the reference version) are appended as additional jobs
using the reference executable. This enables Case 1 backward-compatibility
delta comparison via C44.
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

# --- Reference version: append jobs for missing vehicles ---
ref_job_args = []
if args.ref_vecto_root:
    print('--- vecto ref root, extracting missing vehicles for reference jobs')
    em = _load_extract_missing()

    reports_cache = args.reports_cache or get_default_report_dir()
    pool_ids = em.get_vehicle_ids_from_pool(test_pool_path)

    ref_report = em.find_latest_report(reports_cache, args.ref_vecto_root)
    if ref_report is None:
        print(f'No OR report found in {reports_cache} for reference version - treating all pool vehicles as missing.')
        ref_report_ids = set()
    else:
        print(f'Reference OR report: {ref_report}')
        ref_report_ids = em.get_vehicle_ids_from_report(ref_report)

    missing_ids = em.compute_missing_vehicles(pool_ids, ref_report_ids)
    missing_dirs = em.resolve_input_dirs(missing_ids, test_pool_path)

    print(f'Missing from reference report: {len(missing_ids)} vehicle(s)')

    ref_version = get_vecto_version(args.ref_vecto_root)
    ref_result_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS', ref_version)
    os.makedirs(ref_result_dir, exist_ok=True)
    ref_existing = set(os.listdir(ref_result_dir))

    for input_dir in missing_dirs:
        rel = os.path.normpath(os.path.relpath(input_dir, input_root))
        output_filename = rel.replace(os.sep, '__')
        if not force_rerun and output_filename + '_out.zip' in ref_existing:
            continue
        ref_job_args.append((args.ref_vecto_root, input_dir, output_filename))

    if args.no_hev:
        ref_job_args = [a for a in ref_job_args if 'HEV' not in a[2]]

    print(f'Reference version jobs queued: {len(ref_job_args)}')

# Write queue file
all_jobs = job_args + ref_job_args
queue_file = 'batch_arguments.txt'
with open(queue_file, 'w') as f:
    for arg in all_jobs:
        f.write(f'{arg[0]} {arg[1]} {arg[2]}\n')

print(f'Queue file written: {queue_file}')
print(f'Total jobs queued: {len(all_jobs)} ({len(job_args)} current + {len(ref_job_args)} reference)')
