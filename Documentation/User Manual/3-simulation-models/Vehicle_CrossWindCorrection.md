## Vehicle: Cross Wind Correction


VECTO offers three different modes to consider cross wind influence on the drag coefficient. It is configured in the [Vehicle File](#vehicle-file-.vveh).
The aerodymanic force is calculated according to the following equation:

$F_{aero}=1/2 \rho_{air}(C_{d,v}A(v_{veh})) v_{veh}^2$

The speed dependecy of the $C_dA$ value allows for consideration of average cross widn conditions.

### Speed dependent correction (Declaration Mode)


This is the mode which is used in [Declaration Mode](#declaration-mode).

The crossind correction is based on the following boundary conditions:

1 Average wind conditions: The typical conditions are defined with 3m/s of wind at a height of 4m above ground level, blowin uniformly distributed from all directions.
2 Dependency of $C_dA$ value on yaw angle: The dependency of the $CdA$ value on yaw angle is described by generic $3^{rd}$ order polynomial functions of the form: 

$C_dA(\beta) - C_dA(0) = a_1\beta + a_2\beta^2 + a_3\beta^3$ 

The following table gives the coefficients per vehicle type:

|                        |  a1       | 	a2       | 	a3        |
|------------------------|-----------|-----------|------------|
| rigid solo	         | 0.013526  | 	0.017746 | 	-0.000666 |
| rigid trailer, EMS     | 0.017125  | 	0.072275 | 	-0.004148 |
| tractor semitrailer	 | 0.030042  | 	0.040817 | 	-0.002130 |
| bus, coach	         | -0.000794 | 	0.021090 |  -0.001090 |


In a pre-processing step VECTO calculates the function for $C_dA$ value as a function of vehicle speed. This is done by integration of all possible directions of the ambient wind from ground level to maximum vehicle height considering the boundary layer effect based on the following formulas: 

$C_{d,v}A(v_{veh}) = \frac{1}{2 \pi v_{veh}^2 h_{veh}}\int_{\alpha = 0^{\circ}}^{\alpha = 360^{\circ}}{\int_{h=0}^{h=h_{veh}}{C_dA(\beta)\cdot v_{air}(h, \alpha)^2} \textit{d}h\ \textit{d}\alpha}$

$v_{air}(h) = \sqrt{(v_{wind}(h)\cdot\cos\alpha + v_{veh})^2 + (v_{wind}(h)\cdot\sin\alpha)^2}$

$v_{wind}(h) = v_{wind}(h_{ref})\cdot \left(\frac{h}{h_{ref}}\right)^{0.2}$

with

$\alpha \ldots \text{direction of ambient wind relative to the vehicle x-axis}$

$h \ldots \text{height above ground}$

$h_{ref} \ldots \text{reference heigth, 4m, for 3m/s average ambient wind}$

$v_{air} \ldots \text{resulting air flow velocity from vehicle speed and ambient wind}$

$v_{veh} \ldots \text{vehicle speed}$

$v_{wind} \ldots \text{velocity of ambient wind}$

The generation of the $C_{d,v}A(v_{veh})$ curve is demonstrated in [this Excel sheet](Cdv_Generator_VECTO3.2.xlsx)

### Speed dependent correction (User-defined)
The base C~d~A value (see [Vehicle File](#vehicle-file-.vveh)) is corrected with a user-defined speed dependent scaling function. A [vcdv-File](#speed-dependent-cross-wind-correction-input-file-.vcdv) is needed for this calculation.

The C~d~A value given in the vehicle configuration is corrected depending on the vehicle's speed and the C~d~ scaling factor from the input file as follows:

$C_dA(v_{veh}) = C_dA * F_C_d(v_{veh})$

 ![](pics/VCDV.png)


### Correction using Vair & Beta Input

The actual (measured) air speed and direction can be used to correct cross-wid influence if available. A [vcdb-File](#vair-beta-cross-wind-correction-input-file-.vcdb) is needed for this calculation. This file defines a ΔC~d~A value in \[m²\] depending on the wind angle. The [driving cycle](#driving-cycles-.vdri) must include the air speed relative to the vehicle v~air~ (\<vair\_res\>) and the wind yaw angle (\<vair\_beta\>).

The C~d~A value given in the vehicle configuration is corrected depending on the wind speed and wind angle (given in the driving cycle) using the input file as follows:

$C_dA(v_veh) = C_dA + {\Delta}C_d(\beta)$



 ![](pics/VCDB.png)
