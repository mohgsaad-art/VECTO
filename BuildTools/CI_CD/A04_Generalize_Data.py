# -*- coding: utf-8 -*-
"""
A04 - Make identifying XML element values confidential.

Walks <data_dir> recursively (every subfolder, any depth) and, in place, for
every CONFIDENTIAL_FIELDS element (namespace-agnostic match):
  - Date elements get DEFAULT_DATE (the file must stay XSD-valid);
  - every other field gets CONFIDENTIAL_VALUE;
  - the `id` attribute of each such element's parent is renamed to
    'Confidential-ID00', 'Confidential-ID01', ... in document order, and any
    `URI="#<old id>"` reference (e.g. a signature's di:Reference) is updated
    to match, so the original id cannot leak through it.

A02 applies the same transformation (anonymize_xml_bytes) to every issue
vehicle XML before zipping it, and names it '<vehicle_type>_confidential.xml'.
Generic vehicles are never touched.

With --process-zips, any .zip file encountered is also unzipped, its .xml
entries generalized the same way, and the archive rewritten in place with
the updated content (one level deep only - a zip found inside another zip
is left untouched). Off by default: without the flag, .zip files are left
alone, matching the previous behavior.

Usage:
    python A04_Generalize_Data.py <data_dir>
        [-e | --use-env-var]    Ignore <data_dir> and read the directory to
                                 process from the A04_DATA_DIR environment
                                 variable instead.
        [--process-zips]        Also unzip, generalize, and re-zip .zip
                                 files found while walking data_dir.
"""

import argparse
import os
import shutil
import tempfile
import zipfile
from pathlib import Path

from lxml import etree

# Element names whose text content is identifying and must be replaced.
CONFIDENTIAL_FIELDS = [
    'VIN',
    'Date',
    'DigestValue',
    'Manufacturer',
    'ManufacturerAddress',
    'Model',
    'AppVersion',
    'CertificationNumber',
    'SimulationToolLicenseNumber',
    'Make',
    'TypeApprovalNumber',
    'VectoLicenseNbr',
    'VehicleTypeApprovalNumber',
]

CONFIDENTIAL_VALUE = 'CONFIDENTIAL'
DEFAULT_DATE = '2020-01-01T08:00:00Z'  # Date is xs:dateTime - must stay a valid date
ID_PREFIX = 'Confidential-ID'
CONFIDENTIAL_SUFFIX = '_confidential.xml'  # file name suffix used by A02 for issue vehicles

ENV_VAR_NAME = 'A04_DATA_DIR'


# ---------------------------------------------------------------------------
# Core logic
# ---------------------------------------------------------------------------

def find_xml_files(data_dir: Path) -> list[Path]:
    """Walk data_dir recursively (every subfolder, any depth) and return
    every loose .xml file found. Archives are not extracted or followed —
    use find_zip_files/generalize_zip_file for those."""
    return [
        Path(root) / fname
        for root, _dirs, files in os.walk(data_dir)
        for fname in files
        if fname.lower().endswith('.xml')
    ]


def find_zip_files(data_dir: Path) -> list[Path]:
    """Walk data_dir recursively (every subfolder, any depth) and return
    every .zip file found."""
    return [
        Path(root) / fname
        for root, _dirs, files in os.walk(data_dir)
        for fname in files
        if fname.lower().endswith('.zip')
    ]


def anonymize_tree(root) -> int:
    """Make every CONFIDENTIAL_FIELDS element under root confidential, in
    place (see module docstring). Returns the number of elements changed.

    Elements are matched namespace-agnostically. Elements with no text
    content are left untouched (no text node to replace), but their parent's
    id is still renamed.
    """
    fields = set(CONFIDENTIAL_FIELDS)
    changed = 0
    parents_with_id = []

    # root.iter() walks in document order, so ids are numbered in that order.
    for el in root.iter():
        if not isinstance(el.tag, str) or etree.QName(el).localname not in fields:
            continue
        if el.text is not None:
            el.text = DEFAULT_DATE if etree.QName(el).localname == 'Date' else CONFIDENTIAL_VALUE
            changed += 1
        parent = el.getparent()
        if parent is not None and 'id' in parent.attrib and parent not in parents_with_id:
            parents_with_id.append(parent)

    renamed_ids = {}
    for i, parent in enumerate(parents_with_id):
        new_id = f'{ID_PREFIX}{i:02d}'
        renamed_ids[parent.attrib['id']] = new_id
        parent.attrib['id'] = new_id

    if renamed_ids:
        for el in root.iter():
            uri = el.attrib.get('URI') if isinstance(el.tag, str) else None
            if uri and uri.startswith('#') and uri[1:] in renamed_ids:
                el.attrib['URI'] = '#' + renamed_ids[uri[1:]]

    return changed


