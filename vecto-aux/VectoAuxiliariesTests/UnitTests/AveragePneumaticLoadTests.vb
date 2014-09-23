Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics

Namespace UnitTests

    <TestFixture()>
    Public Class AveragePneumaticLoadDemandMAPTests

        Private Const MAP_GOOD As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap - GoodMap.csv"
        Private Const MAP_INVALIDELEMENTS As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap_Invalid Elements.csv"
        Private Const MAP_INVALIDKEY As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap_InvalidKey.csv"
        Private Const MAP_NOTENOUGHELEMENTS As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap_NotEnoughElements.csv"
        Private Const MAP_NOTENOUGHROWS As String = "TestFiles\testPneumaticAirFlowRateToMechanicalDemandMap_NotEnoughRows.csv"
        Private Const MAP_FILENOTFOUND As String = "File_NotFound.csv"


        <Test>
        Public Sub CreateNewFlowMechPowerMap()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_GOOD)
            Assert.IsNotNull(target)

        End Sub

        <Test>
        Public Sub InitialiseTest()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_GOOD)
            Assert.IsTrue(target.Initialise())

        End Sub

        <Test>
        <ExpectedException("System.ArgumentException")>
        Public Sub InvalidElementsFlowMechPowerMap()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_INVALIDELEMENTS)
            Assert.IsTrue(target.Initialise())

        End Sub

        <Test>
        <ExpectedException("System.ArgumentException")>
        Public Sub NotEnoughElementsFlowMechPowerMap()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_NOTENOUGHELEMENTS)
            Assert.IsTrue(target.Initialise())

        End Sub

        <Test>
       <ExpectedException("System.ArgumentException")>
        Public Sub NotEnoughRowsFlowMechPowerMap()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_NOTENOUGHROWS)
            Assert.IsTrue(target.Initialise())

        End Sub

        <Test>
       <ExpectedException("System.ArgumentException")>
        Public Sub FileNotFoundFlowMechPowerMap_ThrowArgumentException()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_FILENOTFOUND)
            Assert.IsTrue(target.Initialise())

        End Sub

        <Test>
        <ExpectedException("System.ArgumentException")>
        Public Sub InvalidKeyFlowRatePowerMap_ThrowArgumentOutOfRangeException()

            Dim target As New AirFlowRateMechanicalDemandMap(MAP_GOOD)
            Assert.IsTrue(target.Initialise())

            target.GetPower(-100)

        End Sub


    End Class


End Namespace