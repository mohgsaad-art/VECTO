#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
B14 - Extract HTCondor result archives into XX_RESULTS.

Steps:
  1. Move matching *_out.zip files from the current working directory into
     <test_pool_path>/Z00_VECTO_OUTPUTS/<version>.
  2. Unpack each archive into <test_pool_path>/XX_RESULTS/<version>,
     reconstructing the original directory tree from the flat output filename.

HTCondor drops every job's output zip into the same working directory
regardless of which VECTO executable produced it. B12 suffixes reference-
and MR-version job filenames with '_REF'/'_MR' so --suffix here can pick out
just the files belonging to the version being extracted, instead of one
invocation sweeping up (and mislabeling) every version's results at once.
"""

import argparse
import os
import shutil
from pathlib import Path

from FUNCTIONS.REPORTS import get_vecto_version

_KNOWN_SUFFIXES = ('_REF', '_MR')


def matches_suffix(filename: str, suffix: str) -> bool:
    """True if filename (a '*_out.zip' name) belongs to this run: suffix=''
    matches only unsuffixed files, '_REF'/'_MR' matches only that suffix."""
    if not filename.endswith('_out.zip'):
        return False
    stem = filename[: -len('_out.zip')]
    if suffix:
        return stem.endswith(suffix)
    return not stem.endswith(_KNOWN_SUFFIXES)


def move_outputs(cwd: str, version_storage: str, suffix: str) -> list[str]:
    """Move every '*_out.zip' in cwd matching suffix into version_storage.
    Returns the list of filenames moved."""
    Path(version_storage).mkdir(parents=True, exist_ok=True)
    moved = []
    for filename in os.listdir(cwd):
        if not matches_suffix(filename, suffix):
            continue
        shutil.move(os.path.join(cwd, filename), os.path.join(version_storage, filename))
        moved.append(filename)
    return moved


def extract_outputs(version_storage: str, extract_dir: str, force_rerun: bool = False) -> list[str]:
    """Unpack every '*_out.zip' in version_storage into extract_dir,
    reconstructing the result directory tree from the flat filename (with
    any job-type suffix stripped). Returns the output paths (including ones
    skipped because they already existed)."""
    outpaths = []
    for filename in os.listdir(version_storage):
        if not filename.endswith('_out.zip'):
            continue
        zip_outfile = os.path.join(version_storage, filename)

        rel_path = filename[: -len('_out.zip')]
        for known_suffix in _KNOWN_SUFFIXES:
            rel_path = rel_path.removesuffix(known_suffix)
        rel_path = rel_path.replace('__', os.sep)

        outpath = os.path.join(extract_dir, rel_path)
        outpaths.append(outpath)

        if not os.path.exists(outpath) or force_rerun:
            Path(outpath).mkdir(parents=True, exist_ok=True)
            shutil.unpack_archive(zip_outfile, outpath)

    return outpaths


def main() -> None:
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
    parser.add_argument(
        '--suffix',
        default='',
        choices=['', '_REF', '_MR'],
        help="Only pick up output zips carrying this job-type suffix (added by B12 for "
             "--ref-vecto-root/MR jobs). Default '' picks up only unsuffixed (current-version) results.",
    )
    args = parser.parse_args()

    vecto_version = get_vecto_version(args.vecto_root)

    storage_dir = os.path.join(os.getcwd(), 'Z00_VECTO_OUTPUTS')
    extract_dir = os.path.join(os.getcwd(), 'XX_RESULTS')

    version_storage = os.path.join(storage_dir, vecto_version)
    version_extract_dir = os.path.join(extract_dir, vecto_version)

    moved = move_outputs(os.getcwd(), version_storage, args.suffix)
    for filename in moved:
        print(f'Moved: {filename} -> {version_storage}')

    outpaths = extract_outputs(version_storage, version_extract_dir, args.force_rerun)
    for outpath in outpaths:
        print(f'Extracted: {outpath}')

    print('Done.')


if __name__ == '__main__':
    main()
