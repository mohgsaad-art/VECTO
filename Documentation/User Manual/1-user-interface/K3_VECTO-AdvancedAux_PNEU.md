## Pneumatic Auxiliaries Editor
 

![](pics/AA_Pneumatics.jpg)
 
### Description

The "Pneumatics" tab defines various parameters for pneumatic auxiliaries used on the vehicle:

-   Pneumatic Auxiliaries Data/Variables
    : Data for various pneumatic auxiliaries and the relevant pneumatic variables can be edited in the adjacent text boxes.
-   Filepath to the Compressor Map (.ACMP) file
    : Files can be imported by clicking the browse button adjacent to the “Compressor Map” text box.
-   Filepath to the Actuations Map (.APAC) file
    : Files can be imported by clicking the browse button adjacent to the “Actuations Map” text box.
-   The “Retarder Brake”, “Smart Pneumatics” and “Smart Regeneration” and enable via check boxes.


### Default Values

The following table provides a summary of the default values that are populated whenever a new advanced auxiliaries (.AAUX) file is created from scratch.  The table also indicates the editable/default status of the relevant parameters in the VECTO UI in Engineering mode, and the recommended status in Declaration mode (not currently implemented).  The default values / parameter status has been agreed with the project steering group.

**Default parameter values and editable status for the Pneumatic module**

*Pneumatic Auxiliaries Data*

| Category | Default value | Engineering | Declaration | Comments | 
|----------|---------------|-------------|-------------|----------|
| AdBlue NI per minute | 21.25 | Open/editable | Locked default | Only relevant for Pneumatic AdBlue Dosing, also needs drive cycle duration | 
| Air Controlled Suspension NI/Minute | 15 | Open/editable | Locked default | Only relevant for Pneumatic Air Suspension Control, also needs drive cycle duration | 
| Breaking No Retarder NI/KG | 0.00081 | Open/editable | Locked default | also needs vehicle weight | 
| Braking with Retarder NI/KG | 0.0006 | Open/editable | Locked default | Also needs vehicle weight | 
| Air demand per Kneeling NI/Kg mm | 0.000066 | Open/editable | Locked default | Also needs vehicle weight and kneeling height | 
| Dead Vol Blowouts/L/Hour | 24 | Open/editable | Locked default |   | 
| Dead Volume Litres | 30 | Open/editable | Locked default |   | 
| Non Smart Regen Fraction Total Air Demand | 0.26 | Open/editable | Locked default |   | 
| Overrun Utilisation for Compression Fraction | 0.97 | Open/editable | Locked default | Taken directly from White Book | 
| Per Door Opening NI | 12.7 | Open/editable | Locked default | Only relevant for Pneumatic Door Operation, also needs number of door openings | 
| Per Stop Brake Actuation NI/KG | 0.00064 | Open/editable | Locked default | Also needs vehicle weight | 
| Smart Regen Fraction Total Air Demand | 0.12 | Open/editable | Locked default  |

 
 
*Pneumatic Variables*

| Category | Default value |   Engineering | Declaration | Comments |
|----------|---------------|-------------|-------------|----------|
| Compressor Map | <blank> | Open/editable | Locked default | A number of pre-set defaults will be provided; later value from test procedure. | 
| Compressor Gear Ratio | 1.00 | Open/editable | Open/OEM data | Related compressor shaft speed to engine shaft speed |
| Compressor Gear Efficiency | 0.97 | Open/editable | Locked default |   | 
| AdBlue Dosing | Pneumatic | Open/editable | Open/OEM data | Pneumatic (/Electric) | 
| Air Suspension Control | Mechanically | Open/editable | Open/OEM data | Mechanically (/Electrically) | 
| Door Operation | Pneumatic | Open/editable | Open/OEM data | Pneumatic (/Electric) | 
| Kneeling height millimeters | 70 | Open/editable | Open/OEM data | Used with air demand per kneeling | 
| Actuations Map | testPneumatic | ActuationsMap | Open/editable | Locked default | Determined by passenger stops | 
| Retarder brake | Yes | Open/editable | Open/OEM data | Yes (/No) | 
| Smart Pneumatics | No | Open/editable | Open/OEM data | No (/Yes) | 
| Smart Regeneration | No | Open/editable | Open/OEM data | No (/Yes)  |

