# VECTO 4\.x Release Notes

![](img%5CRelease%20Notes%20Vecto4x0.png)

<!-- Cover Slide -->



# VECTO v4.3.4-DEV (10-03-2025)

This development release addresses the issue discussed on previous workshops concerning the definition of Thermal Derating for electric motors and IEPC. The modifications implemented are the following: 

1. A new definition is proposed for torque limits in derating mode and transition to derating mode (through merge request !326, which describes the new equation used). 
2. A specific correction is added for IEPC recently declared overload error due to downshift with high inertia effect, allowing for those cases to run without any issue (Merge request !324). 
    
The new derating mode definition can generate changes in VECTO results for any vehicle equipped with an electric motor or IEPC, therefore stakeholders are invited to test this development release and provide feedback before introducing the modifications in a future VECTO official release.
    
Issues tentatively addressed by this version (the list could change according to discussions that will follow): #761, #767, #826, #830, #850, #867, #877, #878, #886, #888, #889, #893, #939 (and clone #940, #959), #941, #942, #943, #947, #955, #956, #957, #958, #960, #962, #963, #969, #970, #971, #974 (and clone #975), #982, #983. 

## Bug Fixes

- EM-IEPC Thermal Derating - Tq_max and Buffer Mods (vecto/vecto!326)

- Add Driving Actions for IEPC gearshift (vecto/vecto!324)




# VECTO v4.3.3 Official Release (04-03-2025)


## Bug Fixes

