# -*- coding: utf-8 -*-
"""Password-protected zip archives for HTCondor job outputs (password from env var 'zipmep').

Archives are written with AES-256 (WinZip AES), readable by 7-Zip/WinRAR and pyzipper.
Backend: pyzipper when installed, otherwise the 7-Zip command line (SEVEN_ZIP_PATH, PATH or the
default install folder). Both write the same format, so writer and reader may use different backends.
Plain (unencrypted) zips can still be extracted, so older job outputs keep working.
"""

import os
import shutil
import subprocess
from pathlib import Path

ZIP_PASSWORD_ENV = 'zipmep'
SEVEN_ZIP_ENV = 'SEVEN_ZIP_PATH'
DEFAULT_7ZIP_PATH = r'C:\Program Files\7-Zip\7z.exe'


def get_zip_password() -> str:
    """Return the archive password from the environment; raise if it is missing or empty."""
    password = os.environ.get(ZIP_PASSWORD_ENV, '')
    if not password:
        raise RuntimeError(
            f"Environment variable '{ZIP_PASSWORD_ENV}' is not set or empty: it must hold the password "
            f"used to protect the job output archives."
        )
    return password


def _load_pyzipper():
    try:
        import pyzipper
    except ImportError:
        return None
    return pyzipper


def _find_7zip() -> str:
    candidates = [os.environ.get(SEVEN_ZIP_ENV), shutil.which('7z'), DEFAULT_7ZIP_PATH]
    for candidate in candidates:
        if candidate and os.path.isfile(candidate):
            return candidate
    tried = ', '.join(str(c) for c in candidates if c)
    raise RuntimeError(
        f'pyzipper is not installed and 7-Zip was not found (tried: {tried}). Install pyzipper '
        f'(pip install pyzipper) or 7-Zip, or set {SEVEN_ZIP_ENV} to 7z.exe.'
    )


def _run_7zip(args: list[str], password: str, cwd=None) -> None:
    result = subprocess.run([_find_7zip(), *args], cwd=cwd, capture_output=True, text=True)
    if result.returncode != 0:
        message = (result.stderr or result.stdout).strip().replace(password, '***')
        raise RuntimeError(f'7-Zip failed (exit code {result.returncode}): {message}')


def write_encrypted_zip(src_dir, zip_path, password: str) -> None:
    """Zip every file under src_dir (paths relative to src_dir) into zip_path, AES-256 encrypted."""
    src_dir = Path(src_dir)
    pyzipper = _load_pyzipper()
    if pyzipper is None:
        zip_path = Path(zip_path).resolve()
        zip_path.unlink(missing_ok=True)  # 7z would otherwise add to an existing archive
        _run_7zip(['a', '-tzip', '-mem=AES256', f'-p{password}', '-y', str(zip_path), '*'], password, cwd=src_dir)
        return

    with pyzipper.AESZipFile(zip_path, 'w', compression=pyzipper.ZIP_DEFLATED,
                             encryption=pyzipper.WZ_AES) as zf:
        zf.setpassword(password.encode('utf-8'))
        zf.setencryption(pyzipper.WZ_AES, nbits=256)
        for path in sorted(p for p in src_dir.rglob('*') if p.is_file()):
            zf.write(path, path.relative_to(src_dir).as_posix())


def extract_zip(zip_path, out_dir, password: str) -> None:
    """Extract zip_path (encrypted or plain) into out_dir."""
    pyzipper = _load_pyzipper()
    if pyzipper is None:
        _run_7zip(['x', f'-p{password}', '-y', f'-o{out_dir}', str(zip_path)], password)
        return

    try:
        with pyzipper.AESZipFile(zip_path) as zf:
            zf.setpassword(password.encode('utf-8'))
            zf.extractall(out_dir)
    except Exception as exc:
        raise RuntimeError(f'Could not extract {zip_path}: {exc}') from exc
