# -*- coding: utf-8 -*-
"""Password-protected zip archives for HTCondor job outputs (password from env var 'zipmep').

Archives are written with AES-256 (WinZip AES), readable by 7-Zip/WinRAR and pyzipper.
Plain (unencrypted) zips can still be extracted, so older job outputs keep working.
"""

import os
from pathlib import Path

import pyzipper

ZIP_PASSWORD_ENV = 'zipmep'


def get_zip_password() -> str:
    """Return the archive password from the environment; raise if it is missing or empty."""
    password = os.environ.get(ZIP_PASSWORD_ENV, '')
    if not password:
        raise RuntimeError(
            f"Environment variable '{ZIP_PASSWORD_ENV}' is not set or empty: it must hold the password "
            f"used to protect the job output archives."
        )
    return password


def write_encrypted_zip(src_dir, zip_path, password: str) -> None:
    """Zip every file under src_dir (paths relative to src_dir) into zip_path, AES-256 encrypted."""
    src_dir = Path(src_dir)
    with pyzipper.AESZipFile(zip_path, 'w', compression=pyzipper.ZIP_DEFLATED,
                             encryption=pyzipper.WZ_AES) as zf:
        zf.setpassword(password.encode('utf-8'))
        zf.setencryption(pyzipper.WZ_AES, nbits=256)
        for path in sorted(p for p in src_dir.rglob('*') if p.is_file()):
            zf.write(path, path.relative_to(src_dir).as_posix())


def extract_zip(zip_path, out_dir, password: str) -> None:
    """Extract zip_path (encrypted or plain) into out_dir."""
    try:
        with pyzipper.AESZipFile(zip_path) as zf:
            zf.setpassword(password.encode('utf-8'))
            zf.extractall(out_dir)
    except Exception as exc:
        raise RuntimeError(f'Could not extract {zip_path}: {exc}') from exc
