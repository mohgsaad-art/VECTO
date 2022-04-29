## Auxiliary Input File (.vaux)
This file is used to configure a single auxiliary. Multiple .vaux files can be defined in the [Job File](#job-file) via the [Auxiliary Dialog](#auxiliary-dialog). The file uses the [VECTO CSV format](#csv) with three additional parameters on top of the efficiency map.

See [Auxiliaries](#auxiliaries) for details on how the power demand for each auxiliary is calculated.

- Filetype: .vaux
- Multiple Header must exist in the following sequence:
    - **Transmission ratio to engine rpm \[-\]**: Speed ratio between auxiliary and engine. Followed by just one value line.
    - **Efficiency to engine \[-\]**: Efficiency of auxiliary (belt/gear) drive. Followed by just one value line.
    - **Efficiency auxiliary to supply \[-\]**: Consumer efficiency. Followed by just one value line.
    - **Auxiliary speed [rpm], Mechanical power [kW], Supply power [kW]**
        + Actual map entries
- Requires exact one value for the first 3 headers, and at least 3 data lines for the map entries.

**Example:**

~~~
Transmission ratio to engine rpm [-]
4.078
Efficiency to engine [-]
0.96
Efficiency auxiliary to supply [-]
1
Auxiliary speed [rpm],Mechanical power [kW],Supply power [kW]
1415                 ,0.07                 ,0
1415                 ,0.87                 ,0.53
1415                 ,1.03                 ,0.64
1415                 ,1.17                 ,0.75
...
~~~
