"""
Tests for A02_Generate_Files_To_Run.py: collect_generic_vehicles,
collect_issue_vehicles (exercised end-to-end via DevTools/synthesize_issue_pool.py),
and package_vehicles. All XML/vecto content used here is placeholder data
built in tmp_path - no real pool data is read.
"""

import json
import os
import sys
import zipfile

import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..', 'DevTools'))

from A02_Generate_Files_To_Run import (
    VehicleManifest,
    _is_excluded,
    _job_json_bytes,
    _output_dirname,
    _strip_common_affixes,
    _vecto_inputs_to_bytes,
    _vecto_mode,
    _vecto_powertrain,
    _xml_powertrain,
    collect_generic_vehicles,
    collect_issue_vehicles,
    package_vehicles,
)
from FUNCTIONS.utils_generate_vecto_input2run import generate_PRIM_COMP_job, generate_PRIMVIF_COMP_job
from synthesize_issue_pool import synthesize_pool


def _write_zip(path, members: dict[str, bytes]):
    path.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(path, 'w') as zf:
        for name, content in members.items():
            zf.writestr(name, content)
    return path


def _make_issue_folder(root, iid_str, vin, members: dict[str, bytes]):
    folder = root / f"{iid_str}_{vin}"
    _write_zip(folder / "input.zip", members)
    return folder


_GENERIC_PRIMARY_XML = (
    b'<Root><Vehicle xsi:type="PrimaryBusDeclaration" '
    b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
    b'<VIN>GENERICVIN0001</VIN></Vehicle></Root>'
)

_GENERIC_LORRY_XML = (
    b'<Root><Vehicle xsi:type="HeavyLorryDeclaration" '
    b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
    b'<VIN>GENERICVIN0002</VIN></Vehicle></Root>'
)

# read_vecto_file needs a real DeclarationInput namespace plus an xsi:type
# containing "Primary" to land in file_type 1 (any other namespace adds the
# "other" bucket 4); the final get_REF_vehicles() dropna() also requires
# <VehicleCategory>, or the row is silently dropped as missing REF_veh_category.
_ISSUE_PRIMARY_XML = (
    b'<Root xmlns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0"'
    b' xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
    b'<Vehicle xsi:type="PrimaryVehicleDeclarationType"><VIN>PLACEHOLDERVIN001</VIN>'
    b'<VehicleCategory>N2</VehicleCategory>'
    b'</Vehicle>'
    b'<Date>2020-01-01</Date>'
    b'</Root>'
)


class TestIsExcluded:
    def test_excludes_invalid_substring(self):
        assert _is_excluded('/pool/vehicle_invalid/input.vecto') is True

    def test_excludes_p39_40_substring(self):
        assert _is_excluded('/pool/P39_40/input.vecto') is True

    def test_does_not_exclude_normal_path(self):
        assert _is_excluded('/pool/vehicle/input.vecto') is False


class TestOutputDirname:
    def test_builds_mode_powertrain_vehicle_path(self, tmp_path):
        input_root = tmp_path
        input_dir = tmp_path / "TruckA"

        dirname = _output_dirname(str(input_dir), str(input_root), 'GenericDeclaration', 'ICE')

        assert dirname == os.path.join('GenericDeclaration', 'ICE', 'TruckA')

    def test_strips_genericvehicle_prefix(self, tmp_path):
        input_dir = tmp_path / "GenericVehicle_TruckA"

        dirname = _output_dirname(str(input_dir), str(tmp_path), 'GenericDeclaration', 'ICE')

        assert dirname == os.path.join('GenericDeclaration', 'ICE', 'TruckA')

    def test_sanitises_spaces(self, tmp_path):
        input_dir = tmp_path / "Truck A"

        dirname = _output_dirname(str(input_dir), str(tmp_path), 'GenericDeclaration', 'ICE')

        assert ' ' not in dirname

    def test_appends_suffix(self, tmp_path):
        input_dir = tmp_path / "TruckA"

        dirname = _output_dirname(str(input_dir), str(tmp_path), 'GenericDeclaration', 'ICE', suffix='_extra')

        assert dirname.endswith('TruckA_extra')


