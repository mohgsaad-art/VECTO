
# VECTO Changelog

## 2022-09-01: Vecto 3.3.12.2800 Official Release

- Bugfixes (compared to VECTO 3.3.12.2769)
- [VECTO-1635] - Reading / Writing the ADAS Parameter
  ATEcoRollReleaseLockupClutch does not work


## 2022-08-01: Vecto 3.3.12.2769 Release Candidate

- Improvement
  - [VECTO-1584] - Hashing library uses constructor that is deprecated in newer .net
    versions
  - [VECTO-1619] - Adaptations of XML Results in transition period
  - [VECTO-1619] - Reading Engine and Tyre component according to 2nd amendment
    (schema version 2.3)
  - [VECTO-1461] - Alignment of Steering Pump Technology and Steered Axles not
    checked by VECTO
  - [VECTO-1571] - Help file: Vehicle defined Torque Limitations
  - [VECTO-1585] - Validate component hashes in declaration mode (disabled by default)
  - [VECTO-1587] - update build script to include files required for xml parameter
    documentation
  - [VECTO-1598] - Execution time in latest official release
  - [VECTO-1608] - Adaptation of Reports (MRF/CIF) for transition in official VECTO
    Release
- Bugfixes:
  - [VECTO-1622] - Unexpected response: Response Underload in LH LowLoad
  - [VECTO-1623] - Unexpected response: Response Underload in LH EMS RefLoad


## 2022-04-29: Vecto 3.3.11.2675 Official Release

- Bugfixes (compared to VECTO 3.3.11.2645):
  - [VECTO-1557] - Simulation abort Generic Vehicle CityBus APT-S UD
  - [VECTO-1561] - Simulation abort with AT transmission
  - [VECTO-1567] - Different CO2 results with VECTO-3.3.11.2645 release candidate
  - [VECTO-1562] - Adapting WHTC Correction Factor weighting for LH Cycle
  - [VECTO-1560] - update toolchain for generating user manual

  - Added new VECTO parameter documentation


## 2022-03-30 Vecto 3.3.11.2645 Release Candidate

- Improvements:
- [VECTO-1474] - Implementation of ADAS in-the-loop simulation (Declaration and
  Engineering mode)
  - Engine stop/start
  - Eco-roll
  - Predictive cruise control: option 1,2; option 1,2,3
  - Post-processing: fuel consumption correction for ICE-off phases
  - Update shift strategy: EffShift for AMT and AT (Declaration and Engineering mode)
  - [VECTO-1547] Update LongHaul driving cycle (Declaration mode)
  - Dual-fuel support (Engineering mode)
  - WHR support (Engineering mode)
  - BusAuxiliaries model in engineering mode updated (simplified input)
  - [VECTO-1473], [VECTO-1521] - Additional tyre dimensions
  - [VECTO-1536] - Component XML 2nd Amendment (schema version 2.3)
  - [VECTO-1536] - verify generated xml by default when using command-line hashing
    tool
- Bugfixes:
  - [VECTO-1525] - PCC Preprocessing
  - [VECTO-1529] - XML Schema 2.0: missing abstract XML type in Gears
  - [VECTO-1550] - Error in hashing tool: sorting tires before comparing input data with
    MRF
  - [VECTO-1551] - AT-P Transmission Bus Application: Error during braking phase


## 2020-12-01: Vecto 3.3.11.2526 Development Version

- Improvements:
  - Implementation of ADAS in-the-loop simulation (Declaration and Engineering mode)
    - Engine stop/start
    - Eco-roll
    - Predictive cruise control: option 1,2; option 1,2,3
    - Post-processing: fuel consumption correction for ICE-off phases
  - Update shift strategy: EffShift for AMT and AT (Declaration and Engineering mode)
  - Update LongHaul driving cycle (Declaration mode)
  - Dual-fuel support (Engineering mode)
  - WHR support (Engineering mode)
  - BusAuxiliaries model in engineering mode updated (simplified input)

  - Support to read driving cycle and EffShift parameters from file system
    (only once when application is started!)

### EffShift Parameters

- The following EffShift parameters are read from the file
  “Declaration/EffShiftParameters.vtcu” if this file exists.
  (A sample file is provided in the VECTO archive)
- All parameters are optional, if a parameter is not provided the default value
  (see table below) is used

| Parameter name            | Value | Explanations |
|:---------------           |:-----:|:-------------|
| `Rating_current_gear`     | 0.97  | Defines the minimum fuel efficiency advantage in a candidate gear to trigger a gear shift (i.e. 3% of a value of 0.97) - applies only for AMT transmissions|
| `Rating_current_gear_APT` | 0.97  | Defines the minimum fuel efficiency advantage in a candidate gear to trigger a gear shift (i.e. 3% of a value of 0.97) - applies only for APT transmissions |
| `RatioEarly(Up/Down)shiftFC`   | 24 | In gears with a higher total drivetrain ratio (axle plus gearbox) than this parameter, “Efficiency shifts” are disabled, i.e. the shifts are only triggered by the shift lines. Rationale: Gear shift in this gears are primarily triggered by pow er demand.
| `AllowedGearRangeFC`      | 2     | Defines the gear range for candidate gears for “Efficiency Shifts”(+/-) - applies only fot APT transmissions |
| `AllowedGearRangeFCAPT`   | 1     | Defines the gear range for candidate gears for “Efficiency Shifts”(+/-) - applies only fot APT transmissions with ≤ 6 gears Allowed |
| `GearRangeFCAPTSkipGear`  | 2     | Defines the gear range for candidate gears for “Efficiency Shifts”(+/-) - applies only fot APT transmissions with more than 6 gears |
| `AccelerationFactorNP98h` | 0.5   | Defines the reduction of driver target acceleration in the engine speed range betw een nT98h and nP98h |
| `ATLookAheadTime`         | 0.8   | Defines look ahead time for rating of fuel consumption for AT transmissions. |


### Declaration Missions

  - The missions simulated in declaration mode folder “DeclarationMissions” if this folder
  exists.
  - A folder with all declaration missions is provided in the VECTO archive
    - LongHaul is the current candidate cycle #31
    - Previous LongHaul cycle is provided as LongHaul_old
    - Both LongHaul candidate cycles #13 and #31 are provided as well
  - Cycles are only read once on application start


## 2020-07-29: Vecto 3.3.10.2401 Official Release

- Improvements:
  - Handling of exempted vehicles (not changed since release candidate) - see next
    slides for details
- Bugfixes:
  - No additional bugfixes compared to VECTO 3.3.10.2401


## 2020-07-01: Vecto 3.3.10.2373 Release Candidate

- Improvements:
  - [VECTO-1421] - Added vehicle sub-group (CO2-standards to MRF and CIF)
  - [VECTO 1449] - Handling of exempted vehicles: See next slide for details
  - [VECTO-1404] - Corrected URL for CSS in MRF and CIF
- Bugfixes:
  - [VECTO-1419] - Simulation abort in urban cycle: failed to find operating point on
    search braking power with TC gear
  - [VECTO-1439] - Bugfix handling duplicate entries in engine full-load curve when
    intersecting with max-torque of gearbox
  - [VECTO-1429] - error in XML schema 2.x for exempted vehicles - MaxNetPower1/2
    are optional input parameters

### Handling of exempted vehicles

- Axle configuration and sleeper cab are optional input parameters for exempted
  vehicles (XML schema 1.0 and 2.2.1).
  - OEMs are recommended to provide these parameters for exempted vehicles.
  - If the axle configuration is provided as input parameter, the MRF contains the vehicle
    group.
  - The sleeper cab input parameter is also part of the MRF if provided as input.
- Input parameters MaxNetPower1/2 are optional input parameters for all exempted vehicles.
  - If provided in the input these parameters are part of the MRF for all exempted vehicle
    types
  - It is recommended that those parameters are used to specify the rated power also for
    PEV (pure electric vehicles)


## 2020-12-15: Vecto 3.3.9.2175 Official Release

- Bugfixes (compared to version 3.3.9.2147)
  - [VECTO-1374] - VECTO VTP error - regression update


## 2020-11-17: Vecto 3.3.9.2147 Release Candidate

- Bugfixes:
  - [VECTO-1331] - VTP Mode does not function for vehicles of group 3
  - [VECTO-1355] - VTP Simulation Abort
  - [VECTO-1356] - PTO Losses not considered in VTP simulation
  - [VECTO-1361] - Torque Converter in use for the First and Second Gear VTP file does
    not allow for this
  - [VECTO-1372] - Deviation of CdxA Input vs. Output for HDV16
  - [VECTO-1374] - VECTO VTP error
