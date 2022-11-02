## RESS

### Battery

The battery model uses the following model parameters:

- Capacity of the battery pack
- Maximum current for charging and discharging over the state of charge
- Minimum state of charge
- Maximum state of charge
- Voltage of the battery pack over state of charge
- Internal resistance of the battery pack over state of charge. The internal resistance can either be constant over the pulse duration or depending on the length of the pulse duration (see [.vbatr Battery Internal Resistance](#battery-internal-resistance-file-.vbatr))

The voltage curve over state of charge is described in [Battery Internal Voltage File (.vbatv)](#battery-internal-voltage-file-.vbatv) and the internal resistance curve over state of charge is described in [Battery Internal Resistance File (.vbatr)](#battery-internal-resistance-file-.vbatr). The file format of the maximum current map is described in [Battery Max Current Map (.vimax)](#battery-max-current-map-.vimax).

During the simulation the battery's state of charge must always be between the minimum and maximum SoC threshold.

![](pics/BatteryVoltage.png)

The maximum discharge current is further limited by the battery's internal resistance:

$I_\textrm{disch,max} = \frac{U(\textrm{SoC})}{4 * R_i(\textrm{SoC})}$

#### Time-dependent Internal Resistance

If the internal resistance is provided for different pulse durations, the actual internal resistance is interpolated between the provided resistance values with the current pulse duration. No extrapolation is applied. For pulses below Ri_2, Ri_2 is applied, for pulse durations longer then Ri_20 (or Ri_120 if provided) this value is used. The pulse duration is reset every time the current changes its sign.

### Modular Battery System

VECTO allows to connect multiple batteries togehter to a single battery system. Therefore, every battery has assigned a stream identifier. All batteries with the same stream identifier are connected in series. All battery strins are then connected in parallel.

The following picture shows 4 batteries in series (3x Bat A + Bat B) and Bat C parallel to this. So two different streams need to be defined.

![](pics/BatterySystem.png)

All batteries of a string of the mudular battery system are aggregated to a single "big battery". In the example above, BigBattery1 consists of (Bat A, Bat A, Bat A, Bat B), and BigBattery2 consists of (Bat C). Nevertheless, the state of charge is calculated for each battery module independently.

The capacity of a BigBattery is the capacity of the smalles of all modules on a string. The maximum current of a BigBattery is also the lowest maximum current of all modules on a string. The open circuit voltage is the sum of all modules on a string and the internal resistancee is also the sum of all modules on a string.

The maximum charge and discharge power of the whole REESS is the sum of the maximum charge/discharge power of all BigBatteries in the system. The actual power demand is distributed to the BigBatteries as follows:

$P_i = \frac{C_i}{\sum_{j}{C_j}} \cdot \delta_i \cdot P_\textrm{act}$

$\delta_i = 1 - \textrm{sgn}(P_\textrm{act})\frac{\textrm{SoC}_i - \textrm{SoC}_\textrm{avg}}{\textrm{SoC}_\textrm{avg}}$

$\textrm{SoC}_\textrm{avg} = \frac{\sum_j{\textrm{SoC}_j \cdot C_j}}{\sum_j{C_j}}$

$\textrm{SoC}_i = \frac{\min_{B \in BB}(\textrm{SoC}_B \cdot C_B)}{C_i}$


$P_i$ ... Power for BigBattery i

$C_i$ ... Nominal capacity of BigBattery i.

$P_\textrm{act}$ ... actual power demand

In case a BigBattery reaches its max. power, the power for this BigBattery is limited to its max. power and then the power distribution is re-calculated with the remaining power demand.

To update the state of charge of each battery, the current $I_i$ for each BigBattery is computed from the following equation:

$P_i = (U_i + R_i \cdot I_i) \cdot I_i$

And then the power demand for every single battery can be computed from

$P_{B_i} = (U_{B_i} + R_{B_i} \cdot I_i) \cdot I_i$

![](pics/BatterySystemCalculation.png)

### Super Capacitor

The super capacitor model uses the following model parameters:

- Capacity of the SuperCap in Farad
- Internal resistance
- Minimum voltage
- Maximum voltage
- Maximum current charging
- Maximum current discharging

The values of maximum charging current and maximum discharging current need to be positive!

