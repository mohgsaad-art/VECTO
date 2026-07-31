# CI_CD Python Environment

## Setup (first time)

### Option A — Conda

```bash
conda env create -f environment.yml
```

### Option B — Python venv

```bash
python -m venv .venv
```

**Linux / macOS:**
```bash
source .venv/bin/activate
```

**Windows:**
```cmd
.venv\Scripts\activate
```

Then install the dependencies:
```bash
pip install -r requirements.txt
```

## Activate

### Conda

**Linux / macOS:**
```bash
conda activate vecto_env
```

**Windows:**
```cmd
conda activate vecto_env
```

### Python venv

**Linux / macOS:**
```bash
source .venv/bin/activate
```

**Windows:**
```cmd
.venv\Scripts\activate
```

## Update (if environment.yml changes)

### Conda
```bash
conda env update -f environment.yml --prune
```

### Python venv
```bash
pip install --upgrade -r requirements.txt
```

## Running the scripts

All scripts must be run from the `CI_CD` directory so that the `FUNCTIONS` package is on the path.

```bash
cd BuildTools/CI_CD
```

### A01 — Update Article 10 test pool from GitLab
```bash
python A01_Update_Pool.py <pool_root>
    [--gitlab-url <url>] [--project-path <path>] [--token <token>] [--credentials <path>]
    [--ssl-verify <ca_bundle_or_false>]
```
- `pool_root`: root of the local pool directory (contains `A00_RAW` and `A01_EXTRACT` subdirs).
- `--gitlab-url`: GitLab instance URL (default: `https://code.europa.eu`).
- `--project-path`: GitLab project path (default: `vecto/vecto`).
- `--token`: GitLab private token. Falls back to `credentials.GITLAB_TOKEN` if not supplied.
- `--credentials`: path to a `credentials.py` file. Defaults to `credentials.py` resolved from `sys.path`.
- `--ssl-verify`: SSL verification — path to a CA bundle file, or `false` to disable (use `false` for servers with self-signed certificates). Defaults to system CA verification.
- Connects to GitLab, downloads new issue attachments since the last run, extracts archives, and saves a `status.json` timestamp to `<pool_root>`.

### A02 — Generate HTCondor-ready zipped inputs from generic and issue vehicle files
```bash
python A02_Generate_Files_To_Run.py <generic_input_dir> <issues_input_dir> <output_dir> [--force-repackage]
```
- `generic_input_dir`: directory containing generic `.vecto` and `.xml` vehicle input files (e.g. `01_INPUTS_GENERIC`).
- `issues_input_dir`: root of the Article 10 test pool (contains `A00_RAW` and `A01_EXTRACT` subdirs).
- `output_dir`: output root. Zips are written to `<output_dir>/To_Run/<vehicle>/input.zip`. Pass `<output_dir>/To_Run` as `test_pool_path` to B01/B12.
- `--force-repackage`: re-zip even if `<vehicle>/input.zip` already exists.
- Generic vehicles are placed under `<output_dir>/To_Run/<mode>/<powertrain>/<vehicle>/`.
- Issue vehicles are placed under `<output_dir>/To_Run/articles/<vehicle>/`.
- No intermediate folders are created: all vehicles are collected in memory and written directly as `input.zip` files.
- Produces a `Vehicles_found_<timestamp>.xlsx` diagnostic report in the current working directory (only the most recent is kept).
- **Override mechanism (`A02_OTHERS`):** files placed under `A02_OTHERS` replace the corresponding inputs from `A00_RAW`. To override a file, mirror its path under `A02_OTHERS` — the sorting algorithm picks the `A02_OTHERS` copy instead of the original. For example, to override `A00_RAW/555/uploads/23d4ert546tfcb45mnbd3/original_input.xml`, place the replacement at `A02_OTHERS/555/uploads/23d4ert546tfcb45mnbd3/modified_input.xml`.

