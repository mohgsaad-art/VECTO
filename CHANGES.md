# Changelog



**VECTO v5.0.0-RC (05-06-2025)**


- Features

    * CodeEU n.a.: Readers for v2.7 vehicle XSD, and support for fuel cell vehicles. (vecto/vecto!341)
    * CodeEU n.a.: Read monitoring data from job (vecto/vecto!345)
    * CodeEU #1002: 3rd amendment mrf cif xml schemas (vecto/vecto!340)
    * CodeEU n.a.: MRF v1.0 vehicle (lorries and FCHV primary buses) writers (vecto/vecto!354)
    * CodeEU n.a.: CIF v1.0 vehicle part (v2.4 vehicles and v2.7 lorries) (vecto/vecto!355)
    * CodeEU n.a.: Use monitoring data from job to write report (vecto/vecto!360)
    * CodeEU n.a.: In motion charging postprocessing (vecto/vecto!344)
    * CodeEU n.a.: Disable (for RC & official) v27 vehicles except H2-ICE & FCHV lorries (vecto/vecto!375)
    * CodeEU n.a.: Readers for v2.7 buses, improved reader tests. (vecto/vecto!382)
    * CodeEU n.a.: HEV - Get Best dSOC in vsum (vecto/vecto!383)
    * CodeEU 968: Forbid AT upshift for reduced dt before brake (vecto/vecto!381)
    * CodeEU n.a.: New (v2.7) XSD for vehicles (vecto/vecto!334)
    * CodeEU n.a.: Partial implementation for new vehicle battery (vecto/vecto!337)
    * CodeEU n.a.: EM-IEPC Thermal Derating - Tq_max and Buffer Mods - Post VECTO-4.3.4 Feed-Back (vecto/vecto!378)

- Bug Fixes

    * CodeEU n.a.: Non-https link in manual (vecto/vecto!339)
    * CodeEU n.a.: V2.7 reader & XSD (vecto/vecto!342)
    * CodeEU n.a.: Updated XSLT file and hashing code for new vehicles and components. (vecto/vecto!338)
    * CodeEU #1007: Lock StoredResults list before accessing it to avoid race condition (vecto/vecto!343)
    * CodeEU n.a.: V27 vehicle issues (vecto/vecto!346)
    * CodeEU n.a.: 882 merge artifacts (vecto/vecto!347)
    * CodeEU n.a.: FCHV angledrive input (vecto/vecto!348)
    * CodeEU n.a.: Modify schema so that results can be written compatible with results for 2nd amendment: (vecto/vecto!349)
    * CodeEU n.a.: Restore deleted code in monitoring report (vecto/vecto!350)
    * CodeEU n.a.: Typo in MRF Inject module (vecto/vecto!351)
    * CodeEU n.a.: Proper namespace for VIF IEPC sub-element (vecto/vecto!352)
    * CodeEU n.a.: Mockup tests run successfully (vecto/vecto!353)
    * CodeEU #1002: Correcting errors in XML schema (and sample files): no engine output in... (vecto/vecto!356)
    * CodeEU n.a.: Bugfixes/updates for the Monitoring report and testing via the MockupTests. (vecto/vecto!357)
    * CodeEU n.a.: Replace U+2013 by regular dashes (vecto/vecto!359)
    * CodeEU n.a.: Added missing IMC testdata (vecto/vecto!361)
    * CodeEU n.a.: Retarder compulsory in all MRF vehicle components. (vecto/vecto!362)
    * CodeEU n.a.: Add further condition to decide which results to write in case the input data is a Multistep bus (vecto/vecto!363)
    * CodeEU n.a.: Check Articulated in json vehicle (vecto/vecto!365)
    * CodeEU n.a.: Segment in Bus AirDrag data creation (vecto/vecto!364)
    * CodeEU n.a.: FCHV pre-run execution (vecto/vecto!366)
    * CodeEU n.a.: Write ZeroCO2EmissionsRange and HydrogenRange to H2-ICE reports. (vecto/vecto!367)
    * CodeEU n.a.: Remove wrong angledrive restrictions (vecto/vecto!369)
    * CodeEU n.a.: Set vectorundata in completed bus results, (vecto/vecto!368)
    * CodeEU n.a.: Simulate OVC for FCHVs (vecto/vecto!370)
    * CodeEU n.a.: FCHV H2 range in reports (vecto/vecto!371)
    * CodeEU n.a.: Architecture in some MRF v1.0 tests (vecto/vecto!372)
    * CodeEU n.a.: H2 properties check in exempted vehicle input (vecto/vecto!373)
    * CodeEU n.a.: FCHV input classes inheritance (vecto/vecto!374)
    * CodeEU n.a.: Standard values enum entry for v2.6 (vecto/vecto!376)
    * CodeEU n.a.: Add Driving Actions for IEPC gearshift (vecto/vecto!377)
    * CodeEU n.a.: Of v2.7, allow only IMC, H2-ICE conventional, FCHV Lorries (vecto/vecto!384)
    * CodeEU n.a.: Disable reading data from external csv (vecto/vecto!385)
    * CodeEU n.a.: OVC results (vecto/vecto!387)
    * CodeEU n.a.: Ignore FCHV pre-run in best deltaSoC calculation (vecto/vecto!388)
    * CodeEU 972: Take battery limit into account for EM overload - REESS Empty (vecto/vecto!380)
    * CodeEU 994: ReEngage1C tolerance in AT (vecto/vecto!379)
    * CodeEU 886, 888, 889: Add Driving Actions for IEPC gearshift (vecto/vecto!377)
    * CodeEU n.a.: Updated Monitoring Report XSD (FCHV, Multiple powertrains) (vecto/vecto!335)

- Drop

    * CodeEU n.a.: MaxChargingPower requirement for OVC in v2.7 (vecto/vecto!358)



**VECTO 0.11.4-DEV (02.04.2025)**

* Features
  - CodeEU #855: IEPC with multiple load curves (!321)
  - H2 ICE vehicles in declaration mode (!325)

* Fixes
  - Add max-windows-size to fuel cell (!312)
  - Electric system power demand compensation for FCS (!313)
  - Add error message for unknown completed vehicle missions (!330)
  - Segment lookup method (!331)
  - FullLoadCurves proper initialization (!332)
  - Manage 'GetTruckSegment' exception behavior (!333)



**VECTO v4.3.3 Official Release (04-03-2025)**


- Bug Fixes

    * CodeEU #950: Check if XML element is signed (#950) (vecto/vecto!314)
    * CodeEU #954: NgTankSystem optional for HEV lorries MRF XSD (vecto/vecto!315)
    * fix: monitoring report for dual fuel vehicles (vecto/vecto!318)
    * fix: secure XML loading against external entity injection (vecto/vecto!319)
    * fix: correct interim supercap reader type (vecto/vecto!320)
    * fix: parameter IDs for XSDs v2.3 and v2.6 (vecto/vecto!322)
    
- Documentation

    * CodeEU n.a.: Update XSD parameter IDs documentation (vecto/vecto!327)



**VECTO v4.3.3 Official Release (03-03-2025)**


- Bug Fixes

    * CodeEU #950: Check if XML element is signed (#950) (vecto/vecto!314)
    * CodeEU #954: NgTankSystem optional for HEV lorries MRF XSD (vecto/vecto!315)
    * fix: monitoring report for dual fuel vehicles (vecto/vecto!318)
    * fix: secure XML loading against external entity injection (vecto/vecto!319)
    * fix: correct interim supercap reader type (vecto/vecto!320)
    * fix: parameter IDs for XSDs v2.3 and v2.6 (vecto/vecto!322)



**VECTO v4.3.2-RC (06-02-2025)**

- Features

    * CodeEU #872: New battery and supercap readers (vecto/vecto!309)

- Bug Fixes

    * CodeEU #861, #832, #880: ATShiftStrategyOptimized - No UpshiftFomL if not locked (vecto/vecto!301)
    * CodeEU #883: Forbid downshift to locked gear in APT-S if it generates direct upshift condition (vecto/vecto!307)
    * CodeEU #890: Add condition to write BusAuxiliaries output data in vsum (vecto/vecto!306)
    * Allow old XMLs for battery and supercap in development only (vecto/vecto!311)
    * Correct binding for supercap input data class; use correct xml data type in xml component reader for supercap (vecto/vecto!310)




**VECTO v4.2.7 Official Release (09-01-2025)**

