Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.ComponentData


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

        Private Function GetInitialiseMap() As ICompressorMap
            Dim path As String = GOODMAP
            Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
            'target.Initialise()
            Return target
        End Function

#End Region

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub CreateNewCompressorMapInstanceTest()
            Dim pat As String = "test"
            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(pat, 1.0, "")
            end sub, throws.instanceof(of Vectoexception))
        End Sub


        '<Test()>
        'Public Sub InitialisationTest()
        '    Dim target As CompressorMap = GetMap()
        '    Assert.IsTrue(target.Initialise())
        'End Sub

        <Test()>
        Public Sub InitialisationNoFileSuppliedThrowsExceptionTest()
            Dim path As String = ""
            'Dim target As ICompressorMap = CompressorMapReader.ReadFile(path)

            Assert.That(sub() 
                dim tmp = CompressorMapReader.ReadFile(path, 1.0, "")
            end sub, Throws.InstanceOf (of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationWrongNumberOfColumnsThrowsExceptionTest()
            Dim path As String = INVALIDNUMBEROFCOLUMNS

            Assert.That(Sub()
                Dim target = CompressorMapReader.ReadFile(path, 1.0, "")
            End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationInvalidRpmThrowsExceptionTest()
            Dim path As String = INVALIDRPMMAP

            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
            End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationInvalidFlowRateThrowsExceptionTest()
            Dim path As String = INVALIDFLOWRATEMAP
            
            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
                        End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationInvalidPowerCompressorOnThrowsExceptionTest()
            Dim path As String = INVALIDPOWERCOMPRESSORONMAP
            
            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
                        End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationInvalidPowerCompressorOffThrowsExceptionTest()
            Dim path As String = INVALIDPOWERCOMPRESSOROFFMAP
           
            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
                        End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InitialisationInsufficientRowsThrowsExceptionTest()
            Dim path As String = INSSUFICIENTROWSMAP
            
            Assert.That(Sub()
                Dim target As ICompressorMap = CompressorMapReader.ReadFile(path, 1.0, "")
                        End Sub, Throws.InstanceOf (Of VectoException))
        End Sub


        <Test()>
        Public Sub GetFlowRateKeyPassedTest()
            Dim target As ICompressorMap = GetInitialiseMap()
            Dim expected As double = 400
            Dim value As NormLiterPerSecond = target.Interpolate(2000.RPMtoRad()).FlowRate
            Assert.AreEqual(expected.SI(Unit.SI.Liter.Per.Minute).cast(of NormLiterPerSecond).value(), value.Value(), 0.001)
        End Sub

        <Test()>
        Public Sub GetFlowRateInterpolaitionTest()
            Dim target As ICompressorMap = GetInitialiseMap()
            Dim expected As Double = 500
            Dim value As NormLiterPerSecond = target.Interpolate(2500.RPMtoRad()).FlowRate
            Assert.AreEqual(expected.SI(Unit.SI.Liter.Per.Minute).cast(of NormLiterPerSecond).value(), value.Value(), 0.001)
        End Sub


        <Test()>
        Public Sub GetPowerCompressorOnInterpolaitionTest()
            Dim target As ICompressorMap = GetInitialiseMap()
            Dim expected As Double = 5000
            Dim value As Watt = target.Interpolate(2500.RPMtoRad()).PowerOn
            Assert.AreEqual(expected, value.Value(), 0.001)
        End Sub


        <Test()>
        Public Sub GetPowerCompressorOffInterpolaitionTest()
            Dim target As ICompressorMap = GetInitialiseMap()
            Dim expected As Double = 2500
            Dim value As Watt = target.Interpolate(2500.RPMtoRad()).PowerOff
            Assert.AreEqual(expected, value.Value(), 0.001)
        End Sub


        <Test()>
        Public Sub InterpMiddle()

            Dim target As ICompressorMap = CompressorMapReader.ReadFile(GOODMAP, 1.0, "")
            'Assert.IsTrue(target.Initialise())

            Dim actual = target.Interpolate(1750.RPMtoRad()).FlowRate

            Assert.AreEqual(300.SI(Unit.SI.liter.Per.Minute).Cast(Of NormLiterPerSecond).Value(), actual.Value(), 0.001)
        End Sub
    End Class
End Namespace