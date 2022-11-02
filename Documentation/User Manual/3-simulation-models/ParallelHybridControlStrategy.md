## Parallel Hybrid Control Strategy

The basic principle of the hybrid control strategy is to evaluate different options of operating modes, i.e., different splits of the demanded torque at the wheels among the electric motor and the combustion engine. For every option a cost function is calculated, taking onto account the required electric energy and the fuel consumption. Out of the examined operating modes the best option, i.e, the option with the lowest cost value is selected.

The hybrid control is located in the simulated power train right after the wheels. Hence, the hybrid control strategy gets as input the torque and angular velocity at the wheels as input. 

### Model Parameters

#### Hybrid Strategy

* MinICEOnTime
* MinSoC
* MaxSoC
* TargetSoC
* EquivalenceFactor
* Cost Factor SoC Exponent $e$
* AuxReserveTime
* AuxReserveChargeTime

#### Gear Selection

* MinTimeBetweenGearshifts
* DownshiftAfterUpshiftDelay
* UpshiftAfterDownshiftDelay
* AllowedGearRangeUp
* AllowedGearRangeDown


### Evaluation of different options

Note: The convention is that for all powertrain components (except te ICE) a positive torque loss means an additional drag while a negative torque loss means the component contributes to propel the vehicle. So all passive components can only apply positive torque losses and only active components such as electric motors can propel the vehicle which means it has a negative torque loss.

The variable u is used to identify the different evaluated options. The value of u denotes the factor how much of the torque at the output shaft of the electric motor is applied by the electric motor. A u value of -1 thus means the electric motor provides the full torque demanded at its output shaft and the torque at the input shaft is 0. A positive value of u means that the electric motor acts as generator and applies a torque demand in addition. 

In case the driver's action is to accelerate the vehicle, the hybrid control strategy performs the following steps to obtain a list of potential configurations:

1.  Issue a dry-run request with the currently demanded torque and angular speed.
    
    For this request the electric motor is switched off. The purpose of this request is to get the resulting power demand at the combustion engine and more importantly, to get the minimum/maximum torque the electric motor can provide and the maximum/minimum torque the combustion engine can provide.
    This is also a viable configuration and thus added to the list of evaluated configurations

2.  Evaluate options where the electric motor contributes to propel the vehicle.

    i.  Iterate over all negative u values with a certain step size (typically 0.1) up to u_maxDrive
        u_maxDrive is detemined by the torque demanded at the out shaft of the electric motor and the maximum drive torque of the electric motor -- whichever is lower.
    ii. If the case where the electric motor applies its maximum drive torque is not already covered by the  
        iteration of u values in the previous step, calculate the maximum drive configuration explicitly
    iii. If it is allowed to turn off the electric motor or the electric motor can propel during gear shifts, 
         search the torque the electric motor needs to provide so that the torque at the gearbox input gets 0. This means the electric motor provides more torque than demanded in order to overcome losses of components later in the powertrain. If this torque value is within the limits of the electric motor, calculate the corresponding u value and add this option to the list of evaluated configurations.

 3. Evaluate options where the electric motor acts as generator and applies additional drag losses. 

     i. Iterate over all positive u values with a certain step size (typically 0.1) up to the electric 
        motor's maximum generation torque.
    ii. For vehicles of configuration P2 evaluate the configuration where the electric motor's generation 
        torque equals the torque demanded at the electric motor's output shaft (i.e., the torue at the electric motor's input shaft is 0) if it is allowed to turn of the ICE.
    iii. For vehicles of configuration P3 and P4 search for the torque the electric motor has to apply as 
         a generator so that the resulting torque at the combustion engine is 0. If this torque value is within the limits of the electric motor, calculate the corresponding u value and add this option to the list of evaluated configurations.

In case of a coast or roll action (e.g. during look-ahead coasting dur during traction interruption) the electric motor is turned off.

In case the driver performs a brake aktion the following options are considered

