# -*- coding: utf-8 -*-
"""
A2311 - Generate HTCondor-ready zipped inputs from generic and issue vehicle files.

Merges the work of three scripts into one pass with no intermediate folders:
  - A02: classify generic .vecto/.xml inputs by mode and powertrain
  - A03: scan the Article 10 test pool, deduplicate vehicles by content hash
  - A11: zip each vehicle directory into <output_dir>/<vehicle>/input.zip

Output is fed directly to B03/B32 as <test_pool_path>/To_Run.

Usage:
    python A2311_Generate_Files_To_Run.py <generic_input_dir> <issues_input_dir> <output_dir>
        [--force-repackage]
"""

import argparse
import io
import json
import os
import zipfile
from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path

import pandas as pd
from lxml import etree

from FUNCTIONS.VECTO import (
    get_dict_val_from_keypath,
    get_inputs_path,
    load_vecto,
    replace_string_in_filepath,
)
from FUNCTIONS.utils_generate_vecto_input2run import (
    generate_df_path,
    generate_PRIM_COMP_job,
    generate_PRIMVIF_COMP_job,
    get_REF_vehicles,
    get_vecto_file_data,
    get_vehicles,
    scan_pool,
)
from FUNCTIONS.utils_pool import ISSUE_DESC_FILENAME, timed

# ---------------------------------------------------------------------------
# Constants (from A02)
# ---------------------------------------------------------------------------

_EXCLUDE_SUBSTRINGS = ['_invalid', 'P39_40']
_MODE_DECL = 'GenericDeclaration'
_MODE_ENG = 'GenericEngineering'
_MODE_ART10 = 'Declaration'
_MODE_UNKNOWN = 'UNKNOWN'

# ---------------------------------------------------------------------------
# Data model
# ---------------------------------------------------------------------------

@dataclass
class VehicleManifest:
    """In-memory representation of one vehicle's input files for zipping."""
    dirname: str                                    # output subdir, e.g. "DeclarationMode/ICE/TruckA"
    files: list[tuple[str, bytes]] = field(default_factory=list)  # (name_in_zip, content)


# ---------------------------------------------------------------------------
# Helpers shared with A02 logic
# ---------------------------------------------------------------------------

def _sanitise(path: str) -> str:
    return path.replace(' _', '_').replace('_ ', '_').replace(' ', '_')


def _is_excluded(path: str) -> bool:
    return any(s in path for s in _EXCLUDE_SUBSTRINGS)


def _xml_powertrain(xml_tree) -> str:
    vehicle = xml_tree.find('.//{*}Vehicle')
    if vehicle is None or not vehicle.attrib:
        return 'ICE'
    type_attr = next((v for k, v in vehicle.attrib.items() if 'type' in k), '')
    if 'PEV' in type_attr:
        return 'PEV'
    if 'HEV' in type_attr:
        return 'HEV'
    if 'FCHV' in type_attr:
        return 'FCHV'
    return 'ICE'


def _vecto_mode(inputs: dict, has_xml_refs: bool, vfile: str) -> str:
    if has_xml_refs and 'VTP' not in vfile:
        return _MODE_DECL
    try:
        is_decl = inputs['vecto']['data']['Body']['SavedInDeclMode']
    except (KeyError, TypeError):
        return _MODE_UNKNOWN
    return _MODE_DECL if is_decl else _MODE_ENG


def _vecto_powertrain(inputs: dict, has_xml_refs: bool) -> str:
    lpaths = get_inputs_path(inputs)
    if has_xml_refs:
        for fp in lpaths:
            if 'file' not in fp:
                continue
            fn = get_dict_val_from_keypath(inputs, fp)
            if not fn.endswith('.xml'):
                continue
            dp = fp[:-1] + ['data']
            xml_tree = get_dict_val_from_keypath(inputs, dp)
            return _xml_powertrain(xml_tree)
        return 'ICE'
    flds = [p[-2] for p in lpaths if 'file' in p]
    has_ice = 'EngineFile' in flds
    has_em = any(f in flds for f in ('MotorFile', 'IEPC', 'MotorFile_1', 'IEPC_1'))
    has_fc = any(f in flds for f in ('FuelCellSystemFile', 'FuelCellSystemFile_1'))
    if has_ice and has_em:
        return 'HEV'
    if has_ice:
        return 'ICE'
    if has_em and has_fc:
        return 'FCHV'
    if has_em:
        return 'PEV'
    return 'ICE'


