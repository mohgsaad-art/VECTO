
## VECTO v5.1.4-RC (03-09-2026)


### Features

- Extrapolate EHC data from LHE and RD (vecto/vecto!601)

- Deploy local beta releases (vecto/vecto!608)

- Optimized torque splitter (partial implementation) and PCC torque splitter (vecto/vecto!612)

- Added 'ExtraTechnologies' to monitoring data (vecto/vecto!626)

- Improve error reporting (vecto/vecto!635)

- vehicle int-test script in LEGENT server (vecto/vecto!642)

- Improve error reporting part2 (vecto/vecto!641)

- Consider CRF delta in report (vecto/vecto!645)


### Bug Fixes

- Battery-only P2 (vecto/vecto!580)

- Output writing for FCHVs (vecto/vecto!581)

- Consider ratio per gear for calculation of EM shift polygons in case of P2.5... (vecto/vecto!582)

- Extend GearShift cond when doCoastOvl for AMT (vecto/vecto!584)

- Add AMT 1st Gear DownShift safety (vecto/vecto!583)

- angledrive component file hashing (vecto/vecto!589)

- validation of IEPC data coming from a VIF (vecto/vecto!590)

- Unit Tests after multiple powertrains merge (vecto/vecto!592)

- PreRunOptions.WriteModAndSumData outputs (vecto/vecto!591)

- bugs created by multiple-powertrain refactoring. (vecto/vecto!597)

- Added missing mocked property for vehicle container (vecto/vecto!598)

- Add inertia torque reserve in Upshifts (vecto/vecto!595)

- Removed duplicate clutch from P1 vehicles (vecto/vecto!599)

- Validation & serialization vecto rundata in case of multiple powertrains, gearbox bindings multiple powertrains (vecto/vecto!600)

- Vocational output for group 16 vehicles (vecto/vecto!586)

- Added missing IPowertrainContainer parameter to creation methods of battery-only hybrids. (vecto/vecto!602)

- Added missing parameters to component factory methods of tests (vecto/vecto!603)

- Added property to VectoRunData for generator of multiple-powertrain SHEV (vecto/vecto!604)

- Removing unused classes (vecto/vecto!605)

- Correctly identify VIF report for mulitple-powertrain vehicles in HashingTool (vecto/vecto!606)

- error in validation of auxiliaries for battery-only hybrids (vecto/vecto!607)

- Engine fan's power demand ignored when powertrain has no engine (vecto/vecto!610)

- Property HybridElectricHDV returns true for vehicle data originating from hybrid VIFs (vecto/vecto!613)

- Add further VectoTrailer projects (tests), build script publish (vecto/vecto!614)

- Added missing components to simple powertrains for multiple-powertrain vehicles. (vecto/vecto!615)

- Added class1s vehicles for VTP selected mission: Regional Delivery (vecto/vecto!617)

- Debugged optimized torquesplitter (vecto/vecto!618)

- Improved error message for missing auxiliaries in completed vehicles (vecto/vecto!621)

- Added missing gearbox configuration for dependency injection (vecto/vecto!622)

- Reverted type of dummy gearbox back to APTN (vecto/vecto!624)

- Write axle number correctly in Monitoring Report & validate report in tests (vecto/vecto!625)

- Proper creation of IEPC completed vehicle in multistep tool (vecto/vecto!627)

- Allow v2.6 AirDrag component to be read for input in multistep tool (vecto/vecto!628)

- 1187cor - apply inertia trq reserve only if tqInterrupt (vecto/vecto!630)

- Adaptions for Vecto Trailer (vecto/vecto!629)

- Get proper segment from completed buses to retrieve CdxA for reports (vecto/vecto!632)

- P2.5 battery-only complete bus simulation aborts (vecto/vecto!631)

- Add build option to build script to create a release candidate build (vecto/vecto!633)

- problems of error handling (vecto/vecto!636)

- Caught exception when raising log event (vecto/vecto!637)

- Avoid null reference in 'Error Details' UI (vecto/vecto!638)

- Properly read certification number of fuel-cell component (vecto/vecto!639)

- Improved 'Error Details' UI component and GUI output messages. (vecto/vecto!640)

- CIF summary entry (vecto/vecto!644)

- Disable vehicles with non-identical axle-powertrains (vecto/vecto!646)

- Multiple-powertrains - torque share for coast or roll (vecto/vecto!648)

- Avoid null reference when calculating gearbox loss during initialization (vecto/vecto!649)

- Removed obsolete target .NET 8 from test and auxiliary projects (vecto/vecto!651)

- errors and updated information in README file (vecto/vecto!652)

- Added missing error codes (vecto/vecto!656)

- Server problems (vecto/vecto!653)

- label placement for cycle run info values (vecto/vecto!658)

- Prevent exception in gearbox loss calculation (vecto/vecto!659)

- Changed values for 'Manufacturer' and 'Certification Number' elements in... (vecto/vecto!661)

- Can open GUI documentation links when running VECTO from netXX folder (vecto/vecto!666)

- Overwriting vmods for FCHV OVC vehicles (vecto/vecto!671)

- Correct DS detect Condition for Brake after Acc UL (vecto/vecto!670)

- Coasting action after reengage (vecto/vecto!669)

- Change driveOffTorque APT Threshold for reengage (vecto/vecto!668)

- EM generic effmap for EM with asymmetric perfo. (vecto/vecto!667)


### Refactor

- Refactoring of multiple powertrains (vecto/vecto!587)

- Removed obsolete methods from IDataBus and increased component access through IPowerContainer properties (vecto/vecto!611)

- Removed some built-time warnings (vecto/vecto!619)

- Removed build-time warnings (vecto/vecto!623)

- Kept only one property denoting position in the electric motor class (vecto/vecto!643)

- Improve error reporting part3 (vecto/vecto!650)

- Improved error messages and added suggestions (vecto/vecto!654)

- Making methods virtual in moddatacontainer and allow overrides in trailer project (vecto/vecto!665)


### Testing

- Added HybridStrategyTests (vecto/vecto!593)


### Improvement

- Set internals visible to trailer project (vecto/vecto!609)

