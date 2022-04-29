## Fuel Consumption Map (.vmap)
The FC map is used to interpolate the base fuel consumption before corrections are applied. For details see [Fuel Consumption Calculation](#engine-fuel-consumption-calculation). The file uses the [VECTO CSV format](#csv).


- Filetype: .vmap
- Header: **engine speed [rpm], torque [Nm], fuel consumption [g/h]**
- Requires at least 3 data entries
- The map must cover the full engine range between full load and motoring curve.

Extrapolation of fuel consumption map is possible in Engineering Mode (with warnings!). In Declaration Mode it is not allowed.

**Example:**

~~~
engine speed [rpm],torque [Nm],fuel consumption [g/h]
600               ,-45        ,0
600               ,0          ,767
600               ,100        ,1759
600               ,200        ,2890
600               ,300        ,4185
600               ,400        ,5404
600               ,500        ,6535
600               ,600        ,7578
...
~~~



