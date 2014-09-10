
Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliariesTests.Mocks

Namespace UnitTests


    <TestFixture()>
    Public Class AlternatorTests

#Region "Test Constants"

        ''' <summary>
        ''' Implemented range is [1.25 - 5.5]
        ''' </summary>
        Const GoodRatio As Single = 3.0

        ''' <summary>
        ''' Implemented range is [1.25 - 5.5]
        ''' </summary>
        Const TooLowRatio As Single = 0.0
        Private Const TooHighRatio As Single = 6.0

        ''' <summary>
        ''' Implemented range is [0.25 - 0.95]
        ''' </summary>
        Private Const GoodEfficiency As Single = 0.5

        ''' <summary>
        ''' Implemented range is [0.25 - 0.95]
        ''' </summary>
        Private Const TooLowEfficiency As Single = 0
        Private Const TooHighEfficiency As Single = 1

        Private Function GetNonFailingMapMock() As IAlternatorMap
            Return New AlternatorMapMock(False)
        End Function

        Private Function GetFailingMapMock() As IAlternatorMap
            Return New AlternatorMapMock(True)
        End Function

        Private Function GetGoodAlternator() As Alternator
            Dim value As Alternator = New Alternator(GetNonFailingMapMock(), GoodRatio, GoodEfficiency)
            Return value
        End Function

#End Region

        <Test()>
        Public Sub CreateNewAlternatorJustPathTest()
            Dim target As Alternator = New Alternator(GetNonFailingMapMock())
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub CreateNewAlternatorAllParametersTest()
            Dim target As Alternator = New Alternator(GetNonFailingMapMock(), GoodRatio, GoodEfficiency)
            Assert.IsNotNull(target)
        End Sub

        <TestCase(TooLowRatio)> _
        <TestCase(TooHighRatio)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub CreateNewAlternatorInvalidRatioTest(ByVal ratio As Single)
            Dim target As Alternator = New Alternator(GetFailingMapMock(), ratio, GoodEfficiency)
        End Sub

        <TestCase(TooLowEfficiency)> _
        <TestCase(TooHighEfficiency)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub CreateNewAlternatorInvalidEfficiencyTest(ByVal efficiency As Single)
            Dim target As Alternator = New Alternator(GetNonFailingMapMock(), GoodRatio, efficiency)
        End Sub


        <Test()>
        Public Sub InitialiseTest()
            Dim target As Alternator = New Alternator(GetNonFailingMapMock())
            Dim result As Boolean = target.Initialise()
            Assert.IsTrue(result)
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub InitialiseInvalidMapTest()
            Dim target As Alternator = New Alternator(GetFailingMapMock())
            Dim result As Boolean = target.Initialise()
        End Sub


        <Test()>
        Public Sub GetEfficiencyTest()
            Dim target As Alternator = GetGoodAlternator()
            target.Initialise()
            Dim expected As Single = 0.5
            Dim actual As Single = target.GetEfficiency(100)
            Assert.AreEqual(actual, expected)
        End Sub


        <Test()>
        Public Sub GetMaximumRegenerationPowerTest()
            Dim target As Alternator = GetGoodAlternator()
            target.Initialise()
            Dim expected As Single = 100
            Dim actual As Single = target.GetMaximumRegenerationPower(500)
            Assert.AreEqual(expected, actual)
        End Sub


        <Test()>
        Public Sub GetMaximumRegeneratinPowerAtCrankTest()
            'TODO: is a simple value returned, no exception test ok here, should we be checking specific values?
            Dim target As Alternator = GetGoodAlternator()
            target.Initialise()
            Dim expected As Single = 100
            Dim actual As Single = target.GetMaximumRegeneratinPowerAtCrank(500)
            Assert.AreEqual(expected, actual)
        End Sub


        <Test()>
        Public Sub SetGearRatioTest()
            Dim target As Alternator = GetGoodAlternator()
            Dim expected As Single = 1.25
            target.PulleyGearRatio = expected
            Dim actual As Single = target.PulleyGearRatio
            Assert.AreEqual(actual, expected)
        End Sub

        <TestCase(TooLowRatio)> _
        <TestCase(TooHighRatio)>
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub SetGearRatioOutOfRange(ByVal ratio As Single)
            Dim target As Alternator = New Alternator(GetNonFailingMapMock())
            target.PulleyGearRatio = ratio
        End Sub

        <Test()>
        Public Sub GetGearRatioTest()
            Dim target As Alternator = New Alternator(GetNonFailingMapMock(), GoodRatio, GoodEfficiency)
            Dim expected As Single = GoodRatio
            Dim actual As Single = target.PulleyGearRatio
            Assert.AreEqual(actual, expected)
        End Sub


        <Test()>
        Public Sub SetGearEfficiencyTest()
            Dim target As Alternator = GetGoodAlternator()
            Dim expected As Single = GoodEfficiency
            target.PulleyGearEfficiency = expected
            Dim actual As Single = target.PulleyGearEfficiency
            Assert.AreEqual(actual, expected)
        End Sub

        <TestCase(TooLowEfficiency)> _
        <TestCase(TooHighEfficiency)> _
        <ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub SetGearEfficiencyTooHighTest(ByVal efficiency As Single)
            Dim target As Alternator = New Alternator(GetNonFailingMapMock())
            target.PulleyGearEfficiency = efficiency
        End Sub

        <Test()>
        Public Sub GetGearEfficiencyTest()
            Dim target As Alternator = New Alternator(GetNonFailingMapMock(), GoodRatio, GoodEfficiency)
            Dim expected As Single = GoodEfficiency
            Dim actual As Single = target.PulleyGearEfficiency
            Assert.AreEqual(actual, expected)
        End Sub

    End Class
End Namespace