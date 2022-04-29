## Acceleration Limiting Input File (.vacc)

The file is used for [Acceleration Limiting](#driver-acceleration-limiting). It defines the acceleration and deceleration limits as function of
vehicle speed. The filepath has to be defined in the [Job File](#job-file). The file uses the [VECTO CSV format](#csv).

- Filetype: .vacc
- Header: **v \[km/h], acc \[m/s^2], dec \[m/s^2]**
    + **v [km/h]**: the vehicle speed. Must be >= 0 km/h.
    + **acc [m/s^2]**: the maximum acceleration. Must be > 0 m/s^2.
    + **dec [m/s^2]**: the maximum deceleration. Must be < 0 m/s^2.
- Requires at least 2 data entries
- Data should cover the whole possible range of vehicle speeds

**Note:** The deceleration should be lower than a certain threshold for low speeds in order to guarantee accurate vehicle stops during simulation. The suggested deceleration should be lower than -0.5m/s^2 for vehicle speeds below 30 km/h.

**Example Data:**

~~~
v [km/h],acc [m/s^2],dec [m/s^2]
0       ,1          ,-1
25      ,1          ,-1
50      ,0.6        ,-1
60      ,0.5        ,-0.5
120     ,0.5        ,-0.5
~~~


**Example Graph: **

![The graph shows the acceleration and deceleration limits depending on the current vehicle speed.](pics/AccLimit.png)
