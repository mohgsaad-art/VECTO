XML Schema 2.3 DEV
===============

  * Introduction of a new model parameter for ADAS in combination with AT transmisisons: ATEcoRollReleaseLockupClutch
  * New Engine component data: 
      + Adding model parameters for WHR systems: WHR Type, electrical and/or mechanical WHR Power
      + Adding support for dual fuel engines, adding support for dual mode engines
  * New gearbox component data: adding parameter whether Differential (axlegear) is included in the loss-map (required for FWD vehicles)


##AdvancedDriverAssistantSystemsType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3

*Base Type:* v2.1:AbstractAdvancedDriverAssistantSystemsType

![](XMLSchema2.3_DEV/AdvancedDriverAssistantSystemsType.png)

##EngineDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3

*Base Type:* v1.0:AbstractCombustionEngineDataDeclarationType

![](XMLSchema2.3_DEV/EngineDataDeclarationType.png)

##TyreDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:DEV:v2.3

*Base Type:* v1.0:AbstractTyreDataDeclarationType

![](XMLSchema2.3_DEV/TyreDataDeclarationType.png)
