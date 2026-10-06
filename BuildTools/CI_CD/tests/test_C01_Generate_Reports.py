"""
Tests for C01_Generate_Reports.py.

C01 resolves CLI args and report metadata at module import time (not inside
main()), so `_import_c01` sets sys.argv/cwd to a harmless tmp dir before
importing. All fixture content is placeholder/synthetic.
"""

import json
import os
import sys
import tempfile

import pandas as pd
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))


def _import_c01():
    sys.modules.pop('C01_Generate_Reports', None)
    tmp_root = tempfile.mkdtemp()
    old_argv, old_cwd = sys.argv, os.getcwd()
    sys.argv = ['C01_Generate_Reports.py', tmp_root, '--report-dir', tmp_root]
    os.chdir(tmp_root)
    try:
        import C01_Generate_Reports as C01
    finally:
        sys.argv = old_argv
        os.chdir(old_cwd)
    return C01


@pytest.fixture(scope='module')
def C01():
    return _import_c01()


def _init_git_repo(path):
    """get_vecto_hash runs `git rev-parse --short HEAD` against the given
    root - give it a real (if minimal) repo so it returns an actual hash
    instead of git's "not a git repository" error text."""
    import subprocess
    subprocess.run(['git', 'init', '-q'], cwd=path, check=True)
    subprocess.run(['git', 'config', 'user.email', 'test@example.com'], cwd=path, check=True)
    subprocess.run(['git', 'config', 'user.name', 'Test'], cwd=path, check=True)
    subprocess.run(['git', 'commit', '--allow-empty', '-q', '-m', 'init'], cwd=path, check=True)


def _import_c01_with_ref(ref_vecto_root, cwd):
    sys.modules.pop('C01_Generate_Reports', None)
    vecto_root = os.path.join(cwd, 'vecto_root')
    os.makedirs(vecto_root, exist_ok=True)
    argv = ['C01_Generate_Reports.py', vecto_root, '--report-dir', cwd,
            '--ref-vecto-root', ref_vecto_root]

    old_argv, old_cwd = sys.argv, os.getcwd()
    sys.argv = argv
    os.chdir(cwd)
    try:
        import C01_Generate_Reports as C01
    finally:
        sys.argv = old_argv
        os.chdir(old_cwd)
    return C01


_VSUM_COMMENT_LINE = '# VECTO-DEV 5.1.3-DEV - 01.01.2026 00:00'
_VSUM_HEADER = 'Job [-],Input File [-],Cycle [-],Status,CO2 [g/km],FC-Final [g/km],EC_el_final [kWh/km]'
_VSUM_FOOTER = '#@ SHA256: deadbeef='
_OK_LOG_TAIL = 'Simulation runs finished in 10.0 seconds'


def _vsum_text(rows):
    lines = [_VSUM_COMMENT_LINE, _VSUM_HEADER]
    for job, input_file, cycle, status, co2 in rows:
        lines.append(f'{job},{input_file},{cycle},{status},{co2},{co2 * 0.5},0.0')
    lines.append(_VSUM_FOOTER)
    return '\n'.join(lines) + '\n'


def _write_result_dir(base, rel_path, *, log_lines, vsum_rows, xml_name='vehicle.xml',
                       vecto_json=None, rslt_manufacturer_ok=None, crf_xml=None):
    d = base / rel_path
    d.mkdir(parents=True, exist_ok=True)
    if vecto_json is not None:
        (d / 'job.vecto').write_text(json.dumps(vecto_json))
    else:
        (d / xml_name).write_bytes(b'<Root/>')
    (d / 'job.log').write_text('\n'.join(log_lines) + '\n', encoding='utf-8')
    (d / 'job.vsum').write_text(_vsum_text(vsum_rows), encoding='utf-8')
    if rslt_manufacturer_ok is not None:
        content = 'OK\n' if rslt_manufacturer_ok else 'Error: something broke\n'
        (d / 'job.RSLT_MANUFACTURER.xml').write_text(content, encoding='utf-8')
    if crf_xml is not None:
        (d / 'job.RSLT_CUSTOMER.xml').write_bytes(crf_xml)
    return d


