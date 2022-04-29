## Electrical Auxiliaries Editor
 
![](pics/AA_Electrics.jpg)
 
### Description

The "Electrics" tab defines various parameters for electric auxiliaries used on the vehicle:

-   Powernet Voltage [locked field/fixed value]
-   Alternator Map, including filepath to the new Combined Alternator Map (.AALT) file
    : Files can be imported (blank field)/the [Combined Alternator Map editor](#combined-alternator-map-file-.aalt) opened (file present) by clicking on the ‘browse’ button adjacent to the “Alternator Map” text box.
-   Alternator Pulley Efficiency [locked field/fixed value]
-   Door Actuation Time(S) [locked field/fixed value]
-   Stored Energy Efficiency [locked field/fixed value]
-   Smart Electrics [On/Off]
    : Smart electrics are enabled by checking the "Smart Electrics" box
-   Electrical Consumables
    : The "Electrical Consumables" table contains a list electrical equipment that place demand on the engine. Check boxes enable the user to select whether the energy demanded by each consumable is included in the calculation of the base vehicle. The user can modify only the number of consumables of each type installed on the vehicle*.  The Nominal Consumption (amps) for each consumer, and the percentage of time each consumer is active during the cycle are locked default values as agreed with the project steering group.

* Note: for certain fields the allowable values are also controlled/prescribed according to the requirements of the project steering group.

### Results Cards

Upon activation of Smart Electrics using the check box, the user may enter Result Card values according to the methodology proposed by the steering group.  Until the certification procedure to determine the correct values is agreed, it is recommended to use the following default values:

Example Default Results Card values

| Amps | SmartAmps   |
|------|-------------|
| 40   | 0           |
| 50   | 0           |
| 60   | 54          |
| 70   | 64          |
| 80   | 30          | 

  : Result Card: Idle


| Amps  | SmartAmps       |
|-------|-----------------|
| 40    | 0               |
| 50    | 0               |
| 60    | 83              |
| 70    | 94              |
| 80    | 45              |

  : Result Card: TractionON


| Amps  |  SmartAmps   |
|-------|--------------|
| 40    | 0            |
| 50    | 0            |
| 60    | 172          |
| 70    | 182          |
| 80    | 90           |    

  : Result Card: Overrun

### Default Values

The following table provides a summary of the default values that are populated whenever a new advanced auxiliaries (.AAUX) file is created from scratch (nominal consumption and % active are always fixed defaults, so are not shown).  The table also indicates the editable/default status of the relevant parameters in the VECTO UI in Engineering mode, and the recommended status in Declaration mode (not currently implemented).  The default values / parameter status has been agreed with the project steering group.

*Default parameter values and editable status for the Electrical module*

*General Inputs*
 
| Category                     | Name                         | Default value    | Engineering    | Declaration    |
|------------------------------|------------------------------|------------------|----------------|----------------|
| Powernet Voltage             | Powernet Voltage             |             28.3 | Locked default | Locked default |
| Alternator Map               | Alternator Map               |            blank | Open/editable  | Open/OEM data  |
| Alternator Pulley Efficiency | Alternator Pulley Efficiency |             0.92 | Locked default | Locked default |
| Door Actuation Time (s)      | Door Actuation Time (s)      |              4.0 | Locked default | Locked default |
| Smart Electrics              | Smart Electrics              |        No (/Yes) | Open/editable  | Open/OEM data  |


*List of Electrical Consumables*
 


| Category                     | Name                | No. in Vehicle, Default Value    | Engineering    | Declaration    |
|------------------------------|-----------------------------------------------------------|------------------|----------------|----------------|
| Doors                        | Doors per vehicle                                         |                3 | Open/editable  | Open/OEM data  |
| Veh Electronics &Engine      | Controllers, Valves, etc                                  |                1 | Locked default | Locked default |
| Vehicle basic equipment      | Radio City                                                |                1 | Open/editable  | Open/OEM data  |
| Vehicle basic equipment      | Radio Intercity                                           |                0 | Open/editable  | Open/OEM data  |
| Vehicle basic equipment      | Radio/Audio Tourism                                       |                0 | Open/editable  | Open/OEM data  |
| Vehicle basic equipment      | Fridge                                                    |                0 | Open/editable  | Open/OEM data  |
| Vehicle basic equipment      | Kitchen Standard                                          |                0 | Open/editable  | Open/OEM data  |
| Vehicle basic equipment      | Interior lights City/ Intercity + Doorlights [1/m]        |               12 | Open/editable  | Locked default |
| Vehicle basic equipment      | LED Interior lights ceiling city/ontercity + door [1/m]   |                0 | Open/editable  | Locked default |
| Vehicle basic equipment      | Interior lights Tourism + reading [1/m]                   |                0 | Open/editable  | Locked default |
| Vehicle basic equipment      | LED Interior lights ceiling Tourism + LED reading [1/m]   |                0 | Open/editable  | Locked default |
| Customer Specific Equipment  | External Displays Font/Side/Rear                          |                4 | Open/editable  | Open/OEM data  |
| Customer Specific Equipment  | Internal display per unit ( front side rear)              |                1 | Open/editable  | Open/OEM data  |
| Customer Specific Equipment  | CityBus Ref EBSF Table4 Devices ITS No Displays           |                1 | Open/editable  | Open/OEM data  |
| Lights                       | Exterior Lights BULB                                      |                1 | Locked default | Locked default |
| Lights                       | Day running lights LED bonus                              |                1 | Open/editable  | Open/OEM data  |
| Lights                       | Antifog rear lights LED bonus                             |                1 | Open/editable  | Open/OEM data  |
| Lights                       | Position lights LED bonus                                 |                1 | Open/editable  | Open/OEM data  |
| Lights                       | Direction lights LED bonus                                |                1 | Open/editable  | Open/OEM data  |
| Lights                       | Brake Lights LED bonus                                    |                1 | Open/editable  | Open/OEM data  |

