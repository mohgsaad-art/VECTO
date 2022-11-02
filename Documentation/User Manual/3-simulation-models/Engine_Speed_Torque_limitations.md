
## Torque and Speed Limitations

The torque and speeds in the powertrain can be limited by different components such as the gearbox, the electric motor or the combustion engine, depending on the powertrain configuration.

Some additional limits can be defined in the vehicle configuration as described below.

### Combustion engine limitations / Transmission Limiations

The engine's maximum speed and maximum torque may be limited by either the gearbox (due to mechanical constraints) or the vehicle control.
Engine torque limitations are modeled by limiting the engine full-load curve to the defined maximum torque, i.e., the original engine full-load curve is cropped at the defined maximum torque for a certain gear. Limits regarding the gearbox' maximum input speed are modeled by intersecting (and limiting) the upshift line with the max. input speed. In the last gear, where no upshifts are possible, the engine speed is limited to the gearbox' maximum input speed.

Gear shift polygons are calculated by VECTO based on the overall (i.e. from gearbox and vehicle control) cropped engine fullload curve.


<div class="engineering">
In Engineering Mode, speed and torque limits can be defined and will be effective for every gear.
</div>

<div class="declaration">
In Declaration Mode, the following rules restrict the limitations of engine torque:

#### Transmission Input-Speed Limitations

* Applicable for every gear

#### Transmission Torque Limitations

* For higher 50% of gears (i.e., gears 7 to 12 for a 12-gear transmission):
    - Transmissions max torque > 90% of engine max torque: max. torque limitation *not* applicable (VECTO extrapolates loss-maps)
    - Transmissions max torque <= 90% of engine max torque: max. torque limitation applicable
* For lower 50% of gears (i.e., gears 1 to 6 for a 12-gear transmission):
    - Transmission torque limit is always applicable

#### Vehicle defined Torque Limitations

* For higher 50% of gears (i.e., gears 7 to 12 for a 12-gear transmission):
    - Torque limit > 95% of engine max torque: max. torque limitation *not* applicable (VECTO extrapolates loss-maps)
    - Torque limit <= 90% of engine max torque: max. torque limitation applicable
* For lower 50% of gears (i.e., gears 1 to 6 for a 12-gear transmission):
    - Torque limit is *not* applicable

</div>


### Electric Motor Limiations

The electric motor's maximum drive and maximum recuperation curve can be overridden in the vehicle. Therefore, the same map for maximum drive and maximum recuperation needs to be provided. Such a limit directly overrides the eletric motors model parameters.

### Vehicle Propulsion Limitations

For hybrid electric vehicles the electric machine may provide additional torque to the powertrain and thus cause higher accelerations than a conventional vehicle. To limit such boosting by the electric motor. 

The input is the additional torque the electric motor is allowed to boost in addition to the ICE over ICE speed.

*Note:* this boosting torque has to be provided from 0 rpm up to the max. ICE speed. The angular speed refers to the gearbox input shaft.


#### Example 1: No boosting

![](pics/TorqueLimitWithoutBoosting.png)

The blue curve shows the ICE's full-load curve and the gray line represents the electric motors max drive torque. 

If the electric motor shall not be allowed to provide additional torque beyond the ICE's full-load curve the input for the boosting limitation looks as follows:

~~~
n [rpm] , T_drive [Nm]
0       , 0
2500    , 0
~~~

For speeds below idle speed the full-load torque available from the ICE equals the ICE full-load torque at engine idling speed due to the modeling of the clutch behavior during vehicle starts.

#### Example 2: 

![](pics/TorqueLimitWithBoosting.png)

In this example the electric motor is allowed to provide torque in addition to the combustion engine (in this example 100Nm). The boosting limitation for this example looks as follows:

~~~
n [rpm] , T_drive [Nm]
0       , 100
2500    , 100
~~~

For speeds above approx. 1700 rpm, the propulsion torque limit is limited by the electric motor's max drive curve as the electric motor cannot provide the allowed 100 Nm at this high angular speed.
