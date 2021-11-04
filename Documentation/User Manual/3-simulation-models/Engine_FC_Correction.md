##Engine Fuel Consumption Correction

The final fuel consumption is corrected in a post-processing to reflect systems not directly modeled in VECTO (e.g. electric waste heat recovery sysmtes) or to account for systems not active all the time for different reasons (e.g., engine stop-start).

###Engine Stop/Start Correction

As the energy demand of auxiliaries is modeled as an average power demand over the whole simulated cycle, the demand of certain auxiliaries during engine-off periods needs to be compensated during engine-on periods. This is done using the [Engine-Line approach](#engine-line-approach).

During the simulation the combustion engine is allways off. In this phases the "missing" auxiliary demand is balanced in separate colums for the cases a) the ICE is really off, and b) the ICE would be on. This allows for an accurate correction of the fuel consumption taking into account that ESS is in reality not active in all possible cases due to e.g. auxiliary power demand, environmental conditions, etc.

A general goal is that the actual auxiliary demand matches the target auxiliary demand over the cycle. So in case the ICE is off, some systems still consume electric energy but no electric energy is created during ICE-off phases. Or in case of bus auxiliaries the total air demand is pre-calculated and thus leading to an average air demand over the cycle. During ICE-off phases, however, no compressed air is generated. This 'missing' compressed air is corrected in the post-processing.

A utility factor (UF) considers that the ICE is not off in all cases. Therefore the fuel consumption for compensating the missing auxiliary demand consists of two parts. The first part considers the fuel consumption required for the 'missing' auxiliary demand if the ICE is really off. Here the according auxiliary energy demand is multiplied by the utility factor and the engine line. The second part considers the fuel consumption in case the ICE would not be switched off. Here the 'missing' auxiliary energy demand is multiplied by (1 - utility factor) and the engine line and the idle fuel consumption is added for time periods the ICE would be on.

For the post-processing two different utility factors are considered. One for ICE-off phases during vehicle standstill and one for ICE-off phases during driving.

####ICE Start

$\textrm{E\_ICE\_start} = \sum{\textrm{P\_ICE\_start} \cdot dt}$

$\textrm{FC\_ICE\_start} = \textrm{E\_ICE\_start} \cdot k_\textrm{engline}$

####Mechanical Auxiliaries

$\textrm{E\_aux\_ESS\_mech\_ICEoff\_standstill} = \sum_{\forall \textrm{v\_act}_i = 0}{\textrm{P\_aux\_ESS\_mech\_ICE\_off} \cdot dt}$

$\textrm{E\_aux\_ESS\_mech\_ICEoff\_driving} = \sum_{\forall \textrm{v\_act}_i > 0}{\textrm{P\_aux\_ESS\_mech\_ICE\_off} \cdot dt}$


$\textrm{E\_aux\_ESS\_mech\_ICEon\_standstill} = \sum_{\forall \textrm{v\_act}_i = 0}{\textrm{P\_aux\_ESS\_mech\_ICE\_on} \cdot dt}$

$\textrm{E\_aux\_ESS\_mech\_ICEon\_driving} = \sum_{\forall \textrm{v\_act}_i > 0}{\textrm{P\_aux\_ESS\_mech\_ICE\_on} \cdot dt}$


$$
\begin{align*}
\textbf{\textrm{FC\_ESS}} =\, &  \textrm{FC\_ICE\_start} + \\
     &  \textrm{E\_aux\_ESS\_mech\_ICEoff\_standstill} \cdot k_\textrm{engline} \cdot \textrm{UF}_\textrm{standstill}  + \\
     &  (\textrm{E\_aux\_ESS\_mech\_ICEon\_standstill} \cdot k_\textrm{engline} + \textrm{FC}(n_\textrm{idle}, 0) \cdot \textrm{t\_ICEoff\_standstill}) \cdot (1 - \textrm{UF}_\textrm{standstill}) \\
     &   \textrm{E\_aux\_ESS\_mech\_ICEoff\_driving} \cdot k_\textrm{engline} \cdot \textrm{UF}_\textrm{driving} + \\
     &  (\textrm{E\_aux\_ESS\_mech\_ICEon\_driving} \cdot k_\textrm{engline} + \textrm{FC}(n_\textrm{idle}, 0) \cdot \textrm{t\_ICEoff\_driving}) \cdot (1 - \textrm{UF}_\textrm{driving})
\end{align*}
$$


####Bus Auxiliaries Correction -- Electric System

The bus auxiliaries electric system correction is used for conventional vehicles with ESS and buses with smart electric system in the same way. 

