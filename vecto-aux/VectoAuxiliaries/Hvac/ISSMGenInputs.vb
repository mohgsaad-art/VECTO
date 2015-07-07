
Namespace Hvac

    Public Interface ISSMGenInputs

        'Bus Parameterisation	
        Property BP_BusModel As String
        Property BP_NumberOfPassengers As Double
        Property BP_BusFloorType As String
        Property BP_DoubleDecker As Boolean
        Property BP_BusLength As Double
        Property BP_BusWidth As Double
        Property BP_BusHeight As Double

        ReadOnly Property BP_BusFloorSurfaceArea As Double
        ReadOnly Property BP_BusWindowSurface As Double
        ReadOnly Property BP_BusSurfaceAreaM2 As Double
        ReadOnly Property BP_BusVolume As Double

        'Boundary Conditions:			
        Property BC_GFactor As Double
        ReadOnly Property BC_SolarClouding As Double
        ReadOnly Property BC_HeatPerPassengerIntoCabinW As Double
        Property BC_PassengerBoundaryTemperature As Double
        ReadOnly Property BC_PassengerDensityLowFloor As Double
        ReadOnly Property BC_PassengerDensitySemiLowFloor As Double
        ReadOnly Property BC_PassengerDensityRaisedFloor As Double
        ReadOnly Property BC_CalculatedPassengerNumber As Double
        ReadOnly Property BC_UValues As Double
        Property BC_HeatingBoundaryTemperature As Double
        Property BC_CoolingBoundaryTemperature As Double
        ReadOnly Property BC_TemperatureCoolingTurnsOff As Double
        Property BC_HighVentilation As Double
        Property BC_lowVentilation As Double
        ReadOnly Property BC_High As Double
        ReadOnly Property BC_Low As Double
        ReadOnly Property BC_HighVentPowerW As Double
        ReadOnly Property BC_LowVentPowerW As Double
        Property BC_SpecificVentilationPower As Double
        Property BC_AuxHeaterEfficiency As Double
        Property BC_GCVDieselOrHeatingOil As Double
        ReadOnly Property BC_WindowAreaPerUnitBusLength As Double
        ReadOnly Property BC_FrontRearWindowArea As Double
        Property BC_MaxTemperatureDeltaForLowFloorBusses As Double
        Property BC_MaxPossibleBenefitFromTechnologyList As Double

        'EnviromentalConditions				
        Property EC_EnviromentalTemperature As Double
        Property EC_Solar As Double
        ReadOnly Property EC_EnvironmentalConditionsMap As IEnvironmentalConditionsMap
        Property EC_EnviromentalConditions_BatchFile As String
        Property EC_EnviromentalConditions_BatchEnabled As Boolean

        'AC-system				            
        Property AC_CompressorType As String
        ReadOnly Property AC_CompressorTypeDerived As String
        Property AC_CompressorCapacitykW As Double
        ReadOnly Property AC_COP As Double

        'Ventilation				
        Property VEN_VentilationOnDuringHeating As Boolean
        Property VEN_VentilationWhenBothHeatingAndACInactive As Boolean
        Property VEN_VentilationDuringAC As Boolean
        Property VEN_VentilationFlowSettingWhenHeatingAndACInactive As String
        Property VEN_VentilationDuringHeating As String
        Property VEN_VentilationDuringCooling As String

        'Aux. Heater				
        Property AH_EngineWasteHeatkW As Double
        Property AH_FuelFiredHeaterkW As Double

    End Interface

End Namespace


