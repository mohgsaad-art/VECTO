# -*- coding: utf-8 -*-
"""B03 - Submit VECTO test cases to HTCondor and wait for completion.

Steps:
  1. Generate the HTCondor queue file from the test pool (calls B32).
  2. Patch the submit script template with the queue name.
  3. Submit the job to HTCondor via condor_submit.
  4. Poll condor_q every 30 s until the queue is empty (max 10 h).
  5. Extract result files from the worker output archives.
"""

import argparse
import subprocess
import time
import os

SUBMIT_TEMPLATE = 'B31_htc_submit_run_VECTO_AD.sh'
SUBMIT_GENERATED = 'htc_submit_run_VECTO_generated.sh'

POLL_INTERVAL_SECONDS = 30
MAX_WAIT_SECONDS = 10 * 3600  # 10 hours

parser = argparse.ArgumentParser(
    description='Submit VECTO test cases to HTCondor and wait for completion.'
)
parser.add_argument(
    'vecto_root',
    help='Path to the VECTO installation directory (passed through to B32 and B33).'
)
parser.add_argument(
    'test_pool_path',
    help='Root directory of the test pool. Input files are expected directly under <test_pool_path>/<vehicle>/input.zip.'
)
parser.add_argument(
    '--queue-name',
    default=None,
    help='HTCondor queue name used to identify this submission. Defaults to the basename of vecto_root.'
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
    help='Directory containing OR_*.xlsx report files used to detect missing vehicles for the reference version.'
)
args = parser.parse_args()

if not os.path.exists(args.vecto_root):
    raise FileNotFoundError(f'VECTO path does not exist: {args.vecto_root}')
if not os.path.exists(args.test_pool_path):
    raise FileNotFoundError(f'Test pool path does not exist: {args.test_pool_path}')

vecto_root = args.vecto_root
test_pool_path = args.test_pool_path
QUEUE_NAME = args.queue_name or os.path.basename(os.path.normpath(vecto_root))


def run(cmd):
    """Run a shell command and return its stdout as a string."""
    result = subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)
    return result.stdout.decode('utf-8')


# --- Step 1: Generate the HTCondor queue file ---
print('Generating queue file...')
print(f'--- vecto ref root: "{args.ref_vecto_root}"')
_ref_arg = f' --ref-vecto-root "{args.ref_vecto_root}"' if args.ref_vecto_root else ''
_cache_arg = f' --reports-cache "{args.reports_cache}"' if args.reports_cache else ''
output32 = run(f'python B32_generate_JB_queuefile.py "{vecto_root}" "{test_pool_path}"{_ref_arg}{_cache_arg}')
print(output32)

# --- Step 2: Patch the submit template with the queue name ---
print(f'Patching submit template -> {SUBMIT_GENERATED}')
with open(SUBMIT_TEMPLATE) as f:
    content = f.read()

content = content.replace('LEGENT_pyjeo_vecto', QUEUE_NAME)

with open(SUBMIT_GENERATED, 'w') as f:
    f.write(content)

# --- Step 3: Submit the job to HTCondor ---
print(f'Submitting: condor_submit {SUBMIT_GENERATED}')
output = run(f'condor_submit {SUBMIT_GENERATED}')
print(output)

# --- Step 4: Poll condor_q until the queue is empty ---
print('Waiting for HTCondor jobs to complete...')
t_start = time.time()
t_next_check = time.time() + POLL_INTERVAL_SECONDS

while time.time() - t_start < MAX_WAIT_SECONDS:
    if time.time() >= t_next_check:
        queue_status = run('condor_q')
        print(queue_status)

        if QUEUE_NAME not in queue_status:
            print('Queue is empty - all jobs finished.')
            break

        t_next_check = time.time() + POLL_INTERVAL_SECONDS

# --- Step 5: Extract result files ---
print('Extracting result files...')
run('python B34_extract_results.py')

print('Done.')
