# -*- coding: utf-8 -*-
"""
Cross-validates a VECTO console log's per-cycle progress lines against the
corresponding .vsum result rows, replacing a raw "100%" substring check with a
real completeness check (used by C01, C03).
"""

import re
from dataclasses import dataclass

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
    logical cycle groups, paired ordinally (by position).
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
