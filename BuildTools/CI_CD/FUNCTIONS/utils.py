# -*- coding: utf-8 -*-
"""
Created on Tue Apr  7 11:05:44 2026

@author: alixgui
"""

import os
from pathlib import Path
from datetime import datetime
import shutil
import hashlib
import json
import pandas as pd
import functools as fnt
import patoolib

from typing import overload
from typing import Literal as L
from typing import TypedDict

import re
from ordered_set import OrderedSet as oset  # type: ignore

# %%
# =============================================================================
# 0.2. Utilities
# =============================================================================


def banner(title: str) -> None:
    w = 50
    w2 = int((w - len(title)) / 2)
    print("", "=" * w, (" " * w2) + title, "=" * w, sep="\n")


def strlist(it: list):
    return ", ".join([str(i) for i in it])


def get_nested(d, keys: list[str], default=..., *, nexist=1, scream=None):
    """
    Recursively index into given nested dictionary with key list.

    :param default:
        what to return if any key is missing,
        by default, the last existing value.
    :param nexist:
        how many key must exist (or raise)?
            - >0: the first `nexist`,
            - 0 : all,
            - <0: none.
    :param scream:
        when falsy, return `default` when any valye is not a dict.
    """
    for i, k in enumerate(keys):
        try:
            dd = d[k]
        except (TypeError, AttributeError):
            if scream:
                raise
        except KeyError:
            if nexist >= 0 and i < nexist:
                raise
        else:
            d = dd
            continue

        if default is not ...:
            d = default
        break

    return d


assert get_nested(_d := {"a": {"b": 2}}, ["a", "b"]) == 2
assert get_nested(_d, list("acab")) == {"b": 2}
assert get_nested(_d, list("ac"), 4) == 4
assert get_nested(_d, list("abcd"), nexist=-1) == 2
assert get_nested(_d, list("abcd"), default=None) == None


def filter_nested_dict(d, fields, *a, sep=".", **kw) -> dict:
    return {k: get_nested(d, kk, *a, **kw) for k in fields for kk in [k.split(sep)]}


# assert filter_nested_dict(_d, ["a.b.c.d"]) == {"a.b.c.d": 2}
assert filter_nested_dict(_d, ["a.b.c.d"], default=None) == {"a.b.c.d": None}


def dictify(objlist):
    return [i.asdict() for i in objlist]


def trim_root(path: Path | str) -> str:
    path = os.path.normpath(str(path))
    while path.startswith(os.sep):
        path = path[1:]

    return path


def ensure_dir(dir_path: Path | str) -> None:
    Path(dir_path).mkdir(parents=True, exist_ok=True)


def backup_dir_if_exists(dir, delete=False):
    if dir.exists():
        now_str = datetime.now().strftime("%Y%m%d-%H%M")
        target = dir.with_suffix(f".{now_str}")
        if delete:
            dir.rename(target)
        else:
            shutil.copytree(dir, target)


@overload
def next_fpath(
    fpath_template: Path | str,
    start=...,
    *,
    overwrite=...,
    also_last: L[False] = ...,
    limit=...,
) -> Path: ...
@overload
def next_fpath(
    fpath_template: Path | str,
    start=...,
    *,
    overwrite=...,
    also_last: L[True] = ...,
    limit=...,
) -> tuple[Path, Path | None]: ...
def next_fpath(
    fpath_template: Path | str,
    start=0,
    *,
    overwrite=False,
    also_last=False,
    limit=64,
):
    """
    Interpolate `fpath_template.format(<counter>)` and return the 1st non-existent path.

    :param overwrite: interpolate only once, regardless of file's existence
    :param also_last:
        when truthy, return a 2-tuple with the 2nd element being
        the last existent path found (or None if non exists).
    :param limit: stop looping when counter reach this, safeguarding bad templates
    """
    last_fpath = None
    while (p := Path(str(fpath_template).format(start))).exists() and not overwrite:
        start += 1
        last_fpath = p
        if start == limit:
            raise ValueError(f"Template {fpath_template!r} reached {start} iterations!")
    return (p, last_fpath) if also_last else p


