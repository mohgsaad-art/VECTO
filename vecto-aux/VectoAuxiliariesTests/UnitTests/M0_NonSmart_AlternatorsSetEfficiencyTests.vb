Imports NUnit.Framework
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac



Namespace UnitTests

<TestFixture()>
Public Class M0_NonSmart_AlternatorsSetEfficiencyTests

Private Const cstrAlternatorsEfficiencyMapLocation As String = "tests\testAlternatorMap.aalt"
Private Const cstrHVACMapLocation As String = "TestFiles\TestHvacMap.csv"
Private Const cstrAlternatorMap As String = "TestFiles\testAlternatorMap.aalt"

Private elecConsumers As IElectricalConsumerList

Private alternatorMap As IAlternatorMap
Private signals As Signals = New Signals 
Private powernetVoltage As Single = 26.3
Private ssm As IHVACSteadyStateModel = New HVACSteadyStateModel(100,100,100)

Private Function GetSSM() As ISSMTOOL


  Const _SSMMAP As String = "TestFiles\ssm.Ahsm
  Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

  Dim ssm As ISSMTOOL = New SSMTOOL( _SSMMAP )


  ssm.Load( _SSMMAP)


   Return ssm


End Function


Public Sub New()


    signals.EngineSpeed=2000

   'Setup consumers and HVAC ( 1 Consumer in Test Category )
    elecConsumers = CType(New ElectricalConsumerList(0.096,26.3), IElectricalConsumerList)
    elecConsumers.AddConsumer(New ElectricalConsumer(False, "TEST", "CONSUMER1", 20, 0.5, 26.3, 1))


    'Alternator Map
    alternatorMap = CType(New AlternatorMap(cstrAlternatorMap), IAlternatorMap)
    alternatorMap.Initialise()



End Sub

<Test()>
Public Sub CreateNewTest()
       Dim target As M0_NonSmart_AlternatorsSetEfficiency = New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,  alternatorMap, powernetVoltage,signals,GetSSM())
       Assert.IsNotNull(target)
End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingElecConsumers_ThrowArgumentExceptionTest()
       Dim target As M0_NonSmart_AlternatorsSetEfficiency = New M0_NonSmart_AlternatorsSetEfficiency(Nothing, alternatorMap, powernetVoltage,signals,GetSSM())
End Sub






<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingAlternatorMap_ThrowArgumentExceptionTest()
       Dim target As M0_NonSmart_AlternatorsSetEfficiency = New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers,  Nothing, powernetVoltage,signals,GetSSM())
End Sub



<Test()>
Public Sub EfficiencyValueTest()
       Dim target As M0_NonSmart_AlternatorsSetEfficiency = New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers, alternatorMap, powernetVoltage,signals,GetSSM())

       Dim actual As Single = target.AlternatorsEfficiency

       Dim expected As Single = 0.655407

       Assert.AreEqual(expected, actual)


End Sub


<Test()>
Public Sub HVAC_PowerDemandAmpsTest()

      Dim target As M0_NonSmart_AlternatorsSetEfficiency = New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers, alternatorMap, powernetVoltage,signals,GetSSM())

      Dim actual As Single
      Dim expected As Single = 18.81821 

      actual = target.GetHVACElectricalPowerDemandAmps() 


      Assert.AreEqual(expected, actual)


End Sub


End Class



End Namespace



