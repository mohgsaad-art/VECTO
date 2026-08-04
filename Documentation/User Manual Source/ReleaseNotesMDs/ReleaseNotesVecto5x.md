# VECTO 5\.x Release Notes

![](img%5CRelease%20Notes%20Vecto4x0.png)

<!-- Cover Slide -->



# VECTO v5.1.3 Official Release (03-08-2026)

This version replaces v5.1.1 which had to be retracted. It is the same in terms of functionality and results.

## Features

- multiple powertrains completed and single bus. (vecto/vecto!516)

- Single-gear IEPC in multiple powertrains (vecto/vecto!524)

- Added input (axle-number specific) validation for multiple-powertrain vehicles. (vecto/vecto!537)

- PCC for multiple powertrains. (vecto/vecto!565)


## Bug Fixes
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


## Refactor

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


# VECTO v5.0.7 Official Release (14-10-2025)


## Bug Fixes

- Release notes link in GUI dialog (vecto/vecto!480)
- Battery only mode for IHPC (vecto/vecto!481)
- Add System.Data.SqlClient to VECTO.vbproj (vecto/vecto!482)
- Treat 'not applicable' as 'none' in HeatPumpTypeDriverCompartmentType (vecto/vecto!483)
- Correct parsing of fuel cell in interim file (vecto/vecto!484)
- Proper mapping for ADC loss-map in XML (vecto/vecto!485)
- Improved error message for missing battery SoC bounds (vecto/vecto!486)
- VETO main version in dialogs (vecto/vecto!488)
- Corrected XSD dynamic charging types (vecto/vecto!491)
- Introduce new mod data postprocessing for BO-HEV (vecto/vecto!489)
- Copy monitoring data from input (for completed vehicles) (vecto/vecto!494)
- Generic vehicles engineering mode VTP files (vecto/vecto!492)
- Surround post-mortem analysis with try/catch block (vecto/vecto!495)
- Iepc gearshift (vecto/vecto!487)
- Iepc gearshift torque reserve (post MR !487) (vecto/vecto!496)
- Corrected name for Tyre in VIF (vecto/vecto!497)
- Corrected json sample file (vecto/vecto!498)
- Added missing bindings for mod-data post processing (vecto/vecto!499)
- Allow VTP with v2.4 vehicles. (vecto/vecto!503)
- Updated some sample vehicles to v2.7  (vecto/vecto!504)

## Refactor

- Remove redundant code (vecto/vecto!493)



# VECTO v5.0.6-RC (22-09-2025)


## Features

- Update jobs in Generic Vehicles to version v2.7 (vecto/vecto!453)
- Disable v2.4 jobs (vecto/vecto!458)
- CodeEU #1140: Update multistep GUI to work with new XSDs (vecto/vecto!469)
- Multiple axles partial implementation (vecto/vecto!471)


## Bug Fixes

- Fix FCHV unit tests (vecto/vecto!448)
- XMLConversionTool bug fixes, more test cases, refactoring (vecto/vecto!452)
- Bug fixes for FCHV bus (vecto/vecto!454)
- CodeEU #1147: Made FuelCell Minpower, Maxpower optional (vecto/vecto!456)
- Engine-only simulation (vecto/vecto!457)
- EM data in PHEV rundata creation (vecto/vecto!459)
- CodeEU #1164: Lifetime ranges in reports for PEV, HEV-OVC. (vecto/vecto!461)
- Work-around in ranges to make tests succeed (vecto/vecto!462)
- CodeEU #1163: Respect job's battery SoC limits (vecto/vecto!463)
- CodeEU #870, #871, #924: Extend Accelerate condition after xEV Overload (vecto/vecto!466)
- VTP generic vehicles (vecto/vecto!465)
- Extend Accelerate condition after xEV Overload
- Changed v2.6 XSD to allow DeltaCdxA_declared and DeltaTransferredCdxA value: zero (vecto/vecto!472)
- Avoid cyclic refs from !473 (vecto/vecto!475)
- CodeEU #1167: Added WheelEnd info to MRF (vecto/vecto!476)
- Angledrive mod data, and PWheel axlegear efficiency (vecto/vecto!477)
- Disable engineering mode for multiple powertrains (vecto/vecto!478)


## Refactor