- Check if XML element is signed (#950) (vecto/vecto!314)

- NgTankSystem optional for HEV lorries MRF XSD (vecto/vecto!315)

- fix: monitoring report for dual fuel vehicles (vecto/vecto!318)

- fix: secure XML loading against external entity injection (vecto/vecto!319)

- fix: correct interim supercap reader type (vecto/vecto!320)

- fix: parameter IDs for XSDs v2.3 and v2.6 (vecto/vecto!322)

## Documentation

- Update XSD parameter IDs documentation (vecto/vecto!327)




# VECTO v4.3.3 Official Release (03-03-2025)


## Bug Fixes

- Check if XML element is signed (#950) (vecto/vecto!314)

- NgTankSystem optional for HEV lorries MRF XSD (vecto/vecto!315)

- fix: monitoring report for dual fuel vehicles (vecto/vecto!318)

- fix: secure XML loading against external entity injection (vecto/vecto!319)

- fix: correct interim supercap reader type (vecto/vecto!320)

- fix: parameter IDs for XSDs v2.3 and v2.6 (vecto/vecto!322)




# VECTO v4.3.2-RC (06-02-2025)

## Features

- New battery and supercap readers (vecto/vecto!309)

## Bug Fixes

- ATShiftStrategyOptimized - No UpshiftFomL if not locked (vecto/vecto!301)

- Forbid downshift to locked gear in APT-S if it generates direct upshift condition (vecto/vecto!307)

- Add condition to write BusAuxiliaries output data in vsum (vecto/vecto!306)

- Allow old XMLs for battery and supercap in development only (vecto/vecto!311)

- Correct binding for supercap input data class; use correct xml data type in xml component reader for supercap (vecto/vecto!310)



# VECTO v4.2.7 Official Release (09-01-2025)

## Bug Fixes

- Track release_notes.md for release
- CodeEU #858: Convert steering pump tech (vecto/vecto!303)
- Converter Tool: ngTankSystem for dual fuel (vecto/vecto!304)
- CodeEU #836: Restrictions on IEPC gear and MaxTorqueCurve XSD attributes (vecto/vecto!302)



# VECTO v4.2.6-RC (06-12-2024)


## Features

- Support Gitlab issue pattern (vecto/vecto!272)
- CodeEU #854: Verify primary bus VIF hash against Job (vecto/vecto!295)
- CodeEU #833, #834, #835, #836, #837: Add 3rd amendment XSD definitions (vecto/vecto!291)
- CodeEU #838: Include Engine into v2.6 (vecto/vecto!297)

## Bug Fixes

- Homogenize versions across tools (vecto/vecto!270)
- CodeEU #807: Produce same data from ADC loss map (#807) (vecto/vecto!278)
- Authors and readme metadata content (vecto/vecto!277)
- CodeEU #809: Writing engine information in MRF (#809) (vecto/vecto!275)
- CodeEU #842, #840, #841, #839, #798: Driver model: in case of an APT vehicle where the driving action is Brake... (vecto/vecto!287)
- CodeEU #812, #788: Conversion of doubles for SI (vecto/vecto!283)
- CodeEU #750, #758, #769, #816, #821, #829: Avoid wrong upshift and downshift for light SMT vehicles (vecto/vecto!290)
- CodeEU #844, #705, #530: During a coasting action (look-ahead coasting) a gear hunting occurs in the... (vecto/vecto!289)
- CodeEU #784: Override DoWriteModalResult for VTP (vecto/vecto!294)
- CodeEU #495, #642, #739: Add SMT downshift condition - DroppedSpd>DisengSpd (vecto/vecto!293)



# VECTO v4.2.5 Official Release (02-10-2024)


## Hot Fixes

- Missing Build.props DefineConstants (!268)
- Version 4th number read from Build.props (!269)


# VECTO v4.2.3 Official Release (01-10-2024)

## Enhancements
- CodeEU #799: Adapt VECTO for the new CI updates (!263)

## Bug Fixes
- CodeEU #794: Added missing monitoring report file (!265)
- CodeEU #780: Update weights for bus subgroups (!264)

# Vecto 4.2.2.3539 RELEASE CANDIDATE (09-09-2024)

## Bug Fixes

* <span style="color:#000000">CodeEU\-710: Hashing tool check fail with VECTO version 3330</span>
* <span style="color:#000000">CodeEU\-711: Hashing tool check fail with VECTO 4.1.3.3415</span>
* <span style="color:#000000">CodeEU\-712: VECTO VTP error</span>
* <span style="color:#000000">CodeEU\-754: "Failed to find operating point"; "Failed to find mechanic power for given electric power" in E2 vehicle</span>
* <span style="color:#000000">CodeEU\-727: Failure in simulating HEV in different VECTO versions</span>
* <span style="color:#000000">CodeEU\-749: Double summary for electric vehicles</span>
* <span style="color:#000000">CodeEU\-542: IVECO confidential : BUG REPORT : CRW LE T7D VOITH NXT 5.63</span>
* <span style="color:#000000">CodeEU\-663: IHPC: Failed to find operating point </span>
* <span style="color:#000000">CodeEU\-634: Article 10(2) issue - VIN YS2G6X20002202570</span>
* <span style="color:#000000">CodeEU\-671: IHPC: simulation abort due to unexpected response</span>

# Vecto 4.2.1.3469 OFFICIAL RELEASE (01-07-2024)

## Features

* <span style="color:#000000">CodeEU\-726: Build an XML converter tool for older VECTO jobs</span>

## Bug Fixes

* <span style="color:#000000">CodeEU\-719: the six new tyre dimensions from line 126 onwards to the latest "wheels\.csv" file in the VECTO repository</span>
* <span style="color:#000000">CodeEU\-717: VECTO\-4\.2\.0\.3448\-RC \- Buses AMT Gearbox Type with 1% higher C02 in primary results</span>
* <span style="color:#000000">CodeEU\-724: Error in Primary Bus Simulation: Object reference not set to an instance of an object</span>
* <span style="color:#000000">CodeEU\-716: VECTO\-4\.2\.0\.3448\-RC \- Buses Result Summary section missing in RLST\_Customer\.xml</span>
* <span style="color:#000000">CodeEU\-694: Primary and Completed heavybus FCV article 9 exempted hashcode mismatch\.</span>
* <span style="color:#000000">CodeEU\-735: SMT strategy different between engineering and declaration mode</span>
* <span style="color:#000000">CodeEU\-736: Existing customer reports \(CIF\) fail validation</span>
* <span style="color:#000000">CodeEU\-737: Missing data from XML report</span>

# Vecto 4.2.0.3448 RELEASE CANDIDATE (10-06-2024)

## Features

<span style="color:#000000">CodeEU\-696: Double summary in CIF for vocationals and non vocational missions\.</span>

<span style="color:#000000">CodeEU\-698: Incorporate missions RD\, LH and EMS to class 16 vehicles</span>

<span style="color:#000000">CodeEU\-697: Re\-evaluate subgroup allocations for Long Haul</span>

<span style="color:#000000">CodeEU\-676: Feature: Implement monitoring report</span>

## Bug fixes (1/4)

* <span style="color:#000000">CodeEU\-471: VectoSimulationException: VF640J869RB022573</span>

* <span style="color:#000000">CodeEU\-462: Article10\-2\-issue | Order\-Nr 28206354 | HEV P1 Error Mercedes\-Benz and Setra Hybrid</span>

* <span style="color:#000000">BusesCodeEU\-326: Article10\-2\-issue | Order\-Nr 28195581 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-348: Article10\-2\-issue | Order\-Nr 28204519 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-457: Article10\-2\-issue | Order\-Nr 28174000 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-458: Article10\-2\-issue | Order\-Nr 28186528 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-459: Article10\-2\-issue | Order\-Nr 28203057 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-545: IVECO confidential : BUG REPORT : HEV\-P1 : UW18m C9 VOITH NXT CRU 48V mild hybrid</span>

* <span style="color:#000000">CodeEU\-346: Article10\-2\-issue | Order\-Nr 28202338 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-352: Article10\-2\-issue | Order\-Nr 28202130 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-394: Article10\-2\-issue | Order\-Nr 28204065 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

## Bug fixes (2/4)

* <span style="color:#000000">CodeEU\-433: Article10\-2\-issue | Order\-Nr 28192321 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-451: Article10\-2\-issue | Order\-Nr 28204280 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-452: Article10\-2\-issue | Order\-Nr 28197394 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-453: Article10\-2\-issue | Order\-Nr 28199435 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-454: Article10\-2\-issue | Order\-Nr 28206982 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-655: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28208126 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-660: Retarder lossmap error in completed vehicle simulation</span>

* <span style="color:#000000">CodeEU\-662: Generic retarder map speed range insufficient in some cases</span>

* <span style="color:#000000">CodeEU\-648: Error in Multistep Tool PEV/P\-HEV</span>

* <span style="color:#000000">CodeEU\-618: PEV vehicles simulation error depending the time format</span>

* <span style="color:#000000">CodeEU\-700: Factor Method Generic IHPC Powermap De\-normaization bug</span>

* <span style="color:#000000">CodeEU\-482: Article10\-2\-issue | Order\-Nr 28203040 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-514: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28208051 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-529: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28199994 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

## Bug fixes (3/4)

* <span style="color:#000000">CodeEU\-544: IVECO confidential : BUG REPORT : HEV\-P1 : CRW LE C9 VOITH NXT CRU 48V mild hybrid</span>

* <span style="color:#000000">CodeEU\-552: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28201759 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-556: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28208176 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-557: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28209751 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-622: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28210594 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-632: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28210591 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-672: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28208841 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-673: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28209179 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-674: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28211540 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

## Bug fixes (4/4)

* <span style="color:#000000">CodeEU\-678: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28192673 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-697: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28209551 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-685: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28200286 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-686: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28201178 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-687: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28209679 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

* <span style="color:#000000">CodeEU\-692: Article10\-2\-issue | VECTO\-4\.1\.3 | Order\-Nr 28209789 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

# Vecto 4.1.3.3415 OFFICIAL RELEASE (08-05-2024)

## Hot fixes
  * <span style="color:#000000">CodeEU\-638: Incorrect construction payloads for group 9 non\-vocational vehicle</span>

## Bug fixes
  * <span style="color:#000000">CodeEU\-615: Multistep freezes after loading VIF chassis</span>
  * <span style="color:#000000">CodeEU\-616: Multistep tool freezes</span>
  * <span style="color:#000000">CodeEU\-619: restore wrong exempted techs in XSD for backwards compatibility</span>
  * <span style="color:#000000">CodeEU\-617: Results change depending the time format</span>
  * <span style="color:#000000">CodeEU\-635: Revert multiple summary in CIF</span>

# Vecto 4.1.0.3392 RELEASE CANDIDATE (15-04-2024)

## Features
  * <span style="color:#000000">CodeEU\-577: Add missing mission profiles to vocational and non\-vocational</span>

## Bug fixes (1/4)
  * <span style="color:#000000">CodeEU\-331: Gear 1 DrivingActionAccelerate: Fail</span>
  * <span style="color:#000000">CodeEU\-367: Gear 1 DrivingActionAccelerate: Fail</span>
  * <span style="color:#000000">CodeEU\-372: ADT Error on Bus Category Primary Vehicle Simulation on VECTO</span>
  * <span style="color:#000000">CodeEU\-373: ADT Error on Bus Category Primary Vehicle Simulation on VECTO</span>
  * <span style="color:#000000">CodeEU\-374: ADT Error on Bus Category Primary Vehicle Simulation on VECTO</span>
  * <span style="color:#000000">CodeEU\-375: ADT Error on Bus Category Primary Vehicle Simulation on VECTO</span>
  * <span style="color:#000000">CodeEU\-393: Finished Run VEH\-PrimaryBus\_nonSmart Interurban \_P32SD\_ReferenceLoad with ERROR: 16</span>
  * <span style="color:#000000">CodeEU\-439: 615 \(Interurban \_P32DD\_ReferenceLoad\) \- absTime: 7129\.6359 \[s\]\, distance: 53875\.9765 \[m\]\, dt: 0\.6388 \[s\]\, v: 0\.5098 \[m/s\]\, Gear: 1 | DrivingActionAccelerate: Failed</span>
  * <span style="color:#000000">CodeEU\-446: Finished Run VEH\-PrimaryBus\_nonSmart Interurban \_P32SD\_ReferenceLoad with ERROR: 16</span>
  * <span style="color:#000000">CodeEU\-449: Finished Run VEH\-PrimaryBus\_nonSmart Interurban \_P32SD\_ReferenceLoad with ERROR: 16</span>
  * <span style="color:#000000">CodeEU\-478: Finished Run VEH\-PrimaryBus\_nonSmart Urban \_P31SD\_ReferenceLoad with ERROR: 4 \(Urban \_P31SD\_ReferenceLoad\) \- absTime: 8606\.6241 \[s\]\, distance: 39112\.5127 \[m\]\, dt: 1\.1131 \[s\]\, v: 0\.1833 \[m/s\]\, Gear: 1 | DrivingActionAccelerate: Failed to find operating poi</span>

## Bug fixes (2/4)
  * <span style="color:#000000">CodeEU\-481: Finished Run VEH\-PrimaryBus\_nonSmart Urban \_P31SD\_ReferenceLoad with ERROR: 4 \(Urban \_P31SD\_ReferenceLoad\) \- absTime: 8606\.6241 \[s\]\, distance: 39112\.5127 \[m\]\, dt: 1\.1131 \[s\]\, v: 0\.1833 \[m/s\]\, Gear: 1 |</span>
  * <span style="color:#000000">CodeEU\-488: Finished Run VEH\-PrimaryBus\_nonSmart Urban \_P31SD\_ReferenceLoad with ERROR: 4 \(Urban \_P31SD\_ReferenceLoad\) \- absTime: 8606\.6241 \[s\]\, distance: 39112\.5127 \[m\]\, dt: 1\.1131 \[s\]\, v: 0\.1833 \[m/s\]\, Gear: 1 | DrivingActionAccelerate: Failed to find operating poi</span>
  * <span style="color:#000000">CodeEU\-494: Finished Run VEH\-PrimaryBus\_nonSmart Urban \_P31SD\_ReferenceLoad with ERROR: 26 \(Urban \_P31SD\_ReferenceLoad\) \- absTime: 8606\.6241 \[s\]\, distance: 39112\.5127 \[m\]\, dt: 1\.1131 \[s\]\, v: 0\.1833 \[m/s\]\, Gear: 1 | DrivingActionAccelerate: Failed to fi</span>
  * <span style="color:#000000">CodeEU\-420: Finished Run VEH\-PrimaryBus\_nonSmart Interurban \_P32SD\_ReferenceLoad with ERROR: 16</span>
  * <span style="color:#000000">CodeEU\-573: Unhandled powertrain architecture Conventional Vehicle to calculate gradability</span>
  * <span style="color:#000000">CodeEU\-582: unhandled powertrain architecture ConventionalVehicle to calculate gradability\, 11:44:36\.63\,</span>
  * <span style="color:#000000">CodeEU\-595: Inconsistency exempted vehicles "Fuel cell vehicle" vs\. "FCV Article 9 exempted"</span>
  * <span style="color:#000000">CodeEU\-501: Help regarding VTP vdri format is not up to date in v4</span>
  * <span style="color:#000000">CodeEU\-594: inadequate validation for auxiliaries in completed vehicle XML</span>
  * <span style="color:#000000">CodeEU\-580: Potential error in XML schema v2\.4 \(exempted vehicles\)</span>
  * <span style="color:#000000">CodeEU\-551: Article\-10\-2 XLRASF5E00G419905</span>
  * <span style="color:#000000">CodeEU\-547: Max ICE Off timespan for buses</span>
  * <span style="color:#000000">CodeEU\-583: Incorrect internal resistance for SuperCap used in factor method</span>

## Bug fixes (3/4)
  * <span style="color:#000000">CodeEU\-571: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28202896 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>
  * <span style="color:#000000">CodeEU\-526: VTP calculation aborted on fuel consumption map</span>
  * <span style="color:#000000">CodeEU\-502: Error in VECTO calculation WMA10CZZ0RF022326</span>
  * <span style="color:#000000">CodeEU\-401: Electric steering system with conventional vehicle not according to 2017/2400</span>
  * <span style="color:#000000">CodeEU\-476: Setting Initial SoC in REESS editor leads to error</span>
  * <span style="color:#000000">CodeEU\-533: Question about heavy lorry\_IEPC\_Gb×4speed: Failed to generate electric power map \- at least two negative entries are required</span>
  * <span style="color:#000000">CodeEU\-456: Clarification Documentation official Results</span>
  * <span style="color:#000000">CodeEU\-506: Article 10\(2\) issue \- VIN YS2P6X200R2201285</span>
  * <span style="color:#000000">CodeEU\-507: Article 10\(2\) issue \- VIN YS2P6X20005732399</span>
  * <span style="color:#000000">CodeEU\-508: Article 10\(2\) issue \- VIN S2P6X20005734199</span>
  * <span style="color:#000000">CodeEU\-499: Object Reference not set Error</span>
  * <span style="color:#000000">CodeEU\-546: Problem with P181 \(Cooling Fan Technology\) for a conventional vehicle</span>
  * <span style="color:#000000">CodeEU\-516: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28208044 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>
  * <span style="color:#000000">CodeEU\-517: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28208037 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>

## Bug fixes (4/4)
  * <span style="color:#000000">CodeEU\-519: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 10098181 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>
  * <span style="color:#000000">CodeEU\-527: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28196233 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>
  * <span style="color:#000000">CodeEU\-528: Article10\-2\-issue | VECTO\-4\.0\.3 | Order\-Nr 28196652 | HEV P1 Error Mercedes\-Benz and Setra Hybrid Buses</span>
  * <span style="color:#000000">CodeEU\-338: Eco\-roll only\, without engine stop\, impact on CO2 emission</span>
  * <span style="color:#000000">CodeEU\-475: XML files loading in VTP Job not OK; No auxiliary data and torque curve visible\, Job can't be saved in VECTO 4\.0\.3\.3330</span>
  * <span style="color:#000000">CodeEU\-610: VTP mode is broken due to changes in the Clutch component\.</span>

# Vecto 4.0.3.3330 OFFICIAL RELEASE (13-02-2024)

* ## Bug Fixes
  * <span style="color:#000000">CodeEU\-293: DistanceRun got an unexpected response</span>
  * <span style="color:#000000">CodeEU\-298: Object reference not set to an instance of an object</span>
  * <span style="color:#000000">CodeEU\-300: Full drive torque miscalculation</span>
  * <span style="color:#000000">CodeEU\-336: Feature: Hashing tool must validate the previous step data for multi\-step jobs</span>
  * <span style="color:#000000">CodeEU\-337: Electric Citybus \- ERROR with "31a\-Specific" bus configuration</span>
  * <span style="color:#000000">CodeEU\-343: Cannot simulate Primary Vehicle using vectocmd\.exe</span>
  * <span style="color:#000000">CodeEU\-387: VECTO sometimes fails to properly read Tyre data from primary vehicle xml</span>
  * <span style="color:#000000">CodeEU\-427: Vehicle speed resulting to exceeding max gearbox speed</span>
  * <span style="color:#000000">CodeEU\-428: Remove speed safety margin for gearbox re\-engaging</span>
  * <span style="color:#000000">CodeEU\-438: Mismatch XML schema vs Regulation \(exempted vehicles\)</span>
  * <span style="color:#000000">CodeEU\-248: Bus P2 hybrid VECTO error in urban cycle</span>
  * <span style="color:#000000">CodeEU\-278: Simulation crash when writing fuel consumption results to reports</span>
  * <span style="color:#000000">CodeEU\-284: New error message and failed simulation obtained for P2 hybrid buses</span>
  * <span style="color:#000000">CodeEU\-285: PEV vehicle error in routine to write the results with new vecto version  4\.0\.2\.3275</span>
  * <span style="color:#000000">CodeEU\-287: PEV\_IEPC error message:"can only operate on SI Objects with the same unit"</span>
  * <span style="color:#000000">CodeEU\-289: VECTO Simulation Error for bus with validated input data</span>

# Vecto 4.0.2.3275 OFFICIAL RELEASE (20-12-2023)

## Hot fix
  * <span style="color:#000000">CodeEU\-273\, CodeEU\-274: Changes in the AMT shift strategy regarding idling speed caused simulation aborts</span>
  * <span style="color:#000000">CodeEU\-260: regression fix handling overload buffer</span>

## Bug fixes (1/2)
  * <span style="color:#000000">CodeEU\-94: DistanceRun got an unexpected response</span>
  * <span style="color:#000000">CodeEU\-153: DrivingActionAccelerate: Failed to find operating point after Overload</span>
  * <span style="color:#000000">CodeEU\-158: Urban RefLoad DrivingActionAccelerate: Failed to find operating point \(IEPC Wheelhub 1 measured\)</span>
  * <span style="color:#000000">CodeEU\-168: Vecto Declaration Simulation with P1\-Hybrid shows multiple errors \- Simulation Aborts\!</span>
  * <span style="color:#000000">CodeEU\-191: Boosting limits HEV ovc are not working</span>
  * <span style="color:#000000">CodeEU\-202: EMS Standard Values: Continuous/Overload Torque 0 Nm</span>
  * <span style="color:#000000">CodeEU\-203: HybridStrategy error for IHPC type 1 hybrid lorry</span>
  * <span style="color:#000000">CodeEU\-206: VTP \+ PEMS test done by Renault Trucks France \- VECTO tool errors most probably linked to automatic gearbox \(with torque converter\)</span>
  * <span style="color:#000000">CodeEU\-211: Hybrid P1 configurations with errors</span>
  * <span style="color:#000000">CodeEU\-215: MultiStep tool help</span>
  * <span style="color:#000000">CodeEU\-216: NrOfGears in CIF is 1 for IEPC no matter how many gears it has</span>
  * <span style="color:#000000">CodeEU\-220: Error manual transmission 2\. gear</span>
  * <span style="color:#000000">CodeEU\-224: Issue with PEV complete vehicle simulation \(Vecto MultiStage\)</span>
  * <span style="color:#000000">CodeEU\-231: IVECO CONFIDENTIAL : hybrid buses completed simulation aborted</span>
  * <span style="color:#000000">CodeEU\-234: Error Conventional Lorry Gear: 1C DistanceRun got an unexpected response</span>
  * <span style="color:#000000">CodeEU\-235: Tyre error calculation buses "invalid xsi:type 'TyreDataDeclarationType'"</span>
  * <span style="color:#000000">CodeEU\-238: Bus VIF files not valid in Multistep VECTO</span>
  * <span style="color:#000000">CodeEU\-243: Simulation aborted: Gear 5 Lossmap not sufficiant</span>
  * <span style="color:#000000">CodeEU\-244: Signature validation fails for old \(prior to v4\) Manufacturer reports</span>

## Bug fixes (2/2)
  * <span style="color:#000000">CodeEU\-249: Fix handling gear torque limits in case of IEPC WheelHub motor and only one side is measured</span>
  * <span style="color:#000000">CodeEU\-250: Maximum vehicle speed exceeded during pre\-processing</span>
  * <span style="color:#000000">CodeEU\-253: Replace VTP HeavyBus in Generic Vehicles with VTP Truck</span>
  * <span style="color:#000000">CodeEU\-259: Inconsistent calculation of AverageRRC in CIF</span>
  * <span style="color:#000000">CodeEU\-260: Handling of overload buffer in case Continuous Torque is 0Nm</span>
  * <span style="color:#000000">CodeEU\-261: SuperCap internal resistance standard values correction</span>
  * <span style="color:#000000">CodeEU\-263: Determination of rated power for IEPCs for MRF and CIF</span>
  * <span style="color:#000000">CodeEU\-266: Fix Measured Speed Testcases</span>

# Vecto 4.0.1.3217 OFFICIAL RELEASE (23-10-2023)

## Improvements
  * <span style="color:#000000">VTP mode: create zip archive for input and output files</span>

## Bug Fixes
  * <span style="color:#000000">bugfix in CIF for complete\(d\) buses \- do not write PrimaryVehicleSubgroup element</span>
  * <span style="color:#000000">fix hybrid strategy: power comparison</span>
  * <span style="color:#000000">fix exempted vehicles do not work \(error message that XML version is not supported\)</span>
  * <span style="color:#000000">bugfix in XML schema: remove wrong PS technology entry for lorries</span>
  * <span style="color:#000000">bugfix for conditioning power demand for IHPC vehicles</span>

# VECTO 4\.0\.0\.3210 Release Notes (16-10-2023)

![](img%5CRelease%20Notes%20Vecto4x1.png)

<!-- <h3 style="text-align: center; font-style: bold; color:#990000"> Release Notes </h3> -->
<span style="color:#990000"> __Release Notes__ </span>

# First official VECTO release for the 2nd Amendment  of Regulation (EU) 2017/2400

# Features

* __Declaration Mode__
  * __xEV\-Heavy Lorries__
  * __Conventional Medium Lorries__
  * __xEV Medium Lorries__
  * __Conventional Buses__
  * __xEV Buses__
* __Dedicated user interface for simulating buses in Declaration Mode using the multistep approach: VECTOMultistep\.exe__
* __Engineering Mode__
  * __xEV\-Vehicles__
* __Updated XML\-Reports and Vehicle Information Files to support new Vehicle Architectures__
  * __New Manufacturer Record File \(MRF\)__
  * __New Customer Information File \(CIF\)__
  * __Multistep Output \(VIF\):__
* __Updated simulation output \(\.vsum\, \.vmod\)__
* __Updated Hashing Tool to handle new powertrain components \(electric motor\, battery\, supercap \.\.\.\)__
* __Supports \.NET Framework 4\.8 and \.NET 6\.0__

# Features - Tool modes

|  | Medium Lorries (XML) | Heavy Lorries (XML) | Primary buses (XML) | Interim buses  (GUI) | Interim buses (XML) | Completed buses (JSON v7) | Completed buses (GUI) | Completed buses (XML) | Complete buses (JSON v10) | Complete buses (XML) |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| VECTO.exe | S | S | S |   |   | S |   | S | S | S |
| VECTOcmd.exe | S | S | S |   |   | S |   | S | S | S |
| VECTOMultistep.exe | S | S | S | S+E | S+E | S+E | S+E | S+E | S+E | S+E |
| Comments |   |   |   | function: generates updated VIF |  |   |   |   | "Single step" manufacturing process |  |
|  |   |   |   |   |   |   |   |   |   |   |

***S:*** *Simulation only.*

***S+E:*** *Simulation and editing.*

# Changes

* __XML Job Files < v2\.4 are no longer supported__

* __Dropped support for \.NET Framework 4\.5 \(EOL 04/2022\)__

# Implementation of Declaration Mode for xEV-Vehicles

* __Generic auxiliary parametrisation__
* __Generic parametrisation of usable SOC range__
* __Generic parametrisation of Serial/Parallel Hybrid Strategy parameters__
* __Automated simulation of OVC\-HEV__
  * __Charge depleting mode \(CD\): The propulsion energy is provided by the electric storage only__
  * __Charge sustaining mode \(CS\): The propulsion energy is provided by the fuel storage__

# Implementation of Declaration Mode for xEV-Lorries

__Generic E\-PTO for PEV and S\-HEV in Declaration Mode__

__E\-PTO for PEV and S\-HEV in Engineering Mode__

## Implementation of Declaration Mode for Buses

__Factor Method for conventional and xEV\-Buses__

__Updated bus auxiliary model__

