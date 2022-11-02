## Advanced Compressor Map (.acmp)


This file is used to configure the compressor map for pneumatic auxiliaries, and contains data relating to the compressor performance at various engine speeds. 

### File Format

The file uses the VECTO CSV format, with an example provided below.

### Format

Example Configuration for Advanced Compressor Map:

~~~
RPM  , FlowRate [l/min] , Power [on] [W] , Power [off] [W]
1500 , 200              , 2000           , 1000
2000 , 400              , 4000           , 2000
3000 , 600              , 6000           , 3000
4000 , 800              , 8000           , 4000
5000 , 1000             , 10000          , 5000
6000 , 1200             , 12000          , 6000
7000 , 1400             , 14000          , 7000
~~~ 

The following four Default maps have been provided for use until a certified test procedure is established:
 
1. DEFAULT_1-Cylinder_1-Stage_393ccm

~~~
rpm  , flowRate [l/min] , power [on] [W] , power [off] [W]
500  , 83.42357042      , 1428           , 181.9
750  , 141.6565216      , 1890           , 342.4
1000 , 198.5612781      , 2467.5         , 513.6
1250 , 241.9965577      , 3097.5         , 716.9
1500 , 293.5664883      , 3759           , 866.7
1750 , 335.5358341      , 4294.5         , 1080.7
2000 , 398.488427       , 5166           , 1273.3
2250 , 425.0944822      , 6006           , 1433.8
2500 , 458.3225806      , 6541.5         , 1540.8
2750 , 478.2312925      , 7066.5         , 1712
3000 , 511.85438        , 7665           , 1958.1
~~~

 
2. DEFAULT_2-Cylinder_1-Stage_650ccm


~~~
rpm  , flowRate [l/min] , power [on] [W] , power [off] [W]
800  , 250.5365596      , 3139.5         , 524.3
1200 , 374.3533986      , 4609.5         , 1027.2
1600 , 508.4123859      , 6205.5         , 1572.9
2000 , 619.1263282      , 7770           , 2065.1
2400 , 762.6185788      , 9723           , 2696.4
2550 , 819.2371476      , 10363.5        , 2856.9
2800 , 898.7501978      , 11613          , 3349.1
3200 , 979.4827586      , 13282.5        , 4012.5
~~~

 
3. DEFAULT_2-Cylinder_2-Stage_398ccm

~~~
rpm  , flowRate [l/min] , power [on] [W] , power [off] [W]
800  , 209.7130243      , 2079           , 160.5
1200 , 348.3681702      , 3160.5         , 342.4
1600 , 411.2603567      , 4315.5         , 604.55
2000 , 520.8333333      , 5901           , 963
2400 , 598.4042553      , 6961.5         , 1433.8
2550 , 618.1318681      , 7360.5         , 1637.1
2800 , 655.1473124      , 8127           , 1968.8
3200 , 806.2234795      , 10043.25       , 2755.25
3600 , 857.9169175      , 11571          , 3702.2
~~~
 
4. DEFAULT_3-Cylinder_2-Stage_598ccm

~~~
rpm  , flowRate [l/min] , power [on] [W] , power [off] [W]
700  , 268.8679245      , 2698.5         , 149.8
1200 , 455.170778       , 4641           , 363.8
1700 , 619.9877948      , 6772.5         , 823.9
2200 , 723.0141287      , 8778           , 1508.7
2550 , 800.5469547      , 10468.5        , 2075.8
2800 , 913.4228898      , 12253.5        , 2461
3300 , 996.5379955      , 14070          , 3145.8
3550 , 1048.442907      , 15078          , 3755.7
~~~