* Bug Fixes
    - Track release_notes.md for release
    - CodeEU #858: Convert steering pump tech (vecto/vecto!303)
    - Converter Tool: ngTankSystem for dual fuel (vecto/vecto!304)
    - CodeEU #836: Restrictions on IEPC gear and MaxTorqueCurve XSD attributes (vecto/vecto!302)



**VECTO v4.2.6-RC (06-12-2024)**


- Features

    * Support Gitlab issue pattern (vecto/vecto!272)
    * CodeEU #854: Verify primary bus VIF hash against Job (vecto/vecto!295)
    * CodeEU #833, #834, #835, #836, #837: Add 3rd amendment XSD definitions (vecto/vecto!291)
    * CodeEU #838: Include Engine into v2.6 (vecto/vecto!297)

- Bug Fixes

    * Homogenize versions across tools (vecto/vecto!270)
    * CodeEU #807: Produce same data from ADC loss map (#807) (vecto/vecto!278)
    * Authors and readme metadata content (vecto/vecto!277)
    * CodeEU #809: Writing engine information in MRF (#809) (vecto/vecto!275)
    * CodeEU #842, #840, #841, #839, #798: Driver model: in case of an APT vehicle where the driving action is Brake... (vecto/vecto!287)
    * CodeEU #812, #788: Conversion of doubles for SI (vecto/vecto!283)
    * CodeEU #750, #758, #769, #816, #821, #829: Avoid wrong upshift and downshift for light SMT vehicles (vecto/vecto!290)
    * CodeEU #844, #705, #530: During a coasting action (look-ahead coasting) a gear hunting occurs in the... (vecto/vecto!289)
    * CodeEU #784: Override DoWriteModalResult for VTP (vecto/vecto!294)
    * CodeEU #495, #642, #739: Add SMT downshift condition - DroppedSpd>DisengSpd (vecto/vecto!293)


**VECTO v4.2.5 Official Release (02-10-2024)**

- Hot Fixes
  - Missing Build.props DefineConstants (!268)
  - Version 4th number read from Build.props (!269)

**VECTO v4.2.3 Official Release (01-10-2024)**

- Enhancements
  - CodeEU #799: Adapt VECTO for the new CI updates (!263)

- Bug Fixes
  - CodeEU #794: Added missing monitoring report file (!265)
  - CodeEU #780: Update weights for bus subgroups (!264)

**VECTO-4.2.2-RC**

**Build 3539 (2024-09-09)**

- Bugfixes
   * CodeEU-710: Hashing tool check fail with VECTO version 3330
   * CodeEU-711: Hashing tool check fail with VECTO 4.1.3.3415
   * CodeEU-712: VECTO VTP error
   * CodeEU-754: "Failed to find operating point"; "Failed to find mechanic power for given electric power" in E2 vehicle
   * CodeEU-727: Failure in simulating HEV in different VECTO versions
   * CodeEU-749: Double summary for electric vehicles
   * CodeEU-542: IVECO confidential : BUG REPORT : CRW LE T7D VOITH NXT 5.63
   * CodeEU-663: IHPC: Failed to find operating point
   * CodeEU-634: Article 10(2) issue - VIN YS2G6X20002202570
   * CodeEU-671: IHPC: simulation abort due to unexpected response

**VECTO-4.2.1**

**Build 3469 (2024-07-01)**

- Features
   * CodeEU-726: Build an XML converter tool for older VECTO jobs

- Bugfixes
   * CodeEU-719: the six new tyre dimensions from line 126 onwards to the latest “wheels.csv” file in the VECTO repository
   * CodeEU-717: VECTO-4.2.0.3448-RC - Buses AMT Gearbox Type with 1% higher C02 in primary results
   * CodeEU-724: Error in Primary Bus Simulation: Object reference not set to an instance of an object
   * CodeEU-716: VECTO-4.2.0.3448-RC - Buses Result Summary section missing in RLST_Customer.xml
   * CodeEU-694: Primary and Completed heavybus FCV article 9 exempted hashcode mismatch.
   * CodeEU-735: SMT strategy different between engineering and declaration mode
   * CodeEU-736: Existing customer reports (CIF) fail validation
   * CodeEU-737: Missing data from XML report

**VECTO-4.2.0-RC**

**Build 3448 (2024-06-10)**

- Features
   * CodeEU-697: Re-evaluate subgroup allocations for Long Haul
   * CodeEU-696: Double summary in CIF for vocationals and non vocational missions.
   * CodeEU-698: Incorporate missions RD, LH and EMS to class 16 vehicles
   * CodeEU-676: Feature: Implement monitoring report

- Bugfixes
   * CodeEU-471: VectoSimulationException: VF640J869RB022573
   * CodeEU-462: Article10-2-issue | Order-Nr 28206354 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-326: Article10-2-issue | Order-Nr 28195581 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-348: Article10-2-issue | Order-Nr 28204519 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-457: Article10-2-issue | Order-Nr 28174000 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-458: Article10-2-issue | Order-Nr 28186528 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-459: Article10-2-issue | Order-Nr 28203057 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-545: IVECO confidential : BUG REPORT : HEV-P1 : UW18m C9 VOITH NXT CRU 48V mild hybrid
   * CodeEU-346: Article10-2-issue | Order-Nr 28202338 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-352: Article10-2-issue | Order-Nr 28202130 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-394: Article10-2-issue | Order-Nr 28204065 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-433: Article10-2-issue | Order-Nr 28192321 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-451: Article10-2-issue | Order-Nr 28204280 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-452: Article10-2-issue | Order-Nr 28197394 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-453: Article10-2-issue | Order-Nr 28199435 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-454: Article10-2-issue | Order-Nr 28206982 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-655: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28208126 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-660: Retarder lossmap error in completed vehicle simulation
   * CodeEU-662: Generic retarder map speed range insufficient in some cases
   * CodeEU-648: Error in Multistep Tool PEV/P-HEV
   * CodeEU-618: PEV vehicles simulation error depending the time format
   * CodeEU-700: Factor Method Generic IHPC Powermap De-normaization bug
   * CodeEU-482: Article10-2-issue | Order-Nr 28203040 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-514: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28208051 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-529: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28199994 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-544: IVECO confidential : BUG REPORT : HEV-P1 : CRW LE C9 VOITH NXT CRU 48V mild hybrid
   * CodeEU-552: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28201759 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-556: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28208176 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-557: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28209751 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-622: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28210594 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-632: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28210591 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-672: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28208841 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-673: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28209179 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-674: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28211540 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-678: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28192673 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-697: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28209551 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-685: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28200286 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-686: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28201178 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-687: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28209679 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-692: Article10-2-issue | VECTO-4.1.3 | Order-Nr 28209789 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses

**VECTO-4.1.3**

***Build 3415 (2024-05-08)***

- Hotfixes
   * CodeEU-638: Incorrect construction payloads for group 9 non-vocational vehicle

**VECTO-4.1.1**

***Build 3413 (2024-05-06)***

- Bugfixes
   * CodeEU-615: Multistep freezes after loading VIF chassis
   * CodeEU-616: Multistep tool freezes
   * CodeEU-619: restore wrong exempted techs in XSD for backwards compatibility
   * CodeEU-617: Results change depending the time format
   * CodeEU-635: Revert multiple summary in CIF

**VECTO-4.1.0-RC**

***Build 3392 (2024-04-15)***

- Features
   * CodeEU-577: Add missing mission profiles to vocational and non-vocational

