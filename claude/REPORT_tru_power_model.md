# Is a physics-based fridge-truck (TRU) power model worth adding to Bob's HDEV engine?

**Research only — no changes to Bob's code.** All values are cited or labelled
as my own calculation; assumptions are flagged. The proposed model being
assessed, per weather hour:

```
heat_leak [W] = K [W/m²K] × S [m²] × (T_outside − T_set)
electric [W]  = heat_leak / COP      (steady state: no doors, no warm cargo)
```

No relevant source documents exist in the workspace (only VECTO release
checklists), so everything below is external and cited inline.

---

## 1. Verify against the UNECE ATP agreement

**1a. Definition of K and S.** ATP **Annex 1, Appendix 2** defines the overall
heat-transfer coefficient as **K = W / (S × ΔT)** — power input W [W], surface
S [m²], temperature difference ΔT [K] (ECE/TRANS/WP.11/2021/21: "with power
input W, mean surface area S and temperature difference ΔT"). So the proposed
`heat_leak = K·S·ΔT` is exactly the ATP relation rearranged. ✅

The S term is the **root mean surface area** defined in Appendix 2
(paragraphs ~29–31, headed "Mean internal surface area Si", "Mean external
surface area Se", "Root mean surface area S"; see the ATP consolidated text and
UNECE wp119905ad1). It is the **geometric** mean **S = √(Si × Se)**, not the
arithmetic mean. Si and Se are computed from the box dimensions
(ECE-TRANS-WP11-inf9e gives the Si/Se box formulas). *Content rephrased for
compliance.*

→ **Flag for Bob:** the proposal wrote S as "mean of inner and outer body
surface". ATP uses the **geometric** mean. My own calculation on a standard
reefer trailer (below) shows the two differ by **only 0.04%**, so it doesn't
matter numerically — but the model should state it uses the ATP √(Si·Se) so a
client's ATP certificate value drops straight in.

**1b. K limits (Annex 1, paragraph 1).** Confirmed verbatim in the ATP text:
- **IN = Normally insulated:** K **≤ 0.70 W/m²·K**
- **IR = Heavily insulated:** K **≤ 0.40 W/m²·K**

(ECE-TRANS-WP11-2005-02e: "0.70 W/m².K; IR = Heavily insulated equipment …
a K coefficient equal to or less than 0.40 W/m².K"; and
ECE-TRANS-WP11-2019-18e: "Annex 1, paragraph 1 specifies the limits for heat
transfer coefficient (K) of Normally Insulated (IN) and Heavily Insulated (IR)
equipment".) ✅ Matches the task's values.

**1c. The 1.75 capacity-factor rule.** Confirmed: the effective refrigerating
capacity of the unit must exceed the heat loss through the walls for the class
in question, **multiplied by the factor 1.75** (ECE/TRANS/WP.11/2021/21 and
ECE-TRANS-WP11-2014-02e: "…dividing and outer body walls of the equipment as a
whole multiplied by the factor 1.75"). It lives in **Annex 1, Appendix 2**.
*Rephrased for compliance.*

→ **What 1.75 is and is NOT:** it is a **sizing/type-approval margin** — the unit
must be oversized to 1.75× the steady leak so it can pull down temperature and
cope with doors/loading. It is **not** a running-power multiplier. Using it to
inflate steady-state consumption would be wrong; the proposed model correctly
leaves it out of the energy term.

---

## 2. COP / unit-efficiency data: what a client can actually get

**OEM spec sheets give cooling CAPACITY at a couple of temperatures — rarely
electrical power input, and essentially never COP across temperature.**

- **Thermo King E-1200 (electric, Whisper Pro):** cooling capacity **8,500–9,600 W
  at 0 °C box / 30 °C ambient**, **5,100–5,300 W at −20 °C / 30 °C**, heating
  3,600–6,000 W (TK E-1200 Tech-Specs sheet, EN/IT/ES). Capacity quoted at **two
  box temps, one ambient (30 °C)**. No electrical input power or COP in the public
  sheet.
- **Thermo King UT-1200 E-EVAP:** 11,400 W at 0 °C (thermoking.com range page).
- **Carrier Transicold Supra eCool / Vector eCool (electric):** capacity quoted
  "under A.T.P. conditions at 30 °C ambient"; Supra eCool carries a **38-kWh**
  DC power pack (carrier.com). Battery size is given; COP-vs-temperature is not.

**COP itself:** measured transport-refrigeration COP is **low and strongly
temperature-dependent** — about **1.1–1.4** for a commercial transport system
across operating conditions (Bagheri/Fayazbakhsh/Bahrami, SEMI-THERM 2016, SFU),
~**1.7** at a standard 35/27 °C rating point for a comparable transport AC system
(MDPI Appl. Sci. 11, 5614). COP "is highly dependent on operating conditions,
especially … relative temperature between sink and system" (Wikipedia, COP).

**What a client realistically has:**
1. ATP certificate → **K** (and class IN/IR) and the body geometry → **S**, ΔT. ✅ easy.
2. OEM sheet → **capacity** at **one ambient (usually 30 °C)** and 1–2 box temps. ✅
3. **Electrical input power** → sometimes (amps/kW on some sheets); **often missing**.
4. **COP, and especially COP vs outside temperature** → **almost never published**. ✗

→ So the model's `/COP` term is the weak link: the one input the physics most
needs is the one a client is least likely to have, and it is not a constant.

---

## 3. Validation — formula vs measured energy (my own calculation)

**Body used (cited):** standard reefer **semi-trailer internal 13.400 × 2.460 ×
2.580 m** (TSSC Refrigerated Semi-Trailer spec sheet). From these I computed
(my own calculation): Si ≈ 147.8 m², Se ≈ 156.7 m² (**wall thickness 0.06 m
assumed each side — FLAGGED, my estimate**), giving ATP **S = √(Si·Se) ≈
152.2 m²**. (Arithmetic mean also 152.2 m² → 0.04% difference, as noted in 1a.)

**Formula output** (K·S·ΔT / COP), steady state:

| body / condition | ΔT | heat-leak W | P_elec (COP 1.2) | P_elec (COP 1.7) | kWh/day (24 h) |
|---|---|---|---|---|---|
| IR frozen −20 °C @ 30 °C | 50 K | 3,043 | 2,536 W | 1,790 W | **43–61** |
| IR chilled 2 °C @ 30 °C | 28 K | 1,704 | 1,420 W | 1,003 W | 24–34 |
| IN chilled 2 °C @ 30 °C | 28 K | 2,983 | 2,485 W | 1,754 W | 42–60 |
| IR chilled 2 °C @ 15 °C | 13 K | 791 | 659 W | 465 W | 11–16 |

**Independent measured data to compare against:**
- **EPRI Transport Refrigeration Units:** average annual energy **15,000 kWh/yr**
  (EPRI 3002005925) → **≈ 41 kWh/day** average, **≈ 1,712 W** average.
- **SFU/Bahrami refrigerated-trailer field evaluation:** peak real-time cooling
  power **17.8 kW** and power consumption **18.4 kW** (max, not mean).
- **Maritime reefer containers:** ~**19%** of journey energy is refrigeration
  (ResearchGate 227415548) — different duty, context only.

**How far off is the formula?** The frozen-IR steady number (**43–61 kWh/day**)
straddles the EPRI **~41 kWh/day** figure — same order of magnitude, encouraging.
**But this is not a tight validation:** EPRI's 41 kWh/day is an **annual average**
over varying ambient, with door openings, pulldown and defrost **included**,
whereas the formula number is a single worst-case 30 °C steady leak with doors
**excluded**. The agreement is "right ballpark", and the match is sensitive to
the COP guess (1.2 vs 1.7 moves the result ±40%) and to the assumed wall
thickness. **No source in this search gave K, S, set-point AND measured energy
for the same body**, so a clean point-for-point error figure can't be stated.
Honest read: the formula is plausibly within a factor of ~1.5–2 of reality,
dominated by the COP and door assumptions — not by the K·S·ΔT physics, which is
sound.

---

## 4. Door openings — small enough to ignore?

**No — leaving doors out can under-count badly for urban multi-drop.** Measured:

- Warm-air infiltration during door openings can be **≈ 34% of the overall
  refrigeration load** of a refrigerated delivery truck, and **higher** for
  longer/more frequent openings (Brunel University PhD thesis, bura.brunel.ac.uk
  2438/18126). *Rephrased for compliance.*
- Dedicated studies exist on the infiltration heat load per opening (Int. J.
  Refrigeration 2015, doi:10.1016/j.ijrefrig.2015.02.009; USDA TB-1375
  "Refrigeration Requirements for Truck Bodies: Effects of Door Usage").

→ So the steady-state-only model is a **reasonable approximation for long-haul /
few-stops** duty, but for **urban multi-drop it can under-predict TRU energy by
roughly a third**. That is too large to ignore silently. If doors are omitted,
the model must **say so** and flag multi-drop results as a lower bound.

---

## 5. PTO-while-driving public measured profiles?

- **(a) Refuse-truck compaction while driving — YES.** Ban & Stipetić, *Electric
  Multipurpose Vehicle Power Take-Off: Overview, Load Cycles and Actuation…*,
  IEEE ACEMP-OPTIM 2019 (doi:10.1109/ACEMP-OPTIM44294.2019.9007187) gives ePTO
  **load cycles** for refuse/eMPV bodies. Supporting: refuse hydraulic-system
  energy-loss analysis (Energies 2021, 14, 2707) and NREL refuse driving/duty
  cycles (nrel.gov 59136).
- **(b) Electric street-sweeper brush/suction power — YES.** Caltrans **"Field
  Study of Battery-Electric Sweepers"** (task4137) and **"Evaluation of Hybrid
  Electric Street Sweepers"** (task3130) provide measured energy; an industry
  breakdown puts the **sweeping system (brushes, vacuum, water) at ~30–40% of a
  full-shift's energy** (rigidchill.com). *Rephrased for compliance.*

Both exist as sourced/measured profiles — though note these are separate PTO
work-while-driving loads, **not** part of the fridge model; they'd be a distinct
feature.

---

## Verdict

**Does the physics TRU model beat a single constant-W number, given realistic
client inputs?**

- **The physics core (K·S·ΔT) is correct, standards-backed, and cheap.** It is
  exactly the ATP definition (Q1), and its one genuinely free input that a client
  can't easily supply — **K** — is pinned by the ATP class to **0.40 (IR) / 0.70
  (IN) W/m²·K**, with S and ΔT coming from the certificate and the weather feed.
  Because it reacts to outside temperature every hour, it **does beat a single
  constant W** for any duty where ambient swings a lot (a constant number can't
  know that −20 °C frozen at 30 °C ambient draws ~4× the 15 °C-ambient chilled
  case — see §3).
- **But the two things that most move the answer are the two weakest inputs:**
  **COP** (not published vs temperature; a 1.2↔1.7 guess is ±40%) and **door
  openings** (up to ~34% of load, and omitted). So the model trades a crude
  constant for a physically-honest skeleton that still rests on two soft
  assumptions.

**Recommendation: implement as an OPTION, with a tiered input contract.**

- **Mandatory inputs:** K (or ATP class IN/IR → default K), body S (or box
  dimensions → compute ATP √(Si·Se)), set-point T_set, and the existing hourly
  T_outside. These a client reliably has from the ATP certificate.
- **COP:** require it, but allow a **single cited default** (e.g. ~1.5, mid of the
  measured 1.1–1.7 range) with a clear flag that it's an assumption; ideally
  accept 2–3 (ambient, COP) points from the OEM sheet and interpolate.
- **Doors:** ship an **optional multi-drop uplift** (default off; a documented
  ~1.3× for urban multi-drop per §4) and label steady-state-only runs as a lower
  bound.
- **Keep the constant-W path** as the fallback for clients who have nothing but a
  nameplate.

In short: implement the physics as an **optional, better-than-constant** model —
**don't make it the sole/default** path, because its accuracy is gated by COP and
door data the client usually lacks. The K·S·ΔT part is worth having; the honesty
about COP and doors is what makes it trustworthy.