class TestStripCommonAffixes:
    def test_single_string_unchanged(self):
        assert _strip_common_affixes(['only.xml']) == ['only.xml']

    def test_strips_shared_prefix_and_extension_suffix(self):
        result = _strip_common_affixes(['TruckA_decl.xml', 'TruckB_decl.xml'])

        assert result == ['A_decl', 'B_decl']

    def test_no_common_affix_returns_unchanged_relative_to_shared_parts(self):
        result = _strip_common_affixes(['abc', 'xyz'])

        assert result == ['abc', 'xyz']


class TestXmlPowertrain:
    def _tree(self, type_attr: str | None):
        from lxml import etree
        if type_attr is None:
            xml = b'<Root><Vehicle/></Root>'
        else:
            xml = (
                b'<Root><Vehicle xsi:type="' + type_attr.encode() + b'" '
                b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/></Root>'
            )
        return etree.fromstring(xml).getroottree()

    def test_no_vehicle_element_defaults_to_ice(self):
        from lxml import etree
        tree = etree.fromstring(b'<Root/>').getroottree()

        assert _xml_powertrain(tree) == 'ICE'

    def test_vehicle_with_no_attrib_defaults_to_ice(self):
        tree = self._tree(None)

        assert _xml_powertrain(tree) == 'ICE'

    def test_pev_type_detected(self):
        tree = self._tree('PEVDeclaration')

        assert _xml_powertrain(tree) == 'PEV'

    def test_hev_type_detected(self):
        tree = self._tree('HEVDeclaration')

        assert _xml_powertrain(tree) == 'HEV'

    def test_fchv_type_detected(self):
        tree = self._tree('FCHVDeclaration')

        assert _xml_powertrain(tree) == 'FCHV'

    def test_conventional_defaults_to_ice(self):
        tree = self._tree('ConventionalDeclaration')

        assert _xml_powertrain(tree) == 'ICE'


class TestVectoMode:
    def test_has_xml_refs_and_not_vtp_is_declaration(self):
        assert _vecto_mode({}, has_xml_refs=True, vfile='/pool/TruckA/input.vecto') == 'GenericDeclaration'

    def test_has_xml_refs_with_vtp_falls_through_to_saved_in_decl_mode(self):
        inputs = {'vecto': {'data': {'Body': {'SavedInDeclMode': True}}}}

        result = _vecto_mode(inputs, has_xml_refs=True, vfile='/pool/VTP/input.vecto')

        assert result == 'GenericDeclaration'

    def test_saved_in_decl_mode_true_is_declaration(self):
        inputs = {'vecto': {'data': {'Body': {'SavedInDeclMode': True}}}}

        assert _vecto_mode(inputs, has_xml_refs=False, vfile='/pool/x.vecto') == 'GenericDeclaration'

    def test_saved_in_decl_mode_false_is_engineering(self):
        inputs = {'vecto': {'data': {'Body': {'SavedInDeclMode': False}}}}

        assert _vecto_mode(inputs, has_xml_refs=False, vfile='/pool/x.vecto') == 'GenericEngineering'

    def test_missing_saved_in_decl_mode_is_unknown(self):
        inputs = {'vecto': {'data': {'Body': {}}}}

        assert _vecto_mode(inputs, has_xml_refs=False, vfile='/pool/x.vecto') == 'UNKNOWN'


