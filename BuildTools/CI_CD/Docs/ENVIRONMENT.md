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
- Issue vehicles labeled `art10::rejected`, `art10::open`, or `art10::need_info` are excluded from the run.
- **Bus job files:** for a completed bus A02 writes a `job.vecto`. Primary + completed XML → FileVersion 10 (`PrimaryVehicle`, `InterimStep`, `Completed`, `RunSimulation` = the completed file's valid-to-run result). VIF + completed XML → FileVersion 7 (`PrimaryVehicleResults`, `CompletedVehicle`) with **no** `RunSimulation`: a `false` there makes VECTO skip the completed simulation, so v7 jobs always simulate.
- **Confidential inputs:** every issue vehicle XML is made confidential with A04's `anonymize_xml_bytes` (see A04) before it is zipped, and is named `<vehicle_type>_confidential.xml` (the generated bus `job.vecto` references these names). Generic vehicles are not modified. Duplicate removal uses the original files, so it is unaffected. Existing `input.zip` files are only rewritten with `--force-repackage`.
- **Duplicate removal:** issue vehicles sharing the same data hash are run once. The hash is computed on the XML with non-simulation fields blanked (`VIN`, `Date`, `DigestValue`, `Manufacturer`, `ManufacturerAddress`, `Model`, `Make`, `AppVersion`, `CertificationNumber`, `SimulationToolLicenseNumber`, `TypeApprovalNumber`, `VectoLicenseNbr`, `VehicleTypeApprovalNumber`, plus the parent's `id` attribute). For a completed-bus file that is not valid to run, the hash is replaced by its `BodyworkCode`, since only that drives the primary-step missions. Known limitation (deferred to the next planned hash/naming change, since it renames almost every job): signature `URI="#<id>"` attributes are not blanked, so an already-anonymised file (A04 renames ids and URIs) hashes differently from its original — A02 avoids this by hashing the original files. Job names are content hashes: when an issue holds several uploaded versions of a vehicle, A02 keeps the last one in the issue's link order, and another chosen version means another job name.
- Issue vehicle folders are named `<VehicleCategory>_<Powertrain>_<ShortHash>`. Vehicles with multiple powertrains get an `m` prefix on the powertrain (e.g. `mCONV`).
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
- Walks `extract_root` for `.xml` files. Any archive encountered is extracted to a temporary directory (one level deep only — archives nested inside another archive's extracted output are not followed) and scanned for `.xml` files as well. Files whose name starts with `DEBUG_VIN_` (case-insensitive) are intermediate debug artifacts, not valid declaration XML, and are excluded entirely — not counted, copied, or parsed.
- Without `--copy-only`: every XML file is parsed for `Manufacturer`, `VIN`, and `CertificationNumber` elements (namespace-agnostic match) — all occurrences are collected, since these elements can repeat per file (once per vehicle plus once per component/stage). Placeholder/non-values are excluded: `NA`, `N.A.`, `N/A`, `None`, `void`, `token` (case-insensitive), and any `##...##` template placeholder (e.g. `##TYRE_2_MANUFACTURER##`). Produces `Manufacturer.txt`, `VIN.txt`, and `CertificationNumber.txt` in `--output-dir`, each a sorted, deduplicated list of the values found, one per line.
- Temporary extraction directories (for archives found while walking) are not cleaned up automatically.
- If an XML file fails to parse, a `WARNING: failed to parse ...` line is printed to stdout. To avoid leaking local filesystem paths into CI logs, only the file's immediate parent folder name is printed (not the full path), and the trailing `(path/URI, line N)` segment embedded in the underlying lxml error message is stripped entirely.

### A04 — Make identifying XML element values confidential
```bash
python A04_Generalize_Data.py <data_dir> [--process-zips]
python A04_Generalize_Data.py -e | --use-env-var [--process-zips]
```
- `data_dir`: directory to walk for `.xml` files. Ignored if `-e`/`--use-env-var` is passed.
- `-e`, `--use-env-var`: read the directory to process from the `A04_DATA_DIR` environment variable instead of the `data_dir` argument.
- `--process-zips`: also unzip, process and re-zip `.zip` files found while walking `data_dir` (one level deep). Without it only loose `.xml` files are processed; `.7z`/`.rar` are never followed.
- In every `.xml` file found, in place, for every `VIN`, `Date`, `DigestValue`, `Manufacturer`, `ManufacturerAddress`, `Model`, `AppVersion`, `CertificationNumber`, `SimulationToolLicenseNumber`, `Make`, `TypeApprovalNumber`, `VectoLicenseNbr` and `VehicleTypeApprovalNumber` element (namespace-agnostic match, all occurrences):
  - `Date` is set to `2020-01-01T08:00:00Z` (it must stay a valid `xs:dateTime`); every other field is set to `CONFIDENTIAL`. Empty elements are left as they are.
  - The `id` attribute of each such element's parent is renamed `Confidential-ID00`, `Confidential-ID01`, … in document order, and every `URI="#<old id>"` (e.g. a component signature's `di:Reference`) is updated to match, so the original id cannot leak through it.
- The result still passes VECTO's XSD validation and simulates: VECTO copies signature references and digests into its reports without verifying them.
- A02 applies the same transformation to all issue vehicle inputs (see A02).
- A04 makes every value it finds in these fields confidential, across all files in a directory. Rather than typing the directory path every time, set it once in the `A04_DATA_DIR` environment variable and run the script with `-e` to reuse it:
  ```powershell
  python.exe .\A04_Generalize_Data.py ".\Group 54 XML"
  ```
  or, with `A04_DATA_DIR` set:
  ```powershell
  python.exe .\A04_Generalize_Data.py -e
  ```

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
- Requires the environment variable **`zipmep`** (password of the job output archives); B01 stops before submitting if it is missing or empty. The submit template forwards it to every job (`environment = "zipmep=$ENV(zipmep)"`); avoid quote characters in the password. In CI define it as a masked GitLab variable.
- Internally calls B12 to generate the queue file, then submits via `condor_submit`.
- Polls `condor_q` every 30 s until the queue is empty (timeout: 10 h).
- Extracts result archives via B14 once all jobs finish: once for the current version, once for `--ref-vecto-root` (`--suffix _REF`) if supplied, and once for `<ref-vecto-root>/MR` (`--suffix _MR`) if it exists — so each version's results land in its own `Z00_VECTO_OUTPUTS`/`XX_RESULTS` folder.

### B12 — Generate HTCondor queue file
```bash
python B12_Generate_HTCondor_Queue.py <vecto_root> <test_pool_path> [--force-rerun] [--no-hev] [--ref-vecto-root <path>] [--reports-cache <path>]
```
- `vecto_root`: path to the VECTO installation directory.
- `test_pool_path`: root directory of the test pool. Input files are expected directly under `<test_pool_path>/<vehicle>/input.zip`.
- `--force-rerun`: include jobs whose output archive already exists in `Z00_VECTO_OUTPUTS` (default: skip completed jobs).
- `--no-hev`: exclude HEV vehicles from the job list.
- `--ref-vecto-root`: path to the reference (previous official) VECTO installation directory. When supplied, two extra job groups are appended: (1) vehicles missing from the reference OR report, run against the reference executable, output filenames suffixed `_REF`; (2) vehicles missing from the MR OR report (`<reports-cache>/MR`), run against the MR executable (`<ref-vecto-root>/MR`), output filenames suffixed `_MR`. These suffixes let B14 tell which VECTO version produced each result archive. This enables both the reference delta (C44) and the Intermediate (MR) delta.
- `--reports-cache`: directory containing `OR_*.xlsx` report files used to detect missing vehicles for the reference version. Defaults to the standard CI Reports directory. MR reports are read from `<reports-cache>/MR`.
- Writes `batch_arguments.txt` in the current working directory — one line per job in the format `<vecto_root> <input_dir> <output_filename>`. Current-version jobs come first, then reference-version jobs, then MR-version jobs.
- Prepares the job list before submission.

### B13 — Run a single VECTO job on an HTCondor worker
```bash
python B13_Run_Single_Vecto_File.py <vecto_root> <input_archive> <output_filename>
```
- `vecto_root`: path to the VECTO installation directory, or a `.zip` archive of it. Either way it is staged under `_CONDOR_SCRATCH_DIR/VECTO_EXE/<name>` (unpacked if a zip, copied if a directory) before running.
- `input_archive`: path to the zipped input files (`input.zip`).
- `output_filename`: output archive name (without extension). Result is saved as `<output_filename>.zip`.
- Called by the HTCondor submit script — not intended to be run directly.
- Unpacks the input archive, runs VECTO, and archives the results.
- The result archive is **password-protected** (AES-256, WinZip AES — open with 7-Zip/WinRAR, not Windows Explorer) with the password from the `zipmep` environment variable (`FUNCTIONS/SECURE_ZIP.py`). The job fails at start, before running VECTO, if `zipmep` is missing or empty.
- Requires `pyzipper` in the Python used by HTCondor jobs (`D:\Apps\Python\WPy64-312101`, not `vecto_env`): `python -m pip install pyzipper`. If pyzipper is not installed, B13/B14 fall back to the **7-Zip** command line, looked up in this order: env `SEVEN_ZIP_PATH`, `7z` on `PATH`, `C:\Program Files\7-Zip\7z.exe`; if neither backend is available the job fails. Both backends write the same AES-256 zip format. Note: with 7-Zip the password is passed on its command line (visible in the process list while it runs); pyzipper has no such exposure and is preferred.

### B14 — Extract HTCondor result archives
```bash
python B14_Unzip_Results.py <vecto_root> [--force-rerun] [--suffix {_REF,_MR}]
```
- `vecto_root`: path to the VECTO installation directory. Used to determine the version subfolder (`XX_RESULTS/<version>/`, `Z00_VECTO_OUTPUTS/<version>/`).
- `--force-rerun`: overwrite already-extracted result directories (default: skip existing).
- `--suffix`: only pick up output zips carrying this job-type suffix (`_REF`/`_MR`, added by B12 for `--ref-vecto-root`/MR jobs). Default (unset) picks up only unsuffixed, current-version results. HTCondor drops every job's output zip into the same working directory regardless of which VECTO executable produced it, so this filter is what keeps reference/MR results out of the current version's folder — call B14 once per version, each with the matching `--suffix`, rather than once for everything.
- Moves matching `*_out.zip` files from the current working directory into `Z00_VECTO_OUTPUTS/<version>/` (created in cwd); the suffix is stripped when reconstructing the result path.
- Unpacks each archive into `XX_RESULTS/<version>/` (created in cwd), reconstructing the directory tree from the flat output filename. Archives are decrypted with the `zipmep` password (B14 fails if it is missing); older unencrypted archives still extract. `Z00_VECTO_OUTPUTS` keeps the encrypted archives.
- Called automatically by B01 after all HTCondor jobs finish (once per version, with the matching `--suffix`).

### C01 — Generate all reports
```bash
python C01_Generate_Reports.py <vecto_root> [--report-dir <path>] [--interim-report] [--reports-cache <path>] [--mr-reports-cache <path>] [--ref-vecto-root <path>] [--is-mr-run]
```
- `vecto_root`: path to the VECTO installation directory (used to retrieve version and commit hash for the report name).
- `--report-dir`: directory inside which a `Reports` folder will be created (default: `D:/CI/Reports` if `D:/` exists, otherwise `C:/CI/Reports`).
- `--interim-report`: write reports to an `Interim_Reports` subfolder of the report directory.
- `--reports-cache`: directory used to determine the next report number (NNNN) and to receive a copy of the generated report. Created automatically if it does not exist. Defaults to the same directory as the report output.
- `--mr-reports-cache`: directory where the single MR OR report is stored. Defaults to `<reports-cache>/MR`.
- `--ref-vecto-root`: path to `REF_VECTO_ROOT`. Required when `--is-mr-run` is set; the MR VECTO executable is taken from `<ref-vecto-root>/MR`. Whenever it's supplied, the reference OR report is *always* (re)generated from `XX_RESULTS/<ref_version>/` before the current report — no flag needed. The version is read from `<ref-vecto-root>/vectocmd.exe`, so pass the **same versioned folder given to B01/B12** (e.g. `<REF_VECTO_ROOT>/5.1.3`); if no version can be read a `WARNING` naming the path is printed. Because `XX_RESULTS/<ref_version>/` only holds the reference jobs of the current run, the new results are **merged** into the latest existing `OR_<ref_version>_*.xlsx` in `<reports-cache>` (cases present in the new results replace the old rows, all other cases are kept; the derived `stats of non-zero variations` sheet is dropped), written with the next NNNN, and the older report(s) are deleted. If the existing report can't be read, the new results are written alone. Requires B01/B12/B14 to have been run with `--ref-vecto-root` so `XX_RESULTS/<ref_version>/` is actually populated — B14 routes reference-build result archives there via its own `--suffix _REF` (see B14 below); without that, the refresh has nothing to read and prints a warning instead.
- `--is-mr-run`: when set, generates (or refreshes) the MR OR report from `XX_RESULTS/<mr_version>/` before generating the current report. The old MR OR report in `<mr-reports-cache>` is deleted first (single-report invariant). After this flag's logic runs, the Intermediate Delta is produced regardless.
- Reads test cases from `XX_RESULTS` in the current working directory.
- Report naming convention: `OR_{Version}_{Date}_{NNNN}_{Hash}[_suffix].xlsx` (e.g. `OR_5.1.1_260408_0001_a1b2c3d.xlsx`). NNNN is one higher than the highest number already present in the cache directory, ensuring no collision on re-runs.
- Output: a single combined `.xlsx` containing:
  - `Success`, `cpu_time` — pass/fail summary sheets. `Success` values per result folder:
    - `not run` (no `.vsum`, no `.log`), `ERROR no vsum`, `ERROR empty vsum`
    - `OK` — all runs `Success`/`PrimaryBusSimulationIgnore`, log matches vsum, no `Error` line in the (final) `RSLT_MANUFACTURER` file and, for v10 primary+completed jobs with `RunSimulation: true`, a final `.RSLT_MANUFACTURER.xml` exists
    - `ERROR Timeout` — the `.log` content mentions a timeout. Jobs removed by HTCondor's 12 h `periodic_remove` leave no output and appear as `not run`.
    - `ERROR VIF` — runs and log are fine, but the manufacturer report contains an `Error` line (e.g. results VECTO reports as errors for ignored primary runs)
    - `ERROR` — any other failure, including a v10 completed step that should have run but produced no final `.RSLT_MANUFACTURER.xml`
  - `CO2__g_km_`, `FC-Final__g_km_`, `EC_el_final__kWh_km_`, … — VSUM detail sheets
  - `CRF_CO2__g_km_`, `CRF_FuelConsumption__g_km_`, `CRF_FuelConsumption__MJ_km_` — MRF/CRF fuel consumption sheets
  - `stats_of_non-zero_variations` — VSUM deviation statistics
  - `Delta Summary`, `Δ<var>` — reference delta sheets (when a release OR report is available in `--reports-cache`)
  - `Delta Summary_MR`, `Δ<var>_MR` — Intermediate (MR) delta sheets (when an MR OR report is available in `--mr-reports-cache`)
- The `Success` sheet gains a `Delta Status` column (reference delta) and/or a `Delta Status_MR` column (Intermediate Delta) when the respective delta data is present.
- `log_ok` (shown as "Log/Vsum Match" in the C04 HTML report) cross-validates each per-cycle log progress line against its corresponding vsum result row(s), rather than checking for a raw `100%` substring in the log. A cycle is flagged as not OK if its vsum row(s) show a non-Success/non-PrimaryBusSimulationIgnore status while the log claims 100%, or if the log expected a cycle that has no vsum row at all (a silently dropped result).
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
