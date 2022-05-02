
## Torque and Speed Limitations

The engine's maximum speed and maximum torque may be limited by either the gearbox (due to mechanical constraints) or the vehicle control.
Engine torque limitations are modeled by limiting the engine full-load curve to the defined maximum torque, i.e., the original engine full-load curve is cropped at the defined maximum torque for a certain gear. Limits regarding the gearbox' maximum input speed are modeled by intersecting (and limiting) the upshift line with the max. input speed. In the last gear, where no upshifts are possible, the engine speed is limited to the gearbox' maximum input speed.

Gear shift polygons are calculated by VECTO based on the overall (i.e. from gearbox and vehicle control) cropped engine fullload curve.


<div class="engineering">
In Engineering Mode, speed and torque limits can be defined and will be effective for every gear.
</div>

<div class="declaration">
In Declaration Mode, the following rules restrict the limitations of engine torque:

### Transmission Input-Speed Limitations

* Applicable for every gear

### Transmission Torque Limitations

* For higher 50% of gears (i.e., gears 7 to 12 for a 12-gear transmission):
    - Transmissions max torque > 90% of engine max torque: max. torque limitation *not* applicable (VECTO extrapolates loss-maps)
    - Transmissions max torque <= 90% of engine max torque: max. torque limitation applicable
* For lower 50% of gears (i.e., gears 1 to 6 for a 12-gear transmission):
    - Transmission torque limit is always applicable

### Vehicle defined Torque Limitations

* For higher 50% of gears (i.e., gears 7 to 12 for a 12-gear transmission):
    - Torque limit > 95% of engine max torque: max. torque limitation *not* applicable
    - Torque limit <= 95% of engine max torque: max. torque limitation applicable
* For lower 50% of gears (i.e., gears 1 to 6 for a 12-gear transmission):
    - Torque limit is *not* applicable

</div>