class TestVectoPowertrain:
    def test_ice_and_motor_is_hev(self):
        inputs = {
            'subfiles': {
                'EngineFile': {'file': 'engine.xml'},
                'MotorFile': {'file': 'motor.xml'},
            }
        }

        assert _vecto_powertrain(inputs, has_xml_refs=False) == 'HEV'

    def test_ice_only_is_ice(self):
        inputs = {'subfiles': {'EngineFile': {'file': 'engine.xml'}}}

        assert _vecto_powertrain(inputs, has_xml_refs=False) == 'ICE'

    def test_motor_and_fuel_cell_is_fchv(self):
        inputs = {
            'subfiles': {
                'MotorFile': {'file': 'motor.xml'},
                'FuelCellSystemFile': {'file': 'fc.xml'},
            }
        }

        assert _vecto_powertrain(inputs, has_xml_refs=False) == 'FCHV'

    def test_motor_only_is_pev(self):
        inputs = {'subfiles': {'MotorFile': {'file': 'motor.xml'}}}

        assert _vecto_powertrain(inputs, has_xml_refs=False) == 'PEV'

    def test_no_matching_files_defaults_to_ice(self):
        inputs = {'subfiles': {}}

        assert _vecto_powertrain(inputs, has_xml_refs=False) == 'ICE'


class TestVectoInputsToBytes:
    def test_json_subfile_serialised(self):
        inputs = {
            'subfiles': {
                'EngineFile_0': {'file': 'engine.json', 'type': 'json', 'data': {'a': 1}},
            }
        }

        result = _vecto_inputs_to_bytes(inputs)

        assert len(result) == 1
        fname, content = result[0]
        assert fname == 'engine.json'
        assert json.loads(content.decode('utf-8-sig')) == {'a': 1}

    def test_csv_subfile_serialised(self):
        import pandas as pd
        df = pd.DataFrame({'a': [1, 2]})
        inputs = {'subfiles': {'DataFile_0': {'file': 'data.csv', 'type': 'csv', 'data': df}}}

        result = _vecto_inputs_to_bytes(inputs)

        assert len(result) == 1
        fname, content = result[0]
        assert fname == 'data.csv'
        assert b'a' in content

    def test_xml_subfile_serialised(self):
        from lxml import etree
        tree = etree.fromstring(b'<Root/>').getroottree()
        inputs = {'subfiles': {'CompFile_0': {'file': 'comp.xml', 'type': 'xml', 'data': tree}}}

        result = _vecto_inputs_to_bytes(inputs)

        assert len(result) == 1
        fname, content = result[0]
        assert fname == 'comp.xml'
        assert b'<Root' in content

    def test_unknown_type_skipped(self):
        inputs = {'subfiles': {'Other_0': {'file': 'x.dat', 'type': 'dat', 'data': None}}}

        result = _vecto_inputs_to_bytes(inputs)

        assert result == []


class TestJobJsonBytes:
    def test_prim_comp_job_run_simulation_true_by_default(self):
        result = _job_json_bytes('prim.xml', 'comp.xml', generate_PRIM_COMP_job)

        job = json.loads(result)
        assert job['Body']['RunSimulation'] == 'true'
        assert job['Body']['PrimaryVehicle'] == 'prim.xml'
        assert job['Body']['InterimStep'] == 'comp.xml'

    def test_prim_comp_job_run_simulation_false(self):
        result = _job_json_bytes('prim.xml', 'comp.xml', generate_PRIM_COMP_job, run_simulation=False)

        job = json.loads(result)
        assert job['Body']['RunSimulation'] == 'false'

    def test_primvif_comp_job_run_simulation_false(self):
        result = _job_json_bytes('vif.xml', 'comp.xml', generate_PRIMVIF_COMP_job, run_simulation=False)

        job = json.loads(result)
        assert job['Body']['RunSimulation'] == 'false'
        assert job['Body']['PrimaryVehicleResults'] == 'vif.xml'
        assert job['Body']['CompletedVehicle'] == 'comp.xml'


