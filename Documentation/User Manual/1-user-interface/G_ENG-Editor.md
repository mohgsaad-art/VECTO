## Engine Editor

![](pics/ENG-Editor.PNG)

### Description

The [Engine File (.veng)](#engine-file-.veng) defines all engine-related parameters and input files like Fuel Consumption Map and Full Load Curve.

### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths.
Example: "Demo\\FLD1.vfld" points to the "Demo" subdirectory of the Engine File's directory.

VECTO automatically uses relative paths if the input file (e.g. FC Map) is in the same directory as the Engine File. *Note:* The Engine File must be saved before browsing for input files.)

### Main Engine Parameters

Make and Model \[text]\
:   Free text defining the engine model, type, etc.

Idling Engine Speed \[rpm\]
:   Low idle, applied in simulation for vehicle standstill in neutral gear position.

Displacement \[ccm\]
:   Used in [Declaration Mode](#declaration-mode) to calculate inertia.

Fuel Type
:   Used to compute derived results such as fuel consumption in liters and CO2 values. This parameter influences the CO2-to-fuel ratio and fuel density. The actual values can be looked up in [FuelTypes.csv](../Declaration/FuelTypes.csv).

Inertia including Flywheel \[kgm²\]
:   Inertia for rotating parts including engine flywheel. In [Declaration Mode](#declaration-mode) the inertia is calculated  depending on the engine's displacement and also accounts for the clutch's inertia.

### Full Load and Drag Curves


The [Engine's Full Load and Drag Curves (.vfld)](#full-load-and-drag-curves-.vfld) limits the engine's maximum torque and drag torque respectively The full-load curve must at least cover the engine-speed range from idling speed up to the speed where the power goes down to 70% of the maximum power. The input file (.vfld) file format is described [here](#full-load-and-drag-curves-.vfld).

### Fuel Consumption Map


The [Fuel Consumption Map](#fuel-consumption-map-.vmap) is used to calculate the base FC value. See [Fuel Consumption Calculation](#engine-fuel-consumption-calculation) for details.

The input file (.vmap) file format is described [here](#fuel-consumption-map-.vmap).

### WHTC Correction Factors

<div class="declaration">
The WHTC Correction Factors are required in [Declaration Mode](#declaration-mode) for the [WHTC FC Correction](#engine-fuel-consumption-calculation).

The Cold/Hot Emission Balancing Factor is an additional correction factor that is used to correct the fuel consumption.
</div>

<div class="engineering">
In engineering a single correction factor for correcting WHTC, Cold/Hot Balancing, ... can be specified. 
</div>


### Chart Area


The Chart Area shows the fuel consumption map and the selected full load curve.

### Controls


![new](pics/blue-document-icon.png)New file
:   Create a new empty .veng file

![open](pics/Open-icon.png)Open existing file
:   Open an existing .veng file

![save](pics/Actions-document-save-icon.png)***Save current file*** \
   

![SaveAs](pics/Actions-document-save-as-icon.png)***Save file as...*** \
   

![sendto](pics/export-icon.png)Send current file to the [VECTO Editor](#job-editor)
:   **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.

![](pics/browse.png)***Open file browser***.

![](pics/OpenFile.PNG)***Open file*** (see [File Open Command)](#settings).

![OK](pics/OK.png)Save and close file
:   If necessary the file path in the [VECTO Editor](#job-editor) will be updated.

![Cancel](pics/Cancel.png)***Cancel without saving***
