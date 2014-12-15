Imports NUnit.Framework
Imports VectoAuxiliaries.Hvac


Namespace UnitTests

    <TestFixture()> Public Class HVACSSMMapTests
#Region "Test Values"
        const goodMap As String = "TestFiles\HVACSteadyStateModelGOODMAP.ahsm"
        Const badHeaders As String = "TESTFILES\HVACSteadyStateModelBADHEADERS.ahsm"
        Const badValues As String = "TestFiles\HVACSteadyStateModelBADVALUES.ahsm"
        Const insufficientLines As String = "TestFiles\HVACSteadyStateModelInsufficientLines.ahsm"
        Const badMapFileNotExist As String = "TestFiles\fileNotExist.csv"

        Private Function GetMap() As  HVACSteadyStateModel

            Dim target As New   HVACSteadyStateModel()
            Return target

        End Function


#End Region


        <Test()>
        Public Sub InitialiseTestWithValues()
            Dim message As String = String.Empty
            Dim expected As boolean = true
            Dim actual As Boolean
            Dim target = GetMap

            actual = target.SetValuesFromMap(goodMap, message)
            Assert.AreEqual(expected, actual)
            Assert.AreEqual(target.HVACFuellingLitresPerHour,0)
            Assert.AreEqual(target.HVACElectricalLoadPowerWatts,100)
            Assert.AreEqual(target.HVACMechanicalLoadPowerWatts,100)

        End Sub

        <Test()>
        Public Sub BadHeaderTest()
            Dim message As String = String.Empty
            Dim expected As boolean = false
            Dim actual As Boolean
            Dim target = GetMap

            actual = target.SetValuesFromMap( badHeaders, message)
            Assert.AreEqual(expected, actual)


        End Sub

        <Test()>
        Public Sub BadValuesTest()
            Dim message As String = String.Empty
            Dim expected As boolean = false
            Dim actual As Boolean
            Dim target = GetMap

            actual = target.SetValuesFromMap( badValues, message)
            Assert.AreEqual(expected, actual)


        End Sub


        <Test()>
        Public Sub InsufficientLinesTest()
            Dim message As String = String.Empty
            Dim expected As boolean = false
            Dim actual As Boolean
            Dim target = GetMap

            actual = target.SetValuesFromMap(  badMapFileNotExist, message)
            Assert.AreEqual(expected, actual)


        End Sub

        <Test()>
        Public Sub BadFilePathNotExistTest()
            Dim message As String = String.Empty
            Dim expected As boolean = false
            Dim actual As Boolean
            Dim target = GetMap

            actual = target.SetValuesFromMap(  insufficientLines, message)
            Assert.AreEqual(expected, actual)


        End Sub


    End Class

End Namespace


