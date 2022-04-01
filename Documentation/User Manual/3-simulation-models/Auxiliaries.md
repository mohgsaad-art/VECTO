## Auxiliaries

<div class="declaration">
In Declaration mode the auxiliaries are pre-defined and the power demand is defined based on the vehicle category and mission. For every type of auxiliary (fan, steering pump, HVAC, electrig system, pneumatic system) the user can select a technology from a given list.
</div>

<div class="engineering">
In Engineering mode VECTO uses a generic map-based approach to consider all types of auxiliaries. The supply power demand for each single auxiliary is defined in the driving cycle. Hence a time/distance-dependent power demand can be defined. Based on the supply power and a pre-defined efficiency map the auxiliary input power is calculated. A constant efficiency determines the losses between auxiliary and engine.

For each auxiliary the power demand is calculated using the following steps:

![](pics/AuxModel.svg)

1.  Auxiliary speed: **n~aux~ = n~Eng~ \* TransRatio**

2.  Auxiliary output power: **P~auxOut~ = P~supply~/EffToSply**

3.  Auxiliary input power: **P~auxIn~ = EffMap(n~Aux~, P~AuxOut~)**

4.  Auxiliary power consumption: **P~aux~ = P~auxIn~/EffToEng**

5.  **P~aux~ is added to the engine's power demand**

6.  **P~supply~ is defined in the driving cycle


|            |                                                                                                       |                                 |  
| ---------- | ----------------------------------------------------------------------------------------------------- | --------------------------------|  
| n~Eng~     | Calculated engine speed.                                                                            | \[1/min\]                       |  
| TransRatio | Speed ratio between auxiliary and engine. [Defined in the Auxiliary File](#auxiliary-input-file-.vaux). | \[-\]                           |  
| n~aux~     | Auxiliary speed                                                                                     | \[1/min\]                       |  
| P~supply~  | Effective supply power demand. [Defined in the driving cycle](#driving-cycles-.vdri).             | \[kW\]                          |  
| EffToSply  | Consumer efficiency. [Defined in the Auxiliary File](#auxiliary-input-file-.vaux).                      | \[-\]                           |  
| P~auxOut~  | Auxiliary output power                                                                              | \[kW\]                          |  
| EffMap     | Auxiliary efficiency map. [Defined in the Auxiliary File](#auxiliary-input-file-.vaux).                 | \[kW\] = f( \[1/min\], \[kW\] ) |  
| P~auxIn~   | Auxiliary input power                                                                               | \[kW\]                          |  
| EffToEng   | Efficiency of auxiliary (belt/gear) drive. [Defined in the Auxiliary File](#auxiliary-input-file-.vaux).| \[-\]                           |  
| P~aux~     | Mechanical auxiliary power demand at the crank shaft                                                | \[kW\]                          |  





Each auxiliary must be defined in the [Job File](#job-file) and each [driving cycle](#driving-cycles-.vdri) used with this vehicle/auxiliary must include supply power for each auxiliary. To link the supply power in the driving cycle to the correct auxiliary in the Job File an ID is used. The corresponding supply power is then named *"&lt;Aux\_ID&gt;"*.


***Example:*** *The Auxiliary with the ID "ALT" (in the Job File) is linked to the supply power in the column "&lt;Aux\_ALT&gt;" in the driving cylce.*

In addition to the generic map-based auxiliaries approach it is also possible to specify a constant load applied to the engine during the whole mission.

</div>