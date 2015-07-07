Imports System.IO

Namespace Hvac

    'Used by SSMHVAC Class
    Public Class SSMGenInputs
        Implements ISSMGenInputs

        Private _EC_EnviromentalConditions_BatchFile As String
        Private _EC_EnvironmentalConditionsMap As IEnvironmentalConditionsMap

#Region "Constructors"

        Sub New(Optional initialiseDefaults As Boolean = False)

            If initialiseDefaults Then SetDefaults()

        End Sub

#End Region

#Region "Bus Parameterisation"

        'C4/D4
        Public Property BP_BusModel As String Implements ISSMGenInputs.BP_BusModel

        'C5/D5
        Public Property BP_NumberOfPassengers As Double Implements ISSMGenInputs.BP_NumberOfPassengers

        'C6/D6
        Public Property BP_BusFloorType As String Implements ISSMGenInputs.BP_BusFloorType

        'D7/C7 - ( M/2 )
        Public ReadOnly Property BP_BusFloorSurfaceArea As Double Implements ISSMGenInputs.BP_BusFloorSurfaceArea
            Get

                '=IF(AND(C6="low floor",C13<=2.55,C13>=2.5),(2.55*(C12-1.2)),((C12-1.2)*C13))
                If BP_BusFloorType = "low floor" AndAlso BP_BusWidth <= 2.55 AndAlso BP_BusWidth >= 2.5 Then
                    Return Math.Round((2.55 * (BP_BusLength - 1.2)), 6)
                Else
                    Return Math.Round(((BP_BusLength - 1.2) * BP_BusWidth), 6)
                End If

            End Get
        End Property

        'D8/C8 - ( M/2 )
        Public ReadOnly Property BP_BusSurfaceAreaM2 As Double Implements ISSMGenInputs.BP_BusSurfaceAreaM2
            Get
                '2 * (C12*C13 + C12*C14 + C13*C14)
                Return 2 * ((BP_BusLength * BP_BusWidth) + (BP_BusLength * BP_BusHeight) + (BP_BusWidth * BP_BusHeight))
            End Get
        End Property

        'D9/C9 - ( M/2 )
        Public ReadOnly Property BP_BusWindowSurface As Double Implements ISSMGenInputs.BP_BusWindowSurface
            Get
                '=(C40*C12)+C41
                Return (BC_WindowAreaPerUnitBusLength * BP_BusLength) + BC_FrontRearWindowArea
            End Get
        End Property

        'C10/D10
        Public Property BP_DoubleDecker As Boolean Implements ISSMGenInputs.BP_DoubleDecker

        'D11/C11 - ( M/3 )
        Public ReadOnly Property BP_BusVolume As Double Implements ISSMGenInputs.BP_BusVolume
            Get
                '=(C12*C13*C14)
                Return (BP_BusLength * BP_BusWidth * BP_BusHeight)
            End Get
        End Property

        'D12/C12 - ( M )
        Public Property BP_BusLength As Double Implements ISSMGenInputs.BP_BusLength

        'D13/C13 - ( M )
        Public Property BP_BusWidth As Double Implements ISSMGenInputs.BP_BusWidth

        'D14/C14 - ( M )
        Public Property BP_BusHeight As Double Implements ISSMGenInputs.BP_BusHeight

#End Region

