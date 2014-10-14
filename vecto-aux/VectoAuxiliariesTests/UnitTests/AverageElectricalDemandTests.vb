Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks

Namespace UnitTests

    <TestFixture()>
    Public Class AverageElectricalDemandTests

#Region "Helpers"
        Private Function GetAverageElectricalDemandInstance() As M2_AverageElectricalLoadDemand
            Dim alt As IAlternator = New AlternatorMock
            Dim consumers As IElectricalConsumerList = CType(New ElectricalConsumerList(), IElectricalConsumerList)

            Return New M2_AverageElectricalLoadDemand(consumers, 26.3)
        End Function
#End Region

        <Test()>
        Public Sub NewTest()
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Assert.IsNotNull(target)
        End Sub


        <Test()>
        Public Sub GetElectricalConsumersTest()
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Assert.Fail()
        End Sub

        <Test()>
        Public Sub GetElectricalConsumersContainsSpecificConsumerTest()
            Dim alt As IAlternator = New AlternatorMock
            Dim consumers As List(Of IElectricalConsumer) = New List(Of IElectricalConsumer)()
            Dim mock As ElectricalConsumerMock = New ElectricalConsumerMock
            consumers.Add(mock)
            Dim target As M2_AverageElectricalLoadDemand = New M2_AverageElectricalLoadDemand(consumers, 26.3)

            'Assert.IsTrue(target.ElectricalConsumers.Contains(mock))
            Assert.Fail()

        End Sub


        <Test()>
        Public Sub GetAveragePowerAtAlternatorTest()

            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Dim actual As Single = target.GetAveragePowerDemandAtAlternator()
            Assert.AreEqual(actual, 200)

        End Sub

        <Test()>
        Public Sub GetAveragePowerAtCrankTest()
            Dim target As M2_AverageElectricalLoadDemand = GetAverageElectricalDemandInstance()
            Dim expected As Single = 400.0
            Dim actual As Single = target.GetAveragePowerAtCrank(100)
            Assert.AreEqual(expected, actual)
        End Sub

    End Class
End Namespace