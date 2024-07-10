# <span style="color:#990000"> __VECTO 4\.x (6-6-2023) Release Notes__ </span>

![](img%5Crelease%20notes%20vecto3x0.png)

<!-- Cover Slide -->

# Vecto 4.0.0.xx  Development Version NOT FOR CERTIFICATION!  June 2023

* <span style="color:#000000"> First fully functional tool version according to the provisions of the 2nd amendment of Regulation \(EU\) 2017/2400. 
* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Dropped support for </span>  <span style="color:#000000">\.Net</span>  <span style="color:#000000"> Framework 4\.5 \(EOL 04/2022\)</span>
    * <span style="color:#000000">Supported </span>  <span style="color:#000000">\.Net</span>  <span style="color:#000000"> versions: </span>  <span style="color:#000000">\.Net</span>  <span style="color:#000000"> Framework 4\.8\, </span>  <span style="color:#000000">\.Net</span>  <span style="color:#000000"> 6\.0</span>
  * <span style="color:#000000">Implementation of Declaration\-Mode for </span>  <span style="color:#000000">xEV</span>  <span style="color:#000000">\-Buses \(see next slides\)</span>
    * <span style="color:#000000">New </span>  <span style="color:#000000">g</span>  <span style="color:#000000">eneric </span>  <span style="color:#000000">vehicles \(XML\)</span>
  * <span style="color:#000000">Bugfixes XML </span>  <span style="color:#000000">schema \(see VECTO DEV WS \#16\, sl\. 4\)</span>
  * <span style="color:#000000">Bugfixes Battery Capacity in MRF/CIF \(see VECTO DEV WS \#16\, sl\. 5\)</span>
  * <span style="color:#000000">Correction of internal resistance in case</span>  <span style="color:#000000"> of standard values for batteries \(see VECTO DEV WS \#16\, sl\. 6\)</span>

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Updated bus auxiliary model (1/2)

* From the existing basis for conventional ICE\-only buses released in summer 2020\, the following elements were implemented/adapted:
  * Multiple bug\-fixes in existing code developed by other contractor _ exact aux power demand does not match anymore with 2020 version_
  * Basic methodology for determining aux power demand of each specific system was kept _ post\-processing of all different systems was correctly implemented from a methodical point of view and completely restructured to be compatible with other elements outside of bus aux model_
  * Different calculation cases for HEV depending on source of electric aux power \(alternator or HV\-REESS\) and also alternator type \(smart or not\) implemented
  * Details regarding methodology for post\-processing can be found in DEV\-WS\#6 \(12\.07\.2021\)

![](img%5Crelease%20notes%20vecto3x1.png)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

* List continued…
  * Electric technologies for all different aux systems
  * HVAC specific:
    * Input data structure for HVAC systems\+ differentiation of systems for cooling and heating
    * Generation of cooling/heating power depending on HVAC system layout and technologies \(handling of mixed configurations\)
  * Methods for smart alternators \(i\.e\. P0\-HEV\)
  * Methods for engine stop\-start and corresponding post\-processing
  * Methods for post\-processing for all electrical auxiliary technologies
  * Methods for auxiliary fuel heater for different powertrain architectures
  * Differentiation in output regarding electrical or mechanical energy demand

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Updated bus auxiliary model - HVAC implementation (1/5)

* Details for HVAC system implementation
  * Generic values for efficiency/COP for all differentcooling/heating systems
  * Generic load share per technology for all possible cases of combination of different heating systems

<span style="color:#FF0000"> _as presented and distributed in DEV\-WS\#12 \(18\.05\.2022\)_ </span>

![](img%5Crelease%20notes%20vecto3x2.png)

<span style="color:#FF0000"> _Example for one specific load distribution case_ </span>

![](img%5Crelease%20notes%20vecto3x3.png)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

<span style="color:#990000"> __Updated bus auxiliary model - HVAC implementation \(2/5\)__ </span>

* Matching of configurations defined in Annex to specific calculation settings
  * Separate consideration of all cases for cooling and heating
  * Configuration defines:
    * If thermal comfort system is available _\(effect on ventilation power demand via different air exchange rate\)_
    * If heat\-pump is available for driver and/or passenger compartment
    * If heat\-pump of passenger compartment supplies also driver compartment
    * Number of heat\-pumps available  _\(effect on limitation of heating/cooling power; more heat\-pumps\, more power\)_

![](img%5Crelease%20notes%20vecto3x4.png)

_NB: Input independent of cooling/heating functionality\!_

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

![](img%5Crelease%20notes%20vecto3x5.wmf)

Matching of HVAC input to specific calculation settings - Valid inputs

_Calculation configuration_  _determined separately and independently_  _for cooling \+ heating_

![](img%5Crelease%20notes%20vecto3x6.wmf)

![](img%5Crelease%20notes%20vecto3x7.wmf)

![](img%5Crelease%20notes%20vecto3x8.png)

<span style="color:#FF0000"> _Table 13 in Annex IX_ </span>

<span style="color:#FF0000"> _\!\!\!_ </span>  <span style="color:#FF0000"> _At least for either cooling or heating number of declared HP types needs to match with declared _ </span>  <span style="color:#FF0000"> _config_ </span>  <span style="color:#FF0000"> _\!_ </span>

<span style="color:#FF0000"> _Configs_ </span>  <span style="color:#FF0000"> _ 8\-10 are matching _ </span>  <span style="color:#FF0000"> _configs_ </span>  <span style="color:#FF0000"> _ 5\-7 just with higher cooling/heating power limitation_ </span>

<span style="color:#FF0000"> _Calculation settings are exactly the same_ </span>

_NB: Input independent of cooling/heating functionality\!_

<span style="color:#FF0000"> _VECTO_ </span>  <span style="color:#FF0000"> _determines_ </span>  <span style="color:#FF0000"> _calculation_ </span>  <span style="color:#FF0000"> _config_ </span>

<span style="color:#FF0000"> _Multiple_ </span>  <span style="color:#FF0000"> _pass\. HP_ </span>

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

* Matching of HVAC input to specific calculation settings - Valid inputs
  * "None"  vs\.  "Not applicable" ?
  * "None" is declared either for heating or cooling HP type if a HP is present in vehicle according to declared Config\, but this HP is not used for one case _\(i\.e\. HP used only for heating or only for cooling and not for both cases\)_
  * "Not applicable" is declared  _only_  for configurations 6 and 10\, since there the driver compartment is supplied by HP of the passenger compartment

<span style="color:#FF0000"> _Table 3a in Annex III_ </span>

![](img%5Crelease%20notes%20vecto3x9.png)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

* HVAC input - valid combinations of different heating systems
  * All other combinations are considered invalid
  * Excel table with specific heating distribution caseswill be distributed together with meeting material

<span style="color:#FF0000"> _Valid combinations of_ </span>  <span style="color:#FF0000"> _heating distribution case_ </span>

![](img%5Crelease%20notes%20vecto3x10.wmf)

<span style="color:#FF0000"> _Example for one specific_ </span>  <span style="color:#FF0000"> _heating distribution case_ </span>

![](img%5Crelease%20notes%20vecto3x11.png)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - General principle

![](img%5Crelease%20notes%20vecto3x12.png)

* First step \(separately for each relevant fuel/energy carrier\): Factor method is applied to calculate energy consumption \(EC in MJ/km\)\, e\.g\.:
  * Conventional vehicle: Diesel or NG only
  * OVC\-HEV: Diesel \(or NG\) and electric energy \(separately for CD and CS mode\)
  * Dual\-fuel vehicle: Diesel and NG
* Second step \(separately for each relevant fuel/energy carrier\): Fuel consumption e\.g\. \[kg/km\] = EC \[MJ/km\] / Lower Heating Value \[MJ/kg\]
* Third step: Sum up CO2 all fuels: CO2 \[g/km\] = FC \[kg/km\] \* CO2 content \[kg/kg\] \* 1000
* For electrical energy cons\. \(EC\_el\): Factor method is applied to the values incl\. battery losses during external charging

![](img%5Crelease%20notes%20vecto3x13.png)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - Required number of simulations

* Primary vehicle
  * Simulations for four sub\-groups\(LF/HF x SD/DD\)
  *  11 x 2 =  __22 simulations__
* Completed vehicle
  * 2 to 4 cycles x 2 payloadsx 2 \(spec / gen body\)
  *   __min 8 __ to  __max 16 simulations__
* For OVC HEVs:
  * Additional simulations for CD and CS mode\, and \- for P\-HEVs - to determine the EF to be considered
  * "Worst case" = complete OVC P\-HEV: __ 152 simulations__

![](img%5Crelease%20notes%20vecto3x14.wmf)

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - Generic bodies for primary xEV

* Basis of the methods for xEV are the generic bodies for conventional\(i\.e\. ICE only\) buses
* Modified parameters are:
  * Vehicle mass
  * Auxiliary configuration

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - Generic masses for primary xEV

Approach already discussed in DEV Workshops \#13 and \#14

EM considered twice in case of S\-HEV

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - HVAC and AUX config for primary xEV

| Parameter | Door drive technology | HVAC/SystemConfiguration | HVAC/ HeatPumpTypeDriverCompartmentCooling | HVAC/ HeatPumpTypeDriverCompartmentHeating | HVAC/ HeatPumpTypePassengerCompartmentCooling | HVAC/ HeatPumpTypePassengerCompartmentHeating | HVAC/AuxiliaryHeaterPower | HVAC/WaterElectricHeater |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| ID | P291 | P317 | P318 | P319 | P320 | P321 | P322 | P328 |
| Type | string | int | string | string | string | string | int | bool. |
| Unit | [-] | [-] | [-] | [-] | [-] | [-] | [-] | [-] |
| Description | Allowed values: 'pneumatic', 'electric', 'mixed' | Allowed values: '0' to '10' In the case of an incomplete HVAC system, '0' shall be provided. '0' is not applicable for complete or completed vehicles. | Allowed values: 'none', 'not applicable', 'R-744', 'non R-744 2-stage', 'non R-744 3-stage', 'non R-744 4- stage', 'non R-744 continuous' 'not applicable' shall be declared for HVAC system configurations 6 and 10 due to supply from passenger heat pump | Allowed values: 'none', 'not applicable', 'R-744', 'non R-744 2-stage', 'non R-744 3-stage', 'non R-744 4- stage', 'non R-744 continuous' 'not applicable' shall be declared for HVAC system configurations 6 and 10 due to supply from passenger heat pump | Allowed values: 'none', 'R-744', 'non R-744 2-stage', 'non R-744 3-stage', non R-744 4-stage', 'non R-744 continuous' In the case of multiple heat pumps with different technologies for cooling the passenger compartment, the dominant technology shall be declared (e.g. according to available power or preferred usage in operation) | Allowed values: 'none', 'R-744', 'non R-744 2-stage', 'non R-744 3-stage', non R-744 4-stage', 'non R-744 continuous' In the case of multiple heat pumps with different technologies for heating the passenger compartment, the dominant technology shall be declared (e.g. according to available power or preferred usage in operation) | Enter '0' if no auxiliary heater is installed. | Input to be provided only for HEV and PEV |
| Conventional | pneumatic | 6 | not applicable | none | non R-744 2-stage | none | LF: 15000 HF: 30000 | - |
| HEV | pneumatic | 6 | not applicable | none | non R-744 2-stage | none | LF: 15000 HF: 30000 | False |
| PEV | pneumatic | 6 | not applicable | not applicable | non R-744-continuous | non R-744 continuous | LF: 0 HF: 0 | True |

HVAC Configuration 6: Thermal comfort system; Heat pump for passenger compartment; No independent heat pump for driver compartment

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Factor Method - OVC-HEVs (1/2)

* Approach for energy consumption „EC" \[MJ/km\]
  * Applied both for fuel and electric energy
  * Fuel consumption in grams / liters and CO2 emissions are derivates of those

| CD | ECcompl,specBody,CD |
| :-: | :-: |
| CS | ECcompl,specBody,CS |
| Weighted | not calculated |

| CD | ECcompl,CD = ECprim,CD x (ECcompl,specBody,CD / ECcompl,genBody,CD) |
| :-: | :-: |
| CS | ECcompl,CS = ECprim,CS x (ECcompl,specBody,CS / ECcompl,genBody,CS) |
| Weighted (CD, CS) | ECcompl,weighted = ECcompl,CD x UF + ECcompl,CS x (1-UF) |

| CD | ECprim,CD |
| :-: | :-: |
| CS | ECprim,CS |
| Weighted | ECprim,weighted= ECprim,CD x UF + ECprim,CS x (1-UF) |

| CD | ECcompl,genBody,CD |
| :-: | :-: |
| CS | ECcompl,genBody,CS |
| Weighted | not calculated |

Results for primary vehicle\(similar to "single step" OVC\)

__Results for complete\(d\) vehicle__

Factor method simulations at the complete\(d\) vehicle stage

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

* Approach for electric ranges
  * Results for completed vehicle can simply be calulated based on ECcompl\,CD as calculated above
* Approach for vehicle speeds and other simulation related results reported in the MRF \(nr\. of gear shifts\, % full\-load etc\.\)
  * Results for completed vehicle to be taken from simulations for completed vehicle with specific body

| CD | vcompl,specBody,CD |
| :-: | :-: |
| CS | vcompl,specBody,CS |
| Weighted | not required |

| CD | vcompl,CD = vcompl,specBody,CD |
| :-: | :-: |
| CS | vcompl,CS = vcompl,specBody,CS |
| Weighted (CD, CS) | not required |

| CD | vprim,CD |
| :-: | :-: |
| CS | vprim,CS |
| Weighted | not required |

| CD | vcompl,genBody,CD |
| :-: | :-: |
| CS | vcompl,genBody,CS |
| Weighted | not required |

__Results for complete\(d\) vehicle__

Results for primary vehicle

Factor method simulations at the complete\(d\) vehicle stage

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Results for fuel-fired heater

* For vehicles with ICE:
  * the fuel consumed by a fuel\-fired heater is added to the fuel consumption of the ICE
  * the input data for the fuel\-fired heater does not specify the fuel with which it is operated\. VECTO assigns by default the fuel of the ICE
  * in the factor method\, this part of the fuel consumption is implicitly part of the "factor formula"
  * nearly no impact on overall result \(only 0\,5% weighting of \-20°C climate scenario\)
* For PEV:
  * if a heater is declared to be present on the complete\(d\) vehicle\, automatically "Diesel" is allocated
  * the fuel consumption / CO2 emissions is reported separately \(to not come across with the emissions in accordance with the ZEV definition\, which is - so far - only linked to propulsion system\)
  * in the factor method\, the result is taken from the simulation for the completed vehicle with the specific body

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Overview VECTO Tool Modes

|   | Medium Lorries (XML) | Heavy Lorries (XML) | Primary buses (XML) | Interim buses  (GUI) | Interim buses (XML) | Completed buses (JSON v7) | Completed buses  (GUI) | Completed buses (XML) | Complete buses (JSON v10) | Complete buses (XML) | Single bus (JSON v6 + 2 XMLs) |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| VECTO.exe | S | S | S |   |   | S |   | S | S | S | S |
| VECTOcmd.exe | S | S | S |   |   | S|   | S | S | S | S |
| VECTOMultistep.exe | S | S | S | S+E | S+E | S+E | S+E | S+E | S+E | S+E | S+E |
| Comments |   |   |   | function: generates updated VIF |  |   |   |   | "Single step" manufacturing process |  | for validation of factor method |

***S:*** *Simulation only.*

***S+E:*** *Simulation and editing.*

<span style="color:#990000"> __Vecto 4\.0\.0\.xx  Development Version__ </span>

# Known issues

* Elements not yet implemented
  * Battery connectors / junction box not included define and implement generic additional resistances \(i\.e\. loss factors\)
  * Technical elements as resulting from the revision of the CO2 Standards to be added
    * Sub\-group allocation for the for the newly covered vehicle groups
    * Generation of weighted results for vocational vehicles
    * Anything related to ZEV definition?
* Elements still under discussion
  * Medium lorries mission profile and payload weighting factors \(equally weighted\, only preliminary\)
  * Multiple SOC level\(s\) for VECTO PEV simulation and respective weighting of results \(?\)

# Vecto 0.7.10.2996  Development Version  March 2023

* <span style="color:#000000"> __Important Note__ </span>  <span style="color:#000000">This is a development release for </span>  <span style="color:#000000">xEV</span>  <span style="color:#000000">\-Lorries\. Bus related functionalities may be broken\. </span>  <span style="color:#000000"> </span>
* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Test settings:</span>
    * <span style="color:#000000">Charge\-Sustaining\-Mode:</span>
      * <span style="color:#000000">Iterative </span>  <span style="color:#000000">simulation for OVC\-P\-HEVs </span>
      * <span style="color:#000000">	can be disabled</span>
    * <span style="color:#000000">Charge</span>  <span style="color:#000000">\-</span>  <span style="color:#000000">Depleting\-</span>  <span style="color:#000000">Mode:</span>
      * <span style="color:#000000">Central SOC can be overridden for </span>
      * <span style="color:#000000">	</span>  <span style="color:#000000">Charge</span>  <span style="color:#000000">\-Depleting\-Mode </span>
  * <span style="color:#000000">Declaration values can be overridden by copying the particular file to <</span>  <span style="color:#000000">VectoDir</span>  <span style="color:#000000">>\\Declaration\\Override and changing the desired value\.</span>
  * <span style="color:#000000">New XSD\-Schema for XML Reports</span>
  * <span style="color:#000000">Updated Hybrid </span>  <span style="color:#000000">Strategy and </span>  <span style="color:#000000">Gearshifting</span>

![](img%5Crelease%20notes%20vecto3x15.png)

# Declaration Mode Lorries - Overview (1/2)

* VECTO Development Version  <span style="color:#FF0000">0\.7\.10\.2996</span>  as released on  <span style="color:#FF0000">March 15</span> \, features the Declaration Mode for Lorries according to the 2nd Amendment\.
* This enables the official output XMLs \(MRF\, CIF\) to be generated using the official input XMLs\.
* The result values correspond to the official results\, subject to minor adjustments and additions from the testing period \(see later slides\)\.
* With this version\, the Declaration Mode can also be calculated via the GUI \(json \+ csv\)\. This mode also contains some special features for the upcoming test phase \(see later slides\)\.

* Overview new elements
  * Generic parameterisation of auxiliaries
  * Generic parametrisation of P\-HEV strategy including update of gear shift strategy
  * Generic parametrisation of S\-HEV strategy
  * Generic parametrisation of usable SOC range
  * Separate approach for „Charge depleting mode" simulation
  * Calculation of energy consumption at "battery terminals" considering battery charging losses during external charging
  * Range calculations
  * Automation of the various simulation runs necessary for PHEVs plus post processing elements
  * Generic e\-PTO cycle for PEV and S\-HEV in the MUN cycle
  * Generation of MRF and CIF

# DECL Lorries - Generic parameterisation of auxiliaries

* Basic principles of generic data used in DECL
  * Relevant auxiliary systems:
    * Pneumatic system
    * ICE cooling fan \+ conditioning power for electric propulsion components
    * Steering system
    * HVAC: for lorries only "yes/no"
    * Electric board net \(is called "electric system"\): for lorries only headlights technology \(LED or standard\)
  * Respective tables with generic power demand for each auxiliary system can be found in specific subfolder "Declaration\\VAUX" under VECTO main path as csv\-files
  * General table structure looks as follows:

<span style="color:#CC0000"> __can__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __be__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __directly__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __supplied__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __by__ </span>  <span style="color:#CC0000"> __ REESS __ </span>  <span style="color:#CC0000"> __if__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __present__ </span>  <span style="color:#CC0000"> __\(__ </span>  <span style="color:#CC0000"> __indicated__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __with__ </span>  <span style="color:#CC0000"> __ 0/1\)__ </span>

<span style="color:#CC0000"> __cycle specific __ </span>  <span style="color:#CC0000"> _mechanical_ </span>  <span style="color:#CC0000"> __ power demand \[W\]__ </span>

<span style="color:#CC0000"> __applicability__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __for__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __architecture__ </span>  <span style="color:#CC0000"> __\(__ </span>  <span style="color:#CC0000"> __indicated__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __with__ </span>  <span style="color:#CC0000"> __ 0/1\)__ </span>

| Technology                              | conventional | P-HEV | S-HEV | PEV |  fully electric  |  Long haul  |  Regional delivery  |  Urban delivery  |  Municipal utility  |  Construction  |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Tech #1 |  |  |  |  |  |  |  |  |  |  |
| Tech #2 |  |  |  |  |  |  |  |  |  |  |
| Tech #3 |  |  |  |  |  |  |  |  |  |  |
| Tech #... |  |  |  |  |  |  |  |  |  |  |

* Basic principles of generic data used in DECL
  * Details of application in DECL:
    * _For all HEV:_  "fully electric" technology is connected to REESS mechanical power \(i\.e\. respective alternator drive power\) is converted to electric power with generic alternator efficiency of 0\.7 used consistently throughout whole VECTO auxiliary approach\(i\.e\. mechanical power from table is multiplied by 0\.7 and therefore reduced\)
    * _For all PEV:_  only "fully electric" technology can be declared \(acc\. to Annex specifications as well\) same principle of conversion from mechanical to electrical power \(as explained above applies\)

<span style="color:#CC0000"> __can__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __be__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __directly__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __supplied__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __by__ </span>  <span style="color:#CC0000"> __ REESS __ </span>  <span style="color:#CC0000"> __if__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __present__ </span>  <span style="color:#CC0000"> __\(__ </span>  <span style="color:#CC0000"> __indicated__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __with__ </span>  <span style="color:#CC0000"> __ 0/1\)__ </span>

<span style="color:#CC0000"> __applicability__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __for__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __architecture__ </span>  <span style="color:#CC0000"> __\(__ </span>  <span style="color:#CC0000"> __indicated__ </span>  <span style="color:#CC0000"> __ __ </span>  <span style="color:#CC0000"> __with__ </span>  <span style="color:#CC0000"> __ 0/1\)__ </span>

<span style="color:#CC0000"> __cycle specific __ </span>  <span style="color:#CC0000"> _mechanical_ </span>  <span style="color:#CC0000"> __ power demand \[W\]__ </span>

| Technology                              | conventional | P-HEV | S-HEV | PEV |  fully electric  |  Long haul  |  Regional delivery  |  Urban delivery  |  Municipal utility  |  Construction  |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Tech #1 |  |  |  |  |  |  |  |  |  |  |
| Tech #2 |  |  |  |  |  |  |  |  |  |  |
| Tech #3 |  |  |  |  |  |  |  |  |  |  |
| Tech #... |  |  |  |  |  |  |  |  |  |  |

* Basic principles of pneumatic system in DECL
  * Structure of table is slightly different\, stating mechanical and electrical power demandat the same time
  * Reason is that generic saving of AMS techonolgies is reduced for xEV vehicles \(only minus 10%\) due to reduced recuperative potential compared to conventional ones

<span style="color:#CC0000">can</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">be</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">directly</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">supplied</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">by</span>  <span style="color:#CC0000"> REESS </span>  <span style="color:#CC0000">if</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">present</span>  <span style="color:#CC0000">\(</span>  <span style="color:#CC0000">indicated</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">with</span>  <span style="color:#CC0000"> 0/1\)</span>

<span style="color:#CC0000"> __cycle specific indication of power demand \[W\]__ </span>

<span style="color:#CC0000">applicability</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">for</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">architecture</span>  <span style="color:#CC0000">\(</span>  <span style="color:#CC0000">indicated</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">with</span>  <span style="color:#CC0000"> 0/1\)</span>

| Technology                              | conventional | P-HEV | S-HEV | PEV |  fully electric  |  mechanical power demand [W] |  electrical power demand [W] |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Tech mechanical #1 |  |  |  |  |  |  |  |
| Tech mechanical #2 |  |  |  |  |  |  |  |
| Tech mechanical #3 |  |  |  |  |  |  |  |
| Tech mechanical #... |  |  |  |  |  |  |  |
| Tech electrical #1 |  |  |  |  |  |  |  |
| Tech electrical #2 |  |  |  |  |  |  |  |
| Tech electrical #3 |  |  |  |  |  |  |  |
| Tech electrical #... |  |  |  |  |  |  |  |

<span style="color:#CC0000">Block </span>  <span style="color:#CC0000">of</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000"> __mechanical__ </span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">technologies</span>

<span style="color:#CC0000">Block </span>  <span style="color:#CC0000">of</span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000"> __electrical__ </span>  <span style="color:#CC0000"> </span>  <span style="color:#CC0000">technologies</span>

* Basic principles of ICE fan and electric components conditioning power in DECL
  * ICE fan power is always fully applied when ICE = on in simulation
  * _For PEV and S\-HEV:_  values from table below are fully applied when at least one EM = on in simulation \(i\.e\. electric powertrain is propelling/braking\) or when E\_PTO = on
  * _For P\-HEV:_  values from table below are applied according to equations further belowwith scaling factor "x" reflecting actual split of propulsion power for each timestep
    * x = ABS\(PEM\) / \[ABS\(PEM\) \+ ABS\(PICE\)\] with and PEM and PICE being the mechanical power of EM and ICE
    * Applied electric components conditioning power equals: "table value" multiplied by x
    * For further details please refer to  _VECTO\-DEV WS\#12 18\.05\.2022 \(slide 15ff\)_

__Fan\-Tech\.csv \+ Cond\-Table\.csv__

| Electric conditioning power demand applied to electric system [W] |  |  |  |  |
| :-: | :-: | :-: | :-: | :-: |
|  Long Haul  |  Regional Delivery  |  Urban  Delivery  |  Municipal utility  |  Construction |
| 600 | 600 | 550 | 550 | 600 |

* Basic principles of steering system in DECL
  * Foundation of approach is that basic power demand defined per cycle ismultiplied by technology dependent scaling factors \(CFs\)
  * 3 sub\-groups of power demands defined:
    * U&F: unloaded and friction
    * B: banking
    * S: steering
  * Total steering power acc\. to equation below is calculated separately formechanical and electrical stering power demand:
    * All CFs are averaged over all steered axles with electric technology
    * All CFs are averaged over all steered axles with mechanical technology
  * If all axles are electric technologies\, total steering power is applied as electrical power to REESS
  * With at least one mechanical techn\. present\, total steering power is applied as mechanical load

__SP\-Tech\.csv \+__  __SP\-Table\.csv \+__  __SP\-Axles\.csv__

![](img%5Crelease%20notes%20vecto3x16.png)

* Basic principles of HVAC system in DECL
  * _Conventional _  _and_  _ P\-HEV:_
    * HVAC load is applied as mechanical auxiliary
  * _S\-HEV _  _and_  _ PEV:_
    * HVAC load is applied as electrical auxiliary
    * Conversion from mechanical to electrical is done by dividing mechanical base power by generic efficiency of 0\.8

* General remark about post\-processing of "missing" auxiliary power demand for mechanically driven auxiliaries in timesteps where the ICE is off:
  * These topic is related to "Utility Factor \(UF\) for Engine\-Stop\-Start \(ESS\)"
  * For lorries only the very simple case is relevant \(application case 1\)
  * Basic rule: for auxiliaries operatedindependently of ICE status energy demandover cycle is the same with missing portion being accounted for in post\-processing
  * For further details please refer to: _VECTO\-DEV WS\#6 12\.07\.2021 \(slide 7ff\)_

![](img%5Crelease%20notes%20vecto3x17.png)

# DECL Lorries - Generic parametrisation P-HEV strategy

* Operation in Charge sustaining \(CS\) mode
  * Fixed number of 3 simulations to determine optimal strategy parameters done automatically
  * _1_  _st_  _ run:_  generic start value for equivalence factor \(EF\) depending on vehicle group\, mission\, payload and usable SOC range applied
    * refer to \\Declaration\\HEVParameters\\Lorry\\HEV\_Strategy\_Parameters\_fequiv\_ <span style="color:#FF0000"> __XX__ </span> soc\.csv
    * <span style="color:#FF0000"> __XX__ </span>  = 10\, 20 or 40 depending on usable SOC range in percent \(interpolation\, no extrapolation\)
  * _2_  _nd_  _ run:_  from resulting ∆SOC1 of 1st run \(∆SOC = SOCEnd - SOCStart\) new EF is calculated according to following equations:
  * _3_  _rd_  _ run:_  from resulting ∆SOC2 of 2nd run new equivalence factor is calculated according to following equation:
  * For further details please refer to:  _VECTO\-DEV WS\#8 16\.11\.2021 \(slide 7ff\)_

<span style="color:#C00000">refer to \\Declaration\\</span>  <span style="color:#C00000">HEVParameters</span>  <span style="color:#C00000">\\Lorry\\Gradient\_40\.csv</span>

<span style="color:#C00000">equivalence factor used in 1</span>  <span style="color:#C00000">st</span>  <span style="color:#C00000"> simulation run</span>

![](img%5Crelease%20notes%20vecto3x18.png)

* Operation in Charge sustaining \(CS\) mode
  * Dedicated GUI feature available in order to overrule 3 iterations and perform calculation with single individual EF only \(for engineering exercise\)
  * Instructions:
    * Copy files from "\\Declaration\\HEVParameters\\Lorry\\HEV\_Strategy\_Parameters\_fequiv\_ <span style="color:#FF0000"> __XX__ </span> soc\.csv" to "\\Declaration\\Override\\" without adding any sub\-directories
    * Adapt respective values for individual EF in files \(per vehicle group\, mission profile and payload\)		    _ 1_  _st_  _ value for low loading\, 2_  _nd_  _ for reference loading_

| vehiclegroup                  |  longhaul                      |  regionaldelivery               |  urbandelivery                  |  municipalutility             |  construction                   |
| :-: | :-: | :-: | :-: | :-: | :-: |
| 53 |                                  |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 54 |                                  |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 1s                               |                                  |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 1 |                                  |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 2 |  0.1/0.1                               |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 3 |                                  |  2.6/2.3                               |  2.7/2.3                               |                                  |    |
| 4 |  1.4/1.3                               |  2.2/2.6                               |  1.9/2.3                               |  2.1/2.1                               |  2.1/2.1 |
| 5 |  1.4/1.3                               |  2.2/2.6                               |  1.9/2.3                               |                                  |  2.1/2.1 |
| … | … | … | … | … | … |

* Operation in Charge sustaining \(CS\) mode
  * Dedicated GUI feature available in order to overrule 3 iterations and perform calculation with single individual EF only \(for engineering exercise\)
  * Instructions:
    * Step 3:Activate GUI feature under "Options" tabto apply single run with individually set EFloaded from "\\Declaration\\Override\\" folder
    * _ATTENTION:_  Only loaded when VECTO islaunched initially \(updates during activeVECTO session have no effect\)\!
* To be checked: Handling of vehicles with very small batteries in LH _\(due to high amount of energy being captured at the very end of the cycle\)_

![](img%5Crelease%20notes%20vecto3x19.png)

# Updated P-HEV gear shift model (DECL + ENG)

* Gear shift model for P\-HEV used slightly wrong ICE operation point for evaluation of fuel costs mainly relevant for overdrive drivetrain configurations effect for non\-overdrive drivetrains:
  * Typical LH application: from slight decrease <1% up to slight increase <1% of HEV FC
  * Typical dynamic \(urban\) application:
    * More realistic \(higher\) acceleration behavior and better following of speed trace like ICE\-only counterpart
    * Still some percent decrease of FC of HEV compared to previous version
* In actual simulation nothing was changed\, the change only affects methods used for cost evaluation

* Class 4 vehicle on RD reference load;
* Gearbox with direct gear \(3% higher η\) and overdrive;
* Much better results after the update compared to the previous implementation;

| Test | Consumption [-] | Direct gear usage [% time] | Overdrive gear usage [% time] |
| :-: | :-: | :-: | :-: |
| ICE  | 1 | 42% | 26% |
| Direct + overdrive | 0.965 | 7% | 68% |
| Direct only | 0.895 | 78% | - |
| Direct + overdrive | 0.870 | 57% | 12% |

# Updated time-based with gear mode for AT (ENG only)

* Several manufacturers reported that the TC can be briefly active in higher gears - mainly during gearshifts;
* Allow input of "TC active" signal in vdri in this mode;
* Changes apply only in engineering mode\. Included in the upcoming DEV release;

# DECL Lorries - Generic parametrisation usable SOC range

* Actual SOC range used for simulation is determined according to 3 elements:
  * Generic SOC range for new vehicle
    * Defined separately for PEV\, OVC\-HEV\, Non\-OVC\-HEV \(NOVC\-HEV\)
  * Declared SOC range for new vehicle acc\. to Annex III \( _P413 \+ P414_ \)
  * Deterioration factor applied for half of lifetime
    * Preliminary assumption: 5% decrease of usable SOC range\(i\.e\. 0\.95 x SOC\-range of new vehicle\)
  * Preliminary figures in table below:
* Further discussions regarding final values ongoing see separate agenda point

![](img%5Crelease%20notes%20vecto3x20.wmf)

# DECL Lorries - Separate approach „CD mode" simulation

* „Frozen" SOC \(infinity battery\) at „SOCcenter" applied\(i\.e\. average of actual SOCmax and SOCmin\) _ see previous slide_
* GUI feature to set frozen SOC to othervalues than „center SOC" \( _see figure_ \)
  * Value needs to be between actual SOCmax and SOCminfor each individual battery installed
* Influence factors on result:
  * Limitation of maximum battery power = f\(SOC\)
  * Internal resistance = f\(SOC\) \(i\.e\. battery losses\)
* Discussions ongoing which SOC level\(s\) shall be considered for final method see separate agenda point

![](img%5Crelease%20notes%20vecto3x21.png)

# DECL Lorries - Calculation EC at "battery terminals"

* Case PEV:
  * Only re\-charging in depot relevant for charging losses\(since no detailled information in charging behaviour is available\)
  * Power level for determining charging losses in battery defined by following equation: nominal charging power in kW =  _m_  _ax\(10\, \(usable SOC in kWh\) / 6h\)_
    * Usable SOC is determined by integration of OCV\-data from actual SOCmin to SOCmax
  * Charging efficiency is determined as follows:
    * Nominal charging current:Charging power above is divided by OCV at SOCcenter \(i\.e\. average of actual SOCmax and SOCmin\)
    * Charging losses: nominal charging current and internal resistance at SOCcenter
    * Charging efficiency: 1 - \(charging losses / charging power\)

* Case OVC\-HEV :
  * Charging in  _depot_  as well as  _stationary_  _ _  _during_  _ _  _mission_  \(acc\. to generic usage data\) relevant for charging losses
  * Power level for determining charging losses in battery defined as follows:
    * _Depot:_
      * same as for PEV \(see previous slide\)
      * but limited with declared maximum charging power acc\. to Annex III \( _P402\)_
    * _Stationary_  _ _  _during_  _ _  _mission_  _:_
      * determined from matrix of generic usage data as already defined for OVC\-UF calculation \(function of vehicle group and mission profile\)
      * limited with declared maximum charging power acc\. to Annex III \( _P402\)_  _ _
  * 2 separate values of charging efficiency determined as described on previous slide
    *  weighted averaging by amount of charged energy for depot and stationary during mission to determine final charging efficiency

* Case OVC\-HEV calculation example:
  * Charging power levels:  Depot 10 kW  /  Stationary during mission 250 kW
  * Battery related data and charging currents:
  * Charging losses and efficiencies:
  * Weighted final charging efficiency:

![](img%5Crelease%20notes%20vecto3x22.wmf)

![](img%5Crelease%20notes%20vecto3x23.wmf)

**Energy charged in depot limited by:**

* _Usable_  _ _  _battery_  _ _  _capacity_

* _Real world usage factor \(0\.75\)_

**Energy charged during mission limited by:**

* _Max\. charging power_
* _Duration of event_
* _Usable battery capacity_
* _Number of charging events_
* _Real world usage factor \(0\.5\)_

![](img%5Crelease%20notes%20vecto3x24.wmf)

For further details please refer to:  _VECTO\-DEV WS\#9 11\.01\.2022 \(slide 23ff\)_

_Excel will be distributed together with meeting material_

# DECL Lorries - Automation of OVC-HEV simulation

* OVC\-HEV are HEV with external charging feature \("Plug\-in HEV"\)
  * Declared acc\. to Annex III \( _P401\)_
* 2 operation modes for OVC\-HEV defined in VECTO:
  * Charge depleting mode \(CD\) where the propulsion energy is provided by the electric storage only _ results: _  _kWh\_el_  _/km\, electric range_
  * Charge sustaining mode \(CS\) where the propulsion energy is provided by the fuel storage _ SOC is balanced over complete cycle\, thus no electric energy consumption_  _    \(_  _kWh\_el_  _/km = 0 per definition\!\)_

* Separate simulation runs are performed in VECTO for each mode automatically
* Separate results for CD and CS are weighted for final result based on utility factor \(UF\)
  * UF is defined as share of electric range in CD mode on total daily mileage of each mission profile
    * electric range is defined by:
      * Electric energy consumption in the specific mission in \[kWh/km\] as result from the simulation
      * Usable electric energy\, which in turn is defined by:
        * battery capacity due to initial SOC
        * generic assumptions for re\-charging during each mission\(number and duration of charging events\, available charging power from infrastructure\)
  * Weighted results \(fuel or energy consumption\) are calculated according to the following equation:

![](img%5Crelease%20notes%20vecto3x25.png)

# DECL Lorries - Range calculations

* Overview of different ranges in result matrix in VECTO

| Electric range | Definition |
| :-: | :-: |
| Actual charge depleting range<br />(RCDA) | The range that can be driven in charge depleting mode based on the usable amount of REESS energy, without any interim charging. (Annex IV) |
| Equivalent all electric range<br />(EAER) | The part of the actual charge depleting range that can be attributed to the use of electric energy from the REESS, i.e. without any energy provided by the non-electric propulsion energy storage system. (Annex IV)<br />i.e. RCDA mathematically reduced by energy content of fuel used (based on fuel consumption in charge sustaining mode) |
| Zero CO2 emissions range<br />(ZCER) | The range that can be attributed to energy provided by propulsion energy storage systems considered with zero CO2 impact. (Annex IV)<br /> |

# DECL Lorries - Automation of OVC-HEV simulation

Schematic overview of result matrix in VECTO for CD \+ CS mode

![](img%5Crelease%20notes%20vecto3x26.wmf)

Comments:

* General structure given by XML schema structure
* The "final results" for each mission profile and payload case are always output in the XML element "Total"
* FC\, CO2\, EC in XML element "Total" are weighted with UF
* Ranges in XML element "Total" are identical to results from CD mode for OVC\-HEV
* Zero CO2 emissions range equals EAER for OVC\-HEV

![](img%5Crelease%20notes%20vecto3x27.wmf)

For further details please refer to:  _VECTO\-DEV WS\#13 04\.07\.2022 \(slide 12ff\)_

_Declared_  _ / _  _Component_  _ _  _data_

_„_  _Raw_  _" _  _results_  _ _  _by_  _ VECTO_

_Calculation_  _ _  _of_  _ different _  _specific_  _ _  _ranges_  _ _  _from_  _ _  _data_  _ _  _above_

![](img%5Crelease%20notes%20vecto3x28.wmf)

_Relevant _  _for_  _ CO2 Standards Regulation\, not _  _for_  _ _  _weighting_  _ _  _of_  _ CD\+CS _  _results_

![](img%5Crelease%20notes%20vecto3x29.wmf)

_Calculation_  _ _  _of_  _ UF _  _specific_  _ _  _ranges_  _ _  _from_  _ _  _consumption_  _ _  _figures_  _ \(_  _previous_  _ _  _slide_  _\) _  _and_  _ _  _generic_  _ _  _data_  _ _  _above_

_R_  _CDA_  _ _  _corrected_  _ _  _by_  _ real _  _world_  _ _  _factor_

![](img%5Crelease%20notes%20vecto3x30.wmf)

_Due _  _to_  _ _  _re\-charging_  _ _  _during_  _ _  _mission_

_Calculation_  _ _  _of_  _ UF _  _as_  _ _  _share of electric range in CD mode on total daily mileage \(163\.9 \+ 32\.0 = 195\.8 / 480\)_

![](img%5Crelease%20notes%20vecto3x31.wmf)

![](img%5Crelease%20notes%20vecto3x32.png)

# DECL Lorries - Generic e-PTO in MUN cycle

* e\-PTO applied for PEV and S\-HEV in the MUN cycle \(generic refuse body\)
* e\-PTO cycle Pel\,PTO = f\(t\) derived from:
  * generic hydraulic pump cycle for ICE and the allocated  ICE high idle speed
  * assuming an average e\-PTO efficiency of 80%
* Calculation is included time\-resolved in the vmod file
* Electric energy consumption is considered in electric range for MUN cyle

# DECL Lorries - Known issues with current version

* Elements not yet implemented
  * Battery connectors / junction box not included define and implement generic additional resistances \(i\.e\. loss factors\)
  * Generic P\-HEV strategy: To be tested whether an alternative post\-processing method for vehicles with very small batteries is needed as back\-up
  * Technical elements as resulting from the revision of the CO2 Standards to be added
    * Sub\-group allocation for the for the newly covered vehicle groups
    * Generation of weighted results for vocational vehicles
* Elements still under discussion
  * Medium lorries mission profile and payload weighting factors \(equally weighted\, only preliminary\)
  * Generic SOC min/max ranges and deterioration \(only preliminary values\)
  * Multiple SOC level\(s\) for VECTO PEV simulation and respective weighting of results

# DECL Lorries - Testing requests and organ. of feedback

* Testing requests:
  * Test configurations relevant in your portfolio
  * Specific analysis regarding open issues as mentioned on slide "Known issues with current version"
* Feedback
  * Bug reports\, questions etc\.: please  _ONLY use JIRA_  \(no specific emails regarding those topics in the upcoming period\)
  * If there is an urgent need for a meeting before the next DEV workshop \(e\.g\. because of a special problem\)\, this will be announced separately\.
  * Bug fixing and support
  * TUG will focus on finalising the DECL for buses in the coming weeks\.
  * Topics for the DECL for lorries will be collected and sorted by priority\.
  * Unless urgent bugfix releases for blocking issues\, there will be no new release before May\.

# Vecto 0.7.9.2864 JRC Development Version  November 2022

* <span style="color:#000000"> __Important Note__ </span>  <span style="color:#000000">This is a special development release by JRC to validate the time\-runs for BEVs\. </span>  <span style="color:#000000"> </span>
* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Implementation of measured speed cycle for BEVs \(E2\, E3\, E4\, IEPC\)</span>
  * <span style="color:#000000">Implementation of measured speed with gear for BEVs \(E2\, IEPC\)</span>
  * <span style="color:#000000">Implementation of Pwheel mode for BEVs \(E2\, E3\, E4\, IEPC\)</span>  <span style="color:#000000">In this type of cycle\, the input field </span>  <span style="color:#000000"> __\<n>__ </span>  <span style="color:#000000"> stands for the speed of the electric motor\.</span>
  * <span style="color:#000000">The E2 vehicle job "Generic Vehicles\\Engineering Mode\\GenericVehicleE2\\BEV\_ENG\_timeruns\.vecto" is preconfigured to run time\-run cycles\.</span>

# Vecto 0.7.9.2741 Development Version June 2022

* <span style="color:#000000"> __Changes/Bugfixes__ </span>
  * <span style="color:#000000">Implementation of IEPC</span>
  * <span style="color:#000000">Implementation of IEPC\-S</span>
  * <span style="color:#000000">Implementation of IHPC</span>
  * <span style="color:#000000">Adaptations HEV\-S strategy\, SoC correction for HEV\-S</span>
  * <span style="color:#000000">PTO idle drag losses for PEV\, adapting PTO idle losses according to 2nd amendment</span>
  * <span style="color:#000000">Bugfixes</span>
  * <span style="color:#000000">Hashing tool crashes in \.net version 6\.0</span>
  * <span style="color:#000000">Corrected length for BusAux volume calculation</span>

# Modeling of IEPC component

* <span style="color:#000000">IEPC is modelled as APT\-N transmission and electric machine</span>
  * <span style="color:#000000">Using already available models\, same shift strategy as for E2/S2</span>
  * <span style="color:#000000">Simplified transmission in case of single\-speed IEPC</span>

![](img%5Crelease%20notes%20vecto3x33.png)

<span style="color:#000000"> __\_int __ </span>  <span style="color:#000000"> __signals refer to the out\-shaft of the electric motor__ </span>

# Peculiarities of IEPC

* <span style="color:#000000">Model parameters \(max torque\, power maps\) refer to IEPC out\-shaft</span>
  * <span style="color:#000000">Internally converted to electric motor</span>
* <span style="color:#000000">Overload parameters are measured in the gear with the </span>  <span style="color:#000000">ratio closest to 1 </span>
* <span style="color:#000000">2 options for drag curve acc\. to Annex Xb:</span>
  * <span style="color:#000000">A single drag curve is provided </span>  <span style="color:#000000">\(measured in gear with ratio closest to 1\)</span>
  * <span style="color:#000000">Drag curve for every gear is provided</span>

# Input data for IEPC component

* <span style="color:#000000">IEPC powertrain architecture definition</span>
  * <span style="color:#000000">Specific system layout is described by setting of 3 parameters</span>  <span style="color:#000000">\(for detailed description please refer to Annex Xb\)</span>

| IEPC layout case # | Differential Included | DesignType WheelMotor | DesignTypeWheel MotorMeasured | Layout schematics | Comments |
| :-: | :-: | :-: | :-: | :-: | :-: |
| 1 | No | No | n.a. |  | Regular axle component required in job file<br /><br /> useage of transmission type „IEPC Transmission - dummy entry"<br />(allows axle component alone without a min. number of gears) |
| 2 | Yes | No | n.a. |  | No axle component<br /><br />All parameterization via IEPC specific window |
| 3 | No | Yes | 1 |  | Component test performed in L-config acc. to Annex Xb (only one side)<br /> All torque and power values in input data are multiplied with a factor of 2 |
| 4 | No | Yes | 2 |  | Component test performed in T-config acc. to Annex Xb (both sides) |

![](img%5Crelease%20notes%20vecto3x34.png)

![](img%5Crelease%20notes%20vecto3x35.png)

![](img%5Crelease%20notes%20vecto3x36.png)

![](img%5Crelease%20notes%20vecto3x37.png)

![](img%5Crelease%20notes%20vecto3x38.png)

<span style="color:#000000">IEPC General parameters</span>

| Parameter name | Description |
| :-: | :-: |
| RotationalInertia | For EM inertia, gearbox parts not to be considered (as for regular transmission components). Reference point for definition of inertia is EM output shaft. <br />Determined in accordance with point 8 of Appendix 8 of Annex Xb. |
| DifferentialIncluded | Set to 'true' in the case a differential is part of the IEPC |
| DesignTypeWheelMotor | Set to 'true' in the case of an IEPC design type wheel motor |
| DesignTypeWheelMotorMeasured | Input only relevant in the case of an IEPC design type wheel motor, in accordance with paragraph 4.1.1.2 of Annex Xb.<br />Allowed values: '1', '2' |

<span style="color:#000000">IEPC performance parameters</span>

| Parameter name | Description |
| :-: | :-: |
| Gear data | For each gear: number, ratio, MaxOutputShaftTorque (optional), MaxOutputShaftSpeed (optional) |
| Overload data | Overload and Continuous operation point and Overload Time<br />For each voltage level separately<br />Measured in gear closest to ratio 1 acc. to Annex Xb |
| Max/min torque limits | Full load curve (only for one gear closest to ratio 1 acc. to Annex Xb)<br />For each voltage level separately |
| Drag torque | Only for one voltage level acc. to Annex Xb |
| Electric power maps | For each gear<br />For each voltage level separately |

# IEPC Input Form

![](img%5Crelease%20notes%20vecto3x39.png)

![](img%5Crelease%20notes%20vecto3x40.png)

<span style="color:#FF0000"> __Click for specific IEPC GUI\-window__ </span>

<span style="color:#000000"> __Definition of IEPC component via__ </span>  <span style="color:#000000"> _"Vehicle \\ IEPC\-Tab"_ </span>

<span style="color:#000000"> __Lists with gear entries are synchronized\, gears can be added/removed in the gear\-list on the right__ </span>

# Gearbox Form for IEPC Vehicles

<span style="color:#000000">No gearbox file needs to be provided for IEPC with differential included \(case 2\)</span>

<span style="color:#000000">New IEPC gearbox type for all other variants where only the axlegear ratio needs to be provided</span>

![](img%5Crelease%20notes%20vecto3x41.png)

<span style="color:#FF0000"> __Specific Gearbox Type for IEPC\. Only requires axlegear to be provided__ </span>

# Modeling of IHPC component

* <span style="color:#000000">IHPC transmission is modeled as APT\-N</span>
* <span style="color:#000000">IHPC electric motor is modeled as regular electric motor at position P2</span>
  * <span style="color:#000000">Gear\-dependent power maps</span>
* <span style="color:#000000">Peculiarities of IHPC</span>
  * <span style="color:#000000">Drag\-curve shall be set to </span>  <span style="color:#000000"> __0 Nm__ </span>  <span style="color:#000000"> acc\. to Annex Xb</span>
* <span style="color:#000000">For detailed description of component characteristics as well as</span>  <span style="color:#000000">testing and measurement post\-processing methods please refer to Annex Xb</span>

# Input data for IHPC component

<span style="color:#000000">IHPC general parameters EM </span>  <span style="color:#000000"> _\(same as for basic EM component\)_ </span>

| Parameter name | Description |
| :-: | :-: |
| RotationalInertia | For EM inertia, gearbox parts not to be considered (as for regular transmission components). Reference point for definition of inertia is EM output shaft. <br />Determined in accordance with point 8 of Appendix 8 of Annex Xb. |

<span style="color:#000000">IHPC performance parameters EM</span>  <span style="color:#000000"></span>  <span style="color:#000000"> </span>  <span style="color:#000000"> _same as for basic EM component\, except _ </span>  <span style="color:#000000"> _multiple El\. power maps_ </span>

| Parameter name | Description |
| :-: | :-: |
| Gear data | For each gear: number (needs to match with specifications in gearbox file) |
| Overload data | Overload and Continuous operation point and Overload Time<br />For each voltage level separately<br />Measured in gear closest to ratio 1 acc. to Annex Xb |
| Max/min torque limits | Full load curve (only for one gear closest to ratio 1 acc. to Annex Xb)<br />For each voltage level separately |
| Drag torque | NOT MEASURED acc. to Annex Xb, but input of "0 Nm" drag torque reqired! |
| Electric power maps | For each gear<br />For each voltage level separately |

<span style="color:#000000">IHPC parameters Gearbox</span>  <span style="color:#000000"></span>  <span style="color:#000000"> </span>  <span style="color:#000000"> _same as for basic transmission component_ </span>  <span style="color:#000000"> __ </span>  <span style="color:#000000"> _ transmission type needs to be set to _ </span>  <span style="color:#000000"> _"IHPC Transmission"_ </span>

| Parameter name | Description |
| :-: | :-: |
| Gear identifier | For each gear (number needs to match with specifications in IHPC file)<br /> Regular axle component to be specified as well |
| Ratio | For each gear<br /><br /> Regular axle component to be specified as well |
| Loss Map | For each gear<br /> Methodology how to derive data acc. to Annex Xb (esp. braking side needs to be measured separately as well - as opposed to copy-paste for regular transmission testing)<br /><br /> Regular axle component to be specified as well |

# IHPC Input Form

![](img%5Crelease%20notes%20vecto3x42.png)

<span style="color:#FF0000"> __Item \#1:__ </span>  <span style="color:#FF0000"> __IHPC\-EM__ </span>

![](img%5Crelease%20notes%20vecto3x43.png)

![](img%5Crelease%20notes%20vecto3x44.png)

<span style="color:#FF0000"> __Item \#1:__ </span>  <span style="color:#FF0000"> __IHPC\-EM__ </span>

<span style="color:#FF0000"> __Click for specific IHPC GUI\-window__ </span>

<span style="color:#FF0000"> __Item \#2:__ </span>  <span style="color:#FF0000"> __IHPC\-Gearbox__ </span>

<span style="color:#000000"> __Number of gears__ </span>  <span style="color:#000000"> __need to match__ </span>

![](img%5Crelease%20notes%20vecto3x45.png)

<span style="color:#FF0000"> __Item \#2:__ </span>  <span style="color:#FF0000"> __IHPC\-Gearbox__ </span>

<span style="color:#000000"> __Definition of IHPC component as two separate component parts via__ </span>  <span style="color:#000000"> __\#1: __ </span>  <span style="color:#000000"> _"Vehicle \\ IHPC\-Tab"_ </span>

<span style="color:#000000"> __\#2: __ </span>  <span style="color:#000000"> _"Gearbox"_ </span>

# Adaptations Serial Hybrid Strategy  (1/3)

<span style="color:#000000">Observation: GenSet often switching between optimal and maximum power\, while operating constantly at optimal power would be sufficient</span>

![](img%5Crelease%20notes%20vecto3x46.png)

![](img%5Crelease%20notes%20vecto3x47.png)

* <span style="color:#000000">Minor changes in Serial Hybrid Strategy</span>
  * <span style="color:#000000">GenSet is switched on as soon as SoC falls below SoC\_min</span>
  * <span style="color:#000000">GenSet is switched off as soon as SoC reaches SoC\_target</span>
  * <span style="color:#000000">GenSet operation points P\_opt and P\_max:</span>
    * <span style="color:#000000">In total 4 points defined\, for GenSet in regular and in de\-rating condition</span>
    * <span style="color:#000000">SoC\_min ≤ </span>  <span style="color:#000000"> __SoC__ </span>  <span style="color:#000000"> < SOC\_target: P\_opt</span>
    * <span style="color:#000000"> __SoC__ </span>  <span style="color:#000000"> < SoC\_min:</span>
      * <span style="color:#000000">requested el\. power > P\_opt: GENSET @ P\_max</span>
      * <span style="color:#000000">requested el\. power ≤ P\_opt: GENSET @ P\_opt</span>
  * <span style="color:#000000"> _Note: SoC\_min shall be above battery's min SoC_ </span>

![](img%5Crelease%20notes%20vecto3x48.png)

<span style="color:#000000">GenSet now always operates in optimal point</span>

<span style="color:#000000">Speed profile is unchanged\, SoC trace slightly different</span>

![](img%5Crelease%20notes%20vecto3x49.png)

# SoC Correction for HEV-S

* <span style="color:#000000">REESS SoC for HEV is corrected using the engine line</span>
  * <span style="color:#000000">Engine line makes no sense for HEV\-S as the ICE is decoupled from the drivetrain and operates only in certain points</span>
* <span style="color:#000000">SoC Correction:</span>

![](img%5Crelease%20notes%20vecto3x50.png)

![](img%5Crelease%20notes%20vecto3x51.png)

![](img%5Crelease%20notes%20vecto3x52.png)

<span style="color:#000000"> _… in case the GenSet was on during the simulation_ </span>

![](img%5Crelease%20notes%20vecto3x53.png)

<span style="color:#000000"> _… in case the GenSet was not on during the simulation_ </span>

# Vecto 0.7.8.2679 Development Version April 2022

* <span style="color:#000000"> __Changes/Bugfixes__ </span>
  * <span style="color:#000000">Multi\-Target Executables in \.net 6\.0 and \.net Framework 4\.8 \(and \.net Framework 4\.5\, but this will be removed with the next release\)</span>
  * <span style="color:#000000">Implementation Serial Hybrid Vehicles S2\, S3\, S4</span>
  * <span style="color:#000000">Implementation APT\-S/P with E2 and S2 vehicle architecture</span>
  * <span style="color:#000000">Correct handling of retarder positions with HEV and PEV\, adding axlegear input retarder option</span>
  * <span style="color:#000000">Update hashing tool to handle new powertrain components \(electric motor\, battery\, supercap\)</span>
  * <span style="color:#000000">Changing EM overload buffer calculation: interpolate between both voltage levels \(pre\-processing\)</span>
  * <span style="color:#000000">Handling of torque limitations \(engine torque limit\, boosting torque limits\, gearbox torque limits\) for P2 architecture</span>
  * <span style="color:#000000">Connect electric WHR system to REESS in case of hybrid vehicles</span>
  * <span style="color:#000000">Update LH cycle</span>
  * <span style="color:#000000">GUI improvements: enable/disable some fields in Job Form\, Vehicle Form\, and Hybrid Strategy Parameters to simplify user interface and avoid erratic entries\.</span>

# Serial Hybrid Implementation

* <span style="color:#000000">General approach</span>
  * <span style="color:#000000">Simple operating strategy\, not ECMS-based</span>
  * <span style="color:#000000">Three\-point controller for GenSet \(off/optimal point/maximum power\)</span>
  * <span style="color:#000000">Operate GenSet in optimal load\-points whenever required and possible</span>
  * <span style="color:#000000">Drivetrain similar to PEV</span>

# Serial Hybrids: GenSet Pre-Processing

* <span style="color:#000000">Find optimal points for the GenSet</span>
  * <span style="color:#000000">Iterate from 0 to maximum power of the ICE \(20 steps\)</span>
  * <span style="color:#000000">Iterate from ICE idle speed to n</span>  <span style="color:#000000">95h</span>  <span style="color:#000000"> speed \(20 steps\)</span>
  * <span style="color:#000000">Calculate electric power and fuel consumption for all load\-points</span>
  * <span style="color:#000000">Select operating points:</span>
    * <span style="color:#000000">Maximum electric power\, EM de\-rated/not de\-rated</span>
    * <span style="color:#000000">Optimal operating point \(minimum FC per electric power generated\)\, EM de\-rated/not de\-rated</span>

# Serial Hybrid Strategy (main principles)

<span style="color:#000000">Calculate max\. electric power GenSet can provide</span>

<span style="color:#000000">Calculate power demand of electric motor\, assuming max\. electric power \(REESS \+ GenSet\)</span>

<span style="color:#000000">Depending on drivetrain power demand and SoC turn on ICE and operate GenSet either in optimal point or at maximum power \(considering de\-rating of GenSet\)</span>

![](img%5Crelease%20notes%20vecto3x54.png)

![](img%5Crelease%20notes%20vecto3x55.png)

<span style="color:#808080"> _Note: SoC\_min and SoC\_target are parameters of the SerialHybridStrategy\, not REESS_ </span>

# Serial Hybrids: GenSet

<span style="color:#000000">When switching on the ICE\, it is often not possible to directly operate it in the desired operating point \(high ICE speed\, too high torque demand due to inertia\)</span>  <span style="color:#000000"></span>  <span style="color:#000000"> Ramp\-up ICE with electric machine switched off \(usually 1 or 2 </span>  <span style="color:#000000">     simulation steps\) and then turn on the electric machine</span>

# Serial Hybrid Strategy Parameters

<span style="color:#000000">MinSoC</span>

<span style="color:#000000">TargetSoC</span>

![](img%5Crelease%20notes%20vecto3x56.png)

# EM: overload buffer calculated from both voltage levels

* <span style="color:#000000">Input parameters of EM measured for min and max voltage level</span>
  * <span style="color:#000000">Overload data</span>
    * <span style="color:#000000">Continuous Torque</span>
    * <span style="color:#000000">Overload Torque \+ Overload Duration</span>
  * <span style="color:#000000">Maximum and Minimum torque limitations</span>
  * <span style="color:#000000">Electric power map</span>
* <span style="color:#000000">Implementation in software:</span>
  * <span style="color:#000000">From overload data above\, thermal buffer is calculated for each voltage level separately \(acc\. to equations in VECTO user manual						    \)</span>
  * <span style="color:#000000">Applied values for simulation run are calculated by linear interpolation between two voltage levels\, where average of usable SOC range is used</span>
    * <span style="color:#000000">Overload buffer</span>
    * <span style="color:#000000">Maximum and Minimum torque limitations</span>
    * <span style="color:#000000">Continuous Torque</span>

![](img%5Crelease%20notes%20vecto3x57.png)

# Consider gearbox MaxTorque limits for HEV

* <span style="color:#000000"> __Ref\. to ticket nr\. 1478\+1479__ </span>
* <span style="color:#000000"> __Only relevant for P1 \+ P2 HEV where EM is located upstream of transmission__ </span>
* <span style="color:#000000"> __Starting Point: ICE FLD__ </span>
  * <span style="color:#000000"> __Crop with max ICE torque declared at vehicle level \(per gear\) __ </span>  <span style="color:#376092"> _'Vehicle/EngineTorqueLimits' per gear - P196 \+ P197_ </span>
  * <span style="color:#000000"> __Add boosting torque__ </span>  <span style="color:#376092"> _Boosting limitations for parallel HEV - P415 \+ P416_ </span>
  * <span style="color:#000000"> __In case of P1 or P2: crop with gearbox max input torque \(per gear\)  __ </span>  <span style="color:#376092"> _MaxTorque - P157_ </span>
* <span style="color:#000000"> ____ </span>  <span style="color:#000000"> __ Torque limits per gear applied by HybridStrategy__ </span>

![](img%5Crelease%20notes%20vecto3x58.png)

![](img%5Crelease%20notes%20vecto3x59.png)

# Vecto 0.7.8.xxx Development Version April 2022

* <span style="color:#000000"> __Detailed Changes/Bugfixes \(See __ </span>  <span style="color:#0000FF"> _[CITnet/Jira](https://citnet.tech.ec.europa.eu/CITnet/jira/projects/VECTO/versions/341166)_ </span>  <span style="color:#000000"> __\)__ </span>
  * <span style="color:#000000">\[VECTO\-1541\] \- Implement E2/S2 with APT\-S/P</span>
  * <span style="color:#000000">\[VECTO\-1549\] \- Error in gearshift behavior for AT transmissions when braking</span>
  * <span style="color:#000000">\[VECTO\-1466\] \- 076\.2451 E2 significant increase of calculation time</span>
  * <span style="color:#000000">\[VECTO\-1479\] \- Maximum gearbox torque not respected</span>
  * <span style="color:#000000">\[VECTO\-1519\] \- 077\.2457 E2 Gear oscillation</span>
  * <span style="color:#000000">\[VECTO\-1520\] \- 077\.2457 P2 Crashing at takeoff</span>
  * <span style="color:#000000">\[VECTO\-1531\] \- ElectricMotor Lookup Maximum Torque: Object reference not set to an instance of an object</span>
  * <span style="color:#000000">\[VECTO\-1532\] \- 077\.2547 P2 crashing with 600 RPM idling speed</span>
  * <span style="color:#000000">\[VECTO\-1539\] \- Engine speed ramping up before vehicle halt</span>
  * <span style="color:#000000">\[VECTO\-1551\] \- AT\-P Transmission Bus Application: Error during braking phase</span>
  * <span style="color:#000000">\[VECTO\-1559\] \- battery internal resistance curve</span>
  * <span style="color:#000000">\[VECTO\-1411\] \- Switching to new \.NET version</span>
  * <span style="color:#000000">\[VECTO\-1456\] \- Cycle Cache in engineering mode</span>
  * <span style="color:#000000">\[VECTO\-1478\] \- Torque limits only apply to ICE</span>
  * <span style="color:#000000">\[VECTO\-1521\] \- Updating tyre dimensions</span>
  * <span style="color:#000000">\[VECTO\-1522\] \- VECTO warning if there are more steered axles than steering pump technologies</span>

* <span style="color:#000000"> __Detailed Changes/Bugfixes \(See __ </span>  <span style="color:#0000FF"> _[CITnet/Jira](https://citnet.tech.ec.europa.eu/CITnet/jira/projects/VECTO/versions/341166)_ </span>  <span style="color:#000000"> __\)__ </span>
  * <span style="color:#000000">\[VECTO\-1525\] \- PCC Preprocessing</span>
  * <span style="color:#000000">\[VECTO\-1533\] \- Multi\-Target Compilation and \.NET Upgrade</span>
  * <span style="color:#000000">\[VECTO\-1538\] \- Transmission ratios limitation of input value to <=25</span>
  * <span style="color:#000000">\[VECTO\-1540\] \- Retarder Types AxleGearInputRetarder</span>
  * <span style="color:#000000">\[VECTO\-1543\] \- Calculation of EM Overload buffer\, and continuous torque</span>
  * <span style="color:#000000">\[VECTO\-1545\] \- WHR electric \- connect to REESS</span>
  * <span style="color:#000000">\[VECTO\-1548\] \- Update Coach cycle</span>
  * <span style="color:#000000">\[VECTO\-1560\] \- update toolchain for generating usermanual</span>
  * <span style="color:#000000">\[VECTO\-1562\] \- Adapting WHTC Correction Factor weighting for LH Cycle</span>
  * <span style="color:#000000">\[VECTO\-1563\] \- Electric Motor: Calculation of Overload Buffer</span>
  * <span style="color:#000000">\[VECTO\-1564\] \- XML Schema adaptations</span>
  * <span style="color:#000000">\[VECTO\-1565\] \- VECTO warning if there are more steered axles than steering pump technologies</span>
  * <span style="color:#000000">\[VECTO\-1569\] \- Bugfix: Extrapolation warning in PEV shift strategy</span>
  * <span style="color:#000000">\[VECTO\-1573\] \- Update LH Cycle and WHTC correction factor weighting</span>

# Vecto 0.7.7.2547 Development Version December 2021

* <span style="color:#000000"> __Changes/Bugfixes__ </span>
  * <span style="color:#000000">Implementation APT\-N Gearbox \+ shift strategy</span>
  * <span style="color:#000000">Update EM model \(drag curve independent of voltage level\)</span>
  * <span style="color:#000000">Update EM electric power map interpolation method</span>
  * <span style="color:#000000">Update user manual</span>
  * <span style="color:#000000">Update/bugfixes PEV shift strategy \(influence of drag curve\, error in calculation of costs\)</span>
  * <span style="color:#000000">Bugfix calculation E\_EM\_off\_loss</span>
  * <span style="color:#000000">Update generic shift lines for PEV E2</span>
  * <span style="color:#000000">Bugfix post\-processing fuel consumption correction bus auxiliaries \(in case of no alternator\)</span>
  * <span style="color:#000000">Bugfixes PCC for xEV</span>

* <span style="color:#000000"> __General comments \(1/x\)__ </span>
  * <span style="color:#000000">All topics in this section will also be explained in the next xEV workshop \#9 on 11\.01\.2022</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">Also topics for testing and feedback will be discussed in this workshop\.</span>  <span style="color:#000000">Until then\, no feedback on the distributed version is expected\.</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">Calculation of overload buffer for EM</span>
    * <span style="color:#000000">Overload buffer is currently calculated with data for lower voltage level only</span>
    * <span style="color:#000000">Update of calculation method based on two voltage levels will be implemented once all details reg\. the method are clarified</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">Propulsion Torque Limits \(Vehicle Boosting Limits\)</span>
    * <span style="color:#000000">In the rotational speed range from 0 to engine idling speed the full load torque available from the ICE equals the ICE full load torque at engine idling speed due to the modelling of the clutch behaviour during vehicle starts\.</span>
    * <span style="color:#000000">Any arbitrary number of values may be declared in between the range of zero and the maximum rotational speed of the ICE full load curve\.</span>
    * <span style="color:#000000">Declared values lower than zero are not allowed for the additional torque\.</span>
    * <span style="color:#000000">For detailed explanation refer to user manual \("Vehicle Editor - Torque Limits Tab"\, "Torque and Speed Limitations"\)</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">Pulse\-duration dependency of internal resistance of battery</span>
    * <span style="color:#000000">For detailed explanation refer to user manual</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">HEV operation strategy method for declaration mode</span>
    * <span style="color:#000000">Method explained in "User Manual/HEV\_DECL\_method\_20211221\.pptx" distributed together with VECTO archive</span>
    * <span style="color:#000000">Generic values defined per \(vehicle group\, mission\, payload and usable SOC range\) in "User Manual/HEV\_Strategy\_Parameters\.xlsx" distributed together with VECTO archive</span>
    * <span style="color:#000000">For further details refer to distributed material/recording from xEV workshop \#8</span>

* <span style="color:#000000"> __General comments \(2/x\)__ </span>
  * <span style="color:#000000">Auxiliaries for PEV</span>
    * <span style="color:#000000">Electric power demand of auxiliaries for both\, lorries and buses\, need to be parameterized via the "BusAux" GUI window</span>  <span style="color:#000000"> </span>
  * <span style="color:#000000">Gearshift model for PEV</span>
    * <span style="color:#000000">Details explained on next slides</span>

![](img%5Crelease%20notes%20vecto3x60.png)

![](img%5Crelease%20notes%20vecto3x61.png)

<span style="color:#000000">Demand during vehicle driving</span>

<span style="color:#000000">Demand during vehicle standstill</span>

<span style="color:#000000"> __Gearshift model for PEV \(1/x\)__ </span>

* <span style="color:#000000"> _Basics:_ </span>
  * <span style="color:#000000">Downshift for operation point left of green dot\-dashed downshift lines</span>
  * <span style="color:#000000">Upshift for operation point right of green dot\-dashed upshift line</span>
  * <span style="color:#000000">EffShift method applied for operation point between downshift and upshift lines \(refer to user manual\) </span>
* <span style="color:#000000"> _Driving:_ </span>
  * <span style="color:#000000">Maximum downshift speed always located at </span>  <span style="color:#000000"> __n\_P80low__ </span>  <span style="color:#000000"> \(where 80% of max power is available\)</span>
  * <span style="color:#000000">For EM in de\-rating </span>  <span style="color:#000000"> __n\_P80low__ </span>  <span style="color:#000000"> is calculated from the de\-rated power curve</span>
* <span style="color:#000000"> _Braking:_ </span>
  * <span style="color:#000000">EffShift is suppressed for operation point within red shaded area</span>  <span style="color:#000000">\(2% below max recuperation power\)</span>
  * <span style="color:#000000">New gear after downshift is selected so that operation point is closest to and above </span>  <span style="color:#000000"> __n\_brake\_target\_norm__ </span>  <span style="color:#000000"> \(or only closest to </span>  <span style="color:#000000"> __n\_brake\_target\_norm__ </span>  <span style="color:#000000"> in case no operation point with higher speed exists\)</span>

![](img%5Crelease%20notes%20vecto3x62.wmf)

<span style="color:#000000"> __Gearshift model for PEV \(2/x\)__ </span>

* <span style="color:#000000"> _All factors in bold red can be tuned via shift parameter file \.vtcu_ </span>
  * <span style="color:#000000">Refer to sample file for User Manual/PEV\_ShiftParameters\.vtcu distributed together with VECTO archive</span>

![](img%5Crelease%20notes%20vecto3x63.wmf)

* <span style="color:#000000"> __Detailed Changes/Bugfixes \(see __ </span>  <span style="color:#0000FF"> _[CITnet/JIRA](https://citnet.tech.ec.europa.eu/CITnet/jira/projects/VECTO/versions/335270)_ </span>  <span style="color:#000000"> __\)__ </span>
  * <span style="color:#000000">\[VECTO\-1460\] \- APT\-N Gearbox</span>
  * <span style="color:#000000">\[VECTO\-1501\] \- Update EM Map interpolation method</span>
  * <span style="color:#000000">\[VECTO\-1477\] \- Update user manual</span>
  * <span style="color:#000000">\[VECTO\-1383\] \- E2 PEV\, 2\-speed AMT: "Failed to find operating point"</span>
  * <span style="color:#000000">\[VECTO\-1384\] \- E2 PEV\, 2\-speed AMT: Downshifts missing</span>
  * <span style="color:#000000">\[VECTO\-1414\] \- E2 selects high gear at low speed after take\-off\, does not reach target acc</span>
  * <span style="color:#000000">\[VECTO\-1443\] \- 0\.7\.5\.2356 EC\_el\_final calculated from REESS terminal signals</span>
  * <span style="color:#000000">\[VECTO\-1445\] \- 073\.2356\, E2 Missing Downshifts\, simulation abort</span>
  * <span style="color:#000000">\[VECTO\-1464\] \- Engine Off not working with activated PCC \(Hybrid Vehicles\)</span>
  * <span style="color:#000000">\[VECTO\-1465\] \- PEV PCCState oscillates between Interrupt and UseCase1</span>
  * <span style="color:#000000">\[VECTO\-1467\] \- PCC HEV Small Spike at end of UseCase1 when Target speed is reached</span>
  * <span style="color:#000000">\[VECTO\-1468\] \- EM map interpolation comments</span>
  * <span style="color:#000000">\[VECTO\-1470\] \- 076\.2463 EM drag torque questions/findings</span>
  * <span style="color:#000000">\[VECTO\-1482\] \- Hybrid Vehicle EM speed gets very high\, no gearshift</span>
  * <span style="color:#000000">\[VECTO\-1483\] \- Job aborting: E4\_Group 5 LH\_ll\_LongHaul\_mod5\_id\_2</span>
  * <span style="color:#000000">\[VECTO\-1484\] \- Job aborting: P2\_Group5\_s2c0\_rep\_Payload\_LongHaul\_mod5\_id\_27</span>
  * <span style="color:#000000">\[VECTO\-1486\] \- NG PI fuel type not working with Engine GUI \(\.veng\) in Engineering Mode</span>
  * <span style="color:#000000">\[VECTO\-1487\] \- Calculation of EM efficiency during recuperation</span>
  * <span style="color:#000000">\[VECTO\-1490\] \- Exceeded max iterations: Operating Point could not be found</span>

* <span style="color:#000000"> __Detailed Changes/Bugfixes \(see __ </span>  <span style="color:#0000FF"> _[CITnet/JIRA](https://citnet.tech.ec.europa.eu/CITnet/jira/projects/VECTO/versions/335270)_ </span>  <span style="color:#000000"> __\)__ </span>
  * <span style="color:#000000">\[VECTO\-1491\] \- Electric Energy Storage Editor does not show values</span>
  * <span style="color:#000000">\[VECTO\-1499\] \- Energy balance when ICE is turned on in a P1 hybrid configuration</span>
  * <span style="color:#000000">\[VECTO\-1514\] \- Modeldata uses wrong inertia in case multiple EMs are used on a position</span>
  * <span style="color:#000000">\[VECTO\-1515\] \- Output in vsum file</span>
  * <span style="color:#000000">\[VECTO\-1516\] \- GUI: PEV vehicle\, disable hybrid parameters</span>
  * <span style="color:#000000">\[VECTO\-1489\] \- Engine\-off eco\-roll for P1 hybrids</span>
  * <span style="color:#000000">\[VECTO\-1448\] \- 075\.2356\, E2 Selecting high gear in downhill\, needs to use wheel brakes</span>
  * <span style="color:#000000">\[VECTO\-1472\] \- Do not end PCC if acceleration is still high enough</span>
  * <span style="color:#000000">\[VECTO\-1473\] \- Additional tyre dimensions</span>
  * <span style="color:#000000">\[VECTO\-1476\] \- Update EM GUI\, EM component</span>
  * <span style="color:#000000">\[VECTO\-1493\] \- Use EngineStartStop in PCC\-Events for Hybrid P1</span>
  * <span style="color:#000000">\[VECTO\-1498\] \- Crosswind correction in engineering mode</span>
  * <span style="color:#000000">\[VECTO\-1508\] \- EM\-Off Losses in \.vsum</span>
  * <span style="color:#000000">\[VECTO\-1509\] \- SP Technology "Electric" already contains mechanical power demand</span>
  * <span style="color:#000000">\[VECTO\-1510\] \- Remove selection of shift strategy in job dialog</span>
  * <span style="color:#000000">\[VECTO\-1511\] \- Error Message for "HEVs and BEVs not supported in Declaration Mode"</span>
  * <span style="color:#000000">\[VECTO\-1512\] \- Adapt shift lines for xEV vehicles</span>
  * <span style="color:#000000">\[VECTO\-1513\] \- Bus Aux post\-processing correction</span>
  * <span style="color:#000000">\[VECTO\-1481\] \- New warning message regarding transmission: what does it mean?</span>

# Vecto 0.7.6.2451 Development Version September 2021

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">ADAS for HEV and PEV vehicles</span>
    * <span style="color:#000000">For HEV vehicles interaction of PCC and HEV strategy under further analysis</span>
  * <span style="color:#000000">Update electric motor model: support for different voltage levels</span>
  * <span style="color:#000000">Update battery model: modular battery components</span>
  * <span style="color:#000000">Update battery model: pulse\-duration dependent internal resistance</span>
  * <span style="color:#000000">Bugfixes</span>

# Vecto 0.7.5.2353 Development Version June 2021

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Bugfixes in shift strategy for PEV \(E2\)</span>
  * <span style="color:#000000">Additional Powertrain architectures</span>
    * <span style="color:#000000">P1 AMT and APT\-S/P</span>
    * <span style="color:#000000">P2\.5 \(transmission ratio EM per gear\)</span>
    * <span style="color:#000000">P3 & P4 with APT transmissions</span>
  * <span style="color:#000000">Loss\-map for electric machine</span>
  * <span style="color:#000000">Bus auxiliaries in engineering mode</span>
  * <span style="color:#000000">Combine Bus auxiliaries with HEV powertrains</span>
  * <span style="color:#000000">Refactoring Engine Stop/Start approach</span>
    * <span style="color:#000000">ICE is always off during simulation\, balance missing energy in case ICE is on / ICE is off for all auxiliaries in \.vmod\.</span>
    * <span style="color:#000000">Correct for missing aux energy in post\-processing \(not fully implemented in this version\)</span>
  * <span style="color:#000000">Added vehicle propulsion limits \(relative to ICE max torque\)</span>
    * <span style="color:#000000">P2\, P3\, and P4</span>
  * <span style="color:#000000">Refactored EM model: continuous torque\, overload torque as separate inputs</span>

# Vecto 0.7.3.2164 Development Version December 2020

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Bugfixes in shift strategy for PEV \(E2\)</span>
  * <span style="color:#000000">Refined electric motor model: model transmission stage of electric machine as individual sub\-component instead of transforming the maps\. Adapted columns in \.vmod and \.vsum regarding electric machine\. See user manual for details\. </span>
  * <span style="color:#000000">Bugfixes SuperCap model</span>
  * <span style="color:#000000">Removed E\_REESS from \.vmod file\, calculate delta\_E\_REESS from P\_REESS in \.vsum file</span>

# Vecto 0.7.2.2118 Development Version October 2020

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">Implementation of PEV architecture E2 with EffShift\-like gearshift strategy</span>
  * <span style="color:#000000">Bugfix REESS Dialog \(\.vimax\)</span>
  * <span style="color:#000000">Bugfix in output of EM overload output in \.vmod file </span>

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000">More stable operating point search hybrid strategy</span>
  * <span style="color:#000000">Bugfixes hybrid strategy</span>
  * <span style="color:#000000">Adding Super Cap as REESS </span>
  * <span style="color:#000000">Adding electric machine thermal de\-rating</span>
  * <span style="color:#000000">Modified battery model: max charge/discharge current over state of charge</span>
  * <span style="color:#000000">Improvements simulation time hybrid vehicles</span>

<span style="color:#000000"> __Note: __ </span>  <span style="color:#000000">As the model data for electric machine and battery changed from the previous VECTO version for hybrid vehicles it is necessary to open the models and provide the newly added model parameters\. </span>

# Vecto 0.7.0.2076 Development Version September 2020

* <span style="color:#000000"> __Changes__ </span>
  * <span style="color:#000000"> _This release is the first beta version with capabilities to simulate hybrid and battery electric vehicles_ </span>  <span style="color:#000000">\.</span>
  * <span style="color:#000000">It covers powertrain architectures "P2"\, "P3"\, "P4" for hybrid electric vehicles \(HEV\) and  "E3"\, "E4" for battery electric vehicles \(PEV\, pure electric vehicles\)\.</span>
  * <span style="color:#000000">The definitions of the powertrain architectures and other input parameters on vehicle level can be found in the document </span>  <span style="color:#000000"> _Input parameters vehicle level 20200907\.docx_ </span>  <span style="color:#000000">\. This document is an excerpt of working document 3 of the HDV CO2 Editing board\. Items marked in red are not yet featured by the tool and will follow in the next releases\. </span>
  * <span style="color:#000000">Known limitations</span>
    * <span style="color:#000000">Electric machine system overload feature not available - work in progress</span>
    * <span style="color:#000000">Supercaps not implemented - work in progress</span>
    * <span style="color:#000000">Engine Stop Start Utility Factor has to be set to 1 - further discussions in task force needed how UF\-approach shall be aligned with hybrid control strategy</span>
    * <span style="color:#000000">P\_aux\_el are electric auxiliaries connected to the high\-voltage system\. For hybrid electric vehicles this model parameter shall currently be set to 0\. - further discussions in task force needed</span>
    * <span style="color:#000000">APTs with HEV - to be implemented</span>
    * <span style="color:#000000">Eco\-roll and PCC not implemented</span>

# Vecto 0.6.1.1975 Development Version May 2020

* <span style="color:#000000"> __Bugfixes __ </span>
  * <span style="color:#000000">Change passenger density primary vehicle for IU cycle</span>
  * <span style="color:#000000">primary vehicle: ignore TPMLM \(to limit payload\, do not abort simulation\)</span>
  * <span style="color:#000000">completed vehicle: limit total mass with TPMLM \(abort simulation\)</span>
  * <span style="color:#000000">consider l\_drivetrain\. this length is subtracted from internal length for HVAC calculation only \(bus volume for ventilation\, max cooling capacity\); completed vehicle</span>
  * <span style="color:#000000">set alternator gear efficiency to 1 to align with lorries \(currently 0\.92\)</span>
  * <span style="color:#000000">electric system / radio: all vehicles are equipped with radio\. modify consumed amps to consider 'usage factor' \- updated already in master excel\.</span>
  * <span style="color:#000000">Update generic values for primary HF vehicles CdxA</span>
  * <span style="color:#000000">HVAC: check/correct internal height for coaches \(shall be 1\.8m\)</span>
  * <span style="color:#000000">HVAC: subtract driver compartment from total volume for max cooling capacity passengers</span>
  * <span style="color:#000000">HVAC: add configuration \#10\, add sanity checks \(i\.e\. if driver AC is provided for configurations \#2\,</span>
  * <span style="color:#000000">Steering pump power demand: tubing factor only applied for first steered axle \(as the additionally steered axles are close to the engine and the distance is not known\)</span>
  * <span style="color:#000000">Allow enabling/disabling LAC in declaration mode via GUI \(not persistent\)\. Additional input on "Options" tab to set min activation speed</span>
  * <span style="color:#000000">\.vsum file: E\_AUX\_SUM matches sum of individual AUX entries; bugfix writing P\_busAux\_ES\_mech</span>
  * <span style="color:#000000">Update user manual on VTP cycle file format</span>
  * <span style="color:#000000">APT gearbox \(all gears\) generic efficiency 0\.925</span>
  * <span style="color:#000000">retarder: set factor for generic retarder losses from 0\.5 to 1 \(i\.e\.\, use standardvalues\)</span>
  * <span style="color:#000000">VTP engineering mode \- vsum actual CF different to declaration mode</span>
* <span style="color:#000000">Improvements</span>
  * <span style="color:#000000">Switch GUI to 64bit mode \(see VECTO\-1270\)</span>
  * <span style="color:#000000">New normalization/denormalization method for generic engine FC\-Map \(see VECTO\-1269\) </span>
  * <span style="color:#000000">\.vsum\-file: new entries P\_ice\_fcmap\, P\_wheel\_in\, k\_vehline \(previous k\_vehline is now k\_engline</span>
  * <span style="color:#000000">Bus VTP: Fan Parameter C4</span>
  * <span style="color:#000000">don't write CIF for primary vehicle</span>

* <span style="color:#000000"> __Improvements __ </span>
  * <span style="color:#000000">Declaration mode for complete and completed buses via "factor method"</span>
  * <span style="color:#000000">VTP mode for heavy buses</span>
  * <span style="color:#000000">VTP mode for dual\-fuel vehicles \(new cycle file required\!\)</span>
  * <span style="color:#000000">"Single bus" mode with generic data from segmentation table completed vehicle \(previous version used generic data from primary vehicle\)</span>
  * <span style="color:#000000">Adding highway sections on coach cycle \(required for PCC\)</span>
  * <span style="color:#000000">New graphical user interface \(VECTO3\.exe\) \(</span>  <span style="color:#953735"> _alpha version\!_ </span>  <span style="color:#000000">\)</span>
    * <span style="color:#000000">Creating & editing complete\(d\) bus XML</span>
    * <span style="color:#000000">Creating & editing complete\(d\) bus job file \(PIF \+ completed XML\)</span>
    * <span style="color:#000000">Creating & editing single bus job file \(primary XML \+ completed XML\)</span>
    * <span style="color:#000000">Simulation of all types of VECTO jobs</span>
    * <span style="color:#000000">Currently\, other existing job files can not be edited in the new GUI - planned to be extended in future</span>
    * <span style="color:#000000">Functionality similar to currently official VECTO version\, for further information see separate user manual on new GUI</span>
  * <span style="color:#000000">XML reports \(MRF\, CIF\, PIF\) according to draft Annex IV</span>
  * <span style="color:#953735"> _Expert feature:_ </span>  <span style="color:#000000"> allow adding torque converter data to PIF manually \(not added by VECTO automatically\)  to investigate on generic TC data for APT\-P \(see generic vehicles for reference\)</span>
  * <span style="color:#953735"> _Expert feature: _ </span>  <span style="color:#000000">allow writing internal model data used for VECTO simulation as JSON file\. Allows to compare model data between primary bus\, completed bus generic body\, completed bus specific body \(factor method\) as well as single bus</span>

* <span style="color:#000000"> __Bugfixes heavy buses __ </span>
  * <span style="color:#000000">Correcting power demand for engine fan</span>
  * <span style="color:#000000">Consider electric auxiliaries \(fan\, steering pump\) as electrical consumer</span>
  * <span style="color:#000000">Correcting power demand pneumatic system with mechanical clutch</span>
  * <span style="color:#000000">Correcting electrical consumers LED bonus</span>
  * <span style="color:#000000">Correction for double compensation of smart electrics \(battery & generated vs\. consumed\)</span>
  * <span style="color:#000000">Correcting passenger count depending on HVAC configuration</span>
* <span style="color:#000000">Bugfixes medium lorries</span>
  * <span style="color:#000000">New technologies for electric steering pump</span>
  * <span style="color:#000000">New tyre xml supported</span>
  * <span style="color:#000000">Bugfixes graphical user interface</span>
  * <span style="color:#000000">New sample data VTP</span>

# Vecto 0.6.0.1908 Development Version March. 2020

* <span style="color:#000000"> __Bugfixes __ </span>
  * <span style="color:#000000">Consider vehicle's max speed when validating input data\, calculating velocity drop during traction interruption </span>
  * <span style="color:#000000">Reading vehicle design speed \(85km/h\) from segment table \(also related to vehicle's max speed\)</span>
  * <span style="color:#000000">Fix JobEditor GUI - toolbar with icons not visible</span>
  * <span style="color:#000000">Fix null\-reference exception simulating vehicles in engineering mode</span>

# Vecto 0.6.0.1884 Development Version Feb. 2020

* <span style="color:#000000"> __Improvements __ </span>
  * <span style="color:#000000">Refactoring and bug fixes in bus auxiliaries model \(former Ricardo AAUX model\)\, parametrization of bus auxiliaries model \(HVAC\) from "primary bus" input data and generic values in segmentation matrix\, </span>
  * <span style="color:#000000">Support for P0 hybrids in bus auxiliaries model</span>
  * <span style="color:#000000">Support for "primary bus" \(declaration mode\)</span>
  * <span style="color:#000000">Support for "single bus" - combine input parameters of primary bus and completed bus to a single simulation \(no factor method\) \(declaration mode\)</span>
  * <span style="color:#000000">Support for medium lorries</span>
  * <span style="color:#000000">Support for vehicles with front\-wheel drive \(axlegear included in gearbox\)</span>
  * <span style="color:#000000">VTP Mode for medium lorries</span>
  * <span style="color:#000000">New XML schema for new vehicle categories\, adapted XML schema for generated reports \(MRF\, CIF\, PIF\)</span>
* <span style="color:#000000">This development version includes all features already included in version 0\.5\.0</span>

# Vecto 0.5.0 Development Version Dec. 2019

* <span style="color:#000000"> __Improvements __ </span>
  * <span style="color:#000000">Adding in\-the\-loop simulation of advanced driver assistant systems \(ADAS\)</span>
    * <span style="color:#000000">Engine stop\-start during vehicle stop</span>
    * <span style="color:#000000">Eco\-roll with and without engine stop</span>
    * <span style="color:#000000">Predictive cruise control: dip coasting\, crest coasting</span>
  * <span style="color:#000000">Adding support for dual\-fuel vehicles</span>
  * <span style="color:#000000">Adding support for waste\- / exhaust\-heat recovery systems \(electrical and mechanical\)</span>
  * <span style="color:#000000">New gear shift strategy \(EffShift\) for AMT and AT transmissions</span>
* <span style="color:#000000">Details on the new models\, model parameters\, and new signals in the output are described in the user manual</span>

# Vecto 3.3.10.2401 Official Release 2020-07-29

* <span style="color:#000000"> __Improvements__ </span>
  * <span style="color:#000000">Handling of exempted vehicles \(not changed since release candidate\) - see next slides for details</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">No additional bugfixes compared to VECTO 3\.3\.10\.2401</span>

# Vecto 3.3.10.2373 Release Candidate 2020-07-01

* <span style="color:#000000"> __Improvements__ </span>
  * <span style="color:#000000">\[VECTO\-1421\] - Added vehicle sub\-group \(CO</span>  <span style="color:#000000">2</span>  <span style="color:#000000">\-standards to MRF and CIF\)</span>
  * <span style="color:#000000">\[VECTO 1449\] - Handling of exempted vehicles: See next slide for details</span>
  * <span style="color:#000000">\[VECTO\-1404\] - Corrected URL for CSS in MRF and CIF</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1419\] - Simulation abort in urban cycle: failed to find operating point on search braking power with TC gear</span>
  * <span style="color:#000000">\[VECTO\-1439\] - Bugfix handling duplicate entries in engine full\-load curve when intersecting with max\-torque of gearbox</span>
  * <span style="color:#000000">\[VECTO\-1429\] - error in XML schema 2\.x for exempted vehicles - MaxNetPower1/2 are optional input parameters</span>

* <span style="color:#990000"> __Handling of exempted vehicles__ </span>
* <span style="color:#000000">Axle configuration and sleeper cab are optional input parameters for exempted vehicles \(XML schema 1\.0 and 2\.2\.1\)\. </span>
  * <span style="color:#000000">OEMs are recommended to provide these parameters for exempted vehicles\.</span>
  * <span style="color:#000000">If the axle configuration is provided as input parameter\, the MRF contains the vehicle group\. </span>
  * <span style="color:#000000">The sleeper cab input parameter is also part of the MRF if provided as input\.</span>
* <span style="color:#000000">Input parameters MaxNetPower1/2 are optional input parameters for all exempted vehicles\. </span>
  * <span style="color:#000000">If provided in the input these parameters are part of the MRF for all exempted vehicle types</span>
  * <span style="color:#000000">It is recommended that those parameters are used to specify the rated power also for PEV \(pure electric vehicles\)</span>

# Vecto 3.3.9.2175 Official Release 2020-12-15

* <span style="color:#000000"> __Bugfixes \(compared to version 3\.3\.9\.2147\)__ </span>
  * <span style="color:#000000">\[VECTO\-1374\] \- VECTO VTP error - regression update</span>

# Vecto 3.3.9.2147 Release Candidate 2020-11-17

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1331\] \- VTP Mode does not function for vehicles of group 3</span>
  * <span style="color:#000000">\[VECTO\-1355\] \- VTP Simulation Abort</span>
  * <span style="color:#000000">\[VECTO\-1356\] \- PTO Losses not considered in VTP simulation</span>
  * <span style="color:#000000">\[VECTO\-1361\] \- Torque Converter in use for the First and Second Gear VTP file does not allow for this</span>
  * <span style="color:#000000">\[VECTO\-1372\] \- Deviation of CdxA Input vs\. Output for HDV16</span>
  * <span style="color:#000000">\[VECTO\-1374\] \- VECTO VTP error</span>
* <span style="color:#000000"> __Improvements__ </span>
  * <span style="color:#000000">\[VECTO\-1360\] \- make unit tests execute in parallel</span>

# Vecto 3.3.8.2052 Official Release 2020-08-14

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">No additional bugfixes compared to VECTO 3\.3\.8\.2024</span>

# Vecto 3.3.8.2024 Release Candidate 2020-07-17

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1288\] \- Simulation Abort UD RL</span>
  * <span style="color:#000000">\[VECTO\-1327\] \- Simulation abort Construction RefLoad: unexpected response ResponseOverload</span>
  * <span style="color:#000000">\[VECTO\-1266\] \- Gear 4 Loss\-Map was extrapolated</span>

# Vecto 3.3.7.1964 Official Release 2020-05-18

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1254\] \- Hashing method does not ignore certain XML attributes</span>
  * <span style="color:#000000">\[VECTO\-1259\] \- Mission profile weighting factors for vehicles of group 16 are not correct</span>

# Vecto 3.3.6.1916 Official Release 2020-03-31

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1250\] \- Error creating new gearbox file from scratch</span>

# Vecto 3.3.6.1898 Release Candidate 2020-03-13

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-1239\] \- Adaptation of Mission Profile Weighting Factors</span>
  * <span style="color:#000000">\[VECTO\-1241\] \- Engineering mode: Adding support for additional PTO activations</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1243\] \- Bug in VTP mode for heavy lorries</span>
  * <span style="color:#000000">\[VECTO\-1234\] \- urban cycle at reference load not running for bug when find braking operating point</span>

# Vecto 3.3.5.1812 Official Release 2019-12-18

* <span style="color:#000000"> __Bugfixes __ </span>  <span style="color:#000000">\(compared to VECTO 3\.3\.5\.1783\-RC\)</span>
  * <span style="color:#000000">\[VECTO\-1220\] \- Simulation Abort Urban Delivery RefLoad</span>

# Vecto 3.3.5.1783 Release Candidate 2019-11-19

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-1194\] \- Handling input parameter 'vocational' for groups other than 4\, 5\, 9\, 10</span>
  * <span style="color:#000000">\[VECTO\-1147\] \- Updating declaration mode cycles values in user manual </span>
  * <span style="color:#000000">\[VECTO\-1207\] \- run VECTO in 64bit mode by default</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1074\] \- Vecto Calculation Aborts with Interpolation Error</span>
  * <span style="color:#000000">\[VECTO\-1159\] \- Simulation Abort in UrbanDelivery LowLoading</span>
  * <span style="color:#000000">\[VECTO\-1189\] \- Error in delaunay triangulation invariant violated</span>
  * <span style="color:#000000">\[VECTO\-1209\] \- Unexpected Response Response Overload</span>
  * <span style="color:#000000">\[VECTO\-1211\] \- Simulation Abort Urban Delivery Ref Load</span>
  * <span style="color:#000000">\[VECTO\-1214\] \- Validation of input data fails when gearbox speed limits are applied</span>

# Vecto 3.3.4.1716 Official Release 2019-09-13

* <span style="color:#000000"> __Bugfixes __ </span>  <span style="color:#000000">\(compared to VECTO 3\.3\.4\.1686\-RC\)</span>
  * <span style="color:#000000">\[VECTO\-1074\] \- Vecto Calculation Aborts with Interpolation Error \(\[VECTO\-1046\]\)</span>
  * <span style="color:#000000">\[VECTO\-1111\] \- Simulation Abort in Municipal Reference Load</span>

# Vecto 3.3.4.1686 Release Candidate 2019-08-14

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-1042\] \- Add option to write results into a certain directory</span>
  * <span style="color:#000000">\[VECTO\-1064\] \- add weighting factors for vehicle groups 1\, 2\, 3\, 11\, 12\, 16</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-1030\] \- Exceeded max iterations when searching for operating point\! Failed to find operating point\!</span>
  * <span style="color:#000000">\[VECTO\-1032\] \- Gear 5 LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient</span>
  * <span style="color:#000000">\[VECTO\-1067\] \- Vair and Beta correction for Aerodynamics</span>
  * <span style="color:#000000">\[VECTO\-1000\] \- Error Loss\-Map extrapolation in Declaration Mode</span>
  * <span style="color:#000000">\[VECTO\-1040\] \- Gear 6 LossMap data was extrapolated in Declaration Mode</span>
  * <span style="color:#000000">\[VECTO\-1047\] \- Failed to find operating point on construction cycle\, ref load\, AT gearbox</span>

# Vecto 3.3.3.1639 Official Release 2019-06-27

* <span style="color:#000000"> __Bugfixes __ </span>  <span style="color:#000000">\(compared to VECTO 3\.3\.3\.1609\-RC\)</span>
  * <span style="color:#000000">\[VECTO\-1003\] \- Vecto Error: Loss\-Map extrapolation in declaration mode required</span>  <span style="color:#000000">\(issue VECTO\-991\)</span>
  * <span style="color:#000000">\[VECTO\-1006\] \- Failed to find torque converter operating point on UD cycle </span>  <span style="color:#000000">\(issue VECTO\-996\)</span>
  * <span style="color:#000000">\[VECTO\-1010\] \- Unexpected Response: ResponseOverload in UD cycle </span>  <span style="color:#000000">\(issue VECTO\-996\)</span>
  * <span style="color:#000000">\[VECTO\-1015\] \- XML Schema not correctly identified</span>
  * <span style="color:#000000">\[VECTO\-1019\] \- Error opening job in case a file is missing</span>
  * <span style="color:#000000">\[VECTO\-1020\] \- HashingTool Crashes</span>
  * <span style="color:#000000">\[VECTO\-1021\] \- Invalid hash of job data</span>

# Vecto 3.3.3.1609 Release Candidate 2019-05-29

* <span style="color:#000000"> __ Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-916\] \- Adding new tyre sizes</span>
  * <span style="color:#000000">\[VECTO\-946\] \- Refactoring XML reading</span>
  * <span style="color:#000000">\[VECTO\-965\] \- Add input fields for ADAS into VECTO GUI</span>
  * <span style="color:#000000">\[VECTO\-966\] \- Allow selecting Tank System for NG engines in GUI</span>
  * <span style="color:#000000">\[VECTO\-932\] \- Consistency in NA values in the vsum file</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-954\] \- Failed to find operating point for braking power \(Fix for Notification Art\. 10\(2\) \- \[VECTO\-952\]\)</span>
  * <span style="color:#000000">\[VECTO\-979\] \- VECTO Simulation abort with 8\-speed MT transmission \(Fix for Notification Art\. 10\(2\) \- \[VECTO\-978\]\)</span>
  * <span style="color:#000000">\[VECTO\-931\] \- AT error in VECTO version 3\.3\.2\.1519</span>
  * <span style="color:#000000">\[VECTO\-950\] \- Error when loading Engine Full\-load curve</span>
  * <span style="color:#000000">\[VECTO\-967\] \- Engine\-Only mode: Engine Torque reported in \.vmod does not match the provided cycle</span>
  * <span style="color:#000000">\[VECTO\-980\] \- Error during simulation run</span>

# Vecto 3.3.2.1548 Official Release 2019-03-29

* <span style="color:#000000"> __Bugfixes \(compared to 3\.3\.2\.1519\-RC\)__ </span>
  * <span style="color:#000000">\[VECTO\-861\] \- 3\.3\.1: Torque converter not working correctly</span>
  * <span style="color:#000000">\[VECTO\-904\] \- Range for gear loss map not sufficient\.</span>
  * <span style="color:#000000">\[VECTO\-909\] \- 3\.3\.2\.1519: Problems running more than one input \.xml</span>
  * <span style="color:#000000">\[VECTO\-917\] \- TargetVelocity \(0\.0000\) and VehicleVelocity \(>0\) must be zero when vehicle is halting</span>
  * <span style="color:#000000">\[VECTO\-918\] \- RegionalDeliveryEMS LowLoading \- ResponseSpeedLimitExceeded</span>
  * <span style="color:#000000">\[VECTO\-920\] \- Urban Delivery: Simulation Run Aborted\, TargetVelocity and VehicleVelocity must be zero when vehicle is halting\!</span>

# Vecto 3.3.2.1519 Release Candidate 2019-03-01

* <span style="color:#000000"> __Improvements__ </span>
  * <span style="color:#000000">\[VECTO\-869\] \- change new vehicle input fields \(ADAS\, sleeper cab\, etc\.\) to be mandatory</span>
  * <span style="color:#000000">\[VECTO\-784\] \- Configuration file for VECTO log files</span>
  * <span style="color:#000000">\[VECTO\-865\] \- Extend Sum\-Data</span>
  * <span style="color:#000000">\[VECTO\-873\] \- Add digest value to SumData</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-729\] \- Bugs APT submodel</span>
  * <span style="color:#000000">\[VECTO\-787\] \- APT: DrivingAction Accelerate after Overload</span>
  * <span style="color:#000000">\[VECTO\-789\] \- APT: ResponseUnderload</span>
  * <span style="color:#000000">\[VECTO\-797\] \- VECTO abort with AT transmission and TC table value</span>
  * <span style="color:#000000">\[VECTO\-798\] \- VECTO abort with certified AT transmission data and certified TC data</span>
  * <span style="color:#000000">\[VECTO\-807\] \- VECTO errors in vehicle class 1/2/3</span>
  * <span style="color:#000000">\[VECTO\-827\] \- Torque converter inertia </span>
  * <span style="color:#000000">\[VECTO\-838\] \- APT: ResponseOverload</span>

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-843\] \- AT Transmissions problem on VECTO 3\.3\.1\.1463</span>
  * <span style="color:#000000">\[VECTO\-844\] \- Error with AT gearbox model</span>
  * <span style="color:#000000">\[VECTO\-847\] \- Simulation abort due to error in NLog?</span>
  * <span style="color:#000000">\[VECTO\-848\] \- AT Gearbox Simulation abort \(was: Problem related to Tyres?\)</span>
  * <span style="color:#000000">\[VECTO\-858\] \- Urban Delivery Abort \- with APT\-S Transmission and TC</span>
  * <span style="color:#000000">\[VECTO\-861\] \- 3\.3\.1: Torque converter not working correctly</span>
  * <span style="color:#000000">\[VECTO\-872\] \- MRF/CIF: Torque Converter certification method and certification nbr not correctly set</span>
  * <span style="color:#000000">\[VECTO\-879\] \- SIMULATION RUN ABORTED DistanceRun got an unexpected resp\.</span>
  * <span style="color:#000000">\[VECTO\-883\] \- Traction interruption may be too long</span>
  * <span style="color:#000000">\[VECTO\-815\] \- Unexpected Response: SpeedLimitExceeded</span>
  * <span style="color:#000000">\[VECTO\-816\] \- object reference not set to an instance of an object</span>
  * <span style="color:#000000">\[VECTO\-817\] \- TargetVelocity and VehicleVelocity must not be 0</span>
  * <span style="color:#000000">\[VECTO\-820\] \- DistanceRun got an unexpected response: ResponseSpeedLimitExceeded</span>
  * <span style="color:#000000">\[VECTO\-864\] \- Prevent VECTO loss\-map extension to result in negative torque loss</span>