#Region "Boundary Conditions"

        'C17
        Public Property BC_GFactor As Double Implements ISSMGenInputs.BC_GFactor

        'C18            
        Public ReadOnly Property BC_SolarClouding As Double Implements ISSMGenInputs.BC_SolarClouding
            Get
                '=IF(C46<17,0.65,0.8)
                Return If(EC_EnviromentalTemperature < 17, 0.65, 0.8)
            End Get
        End Property

        'C19 - ( W )
        Public ReadOnly Property BC_HeatPerPassengerIntoCabinW As Double Implements ISSMGenInputs.BC_HeatPerPassengerIntoCabinW
            Get
                '=IF(C46<17,50,80)
                Return If(EC_EnviromentalTemperature < 17, 50, 80)
            End Get
        End Property

        'C20 - ( oC )
        Public Property BC_PassengerBoundaryTemperature As Double Implements ISSMGenInputs.BC_PassengerBoundaryTemperature

        'C21 - ( Passenger/Metre Squared )
        Public ReadOnly Property BC_PassengerDensityLowFloor As Double Implements ISSMGenInputs.BC_PassengerDensityLowFloor
            Get
                '=IF($C$10="No",3,3.7)
                Return If(BP_DoubleDecker, 3.7, 3)
            End Get
        End Property

        'C22 - ( Passenger/Metre Squared )
        Public ReadOnly Property BC_PassengerDensitySemiLowFloor As Double Implements ISSMGenInputs.BC_PassengerDensitySemiLowFloor
            Get
                '=IF($C$10="No",2.2,3)
                Return If(BP_DoubleDecker, 3, 2.2)
            End Get
        End Property

        'C23 - ( Passenger/Metre Squared )
        Public ReadOnly Property BC_PassengerDensityRaisedFloor As Double Implements ISSMGenInputs.BC_PassengerDensityRaisedFloor
            Get
                '=IF($C$10="No",1.4,2)
                Return If(BP_DoubleDecker, 2, 1.4)
            End Get
        End Property

        'C24               
        Public ReadOnly Property BC_CalculatedPassengerNumber As Double Implements ISSMGenInputs.BC_CalculatedPassengerNumber
            Get
                '=ROUND(IF($D$5<IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7,$D$5,IF(D6="low floor",C21,IF(D6="semi low floor",C22,C23))*D7),0)
                Dim tmp As Double = If(BP_BusFloorType = "low floor", BC_PassengerDensityLowFloor, If(BP_BusFloorType = "semi low floor", BC_PassengerDensitySemiLowFloor, BC_PassengerDensityRaisedFloor)) * BP_BusFloorSurfaceArea
                Return Math.Round(If(BP_NumberOfPassengers < tmp, BP_NumberOfPassengers, tmp), 0)
            End Get
        End Property

        'C25 - ( W/K/M3 )
        Public ReadOnly Property BC_UValues As Double Implements ISSMGenInputs.BC_UValues
            Get
                '=IF(D6="low floor",4,IF(D6="semi low floor",3.5,3))
                Return If(BP_BusFloorType = "low floor", 4, If(BP_BusFloorType = "semi low floor", 3.5, 3))
            End Get
        End Property

        'C26 - ( oC )
        Public Property BC_HeatingBoundaryTemperature As Double Implements ISSMGenInputs.BC_HeatingBoundaryTemperature

        'C27 - ( oC )
        Public Property BC_CoolingBoundaryTemperature As Double Implements ISSMGenInputs.BC_CoolingBoundaryTemperature

        'C28 - ( oC )
        Public ReadOnly Property BC_TemperatureCoolingTurnsOff As Double Implements ISSMGenInputs.BC_TemperatureCoolingTurnsOff
            Get
                Return 17
            End Get
        End Property

        'C29 - ( L/H )
        Public Property BC_HighVentilation As Double Implements ISSMGenInputs.BC_HighVentilation

        'C30 - ( L/H )
        Public Property BC_lowVentilation As Double Implements ISSMGenInputs.BC_lowVentilation

        'C31 - ( M3/H )
        Public ReadOnly Property BC_High As Double Implements ISSMGenInputs.BC_High
            Get
                '=D11*C29
                Return BP_BusVolume * BC_HighVentilation
            End Get
        End Property

        'C32 - ( M3/H )
        Public ReadOnly Property BC_Low As Double Implements ISSMGenInputs.BC_Low
            Get
                '=C30*D11
                Return BP_BusVolume * BC_lowVentilation
            End Get
        End Property

        'C33 - ( W )
        Public ReadOnly Property BC_HighVentPowerW As Double Implements ISSMGenInputs.BC_HighVentPowerW
            Get
                '=C31*C35
                Return BC_High * BC_SpecificVentilationPower
            End Get
        End Property

        'C34 - ( W )
        Public ReadOnly Property BC_LowVentPowerW As Double Implements ISSMGenInputs.BC_LowVentPowerW
            Get
                '=C32*C35
                Return BC_Low * BC_SpecificVentilationPower
            End Get
        End Property

        'C35 - ( Wh/M3 )
        Public Property BC_SpecificVentilationPower As Double Implements ISSMGenInputs.BC_SpecificVentilationPower

        'C37               
        Public Property BC_AuxHeaterEfficiency As Double Implements ISSMGenInputs.BC_AuxHeaterEfficiency

        'C38 - ( KW/HKG )
        Public Property BC_GCVDieselOrHeatingOil As Double Implements ISSMGenInputs.BC_GCVDieselOrHeatingOil

        'C40 - ( M2/M )
        Public ReadOnly Property BC_WindowAreaPerUnitBusLength As Double Implements ISSMGenInputs.BC_WindowAreaPerUnitBusLength
            Get
                '=IF($C$10="No",1.5,2.5)
                Return If(BP_DoubleDecker, 2.5, 1.5)
            End Get
        End Property

        'C41 - ( M/2 )
        Public ReadOnly Property BC_FrontRearWindowArea As Double Implements ISSMGenInputs.BC_FrontRearWindowArea
            Get
                '=IF($C$10="No",5,8)
                Return If(BP_DoubleDecker, 8, 5)
            End Get
        End Property

        'C42 - ( K )
        Public Property BC_MaxTemperatureDeltaForLowFloorBusses As Double Implements ISSMGenInputs.BC_MaxTemperatureDeltaForLowFloorBusses

        'C43 - ( Fraction )
        Public Property BC_MaxPossibleBenefitFromTechnologyList As Double Implements ISSMGenInputs.BC_MaxPossibleBenefitFromTechnologyList

