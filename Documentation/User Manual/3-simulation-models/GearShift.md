## Gearbox: Gear Shift Model


The Gear Shift Model is based on shift curves that define the engine speed for up- and down- shifting as a function of engine torque. As soon as the engine operation point passes one of the shift curves a gear change is initiated.


![](pics/Shift.svg)

*Example shift polygons*

- **Down-shift:**
   		
   	- As soon as the current engine speed falls below the down-shift cuve a down-shift is initiated

-  **Up-shift:**
  	 	
  	- MT and AMT transmissions: Up-shift is initiated when the current engine speed is above the up-shift curve
   	- AT transmissions: Up-shift is initiated when the next-gear engine speed is above the up-shift curve



The shift polygons are saved in the [Shift Polygons Input File (.vgbs)](#shift-polygons-input-file-.vgbs) and have to be added to the [Gearbox File](#gearbox-file-.vgbx) when not in [Declaration Mode](#declaration-mode).

In [Declaration Mode](#declaration-mode) the generic shift polygons are computed from the engine's full-load curve. If the maximum torque is limited by the gearbox, the minimum of the current gear and engine maximum torque will be used to compute the [generic shift polygons](#gearbox-gear-shift-model). Note: the computation of the shift polygons uses characteristic values from the engine such as n~95h~, n~pref~, etc. which are also derived from the full-load curve.


In the Gearbox File two additional parameters are defined:

-   **Torque Reserve** \[%\] - Required for the "**Early Upshift**" and "**Skip Gears**" options, see below.
-   **Minimum shift time** \[s\] - Limits the time between two gear shifts in whole seconds. This rule will be ignored if rpms are too high or too low. Note that high values may cause high rpms during acceleration.


### Gear Skipping


Gear Skipping  is active for AMT and MT. Whenever a gear change is initiated (by crossing the up- or down-shift line) VECTO may skip one or several gears as long as the required torque reserve is provided.

![](pics/GBX-Editor-shift3.svg)


### Early Upshift
Early Upshift (allow upshifts inside the shift polygons) is enabled for AMT only. If the next higher gear provides the required torque reserve and it's rpm is still above down-shift-rpm VECTO will shift up.


![](pics/GBX-Editor-shift2.svg)



