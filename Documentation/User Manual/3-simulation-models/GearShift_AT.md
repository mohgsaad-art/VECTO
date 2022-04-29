## Gearbox: AT Gearshift Rules

For AT gearboxes neither Skip Gears nor Early upshift (see [Gearbox: Gear Shift Model](#gearbox-gear-shift-model)) are enabled. Moreover, the gears are shifted strictly sequentially:

- 1C -> 1L -> 2L -> ...  (torque converter only in 1st gear)
- 1C -> 2C -> 2L -> ...  (torque converter in 1st and 2nd gear)

### Shift Polygons in Declaration Mode

The shift lines in Declaration Mode only apply for trucks and gearboxes with serial torque converter (AT-S).

* Downshift line: 700 rpm (torque independent, vertical line)
* Upshift line: 900 rpm for torque <= 0; 1150 rpm @ Engine's maximum torque

![](pics/at_gearbox_shiftlines.PNG)

### Upshift rules

+ If engine speed and engine torque in the *next gear* (see shift sequence) is above the upshift line AND
+ the acceleration in the next gear is above a certain threshold if the driver is accelerating, i.e., acceleration_nextGear > min(Min. acceleration threshold, Driver acceleration)

The user interface allows to enter two acceleration thresholds, one for locked gear to locked gear shifts and another vor converter to locked gear shifts. For converter to converter shifts the latter threshold applies.

### Downshift

* If the engine speed falls below the downshift curve

* Drivetrain in "Neutral" when either
   	- velocity < 5 km/h
    - OR during deceleration phase when the torque converter is active and the engine speed would fall below idle speed


### Shift parameters

- Min. time between two consecutive gearshifts.
- Min. acceleration after gearshift for L to L gear shifts
- Min. acceleration after gearhsift for C to L gear shifts
- Min. acceleration after gearshift for C to C geear shifts