assert (_p := next_fpath("foo{:02d}", start=2)) == Path("foo02"), _p
assert (_p := next_fpath(__file__, overwrite=1)) == Path(__file__), _p
assert (_pp := next_fpath("foo{}", also_last=True)) == (Path("foo0"), None), _pp


# %%

def get_issue_id_from_path(fpath: Path | str) -> str:
    filep = str(fpath)
    regexp = r"[\\/]([0-9]{4})[\\/]"
    iids = re.findall(regexp, filep)
    iid=None
    if iids:
        iid = iids[0]
    return int(iid)

def locate_pswd(df_passwords, fname, iid: int | str | None = None) -> str | None:
    if iid:
        iid = int(iid)
    pswd = None
    try:
        pswd = df_passwords.at[(iid, fname), "Password"]
    except KeyError:
        try:
            pswd_sr = df_passwords.loc[(slice(None), fname), "Password"]
        except KeyError:
            pass
        else:
            pswds = pswd_sr.unique()
            if len(pswds) != 1:
                raise KeyError(
                    f"Multiple ({len(pswds)}) passwords for file {fname}."
                    f"  (issue-id #{iid} ignored)"
                )
            pswd = pswds[0]

    return pswd


def hashbytes(b: bytes) -> str:
    return hashlib.sha1(b).hexdigest()


def read_all_json(fpaths) -> list[dict]:
    return [json.loads(f.read_text()) for f in fpaths]


# %%
## Allow to serialize to JSON collections with pandas in them.


def _call2dict(func, *args, **kw):
    return {"!func": func.__qualname__, "!args": [args, kw]}


def _json_encoder(o):
    """
    Encode dataframes to JSON with `json.dumps(..., default=_json_encoder)`.

    Serialization: `{"!func": "<FQClassname>", "!args": [[<*a>], {<**kw>}]}`
    """
    if isinstance(o, (pd.DataFrame, pd.Series)):
        o = _call2dict(type(o), o.to_dict())
    return o


_decoders = {
    "DataFrame": pd.DataFrame.from_dict,
    "Series": pd.Series,
}


def _json_decode_hook(pairs):
    """
    Decode objects from JSON with `json.loads(..., object_pairs_hook=_json_decode_hook)`.
    """
    match pairs:
        case [("!func", func), ("!args", [a, kw])]:
            return _decoders[func](*a, **kw)
        case [("!func", func), *_]:
            raise ValueError("Expected call !args.", pairs)
    return dict(pairs)


json_dump = fnt.partial(json.dump, default=_json_encoder)
json_dumps = fnt.partial(json.dumps, default=_json_encoder)
json_load = fnt.partial(json.load, object_pairs_hook=_json_decode_hook)
json_loads = fnt.partial(json.loads, object_pairs_hook=_json_decode_hook)

assert (_j := json_loads(json_dumps(_dc := {"a": 4}))) == _dc, _j
assert (
    _j := json_loads(json_dumps(_df := pd.DataFrame({"a": [1]}, index=["A"])))
).equals(_df), _j
assert (_j := json_loads(json_dumps(_sr := pd.Series({"a": 1})))).equals(_sr), _j
assert (
    json_dumps((1, {"a": _df, "s": _sr}))
    == '[1, {"a": {"!func": "DataFrame", "!args": [[{"a": {"A": 1}}], {}]}, '
    + '"s": {"!func": "Series", "!args": [[{"a": 1}], {}]}}]'
)

# scan issue desc content

def find_links(link_txt: str) -> list[str]:
    # regexp = '\(/uploads.+\)'
    regexp = r"(?<=\()/uploads[\w/.\- ]+(?=\))"

    links = re.findall(regexp, link_txt)
    links = [i for i in links if not i.lower().endswith(".png")]
    # secret_file_list = [(lnk.split('/')[-2], lnk.split('/')[-1].replace(')','')) for lnk in links]

    return list(oset(links))