- Bugfixes
   * CodeEU-331: Gear 1 DrivingActionAccelerate: Fail
   * CodeEU-367: Gear 1 DrivingActionAccelerate: Fail
   * CodeEU-372: ADT Error on Bus Category Primary Vehicle Simulation on VECTO
   * CodeEU-373: ADT Error on Bus Category Primary Vehicle Simulation on VECTO
   * CodeEU-374: ADT Error on Bus Category Primary Vehicle Simulation on VECTO
   * CodeEU-375: ADT Error on Bus Category Primary Vehicle Simulation on VECTO
   * CodeEU-393: Finished Run VEH-PrimaryBus_nonSmart Interurban _P32SD_ReferenceLoad with ERROR: 16
   * CodeEU-439: 615 (Interurban _P32DD_ReferenceLoad) - absTime: 7129.6359 [s], distance: 53875.9765 [m], dt: 0.6388 [s], v: 0.5098 [m/s], Gear: 1 | DrivingActionAccelerate: Failed
   * CodeEU-446: Finished Run VEH-PrimaryBus_nonSmart Interurban _P32SD_ReferenceLoad with ERROR: 16
   * CodeEU-449: Finished Run VEH-PrimaryBus_nonSmart Interurban _P32SD_ReferenceLoad with ERROR: 16
   * CodeEU-478: Finished Run VEH-PrimaryBus_nonSmart Urban _P31SD_ReferenceLoad with ERROR: 4 (Urban _P31SD_ReferenceLoad) - absTime: 8606.6241 [s], distance: 39112.5127 [m], dt: 1.1131 [s], v: 0.1833 [m/s], Gear: 1 | DrivingActionAccelerate: Failed to find operating poi
   * CodeEU-481: Finished Run VEH-PrimaryBus_nonSmart Urban _P31SD_ReferenceLoad with ERROR: 4 (Urban _P31SD_ReferenceLoad) - absTime: 8606.6241 [s], distance: 39112.5127 [m], dt: 1.1131 [s], v: 0.1833 [m/s], Gear: 1 |
   * CodeEU-488: Finished Run VEH-PrimaryBus_nonSmart Urban _P31SD_ReferenceLoad with ERROR: 4 (Urban _P31SD_ReferenceLoad) - absTime: 8606.6241 [s], distance: 39112.5127 [m], dt: 1.1131 [s], v: 0.1833 [m/s], Gear: 1 | DrivingActionAccelerate: Failed to find operating poi
   * CodeEU-494: Finished Run VEH-PrimaryBus_nonSmart Urban _P31SD_ReferenceLoad with ERROR: 26 (Urban _P31SD_ReferenceLoad) - absTime: 8606.6241 [s], distance: 39112.5127 [m], dt: 1.1131 [s], v: 0.1833 [m/s], Gear: 1 | DrivingActionAccelerate: Failed to fi
   * CodeEU-420: Finished Run VEH-PrimaryBus_nonSmart Interurban _P32SD_ReferenceLoad with ERROR: 16
   * CodeEU-573: Unhandled powertrain architecture Conventional Vehicle to calculate gradability
   * CodeEU-582: unhandled powertrain architecture ConventionalVehicle to calculate gradability, 11:44:36.63,
   * CodeEU-595: Inconsistency exempted vehicles "Fuel cell vehicle" vs. "FCV Article 9 exempted"
   * CodeEU-501: Help regarding VTP vdri format is not up to date in v4
   * CodeEU-594: inadequate validation for auxiliaries in completed vehicle XML
   * CodeEU-580: Potential error in XML schema v2.4 (exempted vehicles)
   * CodeEU-551: Article-10-2 XLRASF5E00G419905
   * CodeEU-547: Max ICE Off timespan for buses
   * CodeEU-583: Incorrect internal resistance for SuperCap used in factor method
   * CodeEU-571: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28202896 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-526: VTP calculation aborted on fuel consumption map
   * CodeEU-502: Error in VECTO calculation WMA10CZZ0RF022326
   * CodeEU-401: Electric steering system with conventional vehicle not according to 2017/2400
   * CodeEU-476: Setting Initial SoC in REESS editor leads to error
   * CodeEU-533: Question about heavy lorry_IEPC_Gb×4speed: Failed to generate electric power map - at least two negative entries are required
   * CodeEU-456: Clarification Documentation official Results
   * CodeEU-506: Article 10(2) issue - VIN YS2P6X200R2201285
   * CodeEU-507: Article 10(2) issue - VIN YS2P6X20005732399
   * CodeEU-508: Article 10(2) issue - VIN S2P6X20005734199
   * CodeEU-499: Object Reference not set Error
   * CodeEU-546: Problem with P181 (Cooling Fan Technology) for a conventional vehicle
   * CodeEU-516: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28208044 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-517: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28208037 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-519: Article10-2-issue | VECTO-4.0.3 | Order-Nr 10098181 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-527: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28196233 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-528: Article10-2-issue | VECTO-4.0.3 | Order-Nr 28196652 | HEV P1 Error Mercedes-Benz and Setra Hybrid Buses
   * CodeEU-338: Eco-roll only, without engine stop, impact on CO2 emission
   * CodeEU-475: XML files loading in VTP Job not OK; No auxiliary data and torque curve visible, Job can't be saved in VECTO 4.0.3.3330
   * CodeEU-610: VTP mode is broken due to changes in the Clutch component.

**VECTO-4.0.3**

***Build 3330 (2024-02-13)***

- Bugfixes
    * CodeEU-293: DistanceRun got an unexpected response
    * CodeEU-298: Object reference not set to an instance of an object
    * CodeEU-300: Full drive torque miscalculation
    * CodeEU-336: Feature: Hashing tool must validate the previous step data for multi-step jobs
    * CodeEU-337: Electric Citybus - ERROR with "31a-Specific" bus configuration
    * CodeEU-343: Cannot simulate Primary Vehicle using vectocmd.exe
    * CodeEU-387: VECTO sometimes fails to properly read Tyre data from primary vehicle xml
    * CodeEU-427: Vehicle speed resulting to exceeding max gearbox speed
    * CodeEU-428: Remove speed safety margin for gearbox re-engaging
    * CodeEU-438: Mismatch XML schema vs Regulation (exempted vehicles)
    * CodeEU-248: Bus P2 hybrid VECTO error in urban cycle
    * CodeEU-278: Simulation crash when writing fuel consumption results to reports
    * CodeEU-284: New error message and failed simulation obtained for P2 hybrid buses
    * CodeEU-285: PEV vehicle error in routine to write the results with new vecto version  4.0.2.3275
    * CodeEU-287: PEV_IEPC error message:"can only operate on SI Objects with the same unit"
    * CodeEU-289: VECTO Simulation Error for bus with validated input data

**VECTO-4.0.2**

***Build 3275 (2023-12-20)***

- Hotfix
    * CodeEU-273, CodeEU-274: Changes in the AMT shift strategy regarding idling speed caused simulation aborts
    * CodeEU-260: regression fix handling overload buffer

***Build 3273 (2023-12-18)***

- Bugfixes
    * CodeEU-94: DistanceRun got an unexpected response
    * CodeEU-153: DrivingActionAccelerate: Failed to find operating point after Overload
    * CodeEU-158: Urban RefLoad DrivingActionAccelerate: Failed to find operating point (IEPC Wheelhub 1 measured)
    * CodeEU-168: Vecto Declaration Simulation with P1-Hybrid shows multiple errors - Simulation Aborts!
    * CodeEU-191: Boosting limits HEV ovc are not working
    * CodeEU-202: EMS Standard Values: Continuous/Overload Torque 0 Nm
    * CodeEU-203: HybridStrategy error for IHPC type 1 hybrid lorry
    * CodeEU-206: VTP + PEMS test done by Renault Trucks France - VECTO tool errors most probably linked to automatic gearbox (with torque converter)
    * CodeEU-211: Hybrid P1 configurations with errors
    * CodeEU-215: MultiStep tool help
    * CodeEU-216: NrOfGears in CIF is 1 for IEPC no matter how many gears it has
    * CodeEU-220: Error manual transmission 2. gear
    * CodeEU-224: Issue with PEV complete vehicle simulation (Vecto MultiStage)
    * CodeEU-231: IVECO CONFIDENTIAL : hybrid buses completed simulation aborted
    * CodeEU-234: Error Conventional Lorry Gear: 1C DistanceRun got an unexpected response
    * CodeEU-235: Tyre error calculation buses "invalid xsi:type 'TyreDataDeclarationType'"
    * CodeEU-238: Bus VIF files not valid in Multistep VECTO
    * CodeEU-243: Simulation aborted: Gear 5 Lossmap not sufficiant
    * CodeEU-244: Signature validation fails for old (prior to v4) Manufacturer reports
    * CodeEU-246: VTP report generation problems (failing to read some data from MRF)
    * CodeEU-249: Fix handling gear torque limits in case of IEPC WheelHub motor and only one side is measured
    * CodeEU-250: Maximum vehicle speed exceeded during pre-processing
    * CodeEU-253: Replace VTP HeavyBus in Generic Vehicles with VTP Truck
    * CodeEU-259: Inconsistent calculation of AverageRRC in CIF
    * CodeEU-260: Handling of overload buffer in case Continuous Torque is 0Nm
    * CodeEU-261: SuperCap internal resistance standard values correction
    * CodeEU-263: Determination of rated power for IEPCs for MRF and CIF
    * CodeEU-266: Fix Measured Speed Testcases