### A03 — Scan extracted pool XML metadata
```bash
python A03_Scan_XML_Metadata.py <extract_root> [--output-dir <path>] [--copy-only] [--force]
```
- `extract_root`: root directory to scan (typically `A01_EXTRACT`). Nested subfolders and `.zip`/`.7z`/`.rar` archives are included.
- `--output-dir`: directory to write output into (default: current directory).
- `--copy-only`: copy every XML file found into `<output-dir>/A03XMLs` instead of parsing them — no `Manufacturer`/`VIN`/`CertificationNumber` extraction is performed in this mode. Copied filenames are prefixed with an incrementing counter to avoid collisions between same-named files from different source folders.
- `--force`: before running, delete this script's own previous outputs already present in `--output-dir` (`Manufacturer.txt`, `VIN.txt`, `CertificationNumber.txt`, `A03XMLs/`). Other, unrelated content in `--output-dir` is left untouched.
- Walks `extract_root` for `.xml` files. Any archive encountered is extracted to a temporary directory (one level deep only — archives nested inside another archive's extracted output are not followed) and scanned for `.xml` files as well.
- Without `--copy-only`: every XML file is parsed for `Manufacturer`, `VIN`, and `CertificationNumber` elements (namespace-agnostic match) — all occurrences are collected, since these elements can repeat per file (once per vehicle plus once per component/stage). Placeholder/non-values are excluded: `NA`, `N.A.`, `N/A`, `None`, `void`, `token` (case-insensitive), and any `##...##` template placeholder (e.g. `##TYRE_2_MANUFACTURER##`). Produces `Manufacturer.txt`, `VIN.txt`, and `CertificationNumber.txt` in `--output-dir`, each a sorted, deduplicated list of the values found, one per line.
- Temporary extraction directories (for archives found while walking) are not cleaned up automatically.
- If an XML file fails to parse, a `WARNING: failed to parse ...` line is printed to stdout. To avoid leaking local filesystem paths into CI logs, only the file's immediate parent folder name is printed (not the full path), and the trailing `(path/URI, line N)` segment embedded in the underlying lxml error message is stripped entirely.

### C04 — Generate HTML dashboard
```bash
python C04_Generate_HTML_Report.py [--report-dir <path>] [--interim-report]
```
- `--report-dir`: directory inside which the `Reports` folder is located (default: `D:/Reports` if `D:/` exists, otherwise `C:/Reports`).
- `--interim-report`: read from and write to the `Interim_Reports` subfolder of the report directory.
- Reads the most recent `OR*.xlsx` from the `Reports` folder — i.e. the combined report produced by C01.
- Produces a self-contained `HTML_REPORT_{Date}_{CommitHash}.html` — open directly in any browser, no server needed.
- Features: colour-coded pass/fail rows, summary counts, live search filter, sortable columns.
- Displays a `Delta` column when the OR report contains a `Delta Status` column (reference delta), and a `Trans Delta` column when it contains a `Delta Status_MR` column (Intermediate/MR delta).
- Displays a `Regression` column when the OR report contains a `Regression Status` column — shows `ERROR` for vehicles that passed in the reference run but fail in the current run, `OK` otherwise, and blank for new vehicles with no reference entry.
- Summary cards show warning counts for each delta type and a regression error count when present.

### 00 — Extract missing vehicles
```bash
python 00_Extract_Missing_Vehicles.py <vecto_root> <test_pool_path> [--reports-cache <path>] [--output <path>]
```
- `vecto_root`: path to the VECTO installation directory (used to resolve git tags when searching for the latest OR report).
- `test_pool_path`: root directory of the test pool. Input files are expected directly under `<test_pool_path>/<vehicle>/input.zip`.
- `--reports-cache`: directory containing `OR_*.xlsx` report files. Defaults to the standard CI Reports directory (`D:/CI/Reports` or `C:/CI/Reports`).
- `--output`: output file path for missing vehicle input directories (default: `./missing_vehicles.txt`).
- Compares vehicles in the test pool against the latest OR report and writes the input directory path of each missing vehicle (one per line) to the output file.
- Exits with a return code equal to the number of missing vehicles (0 if none).
- Implements the Case 2 backwards-compatibility check: run only new/un-run vehicles against the current VECTO version.

### B01 — Submit test cases to HTCondor
```bash
python B01_Run_Test_Files_with_HTCondor.py <vecto_root> <test_pool_path> [--queue-name <name>] [--ref-vecto-root <path>] [--reports-cache <path>] [--limit N]
```
- `vecto_root`: path to the VECTO installation directory (passed through to B12 and B13).
- `test_pool_path`: root directory of the test pool (input files expected directly under `<test_pool_path>/<vehicle>/input.zip`).
- `--queue-name`: HTCondor queue name used to track this submission (default: basename of `vecto_root`).
- `--ref-vecto-root`: path to the reference (previous official) VECTO installation directory. When supplied, missing vehicles are also queued against this version for delta comparison (Case 1 backward-compatibility). Also used to derive the MR VECTO root (`<ref-vecto-root>/MR`).
- `--reports-cache`: directory containing `OR_*.xlsx` report files, used to detect vehicles missing from the reference version report. Passed through to B12.
- `--limit N`: truncate the queue to the first N jobs after generation. If omitted, all jobs are submitted.
- Requires HTCondor (`condor_submit`, `condor_q`) to be available on the system.
- Internally calls B12 to generate the queue file, then submits via `condor_submit`.
- Polls `condor_q` every 30 s until the queue is empty (timeout: 10 h).
- Extracts result archives via B14 once all jobs finish. If `<ref-vecto-root>/MR` exists, also extracts MR result archives.

### B12 — Generate HTCondor queue file
```bash
python B12_Generate_HTCondor_Queue.py <vecto_root> <test_pool_path> [--force-rerun] [--no-hev] [--ref-vecto-root <path>] [--reports-cache <path>]
```
- `vecto_root`: path to the VECTO installation directory.
- `test_pool_path`: root directory of the test pool. Input files are expected directly under `<test_pool_path>/<vehicle>/input.zip`.
- `--force-rerun`: include jobs whose output archive already exists in `Z00_VECTO_OUTPUTS` (default: skip completed jobs).
- `--no-hev`: exclude HEV vehicles from the job list.
- `--ref-vecto-root`: path to the reference (previous official) VECTO installation directory. When supplied, two extra job groups are appended: (1) vehicles missing from the reference OR report, run against the reference executable; (2) vehicles missing from the MR OR report (`<reports-cache>/MR`), run against the MR executable (`<ref-vecto-root>/MR`). This enables both the reference delta (C44) and the Intermediate (MR) delta.
- `--reports-cache`: directory containing `OR_*.xlsx` report files used to detect missing vehicles for the reference version. Defaults to the standard CI Reports directory. MR reports are read from `<reports-cache>/MR`.
- Writes `batch_arguments.txt` in the current working directory — one line per job in the format `<vecto_root> <input_dir> <output_filename>`. Current-version jobs come first, then reference-version jobs, then MR-version jobs.
- Prepares the job list before submission.

### B13 — Run a single VECTO job on an HTCondor worker
```bash
python B13_Run_Single_Vecto_File.py <vecto_root> <input_archive> <output_filename>
```
- `vecto_root`: path to the VECTO installation directory, or a `.zip` archive of it (unpacked to `_CONDOR_SCRATCH_DIR`).
- `input_archive`: path to the zipped input files (`input.zip`).
- `output_filename`: output archive name (without extension). Result is saved as `<output_filename>.zip`.
- Called by the HTCondor submit script — not intended to be run directly.
- Unpacks the input archive, runs VECTO, and archives the results.

### B14 — Extract HTCondor result archives
```bash
python B14_Unzip_Results.py <vecto_root> [--force-rerun]
```
- `vecto_root`: path to the VECTO installation directory. Used to determine the version subfolder (`XX_RESULTS/<version>/`, `Z00_VECTO_OUTPUTS/<version>/`).
- `--force-rerun`: overwrite already-extracted result directories (default: skip existing).
- Moves `*_out.zip` files from the current working directory into `Z00_VECTO_OUTPUTS/<version>/` (created in cwd).
- Unpacks each archive into `XX_RESULTS/<version>/` (created in cwd), reconstructing the directory tree from the flat output filename.
- Called automatically by B01 after all HTCondor jobs finish.

### C01 — Generate all reports
```bash
python C01_Generate_Reports.py <vecto_root> [--report-dir <path>] [--interim-report] [--reports-cache <path>] [--mr-reports-cache <path>] [--ref-vecto-root <path>] [--is-mr-run]
```
- `vecto_root`: path to the VECTO installation directory (used to retrieve version and commit hash for the report name).
- `--report-dir`: directory inside which a `Reports` folder will be created (default: `D:/CI/Reports` if `D:/` exists, otherwise `C:/CI/Reports`).
- `--interim-report`: write reports to an `Interim_Reports` subfolder of the report directory.
- `--reports-cache`: directory used to determine the next report number (NNNN) and to receive a copy of the generated report. Created automatically if it does not exist. Defaults to the same directory as the report output.
- `--mr-reports-cache`: directory where the single MR OR report is stored. Defaults to `<reports-cache>/MR`.
- `--ref-vecto-root`: path to `REF_VECTO_ROOT`. Required when `--is-mr-run` is set; the MR VECTO executable is taken from `<ref-vecto-root>/MR`.
- `--is-mr-run`: when set, generates (or refreshes) the MR OR report from `XX_RESULTS/<mr_version>/` before generating the current report. The old MR OR report in `<mr-reports-cache>` is deleted first (single-report invariant). After this flag's logic runs, the Intermediate Delta is produced regardless.
- Reads test cases from `XX_RESULTS` in the current working directory.
- Report naming convention: `OR_{Version}_{Date}_{NNNN}_{Hash}[_suffix].xlsx` (e.g. `OR_5.1.1_260408_0001_a1b2c3d.xlsx`). NNNN is one higher than the highest number already present in the cache directory, ensuring no collision on re-runs.
- Output: a single combined `.xlsx` containing:
  - `Success`, `cpu_time` — pass/fail summary sheets
  - `CO2__g_km_`, `FC-Final__g_km_`, `EC_el_final__kWh_km_`, … — VSUM detail sheets
  - `CRF_CO2__g_km_`, `CRF_FuelConsumption__g_km_`, `CRF_FuelConsumption__MJ_km_` — MRF/CRF fuel consumption sheets
  - `stats_of_non-zero_variations` — VSUM deviation statistics
  - `Delta Summary`, `Δ<var>` — reference delta sheets (when a release OR report is available in `--reports-cache`)
  - `Delta Summary_MR`, `Δ<var>_MR` — Intermediate (MR) delta sheets (when an MR OR report is available in `--mr-reports-cache`)
- The `Success` sheet gains a `Delta Status` column (reference delta) and/or a `Delta Status_MR` column (Intermediate Delta) when the respective delta data is present.
- On every run, output files of failed vehicles (any `Success` containing `'error'`) are copied to `D:\CI\FailedReports\<run_label>\<vehicle>\` (or `C:\CI\FailedReports` when `D:\` is not present). `<run_label>` matches the OR filename stem (`<Version>_<Date>_<NNNN>_<Hash>`), grouping all failures from one run together.
- Run subdirectories in `FailedReports` older than 10 days are automatically purged on each run.
- Exits with a non-zero return code equal to the number of failed vehicles if any errors are detected.

### C44 — Generate delta report between two OR reports
```bash
python C44_Generate_Delta_Report.py <ref_report> <current_report> [--output <path>]
```
- `ref_report`: path to the reference (old) OR report `.xlsx`.
- `current_report`: path to the current (new) OR report `.xlsx`.
- `--output`: output `.xlsx` path. Defaults to `DELTA_REPORT_{Date}_{Hash}.xlsx` in `CI_CD/Reports`.
- Computes per-test-case absolute and percentage deltas for CO2, FC-Final, and EC_el_final.
- Produces a `Delta Summary` sheet (with `OK`/`WARNING` status per test case, threshold: 0.03%) and one detail sheet per variable.
- Also callable as a library:
  - `append_delta_to_report(ref, current, sheet_suffix='')` — appends delta sheets directly to an existing OR report and patches the `Success` sheet with a `Delta Status<sheet_suffix>` column. Use `sheet_suffix='_MR'` to write the Intermediate Delta alongside the reference delta in the same workbook without overwriting existing sheets.
  - `generate_delta_report(ref, current, output_path=None)` — writes a standalone delta `.xlsx`.
- Returns `None` (and skips writing) if no matching variable sheets are found in both reports.

#### Troubleshooting: delta column not appearing in the report

If the `Delta` or `Inter Delta` column is absent from the HTML/OR report, check:

- **Reports cache**: confirm that at least one `OR_*.xlsx` file is present in
  `--reports-cache` (for reference delta) or `--mr-reports-cache` (for Inter Delta).
- **Reference VECTO version**: the version string of the reference OR report must
  match the current run's version (e.g. reference `5.1.1` vs. current `5.1.1-DEV`).
  A mismatch causes no vehicles to be paired and the delta is silently skipped.
- **Root vs. MR cache**: for a missing `Delta`, check the root `--reports-cache`
  folder; for a missing `Inter Delta`, check the `--mr-reports-cache` folder
  (defaults to `<reports-cache>/MR`).
