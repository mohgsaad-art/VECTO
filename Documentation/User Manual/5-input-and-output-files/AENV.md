## Environmental Conditions Batch Input File (.aenv)
 
This file contains data on number of different environmental/climatic conditions that can be run through the HVAC SSM module when it is in batch-mode to generate a weighted average output for HVAC power and fuelling loads. 


### File Format

The file uses the VECTO CSV format, with an example provided below, with the default values based on the methodology agreed with the European Commission and the project Steering Group.

### Format

**Default Climatic Conditions input file:**

~~~
ID, EnvTemp, Solar, WeightingFactor
1, -20, 10, 0.0053
2, -5, 30, 0.0826
3, 2, 30, 0.0826
4, 8, 20, 0.1661
5, 8, 155, 0.0826
6, 14, 30, 0.0826
7, 14, 175, 0.1243
8, 20.5, 30, 0.1243
9, 20.5, 200, 0.1243
10, 26, 150, 0.0826
11, 33, 150, 0.0427
~~~

