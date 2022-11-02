
## Rechargeable Electric Energy Storage Editor

Two types of rechargeable electric energy storage can be configured in VECTO: either a battery pack or a super capacitor.

### Battery Pack

![](pics/VECTO_Battery.png)

#### Description

The electric energy storage editor allows to edit all model parameters relevant for the electric energy storage.

#### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. 

VECTO automatically uses relative paths if the input file (e.g. SoC) is in the same directory as the Battery file. (The Battery File must be saved before browsing for input files.)


#### Main Parameters

Make and Model
:   Free text defining the model, type, etc.

Capacity \[Ah\]
:   Nominal capacity of the battery

C-Factor \[-\]
:   Factor defining the battery's maximum current (derived from the capacity)

SoC min \[%\]
:   Minimum allowed state of charge

SoC max \[%\]
:   Maximum allowed state of charge

SoC Curve
:   Battery internal voltage depending on the battery's state of charge (see [Battery Internal Voltage File (.vbatv)](#battery-internal-voltage-file-.vbatv))

Internal Resistance Curve
:   Defines the battery's internal resistance depending on its state of charge. The file must cover the SOC range from 0 to 100%! (see [Battery Internal Resistance File (.vbatr)](#battery-internal-resistance-file-.vbatr))


#### Chart Area

The Chart Area displays the battery's internal voltage (blue) and the internal resistance (red) over its state of charge.


### SuperCap

![](pics/VECTO_SuperCap.png)

#### Main Parameters

Make and Model
:   Free text defining the model, type, etc.

Capacitance \[F\]
:   Nominal capacity of the capacitor

Min Voltage \[V\]
:   Minimum allowed state of charge

Max Voltage \[v\]
:   Maximum allowed state of charge

Internal Resistance
:   Defines the capacitor's internal resistance 


### Controls



![](pics/blue-document-icon.png) New file
:   Create a new empty .vbat file

![open](pics/Open-icon.png)Open existing file
:   Open an existing .vbat file


![save](pics/Actions-document-save-icon.png) ***Save current file***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save file as...***

![sendto](pics/export-icon.png) Send current file to the [VECTO Editor](#job-editor)
:   **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.


![](pics/browse.png) ***Open file browser***

![](pics/OpenFile.PNG) ***Open file*** (see [File Open Command)](#settings).

![OK](pics/OK.png) Save and close file
:   If necessary the file path in the [VECTO Editor](#job-editor) will
be updated.


![Cancel](pics/Cancel.png) ***Cancel without saving***