1. In case of vehicle configurations P3 or P4, or vehicle configuration P2 and the gearbox is engaged:
   (1) If the combustion engine is on and the torque demand at the combustion engine is above the drag 
       curve, switch the electric motor off.
   (2) If the torque demand at the combustion engine is below the drag curve, evaluate all options as 
       described for the case the driver accelerates (see above).
2. In case of vehicle configuration P2 and the gearbox is not engaged, turn the electric motor off 

### Gear selection

For hybrid vehicles it is not possible to decouple gear selection from the electric motor's operating point because the gearshift strategy only considers the çombustion engine's operating point. In some situations it is more efficient to select a different gear which results in an overall more efficient operating point (considering both, electric motor and combustion engine).

The hybrid strategy combines the main ideas of the EffShift gearshift strategy and the selection of the best operating point.

Depending on the last gearshift the allowed gear range for upshifts and downshifts is determined. For every allowed gear all possible settings of the hybrid powertrain as describe above are evaluated.

### Cost Function

A cost value is calculated for every evaluated solution described above. In case the configurration results in an invalid operating point the cost value is set to invalid. Reasons for invalid configurations are that the engine operating point is outside the shift polygons, the engine speed is too high or too low, the electric power demand is too high or too low, the battery's SoC would go below the $\textrm{SoC}_{low}$ threshold, etc.

If a configuration is not valid because for example the ICE speed is too high or too low, or the torque demand is too high, or too low the corresponding value of the cost function is set to 'NaN' (not a number) and thus the total score is invalid. In addition, certain flags indicating why a certain configuration is considered invalid are set. These flags are used for the selection of a hybrid configuration to be used as described below. *Note: the calculated  score may be a valid number but certain ignore flags may be set. For example if the engine speed is slightly too high or the battery SoC is

For all valid configurations a cost function is calculated which basically considers the fuel consumption and the electric power:

$C = \sum_{i \in  \textrm{Fuels}}{FC_{i} \cdot NCV_{i} \cdot dt} + f_{\textrm{equiv}} \cdot (P_\textrm{Bat} \cdot dt + C_{\textrm{Pen1}}) \cdot f_{SoC} + C_{\textrm{Pen2}}$

