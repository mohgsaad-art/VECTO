## Job Editor


![](pics/VECTO-Editor.png)


### Description

The [job file (.vecto)](#job-file) includes all informations to run a VECTO calculation. It defines the vehicle and the driving cycle(s) to be used for calculation. In summary it defines:

-   Filepath to the [Vehicle File (.vveh)](#vehicle-editor) which defines the not-engine/gearbox-related vehicle parameters
-   Filepath to the [Engine File (.veng)](#engine-editor) which includes full load curve(s) and the fuel consumption map
-   Filepath ot the [Gearbox File (.vgbx)](#gearbox-editor) which defines gear ratios and transmission losses
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
:	Files can be created and edited using the [Vehicle Editor](#vehicle-editor).

Filepath to the Engine File (.veng)
:	Files can be created and edited using the [Engine Editor](#engine-editor).

Filepath ot the Gearbox File(.vgbx)
:	Files can be created and edited using the [Gearbox Editor](#gearbox-editor).

<div class="declaration">
Auxiliaries
:	This group contains input elements to define the vehicle's load from the auxiliaries.
In Declaration Mode only the pre-defined auxiliaries are available and their power-demand is also pre-defined, depending on the vehicle category and driving cycle. This means the Auxiliary Type is set to 'Classic: Vecto Auxiliary' and no 'Constant Aux Load' can be specified.
The following list contains the pre-defined auxiliaries where the concrete technology for each auxiliary can be configured using the [Auxiliary Dialog](#auxiliary-dialog). 
**Double-click** entries to edit with the [Auxiliary Dialog](#auxiliary-dialog).
</div>

<div class="engineering">
Auxiliaries
:	In Engineering Mode the set of auxiliaries can be freely defined.
First, the Auxiliary Type can be selected. If the Bus Auxiliaries are selected a configuration file for the Advanced Auxiliaries has to be specified. When using the Bus Auxiliaries, the standard auxiliaries can  be added as well in the list below to take into account the steering pump, etc.
The 'Constant Aux Load' can be used to define a constant power demand from the auxiliaries (similar to P_add in the driving cycle, but constant over the whole cycle).
The following list can be used to define the auxiliary load in more detail via a separate input file. The auxiliaries are configured using the [Auxiliary Dialog](#auxiliary-dialog). 
 For each auxiliary an [Auxiliary Input File (.vaux)](#auxiliary-input-file-.vaux) must be provided and the [driving cycle](#driving-cycles-.vdri) must include the corresponding supply power.
**Double-click** entries to edit with the [Auxiliary Dialog](#auxiliary-dialog).
: ![addaux](pics/plus-circle-icon.png) Add new Auxiliary
: ![remaux](pics/minus-circle-icon.png) Remove the selected Auxiliary from the list
</div>

See [Auxiliaries](#auxiliaries) for details.

Cycles
:	List of cycles used for calculation. The .vdri format is described [here](#driving-cycles-.vdri).
**Double-click** an entry to open the file (see [File Open Command](#settings)).
**Click** selected items to edit file paths.

: ![addcycle](pics/plus-circle-icon.png) Add cycle (.vdri)
: ![remcycle](pics/minus-circle-icon.png) Remove the selected cycle from the list


### Driver Assist Tab

![](pics/VECTO-Editor-DriverAssist.png)


In this tab the driver assistance functions are enabled and parameterised.

Overspeed
:	See [Overspeed](#overspeed) for details.

Look-Ahead Coasting
:	See [Look-Ahead Coasting](#driver-look-ahead-coasting) for details.

Acceleration Limiting
:	See [Acceleration Limiting](#driver-acceleration-limiting) for details.


### Chart Area

If a valid [Vehicle File](#vehicle-editor), [Engine File](#engine-file-.veng) and [Gearbox File](#gearbox-file-.vgbx) is loaded into the Editor the main vehicle parameters like HDV group and axle configuration are shown here. The plot shows the full load curve(s) and sampling points of the fuel consumption map. 

### Controls

![new](pics/blue-document-icon.png) New Job File
:	Create a new empty .vecto file

![open](pics/Open-icon.png) Open existing Job File
:	Open an existing .vecto file

![save](pics/Actions-document-save-icon.png) ***Save current Job File***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save Job File as...***

![sendto](pics/export-icon.png) Send current file to Job List in [Main Form](#main-form)
:	**Note:** The file will be sent to the Job List automatically when saved.

![veh](pics/Veh.png) ***Open [Vehicle Editor](#vehicle-editor)***

![eng](pics/Eng.png) ***Open [Engine Editor](#engine-editor)***

![gbx](pics/Gbx.png) ***Open [Gearbox Editor](#gearbox-editor)***

![](pics/browse.png) ***Browse for vehicle/engine/gearbox files***

![OK](pics/OK.png) Save and close file
:	File will be added to Job List in the [Main Form](#main-form).

![Cancel](pics/Cancel.png) ***Cancel without saving***
