"""Shared test helpers that build a placeholder issue pool (A00_RAW / A01_EXTRACT) in a temp folder."""

import hashlib
import json
from pathlib import Path
from zipfile import ZipFile

ISSUE_DESC_FILENAME = 'issue_txt_data'
SAFE_LABEL = 'art10::approved'


def make_fake_hash(iid: int, filename: str) -> str:
    """Deterministic placeholder matching the [a-z0-9]{16,64} GitLab upload hash."""
    return hashlib.sha1(f"{iid}:{filename}".encode()).hexdigest()


def build_minimal_issue_json(iid: int, vin: str, label: str, upload_links: list[str] = ()) -> dict:
    """Minimal issue data accepted by scan_issue_files and read_issue_files.
    upload_links ("/uploads/<hash>/<filename>") go into the description so find_links picks them up."""
    description_lines = [f"VIN: {vin}"] + [f"({link})" for link in upload_links]
    return {
        "iid": iid,
        "issue": {
            "title": f"synthetic issue {iid:04d}",
            "description": "\n".join(description_lines),
            "labels": [label],
            "state": "closed",
            "author": {"name": "synthetic", "id": 0},
            "created_at": "2020-01-01T00:00:00Z",
            "closed_at": "2020-01-02T00:00:00Z",
            "closed_by": {"name": "synthetic"},
            "assignee": {"name": "synthetic"},
            "confidential": False,
            "discussion_locked": False,
            "milestone": None,
        },
        "notes": [],
        "label_events": [],
        "milestone_events": [],
    }


def synthesize_pool(test_pool_issue_dir: Path, out_pool_root: Path) -> None:
    """For each <iid>_<VIN>/input.zip under test_pool_issue_dir, write:
    - out_pool_root/A00_RAW/<iid>/issue_txt_data.json
    - out_pool_root/A01_EXTRACT/<iid>/uploads/<fake_hash>/<filename> (per entry)
    """
    for subdir in sorted(test_pool_issue_dir.iterdir()):
        if not subdir.is_dir():
            continue
        iid_str, vin = subdir.name.split("_", 1)
        iid = int(iid_str)

        upload_links = []
        with ZipFile(subdir / "input.zip") as zf:
            for name in zf.namelist():
                if name.endswith("/"):
                    continue
                fake_hash = make_fake_hash(iid, name)
                fname = Path(name).name
                dest = out_pool_root / "A01_EXTRACT" / iid_str / "uploads" / fake_hash / fname
                dest.parent.mkdir(parents=True, exist_ok=True)
                dest.write_bytes(zf.read(name))
                upload_links.append(f"/uploads/{fake_hash}/{fname}")

        json_path = out_pool_root / "A00_RAW" / iid_str / f"{ISSUE_DESC_FILENAME}.json"
        json_path.parent.mkdir(parents=True, exist_ok=True)
        json_path.write_text(json.dumps(build_minimal_issue_json(iid, vin, SAFE_LABEL, upload_links), indent=2))