#End Region

#Region "Environmental Conditions"

        'C46 - ( oC )
        Public Property EC_EnviromentalTemperature As Double Implements ISSMGenInputs.EC_EnviromentalTemperature

        'C47 - ( W/M3 )
        Public Property EC_Solar As Double Implements ISSMGenInputs.EC_Solar

        '( EC_EnviromentalTemperature and  EC_Solar) (Batch Mode)
        Public ReadOnly Property EC_EnvironmentalConditionsMap As IEnvironmentalConditionsMap Implements ISSMGenInputs.EC_EnvironmentalConditionsMap
            Get
                Return _EC_EnvironmentalConditionsMap
            End Get
        End Property

        Public Property EC_EnviromentalConditions_BatchFile As String Implements ISSMGenInputs.EC_EnviromentalConditions_BatchFile
            Get
                Return _EC_EnviromentalConditions_BatchFile
            End Get
            Set(value As String)
                _EC_EnvironmentalConditionsMap = New EnvironmentalConditionsMap(value)
                _EC_EnviromentalConditions_BatchFile = value
            End Set
        End Property

        Public Property EC_EnviromentalConditions_BatchEnabled As Boolean Implements ISSMGenInputs.EC_EnviromentalConditions_BatchEnabled

#End Region

#Region "AC System"

        'C53 - "Continous/2-stage/3-stage/4-stage
        Public Property AC_CompressorType As String Implements ISSMGenInputs.AC_CompressorType

        'mechanical/electrical
        Public ReadOnly Property AC_CompressorTypeDerived As String Implements ISSMGenInputs.AC_CompressorTypeDerived
            Get
                Return If(AC_CompressorType = "Continuous", "Electrical", "Mechanical")
            End Get
        End Property

        'C54 -  ( KW )
        Public Property AC_CompressorCapacitykW As Double Implements ISSMGenInputs.AC_CompressorCapacitykW

        'C59
        Public ReadOnly Property AC_COP As Double Implements ISSMGenInputs.AC_COP
            Get
                Dim cop As Double = 3.5R

                If (Not AC_CompressorType Is Nothing) Then
                    cop = If(AC_CompressorType.ToLower = "3-stage", cop * 1.02, cop)
                    cop = If(AC_CompressorType.ToLower = "4-stage", cop * 1.02, cop)
                    cop = If(AC_CompressorType.ToLower = "continuous", If(BP_BusFloorType.ToLower = "low floor", cop * 1.04, cop * 1.06), cop)
                End If
                
                Return Math.Round(cop, 2)
            End Get
        End Property

#End Region

