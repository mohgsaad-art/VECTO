Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules

Public Class M14
 Implements IM14


  Private m13 As IM13
  Private signals As ISignals
  Private constants As IHVACConstants
  Private ssm As ISSMTOOL


  Public Sub new ( m13 As IM13 , hvacSSM As ISSMTOOL, constants As IHVACConstants, signals As ISignals)

     If m13 is Nothing then Throw New ArgumentException("M14, No M13 Supplied in  arguments")
     If hvacSSM is Nothing then Throw New ArgumentException("M14, No SSMTOOL constants Supplied in  arguments")
     If constants is Nothing then Throw New ArgumentException("M14, No signals Supplied in  arguments")
     If signals is Nothing then Throw New ArgumentException("M14, No signals constants Supplied in  arguments")


     Me.m13 = m13
     Me.signals = signals
     Me.constants = constants
     Me.ssm=  hvacSSM
     

  End Sub

 'Staging Calculations
  Private ReadOnly Property S1 As Single
      Get
         Return  m13.WHTCTotalCycleFuelConsumptionGrams * constants.DieselGCVJperGram
      End Get
  End Property
  Private ReadOnly Property S2 As Single
        Get
          Return constants.FuelEnergyToHeatToCoolant * s1
        End Get
  End Property
  Private ReadOnly Property S3 As Single
      Get
        Return S2 * constants.CoolantHeatTransferredToAirCabinHeater
      End Get
  End Property
  Private ReadOnly Property S4 As Single
      Get
        Return (S3 / signals.CurrentCycleTimeInSeconds )/1000
      End Get
  End Property


  Private ReadOnly Property S5 As Single
      Get
        Return signals.CurrentCycleTimeInSeconds/3600
      End Get
  End Property
  Private ReadOnly Property S6 As Single
      Get
        Return S5 * ssm.FuelPerHBaseAsjusted(S4) *  constants.FuelDensity835GramsPerLitre
      End Get
  End Property
  Private ReadOnly Property S7 As Single
      Get
        Return m13.WHTCTotalCycleFuelConsumptionGrams + s6
      End Get
  End Property

  Private ReadOnly Property S8 As Single
      Get
        Return S7 / constants.FuelDensity835GramsPerLitre
      End Get
  End Property
 Public ReadOnly Property TotalCycleFCGrams As Single Implements IM14.TotalCycleFCGrams
     Get
       Return S7
     End Get
 End Property

  Public ReadOnly Property TotalCycleFCLitres As Single Implements IM14.TotalCycleFCLitres
      Get
        Return S8
      End Get
  End Property



End Class



End Namespace
