# -*- coding: utf-8 -*-
"""
Shared helpers for the A02/A03 pool-management scripts.
"""

import contextlib
import importlib
import importlib.util
import json
import sys
import time
from datetime import datetime, timedelta
from pathlib import Path

import pandas as pd

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

GITLAB_URL_DEFAULT = 'https://code.europa.eu'
PROJECT_PATH_DEFAULT = 'vecto/vecto'
ISSUE_DESC_FILENAME = 'issue_txt_data'
DATE_FORMAT = r'%Y-%m-%dT%H:%M:%SZ'
EXTRACT_VERBOSITY = -1  # suppress patool output

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

@contextlib.contextmanager
def timed(label: str):
    """Context manager that prints elapsed time for a labelled step."""
    t = time.time()
    yield
    print(f'  {label}: {time.time() - t:.1f}s')


def try_import_credentials(credentials_path: str | None = None):
    """Import credentials without crashing if the module is absent.

    Args:
        credentials_path: optional path to a credentials.py file. When given, the module
                          is loaded directly from that path rather than from sys.path.
    """
    sys.modules.pop('credentials', None)
    if credentials_path:
        spec = importlib.util.spec_from_file_location('credentials', credentials_path)
        if spec is None:
            raise FileNotFoundError(f'credentials module not found at: {credentials_path}')
        mod = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(mod)
        return mod
    try:
        return importlib.import_module('credentials')
    except ImportError:
        return None


def load_credentials(
    token_arg: str | None,
    credentials_path: str | None = None,
) -> tuple[str, pd.DataFrame, str | None]:
    """Return (token, df_passwords, default_password).

    Tries the CLI --token argument first, then falls back to credentials.
    Raises RuntimeError if no token is available.
    """
    ms = try_import_credentials(credentials_path)

    token = token_arg or (getattr(ms, 'GITLAB_TOKEN', None) if ms else None)
    if not token:
        raise RuntimeError(
            'GitLab token not found. Pass --token or define GITLAB_TOKEN in credentials.py.'
        )

    archive_passwords = getattr(ms, 'ARCHIVE_PASSWORDS', []) if ms else []
    df_passwords = (
        pd.DataFrame(archive_passwords, columns=['iid', 'filename', 'Password'])
        .drop_duplicates()
        .set_index(['iid', 'filename'], drop=True)
        .sort_index()
    )
    default_password = getattr(ms, 'DEFAULT_ARCHIVE_PASSWORD', None) if ms else None

    return token, df_passwords, default_password


def get_reference_date(
    save_dir_raw: Path,
    save_dir_extract: Path,
    saved_last_status_path: Path,
) -> datetime:
    """Return the reference date for filtering new GitLab issues.

    - Falls back to 5 years ago if the raw/extract dirs are new or empty.
    - Uses status.json if present.
    - Falls back to 60 days ago otherwise.
    """
    downloaded_files = [f for f in save_dir_raw.glob('*/uploads/**/*') if f.is_file()]
    if not save_dir_raw.exists() or not save_dir_extract.exists() or len(downloaded_files) < 100:
        return datetime.now() - timedelta(days=365 * 5)

    if saved_last_status_path.exists():
        with open(saved_last_status_path) as f:
            date_string = json.load(f)['last_update']
        return datetime.strptime(date_string, DATE_FORMAT)

    return datetime.now() - timedelta(days=60)
