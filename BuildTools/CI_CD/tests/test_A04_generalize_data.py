"""
Tests for A04_Generalize_Data.py
"""

import os
import sys
import zipfile

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
from A04_Generalize_Data import (
    CONFIDENTIAL_FIELDS,
    CONFIDENTIAL_SUFFIX,
    CONFIDENTIAL_VALUE,
    DEFAULT_DATE,
    ENV_VAR_NAME,
    ID_PREFIX,
    anonymize_xml_bytes,
    find_xml_files,
    find_zip_files,
    generalize_data,
    generalize_xml_file,
    generalize_zip_file,
)

_XML_TEMPLATE = """<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v3.0">
  <Vehicle>
    <Manufacturer>Real Manufacturer Co</Manufacturer>
    <ManufacturerAddress>123 Real Street</ManufacturerAddress>
    <Model>RM-9000</Model>
    <VIN>WVWZZZ1JZXW000001</VIN>
    <CertificationNumber>e12*0815/8051*2017/05E0000*00</CertificationNumber>
  </Vehicle>
</tns:VectoInputDeclaration>
"""


def _write_xml(path, content=_XML_TEMPLATE):
    path.write_text(content, encoding='utf-8')
    return path


def _write_zip(path, members: dict[str, str]):
    """Write a .zip at path containing one file per (name, text content) pair."""
    with zipfile.ZipFile(path, 'w') as zf:
        for name, content in members.items():
            zf.writestr(name, content)
    return path


# ---------------------------------------------------------------------------
# find_xml_files / find_zip_files
# ---------------------------------------------------------------------------

class TestFindXmlFiles:
    def test_finds_loose_xml_files_recursively(self, tmp_path):
        _write_xml(tmp_path / 'a.xml')
        nested = tmp_path / 'nested'
        nested.mkdir()
        _write_xml(nested / 'b.xml')

        result = find_xml_files(tmp_path)

        assert len(result) == 2

    def test_ignores_non_xml_files(self, tmp_path):
        _write_xml(tmp_path / 'a.xml')
        (tmp_path / 'notes.txt').write_text('not xml')

        result = find_xml_files(tmp_path)

        assert len(result) == 1
        assert result[0].name == 'a.xml'

    def test_does_not_descend_into_archives(self, tmp_path):
        (tmp_path / 'bundle.zip').write_bytes(b'PK\x03\x04fake-zip-content')
        _write_xml(tmp_path / 'a.xml')

        result = find_xml_files(tmp_path)

        assert len(result) == 1
        assert result[0].name == 'a.xml'


class TestFindZipFiles:
    def test_finds_zip_files_recursively(self, tmp_path):
        _write_zip(tmp_path / 'a.zip', {'x.xml': _XML_TEMPLATE})
        nested = tmp_path / 'nested'
        nested.mkdir()
        _write_zip(nested / 'b.zip', {'x.xml': _XML_TEMPLATE})

        result = find_zip_files(tmp_path)

        assert len(result) == 2

    def test_ignores_non_zip_files(self, tmp_path):
        _write_zip(tmp_path / 'a.zip', {'x.xml': _XML_TEMPLATE})
        _write_xml(tmp_path / 'a.xml')

        result = find_zip_files(tmp_path)

        assert len(result) == 1
        assert result[0].name == 'a.zip'


# ---------------------------------------------------------------------------
# generalize_xml_file
# ---------------------------------------------------------------------------