- Improvements:
  - [VECTO-1360] - make unit tests execute in parallel


## 2020-08-14: Vecto 3.3.8.2052 Official Release

- Bugfixes:
  - No additional bugfixes compared to VECTO 3.3.8.2024


## 2020-07-17: Vecto 3.3.8.2024 Release Candidate

- Bugfixes:
- [VECTO-1288] - Simulation Abort UD RL
- [VECTO-1327] - Simulation abort Construction RefLoad: unexpected response
  ResponseOverload
- [VECTO-1266] - Gear 4 Loss-Map was extrapolated


## 2020-05-18: Vecto 3.3.7.1964 Official Release

- Bugfixes:
  - [VECTO-1254] - Hashing method does not ignore certain XML attributes
  - [VECTO-1259] - Mission profile weighting factors for vehicles of group 16 are not
    correct


## 2020-03-31L Vecto 3.3.6.1916 Official Release

- Bugfixes:
  - [VECTO-1250] - Error creating new gearbox file from scratch


## 2020-03-13: Vecto 3.3.6.1898 Release Candidate

- Improvement
  - [VECTO-1239] - Adaptation of Mission Profile Weighting Factors
  - [VECTO-1241] - Engineering mode: Adding support for additional PTO activations
- Bugfixes:
  - [VECTO-1243] - Bug in VTP mode for heavy lorries
  - [VECTO-1234] - urban cycle at reference load not running for bug when find braking
    operating point


## 2019-12-18: Vecto 3.3.5.1812 Official Release

- Bugfixes (compared to VECTO 3.3.5.1783-RC)
  - [VECTO-1220] - Simulation Abort Urban Delivery RefLoad


## 2019-11-19: Vecto 3.3.5.1783 Release Candidate

- Improvement
  - [VECTO-1194] - Handling input parameter 'vocational' for groups other than 4, 5, 9,
    10
  - [VECTO-1147] - Updating declaration mode cycles values in user manual
  - [VECTO-1207] - run VECTO in 64bit mode by default
- Bugfixes:
  - [VECTO-1074] - Vecto Calculation Aborts with Interpolation Error
  - [VECTO-1159] - Simulation Abort in UrbanDelivery LowLoading
  - [VECTO-1189] - Error in delaunay triangulation invariant violated
  - [VECTO-1209] - Unexpected Response Response Overload
  - [VECTO-1211] - Simulation Abort Urban Delivery Ref Load
  - [VECTO-1214] - Validation of input data fails when gearbox speed limits are applied


## 2019-09-13: Vecto 3.3.4.1716 Official Release

- Bugfixes (compared to VECTO 3.3.4.1686-RC)
  - [VECTO-1074] - Vecto Calculation Aborts with Interpolation Error ([VECTO-1046])
  - [VECTO-1111] - Simulation Abort in Municipal Reference Load


## 2019-08-14: Vecto 3.3.4.1686 Release Candidate

- Improvement
  - [VECTO-1042] - Add option to write results into a certain directory
  - [VECTO-1064] - add weighting factors for vehicle groups 1, 2, 3, 11, 12, 16
- Bugfixes:
  - [VECTO-1030] - Exceeded max iterations when searching for operating point! Failed
    to find operating point!
  - [VECTO-1032] - Gear 5 LossMap data was extrapolated in Declaration Mode: range
    for loss map is not sufficient
  - [VECTO-1067] - Vair and Beta correction for Aerodynamics
  - [VECTO-1000] - Error Loss-Map extrapolation in Declaration Mode
  - [VECTO-1040] - Gear 6 LossMap data was extrapolated in Declaration Mode
  - [VECTO-1047] - Failed to find operating point on construction cycle, ref load, AT
    gearbox



## 2019-06-27: Vecto 3.3.3.1639 Official Release

- Bugfixes (compared to VECTO 3.3.3.1609-RC)
  - [VECTO-1003] - Vecto Error: Loss-Map extrapolation in declaration mode required
    (issue VECTO-991)
  - [VECTO-1006] - Failed to find torque converter operating point on UD cycle
    (issue VECTO-996)
  - [VECTO-1010] - Unexpected Response: ResponseOverload in UD cycle
    (issue VECTO-996)
  - [VECTO-1015] - XML Schema not correctly identified
  - [VECTO-1019] - Error opening job in case a file is missing
  - [VECTO-1020] - HashingTool Crashes
  - [VECTO-1021] - Invalid hash of job data


## 2019-05-29: Vecto 3.3.3.1609 Release Candidate

- Improvement
  - [VECTO-916] - Adding new tyre sizes
  - [VECTO-946] - Refactoring XML reading
  - [VECTO-965] - Add input fields for ADAS into VECTO GUI
  - [VECTO-966] - Allow selecting Tank System for NG engines in GUI
  - [VECTO-932] - Consistency in NA values in the vsum file
- Bugfixes:
  - [VECTO-954] - Failed to find operating point for braking power (Fix for Notification
    Art. 10(2) - [VECTO-952])
  - [VECTO-979] - VECTO Simulation abort with 8-speed MT transmission (Fix for
    Notification Art. 10(2) - [VECTO-978])
  - [VECTO-931] - AT error in VECTO version 3.3.2.1519
  - [VECTO-950] - Error when loading Engine Full-load curve
  - [VECTO-967] - Engine-Only mode: Engine Torque reported in .vmod does not match
    the provided cycle
  - [VECTO-980] - Error during simulation run


## 2019-03-29: Vecto 3.3.2.1548 Official Release

- Bugfixes (compared to 3.3.2.1519-RC)
  - [VECTO-861] - 3.3.1: Torque converter not working correctly
  - [VECTO-904] - Range for gear loss map not sufficient.
  - [VECTO-909] - 3.3.2.1519: Problems running more than one input .xml
  - [VECTO-917] - TargetVelocity (0.0000) and VehicleVelocity (>0) must be zero when
    vehicle is halting
  - [VECTO-918] - RegionalDeliveryEMS LowLoading - ResponseSpeedLimitExceeded
  - [VECTO-920] - Urban Delivery: Simulation Run Aborted, TargetVelocity and
    VehicleVelocity must be zero when vehicle is halting!


## 2019-03-01: Vecto 3.3.2.1519 Release Candidate

- Improvements:
  - [VECTO-869] - change new vehicle input fields (ADAS, sleeper cab, etc.) to be
    mandatory
  - [VECTO-784] - Configuration file for VECTO log files
  - [VECTO-865] - Extend Sum-Data
  - [VECTO-873] - Add digest value to SumData
- Bugfixes:
  - [VECTO-729] - Bugs APT submodel
  - [VECTO-787] - APT: DrivingAction Accelerate after Overload
  - [VECTO-789] - APT: ResponseUnderload
  - [VECTO-797] - VECTO abort with AT transmission and TC table value
  - [VECTO-798] - VECTO abort with certified AT transmission data and certified TC data
  - [VECTO-807] - VECTO errors in vehicle class 1/2/3
  - [VECTO-827] - Torque converter inertia
  - [VECTO-838] - APT: ResponseOverload
  - [VECTO-843] - AT Transmissions problem on VECTO 3.3.1.1463
  - [VECTO-844] - Error with AT gearbox model
  - [VECTO-847] - Simulation abort due to error in NLog?
  - [VECTO-848] - AT Gearbox Simulation abort (was: Problem related to Tyres?)
  - [VECTO-858] - Urban Delivery Abort - with APT-S Transmission and TC
  - [VECTO-861] - 3.3.1: Torque converter not working correctly
  - [VECTO-872] - MRF/CIF: Torque Converter certification method and certification nbr
    not correctly set
  - [VECTO-879] - SIMULATION RUN ABORTED DistanceRun got an unexpected resp.
  - [VECTO-883] - Traction interruption may be too long
  - [VECTO-815] - Unexpected Response: SpeedLimitExceeded
  - [VECTO-816] - object reference not set to an instance of an object
  - [VECTO-817] - TargetVelocity and VehicleVelocity must not be 0
  - [VECTO-820] - DistanceRun got an unexpected response:
    ResponseSpeedLimitExceeded
  - [VECTO-864] - Prevent VECTO loss-map extension to result in negative torque loss

### Installation Option (VECTO-784)

- VECTO 3.3.2 adds a new feature to run as "installed application" instead of the
  `portable` mode
- VECTO as `installed application`
  - Needs no write permissions to the VECTO application folder
  - All configuration files and settings are written to `%AppData%\VECTO\<Version>`
  - All log files are written to `%LocalAppData%\VECTO\<Version>`
