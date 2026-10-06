"""
Tests for C44_Generate_Delta_Report.py: build_var_deltas / load_var_series_wkey
/ build_delta_summary, the functions used by generate_delta_report and
append_delta_to_report.
"""

import os
import sys

import pandas as pd
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
from C44_Generate_Delta_Report import (
    _DELTA_THRESHOLD,
    build_delta_summary,
    build_var_deltas,
    load_var_series_wkey,
)

_VAR = 'FC-Final [g/km]'
_SHEET = 'FC-Final__g_km'
_KEYS = [
    'case', 'Cycle [-]', 'HDV CO2 vehicle class [-]',
    'Corrected Actual Curb Mass [kg]', 'Loading [kg]',
    'Vehicle fuel type [-]', 'Ovc Mode [-]', 'Job [-]',
]


def _row(case, value, *, cycle='CycA', job='1-1', ovc_mode='NotOvc',
         hdv_class='N2', mass=8000, loading=0, fuel='Diesel CI', status='Success'):
    return {
        'case': case, _VAR: value, 'Status': status,
        'Cycle [-]': cycle, 'Job [-]': job, 'Ovc Mode [-]': ovc_mode,
        'HDV CO2 vehicle class [-]': hdv_class,
        'Corrected Actual Curb Mass [kg]': mass,
        'Loading [kg]': loading,
        'Vehicle fuel type [-]': fuel,
    }


def _write_report(path: str, rows: list[dict]) -> None:
    df = pd.DataFrame(rows)
    with pd.ExcelWriter(path, engine='openpyxl') as writer:
        df.to_excel(writer, sheet_name=_SHEET, index=False)


class TestLoadVarSeriesWkey:
    def test_returns_series_indexed_by_keys(self, tmp_path):
        report = str(tmp_path / 'ref.xlsx')
        _write_report(report, [_row('VehA', 100.0, job='1-1'), _row('VehA', 200.0, job='1-2')])

        result = load_var_series_wkey(report, _VAR, key=_KEYS, filter_success=True)

        assert result is not None
        assert list(result.index.names) == _KEYS
        assert len(result) == 2

    def test_missing_sheet_returns_none(self, tmp_path):
        report = str(tmp_path / 'empty.xlsx')
        pd.DataFrame({'x': [1]}).to_excel(report, sheet_name='Unrelated', index=False)

        result = load_var_series_wkey(report, _VAR, key=_KEYS, filter_success=True)

        assert result is None

    def test_filter_success_drops_non_success_rows(self, tmp_path):
        report = str(tmp_path / 'ref.xlsx')
        _write_report(report, [
            _row('VehA', 100.0, job='1-1', status='Success'),
            _row('VehA', 999.0, job='1-2', status='Aborted'),
        ])

        result = load_var_series_wkey(report, _VAR, key=_KEYS, filter_success=True)

        assert len(result) == 1
        assert result.iloc[0] == 100.0

    def test_missing_key_column_returns_none(self, tmp_path):
        report = str(tmp_path / 'ref.xlsx')
        rows = [_row('VehA', 100.0)]
        for r in rows:
            del r['Job [-]']
        _write_report(report, rows)

        result = load_var_series_wkey(report, _VAR, key=_KEYS, filter_success=True)

        assert result is None


