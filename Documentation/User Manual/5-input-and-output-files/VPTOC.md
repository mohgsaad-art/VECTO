## PTO Cycle (.vptoc)

The PTO cycle defines the power demands during standing still and doing a pto operation. This can only be used in [Engineering Mode](#engineering-mode) when a pto transmission is defined. It can be set in the [Vehicle-Editor](#vehicle-editor-pto-tab). The basic file format is [Vecto-CSV](#csv) and the file type ending is ".vptoc". A PTO cycle is time-based and may have variable time steps, but it is recommended to use a resolution between 1[Hz] and 2[Hz]. Regardless of starting time, VECTO shifts it to always begin at 0[s].

Header: **\<t>, \<Engine speed>, \<PTO Torque>**

**Bold columns** are mandatory. Only the listed columns are allowed (no other columns!).<br />
The order is not important when the headers are annotated with \<angle-brackets\> (less-than-sign "<" and greater-than-sign ">").<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|    Identifier    |  Unit |                                                                Description                                                                 |
|------------------|-------|--------------------------------------------------------------------------------------------------------------------------------------------|
| **t**            | [s]   | The time during the pto cycle. Must always be increasing. Gets shifted to begin with 0 by VECTO (if thats not already the case).                                                                                  |
| **Engine speed** | [rpm] | Actual engine speed                                                                                                                        |
| **PTO Torque**   | [Nm]  | The torque at the PTO consumer (including prop-shaft losses if applicable) as measured by the DIN test converted to torque at engine speed |

**Example:**

~~~
<t> [s], <Engine speed> [rpm], <PTO Torque> [Nm]
0      , 600                 , 0
1      , 600                 , 0
2      , 900                 , 0
3      , 1200                , 50
4      , 1200                , 70
5      , 1200                , 100
~~~