* <span style="color:#000000"> __Installation Option __ </span>  <span style="color:#000000">\(VECTO\-784\)</span>
  * <span style="color:#000000">VECTO 3\.3\.2 adds a new feature to run as 'installed application' instead of the 'portable' mode</span>
  * <span style="color:#000000">VECTO as 'installed application'</span>
    * <span style="color:#000000">Needs no write permissions to the VECTO application folder</span>
    * <span style="color:#000000">All configuration files and settings are written to %AppData%\\VECTO\\\<Version></span>
    * <span style="color:#000000">All log files are written to %LocalAppData%\\VECTO\\\<Version></span>
  * <span style="color:#000000">Switch to 'installed application'</span>
    * <span style="color:#000000">Copy the VECTO directory and all its files and subdirectories to the appropriate location where the user has execute permissions</span>
    * <span style="color:#000000">Edit the file '</span>  <span style="color:#000000"> _install\.ini_ </span>  <span style="color:#000000">' and remove the comment character \(\#\) in the line containing </span>  <span style="color:#000000">	</span>  <span style="color:#000000"> __ExecutionMode = install__ </span>

# Vecto 3.3.1.1492 Official Release 2019-02-01

* <span style="color:#000000"> __Bugfixes \(compared to 3\.3\.1\.1463\-RC\)__ </span>
  * <span style="color:#000000">\[VECTO\-845\] \- Fixing bug for VECTO\-840</span>
  * <span style="color:#000000">\[VECTO\-826\] \- DistanceRun got an unexpected response: ResponseSpeedLimitExceeded</span>
  * <span style="color:#000000">\[VECTO\-837\] \- VECTO GUI displays incorrect cycles prior to simulation</span>
  * <span style="color:#000000">\[VECTO\-831\] \- Addition of indication to be added in Help and Release notes for simulations with LNG</span>

# Vecto 3.3.1.1463 Release Candidate 2019-01-03

* <span style="color:#000000"> __Changes according to 2017/2400 amendments__ </span>
  * <span style="color:#000000">\[VECTO\-761\] \- Adaptation of input XML Schema</span>
  * <span style="color:#000000">\[VECTO\-762\] \- Extension of Input Interfaces</span>
  * <span style="color:#000000">\[VECTO\-763\] \- Extension of Segmentation Table</span>
  * <span style="color:#000000">\[VECTO\-764\] \- ADAS benefits \(according to Annex III Point 8\. of amendment to 2017/2400\)</span>
  * <span style="color:#000000">\[VECTO\-766\] \- Update power demand auxiliaries \(for extended segmentation table\)</span>
  * <span style="color:#000000">\[VECTO\-767\] \- Report for exempted vehicles</span>
  * <span style="color:#000000">\[VECTO\-768\] \- VTP mode</span>
  * <span style="color:#000000">\[VECTO\-770\] \- Fuel Types</span>
  * <span style="color:#000000">\[VECTO\-771\] \- Handling of exempted vehicles</span>
  * <span style="color:#000000">\[VECTO\-824\] \- Throw exception for certain combinations of exempted vehicle parameters</span>
  * <span style="color:#000000">\[VECTO\-773\] \- Correction Factor for Reference Fuel</span>
  * <span style="color:#000000">\[VECTO\-790\] \- Adapt generic data for construction/municipal utility</span>
  * <span style="color:#000000">\[VECTO\-493\] \- Implementation of generic body weights and air drag values for construction cycle</span>
  * <span style="color:#000000">\[VECTO\-565\] \- Consideration of LNG as possible fuel is missing</span>

* <span style="color:#000000"> __New input parameters__ </span>
  * <span style="color:#000000">The new input fields \(see table below\) are optional in this version\. When this release candidate will be an official version manufacturers MAY certify their new vehicles using the new input parameters\. As from 1</span>  <span style="color:#000000">st</span>  <span style="color:#000000"> July 2019 the new input fields will become mandatory\. Further details are provided in the timetable on the next page\.</span>
  * <span style="color:#000000">Default values when the new input parameters are not provided:</span>

| Input parameters vehicle - Table 1 Annex III |  |
| :-: | :-: |
| Field | Default value |
| VehicleCategory | No default value but input 'rigid truck' will be converted automatically into 'rigid lorry' |
| ZeroEmissionVehicle | 'No' |
| NgTankSystem | 'Compressed' (only applicable to gas vehicles) |
| SleeperCab | 'Yes' |
| Input parameters ADAS - Pt. 8 Annex III |  |
| Field | Default value |
| EngineStartStop | 'No' |
| EcoRollWithoutEngineStop | 'No' |
| EcoRollWithEngineStop | 'No' |
| PredictiveCruiseControl | 'No' |

  * <span style="color:#000000">Currently </span>  <span style="color:#000000"> __only__ </span>  <span style="color:#000000"> the fuel type '</span>  <span style="color:#000000"> __NG PI__ </span>  <span style="color:#000000">' for the </span>  <span style="color:#000000"> __engine certification __ </span>  <span style="color:#000000">is allowed by 2017/2400\. For LNG vehicles\, therefore\, the engine fuel type has to be set to '</span>  <span style="color:#000000"> __NG PI__ </span>  <span style="color:#000000">' and at the vehicle level NgTankSystem has to be set to </span>  <span style="color:#000000"> __liquefied__ </span>  <span style="color:#000000">\. For CNG the same engine fuel type is used but NgTankSystem has to be set to </span>  <span style="color:#000000"> __compressed__ </span>  <span style="color:#000000">\.</span>

| Planned Date | Version | Description |
| :-: | :-: | :-: |
| 1. Feb. 2019 | 3.3.1.x | Release of official version of release candidate 3.3.1.1463 |
| 1. March 2019 | 3.3.2.x-RC | Release candidate, new input parameters are mandatory (+ further bugfixes) |
| 1. April 2019 | 3.3.2.x | Official version of VECTO 3.3.2 |
| 1. May 2019 |  | Mandatory use of 3.3.1.x for certification |
| 1. July 2019 |  | Mandatory use of 3.3.2.x for certification |

* <span style="color:#000000"> __Changes/Improvements__ </span>
  * <span style="color:#000000">\[VECTO\-799\] \- Remove TUG Logos from Simulation Tool\, Hashing Tool</span>
  * <span style="color:#000000">\[VECTO\-808\] \- Add Monitoring Report</span>
  * <span style="color:#000000">\[VECTO\-754\] \- Extending Loss\-Maps in case of AT gearbox for each gear\, axlegear\, gearbox </span>
  * <span style="color:#000000">\[VECTO\-757\] \- Correct contact mail address in Hashing Tool</span>
  * <span style="color:#000000">\[VECTO\-779\] \- Update Construction Cycle \- shorter stop times</span>
  * <span style="color:#000000">\[VECTO\-783\] \- Rename columns in segmentation table and GUI</span>
  * <span style="color:#000000">\[VECTO\-709\] \- VTP editor from user manual not matching new VECTO one: updated documentation</span>
  * <span style="color:#000000">\[VECTO\-785\] \- Handling of Vehicles that cannot reach the cycle's target speed: Limit max speed in driver model</span>
  * <span style="color:#000000">\[VECTO\-716\] \- Validate data in Settings Tab: update documentation</span>
  * <span style="color:#000000">\[VECTO\-793\] \- Inconsistency between GUI\, Help and Regulation: update wording in GUI and user manual</span>
  * <span style="color:#000000">\[VECTO\-796\] \- Adaptation of FuelProperties</span>
  * <span style="color:#000000">\[VECTO\-806\] \- extend loss\-maps \(gbx\, axl\, angl\) for MT and AMT transmissions</span>
  * <span style="color:#000000">\[VECTO\-750\] \- Simulation error DrivingAction: adapt downshift rules for AT to drive over hill with 6% inclination</span>

<span style="color:#000000"> __Update of Fuel Properties \[VECTO\-796\]__ </span>

| Fuel type | Reference for fuel properties | Density | CO2 emission factor | Lower Heating Value | Data Source |
| :-: | :-: | :-: | :-: | :-: | :-: |
| [-] | [-] | [kg/m³] | [g_CO2/g_Fuel] | [MJ/kg] | [-] |
| Diesel | B7 | 836 | 3.13 | 42.7 | CONCAWE/JEC (2018)  |
| ED95 | ED95 | 820 | 1.81 | 25.4 | CONCAWE/JEC (2018) |
| Petrol | E10 | 748 | 3.04 | 41.5 | CONCAWE/JEC (2018) |
| E85 | E85 | 786 | 2.10 | 29.3 | Calculated from E0 and E100  from CONCAWE/JEC (2018) |
| LPG | LPG | not required* | 3.02 | 46.0 | CONCAWE/JEC (2018) |
| CNG | CNG (H-Gas) | not required* | 2.69 | 48.0 | CONCAWE/JEC (2018) |
| LNG | LNG (EU mix. 2016/2030) | not required* | 2.77 | 49.1 | CONCAWE/JEC (2018) |
| * VECTO does not provide volume based figures for gaseous fuels |  |  |  |   |   |

<span style="color:#000000">CONCAWE/JEC \(2018\): </span>  <span style="color:#000000">Specifications are based on a recent analysis \(2018\) performed by CONCAWE/EUCAR and shall reflect typical fuel on the European market\. The data is scheduled to be published in March 2019 in the context of the study: </span>  <span style="color:#000000">Well\-To\-Wheels Analysis Of Future Automotive Fuels And Powertrains in the European Context - Heavy Duty vehicles</span>

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-819\] \- object reference not set to an instance of an object</span>
  * <span style="color:#000000">\[VECTO\-818\] \- SearchOperatingPoint: Unknown response type\. ResponseOverload</span>
  * <span style="color:#000000">\[VECTO\-813\] \- Error "Infinity \[\] is not allowed for SI\-Value"</span>
  * <span style="color:#000000">\[VECTO\-769\] \- DrivingAction Brake: request failed after braking power was found\.ResponseEngineSpeedTooHigh</span>
  * <span style="color:#000000">\[VECTO\-804\] \- Error on simulation with VECTO 3\.3\.0\.1433</span>
  * <span style="color:#000000">\[VECTO\-805\] \- Total vehicle mass exceeds TPMLM</span>
  * <span style="color:#000000">\[VECTO\-811\] \- AMT: ResponseGearShift</span>
  * <span style="color:#000000">\[VECTO\-812\] \- AMT: ResponseOverload</span>
  * <span style="color:#000000">\[VECTO\-822\] \- SIMULATION RUN ABORTED by Infinity</span>
  * <span style="color:#000000">\[VECTO\-792\] \- Vecto Hashing Tool \- error object reference not set to an instance of an object \(overwriting Date element\)</span>
  * <span style="color:#000000">\[VECTO\-696\] \- Problem with Primary Retarder: regression update\, set torque loss to 0 for 0 speed and engaged gear</span>
  * <span style="color:#000000">\[VECTO\-776\] \- Decision Factor \(DF\)  field is emptied after each simulation</span>
  * <span style="color:#000000">\[VECTO\-814\] \- Error: DistanceRun got an unexpected response: ResponseGearshift</span>

