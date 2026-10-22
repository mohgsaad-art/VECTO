# Terrain-aware wind-height option for Bob's crosswind engine — test report

**Scope:** test only. Bob's engine functions were not modified. The test file
`claude/hdev_eurocode_wind_test.py` copies Bob's functions verbatim from
`HDEV_NEW.ipynb` and calls them as-is; the only new code is the EN 1991-1-4
wind profile and the test harness. A self-check confirms the harness's
profile path reproduces Bob's `crosswind_engine` exactly (0.2 power law).

The harness mentioned in the task (`claude/hdev_test_harness_v1.7.py`) and the
notebook file are **not present in this workspace**, so Bob's code was taken
from the notebook text in the request. Everything else (VECTO source, cycles,
coefficients) comes from the `VECTO` repo in the workspace.

---

## 0. First: verify the formulas against the real EN 1991-1-4 text

| Item in the task | EN 1991-1-4 says | Match? |
|---|---|---|
| kr = 0.19·(z0/0.05)^0.07 | Eq. 4.5: kr = 0.19·(z0/z0,II)^0.07, z0,II = 0.05 m | ✅ exact |
| cr(z) = kr·ln(z/z0) | Eq. 4.4: cr(z) = kr·ln(z/z0), valid zmin ≤ z ≤ zmax(=200 m) | ✅ exact |
| Table 4.1 (0:0.003/1, I:0.01/1, II:0.05/2, III:0.3/5, IV:1.0/10) | Table 4.1 identical | ✅ exact |
| basic wind at 10 m, cat II | vb is the 10-min mean at **10 m in open country (cat II)** | ✅ confirmed |

Verified against the EN 1991-1-4 text reproduced at eurocodes-tools.com
(roughness factor cr(z)) and the EN_029 excerpt at sigmundcarlo.net. *Content
rephrased for compliance with licensing restrictions.*

**One difference to flag in the proposed rule.** The task wrote the clamp as
`z = max(h_slice, zmin_route)` — i.e. only the route (numerator) height is
clamped to zmin. The standard applies the `zmin ≤ z` limit to **both** heights.
Since the 10 m station height is always above every zmin, this only matters for
the slice heights, so in practice it is the same here — but the harness applies
zmin to both to stay faithful to the standard. **Flagged assumption.**

**A bigger mismatch to flag — what VECTO actually does.** VECTO's real
crosswind model (`Physics.cs`) does **not** use a 10 m wind at all. It uses a
**fixed 3 m/s wind at a 4 m reference height** (`BaseWindSpeed = 3 m/s`,
`BaseWindHeight = 4 m`, `HellmannExponent = 0.2`) and builds a single
CdxA-vs-speed curve (`AirdragDataAdapter.cs`). Bob's `crosswind_engine` already
departs from VECTO by taking a **variable** wind referenced to an arbitrary
`h_ref`. So this study compares two profile *shapes* (0.2 power law vs EN log
law) both referenced to the 10 m DMI station — it is a comparison inside Bob's
HDEV extension, not a change to certified VECTO behaviour. **Flagged.**

All CdxA inputs are cited VECTO values: CdxA0 = 5.8 m² (task); a1/a2/a3 =
0.030042 / 0.040817 / −0.00213 = the **TractorSemitrailer** row of
`VECTO/.../VCDV/VCDV_parameters.csv`; grid 60–130 km/h step 5, alpha 0–180 step
10, 10 % height steps = `Configuration/Constants.cs → CrosswindCorrection`.

---

## 1. Profile shape (slice wind ÷ 10 m station wind)

