

Imports NUnit.Framework

Namespace UnitTests
	<TestFixture()>
	Public Class ElectricalConsumerTests

#Region "Helpers"

		'Public Function GetGoodConsumer() As ElectricalConsumer
		'	Return New ElectricalConsumer(False, "Doors", "Doors per Door", 0.5, 1)
		'End Function

#End Region


		'<Test()>
		'Public Sub CreateNewTest()
		'	Dim target As ElectricalConsumer = GetGoodConsumer()
		'	Assert.IsNotNull(target)
		'End Sub

		''  <Test(), ExpectedException("System.ArgumentException")>

		'<Test()>
		'Public Sub ZeroLengthConsumerNameTest()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "Doors", "", 0.5, 1), Throws.InstanceOf(Of ArgumentException))

		'End Sub

		'<Test()>
		'Public Sub ZeroLengthCategoryNameTest_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 0.5, 1), Throws.InstanceOf(Of ArgumentException))
		'End Sub


		''TooLow     NominalConsumption
		''TooHigh    NominalConsumption


		'<Test()>
		'Public Sub ToLow_PhaseIdleTractionOn_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionOnMin - 1, 1), Throws.InstanceOf(Of ArgumentException))

		'End Sub

		'<Test()>
		'Public Sub ToHigh_PhaseIdleTractionOn_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", Constants.BusAuxiliaries.ElectricConstants.PhaseIdleTractionMax + 1, 1), Throws.InstanceOf(Of ArgumentException))
		'End Sub


		'<Test()>
		'Public Sub ToLowNumberInVehicle_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 0.5, -1), Throws.InstanceOf(Of ArgumentException))
		'End Sub

		''TooLow     PowerNetVoltage
		'<Test()>
		'Public Sub ToLowPowerNetVoltageTest_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 0.5, 1), Throws.InstanceOf(Of ArgumentException))

		'End Sub

		''TooHigh    PowerNetVoltage
		'<Test()>
		'Public Sub ToHighPowerNetVoltageTest_ThrowsArgumentException()
		'	Dim target As ElectricalConsumer
		'	Assert.That(Sub() target = New ElectricalConsumer(False, "", "Doors per Door", 0.5, 1), Throws.InstanceOf(Of ArgumentException))

		'End Sub
	End Class
End Namespace