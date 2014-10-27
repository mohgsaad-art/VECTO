Imports NUnit.Framework
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac


Namespace UnitTests

<TestFixture()>
Public Class M1_AverageHVACLoadDemandTests

  Private Const _GOODMAP As String = "TestFiles\testAlternatorMap.csv"

Private signals As ISignals = New Signals With {.EngineSpeed=2000}
Private powernetVoltage As Single = 26.3
Private ssm As IHVACSteadyStateModel = New HVACSteadyStateModel(100,100,100) 
Private m0 As IM0_NonSmart_AlternatorsSetEfficiency
Private alternatorMap As IAlternatorMap = New AlternatorMap(_GOODMAP)
Private hvacMap As IHVACMap = New HVACMap("")
Private alternatorGearEfficiency As Single = 0.8
Private compressorGrearEfficiency As Single = 0.8


Public Sub new()

alternatorMap.Initialise()


   m0 = New M0_NonSmart_AlternatorsSetEfficiency(New ElectricalConsumerList(powernetVoltage,0.096,True),New HVACInputs(),alternatorMap,powernetVoltage,signals,ssm )


End Sub

Private function GETM1Instance()  As IM1_AverageHVACLoadDemand

return  New M1_AverageHVACLoadDemand( m0,
                                       hvacMap,
                                       New HVACInputs(),
                                       alternatorGearEfficiency,
                                       compressorGrearEfficiency,
                                       powernetVoltage,
                                       signals,
                                       ssm)
End Function



<Test()>
Public Sub CreateNew()

  Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()

  Assert.NotNull(target)

End Sub



<Test()>
Public Sub GetAveragePowerDemandAtCrankFromHVACMechanicsWattsTest()


   Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
   Dim expected As Single = 125
   dim actual as Single = target.AveragePowerDemandAtCrankFromHVACMechanicalsWatts

   Assert.AreEqual( expected , actual)

End Sub

<Test()>
Public Sub AveragePowerDemandAtCrankFromHVACElectricsWattsTest()


   Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
   Dim expected As Single = 195.171173
   dim actual as Single = target.AveragePowerDemandAtCrankFromHVACElectricsWatts

   Assert.AreEqual( expected , actual)

End Sub

<Test()>
Public Sub AveragePowerDemandAtAlternatorFromHVACElectricsWattsTest()


   Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
   Dim expected As Single = 100
   dim actual as Single = target.AveragePowerDemandAtAlternatorFromHVACElectricsWatts

   Assert.AreEqual( expected , actual)

End Sub

<Test()>
Public Sub HVACFuelingLitresPerHourTest()


   Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
   Dim expected As Single = 100
   dim actual as Single = target.HVACFuelingLitresPerHour

   Assert.AreEqual( expected , actual)

End Sub

End Class


End Namespace