$\textrm{E\_BusAux\_ES\_consumed} = \sum{\textrm{P\_BusAux\_ES\_consumed} \cdot dt}$

$\textrm{E\_BusAux\_ES\_gen} =  \sum{\textrm{P\_BusAux\_ES\_gen} \cdot dt}$

$\Delta\textrm{E\_BusAux\_ES\_mech} = (\textrm{E\_BusAux\_ES\_consumed} - \textrm{E\_BusAux\_ES\_gen}) / \textrm{AlternatorEfficiency} / \textrm{AlternatorGearEfficiency}$

$\textbf{\textrm{FC\_BusAux\_ES}} = \textrm{E\_BusAux\_ES} \cdot k_\textrm{engline}$

####Bus Auxiliaries Correction -- Electric System Supply from REESS

$\textrm{E\_DCDC\_missing} =  \textrm{P\_DCDC\_missing} \cdot dt$

$\textrm{E\_DCDC\_missing\_mech} = \textrm{E\_DCDC\_missing} / \textrm{DCDC\_ConverterEfficiency} / \eta_{\textrm{EM}_\textrm{chg}}$

$\textbf{\textrm{FC\_DCDCMissing}} = \textrm{E\_DCDC\_missing\_mech} \cdot k_\textrm{engline}$


####Bus Auxiliaries Correction -- Pneumatic System

For the pneumatic system the goal of the post-processing correction is that the correct amount of compressed air is generated, even when the ICE is off. As the average
air demand is calculated with an estimated cycle driving time, the first step is to correct the air demand using the actual cycle driving time.
The missing (or excessive) amout of air is transferred into mechanical energy demand using $k_\textrm{Air}$. This value depicts the delta energy demand for a certain delta compressed air.
$k_\textrm{Air}$ is derived from two points. on the one hand the compressor runs in idle mode, applying only the drag load and producing no compressed air and the second point is that the compressor 
is always on, applying the always-on mechanical power demand and generating the maximum possible amount of compressed air.
The mechanical energy is then corrected using the engineline.

$\textrm{E\_busAux\_PS\_drag} = \sum_{\textrm{Nl\_busAux\_consumed}_i = \textrm{Nl\_busAux\_gen}_i}{\textrm{P\_busAux\_PS\_drag}\cdot dt}$

$\textrm{E\_busAux\_PS\_alwaysOn} =  \sum_{\textrm{Nl\_busAux\_consumed}_i = \textrm{Nl\_busAux\_gen}_i}{\textrm{P\_busAux\_PS\_alwaysOn} \cdot dt}$

$\textrm{Nl\_alwaysOn} =  \sum_{\textrm{Nl\_busAux\_consumed}_i = \textrm{Nl\_busAux\_gen}_i}{\textrm{Nl\_busAux\_gen\_max}}$

$k_\textrm{Air} = \frac{\textrm{E\_busAux\_PS\_alwaysOn} - \textrm{E\_busAuxPS\_drag}}{\textrm{Nl\_alwaysOn} - 0}$


![](pics/BusAux_PS_kAir.png)

$\textrm{CorrectedAirDemand} = \textrm{[Calculate Air demand with actual cycle time]}$

$\textrm{AirGenerated} =  \sum{\textrm{Nl\_busAux\_PS\_gen}}$

$\Delta\textrm{Air} = \textrm{CorrectedAirDemand} - \textrm{AirGenerated}$

$\textrm{E\_busAux\_PS\_corr} = \Delta\textrm{Air} \cdot k_\textrm{Air}$

$\textrm{FC\_BusAux\_PS\_AirDemand} = \textrm{E\_busAux\_PS\_corr} \cdot k_\textrm{engline}$

$\textrm{FC\_BusAux\_PS\_Drag\_ICEoff\_driving} = \textrm{P\_PS\_drag}(n_\textrm{idle}) \cdot k_\textrm{engline} \cdot \textrm{t\_ICEoff\_driving} \cdot (1 - \textrm{UF}_\textrm{driving})$

$\textrm{FC\_BusAux\_PS\_Drag\_ICEoff\_standstill} = \textrm{P\_PS\_drag}(n_\textrm{idle}) \cdot k_\textrm{engline} \cdot \textrm{t\_ICEoff\_standstill} \cdot (1 - \textrm{UF}_\textrm{standstill})$


