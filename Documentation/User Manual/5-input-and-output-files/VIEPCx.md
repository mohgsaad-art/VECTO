## IEPC Max Torque File (.viepcp)

This file contains the IEPC's maximum drive torque and maximum recuperation torque depending on the motor's angluar speed. The file uses the [VECTO CSV format](#csv).

- Filetype: .viepcp
- Header: **n_out [rpm] , T_drive_out [Nm] , T_recuperation_out [Nm]**
- Requires at least 2 data entries

**Example:**

~~~
n_out   , T_drive_out , T_recuperation_out
0.00    , 4027.80     , -4193.88
14.96   , 4027.80     , -4193.88
151.09  , 4027.80     , -4193.88
302.19  , 4027.80     , -4193.88
452.92  , 4027.80     , -4193.88
604.01  , 4027.80     , -4193.88
755.11  , 4027.80     , -4193.88
906.20  , 3356.50     , -3494.90
1057.30 , 2876.98     , -2995.60
1208.03 , 2517.38     , -2621.17
1359.12 , 2237.68     , -2329.95
1510.22 , 2013.90     , -2096.94
...
~~~

## IEPC Drag Curve File (.viepcd)

This file contains the IEPC's drag torque (i.e. the eletric motor is not energized) depending on the motor's angluar speed. The file uses the [VECTO CSV format](#csv).

- Filetype: .viepcd
- Header: **n_out [rpm] , T_drag_out [Nm]**
- Requires at least 2 data entries

**Example:**

~~~
n_out   , T_drag_out
0.00    , -8.02
15.10   , -8.28
151.02  , -10.62
302.05  , -13.22
453.07  , -15.82
604.09  , -18.45
755.11  , -21.05
906.14  , -23.65
1057.16 , -26.25
1208.18 , -28.85
...
~~~


## IEPC Power Map (.viepco)

This file is used to interpolate the electric power required for a certain mechanical power at the IEPC's output shaft.  The file uses the [VECTO CSV format](#csv).

- Filetype: .viepco
- Header: **n_out [rpm] , T_out [Nm] , P_el [kW]**
- Requires at least 2 data entries

**Example:**

~~~
n [rpm], T [Nm], P_el [kW]
0.00   , -12984.38 , 0.00
0.00   , -12335.16 , 0.00
0.00   , -11685.94 , 0.00
0.00   , -11036.72 , 0.00
...
0.00   , 10171.44  , 11.99
0.00   , 10769.76  , 12.87
0.00   , 11368.08  , 13.76
0.00   , 11966.40  , 14.67
4.98   , -12984.38 , 0.00
4.98   , -12335.16 , 0.00
4.98   , -11685.94 , 0.00
...
995.92 , 10171.44  , 1271.27
995.92 , 10769.76  , 1347.52
995.92 , 11368.08  , 1424.25
995.92 , 11966.40  , 1501.45
...
~~~


