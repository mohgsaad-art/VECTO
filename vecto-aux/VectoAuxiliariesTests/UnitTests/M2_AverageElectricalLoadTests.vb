Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks
imports VectoAuxiliaries

Namespace UnitTests

    <TestFixture()>
    Public Class M2_AverageElectricalDemandTests

    Private signals As ISignals = New Signals

    Private Const csngDoorDutyCycleZeroToOne As Single = 0.0963391136801541
    Private Const csngPowernetVoltage As Single = 26.3
    Private ssmHVac As IHVACSteadyStateModel = New HVACSteadyStateModel(100,100,100)



#Region "Helpers"
        Private Function GetAverageElectricalDemandInstance() As M2_AverageElectricalLoadDemand

            signals.EngineSpeed=2000


            Dim consumers As IElectricalConsumerList = CType(New ElectricalConsumerList(26.3,0.096, True), IElectricalConsumerList)
            Dim hvacInp As IHVACInputs = CType(New HVACInputs(1, 1), IHVACInputs)
            Dim hvacmap As IHVACMap = CType(New HVACMap("testfiles\TestHvacMap.csv"), IHVACMap)
            hvacmap.Initialise()
            Dim altMap As IAlternatorMap = CType(New AlternatorMap("testfiles\testAlternatorMap.aalt"), IAlternatorMap)
            altMap.Initialise()
            Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(consumers,hvacInp, altMap, 26.3,signals,ssmHVac)

            'Get Consumers.



            Return New M2_AverageElectricalLoadDemand(consumers, m0, 0.8, 26.3,signals)


        End Function
#End Region

        <Test()>
        Public Sub NewTest()
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Assert.IsNotNull(target)
        End Sub




         <Test()>
        Public Sub GetAveragePowerAtAlternatorTest()


            Dim expected As Single =937.1157
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Dim actual As Single = target.GetAveragePowerDemandAtAlternator()
            Assert.AreEqual(expected,actual)

        End Sub

        <Test()>
        Public Sub GetAveragePowerAtCrankTest()
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Dim expected As Single = 2302.69116
            Dim actual As Single = target.GetAveragePowerAtCrankFromElectrics()
            Assert.AreEqual(expected, actual)
        End Sub

    End Class
End Namespace