class TestGeneralizeXmlFile:
    def test_replaces_all_five_target_elements(self, tmp_path):
        fpath = _write_xml(tmp_path / 'vehicle.xml')

        changed = generalize_xml_file(fpath)

        assert changed == 5
        content = fpath.read_text(encoding='utf-8')
        assert '<Manufacturer>CONFIDENTIAL</Manufacturer>' in content
        assert '<ManufacturerAddress>CONFIDENTIAL</ManufacturerAddress>' in content
        assert '<Model>CONFIDENTIAL</Model>' in content
        assert '<VIN>CONFIDENTIAL</VIN>' in content
        assert '<CertificationNumber>CONFIDENTIAL</CertificationNumber>' in content

    def test_overwrites_unconditionally_even_if_already_generic(self, tmp_path):
        xml = _XML_TEMPLATE.replace('Real Manufacturer Co', 'CONFIDENTIAL')
        fpath = _write_xml(tmp_path / 'vehicle.xml', xml)

        changed = generalize_xml_file(fpath)

        assert changed == 5

    def test_matches_elements_regardless_of_namespace(self, tmp_path):
        xml = """<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v3.0"
    xmlns:v2="urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0">
  <v2:Component>
    <v2:Manufacturer>Real Co</v2:Manufacturer>
  </v2:Component>
</tns:VectoInputDeclaration>
"""
        fpath = _write_xml(tmp_path / 'vehicle.xml', xml)

        changed = generalize_xml_file(fpath)

        assert changed == 1
        assert 'CONFIDENTIAL<' in fpath.read_text(encoding='utf-8')

    def test_repeated_elements_are_all_replaced(self, tmp_path):
        xml = """<?xml version="1.0" encoding="utf-8"?>
<Root>
  <Component><Manufacturer>Co A</Manufacturer></Component>
  <Component><Manufacturer>Co B</Manufacturer></Component>
</Root>
"""
        fpath = _write_xml(tmp_path / 'vehicle.xml', xml)

        changed = generalize_xml_file(fpath)

        assert changed == 2
        content = fpath.read_text(encoding='utf-8')
        assert content.count('<Manufacturer>CONFIDENTIAL</Manufacturer>') == 2

    def test_element_with_no_text_is_left_untouched(self, tmp_path):
        xml = """<?xml version="1.0" encoding="utf-8"?>
<Root>
  <Manufacturer/>
</Root>
"""
        fpath = _write_xml(tmp_path / 'vehicle.xml', xml)

        changed = generalize_xml_file(fpath)

        assert changed == 0

    def test_unrelated_elements_are_not_touched(self, tmp_path):
        fpath = _write_xml(tmp_path / 'vehicle.xml')

        generalize_xml_file(fpath)

        content = fpath.read_text(encoding='utf-8')
        assert 'VectoInputDeclaration' in content

    def test_malformed_xml_is_left_untouched(self, tmp_path):
        fpath = tmp_path / 'broken.xml'
        fpath.write_text('<Root><Manufacturer>Co</Manufacturer>', encoding='utf-8')

        changed = generalize_xml_file(fpath)

        assert changed == 0
        assert fpath.read_text(encoding='utf-8') == '<Root><Manufacturer>Co</Manufacturer>'


# ---------------------------------------------------------------------------
# generalize_zip_file
# ---------------------------------------------------------------------------

class TestGeneralizeZipFile:
    def test_generalizes_xml_entry_and_rewrites_archive(self, tmp_path):
        fpath = _write_zip(tmp_path / 'bundle.zip', {'vehicle.xml': _XML_TEMPLATE})

        files_changed, elements_changed = generalize_zip_file(fpath)

        assert files_changed == 1
        assert elements_changed == 5
        with zipfile.ZipFile(fpath) as zf:
            content = zf.read('vehicle.xml').decode('utf-8')
        assert '<Manufacturer>CONFIDENTIAL</Manufacturer>' in content
        assert '<VIN>CONFIDENTIAL</VIN>' in content

    def test_processes_all_xml_entries_keeps_others_unchanged(self, tmp_path):
        fpath = _write_zip(tmp_path / 'bundle.zip', {
            'a.xml': _XML_TEMPLATE,
            'b.xml': _XML_TEMPLATE,
            'readme.txt': 'not xml, leave me alone',
        })

        files_changed, elements_changed = generalize_zip_file(fpath)

        assert files_changed == 2
        assert elements_changed == 10
        with zipfile.ZipFile(fpath) as zf:
            assert zf.read('readme.txt').decode('utf-8') == 'not xml, leave me alone'
            assert set(zf.namelist()) == {'a.xml', 'b.xml', 'readme.txt'}

    def test_nested_zip_entry_is_left_untouched(self, tmp_path):
        # A zip found while walking data_dir is unzipped/processed, but a
        # .zip *entry inside* that zip (a nested archive) is not followed -
        # one level deep only.
        inner = tmp_path / 'inner.zip'
        _write_zip(inner, {'vehicle.xml': _XML_TEMPLATE})
        inner_bytes = inner.read_bytes()

        outer = tmp_path / 'outer.zip'
        with zipfile.ZipFile(outer, 'w') as zf:
            zf.writestr('a.xml', _XML_TEMPLATE)
            zf.writestr('nested/inner.zip', inner_bytes)

        files_changed, _ = generalize_zip_file(outer)

        assert files_changed == 1  # only a.xml, not the nested zip's contents
        with zipfile.ZipFile(outer) as zf:
            assert zf.read('nested/inner.zip') == inner_bytes

    def test_no_xml_entries_leaves_archive_untouched(self, tmp_path):
        fpath = _write_zip(tmp_path / 'bundle.zip', {'readme.txt': 'nothing to see here'})
        original_bytes = fpath.read_bytes()

        files_changed, elements_changed = generalize_zip_file(fpath)

        assert files_changed == 0
        assert elements_changed == 0
        assert fpath.read_bytes() == original_bytes

    def test_malformed_zip_is_left_untouched(self, tmp_path):
        fpath = tmp_path / 'broken.zip'
        fpath.write_bytes(b'not actually a zip')

        files_changed, elements_changed = generalize_zip_file(fpath)

        assert files_changed == 0
        assert elements_changed == 0
        assert fpath.read_bytes() == b'not actually a zip'