- FCHV iterative run strategy  (vecto/vecto!449)
- Update VECTO to NET 8 (vecto/vecto!467)
- Old .NET references (vecto/vecto!468)
- MultistepTool deprecated views (vecto/vecto!470)
- Remove unnecessary usings and nugets (vecto/vecto!473)


# VECTO v5.0.4-DEV (25-08-2025)


## Features

- 3rd amendment reports for buses (vecto/vecto!421)
- V1.0 reports for multiple powertrain lorries (vecto/vecto!395)
- Add Diesel B100 CI fuel (vecto/vecto!399)
- MRF and Monitoring report for multiple-powertrain primary buses (vecto/vecto!410)
- battery only mode for P2 (vecto/vecto!425)
- Run simulation for H2-ICE bus (primary + completed) (vecto/vecto!432)
- FCHV bus simulation, primary & completed (vecto/vecto!433)
- Single-bus mode for FCHV (vecto/vecto!435)
- Enable all v2.7 vehicles (vecto/vecto!436)
- VTP input and formulas for buses and trucks (vecto/vecto!424)


## Bug Fixes

- Exception when getting MaxWindowsSize (vecto/vecto!422)
- For FCHV, APT-S/P gearboxes simulated as APT-N. (vecto/vecto!423)
- Ovc s-hev cs cd (vecto/vecto!420)
- Convert property Type to Architecture in axle powertrains (vecto/vecto!393)
- 3 job types for multiple powertrains (vecto/vecto!394)
- Removed NgTankSystem from Multiple_SHEV primary bus (vecto/vecto!398)
- Remove Retarder component from X4 architectures (vecto/vecto!400)
- Use multiple factory methods for Retarder and Angledrive data providers (vecto/vecto!406)
- Added FCHV missing gearbox bindings (vecto/vecto!427)
- Airdrag element in VIF report. (vecto/vecto!430)
- Do not require SoC limits for HV non-OVC (vecto/vecto!431)
- B100 density to 890 kg/m3 (vecto/vecto!411)
- Run old VIFs with v2.7 Completed vehicles (vecto/vecto!434)
- 1065 vehicle co2 group (vecto/vecto!428)
- DoCoast - add drive condition for overload (vecto/vecto!426)
- Generic retarder and failing tests (vecto/vecto!437)
- IEPC data adaptation (vecto/vecto!438)
- IHPC VECTO run data (vecto/vecto!439)
- FCHV IEPC rundata gearbox creation (vecto/vecto!440)
- Generic vehicles that failed to run (vecto/vecto!441)
- WHRCharger creation (vecto/vecto!442)
- Update wheelEnd sample (vecto/vecto!443)
- FCHV files in engineering mode (vecto/vecto!444)
- Initialize MaxChargingPower from static data if not available in input (vecto/vecto!445)
- FCHV battery and CD and CS runs (vecto/vecto!446)

## Refactor

- Merge refactoring branch (vecto/vecto!419)
- Merging refactorings from SW3 project to (vecto/vecto!396)



# VECTO v5.0.3 Official Release (08-07-2025)


## Bug Fixes

- Avoid em conditioning calculation for non-FCHV (vecto/vecto!416)
- Safe conditioning data lookup (vecto/vecto!417)


# VECTO v5.0.1 Official Release (03-07-2025)


## Bug Fixes

- FCHV engineering run (vecto/vecto!391)

- H2 check in bus job (vecto/vecto!392)

- Auxiliaries REESS connection (vecto/vecto!397)

- Read PowerOutputConsumptionMap as kW (vecto/vecto!401)

- Select pruned missions for FCHV primary bus (vecto/vecto!403)

- Use angledrive in lorries' gearshift data creation (vecto/vecto!404)

- Remove wrong bus angledrive restrictions (vecto/vecto!405)

- Decl GUI error message when FCHV in eng mode (vecto/vecto!407)

- Set NgTankSystem default for primary buses (vecto/vecto!408)

- FCHV F-IEPC simulation runs! (vecto/vecto!409)

- Operational range for group 10 vehicle weights (vecto/vecto!402)

- Ovc fc weighting to correspond to CO2 computation (vecto/vecto!413)

- 3s Buffer compute max EM PLoss for FL OPs (vecto/vecto!412)

