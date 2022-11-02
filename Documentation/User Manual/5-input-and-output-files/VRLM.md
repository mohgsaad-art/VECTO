## Retarder Loss Torque Input File (.vrlm)
This file is used to define retarder idling losses. It can be used for primary and secondary retarders and must be set in the [Vehicle File](#vehicle-editor-powertrain-tab). The file uses the [VECTO CSV format](#csv).

- Filetype: .vrlm
- Header: **Retarder Speed \[1/min], Torque Loss \[Nm]**
- Requires at least 2 data entries

**Example:**

~~~
Retarder Speed [1/min],Torque Loss [Nm]
0                     ,10
100                   ,10.02
200                   ,10.08
300                   ,10.18
...
~~~

