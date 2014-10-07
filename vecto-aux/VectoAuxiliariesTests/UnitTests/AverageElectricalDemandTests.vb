Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks

Namespace UnitTests

    <TestFixture()>
    Public Class AverageElectricalDemandTests

#Region "Helpers"
        Private Function GetAverageElectricalDemandInstance() As AverageElectricalDemand
            Dim alt As IAlternator = New AlternatorMock
            Dim consumers As List(Of IElectricalConsumer) = New List(Of IElectricalConsumer)()
            consumers.Add(New ElectricalConsumerMock)
            consumers.Add(New ElectricalConsumerMock)
            Return New AverageElectricalDemand(consumers, 26.3)
        End Function
#End Region

        <Test()>
        Public Sub NewTest()
            Dim target As AverageElectricalDemand = GetAverageElectricalDemandInstance()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub InitialiseTest()
            Dim target As AverageElectricalDemand = GetAverageElectricalDemandInstance()
            Assert.IsTrue(target.Initialise())
        End Sub

        <Test()>
        Public Sub GetElectricalConsumersTest()
            Dim target As AverageElectricalDemand = GetAverageElectricalDemandInstance()
            Assert.Fail()
        End Sub

        <Test()>
        Public Sub GetElectricalConsumersContainsSpecificConsumerTest()
            Dim alt As IAlternator = New AlternatorMock
            Dim consumers As List(Of IElectricalConsumer) = New List(Of IElectricalConsumer)()
            Dim mock As ElectricalConsumerMock = New ElectricalConsumerMock
            consumers.Add(mock)
            Dim target As AverageElectricalDemand = New AverageElectricalDemand(consumers, 26.3)

            'Assert.IsTrue(target.ElectricalConsumers.Contains(mock))
            Assert.Fail()

        End Sub


        <Test()>
        Public Sub GetAlternatorTest()
            Dim alt As IAlternator = New AlternatorMock
            Dim consumers As List(Of IElectricalConsumer) = New List(Of IElectricalConsumer)()
            Dim mock As ElectricalConsumerMock = New ElectricalConsumerMock
            consumers.Add(mock)
            Dim target As AverageElectricalDemand = New AverageElectricalDemand(consumers, 26.3)

            'Assert.AreSame(alt, target.Alternator) TODO: CHECK
            Assert.Fail()
        End Sub


        <Test()>
        Public Sub GetAveragePowerAtAlternatorTest()

            Dim target As AverageElectricalDemand = GetAverageElectricalDemandInstance()
            Dim actual As Single = target.GetAveragePowerDemandAtAlternator()
            Assert.AreEqual(actual, 200)

        End Sub

        <Test()>
        Public Sub GetAveragePowerAtCrankTest()
            Dim target As AverageElectricalDemand = GetAverageElectricalDemandInstance()
            Dim expected As Single = 400.0
            Dim actual As Single = target.GetAveragePowerAtCrank(100)
            Assert.AreEqual(expected, actual)
        End Sub

    End Class
End Namespace