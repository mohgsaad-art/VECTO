# -*- coding: utf-8 -*-
"""Unit tests for FUNCTIONS/SECURE_ZIP.py (password-protected job output zips)."""

import os
import sys
import zipfile

import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from FUNCTIONS.SECURE_ZIP import ZIP_PASSWORD_ENV, extract_zip, get_zip_password, write_encrypted_zip

PASSWORD = 'test-password'


def _make_tree(root):
    (root / 'sub').mkdir(parents=True)
    (root / 'job.vsum').write_text('vsum content', encoding='utf-8')
    (root / 'sub' / 'vehicle.xml').write_text('<Root/>', encoding='utf-8')


def _read_tree(root):
    return {
        os.path.relpath(os.path.join(d, f), root).replace(os.sep, '/'): open(os.path.join(d, f), encoding='utf-8').read()
        for d, _, files in os.walk(root) for f in files
    }


class TestGetZipPassword:
    def test_returns_value_when_set(self, monkeypatch):
        monkeypatch.setenv(ZIP_PASSWORD_ENV, PASSWORD)

        assert get_zip_password() == PASSWORD

    @pytest.mark.parametrize('value', [None, ''])
    def test_missing_or_empty_raises(self, monkeypatch, value):
        if value is None:
            monkeypatch.delenv(ZIP_PASSWORD_ENV, raising=False)
        else:
            monkeypatch.setenv(ZIP_PASSWORD_ENV, value)

        with pytest.raises(RuntimeError, match=ZIP_PASSWORD_ENV):
            get_zip_password()


class TestEncryptedZip:
    def test_round_trip_keeps_files_and_tree(self, tmp_path):
        src, out = tmp_path / 'src', tmp_path / 'out'
        _make_tree(src)
        archive = tmp_path / 'case_out.zip'

        write_encrypted_zip(src, archive, PASSWORD)
        extract_zip(archive, out, PASSWORD)

        assert _read_tree(out) == _read_tree(src)

    def test_archive_cannot_be_read_without_password(self, tmp_path):
        src = tmp_path / 'src'
        _make_tree(src)
        archive = tmp_path / 'case_out.zip'

        write_encrypted_zip(src, archive, PASSWORD)

        with zipfile.ZipFile(archive) as zf:
            assert all(info.flag_bits & 0x1 for info in zf.infolist())
            with pytest.raises((RuntimeError, NotImplementedError)):
                zf.read('job.vsum')

    def test_wrong_password_raises(self, tmp_path):
        src = tmp_path / 'src'
        _make_tree(src)
        archive = tmp_path / 'case_out.zip'
        write_encrypted_zip(src, archive, PASSWORD)

        with pytest.raises(RuntimeError):
            extract_zip(archive, tmp_path / 'out', 'wrong-password')

    def test_plain_zip_still_extracts(self, tmp_path):
        archive = tmp_path / 'legacy_out.zip'
        with zipfile.ZipFile(archive, 'w') as zf:
            zf.writestr('job.vsum', 'vsum content')

        extract_zip(archive, tmp_path / 'out', PASSWORD)

        assert (tmp_path / 'out' / 'job.vsum').read_text(encoding='utf-8') == 'vsum content'