def find_VINs(link_txt: str) -> list[str]:
    ## From https://stackoverflow.com/a/30314935/548792
    ## Note that by the standard, S.America has numbers in the first 2 places,
    ## see https://en.wikipedia.org/wiki/Vehicle_identification_number#List_of_common_WMI

    ## test_string = r'NLTPNG23L01005961.xml NLTPNG23601005968.xml 012345697.xml W1V5M23Z0RN293908 1V5WM23Z0RN293908 VNE454454 V1E123456 V11123456 1VE123456 NMC250LKBLBF00004 320LGBLBE00000 ZX9E6BLC0RATAM106_completed_veh.xml'

    # regexp = r"(?<![A-HJ-NPR-Z\d])[A-Z]{2,3}[A-Z0-9]+[0-9]+[A-Z0-9]+(?![A-HJ-NPR-Z\d])" # Original, cannot find W1V5M23Z0RN293908

    regexp = r"(?<![A-HJ-NPR-Z\d])[A-Z0-9]*[A-Z][A-Z0-9]+[0-9]{3}(?![A-HJ-NPR-Z\d])" # modified, find all
    VIN_list = re.findall(regexp, link_txt)

    # secret_file_list = [(lnk.split('/')[-2], lnk.split('/')[-1].replace(')','')) for lnk in link_list]

    return list(oset(VIN_list))

T_scan = TypedDict("T_scan", {"iid": int, "links": list[str], "VINs": list[str]})
def _chronological_links(issue: dict, notes: list[dict]) -> list[str]:
    """Upload links ordered oldest first, by note date (updated_at, else created_at)."""
    texts = [(issue.get("created_at") or "", "\n".join([issue.get("title") or "", issue.get("description") or ""]))]
    texts += [(note.get("updated_at") or note.get("created_at") or "", note.get("body") or "") for note in notes]
    texts.sort(key=lambda dated_text: dated_text[0])

    links: list[str] = []
    for _, text in texts:
        for link in find_links(text):
            if link in links:
                links.remove(link)
            links.append(link)
    return links


def scan_issue_files(datafiles: list) -> list[T_scan]:
    all_scans = []
    for datafile in datafiles:
        issue_data = json.loads(datafile.read_text())
        iid: int = issue_data["iid"]
        art10label: str = ''

        art10labels = [l for l in issue_data["issue"]["labels"] if 'art10' in l]
        if art10labels: # for issues kept in the database while not having and art10 label anymore
            art10label = art10labels[0]

        ## Scan certain text bodies for file links & VINs.
        #
        issue_dict: dict = issue_data["issue"]
        title: str = issue_dict["title"]
        desc: str = issue_dict["description"]
        notes: list[str] = [i["body"] for i in issue_data["notes"]]
        # label_events: list = [(label_event["created_at"],label_event["label"]["name"]) for label_event in issue_data["label_events"]
        #                       if label_event["label"] and 'art10' in label_event["label"]["name"]]
        text_blob = "\n".join([title, desc, *notes])

        scanned = {
            "iid": iid,
            # "label": art10label,
            # "created_at": issue_dict["created_at"],
            # "closed_at": issue_dict["closed_at"],
            # "label_events": label_events,
            "VINs": find_VINs("\n".join([title, desc])),
            "links": _chronological_links(issue_dict, issue_data["notes"]),
        }
        if "assword" in text_blob:
            scanned["pswd"] = True

        all_scans.append(scanned)

    return all_scans

