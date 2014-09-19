Imports System.Configuration
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework


Namespace UnitTests

    <TestFixture()> Public Class AverageHVACLoadDemandTests
#Region "Helpers"
        Private Function GetAlternatorMock() As IAlternator
            Dim alt As IAlternator = New AlternatorMock()
            Return alt
        End Function

        Private Function GetHVACMapMock() As IHVACMap
            Dim map As IHVACMap = New HVACMapMock()
            Return map
        End Function

        Private Function GetAverageHVACLoadDemandIntance() As AverageHVACLoadDemand
            Dim alt As IAlternator = GetAlternatorMock()
            Dim map As IHVACMap = GetHVACMapMock()
            Dim target As AverageHVACLoadDemand = New AverageHVACLoadDemand(map, alt, Nothing)
            Return target
        End Function

        Private Function GetInitialisedAverageHVACLoadDemandIntance() As AverageHVACLoadDemand
            Dim alt As IAlternator = GetAlternatorMock()
            Dim map As IHVACMap = GetHVACMapMock()
            Dim target As AverageHVACLoadDemand = New AverageHVACLoadDemand(map, alt, Nothing)
            target.Initialise()
            Return target
        End Function

#End Region

        <Test()> Public Sub NewTest()
            Dim target As AverageHVACLoadDemand = GetAverageHVACLoadDemandIntance()
            Assert.IsNotNull(target)
            Assert.IsInstanceOf(GetType(AverageHVACLoadDemand), target)
        End Sub


        <Test()> Public Sub InitialiseTest()
            Dim target As AverageHVACLoadDemand = GetAverageHVACLoadDemandIntance()
            Assert.IsTrue(target.Initialise())
        End Sub

        <Test()> Public Sub AverageMechanicalPowerAtCrankTest()
            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Integer = 10
            Dim actual As Integer = target.AverageMechanicalPowerDemandAtCrank(100)
        End Sub

        <Test()> Public Sub AverageElectricalPowerAtAlternatorTest()
            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Integer = 10
            Dim actual As Integer = target.AverageMechanicalPowerDemandAtCrank(100)
        End Sub

        <Test()> Public Sub AverageElectricalPowerAtCrankTest()
            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Integer = 10
            Dim actual As Integer = target.AverageMechanicalPowerDemandAtCrank(100)
        End Sub

    End Class


End Namespace


