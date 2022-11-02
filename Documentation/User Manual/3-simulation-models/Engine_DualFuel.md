## Dual Fuel Engine

VECTO supports to simulate vehicles equipped with dual-fuel engines, i.e. two different fuels are used simulateously. Therefore, the engine model contains a second fuel comsumption map and VECTO interpolates the fuel consumtion from both consumption maps. In the .vmod and .vsum files the consumption of every fuel is reported. The CO2 emissions are te sum of CO2 emissions from both fuels.

In case a WHR system is used with a dual-fuel vehicle the WHR map shall be provided in the fuel consumption map of the primary fuel.