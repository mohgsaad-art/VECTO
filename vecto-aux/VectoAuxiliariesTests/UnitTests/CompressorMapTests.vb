Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics

Namespace UnitTests

    <TestFixture()>
    Public Class CompressorMapTests

        Private Const GOODMAP As String = "TestFiles\testCompressorMap.acmp"
        Private Const INVALIDPOWERCOMPRESSORONMAP As String = "TestFiles\testCompressorMapInvalidOnPower.acmp"
        Private Const INVALIDPOWERCOMPRESSOROFFMAP As String = "TestFiles\testCompressorMapInvalidOffPower.acmp"
        Private Const INVALIDFLOWRATEMAP As String = "TestFiles\testCompressorMapInvalidFlow.acmp"
        Private Const INSSUFICIENTROWSMAP As String = "TestFiles\testCompressorMapNotEnoughRows.acmp"
        Private Const INVALIDRPMMAP As String = "TestFiles\testCompressorMapInvalidRpm.acmp"
        Private Const INVALIDNUMBEROFCOLUMNS As String = "TestFiles\testCompressorMapWrongNumberOfColumns.acmp"


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





        <Test()>
        Public Sub GetPowerCompressorOnInterpolaitionTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 5000
            Dim value As Single = target.GetPowerCompressorOn(2500)
            Assert.AreEqual(expected, value)
        End Sub




        <Test()>
        Public Sub GetPowerCompressorOffInterpolaitionTest()
            Dim target As CompressorMap = GetInitialiseMap()
            Dim expected As Single = 2500
            Dim value As Single = target.GetPowerCompressorOff(2500)
            Assert.AreEqual(expected, value)
        End Sub



        <Test()>
        Public Sub InterpMiddle()

            Dim target As CompressorMap = New CompressorMap(GOODMAP)
            Assert.IsTrue(target.Initialise())

            Dim actual = target.GetFlowRate(1750)

            Assert.AreEqual( actual,300)

        End Sub


    End Class

End Namespace