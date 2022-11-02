## Battery Internal Voltage File (.vbatv)

This file contains the battery's internal voltage as function of the state of charge (SoC). The file must cover the SOC range from 0 to 100%! The file uses the [VECTO CSV format](#csv).

- Filetype: .vbatv
- Header: **SOC, V**
- Requires at least 2 data entries


**Example:**

~~~
SOC , V
0   , 590
10  , 614
20  , 626
30  , 634
40  , 638
50  , 640
60  , 640
70  , 642
80  , 646
90  , 650
100 , 658
~~~


## Battery Internal Resistance File (.vbatr)

This file contains the battery's internal resistance as function of the state of charge (SoC). The file must cover the SOC range from 0 to 100%! The file uses the [VECTO CSV format](#csv).

- Filetype: .vbatr
- Header: **SOC, Ri** in case of pulse independent internal resistance
- Header: **SoC, Ri-2, Ri-10, Ri-20** *,Ri-120*
- Requires at least 2 data entries


**Example:**

~~~
SoC , Ri
0   , 0.04
100 , 0.04
~~~

~~~
SoC , Ri-2 , Ri-10 , Ri-20
0   , 0.04 , 0.06  , 0.08
100 , 0.04 , 0.06  , 0.08
~~~

## Battery Max Current Map (.vimax)

This file contains the battery's maximum current for charging and discharging depending on the state of charge (SoC). The file must cover the SOC range from 0 to 100%! The values for both, the charging and discharging current need to be positive.

The file uses the [VECTO CSV format](#csv).

- Filetype: .vbatr
- Header: **SOC, I_charge, I_discharge**
- Requires at least 2 data entries

**Example:**

~~~
SOC , I_charge , I_discharge
0   , 1620     , 1620
100 , 1620     , 1620
~~~