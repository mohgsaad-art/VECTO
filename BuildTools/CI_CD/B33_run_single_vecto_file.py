# -*- coding: utf-8 -*-
"""
B33 — Run a single VECTO simulation inside an HTCondor worker node.

This script is the HTCondor equivalent of B02's run_individual_file. Each
worker node:
  1. Optionally unpacks the VECTO executable archive (if a .zip is given).
  2. Unpacks the input archive into a scratch working directory.
  3. Locates the runnable input file (.xml or .vecto).
  4. Runs VECTO and writes a .log file alongside the result.
  5. Archives the output and moves it to the submission directory.

Called by the HTCondor submit script with three positional arguments:
    python B33_run_single_vecto_file.py <vecto_root> <input_archive> <output_filename>
"""

import argparse
import os
import shutil
import subprocess
import time
from pathlib import Path

from FUNCTIONS.REPORTS import get_vecto_version

# ---------------------------------------------------------------------------
# Argument parsing
# ---------------------------------------------------------------------------

parser = argparse.ArgumentParser(
    description='Run a single VECTO simulation job on an HTCondor worker node.'
)
parser.add_argument(
    'vecto_root',
    help='Path to the VECTO installation directory, or a .zip archive of it.'
)
parser.add_argument(
    'input_archive',
    help='Path to the zipped input files (input.zip).'
)
parser.add_argument(
    'output_filename',
    help='Output archive filename (without extension). Used to name the result .zip.'
)
args = parser.parse_args()

vecto_root = args.vecto_root
input_archive = args.input_archive
output_filename = args.output_filename

# ---------------------------------------------------------------------------
# Platform setup
# ---------------------------------------------------------------------------

vecto_cmd_local = 'vectocmd.dll' if os.name == 'posix' else 'vectocmd.exe'

# ---------------------------------------------------------------------------
# Helper
# ---------------------------------------------------------------------------

def run_individual_file(vectofile_fpath, vecto_cmd_path, vecto_cmd_option, log=True):
    """Run a single VECTO input file and write a .log. Returns (path, runtime)."""
    job_ext = os.path.splitext(vectofile_fpath)[-1]

    if os.name == 'posix':
        job_cmd = 'dotnet "{vectopath}" {vecto_cmd_option} "{vectojob}"'
    else:
        job_cmd = '"{vectopath}" {vecto_cmd_option} "{vectojob}"'

    job_cmd = job_cmd.format(
        vectopath=vecto_cmd_path,
        vectojob=vectofile_fpath,
        vecto_cmd_option=vecto_cmd_option,
    )

    st = time.time()
    res = subprocess.run(job_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)
    et = time.time()

    txt = res.stdout.decode('utf-8') + f'\nRunning Time : \t{et - st}s.'

    if log:
        logpath = vectofile_fpath.replace(job_ext, '.log')
        with open(logpath, 'w') as f:
            f.write(job_cmd + '\n\n')
            f.write(txt)

    return vectofile_fpath, et - st


# ---------------------------------------------------------------------------
# Step 1 — Resolve VECTO executable directory
# ---------------------------------------------------------------------------

scratch_dir = os.environ.get('_CONDOR_SCRATCH_DIR', '.')

if vecto_root.endswith('.zip'):
    vecto_dir = os.path.join(scratch_dir, 'VECTO_EXE', Path(vecto_root).stem)
    if not os.path.exists(vecto_dir):
        Path(vecto_dir).mkdir(parents=True, exist_ok=True)
        shutil.unpack_archive(vecto_root, vecto_dir)
else:
    vecto_dir = vecto_root

print(f'vecto_dir: {vecto_dir}')

vecto_version = get_vecto_version(vecto_dir)
print(f'vecto_version: {vecto_version}')

# ---------------------------------------------------------------------------
# Step 2 — Unpack input archive into a working directory
# ---------------------------------------------------------------------------

work_dir = os.path.join(scratch_dir, 'OUTPUTS', vecto_version, output_filename)
Path(work_dir).mkdir(parents=True, exist_ok=True)
print(f'work_dir: {work_dir}')

shutil.unpack_archive(input_archive, work_dir)

# ---------------------------------------------------------------------------
# Step 3 — Locate the VECTO executable
# ---------------------------------------------------------------------------

vecto_cmds = [
    os.path.join(root, f)
    for root, dummy, files in os.walk(vecto_dir)
    for f in files
    if f == vecto_cmd_local
]
vecto_cmds.sort()

if not vecto_cmds:
    raise FileNotFoundError(f'Cannot find {vecto_cmd_local} under {vecto_dir}')

vecto_cmd = vecto_cmds[0]

# ---------------------------------------------------------------------------
# Step 4 — Locate the runnable input file
# ---------------------------------------------------------------------------

file_list_RUNABLE = [
    os.path.join(root, f)
    for root, dummy, files in os.walk(work_dir)
    for f in files
    if (f.endswith('.xml') and 'RSLT' not in f and 'VIF' not in f) or f.endswith('.vecto')
]

dir_with_vecto_file = [os.path.dirname(f) for f in file_list_RUNABLE if f.endswith('.vecto')]

file_list_TORUN = [
    f for f in file_list_RUNABLE
    if os.path.dirname(f) not in dir_with_vecto_file or f.endswith('.vecto')
]

if not file_list_TORUN:
    raise FileNotFoundError(f'No runnable input file found in {work_dir}')

file2run = file_list_TORUN[0]

# ---------------------------------------------------------------------------
# Step 5 — Run VECTO
# ---------------------------------------------------------------------------

vecto_cmd_option = '-nv -t'
if 'ENG_mode' in file2run or 'ENGmode' in file2run:
    vecto_cmd_option += ' -eng'

print(f'Running: {file2run}')
run_individual_file(
    vectofile_fpath=file2run,
    vecto_cmd_path=vecto_cmd,
    vecto_cmd_option=vecto_cmd_option,
    log=True,
)

# Remove intermediate .vmod files generated for ENG mode
for root, dummy, files in os.walk(Path(file2run).parent):
    for f in files:
        if f.lower().endswith('.vmod'):
            os.remove(os.path.join(root, f))

# ---------------------------------------------------------------------------
# Step 6 — Archive and move result into <scratch_dir>/<version>/
# ---------------------------------------------------------------------------

version_dir = os.path.join(scratch_dir, vecto_version)
Path(version_dir).mkdir(parents=True, exist_ok=True)

arch_stem = Path(output_filename).stem
arch_ext = Path(output_filename).suffixes[-1].lstrip('.') if Path(output_filename).suffixes else 'zip'

print(f'Archiving results: {arch_stem}.{arch_ext} from {work_dir}')
shutil.make_archive(arch_stem, arch_ext, root_dir=work_dir)
shutil.move(f'{arch_stem}.{arch_ext}', version_dir)

print(f'Result placed in: {version_dir}')
print('Done.')
