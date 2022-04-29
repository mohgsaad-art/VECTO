## HVAC Auxiliaries Editor
 
![](pics/AA_HVAC.jpg)

### Description

The "HVAC" tab defines various parameters for heating, ventilation and air conditioning (HVAC) auxiliaries used on the vehicle, calculated from the HVAC Steady State Model (HVAC SSM):
-   Disable HVAC Module [tickbox]
-   Filepath to the Steady State Model File (.AHSM)
    : Files can be imported by clicking the browse button adjacent to the HVAC "Steady State Model File (.AHSM)" text box.
-   Filepath to the Bus Parameter Database (.ABDB)
    " Files can be imported by clicking the browse button adjacent to the HVAC SSM bus parameters database file (.ABDB) text box.  The bus parameter database contains a list of default parameters for a number of pre-existing/defined buses that can be quickly switched between within the HVAC SSM Editor module.

Outputs from the HVAC SSM include:
-   Electrical Load Power Watts
-   Mechanical Load Power Watts
-   Fuelling Litres Per Hour


### HVAC Steady-State Model Editor

The HVAC Steady-State Model (HVAC SSM) Editor defines various data and parameters for calculation of HVAC auxiliary demands (electrical, mechanical and fuelling) from the vehicle, replicating the key inputs/functionality from the HVAC CO2SIM model developed for ACEA:

-   Bus Parameters
-   Boundary Conditions
-   Other
-   Tech List Input
-   Diagnostics

At the top of the window, two sets of outputs are presented for electrical, mechanical and fuelling demand:

-   'Base' values: These are the calculated resulting demands from the inputs on the 'Bus Parameters', 'Boundary Conditions' and 'Other' tabs.
-   'Adjusted' values: these are the final values output from the model, which additionally factor in the HVAC technologies included in the 'Tech List Input' tab.

### Bus Parameters

![](pics/HVAC_BusParameters.jpg)

Input bus parameters can be edited directly or imported/calculated from the Bus Parameter Database (.abdb) file via the '\<Select\>' drop-down box at the top of the page. Parameters in the accompanying database file (.abdb) include:

-   Bus Model Name (free text)
-   Registered passengers
-   Type (i.e. 'raised floor' = Class III, 'semi low floor' = Class II, or 'low floor' = Class I)
-   Is Double Decker [tick box]
-   Length in m,
-   Wide in m,
-   Height in m,
-   \[Engine Type (only 'diesel' is currently supported), only when creating a 'New' entry\]
-   Other fields, that are greyed out, are locked and not editable, containing fixed default values or calculations.
 

### Boundary Conditions

![](pics/HVAC_BoundaryConditions.jpg)

On this tab the various boundary conditions for the HVAC SSM calculations can be set. Certain fields (greyed out) are locked and not editable, containing fixed default values or calculations.

### Other

![](pics/HVAC_Other.jpg)

On this tab a number of other parameters for the HVAC SSM calculations can be set:
-    Environmental conditions: when in 'Batch Mode' a climatic conditions dataset (.aenv) file must be used containing a series of environmental conditions. Otherwise single values for temperature and solar load may be input (these fields are locked/not used when in batch mode).
-   AC System specifications/type: the AC-Compressor Type selection determines the COP value used, according to the specification of the project steering group.
-   Ventilation settings
-   Auxiliary Heater parameters: the power of the fuel fired heater may be included, other fields are provided for information only and are locked. The 'Engine Waste Heat' values are calculated during the actual model runs, which are determined via a pre-run of the model over the selected drive-cycle.

### TechList Input

![](pics/HVAC_TechList.jpg)

To determine energy consumption of a certain bus-HVAC system combination, a customisable list of technologies may be added/edited on this tab to allow to take special features into account which have a reducing or increasing influence. Because several technologies are only available for certain bus types, the list has to be bus type-specific. The technologies list and the default values has been populated according to the steering group recommendations, however these may be deleted, edited or added to as required on this tab in Engineering mode.