#Region "Ventilation"

        'C62 - Boolean Yes/No
        Public Property VEN_VentilationOnDuringHeating As Boolean Implements ISSMGenInputs.VEN_VentilationOnDuringHeating

        'C63 - Boolean Yes/No
        Property VEN_VentilationWhenBothHeatingAndACInactive As Boolean Implements ISSMGenInputs.VEN_VentilationWhenBothHeatingAndACInactive

        'C64 - Boolean Yes/No
        Public Property VEN_VentilationDuringAC As Boolean Implements ISSMGenInputs.VEN_VentilationDuringAC

        'C65 - String high/low
        Public Property VEN_VentilationFlowSettingWhenHeatingAndACInactive As String Implements ISSMGenInputs.VEN_VentilationFlowSettingWhenHeatingAndACInactive

        'C66 - String high/low
        Property VEN_VentilationDuringHeating As String Implements ISSMGenInputs.VEN_VentilationDuringHeating

        'C67 - String high/low                                               
        Property VEN_VentilationDuringCooling As String Implements ISSMGenInputs.VEN_VentilationDuringCooling

#End Region

#Region "AUX Heater"

        'C70 - ( KW )
        Public Property AH_EngineWasteHeatkW As Double Implements ISSMGenInputs.AH_EngineWasteHeatkW

        'C71 - ( KW )
        Public Property AH_FuelFiredHeaterkW As Double Implements ISSMGenInputs.AH_FuelFiredHeaterkW

#End Region

#Region "Default Values"

        Private Sub SetDefaults()

            'BUS Parameterisation
            '********************
            BP_BusModel = "DummyBus"
            BP_NumberOfPassengers = 47.0R
            BP_BusFloorType = "raised floor"
            BP_DoubleDecker = False
            BP_BusLength = 10.655R
            BP_BusWidth = 2.55R
            BP_BusHeight = 2.275R
            'BP_BusFloorSurfaceArea  : Calculated
            'BP_BusSurfaceAreaM2 : Calculated
            'BP_BusWindowSurface    : Calculated
            'BP_BusVolume : Calculated

            'BOUNDRY CONDITIONS
            '******************

            BC_GFactor = 0.95R
            'BC_SolarClouding As Double :Calculated
            'BC_HeatPerPassengerIntoCabinW  :Calculated
            BC_PassengerBoundaryTemperature = 12.0R
            'BC_PassengerDensityLowFloor :Calculated
            'BC_PassengerDensitySemiLowFloor :Calculated
            'BC_PassengerDensityRaisedFloor :Calculated
            'BC_CalculatedPassengerNumber  :Calculated
            'BC_UValues :Calculated
            BC_HeatingBoundaryTemperature = 18.0R
            BC_CoolingBoundaryTemperature = 23.0R
            'BC_CoolingBoundaryTemperature : ReadOnly Static
            BC_HighVentilation = 20.0R
            BC_lowVentilation = 7.0R
            'BC_High  :Calculated
            'BC_Low  :Calculated
            'BC_HighVentPowerW  :Calculated
            'BC_LowVentPowerW  :Calculated
            BC_SpecificVentilationPower = 0.56R
            'BC_COP :Calculated
            BC_AuxHeaterEfficiency = 0.84R
            BC_GCVDieselOrHeatingOil = 11.8R
            'BC_WindowAreaPerUnitBusLength   :Calculated 
            'BC_FrontRearWindowArea  :Calculated
            BC_MaxTemperatureDeltaForLowFloorBusses = 3.0R
            BC_MaxPossibleBenefitFromTechnologyList = 0.5R

            'Environmental Conditions
            '************************
            EC_EnviromentalTemperature = 25.0R
            EC_Solar = 400.0R
            EC_EnviromentalConditions_BatchEnabled = False

            'AC SYSTEM
            '*********
            AC_CompressorType = "2-stage"
            AC_CompressorCapacitykW = 18.0R


            'VENTILATION
            '***********
            VEN_VentilationOnDuringHeating = True
            VEN_VentilationWhenBothHeatingAndACInactive = True
            VEN_VentilationDuringAC = True
            VEN_VentilationFlowSettingWhenHeatingAndACInactive = "high"
            VEN_VentilationDuringHeating = "high"
            VEN_VentilationDuringCooling = "high"


            'AUX HEATER
            '**********
            AH_FuelFiredHeaterkW = 30.0R

        End Sub

#End Region

    End Class

End Namespace



