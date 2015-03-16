Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries

Namespace UnitTests

<TestFixture()>
Public Class M5_SmartAlternatorSetGenerationTests

'Constants
Private const _powerNetVoltage As Single = 26.3
Private Const _hvacMap As String = "testFiles\TestHvacMap.csv"
Private Const _altMap As String  = "testFiles\testAlternatormap.aalt"
Private const _rpm as Integer = 2000
Private const _altGearPullyEfficiency As Single =0.8

'Private fields
Private _m05 As M0_5_SmartAlternatorSetEfficiency
Private _target As M5__SmartAlternatorSetGeneration
Private _signals As ISignals = New Signals
Private ssmHVac As IHVACSteadyStateModel = New HVACSteadyStateModel(100,100,100)


Private Function GetSSM() As ISSMTOOL


  Const _SSMMAP As String = "TestFiles\ssm.Ahsm
  Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

  Dim ssm As ISSMTOOL = New SSMTOOL( _SSMMAP )


  ssm.Load( _SSMMAP)


   Return ssm


End Function

Private sub Initialise()

_signals.EngineSpeed= 2000

Dim elecConsumers As New ElectricalConsumerList(_powerNetVoltage,0.096,True)

Dim alternatoMap As New AlternatorMap(_altMap)
alternatoMap.Initialise()
Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,alternatoMap,_powerNetVoltage,_signals,GetSSM())

'Results Cards
Dim readings = new List(Of SmartResult)
readings.Add( New SmartResult(10,8))
readings.Add( New SmartResult(70,63))

Dim idleResult As New ResultCard(readings)
Dim tractionResult As New ResultCard(readings)
Dim overrunResult As New ResultCard(readings)

Dim signals As ISignals = New Signals
signals.EngineSpeed=2000

_m05 = New M0_5_SmartAlternatorSetEfficiency(m0,elecConsumers,alternatoMap,idleResult,tractionResult,overrunResult,signals)

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
 Dim expected As Single =4385.07
 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankIdleWatts()

 Assert.AreEqual( expected,  actual)

End Sub

<Test()> _
Public Sub  PowerAtCrankTractionWatts()

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Dim expected As Single =4385.07
 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankTractionOnWatts()

 Assert.AreEqual( expected, actual)

End Sub

<Test()> _
Public Sub  PowerAtCrankOverrunWatts()

 Initialise()
 _target = New M5__SmartAlternatorSetGeneration(_m05,_powerNetVoltage,_altGearPullyEfficiency)
 Dim expected As Single =4385.06982f

 Dim actual As Single = _target.AlternatorsGenerationPowerAtCrankOverrunWatts()

 Assert.AreEqual( expected,actual)

End Sub

End Class


End Namespace



