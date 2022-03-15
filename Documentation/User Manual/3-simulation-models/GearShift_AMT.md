##Shift Strategy: AMT Gearshift Rules

This section describes the gearshift rules for automatic manual transmission models. When a gearshift is triggered, gears may be skipped.


The Effshift control algorithm differentiates between the shift rules:

  * emergency shifts, 
  * polygon shifts, and
  * efficiency gear shifts. 

For the EffShift model general shift conditions apply regardless of the shift rule, with exception of emergency shifts, these have always priority. 

The general gearshift conditions for downshifting are:

  * $t_{lastshift} + t_{between shifts} < t_{act}$ 
  * $t_{lastUpshift} + Downshift delay < t_{act}$

The general gearshift conditions for upshifting are:

  * Driver behaviour is accelerating or driving 
  * $t_{lastshift} + t_{between shifts} < t_{act}$ 
  * $t_{lastDownshift} + Upshift delay < t_{act}$

The general shift conditions are checked first in the shift algorithm. The following table lists the generic values for the parameters used in the declaration mode settings of current version of the AMT Effshift model.


| **Parameter**          | **Value** |
|--------------------|-------|
| $t_{between shifts}$ | 2 [s] |
| Downshift delay    | 6 [s] |
| Upshift delay      | 6 [s] |
| Allowed gear range | 2 |
| RatioEarlyDownshift, RatioEarlyUpshift | 24 |
| Rating current gear | 0.97 |
| $T_{reserve}$          |  0 |

###Emergency shifts

Emergency shifts depend on the current gear and the engine speed. The shifting rules for emergency shifts have been adopted from the "Classic" gearshift strategy in VECTO. In case of application of emergency rule no skipping of gears is applied. 

Shift to neutral, if: 

  *  Current gear = 1 and
  *  $n_{eng} < n_{idle}$

Downshift conditions:

   * Current gear > 1 and
   * $n_{eng} < n_{idle}$

Upshift conditions:

   * Current gear < highest gear
   * $n_{eng} < n_{95h}$

###Polygon shifts

The second level of the gearshift algorithm is the polygon shift rule. If the current operating point is outside of the shift polygons, the polygon shift rule applies:

Downshift behaviour: 

  * If the operating point (Teng, neng) is left the downshift line, shift to the next lower gear

Upshift behaviour:

  * If the operating point (Teng, neng) is right to the upshift line, shift to the highest gear which is right to the downshift line and below the full load torque considering similar engine power output.

It should be noted, that there is no skip gears at downshifting in the polygon shift mode.

###Efficiency shifts

The efficiency shift rule is added on top of the polygon shift rule. The EffShift strategy allows gear shifts if the current engine operating point is inbetween the gearshift lines and a certain threshold above the engine's drag curve and the combined fuel efficiency considering engine and gearbox characteristics in the candidate gear is better than in the current gear. Therefore the fuel consumption of the current gear and the gears within an allowed gear shift range (parameter allowed +/- gears) is calculated. For AMT transmissions, the current operating point is used for this efficiency evaluation. Since, the velocity drop due to traction interruption is not relevant for this evaluation as this operating point only occurs for a short period of time. Efficiency shifts are only allowed below a certain gear ratio (gearbox + axle) to prevent frequent gear changes in the very lowest gears. 

$FC_{gear}=min⁡(FC_{gear + i})   \forall i \in \textrm{Allowed gear range}$

Additionally the following boundary conditions must be fulfilled for an efficiency upshift to happen:  

   * $i_{gear + axle} \leq \textrm{RatioEarlyUpshift}$
   * Not left to downshift line
   * $1-P_{eng}(candidate gear) / P_{eng,max}⁡(candidate gear) > T_reserve$      ($T_{reserve}$  is set to 0 for efficiency shifts)
   * $P_{eng,act } \leq P_{eng,post_shift}$    This condition is based on the assumption that sufficient power for the current acceleration is available in the next gear. The check for sufficient power in a candidate gear considers the velocity drop during traction interruption. 
   * $FC_{gear} < FC_{current gear} * \textrm{RatingFactor}$

For an efficiency downshift following conditions are met:

   * $i_{gear + axle} \leq \textrm{RatioEarlyDownshift}$
   * Not right upshift line
   * $1-P_{eng}(next gear) / P_{eng,max} > T_{reserve}$      ($T_{reserve}$  is set to 0 for efficiency shifts)
   * $FC_{gear} < FC_{current gear} * \textrm{RatingFactor}$

