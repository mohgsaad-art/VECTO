# -*- coding: utf-8 -*-
"""
Shared utilities for CI/CD report generation (C01, C02, C03).
"""

import glob
import os
import re
import subprocess
from dataclasses import dataclass

from FUNCTIONS.VECTO import get_vecto_git_root

VALID_VSUM_STATUSES = {'Success', 'PrimaryBusSimulationIgnore'}

_PROGRESS_LINE_RE = re.compile(r'^\S.*?\s+\d+\.\d+%\s+\d+\.\d+s\s*$')
_PROGRESS_LINE_PARSE_RE = re.compile(
    r'^(?P<vehicle_id>\S+)\s+(?P<cycle>\S+)\s+(?P<label>.*?)\s+'
    r'(?P<percent>\d+\.\d+)%\s+\d+\.\d+s\s*$'
)
_JOB_INDEX_RE = re.compile(r'^(?P<prefix>\d+-\d+)-(?:\d+|F)?$')


@dataclass(frozen=True)
class LogCycleEntry:
    """One per-cycle progress line parsed from a VECTO .log file."""
    line_index: int
    vehicle_id: str
    cycle: str
    label: str
    percent: float
    raw_line: str


@dataclass(frozen=True)
class VsumCycleGroup:
    """One logical simulated cycle, collapsing vsum job-index sub-rows (-, -1, -2, -F)."""
    group_index: int
    job_prefix: str
    input_file: str
    cycle_file: str
    all_valid: bool
    statuses: tuple


@dataclass(frozen=True)
class LogVsumMismatch:
    """One detected disagreement between a log cycle and its vsum counterpart."""
    kind: str  # 'unexplained_non_success' | 'missing_vsum_row' | 'unmatched_log_line'
    log_entry: object
    vsum_group: object
    detail: str


@dataclass(frozen=True)
class LogVsumCheckResult:
    """Result of cross-validating a log file's per-cycle lines against its vsum."""
    ok: bool
    mismatches: tuple


def parse_log_progress_lines(log_path):
    """Parse per-cycle simulation progress lines from a VECTO console log, in file order."""
    entries = []
    with open(log_path, encoding='utf-8', errors='replace') as fh:
        for raw_line in fh:
            line = raw_line.rstrip('\n')
            if not _PROGRESS_LINE_RE.match(line):
                continue
            m = _PROGRESS_LINE_PARSE_RE.match(line)
            if not m:
                continue
            entries.append(LogCycleEntry(
                line_index=len(entries),
                vehicle_id=m.group('vehicle_id'),
                cycle=m.group('cycle'),
                label=m.group('label'),
                percent=float(m.group('percent')),
                raw_line=line,
            ))
    return entries


def group_vsum_cycles(df_vsum, valid_statuses=VALID_VSUM_STATUSES):
    """Collapse vsum sub-rows (job index '-', '-1', '-2', '-F') into one record per
    logical simulated cycle, in job-index order. A group is valid only if every
    sub-row's Status is valid (not just the base row) - verified necessary since a
    base row can be Success while a sub-row is Aborted."""
    groups = {}
    order = []
    for _, row in df_vsum.iterrows():
        job = str(row['Job [-]'])
        m = _JOB_INDEX_RE.match(job)
        prefix = m.group('prefix') if m else job
        if prefix not in groups:
            groups[prefix] = []
            order.append(prefix)
        groups[prefix].append(row)

    result = []
    for idx, prefix in enumerate(order):
        rows = groups[prefix]
        base = rows[0]
        statuses = tuple(str(r['Status']) for r in rows)
        result.append(VsumCycleGroup(
            group_index=idx,
            job_prefix=prefix,
            input_file=str(base['Input File [-]']),
            cycle_file=str(base['Cycle [-]']),
            all_valid=all(s in valid_statuses for s in statuses),
            statuses=statuses,
        ))
    return result