- Switch to `installed application`
  1. Copy the VECTO directory and all its files and subdirectories to the appropriate
     location where the user has execute permissions
  2. Edit the file `install.ini` and remove the comment character (`#`) in 
     the line containing:
    
     ```txt
     ExecutionMode = install
     ```


## 2019-02-01: Vecto 3.3.1.1492 Official Release

- Bugfixes (compared to 3.3.1.1463-RC)
  - [VECTO-845] - Fixing bug for VECTO-840
  - [VECTO-826] - DistanceRun got an unexpected response: ResponseSpeedLimitExceeded
  - [VECTO-837] - VECTO GUI displays incorrect cycles prior to simulation
  - [VECTO-831] - Addition of indication to be added in Help and Release notes for simulations with
    LNG
- Changes according to 2017/2400 amendments
  - [VECTO-761] - Adaptation of input XML Schema
  - [VECTO-762] - Extension of Input Interfaces
  - [VECTO-763] - Extension of Segmentation Table
  - [VECTO-764] - ADAS benefits (according to Annex III Point 8. of amendment to 2017/2400)
  - [VECTO-766] - Update power demand auxiliaries (for extended segmentation table)
  - [VECTO-767] - Report for exempted vehicles
  - [VECTO-768] - VTP mode
  - [VECTO-770] - Fuel Types
  - [VECTO-771] - Handling of exempted vehicles
  - [VECTO-824] - Throw exception for certain combinations of exempted vehicle parameters
  - [VECTO-773] - Correction Factor for Reference Fuel
  - [VECTO-790] - Adapt generic data for construction/municipal utility
  - [VECTO-493] - Implementation of generic body weights and air drag values for construction cycle
  - [VECTO-565] - Consideration of LNG as possible fuel is missing

### New input parameters

- The new input fields (see table below) are optional in this version. When this release candidate
  will be an official version manufacturers MAY certify their new vehicles using the new input
  parameters. As from 1 st July 2019 the new input fields will become mandatory. Further details are
  provided in the timetable on the next page.

- Default values when the new input parameters are not provided:

  **Input parameters vehicle - Table 1 Annex III**

  | Field               | Default value |
  |---------------------|---------------|
  | VehicleCategory     | No default value but input 'rigid truck' will be converted automatically into 'rigid lorry' |
  | ZeroEmissionVehicle | 'No' |
  | NgTankSystem        | 'Compressed' (only applicable to gas vehicles) |
  | SleeperCab          | 'Yes' |
  
  **Input parameters ADAS - Pt. 8 Annex III**

  | Field                     | Default value |
  |---------------------------|---------------|
  | EngineStartStop           | 'No' |
  | EcoRollWithoutEngineStop  | 'No' |
  | EcoRollWithEngineStop     | 'No' |
  | PredictiveCruiseControl   | 'No' |

  Currently **only** the fuel type `NG PI` for the **engine certification** is allowed by 2017/2400. For LNG
  vehicles, therefore, the engine fuel type has to be set to `NG PI` and at the vehicle level
  NgTankSystem has to be set to **liquefied**. For CNG the same engine fuel type is used but
  NgTankSystem has to be set to **compressed**.


### Timetable

| Planned Date | Version  | Description |
|--------------|----------|-------------|
| 1. Feb. 2019  | 3.3.1.x | Release of official version of release candidate 3.3.1.1463
| 1. March 2019 | 3.3.2.x-RC | Release candidate, new input parametersare mandatory (+ further bugfixes)
| 1. April 2019 | 3.3.2.x | Official version of VECTO 3.3.2
| 1. May 2019   |         | Mandatory use of 3.3.1.x for certification
| 1. July 2019  |         | Mandatory use of 3.3.2.x for certification


## 2019-01-03: Vecto 3.3.1.1463 Release Candidate

- Changes/Improvements
  - [VECTO-799] - Remove TUG Logos from Simulation Tool, Hashing Tool
  - [VECTO-808] - Add Monitoring Report
  - [VECTO-754] - Extending Loss-Maps in case of AT gearbox for each gear, axlegear, gearbox
  - [VECTO-757] - Correct contact mail address in Hashing Tool
  - [VECTO-779] - Update Construction Cycle - shorter stop times
  - [VECTO-783] - Rename columns in segmentation table and GUI
  - [VECTO-709] - VTP editor from user manual not matching new VECTO one: updated
    documentation
  - [VECTO-785] - Handling of Vehicles that cannot reach the cycle's target speed: Limit max speed
    in driver model
  - [VECTO-716] - Validate data in Settings Tab: update documentation
  - [VECTO-793] - Inconsistency between GUI, Help and Regulation: update wording in GUI
    and user manual
  - [VECTO-796] - Adaptation of FuelProperties
  - [VECTO-806] - extend loss-maps (gbx, axl, angl) for MT and AMT transmissions
  - [VECTO-750] - Simulation error DrivingAction: adapt downshift rules for AT to drive 
    over hill with 6% inclination
- Bugfixes:
  - [VECTO-819] - object reference not set to an instance of an object
  - [VECTO-818] - SearchOperatingPoint: Unknown response type. ResponseOverload
  - [VECTO-813] - Error "Infinity [] is not allowed for SI-Value"
  - [VECTO-769] - DrivingAction Brake: request failed after braking power was
    found.ResponseEngineSpeedTooHigh
  - [VECTO-804] - Error on simulation with VECTO 3.3.0.1433
  - [VECTO-805] - Total vehicle mass exceeds TPMLM
  - [VECTO-811] - AMT: ResponseGearShift
  - [VECTO-812] - AMT: ResponseOverload
  - [VECTO-822] - SIMULATION RUN ABORTED by Infinity
  - [VECTO-792] - Vecto Hashing Tool - error object reference not set to an instance of an object
    (overwriting Date element)
  - [VECTO-696] - Problem with Primary Retarder: regression update, set torque loss to 0 
    for 0 speed and engaged gear
  - [VECTO-776] - Decision Factor (DF) field is emptied after each simulation
  - [VECTO-814] - Error: DistanceRun got an unexpected response: ResponseGearshift

### Update of Fuel Properties [VECTO-796]

| Fuel type | Density | Reference for<br>fuel properties | CO2 emission<br>factor | Lower Heating<br> Value | Data Source |
|---------|---------|---------|---------|---------|---------|
|         |         | [kg/m³] | [g_CO2/g_Fuel] | [MJ/kg] |
| Diesel  | B7    | 836 | 3.13 | 42.7 | CONCAWE/JEC (2018) |
| ED95    | ED95  | 820 | 1.81 | 25.4 | CONCAWE/JEC (2018) |
| Petrol  | E10   | 748 | 3.04 | 41.5 | CONCAWE/JEC (2018) |
| E85 | E85 | 786 | 2.10 | 29.3 | Calculated from E0 and E100 from CONCAWE/JEC (2018) |
| LPG | LPG | not required* | 3.02 | 46.0 | CONCAWE/JEC (2018)
| CNG | CNG(H-Gas) | not required* | 2.69 | 48.0 | CONCAWE/JEC (2018)
| LNG | LNG (EU mix. 2016/2030) | not required* | 2.77 | 49.1 | CONCAWE/JEC (2018)

- **[*]** VECTO does not provide volume based figures for gaseous fuels |
- **CONCAWE/JEC (2018):** Specifications are based on a recent analysis (2018) performed
  by CONCAWE/EUCAR and shall reflect typical fuel on the European market.
  The data is scheduled to be published in March 2019 in the context of the study:
  Well-To-Wheels Analysis Of Future Automotive Fuels And Powertrains in the European Context - Heavy Duty vehicles



## 2018-12-04: Vecto 3.3.0.1433 Official Release

- Bugfixes (compared to 3.3.0.1398)
  - [VECTO-795] - VECTO Hashing Tool crashes
  - [VECTO-802] - Error in XML schema for manufacturer's record file
- Bugfixes (compared to 3.3.0.1250)
  - [VECTO-723] - Simulation aborts with engine speed too high in RD cycle
  - [VECTO-724] - Simulation aborts with error 'EngineSpeedTooHigh' - duplicate of VECTO-744
  - [VECTO-728] - Simulation aborts when vehicle's max speed (n95h) is below the target speed
  - [VECTO-730] - Simulation Aborts with ResponseOverload
  - [VECTO-744] - ResponseEngineSpeedTooHigh (due to torque limits in gearbox)
  - [VECTO-731] - Case Mismatch - Torque Converter
  - [VECTO-711] - Elements without types in CIF and MRF
  - [VECTO-757] - Correct contact mail address in Hashing Tool
  - [VECTO-703] - PTO output in MRF file
  - [VECTO-713] - Manufacturer Information File in the legislation is not compatible with the
    Simulation results