class TestCollectGenericVehicles:
    def test_empty_dir_returns_no_manifests(self, tmp_path):
        result = collect_generic_vehicles(str(tmp_path))

        assert result == []

    def test_loose_standalone_xml_file_collected(self, tmp_path):
        veh_dir = tmp_path / "TruckA"
        veh_dir.mkdir()
        (veh_dir / "vehicle.xml").write_bytes(_GENERIC_PRIMARY_XML)

        result = collect_generic_vehicles(str(tmp_path))

        assert len(result) == 1
        assert result[0].dirname.startswith(os.path.join('GenericDeclaration', 'ICE'))
        assert result[0].files[0][0] == 'vehicle.xml'

    def test_excluded_path_is_skipped(self, tmp_path):
        veh_dir = tmp_path / "TruckA_invalid"
        veh_dir.mkdir()
        (veh_dir / "vehicle.xml").write_bytes(_GENERIC_PRIMARY_XML)

        result = collect_generic_vehicles(str(tmp_path))

        assert result == []

    def test_exempted_vehicle_skipped(self, tmp_path):
        veh_dir = tmp_path / "TruckA"
        veh_dir.mkdir()
        xml = (
            b'<Root><Vehicle xsi:type="ExemptedDeclaration" '
            b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
            b'<VIN>X</VIN></Vehicle></Root>'
        )
        (veh_dir / "vehicle.xml").write_bytes(xml)

        result = collect_generic_vehicles(str(tmp_path))

        assert result == []

    def test_two_xml_files_in_same_dir_with_different_types_are_runnable(self, tmp_path):
        veh_dir = tmp_path / "BusA"
        veh_dir.mkdir()
        prim_xml = (
            b'<Root><Vehicle xsi:type="PrimaryBusDeclaration" '
            b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
            b'<VIN>V1</VIN></Vehicle></Root>'
        )
        lorry_xml = (
            b'<Root><Vehicle xsi:type="HeavyLorryDeclaration" '
            b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
            b'<VIN>V2</VIN></Vehicle></Root>'
        )
        (veh_dir / "a.xml").write_bytes(prim_xml)
        (veh_dir / "b.xml").write_bytes(lorry_xml)

        result = collect_generic_vehicles(str(tmp_path))

        assert len(result) == 2

    def test_two_xml_files_same_dir_with_completed_type_are_not_runnable(self, tmp_path):
        veh_dir = tmp_path / "BusB"
        veh_dir.mkdir()
        prim_xml = (
            b'<Root><Vehicle xsi:type="PrimaryBusDeclaration" '
            b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
            b'<VIN>V1</VIN></Vehicle></Root>'
        )
        comp_xml = (
            b'<Root><Vehicle xsi:type="CompletedBusDeclaration" '
            b'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">'
            b'<VIN>V2</VIN></Vehicle></Root>'
        )
        (veh_dir / "a.xml").write_bytes(prim_xml)
        (veh_dir / "b.xml").write_bytes(comp_xml)

        result = collect_generic_vehicles(str(tmp_path))

        assert result == []

    def test_vtp_directory_is_ignored_for_standalone_xml(self, tmp_path):
        veh_dir = tmp_path / "VTP"
        veh_dir.mkdir()
        (veh_dir / "vehicle.xml").write_bytes(_GENERIC_PRIMARY_XML)

        result = collect_generic_vehicles(str(tmp_path))

        assert result == []

    def test_no_vehicle_element_xml_is_skipped_from_lone_dirs_output(self, tmp_path):
        veh_dir = tmp_path / "TruckA"
        veh_dir.mkdir()
        (veh_dir / "vehicle.xml").write_bytes(b'<Root/>')

        result = collect_generic_vehicles(str(tmp_path))

        assert len(result) == 1