**Diagnostics**

The final 'Diagnostics' tab provides a summary of the resulting outputs from the HVAC Tech List tab.


### Default Values

The following table provides a summary of the default values that are populated whenever a new advanced auxiliaries (.AAUX) file is created from scratch.  The table also indicates the editable/default status of the relevant parameters in the VECTO UI in Engineering mode, and the recommended status in Declaration mode (not currently implemented).  The default values / parameter status has been agreed with the project steering group.

**Default parameter values and editable status for the HVAC module**

**INP - BusParameters tab**

*Bus Parameterisation*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Select | \<Select\> |   |   | 
| Bus Model | <ABDB or input> | Open/editable | Locked default |
| Number of Passengers | \<ABDB or input\> | Open/editable | Locked default | 
| Bus Type | <ABDB or input> | Open/editable | Locked default | 
| Double Decker? | No | Open/editable | Open/OEM data | 
| Bus Length (m) | \<ABDB or input\> | Open/editable | Locked default | 
| Bus Width (m) | \<ABDB or input\> | Open/editable | Locked default | 
| Bus Height (m) | \<ABDB or input\> | Open/editable | Locked Calc | 
| Bus Floor Surface Area (m^2) | Calculation | Locked Calc | Locked Calc | 
| Bus Window Surface (m^2) | Calculation | Locked Calc | Locked Calc | 
| Bus Surface Area (m^2) | Calculation | Locked Calc | Locked Calc | 
| Bus Volume (m^3) | Calculation | Locked Calc | Locked Calc | 

**INP - Boundary Conditions tab**

*Boundary Conditions*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| G-Factor ** | 0.95 | Open/editable | Open/editable | 
| Solar Clouding | 0.8 | Locked Calc | Locked Calc | 
| Heat per Passenger into Cabin (W) | 80 | Locked Calc | Locked Calc | 
| Passenger Boundary Temperature (oC) | 12 | Open/editable | Locked default | 
| Passenger Density: Low Floor (Pass/m^2) | 3 | Locked Calc | Locked default | 
| Passenger Density: Semi Low Floor (Pass/m^2) | 2.2 | Locked Calc | Locked default |
| Passenger Density: Raised Floor (Pass/m^2) | 1.4 | Locked Calc | Locked default |
| Calculated Passenger Number | Calculation | Locked Calc | Locked Calc | 
| U-Values W/(K\*m^3) | Calculation | Locked Calc | Locked Calc | 
| Heating Boundary Temperature (oC) | 18 | Open/editable | Locked default | 
| Cooling Boundary Temperature (oC) | 23 | Open/editable | Locked default | 
| Temperature at which cooling turns OFF | 17 | Locked default |   | 
| High Ventilation (l/h) | 20 | Open/editable | Locked default | 
| How Ventilation (l/h) | 7 | Open/editable | Locked default | 
| High (m^3/h) | Calculation | Locked Calc | Locked Calc | 
| low (m^3/h) | Calculation | Locked Calc | Locked Calc | 
| High Vent Power (W) | Calculation | Locked Calc | Locked Calc | 
| Low Vent Power (W) | Calculation | Locked Calc | Locked Calc | 
| Specific Ventilation Power (Wh/m3) | 0.56 | Open/editable | Locked default | 
| Aux. Heater Efficiency | 0.84 | Open/editable | Locked default | 
| GCV (Diesel / Heating oil) (kwh/kg) | 11.8 | Open/editable | Locked default | 
| Window Area per Unit Bus Length (m^2/m) | Calculation | Locked Calc | Locked Calc |
| Front + Rear Window Area (m^2) | Calculation | Locked Calc | Locked Calc | 
| Max Temperature Delta for low Floor Busses (K) | 3 | Open/editable | Locked default | 
| Max Possible Benefit from Technology List (Fraction) | 0.5 | Open/editable | Locked default |

