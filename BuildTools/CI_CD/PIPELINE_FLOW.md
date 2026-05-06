# CI/CD Pipeline Flow

```mermaid
flowchart TD
    START([CI Pipeline Start]) --> A01

    %% ── Pool preparation ──────────────────────────────────────────────────
    A01["A01 — Update pool from GitLab <br> Download new issue attachments <br>Extract archives → A01_EXTRACT"]
    A01 --> A02

    A02["A02 — Generate To_Run zips. <br> Classify .vecto by mode & powertrain. <br> Classify XML issues by powertrain."]
    A02 --> A02d1

    A02d1{".vecto file?"}
    A02d1 -->|"Yes — detect mode <br> (DeclMode / XML refs)"| A02d2
    A02d1 -->|"No — XML issue vehicle"| A02d3

    A02d2{Powertrain}
    A02d2 -->|ICE / HEV / PEV / FCHV| A02g["GenericDeclaration/&lt;powertrain&gt;/&lt;vehicle&gt;/input.zip or GenericEngineering/…"]

    A02d3["Declaration/&lt;powertrain&gt;/&lt;category&gt;_&lt;hash&gt;/input.zip"]

    A02g --> B03
    A02d3 --> B03

    %% ── HTCondor submission ────────────────────────────────────────────────
    B03["B03 — Submit to HTCondor <br> Patch submit template <br> condor_submit"]
    B03 --> B32

    B32["B32 — Generate queue file <br> Walk test_pool_path for input.zip <br> Build job list"]
    B32 --> B32d1

    B32d1{"--force-rerun?"}
    B32d1 -->|No| B32d2["Skip already-completed <br> (*_out.zip exists)"]
    B32d1 -->|Yes| B32d3

    B32d2 --> B32d3

    B32d3{"--no-hev?"}
    B32d3 -->|Yes| B32d4["Exclude HEV jobs"]
    B32d3 -->|No| B32d5

    B32d4 --> B32d5

    B32d5{"--ref-vecto-root <br> supplied?"}
    B32d5 -->|Yes| B32d6["get_missing_ids() <br> vs latest release OR report"]
    B32d5 -->|No| B32write

    B32d6 --> B32d7{"REF OR report <br> found?"}
    B32d7 -->|No| B32write
    B32d7 -->|Yes — append REF jobs| B32d8

    B32d8{"MR dir exists? <br> (ref_root/MR)"}
    B32d8 -->|Yes| B32d9["get_missing_ids() <br> vs MR OR report <br> Append MR jobs"]
    B32d8 -->|No| B32write

    B32d9 --> B32write["Write batch_arguments.txt <br> (current + REF + MR jobs)"]
    B32write --> B03lim

    B03lim{"--limit N set?"}
    B03lim -->|Yes| B03trunc["Truncate queue to N jobs"]
    B03lim -->|No| B03submit

    B03trunc --> B03submit["condor_submit <br> htc_submit_generated.sh"]
    B03submit --> B03poll

    B03poll["Poll condor_q every 30s <br> (max 10h)"]
    B03poll --> B03d1{"QUEUE_NAME <br> in condor_q?"}
    B03d1 -->|Yes — still running| B03poll
    B03d1 -->|No — all done| B34

    %% ── Result extraction ──────────────────────────────────────────────────
    B34["B34 — Unzip results <br> Move *_out.zip → Z00_VECTO_OUTPUTS/&lt;version&gt;/ <br> Extract → XX_RESULTS/&lt;version&gt;/"]
    B34 --> B34d1

    B34d1{"ref_vecto_root/MR <br> exists?"}
    B34d1 -->|Yes| B34mr["B34 — Unzip MR results <br> XX_RESULTS/&lt;mr_version&gt;/"]
    B34d1 -->|No| C01
    B34mr --> C01

    %% ── Report generation ──────────────────────────────────────────────────
    C01["C01 — Generate reports <br> Read XX_RESULTS/&lt;version&gt;/ <br> Build Success / VSUM / CRF sheets"]
    C01 --> C01d1

    C01d1{"--is-mr-run?"}
    C01d1 -->|Yes| C01mr["refresh_mr_report() <br> Delete old MR OR report <br> Write new MR OR report <br> from XX_RESULTS/&lt;mr_version&gt;/"]
    C01d1 -->|No| C01write

    C01mr --> C01write["write_report() <br> OR_&lt;version&gt;_&lt;date&gt;_&lt;nnnn&gt;_&lt;hash&gt;.xlsx"]
    C01write --> C01d2

    C01d2{"Latest release <br> OR report found?"}
    C01d2 -->|Yes| C01ref["C44 — generate_delta_report() <br> C44 — append_delta_to_report() <br> Adds Delta Summary + Δ sheets"]
    C01d2 -->|No — skip| C01d3

    C01ref --> C01d3

    C01d3{"MR OR report <br> found in MR cache?"}
    C01d3 -->|Yes| C01mrd["C44 — generate_delta_report() <br> C44 — append_delta_to_report(_MR) <br> Adds Delta Summary_MR + Δ_MR sheets"]
    C01d3 -->|No — skip| C01fail

    C01mrd --> C01fail

    C01fail["purge_old_failed_reports() <br> (remove entries > 10 days)"]
    C01fail --> C01d4{"Any failed <br> vehicles?"}
    C01d4 -->|Yes| C01copy["copy_failed_outputs() <br> Copy to FailedReports/&lt;run_label&gt;/"]
    C01d4 -->|No| C04

    C01copy --> C04

    %% ── HTML report ────────────────────────────────────────────────────────
    C04["C04 — Generate HTML report <br> Read most recent OR*.xlsx <br> Render pass/fail table"]
    C04 --> C04d1{"Delta Status <br> column present?"}
    C04d1 -->|Yes| C04delta["Add Delta column <br> + Warnings card"]
    C04d1 -->|No| C04d2

    C04delta --> C04d2

    C04d2{"Delta Status_MR <br> column present?"}
    C04d2 -->|Yes| C04mr["Add Trans Delta column <br> + Trans Δ Warnings card"]
    C04d2 -->|No| C04out

    C04mr --> C04out["Write HTML_REPORT_&lt;date&gt;_&lt;hash&gt;.html"]
    C04out --> END([Pipeline complete])

    %% ── Worker node (HTCondor) ─────────────────────────────────────────────
    B33["B33 — Run single VECTO job <br> (HTCondor worker node) <br> Unzip vecto_root + input.zip <br> Run vectocmd <br> Archive *_out.zip"]
    B03submit -.->|"dispatched per job"| B33
    B33 -.->|"output zip returned"| B34

    %% ── Styling ────────────────────────────────────────────────────────────
    classDef script fill:#dbeafe,stroke:#2563eb,color:#1e3a5f
    classDef decision fill:#fef9c3,stroke:#ca8a04,color:#713f12
    classDef io fill:#dcfce7,stroke:#16a34a,color:#14532d
    classDef endpoint fill:#f1f5f9,stroke:#64748b,color:#1e293b

    class A01,A02,B03,B32,B34,B33,C01,C04 script
    class A02d1,A02d2,B03lim,B03d1,B32d1,B32d3,B32d5,B32d7,B32d8,B34d1,C01d1,C01d2,C01d3,C01d4,C04d1,C04d2 decision
    class A02g,A02d3,B32write,B34mr,C01mr,C01write,C01ref,C01mrd,C01fail,C01copy,C04delta,C04mr,C04out io
    class START,END endpoint
```
