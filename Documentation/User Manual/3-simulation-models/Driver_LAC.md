## Driver: Look-Ahead Coasting

Look-Ahead Coasting is a function that aims on modelling real driver behaviour. It is a forward-looking function that detects forthcoming reductions in target speed in the mission profile (e.g. speed limit, etc.) and induces an early deceleration using engine braking before applying mechanical brakes according to the [deceleration limit](#driver-acceleration-limiting).

 ![](pics/LookAheadCoasting.svg)

At the resulting deceleration start point the model calculates the
coasting trajectory until it meets the brake deceleration trajectory. The resulting deceleration consists of a coasting phase followed by combined mechanical/engine braking. If Look-Ahead Coasting is disabled only the braking phase according to the [deceleration limit](#driver-acceleration-limiting) will be applied.

Since Vecto 3.0.4 the coasting strategy according to the ACEA White Book 2016 is implemented. 

The look ahead coasting functionality represents the driver behavior prior to a deceleration event. Due to information of the route ahead the driver is able to anticipate on the deceleration event by releasing the accelerator pedal.

This pedal release decision is based on an estimation of kinetical and potential (height) energy gain versus the expected dissipated energy tue to vehicle resistances during the route section ahead.

For an upcoming target speed change the energy level after the speed change is compared to the vehicle's current energy level (kinetic and potential energy). The difference of those energy levels is used to estimate the average deceleration force to reach the next target speed. Coasting starts if the vehicle's (estimated) average resistance force during coasting multiplied by a speed dependent 'Decision Factor' becomes smaller than the average deceleration force. (For details on the equations please see the ACEA White Book 2016, Section 8)

The *Decision Factor (DF)* depends on the next target speed and the speed change:

$DF_{Coasting} = 2.5 - 1.5 * DF_{vel} * DF_{vdrop}$

whereas $DF_{vel}$ and $DF_{vdrop}$ are speed dependent and speed change dependent lookup curves, giving a value from 0 and 1.

For the look ahead coasting target speed changes within the preview distance are considered. 

$preview distance [m] = 10 * vehicle speed [km/h]$

Parameters in [Job File](#job-file):
: - **PreviewDistanceFactor**
- **DF_offset**: offset in the equation for DF~coasting~ (default 2.5)
- **DF_scaling**: factor in the equation for DF~coasting~ (default 1.5)
- **DF_targetSpeedLookup**: csv file for DF~vel~ lookup (see below)
- **Df_velocityDropLookup**: csv file for DF~vdrop~ lookup (see below)

In engineering mode the parameters can be freely chosen while in declaration mode the default values are used.

![](pics/Vecto-UI_LAC.svg)

#### Decision Factor for target velocity lookup (DF~vel~)

![](pics/Vecto_LAC-DF.png)

Example (default values):

~~~
v_target [km/h], decision_factor [-]
0              , 0
48             , 0
52             , 1
100            , 1
~~~

#### Decision Factor for velocity drop lookup (DF~vdrop~)

Example (default values):

~~~
v_drop [km/h], decision_factor [-]
-100          , 1
9             , 1
11            , 0
100           , 0
~~~


