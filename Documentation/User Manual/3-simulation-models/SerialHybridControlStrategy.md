## Serial Hybrid Control Strategy

The basic principle of the serial hybrid strategy is to operate the GenSet in three different states, depending on the power demand of the drivetrain and the REESS' state of charge. So the serial hybrid strategy operates as a three-point controller with a hysteresis.

The following picture illustrates the basic idea. If the SoC is above the target SoC, the GenSet is off and the vehicle drives solely from the battery. When the SoC gets lower and reaches the SoC_min threshold, the GenSet is switched on. As long as the SoC is between SoC_min and SoC_target, the GenSet operates in the optimal operating point. When the upper threshold SoC_target is reached, the GenSet is switched off. In case the power demand from the drivetrain is higher than what the GenSet can provide in the optimal point and the SoC falls below the lower threshold SoC_min, the GenSet operates either in the maximum power operating point (if the drivetrain power demand is higher than the power generated in the optimal point) or in the optimal point.

![](pics/SerialHybrid_SoCBoundaries.png)

The statemachine for the serial hybrid control strategy is depicted here:

![](pics/SerialHybrid_Statemachine.png)

**Note:** The SoC boundaries (SoC_target and SoC_min) shall be narrower than the batteries SoC limits so that on the one hand the vehicle can still recuperate even in case the REESS is charging and reaching the target SoC and on the other hand that there is a buffer available if the drivetrain power demand is high and the GenSet needs some time to ramp up to the maximal power operating point.

### GenSet Pre-Processing

The optimal and maximal GenSet operating points are calculated in a pre-processing step. The fuel consumption and generated electric power is calculated for 400 different operating points: from ICE idle speed up to the maximum speed (minimum of ICE and electric motor), and from 0 mechanical power up to the maximum mechanical power of the ICE. Out of this set of operating points the one with the highest electrical power and the operating point with the best fuel efficiency is selected. This is done for the GenSet operating in de-rating or not. 