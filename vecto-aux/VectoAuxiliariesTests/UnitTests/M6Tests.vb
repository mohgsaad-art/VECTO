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


'Mikes Test Results
<Test()> _
<TestCase(100,100,100,100,20,20,40,100,100,100,100,-550,false,   0,0,   0, 20,   0,100,20,200,0)> _
<TestCase(100,100,100,100,20,20,40,100,100,100,100,-550,true ,   1,1,  100,40, 100,100,40,200,1)> _
Public Sub MikesConditionsTest( M1_1 As Single, _
                                M1_2 As Single, _
                                M2_1 As Single, _
                                M3_1 As Single, _
                                M4_1 As Single, _
                                M4_2 As Single, _
                                M4_3 As Single, _
                                M5_1 As Single, _
                                M5_2 As Single, _
                                AUX As Single, _
                                EMP As Single, _
                                EDP As Single, _
                                SM As Boolean, _
                                OUT1 As Single, _
                                OUT2 As single, _
                                OUT3 As Single, _
                                OUT4 As Single, _
                                OUT5 As Single, _
                                OUT6 As Single, _
                                OUT7 As Single, _
                                OUT8 As Single, _
                                OUT9 As Single)


                 dim  M1 = New M1_Mock()
                 dim  M2 = New M2_Mock()
                 dim  M3 = New M3_Mock()
                 dim  M4 = New M4_Mock()
                 dim  M5 = New M5_Mock()

                 Dim signals As New Signals()


                 M1._AveragePowerDemandAtCrankFromHVACMechanicalsWatts= M1_1
                 M1._AveragePowerDemandAtCrankFromHVACElectricsWatts=M1_2
                 M2._GetAveragePowerAtCrankFromElectrics=M2_1
                 M3._GetAveragePowerDemandAtCrankFromPneumatics= M3_1     
                 M4._PowerCompressorOff=M4_1
                 M4._PowerDifference=M4_2
                 M4._PowerCompressorOn=M4_3
                 M5._AlternatorsGenerationPowerAtCrankTractionOnWatts=M5_1
                 M5._AlternatorsGenerationPowerAtCrankOverrunWatts=M5_2
                 
                 

                 signals.EngineMotoringPower= EMP
                 signals.PreExistingAuxPower=AUX
                 signals.EngineDrivelinePower=EDP
                 signals.SmartElectrics=SM


                 Dim target As New M6(M1,M2,M3,M4,M5,Signals)

                 Assert.AreEqual(OUT1,target.OverrunFlag)
                 Assert.AreEqual(OUT2,target.SmartElecAndPneumaticsCompressorFlag)
                 Assert.AreEqual(OUT3,target.SmartElecAndPneumaticAltPowerGenAtCrank)
                 Assert.AreEqual(OUT4,target.SmartElecAndPneumaticAirCompPowerGenAtCrank)
                 Assert.AreEqual(OUT5,target.SmartElecOnlyAltPowerGenAtCrank)
                 Assert.AreEqual(OUT6,target.AveragePowerDemandAtCrankFromPneumatics)
                 Assert.AreEqual(OUT7,target.SmartElecAndPneumaticAirCompPowerGenAtCrank)
                 Assert.AreEqual(OUT8,target.AvgPowerDemandAtCrankFromElectricsIncHVAC)
                 Assert.AreEqual(OUT9,target.SmartPneumaticsOnlyCompressorFlag)




End Sub



End Class



End Namespace