**VECTO-4.0.1**

***Build 3217 (2023-10-23) OFFICIAL RELEASE***

- Improvements
    * VTP mode: create zip archive for input and output files
- Bugfixes
    * bugfix in CIF for complete(d) buses - do not write PrimaryVehicleSubgroup element
    * fix hybrid strategy: power comparison
    * fix exempted vehicles do not work (error message that XML version is not supported)
    * bugfix in XML schema: remove wrong PS technology entry for lorries
    * bugfix for conditioning power demand for IHPC vehicles

**VECTO-4.0.0**

***Build 3211 (2023-10-16) OFFICIAL RELEASE***

- First official VECTO release for the 2nd amendment of Regulation (EU) 2017/2400
- Features
   - Declaration Mode simulation
       - xEV heavy lorries
       - Conventional medium lorries
       - xEV medium lorries
       - conventional buses (primary and complete(d))
       - xEV buses (primary and complete(d))
    - Dedicated user interface for simulating buses in declaration mode using the multistep tool
    - Updated XML reports (MRF, CIF, VIF)
    - Updated simulation output (.vsum, .vmod)
    - XML job files in version below 2.4 are no longer supported
       - XML component data is still supported in all XML versions
    - Dropped support for .NET Framework 4.5 (EOL 04/2022)
    - Engineering mode simulation of xEV vehicles



***Build 3078 (2023-06-06) RELEASE CANDIDATE***

- First fully functional tool version according to the provisions of the 2nd amendment of Regulation (EU) 2017/2400.  
- Changes
   - Dropped support for .Net Framework 4.5 (EOL 04/2022)
   - Multi-target build. Supported .Net versions: .Net Framework 4.8, .Net 6.0
   - Implementation of Declaration-Mode for xEV-Lorries (see release notes)
   - Implementation of Declaration-Mode for xEV-Buses (see release notes)
   - New generic vehicles (XML)
- Known issues
   - Elements not yet implemented
       - Battery connectors / junction box not included define and implement generic additional resistances (i.e. loss factors)
       - Technical elements as resulting from the revision of the CO2 Standards to be added
          - Sub-group allocation for the for the newly covered vehicle groups
          - Generation of weighted results for vocational vehicles
          - Anything related to ZEV definition?
   - Elements still under discussion
      - Medium lorries mission profile and payload weighting factors (equally weighted, only preliminary)
      - Multiple SOC level(s) for VECTO PEV simulation and respective weighting of results (?)


**VECTO-3.3.10**

***Build 2401 (2021-07-29) OFFICIAL RELEASE***

- Bugfixes (compared to 3.3.10.2373)
    * *No additional bugfixes*

***Build 2373 (2021-07-01) RELEASE CANDIDATE***

- Improvements
    * [VECTO-1421] – Added vehicle sub-group (CO2-standards to MRF and CIF)
    * [VECTO 1449] – Handling of exempted vehicles: See next slide for details
    * [VECTO-1404] – Corrected URL for CSS in MRF and CIF
- Bugfixes
    * [VECTO-1419] – Simulation abort in urban cycle: failed to find operating point on search braking power with TC gear
    * [VECTO-1439] – Bugfix handling duplicate entries in engine full-load curve when intersecting with max-torque of gearbox
    * [VECTO-1429] – error in XML schema 2.x for exempted vehicles – MaxNetPower1/2 are optional input parameters

***Handling of exempted vehicles***

- Axle configuration and sleeper cab are optional input parameters for exempted vehicles (XML schema 1.0 and 2.2.1).
    * OEMs are recommended to provide these parameters for exempted vehicles.
    * If the axle configuration is provided as input parameter, the MRF contains the vehicle group.
    * The sleeper cab input parameter is also part of the MRF if provided as input.
- Input parameters MaxNetPower1/2 are optional input parameters for all exempted vehicles.
    * If provided in the input these parameters are part of the MRF for all exempted vehicle types
    * It is recommended that those parameters are used to specify the rated power also for PEV (pure electric vehicles)


**VECTO-3.3.9**

***Build 2175 (2020-12-15) OFFICIAL RELEASE***

- Bugfixes (compared to 3.3.9.2147)
    * [VECTO-1374] - VECTO VTP error - regression update

***Build 2147 (2020-11-17) RELEASE CANDIDATE***

- Bugfixes
    * [VECTO-1331] - VTP Mode does not function for vehicles of group 3
    * [VECTO-1355] - VTP Simulation Abort
    * [VECTO-1356] - PTO Losses not considered in VTP simulation
    * [VECTO-1361] - Torque Converter in use for the First and Second Gear VTP file does not allow for this
    * [VECTO-1372] - Deviation of CdxA Input vs. Output for HDV16
    * [VECTO-1374] - VECTO VTP error

- Improvements
    * [VECTO-1360] - make unit tests execute in parallel


**VECTO-3.3.8**

***Build 2052 (2020-08-14) OFFICIAL RELEASE***

- Bugfixes (compared to 3.3.8.2024)
    * *No additional bugfixes*

***Build 2024 (2020-07-17) RELEASE CANDIDATE***

- Bugfixes
    * [VECTO-1288] - Simulation Abort UD RL
    * [VECTO-1327] - Simulation abort Construction RefLoad: unexpected response ResponseOverload
    * [VECTO-1266] - Gear 4 Loss-Map was extrapolated


**VECTO 3.3.7**

***Build 1964 (2020-05-18) OFFICIAL RELEASE***

- Bugfixes
    * [VECTO-1254] - Hashing method does not ignore certain XML attributes
    * [VECTO-1259] - Mission profile weighting factors for vehicles of group 16 are not correct


**VECTO 3.3.6**

***Build 1916 (2020-03-31) OFFICIAL RELEASE***

- Bugfixes
    * [VECTO-1250] - Error creating new gearbox file from scratch

***Build 1898 (2020-03-13) RELEASE CANDIDATE***

- Improvement
    * [VECTO-1239] - Adaptation of Mission Profile Weighting Factors
    * [VECTO-1241] - Engineering mode: Adding support for additional PTO activations

- Bugfixes
    * [VECTO-1243] - Bug in VTP mode for heavy lorries
    * [VECTO-1234] - urban cycle at reference load not running for bug when find braking operating point


**VECTO 3.3.5**

***Build 1812 (2019-12-18) OFFICIAL RELEASE***

- Bugfixes
    * [VECTO-1220] - Simulation Abort Urban Delivery RefLoad

***Build 1783 (2019-11-19) RELEASE CANDIDATE***

- Improvement
    * [VECTO-1194] - Handling input parameter 'vocational' for groups other than 4, 5, 9, 10
    * [VECTO-1147] - Updating declaration mode cycles values in user manual
    * [VECTO-1207] - run VECTO in 64bit mode by default

- Bugfixes
    * [VECTO-1074] - Vecto Calculation Aborts with Interpolation Error
    * [VECTO-1159] - Simulation Abort in UrbanDelivery LowLoading
    * [VECTO-1189] - Error in delaunay triangulation invariant violated
    * [VECTO-1209] - Unexpected Response Response Overload
    * [VECTO-1211] - Simulation Abort Urban Delivery Ref Load
    * [VECTO-1214] - Validation of input data fails when gearbox speed limits are applied


**VECTO 3.3.4**

***Build 1716 (2019-09-13) OFFICIAL RELEASE***

- Bugfixes
    * [VECTO-1074] - Vecto Calculation Aborts with Interpolation Error ([VECTO-1046])
    * [VECTO-1111] - Simulation Abort in Municipal Reference Load


***Build 1686 (2019-08-14) RELEASE CANDIDATE***

- Improvement
    * [VECTO-1042] - Add option to write results into a certain directory
    * [VECTO-1064] - add weighting factors for vehicle groups 1, 2, 3, 11, 12, 16

- Bugfixes
    * [VECTO-1030] - Exceeded max iterations when searching for operating point! Failed to find operating point!
    * [VECTO-1032] - Gear 5 LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient
    * [VECTO-1067] - Vair and Beta correction for Aerodynamics
    * [VECTO-1000] - Error Loss-Map extrapolation in Declaration Mode
    * [VECTO-1040] - Gear 6 LossMap data was extrapolated in Declaration Mode
    * [VECTO-1047] - Failed to find operating point on construction cycle, ref load, AT gearbox