class TestPackageVehicles:
    def test_writes_input_zip_per_manifest(self, tmp_path, capsys):
        manifests = [
            VehicleManifest(dirname='GenericDeclaration/ICE/TruckA', files=[('vehicle.xml', b'<Root/>')]),
        ]

        package_vehicles(manifests, tmp_path, force_repackage=False)

        zip_path = tmp_path / 'GenericDeclaration' / 'ICE' / 'TruckA' / 'input.zip'
        assert zip_path.is_file()
        with zipfile.ZipFile(zip_path) as zf:
            assert zf.namelist() == ['vehicle.xml']
        assert '1 input.zip file(s) written' in capsys.readouterr().out

    def test_skips_existing_zip_without_force_repackage(self, tmp_path):
        manifest = VehicleManifest(dirname='TruckA', files=[('vehicle.xml', b'<Root/>')])
        zip_path = tmp_path / 'TruckA' / 'input.zip'
        zip_path.parent.mkdir(parents=True)
        zip_path.write_bytes(b'PK\x03\x04existing')

        package_vehicles([manifest], tmp_path, force_repackage=False)

        assert zip_path.read_bytes() == b'PK\x03\x04existing'

    def test_force_repackage_overwrites_existing_zip(self, tmp_path):
        manifest = VehicleManifest(dirname='TruckA', files=[('vehicle.xml', b'<Root/>')])
        zip_path = tmp_path / 'TruckA' / 'input.zip'
        zip_path.parent.mkdir(parents=True)
        zip_path.write_bytes(b'PK\x03\x04existing')

        package_vehicles([manifest], tmp_path, force_repackage=True)

        with zipfile.ZipFile(zip_path) as zf:
            assert zf.namelist() == ['vehicle.xml']


class TestCollectIssueVehicles:
    def test_single_valid_issue_produces_one_manifest(self, tmp_path):
        test_pool_issue_dir = tmp_path / "ISSUE"
        pool_root = tmp_path / "pool"
        _make_issue_folder(
            test_pool_issue_dir, "0180", "YV2XT60C1RA341486",
            {"vehicle_input.xml": _ISSUE_PRIMARY_XML},
        )
        synthesize_pool(test_pool_issue_dir, pool_root)

        result = collect_issue_vehicles(pool_root)

        assert len(result) == 1
        assert result[0].dirname.startswith('Declaration')
        assert any(name.endswith('.xml') for name, _ in result[0].files)

    def test_no_issues_raises_keyerror(self, tmp_path):
        # Pre-existing behavior: with no issue_txt_data*.json files at all,
        # read_pool_stats returns a columnless DataFrame, so
        # df_issue_label[['art10_label']] (A02_Generate_Files_To_Run.py:336)
        # raises KeyError instead of returning [].
        pool_root = tmp_path / "pool"
        (pool_root / "A00_RAW").mkdir(parents=True)
        (pool_root / "A01_EXTRACT").mkdir(parents=True)

        with pytest.raises(KeyError):
            collect_issue_vehicles(pool_root)

    def test_writes_diagnostic_report_in_cwd(self, tmp_path, monkeypatch):
        test_pool_issue_dir = tmp_path / "ISSUE"
        pool_root = tmp_path / "pool"
        cwd = tmp_path / "cwd"
        cwd.mkdir()
        _make_issue_folder(
            test_pool_issue_dir, "0180", "YV2XT60C1RA341486",
            {"vehicle_input.xml": _ISSUE_PRIMARY_XML},
        )
        synthesize_pool(test_pool_issue_dir, pool_root)
        monkeypatch.chdir(cwd)

        collect_issue_vehicles(pool_root)

        reports = list(cwd.glob('Vehicles_found_*.xlsx'))
        assert len(reports) == 1

    def test_manifests_are_zippable_by_package_vehicles(self, tmp_path):
        test_pool_issue_dir = tmp_path / "ISSUE"
        pool_root = tmp_path / "pool"
        out_dir = tmp_path / "out"
        _make_issue_folder(
            test_pool_issue_dir, "0180", "YV2XT60C1RA341486",
            {"vehicle_input.xml": _ISSUE_PRIMARY_XML},
        )
        synthesize_pool(test_pool_issue_dir, pool_root)
        manifests = collect_issue_vehicles(pool_root)

        package_vehicles(manifests, out_dir, force_repackage=False)

        zips = list(out_dir.glob('Declaration/*/input.zip'))
        assert len(zips) == 1
