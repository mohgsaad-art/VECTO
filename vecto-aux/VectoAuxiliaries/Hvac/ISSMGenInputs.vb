
Namespace Hvac

  Public Interface ISSMGenInputs
  
    'Bus Parameterisation	
    Property BP_BusModel 	                                     As String	
    Property BP_NumberOfPassengers		                         As Double	
    Property BP_BusFloorType	                                 As string
    Readonly Property BP_BusFloorSurfaceArea                     As Double
    Property BP_BusSurfaceAreaM2                                 As Double
    Readonly Property BP_BusWindowSurface                        As Double
    Property BP_BusVolume                                        As Double                 
    Property BP_BusLength                                        As Double
    Property BP_BusWidth                                         As Double
    					
    'Boundary Conditions:			
             Property BC_GFactor				                 As Double
    Readonly Property BC_SolarClouding				             As Double
    Readonly Property BC_HeatPerPassengerIntoCabinW	             As Double
             Property BC_PassengerBoundaryTemperature            As Double
             Property BC_PassengerDensityLowFloor                As Double
             Property BC_PassengerDensitySemiLowFloor	         As Double
             Property BC_PassengerDensityRaisedFloor	         As Double
    Readonly Property BC_CalculatedPassengerNumber			     As Double
    Readonly Property BC_UValues                                 As Double
             Property BC_HeatingBoundaryTemperature	             As Double
             Property BC_CoolingBoundaryTemperature              As Double
             Property BC_HighVentilation                         As Double
             Property BC_lowVentilation	                         As Double
    Readonly Property BC_High                                    As Double
    Readonly Property BC_Low	                                 As Double
    Readonly Property BC_HighVentPowerW                          As Double
    Readonly Property BC_LowVentPowerW                           As Double
             Property BC_SpecificVentilationPower                As Double
             Property BC_COP			                         As Double
             Property BC_AuxHeaterEfficiency		             As Double
             Property BC_GCVDieselOrHeatingOil                   As Double
             Property BC_VolumicMassDieselOrHeatingOil	         As Double
    Readonly Property BC_WindowAreaPerUnitBusLength	             As Double
    Readonly Property BC_FrontRearWindowArea                     As Double
             Property BC_MaxTemperatureDeltaForLowFloorBusses    As Double
             Property BC_MaxPossibleBenefitFromTechnologyList	 As Double
    					
    'EnviromentalConditions				
    Property EC_EnviromentalTemperature                          As Double
    Property EC_Solar   	                                     As Double
    					                                         
    'AC-system				                                     
    Property AC_InCabinRoomAC_System	                         As Boolean
    Property AC_CompressorType			                         As String
    Property AC_CompressorCapacitykW	                         As Double
    					
    'Ventilation				
    Property VEN_VentilationOnDuringHeating				         As Boolean  
    Property VEN_VentilationWhenBothHeatingAndACInactive		 As Boolean
    Property VEN_VentilationDuringAC			                 As Boolean
    Property VEN_VentilationFlowSettingWhenHeatingAndACInactive	 As String
    Property VEN_VentilationDuringHeating			             As String
    Property VEN_VentilationDuringCooling				         As String
    					
    'Aux. Heater				
    Property AH_EngineWasteHeatkW	                             As Double
    Property AH_FuelFiredHeaterkW                                As Double
  
  
  End Interface


End Namespace


