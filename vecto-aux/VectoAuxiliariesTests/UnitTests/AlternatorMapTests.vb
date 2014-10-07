Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliaries

Namespace UnitTests

    <TestFixture()>
    Public Class AlternatorMapTests

        Private Const _GOODMAP As String = "TestFiles\testAlternatorMap.csv"
        Private Const _INVALIDRPMMAP As String = "TestFiles\testAlternatorMapWithInvalidRpm.csv"
        Private Const _INVALIDAMPSMAP As String = "TestFiles\testAlternatorMapWithInvalidAmps.csv"
        Private Const _IVALIDEFFICIENCYMAP As String = "TestFiles\testAlternatorMapWithInvalidEfficiency.csv"
        Private Const _INVALIDPOWERMAP As String = "TestFiles\testAlternatorMapWithInvalidPower.csv"

        <Test()>
        Public Sub CreateNewAlternatorMapInstanceTest()
            Dim path As String = "test"
            Dim target As AlternatorMap = New AlternatorMap(path)

            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub InitialiseTest()
            Dim target As AlternatorMap = GetMap()
            Dim actual As Boolean = target.Initialise()
            Assert.IsTrue(actual)
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialiseNoFileSuppliedThrowsExceptionTest()
            Dim path As String = ""
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialiseWrongNumberOfColumnsThrowsExceptionTest()
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMapWrongNoOfColumns.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialiseInsufficientRowsThrowsExceptionTest()
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMapNotEnoughRows.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub


        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidRpmThrowsExceptionTest()
            Dim path As String = _INVALIDRPMMAP
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidAmpsThrowsExceptionTest()
            Dim path As String = _INVALIDAMPSMAP
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidEfficiencyThrowsExceptionTest()
            Dim path As String = _IVALIDEFFICIENCYMAP
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <TestCase(2300, 15)> _
        Public Sub GetEfficiencyInterpolationTest(ByVal rpm As Integer, ByVal amps As Integer)

            Dim target As AlternatorMap = GetInitialisedMap()
            Dim expected As Single = 0.6444162
            Dim value As Single = target.GetEfficiency(rpm, amps).Efficiency
            Assert.AreEqual(expected, value)
            Assert.AreEqual(expected, value)

        End Sub


        <TestCase(0, 0)> _
        <TestCase(0, 1500)> _
        <TestCase(10, 0)> _
        <TestCase(136, 8000)> _
        <TestCase(200, 7000)> _
        <ExpectedException("System.ArgumentOutOfRangeException")> _
        Public Sub GetEfficiencyRpmOutOfRangeThrowsExceptionTest(ByVal amps As Integer, ByVal rpm As Integer)

            Dim target As AlternatorMap = GetInitialisedMap()
            Dim value As AlternatorMapValues = target.GetEfficiency(rpm, amps)

        End Sub

        <TestCase(10, 1500)> _
        Public Sub GetEfficiencyOnLowerBoundary(ByVal amps As Integer, ByVal rpm As Integer)

            Dim target As AlternatorMap = GetInitialisedMap()
            Dim actual As Single = target.GetEfficiency(rpm, amps).Efficiency
            Dim expected As Single = 0.615
            Assert.AreEqual(expected, actual)

        End Sub
        <TestCase(27, 2000)> _
        Public Sub GetEfficiencyOnInterimBoundary(ByVal amps As Integer, ByVal rpm As Integer)
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim actual As Single = target.GetEfficiency(rpm, amps).Efficiency
            Dim expected As Single = 0.7

            Assert.AreEqual(expected, actual)

        End Sub
        <TestCase(136, 7000)> _
          Public Sub GetEfficiencyTopBoundary(ByVal amps As Integer, ByVal rpm As Integer)

            Dim target As AlternatorMap = GetInitialisedMap()
            Dim actual As Single = target.GetEfficiency(rpm, amps).Efficiency
            Dim expected As Single = 0.5953

             Assert.AreEqual(expected, actual)

        End Sub


#Region "Helpers"

        Private Function GetInitialisedMap() As AlternatorMap
            Dim target As AlternatorMap = GetMap()
            target.Initialise()
            Return target
        End Function

        Private Function GetMap() As AlternatorMap
            Dim path As String = _GOODMAP
            Dim target As AlternatorMap = New AlternatorMap(path)
            Return target
        End Function

#End Region

    End Class

End Namespace