# Vecto 3.3.0.1433 Official Release 2018-12-04

* <span style="color:#000000"> __Bugfixes \(compared to 3\.3\.0\.1398\)__ </span>
  * <span style="color:#000000">\[VECTO\-795\] - VECTO Hashing Tool crashes</span>
  * <span style="color:#000000">\[VECTO\-802\] - Error in XML schema for manufacturer's record file</span>
* <span style="color:#000000"> __Bugfixes \(compared to 3\.3\.0\.1250\)__ </span>
  * <span style="color:#000000">\[VECTO\-723\] \- Simulation aborts with engine speed too high in RD cycle</span>
  * <span style="color:#000000">\[VECTO\-724\] \- Simulation aborts with error 'EngineSpeedTooHigh' \- duplicate of VECTO\-744</span>
  * <span style="color:#000000">\[VECTO\-728\] \- Simulation aborts when vehicle's max speed \(n95h\) is below the target speed</span>
  * <span style="color:#000000">\[VECTO\-730\] \- Simulation Aborts with ResponseOverload</span>
  * <span style="color:#000000">\[VECTO\-744\] \- ResponseEngineSpeedTooHigh \(due to torque limits in gearbox\)</span>
  * <span style="color:#000000">\[VECTO\-731\] \- Case Mismatch \- Torque Converter</span>
  * <span style="color:#000000">\[VECTO\-711\] \- Elements without types in CIF and MRF</span>
  * <span style="color:#000000">\[VECTO\-757\] \- Correct contact mail address in Hashing Tool</span>
  * <span style="color:#000000">\[VECTO\-703\] \- PTO output in MRF file</span>
  * <span style="color:#000000">\[VECTO\-713\] \- Manufacturer Information File in the legislation is not compatible with the Simulation results</span>
