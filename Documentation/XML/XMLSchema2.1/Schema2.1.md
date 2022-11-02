XML Schema 2.1
===============

Schema version 2.1 includes all changes introduced with the first amendment of EU Regulation 2017/2400. The following parameters are mandatory:

  * ZeroEmissionVehicle
  * VocationalVehicle
  * SleeperCab
  * NgTankSystem (only for vehicles with natural gas as fuel type)
  * ADAS Parameters (advanced driver assistant system): Engine Stop/Start, EcoRoll without engine stop, EcoRoll with engine stop, Predictive CruiseControl

Introduction of a new Vehicle type: Exempted vehicles. No VECTO simulation is performed for such vehicles.

##VehicleDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1

*Base Type:* v2.0:AbstractVehicleDeclarationType

![](XMLSchema2.1/VehicleDeclarationType.png)

##ExemptedVehicleDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1

*Base Type:* v2.0:AbstractVehicleDeclarationType

![](XMLSchema2.1/ExemptedVehicleDeclarationType.png)

##AdvancedDriverAssistantSystemsType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1

*Base Type:* v2.1:AbstractAdvancedDriverAssistantSystemsType

![](XMLSchema2.1/AdvancedDriverAssistantSystemsType.png)

##EngineDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1

*Base Type:* v1.0:AbstractCombustionEngineDataDeclarationType

![](XMLSchema2.1/EngineDataDeclarationType.png)


