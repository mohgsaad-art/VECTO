"""Tests for FUNCTIONS/utils_harvest_gitlab.py store_issue_data: versioned, never overwritten."""

import copy
import json
import os
import sys

import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from FUNCTIONS.utils_harvest_gitlab import store_issue_data

NAME = 'issue_txt_data'


def _issue_data():
    return {
        'iid': 887,
        'issue': {'title': 't', 'description': 'd', 'labels': ['art10::approved'], 'state': 'opened',
                  'updated_at': '2025-07-07T00:00:00.000Z', 'user_notes_count': 1},
        'notes': [{'id': 1, 'body': 'first', 'created_at': '2025-03-25T00:00:00.000Z',
                   'updated_at': '2025-03-25T00:00:00.000Z'}],
        'status_events': [],
        'label_events': [{'id': 10, 'created_at': '2025-01-01T00:00:00.000Z', 'action': 'add',
                          'label': {'name': 'art10::approved'}}],
        'milestone_events': [],
    }


def _versions(folder):
    return sorted(p.name for p in folder.glob(f'{NAME}*.json'))


@pytest.fixture
def stored(tmp_path):
    """Issue folder holding a first stored version, and the data it was stored from."""
    data = _issue_data()
    store_issue_data(data, tmp_path, NAME)
    return tmp_path, data


class TestStoreIssueData:
    def test_first_store_writes_v01(self, tmp_path):
        created = store_issue_data(_issue_data(), tmp_path, NAME)

        assert created is True
        assert _versions(tmp_path) == [f'{NAME}_v01.json']

    def test_identical_data_writes_no_new_version(self, stored):
        folder, data = stored

        created = store_issue_data(copy.deepcopy(data), folder, NAME)

        assert created is False
        assert _versions(folder) == [f'{NAME}_v01.json']

    def test_additional_note_writes_v02_and_keeps_v01(self, stored):
        folder, data = stored
        original_v01 = (folder / f'{NAME}_v01.json').read_text()
        data = copy.deepcopy(data)
        data['notes'].append({'id': 2, 'body': 'new (/uploads/a/b.xml)', 'created_at': '2025-07-07T00:00:00.000Z',
                              'updated_at': '2025-07-07T00:00:00.000Z'})

        created = store_issue_data(data, folder, NAME)

        assert created is True
        assert _versions(folder) == [f'{NAME}_v01.json', f'{NAME}_v02.json']
        assert (folder / f'{NAME}_v01.json').read_text() == original_v01
        assert len(json.loads((folder / f'{NAME}_v02.json').read_text())['notes']) == 2

    @pytest.mark.parametrize('change', [
        lambda d: d['notes'][0].update(updated_at='2025-08-01T00:00:00.000Z'),
        lambda d: d['notes'][0].update(created_at='2025-03-26T00:00:00.000Z'),
        lambda d: d['issue'].update(description='d (/uploads/a/new.xml)'),
        lambda d: d['issue'].update(labels=['art10::need_info']),
        lambda d: d['issue'].update(state='closed'),
        lambda d: d['label_events'].append({'id': 11, 'created_at': '2025-02-01T00:00:00.000Z', 'action': 'add',
                                            'label': {'name': 'art10::resolved'}}),
    ], ids=['note_updated_at', 'note_created_at', 'description', 'labels', 'state', 'label_event'])
    def test_meaningful_change_writes_new_version(self, stored, change):
        folder, data = stored
        data = copy.deepcopy(data)
        change(data)

        assert store_issue_data(data, folder, NAME) is True
        assert _versions(folder) == [f'{NAME}_v01.json', f'{NAME}_v02.json']

    @pytest.mark.parametrize('change', [
        lambda d: d['issue'].update(user_notes_count=5),
        lambda d: d['issue'].update(updated_at='2026-01-08T00:00:00.000Z'),
        lambda d: d['notes'].reverse(),
    ], ids=['notes_count_field', 'issue_updated_at', 'notes_order'])
    def test_volatile_change_writes_no_new_version(self, stored, change):
        folder, data = stored
        data = copy.deepcopy(data)
        data['notes'].append({'id': 2, 'body': 'x', 'created_at': '2025-04-01T00:00:00.000Z',
                              'updated_at': '2025-04-01T00:00:00.000Z'})
        store_issue_data(copy.deepcopy(data), folder, NAME)
        change(data)

        assert store_issue_data(data, folder, NAME) is False
        assert _versions(folder) == [f'{NAME}_v01.json', f'{NAME}_v02.json']