# ---------------------------------------------------------------------------
# generalize_data
# ---------------------------------------------------------------------------

class TestGeneralizeData:
    def test_summary_counts_across_multiple_files(self, tmp_path):
        _write_xml(tmp_path / 'a.xml')
        _write_xml(tmp_path / 'b.xml')

        summary = generalize_data(tmp_path)

        assert summary['files_scanned'] == 2
        assert summary['files_changed'] == 2
        assert summary['elements_changed'] == 10

    def test_file_with_no_target_elements_is_scanned_but_not_changed(self, tmp_path):
        _write_xml(tmp_path / 'a.xml', '<Root><Other>value</Other></Root>')

        summary = generalize_data(tmp_path)

        assert summary['files_scanned'] == 1
        assert summary['files_changed'] == 0
        assert summary['elements_changed'] == 0

    def test_zips_are_ignored_by_default(self, tmp_path):
        _write_zip(tmp_path / 'bundle.zip', {'vehicle.xml': _XML_TEMPLATE})

        summary = generalize_data(tmp_path)

        assert summary['zips_scanned'] == 0
        assert summary['zips_changed'] == 0
        with zipfile.ZipFile(tmp_path / 'bundle.zip') as zf:
            assert zf.read('vehicle.xml').decode('utf-8') == _XML_TEMPLATE

    def test_process_zips_true_generalizes_zip_contents_too(self, tmp_path):
        _write_xml(tmp_path / 'a.xml')
        _write_zip(tmp_path / 'bundle.zip', {'vehicle.xml': _XML_TEMPLATE})

        summary = generalize_data(tmp_path, process_zips=True)

        assert summary['files_scanned'] == 1        # loose .xml only
        assert summary['files_changed'] == 2         # a.xml + vehicle.xml inside the zip
        assert summary['elements_changed'] == 10     # 5 + 5
        assert summary['zips_scanned'] == 1
        assert summary['zips_changed'] == 1
        with zipfile.ZipFile(tmp_path / 'bundle.zip') as zf:
            content = zf.read('vehicle.xml').decode('utf-8')
        assert '<Manufacturer>CONFIDENTIAL</Manufacturer>' in content

    def test_process_zips_true_with_no_matching_entries_does_not_rewrite(self, tmp_path):
        _write_zip(tmp_path / 'bundle.zip', {'readme.txt': 'no xml here'})

        summary = generalize_data(tmp_path, process_zips=True)

        assert summary['zips_scanned'] == 1
        assert summary['zips_changed'] == 0




# ---------------------------------------------------------------------------
# anonymize_xml_bytes
# ---------------------------------------------------------------------------

_ALL_FIELDS_XML = """<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0"
    xmlns:di="http://www.w3.org/2000/09/xmldsig#">
  <Vehicle id="VEH-WVWZZZ1JZXW000001">
    <Manufacturer>Real Manufacturer Co</Manufacturer>
    <ManufacturerAddress>123 Real Street</ManufacturerAddress>
    <Model>RM-9000</Model>
    <VIN>WVWZZZ1JZXW000001</VIN>
    <Date>2024-05-17T10:11:12Z</Date>
    <Make>RealMake</Make>
    <AppVersion>RealTool 1.2.3</AppVersion>
    <VehicleTypeApprovalNumber>e1*2018/858*00001*01</VehicleTypeApprovalNumber>
    <Components>
      <Engine>
        <Data id="ENG-e12*0815/8051*2017/05E0000*00">
          <Manufacturer>Real Engine Co</Manufacturer>
          <Model>Engine X</Model>
          <CertificationNumber>e12*0815/8051*2017/05E0000*00</CertificationNumber>
          <Date>2017-02-15T11:00:00Z</Date>
          <AppVersion>EngTool 3</AppVersion>
          <TypeApprovalNumber>TA-123</TypeApprovalNumber>
          <Displacement>12740</Displacement>
        </Data>
        <Signature>
          <di:Reference URI="#ENG-e12*0815/8051*2017/05E0000*00">
            <di:DigestValue>e0c253b643f7f8f09b963aca4a264d06fbfa599f</di:DigestValue>
          </di:Reference>
        </Signature>
      </Engine>
    </Components>
    <SimulationToolLicenseNumber>LIC-1</SimulationToolLicenseNumber>
    <VectoLicenseNbr>LIC-2</VectoLicenseNbr>
  </Vehicle>
</tns:VectoInputDeclaration>
"""

