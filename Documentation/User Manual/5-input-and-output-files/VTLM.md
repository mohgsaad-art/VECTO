## Transmission Loss Map (.vtlm)

This file defines losses in transmission components, i.e. every gear, axlegear, angledrive. See [Transmission Losses](#transmission-losses) for the formula how the losses are accounted in the components. The file uses the [VECTO CSV format](#csv).

- Filetype: .vtlm
- Header: **Input Speed [rpm], Input Torque [Nm], Torque Loss [Nm]**
- Requires at least 3 data entries

Input speed and input torque are meant at the engine-side.

**Example:**

~~~
Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]
0                ,-350             ,6.81
0                ,-150             ,5.81
0                ,50               ,5.31
0                ,250              ,6.31
0                ,450              ,7.31
0                ,650              ,8.31
~~~


#### Sign of torque values

* Input Torque >0 means normal driving operation.
* Input Torque \<0 means motoring operation. **The Torque Loss Map must include negative torque values for engine motoring operation!**
* Torque Loss must always be positive!


