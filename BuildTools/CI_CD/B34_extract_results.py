#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
B34 — Extract HTCondor result archives into XX_RESULTS.

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

parser = argparse.ArgumentParser(
    description='Extract HTCondor result archives into XX_RESULTS in the current working directory.'
)
parser.add_argument(
    '--force-rerun',
    action='store_true',
    default=False,
    help='Overwrite already-extracted result directories. Default: skip existing.'
)
args = parser.parse_args()

force_rerun = args.force_rerun

storage_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS')
extract_dir = os.path.join(os.getcwd(), 'XX_RESULTS')

Path(storage_dir).mkdir(parents=True, exist_ok=True)
Path(extract_dir).mkdir(parents=True, exist_ok=True)

# --- Step 1: Move *_out.zip files from cwd into Z00_VECTO_OUTPUTS ---
out_zips_in_cwd = [f for f in os.listdir('.') if f.endswith('out.zip')]

for filename in out_zips_in_cwd:
    dest = os.path.join(storage_dir, filename)
    shutil.move(filename, dest)
    print(f'Moved: {filename} -> {storage_dir}')

# --- Step 2: Unpack each archive into XX_RESULTS ---
zip_outfiles = [
    os.path.join(storage_dir, f)
    for f in os.listdir(storage_dir)
    if f.endswith('out.zip')
]

for zip_outfile in zip_outfiles:
    # Reconstruct the directory path from the flat filename:
    # '__' separators become path separators, except for the known
    # double-underscore prefixes GEN__DECL_mode and GEN__ENG_mode.
    rel_path = os.path.basename(zip_outfile)
    rel_path = rel_path.replace('_out.zip', '')
    rel_path = rel_path.replace('__', os.sep)
    rel_path = rel_path.replace('GEN' + os.sep + 'DECL_mode', 'GEN__DECL_mode')
    rel_path = rel_path.replace('GEN' + os.sep + 'ENG_mode', 'GEN__ENG_mode')

    outpath = os.path.join(extract_dir, rel_path)

    if not os.path.exists(outpath) or force_rerun:
        print(f'Extracting: {outpath}')
        Path(outpath).mkdir(parents=True, exist_ok=True)
        shutil.unpack_archive(zip_outfile, outpath)

print('Done.')