- Conditioning power demand for FCHVs (vecto/vecto!414)




# VECTO v5.0.0-RC (05-06-2025)


## Features

- New (v2.7) XSD for vehicles (vecto/vecto!334)

- Partial implementation for new vehicle battery (vecto/vecto!337)

- Readers for v2.7 vehicle XSD, and support for fuel cell vehicles. (vecto/vecto!341)

- Read monitoring data from job (vecto/vecto!345)

- 3rd amendment mrf cif xml schemas (vecto/vecto!340)

- MRF v1.0 vehicle (lorries and FCHV primary buses) writers (vecto/vecto!354)

- CIF v1.0 vehicle part (v2.4 vehicles and v2.7 lorries) (vecto/vecto!355)

- Use monitoring data from job to write report (vecto/vecto!360)

- In motion charging postprocessing (vecto/vecto!344)

- Disable (for RC & official) v27 vehicles except H2-ICE & FCHV lorries (vecto/vecto!375)

- EM-IEPC Thermal Derating - Tq_max and Buffer Mods - Post VECTO-4.3.4 Feed-Back (vecto/vecto!378)

- Forbid AT upshift for reduced dt before brake (vecto/vecto!381)

- Readers for v2.7 buses, improved reader tests. (vecto/vecto!382)

- HEV - Get Best dSOC in vsum (vecto/vecto!383)


## Bug Fixes

- Non-https link in manual (vecto/vecto!339)

- V2.7 reader & XSD (vecto/vecto!342)

- Updated Monitoring Report XSD (FCHV, Multiple powertrains) (vecto/vecto!335)

- Updated XSLT file and hashing code for new vehicles and components. (vecto/vecto!338)

- Lock StoredResults list before accessing it to avoid race condition (vecto/vecto!343)

- V27 vehicle issues (vecto/vecto!346)

- 882 merge artifacts (vecto/vecto!347)

- FCHV angledrive input (vecto/vecto!348)

- Modify schema so that results can be written compatible with results for 2nd amendment: (vecto/vecto!349)

- Restore deleted code in monitoring report (vecto/vecto!350)

- Typo in MRF Inject module (vecto/vecto!351)

- Proper namespace for VIF IEPC sub-element (vecto/vecto!352)

- Mockup tests run successfully (vecto/vecto!353)

- Correcting errors in XML schema (and sample files): no engine output in... (vecto/vecto!356)

- Bugfixes/updates for the Monitoring report and testing via the MockupTests. (vecto/vecto!357)

- Replace U+2013 by regular dashes (vecto/vecto!359)

- Added missing IMC testdata (vecto/vecto!361)

- Retarder compulsory in all MRF vehicle components. (vecto/vecto!362)

- Add further condition to decide which results to write in case the input data is a Multistep bus (vecto/vecto!363)

- Check Articulated in json vehicle (vecto/vecto!365)

- Segment in Bus AirDrag data creation (vecto/vecto!364)

- FCHV pre-run execution (vecto/vecto!366)

- Write ZeroCO2EmissionsRange and HydrogenRange to H2-ICE reports. (vecto/vecto!367)

- Remove wrong angledrive restrictions (vecto/vecto!369)

- Set vectorundata in completed bus results, (vecto/vecto!368)

- Simulate OVC for FCHVs (vecto/vecto!370)

- FCHV H2 range in reports (vecto/vecto!371)

- Architecture in some MRF v1.0 tests (vecto/vecto!372)

- H2 properties check in exempted vehicle input (vecto/vecto!373)

- FCHV input classes inheritance (vecto/vecto!374)

- Standard values enum entry for v2.6 (vecto/vecto!376)

- Add Driving Actions for IEPC gearshift (vecto/vecto!377)

- ReEngage1C tolerance in AT (vecto/vecto!379)

- Take battery limit into account for EM overload - REESS Empty  (vecto/vecto!380)

- Of v2.7, allow only IMC, H2-ICE conventional, FCHV Lorries (vecto/vecto!384)

- Disable reading data from external csv (vecto/vecto!385)

- OVC results (vecto/vecto!387)

- Ignore FCHV pre-run in best deltaSoC calculation (vecto/vecto!388)


## Drop

- MaxChargingPower requirement for OVC in v2.7 (vecto/vecto!358)