def check_log_vsum_match(log_path, df_vsum, valid_statuses=VALID_VSUM_STATUSES):
    """Cross-validate a VECTO log's per-cycle progress lines against the vsum's
    logical cycle groups, paired ordinally (by position). Replaces a raw '100%'
    substring check with a real completeness check: a cycle is flagged if the log
    claims 100% but the vsum's matching cycle has a non-valid status
    ('unexplained_non_success'), or if the log has a cycle with no vsum
    counterpart at all ('missing_vsum_row') - the silent-drop bug this exists to
    catch. A vsum cycle with a valid status is treated as authoritative even if
    the log's own percentage for that cycle reads below 100%.
    """
    log_entries = parse_log_progress_lines(log_path)
    vsum_groups = group_vsum_cycles(df_vsum, valid_statuses)

    if not log_entries:
        return LogVsumCheckResult(
            ok=False,
            mismatches=(LogVsumMismatch('missing_vsum_row', None, None,
                                         'No per-cycle progress lines found in log.'),),
        )

    mismatches = []
    n_log, n_vsum = len(log_entries), len(vsum_groups)

    for i in range(n_log):
        log_e = log_entries[i]
        if i >= n_vsum:
            mismatches.append(LogVsumMismatch(
                'missing_vsum_row', log_e, None,
                f'Log cycle #{i} ({log_e.vehicle_id} {log_e.cycle} {log_e.label}, '
                f'{log_e.percent}%) has no corresponding vsum cycle group '
                f'(log has {n_log} progress lines, vsum has {n_vsum} cycle groups).',
            ))
            continue

        vsum_g = vsum_groups[i]
        log_success = log_e.percent == 100.0
        vsum_success = vsum_g.all_valid

        if not vsum_success and log_success:
            mismatches.append(LogVsumMismatch(
                'unexplained_non_success', log_e, vsum_g,
                f'Log cycle #{i} ({log_e.vehicle_id} {log_e.cycle} {log_e.label}) '
                f'reports 100% but vsum group {vsum_g.job_prefix} has non-valid '
                f'status(es) {vsum_g.statuses}.',
            ))
        # (not vsum_success and not log_success) -> agreement, not flagged.
        # (vsum_success and not log_success) -> vsum success is authoritative, not flagged.

    for j in range(n_log, n_vsum):
        vsum_g = vsum_groups[j]
        mismatches.append(LogVsumMismatch(
            'unmatched_log_line', None, vsum_g,
            f'Vsum cycle group {vsum_g.job_prefix} has no corresponding log '
            f'progress line. Not counted against log_ok.',
        ))

    blocking_kinds = {'unexplained_non_success', 'missing_vsum_row'}
    ok = not any(m.kind in blocking_kinds for m in mismatches)
    return LogVsumCheckResult(ok=ok, mismatches=tuple(mismatches))


def get_vecto_version(vecto_root):
    """Return the VECTO version string by calling vectocmd -V."""
    vecto_cmd_local = 'vectocmd.dll' if os.name == 'posix' else 'vectocmd.exe'
    vecto_cmd = os.path.join(vecto_root, vecto_cmd_local)
    if os.name == 'posix':
        version_cmd = f'dotnet "{vecto_cmd}" -V'
    else:
        version_cmd = f'"{vecto_cmd}" -V'
    result = subprocess.run(version_cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)
    output = result.stdout.decode('utf-8')

    match = re.search(r'(\d+\.\d+\.\d+(?:-RC|-DEV)?)', output)
    if match:
        return match.group(1)

    return 'unknown'


def get_vecto_hash(vecto_root):
    """Return the short git commit hash of the VECTO repository."""
    result = subprocess.run(
        f'git -C "{vecto_root}" rev-parse --short HEAD',
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True
    )
    return result.stdout.decode('utf-8').strip() or 'unknown'


def get_default_report_dir():
    """Return the default Reports directory: D:/Reports if D:/ exists, otherwise C:/Reports."""
    drive = 'D:' if os.path.exists('D:/') else 'C:'
    default_report_path = os.path.join(drive, 'CI', 'Reports')
    print(f'No --report-dir specified, using default: {default_report_path}')
    return default_report_path


def _fetch_release_tags(vecto_root):
    """Fetch from remote and return all Release/v* tags sorted by creation date (newest first)."""
    subprocess.run(
        f'git -C "{vecto_root}" fetch --tags',
        stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, shell=True
    )
    result = subprocess.run(
        f'git -C "{vecto_root}" tag --sort=-creatordate',
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True
    )
    _tag_re = re.compile(r'^Release/v\d+\.\d+\.\d+(-RC|-DEV)?$')
    return [t for t in result.stdout.decode('utf-8').splitlines() if _tag_re.match(t)]


