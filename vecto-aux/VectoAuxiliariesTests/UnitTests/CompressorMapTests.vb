Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics

Namespace UnitTests

    <TestFixture()>
    Public Class CompressorMapTests

        Private Const GOODMAP As String = "TestFiles\testCompressorMap.csv"
        Private Const INVALIDPOWERCOMPRESSORONMAP As String = "TestFiles\testCompressorMapInvalidOnPower.csv"
        Private Const INVALIDPOWERCOMPRESSOROFFMAP As String = "TestFiles\testCompressorMapInvalidOffPower.csv"
        Private Const INVALIDFLOWRATEMAP As String = "TestFiles\testCompressorMapInvalidFlow.csv"
        Private Const INSSUFICIENTROWSMAP As String = "TestFiles\testCompressorMapNotEnoughRows.csv"
        Private Const INVALIDRPMMAP As String = "TestFiles\testCompressorMapInvalidRpm.csv"
        Private Const INVALIDNUMBEROFCOLUMNS As String = "TestFiles\testCompressorMapWrongNumberOfColumns.csv"


#Region "Helpers"

        Private Function GetInitialiseMap() As CompressorMap
            Dim target As CompressorMap = GetMap()
            target.Initialise()
            Return target
        End Function

        Private Function GetMap() As CompressorMap
            Dim path As String = GOODMAP
            Dim target As CompressorMap = New CompressorMap(path)
            Return target
        End Function

#End Region

        <Test()>
        Public Sub CreateNewCompressorMapInstanceTest()
            Dim pat As String = "test"
            Dim target As CompressorMap = New CompressorMap(pat)
        End Sub


        <Test()>
        Public Sub InitialisationTest()
            Dim target As CompressorMap = GetMap()
            Assert.IsTrue(target.Initialise())
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialisationNoFileSuppliedThrowsExceptionTest()
            Dim path As String = ""
            Dim target As CompressorMap = New CompressorMap(path)
            Assert.IsTrue(target.Initialise())
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialisationWrongNumberOfColumnsThrowsExceptionTest()
            Dim path As String = INVALIDNUMBEROFCOLUMNS
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialisationInvalidRpmThrowsExceptionTest()
            Dim path As String = INVALIDRPMMAP
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialisationInvalidFlowRateThrowsExceptionTest()
            Dim path As String = INVALIDFLOWRATEMAP
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialisationInvalidPowerCompressorOnThrowsExceptionTest()
            Dim path As String = INVALIDPOWERCOMPRESSORONMAP
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.InvalidCastException")>
        Public Sub InitialisationInvalidPowerCompressorOffThrowsExceptionTest()
            Dim path As String = INVALIDPOWERCOMPRESSOROFFMAP
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialisationInsufficientRowsThrowsExceptionTest()
            Dim path As String = INSSUFICIENTROWSMAP
            Dim target As CompressorMap = New CompressorMap(path)
            target.Initialise()
        End Sub


        <Test()>
        Public Sub GetFlowRateKeyPassedTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 400
            Dim value As Single = target.GetFlowRate(2000)
            Assert.AreEqual(expected, value)
        End Sub

        <Test()>
        Public Sub GetFlowRateInterpolaitionTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 500
            Dim value As Single = target.GetFlowRate(2500)
            Assert.AreEqual(expected, value)
        End Sub

        <TestCase(50)> _
        <TestCase(8000)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub GetFlowRateRpmOutOfRangeThrowExceptionTest(ByVal rpm As Integer)
            Dim target As CompressorMap = GetInitialiseMap()
            Dim value As Single = target.GetFlowRate(rpm)
        End Sub


        <Test()>
        Public Sub GetPowerCompressorOnKeyPassedTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 4
            Dim value As Single = target.GetPowerCompressorOn(2000)
            Assert.AreEqual(expected, value)
        End Sub

        <Test()>
        Public Sub GetPowerCompressorOnInterpolaitionTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 5
            Dim value As Single = target.GetPowerCompressorOn(2500)
            Assert.AreEqual(expected, value)
        End Sub

        <TestCase(50)> _
        <TestCase(550)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub GetPowerCompressorOnRpmOutOfRangeThrowExceptionTest(ByVal rpm As Integer)
            Dim target As CompressorMap = GetInitialiseMap()
            Dim value As Single = target.GetPowerCompressorOn(rpm)
        End Sub


        <Test()>
        Public Sub GetPowerCompressorOffKeyPassedTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 2
            Dim value As Single = target.GetPowerCompressorOff(2000)
            Assert.AreEqual(expected, value)
        End Sub

        <Test()>
        Public Sub GetPowerCompressorOffInterpolaitionTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 2.5
            Dim value As Single = target.GetPowerCompressorOff(2500)
            Assert.AreEqual(expected, value)
        End Sub

        <TestCase(50)> _
        <TestCase(550)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub GetPowerCompressorOffRpmOutOfRangeThrowExceptionTest(ByVal rpm As Integer)
            Dim target As CompressorMap = GetInitialiseMap()
            Dim value As Single = target.GetPowerCompressorOff(rpm)
        End Sub


        <Test>
        Public Sub GetAveragePowerDemandPerCompressorUnitFlowRate()

            Dim target As CompressorMap = GetInitialiseMap()

            Dim expected As Single
            Dim actual As Single

            expected = 0.01

            actual = target.AveragePowerDemandPerCompressorUnitFlowRate

            Assert.AreEqual(expected, actual)

        End Sub


    End Class

End Namespace