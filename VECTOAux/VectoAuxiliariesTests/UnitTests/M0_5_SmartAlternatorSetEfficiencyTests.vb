Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils

Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests
    <TestFixture()>
    Public Class M0_5_SmartAlternatorSetEfficiencyTests
        Private target As IM0_5_SmartAlternatorSetEfficiency
        Private signals As Signals = New Signals

       
		Public Sub New()
		    Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <OneTimeSetUp>
        Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)

            Initialise()
        end Sub

        Private Function GetSSM() As ISSMTOOL


            Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
            'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

            dim ssmInput As ISSMDeclarationInputs = SSMInputData.ReadFile(_SSMMAP, utils.GetDefaultVehicleData(), Nothing)
            'CType(CType(ssmInput, SSMInputs).Vehicle, VehicleData).Height = 0.SI (Of Meter)
            Dim ssm As ISSMTOOL = New SSMTOOL(ssmInput)


            'ssm.Load(_SSMMAP)


            Return ssm
        End Function

  
        Private Sub Initialise()


            Dim ssm As ISSMTOOL = GetSSM()
            
            signals.EngineSpeed = 2000.RPMtoRad()
           
            Dim auxConfig = Utils.GetAuxTestConfig()
            'Dim  hvacMap As New HVACMap("testFiles\TestHvacMap.csv")
            'hvacMap.Initialise()
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage = 26.3.SI (Of Volt)
            Dim m0 As New M00Impl(auxConfig.ElectricalUserInputsConfig, signals, ssm.ElectricalWAdjusted)

            'Results Cards
            Dim readings = New List(Of SmartResult)
            readings.Add(New SmartResult(10.SI (of Ampere), 8.SI (of Ampere)))
            readings.Add(New SmartResult(70.SI (of Ampere), 63.SI (of Ampere)))

            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardIdle = New ResultCard(readings)
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardTraction = New ResultCard(readings)
            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ResultCardOverrun = New ResultCard(readings)

            
            target = New M0_5Impl(m0, auxConfig.ElectricalUserInputsConfig, signals)
        End Sub

        <Test()>
        Public Sub CreateNewTest()
            Initialise()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub SmartIdleCurrentTest()
            Initialise()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub SmartTractionCurrentTest()
            Initialise()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub SmartOverrunCurrentTest()
            Initialise()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub AlternatorsEfficiencyIdle2000rpmTest()
            Initialise()

            Dim expected As Double =  0.62 '0.6308339
            Dim actual As Double = target.AlternatorsEfficiencyIdleResultCard()

            Assert.AreEqual(expected, actual, 0.000001)
        End Sub


        <Test()>
        Public Sub AlternatorsEfficiencyTraction2000rpmTest()
            Initialise()

            Dim expected As Double = 0.62 '0.6308339
            Dim actual As Double = target.AlternatorsEfficiencyTractionOnResultCard()

            Assert.AreEqual(expected, actual, 0.000001)
        End Sub


        <Test()>
        Public Sub AlternatorsEfficiencyOverrun2000rpmTest()
            Initialise()

            Dim expected As Double = 0.62 '0.6308339
            Dim actual As Double = target.AlternatorsEfficiencyOverrunResultCard()

            Assert.AreEqual(expected, actual, 0.000001)
        End Sub
    End Class
End Namespace


