
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports TUGraz.VectoCore.Models.Declaration

Namespace UnitTests
	<TestFixture()>
	Public Class M5_SmartAlternatorSetGenerationTests
		'Constants
		Private Shared ReadOnly _powerNetVoltage As Volt = 26.3.SI(of Volt)
		Private Const _hvacMap As String = "testFiles\TestHvacMap.csv"
		Private Const _altMap As String = "testFiles\testAlternatormap.aalt"
		Private Const _rpm As Integer = 2000
		Private Const _altGearPullyEfficiency As Single = 0.8

		'Private fields
		Private _m05 As IM0_5_SmartAlternatorSetEfficiency
		Private _target As IM5_SmartAlternatorSetGeneration
		Private _signals As ISignals = New Signals
		

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

		Private Function GetSSM() As ISSMTOOL


			Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
			'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

            Dim auxConfig = Utils.GetAuxTestConfig()
		    
			Dim ssm As ISSMTOOL = New SSMTOOL(auxConfig.SSMInputs)
                'New SSMTOOL(SSMInputData.ReadFile(_SSMMAP, DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions, DeclarationData.BusAuxiliaries.SSMTechnologyList)) ', New HVACConstants())

		    

			'ssm.Load(_SSMMAP)


			Return ssm
		End Function

		Private Sub Initialise()

			_signals.EngineSpeed = 2000.RPMtoRad()

			Dim elecConsumers  = DeclarationData.BusAuxiliaries.DefaultElectricConsumerList

			Dim alternatoMap   = AlternatorReader.ReadMap(_altMap)
			
            Dim auxConfig = Utils.GetAuxTestConfig()
		    'CType(CType(auxConfig.SSMInputs,SSMInputs).Vehicle, VehicleData).Height = 0.SI(of Meter)
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = _powerNetVoltage
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap = alternatoMap

            Dim m0 As New M00Impl(auxConfig.ElectricalUserInputsConfig, _signals,
                                New SSMTOOL(auxConfig.SSMInputs).ElectricalWAdjusted)

			'Results Cards
			Dim readings = New List(Of SmartResult)
			readings.Add(New SmartResult(10.SI(Of Ampere), 8.SI(Of Ampere)))
			readings.Add(New SmartResult(70.SI(Of Ampere), 63.SI(Of Ampere)))

		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardIdle = New ResultCard(readings)
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardTraction = New ResultCard(readings)
		    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardOverrun = New ResultCard(readings)

			Dim signals As ISignals = New Signals
			signals.EngineSpeed = 2000.RPMtoRad()

			_m05 = New M0_5Impl(m0, auxConfig.ElectricalUserInputsConfig, signals)
		End Sub

		<TestCase()>
		Public Sub CreateNewTest()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage, _altGearPullyEfficiency)
			Assert.IsNotNull(_target)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankIdleWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage, _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791
			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankIdle()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankTractionWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage, _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791
			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankTractionOn()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankOverrunWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage, _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791F

			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankOverrun()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub
	End Class
End Namespace


