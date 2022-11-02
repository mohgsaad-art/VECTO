Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports System.IO
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports Signals = TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.Signals

Namespace UnitTests
    <TestFixture()>
    Public Class M0_NonSmart_AlternatorsSetEfficiencyTests
        Private Const cstrAlternatorsEfficiencyMapLocation As String = "tests\testAlternatorMap.aalt"
        Private Const cstrHVACMapLocation As String = "TestFiles\TestHvacMap.csv"
        Private Const cstrAlternatorMap As String = "TestFiles\testAlternatorMap.aalt"

        'Private elecConsumers As IElectricalConsumerList

        Private alternatorMap As IAlternatorMap
        Private signals As Signals = New Signals
        Private powernetVoltage As Volt = 26.3.SI (Of Volt)()

        Private Function GetSSM() As ISSMTOOL

            Const _SSMMAP As String = "TestFiles\ssm.Ahsm"

            Dim auxconfig = Utils.GetAuxTestConfig()

            Dim ssm As SSMTOOL = New SSMTOOL(auxconfig.SSMInputs) _
            ', New HVACConstants())
            'CType(CType(ssm.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            'ssm.Load(_SSMMAP)

            Return ssm
        End Function

        Public Sub New()

            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)

            signals.EngineSpeed = 2000.RPMtoRad()

            'Setup consumers and HVAC ( 1 Consumer in Test Category )

            'Dim list = New List(Of IElectricalConsumer)()
            'Dim consumer = New ElectricalConsumer(False, "TEST", "CONSUMER1", 0.5, 1)
            'list.Add(consumer)
            'elecConsumers = CType(New ElectricalConsumerList(list), IElectricalConsumerList)

            'Alternator Map
            alternatorMap = AlternatorReader.ReadMap(cstrAlternatorMap)
            'alternatorMap.Initialise()
        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub CreateNewTest()

            Dim auxConfig = utils.GetAuxTestConfig()
            'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = powernetVoltage
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap =alternatorMap

            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(auxConfig.ElectricalUserInputsConfig,
                                                                              signals,  New SSMTOOL(auxconfig.SSMInputs).ElectricalWAdjusted)
            Assert.IsNotNull(target)
        End Sub

        '<Test()>
        'Public Sub CreateNew_MissingElecConsumers_ThrowArgumentExceptionTest()

        '    Dim target As IM0_NonSmart_AlternatorsSetEfficiency
        '    Dim auxConfig = utils.GetAuxTestConfig()
        '    'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
        '    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = powernetVoltage
        '    CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap =alternatorMap

        '    Assert.That(Sub() target = New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, New SSMTOOL(auxconfig.SSMInputs)),
        '                Throws.InstanceOf (Of ArgumentException))
        'End Sub

        <Test()>
        Public Sub CreateNew_MissingAlternatorMap_ThrowArgumentExceptionTest()
            Dim target As IM0_NonSmart_AlternatorsSetEfficiency
            Dim auxConfig = utils.GetAuxTestConfig()
            'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = powernetVoltage
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap = Nothing

            Assert.That(Sub() target = New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, New SSMTOOL(auxconfig.SSMInputs).ElectricalWAdjusted),
                        Throws.InstanceOf (Of ArgumentException))
        End Sub

        <Test()>
        Public Sub EfficiencyValueTest()

            Dim auxConfig = utils.GetAuxTestConfig()

            'CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ElectricalConsumers = elecConsumers
            'CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AverageCurrentDemandInclBaseLoad = 0.5.SI(Of Ampere)
            'CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AverageCurrentDemandInclBaseLoad = 0.5.SI(Of Ampere)
            'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)

            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = powernetVoltage
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap =alternatorMap


            Dim target As M00Impl = New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, New SSMTOOL(auxconfig.SSMInputs).ElectricalWAdjusted)

            Dim actual As Double = target.AlternatorsEfficiency

            Dim expected As Single = 0.62

            Assert.AreEqual(expected, actual, 0.001)
        End Sub

        <Test()>
        Public Sub HVAC_PowerDemandAmpsTest()
            Dim auxConfig = utils.GetAuxTestConfig()
            'CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = powernetVoltage
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap =alternatorMap


            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, New SSMTOOL(auxconfig.SSMInputs).ElectricalWAdjusted)

            Dim actual As Ampere
            Dim expected As Single = 0

            actual = target.GetHVACElectricalCurrentDemand()

            Assert.AreEqual(expected, actual.Value(), 0.001)
        End Sub
    End Class
End Namespace


