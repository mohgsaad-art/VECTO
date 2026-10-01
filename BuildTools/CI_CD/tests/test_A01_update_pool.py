"""Tests for A01_Update_Pool.update_pool: forced update and versioned issue data (GitLab mocked)."""

import copy
import os
import sys
from datetime import datetime
from unittest import mock

import pandas as pd
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

import A01_Update_Pool as A01

REFERENCE_DATE = datetime(2026, 9, 1)


def _issue_data(iid, notes):
    return {
        'iid': iid,
        'issue': {'title': f'issue {iid}', 'description': '', 'labels': ['art10::approved'], 'state': 'opened',
                  'created_at': '2025-01-01T00:00:00.000Z'},
        'notes': notes,
        'status_events': [], 'label_events': [], 'milestone_events': [],
    }


def _note(note_id, link, date):
    return {'id': note_id, 'body': f'file ({link})', 'created_at': date, 'updated_at': date}


@pytest.fixture
def gitlab_mocks():
    """Patch every GitLab access of update_pool; yields the mocks used by the tests."""
    project = mock.MagicMock()
    project._links = {'self': 'https://gitlab/api/v4/projects/1'}
    client = mock.MagicMock()
    client.projects.get.return_value = project
    with mock.patch.object(A01.gitlab, 'Gitlab', return_value=client), \
            mock.patch.object(A01, 'get_reference_date', return_value=REFERENCE_DATE), \
            mock.patch.object(A01, 'get_last_issues', return_value=[]) as get_last_issues, \
            mock.patch.object(A01, 'get_issue_data') as get_issue_data, \
            mock.patch.object(A01, 'dl_link_gitlab', return_value=200) as dl_link_gitlab:
        yield mock.Mock(get_last_issues=get_last_issues, get_issue_data=get_issue_data,
                        dl_link_gitlab=dl_link_gitlab)


def _update(pool_root, force_update):
    A01.update_pool(pool_root=pool_root, gitlab_url='https://gitlab', project_path='vecto/vecto', token='t',
                    df_passwords=pd.DataFrame(), default_password=None, force_update=force_update)


class TestForceUpdateIssueSelection:
    def test_forced_update_lists_all_issues(self, tmp_path, gitlab_mocks):
        _update(tmp_path, force_update=True)

        assert gitlab_mocks.get_last_issues.call_args.kwargs['updated_after'] is None

    def test_normal_update_lists_issues_after_reference_date(self, tmp_path, gitlab_mocks):
        _update(tmp_path, force_update=False)

        assert gitlab_mocks.get_last_issues.call_args.kwargs['updated_after'] == REFERENCE_DATE


class TestForceUpdateVersions:
    OLD_LINK = '/uploads/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa/old.xml'
    NEW_LINK = '/uploads/bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb/new.xml'

    def _run(self, tmp_path, gitlab_mocks, fetched_by_iid):
        gitlab_mocks.get_last_issues.return_value = [mock.Mock(iid=iid) for iid in fetched_by_iid]
        gitlab_mocks.get_issue_data.side_effect = lambda issue: copy.deepcopy(fetched_by_iid[issue.iid])
        _update(tmp_path, force_update=True)

    def _versions(self, tmp_path, iid):
        return sorted(p.name for p in (tmp_path / 'A00_RAW' / f'{iid:04d}').glob('issue_txt_data*.json'))

    def test_changed_issue_gets_second_version_and_unchanged_issue_does_not(self, tmp_path, gitlab_mocks):
        old_note = _note(1, self.OLD_LINK, '2025-03-25T00:00:00.000Z')
        first = {887: _issue_data(887, [old_note]), 1014: _issue_data(1014, [])}
        self._run(tmp_path, gitlab_mocks, first)
        second = {887: _issue_data(887, [old_note, _note(2, self.NEW_LINK, '2025-07-07T00:00:00.000Z')]),
                  1014: _issue_data(1014, [])}

        self._run(tmp_path, gitlab_mocks, second)

        assert self._versions(tmp_path, 887) == ['issue_txt_data_v01.json', 'issue_txt_data_v02.json']
        assert self._versions(tmp_path, 1014) == ['issue_txt_data_v01.json']

    def test_only_links_of_changed_issues_are_downloaded(self, tmp_path, gitlab_mocks):
        old_note = _note(1, self.OLD_LINK, '2025-03-25T00:00:00.000Z')
        self._run(tmp_path, gitlab_mocks, {887: _issue_data(887, [old_note])})
        gitlab_mocks.dl_link_gitlab.reset_mock()

        self._run(tmp_path, gitlab_mocks, {887: _issue_data(887, [old_note])})

        gitlab_mocks.dl_link_gitlab.assert_not_called()
