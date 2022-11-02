## Full Load and Drag Curves (.vfld)

This file contains the full load and drag curves and the PT1 values for the [transient full load calculation](#engine-transient-full-load). The file uses the [VECTO CSV format](#csv).

- Filetype: .vfld
- Header: **engine speed [1/min], full load torque [Nm], motoring torque [Nm], PT1 [s]**
    + **engine speed [1/min]**: the engine speed in rpm.
    + **full load torque [Nm]**: the maximum possible full load for the engine speed.
    + **motoring torque [Nm]**: the minimum possible drag load in motoring for the engine speed.
    + **PT1 [s]**: the PT1 constant for the transient full load calculation.
- Requires at least 2 data entries

**Note:** The PT1 column is not required in Declaration Mode! Pre-defined values are used.

**Example:**

~~~
engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]
560                 ,1180                 ,-149                ,0.6
600                 ,1282                 ,-148                ,0.6
800                 ,1791                 ,-149                ,0.6
...
~~~


