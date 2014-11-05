Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries

Namespace UnitTests

<TestFixture()>
Public Class M6Test

Private M1 As New M1_Mock(100, 200, 300, 50)
Private M2 As New M2_Mock(120, 130)
Private M3 As New M3_Mock(200, 5000)
Private M4 As New M4_Mock(100, 2, 200, 100, 100)
Private M5 As New M5_Mock(200, 50, 80)
Private Signals As New Signals()

Private Function GetStandardInstanceM6() As IM6

 M1 = New M1_Mock(100, 200, 300, 50)
 M2 = New M2_Mock(120, 130)
 M3 = New M3_Mock(200, 5000)
 M4 = New M4_Mock(100, 2, 200, 100, 100)
 M5 = New M5_Mock(200, 50, 80)

 Return   New M6(M1,M2,M3,M4,M5,Signals)

End Function

Public Sub new ()

Signals.EngineMotoringPower=100
Signals.EngineDrivelinePower=150
Signals.PreExistingAuxPower=30


End Sub

<Test()>
Public Sub CreateNewM6Instance()

Dim target As IM6 = GetStandardInstanceM6()

Assert.IsNotNull( target)

End Sub

<Test()>
Public Sub SmartElectricalOnTest()

  Signals.SmartElectrics=True
  
    Dim target As Im6 = GetStandardInstanceM6()

    Dim OverRunFlag As Integer = target.OverrunFlag
    Dim SmartElecAndPneumaticsCompressorFlag As Integer = target.SmartElecAndPneumaticsCompressorFlag
    Dim SmartElectricalAndPneumaticAlternatorPowerGenAtCrank As Single = target.SmartElecAndPneumaticAltPowerGenAtCrank
    Dim SmartElectricalAndPneumaticAirCompPowerGenAtCrank As Single = target.SmartElecAndPneumaticAirCompPowerGenAtCrank
    Dim SmartElectricalOnlyAlternatorPowerGenAtCrank As Single = target.SmartElecOnlyAltPowerGenAtCrank
    Dim AveragePowerDemandAtCrankFromPneumatics As Single = target.AveragePowerDemandAtCrankFromPneumatics
    Dim SmartPneumaticsOnlyAirCompPowerGenAtCrank As Single = target.SmartPneumaticOnlyAirCompPowerGenAtCrank
    Dim AveragePowerDemandatCrankFromElectricsInHVAC As Single = target.AvgPowerDemandAtCrankFromElectricsIncHVAC
    Dim SmartPneumaticsOnlySmartCompressorFlag As Integer = target.SmartPneumaticsOnlyCompressorFlag

    Assert.AreEqual(0,OverRunFlag                                         )
    Assert.AreEqual(0,SmartElecAndPneumaticsCompressorFlag                )
    Assert.AreEqual(0,SmartElectricalAndPneumaticAlternatorPowerGenAtCrank)
    Assert.AreEqual(200,SmartElectricalAndPneumaticAirCompPowerGenAtCrank )
    Assert.AreEqual(0,SmartElectricalOnlyAlternatorPowerGenAtCrank        )
    Assert.AreEqual(200,AveragePowerDemandAtCrankFromPneumatics           )
    Assert.AreEqual(200,SmartPneumaticsOnlyAirCompPowerGenAtCrank         )
    Assert.AreEqual(320,AveragePowerDemandatCrankFromElectricsInHVAC      )
    Assert.AreEqual(0,SmartPneumaticsOnlySmartCompressorFlag              )

End Sub


<Test()>
Public Sub SmartElectricalOffTest()

    Signals.SmartElectrics=true
    Signals.EngineDrivelinePower=0
    Signals.EngineMotoringPower=0
  
    Dim target As Im6 = GetStandardInstanceM6()

    Dim OverRunFlag As Integer = target.OverrunFlag
    Dim SmartElecAndPneumaticsCompressorFlag As Integer = target.SmartElecAndPneumaticsCompressorFlag
    Dim SmartElectricalAndPneumaticAlternatorPowerGenAtCrank As Single = target.SmartElecAndPneumaticAltPowerGenAtCrank
    Dim SmartElectricalAndPneumaticAirCompPowerGenAtCrank As Single = target.SmartElecAndPneumaticAirCompPowerGenAtCrank
    Dim SmartElectricalOnlyAlternatorPowerGenAtCrank As Single = target.SmartElecOnlyAltPowerGenAtCrank
    Dim AveragePowerDemandAtCrankFromPneumatics As Single = target.AveragePowerDemandAtCrankFromPneumatics
    Dim SmartPneumaticsOnlyAirCompPowerGenAtCrank As Single = target.SmartPneumaticOnlyAirCompPowerGenAtCrank
    Dim AveragePowerDemandatCrankFromElectricsInHVAC As Single = target.AvgPowerDemandAtCrankFromElectricsIncHVAC
    Dim SmartPneumaticsOnlySmartCompressorFlag As Integer = target.SmartPneumaticsOnlyCompressorFlag

    Assert.AreEqual(0,OverRunFlag                                         )
    Assert.AreEqual(0,SmartElecAndPneumaticsCompressorFlag                )
    Assert.AreEqual(0,SmartElectricalAndPneumaticAlternatorPowerGenAtCrank)
    Assert.AreEqual(200,SmartElectricalAndPneumaticAirCompPowerGenAtCrank )
    Assert.AreEqual(0,SmartElectricalOnlyAlternatorPowerGenAtCrank        )
    Assert.AreEqual(200,AveragePowerDemandAtCrankFromPneumatics           )
    Assert.AreEqual(200,SmartPneumaticsOnlyAirCompPowerGenAtCrank         )
    Assert.AreEqual(320,AveragePowerDemandatCrankFromElectricsInHVAC      )
    Assert.AreEqual(0,SmartPneumaticsOnlySmartCompressorFlag              )


End Sub


End Class



End Namespace



