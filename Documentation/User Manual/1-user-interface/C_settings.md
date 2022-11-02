## Settings


![](pics/Settings.PNG)


### Description

In the Settings dialog controls general application settings. The settings are saved in the [settings.json](#application-files) file.

### Interface Settings

File Open Command
:	This command will be used to open CSV Input Files like Driving Cycles (.vdri). See: [Run command![](pics/external-icon%2012x12.png)](http://en.wikipedia.org/wiki/Run_command)\
**Name**: Name of the command as it will be shown in the menu when clicking the ![](pics/OpenFile.PNG) button.\
**Command**: The actual command.

:	***Example*** *: If the command is* ***excel*** *and the file is* ***C:\\VECTO\\cycle1.vdri*** *then VECTO will run:* ***excel "C:\\VECTO\\cycle1.vdri"***


### Calculation Settings

<div class="engineering">
Air Density \[kg/m³\]
:	The Air Density is needed to calculate the air resistance together with the **Drag Coefficient** and the **Cross Sectional Area** (see [Vehicle Editor](#vehicle-editor-general-tab)).

This  setting is only used in Engineering mode. In Declaration mode the default value of 1.188 \[kg/m³\] is used.
</div>

### Controls


Reset All Settings
:	All values in the Settings dialog and Options Tab of the [Main Form](#main-form) will be restored to default values.

![](pics/OK.png) ***Save and close dialog***

![](pics/Cancel.png) ***Close without saving***
