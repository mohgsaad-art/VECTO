## Pneumatic Actuations Map (.apac)
 
This file contains data on number of different kinds of pneumatic actuations on different duty cycles. 

**Important notes**
Note that the cycle file name used should ideally respect this syntax to be correctly associated with the actuation map (.apac), otherwise the number of actuations will be set at 0 by default:

-   "AnyOtherText _X_Bus.vdri", with "X" = "Urban", "Heavy urban", "Suburban", or "Interurban"
-   "AnyOtherText_Coach.vdri"
 
Some flexibility in syntax is allowable (the model looks for ‘Bus’, ‘Coach’, ‘Urban’, etc. in the file name), meaning that the standard default cycles are fully/correctly supported. However, for newly created cycles (i.e. for use in Engineering Mode) it is recommended to follow the above convention to guarantee correct functionality.

This file contains also the estimated time required for a cycle which is required to estimate the air demand for certain actuations.

### File Format

The file uses the VECTO CSV format, with an example provided below, with the default values based on the methodology agreed with the European Commission and the project Steering Group.

### Format

**Default Configuration for Pneumatic Actuations Map:**

~~~
ConsumerName, CycleName, Actuations
Brakes, Heavy Urban, 191
Brakes, Urban, 153
Brakes, Suburban, 49
Brakes, Interurban, 190
Brakes, Coach, 27
Brakes, UnknownCycleName, 0
Park brake + 2 doors, Heavy Urban, 82
Park brake + 2 doors, Urban, 75
Park brake + 2 doors, Suburban, 25
Park brake + 2 doors, Interurban, 9
Park brake + 2 doors, Coach, 6
Park brake + 2 doors, UnknownCycleName, 0
Kneeling, Heavy Urban, 27
Kneeling, Urban, 25
Kneeling, Suburban, 6
Kneeling, Interurban, 0
Kneeling, Coach, 0
Kneeling, UnknownCycleName, 0,
CycleTime,Heavy urban,8912
CycleTime,Urban,8149
CycleTime,Suburban,3283
CycleTime,Interurban,12962
CycleTime,Coach,15086
CycleTime,UnknownCycleName,3600
~~~

