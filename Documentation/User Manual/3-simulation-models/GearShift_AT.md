## Shift Strategy: APT Gearshift Rules

For APT gearboxes gear skipping is only allowed for transmissions with more than 6 gears. Otherwise, the gears are shifted strictly sequentially:

- 1C -> 1L -> 2L -> ...  (torque converter only in 1st gear)
- 1C -> 2C -> 2L -> ...  (torque converter in 1st and 2nd gear)

The model structure for shifting between "locked" gears for APT does not differ from the AMT algorithm. That means that the shift logic also differentiates between emergency shifts, polygon gearshifts and efficiency shifts which are processed in the same sequence.

In addition rules for shifting from torque converter (TC) to locked gears apply. These rules are described below. First step in the algorithm is the check of general conditions. 

General gearshift conditions for downshifting:

   * $t_{lastshift} + t_{between shifts} < t_{act}$ 

General gearshift conditions for the upshift in a locked gear (1C -> 1L, 2C ->2L, L ->L):

   * $t_{lastshift} + t_{between shifts} < t_{act}$ 


Parameters used in the APT Effshift model:

| **Parameter** |  **Value** |
|-----------|--------|
| t_(between shifts) | 1.8 [s] |
| Downshift delay  | 6 [s] |
| Upshift delay    | 6 [s] |
| Allowed gear range (skip of gears)  | Total number of mechanical gears ≤ 6:  1, else 2 |
| CCMinAcceleration  | 0.1 [m/s²] |
| CLMinAcceleration  | 0.1 [m/s²] |
| UpshiftMinAcceleration |  0.1 [m/s²] |
| RatioEarlyDownshift  | 24 |
| RatioEarlyUpshift  | 24 |
| Rating current gear  | 0.97 |
| T_reserve  | 0 |

For triggering gear shifts between gears "1C" and "2C" (if applicable for a certain transmission) the same function as in the VECTO Classic model is applied.

