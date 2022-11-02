
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests
	<TestFixture()>
	Public Class M2_AverageElectricalDemandTests
		Private signals As ISignals = New Signals

		Private Const csngDoorDutyCycleZeroToOne As Single = 0.0963391136801541
		Private Const csngPowernetVoltage As Single = 26.3
		

		Private Function GetSSM() As ISSMTOOL


			Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
			'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

			Dim ssm As ISSMTOOL = New SSMTOOL(SSMInputData.ReadFile(_SSMMAP, Utils.GetDefaultVehicleData(), Nothing))
		    'CType(CType(ssm.SSMInputs, SSMInputs).Vehicle, VehicleData).Height = 0.SI(of Meter)

			'ssm.Load(_SSMMAP)


			Return ssm
		End Function

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

#Region "Helpers"

		Private Function GetAverageElectricalDemandInstance() As IM2_AverageElectricalLoadDemand

			signals.EngineSpeed = 2000.RPMtoRad()

            Dim auxConfig = Utils.GetAuxTestConfig()
            'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height = 0.SI(of Meter)

			Dim altMap As IAlternatorMap = AlternatorReader.ReadMap( "testfiles\testAlternatorMap.aalt")
			'altMap.Initialise()
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = 26.3.SI(Of Volt)
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap =altMap
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorGearEfficiency = 0.8

			Dim m0 As New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, New SSMTOOL(auxConfig.SSMInputs).ElectricalWAdjusted)

			'Get Consumers.


			Return New M02Impl(m0, auxConfig.ElectricalUserInputsConfig, signals)
		End Function

#End Region

		<Test()>
		Public Sub NewTest()
			Dim target As IM2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
			Assert.IsNotNull(target)
		End Sub


		'<Test()>
		'Public Sub GetAveragePowerAtAlternatorTest()


		'	Dim expected As Single = 1594.61572
		'	Dim target As IM2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
		'	Dim actual As Watt = target.GetAveragePowerDemandAtAlternator()
		'	Assert.AreEqual(expected, actual.Value(), 0.001)
		'End Sub

		<Test()>
		Public Sub GetAveragePowerAtCrankTest()
			Dim target As IM2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
			Dim expected As Single = 10914.6543
			Dim actual As Watt = target.GetAveragePowerAtCrankFromElectrics()
			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub
	End Class
End Namespace