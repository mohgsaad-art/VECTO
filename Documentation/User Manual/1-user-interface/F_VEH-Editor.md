## Vehicle Editor -- General Tab

![](pics/VEH-Editor.PNG)

### Description

The [Vehicle File (.vveh)](#vehicle-file-.vveh) defines the main vehicle/chassis parameters like axles including [RRC](#vehicle-rolling-resistance-coefficient)s, air resistance and masses.

The Vehicle Editor contains up to 6 tabs, depending on the powertrain architecture and simulation mode, to edit all vehicle-related parameters. The 'General' tab allows to input mass, loading, air resistance, vehicle axles, etc. The 'Powertrain' tab allows to define the retarder, an optional angle drive. The third tab is dedicated to all electric components in case of hybrid electric and battery electric vehicles. In the fourth tab the torque limitations for the combustion engine, the electric motor and the whole vehicle can be specified. The fifth tab allows to enable or disable certain advanced driver assistant systems to be considered in the vehicle. The last tab is dedicated to PTOs, either as a basic component or to simulate municipal vehicles such as refuse trucks or road sweepers with dedicated PTO activation either during driving or during standstill. 

### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths.
Example: "Demo\\RT1.vrlm" points to the "Demo" subdirectory of the Vehicle File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Retarder Losses File) is in the same directory as the Vehicle File. (*Note:* The Vehicle File must be saved before browsing for input files.)

### General vehicle parameters

Vehicle Category
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Group.

Axle Configuration
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Group.

Technically Permissible Maximum Laden Mass [t] (TPMLM)
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

For cross wind correction four different options are available (see [Cross Wind Correction](#vehicle-cross-wind-correction) for details):
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
For missions with a trailer, predefined wheels and load-shares are added by Vecto automatically.
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

### Vehicle Idling Speed

The idling speed of the combustion engine can be increased in the vehicle settings. This may be necessary due to certain auxiliaries or for other technical reasons. This value is only considered if it is higher than the idling speed defined in the combustion engine.

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



## Vehicle Editor -- Electric Machine Tab

![](pics/VehicleForm_ElectricMachine.png)

For hybrid vehicles and battery electric vehicles the input elements on the *electric machine* tab is enabled. Here the component file for the eletric motor can be loaded or created (see [Electric Motor Editor](#electric-motor-editor))

The position where the electric machine is located in the powertrain can be selected. It is possible that the electric machine is connected to the powertrain via a fixed gear ratio.
At the moment electric machines are supported to be present at a single position only. It is not possible to have an electric motor at position P2 and another at position P4!
However, it is possible that more than one electric machine is used at a certain position. 

The *Loss map EM ADC* can be used to consider the losses of a transmission step between drivetrain and electric machine or to consider losses of a summation gear. The loss map has the same format as for all other transmission components (see [Transmission Loss Map (.vtlm)](#transmission-loss-map-.vtlm)). For simplicity or if no such transmission step is used it is possible to enter the efficiency directly (i.e., "1" if no transmission step is used).

In case of a P2.5 configuration (the electric motor is connected to an internal shaft of the tranmission) the transmission ratio for every single gear of the transmission has to be specified in the list to the right of the electric motor parameters. The ratio is defeined as $n_\textrm{GBX,in} / n_\textrm{EM}$ in case of EM without additional ADC or $n_\textrm{GBX,in} / n_\textrm{ADC,out}$ in case of EM with additional ADC.



## Vehicle Editor -- REESS Tab

![](pics/VehicleForm_REESS.png)

For hybrid vehicles and battery electric vehicles the input elements on the *rechargeable electric energy storage system (REESS)* tab is enabled. Here the component file for the battery pack can be loaded or created (see [Electric Energy Storage Editor](#rechargeable-electric-energy-storage-editor))

For the electric energy storage multiple battery packs can be configured either in series or in parallel and the initial state of charge of the whole battery system can be defined.  For every entry of a battery pack the number of packs (count) in series and a stream identifier need to be specified. Battery packs on the same stream are connected in series (e.g., two different battery packs on stream number 1 are in series) while all streams are then connected in parallel (see [Battery Model](#ress) for details). This is only supported for batteries and **not** for SuperCaps.

**Double-click** an entry to edit.

**Click** selected item.
: ![addcycle](pics/plus-circle-icon.png) Add REESS (.vbat)
: ![remcycle](pics/minus-circle-icon.png) Remove the selected REESS from the list

In the REESS Dialog the battery file itself and how it is connected to the electric system (i.e, the stream identifier and number of packs used) can be modified.

![](pics/BatteryPackDialog.png)

## Vehicle Editor -- IEPC Tab

![](pics/VehicleForm_IEPC.png)

For battery electric vehicles of type IEPC-E or hybrid vehicles of type IEPC-S, the tab for the *integrated electric powertrain component (IEPC)* is visible. Here the component file for the IEPC can be loaded or created (see [IEPC Editor](#iepc-editor)).

## Vehicle Editor -- IHPC Tab

![](pics/VehicleForm_IHPC.png)

For hybrid vehicles if type IHPC the input tab for *integrated hybrid powertrain component (IHPC)* is visible. Here the component file for the electric machine part of the IHPC can be loaded or created (see [IHPC Editor](#ihpc-editor))

## Vehicle Editor -- GenSet Tab

![](pics/VehicleForm_GenSet.png)

For serial hybrid vehicles the *GenSet tab* is visible. On this tab the electric machine used as generator connected to the combustion engine can be specified (see [Electric Motor Editor](#electric-motor-editor)). It is possible to have multiple equal electric machines as generator. The electric machines can be connected via a transmission to the combustion engine.

The *Loss map EM ADC* can be used to consider the losses of a transmission step between drivetrain and electric machine or to consider losses of a summation gear. The loss map has the same format as for all other transmission components (see [Transmission Loss Map (.vtlm)](#transmission-loss-map-.vtlm)). For simplicity or if no such transmission step is used it is possible to enter the efficiency directly (i.e., "1" if no transmission step is used).

## Vehicle Editor -- Torque Limits Tab

![](pics/VehicleForm_TorqueLimits.png)

On this tab different torque limits can be applied at the vehicle level. For details which limits are applicable and who the limits are applied in the simulation [see here](#torque-and-speed-limitations).

First, the maximum torque of the ICE may be limited for certain gears (see [Engine Torque Limitations](#torque-and-speed-limitations)).
In case that the gearbox' maximum torque is lower than the engine's maximum torque or to model certain features like Top-Torque (where in the highest gear more torque is available) it is possible to limit the engine's maximum torque depending on the engaged gear. 

Next, the maximum available torque for the electric machine can be reduced at the vehicle level, both for propulsion and recuperation. The input file is the same as the maximum drive and maximum recuperation curve (see [Electric Motor Max Torque File](#electric-motor-max-torque-file-.vemp))

Last, the overall propulsion of the vehicle (i.e., HEV Px, electric motor plus combustion engine) can be limited. The "Propulsion Torque Limit" curve limits the maximum effective torque at the gearbox input shaft over the input speed. This curve is added to the combustion engine's maximum torque curve (only positive values are allowed!). For details on the file format see [Vehicle Boosting Limits](#vehicle-boosting-limits-.vtqp). The propulsion torque limit has to be provided from 0 rpm to the maximum speed of the combustion engine. In case of P3 or P4 configuration, the torque at the gearbox input shaft is calculated assuming that the electric motor does not contribute to propelling the vehicle, considering the increased losses in the transmission components inbetween. For P2.5 powertrain configurations no special calculations are necessary as this architecture is internally anyhow modelled as P2 architecture.

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