- Improvement (compared to 3.3.0.1250)
  - [VECTO-704] - Allow VTP-simulations for AT gearboxes


## 2018-10-30: Vecto 3.3.0.1398 Release Candiate

- Bugfixes:
  - [VECTO-723] - Simulation aborts with engine speed too high in RD cycle
  - [VECTO-724] - Simulation aborts with error 'EngineSpeedTooHigh' - duplicate of VECTO-744
  - [VECTO-728] - Simulation aborts when vehicle's max speed (n95h) is below the target speed
  - [VECTO-730] - Simulation Aborts with ResponseOverload
  - [VECTO-744] - ResponseEngineSpeedTooHigh (due to torque limits in gearbox)
  - [VECTO-731] - Case Mismatch - Torque Converter
  - [VECTO-711] - Elements without types in CIF and MRF
  - [VECTO-757] - Correct contact mail address in Hashing Tool
  - [VECTO-703] - PTO output in MRF file
  - [VECTO-713] - Manufacturer Information File in the legislation is not compatible with the
    Simulation results
- Improvement
  - [VECTO-704] - Allow VTP-simulations for AT gearboxes


## 2018-06-04: Vecto 3.3.0.1250

- Improvement
  - [VECTO-665] - Adding style information to XML Reports
  - [VECTO-669] - Group 1 vehicle comprises vehicles with gross vehicle weight > 7.5t
  - [VECTO-672] - Keep manual choice for "Validate data"
  - [VECTO-682] - VTP Simulation in declaration mode
  - [VECTO-652] - VTP: Check Cycle matches simulation mode
  - [VECTO-683] - VTP: Quality and plausibility checks for recorded data from VTP
  - [VECTO-685] - VTP Programming of standard VECTO VTP report
  - [VECTO-689] - Additional Tyre sizes
  - [VECTO-702] - Hashing tool: adapt warnings
  - [VECTO-667] - Removing NCV Correction Factor
  - [VECTO-679] - Engine n95h computation gives wrong (too high) engine speed
    (above measured FLD, n70h)
  - [VECTO-693] - extend vehicle performance in manufacturer record
- Bugfixes:
  - [VECTO-656] - Distance computation in vsum
  - [VECTO-666] - CF_RegPer no effect in vehicle simulation -- added to the engine correction factors
  - [VECTO-687] - Saving a Engine-Only Job is not possible
  - [VECTO-695] - Bug in vectocmd.exe - process does not terminate
  - [VECTO-699] - Output in manufacturer report and customer report (VECTO) uses different units
    than described in legislation
  - [VECTO-700] - errorr in simulation with 0 stop time at the beginning of the cycle


## 2018-02-07: Vecto 3.2.1.1133

- Improvement
  - [VECTO-634] - VTP Mode: specific fuel consumption
- Bugfixes:
  - [VECTO-642] - VECTO BUG - secondary retarder losses:
  **IMPORTANT:** Fuel-consumption relevant bug! wrong calculation of retarder losses for retarder
  ratio not equal to 1
  - [VECTO-624] - Crash w/o comment: Infinite recursion
  - [VECTO-627] - Cannot open Engine-Only Job
  - [VECTO-629] - Vecto crashes without errror message (same issue as VECTO-624)
  - [VECTO-639] - Failed to find operating point for braking power: cycle with low target speed
    (3km/h). allow driving with slipping clutch
  - [VECTO-640] - Exceeded max. iterations: driving fully-loaded vehicle steep uphill. fixed by
    allowing full-stop and drive off again
  - [VECTO-633] - unable to start VTP Mode simulation
  - [VECTO-645] - Encountered error while validating Vecto output (generated by API) through
    Hashing tool for vehicle without retarder


## 2017-12-15: Vecto 3.2.1.1079

- Improvement
  - [VECTO-618] - Add Hash value of tyres to manufacturer's record file
  - VECTO-590] - Handling of hash values: customer's record contains hash of
    manufacturer'srecord
  - [VECTO-612] - Continuously changing hashes: Info in GUI of HashingTool
  - [VECTO-560] - Change Mail-Address of general VECTO contact
  - [VECTO-616] - SI-Unit - display derived unit instead of base units
- Bugfixes:
  - [VECTO-608] - Power balance in EPT-mode not closed
  - [VECTO-611] - Invalid input. Cannot cast `Newtonsoft.Json.Linq.JObject` to
    `Newtonsoft.Json.Linq.Jtoken`
  - [VECTO-610] - TyreCertificationNumber missing in Manufacturer Report
  - [VECTO-613] - Incomplete description of allowed values of LegislativeClass (p251) in VECTO
    parameter documentation
  - [VECTO-625] - Update XML Schema: Tyre dimensions according to Technicall Annex, trailing
    spaces in enums
- Support
  - [VECTO-615] - Error torque interpolation in declaration jobs exported to XMLImprovements


## 2017-11-20: Vecto 3.2.1.1054

- Improvements:
  - [VECTO-592] - VTP Simulation Mode
  - [VECTO-605] - Improve simulation speed
- Bugfixes:
  - [VECTO-602] - Error in simulation without airdrag component
  - [VECTO-589] - Scheme .xml error


### VTP Simulation Mode
- Verification Test Procedure (VTP) Simulation Mode
- Similar to Pwheel mode, different cycle format (see user manual)
- Requires:
  - Vehicle in declaration mode (XML)
  - Measured driving cycle
  - Parameters for engine-fan model
- VECTO calculates the gear based on the wheel speed and engine speed (and
  vehicle parameters) and ignores the gear provided in the driving cycle
- Fuel consumption interpolation is done using the engine speed from the cycle
  and calculated power demand (to avoid wrong engine speeds due to wrong gear
  selection)
- Simulation uses all auxiliaries except engine fan
  - Engine fan is modeled separately, power demand depends on fan speed (see user manual)
- Auxiliary power selected according to segment table, BUT power demand
  depends on vehicle speed
  - v < 50 km/h: Urban
  - 50 <= v < 70 km/h: Rural
  - v >70 km/h: Long haul
- Gear and fuel consumption in the driving cycle are optional for now, may be used
  in future versions


## 2017-10-19: Vecto 3.2.0.1022

- Bugfixes:
  - [VECTO-585, VECTO-587] - VECTO Simulation aborts when run as WCF Service
  - [VECTO-586] - Gearshiftcout in reports too high
  - [VECTO-573] - Use of old library references .net framework 2.0


## 2017-10-02: Vecto 3.2.0.1005

- Improvements:
  - **Release of Vecto Hashing Tool**
  - [VECTO-557] Engine speed simulated too high during long stops
- Bugfixes:
  - [VECTO-569] - `Engine Retarder` not correctly recognized as input
  - [VECTO-571] - Customer Report - wrong output format of average RRC
  - [VECTO-573] - Correction of displayed units in graph window
  - [VECTO-575] - Correction of simulation aborts (due to gearbox inertia, engineering mode)
  - [VECTO-577] - Correction of XML export functionality
  - [VECTO-579] - Bug fix GUI crashes on invalid input
  - [VECTO-558] - Correction of output in .vsum file - BFColdHot always 0
  - [VECTO-564] - Bug fix: correct output of vehicle group in XML report
  - [VECTO-566] - Vehicle height not correctly read (engineering mode)
  - [VECTO-545] - Update documentation on Settings dialog


## 2017-07-28: Vecto 3.2.0.940

- Bugfixes:
  - [VECTO-546] - GearboxCertificationOptionType Option 2 not accepted by VECTO
  - [VECTO-547] - Engine Manufacturer and Engine Model are empty in .vsum
  - [VECTO-548] - online user manual
  - [VECTO-549] - Inconsistent (and wrong) decimal separator in XML output (manufacturer report)
  - [VECTO-551] - Average Tyre RRC not in Customer Information File output
  - [VECTO-536] - GUI: improvements vehicle dialog (add missing pictures for vehicle categories)
  - [VECTO-550] - Allow custom settings for AirDensity in Engineering mode
  - [VECTO-552] - set engine rated power, rated speed to computed values from FLD
    if not provided as input


## 2017-07-14: Vecto 3.2.0.925

