Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports NUnit.Framework

Namespace UnitTests

<TestFixture()>
Public Class M0_5_SmartAlternatorSetEfficiencyTests


Private target As M0_5_SmartAlternatorSetEfficiency

Public Sub new()

Initialise()

End Sub

Private sub Initialise()

Dim elecConsumers As New ElectricalConsumerList(26.3,0.096,True)
Dim hvacInputs As New HVACInputs(1,1)
Dim  hvacMap As New HVACMap("testFiles\TestHvacMap.csv")
hvacMap.Initialise()
Dim alternatoMap As New AlternatorMap("testFiles\testAlternatormap.csv")
alternatoMap.Initialise()
Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,hvacInputs,hvacMap,alternatoMap,26.3)

'Results Cards
Dim readings = new Dictionary(Of single, single)
readings.Add(10,8)
readings.Add(70,63)

Dim idleResult As New ResultCard(readings)
Dim tractionResult As New ResultCard(readings)
Dim overrunResult As New ResultCard(readings)

target = New M0_5_SmartAlternatorSetEfficiency(m0,elecConsumers,alternatoMap,idleResult,tractionResult,overrunResult)

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

   Dim expected As Single = 0.618566155
   Dim actual As Single = target.AlternatorsEfficiencyIdleResultCard(2000)

   Assert.AreEqual(expected, actual)
   

End Sub


<Test()> _
Public Sub AlternatorsEfficiencyTraction2000rpmTest()
   Initialise()

   Dim expected As Single = 0.618566155
   Dim actual As Single = target.AlternatorsEfficiencyTractionOnResultCard(2000)

   Assert.AreEqual(expected, actual)

End Sub


<Test()> _
Public Sub AlternatorsEfficiencyOverrun2000rpmTest()
   Initialise()

   Dim expected As Single = 0.618566155
   Dim actual As Single = target.AlternatorsEfficiencyOverrunResultCard(2000)

   Assert.AreEqual(expected, actual)

End Sub



End Class


End Namespace




