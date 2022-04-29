## Vehicle Editor

![](pics/VEH-Editor.PNG)

### Description

The [Vehicle File (.vveh)](#vehicle-file-.vveh) defines the main vehicle/chassis parameters like axles including [RRC](#vehicle-rolling-resistance-coefficient)s, air resistance and masses.

The Vehicle Editor contains 3 tabs to edit all vehicle-related parameters. The 'General' tab allows to input mass, loading, air resistance, vehicle axles, etc. The 'Powertrain' allows to define the retarder, an optional angle drive, or PTO consumer. In the third tab the engine torque can be limited to a maximum for individual gears.

### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths.
Example: "Demo\\RT1.vrlm" points to the "Demo" subdirectory of the Vehicle File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Retarder Losses File) is in the same directory as the Vehicle File. (*Note:* The Vehicle File must be saved before browsing for input files.)

### General vehicle parameters

Vehicle Category
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Group.

Axle Configuration
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Group.

Technically Permissible Maximum Laden Mass [t]
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Group.

HDV Group
: Displays the automatically selected HDV Group depending on the settings above.

### Masses/Loading

Corrected Actual Curb Mass Vehicle
: Specifies the vehicle's mass without loading

<div class="engineering">
Curb Mass Extra Trailer/Body
: Specifies additional mass due to superstructures on the vehicle or an additional trailer

Loading
: Speciefies the loading of both, the vehicle and if available the trailer
</div>

**Max. Loading** displays a hint for the maximum possible loading for the selected vehicle depending on curb mass and TPMLM values (without taking into account the loading capacity of an additional trailer).

***Note:*** *VECTO uses the sum of* ***Corrected Actual Curb Mass Vehicle, Curb Mass Extra Trailer/Body*** *and* ***Loading*** *for calculation! The total mass is distributed to all defined axles according to the relative axle load share.*

<div class="declaration">
In Declaration Mode only the vehicle itself needs to be specified. Depending on the vehicle category and mission the simulation adds a standard trailer for certain missions.
</div>

### Air Resistance and Corss Wind Correction Options

