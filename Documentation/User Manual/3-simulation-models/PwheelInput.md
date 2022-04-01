## P~wheel~-Input (SiCo Mode)

For verification tasks it is possible to manually input the power at wheels (P~wheel~) which is normally calculated via longitudinal dynamics. In this case VECTO only calculates the losses between wheels and engine and adds auxiliary power demand. This mode is active as soon as P~wheel~, Gear and Engine Speed are defined in the driving cycle.

### Requirements

- Driving Cycle must include t, P~wheel~ (Pwheel), Gear (Gear) and Engine Speed (n), see [Driving Cycle (.vdri) format](#driving-cycles-.vdri).
- The driving cycle must be time-based.


**Example driving cycle with P~wheel~ input.**

| \<t> | \<Pwheel> | \<gear> | \<n>
|-------|------------|----------|--------
|1      |0.0         |0         |560.0
|2      |0.0         |0         |560.0
|3      |14.0        |1         |593.2
|4      |51.9        |1         |705.5
|...    |...         |...       |...