* <span style="color:#000000"> __Improvement \(compared to 3\.3\.0\.1250\)__ </span>
  * <span style="color:#000000">\[VECTO\-704\] \- Allow VTP\-simulations for AT gearboxes</span>

# Vecto 3.3.0.1398 Release Candiate 2018-10-30

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-723\] \- Simulation aborts with engine speed too high in RD cycle</span>
  * <span style="color:#000000">\[VECTO\-724\] \- Simulation aborts with error 'EngineSpeedTooHigh' \- duplicate of VECTO\-744</span>
  * <span style="color:#000000">\[VECTO\-728\] \- Simulation aborts when vehicle's max speed \(n95h\) is below the target speed</span>
  * <span style="color:#000000">\[VECTO\-730\] \- Simulation Aborts with ResponseOverload</span>
  * <span style="color:#000000">\[VECTO\-744\] \- ResponseEngineSpeedTooHigh \(due to torque limits in gearbox\)</span>
  * <span style="color:#000000">\[VECTO\-731\] \- Case Mismatch \- Torque Converter</span>
  * <span style="color:#000000">\[VECTO\-711\] \- Elements without types in CIF and MRF</span>
  * <span style="color:#000000">\[VECTO\-757\] \- Correct contact mail address in Hashing Tool</span>
  * <span style="color:#000000">\[VECTO\-703\] \- PTO output in MRF file</span>
  * <span style="color:#000000">\[VECTO\-713\] \- Manufacturer Information File in the legislation is not compatible with the Simulation results</span>
* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-704\] \- Allow VTP\-simulations for AT gearboxes</span>

# Vecto 3.3.0.1250 2018-06-04

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-665\] \- Adding style information to XML Reports</span>
  * <span style="color:#000000">\[VECTO\-669\] \- Group 1 vehicle comprises vehicles with gross vehicle weight > 7\.5t</span>
  * <span style="color:#000000">\[VECTO\-672\] \- Keep manual choice for "Validate data"</span>
  * <span style="color:#000000">\[VECTO\-682\] \- VTP Simulation in declaration mode</span>
  * <span style="color:#000000">\[VECTO\-652\] \- VTP: Check Cycle matches simulation mode</span>
  * <span style="color:#000000">\[VECTO\-683\] \- VTP: Quality and plausibility checks for recorded data from VTP</span>
  * <span style="color:#000000">\[VECTO\-685\] \- VTP Programming of standard VECTO VTP report</span>
  * <span style="color:#000000">\[VECTO\-689\] \- Additional Tyre sizes</span>
  * <span style="color:#000000">\[VECTO\-702\] \- Hashing tool: adapt warnings</span>
  * <span style="color:#000000">\[VECTO\-667\] \- Removing NCV Correction Factor</span>
  * <span style="color:#000000">\[VECTO\-679\] \- Engine n95h computation gives wrong \(too high\) engine speed \(above measured FLD\, n70h\)</span>
  * <span style="color:#000000">\[VECTO\-693\] \- extend vehicle performance in manufacturer record</span>

* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-656\] \- Distance computation in vsum</span>
  * <span style="color:#000000">\[VECTO\-666\] \- CF\_RegPer no effect in vehicle simulation \-\- added to the engine correction factors</span>
  * <span style="color:#000000">\[VECTO\-687\] \- Saving a Engine\-Only Job is not possible</span>
  * <span style="color:#000000">\[VECTO\-695\] \- Bug in vectocmd\.exe \- process does not terminate</span>
  * <span style="color:#000000">\[VECTO\-699\] \- Output in manufacturer report and customer report \(VECTO\) uses different units than described in legislation</span>
  * <span style="color:#000000">\[VECTO\-700\] \- errorr in simulation with 0 stop time at the beginning of the cycle </span>

# Vecto 3.2.1.1133 2018-02-07

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-634\] \- VTP Mode: specific fuel consumption</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-642\] \- VECTO BUG - secondary retarder losses: </span>  <span style="color:#000000"> __IMPORTANT:__ </span>  <span style="color:#000000"> Fuel\-consumption relevant bug\! wrong calculation of retarder losses for retarder ratio not equal to 1</span>
  * <span style="color:#000000">\[VECTO\-624\] \- Crash w/o comment: Infinite recursion</span>
  * <span style="color:#000000">\[VECTO\-627\] \- Cannot open Engine\-Only Job</span>
  * <span style="color:#000000">\[VECTO\-629\] \- Vecto crashes without errror message \(same issue as VECTO\-624\)</span>
  * <span style="color:#000000">\[VECTO\-639\] \- Failed to find operating point for braking power: cycle with low target speed \(3km/h\)\. allow driving with slipping clutch</span>
  * <span style="color:#000000">\[VECTO\-640\] \- Exceeded max\. iterations: driving fully\-loaded vehicle steep uphill\. fixed by allowing full\-stop and drive off again</span>
  * <span style="color:#000000">\[VECTO\-633\] \- unable to start VTP Mode simulation</span>
  * <span style="color:#000000">\[VECTO\-645\] \- Encountered error while validating Vecto output \(generated by API\) through Hashing tool for vehicle without retarder</span>