* FC is the combustion engine's fuel consumption for the current simulation interval
* NCV denotes the fuel's net calorific value
* $P_\textrm{Bat}$ is the power drawn from the battery. Positive values denote the battery is discharged
* $f_\textrm{equiv}$ is the equivalence factor to compare energy from the ICE and energy from the electric system. Typically in the range of 2.5
* $f_\textrm{SoC}$ is a cost factor that depends on the battery's state of charge.
* $C_\textrm{Pen1}$ is a penalty for starting the combustion engine. It is set to 0.1 times the energy required to ramp up the combustion engine. The ramp-up energy is calculated the same way as for the engine stop/start correction - see [Advanced Driver Assistant Systems: Engine Stop/Start](#advanced-driver-assistant-systems-engine-stopstart).

    If the combustion engine is currently off and is off in the considered configuration $P_\textrm{Pen1}$ is set to 0.

    If the battery's SoC is below the lower SoC threshold $\textrm{SoC}_{low}$ then $P_\textrm{Pen1}$ is set to 0.
* $C_\textrm{Pen2}$ is a penalty considering idling costs of the combustion engine, currently set to 0.

$f_\textrm{SoC} = 1 - \left(\frac{\textrm{SoC} - \textrm{TargetSoC}}{0.5 \cdot (\textrm{SoC}_\textrm{max} - \textrm{SoC}_{min}}  \right)^e + C_\textrm{SoC}$

$C_\textrm{SoC} = \left\{
	\begin{array}{ll} 
		\frac{C_\textrm{minSoC}}{\textrm{SoC}_\textrm{min} - \textrm{SoC}_{low}} \cdot \textrm{SoC} + p_\textrm{minSoC} - \frac{C_\textrm{minSoC}}{\textrm{SoC}_\textrm{min} - \textrm{SoC}_{low}} \cdot \textrm{SoC}_{min} : \textrm{SoC} < \textrm{SoC}_{low}\\
		0 : \textrm{otherwise}
	\end{array}
\right.$

$\textrm{SoC}_\textrm{low} = \textrm{SoC}_{min} + 0.1 \cdot \left(\textrm{SoC}_{max} - \textrm{SoC}_{min}\right)$

$C_\textrm{minSoC} = 10$

The following graph depicts the shape of $f_\textrm{SoC}$ (red line) and both summands separately (blue: polynomial function, orange: C_\textrm{SoC}) for a minimum SoC of 20%, maximum SoC of 80% and a target SoC of 50%;

![](pics/graph_SoC-Factor.png)

#### Flags for ignoring a evaluated hybrid configuration

- *EngineSpeedTooLow*: the engine speed is below the engine idle speed
- *EngineSpeedTooHigh*: the engine speed is above the engine's $n_{95h}$ speed
- *EngineTorqueDemandTooHigh*: the torque demanded from the engine is above the dynamic full-load
- *EngineTorqueDemandTooLow*: the torque demanded from the engine is below the drag-torque
- *EngineSpeedAboveUpshift*: the engine operating point is right of the upshift line of the current gear
- *EngineSpeedBelowDownshift*: the engine operating point is left of the downshift line of the current gear
- *BatteryBelowMinSoC*: the battery's state of charge falls below the allowed minimum SoC
- *BatteryAboveMaxSoC*: the battery's state of charge exceeds the allowed maximum SoC
- *BatterySoCTooLow*: the strategy may add a certain safety margin to the minimum SoC for certain reasons. Set if the SoC falls below the lower boundary $\textrm{SoC}_\textrm{low}$


### Selection of the best option.

From the list of possible hybrid powertrain configurations with its cost value the best option is selected according to the following list of conditions. If one or many configurations match the criteria listed in a step, the first configuration is used. If no configuration matches the criteria the next step is evaluated.

1. Select all configurations with a valid score (i.e. the score is not NaN). 
    - If the vehicle speed is above the gearbox' start speed no flag to ignore the configuration must be set.
    - If the vehicle speed is below the gearbox' start speed (i.e. the vehicle is accelerating from standstill) the engine speed must not be too high.
    - Order the configurations by score
2. Select all configurations with a valid score and the engine speed is valid (i.e., not too high, nor too low and within the shift lines) and order by score
3. Select all configurations with a valid score and order by score
4. If the driver is accelerating and in all evaluated configurations the engine's torque demand is above the engine's maximum torque filter the possible configurations according to the following criteria
    (1) If the electric motor can propell during traction interruptions (i.e., P4 and P3 configurations) or the gearbox is engaged (P2 configuration) select all configurations where the battery SoC is within the allowed range, order the configurations by difference in gear to the current gear and then order the configurations by the mecanical torque the electric motor can provide
    (2) 
5. If the driver is accelerating and in all evaluated configurations the engine's torque demand is below the engine's drag torque filter the possible configurations according to the following criteria. If the electric motor can propel during traction interruptions (i.e., P4 and P3 configurations) or the gearbox is engaged (P2 configuration) 
    (1) Select all configurations where the engine speed is valid and the battery's SoC is within the allowed range and order the configurations by the difference in gear to the current gear and then by the mechanical torque the motor can provide
    (2) Select all configurations where the battery's SoC is within the allowed range and order the configurations by the difference in gear to the current gear and then by the mechanical torque the motor can provide
6. If the driver is accelerating and the gearbox is engaged filter the possible configurations according to the following criteria.
    (1) Select all configurations where the engine speed is not too low nor too high and order the configurations by the difference in gear to the current gear
    (2) If no entries match the previous criteria order all configurations by the difference in gear to the current gear
    (3) Order the configurations by the mechanical torque provided by the electric motor
7. If the driver is braking and the gearbox is engaged select all configurations where the battery SoC is within the allowed range and order by the torque the electric motor can apply for braking