Wind-speed-independent (it's a ratio). 4.0 m vehicle, slice midpoints.

| height | Hellmann 0.2 | cat 0 | cat I | cat II | cat III | cat IV |
|---|---|---|---|---|---|---|
| 0.1 m | 0.398 | 0.900 | 0.777 | 0.696 | 0.602 | 0.536 |
| 1.0 m | 0.631 | 0.900 | 0.777 | 0.696 | 0.602 | 0.536 |
| 2.0 m | 0.725 | 1.008 | 0.893 | 0.696 | 0.602 | 0.536 |
| 3.9 m | 0.828 | 1.111 | 1.006 | 0.822 | 0.602 | 0.536 |
| **slice mean** | **0.695** | **0.999** | **0.884** | **0.733** | **0.602** | **0.536** |

Read-out: the EN log law is **flat below zmin** (it freezes the ratio under
1–10 m, exactly the band a truck lives in), whereas the power law keeps falling
toward the ground. The closest EN match to the current 0.2 rule is **cat II**
(slice-mean 0.733 vs 0.695). Rougher categories (III, IV) give noticeably less
wind at vehicle height.

## 2. CdxA_eff tables (vs current rule, whole 0–15 m/s × 60–130 km/h grid)

| category | max % diff | mean % diff |
|---|---|---|
| cat 0 | +30.5 % | +9.0 % |
| cat I | +17.6 % | +5.5 % |
| **cat II** | **+2.6 %** | **+1.1 %** |
| cat III | −7.9 % | −2.6 % |
| cat IV | −12.5 % | −4.6 % |

Sample @85 km/h, 6 m/s wind: current 7.302 m² → II 7.418, III 7.070, IV 6.875.
So the choice of terrain category moves the effective drag area by roughly
**±5 % mean / ±10 % worst-case** — cat II barely changes it, rougher terrain
lowers it.

## 3. Energy — full HDEV pipeline, 1 year hourly Copenhagen wind

Wind data: **8760 hourly 10 m values, Copenhagen (55.68 N, 12.57 E), 2023**,
mean 5.30 m/s, max 16.3 m/s. Primary intent was DMI metObs `wind_speed` (10 m,
10-min mean); the harness pulls the **Open-Meteo ERA5 archive** 10 m wind for
Copenhagen, which is the same 10 m reference — used here because it needs no API
key. (Flagged: swap in the DMI metObs station series for production.)
Ambient fixed at 15 °C to isolate the wind effect (flagged). Vehicle mass
18 t + 19.3 t payload (flagged, order-of-magnitude generic tractor-semitrailer).

Effect **on total trip energy (kWh/km)**, EN vs current rule:

| cycle | cat II | cat III | cat IV |
|---|---|---|---|
| Urban Delivery | +0.20 % | −0.46 % | −0.82 % |
| Regional Delivery | +0.57 % | −1.28 % | −2.28 % |
| Long Haul | +0.67 % | −1.51 % | −2.68 % |

Effect **on air-drag energy alone**: cat II ≈ +1.1 %, cat III ≈ −2.6 %, cat IV
≈ −4.6 % across all three cycles.

Read-out: air drag is a big share of long-haul energy, so even a few-% change in
the drag table shows up as **up to ~2.7 % on total kWh/km** (Long Haul, cat IV).
Urban is barely affected (low speeds → little air drag). If the route is called
cat II, total energy moves **< 1 %**.

## 4. Station sensitivity (station = cat III instead of II)

Changing only the assumed **station** category (II→III) shifts every result up
by a near-constant offset:

| cycle | route II | route III | route IV |
|---|---|---|---|
| Urban | +1.42 % | +0.55 % | +0.11 % |
| Regional | +3.79 % | +1.52 % | +0.33 % |
| Long Haul | +4.41 % | +1.78 % | +0.39 % |

Read-out: **the station category you assume matters as much as the route
category.** A II→III station re-label alone adds up to +4.4 % on Long Haul
energy. The method just trades a height assumption for *two* terrain-category
assumptions (route **and** station), neither of which the DMI feed tells you.

## 5. Terrain transitions (Regional Delivery, urban ends / rural middle)

| treatment | air-drag kWh | vs (a) |
|---|---|---|
| (a) one type, whole route = cat III | 53.357 | — |
| (b) per-stretch IV / II / IV | 54.294 | +1.76 % |
| (c) EN Annex A.2 → whole route cat II | 55.400 | +3.83 % |

**EN 1991-1-4 Annex A.2 does provide a transition rule** (so the task's "if it
exists" is answered: it exists). Paraphrased: *within about 2 km of a smoother
category 0, or about 1 km of a smoother categories I–III, the smoother upwind
category is used.* *(Rephrased for licensing compliance.)* On a short mixed
route the smoother terrain tends to win, which here swings air-drag energy by
up to ~3.8 % depending on how you apply it. There is no single "right" per-route
number — it depends on upwind distance per wind direction, which the pipeline
doesn't have.

## 6. Validation — are we swapping one assumption for another?

**6a. Where does 0.2 come from?** VECTO hard-codes `HellmannExponent = 0.2` in
`Physics.cs` with the comment "for modelling of wind speed in specific heights";
the source files carry no citation for the number. In the wider literature the
power-law (Hellmann) exponent is terrain-dependent: ~0.14 for open flat ground,
~0.25–0.30 for suburban, ~0.40 for dense urban (weatherapi.com summary of
standard shear values; Wikipedia "Wind profile power law" gives 0.143 over open
land). So **0.2 is a single mid-open-terrain value** applied to all vehicles and
routes — itself an assumption, not a measurement.

**6b. Measurements at road/vehicle height (0–4 m) vs a 10 m mast.** No public
Danish Vejdirektoratet road-weather dataset giving paired 0–4 m vs 10 m wind was
found in this search. What *is* well established: WMO/meteorological practice
references wind at 10 m over open ground, and near-surface wind falls off
sharply below that; the exact 0–4 m behaviour depends on local roughness and
atmospheric stability (AMS JAMC-D-11-0247; "wind profile" literature). Notably,
VECTO's own air-drag certification (EU constant-speed test, `201811_air_drag`)
computes CdxA(β) from the **actually measured air velocity at the vehicle** and
corrects for crosswind — i.e. the correction exists precisely because
vehicle-level wind ≠ mast wind. **Neither the 0.2 power law nor the EN log law
was validated here against paired road-height measurements; I did not find a
dataset that lets you say which is closer.**

**6c. Is EN 1991-1-4 used for vehicle energy / road-level mean wind anywhere?**
EN 1991-1-4 is a **building/structure wind-load standard**. No use of it for
road-vehicle energy or for road-level mean wind was found. Its profile is meant
for structures tens of metres tall (it is only defined down to zmin = 1–10 m and
flattens below that), not for a 0–4 m vehicle band. Using it here is an
**off-label application** of the standard.

---

## Verdict

- **Size of effect on total trip energy:** small-to-moderate. With the route
  called cat II (the closest match to today's rule) it's **< 1 %**. Across
  plausible categories it spans about **−2.7 % … +0.7 %** on kWh/km (Long Haul
  most sensitive, Urban least). Changing the assumed *station* category alone
  moves it by **up to +4.4 %** — the dominant lever.

- **Does the Eurocode option remove an assumption with a validated standard, or
  just swap assumptions?** It **swaps assumptions.** It replaces one unsourced
  number (Hellmann 0.2) with a formula that is validated — but validated for
  *building wind loads above ~1–10 m*, not for vehicle-height mean wind, and it
  introduces **two new free choices** (route category and station category) that
  the DMI feed cannot pin down and that each move the answer by several percent.
  No paired road-height measurement was found that shows the log law is actually
  closer to reality at 0–4 m than the power law.

- **Recommendation: implement only as an option (off by default).** Keep Bob's
  current 0.2 rule as the default (it stays close to VECTO and to EN cat II).
  Expose the EN profile as an optional, clearly-labelled experimental setting
  with explicit route- and station-category inputs, so it can be used for
  sensitivity studies — not as a validated default. Do **not** promote it to the
  default until paired road-level-vs-10 m wind measurements show it is the better
  fit. Until then it is one assumption traded for two.
