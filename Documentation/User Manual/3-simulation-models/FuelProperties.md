## Fuel Properties

| FuelType     | Tanksystem   | FuelDensity [kg/m3]   | CO2 per FuelWeight [kgCo2/kgFuel]   | NCV_stdEngine [kJ/kg]   | NCV_stdVecto [kJ/kg]   | Note             |
| ------------ | ------------ | --------------------- | ----------------------------------- | ----------------------- | ---------------------- | -------          |
| Diesel CI    |              | 836                   | 3.13                                | 42700                   | 42700                  |                  |
| Ethanol CI   |              | 820                   | 1.81                                | 25700                   | 25400                  |                  |
| Petrol PI    |              | 748                   | 3.04                                | 41500                   | 41500                  |                  |
| Ethanol PI   |              | 786                   | 2.10                                | 29100                   | 29300                  |                  |
| LPG PI       |              |                       | 3.02                                | 46000                   | 46000                  |                  |
| NG PI        | compressed   |                       | 2.69                                | 45100                   | 48000                  | H-Gas            |
| NG PI        | liquefied    |                       | 2.77                                | 45100                   | 49100                  | EU mix 2016/2030 |

Specifications are based on an analysis (2018) performed by CONCAWE/EUCAR and shall reflect typical fuel on the European market. The data was in the context of the study:
Well-To-Wheels Analysis Of Future Automotive Fuels And Powertrains in the European Context – Heavy Duty vehicles ([doi:10.2760/100379](http://dx.doi.org/10.2760/100379))

### VECTO Input for CNG/LNG Vehicles

Currently only the fuel type 'NG PI' for the engine certification is allowed according to Regulation (EU) 2017/2400. For LNG vehicles, therefore, the engine fuel type has to be set to 'NG PI' and at the vehicle level NgTankSystem has to be set to 'liquefied'. For CNG vehicles the same engine fuel type is provided but NgTankSystem has to be set to 'compressed'.
