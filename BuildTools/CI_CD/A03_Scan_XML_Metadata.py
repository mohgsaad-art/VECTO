# -*- coding: utf-8 -*-
"""
A03 - Scan extracted test pool XML files for Manufacturer/VIN/CertificationNumber.

Walks <extract_root> (typically A01_EXTRACT), unzipping any .zip files found
along the way (one level - zips nested inside other zips' extracted output are
not followed), and collects a unique, sorted list of Manufacturer, VIN, and
CertificationNumber values from every declaration XML encountered. Writes
three output .txt files, one per element.

Zip archives are extracted to a fresh temporary directory per archive; these
temporary directories are not cleaned up automatically.

Usage:
    python A03_Scan_XML_Metadata.py <extract_root>
        [--output-dir <path>]   Directory for the three output .txt files (default: cwd)
        [--copy-only]           Copy every XML file found into <output-dir>/A03XMLs
                                 instead of parsing them for metadata.
        [--force]               Delete this script's previous outputs in <output-dir>
                                 (Manufacturer.txt, VIN.txt, CertificationNumber.txt,
                                 A03XMLs/) before running.
"""

import argparse
import logging
import os
import shutil
import tempfile
from pathlib import Path

import patoolib
from lxml import etree

from FUNCTIONS.utils import check_archive, extract_archive
from FUNCTIONS.utils_pool import EXTRACT_VERBOSITY

# Silence patool's INFO-level "could not find a 'file' executable" notice - it
# only means mime-type detection falls back to file extension, which is fine
# for the .xml/.zip/.7z/.rar files this script handles.
logging.getLogger('patool').setLevel(logging.WARNING)

TARGET_ELEMENTS = ('Manufacturer', 'VIN', 'CertificationNumber')

# ---------------------------------------------------------------------------
# Core logic
# ---------------------------------------------------------------------------

def find_xml_files(extract_root: Path) -> list[Path]:
    """Walk extract_root, unzip any .zip/.7z/.rar files found (one level),
    and return the full list of .xml file paths (original + extracted)."""
    xml_files = []
    for root, _dirs, files in os.walk(extract_root):
        for fname in files:
            fpath = Path(root) / fname
            if fname.lower().endswith('.xml'):
                xml_files.append(fpath)
            elif patoolib.is_archive(str(fpath)):
                xml_files.extend(_extract_and_find_xml(fpath))
    return xml_files


def _extract_and_find_xml(archive_path: Path) -> list[Path]:
    """Extract a single archive to a fresh temp directory and return the
    .xml files found inside it (non-recursive: zips inside this archive's
    extracted output are not followed)."""
    extract_dir = Path(tempfile.mkdtemp(prefix='a03_scan_'))

    err = check_archive(archive_path, verbosity=EXTRACT_VERBOSITY)
    if err is not None:
        print(f'  WARNING: could not read archive {archive_path}: {err}')
        return []

    err = extract_archive(archive_path, extract_dir, verbosity=EXTRACT_VERBOSITY)
    if err is not None:
        print(f'  WARNING: could not extract {archive_path}: {err}')
        return []

    return [
        Path(sub_root) / sub_fname
        for sub_root, _sub_dirs, sub_files in os.walk(extract_dir)
        for sub_fname in sub_files
        if sub_fname.lower().endswith('.xml')
    ]


def scan_xml_files(xml_files: list[Path]) -> dict[str, set[str]]:
    """Parse every xml file and collect unique values per target element."""
    values: dict[str, set[str]] = {name: set() for name in TARGET_ELEMENTS}
    for fpath in xml_files:
        try:
            tree = etree.parse(str(fpath))
        except etree.XMLSyntaxError as ex:
            # Log only the immediate parent folder name, not the full path.
            safe_ex = re.sub(r'\s*\(.*, line \d+\)\s*$', '', str(ex))
            print(f'  WARNING: failed to parse {fpath.parent.name}: {safe_ex}')
            continue
        for name in TARGET_ELEMENTS:
            for el in tree.findall(f'.//{{*}}{name}'):
                if el.text and el.text.strip():
                    values[name].add(el.text.strip())
    return values


def copy_xml_files(xml_files: list[Path], dest_dir: Path) -> None:
    """Copy every xml file found into dest_dir, without inspecting its content.

    Filenames are prefixed with an incrementing counter to avoid collisions
    between files that share the same name in different source folders.
    """
    dest_dir.mkdir(parents=True, exist_ok=True)
    for i, fpath in enumerate(xml_files, start=1):
        dest_name = f'{i:05d}_{fpath.name}'
        shutil.copy2(fpath, dest_dir / dest_name)
    print(f'  Copied {len(xml_files)} XML file(s) -> {dest_dir}')


def clear_previous_output(output_dir: Path) -> None:
    """Delete this script's own previous outputs from output_dir, if present:
    the three .txt files and the A03XMLs folder. Leaves any other content in
    output_dir untouched."""
    for name in TARGET_ELEMENTS:
        outfile = output_dir / f'{name}.txt'
        if outfile.exists():
            outfile.unlink()
            print(f'  Removed {outfile}')

    xmls_dir = output_dir / 'A03XMLs'
    if xmls_dir.exists():
        shutil.rmtree(xmls_dir)
        print(f'  Removed {xmls_dir}')


def write_output(values: dict[str, set[str]], output_dir: Path) -> None:
    """Write one sorted, deduplicated .txt file per target element."""
    for name, value_set in values.items():
        outfile = output_dir / f'{name}.txt'
        with outfile.open('w', encoding='utf-8') as f:
            for v in sorted(value_set):
                f.write(v + '\n')
        print(f'  {name}: {len(value_set)} unique value(s) -> {outfile}')


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Scan extracted test pool XML files for Manufacturer/VIN/CertificationNumber.'
    )
    parser.add_argument(
        'extract_root',
        help='Root directory to scan (typically A01_EXTRACT). Nested subfolders and .zip files are included.',
    )
    parser.add_argument(
        '--output-dir',
        default='.',
        help='Directory to write the three output .txt files. Default: current directory.',
    )
    parser.add_argument(
        '--copy-only',
        action='store_true',
        help='Copy every XML file found into <output-dir>/A03XMLs instead of parsing '
             'them for Manufacturer/VIN/CertificationNumber.',
    )
    parser.add_argument(
        '--force',
        action='store_true',
        help="Delete this script's previous outputs in <output-dir> (Manufacturer.txt, "
             "VIN.txt, CertificationNumber.txt, A03XMLs/) before running.",
    )
    args = parser.parse_args()

    extract_root = Path(args.extract_root)
    if not extract_root.is_dir():
        raise FileNotFoundError(f'extract_root not found: {extract_root}')

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    print(70 * '=')
    print('A03 - Scan XML Metadata')

    if args.force:
        clear_previous_output(output_dir)

    xml_files = find_xml_files(extract_root)
    print(f'  {len(xml_files)} XML file(s) found')

    if args.copy_only:
        copy_xml_files(xml_files, output_dir / 'A03XMLs')
    else:
        values = scan_xml_files(xml_files)
        write_output(values, output_dir)

    print('Done.')


if __name__ == '__main__':
    main()