Upshift between TC gears (1C -> 2C):

   * $n_{eng} > min(700, (n_{80h} - 150) * i_{nextGear} / i_{currentGear}$ 
   * $T_{eng} < T_{max,stat} - T_{eng,inertia}
   * $a_{estimated} > min(\textrm{CCMinAcceleration},\textrm{DriverAcceleration})$

With: 

   * $a_{estimated} = P_{acc}  / (v_{act} * (m_{veh} + m_{red.wheels}))$

and

   * $P_{acc} = P_{eng,max} - P_{Gb_loss} - P_{Axle_loss} - P_{Air drag_loss} - P_{RR} - P_{slope}$


### Emergency shifts

The Emergency shift strategy for APT transmission looks as follows.

Downshift:

  * $n_{eng} < n_{idle}$

Upshift (all conditions are met):

  * $n_{eng} > min(n_{max,gear}, n_{95h})$
  * gear < maxGear
  * $a_{estimated} > 0$
  * TC = locked
  * Gear + 1 is above downshift line 

### Polygon shifts

The Polygon shift rule for APT works on the same principle as for AMT. But, as already mentioned above the calculation of the upshift line is based on the post-shift engine speed. If the general requirements are fulfilled and it is not an emergency shift, the algorithm of the EffShift model uses the polygon shift rule. In this regard, two different cases related to a downshift are distinguished.

Conditions for downshift case 1:

   * Operation point (Teng, neng) before downshift is left to downshift line.

Conditions for downshift case 2 (all conditions have to be met):

  * DriverAction = Accelerating
  * $a_{act} < 0$
  * $v_{veh} < v_{target} - 10km/h$
  * Locked gear
  * DeltaFullLoad(gear – 1) < DeltaFullLoad(gear)

Conditions for an upshift:

  * Operation point (Teng, neng) before upshift is right to upshift line.
  * $a_{estimated} > min(\textrm{UpshiftMinAcceleration}, \textrm{DriverAcceleration})$  (if TC is locked)

       Or
	$a_{estimated} > min(\textrm{CLUpshiftMinAcceleration}, \textrm{DriverAcceleration})$  (if TC is unlocked)


### Efficiency shifts

The efficiency shift algorithm for APT works similar to the AMT algorithm in case of locked gears. In order to depict differences in gear selection which result from the different shifting sequences (APT: powershift, AMT: traction interruption) the operation points used for rating of fuel efficiency and for checking the power requirements in a candidate gear are calculated differently. More specifically, this assessment looks 0.8 seconds into the future, so that a relevant operating point after the shift is considered.

For up-shifts from a torque converter gear ("C") to a locked gear ("L") the estimated engine speed in the locked gear has to be above a certain threshold. This threshold depends on the engine's load stage and the road gradient.

**Shift rules for L -> L shifts (Efficiency shifts):**

The search algorithm for the next gear is as follows:

  $FC_{gear} = min(FC_{gear + i})   \forall i \in \textrm{Allowed gear range}$

Additionally the candidate gear has to fulfil the boundary conditions below for an efficiency upshift.  

  * $i_{gear + axle} \leq  \textrm{RatioEarlyDownshift}$ 
  * Not left to downshift line 
  * $1 - (P_{eng} / P_{eng,max}) > T_{reserve} 		($T_{reserve}$  is set to 0)
  * $FC_{gear} < FC_{current gear} * \textrm{Rating current gear}$

For an efficiency downshift following conditions are met for the potential gear:

  * $i_{gear + axle} \leq \textrm{RatioEarlyDownshift}$ 
  * Not right upshift line
  * $1 - (P_{eng} / P_{eng,max}) > T_{reserve}$ 		($T_{reserve}$  is set to 0)
  * $FC_{gear} < FC_{current gear} * \textrm{Rating current gear}$

**Shift rules for C -> L shifts (Efficiency shifts):**

The used algorithm can be summarised as follows:

Definitions:

| **Parameter** | **Unit** | **Description**  |
|-----------|------|--------------|
| torque ratio | [%] | current engine torque / maximum engine torque at actual engine speed  |
| a_min | [ m/s²] | available acceleration at actual engine torque for maximum loaded vehicle  |
| a_max | [m/s²] | available acceleration at actual engine torque for empty vehicle  |
| a_curr | [m/s²] | available acceleration at actual engine torque for current vehicle mass  |

In each time-step a target post-shift engine speed from the shift strategy is calculated in a three step approach:
  * The current engine load stage is determined based on current torque ratio and a set of hysteresis thresholds
  * For the current engine load stage and the current slope each a rpm value is interpolated from a parameter table
  * The final value for target post-shift engine speed is interpolated for the current value of a_curr from the results of the previous step

If the estimated engine speed after a C -> L shift is calculated to be equal or higher than the target engine speed as calculated above, the gear shift is initiated. This approach in combination with the proposed parameters as shown below reflects the strategy that shifts from C -> L are performed with absolute priority in order to minimise driveline losses from torque converter operation.

Boundary values between engine load stages (values for torque ratio in [%]) (relevant for C -> L shifts)

| Load stage       | 1<->2 | 2<->3  | 3<->4  | 4<->5  |  5<->6 |
|------------------|-------|--------|--------|--------|--------|
| Hysteresis upper | 19.70 |  36.34 |  53.01 |  69.68 |  86.35 |
| Hysteresis lower | 13.70 |  30.34 |  47.01 |  63.68 |  80.35 |


Matrix with target post-shift engine speed offset above idling speed (values in rpm, relevant for C -> L shifts)

| engine load stage|a_max, slope +5% | a_max, slope 0% | a_max, slope -5% | a_min, slope +5% | a_min, slope 0% | a_min, slope -5% |
|------------------|-----------|----------|-----------|------------|----------|------------|
| 1                |  90       | 120      | 165       |  90        | 120      | 165        |
| 2                |  90       | 120      | 165       |  90        | 120      | 165        |
| 3                |  90       | 120      | 165       |  90        | 120      | 165        |
| 4                |  90       | 120      | 165       |  110       | 140      | 185        |
| 5                |  100      | 130      | 175       |  120       | 150      | 195        |
| 6                |  110      | 140      | 185       |  130       | 160      | 205        |