# Vecto 3.2.1.1079 2017-12-15

* <span style="color:#000000"> __Improvement__ </span>
  * <span style="color:#000000">\[VECTO\-618\] \- Add Hash value of tyres to manufacturer's record file</span>
  * <span style="color:#000000">\[VECTO\-590\] \- Handling of hash values: customer's record contains hash of manufacturer's record</span>
  * <span style="color:#000000">\[VECTO\-612\] \- Continuously changing hashes: Info in GUI of HashingTool</span>
  * <span style="color:#000000">\[VECTO\-560\] \- Change Mail\-Address of general VECTO contact</span>
  * <span style="color:#000000">\[VECTO\-616\] \- SI\-Unit \- display derived unit instead of base units</span>
* <span style="color:#000000"> __Bugfixes__ </span>
  * <span style="color:#000000">\[VECTO\-608\] \- Power balance in EPT\-mode not closed</span>
  * <span style="color:#000000"> \[VECTO\-611\] \- Invalid input\. Cannot cast Newtonsoft\.Json\.Linq\.JObject to Newtonsoft\.Json\.Linq\.Jtoken</span>
  * <span style="color:#000000">\[VECTO\-610\] \- TyreCertificationNumber missing in Manufacturer Report</span>
  * <span style="color:#000000">\[VECTO\-613\] \- Incomplete description of allowed values of LegislativeClass \(p251\) in VECTO parameter documentation</span>
  * <span style="color:#000000">\[VECTO\-625\] \- Update XML Schema: Tyre dimensions according to Technicall Annex\, trailing spaces in enums</span>
* <span style="color:#000000"> __Support__ </span>
  * <span style="color:#000000"> __ __ </span>  <span style="color:#000000">\[VECTO\-615\] \- Error torque interpolation in declaration jobs exported to XMLImprovements</span>

# Vecto 3.2.1.1054 2017-11-20

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000">\[VECTO\-592\] \- VTP Simulation Mode</span>

<span style="color:#000000">\[VECTO\-605\] \- Improve simulation speed</span>

<span style="color:#000000"> __Bugfixes__ </span>

<span style="color:#000000">\[VECTO\-602\] \- Error in simulation without airdrag component</span>

<span style="color:#000000">\[VECTO\-589\] \- Scheme \.xml error</span>

# VTP Simulation Mode

* <span style="color:#000000">Verification Test Procedure \(VTP\) Simulation Mode</span>
  * <span style="color:#000000">Similar to Pwheel mode\, different cycle format \(see user manual\)</span>
  * <span style="color:#000000">Requires:</span>
    * <span style="color:#000000">Vehicle in declaration mode \(XML\)</span>
    * <span style="color:#000000">Measured driving cycle</span>
    * <span style="color:#000000">Parameters for engine\-fan model</span>
  * <span style="color:#000000">VECTO calculates the gear based on the wheel speed and engine speed \(and vehicle parameters\) and ignores the gear provided in the driving cycle</span>
  * <span style="color:#000000">Fuel consumption interpolation is done using the engine speed from the cycle and calculated power demand \(to avoid wrong engine speeds due to wrong gear selection\)</span>
  * <span style="color:#000000">Simulation uses all auxiliaries except engine fan</span>
    * <span style="color:#000000">Engine fan is modeled separately\, power demand depends on fan speed \(see user manual\)</span>
  * <span style="color:#000000">Auxiliary power selected according to segment table\, BUT power demand depends on vehicle speed</span>
    * <span style="color:#000000">v < 50 km/h: Urban</span>
    * <span style="color:#000000">50 <= v < 70 km/h: Rural</span>
    * <span style="color:#000000">v >70 km/h: Long haul</span>
  * <span style="color:#000000">Gear and fuel consumption in the driving cycle are optional for now\, may be used in future versions</span>

# Vecto 3.2.0.1022 2017-10-19

<span style="color:#000000"> __Bugfixes__ </span>

<span style="color:#000000">\[VECTO\-585\, VECTO\-587\] - VECTO Simulation aborts when run as WCF Service</span>

<span style="color:#000000">\[VECTO\-586\] - Gearshiftcout in reports too high</span>

<span style="color:#000000">\[VECTO\-573\] - Use of old library references \.net framework 2\.0</span>

# Vecto 3.2.0.1005 2017-10-02

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000"> __Release of Vecto Hashing Tool__ </span>

<span style="color:#000000">\[VECTO\-557\] Engine speed simulated too high during long stops</span>

<span style="color:#000000"> __Bugfixes__ </span>

<span style="color:#000000">\[VECTO\-569\] \- 'Engine Retarder' not correctly recognized as input</span>

<span style="color:#000000">\[VECTO\-571\] \- Customer Report - wrong output format of average RRC</span>

<span style="color:#000000">\[VECTO\-573\] \- Correction of displayed units in graph window</span>

<span style="color:#000000">\[VECTO\-575\] \- Correction of simulation aborts \(due to gearbox inertia\, engineering mode\)</span>

<span style="color:#000000">\[VECTO\-577\] \- Correction of XML export functionality</span>

<span style="color:#000000">\[VECTO\-579\] \- Bug fix GUI crashes on invalid input</span>

<span style="color:#000000">\[VECTO\-558\] \- Correction of output in \.vsum file - BFColdHot always 0</span>

<span style="color:#000000">\[VECTO\-564\] \- Bug fix: correct output of vehicle group in XML report</span>

<span style="color:#000000">\[VECTO\-566\] \- Vehicle height not correctly read \(engineering mode\)</span>

<span style="color:#000000">\[VECTO\-545\] \- Update documentation on Settings dialog</span>

# Vecto 3.2.0.940 2017-07-28

<span style="color:#000000"> __Bugfixes__ </span>

<span style="color:#000000">\[VECTO\-546\] \- GearboxCertificationOptionType Option 2 not accepted by VECTO</span>

<span style="color:#000000">\[VECTO\-547\] \- Engine Manufacturer and Engine Model are empty in \.vsum </span>

<span style="color:#000000">\[VECTO\-548\] \- online user manual</span>

<span style="color:#000000">\[VECTO\-549\] \- Inconsistent \(and wrong\) decimal separator in XML output \(manufacturer report\)</span>

<span style="color:#000000">\[VECTO\-551\] \- Average Tyre RRC not in Customer Information File output</span>

<span style="color:#000000">\[VECTO\-536\] \- GUI: improvements vehicle dialog \(add missing pictures for vehicle categories\)</span>

<span style="color:#000000">\[VECTO\-550\] \- Allow custom settings for AirDensity in Engineering mode</span>

<span style="color:#000000">\[VECTO\-552\] \- set engine rated power\, rated speed to computed values from FLD if not provided as input</span>

# Vecto 3.2.0.925 2017-07-14

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000">\[VECTO\-366\] added EMS vehicle configuration\, EMS is only simulated when engine rated power > 300kW</span>

<span style="color:#000000">\[VECTO\-463\] add pneumatic system technology 'vacuum pump'</span>

<span style="color:#000000">\[VECTO\-465\] change RRC value of trailers \(declaration mode\) from 0\.00555 to 0\.0055 \(due to limits in user interface\)</span>

<span style="color:#000000">\[VECTO\-477\] AT Gearbox\, powershift losses: remove inertia factor</span>

<span style="color:#000000">\[VECTO\-471\] update cross\-wind correction model: height\-dependent wind speed \(see Excel spreadsheet in User Manual folder for details\)</span>

<span style="color:#000000">\[VECTO\-367\] Add Vehicle Design Speed to segmentation table</span>

<span style="color:#000000">\[VECTO\-470\] Add XML reading and export functionality</span>

<span style="color:#000000">\[VECTO\-486\] Adding hashing library</span>

<span style="color:#000000">\[VECTO\-469\] Limit engine max torque \(either due to vehicle or gearbox limits\)\, limit gearbox input speed</span>

<span style="color:#000000">\[VECTO\-466\] Update vehicle payloads: 10% loaded and reference load are simulated</span>

<span style="color:#000000">\[VECTO\-467\] Add generic PTO activation in municipal cycle</span>

<span style="color:#000000">\[VECTO\-468\] Add PTO losses \(idle\) in declaration mode</span>

<span style="color:#000000">\[VECTO\-479</span>  <span style="color:#000000">\] Added PTO option 'only one engaged gearwheel above oil level' with 0 losses</span>

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000">\[VECTO\-483\] Adapt CdxA supplement for additional trailers</span>

<span style="color:#000000">\[VECTO\-494\] Implementation of different fuel types</span>

<span style="color:#000000">\[VECTO\-502\] Implementing standard values for air\-drag area \(if not measured\)</span>

