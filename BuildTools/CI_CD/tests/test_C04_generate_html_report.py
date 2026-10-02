"""
Tests for C04_Generate_HTML_Report.py: summary cards and table rows carry
matching data-filter / data-status attributes, so clicking a card can filter
the results table to that category client-side.

C04 runs entirely at module import time (no main()), so `_run_c04` sets
sys.argv to a --report-dir pointing at a synthetic Reports/ folder before
importing, then reads the generated HTML from disk.
"""

import os
import re
import sys
import tempfile

import pandas as pd
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))


def _write_report(reports_dir, filename, df):
    os.makedirs(reports_dir, exist_ok=True)
    path = os.path.join(reports_dir, filename)
    with pd.ExcelWriter(path, engine='openpyxl') as writer:
        df.to_excel(writer, sheet_name='Success')
    return path


def _run_c04(report_dir):
    sys.modules.pop('C04_Generate_HTML_Report', None)
    old_argv = sys.argv
    sys.argv = ['C04_Generate_HTML_Report.py', '--report-dir', report_dir]
    try:
        import C04_Generate_HTML_Report as C04
    finally:
        sys.argv = old_argv
    return C04


def _read_generated_html(report_dir):
    html_dir = os.path.join(report_dir, 'Reports', 'HTML')
    files = [f for f in os.listdir(html_dir) if f.startswith('HTML_REPORT_')]
    assert len(files) == 1
    with open(os.path.join(html_dir, files[0]), encoding='utf-8') as f:
        return f.read()


_FULL_DF = pd.DataFrame({
    'Success':           ['OK', 'ERROR', 'ERROR Timeout', 'not run', 'OK', 'OK'],
    'vsum_ok':           [True, False, False, False, True, True],
    'log_ok':            [True, False, True, False, True, True],
    'rslt_mfr_ok':       [True, True, True, False, True, True],
    'complete_step_ok':  [True, True, True, False, True, True],
    'Delta Status':      ['OK', 'OK', 'WARNING 5%', float('nan'), 'OK', 'OK'],
    'Delta Status_MR':   ['OK', 'OK', 'OK', float('nan'), 'WARNING 3%', 'OK'],
    'Delta CRF':         ['OK', 'OK', 'OK', float('nan'), 'OK', 'WARNING 1%'],
    'Regression Status': ['OK', 'ERROR', 'OK', float('nan'), 'OK', 'OK'],
}, index=['VehA', 'VehB', 'VehC', 'VehD', 'VehE', 'VehF'])
_FULL_DF.index.name = 'Test_Name'

_PLAIN_DF = pd.DataFrame({
    'Success':          ['OK', 'ERROR', 'not run'],
    'vsum_ok':          [True, False, False],
    'log_ok':           [True, False, False],
    'rslt_mfr_ok':      [True, True, False],
    'complete_step_ok': [True, True, False],
}, index=['VehA', 'VehB', 'VehC'])
_PLAIN_DF.index.name = 'Test_Name'


class TestSummaryCardFilterAttributes:
    def test_all_eight_cards_present_when_delta_and_regression_columns_exist(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        expected_filters = [
            'total', 'passed', 'failed',
            'warn-delta', 'warn-delta-mr', 'warn-crf',
            'regression', 'not-run',
        ]
        for key in expected_filters:
            assert f'data-filter="{key}"' in html
            assert f"filterByStatus('{key}')" in html

    def test_card_counts_match_underlying_data(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        def _count_for(filter_key):
            m = re.search(
                rf'data-filter="{filter_key}"[^>]*><span class="num">(\d+)</span>',
                html,
            )
            assert m, f'card for {filter_key} not found'
            return int(m.group(1))

        assert _count_for('total') == 6
        assert _count_for('passed') == 3
        assert _count_for('failed') == 2
        assert _count_for('warn-delta') == 1
        assert _count_for('warn-delta-mr') == 1
        assert _count_for('warn-crf') == 1
        assert _count_for('regression') == 1
        assert _count_for('not-run') == 1

    def test_only_four_unconditional_cards_when_delta_and_regression_absent(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_def5678.xlsx', _PLAIN_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        for key in ['total', 'passed', 'failed', 'not-run']:
            assert f'data-filter="{key}"' in html
        for key in ['warn-delta', 'warn-delta-mr', 'warn-crf', 'regression']:
            assert f'data-filter="{key}"' not in html


class TestRowStatusAttributes:
    def test_passed_row_has_passed_token_only(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        assert '<td>VehA</td>' in html
        veh_a_row = [line for line in html.splitlines() if '>VehA<' in line][0]
        assert 'data-status="passed"' in veh_a_row

    def test_row_carries_multiple_status_tokens(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        veh_b_row = [line for line in html.splitlines() if 'VehB' in line][0]
        assert 'data-status="failed regression"' in veh_b_row

        veh_c_row = [line for line in html.splitlines() if 'VehC' in line][0]
        assert 'data-status="failed warn-delta"' in veh_c_row

    def test_not_run_row_has_not_run_token(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        veh_d_row = [line for line in html.splitlines() if 'VehD' in line][0]
        assert 'data-status="not-run"' in veh_d_row

    def test_row_count_per_status_token_matches_card_count(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        # every row carrying "failed" in its data-status must equal the Failed card's count
        failed_rows = len(re.findall(r'data-status="[^"]*\bfailed\b[^"]*"', html))
        assert failed_rows == 2


class TestFilterJavaScript:
    def test_apply_filters_and_filter_by_status_functions_present(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        assert 'function applyFilters()' in html
        assert 'function filterByStatus(key)' in html
        assert 'function filterTable()' in html
        assert 'function sortTable(col)' in html

    def test_filter_table_delegates_to_apply_filters(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        filter_table_body = re.search(r'function filterTable\(\) \{([^}]*)\}', html).group(1)
        assert 'applyFilters()' in filter_table_body

    def test_card_active_css_rule_present(self, tmp_path):
        _write_report(str(tmp_path / 'Reports'), 'OR_5.1.1_260101_0001_abc1234.xlsx', _FULL_DF)
        _run_c04(str(tmp_path))

        html = _read_generated_html(str(tmp_path))

        assert '.card-active' in html
        assert 'cursor: pointer' in html
