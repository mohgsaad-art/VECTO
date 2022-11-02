## Hybrid Strategy Parameters Editor


![](pics/HybridStrategyParams.png)

### Description


The [Hybrid Strategy Parameters File (.vhctl)](#hybrid-strategy-parameters-file-.vhctl) defines all parameters used by the [Parallel Hybrid Control Strategy](#parallel-hybrid-control-strategy)/[Serial Hybrid Control Strategy](#serial-hybrid-control-strategy) to evaluate the best option for splitting the demanded torque between electric motor and combustion engine.

### Strategy Parameters

The hybrid control strategy evaluates different allocations of torque to the electric motor and different gears and calculates the following cost function:

$C = \sum_{i \in  \textrm{Fuels}}{FC_{i} \cdot NCV_{i} \cdot dt} + f_{\textrm{equiv}} \cdot (P_\textrm{Bat} \cdot dt + C_{\textrm{Pen1}}) \cdot f_{SoC} + C_{\textrm{Pen2}}$

$f_\textrm{SoC} = 1 - \left(\frac{\textrm{SoC} - \textrm{TargetSoC}}{0.5 \cdot (\textrm{SoC}_\textrm{max} - \textrm{SoC}_{min}}  \right)^e + C_\textrm{SoC}$

The parameters for the cost function can be defined in the hybrid strategy file.

Evquivalence Factor Discharge
:   $f_{\textrm{equiv}}$ in case the battery is discharged

Evquivalence Factor Charge
:   $f_{\textrm{equiv}}$ in case the battery is charged

Min SoC
:   $\textrm{SoC}_\textrm{min}$

Max SoC
:   $\textrm{SoC}_\textrm{max}$

Target SoC
:   $\textrm{TargetSoC}$

Min ICE On Time
:   In case the ICE was turned on, it cannot be turned of for this period of time

Aux Buffer Time 
:   In case electric auxiliaries are connected to the high-voltage system, reserve a certain amount of energy in the battery to supply the auxiliaries for this period of time.

Aux Buffer Charge Time
:   In case electric auxiliaries are connected to the high-voltage system and the reserved energy for the auxiliaries is used, re-charge the "auxiliaries buffer" in within this period of time.

ICE Start penalty factor
:   Penalty added to the cost function in case the ICE needs to be turned on

Cost Factor SoC Exponent
:   Exponent $e$ in the cost function

