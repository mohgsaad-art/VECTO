## Driver: Overspeed

Overspeed controls the vehicle's behaviour on uneven road sections (slope ≠ 0) and can be configured in the [Job File](#job-file)'s Driver Assist Tab. Overspeed is designed to model an average driver's behaviour without the aid of driver assistance systems. Eco-Roll  represents an optional driver assistance feature. For this reason vehicles without Eco-Roll should always have the Overspeed function enabled.

Overspeed activates as soon as the total power demand at the wheels (Pwheel) falls below zero, i.e. the vehicle accelerates on a negative slope. The clutch remains closed, engine in motoring operation, and the vehicle accelerates beyond the cycle's target speed. When the speed limit (target speed plus **Max. Overspeed**) is reached the mechanical brakes are engaged to prevent further acceleration.


![](pics/Overspeed.svg)

*Example with target (purple) and actual speed (orange) on the top left axis, slope (brown) on the top right axis. The bottom graph shows engine power (blue), motoring curve (orange) and mechanical brake power (green). In this example Overspeed is allowed until the vehicle's speed exceeds target speed by 5 \[km/h\].*


Parameters in [Job File](#job-file):
:	-   **Minimum speed \[km/h\]**. Below this speed the function is disabled.
-   **Max. Overspeed \[km/h\]** (relative to target speed)


## Advanced Driver Assistant Systems: Engine Stop/Start

### Description

If engine stop/start is enabled in the Vehicle, the engine is turned off during vehicle stops to reduce the fuel consumption. During vehicle stops the energy demand for certain auxiliaires and for starting the engine is accumulated. In a post-processing step the final [fuel consumption is corrected](#engine-fuel-consumption-correction) to consider the energy demand for the auxiliaries and engine start.

### Model Parameters

   - **Delay engine-off:** if the vehicle stops, the engine is switched off after this timespan
   - **Max engine-off timespan:** if the enine is switched off at a vehicle stand, the engine is turned on again after this timespan. This basically limits the max. time the engine is switched off at a single engine-off event.
   - **Engine stop/start utility factor:** In practice, the engine is not switched off at every vehicle stop. This is considered with this utility factor (0...1). Further details are provided below.

<div class="declaration">
   - delay engine-off: 2 s
   - Max engine-off timespan: 120 s
   - Engine stop/start utility factor: 0.8
</div>

### Engine Start-Up Energy Demand

The energy demand to ramp-up the engine depends on the engine's inertia and the engine's drag torque and is computed according to the following equation:

$E_{ICE,rampUp} = 0.5 * I_{ICE} * n_{idle}^2 + T_{drag}(n_{idle}) * n_{idle} / 2 * t_{ICE,start}$

$E_{ICE,start} = E_{ICE,rampUp} / \eta_{alternator}^2$


$E_{ICE,start}$ is the amount of energy the combustion engine needs to provide to compensate the start up is the ramp-up energy multiplied by the efficiency of the alternator.  $t_{ICE,start}$ is assumed to be 1 second and $\eta_{alternator}$ is 0.7.

### Auxiliaries and Utility Factor

During ICE-off phases the ICE is fully shut of in the simulation (.vmod data). However, in reality the ICE is not always switched off due to certain
boundary conditions (e.g. power demand from an auxiliary, temperature, etc.). This is considered in the [post-processing](#engine-fuel-consumption-correction). 
Therefore, the demand for different auxiliaries is balanced in separate columns in the [.vmod](#modal-results-.vmod) file for the two cases a) ICE is really off, and b) ICE would be on. 
This is done for the mechanical auxiliaries, bus-aux electric demand (all different cases like ES connected to the REESS, smart ES, conventional ES, and combinations thereof), bus-aux pneumatic system. A detailed description which auxiliary power demand is balanced in which columns can be found in [this spreadsheet](BusAuxCases with ESS_Formatted.xlsx) for all combinations of conventional vehicles, bus auxiliaries, and hybrid vehicles.

<div class="declaration">
**Auxiliary energy demand**

In Declaration Mode the energy demand of all auxiliaries except the engine cooling fan and the steering pump is considered during vehicle stops.

</div>

<div class="engineering">
**Auxiliary energy demand**

In Engineering Mode the energy demand of the auxiliaries can be specified for the cases:
   - ICE on
   - ICE off, vehicle standstill
   - ICE off, vehicle driving


</div>


## Advanced Driver Assistant Systems: Eco-Roll

### Description

Eco-roll is a driver assistant system that automatically decouples the internal combustion engine from the power train during specific downhill driving conditions with low negative slopes. The aim is to save fuel during such phases. VECTO supports eco-roll without engine stop/start and eco-roll with engine stop/start. In the former case, the combustion engine is idling during eco-roll phases while in the latter case the combustion engine is turned off during eco-roll events. For vehicles having eco-roll with engine stop/start the fuel consumption is corrected for the engine stop/start events and the auxiliary power demand during engine-off phases.

In case of AT gearboxes eco-roll can either be performed by shifting to neutral, i.e., disengaging the gearbox, or opening the torque converter lockup clutch. Which option is supported by the transmission needs to be specified in the vehicle configuration.

<div class="declaration">
**Auxiliary energy demand**

In Declaration Mode the energy demand of all auxiliaries is applied in the fuel consumption correction during engine-off periods
</div>

<div class="engineering">
**Auxiliary energy demand**

In Engineering Mode the energy demand for the different states 
   - ICE on
   - Vehicle driving, ICE off
   - Vehicle standstill, ICE off

can be specified. When the ICE is on, the auxiliary energy demand is directly applied. The auxiliary energy demand during ICE-off phases is [corrected in post-processing](#engine-fuel-consumption-correction).
</div>


### Model Parameters

  - **Minimum speed:** minimum vehicle speed to allow eco-roll to be activated
  - **Activation delay:** delay between the point in time when all conditions for an eco-roll event are fulfilled until eco-roll is activated
  - **Underspeed threshold:** Threshold below the target speed to disable eco-roll 
  - **AT EcoRoll Release Lockup Clutch:** Required only for AT transmissions. If set to true, the lockup clutch is released during eco-roll events and the gear is engaged. If set to false, the gearbox switches to neutral.

<div class="declaration">
  - Minimum speed: 60 km/h
  - Activation delay: 2s
  - Underspeed threshold: 0 km/h
</div>

### Eco-Roll Model

**Calulations during simulation**

$a_{veh,est} = \frac{F_{grad}(x) + F_{roll}(x) + F_{aero}(v_{veh})}{m_{veh}}$

**Eco-Roll State Diagram**

The following state diagram depicts when eco-roll is activated during the simulation.

![](pics/EcoRollActivation.svg)

## Advanced Driver Assistant Systems: Predictive Cruise Control

### Description

Predictive cruise control (PCC): systems which optimise the usage of potential energy during a driving cycle based on an available preview of road gradient data and the use of a GPS system. A PCC system declared in the input to the simulation tool shall have a gradient preview distance longer than 1000 meters and cover all following use cases:

**Use Case 1: Crest Coasting**

Approaching a crest the vehicle velocity is reduced before the point where the vehicle starts accelerating by gravity alone compared to the set speed of the cruise control so that the braking during the following downhill phase can be reduced.

**Use Case 2: Accelerating without Engine Power**

During downhill driving with a low vehicle velocity and a high negative slope the vehicle acceleration is performed without any engine power usage so that the downhill braking can be reduced.

**Use Case 3: Dip Coasting**

During downhill driving when the vehicle is braking at the overspeed velocity, PCC increases the overspeed for a short period of time to end the downhill event with a higher vehicle velocity. Overspeed is a higher vehicle speed than the set speed of the cruise control system.

In VECTO a vehicle may either support use cases 1 and 2 or all three use cases.

Predictive cruise control is only considered on highway sections of the simulated driving cycle (see [sistance-based driving cycle](#engineering-mode-target-speed-distance-based-cycle).

<div class="declaration">
In declaration mode, the whole long-haul cycle is considered as highway. Moreover, the section from 29760m to 96753m of the regional delivery cycle is considered as highway.
</div>

### Model Parameters

   - **Allowed underspeed:** Threshold below the target speed the vehicle's velocity may be reduced to during a PCC event (use-case 1 & 2, $v_{neg}$)
   - **Allowed overspeed:** Threshold above the target speed the vehicle's velocity may reach during a PCC event (use-cae 3)
   - **PCC enabling velocity:** Only highway sections of the driving cycle with a target velocity greater than or equal to the enabling velocity are considered for PCC events.
   - **Minimum speed:** Minimum vehicle speed for allowing PCC use-case 2
   - **Preview distance use case 1:** Preview distance for use-case 1 PCC events. After this distance (estimated) after starting the PCC event the vehicle shall reach the target speed again.
   - **Preview distance use case 2:** Preview distance for use-case 2 PCC events. After this distance (estimated) after starting the PCC event the vehicle shall reach the target speed again. This distance is typically shorter than the preview distance for use-case 1 as only the acceleration phase is considered.

<div class="declaration">
   - Allowed underspeed: 8 km/h
   - Allowed overspeed: 5 km/h
   - PCC enabling velocity: 80 km/h
   - Minimum speed: 50 km/h
   - Preview distance use case 1: 1500 m
   - Preview distance use case 2: 1000 m
</div>

### Predictive Cruise Control Model Use-cases 1 and 2

**Pre-Processing**

1. In a preprocessing step the road gradient where the vehicle would accelerate on its own is computed for certain velocities. If the vehicle is equipped with eco-roll the powertrain is declutched, otherwise the engine is in full drag. The slope is calculated for every simulated cycle as this values vary with the vehicle's payload, rolling resistance and air drag.
2. All positions in the driving cycle where the slope is lower than the road gradient required that the vehicle accelerates on its own are marked as potential candidates for PCC events. At this distance the vehicle's velocity shall be a minimum. Denoted as $x_{v_{low}}$.
3. For every potential PCC event, the end position is marked in the driving cycle. This is the first position in the driving cycle after $x_{v_{low}}$ where the slope is greater than the road gradient required that the vehicle accelerates on its own. Latest at this position the vehicle shall reach the target velocity again. Denoted as $x_{end, max}$
4. For every potential PCC event, the earliest start position is marked. This is calculated as $x_{start} = x_{v_{low}} - d_{preview}$.
5. For every potential PCC event, the vehicle's energy is calculated:
$E(x_{v_{low}}) = m \cdot g \cdot h(x_{v{low}}) + \frac{m \cdot (v_{target}(x_{v_{low}}) - v_{neg})^2}{2}$
$E(x_{end, max}) = m \cdot g \cdot h(x_{end, max}) + \frac{m \cdot v_{target}(x_{end, max})^2}{2}$

**Calulations during simulation**

If the vehicle enters a potential PCC section, the following calculations are performed to decide on starting a PCC event:

1. Current vehicle position: $x$
2. Position in the cycle where the PCC event shall be finished: $x_{end} = min(x + d_{preview}, x_{end, max})$
3. Estimation of coasting resistance force:  
$F_{coast}(x) = \frac{P_{roll}(x) + P_{aero}(x, v_{target}) + P_{ice, drag} + P_{em, drag}}{v_{target}}$  
$P_{ice, drag}$ is set to 0 in case the vehicle is equipped with eco-roll and pure electric vehicles.
$P_{em,drag}$ is set to 0 for conventional vehicles.
4. Energy demand/gain for coasting from the vehicle's current position to the point with the minimum velocity $x_{v_{low}}$:  
$E_{coast, v_{low}} = F_{coast} \cdot (x_{v_{low}} - x)$
5. Energy demand/gain for coasting from the vehicle's current position to the end of the PCC event $x_{end}$:  
$E_{coast, x_{end}} = F_{coast} \cdot (x_{end} - x)$
6. Vehicle's current energy:  
$E_{veh}(x) = m \cdot g \cdot h(x) + \frac{m \cdot v_{veh}^2}{2}$
7. Vehicle's energy at the end of a PCC event:  
$E(x_{end}) = m \cdot g \cdot h(x_{end}) + \frac{m \cdot v_{target}(x_{end})^2}{2}$

**PCC State Diagram**

The following state diagram depicts when a PCC event is activated during the simulation for conventional vehicles.

![](pics/PredictiveCruiseControl_Conventional.png)

The following state diagram depicts the activation of a PCC event during the simulation for xEV vehicles.

![](pics/PredictiveCruiseControl_xEV.png)

The fuel consumption of vehicles equipped with PCC option 1 & 2 and eco-roll with engine stop/start will be corrected for engine stop/start as described in [engine stop/start correction](#engine-fuel-consumption-correction).

### Predictive Cruise Control Model Use-case 3

To consider predictive cruise control use-case 3, the driver model's allowed overspeed is set to the model parameter *allowed overspeed* in highway sections if the vehicle supports PCC use-case 3.
