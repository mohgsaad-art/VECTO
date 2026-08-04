
## VECTO v5.1.3 Official Release (03-08-2026)

This version replaces v5.1.1 which had to be retracted. It is the same in terms of functionality and results.

### Features

- multiple powertrains completed and single bus. (vecto/vecto!516)

- Single-gear IEPC in multiple powertrains (vecto/vecto!524)

- Added input (axle-number specific) validation for multiple-powertrain vehicles. (vecto/vecto!537)

- PCC for multiple powertrains. (vecto/vecto!565)


### Bug Fixes
- Vecto console references (vecto/vecto!507)

- Completed bus input generation for v2.7 (vecto/vecto!508)

- DefaultDriverStrategy - add accel action after gearshift + roll + brake if overload (vecto/vecto!510)

- Add retarder losses to TC max torque request (vecto/vecto!511)

- Rename constructor parameter to match factory method (vecto/vecto!512)

- Propagate 'allowDeprecated' parameter for XMLReader classes through injection hierarchy (vecto/vecto!513)

- Multiple powertrain FCHV bus execution (vecto/vecto!515)

- vsum fields for Retarder, Axlegear for multiple powertrains. (vecto/vecto!517)

- Increase max iteration in InterpolateSearch (vecto/vecto!514)

- use of battery SoC limits (input vs default) (vecto/vecto!519)

- Create fuel cell in testpowertrain (required in shift strategy) (vecto/vecto!520)

- Added missing function to DummyRunNinjectModule (vecto/vecto!521)

- Calculation of axlegear efficiency for P4 vehicles: in case of an P4, use EM-in to calculate axlegear efficiency instead of Brake-in (vecto/vecto!522)

- Added multiple-powertrain sample jobs (vecto/vecto!525)

- Avoid null reference in DeclarationData.cs (vecto/vecto!526)

- bugs regarding multiple powertrains. (vecto/vecto!527)

- Modified Torque Converter columns to handle multiple powertrains. (vecto/vecto!528)

- OVC-HEV mode for FCHV vehicles, null reference in fuel cells. (vecto/vecto!529)

- Don't allow unsupported variations of multiple powertrain vehicles. (vecto/vecto!530)

- Enable unsupported vehicles in development mode (vecto/vecto!531)

- Modified xArchitecture element in reports to support multiple powertrains. (vecto/vecto!533)

- Calculate TotalPropulsionPower for multiple-powertrain vehicles (vecto/vecto!534)

- Improved error message when not finding primary vehicle missions (vecto/vecto!535)

- Removed engine power from calculation of total propulsion for SHEV (vecto/vecto!536)

- Added missing parameter to method that creates gearbox. (vecto/vecto!538)

- Modified method signature to satisfy interface (vecto/vecto!539)

- Re-enabled unit tests, that required battery input data. (vecto/vecto!540)

- Added OVC info to tests' vehicle input (vecto/vecto!541)

- Ignore monitoring data when calculating job hash for reports (vecto/vecto!544)

- hashing tests (vecto/vecto!545)

- Save vehicle in job editor (vecto/vecto!546)

- Remove xml comments before calculating hash (vecto/vecto!547)

- Hashing Tool ignores comments when verifying XML content, and Monitoring... (vecto/vecto!551)

- Null reference & sample job file (vecto/vecto!552)

- Handle cycles that fail during setup of the follow-up run. (vecto/vecto!553)

- Added pre-processing for APT-N gearboxes (vecto/vecto!554)

- Overloaded equality operators in GearshiftPosition (vecto/vecto!556)

- TC dryRun Response in Motoring conditions (vecto/vecto!561)

- Brake Overload management (vecto/vecto!560)

- Handled PCC pre-processing for IEPC single-speed gearbox (vecto/vecto!562)

- Corrected EM ElectricPowerToBattery for multiple powertrains. (vecto/vecto!563)

- Added missing dummy axlegear to E4 axle-powertrain (vecto/vecto!564)

- Avoid null reference in TestPowertrain Gearboxes (vecto/vecto!566)

- verification of reports with bad job hashes (incl. Monitoring Data) (vecto/vecto!568)

- Handle primary cycles with insufficient fuel cell power (vecto/vecto!569)

- IHPC - add ignoreReason for clutch slipping (vecto/vecto!567)

- Brake Overload management when engaged (vecto/vecto!570)

- date parsing in fuel cell component hashing (vecto/vecto!572)

- Integrate RESS ovl/underload in EM response (vecto/vecto!571)

- failing unit tests for shift strategies (vecto/vecto!573)

- Integrate RESS ovl/underload in EM response - corrected (vecto/vecto!574)

- Handle InterUrban cycle setup fail in FCHV primary bus (vecto/vecto!575)

- fuel cell input in multiple-powertrain vehicle file (vecto/vecto!576)

- Modified version-checking method to accept versions without build part (vecto/vecto!577)


### Refactor

- Refactored multiple powertrains builder methods (vecto/vecto!509)

- Reduced build-time warnings (vecto/vecto!518)

- Project vecto sw3/task 2/unit tests amdm3 (vecto/vecto!460)

- Modified IDataBus properties/methods to handle single & multiple powertrains. (vecto/vecto!542)

- Renamed VectoRunData properties to indicate single powertrain (vecto/vecto!543)

- Minor cleanup/refactoring to reduce build-time warnings. (vecto/vecto!548)

- Cleaned up root folder (vecto/vecto!549)

- Cleanup/refactoring to reduce build-time warnings - part II. (vecto/vecto!550)

- Reduce compile-time warnings part III (vecto/vecto!555)

- Eliminated compile-time warnings in VectoCoreTests. (vecto/vecto!557)

- Reduce compile-time warnings in new Test projects. (vecto/vecto!558)

- Minor refactoring (vecto/vecto!559)

