## Driving Cycles (.vdri)

A Driving Cycle defines the parameters of a simulated route in VECTO. It is either time-based or distance-based and has different fields depending on the driving cycle type.
The basic file format is [VECTO-CSV](#csv) and the file type ending is ".vdri". A Job must have at least one driving cycle (except in Declaration mode, where the driving cycles are predefined).

### Driving Cycle Types
- **Declaration Mode**: [Target speed, distance-based](#declaration-mode-cycles)
- **Verification Test Mode**: [Measured driving cycle, time-based](#verification-test-cycle)
- **Engineering Mode**:
	- [Target speed, distance-based](#engineering-mode-target-speed-distance-based-cycle)
	- [Measured speed, time-based](#engineering-mode-measured-speed-time-based-cycle)
	- [Measured speed with gear, time-based](#engineering-mode-measured-speed-with-gear-time-based-cycle)
	- [Pwheel (SiCo) Mode, time-based](#engineering-mode-pwheel-sico-time-based)
- **Engine Only Mode**: [Engine Only Mode, time-based](#engine-only-mode-engine-only-driving-cycle)


- Distance-based cycles can be defined in any distance resolution, including variable distance steps.
- Time-based cycles can be defined in any time resolution, including variable time steps.

### Declaration Mode Cycles
In Declaration Mode driving cycles are automatically chosen depending on vehicle category and cannot be changed by the user. These predefined cycles are of type target-speed, distance-based.

- Construction: 100km
- Long Haul: 100km
- Municipal Utility: 11.25km
- Regional Delivery: 100km
- Urban Delivery: 100km

### Verification Test Cycle
This kind of cycle is used for simulating vehicles defined in declaration mode (xml) on a real driving cycle.

Header: **t, v, n\_eng,n\_fan, tq\_left, tq\_right, n\_wh\_left, n\_wh\_right***, fc_<Fuel Type>, gear*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

| Identifier      | Unit     | Description                                                                                                                                                                                                                                                                                            |
| -------------   | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **t**           | [s]      | The absolute time. Must always be increasing.                                                                                                                                                                                                                                                          |
| **v**           | [km/h]   | The actual velocity of the vehicle. Must be >= 0 km/h.                                                                                                                                                                                                                                                 |
| **n_eng**       | [rpm]    | The actual engine speed. Must be >= 0 rpm.                                                                                                                                                                                                                                                             |
| **n_fan**       | [rpm]    | The actual engine-fan speed. Must be >= 0 rpm.                                                                                                                                                                                                                                                         |
| **tq_left**     | [Nm]     | The actual torque at the driven wheel (left side)                                                                                                                                                                                                                                                      |
| **tq_right**    | [Nm]     | The actual torque at the driven wheel (left side)                                                                                                                                                                                                                                                      |
| **n_wh_left**   | [rpm]    | The actual wheel speed of the driven wheel (left side). Must be >= 0 rpm.                                                                                                                                                                                                                              |
| **n_wh_right**  | [rpm]    | The actual wheel speed of the driven wheel (right side). Must be >= 0 rpm.                                                                                                                                                                                                                             |
| *fc_<Fuel Type>*| [g/h]    | Fuel consumption, this column has to be provided for every fuel in case of dual-fuel vehicles                                                                                                                                                                                                          |
| *gear*          | [-]      | The actual gear                                                                                                                                                                                                                                                                                        |

**Example:**

~~~
t [s]              , v [km/h]    , n_eng [rpm] , n_fan [rpm] , tq_left [Nm] , tq_right [Nm] , n_wh_left [rpm] , n_wh_right [rpm] , fc_Diesel CI [g/h] , gear
0                  , 0           , 599.7       , 727.3       , 319.1        , 429.8         , 0.78            , 0.78             , 836                , 3
0.5                , 0           , 600.2       , 727.3       , 316.7        , 430.0         , 0.78            , 0.78             , 836                , 3
1                  , 0           , 600.1       , 726.9       , 319.9        , 430.8         , 0.78            , 0.78             , 836                , 3
1.5                , 0           , 599.9       , 726.6       , 317.4        , 431.1         , 0.78            , 0.79             , 836                , 3
2                  , 0           , 600.1       , 726.2       , 319.5        , 421.7         , 0.78            , 0.78             , 836                , 3
2.5                , 0           , 599.7       , 726         , 319.0        , 434.1         , 0.78            , 0.78             , 836                , 3
3                  , 0           , 600.2       , 725.4       , 322.2        , 428.5         , 0.78            , 0.78             , 836                , 3
3.5                , 0           , 599.9       , 724.7       , 317.3        , 430.4         , 0.78            , 0.78             , 836                , 3
4                  , 0           , 599.5       , 724.0       , 320.9        , 428.0         , 0.78            , 0.78             , 836                , 3
4.5                , 0           , 599.9       , 723.4       , 187.0        , 247.6         , 0.78            , 0.78             , 836                , 3
5                  , 0           , 598.7       , 722.5       , 156.9        , 171.5         , 0.78            , 0.78             , 1003.2             , 3
~~~


### Engineering Mode: Target-Speed, Distance-Based Cycle
This driving cycle defines the target speed over distance. VECTO tries to achieve and maintain this target speed.

Header: **s, v, stop***\[, Padd]\[, grad]\[, PTO]\[, vair\_res, vair\_beta]*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

**Note:** if the cycle starts with a target speed of 0 km/h and the stop-time for the first entry is 0, VECTO sets the stop-time to 1s automatically. 


|  Identifier |  Unit  |                                                                                                                                             Description                                                                                                                                              |
|-------------|--------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **s**       | [m]    | Traveled distance. Must always be increasing.                                                                                                                                                                                                                                                        |
| **v**       | [km/h] | The target vehicle velocity.  Must be >= 0 km/h.                                                                                                                                                                                                                                                     |
| **stop**    | [s]    | Stopping Time. Defines the time span the vehicle is standing still (time the vehicle spending in a stop phase). After this time, the vehicle tries to accelerate to v. If during a stop phase the PTO cycle is activated, it is recommended to use at least 2 seconds of stop time (which gets split up: first half before the PTO cycle, second half after the PTO cycle). |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                          |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                                   |
| *HW*        | [0/1]  | Marks highway sections (1) of the driving cycle. Predictive cruise control is only enabled on highway parts of the cycle                                                                                                                                                                             |
| *PTO*       | [0/1/2/3]  | "0"=disabled, "1"=PTO active during standstill, "2"=PTO active during driving with PTO power from driving cycle, "3"=PTO active during driving, separate time-based PTO cycle. If at a vehicle stop (defined by target velocity=0) "1" is specified, the PTO cycle as specified in the *.vptoc–File is simulated. This is described in the [PTO Simulation Model](#pto)  The PTO activation is added to the simulation time in the middle of the stopping time as defined by the cycle parameter "stop". The PTO Cycle can be specified in the [**Vehicle Editor**](#vehicle-editor-pto-tab). When PTO is activated it is recommended to use at least 2 seconds as stop time. |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                               |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                              |
| *P_PTO*     | [kW]   | Auxiliary power applied for PTO activation mode 2 (PTO active during drive, PTO demand defined in cycle)

**Example:**

~~~
s [m]              , v [km/h]    , stop [s]    , grad [%]    , Padd [kW] |
0                  , 10          , 10          , 2.95        , 1.5
1                  , 20          , 0           , 2.97        , 1.3
2                  , 35          , 0           , 3.03        , 1.3
3                  , 50          , 0           , 2.99        , 1.3
~~~

### Engineering Mode: Measured-Speed, Time-Based Cycle
This driving cycle defines the actual measured speed over time. Vecto tries to simulate the vehicle model using this speed as the actual vehicle speed.
Due to differences in the real and simulated shift strategies a small difference in speed can occur, but Vecto immediately tries to catch up after the gear is engaged again.
This type of cycle is now supported for BEVs (E2, E3, E4, IEPC) also, and it was tested with the following input fields: **t**, **v**, *grad*.

Header: **t, v***\[, grad]\[, Padd]\[, vair\_res, vair\_beta\]*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|  Identifier |  Unit  |                                                                                                                                              Description                                                                                                                                               |
| ----------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **t**       | [s]    | The absolute time. Must always be increasing.                                                                                                                                                                                                                                                          |
| **v**       | [km/h] | The actual velocity of the vehicle. Must be >= 0 km/h.                                                                                                                                                                                                                                                 |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                            |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                                     |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                 |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                                |

**Example:**

~~~
t [s]     v [km/h] , grad [%]    , Padd [kW]
0            0     , 2.95        , 1.5
1          0.6     , 2.97        , 1.3
2          1.2     , 3.03        , 1.3
3          2.4     , 2.99        , 1.3
~~~

### Engineering Mode: Measured-Speed With Gear, Time-Based Cycle

This driving cycle defines the actual measured speed of the vehicle, the gear, and the engine speed over time.
It overrides the shift strategy of VECTO and also directly sets the engine speed.
This type of cycle is now supported for BEVs (E2, IEPC) also, and it was tested with the following input fields: **t**, **v**, **gear**, *grad*.

Header: **t, v, gear***\[, tc\_active, grad]\[, Padd]\[, vair\_res, vair\_beta]\[, Aux\_ID\]*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|  Identifier |  Unit  |                                                                                                                                              Description                                                                                                                                               |
| ----------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **t**       | [s]    | The absolute time. Must always be increasing.                                                                                                                                                                                                                                                          |
| **v**       | [km/h] | The actual velocity of the vehicle. Must be >= 0 km/h.                                                                                                                                                                                                                                                 |
| **gear**    | [-]    | The current gear. Must be >= 0 (0 is neutral).                                                                                                                                                                                                                                                         |
| **tc_active**| [-]    | For AT gearboxes mandatory! Indicate if the torque converter is active or locked. Depending on the gearbox type only allowed for 1st gear or 1st and 2nd gear.                                                                                                                                                                                                                                                       |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                            |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                                     |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                         |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#vehicle-cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                                        |

**Example:**

~~~
t [s]              , v [km/h]    , gear [-]    , grad [%]    , Padd [kW]
0                  , 0           , 0           , 2.95        , 1.5
1                  , 0.6         , 3           , 2.97        , 1.3
2                  , 1.2         , 3           , 3.03        , 1.3
3                  , 2.4         , 3           , 2.99        , 1.3
~~~

### Engineering Mode: Pwheel (SiCo), Time-Based
This driving cycle defines the power measured at the wheels over time. VECTO tries to simulate the vehicle with this power requirement.
This type of cycle is now supported for BEVs (E2, E3, E4, IEPC) also, and it was tested with the following input fields: **t**, **Pwheel**, **gear**, **n**.

Header: **t, Pwheel, gear, n***\[, Padd]*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

| Identifier |  Unit |                      Quantity                                                    Description                      |
| ---------- | ----- | ----------------------------------------------------------------------------------------------------------------- |
| **t**      | [s]   | The absolute time. Must always be increasing.                                                                     |
| **Pwheel** | [kW]  | Power at the wheels.                                                                                              |
| **gear**   | [-]   | The current gear. Must be >= 0 (0 is neutral).                                                                    |
| **n**      | [rpm] | The actual engine speed for conventionals or the electric motor speed for BEVs. Must be >= 0 rpm.                                                                        |
| *Padd*     | [kW]  | Additional auxiliary power demand. This power demand will be directly added to the engine power. Must be >= 0 kW. |

**Example:**

~~~
t [s]              , Pwheel [kW] , gear [-]    , n [rpm]     , Padd [kW]
0                  , 0           , 0           , 600         , 1.5
1                  , 4.003       , 3           , 950         , 1.3
2                  , 15.333      , 3           , 1200        , 1.3
3                  , 50.56       , 3           , 1400        , 1.3
~~~

### Engine Only Mode: Engine Only Driving Cycle

This driving cycle directly defines the engine's power or torque at the output shaft over time. VECTO adds the engine's inertia to the given power demand and simulates the engine.

Header: **t, n, (Pe|Me)***\[, Padd]*

**Bold columns** are mandatory. *Italic columns* are optional. Only the listed columns are allowed (no other columns!).<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

| Identifier |  Unit |                                                    Description                                                    |
| ---------- | ----- | ----------------------------------------------------------------------------------------------------------------- |
| **t**      | [s]   | The absolute time. Must always be increasing.                                                                     |
| **n**      | [rpm] | The actual engine speed. Must be >= 0 rpm.                                                                        |
| **Pe**     | [kW]  | The power at the output shaft of the engine. Either Pe or Me must be defined.                               |
| **Me**     | [Nm]  | The torque at the output shaft of the engine. Either Pe or Me must be defined.                              |
| *Padd*     | [kW]  | Additional auxiliary power demand. This power demand will be directly added to the engine power. Must be >= 0 kW. |

**Example:**

~~~
t [s]              , n [rpm]     , Pe [kW]     , Padd [kW]
0                  , 600         , 0           , 1.5
1                  , 950         , 25.3        , 1.3
2                  , 1200        , 65.344      , 1.3
3                  , 1400        , 110.1       , 1.3
~~~