def get_latest_vecto_releases_by_type(vecto_root):
    """Return the latest official, RC, and DEV release tag names from the VECTO git repo.

    Returns a dict with keys 'official', 'rc', 'dev', each being the full tag
    name (e.g. 'Release/v5.1.1') of the highest version in that category,
    or None if no tag of that type exists.
    """
    tags = _fetch_release_tags(vecto_root)

    _tag_re = re.compile(r'^Release/v(\d+)\.(\d+)\.(\d+)(-RC|-DEV)?$')

    official, rc, dev = None, None, None

    for tag in tags:
        m = _tag_re.match(tag)
        if not m:
            continue
        version = (int(m.group(1)), int(m.group(2)), int(m.group(3)))
        suffix = m.group(4)  # '-RC', '-DEV', or None

        if suffix is None:
            if official is None or version > official[0]:
                official = (version, tag)
        elif suffix == '-RC':
            if rc is None or version > rc[0]:
                rc = (version, tag)
        elif suffix == '-DEV':
            if dev is None or version > dev[0]:
                dev = (version, tag)

    return {
        'official': official[1] if official else None,
        'rc':       rc[1]       if rc       else None,
        'dev':      dev[1]      if dev      else None,
    }


def get_latest_vecto_release(vecto_root):
    """Return the most recently published Release/v* tag, regardless of type (official, RC, or DEV)."""
    tags = _fetch_release_tags(vecto_root)
    return tags[0] if tags else None


def get_next_report_number(report_base_dir):
    """Return the next OR report number as a zero-padded 4-digit string."""
    existing = glob.glob(os.path.join(report_base_dir, 'OR_*.xlsx'))
    numbers = []
    for f in existing:
        m = re.search(r'_(\d{4})_', os.path.basename(f))
        if m:
            numbers.append(int(m.group(1)))
    return f'{max(numbers) + 1:04d}' if numbers else '0001'


def find_result_dirs(result_dir: str) -> list[str]:
    """Return leaf directories containing runnable test cases (.xml or .vecto files).

    Directories that contain a .vecto file are bus complete-step cases — they are
    included directly via the .vecto file rather than via their child .xml files.
    """
    file_list_runable = [
        os.path.join(root, f)
        for root, _dirs, files in os.walk(result_dir)
        for f in files
        if (f.endswith('.xml') and 'RSLT' not in f and 'VIF' not in f)
        or f.endswith('.vecto')
    ]

    dirs_with_vecto = {os.path.dirname(f) for f in file_list_runable if f.endswith('.vecto')}

    file_list_torun = [
        f for f in file_list_runable
        if os.path.dirname(f) not in dirs_with_vecto or f.endswith('.vecto')
    ]

    return [os.path.dirname(f) for f in file_list_torun]


def make_sheet_name(var: str) -> str:
    """Convert a variable name to a valid Excel sheet name."""
    for ch in ('/', '[', ']'):
        var = var.replace(ch, ' ')
    return var.strip().replace(' ', '_')


def get_latest_release_report(reports_dir: str, vecto_root: str) -> str | None:
    """Return the path of the OR summary report matching the latest VECTO release tag.

    Tries the most recently published tag first (official, RC, or DEV). Falls back
    through the latest official, RC, and DEV releases in order. Returns the matching
    report with the highest NNNN, or None if no match is found for any release.
    """
    if not os.path.isdir(reports_dir):
        return None

    _report_re = re.compile(r'^OR_(.+)_\d{6}_(\d{4})_[0-9a-f]+\.xlsx$')

    def _best_report_for_version(tag_version: str) -> str | None:
        best_nnnn, best_path = -1, None
        for fname in os.listdir(reports_dir):
            m = _report_re.match(fname)
            if not m or m.group(1) != tag_version:
                continue
            nnnn_val = int(m.group(2))
            if nnnn_val > best_nnnn:
                best_nnnn, best_path = nnnn_val, os.path.join(reports_dir, fname)
        return best_path

    git_root = get_vecto_git_root(vecto_root) or vecto_root

    latest_tag = get_latest_vecto_release(git_root)
    by_type    = get_latest_vecto_releases_by_type(git_root)

    candidates = []
    if latest_tag:
        candidates.append(latest_tag)
    for key in ('official', 'rc', 'dev'):
        tag = by_type.get(key)
        if tag and tag not in candidates:
            candidates.append(tag)

    for tag in candidates:
        tag_version = tag.removeprefix('Release/v')
        result = _best_report_for_version(tag_version)
        if result:
            return result

    return None