**VECTO 3.3.3**

***Build 1639 (2019-06-27) OFFICIAL RELEASE***

- Bugfixes (compared to VECTO 3.3.3.1609-RC)
    * [VECTO-1003] - Vecto Error: Loss-Map extrapolation in declaration mode required (issue VECTO-991)
    * [VECTO-1006] - Failed to find torque converter operating point on UD cycle (issue VECTO-996)
    * [VECTO-1010] - Unexpected Response: ResponseOverload in UD cycle (issue VECTO-996)
    * [VECTO-1015] - XML Schema not correctly identified
    * [VECTO-1019] - Error opening job in case a file is missing
    * [VECTO-1020] - HashingTool Crashes
    * [VECTO-1021] - Invalid hash of job data


***Build 1609 (2019-05-29) RELEASE CANDIDATE***

 - Improvement
    * [VECTO-916] - Adding new tyre sizes
    * [VECTO-946] - Refactoring XML reading
    * [VECTO-965] - Add input fields for ADAS into VECTO GUI
    * [VECTO-966] - Allow selecting Tank System for NG engines in GUI
    * [VECTO-932] - Consistency in NA values in the vsum file

 - Bugfixes
    * [VECTO-954] - Failed to find operating point for braking power (Fix for Notification Art. 10(2) - [VECTO-952])
    * [VECTO-979] - VECTO Simulation abort with 8-speed MT transmission (Fix for Notification Art. 10(2) - [VECTO-978])
    * [VECTO-931] - AT error in VECTO version 3.3.2.1519
    * [VECTO-950] - Error when loading Engine Full-load curve
    * [VECTO-967] - Engine-Only mode: Engine Torque reported in .vmod does not match the provided cycle
    * [VECTO-980] - Error during simulation run


**VECTO 3.3.2**

***Build 1548 (2019-03-29) OFFICIAL RELEASE***


 - Bugfixes
    * [VECTO-861] - 3.3.1: Torque converter not working correctly
    * [VECTO-904] - Range for gear loss map not sufficient.
    * [VECTO-909] - 3.3.2.1519: Problems running more than one input .xml
    * [VECTO-917] - TargetVelocity (0.0000) and VehicleVelocity (>0) must be zero when vehicle is halting
    * [VECTO-918] - RegionalDeliveryEMS LowLoading - ResponseSpeedLimitExceeded
    * [VECTO-920] - Urban Delivery: Simulation Run Aborted, TargetVelocity and VehicleVelocity must be zero when vehicle is halting!


***Build 1519 (2019-03-01) RELEASE CANDIDATE***


Release Notes - VECTO: Vehicle Energy Calculation Tool - Version 3.3.2.1519-RC


 - Improvement
    * [VECTO-869] - change new vehicle input fields (ADAS, sleeper cab, etc.) to be mandatory
    * [VECTO-784] - Configuration file for VECTO log files
    * [VECTO-865] - Extend Sum-Data
    * [VECTO-873] - Add digest value to SumData


 - Bugfixes
    * [VECTO-729] - Bugs APT submodel
    * [VECTO-787] - APT: DrivingAction Accelerate after Overload
    * [VECTO-789] - APT: ResponseUnderload
    * [VECTO-797] - VECTO abort with AT transmission and TC table value
    * [VECTO-798] - VECTO abort with certified AT transmission data and certified TC data
    * [VECTO-807] - VECTO errors in vehicle class 1/2/3
    * [VECTO-827] - Torque converter inertia
    * [VECTO-838] - APT: ResponseOverload
    * [VECTO-843] - AT Transmissions problem on VECTO 3.3.1.1463
    * [VECTO-844] - Error with AT gearbox model
    * [VECTO-847] - Simulation abort due to error in NLog?
    * [VECTO-848] - AT Gearbox Simulation abort (was: Problem related to Tyres?)
    * [VECTO-858] - Urban Delivery Abort - with APT-S Transmission and TC
    * [VECTO-861] - 3.3.1: Torque converter not working correctly
    * [VECTO-872] - MRF/CIF: Torque Converter certification method and certification number not correctly set
    * [VECTO-879] - SIMULATION RUN ABORTED DistanceRun got an unexpected response
    * [VECTO-883] - Traction interruption may be too long
    * [VECTO-815] - Unexpected Response: SpeedLimitExceeded
    * [VECTO-816] - object reference not set to an instance of an object
    * [VECTO-817] - TargetVelocity and VehicleVelocity must not be 0
    * [VECTO-820] - DistanceRun got an unexpected response: ResponseSpeedLimitExceeded
    * [VECTO-864] - Prevent VECTO loss-map extension to result in negative torque loss


**VECTO 3.3.1**

***Build 1492 (2019-02-01) OFFICIAL RELEASE***

 - Bugfixes (compared to 3.3.1.1463)
    * [VECTO-845] - Fixing bug for VECTO-840
    * [VECTO-826] - DistanceRun got an unexpected response: ResponseSpeedLimitExceeded
    * [VECTO-837] - VECTO GUI displays incorrect cycles prior to simulation
    * [VECTO-831] - Addition of indication to be added in Help and Release notes for simulations with LNG


***Build 1463 (2019-01-03) RELEASE CANDIDATE***

 - Changes according to 2017/2400 amendments
    * [VECTO-761] - Adaptation of input XML Schema
    * [VECTO-762] - Extension of Input Interfaces
    * [VECTO-763] - Extension of Segmentation Table
    * [VECTO-764] - ADAS benefits
    * [VECTO-766] - Update Powerdemand Auxiliaires
    * [VECTO-767] - Report for exempted vehicles
    * [VECTO-768] - VTP mode
    * [VECTO-770] - Fuel Types
    * [VECTO-771] - Handling of exempted vehicles
    * [VECTO-824] - Throw exception for certain combinations of exempted vehicle parameters
    * [VECTO-773] - Correction Factor for Reference Fuel
    * [VECTO-790] - Adapt generic data for construction/municipal utility
    * [VECTO-493] - Implementation of generic body weights and air drag values for construction cycle
    * [VECTO-565] - Consideration of LNG as possible fuel is missing

 - Changes/Improvements
    * [VECTO-799] - Remove TUG Logos from Simulation Tool, Hashing Tool
    * [VECTO-808] - Add Moitoring Report
    * [VECTO-754] - Extending Loss-Maps in case of AT gearbox for each gear, axlegear, gearbox
    * [VECTO-757] - Correct contact mail address in Hashing Tool
    * [VECTO-779] - Update Construction Cycle - shorter stop times
    * [VECTO-783] - Rename columns in segmentation table and GUI
    * [VECTO-709] - VTP editor from user manual not matching new VECTO one: updated documentation
    * [VECTO-785] - Handling of Vehicles that cannot reach the cycle's target speed: Limit max speed in driver model
    * [VECTO-716] - Validate data in Settings Tab: update documentation
    * [VECTO-793] - Inconsistency between GUI, Help and Regulation: update wording in GUI and user manual
    * [VECTO-796] - Adaptation of FuelProperties
    * [VECTO-806] - extend loss-maps (gbx, axl, angl) for MT and AMT transmissions
    * [VECTO-750] - Simulation error DrivingAction: adapt downshift rules for AT to drive over hill with 6% inclination

 - Bugfixes
    * [VECTO-819] - object reference not set to an instance of an object
    * [VECTO-818] - SearchOperatingPoint: Unknown response type. ResponseOverload
    * [VECTO-813] - Error "Infinity [] is not allowed for SI-Value"
    * [VECTO-769] - DrivingAction Brake: request failed after braking power was found.ResponseEngineSpeedTooHigh
    * [VECTO-804] - Error on simulation with VECTO 3.3.0.1433
    * [VECTO-805] - Total vehicle mass exceeds TPMLM
    * [VECTO-811] - AMT: ResponseGearShift
    * [VECTO-812] - AMT: ResponseOverload
    * [VECTO-822] - SIMULATION RUN ABORTED by Infinity
    * [VECTO-792] - Vecto Hashing Tool - error object reference not set to an instance of an object (overwriting Date element)
    * [VECTO-696] - Problem with Primary Retarder: regression update, set torque loss to 0 for 0 speed and engaged gear
    * [VECTO-776] - Decision Factor (DF)  field is emptied after each simulation
    * [VECTO-814] - Error: DistanceRun got an unexpected response: ResponseGearshift


