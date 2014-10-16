Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac

Namespace UnitTests

<TestFixture()>
Public Class M5_SmartAlternatorSetGenerationTests

'Constants
Private const _powerNetVoltage As Single = 26.3
Private Const _hvacMap As String = "testFiles\TestHvacMap.csv"
Private Const _altMap As String  = "testFiles\testAlternatormap.csv"
Private const _rpm as Integer = 2000
Private const _altGearPullyEfficiency As Single =0.8

'Private fields
Private _m05 As M0_5_SmartAlternatorSetEfficiency
Private _target As M5__SmartAlternatorSetGeneration

Private sub Initialise()

Dim elecConsumers As New ElectricalConsumerList(26.3,0.096,True)
Dim hvacInputs As New HVACInputs(1,1)
Dim  hvacMap As New HVACMap(_hvacMap)
hvacMap.Initialise()
Dim alternatoMap As New AlternatorMap(_altMap)
alternatoMap.Initialise()
Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,hvacInputs,hvacMap,alternatoMap,_powerNetVoltage)

'Results Cards
Dim readings = new Dictionary(Of single, single)
readings.Add(10,8)
readings.Add(70,63)

Dim idleResult As New ResultCard(readings)
Dim tractionResult As New ResultCard(readings)
Dim overrunResult As New ResultCard(readings)

_m05 = New M0_5_SmartAlternatorSetEfficiency(m0,elecConsumers,alternatoMap,idleResult,tractionResult,overrunResult)

End Sub

<Test()> _
Public Sub CreateNewTest() 

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Assert.IsNotNull( _target )

End Sub

<Test()> _
Public Sub  PowerAtCrankIdleWatts()

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Dim expected As Single =1681.42822
 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankIdleWatts(_rpm)

 Assert.AreEqual( expected, actual)

End Sub

<Test()> _
Public Sub  PowerAtCrankTractionWatts()

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Dim expected As Single =1681.42822
 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankTractionOnWatts(_rpm)

 Assert.AreEqual( expected, actual)

End Sub

<Test()> _
Public Sub  PowerAtCrankOverrunWatts()

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Dim expected As Single =1681.42822
 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankOverrunWatts(_rpm)

 Assert.AreEqual( expected, actual)

End Sub

End Class


End Namespace



