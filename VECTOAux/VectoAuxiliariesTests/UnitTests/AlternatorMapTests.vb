Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.ComponentData


Namespace UnitTests
	<TestFixture()>
	Public Class AlternatorMapTests
		Private Const _GOODMAP As String = "TestFiles\testAlternatorMap.aalt"
		Private Const _GOODMAPORIGINALSINGLEMAP As String = "TestFiles\testAlternatorMapOriginalSingleMap.aalt"
		Private Const _INVALIDRPMMAP As String = "TestFiles\testAlternatorMapWithInvalidRpm.aalt"
		Private Const _INVALIDAMPSMAP As String = "TestFiles\testAlternatorMapWithInvalidAmps.aalt"
		Private Const _IVALIDEFFICIENCYMAP As String = "TestFiles\testAlternatorMapWithInvalidEfficiency.aalt"
		Private Const _INVALIDPOWERMAP As String = "TestFiles\testAlternatorMapWithInvalidPower.aalt"
		Private Const _GOODCOMBINEDMAP As String = "TestFiles\testAlternatorMapCombined.aalt"
		Private Const _ASYMETRICALCOMBINEDROWSMAP As String = "TestFiles\testAlternatorMapAsymetricalRowsCombined.aalt"
		Private Const _ASYMETRICALCOMBINEDXYPAIRSMAP As String = "TestFiles\testAlternatorMapAsymetricalXYPairsCombined.aalt"

        <OneTimeSetUp>
        Sub RunBeforeAnyTests()    
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        end Sub


		<Test()>
		<TestCase(10, 1500, 0.615)> _
		<TestCase(10, 4000, 0.64)> _
		<TestCase(10, 7000, 0.475)> _
		<TestCase(63, 1500, 0.0)> _
		<TestCase(63, 4000, 0.74)> _
		<TestCase(63, 7000, 0.658)> _
		<TestCase(136, 1500, 0.0)> _
		<TestCase(136, 4000, 0.6694)> _
		<TestCase(136, 7000, 0.5953)>
		Public Sub BoundryTests(ByVal amps As Integer, ByVal rpm As Integer, ByVal expected As Double)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()
			Dim actual As Double = map.GetEfficiency(rpm.RPMtoRad(), amps.SI(Of Ampere))
			Assert.AreEqual(expected, actual, 1e-6)
		End Sub

		<TestCase(55, 1500, 0.15576)> _
		<TestCase(55, 7000, 0.6304)> _
		<TestCase(10, 3000, 0.63)> _
		<TestCase(136, 3000, 0.3347)>
		Public Sub FourCornerWithInterpolatedOtherTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As Double)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Double = map.GetEfficiency(rpm.RPMtoRad(), amps.SI(Of Ampere))

			Assert.AreEqual(expected, actual, 1e-6)
		End Sub

		<TestCase(18, 1750, 0.656323552)> _
		<TestCase(18, 6500, 0.5280294)> _
		<TestCase(130, 1750, 0.0)> _
		<TestCase(130.5F, 6500, 0.6144)>
		Public Sub InterpolatedCornersBothMidPreviousTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As Double)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Double = map.GetEfficiency(rpm.RPMtoRad(), amps.SI(Of Ampere))

			Assert.AreEqual(expected, actual, 1e-6)
		End Sub


		<Test()> _
		<TestCase(18.5F, 1750, 0.65875)> _
		<TestCase(40, 1750, 0.473675)> _
		<TestCase(58, 1750, 0.160225)> _
		<TestCase(65.5F, 1750, 0.20955)> _
		<TestCase(96.5F, 1750, 0.173)> _
		<TestCase(130.5F, 1750, 0.0)> _
		<TestCase(18.5F, 3000, 0.658025)> _
		<TestCase(40, 3000, 0.5983)> _
		<TestCase(58, 3000, 0.476825)> _
		<TestCase(65.5F, 3000, 0.57835)> _
		<TestCase(96.5F, 3000, 0.5268)> _
		<TestCase(130.5F, 3000, 0.33735)> _
		<TestCase(18.5F, 5000, 0.605475)> _
		<TestCase(40, 5000, 0.65725)> _
		<TestCase(58, 5000, 0.7006)> _
		<TestCase(65.5F, 5000, 0.715125)> _
		<TestCase(96.5F, 5000, 0.687025)> _
		<TestCase(130.5F, 5000, 0.650575)> _
		<TestCase(18.5F, 6500, 0.529625)> _
		<TestCase(40, 6500, 0.59825)> _
		<TestCase(58, 6500, 0.6557)> _
		<TestCase(65.5F, 6500, 0.681425)> _
		<TestCase(96.5F, 6500, 0.656175)> _
		<TestCase(130.5F, 6500, 0.6144)>
		Public Sub InterpolatedAllMidPointsTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As double)


			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Double = map.GetEfficiency(rpm.RPMtoRad(), amps.SI(Of Ampere))

			Assert.AreEqual(expected, actual, 1e-6)
		End Sub


#Region "Helpers"

		Private Function GetInitialisedMap() As IAlternatorMap
			Dim target As IAlternatorMap = GetMap()
			'target.Initialise()
			Return target
		End Function

		Private Function GetMap() As IAlternatorMap
			Dim path As String = _GOODMAP
			Dim target As IAlternatorMap =  AlternatorReader.ReadMap(path) ' New AlternatorMap(path)
			Return target
		End Function

		Private Function GetMap(path As String) As IAlternatorMap
			Dim target As IAlternatorMap = AlternatorReader.ReadMap(path) ' New AlternatorMap(path)
			Return target
		End Function


#End Region
	End Class
End Namespace