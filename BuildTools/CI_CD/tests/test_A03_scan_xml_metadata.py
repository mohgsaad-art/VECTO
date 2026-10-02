"""
Tests for A03_Scan_XML_Metadata.py
"""

import os
import sys
from pathlib import Path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
from A03_Scan_XML_Metadata import find_xml_files, scan_xml_files, write_output


# ---------------------------------------------------------------------------
# find_xml_files
# ---------------------------------------------------------------------------

class TestFindXmlFiles:
    def test_excludes_debug_vin_files(self, tmp_path):
        upload_dir = tmp_path / 'A01_EXTRACT' / '0432' / 'uploads' / 'a3e375f3642e3b65cc451d808ab80df4'
        upload_dir.mkdir(parents=True)
        debug_xml = upload_dir / 'DEBUG_VIN_WKK1039191J803556_VectoInput.xml'
        good_xml = upload_dir / 'VectoInput.xml'
        debug_xml.write_text('<root/>\n', encoding='utf-8')
        good_xml.write_text('<root/>\n', encoding='utf-8')

        found = find_xml_files(tmp_path)

        assert good_xml in found
        assert debug_xml not in found

    def test_excludes_debug_vin_files_case_insensitively(self, tmp_path):
        upload_dir = tmp_path / 'uploads'
        upload_dir.mkdir(parents=True)
        debug_xml = upload_dir / 'debug_vin_lowercase_VectoInput.xml'
        debug_xml.write_text('<root/>\n', encoding='utf-8')

        found = find_xml_files(tmp_path)

        assert debug_xml not in found


# ---------------------------------------------------------------------------
# scan_xml_files
# ---------------------------------------------------------------------------

class TestScanXmlFilesParseWarning:
    def test_failed_parse_warning_omits_full_path(self, tmp_path, capsys):
        upload_dir = tmp_path / 'A01_EXTRACT' / '0432' / 'uploads' / 'a3e375f3642e3b65cc451d808ab80df4'
        upload_dir.mkdir(parents=True)
        bad_xml = upload_dir / 'DEBUG_VIN_WKK1039191J803556_VectoInput.xml'
        # A second, misplaced XML declaration - invalid per the XML spec and
        # what lxml actually raises on in the reported CI failures.
        bad_xml.write_text(
            '<?xml version="1.0"?>\n<root><?xml version="1.0"?></root>\n',
            encoding='utf-8',
        )

        scan_xml_files([bad_xml])

        output = capsys.readouterr().out
        assert 'a3e375f3642e3b65cc451d808ab80df4' in output
        assert str(bad_xml) not in output
        assert str(upload_dir) not in output
        assert 'file:/' not in output
        assert bad_xml.name not in output
        assert '(' not in output

    def test_failed_parse_warning_omits_path_with_parens_in_filename(self, tmp_path, capsys):
        # Regression: a filename containing literal parentheses (e.g. a
        # "(1)" duplicate suffix) defeats a naive "no nested parens" regex,
        # since lxml's own trailing "(path, line N)" segment then contains
        # an inner "(1)" pair - the fix must still strip the whole segment.
        upload_dir = tmp_path / 'A01_EXTRACT' / '0631' / 'uploads' / '74712bbc227fd9abbf8827e7e2226dee'
        upload_dir.mkdir(parents=True)
        bad_xml = upload_dir / 'DEBUG_VIN_WEB2803961L619523_VectoInput (1).xml'
        bad_xml.write_text(
            '<?xml version="1.0"?>\n<root><?xml version="1.0"?></root>\n',
            encoding='utf-8',
        )

        scan_xml_files([bad_xml])

        output = capsys.readouterr().out
        assert '74712bbc227fd9abbf8827e7e2226dee' in output
        assert str(bad_xml) not in output
        assert 'file:/' not in output
        assert bad_xml.name not in output
        assert '(' not in output


# ---------------------------------------------------------------------------
# write_output
# ---------------------------------------------------------------------------

class TestWriteOutput:
    def test_writes_non_latin1_values_as_utf8(self, tmp_path, monkeypatch):
        # Simulate a platform (e.g. the Windows cp1252 GitLab runner) whose
        # default text encoding cannot represent non-Latin-1 characters.
        # write_output() must not rely on that default - it should always
        # write UTF-8 regardless of the platform's default encoding.
        real_path_open = Path.open

        def _path_open_defaulting_to_cp1252(self, mode='r', *args, **kwargs):
            if 'b' not in mode and kwargs.get('encoding') is None:
                kwargs['encoding'] = 'cp1252'
            return real_path_open(self, mode, *args, **kwargs)

        monkeypatch.setattr(Path, 'open', _path_open_defaulting_to_cp1252)

        values = {
            'Manufacturer': {'Şahin Kamyon'},  # Şahin Kamyon
            'VIN': {'WVWZZZ1JZXW000001'},
            'CertificationNumber': {'e12*0815/8051*2017/05E0000*00'},
        }

        write_output(values, tmp_path)

        with open(tmp_path / 'Manufacturer.txt', 'rb') as f:
            raw = f.read()
        assert raw.decode('utf-8').strip() == 'Şahin Kamyon'
