## PTO Idle Consumption Map (.vptoi)

The pto idle consumption map defines the speed-dependent power demand when the pto cycle is not active. This is only be used in [Engineering Mode](#engineering-mode) when a pto transmission is defined.
The exact demand is interpolated based on the engine speed. PTO consumer idling losses are added to engine loads during any parts of the vehicle operation except the "PTO cycle". It can be defined in the [Vehicle-File](#vehicle-file-.vveh) and set via the [Vehicle-Editor](#vehicle-editor-pto-tab).
The basic file format is [Vecto-CSV](#csv) and the file type ending is ".vptoi".

Header: **\<Engine speed>, \<PTO Torque>**

**Bold columns** are mandatory. Only the listed columns are allowed (no other columns!).<br />
The order is not important when the headers are annotated with \<angle-brackets\> (less-than-sign "<" and greater-than-sign ">").<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|    Identifier    |  Unit |                                                                 Description                                                                 |
|------------------|-------|---------------------------------------------------------------------------------------------------------------------------------------------|
| **Engine speed** | [rpm] | The engine speed.                                                                                                                           |
| **PTO Torque**   | [Nm]  | Torque Loss by the PTO consumer (including prop-shaft losses if applicable) as measured by the DIN test converted to torque at engine speed |

**Example:**

~~~
<Engine speed> [rpm], <PTO Torque> [Nm]
600                   ,  8.0027
800                   , 12.2902
1000                  , 16.7431
1200                  , 20.3244
1400                  , 26.4444
1600                  , 32.1234
~~~