def _strip_common_affixes(strings: list[str]) -> list[str]:
    if len(strings) <= 1:
        return strings
    prefix_len = 0
    for idx in range(1, len(strings[0]) + 1):
        if all(s.startswith(strings[0][:idx]) for s in strings):
            prefix_len = idx
        else:
            break
    suffix_len = 0
    for idx in range(1, len(strings[0]) + 1):
        candidate = strings[0][-idx:]
        if all(candidate in s for s in strings):
            suffix_len = idx
        else:
            break
    suffix = strings[0][-suffix_len:] if suffix_len else ''
    if '.' in suffix:
        suffix = suffix[suffix.index('.'):]
        suffix_len = len(suffix)
    else:
        suffix_len = 0
    return [s[prefix_len: len(s) - suffix_len if suffix_len else None] for s in strings]


def _output_dirname(input_dir: str, input_root: str, mode: str, powertrain: str, suffix: str = '') -> str:
    rel = os.path.normpath(os.path.relpath(input_dir, input_root))
    last = rel.split(os.sep)[-1]
    last = last.replace('GenericVehicle_', '').replace('GenericVehicle', '')
    return _sanitise(os.path.join(mode, powertrain, last + suffix))


def _vecto_inputs_to_bytes(inputs: dict) -> list[tuple[str, bytes]]:
    """Serialise all files in a loaded vecto inputs dict to (filename, bytes) pairs."""
    result = []
    lpaths = get_inputs_path(inputs)
    for fname_path in (p for p in lpaths if 'file' in p):
        data_path = fname_path[:-1] + ['data']
        type_path = fname_path[:-1] + ['type']
        fname = get_dict_val_from_keypath(inputs, fname_path)
        typ = get_dict_val_from_keypath(inputs, type_path)
        data = get_dict_val_from_keypath(inputs, data_path)

        buf = io.BytesIO()
        if typ == 'json':
            content = json.dumps(data, ensure_ascii=False, indent=4).encode('utf-8-sig')
            buf.write(content)
        elif typ == 'xml':
            etree.indent(data, space='\t', level=0)
            data.write(buf, encoding='utf-8', xml_declaration=True, pretty_print=False)
        elif typ == 'csv':
            csv_bytes = data.to_csv(index=False, encoding='utf-8-sig').encode('utf-8-sig')
            buf.write(csv_bytes)
        else:
            continue

        result.append((fname, buf.getvalue()))
    return result


def _job_json_bytes(prim: str, comp: str, generator_fn) -> bytes:
    """Capture generate_PRIM_COMP_job / generate_PRIMVIF_COMP_job output as bytes."""
    import tempfile
    with tempfile.NamedTemporaryFile(suffix='.vecto', delete=False) as f:
        tmp = f.name
    try:
        generator_fn(prim, comp, tmp)
        return Path(tmp).read_bytes()
    finally:
        Path(tmp).unlink(missing_ok=True)


# ---------------------------------------------------------------------------
# Step 1: Generic vehicles (A02 logic)
# ---------------------------------------------------------------------------

