Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics


Namespace UnitTests

    <TestFixture()>
    Public Class PneumaticConsumerTests
#Region "Helpers"

        Private Const GoodName As String = "Test"
        Private Const GoodVolume As Single = 10.0
        Private Const BadVolume As Single = 0.0
        Private Const BadName As String = ""

        Public Function GetGoodConsumer() As PneumaticConsumer
            Return New PneumaticConsumer(GoodName, GoodVolume)
        End Function

#End Region


        <Test()>
        Public Sub CreateNewTest()
            Dim target As PneumaticConsumer = GetGoodConsumer()
            Assert.IsNotNull(target)
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub CreateNewInvalidNameTest()
            Dim target As PneumaticConsumer = New PneumaticConsumer(BadName, GoodVolume)
        End Sub

        <Test(), ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub CreateNewInvalidVolumeTest()
            Dim target As PneumaticConsumer = New PneumaticConsumer(GoodName, BadVolume)
        End Sub


        <Test()>
        Public Sub GetNameTest()
            Dim target As PneumaticConsumer = GetGoodConsumer()
            Dim expected As String = GoodName
            Dim actual As String = target.Name
            Assert.AreEqual(expected, actual)
        End Sub

        <Test()>
        Public Sub GetTotalVolumeTest()
            Dim target As PneumaticConsumer = GetGoodConsumer()
            Dim expected As Single = GoodVolume * 50
            Dim actual As Single = target.GetTotalVolume(50)
            Assert.AreEqual(expected, actual)
        End Sub

        <Test()>
        Public Sub GetVolumeTest()
            Dim target As PneumaticConsumer = GetGoodConsumer()
            Dim expected As Single = GoodVolume
            Dim actual As Single = target.VolumePerCycle
            Assert.AreEqual(expected, actual)
        End Sub

    End Class
End Namespace


