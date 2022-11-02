
Imports Moq
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Namespace UnitTests
	<TestFixture()>
	Public Class M6Test
		Private M1 As New M1_Mock(100, 200, 300, 50)
		Private M2 As New M2_Mock(120, 130)
		Private M3 As New M3_Mock(200, 5000)
		Private M4 As New M4_Mock(100, 2, 200, 100, 100)
		Private M5 As New M5_Mock(200, 50, 80)
		Private Signals As New Signals()

        Private Function GetAuxConfigDummy() As IAuxiliaryConfig
            Dim auxCfg As New Mock(Of IAuxiliaryConfig)
            Dim elecCfg = New Mock(Of IElectricsUserInputsConfig)
            elecCfg.Setup(Function(x) x.AlternatorType()).Returns(AlternatorType.Conventional)
            Dim psconfig = New Mock(Of IPneumaticUserInputsConfig)
            psconfig.Setup(Function(x) x.SmartAirCompression).Returns(False)
            auxCfg.Setup(Function(x) x.ElectricalUserInputsConfig).Returns(elecCfg.Object)
            auxCfg.Setup(Function(x) x.PneumaticUserInputsConfig).Returns(psconfig.Object)

            return auxCfg.Object
        End Function

		Private Function GetStandardInstanceM6() As IM6

			M1 = New M1_Mock(100, 200, 300, 50)
			M2 = New M2_Mock(120, 130)
			M3 = New M3_Mock(200, 5000)
			M4 = New M4_Mock(100, 2, 200, 100, 100)
			M5 = New M5_Mock(200, 50, 80)

			Return New M06Impl(GetAuxConfigDummy().ElectricalUserInputsConfig, M1, M2, M3, M4, M5, Signals)
		End Function

		Public Sub New()

			Signals.EngineMotoringPower = 100000.SI(Of Watt)()
			Signals.EngineDrivelinePower = 150000.SI(Of Watt)()
			Signals.PreExistingAuxPower = 30000.SI(Of Watt)()
		End Sub

		<Test()>
		Public Sub CreateNewM6Instance()

			Dim target As IM6 = GetStandardInstanceM6()

			Assert.IsNotNull(target)
		End Sub


		'Test Cases Supplied by Mike Preston.
		<Test()> _
		<
			TestCase _
				(100, 100, 100, 100, 20, 20, 40, 100, 100, 100, 0.1F, -0.55, False, False, False, 0, 20, 0, 100, 20, 200, False)> _
		<
			TestCase _
				(100, 100, 100, 100, 20, 20, 40, 100, 100, 100, 100, -550, True, False, False, 0, 20, 0, 100, 20, 200, False)>
		Public Sub MikesConditionsTest(M1_1 As Double,
										M1_2 As Double,
										M2_1 As Double,
										M3_1 As Double,
										M4_1 As Double,
										M4_2 As Double,
										M4_3 As Double,
										M5_1 As Double,
										M5_2 As Double,
										AUX As Double,
										EMP As Double,
										EDP As Double,
										SM As Boolean,
										OUT1 As Boolean,
										OUT2 As Boolean,
										OUT3 As Double,
										OUT4 As Double,
										OUT5 As Double,
										OUT6 As Double,
										OUT7 As Double,
										OUT8 As Double,
										OUT9 As Boolean)


			Dim M1 = New M1_Mock()
			Dim M2 = New M2_Mock()
			Dim M3 = New M3_Mock()
			Dim M4 = New M4_Mock()
			Dim M5 = New M5_Mock()

			Dim signals As New Signals()


			M1._AveragePowerDemandAtCrankFromHVACMechanicalsWatts = M1_1.SI(Of Watt)()
			M1._AveragePowerDemandAtCrankFromHVACElectricsWatts = M1_2.SI(Of Watt)()
			M2._GetAveragePowerAtCrankFromElectrics = M2_1.SI(Of Watt)()
			M3._GetAveragePowerDemandAtCrankFromPneumatics = M3_1.SI(Of Watt)()
			M4._PowerCompressorOff = M4_1.SI(Of Watt)()
			M4._PowerDifference = M4_2.SI(Of Watt)()
			M4._PowerCompressorOn = M4_3.SI(Of Watt)()
			M5._AlternatorsGenerationPowerAtCrankTractionOnWatts = M5_1.SI(Of Watt)()
			M5._AlternatorsGenerationPowerAtCrankOverrunWatts = M5_2.SI(Of Watt)()


			signals.EngineMotoringPower = (EMP * 1000).SI(Of Watt)()
			signals.InternalEnginePower = 0.SI(Of Watt)()
			signals.PreExistingAuxPower = (AUX * 1000).SI(Of Watt)()
			signals.EngineDrivelinePower = (EDP * 1000).SI(Of Watt)()
			'signals.SmartElectrics = SM
            signals.ExcessiveDragPower = ((EMP - EDP) * 1000).SI(of Watt)


			Dim target As New M06Impl(GetAuxConfigDummy().ElectricalUserInputsConfig, M1, M2, M3, M4, M5, signals)

			Assert.AreEqual(OUT1, target.OverrunFlag)
			Assert.AreEqual(OUT2, target.SmartElecAndPneumaticsCompressorFlag)
			Assert.AreEqual(OUT3, target.SmartElecAndPneumaticAltPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT4, target.SmartElecAndPneumaticAirCompPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT5, target.SmartElecOnlyAltPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT6, target.AveragePowerDemandAtCrankFromPneumatics.Value(), 0.001)
			Assert.AreEqual(OUT7, target.SmartElecAndPneumaticAirCompPowerGenAtCrank.Value(), 0.001)
			Assert.AreEqual(OUT8, target.AvgPowerDemandAtCrankFromElectricsIncHVAC.Value(), 0.001)
			Assert.AreEqual(OUT9, target.SmartPneumaticsOnlyCompressorFlag)
		End Sub
	End Class
End Namespace


