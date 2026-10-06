"""Tests for issue-data version selection: readers use only the newest issue_txt_data version per issue."""

import json
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
sys.path.insert(0, os.path.dirname(__file__))

from FUNCTIONS.utils_generate_vecto_input2run import read_pool_stats, scan_pool
from FUNCTIONS.utils_pool import ISSUE_DESC_FILENAME, latest_issue_files
from pool_helpers import build_minimal_issue_json, make_fake_hash


def _write(raw_dir, iid, file_name, label='art10::approved', links=()):
    folder = raw_dir / f'{iid:04d}'
    folder.mkdir(parents=True, exist_ok=True)
    path = folder / file_name
    path.write_text(json.dumps(build_minimal_issue_json(iid, 'VIN123456789', label, list(links))), encoding='utf-8')
    return path


class TestLatestIssueFiles:
    def test_highest_version_per_issue(self, tmp_path):
        _write(tmp_path, 887, 'issue_txt_data_v01.json')
        newest = _write(tmp_path, 887, 'issue_txt_data_v02.json')
        other = _write(tmp_path, 1014, 'issue_txt_data_v01.json')

        assert latest_issue_files(tmp_path, ISSUE_DESC_FILENAME) == [newest, other]

    def test_version_ten_is_newer_than_version_nine(self, tmp_path):
        _write(tmp_path, 887, 'issue_txt_data_v09.json')
        newest = _write(tmp_path, 887, 'issue_txt_data_v10.json')

        assert latest_issue_files(tmp_path, ISSUE_DESC_FILENAME) == [newest]

    def test_unversioned_file_is_used_when_alone_and_older_than_any_version(self, tmp_path):
        alone = _write(tmp_path, 1, 'issue_txt_data.json')
        _write(tmp_path, 2, 'issue_txt_data.json')
        newest = _write(tmp_path, 2, 'issue_txt_data_v01.json')

        assert latest_issue_files(tmp_path, ISSUE_DESC_FILENAME) == [alone, newest]

    def test_missing_folder_gives_empty_list(self, tmp_path):
        assert latest_issue_files(tmp_path / 'missing', ISSUE_DESC_FILENAME) == []


class TestReadersUseLatestVersion:
    def _pool_with_two_versions(self, tmp_path):
        raw = tmp_path / 'A00_RAW'
        old_hash, new_hash = make_fake_hash(887, 'old.xml'), make_fake_hash(887, 'new.xml')
        _write(raw, 887, 'issue_txt_data_v01.json', label='art10::need_info',
               links=[f'/uploads/{old_hash}/old.xml'])
        _write(raw, 887, 'issue_txt_data_v02.json', label='art10::approved',
               links=[f'/uploads/{new_hash}/new.xml'])
        return raw, old_hash, new_hash

    def test_scan_pool_only_returns_links_of_latest_version(self, tmp_path):
        raw, _old_hash, new_hash = self._pool_with_two_versions(tmp_path)

        _vins, df_hash_sorted = scan_pool(raw, tmp_path / 'A01_EXTRACT', ISSUE_DESC_FILENAME)

        assert df_hash_sorted['gitlab_upload_hash'].tolist() == [new_hash]

    def test_read_pool_stats_uses_label_of_latest_version(self, tmp_path):
        raw, _old_hash, _new_hash = self._pool_with_two_versions(tmp_path)

        df_issues = read_pool_stats(raw, ISSUE_DESC_FILENAME)

        assert df_issues.loc[887, 'art10_label'] == 'art10::approved'
