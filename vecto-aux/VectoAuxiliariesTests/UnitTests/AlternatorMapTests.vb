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
Public sub FourCornerWithInterpolatedOtherTest(ByVal amps as single, ByVal  rpm as single,  ByVal expected as single)

   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )

   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency

   Assert.AreEqual(expected, actual)
End Sub

<TestCase(18,1750,0.656323552f)>   _
<TestCase(18,6500,0.5280294f)>   _
<TestCase(130,1750,0.0f)>  _
<TestCase(130.5f,6500,0.6144f)>  _
Public sub InterpolatedCornersBothMidPreviousTest(ByVal amps as single, ByVal  rpm as single,  ByVal expected as single)

   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )

   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency

   Assert.AreEqual(expected, actual)

End Sub


<Test()> _
<TestCase(18.5f,  1750, 0.6587500f)>   _
<TestCase(40,     1750, 0.4736750f)>   _
<TestCase(58,     1750, 0.1602250f)>   _
<TestCase(65.5f,  1750, 0.2095500f)>   _
<TestCase(96.5f,  1750, 0.1730000f)>   _
<TestCase(130.5f, 1750, 0.0000000f)>   _
<TestCase(18.5f,  3000, 0.6580250f)>   _
<TestCase(40,     3000, 0.5983000f)>   _
<TestCase(58,     3000, 0.4768250f)>   _
<TestCase(65.5f,  3000, 0.5783500f)>   _
<TestCase(96.5f,  3000, 0.5268000f)>   _
<TestCase(130.5f, 3000, 0.3373500f)>   _
<TestCase(18.5f,  5000, 0.6054750f)>   _
<TestCase(40,     5000, 0.6572500f)>   _
<TestCase(58,     5000, 0.7006000f)>   _
<TestCase(65.5f,  5000, 0.7151250f)>   _
<TestCase(96.5f,  5000, 0.6870250f)>   _
<TestCase(130.5f, 5000, 0.6505750f)>   _
<TestCase(18.5f,  6500, 0.5296250f)>   _
<TestCase(40,     6500, 0.5982500f)>   _
<TestCase(58,     6500, 0.6557000f)>   _
<TestCase(65.5f,  6500, 0.6814250f)>   _		
<TestCase(96.5f,  6500, 0.6561750f)>   _
<TestCase(130.5f, 6500, 0.6144000f)>   _
Public Sub InterpolatedAllMidPointsTest( byval amps As Single, byval rpm As Single, byval expected As single)


   Dim map As IAlternatorMap = GetInitialisedMap()
   Dim target As IAlternatorMap = GetInitialisedMap( )

   Dim actual As Single = map.GetEfficiency( rpm,amps).Efficiency

   Assert.AreEqual(expected, CType(Math.Round(actual,6),Single))


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