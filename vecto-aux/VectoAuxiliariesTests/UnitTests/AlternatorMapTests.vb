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
<TestCase(10,1500,0.6150f)> _
<TestCase(10,4000,0.6400f)> _
<TestCase(10,7000,0.4750f)> _
<TestCase(63,1500,0.0000f)> _
<TestCase(63,4000,0.7400f)> _
<TestCase(63,7000,0.6580f)> _
<TestCase(136,1500,0.0000f)> _
<TestCase(136,4000,0.6694f)> _
<TestCase(136,7000,0.5953f)> _
Public Sub BoundryTests(ByVal amps as integer, ByVal  rpm as integer,  ByVal expected as single)

   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )
   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency
   Assert.AreEqual(expected, actual)

End Sub

<TestCase(55,1500,0.15576f)> _
<TestCase(55,7000,0.6304f)> _
<TestCase(10,3000,0.63f)> _
<TestCase(136,3000,0.3347f)> _
Public sub FourCornerWithInterpolatedOtherTest(ByVal amps as integer, ByVal  rpm as integer,  ByVal expected as single)

   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )

   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency

   Assert.AreEqual(expected, actual)
End Sub

<TestCase(18,1750,0.656323552f)>   _
<TestCase(18,6500,0.5280294f)>   _
<TestCase(130,1750,0.0f)>  _
<TestCase(130,6500,0.6150136f)>  _
Public sub InterpolatedCornersBothMidPreviousTest(ByVal amps as integer, ByVal  rpm as integer,  ByVal expected as single)

   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )

   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency

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