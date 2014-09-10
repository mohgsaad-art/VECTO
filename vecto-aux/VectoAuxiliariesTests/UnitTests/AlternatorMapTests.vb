Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliaries

Namespace UnitTests

    <TestFixture()>
    Public Class AlternatorMapTests

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
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMapWithInvalidRpm.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidEfficiencyThrowsExceptionTest()
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMapWithInvalidEfficiency.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialiseInvalidPowerThrowsExceptionTest()
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMapWithInvalidPower.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            target.Initialise()
        End Sub


        <Test()>
        Public Sub GetEfficiencyKeyPassedTest()
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim expected As Single = 0.1
            target.GetEfficiency(100)
            Dim value As Single = target.GetEfficiency(100)
            Assert.AreEqual(expected, value)

            expected = 0.9
            value = target.GetEfficiency(900)
            Assert.AreEqual(expected, value)
        End Sub

        <Test()>
        Public Sub GetEfficiencyInterpolationTest()
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim expected As Single = 0.15
            Dim value As Single = target.GetEfficiency(150)
            Assert.AreEqual(expected, value)

            expected = 0.85
            value = target.GetEfficiency(850)
            Assert.AreEqual(expected, value)
        End Sub

        <TestCase(0)>
        <TestCase(1000)>
        <ExpectedException("System.ArgumentOutOfRangeException")> _
        Public Sub GetEfficiencyRpmOutOfRangeThrowsExceptionTest(ByVal rpm As Integer)
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim value As Single = target.GetEfficiency(rpm)
        End Sub


        <Test()>
        Public Sub GetPowerKeyPassedTest()
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim expected As Single = 250.0
            Dim value As Single = target.GetMaximumRegenerationPower(100)
            Assert.AreEqual(expected, value)

            expected = 0.9
            value = target.GetEfficiency(900)
            Assert.AreEqual(expected, value)
        End Sub

        <Test()>
        Public Sub GetPowerInterpolationTest()
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim expected As Single = 375.0
            Dim value As Single = target.GetMaximumRegenerationPower(150)
            Assert.AreEqual(expected, value)

        End Sub

        <TestCase(0)>
        <TestCase(1000)>
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub GetPowerRpmOutOfRangeThrowsExceptionTest(ByVal rpm As Integer)
            Dim target As AlternatorMap = GetInitialisedMap()
            Dim value As Single = target.GetMaximumRegenerationPower(rpm)
        End Sub

#Region "Helpers"

        Private Function GetInitialisedMap() As AlternatorMap
            Dim target As AlternatorMap = GetMap()
            target.Initialise()
            Return target
        End Function

        Private Function GetMap() As AlternatorMap
            Dim path As String = "C:\DEV\VECTO\VectoAuxiliaries\VectoAuxiliariesTests\TestFiles\testAlternatorMap.csv"
            Dim target As AlternatorMap = New AlternatorMap(path)
            Return target
        End Function

#End Region

    End Class
End Namespace