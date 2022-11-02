## Electric Motor Max Torque File (.vemp)

This file contains the electric motor's maximum drive torque and maximum recuperation torque depending on the motor's angluar speed. The file uses the [VECTO CSV format](#csv).

- Filetype: .vemp
- Header: **n [rpm] , T_drive [Nm] , T_recuperation [Nm]**
- Requires at least 2 data entries

**Example:**

~~~
n [rpm] , T_drive [Nm] , T_recuperation [Nm]
0       , 802.14       , -802.14
1600    , 802.14       , -802.14
1665    , 802.14       , -802.14
1675    , 798.16       , -798.16
1685    , 793.42       , -793.42
1695    , 788.74       , -788.74
...
~~~

## Electric Motor Drag Curve File (.vemd)

This file contains the electric motor's drag torque (i.e. the eletric motor is not energized) depending on the motor's angluar speed. The file uses the [VECTO CSV format](#csv).

- Filetype: .vemd
- Header: **n [rpm] , T_drag [Nm]**
- Requires at least 2 data entries

**Example:**

~~~
n [rpm] , T_drag [Nm]
0       , -10
5000    , -50
~~~


## Electric Motor Power Map (.vemo)

This file is used to interpolate the electric power required for a certain mechanical power at the eletric motor's shaft.  The file uses the [VECTO CSV format](#csv).

- Filetype: .vemo
- Header: **n [rpm] , T [Nm] , P_el [kW]**
- Requires at least 2 data entries

**Example:**

~~~
n [rpm], T [Nm], P_el [kW]
0      , -1600 , 19.6898
0      , -1550 , 18.5438
0      , -1500 , 17.4322
...
0      , 1450  , 16.9496
0      , 1500  , 18.0462
0      , 1550  , 19.177
0      , 1600  , 20.342
47.746 , -1600 , 11.6734
47.746 , -1550 , 10.7802
...
47.746 , -100  , -0.19622
47.746 , -50   , -0.064626
47.746 , 0     , 0.1449
...
~~~