<span style="color:#000000">\[VECTO\-501\] Implement engine idle speed set in vehicle \(must be higher than engine's idle speed value\)</span>

<span style="color:#000000">\[VECTO\-504\] Adding HVAC technology 'none'</span>

<span style="color:#000000">\[VECTO\-489\] Extrapolate gearbox lossmaps \(required when torque limitation by gearbox is ignored\)</span>

<span style="color:#000000">\[VECTO\-505\] Implement AT transmissions in declaration mode</span>

<span style="color:#000000">\[VECTO\-507\] Allow to ignore validation of model data when starting a simulation \(significant improvement on simulation startup time \- about 10s\)</span>

<span style="color:#000000">\[VECTO\-506\] modified method how torque\-converter characteristics in drag is extended\. allow drag\-values in the input\, only add one point at a high speed ratio</span>

<span style="color:#000000">\[VECTO\-509\] Add axle\-type \(vehicle driven\, vehicle non\-driven\, trailer\) to GUI</span>

<span style="color:#000000">\[VECTO\-511\] Add engine idle speed to Vehicle input form \(GUI\)</span>

<span style="color:#000000">\[VECTO\-510\] Write XML reports \(manufacturer\, customer information\) in declaration mode</span>

<span style="color:#000000">\[VECTO\-474\] new driving cycles for Municipal and Regional Delivery</span>

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000">\[VECTO\-522\] step\-up ratio for using torque converter in second gear set to 1\.85 for busses \(still 1\.8 for trucks\)</span>

<span style="color:#000000">\[VECTO\-525\] remove info\-box with max loading in GUI</span>

<span style="color:#000000">\[VECTO\-531\] Payload calculation: limit truck payload to the truck's max payload\. \(earlier versions only limited the total payload of truc \+ trailer to the total max\. payload\, i\.e\. allowed to shifted loading from truck to the trailer\)</span>

<span style="color:#000000">\[VECTO\-533\] allow second driven axle\, rdyn is calculated as average of both driven axles</span>

<span style="color:#000000">\[VECTO\-537\] new Suburban driving cycles</span>

<span style="color:#000000">\[VECTO\-541\] increase declaration mode PT1 curve to higher speeds \(2500 is too low for some engines\)</span>

<span style="color:#000000">\[VECTO\-542\] reduce overspeed in declaration mode to 2\.5km/h</span>

<span style="color:#000000"> __Bugfixes__ </span>

<span style="color:#000000">\[VECTO\-462\] fix: decision if PTO cycle is simulated</span>

<span style="color:#000000">\[VECTO\-473\] fix: adapt range for validation of torque converter characteristics</span>

<span style="color:#000000">\[VECTO\-464\] fix: extrapolation of engine full\-load curve gives neg\. max\. torque\. Limit engine speed to n95h</span>

<span style="color:#000000">\[VECTO\-480\] fix: a\_pos in \.vsum was less than zero</span>

<span style="color:#000000">\[VECTO\-487\] fix: Duration of PTO cycle was computed incorrectly if PTO cycle does not start at t=0</span>

<span style="color:#000000">\[VECTO\-514\] fix: sort entries in \.vsum numerically\, not lexically</span>

<span style="color:#000000">\[VECTO\-516\] fix: consider axlegear losses for estimation of acceleration after gearshifts</span>

<span style="color:#000000">\[VECTO\-517\] fix: valid shift polygon was considered invalid when extended to very high torque ranges</span>

<span style="color:#000000">\[VECTO\-424\] fix: VectoCore\.dll could not be found when the current working directory is different to the directory of the vectocmd\.exe</span>

<span style="color:#000000">\[VECTO\-425\] fix: vectocmd\.exe \- check if the output is redirected\, and skip updating of the progress bar when this is the case</span>

<span style="color:#000000">\[VECTO\-426\] fix: vectocmd\.exe \- log errors to STDERR</span>

<span style="color:#000000">\[VECTO\-519\] fix: computation of n95h fails for a valid full\-load curve due to numerical inaccuracy\. add tolerance when searching for solutions</span>

<span style="color:#000000">\[VECTO\-520\] fix: gearashift count in vsum is 0</span>

# Vecto 3.1.2.810 2017-01-18

<span style="color:#000000"> __Improvements__ </span>

<span style="color:#000000">\[VECTO\-445\] Additional columns in vsum file</span>

<span style="color:#000000">Allow splitting shift losses among multiple simulation intervals</span>

<span style="color:#000000">Allow coasting overspeed only if vehicle speed > 0</span>

<span style="color:#000000">Torque converter: better handling of 'creeping' situations</span>

<span style="color:#000000"> __Bugfixes:__ </span>

<span style="color:#000000">\[VECTO\-443\] Bugfix in AMT shift strategy: skip gears not working correctly</span>

# Vecto 3.1.2.796 2017-03-07

<span style="color:#000000"> __Improvements:__ </span>

<span style="color:#000000">\[VECTO\-405\] Adding clutch\-losses for AMT/MT gearboxes during drive\-off\, reduce drive\-off distance after stop from 1m to 0\.25m\, set clutch closing speed \(normalized\) to 6\.5%\, changes in clutch model</span>

<span style="color:#000000">\[VECTO\-379\] Make GUI more tolerant against missing files\. Instead of aborting reading the input data the GUI shows a suffix for missing input files</span>

<span style="color:#000000">\[VECTO\-411\] Allow a traction interruption of 0s for AMT/MT gearboxes</span>

<span style="color:#000000">\[VECTO\-408\] Gearbox Inertia for AT gearboxes set to 0</span>

<span style="color:#000000">\[VECTO\-419\] Adapted error messages\, added list of errors</span>

<span style="color:#000000">\[VECTO\-421\,VECTO\-439\] Added volume\-related results to vsum file \(volume is computed based on default bodies\)</span>

<span style="color:#000000">\[\] Energy balance \(vsum\) and balance of engine power output and power consumers \(vmod\) level</span>

<span style="color:#000000">\[VECTO\-430\] AT shift strategy: upshifts may happen too early</span>

<span style="color:#000000">\[VECTO\-431\] AMT shift strategy always started in first gear due to changes in clutch model</span>

<span style="color:#000000">\[VECTO\-433\] adapt generic vehicles: use typical WHTC correction factors</span>

<span style="color:#000000">\[VECTO\-437\] set vehicle speed at clutch\-closed to 1\.3 m/s</span>

<span style="color:#000000">\[VECTO\-436\] fix simulation aborts with AT gearbox \(neg\. braking power\, unexpected response\, underload\)</span>

<span style="color:#000000"> __Bugfixes:__ </span>

<span style="color:#000000">\[VECTO\-415\] Powershift Losses were not considered for AT gearboxes with PowerSplit</span>

<span style="color:#000000">\[VECTO\-416\] Measured Speed with gear failed when cycle contained parts with eco\-roll \(computation of next gear failed\)</span>

<span style="color:#000000">\[VECTO\-428\] Sum of timeshares adds up to 100%</span>

<span style="color:#000000">\[VECTO\-429\] Min Velocity for lookahead coasting was not written to JSON file</span>

# Vecto 3.1.1.748 2017-01-18

<span style="color:#000000"> __Bugfixes:__ </span>

<span style="color:#000000">\[VECTO\-404\] Driving Cycle with PTO stopped simulation after first PTO activation</span>

<span style="color:#000000"> __Improvements:__ </span>

<span style="color:#000000">\[VECTO\-390\, VECTO\-400\] Adapt engine speed to estimated engine speed after gear shift during traction interruption \(double clutching\)</span>

<span style="color:#000000">\[VECTO\-396\, VECTO\-388\] Add shift losses for AT power shifts</span>

<span style="color:#000000">\[VECTO\-389\] new gear shift rules for AT gearboxes</span>

<span style="color:#000000">\[VECTO\-387\] added max input speed for torque converter</span>

<span style="color:#000000">\[VECTO\-385\] Automatically add generic torque converter data for drag</span>

<span style="color:#000000">\[VECTO\-399\] Add missions and loadings for vehicle categories 11\, 12\, and 16 \(declaration mode\)</span>

<span style="color:#000000">\[VECTO\-384\] cleanup memory after simulation run</span>

<span style="color:#000000">\[VECTO\-394\] new option for vectocmd to disable all output</span>

<span style="color:#000000">\[VECTO\-392\] make the GUI scale depending on the Windows font size</span>

<span style="color:#000000">\[VECTO\-391\] Gearbox output speed and output torque added to \.vmod files</span>

<span style="color:#000000">\[VECTO\-386\] Gearbox window: disable input fields not applicable for the selected gearbox type</span>

<span style="color:#000000"> __Bugfixes:__ </span>

<span style="color:#000000">\[VECTO\-401\] Computation of n\_95h etc\. fails if engine's max torque is constant 0 </span>

<span style="color:#000000">Lookup of Airdrag parameters in declaration mode</span>

<span style="color:#000000">\[VECTO\-378\] Improved file\-handling in AAUX module</span>

# Vecto 3.1.0.683 2016-11-14

<span style="color:#000000"> __Bugfixes:__ </span>

<span style="color:#000000">\[VECTO\-375\] Fixed bug when braking during slope change from negative to positive values\.</span>

<span style="color:#000000">\[VECTO\-372\] Added check for unusual acceleration/deceleration data which could lead to error when halting\.</span>

<span style="color:#000000">\[VECTO\-371\] Added additional behavior to overcome such situations</span>

<span style="color:#000000">\[VECTO\-370\] Added additional behavior to overcome such situations</span>

<span style="color:#000000">\[VECTO\-369\] CrosswindCorrection is now saved and read again from JSON files</span>

<span style="color:#000000">\[VECTO\-373\] WHTC\-Engineering correction factor now correctly read/write in JSON files</span>

<span style="color:#000000">\[VECTO\-368\] Fixed validation for specific cases when values are intentionally invalid\.</span>

<span style="color:#000000">\[VECTO\-357\] Updated GUI to not show ECO\-Roll option to avoid confusion</span>

<span style="color:#000000">Fixed numerous bugs in AT\-ShiftStrategy regarding the Torque Converter</span>

<span style="color:#000000">Fixed numerous bugs in MeasuredSpeed Mode \(and MeasuredSpeed with Gear\) in connection with AT\-Gearbox and TorqueConverter</span>

<span style="color:#000000">Fixed a bug when PTO\-Cycle was missing</span>

<span style="color:#000000">Corrected axle loss maps for Generic Vehicles in Declaration Mode to match technical annex</span>

<span style="color:#000000">Corrected SumFile Cruise Time Share\. Added that timeshares must add up to 100%</span>

<span style="color:#990000"> __Vecto 3\.1\.0\.683__ </span>  <span style="color:#990000"> __2016\-11\-14__ </span>

<span style="color:#000000"> __Improvements:__ </span>

<span style="color:#000000">\[VECTO\-355\] Updated documentation\, added powertrain schematics in chapter "Simulation Models"</span>

<span style="color:#000000">\[VECTO\-374\] Check range for Torque Converter speed ratio input data to be at least between 0 and 2\.2</span>

<span style="color:#000000">Updated many error messages to be more explicit about the reason of error</span>

<span style="color:#000000">Added "Mission Profiles" Directory with driving cycles publicly available in the application root directory\.</span>

<span style="color:#000000">Added "Declaration" directory with the declaration data files in the application root directory\.</span>

<span style="color:#000000">Added warning when engine inertia is 0</span>

<span style="color:#000000">Added check that engine speed must not fall below idle speed \(even in measured speed mode\)</span>

<span style="color:#000000">Shift curve validation for AT gearboxes: shift curves may now overlap due to different shift logic in AutomaticTransmissions\.</span>

<span style="color:#000000">Updated Crosswind Coefficients for </span>  <span style="color:#000000">Tractor\+Semitrailer</span>

# Vecto 3.1.0.662 2016-10-24

* <span style="color:#000000"> __Bugfixes__ </span>  <span style="color:#000000">:</span>
  * <span style="color:#000000">\[VECTO\-360\] Fixed error during startup of VECTO \(loading of DLLs\)\.</span>
  * <span style="color:#000000">\[VECTO\-358\] Fixed errors during simulation where vehicle unintentionally was driving backwards\. Fixed 1Hz\-Filter for ModFiles \(distance was wrong under certain circumstances\, vehicle seemingly jumping back before halt\)</span>
  * <span style="color:#000000">\[VECTO\-361\] Fixed classification of vehicles with GVM of exactly 7500kg</span>
  * <span style="color:#000000">\[VECTO\-364\] Fixed an error in measured speed mode \(run aborts\)\.</span>
  * <span style="color:#000000">\[VECTO\-363\] Compute shift polygons in declaration mode now uses correct boundary for full load margin\.</span>
  * <span style="color:#000000">\[VECTO\-365\] Fixed editing gears in declaration mode</span>
* <span style="color:#000000"> __Improvements__ </span>  <span style="color:#000000">:</span>
  * <span style="color:#000000">\[VECTO\-355\] User Manual updated \(Screenshots\, Descriptions\, File Formats\, Vecto V2 Comments removed\)\.</span>
  * <span style="color:#000000">\[VECTO\-317\] Declaration data for Wheel sizes updated</span>
  * <span style="color:#000000">\[VECTO\-359\] Simplified code regarding PT1 behavior\.</span>
  * <span style="color:#000000">\[VECTO\-323\] PTO\-Cycle may now be left empty when not used in driving cycle\.</span>

* <span style="color:#000000">Main Updates</span>
  * <span style="color:#000000">Removed VECTO Core 2\.2</span>
  * <span style="color:#000000">Refactoring of the User\-Interface Backend: loading\, saving files and validating user input uses Vecto 3 models</span>
  * <span style="color:#000000">AT\-Gearbox Model: differentiate between AT gearbox with serial torque converter and AT gearbox using powersplit</span>
  * <span style="color:#000000">Numbering of gears with AT gearbox corresponds to mechanical gears\, new column TC\_locked in \.vmod file to indicate if torque converter is active</span>
  * <span style="color:#000000">Torque converter gear no longer allowed in input \(added by Vecto depending on the AT model\)</span>
  * <span style="color:#000000">New implementation of torque converter model \(analytic solutions\)</span>
  * <span style="color:#000000">Added PTO option for municipal utility vehicles: PTO idle losses\, separate PTO cycle during standstill</span>
  * <span style="color:#000000">Added Angledrive Component</span>
  * <span style="color:#000000">Option for constant Auxiliary Power Demand in Job\-File</span>

* <span style="color:#000000">Main Updates \(cont\.\)</span>
  * <span style="color:#000000">Normalize x/y values before triangulating Delaunay map \(transmission loss\-maps\, fuel consumption loss map\)</span>
  * <span style="color:#000000">Additional fuel consumption correction factor in declaration mode: cold/hot balancing factor</span>
  * <span style="color:#000000">Added fuel consumption correction factor \(WHTC\, Cold/Hot balancing\, …\) in engineering mode</span>
  * <span style="color:#000000">Update auxiliaries power demand according to latest whitebook</span>
  * <span style="color:#000000">Allow multiple steered axles</span>
  * <span style="color:#000000">Adapted engine idle controller \(during declutch\) - engine speed decreases faster</span>
  * <span style="color:#000000">SUM\-File: split E\_axl\_gbx into two columns\, E\_axl and E\_gbx</span>
  * <span style="color:#000000">New columns in mod\-file: PTO\, torque converter</span>
  * <span style="color:#000000">Removed full\-load curve per gear\, only single value MaxTorque</span>
  * <span style="color:#000000">Removed rims \(dynamic wheel radius depends on wheel type\)</span>
  * <span style="color:#000000">Fixes in AAUX module: open correct file\-browser\, save selected files</span>

# Status quo VECTO software and open issues (Oct. 2016)

<span style="color:#000000"> __Next issues on the to do list__ </span>

* <span style="color:#000000"> __Further development of the AT model__ </span>  <span style="color:#000000">Consideration of losses during power shifts\, update of gear shift logics</span>
* <span style="color:#000000"> __Reimplementation of engine stop/start__ </span>
* <span style="color:#000000"> __Declaration mode: implementation of EMS vehicle configurations__ </span>
* <span style="color:#000000"> _Items waiting for decision on methods and resources:_ </span>
* <span style="color:#000000"> __Update engine data \(according to update of Annex II\)__ </span>  <span style="color:#000000">Other fuels than diesel\, "top torque" feature\, correction factor for periodic regerating DPFs</span>
* <span style="color:#000000"> __Declaration mode: __ </span>
  * <span style="color:#000000"> __Revision of calculated vehicle loads__ </span>
  * <span style="color:#000000"> __implementation of refuse cycle \(instead "municipal"\)__ </span>  <span style="color:#000000">Update of driving cycle\, consideration of generic PTO loads during collection part\,</span>  <span style="color:#000000">generic body weight and payload</span>
  * <span style="color:#000000"> __VECTO output \(approval authorities\, customer info\, monitoring\)__ </span>
  * <span style="color:#000000"> __Buses__ </span>
* <span style="color:#000000"> __Predictive ADAS__ </span>

# Vecto 3.0.4.565 2016-07-19

* <span style="color:#000000">Bugfixes</span>
  * <span style="color:#000000">AAUX HVAC Dialog does not store path to ActuationsMap and SSMSource</span>
  * <span style="color:#000000">GUI: check for axle loads in declaration mode renders editing dialog useless </span>
  * <span style="color:#000000">Vecto 2\.2: Simulation aborts \(Vecto terminates\) when simulating EngineOnly cycles</span>
  * <span style="color:#000000">Vecto 3: Building SimulationRun EngineOnly simulation failed</span>

# Vecto 3.0.4.544 2016-06-28

* <span style="color:#000000">Main Updates</span>
  * <span style="color:#000000">New gear shift strategy according to White Book 2016</span>
  * <span style="color:#000000">New coasting strategy according to White Book 2016</span>
  * <span style="color:#000000">New input parameters \(enineering mode\) for coasting and gear shift behavior</span>
  * <span style="color:#000000">Use SI units in Advanced Auxiliaries Module and compile with strict compiler settings \(no implicit casts\, etc\.\)</span>
  * <span style="color:#000000">Allow efficiency for transmission losses \(in engineering mode\)</span>
* <span style="color:#000000">Bugfixes</span>
  * <span style="color:#000000">Auxiliary TechList not read from JSON input data</span>
  * <span style="color:#000000">Improvements in driver strategy</span>
  * <span style="color:#000000">Bugfixes in MeasuredSpeed mode</span>

# Notes for using Vecto 3.x with AAUX (1)

<span style="color:#000000">The AdvancedAuxiliaries module requires the number of activations for pneumatic consumers \(brakes\, doors\, kneeling\) and the \(estimated\) total cycle time\. This can be configured in the \.APAC\-file \(actuations file\)\. For standard bus/coach cycles \(i\.e\.\, the cycle file contains "bus" </span>  <span style="color:#000000"> __and__ </span>  <span style="color:#000000"> "heavy\_urban" or "suburban" or "interurban" or "urban"; </span>  <span style="color:#000000"> __or__ </span>  <span style="color:#000000"> the cycle contains "coach" \(</span>  <span style="color:#000000"> _case insensitive_ </span>  <span style="color:#000000">\)\) the actuations file already contains the number of activations and the cycle time\. For other cycles the filename without extension is used to lookup the activations in the \.APAC file \(</span>  <span style="color:#000000"> _case sensitive_ </span>  <span style="color:#000000">\)</span>

* <span style="color:#000000">Vecto 3 uses an average auxiliaries load \(determined by the AAUX module depending on the settings\) for the simulation\. The AAUX module computes the fuel consumption in parallel to VectoCore and accounts for smart consumers \(e\.g\.\, alternator\, pneumatics\, …\)\.</span>
* <span style="color:#000000">Output</span>
  * <span style="color:#000000">The \.vmod file contains both\, the fuel consumption calculated by VectoCore \(per simulation interval\) and AAUX \(accumulated and per simulation interval\)\.</span>  <span style="color:#000000">Columns in \.vmod file:</span>
    * <span style="color:#000000">AA\_TotalCycleFC\_Grams \[g\]: accumulated fuel consumption as computed by the AAUX model\, considering smart consumers</span>
    * <span style="color:#000000">FC\-Map \[g/h\]: fuel consumption as computed by VectoCore interpolating in the FC\-Map\, using an average base load of auxiliaries</span>
    * <span style="color:#000000">FC\-AUXc \[g/h\]: fuel consumption corrected due to engine stop/start \(currently not applicable\)</span>
    * <span style="color:#000000">FC\-WHTCc \[g/h\]: WHTC\-corrected fuel consumption \(not applicable in engineering mode\)</span>
    * <span style="color:#000000">FC\-AAUX \[g/h\]: fuel consumption per simulation interval\, derived from AA\_TotalCycleFC\_Grams</span>
    * <span style="color:#000000">FC\-Final \[g/h\]: final fuel consumption value with all \(applicable\) corrections applied \(stop/start\, WHTC\, smart auxiliaries\)</span>

* <span style="color:#000000">Output \.vsum</span>
  * <span style="color:#000000">Columns in \.vsum file:</span>
    * <span style="color:#000000">FC\-Map: total fuel consumption as computed by VectoCore interpolating in the FC\-Map\, using an average base load of auxiliaries</span>
    * <span style="color:#000000">FC\-AUXc: total fuel consumption corrected due to engine stop/start \(currently not applicable\)</span>
    * <span style="color:#000000">FC\-WHTCc: WHTC\-corrected fuel consumption \(not applicable in engineering mode\)</span>
    * <span style="color:#000000">FC\-AAUX: fuel consumption per simulation interval\, derived from AA\_TotalCycleFC\_Grams</span>
    * <span style="color:#000000">FC\-Final: final fuel consumption value with all \(applicable\) corrections applied \(stop/start\, WHTC\, smart auxiliaries\)</span>

# Vecto 3.0.3.537 2016-06-21

* <span style="color:#000000">Main Updates</span>
  * <span style="color:#000000">Plot shift lines as computed according to WB 2016 declaration mode in GUI</span>
* <span style="color:#000000">Bugfixes</span>
  * <span style="color:#000000">GUI: Buttons to add/remove auxiliaries are visible again</span>
  * <span style="color:#000000">Error in calculation of WHTC correction factor</span>
  * <span style="color:#000000">Fix: consider gearbox inertia \(engineering mode\) for searching operating point</span>
  * <span style="color:#000000">Wrong output of road gradient in measured speed mode \(correct gradient for simulation\)</span>
  * <span style="color:#000000">Fuel consumption in \.vsum file now accounts for AdvancedAuxiliaries model</span>
  * <span style="color:#000000">GraphDrawer \(Vecto\): handle new \.vmod format of Vecto 3</span>
  * <span style="color:#000000">AdvancedAuxiliaries: language\-settings independent input parsing</span>
  * <span style="color:#000000">Paux was ignored when running Vecto 2\.2</span>
  * <span style="color:#000000">Error in massive multithreaded execution</span>
  * <span style="color:#000000">Fix unhandled response during simulation</span>
  * <span style="color:#000000">Fix output columns in \.vmod</span>

# Vecto 3.0.3.495 2016-05-10

* <span style="color:#000000">Main Updates</span>
  * <span style="color:#000000">Support for Advanced Auxiliaries \(Ricardo\) in Vecto 3\.0\.3 and Vecto 2\.2</span>
  * <span style="color:#000000">Performance improvements</span>
  * <span style="color:#000000">Gearshift polygons according to WB 2016</span>
  * <span style="color:#000000">Revision of SUM\-data file\, changed order of columns\, changed column headers</span>
* <span style="color:#000000">Bugfixes</span>
  * <span style="color:#000000">Delaunay Maps: additional check for duplicate input points</span>
  * <span style="color:#000000">Creation of PDF Report when running multiple jobs at once</span>
  * <span style="color:#000000">Sanity checks for gear shift lines</span>
  * <span style="color:#000000">Improvements DriverStrategy: handling special cases</span>

# Performance Comparison

![](img%5Crelease%20notes%20vecto3x64.png)

<span style="color:#000000">Total execution time \(15 runs in parallel\):  Vecto 3\.0\.2: 6min 6s; </span>  <span style="color:#000000"> __Vecto 3\.0\.3: 35s__ </span>

<span style="color:#990000"> __VECTO 3\.0\.2__ </span>  <span style="color:#990000"> __2016\-03\-11__ </span>

* <span style="color:#000000"> __Main updates__ </span>
* <span style="color:#000000">New simulation modes: </span>
  * <span style="color:#000000">Pwheel \(SiCo\)\, </span>
  * <span style="color:#000000">Measured Speed \(with/without gear\)</span>
  * <span style="color:#000000">v\_air/beta cross\-wind correction \(vcdb\)</span>
* <span style="color:#000000">Adaptations of powertrain components architecture</span>
  * <span style="color:#000000">Move wheels inertia from vehicle to wheels</span>
  * <span style="color:#000000">Auxiliaries no longer connected via clutch to the engine but via a separate port</span>
  * <span style="color:#000000">Engine checks overload of gearbox and engine overload</span>
* <span style="color:#000000">Fixed some driving behavior related issues in VectoCore:</span>
  * <span style="color:#000000">When the vehicle comes to a halt during gear shift\, instead of aborting the cycle\, it tries to drive away again with an appropriate gear\.</span>
* <span style="color:#000000">ModData Format changed for better information and clarity</span>
* <span style="color:#000000">Added validation of input values \(according to latest VectoInputParameters\.xls\)</span>
* <span style="color:#000000">Various bugfixes</span>

# Pwheel (SiCo) Mode

* <span style="color:#000000">Function as already available in Vecto 2\.2 also added in Vecto 3\.0\.2</span>
  * <span style="color:#000000">Driving cycle specifies power at wheel\, engine speed\, gear\, and auxiliary power</span>
  * <span style="color:#000000">No driver model in the simulation\.</span>
  * <span style="color:#000000">The Vecto gear\-shift model is overruled\.</span>
  * <span style="color:#000000">Function used for creating reference results for SiCo tests</span>
  * <span style="color:#000000">For details see user manual: Simulation Models / Pwheel Input \(SiCo\)</span>

# Measured Speed Mode

* <span style="color:#000000">Functionality already available in Vecto 2\.2 added in Vecto 3\.0\.2</span>
  * <span style="color:#000000">Driving cycle not defined by target speed but by actual speed\. No driver model in the simulation\.</span>
  * <span style="color:#000000">Gear and engine speed can be specified in the driving cycle\. In this case the Vecto gear\-shift model is overruled\.</span>
  * <span style="color:#000000">Function used for "proof of concept" purposes</span>
  * <span style="color:#000000">For details see user manual: Calculation Modes / Engineering Mode / Measured Speed</span>

# .vmod File Update

<span style="color:#000000">In Vecto 3\.0\.2 the structure of the modal data output has been revised and re\-structured\. Basicaly for every powertrain component the \.vmod file contains the power at the input shaft and the individual power losses for every component\. For the engine the power\, torque and engine speed at the output shaft is given along with the internal power and torque used for computing the fuel consumption\.</span>

<span style="color:#000000">For details see the user manual: Input and Output / Modal Results \(\.vmod\)</span>

# Changelog 3.0.2

<span style="color:#000000">\- New simulation modes:</span>

<span style="color:#000000">\+ Measured Speed</span>

<span style="color:#000000">\+ Measured Speed with Gear</span>

<span style="color:#000000">\+ Pwheel \(SiCo\)</span>

<span style="color:#000000">\- Adaptations of powertrain components architecture</span>

<span style="color:#000000">\+ Move wheels inertia from vehicle to wheels</span>

<span style="color:#000000">\+ Auxiliaries no longer connected via clutch to the engine but via a separate port</span>

<span style="color:#000000">\+ Engine checks overload of gearbox and engine overload</span>

<span style="color:#000000">\- Fixed some driving behavior related issues in VectoCore:</span>

<span style="color:#000000">\+ When the vehicle comes to a halt during gear shift\, instead of aborting the cycle\, it tries to drive away again with an appropriate gear\.</span>

<span style="color:#000000">\- \[ModData Format\]\(\#modal\-results\-\.vmod\) changed for better information and clarity</span>

<span style="color:#000000">\- Entries in the sum\-file are sorted in the same way as in Vecto 2\.2</span>

<span style="color:#000000">\- In engineering mode the execution mode \(distance\-based\, time\-based measured speed\, time\-based measured speed with gear\, engine only\) are detected based on the cycle</span>

<span style="color:#000000">\- Added validation of input values</span>

<span style="color:#000000">\- Gravity constant set to 9\.80665 \(NIST standard acceleration for gravity\)</span>

<span style="color:#000000">\- Improved input data handling: sort input values of full\-load curves \(engine\, gbx\, retarder\)</span>

<span style="color:#000000">\- Better Integration of VectoCore into GUI \(Notifications and Messages\)</span>

<span style="color:#000000">\- v\_air/beta cross\-wind correction \(vcdb\) impemented</span>

<span style="color:#000000">\- For all calculations the averaged values of the current simulation step are used for interpolations in loss\-maps\.</span>

<span style="color:#000000">\- Allow extrapolation of loss maps in engineering mode \(warnings\)</span>

<span style="color:#000000">\- Refactoring of input data handling: separate InputDataProvider interfaces for model data</span>

<span style="color:#000000">\- Refactoring of result handling: separate result container and output writer</span>

<span style="color:#000000">\- New Long\-Haul driving cycle included</span>

<span style="color:#000000">\- User Manual updated for VECTO V3\.x</span>

<span style="color:#000000">\- Fix: sparse representation of declaration cycles had some missing entries</span>

<span style="color:#000000">\- Bugfix: error in computation of engine's preferred speed </span>

<span style="color:#000000">\- Bugfix: wrong vehicle class lookup</span>

<span style="color:#000000">\- Bugfix: duplicate entries in intersected full\-load curves</span>

<span style="color:#000000">\- Bugfix: retarder takes the retarder ratio into account for lossmap lookup</span>

<span style="color:#000000">\- Bugfix: use unique identifier for jobs in job list</span>

<span style="color:#000000">\- Bugfix: error in triagulation of fuel consumption map</span>

