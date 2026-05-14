#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
B14 - Extract HTCondor result archives into XX_RESULTS.

Steps:
  1. Move all *_out.zip files from the current working directory into
     <test_pool_path>/Z00_VECTO_OUTPUTS.
  2. Unpack each archive into <test_pool_path>/XX_RESULTS, reconstructing
     the original directory tree from the flat output filename.
"""

import argparse
import os
import shutil
from pathlib import Path

from FUNCTIONS.REPORTS import get_vecto_version

parser = argparse.ArgumentParser(
    description='Extract HTCondor result archives into XX_RESULTS in the current working directory.'
)
parser.add_argument(
    'vecto_root',
    help='Path to the VECTO installation directory. Used to determine the version subfolder.',
)
parser.add_argument(
    '--force-rerun',
    action='store_true',
    default=False,
    help='Overwrite already-extracted result directories. Default: skip existing.'
)
args = parser.parse_args()

force_rerun = args.force_rerun
vecto_version = get_vecto_version(args.vecto_root)

storage_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS')
extract_dir = os.path.join(os.getcwd(), 'XX_RESULTS')

Path(storage_dir).mkdir(parents=True, exist_ok=True)
Path(extract_dir).mkdir(parents=True, exist_ok=True)

# --- Step 1: Move *_out.zip files from cwd into Z00_VECTO_OUTPUTS/<version>/ ---
version_storage = os.path.join(storage_dir, vecto_version)
Path(version_storage).mkdir(parents=True, exist_ok=True)

for filename in os.listdir('.'):
    if not filename.endswith('_out.zip'):
        continue
    dest = os.path.join(version_storage, filename)
    shutil.move(filename, dest)
    print(f'Moved: {filename} -> {version_storage}')

# --- Step 2: Unpack each archive into XX_RESULTS/<version>/ ---
for filename in os.listdir(version_storage):
    if not filename.endswith('_out.zip'):
        continue
    zip_outfile = os.path.join(version_storage, filename)

    # Reconstruct the directory path from the flat filename:
    # '__' separators become path separators.
    rel_path = filename.replace('_out.zip', '')
    rel_path = rel_path.replace('__', os.sep)

    outpath = os.path.join(extract_dir, vecto_version, rel_path)

    if not os.path.exists(outpath) or force_rerun:
        print(f'Extracting: {outpath}')
        Path(outpath).mkdir(parents=True, exist_ok=True)
        shutil.unpack_archive(zip_outfile, outpath)

print('Done.')