class TestBuildVarDeltas:
    def test_picks_max_delta_pct_pos_case(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1'),
            _row('VehA', 100.0, cycle='CycB', job='1-2'),
        ])
        _write_report(cur_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1'),
            _row('VehA', 110.0, cycle='CycB', job='1-2'),
        ])

        var_deltas = build_var_deltas(ref_path, cur_path)

        assert _VAR in var_deltas
        delta_pct = var_deltas[_VAR].loc['VehA', 'delta_pct']
        assert abs(delta_pct) == pytest.approx(10.0, abs=0.001)

    def test_matches_by_key_not_row_position(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1'),
            _row('VehA', 200.0, cycle='CycB', job='1-2'),
        ])
        _write_report(cur_path, [
            _row('VehA', 210.0, cycle='CycB', job='1-2'),
            _row('VehA', 101.0, cycle='CycA', job='1-1'),
        ])

        var_deltas = build_var_deltas(ref_path, cur_path)

        delta_pct = var_deltas[_VAR].loc['VehA', 'delta_pct']
        assert abs(delta_pct) == pytest.approx(5.0, abs=0.1)

    def test_zero_reference_value_does_not_crash(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [_row('VehA', 0.0)])
        _write_report(cur_path, [_row('VehA', 0.0)])

        var_deltas = build_var_deltas(ref_path, cur_path)

        assert _VAR in var_deltas

    def test_multiple_vehicles_independent_max(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1'),
            _row('VehA', 100.0, cycle='CycB', job='1-2'),
            _row('VehB', 200.0, cycle='CycA', job='1-1'),
            _row('VehB', 200.0, cycle='CycB', job='1-2'),
        ])
        _write_report(cur_path, [
            _row('VehA', 101.0, cycle='CycA', job='1-1'),
            _row('VehA', 103.0, cycle='CycB', job='1-2'),
            _row('VehB', 200.0, cycle='CycA', job='1-1'),
            _row('VehB', 210.0, cycle='CycB', job='1-2'),
        ])

        var_deltas = build_var_deltas(ref_path, cur_path)

        df = var_deltas[_VAR]
        assert abs(df.loc['VehA', 'delta_pct']) == pytest.approx(3.0, abs=0.001)
        assert abs(df.loc['VehB', 'delta_pct']) == pytest.approx(5.0, abs=0.001)

    def test_charge_sustaining_non_final_job_excluded_from_selection(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1', ovc_mode='ChargeSustaining'),
            _row('VehA', 100.0, cycle='CycB', job='1-F', ovc_mode='ChargeSustaining'),
        ])
        _write_report(cur_path, [
            _row('VehA', 200.0, cycle='CycA', job='1-1', ovc_mode='ChargeSustaining'),
            _row('VehA', 102.0, cycle='CycB', job='1-F', ovc_mode='ChargeSustaining'),
        ])

        var_deltas = build_var_deltas(ref_path, cur_path)

        delta_pct = var_deltas[_VAR].loc['VehA', 'delta_pct']
        assert abs(delta_pct) == pytest.approx(2.0, abs=0.001)

    def test_missing_sheet_omits_variable(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')
        pd.DataFrame({'x': [1]}).to_excel(ref_path, sheet_name='Unrelated', index=False)
        pd.DataFrame({'x': [1]}).to_excel(cur_path, sheet_name='Unrelated', index=False)

        var_deltas = build_var_deltas(ref_path, cur_path)

        assert var_deltas == {}


class TestBuildDeltaSummary:
    def test_warns_when_delta_exceeds_threshold(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')
        threshold_pct = _DELTA_THRESHOLD * 100
        big_delta = threshold_pct * 10

        _write_report(ref_path, [_row('VehA', 100.0)])
        _write_report(cur_path, [_row('VehA', 100.0 * (1 + big_delta / 100))])

        var_deltas = build_var_deltas(ref_path, cur_path)
        df_summary = build_delta_summary(var_deltas)

        assert df_summary.loc['VehA', 'status'] == 'WARNING'

    def test_ok_when_delta_within_threshold(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [_row('VehA', 100.0)])
        _write_report(cur_path, [_row('VehA', 100.0)])

        var_deltas = build_var_deltas(ref_path, cur_path)
        df_summary = build_delta_summary(var_deltas)

        assert df_summary.loc['VehA', 'status'] == 'OK'

    def test_empty_var_deltas_returns_empty_dataframe(self):
        assert build_delta_summary({}).empty


class TestJobAwareDeltas:
    def test_differing_jobs_inner_join_only_matches_common_job(self, tmp_path):
        ref_path = str(tmp_path / 'ref.xlsx')
        cur_path = str(tmp_path / 'cur.xlsx')

        _write_report(ref_path, [
            _row('VehA', 100.0, cycle='CycA', job='1-1'),
            _row('VehA', 200.0, cycle='CycB', job='1-2'),
        ])
        _write_report(cur_path, [
            _row('VehA', 105.0, cycle='CycA', job='1-1'),
        ])

        var_deltas = build_var_deltas(ref_path, cur_path)

        delta_pct = var_deltas[_VAR].loc['VehA', 'delta_pct']
        assert abs(delta_pct) == pytest.approx(5.0, abs=0.001)


_REF_REPORT = (
    r'C:\Users\David Amo\Documents\EC\BugReports\2001_FuegoCI'
    r'\Reports\Release_Reference\Reports\OR_5.1.1-DEV_260602_0001_e4dab35a.xlsx'
)
_CUR_REPORT = (
    r'C:\Users\David Amo\Documents\EC\BugReports\2001_FuegoCI'
    r'\Reports\20260610_DeltaFix\Reports\OR_5.1.1-DEV_260610_0001_4538b8f.xlsx'
)
_REPORTS_PRESENT = os.path.isfile(_REF_REPORT) and os.path.isfile(_CUR_REPORT)


@pytest.mark.skipif(not _REPORTS_PRESENT, reason='Real OR reports not available on this machine')
class TestRealReportRegression:
    def test_fchv_vehicle_no_warning(self):
        # Runs under a different Job in ref vs current (1-1 vs 3-1), so the
        # key-based join finds no matching rows - must not show as WARNING.
        var_deltas = build_var_deltas(_REF_REPORT, _CUR_REPORT)
        df_summary = build_delta_summary(var_deltas)

        vehicle = 'Declaration__Bus_FCHVovc_10426_e903f'
        if vehicle in df_summary.index:
            assert df_summary.loc[vehicle, 'status'] == 'OK'