# upload archive
def read_issue_files(datafiles: list) -> list[T_scan]:
    # read all issues files and get the status: the input must be sorted, as only last file for a given issue is remembered
    report_by_issue = {}
    for datafile in datafiles:
        issue_data = json.loads(datafile.read_text())

        iid: int = issue_data["iid"]
        issued: dict = issue_data["issue"]

        ## TODO: check if resources come back sorted by date?
        # seems so
        all_label_dates = [i["created_at"] for i in issue_data["label_events"]]
        if all_label_dates:
            assert all_label_dates == sorted(all_label_dates), (
                all_label_dates,
                sorted(all_label_dates),
            )

        # art10label: str = ''

        # art10labels = [l for l in issue_data["issue"]["labels"] if 'art10' in l]
        # if art10labels: # for issues kept in the database while not having and art10 label anymore
        #     art10label = art10labels[0]

        label_dates = {
            i["label"]["name"]: i["created_at"]
            for i in issue_data["label_events"]
            if i["label"] is not None and i["action"] == "add" and i["label"]["name"].startswith("art10::")
        }

        all_mile_evs = issue_data["milestone_events"]
        last_mile_ev = all_mile_evs and all_mile_evs[-1]
        if last_mile_ev and last_mile_ev["action"] == "add":
            # last_mile = last_mile_ev["milestone"]
            milestone = {
                # NOTE: use milestone title from issue, event's title may differ
                # if it has been renamed in the mean time.
                "milestone": issue_data["issue"]["milestone"]["title"],
                "milestone_date": last_mile_ev["created_at"],
            }
        elif issue_data["issue"]["milestone"]:
            milestone = {
                # NOTE: use milestone title from issue, event's title may differ
                # if it has been renamed in the mean time.
                "milestone": issue_data["issue"]["milestone"]["title"],
                "milestone_date": None,
            }
        else:
            milestone = {}

        issue_values = filter_nested_dict(
            issued,
            [
                "state",
                "author.name",
                "author.id",
                "created_at",
                "closed_at",
                "closed_by.name",
                "assignee.name",
                "confidential",
                "discussion_locked",
            ],
        )

        art10_label = [
            i
            for i in get_nested(issue_data, ["issue", "labels"])
            if i.startswith("art10::")
        ]
        art10_label = art10_label[0] if art10_label else None

        report_by_issue[iid] = {
            **issue_values,
            "art10_label": art10_label,
            **label_dates,
            **milestone,
        }

    issues_report_df = pd.DataFrame.from_dict(report_by_issue, orient="index")
    issues_report_df = (
        issues_report_df.rename(
            columns={"confidential": "is_confidential", "discussion_locked": "is_locked"}
        )
        .rename_axis("iid")
        .sort_index()
    )

    return issues_report_df

def check_archive(fpath: Path | str, *, password=None, defaultpassword=None, verbosity=0) -> str | None:
    """Return any archive-error as string or None if ok."""
    try:
        patoolib.test_archive(str(fpath), password=password, verbosity=verbosity)
        err = None
    except Exception as ex:
        err = f"{type(ex).__name__}: {ex}"
        try:
            patoolib.test_archive(
                str(fpath),
                password=defaultpassword, # getattr(mystuff, "DEFAULT_ARCHIVE_PASSWORD", None)
                verbosity=verbosity,
            )
            err = None
        except Exception:
            pass

    return err


def extract_archive(
    fpath: Path | str, outdir: Path | str, *, password=None, defaultpassword=None, verbosity=0
) -> str | None:
    """Return any archive-error as string or None if ok."""


    err = None
    simple_zip_case = str(fpath).endswith('.zip') and (password is None)

    if simple_zip_case:

        try:
            shutil.unpack_archive(str(fpath), outdir)
            # works better to unpack zip file containing both simlink and abs path
            # avoid to unpack twice (in root and in a subfolder)

        except Exception as ex:
            err = f"{type(ex).__name__}: {ex}"

    if (not simple_zip_case) or (err != None):

        try:
            patoolib.extract_archive(
                str(fpath), outdir=outdir, password=password, verbosity=verbosity
            )
            err = None
        except Exception as ex:
            err = f"{type(ex).__name__}: {ex}"
            try:
                patoolib.extract_archive(
                    str(fpath),
                    outdir=outdir,
                    password=defaultpassword, # getattr(mystuff, "DEFAULT_ARCHIVE_PASSWORD", None)
                    verbosity=verbosity,
                )
                err = None
            except Exception:
                pass

    return err

# parse xml files

def el_text(xml_tree, *xpaths: str) -> str | None:
    for xpath in xpaths:
        el = xml_tree.find(xpath)
        if el is not None:
            return el.text


def parse_xsd_type(typ: str) -> str | None:
    for t in reversed(typ.split(":")):
        if t and not re.match(r"^v\d", t):
            return t