def collect_generic_vehicles(input_dir: str) -> list[VehicleManifest]:
    """Classify and collect generic .vecto/.xml inputs as in-memory manifests."""
    manifests: list[VehicleManifest] = []

    vecto_files = [
        os.path.join(root, f)
        for root, _, files in os.walk(input_dir)
        for f in files
        if f.endswith('.vecto') and not _is_excluded(os.path.join(root, f))
    ]

    for vfile in vecto_files:
        print(f'  LOADING: {vfile}')
        inputs = load_vecto(vfile)
        if inputs is None:
            continue

        lpaths = get_inputs_path(inputs)
        has_xml_refs = any(
            get_dict_val_from_keypath(inputs, p).endswith('.xml')
            for p in lpaths if 'file' in p
        )

        mode = _vecto_mode(inputs, has_xml_refs, vfile)
        powertrain = _vecto_powertrain(inputs, has_xml_refs)

        sibling_vecto = [f for f in vecto_files if os.path.dirname(f) == os.path.dirname(vfile)]
        extra = _strip_common_affixes(sibling_vecto)[sibling_vecto.index(vfile)] if len(sibling_vecto) > 1 else ''
        vtype_suffix = '_vecto2xml' if has_xml_refs else ''

        dirname = _output_dirname(os.path.dirname(vfile), input_dir, mode, powertrain, suffix=extra + vtype_suffix)

        replace_string_in_filepath(inputs, '..', '.')
        file_entries = _vecto_inputs_to_bytes(inputs)
        manifests.append(VehicleManifest(dirname=dirname, files=file_entries))

    # Standalone XML files
    xml_files = [
        os.path.join(root, f)
        for root, _, files in os.walk(input_dir)
        for f in files
        if f.endswith('.xml') and 'VTP' not in root and not _is_excluded(os.path.join(root, f))
    ]

    by_dir: dict[str, list[str]] = {}
    for fxml in xml_files:
        by_dir.setdefault(os.path.dirname(fxml), []).append(fxml)

    records = []
    for fxml in xml_files:
        print(f'  LOADING: {fxml}')
        xml_tree = etree.parse(fxml)
        vehicle = xml_tree.find('.//{*}Vehicle')
        if vehicle is None or not vehicle.attrib:
            records.append({'path': fxml, 'dir': os.path.dirname(fxml),
                            'type': 'NA', 'powertrain': 'NA', 'tree': xml_tree})
            continue
        type_attr = next((v for k, v in vehicle.attrib.items() if 'type' in k), '')
        if 'Primary' in type_attr:
            vtype = 'PrimaryBus'
        elif 'Completed' in type_attr:
            vtype = 'CompletedBus'
        elif 'HeavyLorry' in type_attr:
            vtype = 'HeavyLorry'
        elif 'MediumLorry' in type_attr:
            vtype = 'MediumLorry'
        else:
            vtype = 'other'
        records.append({'path': fxml, 'dir': os.path.dirname(fxml),
                        'type': vtype, 'powertrain': _xml_powertrain(xml_tree), 'tree': xml_tree})

    df = pd.DataFrame(records)
    if df.empty:
        return manifests

    dir_counts = df.groupby('dir')['path'].count()
    lone_dirs = set(dir_counts[dir_counts == 1].index)

    for row in df[df['dir'].isin(lone_dirs)].itertuples():
        dirname = _output_dirname(row.dir, input_dir, _MODE_DECL, row.powertrain, suffix='_xml')
        buf = io.BytesIO()
        row.tree.write(buf, encoding='utf-8', xml_declaration=True, pretty_print=False)
        manifests.append(VehicleManifest(
            dirname=dirname,
            files=[(_sanitise(os.path.basename(row.path)), buf.getvalue())],
        ))

    multi_df = df[~df['dir'].isin(lone_dirs)]
    dir_types = multi_df.groupby('dir')['type'].apply(list)
    runnable_dirs = {d for d, types in dir_types.items() if len(types) > 1 and 'CompletedBus' not in types}

    for row in multi_df[multi_df['dir'].isin(runnable_dirs)].itertuples():
        stem = os.path.splitext(os.path.basename(row.path))[0]
        dirname = _sanitise(os.path.join(_MODE_DECL, row.powertrain, stem + '_xml'))
        buf = io.BytesIO()
        row.tree.write(buf, encoding='utf-8', xml_declaration=True, pretty_print=False)
        manifests.append(VehicleManifest(
            dirname=dirname,
            files=[(_sanitise(os.path.basename(row.path)), buf.getvalue())],
        ))

    return manifests


# ---------------------------------------------------------------------------
# Step 2: Issue vehicles (A03 logic)
# ---------------------------------------------------------------------------