The product of Drag Coefficient [-] and Cross Sectional Area [m²] (**c~d~ x A**) and **Air Density** [kg/m³] (see [Settings](#settings)) together with the vehicle speed defines the Air Resistance. Vecto uses the combined value **c~d x A** as input. 
**Note that the Air Drag depends on the chosen [**Cross Wind Correction**](#vehicle-cross-wind-correction).**

<div class="declaration">
If the vehicle has attached a trailer for simulating certain missions the given **c~d~ x A** value is increased by a fixed amount depending on the trailer used for the given vehicle category.
</div>

For cross wind correction four different options are available:
: -  No Correction: The specified CdxA value is used to compute the air drag, no cross-wind correction is applied
-  Speed dependent (User-defined): The specified CdxA value is corrected depending on the vehicle's speed.
-  Speed dependent (Declaration Mode): A uniformly distributed cross-wind is assumed and used for correcting the air-drag depending on the vehicle's speed
-  Vair & Beta Input: Correction mode if the actual wind speed and wind angle relative to the vehicle have been measured.

<div class="declaration">
In delcaration mode the 'Speed dependent (Declaration Mode)' cross-wind correction is used.
</div>

Depending on the chosen mode either a [Speed Dependent Cross Wind Correction Input File (.vcdv)](#speed-dependent-cross-wind-correction-input-file-.vcdv) or a [Vair & Beta Cross Wind Correction Input File (.vcdb)](#speed-dependent-cross-wind-correction-input-file-.vcdv) must be defined. For details see [Cross Wind Correction](#vehicle-cross-wind-correction).

### Dynamic Tyre Radius

In [Engineering Mode](#engineering-mode) this defines the effective (dynamic) wheel radius (in [mm]) used to calculate engine speed. In [Declaration Mode](#declaration-mode) the radius calculated automatically using tyres of the powered axle.

### Vehicle Idling Speed

The idling speed of the combustion engine can be increased in the vehicle settings. This may be necessary due to certain auxiliaries or for other technical reasons. This value is only considered if it is higher than the idling speed defined in the combustion engine.

### Axles/Wheels

For each axle the parameters **Relative axle load, RRC~ISO~** and **F~zISO~** have to be given in order to calculate the total [Rolling Resistance Coefficient](#vehicle-rolling-resistance-coefficient).

<div class="engineering">
In Engineering mode, the **Wheels Inertia [kgm²]** has to be set per wheel for each axle.
The axles, for both  truck and trailer, have to be given.

Use the ![](pics/plus-circle-icon.png) and ![](pics/minus-circle-icon.png) buttons to add or remove axles form the vehicle. 

</div>

<div class="declaration">
In [Declaration mode](#declaration-mode) only the axles of the truck have to be given (e.g., 2 axles for a 4x2 truck). 
The dynamic tyre radius is derived from the second axle as it is assumed this is the driven axle.
For missions with a trailer predefined wheels and load-shares are added by Vecto automatically.
</div>

Doubleclick entries to edit existing axle configurations.


### Controls


![](pics/blue-document-icon.png) New file
: Create a new empty .vveh file

![](pics/Open-icon.png) Open existing file
: Open an existing .vveh file

![](pics/Actions-document-save-icon.png) ***Save current file***

![](pics/Actions-document-save-as-icon.png) ***Save file as...***

![](pics/export-icon.png) Send current file to the [VECTO Editor](#job-editor)
: **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.

![](pics/OK.png) Save and close file
: If necessary the file path in the [VECTO Editor](#job-editor) will be updated.

![](pics/Cancel.png) ***Cancel without saving***



## Vehicle Editor -- Powertrain Tab

![](pics/VehicleForm_Powertrain.png)


### Retarder Losses

If a separate retarder is used in the vehicle a **Retarder Torque Loss Map** can be defined here to consider idling losses caused by the retarder.

The following options are available:
: -   No retarder
-	Included in Transmission Loss Maps: Use this if the [Transmission Loss Maps](#transmission-loss-map-.vtlm) already include retarder losses.
-   Primary Retarder (before gearbox, transmission input retarder): The rpm ratio is relative to the engine speed.
-   Secondary Retarder (after gearbox, transmission output retarder): The rpm ratio is relative to the cardan shaft speed.
-   Engine Retarder: Used this if the engine already includes the retarder losses.
-   Axlegear Input Retarder (after axlegear): The rpm ratio is relative to the axlegear input shaft speed. Only available for battery electric vehicles with E3 motor, serial hybrid with S3 motor, S-IEPC, and E-IEPC.

Primary, secondary and axlegear input retarder require an [Retarder Torque Loss Input File (.vrlm)](#retarder-loss-torque-input-file-.vrlm).
The retarder ratio defines the ratio between the engine speed/cardan shaft speed and the retarder.

### Angledrive

If an angledrive is used in the vehicle, it can be defined here.
Three options are available:

- None (**default**)
- Separate Angledrive: Use this if the angledrive is measured separately. In this case the ratio must be set and the [Transmission Loss Map](#transmission-loss-map-.vtlm) (or an Efficiency value in Engineering mode) must also be given.
- Included in transmission: Use this if the gearbox already includes the transmission losses for the angledrive in the respective transmission loss maps.


## Vehicle Editor -- Torque Limits Tab

![](pics/VehicleForm_TorqueLimits.png)

On this tab different torque limits can be applied at the vehicle level. For details which limits are applicable and who the limits are applied in the simulation [see here](#torque-and-speed-limitations).

First, the maximum torque of the ICE may be limited for certain gears (see [Engine Torque Limitations](#torque-and-speed-limitations)).
In case that the gearbox' maximum torque is lower than the engine's maximum torque or to model certain features like Top-Torque (where in the highest gear more torque is available) it is possible to limit the engine's maximum torque depending on the engaged gear. 


## Vehicle Editor -- ADAS Tab

![](pics/VehicleForm_ADAS.png)

On the ADAS tab, the advanced driver assistant systems present in the vehicle can be selected.  See [ADAS - Engine Stop/Start](#advanced-driver-assistant-systems-engine-stopstart), [ADAS - EcoRoll](#advanced-driver-assistant-systems-eco-roll), and [ADAS - Predictive Cruise Control](#advanced-driver-assistant-systems-predictive-cruise-control)

The following table describes which ADAS technology can be used and is supported for different powertrain architectures (X: supported, O: optional, -: not supported):

| ADAS Technology \ Powertrain Architecture  | Conventional   | HEV   | PEV   |
| ------------------------------------------ | -------------- | ----- | ----- |
| Engine Stop/Start                          | X              | X     | -     |
| EcoRoll Without Engine Stop                | X              | -     | -     |
| EcoRoll with Engine Stop                   | X              | -     | -     |
| Predictive Cruise Control                  | X              | X     | X     |
| APT Gearbox EcoRoll Release Lockup Clutch  | O              | -     | -     |

* Engine Stop/Start not allowed for PEV
* EcoRoll for HEV always with ICE off
* For PEV no clutch for disconnecting the EM present, thus no EcoRoll foreseen (very low drag of EM in any case)
* Inputs for EcoRoll possible in GUI, but no effect in simulation


## Vehicle Editor -- PTO Tab

![](pics/Vehicleform_PTO.png)

### PTO Transmission

If the vehicle has an PTO consumer, a pto transmission and consumer can be defined here. (Only in [Engineering Mode](#engineering-mode))

Three settings can be set:

- PTO Transmission: Here a transmission type can be chosen (adds constant load at all times).
- PTO Consumer Loss Map (.vptol): Here the [PTO Idle Loss Map](#pto-idle-consumption-map-.vptoi) of the pto consumer can be defined (adds power demand when the pto cycle is not active).
- PTO Cycle (.vptoc): Defines the [PTO Cycle](#pto-cycle-.vptoc) which is used when the pto-cycle is activated (when the PTO-Flag in the driving cycle is set).

<div class="engineering">
In engineering mode additional PTO activations are available to simulate different types of municipal vehicles. It is possible to add a certain PTO load during driving while the engine speed and gear is fixed (to simulate for example roadsweepers), or to add PTO activation while driving (to simulate side loader refuse trucks for example). In both cases the PTO activation is indicated in the [driving cycle](#driving-cycles-.vdri) (column "PTO").


### Roadsweeper operation

PTO activation mode 2 simulates PTO activation while driving at a fixed engine speed and gear. The minimum engine speed and working gear is entered in the PTO tab. For details see [PTO](#pto).


### Sideloader operation

PTO activation mode 3 simulates a time-based PTO activation while driving. Therefore, a separate PTO cycle ([.vptor]()) containing the PTO power over time has to be provided. The start of PTO activation is indicated with a '3' in the 'PTO' column of the [driving cycle](#driving-cycles-.vdri). For details see [PTO](#pto).

</div>