"""
Tests for B14_Unzip_Results.py: matches_suffix, move_outputs, extract_outputs.

Regression coverage for the fix where reference/MR result archives (suffixed
'_REF'/'_MR' by B12) were being swept into whichever version B14 happened to
be invoked for, since the old code moved every '*_out.zip' in cwd
unconditionally.
"""

import os
import sys
import zipfile

import pytest
import pyzipper

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
from B14_Unzip_Results import extract_outputs, matches_suffix, move_outputs
from FUNCTIONS.SECURE_ZIP import ZIP_PASSWORD_ENV

PASSWORD = 'test-password'


def _write_zip(path, member_name='result.txt', content=b'data'):
    """Write a job output archive the way B13 does: AES-256 encrypted with PASSWORD."""
    with pyzipper.AESZipFile(path, 'w', compression=pyzipper.ZIP_DEFLATED, encryption=pyzipper.WZ_AES) as zf:
        zf.setpassword(PASSWORD.encode('utf-8'))
        zf.writestr(member_name, content)
    return path


class TestMatchesSuffix:
    def test_unsuffixed_matches_empty_suffix(self):
        assert matches_suffix('Declaration__Bus_CONV_abc12_out.zip', '') is True

    def test_ref_suffixed_does_not_match_empty_suffix(self):
        assert matches_suffix('Declaration__Bus_CONV_abc12_REF_out.zip', '') is False

    def test_mr_suffixed_does_not_match_empty_suffix(self):
        assert matches_suffix('Declaration__Bus_CONV_abc12_MR_out.zip', '') is False

    def test_ref_suffix_matches_only_ref_files(self):
        assert matches_suffix('Declaration__Bus_CONV_abc12_REF_out.zip', '_REF') is True
        assert matches_suffix('Declaration__Bus_CONV_abc12_MR_out.zip', '_REF') is False
        assert matches_suffix('Declaration__Bus_CONV_abc12_out.zip', '_REF') is False

    def test_mr_suffix_matches_only_mr_files(self):
        assert matches_suffix('Declaration__Bus_CONV_abc12_MR_out.zip', '_MR') is True
        assert matches_suffix('Declaration__Bus_CONV_abc12_REF_out.zip', '_MR') is False

    def test_non_zip_file_never_matches(self):
        assert matches_suffix('notes.txt', '') is False
        assert matches_suffix('batch_arguments.txt', '_REF') is False


class TestMoveOutputs:
    def test_moves_only_matching_files(self, tmp_path):
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        storage = tmp_path / 'storage'
        _write_zip(cwd / 'VehA_out.zip')
        _write_zip(cwd / 'VehB_REF_out.zip')
        _write_zip(cwd / 'VehC_MR_out.zip')

        moved = move_outputs(str(cwd), str(storage), '')

        assert moved == ['VehA_out.zip']
        assert (storage / 'VehA_out.zip').is_file()
        assert not (storage / 'VehB_REF_out.zip').exists()
        assert (cwd / 'VehB_REF_out.zip').is_file()
        assert (cwd / 'VehC_MR_out.zip').is_file()

    def test_ref_suffix_moves_only_ref_files(self, tmp_path):
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        storage = tmp_path / 'storage'
        _write_zip(cwd / 'VehA_out.zip')
        _write_zip(cwd / 'VehB_REF_out.zip')

        moved = move_outputs(str(cwd), str(storage), '_REF')

        assert moved == ['VehB_REF_out.zip']
        assert (storage / 'VehB_REF_out.zip').is_file()
        assert (cwd / 'VehA_out.zip').is_file()

    def test_non_matching_files_left_in_cwd(self, tmp_path):
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        storage = tmp_path / 'storage'
        (cwd / 'batch_arguments.txt').write_text('data')

        moved = move_outputs(str(cwd), str(storage), '')

        assert moved == []
        assert (cwd / 'batch_arguments.txt').is_file()


