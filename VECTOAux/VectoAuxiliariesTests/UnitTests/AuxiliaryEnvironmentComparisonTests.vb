
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics


Namespace UnitTests
	<TestFixture()>
	Public Class AuxiliaryComparisonTests


		<OneTimeSetUp>
		Sub RunBeforeAnyTests()
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
		end Sub

		shared Function GetDefaultAuxiliaryConfig() As IAuxiliaryConfig
			Dim retVal As IAuxiliaryConfig = Utils.GetAuxTestConfig()

			Return retVal
		End Function

		<Test()>
		Public Sub BasicEqualCompareTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig() 'auxFresh.ShallowCopy()
			Dim compareResult As Boolean

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(True, compareResult)
		End Sub

	   

		<Test()>
		Public Sub BasicUnequalTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean

			'Act
			CType(auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).SmartRegeneration = Not auxNow.PneumaticUserInputsConfig.SmartRegeneration
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

#Region "ElectricalUserConfig"

		<Test()>
		<Category("ElectricalUserConfig")>
		Public Sub ElectricalUserConfig_AlternatorGearEfficiency_UnequalTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType(auxNow.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorGearEfficiency =
				If _
					(auxNow.ElectricalUserInputsConfig.AlternatorGearEfficiency + 0.1 > 1, 1,
					auxNow.ElectricalUserInputsConfig.AlternatorGearEfficiency + 0.1)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_AlternatorMap_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType (auxNow.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorMap = New List(Of ICombinedAlternatorMapRow)()

		'    'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("ElectricalUserConfig")>
		Public Sub ElectricalUserConfig_DoorActuationTimeSecond_UnequalTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.ElectricalUserInputsConfig, ElectricsUserInputsConfig).DoorActuationTimeSecond += 1.SI(Of Second)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Consumers_Unequal_Count_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer)).RemoveAt(0)

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_AvgConsumptionAmps_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'    CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0).AvgConsumptionAmps += 1.SI(of Ampere)

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_BaseVehicle_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0), ElectricalConsumer).BaseVehicle =
		'		Not auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).BaseVehicle

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_Category_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	Dim cat As String = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).Category
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0), ElectricalConsumer).Category = cat & "x"

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_ConsumerName_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	Dim cname As String = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).ConsumerName
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0), ElectricalConsumer).ConsumerName = cname & "x"

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_NominalConsumptionAmps_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0), ElectricalConsumer).NominalConsumptionAmps += 1.SI(of Ampere)

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_NumberInActualVehicle_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	Dim cname As Single = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NumberInActualVehicle
		'	auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).NumberInActualVehicle += 1

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_Consumers_PhaseIdle_TractionOn_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	Dim cname As Double = auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items(0).PhaseIdle_TractionOn
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(of IElectricalConsumer))(0), ElectricalConsumer).PhaseIdle_TractionOn += 1

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnequalCount_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType(auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnequalCount_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnequalCount_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_AMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnEqualAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardIdle_UnEqualSMARTAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 51.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardIdle, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 50.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_AMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(51.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnEqualAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardTraction_UnEqualSMARTAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardTraction, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 50.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_AMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 48.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnEqualAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(50.SI(of Ampere), 49.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		'<Test()>
		'<Category("ElectricalUserConfig")>
		'Public Sub ElectricalUserConfig_Unequal_ResultCardOverrun_UnEqualSMARTAMPS_UnequalTest()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 49.SI(of Ampere)))
		'	CType(CType (auxNow.ElectricalUserInputsConfig.ResultCardOverrun, ResultCard).Results, List(of SmartResult)).Add(New SmartResult(49.SI(of Ampere), 50.SI(of Ampere)))

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("ElectricalUserConfig")>
		Public Sub ElectricalUserConfig_Unequal_PowernetVoltage_UnEqualSMARTAMPS_UnequalTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.ElectricalUserInputsConfig, ElectricsUserInputsConfig).PowerNetVoltage += 1.SI(of Volt)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("ElectricalUserConfig")>
		Public Sub ElectricalUserConfig_Unequal_SmarElectrics_UnEqualSMARTAMPS_UnequalTest()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.ElectricalUserInputsConfig, ElectricsUserInputsConfig).AlternatorType() = Not auxNow.ElectricalUserInputsConfig.AlternatorType()

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

#End Region

#Region "PneumaticAuxiliariesConfig"

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_AdBlueNIperMinute_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).AdBlueInjection += 1.SI(Of NormLiterPerSecond)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_AirControlledSuspensionNIperMinute_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).AirControlledSuspension += 1.SI(of NormLiterPerSecond)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_BrakingNoRetarderNIperKG_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).Braking += 1.SI(of NormLiterPerKilogram)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		'<Test()>
		'<Category("PneumaticAuxiliariesConfig")>
		'Public Sub PneumaticsAuxuiliaryConfig_BrakingWithRetarderNIperKG_Enequal()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'    CType (auxNow.PneumaticAuxillariesConfig, PneumaticsAuxilliariesConfig).BrakingWithRetarderNIperKG += 1.SI(Of NormLiterPerKilogram)

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_BreakingPerKneelingNIperKGinMM_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).BreakingWithKneeling += 1.SI(Of NormLiterPerKilogramMeter)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_DeadVolBlowOutsPerLitresperHour_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).DeadVolBlowOuts += 1.SI(Of PerSecond)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_DeadVolumeLitres_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).DeadVolume += 1.SI(of NormLiter)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_NonSmartRegenFractionTotalAirDemand_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).NonSmartRegenFractionTotalAirDemand += 1

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_OverrunUtilisationForCompressionFraction_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).OverrunUtilisationForCompressionFraction += 1

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_PerDoorOpeningNI_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).DoorOpening += 1.SI(Of NormLiter)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsAuxuiliaryConfig_PerStopBrakeActuationNIperKG_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).StopBrakeActuation += 1.SI(of NormLiterPerKilogram)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticAuxiliariesConfig")>
		Public Sub PneumaticsUserInputsConfig_SmartRegenFractionTotalAirDemand_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticAuxillariesConfig, PneumaticsConsumersDemand).SmartRegenFractionTotalAirDemand += 1

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

