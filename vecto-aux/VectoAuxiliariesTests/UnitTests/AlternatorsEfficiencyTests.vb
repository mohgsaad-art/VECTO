Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac



Namespace UnitTests

<TestFixture()>
Public Class AlternatorsEfficiencyTests

Private Const cstrAlternatorsEfficiencyMapLocation As String = "tests\testAlternatorMap.csv"
Private Const cstrHVACMapLocation As String = "TestFiles\TestHvacMap.csv"
Private Const cstrAlternatorMap As String = "TestFiles\testAlternatorMap.csv"

Private elecConsumers As IElectricalConsumerList
Private hvacInputs As IHVACInputs
Private hvacMap As IHVACMap
Private alternatorMap As IAlternatorMap

Private powernetVoltage As Single = 26.3



Public Sub New()

   'Setup consumers and HVAC ( 1 Consumer in Test Category )
    elecConsumers = CType(New ElectricalConsumerList(), IElectricalConsumerList)
    elecConsumers.AddConsumer(New ElectricalConsumer(False, "TEST", "CONSUMER1", 20, 0.5, 26.3, 1))

    'Setup HVAC
     hvacInputs = CType(New HVACInputs(1, 1), IHVACInputs)
     hvacMap = CType(New HVACMap(cstrHVACMapLocation), IHVACMap)
     hvacMap.Initialise()

    'Alternator Map
    alternatorMap = CType(New AlternatorMap(cstrAlternatorMap), IAlternatorMap)
    alternatorMap.Initialise()



End Sub

<Test()>
Public Sub CreateNewTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, hvacInputs, hvacMap, alternatorMap, powernetVoltage)
       Assert.IsNotNull(target)
End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingElecConsumers_ThrowArgumentExceptionTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(Nothing, hvacInputs, hvacMap, alternatorMap, powernetVoltage)
End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingHVACInputs_ThrowArgumentExceptionTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, Nothing, hvacMap, alternatorMap, powernetVoltage)
End Sub


<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingHVACMAP_ThrowArgumentExceptionTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, hvacInputs, Nothing, alternatorMap, powernetVoltage)
End Sub


<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNew_MissingAlternatorMap_ThrowArgumentExceptionTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, hvacInputs, hvacMap, Nothing, powernetVoltage)
End Sub



<Test()>
Public Sub EfficiencyValueTest()
       Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, hvacInputs, hvacMap, alternatorMap, powernetVoltage)

       Dim actual As Single = target.GetEfficiency(2000, 1)

       Dim expected As Single = 0.6375106

       Assert.AreEqual(expected, actual)


End Sub


<Test()>
Public Sub HVAC_PowerDemandAmpsTest()

      Dim target As AlternatorsEfficiency = New AlternatorsEfficiency(elecConsumers, hvacInputs, hvacMap, alternatorMap, powernetVoltage)

      Dim actual As Single
      Dim expected As Single = 152.091263F   '( HVAC POWER OUTPUT IN KW not Watts )

      actual = target.GetHVACElectricalPowerDemandAmps() * 1000


      Assert.AreEqual(expected, actual)


End Sub


End Class



End Namespace