**VECTO 3.3.0**

***Build 1433 (2018-12-03) OFFICIAL RELEASE***

- Bugfixes (compared to 3.3.0.1250)
    * [VECTO-723] - Simulation aborts with engine speed too high in RD cycle
    * [VECTO-724] - Simulation aborts with error 'EngineSpeedTooHigh' - duplicate of VECTO-744
    * [VECTO-728] - Simulation aborts when vehicle's max speed (n95h) is below the target speed
    * [VECTO-730] - Simulation Aborts with ResponseOverload
    * [VECTO-744] - ResponseEngineSpeedTooHigh (due to torque limits in gearbox)
    * [VECTO-731] - Case Mismatch - Torque Converter
    * [VECTO-711] - Elements without types in CIF and MRF
    * [VECTO-757] - Correct contact mail address in Hashing Tool
    * [VECTO-703] - PTO output in MRF file
    * [VECTO-713] - Manufacturer Information File in the legislation is not compatible with the Simulation results
    * [VECTO-704] - Allow VTP-simulations for AT gearboxes
- Changes (compared to 3.3.0.1398)
    * [VECTO-795] - VECTO Hashing Tool crashes
    * [VECTO-802] - Error in XML schema for manufacturer's record file


***Build 1398 (2018-10-30) RELEASE CANDIDATE***

- Bugfixes (since 3.3.0.1250)
    * [VECTO-723] - Simulation aborts with engine speed too high in RD cycle
    * [VECTO-724] - Simulation aborts with error 'EngineSpeedTooHigh' - duplicate of VECTO-744
    * [VECTO-728] - Simulation aborts when vehicle's max speed (n95h) is below the target speed
    * [VECTO-730] - Simulation Aborts with ResponseOverload
    * [VECTO-744] - ResponseEngineSpeedTooHigh (due to torque limits in gearbox)
    * [VECTO-731] - Case Mismatch - Torque Converter
    * [VECTO-711] - Elements without types in CIF and MRF
    * [VECTO-757] - Correct contact mail address in Hashing Tool
    * [VECTO-703] - PTO output in MRF file
    * [VECTO-713] - Manufacturer Information File in the legislation is not compatible with the Simulation results
    * [VECTO-704] - Allow VTP-simulations for AT gearboxes


***Build 1250 (2018-06-04)***


- Improvement
    * [VECTO-665] - Adding style information to XML Reports
    * [VECTO-669] - Group 1 vehicle comprises vehicles with gross vehicle weight > 7.5t
    * [VECTO-672] - Keep manual choice for "Validate data"
    * [VECTO-682] - VTP Simulation in declaration mode
    * [VECTO-652] - VTP: Check Cycle matches simulation mode
    * [VECTO-683] - VTP: Quality and plausibility checks for recorded data from VTP
    * [VECTO-685] - VTP Programming of standard VECTO VTP report
    * [VECTO-689] - Additional Tyre sizes
    * [VECTO-702] - Hashing tool: adapt warnings
    * [VECTO-667] - Removing NCV Correction Factor
    * [VECTO-679] - Engine n95h computation gives wrong (too high) engine speed (above measured FLD, n70h)
    * [VECTO-693] - extend vehicle performance in manufacturer record

- Bugfixes
    * [VECTO-656] - Distance computation in vsum
    * [VECTO-666] - CF_RegPer no effect in vehicle simulation -- added to the engine correction factors
    * [VECTO-687] - Saving a Engine-Only Job is not possible
    * [VECTO-695] - Bug in vectocmd.exe - process does not terminate
    * [VECTO-699] - Output in manufacturer report and customer report (VECTO) uses different units than described in legislation
    * [VECTO-700] - errorr in simulation with 0 stop time at the beginning of the cycle


**VECTO 3.2.1**

***Build 1133 (2018-02-07)***

- Improvement
    * [VECTO-634] - VTP Mode: specific fuel consumption

- Bugfixes
    * [VECTO-642] - VECTO BUG – secondary retarder losses: **IMPORTANT:** Fuel-consumption relevant bug! wrong calculation of retarder losses for retarder ratio not equal to 1
    * [VECTO-624] - Crash w/o comment: Infinite recursion
    * [VECTO-627] - Cannot open Engine-Only Job
    * [VECTO-629] - Vecto crashes without errror message (same issue as VECTO-624)
    * [VECTO-639] - Failed to find operating point for braking power: cycle with low target speed (3km/h). allow driving with slipping clutch
    * [VECTO-640] - Exceeded max. iterations: driving fully-loaded vehicle steep uphill. fixed by allowing full-stop and drive off again
    * [VECTO-633] - unable to start VTP Mode simulation
    * [VECTO-645] - Encountered error while validating Vecto output (generated by API) through Hashing tool for vehicle without retarder



***Build 1079 (2017-12-15)***

- Improvements
    * [VECTO-618] - Add Hash value of tyres to manufacturer's record file
    * [VECTO-590] - Handling of hash values: customer's record contains hash of manufacturer's record
    * [VECTO-612] - Continuously changing hashes: Info in GUI of HashingTool
    * [VECTO-560] - Change Mail-Address of general VECTO contact
    * [VECTO-616] - SI-Unit - display derived unit instead of base units

- Bugfixes
    * [VECTO-608] - Power balance in EPT-mode not closed
    * [VECTO-611] - Invalid input. Cannot cast Newtonsoft.Json.Linq.JObject to Newtonsoft.Json.Linq.JToken
    * [VECTO-610] - TyreCertificationNumber missing in Manufacturer Report
    * [VECTO-613] - Incomplete description of allowed values of LegislativeClass (p251) in VECTO parameter documentation
    * [VECTO-625] - Update XML Schema: Tyre dimensions according to Technicall Annex, trailing spaces in enums

- Support
    * [VECTO-615] - Error torque interpolation in declaration jobs exported to XML


***Build 1054 (2017-11-20)***

- Improvements
    + [VECTO-592] - NEW VTP Simulation Mode
    + [VECTO-605] - Improve simulation speed

- Bugfixes
    + [VECTO-602] - Error in simulation without airdrag component
    + [VECTO-589] - Scheme .xml error


**VECTO 3.2.0**

***Build 1022 (2017-10-19)***

- Bugfixes
    + [VECTO-585, VECTO-587] – VECTO Simulation aborts when run as WCF Service
    + [VECTO-586] – Gearshiftcout in reports too high
    + [VECTO-573] – Use of old library references .net framework 2.0


***Build 1005 (2017-10-01)***

- Improvements
    + Release of *VECTO Hashing Tool*

- Bugfixes
    + [VECTO-569] - ‘Engine Retarder’ not correctly recognized as input
    + [VECTO-571] - Customer Report – wrong output format of average RRC
    + [VECTO-573] - Correction of displayed units in graph window
    + [VECTO-575] - Correction of simulation aborts (due to gearbox inertia, engineering mode)
    + [VECTO-577] - Correction of XML export functionality
    + [VECTO-579] - Bug fix GUI crashes on invalid input
    + [VECTO-558] - Correction of output in .vsum file – BFColdHot always 0
    + [VECTO-564] - Bug fix: correct output of vehicle group in XML report
    + [VECTO-566] - Vehicle height not correctly read (engineering mode)
    + [VECTO-545] - Update documentation on Settings dialog


***Build 940 (2017-07-28)***

- Bugfixes:
    + [VECTO-546] - GearboxCertificationOptionType Option 2 not accepted by VECTO
    + [VECTO-547] - Engine Manufacturer and Engine Model are empty in .vsum
    + [VECTO-548] - online user manual
    + [VECTO-549] - Inconsistent (and wrong) decimal separator in XML output (manufacturer report)
    + [VECTO-551] - Average Tyre RRC not in Customer Information File output
    + [VECTO-536] - GUI: improvements vehicle dialog (add missing pictures for vehicle categories)
    + [VECTO-550] - Allow custom settings for AirDensity in Engineering mode
    + [VECTO-552] - set engine rated power, rated speed to computed values from FLD if not provided as input


***Build 925 (2017-07-13)***

