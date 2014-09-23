Imports System.Configuration
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliaries


Namespace UnitTests

    <TestFixture()> Public Class AverageHVACLoadDemandTests
#Region "Helpers"

        Const HVACMAPTestData As String = "TestFiles\HVACMap.vaux"

        Private Function GetAlternatorMock() As IAlternator
            Dim alt As IAlternator = New AlternatorMock()
            alt.PulleyGearEfficiency = 0.8
            alt.PulleyGearRatio = 1.5
            Return alt
        End Function

        Private Function GetHVACMapMock() As IHVACMap
            Dim map As IHVACMap = New HVACMapMock(HVACMAPTestData)
            Return map
        End Function

        Private Function GetAverageHVACLoadDemandIntance() As AverageHVACLoadDemand
            Dim alt As IAlternator = GetAlternatorMock()
            Dim map As IHVACMap = GetHVACMapMock()
            Dim inputs As New HVACInputs(1, 1)
            Dim target As AverageHVACLoadDemand = New AverageHVACLoadDemand(map, alt, inputs)
            Return target
        End Function

        Private Function GetInitialisedAverageHVACLoadDemandIntance() As AverageHVACLoadDemand
            Dim alt As IAlternator = GetAlternatorMock()
            Dim map As IHVACMap = GetHVACMapMock()
            Dim target As AverageHVACLoadDemand = New AverageHVACLoadDemand(map, alt, New HVACInputs(1, 1))
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

        <Test()>
        Public Sub AverageMechanicalPowerDemandAtCrankTest()

            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Single = 62.5
            Dim actual As Single = target.AverageMechanicalPowerDemandAtCrank()

            Assert.AreEqual(expected, actual)

        End Sub

        <Test()> Public Sub AverageElectricalPowerDemandAtAlternatorTest()
            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Integer = 10
            Dim actual As Integer = target.AverageElectricalPowerDemandAtAlternator
            Assert.AreEqual(expected, actual)

        End Sub

        <Test()> Public Sub AverageElectricalPowerAtCrankTest()

            Dim target As AverageHVACLoadDemand = GetInitialisedAverageHVACLoadDemandIntance()
            Dim expected As Single = 25
            Dim actual As Single = target.AverageElectricalPowerDemandAtCrank(100)
            Assert.AreEqual(expected, actual)

        End Sub

    End Class

End Namespace