**INP - Other**

*Enviromental Conditions*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Enviromental Temperature (oC) | 25 | Open/editable | Locked default | 
| Solar (W/m²) | 400 | Open/editable | Locked default | 
| Batch-mode | ON | Open/editable | Locked default | 
| Environmental Conditions Database | TBC Default | Open/editable | Locked default | 

*AC-system*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| AC-compressor type | 2-stage (0/100) | Open/editable | Open/OEM data | 
| AC-compressor type (Mechanical / Electrical) | Calculation | Locked Calc | Locked Calc | 
| AC-compressor capacity (kW) | 18 | Open/editable | Locked default | 
| COPCool | 3.50 | Locked Calc | Locked Calc | 

*Ventilation*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Ventilation during heating | Yes | Open/editable | Locked default | 
| Ventilation when both Heating and  AC are inactive | Yes | Open/editable | Locked default | 
| Ventilation during AC | Yes | Open/editable | Locked default | 
| Ventilation flow setting when both Heating and  AC are inactive | High | Open/editable | Locked default*** | 
| Ventilation during Heating | High | Open/editable | Locked default*** | 
| Ventilation during Cooling | High | Open/editable | Locked default*** | 

*Aux. Heater*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Fuel Fired Heater (kW) | 30 | Open/editable | Open/OEM data | 

*TechList Input**

*Insulation*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Double-glazing | TF5 table* | Open/editable | Tick box only | 
| Tinted windows | TF5 table* | Open/editable | Tick box only | 

*Ventilation* 

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Fan controll strategy | TF5 table* | Open/editable | Tick box only | 

*Heating*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Heat pump systems | TF5 table* | Open/editable | Tick box only | 
| Adjustable coolant thermostat | TF5 table* | Open/editable | Tick box only | 
| Adjustable auxiliary heater | TF5 table* | Open/editable | Tick box only | 
| Engine waste-gas heat exchanger | TF5 table* | Open/editable | Tick box only | 

*Cooling*

| Category/Input | Default value | Engineering | Declaration | 
|----------------|---------------|-------------|-------------| 
| Separate air distribution ducts | TF5 table* | Open/editable | Tick box only | 

**Notes: **

 \* Default parameter values for Technology List from ACEA TF5 proposal

![](pics/TechnologyListDefaults.jpg)

** Tinted Window: G-Factor/g-value (= "solar factor" = "total solar energy transmittance")   according ISO 9050. ISO 9050 defines wind speed at the outside surface of 14 km/h.

Definition of bins for transmission rates according to ACEA TF5 recommendation:

| g-value | bonus |
|---------|-------|
| < 0,1 | To be simulated with g = 0,05 |
| 0,11 – 0,20 | To be simulated with g = 0,15 |
| 0,21 – 0,30 | To be simulated with g = 0,25 |
| 0,31 – 0,40 | To be simulated with g = 0,35 |
| 0,41 – 0,50 | To be simulated with g = 0,45 |
| 0,51 – 0,60 | To be simulated with g = 0,55 |
| 0,61 – 0,70 | To be simulated with g = 0,65 |
| 0,71 – 0,80 | To be simulated with g = 0,75 |
| 0,81 – 0,90 | To be simulated with g = 0,85 |
| 0,91 - 1    | To be simulated with g = 0,95 | 

*** Air Flow Rate: recommended for future implementation in Declaration mode by ACEA TF5:

| Phase | With thermal comfort roof mounted system | Without thermal comfort roof mounted system |
|-------|------------------------------------------|---------------------------------------------|
| **Cooling** | High (20x internal volume / h) | Low (7x internal volume / h) | 
| **Ventilation** | High (20x internal volume / h) | Low (7x internal volume / h) | 
| **Heating** | High (10x internal volume / h) | Low (7x internal volume / h) 

### File Format

The HVAC SSM (.ahsm) and Bus Parameter Database (.abdb) files use the VECTO CSV format.

