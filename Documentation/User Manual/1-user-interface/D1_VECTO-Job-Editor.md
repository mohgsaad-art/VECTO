## Job Editor


![](pics/VECTO_JobEditor_ParHyb_General.png)


### Description

The [job file (.vecto)](#job-file) includes all informations to run a VECTO calculation. It defines the vehicle and the driving cycle(s) to be used for calculation. In summary it defines:

-   Filepath to the [Vehicle File (.vveh)](#vehicle-editor-general-tab) which defines the not-engine/gearbox-related vehicle parameters
-   Filepath to the [Engine File (.veng)](#engine-editor) which includes full load curve(s) and the fuel consumption map
-   Filepath to the [Gearbox File (.vgbx)](#gearbox-editor) which defines gear ratios and transmission losses
-   Filepath to the [Gearshift Parameters File (.vtcu)](#gearshift-parameters-file-.vtcu) which allows to override parameters of the [Effshift Gearshift Strategy](#gear-shift-model). The gearshift parameters cannot be edited via the graphical user interface. In case the default parameters shall be used either an empty .vtcu file ([see .vtcu](#gearshift-parameters-file-.vtcu)) or the gearbox file (.vgbx) can be provided. An example .vtcu file is provided [here](#gearshift-parameters-file-.vtcu)
-   Auxiliaries
-   Driver Assist parameters
-   Driving Cycles (only in Engineering Mode)


### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. Example: "Vehicles\\Vehicle1.vveh" points to the "Vehicles" subdirectory of the Job File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Vehicle File) is in the same directory as the Job File. (*Note:* The Job File must be saved before browsing for input files.)


### General Settings

![](pics/checkbox.png) Engine Only Mode

:	Enables [Engine Only Mode](#engine-only-mode) (Engineering mode only). The following parameters are needed for this mode:

-   Filepath to the [Engine File (.veng)](#engine-editor)
-   [Driving Cycles](#driving-cycles-.vdri) including engine torque (or power) and engine speed


Filepath to the Vehicle File (.vveh)
:	Files can be created and edited using the [Vehicle Editor](#vehicle-editor-general-tab).

Filepath to the Engine File (.veng)
:	Files can be created and edited using the [Engine Editor](#engine-editor).

Filepath ot the Gearbox File(.vgbx)
:	Files can be created and edited using the [Gearbox Editor](#gearbox-editor).

Filepath ot the Shift Parameters File(.vtcu)

Filepath ot the Hybrid Strategy Parameters File(.vhctl)
:	Files can be created and edited using the [Hybrid Strategy Parameters Editor](#hybrid-strategy-parameters-editor).


### Auxiliaries Tab

![](pics/VECTO_JobEditor_ParHyb_Aux.png)

<div class="declaration">
Auxiliaries
:	This group contains input elements to define the engine's load from the auxiliaries.
In Declaration Mode only the pre-defined auxiliaries are available and their power-demand is also pre-defined, depending on the vehicle category and driving cycle. 
The list contains the pre-defined auxiliaries where the concrete technology for each auxiliary can be configured using the [Auxiliary Dialog](#auxiliary-dialog). 
**Double-click** entries to edit with the [Auxiliary Dialog](#auxiliary-dialog). No other types of auxiliaries can be used in declaration mode.
</div>

<div class="engineering">
Auxiliaries
:	In Engineering Mode the auxiliary power demand can be defined in three ways. 

The first option is to define the power demand directly in the driving cycle in the column "Padd" (see [Driving Cycles](#driving-cycles-.vdri). This allows to vary the auxiliary load over distance (or time, for time-based driving cycles).

The second option is to define a constant power demand over the whole cycle. The auxiliary power demand can be specified depending on whether the combustion engine is on or off and the vehicle is driving. The auxiliary power demand during engine-off phase is corrected in the [post-processing](#engine-fuel-consumption-correction).

The third option is to use the bus-auxiliaries model. For details see the [Bus Auxiliaries model](#bus-auxiliaries).
</div>


See [Auxiliaries](#auxiliaries) for details.


### Cycles Tab

![](pics/VECTO_JobEditor_Cycles.png)

Cycles
:	List of cycles used for calculation. The .vdri format is described [here](#driving-cycles-.vdri).

<div class="declaration">
In Declaration Mode, the cycles to be simulated depend on the vehicle group. The cycles are listed in this window for reference.
</div>

<div class="engineering">
In Engineering Mode the cycles can be freely selected. All declaration cycles are provided in the Folder "Mission Profiles" and can be used or a custom cycle can be created and used.
</div>

**Double-click** an entry to open the file (see [File Open Command](#settings)).

**Click** selected items to edit file paths.
: ![addcycle](pics/plus-circle-icon.png) Add cycle (.vdri)
: ![remcycle](pics/minus-circle-icon.png) Remove the selected cycle from the list


### Driver Assist Tab

![](pics/JobForm_DriverModel.png)


In this tab the driver assistance functions are enabled and parameterised. The parameters for overspeed, look-ahead coasting and driver acceleration can only be modified in Engineering Mode.

Overspeed
:	See [Overspeed](#driver-overspeed) for details.

Look-Ahead Coasting
:	See [Look-Ahead Coasting](#driver-look-ahead-coasting) for details.

Acceleration Limiting
:	See [Acceleration Limiting](#driver-acceleration-limiting) for details.


### ADAS Parameters

![](pics/JobForm_ADASParams.png)

In this tab certain general parameters for the advanced driver assistant system model can be set. Which ADAS feature is available can be selected in the vehicle itself, in Engineering Mode parameters like minimum activation speed, activation delay, or allowed overspeed can be adjusted. In Declaration Mode all parameters are fixed.

For details on the individual parameters see the corresponding section [Engine Stop/Start](#advanced-driver-assistant-systems-engine-stopstart), [Eco-Roll](#advanced-driver-assistant-systems-eco-roll), [Predictive Cruise Control](#advanced-driver-assistant-systems-predictive-cruise-control)

### Chart Area

The chart area on the right shows the main vehicle parameters like HDV group and axle configuration if a valid [Vehicle File](#vehicle-editor-general-tab), [Engine File](#engine-file-.veng) and [Gearbox File](#gearbox-file-.vgbx) is loaded into the Editor. The plot shows the full load curve(s) and sampling points of the fuel consumption map. 

### Controls

![new](pics/blue-document-icon.png) New Job File
:	Create a new empty .vecto file

![open](pics/Open-icon.png) Open existing Job File
:	Open an existing .vecto file

![save](pics/Actions-document-save-icon.png) ***Save current Job File***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save Job File as...***

![sendto](pics/export-icon.png) Send current file to Job List in [Main Form](#main-form)
:	**Note:** The file will be sent to the Job List automatically when saved.

![veh](pics/Veh.png) ***Open [Vehicle Editor](#vehicle-editor-general-tab)***

![eng](pics/Eng.png) ***Open [Engine Editor](#engine-editor)***

![gbx](pics/Gbx.png) ***Open [Gearbox Editor](#gearbox-editor)***

![](pics/browse.png) ***Browse for vehicle/engine/gearbox files***

![OK](pics/OK.png) Save and close file
:	File will be added to Job List in the [Main Form](#main-form).

![Cancel](pics/Cancel.png) ***Cancel without saving***
