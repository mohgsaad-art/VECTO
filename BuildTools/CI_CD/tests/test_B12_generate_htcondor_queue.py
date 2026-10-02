"""
Tests for B12_Generate_HTCondor_Queue.py: _build_missing_job_args' suffix
parameter, which distinguishes reference-version ('_REF') from MR-version
('_MR') jobs. B12 runs argparse/filesystem checks at import time, so
_import_b12 points vecto_root/test_pool_path at empty tmp dirs first.
"""

import os
import sys
import tempfile

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))


def _import_b12():
    sys.modules.pop('B12_Generate_HTCondor_Queue', None)
    tmp_root = tempfile.mkdtemp()
    vecto_root = os.path.join(tmp_root, 'vecto')
    test_pool_path = os.path.join(tmp_root, 'pool')
    os.makedirs(vecto_root)
    os.makedirs(test_pool_path)

    old_argv, old_cwd = sys.argv, os.getcwd()
    sys.argv = ['B12_Generate_HTCondor_Queue.py', vecto_root, test_pool_path]
    os.chdir(tmp_root)
    try:
        import B12_Generate_HTCondor_Queue as B12
    finally:
        sys.argv = old_argv
        os.chdir(old_cwd)
    return B12


class _FakeExtractMissing:
    """Stub for the dynamically-imported 00_Extract_Missing_Vehicles module."""

    def __init__(self, missing_ids, input_dirs_by_id):
        self._missing_ids = missing_ids
        self._input_dirs_by_id = input_dirs_by_id

    def get_vehicle_ids_from_pool(self, test_pool_path):
        return set(self._input_dirs_by_id)

    def find_latest_report(self, reports_cache, vecto_root):
        return None

    def get_vehicle_ids_from_report(self, report_path):
        return set()

    def compute_missing_vehicles(self, pool_ids, report_ids):
        return sorted(self._missing_ids)

    def resolve_input_dirs(self, missing_ids, test_pool_path):
        return [self._input_dirs_by_id[vid] for vid in missing_ids]


class TestBuildMissingJobArgsSuffix:
    def test_ref_suffix_appended_to_output_filename(self, tmp_path):
        B12 = _import_b12()
        em = _FakeExtractMissing(
            missing_ids=['VehA'],
            input_dirs_by_id={'VehA': str(tmp_path / 'VehA')},
        )

        job_list, missing_count = B12._build_missing_job_args(
            em, str(tmp_path / 'ref_vecto'), str(tmp_path),
            str(tmp_path / 'reports_cache'), str(tmp_path), force_rerun=True, no_hev=False,
            suffix='_REF',
        )

        assert missing_count == 1
        assert job_list[0][2].endswith('_REF')
        assert not job_list[0][2].endswith('_MR')

    def test_mr_suffix_appended_to_output_filename(self, tmp_path):
        B12 = _import_b12()
        em = _FakeExtractMissing(
            missing_ids=['VehA'],
            input_dirs_by_id={'VehA': str(tmp_path / 'VehA')},
        )

        job_list, missing_count = B12._build_missing_job_args(
            em, str(tmp_path / 'mr_vecto'), str(tmp_path),
            str(tmp_path / 'reports_cache'), str(tmp_path), force_rerun=True, no_hev=False,
            suffix='_MR',
        )

        assert job_list[0][2].endswith('_MR')
        assert not job_list[0][2].endswith('_REF')

    def test_ref_and_mr_output_filenames_are_distinguishable(self, tmp_path):
        B12 = _import_b12()
        em = _FakeExtractMissing(
            missing_ids=['VehA'],
            input_dirs_by_id={'VehA': str(tmp_path / 'VehA')},
        )

        ref_jobs, _ = B12._build_missing_job_args(
            em, str(tmp_path / 'ref_vecto'), str(tmp_path),
            str(tmp_path / 'reports_cache'), str(tmp_path), force_rerun=True, no_hev=False,
            suffix='_REF',
        )
        mr_jobs, _ = B12._build_missing_job_args(
            em, str(tmp_path / 'mr_vecto'), str(tmp_path),
            str(tmp_path / 'reports_cache_mr'), str(tmp_path), force_rerun=True, no_hev=False,
            suffix='_MR',
        )

        assert ref_jobs[0][2] != mr_jobs[0][2]
