## Auxiliaries

<div class="declaration">
In Declaration mode the auxiliaries are pre-defined and the power demand is defined based on the vehicle category and mission. For every type of auxiliary (fan, steering pump, HVAC, electrig system, pneumatic system) the user can select a technology from a given list.
</div>

<div class="engineering">
In Engineering mode the auxiliary power demand for the following states of the vehicle can be defined:

   - ICE On
   - Vehicle driving, ICE off
   - Vehicle standstill, ICE off

If the ICE is on, the auxiliary power demand is directly applied to the combustion engine. In case the ICE is off, the according power demand is balanced in the modal data and the fuel consumption is [corrected in post processing](#engine-fuel-consumption-correction).
</div>