# -*- coding: utf-8 -*-
"""Unit tests for FUNCTIONS/REPORTS.py version/hash/report-selection helpers."""

import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from FUNCTIONS.REPORTS import get_latest_release_report, get_vecto_hash


def _init_git_repo(path):
    subprocess.run(['git', 'init', '-q'], cwd=path, check=True)
    subprocess.run(['git', 'config', 'user.email', 'test@example.com'], cwd=path, check=True)
    subprocess.run(['git', 'config', 'user.name', 'Test'], cwd=path, check=True)
    subprocess.run(['git', 'commit', '--allow-empty', '-q', '-m', 'init'], cwd=path, check=True)


class TestGetVectoHash:
    def test_folder_outside_git_returns_unknown(self, tmp_path):
        folder = tmp_path / 'vecto_install'
        folder.mkdir()

        result = get_vecto_hash(str(folder))

        assert result == 'unknown'

    def test_folder_inside_git_returns_short_hash(self, tmp_path):
        _init_git_repo(tmp_path)

        result = get_vecto_hash(str(tmp_path))

        assert re.fullmatch(r'[0-9a-f]{7,}', result)


def _touch_reports(folder, names):
    folder.mkdir(parents=True, exist_ok=True)
    for name in names:
        (folder / name).write_bytes(b'')
    return str(folder)


_EXAMPLE_REPORTS = [
    'OR_5.1.1_206012_0001_ff0471b79.xlsx',
    'OR_5.1.3_260905_0001_0f4d410965.xlsx',
    'OR_5.1.4-DEV_260926_0001_a699fcb2.xlsx',
]


class TestGetLatestReleaseReport:
    def test_mr_cache_ranks_highest_version_first(self, tmp_path):
        reports_dir = _touch_reports(tmp_path / 'MR', _EXAMPLE_REPORTS)

        result = get_latest_release_report(reports_dir, allow_prerelease=True)

        assert os.path.basename(result) == 'OR_5.1.4-DEV_260926_0001_a699fcb2.xlsx'

    def test_reference_cache_ignores_rc_and_dev(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, _EXAMPLE_REPORTS)

        result = get_latest_release_report(reports_dir)

        assert os.path.basename(result) == 'OR_5.1.3_260905_0001_0f4d410965.xlsx'

    def test_same_version_newest_date_wins(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, [
            'OR_5.1.4-DEV_260926_0001_a699fcb2.xlsx',
            'OR_5.1.4-DEV_260927_0001_a699fcb2.xlsx',
        ])

        result = get_latest_release_report(reports_dir, allow_prerelease=True)

        assert os.path.basename(result) == 'OR_5.1.4-DEV_260927_0001_a699fcb2.xlsx'

    def test_same_version_and_date_highest_nnnn_wins(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, [
            'OR_5.1.3_260905_0002_0f4d410965.xlsx',
            'OR_5.1.3_260905_0001_0f4d410965.xlsx',
        ])

        result = get_latest_release_report(reports_dir)

        assert os.path.basename(result) == 'OR_5.1.3_260905_0002_0f4d410965.xlsx'

    def test_same_numeric_version_official_beats_rc_beats_dev(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, [
            'OR_5.1.4-DEV_260930_0003_aaaaaaa.xlsx',
            'OR_5.1.4-RC_260929_0002_bbbbbbb.xlsx',
            'OR_5.1.4_260901_0001_ccccccc.xlsx',
        ])

        assert os.path.basename(get_latest_release_report(reports_dir, allow_prerelease=True))             == 'OR_5.1.4_260901_0001_ccccccc.xlsx'

        os.remove(os.path.join(reports_dir, 'OR_5.1.4_260901_0001_ccccccc.xlsx'))

        assert os.path.basename(get_latest_release_report(reports_dir, allow_prerelease=True))             == 'OR_5.1.4-RC_260929_0002_bbbbbbb.xlsx'

    def test_versions_compare_numerically(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, [
            'OR_5.1.9_260901_0001_aaaaaaa.xlsx',
            'OR_5.1.10_260801_0001_bbbbbbb.xlsx',
        ])

        result = get_latest_release_report(reports_dir)

        assert os.path.basename(result) == 'OR_5.1.10_260801_0001_bbbbbbb.xlsx'

    def test_matching_release_tag_wins_over_higher_version(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, _EXAMPLE_REPORTS)

        result = get_latest_release_report(reports_dir, release_tag='Release/v5.1.1')

        assert os.path.basename(result) == 'OR_5.1.1_206012_0001_ff0471b79.xlsx'

    def test_release_tag_without_report_falls_back_to_ranking(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, _EXAMPLE_REPORTS)

        result = get_latest_release_report(reports_dir, release_tag='Release/v5.1.5-RC')

        assert os.path.basename(result) == 'OR_5.1.3_260905_0001_0f4d410965.xlsx'

    def test_detail_crf_and_unknown_reports_are_ignored(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, [
            'OR_5.1.3_260905_0001_0f4d410965.xlsx',
            'OR_5.1.9_260905_0001_0f4d410965_DETAIL.xlsx',
            'OR_5.1.9_260905_0001_0f4d410965_CRF.xlsx',
            'OR_unknown_260905_0009_0f4d410965.xlsx',
        ])

        result = get_latest_release_report(reports_dir)

        assert os.path.basename(result) == 'OR_5.1.3_260905_0001_0f4d410965.xlsx'

    def test_reference_cache_with_only_prereleases_returns_none(self, tmp_path):
        reports_dir = _touch_reports(tmp_path, ['OR_5.1.4-DEV_260926_0001_a699fcb2.xlsx'])

        assert get_latest_release_report(reports_dir) is None

    def test_empty_or_missing_dir_returns_none(self, tmp_path):
        assert get_latest_release_report(str(tmp_path)) is None
        assert get_latest_release_report(str(tmp_path / 'missing')) is None
