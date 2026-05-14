# -*- coding: utf-8 -*-
"""
A01 - Update the Article 10 test pool from GitLab.

Connects to GitLab, downloads new issue attachments since the last run,
extracts archives, and saves a status.json timestamp.

Usage:
    python A01_Update_Pool.py <pool_root>
        [--gitlab-url <url>]      GitLab instance URL (default: https://code.europa.eu)
        [--project-path <path>]   GitLab project path (default: vecto/vecto)
        [--token <token>]         GitLab private token; falls back to credentials.GITLAB_TOKEN
"""

import argparse
import json
from datetime import datetime
from pathlib import Path

import gitlab
import patoolib

from FUNCTIONS.utils import (
    check_archive,
    extract_archive,
    get_issue_id_from_path,
    locate_pswd,
    scan_issue_files,
)
from FUNCTIONS.utils_harvest_gitlab import (
    dl_link_gitlab,
    get_art10_labels,
    get_issue_data,
    get_issue_local_path,
    get_last_issues,
    get_link_local_path,
    make_scan_df,
    store_issue_data,
)
from FUNCTIONS.utils_pool import (
    DATE_FORMAT,
    EXTRACT_VERBOSITY,
    GITLAB_URL_DEFAULT,
    ISSUE_DESC_FILENAME,
    PROJECT_PATH_DEFAULT,
    get_reference_date,
    load_credentials,
    timed,
)

# ---------------------------------------------------------------------------
# Core logic
# ---------------------------------------------------------------------------

def update_pool(
    pool_root: Path,
    gitlab_url: str,
    project_path: str,
    token: str,
    df_passwords,
    default_password: str | None,
    ssl_verify: bool | str = True,
) -> None:
    """Connect to GitLab, download new issue attachments, and extract archives."""
    save_dir_raw = pool_root / 'A00_RAW'
    save_dir_extract = pool_root / 'A01_EXTRACT'
    saved_last_status_path = pool_root / 'status.json'

    print(70 * '=')
    print('A02 - Update Pool')

    # Connect
    with timed('GitLab connection'):
        gl = gitlab.Gitlab(url=gitlab_url, private_token=token, ssl_verify=ssl_verify)
        gl.auth()
        project = gl.projects.get(project_path)
        dl_url_base = project._links['self'].replace('api/v4/projects/', '-/project/')

    # Determine reference date
    date_ref = get_reference_date(save_dir_raw, save_dir_extract, saved_last_status_path)
    print(f'  date_ref = {date_ref.strftime(DATE_FORMAT)}')

    # List new issues
    art10_labels = get_art10_labels()
    with timed('List issues from GitLab'):
        issues = get_last_issues(
            gitlab_project=project,
            labels=art10_labels,
            updated_after=date_ref,
        )

    if not issues:
        print('  No new issues - nothing to download.')
        return

    print(f'  Issues updated since {date_ref.strftime(DATE_FORMAT)}: {[i.iid for i in issues]}')
    date_ref = datetime.now()

    # Store issue metadata
    save_dirs_updated = []
    with timed('Store issue data'):
        for issue in issues:
            issue_data = get_issue_data(issue)
            save_dir_raw_i = get_issue_local_path(save_dir_raw, issue.iid)
            store_issue_data(issue_data, save_dir_raw_i, ISSUE_DESC_FILENAME)
            save_dirs_updated.append(save_dir_raw_i)

    # Scan for links in updated issues and download
    issue_data_files = list(save_dir_raw.glob(f'*/{ISSUE_DESC_FILENAME}*.json'))
    issue_data_files_select = [f for f in issue_data_files if f.parent in save_dirs_updated]

    all_scans = scan_issue_files(issue_data_files_select)
    df_issue_links = make_scan_df(all_scans, 'links')
    print(f'  {len(df_issue_links)} links to test')

    i_dl = 0
    with timed('Download attachments from GitLab'):
        for iid, line in df_issue_links.iterrows():
            link = line['links']
            dl_path = get_link_local_path(save_dir_raw, iid, link)
            if not dl_path.exists():
                status_code = dl_link_gitlab(gl, dl_url_base, link, dl_path, token)
                if status_code != 200:
                    print(f'  WARNING: file not on GitLab for issue {iid}: {link}')
                i_dl += 1
    print(f'  {i_dl} file(s) downloaded')

    # Extract downloaded archives
    downloaded_files = [f for f in save_dir_raw.glob('*/uploads/**/*') if f.is_file()]

    i_ex = 0
    with timed('Extract archives'):
        for outfile in downloaded_files:
            if not patoolib.is_archive(outfile):
                continue
            extract_path = Path(
                str(outfile).replace(str(save_dir_raw), str(save_dir_extract))
            ).parent
            if extract_path.exists() and any(extract_path.iterdir()):
                continue
            iid = get_issue_id_from_path(outfile)
            password = locate_pswd(df_passwords, outfile.name, iid)
            err = check_archive(
                outfile, password=password,
                defaultpassword=default_password,
                verbosity=EXTRACT_VERBOSITY,
            )
            if err is None:
                extract_archive(
                    outfile, extract_path,
                    password=password,
                    defaultpassword=default_password,
                    verbosity=EXTRACT_VERBOSITY,
                )
                i_ex += 1
            else:
                print(f'  WARNING: extracting {outfile.name} failed')
    print(f'  {i_ex} archive(s) extracted')

    # Persist last-update timestamp
    with open(saved_last_status_path, 'w') as f:
        json.dump({'last_update': date_ref.strftime(DATE_FORMAT)}, f)


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description='Update the Article 10 test pool from GitLab.'
    )
    parser.add_argument(
        'pool_root',
        help='Root of the local pool directory (contains A00_RAW and A01_EXTRACT subdirs).',
    )
    parser.add_argument(
        '--gitlab-url',
        default=GITLAB_URL_DEFAULT,
        help=f'GitLab instance URL (default: {GITLAB_URL_DEFAULT}).',
    )
    parser.add_argument(
        '--project-path',
        default=PROJECT_PATH_DEFAULT,
        help=f'GitLab project path (default: {PROJECT_PATH_DEFAULT}).',
    )
    parser.add_argument(
        '--token',
        default=None,
        help='GitLab private token. Falls back to credentials.GITLAB_TOKEN if not supplied.',
    )
    parser.add_argument(
        '--credentials',
        default=None,
        metavar='PATH',
        help='Path to credentials.py file. Defaults to credentials.py on sys.path.',
    )
    parser.add_argument(
        '--ssl-verify',
        default=None,
        metavar='CA_BUNDLE_OR_FALSE',
        help='SSL verification: path to a CA bundle file, or "false" to disable verification '
             '(use "false" for servers with self-signed certificates). Default: verify with system CAs.',
    )
    args = parser.parse_args()

    # Parse --ssl-verify: "false" → False, a path → str, omitted → True
    if args.ssl_verify is None:
        ssl_verify = True
    elif args.ssl_verify.lower() == 'false':
        ssl_verify = False
    else:
        ssl_verify = args.ssl_verify

    token, df_passwords, default_password = load_credentials(args.token, args.credentials)
    update_pool(
        pool_root=Path(args.pool_root),
        gitlab_url=args.gitlab_url,
        project_path=args.project_path,
        token=token,
        df_passwords=df_passwords,
        default_password=default_password,
        ssl_verify=ssl_verify,
    )
    print('Done.')


if __name__ == '__main__':
    main()
