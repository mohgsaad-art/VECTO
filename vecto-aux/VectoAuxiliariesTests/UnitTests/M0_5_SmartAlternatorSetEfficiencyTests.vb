Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries
Imports NUnit.Framework

Namespace UnitTests

<TestFixture()>
Public Class M0_5_SmartAlternatorSetEfficiencyTests

Private target As M0_5_SmartAlternatorSetEfficiency
Private signals  = New Signals

Public Sub new()

Initialise()

End Sub

Private sub Initialise()


Dim ssm As New HVACSteadyStateModel(100,100,100)
Dim elecConsumers As New ElectricalConsumerList(26.3,0.096,True)
Dim hvacInputs As New HVACInputs(1,1)
'Dim  hvacMap As New HVACMap("testFiles\TestHvacMap.csv")
'hvacMap.Initialise()
Dim alternatoMap As New AlternatorMap("testFiles\testAlternatormap.csv")
alternatoMap.Initialise()

Dim signals = New Signals()
signals.EngineSpeed=2000

Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,hvacInputs,alternatoMap,26.3,signals,ssm)

'Results Cards
Dim readings = new List(of SmartResult)
readings.Add(new SmartResult(10,8))
readings.Add(New SmartResult(70,63))

Dim idleResult As New ResultCard(readings)
Dim tractionResult As New ResultCard(readings)
Dim overrunResult As New ResultCard(readings)


  signals.EngineSpeed=2000
  target = New M0_5_SmartAlternatorSetEfficiency(m0,elecConsumers,alternatoMap,idleResult,tractionResult,overrunResult, signals)

End Sub

<Test()> _
Public Sub CreateNewTest()
   Initialise()
   Assert.IsNotNull( target)
End Sub

<Test()> _
Public Sub SmartIdleCurrentTest()
   Initialise()
   Assert.IsNotNull( target)
End Sub

<Test()> _
Public Sub SmartTractionCurrentTest()
   Initialise()
   Assert.IsNotNull( target)
End Sub

<Test()> _
Public Sub SmartOverrunCurrentTest()
   Initialise()
   Assert.IsNotNull( target)
End Sub

<Test()> _
Public Sub AlternatorsEfficiencyIdle2000rpmTest()
   Initialise()

   Dim expected As Single = 0.5769231
   Dim actual As Single = target.AlternatorsEfficiencyIdleResultCard()

   Assert.AreEqual(expected, actual)
   

End Sub


<Test()> _
Public Sub AlternatorsEfficiencyTraction2000rpmTest()
   Initialise()

   Dim expected As Single = 0.5769231
   Dim actual As Single = target.AlternatorsEfficiencyTractionOnResultCard()

   Assert.AreEqual(expected, actual)

End Sub


<Test()> _
Public Sub AlternatorsEfficiencyOverrun2000rpmTest()
   Initialise()

   Dim expected As Single = 0.5769231
   Dim actual As Single = target.AlternatorsEfficiencyOverrunResultCard()

   Assert.AreEqual(expected, actual)

End Sub



End Class


End Namespace




