## Vair & Beta Cross Wind Correction Input File (.vcdb)

The file is needed for Vair & Beta [Cross Wind Correction](#vehicle-cross-wind-correction). The file uses the [VECTO CSV format](#csv).

- Filetype: .vcdb
- Header: **beta [°], delta CdA [m^2]**
    + **beta [°]**: the wind yaw angle (0° means that the wind comes directly from the front)
    + **delta CdA [m^2]**: The ΔC~d~A value which will be added to the base C~d~A value.
- Requires at least 2 data entries

**Example:**

~~~
beta [°],delta CdA [m^2]
0       ,0.00
1       ,0.07
2       ,0.21
3       ,0.40
4       ,0.64
5       ,0.90
6       ,1.19
7       ,1.48
8       ,1.76
9       ,2.02
10      ,2.25
20      ,3.14
40      ,3.24
60      ,-0.76
80      ,-4.76
100     ,-9.01
120     ,-15.01
140     ,-21.01
160     ,-20.86
180     ,-16.15
~~~