_ORIGINAL_VALUES = [
    'Real Manufacturer Co', '123 Real Street', 'RM-9000', 'WVWZZZ1JZXW000001',
    'RealMake', 'RealTool', 'e1*2018/858*00001*01', 'Real Engine Co', 'Engine X',
    'e12*0815/8051*2017/05E0000*00', 'EngTool', 'TA-123', 'LIC-1', 'LIC-2',
    'e0c253b643f7f8f09b963aca4a264d06fbfa599f', '2024-05-17', '2017-02-15',
    'VEH-WVWZZZ1JZXW000001',
]


def _anonymize(xml: str):
    content, changed = anonymize_xml_bytes(xml.encode('utf-8'))
    return content.decode('utf-8'), changed


class TestAnonymizeXmlBytes:
    def test_all_confidential_fields_are_replaced(self):
        content, changed = _anonymize(_ALL_FIELDS_XML)

        # Vehicle: 10, engine Data: 6, DigestValue: 1.
        assert changed == 17
        for fld in CONFIDENTIAL_FIELDS:
            if fld in ('Date', 'DigestValue'):
                continue
            assert f'<{fld}>CONFIDENTIAL</{fld}>' in content
        assert '<di:DigestValue>CONFIDENTIAL</di:DigestValue>' in content

    def test_date_gets_default_date_not_confidential(self):
        content, _ = _anonymize(_ALL_FIELDS_XML)

        assert content.count(f'<Date>{DEFAULT_DATE}</Date>') == 2
        assert '<Date>CONFIDENTIAL</Date>' not in content

    def test_no_original_value_survives(self):
        content, _ = _anonymize(_ALL_FIELDS_XML)

        for original in _ORIGINAL_VALUES:
            assert original not in content

    def test_parent_ids_renamed_in_document_order(self):
        content, _ = _anonymize(_ALL_FIELDS_XML)

        # Vehicle comes first in the document, then the engine Data element;
        # each gets exactly one id although it has several matched children.
        assert f'<Vehicle id="{ID_PREFIX}00">' in content
        assert f'<Data id="{ID_PREFIX}01">' in content
        assert f'{ID_PREFIX}02' not in content

    def test_reference_uri_follows_renamed_id(self):
        content, _ = _anonymize(_ALL_FIELDS_XML)

        assert f'URI="#{ID_PREFIX}01"' in content

    def test_unrelated_elements_are_untouched(self):
        content, _ = _anonymize(_ALL_FIELDS_XML)

        assert '<Displacement>12740</Displacement>' in content

    def test_unrelated_id_and_uri_are_untouched(self):
        xml = """<?xml version="1.0" encoding="utf-8"?>
<Root>
  <Data id="keep-me"><Displacement>1</Displacement></Data>
  <Reference URI="#keep-me"/>
</Root>
"""
        content, changed = _anonymize(xml)

        assert changed == 0
        assert 'id="keep-me"' in content
        assert 'URI="#keep-me"' in content

    def test_malformed_xml_is_returned_unchanged(self):
        raw = b'<Root><VIN>WVW</VIN>'

        content, changed = anonymize_xml_bytes(raw)

        assert changed == 0
        assert content == raw

    def test_is_idempotent(self):
        once, _ = _anonymize(_ALL_FIELDS_XML)

        twice, _ = _anonymize(once)

        assert twice == once


# ---------------------------------------------------------------------------
# Constants contract
# ---------------------------------------------------------------------------

class TestConfidentialContract:
    def test_all_roadmap_fields_are_covered(self):
        assert set(CONFIDENTIAL_FIELDS) == {
            'VIN', 'Date', 'DigestValue', 'Manufacturer', 'ManufacturerAddress', 'Model',
            'AppVersion', 'CertificationNumber', 'SimulationToolLicenseNumber', 'Make',
            'TypeApprovalNumber', 'VectoLicenseNbr', 'VehicleTypeApprovalNumber',
        }

    def test_replacement_values_match_roadmap(self):
        assert CONFIDENTIAL_VALUE == 'CONFIDENTIAL'
        assert DEFAULT_DATE == '2020-01-01T08:00:00Z'
        assert ID_PREFIX == 'Confidential-ID'
        assert CONFIDENTIAL_SUFFIX == '_confidential.xml'

    def test_env_var_name_is_a04_data_dir(self):
        assert ENV_VAR_NAME == 'A04_DATA_DIR'
