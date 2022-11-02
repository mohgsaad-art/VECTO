## CSV
Many data files in Vecto use CSV (Comma Separated Values) as common file format. They consist of a header which defines the columns and data entries which are separated by a comma (",").

In Vecto 3 the order of the columns is arbitrary if the column header matches the header definitions described in this user manual. If the column header does not match, a warning is written to the log file and the columns are parsed in the sequence as described in this manual as a fall-back.


### Definition###


|                         |                                                                                                                                                                                                                                                                                                                                                                                                    |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Header:**             | Vecto CSV needs exactly one header line with the definition of the columns at the beginning of the file. <br>Columns can be surrounded with "\<" and "\>" to mark them as identifiers (which makes them position independent). In Vecto 3.x every column is seen as identifier, regardless of "\<\>". <br>Columns may be succeded with unit information (enclosed in "[" and "]") for documentation purposes. |
| **Column Separator:**   | **,** (Comma. Separates the columns of a data line.)                                                                                                                                                                                                                                                                                                                                               |
| **Decimal-Mark:**       | **.** (Dot. Splits numbers into integer part and decimal part.)                                                                                                                                                                                                                                                                                                                                    |
| **Thousand-Separator:** | Vecto CSV does not allow a thousand-separator.                                                                                                                                                                                                                                                                                                                                                     |
| **Comments:**           | **#** (Number sign. Declares text coming afterwards in the current line as comment.)                                                                                                                                                                                                                                                                                                               |
| **Whitespace:**         | Whitespaces between columns will be stripped away. Therefore it is possible to align the columns for better readability, if desired.                                                                                                                                                                                                                                                               |


**Note:** All column headers are case insensitive.

**Note:** Unit information in the column header (enclosed in "[" and "]") are only information for the user. Vecto does **not** read the unit string nor convert between units. The values are expected to be in the units as specified in the user manual. 


Following files use the csv:

- [Speed Dependent Cross Wind Correction Input File (.vcdv)](#speed-dependent-cross-wind-correction-input-file-.vcdv)
- [Vair & Beta Cross Wind Correction Input File (.vcdb)](#vair-beta-cross-wind-correction-input-file-.vcdb)
- [Retarder Loss Torque Input File (.vrlm)](#retarder-loss-torque-input-file-.vrlm)
- [Full Load and Drag Curves (.vfld)](#full-load-and-drag-curves-.vfld)
- [Fuel Consumption Map (.vmap)](#fuel-consumption-map-.vmap)
- [Shift Polygons Input File (.vgbs)](#shift-polygons-input-file-.vgbs)
- [Transmission Loss Map (.vtlm)](#transmission-loss-map-.vtlm)
- [Torque Converter Characteristics (.vtcc)](#torque-converter-characteristics-.vtcc)
- [Driving Cycles (.vdri)](#driving-cycles-.vdri)
- [Acceleration Limiting Input File (.vacc)](#acceleration-limiting-input-file-.vacc)
- [Modal Results (.vmod)](#modal-results-.vmod)
- [Summary Results (.vsum)](#summary-results-.vsum)

### Examples###

#### Exampl 1: Acceleration Limiting File####
~~~
v [km/h],acc [m/s^2]     ,dec [m/s^2]
0       ,1.01570922360353,-0.231742702878269
5       ,1.38546581120225,-0.45346198022574
10      ,1.34993329755465,-0.565404125020508
15      ,1.29026714002479,-0.703434814668512
~~~

#### Example 2: Driving Cycle####
~~~
<s>,<v>,<grad>      ,<stop>,<Padd>,<Aux_ALT1>,<Aux_ALT2>,<Aux_ALT3>
0  ,0  ,-0.020237973,2     ,6.1   ,0.25      ,0.25      ,0.25
1  ,64 ,-0.020237973,0     ,6.1   ,0.25      ,0.25      ,0.25
2  ,64 ,-0.020237973,0     ,6.1   ,0.25      ,0.25      ,0.25
3  ,64 ,-0.020237973,0     ,6.1   ,0.25      ,0.25      ,0.25
~~~

#### Example 3: Transmission Loss Map####
~~~
Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]
0                ,-2500            ,77.5
0                ,-1500            ,62.5
0                ,-500             ,47.5
0                ,500              ,47.5
~~~
