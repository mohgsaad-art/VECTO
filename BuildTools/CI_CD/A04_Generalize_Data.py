# -*- coding: utf-8 -*-
"""
A04 - Generalize identifying XML element values to generic placeholders.

Walks <data_dir> (non-recursive into archives — loose .xml files only) and
overwrites the text content of every Manufacturer, ManufacturerAddress,
Model, VIN, and CertificationNumber element (namespace-agnostic match) with
a fixed generic value, in place.

Usage:
    python A04_Generalize_Data.py <data_dir>
        [-e | --use-env-var]    Ignore <data_dir> and read the directory to
                                 process from the A04_DATA_DIR environment
                                 variable instead.
"""

import argparse
import os
from pathlib import Path

from lxml import etree

# Element name -> generic replacement value.
GENERIC_VALUES = {
    'Manufacturer': 'Vecto Manufacturer',
    'ManufacturerAddress': 'Vecto Manufacturer Address',
    'Model': 'Vecto Model',
    'VIN': 'Vecto VIN Number',
    'CertificationNumber': 'Vecto Certification Number',
}

ENV_VAR_NAME = 'A04_DATA_DIR'


# ---------------------------------------------------------------------------
# Core logic
# ---------------------------------------------------------------------------

def find_xml_files(data_dir: Path) -> list[Path]:
    """Walk data_dir and return every .xml file found. Archives are not
    extracted or followed — only loose .xml files are processed."""
    return [
        Path(root) / fname
        for root, _dirs, files in os.walk(data_dir)
        for fname in files
        if fname.lower().endswith('.xml')
    ]


def generalize_xml_file(fpath: Path) -> int:
    """Rewrite every GENERIC_VALUES element's text content in fpath with its
    generic value, in place. Returns the number of elements changed.

    Elements are matched namespace-agnostically. Elements with no text
    content are left untouched (no text node to replace). If the file
    cannot be parsed as XML, it is left untouched and 0 is returned.
    """
    try:
        tree = etree.parse(str(fpath))
    except etree.XMLSyntaxError:
        return 0

    changed = 0
    root = tree.getroot()
    for name, generic_value in GENERIC_VALUES.items():
        for el in root.findall(f'.//{{*}}{name}'):
            if el.text is None:
                continue
            el.text = generic_value
            changed += 1

    if changed:
        tree.write(str(fpath), xml_declaration=True, encoding='utf-8')

    return changed


def generalize_data(data_dir: Path) -> dict[str, int]:
    """Generalize every .xml file found under data_dir. Returns a summary
    dict: {'files_scanned', 'files_changed', 'elements_changed'}."""
    xml_files = find_xml_files(data_dir)

    files_changed = 0
    elements_changed = 0
    for fpath in xml_files:
        changed = generalize_xml_file(fpath)
        if changed:
            files_changed += 1
            elements_changed += changed

    return {
        'files_scanned': len(xml_files),
        'files_changed': files_changed,
        'elements_changed': elements_changed,
    }


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Generalize identifying XML element values to generic placeholders.'
    )
    parser.add_argument(
        'data_dir',
        nargs='?',
        help='Directory to walk for .xml files. Ignored if -e/--use-env-var is passed.',
    )
    parser.add_argument(
        '-e', '--use-env-var',
        action='store_true',
        help=f'Read the directory to process from the {ENV_VAR_NAME} environment '
             'variable instead of the data_dir argument.',
    )
    args = parser.parse_args()

    if args.use_env_var:
        env_value = os.environ.get(ENV_VAR_NAME)
        if not env_value:
            raise SystemExit(f'{ENV_VAR_NAME} environment variable is not set.')
        data_dir = Path(env_value)
    else:
        if not args.data_dir:
            raise SystemExit('data_dir is required unless -e/--use-env-var is passed.')
        data_dir = Path(args.data_dir)

    if not data_dir.is_dir():
        raise FileNotFoundError(f'data_dir not found: {data_dir}')

    print(70 * '=')
    print('A04 - Generalize Data')

    summary = generalize_data(data_dir)
    print(f"  {summary['files_scanned']} XML file(s) scanned")
    print(f"  {summary['files_changed']} XML file(s) changed")
    print(f"  {summary['elements_changed']} element(s) generalized")

    print('Done.')


if __name__ == '__main__':
    main()
