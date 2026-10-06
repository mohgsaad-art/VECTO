"""Tests for FUNCTIONS/utils.py scan_issue_files: upload links in chronological order."""

import json
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from FUNCTIONS.utils import scan_issue_files


def _note(body, created_at, updated_at=None):
    note = {'body': body, 'created_at': created_at}
    if updated_at is not None:
        note['updated_at'] = updated_at
    return note


def _issue_file(tmp_path, notes, description='', created_at='2025-01-01T00:00:00.000Z'):
    data = {
        'iid': 887,
        'issue': {'title': 'VIN: YS2P4X20002205417', 'description': description, 'labels': ['art10::approved'],
                  'created_at': created_at, 'updated_at': '2026-01-08T15:48:31.355Z'},
        'notes': notes,
    }
    path = tmp_path / 'issue_txt_data_v01.json'
    path.write_text(json.dumps(data), encoding='utf-8')
    return path


def _links(tmp_path, **kwargs):
    return scan_issue_files([_issue_file(tmp_path, **kwargs)])[0]['links']


class TestScanIssueFilesLinkOrder:
    def test_notes_returned_newest_first_give_links_oldest_first(self, tmp_path):
        notes = [
            _note('new (/uploads/aaa/new.xml)', '2025-07-07T18:46:11.943Z', '2025-07-07T18:46:11.943Z'),
            _note('old (/uploads/bbb/old.xml)', '2025-03-25T11:40:30.701Z', '2025-03-25T11:40:30.701Z'),
        ]

        assert _links(tmp_path, notes=notes) == ['/uploads/bbb/old.xml', '/uploads/aaa/new.xml']

    def test_updated_at_has_preference_over_created_at(self, tmp_path):
        notes = [
            _note('(/uploads/aaa/a.xml)', '2025-05-01T00:00:00.000Z', '2025-05-01T00:00:00.000Z'),
            _note('(/uploads/bbb/b.xml)', '2025-01-01T00:00:00.000Z', '2025-06-01T00:00:00.000Z'),
        ]

        assert _links(tmp_path, notes=notes) == ['/uploads/aaa/a.xml', '/uploads/bbb/b.xml']

    def test_created_at_used_when_updated_at_missing(self, tmp_path):
        notes = [
            _note('(/uploads/bbb/b.xml)', '2025-06-01T00:00:00.000Z'),
            _note('(/uploads/aaa/a.xml)', '2025-05-01T00:00:00.000Z'),
        ]

        assert _links(tmp_path, notes=notes) == ['/uploads/aaa/a.xml', '/uploads/bbb/b.xml']

    def test_description_links_come_before_later_notes(self, tmp_path):
        notes = [_note('(/uploads/bbb/b.xml)', '2025-06-01T00:00:00.000Z')]

        links = _links(tmp_path, notes=notes, description='input (/uploads/aaa/a.xml)')

        assert links == ['/uploads/aaa/a.xml', '/uploads/bbb/b.xml']

    def test_link_posted_again_later_takes_the_latest_position(self, tmp_path):
        notes = [
            _note('again (/uploads/aaa/a.xml)', '2025-08-01T00:00:00.000Z'),
            _note('(/uploads/bbb/b.xml)', '2025-06-01T00:00:00.000Z'),
            _note('(/uploads/aaa/a.xml)', '2025-05-01T00:00:00.000Z'),
        ]

        assert _links(tmp_path, notes=notes) == ['/uploads/bbb/b.xml', '/uploads/aaa/a.xml']

    def test_vins_still_read_from_title_and_description(self, tmp_path):
        scan = scan_issue_files([_issue_file(tmp_path, notes=[])])[0]

        assert scan['VINs'] == ['YS2P4X20002205417']
