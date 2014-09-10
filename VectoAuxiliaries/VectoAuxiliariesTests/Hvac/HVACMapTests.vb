Imports VectoAuxiliaries.Hvac
Imports NUnit.Framework

Namespace Hvac
    <TestFixture()> Public Class HVACMapTests

        <Test()>
        Public Sub NewTest()
            Dim target As HVACMap = New HVACMap()
            Assert.IsNotNull(target)
        End Sub

        <TestCase(30, 30)>
        Public Sub GetMechanicalDemandTest(ByVal region As Integer, ByVal season As Integer)
            Dim target As HVACMap = New HVACMap()
            Dim actual As Integer = target.GetMechanicalDemand(region, season)
            Assert.Fail("test implementation not complete - compare to expected value")
        End Sub

        <TestCase(30, 30)>
        Public Sub GetElectricalDemandTest(ByVal region As Integer, ByVal season As Integer)
            Dim target As HVACMap = New HVACMap()
            Dim actual As Integer = target.GetElectricalDemand(region, season)
            Assert.Fail("test implementation not complete - compare to expected value")
        End Sub

        <TestCase(5, 30)> _
        <TestCase(30, 5)> _
        <ExpectedException("System.InvalidArgumentException")>
        Public Sub GetElectricalDemandKeyNotPresentTest(ByVal region As Integer, ByVal season As Integer)
            Dim target As HVACMap = New HVACMap()
            Dim actual As Integer = target.GetElectricalDemand(region, season)
        End Sub

    End Class


End Namespace


