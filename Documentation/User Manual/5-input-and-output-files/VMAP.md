## Fuel Consumption Map (.vmap)

The FC map is used to interpolate the base fuel consumption before corrections are applied. For details see [Fuel Consumption Calculation](#engine-fuel-consumption-calculation). The file uses the [VECTO CSV format](#csv).


- Filetype: .vmap
- Header: **engine speed [rpm], torque [Nm], fuel consumption [g/h]**, *whr power electrical \[W\]*, *whr power mechanical \[W\]* (required only if an electric or mechanical WHR system is used)
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


**Example with electric WHR***

~~~
engine speed [rpm] , torque [Nm] , fuel consumption [g/h] , whr power electric [W]
500                , -135.5      , 0                      , 200
500                , 0           , 1355                   , 200
500                , 213.4       , 3412.291               , 200
500                , 426.8       , 5830.1                 , 200
500                , 640.2       , 8316.426               , 200
500                , 853.6       , 10439.87               , 200
500                , 1067        , 12823.69               , 200
500                , 1188        , 14228.79               , 200
500                , 1401.4      , 16628.66               , 200
600                , -138        , 0                      , 200
600                , 0           , 1355                   , 200
600                , 213.4       , 3412.291               , 200
600                , 426.8       , 5830.1                 , 200
...
~~~