_CRF_XML = b'''<?xml version="1.0"?>
<Report xmlns:vtp="urn:tugraz:ivt:VectoAPI:DeclarationOutput:v3.0">
  <VehicleGroup>N2</VehicleGroup>
  <Model>TruckA</Model>
  <TechnicalPermissibleMaximumLadenMass>18000</TechnicalPermissibleMaximumLadenMass>
  <Summary>
    <Vocational>false</Vocational>
    <AveragePayload>4.5</AveragePayload>
    <FuelConsumption unit="g/km">650.0</FuelConsumption>
    <FuelConsumption unit="MJ/km">9.5</FuelConsumption>
    <CO2 unit="g/km">2050.0</CO2>
  </Summary>
</Report>
'''


class TestBuildSummaryDataframeLogVsumMatch:
    def test_success_success_agreement_is_ok(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == True
        assert df.loc[0, 'Success'] == 'OK'
        assert failed == []

    def test_failure_failure_agreement_is_ok_but_overall_error(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseB',
            log_lines=['VIN1 Urban _ReferenceLoad  88.22%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == True
        assert df.loc[0, 'vsum_ok'] == False
        assert df.loc[0, 'Success'] == 'ERROR'
        assert failed == ['CaseB']

    def test_unexplained_non_success_is_flagged(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseC',
            log_lines=['VIN1 Urban _ReferenceLoad  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == False
        assert df.loc[0, 'Success'] == 'ERROR'

    def test_missing_vsum_row_is_flagged_even_when_log_below_100(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseD',
            log_lines=[
                'VIN1 HeavyUrban _LowLoading  100.00%  10.00s',
                'VIN1 HeavyUrban _ReferenceLoad  99.00%  10.00s',
            ],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == False

    def test_vsum_success_authoritative_over_log_below_100(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseE',
            log_lines=['job LongHaul pre  99.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'job', 'LongHaul.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == True
        assert df.loc[0, 'Success'] == 'OK'

    def test_no_progress_lines_falls_back_to_error(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseF',
            log_lines=['Reading job: foo.vecto'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'log_ok'] == False
        assert df.loc[0, 'Success'] == 'ERROR'


class TestBuildSummaryDataframeClassification:
    def test_no_vsum_no_log_is_not_run(self, tmp_path, C01):
        d = tmp_path / 'CaseNoRun'
        d.mkdir()
        (d / 'vehicle.xml').write_bytes(b'<Root/>')

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Success'] == 'not run'
        assert failed == []

    def test_no_vsum_with_log_is_error_no_vsum(self, tmp_path, C01):
        d = tmp_path / 'CaseNoVsum'
        d.mkdir()
        (d / 'vehicle.xml').write_bytes(b'<Root/>')
        (d / 'job.log').write_text('Reading job: foo.vecto\n', encoding='utf-8')

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Success'] == 'ERROR no vsum'
        assert failed == ['CaseNoVsum']

    def test_empty_vsum_is_error_empty_vsum(self, tmp_path, C01):
        d = tmp_path / 'CaseEmptyVsum'
        d.mkdir()
        (d / 'vehicle.xml').write_bytes(b'<Root/>')
        (d / 'job.log').write_text('Reading job: foo.vecto\n', encoding='utf-8')
        (d / 'job.vsum').write_text('', encoding='utf-8')

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Success'] == 'ERROR empty vsum'

    def test_rslt_manufacturer_error_line_flags_rslt_mfr_ok_false(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseRslt',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            rslt_manufacturer_ok=False,
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'rslt_mfr_ok'] == False
        assert df.loc[0, 'Success'] == 'ERROR VIF'
        assert failed == ['CaseRslt']

    def test_rslt_manufacturer_ok_line_keeps_success(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseRsltOk',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            rslt_manufacturer_ok=True,
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'rslt_mfr_ok'] == True
        assert df.loc[0, 'Success'] == 'OK'

    def test_cpu_time_extracted_from_log(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseCpu',
            log_lines=[
                'VIN1 HeavyUrban _LowLoading  100.00%  10.00s',
                'Simulation runs finished in 42.5 seconds',
            ],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'cpu_time'] == 42.5

    def test_test_name_spaces_replaced_with_underscore(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'Case With Spaces',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Test_Name'] == 'Case_With_Spaces'

    def test_multiple_result_dirs_all_classified(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'Good',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        _write_result_dir(
            tmp_path, 'Bad',
            log_lines=['VIN1 Urban _ReferenceLoad  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert len(df) == 2
        assert set(df['Test_Name']) == {'Good', 'Bad'}
        assert failed == ['Bad']


_V10_JOB = {'Header': {'FileVersion': 10},
            'Body': {'PrimaryVehicle': 'primary.xml', 'InterimStep': 'completed.xml'}}


class TestBuildSummaryDataframeCompleteStep:
    def _write_v10_case(self, base, run_simulation):
        job = json.loads(json.dumps(_V10_JOB))
        job['Body']['RunSimulation'] = run_simulation
        d = _write_result_dir(
            base, 'CaseBus',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            vecto_json=job,
        )
        (d / 'job.RSLT_MANUFACTURER_PRIMARY.xml').write_text('OK\n', encoding='utf-8')

    def test_run_simulation_true_without_final_mrf_is_error(self, tmp_path, C01):
        self._write_v10_case(tmp_path, run_simulation=True)

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'complete_step_ok'] == False
        assert df.loc[0, 'Success'] == 'ERROR'
        assert failed == ['CaseBus']

    def test_run_simulation_false_without_final_mrf_is_ok(self, tmp_path, C01):
        self._write_v10_case(tmp_path, run_simulation=False)

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'complete_step_ok'] == True
        assert df.loc[0, 'Success'] == 'OK'


_V7_JOB = {'Header': {'FileVersion': 7},
           'Body': {'PrimaryVehicleResults': 'vif.xml', 'CompletedVehicle': 'completed.xml'}}


class TestBuildSummaryDataframeCompleteStepV7:
    def _write_v7_case(self, base, with_final_mrf):
        return _write_result_dir(
            base, 'CaseBusVif',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            vecto_json=_V7_JOB,
            rslt_manufacturer_ok=True if with_final_mrf else None,
        )

    def test_without_final_mrf_is_error(self, tmp_path, C01):
        self._write_v7_case(tmp_path, with_final_mrf=False)

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'complete_step_ok'] == False
        assert df.loc[0, 'Success'].startswith('ERROR')
        assert failed == ['CaseBusVif']

    def test_with_final_mrf_is_ok(self, tmp_path, C01):
        self._write_v7_case(tmp_path, with_final_mrf=True)

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'complete_step_ok'] == True
        assert df.loc[0, 'Success'] == 'OK'
        assert failed == []


class TestBuildSummaryDataframeTimeLimit:
    # Test names must not contain the word checked for: pytest puts the test name in tmp_path.
    def test_time_limit_message_in_log_is_error_timeout(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseLimit',
            log_lines=['VIN1 HeavyUrban _LowLoading  42.00%  10.00s', 'Simulation aborted: Timeout after 3600 s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Aborted', 0.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Success'] == 'ERROR Timeout'
        assert failed == ['CaseLimit']

    def test_word_only_in_result_path_is_plain_error(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'Case_timeout_dir',
            log_lines=['VIN1 Urban _ReferenceLoad  88.22%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )

        df, failed = C01.build_summary_dataframe(str(tmp_path))

        assert df.loc[0, 'Success'] == 'ERROR'
        assert df.loc[0, 'cpu_time'] == 0.0


class TestWriteSummaryReport:
    def test_writes_success_and_cpu_time_sheets(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        df, failed = C01.build_summary_dataframe(str(tmp_path))
        out_path = tmp_path / 'out.xlsx'

        with pd.ExcelWriter(out_path, engine='openpyxl') as writer:
            C01.write_summary_report(writer, df, failed)

        xls = pd.ExcelFile(out_path)
        assert 'Success' in xls.sheet_names
        assert 'cpu_time' in xls.sheet_names
        success_df = xls.parse('Success', index_col=0)
        assert 'CaseA' in success_df.index


class TestBuildVsumDataframe:
    def test_collects_vsum_rows_with_case_name(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df = C01.build_vsum_dataframe(str(tmp_path))

        assert df is not None
        assert (df['case'] == 'CaseA').all()

    def test_no_vsum_files_returns_none(self, tmp_path, C01):
        d = tmp_path / 'NoVsum'
        d.mkdir()
        (d / 'vehicle.xml').write_bytes(b'<Root/>')

        df = C01.build_vsum_dataframe(str(tmp_path))

        assert df is None

    def test_eng_case_name_sets_vehicle_class_placeholder(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'GenericEngineering__ENG__TruckA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df = C01.build_vsum_dataframe(str(tmp_path))

        assert (df['HDV CO2 vehicle class [-]'] == '-').all()


class TestWriteVsumReport:
    def test_writes_one_sheet_per_variable_plus_stats(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        df_vsum = C01.build_vsum_dataframe(str(tmp_path))
        out_path = tmp_path / 'out.xlsx'

        with pd.ExcelWriter(out_path, engine='openpyxl') as writer:
            C01.write_vsum_report(writer, df_vsum)

        xls = pd.ExcelFile(out_path)
        assert 'stats of non-zero variations' in xls.sheet_names
        assert any('CO2' in s for s in xls.sheet_names)


class TestBuildCrfDataframe:
    def test_collects_crf_summary_when_log_ok(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            crf_xml=_CRF_XML,
        )

        df = C01.build_crf_dataframe(str(tmp_path))

        assert df is not None
        assert df.loc[0, 'VehicleGroup'] == 'N2'
        assert df.loc[0, 'TPMLM [kg]'] == 18000

    def test_skipped_when_log_vsum_mismatch(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseBad',
            log_lines=['VIN1 Urban _ReferenceLoad  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
            crf_xml=_CRF_XML,
        )

        df = C01.build_crf_dataframe(str(tmp_path))

        assert df is None

    def test_missing_crf_file_skipped(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseNoCrf',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )

        df = C01.build_crf_dataframe(str(tmp_path))

        assert df is None


class TestWriteCrfReport:
    def test_writes_crf_sheets(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
            crf_xml=_CRF_XML,
        )
        df_crf = C01.build_crf_dataframe(str(tmp_path))
        out_path = tmp_path / 'out.xlsx'

        with pd.ExcelWriter(out_path, engine='openpyxl') as writer:
            C01.write_crf_report(writer, df_crf)

        xls = pd.ExcelFile(out_path)
        assert any(s.startswith('CRF_') for s in xls.sheet_names)


class TestWriteReport:
    def test_produces_summary_sheet_at_minimum(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        out_path = tmp_path / 'OR_out.xlsx'

        failed_names = C01.write_report(str(tmp_path), str(out_path))

        assert out_path.is_file()
        assert failed_names == []
        xls = pd.ExcelFile(out_path)
        assert 'Success' in xls.sheet_names

    def test_returns_failed_names(self, tmp_path, C01):
        _write_result_dir(
            tmp_path, 'CaseBad',
            log_lines=['VIN1 Urban _ReferenceLoad  100.00%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )
        out_path = tmp_path / 'OR_out.xlsx'

        failed_names = C01.write_report(str(tmp_path), str(out_path))

        assert failed_names == ['CaseBad']


class TestRefreshRefReport:
    def test_writes_report_from_ref_version_results(self, tmp_path, monkeypatch):
        ref_vecto_root = tmp_path / 'ref_vecto'
        ref_vecto_root.mkdir()
        _init_git_repo(str(ref_vecto_root))
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        _write_result_dir(
            cwd / 'XX_RESULTS' / 'unknown', 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        monkeypatch.chdir(cwd)

        C01.refresh_ref_report()

        reports = list((cwd / 'Reports').glob('OR_unknown_*.xlsx'))
        assert len(reports) == 1

    def test_no_ref_vecto_root_is_noop(self, tmp_path, capsys):
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        C01 = _import_c01_with_ref(str(tmp_path / 'ref_vecto'), cwd=str(cwd))
        C01.args.ref_vecto_root = None

        C01.refresh_ref_report()

        assert 'not supplied' in capsys.readouterr().out

    def test_missing_result_dir_is_noop(self, tmp_path, capsys, monkeypatch):
        ref_vecto_root = tmp_path / 'ref_vecto'
        ref_vecto_root.mkdir()
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        monkeypatch.chdir(cwd)

        C01.refresh_ref_report()

        assert list((cwd / 'Reports').glob('OR_*.xlsx')) == []
        assert 'not found' in capsys.readouterr().out

    def test_removes_old_report_for_same_version(self, tmp_path, monkeypatch):
        ref_vecto_root = tmp_path / 'ref_vecto'
        ref_vecto_root.mkdir()
        _init_git_repo(str(ref_vecto_root))
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        _write_result_dir(
            cwd / 'XX_RESULTS' / 'unknown', 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        reports_dir = cwd / 'Reports'
        reports_dir.mkdir()
        old_report = reports_dir / 'OR_unknown_260101_0001_abc1234.xlsx'
        old_report.write_bytes(b'stale')
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        monkeypatch.chdir(cwd)

        C01.refresh_ref_report()

        assert not old_report.exists()
        assert len(list(reports_dir.glob('OR_unknown_*.xlsx'))) == 1

    def test_main_refreshes_reference_report_without_any_flag(self, tmp_path, monkeypatch):
        # vecto_root and ref_vecto_root both resolve to version 'unknown'
        # (no real vectocmd.exe here) but get distinct git hashes, so the
        # current-run report (OR_unknown_..._<cur_hash>.xlsx) and the
        # refreshed reference report (OR_unknown_..._<ref_hash>.xlsx) are
        # still distinguishable by filename.
        ref_vecto_root = tmp_path / 'ref_vecto'
        ref_vecto_root.mkdir()
        _init_git_repo(str(ref_vecto_root))
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        _write_result_dir(
            cwd / 'XX_RESULTS' / 'unknown', 'CaseA',
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
        )
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        _init_git_repo(C01.vecto_root)
        monkeypatch.chdir(cwd)
        monkeypatch.setattr(C01, 'vecto_hash', C01.get_vecto_hash(C01.vecto_root))

        C01.main()

        ref_hash = C01.get_vecto_hash(str(ref_vecto_root))
        ref_reports = list((cwd / 'Reports').glob(f'OR_unknown_*_{ref_hash}.xlsx'))
        cur_reports = list((cwd / 'Reports').glob(f'OR_unknown_*_{C01.vecto_hash}.xlsx'))
        assert len(ref_reports) == 1
        assert len(cur_reports) == 1
        assert ref_reports != cur_reports


_OK_RUN = dict(
    log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
    vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0)],
)


def _sheet_cases(report_path, sheet):
    df = pd.read_excel(report_path, sheet_name=sheet)
    first = df.columns[0]
    return df[first].ffill().tolist()


class TestMergeOrReports:
    def _report(self, C01, base, cases):
        results = base / 'results'
        for name, run in cases.items():
            _write_result_dir(results, name, **run)
        path = base / 'report.xlsx'
        C01.write_report(str(results), str(path))
        return path

    def test_keeps_existing_and_adds_new_vehicles(self, tmp_path, C01):
        existing = self._report(C01, tmp_path / 'old', {'CaseA': _OK_RUN, 'CaseB': _OK_RUN})
        new = self._report(C01, tmp_path / 'new', {'CaseC': _OK_RUN})
        merged = tmp_path / 'merged.xlsx'

        C01.merge_or_reports(str(existing), str(new), str(merged))

        assert _sheet_cases(merged, 'Success') == ['CaseA', 'CaseB', 'CaseC']
        assert _sheet_cases(merged, 'CO2__g_km') == ['CaseA', 'CaseB', 'CaseC']

    def test_replaces_resimulated_vehicle(self, tmp_path, C01):
        failed_run = dict(
            log_lines=['VIN1 Urban _ReferenceLoad  88.22%  10.00s'],
            vsum_rows=[('1-0-', 'VIN1', 'Urban.vdri', 'Aborted', 0.0)],
        )
        existing = self._report(C01, tmp_path / 'old', {'CaseA': failed_run, 'CaseB': _OK_RUN})
        new = self._report(C01, tmp_path / 'new', {'CaseA': _OK_RUN})
        merged = tmp_path / 'merged.xlsx'

        C01.merge_or_reports(str(existing), str(new), str(merged))

        success = pd.read_excel(merged, sheet_name='Success', index_col=0)
        assert list(success.index) == ['CaseA', 'CaseB']
        assert success.loc['CaseA', 'Success'] == 'OK'

    def test_keeps_all_rows_of_a_multi_cycle_vehicle(self, tmp_path, C01):
        two_cycles = dict(
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s',
                       'VIN1 Urban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0),
                       ('1-1-', 'VIN1', 'Urban.vdri', 'Success', 450.0)],
        )
        existing = self._report(C01, tmp_path / 'old', {'CaseA': two_cycles})
        new = self._report(C01, tmp_path / 'new', {'CaseB': _OK_RUN})
        merged = tmp_path / 'merged.xlsx'

        C01.merge_or_reports(str(existing), str(new), str(merged))

        assert _sheet_cases(merged, 'CO2__g_km') == ['CaseA', 'CaseA', 'CaseB']

    def test_merged_sheet_cells_match_a_directly_written_report(self, tmp_path, C01):
        two_cycles = dict(
            log_lines=['VIN1 HeavyUrban _LowLoading  100.00%  10.00s',
                       'VIN1 Urban _LowLoading  100.00%  10.00s', _OK_LOG_TAIL],
            vsum_rows=[('1-0-', 'VIN1', 'HeavyUrban.vdri', 'Success', 500.0),
                       ('1-1-', 'VIN1', 'Urban.vdri', 'Success', 450.0)],
        )
        existing = self._report(C01, tmp_path / 'old', {'CaseA': two_cycles})
        new = self._report(C01, tmp_path / 'new', {'CaseB': _OK_RUN})
        direct = self._report(C01, tmp_path / 'direct', {'CaseA': two_cycles, 'CaseB': _OK_RUN})
        merged = tmp_path / 'merged.xlsx'

        C01.merge_or_reports(str(existing), str(new), str(merged))

        for sheet in ('Success', 'CO2__g_km'):
            expected = pd.read_excel(direct, sheet_name=sheet, header=None)
            actual = pd.read_excel(merged, sheet_name=sheet, header=None)
            pd.testing.assert_frame_equal(actual, expected, check_dtype=False)


class TestRefreshRefReportMerge:
    def test_merges_new_results_into_existing_reference_report(self, tmp_path, monkeypatch):
        ref_vecto_root = tmp_path / 'ref_vecto'
        ref_vecto_root.mkdir()
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        _write_result_dir(cwd / 'XX_RESULTS' / 'unknown', 'CaseNew', **_OK_RUN)
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        monkeypatch.chdir(cwd)
        old_results = tmp_path / 'old_results'
        _write_result_dir(old_results, 'CaseOld', **_OK_RUN)
        reports_dir = cwd / 'Reports'
        reports_dir.mkdir(exist_ok=True)
        old_report = reports_dir / 'OR_unknown_260101_0001_abc1234.xlsx'
        C01.write_report(str(old_results), str(old_report))

        C01.refresh_ref_report()

        reports = list(reports_dir.glob('OR_unknown_*.xlsx'))
        assert not old_report.exists()
        assert len(reports) == 1
        assert _sheet_cases(reports[0], 'Success') == ['CaseNew', 'CaseOld']

    def test_unknown_version_warns_with_the_supplied_path(self, tmp_path, capsys, monkeypatch):
        ref_vecto_root = tmp_path / 'ref_vecto_parent'
        ref_vecto_root.mkdir()
        cwd = tmp_path / 'cwd'
        cwd.mkdir()
        C01 = _import_c01_with_ref(str(ref_vecto_root), str(cwd))
        monkeypatch.chdir(cwd)

        C01.refresh_ref_report()

        out = capsys.readouterr().out
        assert 'WARNING' in out
        assert str(ref_vecto_root) in out


class TestGetLatestMrReport:
    def test_returns_none_for_missing_dir(self, tmp_path, C01):
        assert C01.get_latest_mr_report(str(tmp_path / 'missing')) is None

    def test_returns_none_when_empty(self, tmp_path, C01):
        d = tmp_path / 'mr'
        d.mkdir()
        assert C01.get_latest_mr_report(str(d)) is None

    def test_returns_or_report_when_present(self, tmp_path, C01):
        d = tmp_path / 'mr'
        d.mkdir()
        report = d / 'OR_5.1.1_260101_0001_abc1234.xlsx'
        report.write_bytes(b'fake xlsx')

        result = C01.get_latest_mr_report(str(d))

        assert result == str(report)

    def test_ignores_non_or_files(self, tmp_path, C01):
        d = tmp_path / 'mr'
        d.mkdir()
        (d / 'notes.txt').write_text('not a report')

        assert C01.get_latest_mr_report(str(d)) is None


class TestPurgeOldFailedReports:
    def test_missing_dir_is_noop(self, tmp_path, C01):
        C01.purge_old_failed_reports(str(tmp_path / 'missing'))

    def test_removes_old_run_dirs(self, tmp_path, C01):
        import datetime
        d = tmp_path / 'failed'
        d.mkdir()
        old_run = d / 'old_run'
        old_run.mkdir()
        old_time = (datetime.datetime.now() - datetime.timedelta(days=20)).timestamp()
        os.utime(old_run, (old_time, old_time))

        C01.purge_old_failed_reports(str(d), max_age_days=10)

        assert not old_run.exists()

    def test_keeps_recent_run_dirs(self, tmp_path, C01):
        d = tmp_path / 'failed'
        d.mkdir()
        recent_run = d / 'recent_run'
        recent_run.mkdir()

        C01.purge_old_failed_reports(str(d), max_age_days=10)

        assert recent_run.exists()


class TestCopyFailedOutputs:
    def test_noop_when_no_failed_names(self, tmp_path, C01):
        dest = tmp_path / 'dest'
        C01.copy_failed_outputs([], str(tmp_path / 'src'), str(dest))
        assert not dest.exists()

    def test_copies_matching_result_dir(self, tmp_path, C01):
        src = tmp_path / 'src'
        (src / 'CaseA').mkdir(parents=True)
        (src / 'CaseA' / 'job.log').write_text('log', encoding='utf-8')
        dest = tmp_path / 'dest'

        C01.copy_failed_outputs(['CaseA'], str(src), str(dest))

        assert (dest / 'CaseA' / 'job.log').is_file()

    def test_double_underscore_maps_to_nested_path(self, tmp_path, C01):
        src = tmp_path / 'src'
        (src / 'Declaration' / 'Bus_CONV_abc12').mkdir(parents=True)
        (src / 'Declaration' / 'Bus_CONV_abc12' / 'job.log').write_text('log', encoding='utf-8')
        dest = tmp_path / 'dest'

        C01.copy_failed_outputs(['Declaration__Bus_CONV_abc12'], str(src), str(dest))

        assert (dest / 'Declaration__Bus_CONV_abc12' / 'job.log').is_file()

    def test_missing_source_dir_warns_but_does_not_raise(self, tmp_path, C01, capsys):
        dest = tmp_path / 'dest'

        C01.copy_failed_outputs(['Missing'], str(tmp_path / 'src'), str(dest))

        assert 'WARNING' in capsys.readouterr().out


class TestReportPath:
    def test_builds_expected_filename_pattern(self, C01):
        path = C01.report_path()

        basename = os.path.basename(path)
        assert basename.startswith(f'OR_{C01.vecto_version}_{C01.date}_{C01.nnnn}_')
        assert basename.endswith('.xlsx')
