XML Schema 2.0
===============

Updated and more flexible XML schema for VECTO declaration input. This schema allows to

  * update the XML schema for single components
  * use component data certified with XML schema 1.0
  * use component data certified with XML schema 2.x

The XML structure for the vehicle data references abstract XML base types. Component data is derived from the abstreact base type and thus can be used whereever the abstract base type is required.

XML Schema 2.0 contains the same structure as schema version 1.0 before all updates applied to version 1.0, i.e., the input parameters *VocationalVehicle*, *SleeperCab*, *ZeroEmissionVehicle*, *ADAS*, *NgTankSystem* are not allowed. 

##VectoInputDeclaration

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0

![](XMLSchema2.0/VectoInputDeclaration.png)

##VectoInputComponent

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.0

![](XMLSchema2.0/VectoInputComponent.png)

##VehicleDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:VehicleBaseType

*Base Type:* v2.0:AbstractVehicleDeclarationType

![](XMLSchema2.0/VehicleDeclarationType.png)

##PTOType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:AbstractPTODataType

![](XMLSchema2.0/PTOType.png)

##VehicleComponentsType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractVehicleComponentsDeclarationType

![](XMLSchema2.0/VehicleComponentsType.png)

##EngineDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractCombustionEngineDataDeclarationType

![](XMLSchema2.0/EngineDataDeclarationType.png)

##GearboxDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractTransmissionDataDeclarationType

![](XMLSchema2.0/GearboxDataDeclarationType.png)

##GearsDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:AbstractGearsDeclarationType

![](XMLSchema2.0/GearsDeclarationType.png)

##GearDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:GearBaseType

![](XMLSchema2.0/GearDeclarationType.png)

##TorqueConverterDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractTorqueConverterDataDeclarationType

![](XMLSchema2.0/TorqueConverterDataDeclarationType.png)

##AngledriveDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractAngledriveDataDeclarationType

![](XMLSchema2.0/AngledriveDataDeclarationType.png)

##RetarderDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractRetarderDataDeclarationType

![](XMLSchema2.0/RetarderDataDeclarationType.png)

##AxlegearDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractAxlegearDataDeclarationType

![](XMLSchema2.0/AxlegearDataDeclarationType.png)

##AxleWheelsDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:AbstractAxleWheelsDataDeclarationType

![](XMLSchema2.0/AxleWheelsDataDeclarationType.png)

##AxleDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:AbstractAxleDataDeclarationType

![](XMLSchema2.0/AxleDataDeclarationType.png)

##TyreDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractTyreDataDeclarationType

![](XMLSchema2.0/TyreDataDeclarationType.png)

##AuxiliariesDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v2.0:AbstractAuxiliariesDataDeclarationType

![](XMLSchema2.0/AuxiliariesDataDeclarationType.png)

##AirdragDataDeclarationType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* v1.0:AbstractAirdragDataDeclarationType

![](XMLSchema2.0/AirdragDataDeclarationType.png)

##SignatureType

*Namespace:* urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0

*Base Type:* 

![](XMLSchema2.0/SignatureType.png)