$$
\begin{align*}
\textbf{\textrm{FC\_BusAux\_PS}} =\, &   \textrm{FC\_BusAux\_PS\_AirDemand}  + \\
 & \textrm{FC\_BusAux\_PS\_Drag\_ICEoff\_driving} + \\
 & \textrm{FC\_busAux\_PS\_Drag\_ICEoff\_standstill} \\
\end{align*}
$$


####Bus Auxiliaries Correction -- Aux Heater

The power demand for an additional fuel-fired heater is calculated in the post-processing. The HVAC steaty state model calculates the heating demand (weighted sum of different climatic conditions) and based on the engine's average waste heat over the cycle the power demand for the aux heater is calculated. The fuel consumption for the aux heater is only added for the main fuel:


$E_\textrm{ice,waste heat} = \sum_\textrm{fuels} FC_\textrm{final,sum}(fuel) * NCV_\textrm{fuel}$

$\overline{P}_\textrm{ice,waste heat} = E_\textrm{ice, waste heat} / t_\textrm{cycle}$

$\textrm{E\_auxHeater} = \textrm{HVACSSM}_\textrm{AuxHtr}(\overline{P}_\textrm{ice,waste heat}) * t_\textrm{cycle}$

$\textbf{\textrm{FC\_BusAux\_AuxHeater}} = \textrm{E\_auxHeater} \cdot \textrm{NCV}_\textrm{main fuel}$

####Waste Heat Recovery Systems

$\textrm{E\_WHR\_mech} = \sum{\textrm{P\_WHR\_mech} \cdot dt}$

$\textrm{E\_WHR\_el} = \sum{\textrm{P\_WHR\_el} \cdot dt}$

$$
\textrm{E\_WHR\_el\_mech} = \begin{cases}
\textrm{E\_WHR\_el} / \textrm{AlternatorEfficiency} & if conventional truck \\
\textrm{E\_WHR\_el} / \eta_{\textrm{EM}_\textrm{chg}} & if bus with ES connected to REES and smart alternator \\
\textrm{E\_WHR\_el} / \textrm{BusAlternatorEfficiency} & otherwise
\end{cases}
$$

$\textbf{\textrm{FC\_WHR}} = - (\textrm{E\_WHR\_mech} + \textrm{E\_WHR\_el\_mech}) \cdot k_\textrm{engline}$


####Hybrid Vehicles: REESS SoC Correction

If the REESS Soc at the end of the simulation is higher than the initial SoC the correction is done according to:

$$
\textbf{\textrm{FC\_SoC}} = -\frac{\Delta\textrm{E\_REESS} \cdot k_\textrm{engline}}{\eta_{\textrm{EM}_\textrm{chg}} \cdot \eta_{\textrm{REESS}_\textrm{chg}}} 
$$


If the REESS Soc at the end of the simulation is lower than the initial SoC the correction is done according to:

$$
\textbf{\textrm{FC\_SoC}} = - \Delta\textrm{E\_REESS} \cdot k_\textrm{engline} \cdot \eta_{\textrm{EM}_\textrm{dischg}} \cdot \eta_{\textrm{REESS}_\textrm{dischg}} 
$$


$\eta_{\textrm{REESS}_\textrm{chg}} = \frac{\textrm{E\_REESS\_INT\_CHG}}{\textrm{E\_REEES\_T\_CHG}}$

$\eta_{\textrm{REESS}_\textrm{dischg}} = \frac{\textrm{E\_REESS\_INT\_DISCHG}}{\textrm{E\_REEES\_T\_DISCHG}}$


###Corrected Total Fuel Consumption

The final fuel consumption after all corrections are applied is calcualted as follows:

$$
\begin{align*} 
\textrm{FC\_FINAL} =\;&  \textrm{FC\_ModSum} \;+ \\
                      &  \textrm{FC\_ESS} \;+ \\
                      &  \textrm{FC\_DCDCMissing} \;+ \\
                      &  \textrm{FC\_BusAux\_PS} \;+ \\
                      &  \textrm{FC\_BusAux\_ES} \;+ \\
                      &  \textrm{FC\_WHR} \;+ \\
                      &  \textrm{FC\_BusAux\_AuxHeater} \;+ \\
                      &  \textrm{FC\_SoC} \\
\end{align*}
$$


###Engine-Line Approach

The total fuel consumption is corrected in a post-processing step according to the *engine-line* approach. Therefore, for every engine operating point where the engine is  on and has a positive fuel consumption the fuel consumption is plotted over the engine power. The slope (k) of the linear regression of the fuel consumption is used to compute the additional fuel that is needed for the energy demand during engine-off periods and engine starts.

![](pics/FC_Correction.PNG)