- Improvements:
  - [VECTO-366] added EMS vehicle configuration, EMS is only simulated when 
    engine rated power > 300kW
  - [VECTO-463] add pneumatic system technology 'vacuum pump`
  - [VECTO-465] change RRC value of trailers (declaration mode) from 0.00555 to 0.0055
    (due to limits in user interface)
  - [VECTO-477] AT Gearbox, powershift losses: remove inertia factor
  - [VECTO-471] update cross-wind correction model: height-dependent wind speed (see Excel
    spreadsheet in User Manual folder for details)
  - [VECTO-367] Add Vehicle Design Speed to segmentation table
  - [VECTO-470] Add XML reading and export functionality
  - [VECTO-486] Adding hashing library
  - [VECTO-469] Limit engine max torque (either due to vehicle or gearbox limits),
    limit gearbox input speed
  - [VECTO-466] Update vehicle payloads: 10% loaded and reference load are simulated
  - [VECTO-467] Add generic PTO activation in municipal cycle
  - [VECTO-468] Add PTO losses (idle) in declaration mode
  - [VECTO-479] Added PTO option 'only one engaged gearwheel above oil level' with 0 losses
  - [VECTO-483] Adapt CdxA supplement for additional trailers
  - [VECTO-494] Implementation of different fuel types
  - [VECTO-502] Implementing standard values for air-drag area (if not measured)
  - [VECTO-501] Implement engine idle speed set in vehicle (must be higher than
    engine's idle speed value)
  - [VECTO-504] Adding HVAC technology 'none`
  - [VECTO-489] Extrapolate gearbox lossmaps (required when torque limitation by gearbox is ignored)
  - [VECTO-505] Implement AT transmissions in declaration mode
  - [VECTO-507] Allow to ignore validation of model data when starting a simulation
    (significant improvement on simulation startup time - about 10s)
  - [VECTO-506] modified method how torque-converter characteristics in drag is extended.
    allow drag- values in the input, only add one point at a high speed ratio
  - [VECTO-509] Add axle-type (vehicle driven, vehicle non-driven, trailer) to GUI
  - [VECTO-511] Add engine idle speed to Vehicle input form (GUI)
  - [VECTO-510] Write XML reports (manufacturer, customer information) in declaration mode
  - [VECTO-474] new driving cycles for Municipal and Regional Delivery
  - [VECTO-522] step-up ratio for using torque converter in second gear set to 1.85 for busses 
    (still 1.8 for trucks)
  - [VECTO-525] remove info-box with max loading in GUI
  - [VECTO-531] Payload calculation: limit truck payload to the truck's max payload.
    (earlier versions only limited the total payload of truc + trailer to 
    the total max. payload, i.e. allowed to shifted loading from truck to the trailer)
  - [VECTO-533] allow second driven axle, rdyn is calculated as average of both driven axles
  - [VECTO-537] new Suburban driving cycles
  - [VECTO-541] increase declaration mode PT1 curve to higher speeds 
    (2500 is too low for some engines)
  - [VECTO-542] reduce overspeed in declaration mode to 2.5km/h
- Bugfixes:
  - [VECTO-462] fix: decision if PTO cycle is simulated
  - [VECTO-473] fix: adapt range for validation of torque converter characteristics
  - [VECTO-464] fix: extrapolation of engine full-load curve gives neg. max. torque.
    Limit engine speed to n95h
  - [VECTO-480] fix: a_pos in .vsum was less than zero
  - [VECTO-487] fix: Duration of PTO cycle was computed incorrectly if PTO cycle
    does not start at t=0
  - [VECTO-514] fix: sort entries in .vsum numerically, not lexically
  - [VECTO-516] fix: consider axlegear losses for estimation of acceleration after gearshifts
  - [VECTO-517] fix: valid shift polygon was considered invalid when extended to
    very high torque ranges
  - [VECTO-424] fix: VectoCore.dll could not be found when the current working directory
    is different to the directory of the vectocmd.exe
  - [VECTO-425] fix: vectocmd.exe - check if the output is redirected, and skip updating
    of the progress bar when this is the case
  - [VECTO-426] fix: vectocmd.exe - log errors to STDERR
  - [VECTO-519] fix: computation of n95h fails for a valid full-load curve due to numerical inaccuracy. 
    add tolerance when searching for solutions
  - [VECTO-520] fix: gearashift count in vsum is 0


## 2017-01-18: Vecto 3.1.2.810

- Improvements:
  - [VECTO-445] Additional columns in vsum file
  - Allow splitting shift losses among multiple simulation intervals
  - Allow coasting overspeed only if vehicle speed > 0
  - Torque converter: better handling of "creeping" situations
- Bugfixes:
  - [VECTO-443] Bugfix in AMT shift strategy: skip gears not working correctly


## 2017-03-07: Vecto 3.1.2.796

- Improvements:
  - [VECTO-405] Adding clutch-losses for AMT/MT gearboxes during drive-off, reduce drive-off distance after stop from 1m
to 0.25m, set clutch closing speed (normalized) to 6.5%, changes in clutch model
  - [VECTO-379] Make GUI more tolerant against missing files. Instead of aborting reading
    the input data the GUI shows a suffix for missing input files
  - [VECTO-411] Allow a traction interruption of 0s for AMT/MT gearboxes
  - [VECTO-408] Gearbox Inertia for AT gearboxes set to 0
  - [VECTO-419] Adapted error messages, added list of errors
  - [VECTO-421,VECTO-439] Added volume-related results to vsum file (volume is computed based on default bodies)
  - [] Energy balance (vsum) and balance of engine power output and power consumers (vmod) level
  - [VECTO-430] AT shift strategy: upshifts may happen too early
  - [VECTO-431] AMT shift strategy always started in first gear due to changes in clutch model
  - [VECTO-433] adapt generic vehicles: use typical WHTC correction factors
  - [VECTO-437] set vehicle speed at clutch-closed to 1.3 m/s
  - [VECTO-436] fix simulation aborts with AT gearbox (neg. braking power, unexpected response, underload)
- Bugfixes:
  - [VECTO-415] Powershift Losses were not considered for AT gearboxes with PowerSplit
  - [VECTO-416] Measured Speed with gear failed when cycle contained parts with eco-roll 
    (computation of next gear failed)
  - [VECTO-428] Sum of timeshares adds up to 100%
  - [VECTO-429] Min Velocity for lookahead coasting was not written to JSON file


## 2017-01-18: Vecto 3.1.1.748

  - Bugfixes:
- [VECTO-404] Driving Cycle with PTO stopped simulation after first PTO activation


## 2017-01-12: Vecto 3.1.1.742

- Improvements:
  - [VECTO-390, VECTO-400] Adapt engine speed to estimated engine speed after gear shift
    during traction interruption (double clutching)
  - [VECTO-396, VECTO-388] Add shift losses for AT power shifts
  - [VECTO-389] new gear shift rules for AT gearboxes
  - [VECTO-387] added max input speed for torque converter
  - [VECTO-385] Automatically add generic torque converter data for drag
  - [VECTO-399] Add missions and loadings for vehicle categories 11, 12, and 16 (declaration mode)
  - [VECTO-384] cleanup memory after simulation run
  - [VECTO-394] new option for vectocmd to disable all output
  - [VECTO-392] make the GUI scale depending on the Windows font size
  - [VECTO-391] Gearbox output speed and output torque added to .vmod files
  - [VECTO-386] Gearbox window: disable input fields not applicable for the selected gearbox type
- Bugfixes:
  - [VECTO-401] Computation of n_95h etc. fails if engine`s max torque is constant 0
  - Lookup of Airdrag parameters in declaration mode
  - [VECTO-378] Improved file-handling in AAUX module


## 2016-11-14: Vecto 3.1.0.683

- Bugfixes:
  - [VECTO-375] Fixed bug when braking during slope change from negative to positive
    values.
  - [VECTO-372] Added check for unusual acceleration/deceleration data which could lead to
    error when halting.
  - [VECTO-371] Added additional behavior to overcome such situations
  - [VECTO-370] Added additional behavior to overcome such situations
  - [VECTO-369] CrosswindCorrection is now saved and read again from JSON files
  - [VECTO-373] WHTC-Engineering correction factor now correctly read/write in JSON files
  - [VECTO-368] Fixed validation for specific cases when values are intentionally invalid.
  - [VECTO-357] Updated GUI to not show ECO-Roll option to avoid confusion
  - Fixed numerous bugs in AT-ShiftStrategy regarding the Torque Converter
  - Fixed numerous bugs in MeasuredSpeed Mode (and MeasuredSpeed with Gear) in
    connection with AT-Gearbox and TorqueConverter
  - Fixed a bug when PTO-Cycle was missing
  - Corrected axle loss maps for Generic Vehicles in Declaration Mode to match
    technical annex
  - Corrected SumFile Cruise Time Share. Added that timeshares must add up to 100%
- Improvements:
  - [VECTO-355] Updated documentation, added powertrain schematics in chapter
    "Simulation Models"
  - [VECTO-374] Check range for Torque Converter speed ratio input data to be at least
    between 0 and 2.2
  - Updated many error messages to be more explicit about the reason of error
  - Added "Mission Profiles" Directory with driving cycles publicly available in the application
    root directory.
  - Added "Declaration" directory with the declaration data files in the application root
    directory.
  - Added warning when engine inertia is 0
  - Added check that engine speed must not fall below idle speed
    (even in measured speed mode)
  - Shift curve validation for AT gearboxes: shift curves may now overlap due to different
    shift logic in AutomaticTransmissions.
  - Updated Crosswind Coefficients for Tractor+Semitrailer


## 2016-10-24: Vecto 3.1.0.662

- Bugfixes:
  - [VECTO-360] Fixed error during startup of VECTO (loading of DLLs).
  - [VECTO-358] Fixed errors during simulation where vehicle unintentionally was
    driving backwards. Fixed 1Hz-Filter for ModFiles (distance was wrong under certain
    circumstances, vehicle seemingly jumping back before halt)
  - [VECTO-361] Fixed classification of vehicles with GVM of exactly 7500kg
  - [VECTO-364] Fixed an error in measured speed mode (run aborts).
  - [VECTO-363] Compute shift polygons in declaration mode now uses correct
    boundary for full load margin.
  - [VECTO-365] Fixed editing gears in declaration mode
- Improvements:
  - [VECTO-355] User Manual updated (Screenshots, Descriptions, File Formats,
    Vecto V2 Comments removed).
  - [VECTO-317] Declaration data for Wheel sizes updated
  - [VECTO-359] Simplified code regarding PT1 behavior.
  - [VECTO-323] PTO-Cycle may now be left empty when not used in driving cycle.


## 2016-10-14: Vecto 3.1.0.652

- Main Updates:
  - Removed VECTO Core 2.2
  - Refactoring of the User-Interface Backend: loading, saving files and
    validating user input uses Vecto 3 models
  - AT-Gearbox Model: differentiate between AT gearbox with serial torque
    converter and AT gearbox using powersplit
  - Numbering of gears with AT gearbox corresponds to mechanical gears,
    new column TC_locked in .vmod file to indicate if torque converter is active
  - Torque converter gear no longer allowed in input (added by Vecto
    depending on the AT model)
  - New implementation of torque converter model (analytic solutions)
  - Added PTO option for municipal utility vehicles: PTO idle losses, separate
    PTO cycle during standstill
  - Added Angledrive Component
  - Option for constant Auxiliary Power Demand in Job-File
  - Normalize x/y values before triangulating Delaunay map (transmission
    loss-maps, fuel consumption loss map)
  - Additional fuel consumption correction factor in declaration mode: cold/hot
    balancing factor
  - Added fuel consumption correction factor (WHTC, Cold/Hot balancing, …)
    in engineering mode
  - Update auxiliaries power demand according to latest whitebook
  - Allow multiple steered axles
  - Adapted engine idle controller (during declutch) - engine speed decreases faster
  - SUM-File: split E_axl_gbx into two columns, E_axl and E_gbx
  - New columns in mod-file: PTO, torque converter
  - Removed full-load curve per gear, only single value MaxTorque
  - Removed rims (dynamic wheel radius depends on wheel type)
  - Fixes in AAUX module: open correct file-browser, save selected files


## Oct 2016: Status quo VECTO software and open issues

### Next issues on the to do list

- **Further development of the AT model**
  Consideration of losses during power shifts, update of gear shift logics
- **Reimplementation of engine stop/start**
- **Declaration mode: implementation of EMS vehicle configurations**

### Items waiting for decision on methods and resources:

- **Update engine data (according to update of Annex II)**
  Other fuels than diesel, “top torque” feature, correction factor for periodic regerating DPFs
- **Declaration mode:**
  - **Revision of calculated vehicle loads**
  - **implementation of refuse cycle (instead “municipal”)**
    Update of driving cycle, consideration of generic PTO loads during collection part,
    generic body weight and payload
  - **VECTO output (approval authorities, customer info, monitoring)**
  - **Buses**
- **Predictive ADAS**


## 2016-07-19: Vecto 3.0.4.565

- Bugfixes:
  - AAUX HVAC Dialog does not store path to ActuationsMap and SSMSource
  - GUI: check for axle loads in declaration mode renders editing dialog useless
  - Vecto 2.2: Simulation aborts (Vecto terminates) when simulating EngineOnly cycles
  - Vecto 3: Building SimulationRun EngineOnly simulation failed


## 2016-06-28: Vecto 3.0.4.544

- Main Updates:
  - New gear shift strategy according to White Book 2016
  - New coasting strategy according to White Book 2016
  - New input parameters (enineering mode) for coasting and gear shift behavior
  - Use SI units in Advanced Auxiliaries Module and compile with strict compiler settings
    (no implicit casts, etc.)
  - Allow efficiency for transmission losses (in engineering mode)
- Bugfixes:
  - Auxiliary TechList not read from JSON input data
  - Improvements in driver strategy
  - Bugfixes in MeasuredSpeed mode


## Notes for using Vecto 3.x with AAUX
- The AdvancedAuxiliaries module requires the number of activations
  for pneumatic consumers (brakes, doors, kneeling) and the
  (estimated) total cycle time. This can be configured in the .APAC-file
  (actuations file). For standard bus/coach cycles (i.e., the cycle file
  contains “bus” and “heavy_urban” or “suburban” or “interurban” or
  “urban”; or the cycle contains “coach” (case insensitive)) the
  actuations file already contains the number of activations and the
  cycle time. For other cycles the filename without extension is used
  to lookup the activations in the .APAC file (case sensitive)
  
- Vecto 3 uses an average auxiliaries load (determined by the AAUX
  module depending on the settings) for the simulation. The AAUX module
  computes the fuel consumption in parallel to VectoCore and accounts for
  smart consumers (e.g., alternator, pneumatics, …).

- Output:
  - The .vmod file contains both, the fuel consumption calculated by VectoCore
    (per simulation interval) and AAUX (accumulated and per simulation interval).
  - Columns in .vmod file:
    - AA_TotalCycleFC_Grams [g]: accumulated fuel consumption as computed by the AAUX model,
      considering smart consumers
    - FC-Map [g/h]: fuel consumption as computed by VectoCore interpolating in the FC-Map,
      using an average base load of auxiliaries
    - FC-AUXc [g/h]: fuel consumption corrected due to engine stop/start
      (currently not applicable)
    - FC-WHTCc [g/h]: WHTC-corrected fuel consumption (not applicable in engineering mode)
    - FC-AAUX [g/h]: fuel consumption per simulation interval, derived from
      AA_TotalCycleFC_Grams
    - FC-Final [g/h]: final fuel consumption value with all (applicable) corrections applied
      (stop/start, WHTC, smart auxiliaries)
- Output .vsum
  - Columns in .vsum file:
    - FC-Map: total fuel consumption as computed by VectoCore interpolating in the FC-Map,
      using an average base load of auxiliaries
    - FC-AUXc: total fuel consumption corrected due to engine stop/start
      (currently not applicable)
    - FC-WHTCc: WHTC-corrected fuel consumption (not applicable in engineering mode)
    - FC-AAUX: fuel consumption per simulation interval, derived from AA_TotalCycleFC_Grams
    - FC-Final: final fuel consumption value with all (applicable) corrections applied
      (stop/start, WHTC, smart auxiliaries)


## 2016-06-21: Vecto 3.0.3.537

- Main Updates:
  - Plot shift lines as computed according to WB 2016 declaration mode in GUI
- Bugfixes:
  - GUI: Buttons to add/remove auxiliaries are visible again
  - Error in calculation of WHTC correction factor
  - Fix: consider gearbox inertia (engineering mode) for searching operating
  point
  - Wrong output of road gradient in measured speed mode (correct gradient for simulation)
  - Fuel consumption in .vsum file now accounts for AdvancedAuxiliaries model
  - GraphDrawer (Vecto): handle new .vmod format of Vecto 3
  - AdvancedAuxiliaries: language-settings independent input parsing
  - Paux was ignored when running Vecto 2.2
  - Error in massive multithreaded execution
  - Fix unhandled response during simulation
  - Fix output columns in .vmod


## 2016-05-10: Vecto 3.0.3.495

- Main Updates:
  - Support for Advanced Auxiliaries (Ricardo) in Vecto 3.0.3 and Vecto 2.2
  - Performance improvements
  - Gearshift polygons according to WB 2016
  - Revision of SUM-data file, changed order of columns, changed column headers
- Bugfixes:
  - Delaunay Maps: additional check for duplicate input points
  - Creation of PDF Report when running multiple jobs at once
  - Sanity checks for gear shift lines
  - Improvements DriverStrategy: handling special cases

### Performance Comparison

Total execution time (15 runs in parallel): Vecto 3.0.2: 6min 6s; Vecto 3.0.3: 35s


## 2016-03-11: VECTO 3.0.2

- Main updates:
- New simulation modes:
  - Pwheel (SiCo),
  - Measured Speed (with/without gear)
  - v_air/beta cross-wind correction (vcdb)
- Adaptations of powertrain components architecture
  - Move wheels inertia from vehicle to wheels
  - Auxiliaries no longer connected via clutch to the engine but via a separate port
  - Engine checks overload of gearbox and engine overload
- Fixed some driving behavior related issues in VectoCore:
  - When the vehicle comes to a halt during gear shift, instead of aborting the cycle,
    it tries to drive away again with an appropriate gear.
- ModData Format changed for better information and clarity
- Added validation of input values (according to latest VectoInputParameters.xls)
- Various bugfixes

### Pwheel (SiCo) Mode

- Function as already available in Vecto 2.2 also added in Vecto 3.0.2
  - Driving cycle specifies power at wheel, engine speed, gear, and
    auxiliary power
  - No driver model in the simulation.
  - The Vecto gear-shift model is overruled.
  - Function used for creating reference results for SiCo tests
  - For details see user manual: Simulation Models / Pwheel Input (SiCo)

### Measured Speed Mode

- Functionality already available in Vecto 2.2 added in Vecto 3.0.2
  - Driving cycle not defined by target speed but by actual speed. No driver
    model in the simulation.
  - Gear and engine speed can be specified in the driving cycle. 
    In this case the Vecto gear-shift model is overruled.
  - Function used for “proof of concept” purposes
  - For details see user manual: Calculation Modes / Engineering Mode / Measured Speed


### `.vmod` File Update

- In Vecto 3.0.2 the structure of the modal data output has been
  revised and re-structured. Basicaly for every powertrain component
  the `.vmod` file contains the power at the input shaft and the
  individual power losses for every component. For the engine the
  power, torque and engine speed at the output shaft is given along
  with the internal power and torque used for computing the fuel
  consumption.
- For details see the user manual: Input and Output / Modal Results
  (`.vmod`)


### Changelog 3.0.2

- New simulation modes:
- Measured Speed
- Measured Speed with Gear
- Pw heel (SiCo)
- Adaptations of pow ertrain components architecture
- Move w heels inertia from vehicle to w heels
- Auxiliaries no longer connected via clutch to the engine but via a separate port
- Engine checks overload of gearbox and engine overload
- Fixed some driving behavior related issues in VectoCore:
- When the vehicle comes to a halt during gear shift, instead of aborting the cycle, it tries to drive aw ay again with an appropriate gear.
- [ModData Format](#modal-results-.vmod) changed for better information and clarity
- Entries in the sum-file are sorted in the same w ay as in Vecto 2.2
- In engineering mode the execution mode (distance-based, time-based measured speed, time-based measured speed with gear, engine only) are detected based on the
cycle
- Added validation of input values
- Gravity constant set to 9.80665 (NIST standard acceleration for gravity)
- Improved input data handling: sort input values of full-load curves (engine, gbx, retarder)
- Better Integration of VectoCore into GUI (Notifications and Messages)
- v_air/beta cross-wind correction (vcdb) impemented
- For all calculations the averaged values of the current simulation step are used for interpolations in loss -maps.
- Allow extrapolation of loss maps in engineering mode (w arnings)
- Refactoring of input data handling: separate InputDataProvider interfaces for model data
- Refactoring of result handling: separate result container and output w riter
- New Long-Haul driving cycle included
- User Manual updated for VECTO V3.x
- Fix: sparse representation of declaration cycles had some missing entries
- Bugfix: error in computation of engine's preferred speed
- Bugfix: w rong vehicle class lookup
- Bugfix: duplicate entries in intersected full-load curves
- Bugfix: retarder takes the retarder ratio into account for lossmap lookup
- Bugfix: use unique identifier for jobs in job list
- Bugfix: error in triagulation of fuel consumption map


----

### VECTO 2.2

- TODO: Traction interruption: No engine rev down - hold rpm until new gear engaged (Allison)
- Bugfix: Error in Declaration Mode Pneumatic System aux power calculation ([kW] were interpreted as [W])
- Bugfix: Error in Declaration Mode Electric System aux power calculation
- Moved gear-specific Full Load Curves to Gearbox File
- Combined Drag Coefficient * Cross Sectional Area in one input parameter
- Updated .vgbx file format (Added gear-specific Full Load Curves)
- Updated .veng file format (Removed gear-specific Full Load Curves)
- Updated .vveh file format (Combined Drag Coefficient * Cross Sectional Area in one parameter)
- Updated Generic Vehicles (new file formats)
- Removed WHTC Correction Factor Calculation. Now in external tool, VECTO-Engine.
- Test Options are now only available in Engineering Mode
- Gearbox Editor now shows generic and user-defined shift polygons (if available)
- Various small updates in GUI
- Added 'Create JIRA Issue' dialog

### VECTO 2.1.4
* Bugfixes in start gear and (A)MT shift model
* Updated Coach .vcdv file for higher speeds to avoid extrapolation
* Renamed output "FC" to "FC-Map" for better clarification
* Same header for g/h and g/km output
* Reduced minimum turbine speed for 1C-to-2C AT up-shift condition from 900 to 700rpm.
* Updated cross wind correction parameters to current White Book values

### VECTO 2.1.3
* PwheelPos output in VSUM file.
* Implemented new Cd*A(v) method
* Bugfix in TC model
* Bugfix: Unit error in Cd(v) methods caused incorrect Delta-Cd value being used

### VECTO 2.1.2
* Improved TC iteration for higher precision
* Extended possible TC speed ratio 

### VECTO 2.1.1
* Bugfix: Incorrect torque calculation in AT/TC model caused early up-shifts
* Updated C-to-C shift strategy with acc_min rule (see V2.1)

### VECTO 2.1
* Limit engine rpm in torque converter operation acc. > acc_min
* Shift up (C-to-L, L-to-L) if acc. > acc_min and next-gear-rpm > threshold
* C-to-C up-shift condition based on N80h engine speed (instead of N95h)
* Pwheel-Input (SiCo Mode)
* FC [g/h] is always saved in output (in addition to [g/km]), not only in Engine Only mode
* Updated DEV options for AT update
* "DEV" tab is now "Test"
* GUI: Corrected air density unit in GUI
* Bugfix: Format error in .vmod header

### VECTO 2.0.4-beta4_Test (Test Release)
* Transmission loss extrapolation Errors are now Warnings in Engineering Mode.
* Bugfix: Error in TC Iteration caused crash
* Bugfix: Minimizing Graph window caused crash
* Fixed error in cycle conversion
* Errors if full load curve is too "short"

### VECTO 2.0.4-beta3
* Bugfix: VECTO didn't check if the full load curve covers the speed range up to nhi. Now it will abort if the full load curve is "too short"
* Update in Torque Convert model: Allowed engine speed range up to n95h (before: Pmax-speed)
* Bugfix: Rare crashes caused by gear shift model
* Bugfix: Error in engine inertia power calculation
* Torque Converter losses in modal results
* Implemented speed profile cleaning for very small values. (Caused shifting back to first gear when decelerating.)
* DEV Option for advanced CSV format output (units line, additional info)

### VECTO 2.0.4-beta2
* Bugfix: VECTO freezed if torque converter creeping was not possible due to low full load torque. Now it will abort with error message.
* Bugfix: Small fixes in torque converter model

### VECTO 2.0.4-beta1
* Updated CSV format of some declaration config files
* Various bugfixes in AT model
* rdyn validation
* Fixed bug in map interpolation
* Added ..\Developer Guide\Segment Table Description.xlsx
* Fixed bug that caused engine power > full load

### VECTO 2.0.4-beta
* AT update for 1C2C gearboxes
* Warning when opening or running files if file was created in different mode (Declaration/Engineering Mode)

### VECTO 2.0.3-beta0
* Implemented engine-side TC inertia input parameter in GBX file
* Updated User Manual for TC inertia
* Relabeled "OK" buttons to "Save" in input file editors

### VECTO 2.0.2-beta2
* AT/TC Update
* Various smaller fixes

### VECTO 2.0.2-beta1
* AT/TC Update
* Engine inertia power demand (PaEng) is now always calculated based on the previous engine speed rather than vehicle acceleration.
* Various smaller fixes

### VECTO 2.0.1-beta1-hotfix.VECTO-33
* Fixed VECTO-34
* Updated .vsum(.json)
    *  Added l/100km and CO2 results. (Fixed VECTO-33)
    *  Added FC-Final.
    *  Added Loading. (json)
    *  Added missing fuel parameters. (json)
* Updated README.md

### VECTO 2.0.1-beta1
* Updated Segment Table header
* Fixed Eco Roll (VECTO-30)
* Fixed Cycles in VECTO Editor being overwritten in Engineering Mode (VECTO-31)

### VECTO 2.0.1-beta0
* Update Notes > Release Notes
* Segment Table header

### VECTO 2.0
* Updated CSV file format. Now only one header with units included.
* Changed input file comment symbol form "c" to "#".
* Replaced old Demo/Default Data with "Demo Vehicles"
* Updated User Manual
* Declaration Mode
* Updated GUI including Charts
* New internal Graph for VMOD files (replaces GRAPHi)
* Shift polygons can be set separately for each gear
* Removed rated power (not used anymore)
* Removed rated engine speed form engine file. Now calculated form vfld file.

### VECTO 1.4.RC8
* Bugfix: Eco Roll didn't go into motoring operation when Overspeed-Limit was reached (could cause higher FC than Overspeed Mode)
* Minor update in demo data (12t motoring curve)

### VECTO 1.4.RC7
* Bugfix: Error in road gradient resulted in altitude error
* Speed reduction in smaller steps to get closer to full load curve (before speed was sometimes reduced too much and caused problem with gear shifting)
* Updates in demo data

### VECTO 1.4.RC6
* Bugfix in torque converter calculation

### VECTO 1.4.RC5
* Bugfix: Gears using torque converter and transmission loss maps may cause invalid "out of engine operation range" errors
* Null values for FzISO will abort calculation
* Exact road gradient calculation (sin(arctan(grad)*m*g) instead of grad*m*g) and road gradient influence on roll resistance (cos(arctan(grad)*m*g instead of m*g)
* Torque converter update: rpms over rated speed are not allowed.
* Fixed Wheels inertia in Demo Data

### VECTO 1.4.RC4
* Bugfix: FC interpolation failed when load points matched map points exactly.
* Bugfix: Invalid "FC= -10000!" errors when outside of FC-Map
* Bugfix: Vehicle stand-still at end of cycle was ignored (distance-based cycles only)
* FC extrapolation will not abort calculation. Invalid FC values are marked in output as "ERROR".
* No abortion if transmission output and input torque have different signs (In>0, Out<0). (Caused "Transmission Loss Map invalid" error messages)
* Eco-Roll revised. New rules:
    *  Engages if Pwheel < 0
    *  Disengages if Underspeed is reached.
* Look-Ahead Coasting now uses real coasting also if road gradient > 0 which means the coasting deceleration can be so high that no braking is necessary. In this case the braking phase will be omitted and the total deceleration time can be shorter than expected by the given target coasting deceleration.
* "Minimum (actual) speed" instead of "Min. Target Speed" for Eco-Roll, Overspeed and Look Ahead Coasting
* Major update in Gearbox/Toque Converter:
    *  Torque converter can be defined in multiple gears
    *  Same gear numbers in output as in GBX file, i.e. first gear with TC is not "TC" or "0.5" but simply "1"
    *  "Minimum time between two gear shifts" now also limits torque converter shifts
    *  Unlimited number of gears and new gear list in GUI without fixed gear number
    *  Improved gear shift model for torque converter
    *  Driving Cycle Preprocessing and Gear Shift Model now use approximated efficiency values based in the transmission loss maps. Reduces calculation time significantly with little to no impact on fuel consumption.
* Full load and drag curves (.vfld) can be defined for each gear separately.
* Bugfix: Distance Correction didn't work right with Look Ahead Coasting. Now distance error is acceptable but at the cost of partly interrupted coasting phases. Should be revised in future updates.
* Engine Only Mode: Engine motoring points can be defined explicitly in load cycle with "<DRAG>"
* When speed is under 5km/h and engine in motoring operating then gearbox shifts to Neutral
* Load-dependent rolling resistance coefficient
* Start-Stop activation delay time can be defined in job file
* File signing features added:
    *  After each calculation a signature file (.vsig) is created which includes signatures for all input and result files. The file itself is also signed.
    *  Signature files can be verified or manually created under "Tools" > "Sign or Verify Files"
* Changes in header and new parameters in modal results (.vmod):
    *  engine speed => n
    *  torque => Tq_eng
    *  Pe => Pe_eng
    *  New: Tq_clutch = torque at clutch (before clutch, engine-side)
    *  New: Tq_full = full load torque
    *  New: Tq_drag = drag torque
    *  Removed: Pe_norm, n_norm 
* Changes in summary results (.vsum)
    *  total altitude change instead of average gradient
    *  Auxiliary energy consumption for each auxiliary 
* Removed: Pe_norm, n_norm 
* Same job file list for BATCH and STANDARD (Job file list does not change when switching mode)
* Updated some error messages (units)
* Driving Cycle stop times corrected (No more zero stop times).

### VECTO 1.3.1.1
* Fixed error in power calculation (rotatory part of acceleration force)

### VECTO 1.3.1
* Fixed assembly information

### VECTO 1.3
* Some file-specific error messages link to files
* Eco-Roll, Overspeed, Look Ahead Coasting
* User Manual updated
* Demo Data updated

### VECTO 1.2
* Bugfix: When opening the Gearbox Editor the Gear Shift Settings (e.g. Skip Gears) were not valid for MT
* Fixed inaccuracy in FC interpolation (invalid extrapolation errors caused by rounding errors)
* Bugfix: Cycle was cut by one second in Engine Only (as usual in Vehicle Mode)

### VECTO 1.1 beta 4
* Engine Start/Stop implemented
* Fixed error in FC interpolation (invalid extrapolation errors)

### VECTO 1.1 beta 3
* FC Extrapolation will abort the calculation
* Transmission Type selection in Gearbox (.vgbx) file.
    *  Enables/Disables transmission type-specific options 
    *  In Proof-Of-Concept mode "Custom" type is available with all options enabled.
* Automatic Transmission mode with Torque Converter: Input parameters in Gearbox file !!still being tested!!
* Option to open files with GRAPHi or user-defined tool
* User Manual updated
* Bugfix: Files with relative paths were not located correctly

### VECTO 1.1 beta 2 - 01.03.2013
* Corrected comment line for wheels inertia and axle config in .vveh file
* Changed RRC unit in GUI from [-] to [N/N]
* User Manual updated
    *  Vehicle Editor: RRC
* Tranmission Loss Maps are not converted to n,Pe-Maps anymore. Should fix non-linear interpolation effects.
* Automatic Transmission / Torque Converter !!in development!! 
* Engine Only Mode 

### VECTO 1.1 beta - 06.02.2013
* New independent licensing dll replaces TUG's version
* Speed values below 0.09km/h are set to 0km/h
* Gear shift polygon model
    *  Replaces old gear shift model!
    *  New parameters in .vgbx file including path to gear shift polygons file
    *  Old gear shift model parameters removed from .vecto file
* Command Line Arguments processing:
    *  Changed prefix form "/" to "-"
    *  Bugfix: Argument "-run" was not processed
    *  Job files and driving cycles can be added via command line
    *  Files without path are expected in the Working Directory
* Cleared out some "PHEM" remains in log and message output
* User Manual update for command line arguments
* Various fixes in GUI
* Bugfix: Error in Cycle Conversion (distance- to time-based) when using Aux Power Input.
* Distance Correction is now active only in acceleration and cruise phases.
* Fixed cycles starting with vehicle speed = 0. Before, the first and second time step were averaged to speed values > 0.
* Demo data updated for new gear shift model

### VECTO 1.0 - 09.11.2012
* Bugfix: Error in Distance Correction

### VECTO 1.0 RC1
* Pe_full, Pe_drag output absolute (not normalized)

### VECTO 1.0 Beta 3
* User Manual implemented
* Declaration files processing (open)

### VECTO 1.0 Beta 2
* "Open" Button in main form opens all editable files (.vecto, .vveh, .veng, .vgbx)
* Editable files (.vecto, .vveh, .veng, .vgbx) can be opened via command line arguments (i.e. "Open with...") 
* Bugfix: Torque below drag curve during traction interruption
* Bugfix: Traction interruption failed in distance corrected time steps
* Removed unused parameters from .vecto file. OLD FILES ARE NOT SUPPORTED!!!
* Added JRC contact data in "About" form.