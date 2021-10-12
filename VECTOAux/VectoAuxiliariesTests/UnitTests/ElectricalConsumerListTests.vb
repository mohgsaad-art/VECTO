
Imports System.IO
Imports NUnit.Framework


<TestFixture()>
Public Class ElectricalConsumerListTests
	'Private TestConsumerList As IElectricalConsumerList = Utils.GetElectricConsumers() ' New ElectricalConsumerList(26.3.SI(of Volt), 0.096, True)


	Sub New()
	End Sub

    <OneTimeSetUp>
    Public Sub RunBeforeAnyTests()
        Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
    End Sub

	'<Test()>
	'Public Sub CreateNewTest()

	'	Dim target As ElectricalConsumerList = new ElectricalConsumerList(New List(Of IElectricalConsumer)()) '(26.3.SI(of Volt), 0.096, True)

	'	Assert.IsNotNull(target)
	'End Sub


	'<Test()>
	'Public Sub SumAllConsumersTest()

 '       Dim auxconfig = Utils.GetAuxTestConfig()
 '       Dim TestConsumerList = auxconfig.ElectricalUserInputsConfig.ElectricalConsumers
	'	TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 1

	'	'Dim actual As Ampere = TestConsumerList.GetTotalAverageDemandAmps(False)

 '       Dim m0_1 = New M0_1Impl(auxconfig)
 '       dim actual As Ampere = m0_1.TotalAverageDemandAmpsIncludingBaseLoad

	'	TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 0

	'	Dim expected = 60.63

	'	Assert.AreEqual(expected, actual.Value(), 0.01)
	'End Sub

	'<Test()>
	'Public Sub SumNonExcludedConsumersTest()

	'    Dim auxconfig = Utils.GetAuxTestConfig()
	'    Dim TestConsumerList = auxconfig.ElectricalUserInputsConfig.ElectricalConsumers

 '       TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 1
	'	'Dim actual As Ampere = TestConsumerList.GetTotalAverageDemandAmps(True)
	'    Dim m0_1 = New M0_1Impl(auxconfig)
	'    dim actual As Ampere = m0_1.TotalAverageDemandAmpsWithoutBaseLoad

	'	TestConsumerList.Items.First(Function(item) item.ConsumerName = "Controllers,Valves etc").NumberInActualVehicle = 0
	'	Dim expected = 35.63

 '       Assert.AreEqual(expected, actual.Value(), 0.01)
	'End Sub


    '<Test()>
    'Public Sub DuplicateConsumersTest_ThrowsArgumentException()

    '    Dim target As New ElectricalConsumerList(0.096.SI(of Volt), 26.3)
    '    'Add two OnBaseVehicle consumers
    '    target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10.SI(of Ampere), 1, 26.3.SI(of Volt), 1, ""))
    '    Assert.That(Sub() target.AddConsumer(New ElectricalConsumer(True, "TEST", "Exclude1", 10.SI(of Ampere), 1, 26.3.SI(of Volt), 1, "")), Throws.InstanceOf(Of System.ArgumentException))


    'End Sub
End Class
