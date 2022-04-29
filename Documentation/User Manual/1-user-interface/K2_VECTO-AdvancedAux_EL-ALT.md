## Combined Alternator Map File (.aalt)

The Combined Alternator Map (.AALT) file contains data relating to the efficiency of the alternator at various engine speeds and current demand. The .AALT file is a CSV file containing three fields: “Amp”, “RPM” (engine speed), and “Efficiency”. It can be created via the select file button, or an existing map directly imported into VECTO via the File Browser.


![](pics/AA_Open-Create AALT.jpg)

A new combined alternator map can be created or an existing one edited using the Combined Alternators editor module (see below).  This module enables the creation of a combined average alternator efficiency map by the advanced auxiliaries module, using input data for one or more alternators (Pully Ratio, Efficiency at different RPM/AMP combinations):

![](pics/AA_Electrics_RC.jpg)

Alternators may be added/deleted from the list. Data for existing alternators can be loaded into the form by double-clicking on the relevant alternator, and the data may then be updated and saved back down.

The 'Diagnostics' tab provides a summary of the input data that is fed into combined alternator map calculations:

![](pics/AALT-Editor_Diagnostics.png)

The methodology for calculating the combined efficiency map is summarised below (and also included in the full schematics file included with the User Manual). Note: A simplified calculation is performed using the average of the user input efficiency values in the model pre-run only, to keep total run-time to a minimum (with negligible impact on the final result).  :

![](pics/CombAltSchem.png)


### File Format

The file uses the VECTO CSV format.

Several example default alternator maps are provided for use until a finalised certification procedure is in place for OEM-specific data.

*Example Default Alternator Configuration for Advanced Alternator Map*

---------------------
Pulley Ratio:  3.6 
---------------------


|         |      |            |      |            |      |          | 
|---------|------|------------|------|------------|------|----------|
| *RPM*     | *2000*      | *2000*  | *4000*      | *4000*  | *6000*      | *6000*  |
|         | **Amps** | **Efficiency** | **Amps** | **Efficiency** | **Amps** | **Efficiency**  |
|         | **10.00** | 62.00 | **10.00** | 64.00 | **10.00** | 53.00 | 
| I_max/2 | 27.00     | 70.00 | 63.00     | 74.00 | 68.00     | 70.00 | 
| I_max   | 53.00     | 30.00 | 125.00    | 68.00 | 136.00    | 62.00 |

Notes: Bold values are locked/fixed values; I_max = the maximum current in Amps.

