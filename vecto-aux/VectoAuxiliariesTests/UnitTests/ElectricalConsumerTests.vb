Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework

Namespace UnitTests

    <TestFixture()>
    Public Class ElectricalConsumerTests

#Region "Factory Methods"

        Public Function GetGoodConsumer() As ElectricalConsumer
            Return New ElectricalConsumer(GoodName, GoodPower)
        End Function

#End Region

#Region "Test Constants"
        Private Const GoodName As String = "Test"
        Private Const GoodPower As Single = 10.0
        Private BadName As String = String.Empty
        Private Const BadPower As Single = 0.0
#End Region

        <Test()>
        Public Sub CreateNewTest()
            Dim target As ElectricalConsumer = GetGoodConsumer()
            Assert.IsNotNull(target)
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub CreateNewInvalidNameTest()
            Dim target As ElectricalConsumer = New ElectricalConsumer(BadName, 10.0)
        End Sub

        ' TODO: Probably need to define too high a power and implement check
        <TestCase(BadPower)> _
        <TestCase(BadPower)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub CreateNewInvalidPowerTest(ByVal power As Single)
            Dim target As ElectricalConsumer = New ElectricalConsumer(GoodName, power)
        End Sub


        <Test()>
        Public Sub GetNameTest()
            Dim target As ElectricalConsumer = GetGoodConsumer()
            Dim expected As String = GoodName
            Dim actual As String = target.Name
            Assert.AreEqual(expected, actual)
        End Sub

        <Test()>
        Public Sub GetPowerTest()
            Dim target As ElectricalConsumer = GetGoodConsumer()
            Dim expected As Single = GoodPower
            Dim actual As Single = target.Power
            Assert.AreEqual(expected, actual)
        End Sub
    End Class
End Namespace