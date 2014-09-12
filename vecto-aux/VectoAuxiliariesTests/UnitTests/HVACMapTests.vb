Imports NUnit.Framework
Imports VectoAuxiliaries.Hvac


Namespace UnitTests

    <TestFixture()> Public Class HVACMapTests
#Region "Test Values"
        Dim goodMap As String = "TestFiles\TestHvacMap.csv"
        Const badMapWrongColumns As String = "TestFiles\testHvacMapWrongColumns.csv"
        Const badMapInvalidRows As String = "TestFiles\testHvacMapInvalidRowData.csv"
        Const badMapFileNotExist As String = "TestFiles\fileNotExist.csv"

        Private Function GetGoodMap() As HVACMap
            Return New HVACMap(goodMap)
        End Function

        Private Function GetInitialisedMap() As HVACMap
            Dim map As HVACMap = GetGoodMap()
            map.Initialise()
            Return map
        End Function

#End Region

        <Test()>
        Public Sub CreateNewTest()
            Dim target As HVACMap = GetGoodMap()
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub InitialiseTest()
            Dim target As HVACMap = GetGoodMap()
            Assert.IsTrue(target.Initialise())
        End Sub

        <TestCase(badMapWrongColumns)>
        <ExpectedException("System.ArgumentException")>
        Public Sub InitialiseInvalidColumnCountTest(ByVal path As String)
            Dim target As HVACMap = New HVACMap(path)
            Assert.IsTrue(target.Initialise())
        End Sub

        <TestCase(badMapInvalidRows)>
        <ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidRowDataTest(ByVal path As String)
            Dim target As HVACMap = New HVACMap(path)
            Assert.IsTrue(target.Initialise())
        End Sub

        <TestCase(badMapFileNotExist)>
        <ExpectedException("System.ArgumentException")>
        Public Sub InitialiseFileNotExistTest(ByVal path As String)
            Dim target As HVACMap = New HVACMap(path)
            Assert.IsTrue(target.Initialise())
        End Sub


        <TestCase(3, 1, 24)>
        <TestCase(3, 2, 192)>
        <TestCase(3, 3, 24)>
        <TestCase(3, 4, 192)>
        Public Sub GetMechanicalDemandTest(ByVal region As Integer, ByVal season As Integer, result As Integer)
            Dim target As HVACMap = GetInitialisedMap()
            Dim expected As Integer = result
            Dim actual As Integer = target.GetMechanicalDemand(region, season)
            Assert.AreEqual(expected, actual)
        End Sub

        <TestCase(3, 1, 12)>
        <TestCase(3, 2, 48)>
        <TestCase(3, 3, 12)>
        <TestCase(3, 4, 48)>
        Public Sub GetElectricalDemandTest(ByVal region As Integer, ByVal season As Integer, result As Integer)
            Dim target As HVACMap = GetInitialisedMap()
            Dim expected As Integer = result
            Dim actual As Integer = target.GetElectricalDemand(region, season)
            Assert.AreEqual(expected, actual)
        End Sub

        <TestCase(3, 6)> _
        <TestCase(6, 3)> _
        <ExpectedException("System.ArgumentException")>
        Public Sub GetElectricalDemandKeyNotPresentTest(ByVal region As Integer, ByVal season As Integer)
            Dim target As HVACMap = GetInitialisedMap()
            Dim actual As Integer = target.GetElectricalDemand(region, season)
        End Sub

    End Class


End Namespace


