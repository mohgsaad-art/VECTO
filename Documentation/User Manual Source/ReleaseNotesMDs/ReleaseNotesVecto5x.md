# VECTO 5\.x Release Notes

![](img%5CRelease%20Notes%20Vecto4x0.png)

<!-- Cover Slide -->



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