def anonymize_xml_bytes(content: bytes) -> tuple[bytes, int]:
    """Return (confidential content, number of elements changed) for an XML
    document given as bytes. If content cannot be parsed as XML, it is
    returned unchanged with 0."""
    try:
        root = etree.fromstring(content)
    except etree.XMLSyntaxError:
        return content, 0

    changed = anonymize_tree(root)
    return etree.tostring(root, xml_declaration=True, encoding='utf-8'), changed


def generalize_xml_file(fpath: Path) -> int:
    """Make fpath confidential in place (see anonymize_xml_bytes). Returns
    the number of elements changed. If the file cannot be parsed as XML, it
    is left untouched and 0 is returned."""
    content, changed = anonymize_xml_bytes(fpath.read_bytes())

    if changed:
        fpath.write_bytes(content)

    return changed


def generalize_zip_file(fpath: Path) -> tuple[int, int]:
    """Unzip fpath, generalize every .xml entry inside it the same way as
    generalize_xml_file, and rewrite the archive in place with the updated
    content. Non-.xml entries (and any .zip entries - nested archives are
    not followed) are carried over unchanged. Returns
    (files_changed, elements_changed) for the .xml entries inside.

    If fpath cannot be opened as a zip, it is left untouched and (0, 0) is
    returned.
    """
    try:
        with zipfile.ZipFile(fpath) as zf:
            bad_entry = zf.testzip()
            if bad_entry is not None:
                return 0, 0
            names = zf.namelist()
    except (zipfile.BadZipFile, OSError):
        return 0, 0

    files_changed = 0
    elements_changed = 0

    with tempfile.TemporaryDirectory() as tmp:
        tmp_dir = Path(tmp)
        with zipfile.ZipFile(fpath) as zf:
            zf.extractall(tmp_dir)

        for name in names:
            if name.endswith('/'):
                continue  # directory entry
            if not name.lower().endswith('.xml'):
                continue
            changed = generalize_xml_file(tmp_dir / name)
            if changed:
                files_changed += 1
                elements_changed += changed

        if files_changed:
            rebuilt = fpath.with_name(fpath.name + '.rebuilt')
            with zipfile.ZipFile(rebuilt, 'w', zipfile.ZIP_DEFLATED) as zf:
                for name in names:
                    if name.endswith('/'):
                        continue
                    zf.write(tmp_dir / name, arcname=name)
            shutil.move(str(rebuilt), str(fpath))

    return files_changed, elements_changed


def generalize_data(data_dir: Path, process_zips: bool = False) -> dict[str, int]:
    """Generalize every .xml file found under data_dir (recursively, any
    subfolder depth). With process_zips=True, also unzip/generalize/re-zip
    every .zip file found (one level deep - nested zips are left untouched).

    Returns a summary dict: {'files_scanned', 'files_changed',
    'elements_changed', 'zips_scanned', 'zips_changed'}. 'files_changed' and
    'elements_changed' include changes made to .xml entries inside zips;
    'zips_changed' counts archives that were rewritten because at least one
    entry inside them changed.
    """
    xml_files = find_xml_files(data_dir)

    files_changed = 0
    elements_changed = 0
    for fpath in xml_files:
        changed = generalize_xml_file(fpath)
        if changed:
            files_changed += 1
            elements_changed += changed

    zips_scanned = 0
    zips_changed = 0
    if process_zips:
        zip_files = find_zip_files(data_dir)
        zips_scanned = len(zip_files)
        for fpath in zip_files:
            zip_files_changed, zip_elements_changed = generalize_zip_file(fpath)
            if zip_files_changed:
                zips_changed += 1
                files_changed += zip_files_changed
                elements_changed += zip_elements_changed

    return {
        'files_scanned': len(xml_files),
        'files_changed': files_changed,
        'elements_changed': elements_changed,
        'zips_scanned': zips_scanned,
        'zips_changed': zips_changed,
    }


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Make identifying XML element values confidential.'
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
    parser.add_argument(
        '--process-zips',
        action='store_true',
        help='Also unzip, generalize, and re-zip .zip files found while walking '
             'data_dir (one level deep - nested zips are left untouched). '
             'Off by default: .zip files are left alone.',
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

    summary = generalize_data(data_dir, process_zips=args.process_zips)
    print(f"  {summary['files_scanned']} XML file(s) scanned")
    print(f"  {summary['files_changed']} XML file(s) changed")
    print(f"  {summary['elements_changed']} element(s) generalized")
    if args.process_zips:
        print(f"  {summary['zips_scanned']} zip file(s) scanned")
        print(f"  {summary['zips_changed']} zip file(s) changed")

    print('Done.')


if __name__ == '__main__':
    main()
