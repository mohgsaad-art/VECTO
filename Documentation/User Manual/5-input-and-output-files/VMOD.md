## Modal Results (.vmod)

Modal results are only created if enabled in the [Options](#main-form) tab. One file is created for each calculation and stored in the same directory as the .vecto file.

In Vecto 3 the structure of the modal data output has been revised and re-structured. Basically for every powertrain component the .vmod file contains the power at the input shaft and the individual power losses for every component. For the engine the power, torque and engine speed at the output shaft is given along with the internal power and torque used for computing the fuel consumption. See [Powertrain and Components Structure](#powertrain-and-components-structure) for schematics how the powertrain looks like and which positions in the powertrain the values represent.

Every line in the .vmod file represents the simulation interval from time - dt/2 to time + dt/2. All values represent the average power/torque/angular velocity during this simulation interval. If a certain power value can be described as function of the vehicle's acceleration the average power is calculated by
$P_{avg} = \frac{1}{simulation interval} \int{P(t) dt}$. 
**Note:** Columns for the torque converter operating point represent the torque/angular speed at the end of the simulation interval!

 The following table lists the columns in the .vmod file:
	
***Quantities:***

| **Name**          | **Unit** | **Description** |
| ----------------- | ---------------------- | -------------------------- |
| time				|	[s]		|	Absolute time. Timestamp at the middle of the current simulation interval [time - dt/2, time + dt/2] |
| dt				|	[s]		|	Length of the current simulation interval |
| dist				|	[m]		|	Distance the vehicle traveled at the end of the current simulation interval |
| v_act				|	[km/h]	|	Average vehicle speed in the current simulation interval |
| v_targ			|	[km/h]	|	Target speed |
| acc				|	[m/s^2]	|	Vehicle's acceleration, constant during the current simulation interval |
| grad				|	[%]		|	Road gradient |
| Gear				|	[-]		|	Gear. "0" = clutch opened / neutral |
| TC locked         |   0/1     |   For AT-Gearboxes: if the torque converter is locked or not |
| n_eng_avg			|	[1/min]	|	Average engine speed in the current simulation interval. Used for interpolation of the engine's fuel consumption |
| T_eng_fcmap		|	[Nm]	|	Engine torque used for interpolation of the engine's fuel consumption. T_eng_fcmap is the sum of torque demand on the output shaft, torque demand of the auxiliaries, and engine's inertia torque |
| Tq_full			|	[Nm]	|	Engine's transient maximum torque (see [transient full load](#engine-transient-full-load)) |
| Tq_drag			|	[Nm]	|	Engine's drag torque, interpolated from the full-load curve |
| P_eng_fcmap		|	[kW]	|	Total power the engine has to provide, computed from n_eng_avg and T_eng_fcmap |
| P_eng_full		|	[kW]	|	Engine's transient maximum power  (see [transient full load](#engine-transient-full-load)) |
| P_eng_drag		|	[kW]	|	Engine's drag power |
| P_eng_inertia		|	[kW]	|	Power loss/gain due to the engine's inertia |
| P_eng_out			|	[kW]	|	Power provided at the engine's output shaft |
| P_clutch_loss		|	[kW]	|	Power loss in the clutch due to slipping when driving off |
| P_clutch_out		|	[kW]	|	Power at the clutch's out shaft. P_clutch_out = P_eng_out - P_clutch_loss |
| P_TC_out          |   [kW]    |   Power at the torque converter's out shaft. P_TC_out = P_eng_out - P_TC_loss |
| P_TC_loss         |   [kW]    |   Power loss in the torque converter |
| P_aux				|	[kW]	|	Total power demand from the auxiliaries |
| P_gbx_in			|	[kW]	|	Power at the gearbox' input shaft |
| P_gbx_loss		|	[kW]	|	Power loss at the gearbox, interpolated from the loss-map + shift losses + inertia losses |
| P_gbx_shift       |   [kW]    |   Power loss due to gearshifts (AT gearbox) |
| P_gbx_inertia		|	[kW]	|	Power loss due to the gearbox' inertia |
| P_ret_in			|	[kW]	|	Power at the retarder's input shaft. P_ret_in = P_gbx_in - P_gbx_loss - P_gbx_inertia |
| P_ret_loss		|	[kW]	|	Power loss at the retarder, interpolated from the loss-map. |
| P_angle_in		|	[kW]	|	Power at the anglegear's input shaft. Empty if no Anglegear is used. |
| P_angle_loss		|	[kW]	|	Power loss at the anglegear, interpolated from the loss-map. Empty if no Anglegear is used. |
| P_axle_in			|	[kW]	|	Power at the axle-gear input shaft. P_axle_in = P_ret_in - P_ret_loss ( - P_angle_loss if an Angulargear is used). |
| P_axle_loss		|	[kW]	|	Power loss at the axle gear, interpolated from the loss-map. |
| P_brake_in		|	[kW]	|	Power at the brake input shaft (definition: serially mounted into the drive train between wheels and axle). P_brake_in = P_axle_in - P_axle_loss |
| P_brake_loss		|	[kW]	|	Power loss due to braking. |
| P_wheel_in		|	[kW]	|	Power at the driven wheels. P_wheel_in = P_brake_in - P_brake_loss |
| P_wheel_inertia	|	[kW]	|	Power loss due to the wheels' inertia |
| P_trac			|	[kW]	|	Vehicle's traction power. P_trac = P_wheel_in - P_wheel_inertia |
| P_slope			|	[kW]	|	Power loss/gain due to the road's slope |
| P_air				|	[kW]	|	Power loss due to air drag. |
| P_roll			|	[kW]	|	Rolling resistance power loss. |
| P_veh_inertia		|	[kW]	|	Power loss due to the vehicle's inertia |
| P_aux_<XXX>		|	[kW]	|	Power demand for every individual auxiliary. Only if the run has auxiliaries. |
| P_PTO_RoadSweeping|	[kW]	|	Power demand from the PTO in PTO mode 2. Only in engineering mode if PTO mode 2 is activated. |
| P_PTO_DuringDrive |	[kW]	|	Power demand from the PTO cycle in PTO mode 3. Only in engineering mode if PTO mode 3 is activated. |
| P_PTO_consum		|	[kW]	|	Power demand from the PTO consumer. Only if the vehicle has a PTO consumer. |
| P_PTO_transmission|	[kW]	|	Power demand from the PTO transmission. Only if the vehicle has a PTO consumer. |
| AA_NonSmartAlternatorsEfficiency     | [Fraction]  | Non-Smart Alternators Efficiency, Advance Auxiliaries Module |
| AA_SmartIdleCurrent_Amps             | [Amps]      | Smart Idle Current in Amps, Advance Auxiliaries Module |
| AA_SmartIdleAlternatorsEfficiency    | [Fraction]  | Smart Idle Alternators Efficiency, Advance Auxiliaries Module |
| AA_SmartTractionCurrent_Amps         | [Amps]      | Smart Traction Current in Amps, Advance Auxiliaries Module |
| AA_SmartTractionAlternatorEfficiency | [Fraction]  | Smart Traction Alternator Efficiency, Advance Auxiliaries Module |
| AA_SmartOverrunCurrent_Amps          | [Amps]      | Smart Overrun Current in Amps, Advance Auxiliaries Module |
| AA_SmartOverrunAlternatorEfficiency  | [Fraction]  | Smart Overrun Alternator Efficiency, Advance Auxiliaries Module  |
| AA_CompressorFlowRate_LitrePerSec    | [Ni L/S]    | Compressor Flow Rate in litres per second, Advance Auxiliaries Module |
| AA_OverrunFlag                       | [Bool [0/1] | Overrun Flag (yes/no), Advance Auxiliaries Module |
| AA_EngineIdleFlag                    | [Bool [0/1] | Engine Idle Flag (yes/no), Advance Auxiliaries Module |
| AA_CompressorFlag                    | [Bool [0/1] | Compressor Flag (off/on), Advance Auxiliaries Module |
| AA_TotalCycleFC_Grams                | [Grams]     | Total Cycle Fuel Consumption in grams, Advance Auxiliaries Module |
| AA_TotalCycleFC_Litres               | [Litres]    | Total Cycle Fuel Consumption in litres, Advance Auxiliaries Module |
| TCnu              |   [-]     |   Torque converter operating point:  speed ratio    | 
| TCmu              |   [-]     |   Torque converter operating point:  torque ratio    |   
| T_TC_out          |   [Nm]    |   Torque converter operating point:  output torque    | 
| n_TC_out          |   [rpm]   |   Torque converter operating point:  output speed    |  
| T_TC_in           |   [Nm]    |   Torque converter operating point:  input torque    |    
| n_TC_in           |   [rpm]   |   Torque converter operating point:  input speed    |     
| FC-Map			|	[g/h]	|	Fuel consumption interpolated from FC map. |
| FC-AUXc			|	[g/h]	|	Fuel consumption after [Auxiliary-Start/Stop Correction](#engine-fuel-consumption-calculation) (based on FC) |
| FC-WHTCc			|	[g/h]	|	Fuel consumption after [WHTC Correction](#engine-fuel-consumption-calculation) (based on FC-AUXc) |
| FC-AAUX			|	[g/h]	|	Fuel consumption computed by the AAUX module considering smart auxiliaries |
| FC-ADAS			|	[g/h]	|	Fuel consumption corrected for specified ADAS technologies (declaration mode only) |
| FC-Final			|	[g/h]	|	Final fuel consumption value after all applicable corrections |


P_eng_FCmap = T_eng_fcmap * n_eng_avg

P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer ) = P_loss_total + P_AUX + P_eng_inertia

P_loss_total = P_clutch_loss + P_gbx_loss + P_ret_loss + P_gbx_inertia + P_angle_loss + P_axle_loss + P_brake_loss +
								P_wheel_inertia + P_air + P_roll + P_grad + P_veh_inertia (+ P_PTOconsumer + P_PTO_transm)

P_trac = P_veh_inertia + P_roll + P_air + P_slope

