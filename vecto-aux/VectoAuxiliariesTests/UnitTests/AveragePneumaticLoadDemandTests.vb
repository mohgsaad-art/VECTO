
Imports VectoAuxiliaries.Pneumatics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks


Namespace UnitTests

    <TestFixture>
    Public Class AveragePneumaticLoadDemandTests


        Private Const MAP_GOOD As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap - GoodMap.csv"

        Private map As IAirFlowRateMechanicalDemandMap
        Private noConsumers As New List(Of IPneumaticConsumer)
        Private threeConsumers As New List(Of IPneumaticConsumer)



        'Constructors
        Public Sub New()

            initialise()

        End Sub


        Private Sub initialise()

            map = New AirFlowRateMechanicalDemandMapMock()
            map.Initialise()

            threeConsumers.Add(New PneumaticConsumer("Doors", 100))
            threeConsumers.Add(New PneumaticConsumer("HairDrier", 200))
            threeConsumers.Add(New PneumaticConsumer("BalloonInflater", 300))

        End Sub


        <Test>
        Public Sub AveragePowerDemandAtCrank_ThreeConsumers()

            Dim target = New AveragePneumaticLoadDemand(map, 10, 0.5, threeConsumers)

            Dim result As Single = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Dim expected As Single = 7666.666

            Assert.AreEqual(result, expected)

        End Sub

        <Test>
        Public Sub TotalAirDeliveryRate_ThreeConsumers()

            Dim target = New AveragePneumaticLoadDemand(map, 10, 0.5, threeConsumers)

            Dim result As Single = target.GetTotalRequiredAirPerCompressorUnitDeliveryRate()

            Dim expected As Single = 600

            Assert.AreEqual(result, expected)

        End Sub

        <Test>
        Public Sub TotalAirDeliveryRate_NoConsumers()

            Dim target = New AveragePneumaticLoadDemand(map, 10, 0.5, Nothing)

            Dim result As Single = target.GetTotalRequiredAirPerCompressorUnitDeliveryRate()

            Dim expected As Single = 600

            Assert.AreEqual(result, 0)

        End Sub

        <Test>
        Public Sub AveragePowerDemandAtCrank_NoConsumers()

            Dim target = New AveragePneumaticLoadDemand(map, 10, 0.5, Nothing)

            Dim result As Single = target.GetAveragePowerDemandAtCrankFromPneumatics()

            Dim expected As Single = 0

            Assert.AreEqual(result, expected)

        End Sub


    End Class



End Namespace


