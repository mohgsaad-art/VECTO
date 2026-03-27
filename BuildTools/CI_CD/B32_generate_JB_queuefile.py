# -*- coding: utf-8 -*-
"""
B32 — Generate the HTCondor queue file (batch_arguments.txt).

For each combination of VECTO version and input directory found under
test_pool_path/A01_INPUT_FILES, writes one line to batch_arguments.txt in the
format expected by the HTCondor submit script:

    <vecto_name> <input_dir> <output_filename>

This is the HTCondor equivalent of B01: it prepares the list of jobs to run
before submitting them via B03.
"""

import argparse
import os
import subprocess

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
args = parser.parse_args()

if not os.path.exists(args.vecto_root):
    raise FileNotFoundError(f'VECTO path does not exist: {args.vecto_root}')
if not os.path.exists(args.test_pool_path):
    raise FileNotFoundError(f'Test pool path does not exist: {args.test_pool_path}')

vecto_root = args.vecto_root
test_pool_path = args.test_pool_path
force_rerun = args.force_rerun

# Collect input directories
input_root = os.path.join(test_pool_path, 'A01_INPUT_FILES')

file_list_TORUN = [
    os.path.join(root, file)
    for root, dummy, files in os.walk(input_root)
    for file in files
    if file == 'input.zip'
]
input_dirs = [os.path.dirname(f) for f in file_list_TORUN]

# Build (input_dir, output_filename) combinations
job_args = []

for input_dir in input_dirs:
    # Derive the output name from the path relative to the input root,
    # using '__' as separator to keep it a flat filename.
    rel = os.path.normpath(os.path.relpath(input_dir, input_root))
    output_filename = rel.replace(os.sep, '__')
    job_args.append((vecto_root, input_dir, output_filename))

# Skip already-completed jobs (unless --force-rerun)
if not force_rerun:
    result_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS')
    os.makedirs(result_dir, exist_ok=True)
    existing = set(os.listdir(result_dir))
    job_args = [a for a in job_args if a[2] + '_out.zip' not in existing]

# Exclude HEV if requested
if args.no_hev:
    job_args = [a for a in job_args if 'HEV' not in a[2]]

# Write queue file
queue_file = 'batch_arguments.txt'
with open(queue_file, 'w') as f:
    for arg in job_args:
        f.write(f'{arg[0]} {arg[1]} {arg[2]}\n')

print(f'Queue file written: {queue_file}')
print(f'Total jobs queued: {len(job_args)}')
