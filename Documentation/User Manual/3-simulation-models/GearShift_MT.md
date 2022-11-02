## Shift Strategy: MT Gearshift Rules

This section describes the gearshift rules for manual transmission models. When a gearshift is triggered, gears may be skipped for (see [Gearbox: Gear Shift Model](#gear-shift-model)). 

### Shift Polygons in Declaration Mode (According to ACEA Whitebook 2016)

#### 1. Computation of Characteristic Points
![](pics/shiftlines_1.PNG)

#### 2. Definition of Shift Lines
![](pics/shiftlines_2.PNG)

#### 3. Exception 1: Margin to Max-Torque line (Downshift)
![](pics/shiftlines_3.PNG)

Note: Line L1 is shiftet parallel so that it satisfies the max-torque margin condition, not intersected.

#### 4. Exception 2: Minimal Distance between Downshift and Upshift Lines
![](pics/shiftlines_4.PNG)

#### 5. Final Gearshift Lines (Example)
![](pics/shiftlines_5.PNG)

If the gearbox defines a maximum input speed for certain gears the upshift line may further be intersected 
and limited to the gear's maximum input speed.

### Upshift rules

* If the engine speed is higher than the gearbox maximum input speed or engine n_{95h} speed (whichever is lower)
* If all of the following conditions are met:
    - The vehicle is not decelerating AND
    - Engine operation point (speed and torque) is above (right of) the upshift line AND
    - The acceleration in the next gear is above a certain threshold if the driver is accelerating, i.e., acceleration_nextGear > min(Min. acceleration   threshold, Driver acceleration) AND
    - The last gearshift was longer than a certain threshold (Declaration Mode: 2s) ago AND
    - The last downshift was longer than a certain threshold (Declaration Mode: 10s) ago


### Downshift

* If the engine speed is lower than the engine's idle speed
* If all of the following conditions are met:
    - Engine operation point (speed and torque) is below (left of) the downshift line AND
    - The last gearshift was longer than a certain threshold (Declaration Mode: 2s) ago AND
    - The last upshift was longer than a certain threshold (Declaration Mode: 10s) ago


### Shift parameters

- Gearshift lines
- Engine idle speed
- Gearbox max. input speed
- Engien n_{95h} speed
- Min. time between two consecutive gearshifts.
- Min. time for upshift after a downshift
- Min. time for downshift after an upshift
- Min. acceleration in next gear