#End Region

#Region "PneumaticUserInputsConfig"

		'<Test()>
		'<Category("PneumaticUserInputsConfig")>
		'Public Sub PneumaticUserInputsConfig_ActuationsMap_Enequal()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'    CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).ActuationsMap.Remove(new ActuationsKey("Brakes","Urban" ))
  '          ' = auxNow.PneumaticUserInputsConfig.ActuationsMap & "x"

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_AdBlueDosing_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).AdBlueDosing = ConsumerTechnology.Unknown ' auxNow.PneumaticUserInputsConfig.AdBlueDosing & "x"

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_AirSuspensionControl_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).AirSuspensionControl = ConsumerTechnology.Unknown ' auxNow.PneumaticUserInputsConfig.AirSuspensionControl & "x"

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_CompressorGearEfficiency_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).CompressorGearEfficiency =
				If _
					(auxNow.PneumaticUserInputsConfig.CompressorGearEfficiency - 0.1 < 0, 0,
					auxNow.PneumaticUserInputsConfig.CompressorGearEfficiency - 0.1)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)


			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_CompressorGearRatio_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).CompressorGearRatio += 1

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)
			Assert.AreEqual(False, compareResult)
		End Sub

		'<Test()>
		'<Category("PneumaticUserInputsConfig")>
		'Public Sub PneumaticUserInputsConfig_CompressorMap_Enequal()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean


		'    CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).CompressorMap.RemoveAt(0) ' = auxNow.PneumaticUserInputsConfig.CompressorMap & "x"
		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)


		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_Doors_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).Doors = ConsumerTechnology.Unknown ' auxNow.PneumaticUserInputsConfig.Doors & "x"

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)


			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_KneelingHeightMillimeters_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).KneelingHeight += 1.SI(Of Meter)

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)
			Assert.AreEqual(False, compareResult)
		End Sub

		'<Test()>
		'<Category("PneumaticUserInputsConfig")>
		'Public Sub PneumaticUserInputsConfig_RetarderBrake_Enequal()

		'	'Arrange
		'	Dim auxFresh = GetDefaultAuxiliaryConfig()
		'	Dim auxNow = GetDefaultAuxiliaryConfig()
		'	Dim compareResult As Boolean
		'    CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).RetarderBrake = Not auxNow.PneumaticUserInputsConfig.RetarderBrake

		'	'Act
		'	compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)
		'	Assert.AreEqual(False, compareResult)
		'End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_SmartAirCompression_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).SmartAirCompression = Not auxNow.PneumaticUserInputsConfig.SmartAirCompression

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)


			Assert.AreEqual(False, compareResult)
		End Sub

		<Test()>
		<Category("PneumaticUserInputsConfig")>
		Public Sub PneumaticUserInputsConfig_SmartRegeneration_Enequal()

			'Arrange
			Dim auxFresh = GetDefaultAuxiliaryConfig()
			Dim auxNow = GetDefaultAuxiliaryConfig()
			Dim compareResult As Boolean
			CType (auxNow.PneumaticUserInputsConfig, PneumaticUserInputsConfig).SmartRegeneration = Not auxNow.PneumaticUserInputsConfig.SmartRegeneration

			'Act
			compareResult = auxFresh.ConfigValuesAreTheSameAs(auxNow)

			Assert.AreEqual(False, compareResult)
		End Sub

#End Region
	End Class
End Namespace


