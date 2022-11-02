## Electric Motor

The electric motor is modeled by basically 4 map files:

 - Maximum drive torque over motor speed for two different voltage levels
 - Maximum generation torque over motor speed for two different voltage levels
 - Electric power map ($P_\textrm{map,el}$) for two different voltage levels
 - Drag curve (i.e., the motor is not energized) over motor speed
 - Continuous torque ($T_\textrm{cont}$)
 - Engine speed for continuous torqe ($n_\textrm{T,cont}$)
 - Overload torque ($T_\textrm{ovl}$)
 - Engine speed for overload torque  ($n_\textrm{T,ovl}$)
 - Maximum overload time ($t_\textrm{ovl}$)

The first two curves are read from a .vemp file (see [Electric Motor Max Torque File (.vemp)](#electric-motor-max-torque-file-.vemp)). The drag curve is provided in a .vemd file (see [Electric Motor Drag Curve File (.vemd)](#electric-motor-drag-curve-file-.vemd)) and the electric power map in a .vemo file (see [Electric Motor Map (.vemo)](#electric-motor-power-map-.vemo)).

During the simulation the maximum drive torque, maximum generation torque, and electric power map is interpolated for both voltage levels and the actual value used is interpolated between both voltage levels with the current internal voltage of the REESS.

The drag curve is used to add additional drag to the powertrain in case the electric motor is turned off.

The convention for all input files is that positive torque values drive the vehicle while negative torque values apply additional drag and generate electric power.

The follwing picture shows the signals used in VECTO and provided in the .vmod file. The VECTO convention is that positive torque adds additional drag to the drivetrain. Thus, if the electric motor propells the vehicle it applies negative torque.

![](pics/electric_motor_map.png)

### Electric Motor Model

The VECTO component for the electric motor contains the electric motor itself which is connected via a transmission stage to the drivetrain. The ratio and efficiency of the transmission stage can be defined in the vehicle model.

![](pics/EM_Model_scaled.png)

The naming convention for the signals is that 'X' denotes the position of the EM in the powertrain. P_X_... denotes signals related to the drivetrain speed while P_X-em_... denotes signals to the electric motor shaft.

P_X_in = P_X_out + P_X_mech

P_X_mech = P_X-em_mech + P_X_transm_loss

P_X-em_mech = P_X-em_mech_elmap + P_X-em_inertia

P_X-em_mech_elmap = P_X-em_el + P_X-em_loss

P_X-em_mech_elmap = n_X-em * T_X-em_map

P_X-em_el = PowerMap(n_X-em, T_X-em_map)

P_X_loss = P_X_mech - P_X-em_el


### Electric Power Map Interpolation

The electric power demand of the electric machine is not directly interpolated in the provided power map. Due to the characteristic of the map (increasing electric power with both, torque and speed) the resulting delaunay map may cause deviations from the assumed electric power demand depending on how the triangles are actually added to the delaunay map.

Therefore, the electric power map is converted to a "virtual torque loss" map similar to the transmission loss-maps. For every entry in the electric power map, the virtual torque loss is calculated as follows:

$T_\textrm{loss,em-map} = \frac{P_\textrm{el}(n_\textrm{em}, T_\textrm{em}) - n_\textrm{em} \cdot T_\textrm{em}}{n_\textrm{em}}$

From the tuple $(n_\textrm{em}, T\textrm{em}, T_\textrm{loss,em-map})$ a Delaunay map is created. In the simulation the actual electric power is then calculated as follows:

$P_\textrm{el}(n_\textrm{em}, T_\textrm{em}) = \textrm{Delaunay}_\textrm{EM-Map}(n_\textrm{em}, T_\textrm{em}) \cdot n_\textrm{em} + n_\textrm{em} \cdot T_\textrm{em}$

### Thermal De-Rating

The electric machine can be overloaded for a certain period. In addition to the maximum drive and generation torque (which already is in overload condition) the mechanical power the electric machine can generate is required.

The basic principal of the thermal de-rating is as follows: based on the continuous power and the angular velocity for the continuous power as well as the maximum overload time a thermal energy buffer is calculated. During the simulation the difference between the current losses in the electric machine and the losses at the continuous power operating point are integrated over time. If this value reaches the capacity of the thermal energy buffer the electric machine can only deliver the specified continuous torque until the thermal energy buffer goes below a certain threshold.


$E_\textrm{th,buf} = (P_\textrm{loss,ovl} - P_\textrm{loss,cont}) \cdot t_\textrm{ovl}$

$P_\textrm{loss,ovl} = P_\textrm{map, el}(T_\textrm{ovl}, n_\textrm{T, ovl}) - n_\textrm{T,ovl} \cdot T_\textrm{ovl}$

$P_\textrm{loss,cont} = P_\textrm{map, el}(T_\textrm{cont}, n_\textrm{T, cont}) - n_\textrm{T,cont} \cdot T_\textrm{cont}$

The overload buffer is calculated for both voltage levels of the electric motor. Both, the overload buffer and continuous losses used in the simulation are interpolated with the voltage level of the REESS at average usable SoC level.

In every simulation step the losses of the electric machine are accumulated:

$E_{\textrm{ovl,} i + 1} = E_{\textrm{ovl,} i} + (P_\textrm{loss, i} - P_\textrm{loss,cont}) \cdot dt$

$P_\textrm{loss, i} = P_\textrm{map, el}(T_\textrm{em, mech}, n_\textrm{em}) - n_\textrm{em} \cdot T_\textrm{em, mech}$


If $E_\textrm{ovl, i}$ reaches the overload capacity $E_\textrm{th,buf}$ the power of the electric machine is limited to the continuous torque until $E_\textrm{ovl,i}$ goes below the overload capacity multiplied by the thermal overload recovery factor. Then the maximum torque is available again.

