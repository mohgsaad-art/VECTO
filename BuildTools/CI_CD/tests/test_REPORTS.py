# -*- coding: utf-8 -*-
"""Unit tests for FUNCTIONS/REPORTS.py version/hash helpers."""

import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from FUNCTIONS.REPORTS import get_vecto_hash


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