class TestExtractOutputs:
    def test_extracts_and_strips_ref_suffix(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'Declaration__Bus_CONV_abc12_REF_out.zip')
        extract_dir = tmp_path / 'extract'

        outpaths = extract_outputs(str(storage), str(extract_dir), password=PASSWORD)

        expected = extract_dir / 'Declaration' / 'Bus_CONV_abc12'
        assert str(expected) in outpaths
        assert (expected / 'result.txt').is_file()

    def test_extracts_and_strips_mr_suffix(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'Declaration__Bus_CONV_abc12_MR_out.zip')
        extract_dir = tmp_path / 'extract'

        outpaths = extract_outputs(str(storage), str(extract_dir), password=PASSWORD)

        expected = extract_dir / 'Declaration' / 'Bus_CONV_abc12'
        assert str(expected) in outpaths
        assert (expected / 'result.txt').is_file()

    def test_unsuffixed_file_extracted_as_is(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'Declaration__Bus_CONV_abc12_out.zip')
        extract_dir = tmp_path / 'extract'

        outpaths = extract_outputs(str(storage), str(extract_dir), password=PASSWORD)

        expected = extract_dir / 'Declaration' / 'Bus_CONV_abc12'
        assert str(expected) in outpaths

    def test_skips_already_extracted_without_force_rerun(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'VehA_out.zip', content=b'new content')
        extract_dir = tmp_path / 'extract'
        existing = extract_dir / 'VehA'
        existing.mkdir(parents=True)
        (existing / 'old.txt').write_text('old content')

        extract_outputs(str(storage), str(extract_dir), force_rerun=False, password=PASSWORD)

        assert (existing / 'old.txt').is_file()
        assert not (existing / 'result.txt').exists()

    def test_force_rerun_overwrites_existing(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'VehA_out.zip')
        extract_dir = tmp_path / 'extract'
        existing = extract_dir / 'VehA'
        existing.mkdir(parents=True)
        (existing / 'old.txt').write_text('old content')

        extract_outputs(str(storage), str(extract_dir), force_rerun=True, password=PASSWORD)

        assert (existing / 'result.txt').is_file()

    def test_reference_and_current_do_not_collide_when_extracted_separately(self, tmp_path):
        cur_storage = tmp_path / 'cur_storage'
        cur_storage.mkdir()
        _write_zip(cur_storage / 'Declaration__Bus_CONV_abc12_out.zip', content=b'current result')

        ref_storage = tmp_path / 'ref_storage'
        ref_storage.mkdir()
        _write_zip(ref_storage / 'Declaration__Bus_CONV_abc12_REF_out.zip', content=b'reference result')

        cur_extract = tmp_path / 'extract' / '5.1.4-DEV'
        ref_extract = tmp_path / 'extract' / '5.1.3'

        extract_outputs(str(cur_storage), str(cur_extract), password=PASSWORD)
        extract_outputs(str(ref_storage), str(ref_extract), password=PASSWORD)

        cur_result = cur_extract / 'Declaration' / 'Bus_CONV_abc12' / 'result.txt'
        ref_result = ref_extract / 'Declaration' / 'Bus_CONV_abc12' / 'result.txt'
        assert cur_result.read_bytes() == b'current result'
        assert ref_result.read_bytes() == b'reference result'


class TestExtractOutputsPassword:
    def test_legacy_plain_zip_still_extracts(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        with zipfile.ZipFile(storage / 'VehA_out.zip', 'w') as zf:
            zf.writestr('result.txt', b'plain')
        extract_dir = tmp_path / 'extract'

        extract_outputs(str(storage), str(extract_dir), password=PASSWORD)

        assert (extract_dir / 'VehA' / 'result.txt').read_bytes() == b'plain'

    def test_wrong_password_raises(self, tmp_path):
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'VehA_out.zip')

        with pytest.raises(RuntimeError):
            extract_outputs(str(storage), str(tmp_path / 'extract'), password='wrong-password')

    def test_password_taken_from_environment_by_default(self, tmp_path, monkeypatch):
        monkeypatch.setenv(ZIP_PASSWORD_ENV, PASSWORD)
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'VehA_out.zip')
        extract_dir = tmp_path / 'extract'

        extract_outputs(str(storage), str(extract_dir))

        assert (extract_dir / 'VehA' / 'result.txt').is_file()

    def test_missing_password_environment_raises(self, tmp_path, monkeypatch):
        monkeypatch.delenv(ZIP_PASSWORD_ENV, raising=False)
        storage = tmp_path / 'storage'
        storage.mkdir()
        _write_zip(storage / 'VehA_out.zip')

        with pytest.raises(RuntimeError, match=ZIP_PASSWORD_ENV):
            extract_outputs(str(storage), str(tmp_path / 'extract'))