- Improvements
    + [VECTO-366] added EMS vehicle configuration, EMS is only simulated when engine rated power > 300kW
    + [VECTO-463] add pneumatic system technology 'vacuum pump'
    + [VECTO-465] change RRC value of trailers (declaration mode) from 0.00555 to 0.0055 (due to limits in user interface)
    + [VECTO-477] AT Gearbox, powershift losses: remove inertia factor
    + [VECTO-471] update cross-wind correction model: height-dependent wind speed (see Excel spreadsheet in User Manual folder for details)
    + [VECTO-367] Add Vehicle Design Speed to segmentation table
    + [VECTO-470] Add XML reading and export functionality
    + [VECTO-486] Adding hashing library
    + [VECTO-469] Limit engine max torque (either due to vehicle or gearbox limits), limit gearbox input speed
    + [VECTO-466] Update vehicle payloads: 10% loaded and reference load are simulated
    + [VECTO-467] Add generic PTO activation in municipal cycle
    + [VECTO-468] Add PTO losses (idle) in declaration mode
    + [VECTO-479] Added PTO option 'only one engaged gearwheel above oil level' with 0 losses
    + [VECTO-483] Adapt CdxA supplement for additional trailers
    + [VECTO-494] Implementation of different fuel types
    + [VECTO-502] Implementing standard values for air-drag area (if not measured)
    + [VECTO-501] Implement engine idle speed set in vehicle (must be higher than engine's idle speed value)
    + [VECTO-504] Adding HVAC technology 'none'
    + [VECTO-489] Extrapolate gearbox lossmaps (required when torque limitation by gearbox is ignored)
    + [VECTO-505] Implement AT transmissions in declaration mode
    + [VECTO-507] Allow to ignore validation of model data when starting a simulation (significant improvement on simulation startup time - about 10s)
    + [VECTO-506] modified method how torque-converter characteristics in drag is extended. allow drag-values in the input, only add one point at a high speed ratio
    + [VECTO-509] Add axle-type (vehicle driven, vehicle non-driven, trailer) to GUI
    + [VECTO-511] Add engine idle speed to Vehicle input form (GUI)
    + [VECTO-510] Write XML reports (manufacturer, customer information) in declaration mode
    + [VECTO-474] new driving cycles for Municipal and Regional Delivery
    + [VECTO-522] step-up ratio for using torque converter in second gear set to 1.85 for busses (still 1.8 for trucks)
    + [VECTO-525] remove info-box with max loading in GUI
    + [VECTO-531] Payload calculation: limit truck payload to the truck's max payload. (earlier versions only limited the total payload of truc + trailer to the total max. payload, i.e. allowed to shifted loading from truck to the trailer)
    + [VECTO-533] allow second driven axle, rdyn is calculated as average of both driven axles
    + [VECTO-537] new Suburban driving cycles
    + [VECTO-541] increase declaration mode PT1 curve to higher speeds (2500 is too low for some engines)



- Bugfixes:
    + [VECTO-462] fix: decision if PTO cycle is simulated
    + [VECTO-473] fix: adapt range for validation of torque converter characteristics
    + [VECTO-464] fix: extrapolation of engine full-load curve gives neg. max. torque. Limit engine speed to n95h
    + [VECTO-480] fix: a_pos in .vsum was less than zero
    + [VECTO-487] fix: Duration of PTO cycle was computed incorrectly if PTO cycle does not start at t=0
    + [VECTO-514] fix: sort entries in .vsum numerically, not lexically
    + [VECTO-516] fix: consider axlegear losses for estimation of acceleration after gearshifts
    + [VECTO-517] fix: valid shift polygon was considered invalid when extended to very high torque ranges
    + [VECTO-424] fix: VectoCore.dll could not be found when the current working directory is different to the directory of the vectocmd.exe
    + [VECTO-425] fix: vectocmd.exe - check if the output is redirected, and skip updating of the progress bar when this is the case
    + [VECTO-426] fix: vectocmd.exe - log errors to STDERR
    + [VECTO-519] fix: computation of n95h fails for a valid full-load curve due to numerical inaccuracy. add tolerance when searching for solutions
    + [VECTO-520] fix: gearashift count in vsum is 0





**VECTO 3.1.2**

***Build 810 (2017-03-21)***

- Improvements:
    + [VECTO-445] Additional columns in vsum file
    + Allow splitting shift losses among multiple simulation intervals
    + Allow coasting overspeed only if vehicle speed > 0
    + Torque converter: better handling of ‘creeping’ situations

- Bugfixes:
    + [VECTO-443] Bugfix in AMT shift strategy: skip gears not working correctly


***Build 796 (2017-03-07)***

- Improvements:
    + [VECTO-405] Adding clutch-losses for AMT/MT gearboxes during drive-off, reduce drive-off distance after stop from 1m to 0.25m, set clutch closing speed (normalized) to 6.5%, changes in clutch model
    + [VECTO-379] Make GUI more tolerant against missing files. Instead of aborting reading the input data the GUI shows a suffix for missing input files
    + [VECTO-411] Allow a traction interruption of 0s for AMT/MT gearboxes
    + [VECTO-408] Gearbox Inertia for AT gearboxes set to 0
    + [VECTO-419] Adapted error messages, added list of errors
    + [VECTO-421,VECTO-439] Added volume-related results to vsum file (volume is computed based on default bodies)
    + [] Energy balance (vsum) and balance of engine power output and power consumers (vmod) level
    + [VECTO-430] AT shift strategy: upshifts may happen too early
    + [VECTO-431] AMT shift strategy always started in first gear due to changes in clutch model
    + [VECTO-433] adapt generic vehicles: use typical WHTC correction factors
    + [VECTO-437] set vehicle speed at clutch-closed to 1.3 m/s
    + [VECTO-436] fix simulation aborts with AT gearbox (neg. braking power, unexpected response, underload)
- Bugfixes:
    + [VECTO-415] Powershift Losses were not considered for AT gearboxes with PowerSplit
    + [VECTO-416] Measured Speed with gear failed when cycle contained parts with eco-roll (computation of next gear failed)
    + [VECTO-428] Sum of timeshares adds up to 100%
    + [VECTO-429] Min Velocity for lookahead coasting was not written to JSON file


**VECTO 3.1.1**

***Build 748 (2017-01-18)***

- Bugfixes:
    + [VECTO-404] Driving Cycle with PTO stopped simulation after first PTO activation


***Build 742 (2017-01-12)***

- Improvements:
    + [VECTO-390, VECTO-400] Adapt engine speed to estimated engine speed after gear shift during traction interruption (double clutching)
    + [VECTO-396, VECTO-388] Add shift losses for AT power shifts
    + [VECTO-389] new gear shift rules for AT gearboxes
    + [VECTO-387] added max input speed for torque converter
    + [VECTO-385] Automatically add generic torque converter data for drag
    + [VECTO-399] Add missions and loadings for vehicle categories 11, 12, and 16 (declaration mode)
    + [VECTO-384] cleanup memory after simulation run
    + [VECTO-394] new option for vectocmd to disable all output
    + [VECTO-392] make the GUI scale depending on the Windows font size
    + [VECTO-391] Gearbox output speed and output torque added to .vmod files
    + [VECTO-386] Gearbox window: disable input fields not applicable for the selected gearbox type
- Bugfixes:
    + [VECTO-401] Computation of n_95h etc. fails if engine’s max torque is constant 0
Lookup of Airdrag parameters in declaration mode
    + [VECTO-378] Improved file-handling in AAUX module


**VECTO 3.1.0**

***Build 683 (2016-11-14)***

- Bugfixes:
    + [VECTO-375] Fixed bug when braking during slope change from negative to positive values.
    + [VECTO-372] Added check for unusual acceleration/deceleration data which could lead to error when halting.
    + [VECTO-371] Added additional behavior to overcome such situations
    + [VECTO-370] Added additional behavior to overcome such situations
    + [VECTO-369] CrosswindCorrection is now saved and read again from JSON files
    + [VECTO-373] WHTC-Engineering correction factor now correctly read/write in JSON files
    + [VECTO-368] Fixed validation for specific cases when values are intentionally invalid.
    + [VECTO-357] Updated GUI to not show ECO-Roll option to avoid confusion
    + Fixed numerous bugs in AT-ShiftStrategy regarding the Torque Converter
    + Fixed numerous bugs in MeasuredSpeed Mode (and MeasuredSpeed with Gear) in connection with AT-Gearbox and TorqueConverter
    + Fixed a bug when PTO-Cycle was missing
    + Corrected axle loss maps for Generic Vehicles in Declaration Mode to match technical annex
    + Corrected SumFile Cruise Time Share. Added checks that timeshares must add up to 100%

- Improvements:
    + [VECTO-355] Updated documentation, added powertrain schematics in chapter "Simulation Models"
    + [VECTO-374] Check range for Torque Converter speed ratio input data to be at least between 0 and 2.2
    + Updated many error messages to be more explicit about the reason of error
    + Added "Mission Profiles" Directory with driving cycles publicly available in the application root directory.
    + Added "Declaration" directory with the declaration data files in the application root directory.
	+ Added warning when engine inertia is 0
    + Added check that engine speed must not fall below idle speed (even in measured speed mode)
    + Shift curve validation for AT gearboxes: shift curves may now overlap due to different shift logic in AutomaticTransmissions.
    + Updated Crosswind Coefficients for Tractor+Semitrailer


***Build 662 (2016-10-24)***

- Bugfixes:
    + [VECTO-360] Fixed error during startup of VECTO (loading of DLLs).
    + [VECTO-358] Fixed errors during simulation where vehicle unintentionally was driving backwards. Added stricter sanity checks and invariants to avoid such errors. Fixed 1Hz-Filter for ModFiles (distance was wrong under certain circumstances, vehicle seemingly jumping back before halt).
    + [VECTO-361] Fixed classification of vehicles with GVM of exactly 7500kg (Class 1).
    + [VECTO-364] Fixed an error in measured speed mode (run aborts).
    + [VECTO-363] Compute shift polygons in declaration mode now uses correct boundary for full load margin.
    + [VECTO-365] Fixed editing gears in declaration mode

- Improvements:
    + [VECTO-355] User Manual updated (Screenshots, Descriptions, File Formats, Vecto V2 Comments removed).
    + [VECTO-317] Declaration data for Wheel sizes updated
    + [VECTO-359] Simplified code regarding PT1 behavior.
    + [VECTO-323] PTO-Cycle may now be left empty when not used in driving cycle.

***Build 652 (2016-10-14)***

- Main Updates
    + Removed VECTO Core 2.2
    + Refactoring of the User-Interface Backend: loading, saving files and validating user input uses Vecto 3 models
    + AT-Gearbox Model: differentiate between AT gearbox with serial torque converter and AT gearbox using powersplit
    + Numbering of gears with AT gearbox corresponds to mechanical gears, new column TC_locked in .vmod file to indicate if torque converter is active
    + Torque converter gear no longer allowed in input (added by Vecto depending on the AT model)
    + New implementation of torque converter model (analytic solutions)
    + Added PTO option for municipal utility vehicles: PTO idle losses, separate PTO cycle during standstill
    + Added Angledrive Component
    + Option for constant Auxiliary Power Demand in Job-File
    + Normalize x/y values before triangulating Delaunay map (transmission loss-maps, fuel consumption loss map)
    + Additional fuel consumption correction factor in declaration mode: cold/hot balancing factor
    + Added fuel consumption correction factor (WHTC, Cold/Hot balancing, …) in engineering mode
    + Update auxiliaries power demand according to latest whitebook
    + Allow multiple steered axles
    + Adapted engine idle controller (during declutch) – engine speed decreases faster
    + SUM-File: split E_axl_gbx into two columns, E_axl and E_gbx
    + New columns in mod-file: PTO, torque converter
    + Removed full-load curve per gear, only single value MaxTorque
    + Removed rims (dynamic wheel radius depends on wheel type)
    + Fixes in AAUX module: open correct file-browser, save selected files

-------------------------------------------------------------------------------

**VECTO 3.0.4**

***Build 565 (2016-07-19)***

- Bugfixes
    + AAUX HVAC Dialog does not store path to ActuationsMap and SSMSource
    + GUI: check for axle loads in declaration mode renders editing dialog useless
    + Vecto 2.2: Simulation aborts (Vecto terminates) when simulating EngineOnly cycles
    + Vecto 3: Building SimulationRun EngineOnly simulation failed

***Build 544 (2016-06-28)***

- Main Updates
    + New gear shift strategy according to White Book 2016
    + New coasting strategy according to White Book 2016
    + New input parameters (enineering mode) for coasting and gear shift behavior
    + Use SI units in Advanced Auxiliaries Module and compile with strict compiler settings (no implicit casts, etc.)
    + Allow efficiency for transmission losses (in engineering mode)

- Bugfixes
    + Auxiliary TechList not read from JSON input data
    + Improvements in driver strategy
    + Bugfixes in MeasuredSpeed mode

-------------------------------------------------------------------------------

**VECTO 3.0.3**

***Build 537 (2016-06-21)***

- Main Updates
    + Support for Advanced Auxiliaries (Ricardo) in Vecto 3.0.3 and Vecto 2.2
    + Performance improvements
    + Gearshift polygons according to WB 2016
    + Revision of SUM-data file, changed order of columns, changed column headers
- Bugfixes
    + Delaunay Maps: additional check for duplicate input points
    + Creation of PDF Report when running multiple jobs at once
    + Sanity checks for gear shift lines
    + Improvements DriverStrategy: handling special cases

***Build 495 (2016-05-10)***

- Main Updates
    + Support for Advanced Auxiliaries (Ricardo) in Vecto 3.0.3 and Vecto 2.2
    + Performance improvements
    + Gearshift polygons according to WB 2016
    + Revision of SUM-data file, changed order of columns, changed column headers
- Bugfixes
    + Delaunay Maps: additional check for duplicate input points
    + Creation of PDF Report when running multiple jobs at once
    + Sanity checks for gear shift lines
    + Improvements DriverStrategy: handling special cases

-------------------------------------------------------------------------------

**VECTO 3.0.2**

***Build 466 (2016-04-11)***

- Bugfix: calculation of CO2 consumption based on FC-Final (instead of FC-map)
- Bugfix: acceleration in .vmod was 0 in certain cases (error in output)
- Bugfix: syncronized access to cycle cache (declaration)

***Build 448 (2016-03-24)***

- Bugfix: set WHTC factors to a valid default value in engineering mode
- Bugfix: first page of declaration report was missing
- fixed inconsistencies in user manual
- Bugfix: better error message roll resistance calculation could not be calculated
- Bugfix: measured speed now calculates distance correctly
- Bugfix: measured speed fills missing moddata columns (acc, dist, grad)
- Bugfix: better error message when driving cycle is missing.
- Bugfix: vectocmd errormsg when writing progress

***Build 434 (2016-03-10)***

- New simulation modes:
    + Measured Speed
    + Measured Speed with Gear
    + Pwheel (SiCo)
- Adaptations of powertrain components architecture
    + Move wheels inertia from vehicle to wheels
    + Auxiliaries no longer connected via clutch to the engine but via a separate port
    + Engine checks overload of gearbox and engine overload
- Fixed some driving behavior related issues in VectoCore:
    + When the vehicle comes to a halt during gear shift, instead of aborting the cycle, it tries to drive away again with an appropriate gear.
- [ModData Format](#modal-results-.vmod) changed for better information and clarity
- Entries in the sum-file are sorted in the same way as in Vecto 2.2
- In engineering mode the execution mode (distance-based, time-based measured speed, time-based measured speed with gear, engine only) are detected based on the cycle
- Added validation of input values
- Gravity constant set to 9.80665 (NIST standard acceleration for gravity)
- Improved input data handling: sort input values of full-load curves (engine, gbx, retarder)
- Better Integration of VectoCore into GUI (Notifications and Messages)
- Speed dependent cross-wind correction (vcdv) and v_air/beta cross-wind correction (vcdb) impemented
- For all calculations the averaged values of the current simulation step are used for interpolations in loss-maps.
- Allow extrapolation of loss maps in engineering mode (warnings)
- Refactoring of input data handling: separate InputDataProvider interfaces for model data
- Refactoring of result handling: separate result container and output writer
- New Long-Haul driving cycle included
- User Manual updated for VECTO V3.x
- Fix: sparse representation of declaration cycles had some missing entries
- Bugfix: error in computation of engine's preferred speed
- Bugfix: wrong vehicle class lookup
- Bugfix: duplicate entries in intersected full-load curves
- Bugfix: retarder takes the retarder ratio into account for lossmap lookup
- Bugfix: use unique identifier for jobs in job list
- Bugfix: error in triagulation of fuel consumption map






