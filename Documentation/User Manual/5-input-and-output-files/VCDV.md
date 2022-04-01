## Speed Dependent Cross Wind Correction Input File (.vcdv)

The file is needed for speed dependent [Cross Wind Correction](#vehicle-cross-wind-correction). The file uses the [VECTO CSV format](#csv).

- Filetype: .vcdv
- Header: **v_veh [km/h], Cd [-]**
    + **v_veh [km/h]**: the vehicle speed
    + **Cd [-]**: the scaling factor which will be multiplied with the base C~d~A value.
- Requires at least 2 data entries

**Example:**

~~~
v_veh [km/h],Cd [-]
0           ,1.173
5           ,1.173
10          ,1.173
15          ,1.173
20          ,1.173
25          ,1.173
30          ,1.173
35          ,1.173
40          ,1.173
45          ,1.173
50          ,1.173
55          ,1.173
60          ,1.173
65          ,1.153
70          ,1.136
75          ,1.121
80          ,1.109
85          ,1.099
90          ,1.090
95          ,1.082
100         ,1.07
~~~


