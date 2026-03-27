# CI_CD Python Environment

## Setup (first time)

```bash
conda env create -f environment.yml
```

## Activate

**Linux / macOS:**
```bash
conda activate vecto_env
```

**Windows:**
```cmd
conda activate vecto_env
```

## Update (if environment.yml changes)

```bash
conda env update -f environment.yml --prune
```

## Running the scripts

All scripts must be run from the `CI_CD` directory so that the `FUNCTIONS` package is on the path.

```bash
cd BuildTools/CI_CD
```

### A01 — Generate generic test files
```bash
python A01_Generate_Files_to_run_from_Generic.py <root_path>
```
- `root_path`: directory containing `01_INPUTS_GENERIC`. Output goes to `<root_path>/02_TO_RUN_GENERIC`.

### A02 — Generate issue test files
```bash
python A02_Generate_Files_to_run_from_Issues.py <root_path>
```
- `root_path`: directory containing `11_INPUTS_ISSUE`. Output goes to `<root_path>/12_TO_RUN_ISSUES/ISSUE`.

### B01 — Prepare XX_RESULTS run directory
```bash
python B01_Generate_All_Files_to_Run_for_Vecto_Versions.py <vecto_root> <test_pool_path> [--force-rerun]
```
- `vecto_root`: path to the VECTO installation directory.
- `test_pool_path`: directory containing `02_TO_RUN_GENERIC` and `12_TO_RUN_ISSUES`. `XX_RESULTS` will be created here.
- `--force-rerun`: re-copies input files into `XX_RESULTS` even if they already exist.

### B02 — Run test files with a VECTO version
```bash
python B02_Run_Test_Files_with_Vecto_Version.py <vecto_root> [--max-workers N] [--force-rerun]
```
- `vecto_root`: path to the VECTO installation directory (must contain `vectocmd.exe` on Windows, `vectocmd.dll` on Linux).
- `--max-workers`: number of parallel workers (default: number of CPU cores). Reduce on machines with limited memory.
- `--force-rerun`: re-run simulations even if a result file already exists.
- Reads test cases from `XX_RESULTS` in the current working directory (created by B01).

### C04 — Generate HTML dashboard
```bash
python C04_Generate_HTML_Report.py [--report-dir <path>] [--interim-report]
```
- `--report-dir`: directory inside which the `Reports` folder is located (default: `D:/Reports` if `D:/` exists, otherwise `C:/Reports`).
- `--interim-report`: read from and write to the `Interim_Reports` subfolder of the report directory.
- Reads the most recent `OR*.xlsx` from the `Reports` folder — i.e. the combined report produced by C01.
- Produces a self-contained `HTML_REPORT_{Date}_{CommitHash}.html` — open directly in any browser, no server needed.
- Features: colour-coded pass/fail rows, summary counts, live search filter, sortable columns.

### B03 — Submit test cases to HTCondor
```bash
python B03_Run_Test_Files_with_HTCondor.py <vecto_root> <test_pool_path> [--queue-name <name>]
```
- `vecto_root`: path to the VECTO installation directory (passed through to B32 and B33).
- `test_pool_path`: root directory of the test pool (input files expected under `<test_pool_path>/A01_INPUT_FILES`).
- `--queue-name`: HTCondor queue name used to track this submission (default: basename of `vecto_root`).
- Requires HTCondor (`condor_submit`, `condor_q`) to be available on the system.
- Internally calls B32 to generate the queue file, then submits via `condor_submit`.
- Polls `condor_q` every 30 s until the queue is empty (timeout: 10 h).
- Extracts result archives via `extract_results.py` once all jobs finish.

### B32 — Generate HTCondor queue file
```bash
python B32_generate_JB_queuefile.py <vecto_root> <test_pool_path> [--force-rerun] [--no-hev]
```
- `vecto_root`: path to the VECTO installation directory.
- `test_pool_path`: root directory of the test pool. Input files are expected under `<test_pool_path>/A01_INPUT_FILES`.
- `--force-rerun`: include jobs whose output archive already exists in `Z00_VECTO_OUTPUTS` (default: skip completed jobs).
- `--no-hev`: exclude HEV vehicles from the job list.
- Writes `batch_arguments.txt` in the current working directory — one line per job in the format `<vecto_root> <input_dir> <output_filename>`.
- HTCondor equivalent of B01: prepares the job list before submission.

### B33 — Run a single VECTO job on an HTCondor worker
```bash
python B33_run_single_vecto_file.py <vecto_root> <input_archive> <output_filename>
```
- `vecto_root`: path to the VECTO installation directory, or a `.zip` archive of it (unpacked to `_CONDOR_SCRATCH_DIR`).
- `input_archive`: path to the zipped input files (`input.zip`).
- `output_filename`: output archive name (without extension). Result is saved as `<output_filename>.zip`.
- Called by the HTCondor submit script — not intended to be run directly.
- HTCondor equivalent of B02's `run_individual_file`: unpacks, runs VECTO, archives results.

### B34 — Extract HTCondor result archives
```bash
python B34_extract_results.py [--force-rerun]
```
- `--force-rerun`: overwrite already-extracted result directories (default: skip existing).
- Moves `*_out.zip` files from the current working directory into `Z00_VECTO_OUTPUTS` (created in cwd).
- Unpacks each archive into `XX_RESULTS` (created in cwd), reconstructing the directory tree from the flat output filename.
- Called automatically by B03 after all HTCondor jobs finish.

### C01 — Generate all reports
```bash
python C01_Generate_Reports.py <vecto_root> [--report-dir <path>] [--interim-report] [--reports-cache <path>]
```
- `vecto_root`: path to the VECTO installation directory (used to retrieve version and commit hash for the report name).
- `--report-dir`: directory inside which a `Reports` folder will be created (default: `D:/CI/Reports` if `D:/` exists, otherwise `C:/CI/Reports`).
- `--interim-report`: write reports to an `Interim_Reports` subfolder of the report directory.
- `--reports-cache`: directory used to determine the next report number (NNNN) and to receive a copy of the generated report. Created automatically if it does not exist. Defaults to the same directory as the report output.
- Reads test cases from `XX_RESULTS` in the current working directory.
- Report naming convention: `OR_{Version}_{Date}_{NNNN}_{Hash}[_suffix].xlsx` (e.g. `OR_5.1.1_260408_0001_a1b2c3d.xlsx`). NNNN is one higher than the highest number already present in the cache directory, ensuring no collision on re-runs.
- Output: a single combined `.xlsx` containing:
  - `Success`, `cpu_time` — pass/fail summary sheets
  - `CO2__g_km_`, `FC-Final__g_km_`, `EC_el_final__kWh_km_`, … — VSUM detail sheets
  - `CRF_CO2__g_km_`, `CRF_FuelConsumption__g_km_`, `CRF_FuelConsumption__MJ_km_` — MRF/CRF fuel consumption sheets
  - `stats_of_non-zero_variations` — VSUM deviation statistics
- Exits with a non-zero return code equal to the number of failed vehicles if any errors are detected.
