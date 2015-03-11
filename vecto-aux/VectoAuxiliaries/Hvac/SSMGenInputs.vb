
Namespace Hvac

  Public Class SSMGenInputs
  Implements ISSMGenInputs

  
  
      'BUS Parameterisation
    '********************
     'C4/D4
     Public Property BP_BusModel As String Implements ISSMGenInputs.BP_BusModel
     'D5/C5
      Public Property BP_NumberOfPassengers As Double Implements ISSMGenInputs.BP_NumberOfPassengers
      'D6/C6
      Public Property BP_BusFloorType As String Implements ISSMGenInputs.BP_BusFloorType
      'D7/C7 - ( M/2 )
      Public ReadOnly Property BP_BusFloorSurfaceArea As Double Implements ISSMGenInputs.BP_BusFloorSurfaceArea
        Get

          '=IF(AND(C6="low floor",C12<=2.55,C12>=2.5),(2.55*(C11-1.2)),((C11-1.2)*C12))
          If BP_BusFloorType="low floor" AndAlso BP_BusWidth<=2.55 AndAlso BP_BusWidth>=2.5 then
            Return Math.Round((2.55*(BP_BusLength-1.2)),6)
          Else       
           Return Math.Round(((BP_BusLength-1.2)*BP_BusWidth),6)
          End If

        End Get
    End Property
      'D8/C8 - ( M/2 )
      Public Property BP_BusSurfaceAreaM2 As Double Implements ISSMGenInputs.BP_BusSurfaceAreaM2
      'D9/C9 - ( M/2 )
      Public ReadOnly Property BP_BusWindowSurface As Double Implements ISSMGenInputs.BP_BusWindowSurface
        Get
        '=(C37*C11)+C38
         Return (BC_WindowAreaPerUnitBusLength*BP_BusLength)+BC_FrontRearWindowArea

        End Get
    End Property
      'D10/C10 - ( M/3 )
      Public Property BP_BusVolume As Double Implements ISSMGenInputs.BP_BusVolume
      'D11/C11 - ( M )
      Public Property BP_BusLength As Double Implements ISSMGenInputs.BP_BusLength
      'D12/C12 - ( M )
      Public Property BP_BusWidth As Double Implements ISSMGenInputs.BP_BusWidth
  
      'BOUNDRY CONDITIONS
    '******************
    'C15
      Public             Property BC_GFactor As Double Implements ISSMGenInputs.BC_GFactor
      'C16               
      Public Readonly    Property BC_SolarClouding	 As Double Implements ISSMGenInputs.BC_SolarClouding
        Get
         '=IF(D6="low floor",0.65,IF(D6="semi low floor",0.8,0.8))
         Return If( BP_BusFloorType="low floor",0.65,If(BP_BusFloorType="semi low floor",0.8,0.8))
        End Get
    End Property
      'C17 -             Watts
      Public  Readonly   Property BC_HeatPerPassengerIntoCabinW As Double Implements ISSMGenInputs.BC_HeatPerPassengerIntoCabinW
        Get
         '=IF(C43>C18,80,50)
         Return If( EC_EnviromentalTemperature>BC_PassengerBoundaryTemperature,80,50)
        End Get
    End Property
      'C18 -             Degrees Centegrade
      Public             Property BC_PassengerBoundaryTemperature As Double Implements ISSMGenInputs.BC_PassengerBoundaryTemperature
      'C19 -             Passenger/Metre Squared
      Public             Property BC_PassengerDensityLowFloor As Double Implements ISSMGenInputs.BC_PassengerDensityLowFloor
      'C20 -             Passenger/Metre Squared
      Public             Property BC_PassengerDensitySemiLowFloor As Double Implements ISSMGenInputs.BC_PassengerDensitySemiLowFloor
      'C21 -             Passenger/Metre Squared
      Public             Property BC_PassengerDensityRaisedFloor As Double Implements ISSMGenInputs.BC_PassengerDensityRaisedFloor
      'C22               
      Public Readonly    Property BC_CalculatedPassengerNumber As Double Implements ISSMGenInputs.BC_CalculatedPassengerNumber
        Get
          '=IF(D6="low floor",C19,IF(D6="semi low floor",C20,C21))*D7
          Return IF(BP_BusFloorType="low floor",BC_PassengerDensityLowFloor,IF(BP_BusFloorType="semi low floor",BC_PassengerDensitySemiLowFloor,BC_PassengerDensityRaisedFloor))*BP_BusFloorSurfaceArea
        End Get
    End Property
      'C23               ( W/K/M3 )
      Public Readonly    Property BC_UValues As Double Implements ISSMGenInputs.BC_UValues
        Get
          '=IF(D6="low floor",4,IF(D6="semi low floor",3.5,3))
          Return IF(BP_BusFloorType="low floor",4,IF(BP_BusFloorType="semi low floor",3.5,3))

        End Get
    End Property
      'C24 -             Degrees Centegrade
      Public             Property BC_HeatingBoundaryTemperature As Double Implements ISSMGenInputs.BC_HeatingBoundaryTemperature
      'C25 -             Degrees Centegrde
      Public             Property BC_CoolingBoundaryTemperature As Double Implements ISSMGenInputs.BC_CoolingBoundaryTemperature
      'C26 -             ( L/H )
      Public             Property BC_HighVentilation As Double Implements ISSMGenInputs.BC_HighVentilation
      'C27 -             ( L/H )
      Public             Property BC_lowVentilation As Double Implements ISSMGenInputs.BC_lowVentilation
      'C28  -            ( M3/H )
      Public  Readonly   Property BC_High As Double Implements ISSMGenInputs.BC_High
        Get
          '=D10*C26
          Return BP_BusVolume * BC_HighVentilation
        End Get
    End Property
      'C29  -            ( M3/H )
      Public Readonly    Property BC_Low As Double Implements ISSMGenInputs.BC_Low
        Get
          '=C27*D10
          Return BP_BusVolume * BC_lowVentilation
        End Get
    End Property
      'C30  -             Watts
      Public Readonly    Property BC_HighVentPowerW As Double Implements ISSMGenInputs.BC_HighVentPowerW
        Get
          '=C28*C32
          Return BC_High * BC_SpecificVentilationPower
        End Get
    End Property
      'C31  -             Watts
      Public Readonly     Property BC_LowVentPowerW As Double Implements ISSMGenInputs.BC_LowVentPowerW
        Get
         '=C29*C32
         Return BC_Low * BC_SpecificVentilationPower
        End Get
    End Property
      'C32  -             ( Wh/M3 )
      Public             Property BC_SpecificVentilationPower As Double Implements ISSMGenInputs.BC_SpecificVentilationPower
      'C33               
      Public             Property BC_COP As Double Implements ISSMGenInputs.BC_COP
      'C34               
      Public             Property BC_AuxHeaterEfficiency As Double Implements ISSMGenInputs.BC_AuxHeaterEfficiency
      'C35 -             ( KW/HKG )
      Public             Property BC_GCVDieselOrHeatingOil As Double Implements ISSMGenInputs.BC_GCVDieselOrHeatingOil
      'C36 -             ( KG/L )
      Public             Property BC_VolumicMassDieselOrHeatingOil As Double Implements ISSMGenInputs.BC_VolumicMassDieselOrHeatingOil
      'C37 -             ( M2/M )
      Public  Readonly   Property BC_WindowAreaPerUnitBusLength As Double Implements ISSMGenInputs.BC_WindowAreaPerUnitBusLength
        Get
          'NOTE:This forumla always returns 1.5
          '=IF(D6="low floor",1.5,IF(D6="semi low floor",1.5,1.5))
          Return IF(BP_BusFloorType="low floor",1.5,IF(BP_BusFloorType="semi low floor",1.5,1.5))
        End Get
    End Property
      'C38 -             ( M/2 )
      Public Readonly    Property BC_FrontRearWindowArea As Double Implements ISSMGenInputs.BC_FrontRearWindowArea
        Get
        'NOTE: This formulae always return 5
        '=IF(D6="low floor",5,IF(D6="semi low floor",5,5))
        Return  If(BP_BusFloorType="low floor" ,5, If(BP_BusFloorType="low floor",5,5))
        End Get
    End Property
      'C39 -             ( K )
      Public             Property BC_MaxTemperatureDeltaForLowFloorBusses As Double Implements ISSMGenInputs.BC_MaxTemperatureDeltaForLowFloorBusses
      'C40 -             ( Fraction )
      Public Property BC_MaxPossibleBenefitFromTechnologyList As Double Implements ISSMGenInputs.BC_MaxPossibleBenefitFromTechnologyList
  
      'Environmental Conditions
    '************************
    'C43 - ( Degrees Centegrade )
      Public Property EC_EnviromentalTemperature As Double Implements ISSMGenInputs.EC_EnviromentalTemperature
      'C44 - ( W/M3 )
      Public Property EC_Solar As Double Implements ISSMGenInputs.EC_Solar
  
  
      'AC SYSTEM
    '*********
    'C47 - Boolean Yes/No
      Public Property AC_InCabinRoomAC_System As Boolean Implements ISSMGenInputs.AC_InCabinRoomAC_System
      'C48 - "mechanical/electrical
      Public Property AC_CompressorType As String Implements ISSMGenInputs.AC_CompressorType
      'C49 -  ( KW )
      Public Property AC_CompressorCapacitykW As Double Implements ISSMGenInputs.AC_CompressorCapacitykW
  
  
      'VENTILATION
    '***********
    'C52 - Boolean Yes/No
      Public Property VEN_VentilationOnDuringHeating				         As Boolean  Implements ISSMGenInputs.VEN_VentilationOnDuringHeating 
      'C53 - Boolean Yes/No
       Property VEN_VentilationWhenBothHeatingAndACInactive		         As Boolean  Implements ISSMGenInputs.VEN_VentilationWhenBothHeatingAndACInactive
      'C54 - Boolean Yes/No
      Public Property VEN_VentilationDuringAC                              As Boolean Implements ISSMGenInputs.VEN_VentilationDuringAC
      'C55 - String high/low
      Public Property VEN_VentilationFlowSettingWhenHeatingAndACInactive   As String Implements ISSMGenInputs.VEN_VentilationFlowSettingWhenHeatingAndACInactive
      'C56 - String high/low
      Property VEN_VentilationDuringHeating			                     As String Implements ISSMGenInputs.VEN_VentilationDuringHeating
      'C57 - String high/low                                               
      Property VEN_VentilationDuringCooling				                 As String Implements ISSMGenInputs.VEN_VentilationDuringCooling
  
  
      'AUX HEATER
    '**********
    'C60 - ( KW )
      Public Property AH_EngineWasteHeatkW As Double Implements ISSMGenInputs.AH_EngineWasteHeatkW
      'C61 - ( KW )
      Public Property AH_FuelFiredHeaterkW As Double Implements ISSMGenInputs.AH_FuelFiredHeaterkW
  
  
  
      Sub new ( optional initialiseDefaults As Boolean = false)


         If initialiseDefaults then SetDefaults



    End Sub
  
      Private Sub SetDefaults()


    'BUS Parameterisation
    '********************
     BP_BusModel = "DummyBus"
     BP_NumberOfPassengers =47R
     BP_BusFloorType ="raised floor"
    'BP_BusFloorSurfaceArea  : Calculated
     BP_BusSurfaceAreaM2 = 114.42325000R
     'BP_BusWindowSurface    : Calculated
     BP_BusVolume =61.81231875R
     BP_BusLength = 10.655R
     BP_BusWidth =2.55R

    'BOUNDRY CONDITIONS
    '******************

     BC_GFactor                                     = 1.0R
    'BC_SolarClouding As Double :Calculated
    'BC_HeatPerPassengerIntoCabinW  :Calculated
     BC_PassengerBoundaryTemperature                = 13R
     BC_PassengerDensityLowFloor                    = 3.0R
     BC_PassengerDensitySemiLowFloor                = 2.0R
     BC_PassengerDensityRaisedFloor                 = 1.4R
    'BC_CalculatedPassengerNumber  :Calculated
    'BC_UValues :Calculated
     BC_HeatingBoundaryTemperature                  = 20R
     BC_CoolingBoundaryTemperature                  = 24R
     BC_HighVentilation                             = 25R
     BC_lowVentilation                              =  8R
    'BC_High  :Calculated
    'BC_Low  :Calculated
    'BC_HighVentPowerW  :Calculated
    'BC_LowVentPowerW  :Calculated
     BC_SpecificVentilationPower                    = 0.6R
     BC_COP                                         = 4.0R
     BC_AuxHeaterEfficiency                         = 1.0R
     BC_GCVDieselOrHeatingOil                       = 13.0R
     BC_VolumicMassDieselOrHeatingOil               = 1.0R
    'BC_WindowAreaPerUnitBusLength   :Calculated 
    'BC_FrontRearWindowArea  :Calculated
     BC_MaxTemperatureDeltaForLowFloorBusses        = 4.00R
     BC_MaxPossibleBenefitFromTechnologyList        = 0.03R
                                                   
    'Environmental Conditions
    '************************
     EC_EnviromentalTemperature                     =  25.00R
     EC_Solar                                       = 400.00R


    'AC SYSTEM
    '*********
    AC_InCabinRoomAC_System                         = True
    AC_CompressorType                               = "mechanical"
    AC_CompressorCapacitykW                         = 18R


    'VENTILATION
    '***********
    VEN_VentilationOnDuringHeating                        = True
    VEN_VentilationWhenBothHeatingAndACInactive           = True
    VEN_VentilationDuringAC                               = True
    VEN_VentilationFlowSettingWhenHeatingAndACInactive    = "high"
    VEN_VentilationDuringHeating                          = "high"
    VEN_VentilationDuringCooling                          = "low"



    'AUX HEATER
    '**********
    AH_EngineWasteHeatkW                                  =  2.0R
    AH_FuelFiredHeaterkW                                  = 10.0R



    End Sub
  
  
     
  End Class

End Namespace



