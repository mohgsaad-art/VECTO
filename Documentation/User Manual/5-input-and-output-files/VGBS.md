## Shift Polygons Input File (.vgbs)

Defines up- and down-shift curves. See [Gear Shift Model](#gear-shift-model) for details. The file uses the [VECTO CSV format](#csv).

- Filetype: .vgbs
- Header: **engine torque [Nm], downshift rpm [1/min], upshift rpm [1/min]**
- Requires at least 2 data entries

**Example:**

~~~
engine torque [Nm],downshift rpm [1/min],upshift rpm [1/min]
-400              ,560                  ,1289
759               ,560                  ,1289
1252              ,742                  ,1289
2372              ,1155                 ,1942
...
~~~


**Example Graph:**

![A typical shift curve.](pics/Shift.svg)