def collect_issue_vehicles(issues_input_dir: Path) -> list[VehicleManifest]:
    """Scan the Article 10 pool, deduplicate vehicles, return in-memory manifests."""
    save_dir_raw = issues_input_dir / 'A00_RAW'
    save_dir_extract = issues_input_dir / 'A01_EXTRACT'

    with timed('Scan local pool'):
        df_issue_label, df_issue_vins, df_hash_sorted = scan_pool(
            save_dir_raw, save_dir_extract, ISSUE_DESC_FILENAME
        )

    downloaded_files = [f for f in save_dir_raw.glob('*/uploads/**/*') if f.is_file()]
    extracted_files = [f for f in save_dir_extract.glob('*/uploads/**/*') if f.is_file()]
    df_local_path = generate_df_path(downloaded_files + extracted_files)
    df_local_path = df_local_path.join(df_issue_label.set_index('Issue'), on='iid', how='left')

    with timed('Sort and deduplicate vehicles'):
        df_vecto = get_vecto_file_data(df_local_path)
        df_vecto['isDeclared'] = df_vecto['IssueVIN'].isin(df_issue_vins['IssueVIN'].values)
        df_vecto_filt = df_vecto[df_vecto.is_xml_input == True]
        df_vecto_ordered = (
            df_hash_sorted
            .join(df_vecto_filt.set_index('gitlab_upload_hash'), on='gitlab_upload_hash', how='left')
            .dropna(subset=['VIN'])
        )
        df_vehicles = get_vehicles(df_vecto_ordered)
        df_vehicles_ref = get_REF_vehicles(df_vehicles)

    # Diagnostic report — keep only most recent
    report_path = Path(f'Vehicles_found_{datetime.now().strftime("%Y%m%d-%H%M")}.xlsx')
    with pd.ExcelWriter(report_path, engine='openpyxl') as writer:
        df_vehicles_ref[['REF_save_dirname', 'REF_Issue_VIN', 'Issue_VIN']].to_excel(writer, sheet_name='Duplicates')
        df_vehicles.to_excel(writer, sheet_name='All_vehicles')
    for old in sorted(Path.cwd().glob('Vehicles_found_*.xlsx'))[:-1]:
        old.unlink()

    # Build manifests from df_vehicles_ref
    col_to_explode = ['REF_local_path', 'REF_file_type', 'REF_vehicle_type']
    df_to_run = (
        df_vehicles_ref[df_vehicles_ref.REF_Label != 'art10::rejected']
        .explode(col_to_explode)
    )

    # Group by dirname → collect files
    groups: dict[str, list[tuple[str, bytes]]] = {}
    for _, row in df_to_run.iterrows():
        dirname = str(Path(_MODE_ART10) / row['REF_save_dirname'])
        filename = row['REF_vehicle_type'] + '.xml'
        content = Path(row['REF_local_path']).read_bytes()
        groups.setdefault(dirname, []).append((filename, content))

    # Add generated .vecto job files for bus configurations
    for dirname, files in groups.items():
        xml_names = [f[0] for f in files]
        comp_files = [f for f in xml_names if 'comp' in f.lower()]
        prim_files = [f for f in xml_names if 'prim' in f.lower()]
        vif_files = [f for f in xml_names if 'vif' in f.lower()]
        if comp_files and prim_files:
            job_bytes = _job_json_bytes(prim_files[0], comp_files[0], generate_PRIM_COMP_job)
            files.append(('job.vecto', job_bytes))
        elif comp_files and vif_files:
            job_bytes = _job_json_bytes(vif_files[0], comp_files[0], generate_PRIMVIF_COMP_job)
            files.append(('job.vecto', job_bytes))

    return [VehicleManifest(dirname=dirname, files=files) for dirname, files in groups.items()]


# ---------------------------------------------------------------------------
# Step 3: Package into input.zip (A11 logic)
# ---------------------------------------------------------------------------

def package_vehicles(manifests: list[VehicleManifest], output_dir: Path, force_repackage: bool) -> None:
    """Write each manifest as <output_dir>/<dirname>/input.zip."""
    i = 0
    for manifest in manifests:
        zip_path = output_dir / manifest.dirname / 'input.zip'
        if zip_path.exists() and not force_repackage:
            continue
        zip_path.parent.mkdir(parents=True, exist_ok=True)
        with zipfile.ZipFile(zip_path, 'w', compression=zipfile.ZIP_DEFLATED) as zf:
            for filename, content in manifest.files:
                zf.writestr(filename, content)
        i += 1
    print(f'  {i} input.zip file(s) written')


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Generate HTCondor-ready zipped inputs from generic and issue vehicle files.'
    )
    parser.add_argument(
        'generic_input_dir',
        help='Directory containing generic .vecto and .xml vehicle input files.',
    )
    parser.add_argument(
        'issues_input_dir',
        help='Root of the Article 10 test pool (contains A00_RAW and A01_EXTRACT subdirs).',
    )
    parser.add_argument(
        'output_dir',
        help='Output root. Zips are written to <output_dir>/To_Run/<vehicle>/input.zip. '
             'Pass <output_dir>/To_Run as test_pool_path to B03/B32.',
    )
    parser.add_argument(
        '--force-repackage',
        action='store_true',
        default=False,
        help='Re-zip even if output/<vehicle>/input.zip already exists.',
    )
    args = parser.parse_args()

    output_dir = Path(args.output_dir).joinpath('To_Run')

    print(70 * '=')
    print('A02 - Generate Files To Run')

    with timed('Collect generic vehicles'):
        generic_manifests = collect_generic_vehicles(args.generic_input_dir)
    print(f'  {len(generic_manifests)} generic vehicle(s) collected')

    with timed('Collect issue vehicles'):
        issue_manifests = collect_issue_vehicles(Path(args.issues_input_dir))
    print(f'  {len(issue_manifests)} issue vehicle(s) collected')

    with timed('Package vehicles'):
        package_vehicles(generic_manifests + issue_manifests, output_dir, args.force_repackage)

    print('Done.')


if __name__ == '__main__':
    main()
