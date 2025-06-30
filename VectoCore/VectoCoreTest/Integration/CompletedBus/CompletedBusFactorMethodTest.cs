using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Ninject;
using NLog.LayoutRenderers;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Formatting = Newtonsoft.Json.Formatting;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{

	[TestFixture]
	//[Parallelizable(ParallelScope.All)] --> job file executed more than 20 runs in parallel, which produces out-of-memory if run parallel
	public class CompletedBusFactorMethodTest
	{
		const string JobFile_Group41 = @"TestData/Integration/Buses/FactorMethod/CompletedBus_41-32b.vecto";
		const string JobFile_Group42 = @"TestData/Integration/Buses/FactorMethod/CompletedBus_42-33b.vecto";

		const string JobFilePrimary41 = @"TestData/Integration/Buses/FactorMethod/primary_heavyBus group41_nonSmart.xml";
		const string JobFilePrimary42 = @"TestData/Integration/Buses/FactorMethod/primary_heavyBus group42_SmartPS.xml";

		protected IXMLInputDataReader xmlInputReader;

		class RelatedRun
		{
			public VectoRunData VectoRunDataSpezificBody { get; set; }
			public VectoRunData VectoRunDataGenericBody { get; set; }

		}

		//private List<RelatedRun> relatedRuns;
		

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			//relatedRuns = new List<RelatedRun>();

			var kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = kernel.Get<IXMLInputDataReader>();
			//SetBusSegments();
		}


		private void SetBusSegments()
		{
			PrimaryBusSegment();
			CompletedBusSegment();
		}

		private void PrimaryBusSegment()
		{
			var category = VehicleCategory.HeavyBusPrimaryVehicle;
			var axleConfiguration = AxleConfiguration.AxleConfig_4x2;
			//var floorType = FloorType.HighFloor;
			var articulated = false;

			//primarySegment = DeclarationData.PrimaryBusSegments.Lookup(category, axleConfiguration, articulated);
		}

		private void CompletedBusSegment()
		{
			var numberOfAxles = 2;
			var vehicleCode = VehicleCode.CB;
			var registrationClass = RegistrationClass.II_III;
			var passengersLowerDeck = 30;
			var bodyHeight = 3.SI<Meter>();
			var lowEntry = false;

			//completedSegment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, registrationClass, passengersLowerDeck, bodyHeight, lowEntry);
		}

		[TestCase()]
		public void TestCompletedBus()
		{
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(JobFile_Group41), Path.GetFileName(JobFile_Group41)));
			var inputData = CompletedVIF.CreateCompletedVif(
				JSONInputDataFactory.ReadJsonJob(JobFile_Group41) as JSONInputDataCompletedBusFactorMethodV7,
				xmlInputReader);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;

			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
			//{
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			//var sumContainer = new SummaryDataContainer(writer);
			//var jobContainer = new JobContainer(sumContainer);


			var runs = factory.RunDataFactory.NextRun().ToList();
			Assert.IsTrue(runs.Count == 8 || runs.Count == 12);

			var relatedRuns = SetRelatedVehicleParts(runs);

			for (int i = 0; i < relatedRuns.Count; i++)
			{
				AssertVehicleData(relatedRuns[i],i);
				AssertAirdragData(relatedRuns[i]);
				AssertEngineData(relatedRuns[i]);
				AssertGearbox(relatedRuns[i]);
				AssertTorqueConverter(relatedRuns[i]);
				AssertAxlegearData(relatedRuns[i]);
				AssertAngledriveData(relatedRuns[i]);
				AssertAngledriveData(relatedRuns[i]);
				AssertAuxiliaryData(relatedRuns[i]);
				AssertElectricalUserInputConfig(relatedRuns[i], i);
				AssertPneumaticUserInputsConfig(relatedRuns[i]);
				AssertPneumaticConsumerDemand(relatedRuns[i]);
				AssertSSMBusParameters(relatedRuns[i], i);
				AssertTechnologyBenefits(relatedRuns[i]);
				AssertBoundaryConditions(relatedRuns[i]);
				AssertEnvironmentalConditions(relatedRuns[i]);
				AssertSSMInputs(relatedRuns[i], i);
				AssertRetarder(relatedRuns[i]);
				AssertDriverData(relatedRuns[i]);
			}

		}

		#region Vehicle Data Asserts

		private void AssertVehicleData(RelatedRun relatedRun, int currentIndex)
		{
			var genericVehicleData = relatedRun.VectoRunDataGenericBody.VehicleData;
			var specificVehicleData = relatedRun.VectoRunDataSpezificBody.VehicleData;

			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, genericVehicleData.AxleConfiguration);
			Assert.AreEqual(genericVehicleData.AxleConfiguration, specificVehicleData.AxleConfiguration);

			Assert.AreEqual(13150, genericVehicleData.CurbMass.Value());
			Assert.AreEqual(8300, specificVehicleData.CurbMass.Value());

			Assert.AreEqual(0, genericVehicleData.BodyAndTrailerMass.Value());
			Assert.AreEqual(genericVehicleData.BodyAndTrailerMass, specificVehicleData.BodyAndTrailerMass);

			AssertLoading(genericVehicleData.Loading, specificVehicleData.Loading, currentIndex);

			Assert.AreEqual(0.4992, genericVehicleData.DynamicTyreRadius.Value(), 1e-0);
			Assert.AreEqual(genericVehicleData.DynamicTyreRadius, specificVehicleData.DynamicTyreRadius);

			AssertADASData(genericVehicleData.ADAS, specificVehicleData.ADAS);

			AssertAxles(genericVehicleData.AxleData, specificVehicleData.AxleData);
		}

		private void AssertLoading(Kilogram genericLoading, Kilogram specificLoading, int index)
		{
			switch (index) {
				// generic loading values shall match expected values of primary vehicle for IU and CO cycle
				// see TestPrimaryBusGroup41Test
				case 0:
					Assert.AreEqual(1075.437, genericLoading.Value(), 1e-4);
					Assert.AreEqual(1058.5088, specificLoading.Value(), 1e-4);
					break;
				case 1:
					Assert.AreEqual(3519.612, genericLoading.Value(), 1e-4);
					Assert.AreEqual(2130, specificLoading.Value(), 1e-0);
					break;
				case 2:
					Assert.AreEqual(1094.9904, genericLoading.Value(), 1e-2);
					Assert.AreEqual(1077.7544, specificLoading.Value(), 1e-4);
					break;
				case 3:
					Assert.AreEqual(2737.476, genericLoading.Value(), 1e-2);
					Assert.AreEqual(2130.0, specificLoading.Value(), 1e-0);
					break;
			}
		}

		private void AssertADASData(VehicleData.ADASData genericAdasData, VehicleData.ADASData specificAdasData)
		{
			Assert.AreEqual(false, genericAdasData.EngineStopStart);
			Assert.AreEqual(genericAdasData.EngineStopStart, specificAdasData.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, genericAdasData.EcoRoll);
			Assert.AreEqual(genericAdasData.EcoRoll, specificAdasData.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, genericAdasData.PredictiveCruiseControl);
			Assert.AreEqual(genericAdasData.PredictiveCruiseControl, specificAdasData.PredictiveCruiseControl);
		}

		private void AssertAxles(List<Axle> genericAxles, List<Axle> specificAxles)
		{
			Assert.AreEqual(2, genericAxles.Count);
			Assert.AreEqual(genericAxles.Count, specificAxles.Count);

			for (int i = 0; i < genericAxles.Count; i++)
			{
				Assert.AreEqual("315/70 R22.5", genericAxles[i].WheelsDimension);
				Assert.AreEqual(genericAxles[i].WheelsDimension, specificAxles[i].WheelsDimension);

				Assert.AreEqual(14.9, genericAxles[i].Inertia.Value());
				Assert.AreEqual(genericAxles[i].Inertia, specificAxles[i].Inertia);

				Assert.AreEqual(31300, genericAxles[i].TyreTestLoad.Value());
				Assert.AreEqual(genericAxles[i].TyreTestLoad, specificAxles[i].TyreTestLoad);
				if (i == 0)
				{
					Assert.AreEqual(0.375, genericAxles[i].AxleWeightShare);
					Assert.AreEqual(genericAxles[i].AxleWeightShare, specificAxles[i].AxleWeightShare);
					Assert.AreEqual(AxleType.VehicleNonDriven, genericAxles[i].AxleType);
					Assert.AreEqual(false, genericAxles[i].TwinTyres);
				}
				else if (i == 1)
				{
					Assert.AreEqual(0.625, genericAxles[i].AxleWeightShare);
					Assert.AreEqual(genericAxles[i].AxleWeightShare, specificAxles[i].AxleWeightShare);
					Assert.AreEqual(AxleType.VehicleDriven, genericAxles[i].AxleType);
					Assert.AreEqual(true, genericAxles[i].TwinTyres);
				}
				Assert.AreEqual(genericAxles[i].TwinTyres, specificAxles[i].TwinTyres);
				Assert.AreEqual(genericAxles[i].AxleType, specificAxles[i].AxleType);
			}
		}
		#endregion

		#region Airdrag Data Asserts

		private void AssertAirdragData(RelatedRun relatedRun)
		{
			var genericAirdragData = relatedRun.VectoRunDataGenericBody.AirdragData;
			var specificAirdragData = relatedRun.VectoRunDataSpezificBody.AirdragData;

			var genericDragArea = 3.45.SI<SquareMeter>();
			var specificDragArea = 6.34.SI<SquareMeter>();

			var genericVehicleHeight = 3.45.SI<Meter>();
			var specificVehicleHeight = 3.0.SI<Meter>() + 0.30.SI<Meter>();

			var genericCrosswind = GetCrosswindCorrection("CoachBus", genericDragArea, genericVehicleHeight);
			var specificCrosswind = GetCrosswindCorrection("CoachBus", specificDragArea, specificVehicleHeight);

			var cyclePos = new DrivingCycleData.DrivingCycleEntry() {
				Highway = false
			};
			var genericValueExpected = genericCrosswind.AverageAirDragPowerLoss(cyclePos, 20.KMPHtoMeterPerSecond(),
				21.KMPHtoMeterPerSecond(), Physics.AirDensity).AirdragForce.Value();

			var currentGenericValue = genericAirdragData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(
				cyclePos, 20.KMPHtoMeterPerSecond(), 21.KMPHtoMeterPerSecond(), Physics.AirDensity).AirdragForce.Value();

			var expectedSpecificValue = specificCrosswind.AverageAirDragPowerLoss(cyclePos, 21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).AirdragForce.Value();

			var currentSpecificValue = specificAirdragData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(cyclePos, 21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).AirdragForce.Value();


			Assert.AreEqual(CrossWindCorrectionMode.DeclarationModeCorrection, genericAirdragData.CrossWindCorrectionMode);
			Assert.AreEqual(genericAirdragData.CrossWindCorrectionMode, specificAirdragData.CrossWindCorrectionMode);

			Assert.AreEqual(genericDragArea, genericAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(genericDragArea, genericAirdragData.CrossWindCorrectionCurve.AirDragArea);
			Assert.AreEqual(specificDragArea, specificAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(specificDragArea, specificAirdragData.CrossWindCorrectionCurve.AirDragArea);

			Assert.AreEqual(genericValueExpected, currentGenericValue);
			Assert.AreEqual(expectedSpecificValue, currentSpecificValue);
		}


		#endregion

		#region Engine Data Asserts

		private void AssertEngineData(RelatedRun relatedRun)
		{
			var genericEngine = relatedRun.VectoRunDataGenericBody.EngineData;
			var specificEngine = relatedRun.VectoRunDataSpezificBody.EngineData;

			Assert.AreEqual(7, genericEngine.FullLoadCurves.Count);
			AssertFullLoadAndDragCurve(genericEngine.FullLoadCurves[0].FullLoadEntries);
			AssertFullLoadAndDragCurve(specificEngine.FullLoadCurves[0].FullLoadEntries);

			Assert.AreEqual(700 * Constants.RPMToRad, genericEngine.IdleSpeed.Value(), 1e-9);
			Assert.AreEqual(genericEngine.IdleSpeed, specificEngine.IdleSpeed);

			Assert.AreEqual(7700.SI(Unit.SI.Cubic.Centi.Meter).Value(), genericEngine.Displacement.Value());
			Assert.AreEqual(genericEngine.Displacement, specificEngine.Displacement);

			Assert.AreEqual(
				DeclarationData.Engine.TorqueConverterInertia +
				DeclarationData.Engine.EngineBaseInertia +
				DeclarationData.Engine.EngineDisplacementInertia * genericEngine.Displacement, genericEngine.Inertia);
			Assert.AreEqual(genericEngine.Inertia, specificEngine.Inertia);

			Assert.AreEqual(WHRType.None, genericEngine.WHRType);
			Assert.AreEqual(genericEngine.WHRType, specificEngine.WHRType);

			Assert.AreEqual(null, genericEngine.ElectricalWHR);
			Assert.AreEqual(genericEngine.ElectricalWHR, specificEngine.ElectricalWHR);

			Assert.AreEqual(null, genericEngine.MechanicalWHR);
			Assert.AreEqual(genericEngine.MechanicalWHR, specificEngine.MechanicalWHR);

			Assert.AreEqual(1, genericEngine.Fuels.Count);
			Assert.AreEqual(genericEngine.Fuels.Count, specificEngine.Fuels.Count);

			AssertFuel(genericEngine.Fuels[0]);
			AssertFuel(specificEngine.Fuels[0]);
		}


		private void AssertFullLoadAndDragCurve(List<EngineFullLoadCurve.FullLoadCurveEntry> entries)
		{
			Assert.AreEqual(12, entries.Count);
			AssertFullLoadAndDragCurveEntry(600.00, 546.02, -39.66, entries[0]);
			AssertFullLoadAndDragCurveEntry(800.00, 760.78, -48.83, entries[1]);
			AssertFullLoadAndDragCurveEntry(1000.00, 973.29, -56.44, entries[2]);
			AssertFullLoadAndDragCurveEntry(1200.00, 1092.03, -67.29, entries[3]);
			AssertFullLoadAndDragCurveEntry(1400.00, 1092.03, -77.58, entries[4]);
			AssertFullLoadAndDragCurveEntry(1600.00, 1092.03, -87.88, entries[5]);
			AssertFullLoadAndDragCurveEntry(1800.00, 1022.52, -94.11, entries[6]);
			AssertFullLoadAndDragCurveEntry(2000.00, 944.17, -100.76, entries[7]);
			AssertFullLoadAndDragCurveEntry(2200.00, 868.12, -113.36, entries[8]);
			AssertFullLoadAndDragCurveEntry(2400.00, 741.99, -122.60, entries[9]);
			AssertFullLoadAndDragCurveEntry(2500.00, 647.29, -126.66, entries[10]);
			AssertFullLoadAndDragCurveEntry(2600.00, 0.00, -132.07, entries[11]);
		}

		private void AssertFullLoadAndDragCurveEntry(double engineSpeed, double maxTorque, double dragTorque,
			EngineFullLoadCurve.FullLoadCurveEntry entry)
		{
			Assert.AreEqual(engineSpeed * Constants.RPMToRad, entry.EngineSpeed.Value(), 1e-9);
			Assert.AreEqual(maxTorque.SI<NewtonMeter>(), entry.TorqueFullLoad);
			Assert.AreEqual(dragTorque.SI<NewtonMeter>(), entry.TorqueDrag);
		}

		private void AssertFuel(CombustionEngineFuelData fuel)
		{
			Assert.AreEqual(1, fuel.WHTCMotorway);
			Assert.AreEqual(1.02, fuel.WHTCRural);
			Assert.AreEqual(1.05, fuel.WHTCUrban);
			Assert.AreEqual(1.005, fuel.ColdHotCorrectionFactor);
			Assert.AreEqual(1, fuel.CorrectionFactorRegPer);

			//fuel.ConsumptionMap ???
		}

		#endregion

		#region Gearbox Asserts

		private void AssertGearbox(RelatedRun relatedRun)
		{
			var genericGearbox = relatedRun.VectoRunDataGenericBody.GearboxData;
			var specificGearbox = relatedRun.VectoRunDataSpezificBody.GearboxData;

			Assert.AreEqual(0, genericGearbox.Inertia.Value());
			Assert.AreEqual(genericGearbox.Inertia, specificGearbox.Inertia);

			Assert.AreEqual(0.0.SI<Second>(), genericGearbox.TractionInterruption);
			Assert.AreEqual(specificGearbox.TractionInterruption, genericGearbox.TractionInterruption);

			Assert.AreEqual(6, genericGearbox.Gears.Count);
			Assert.AreEqual(genericGearbox.Gears.Count, specificGearbox.Gears.Count);

			AssertGears(genericGearbox.Gears.Values.ToList());
			AssertGears(specificGearbox.Gears.Values.ToList());
			AssertGearsLossmap(
				genericGearbox.Gears.Values.ToList(), specificGearbox.Gears.Values.ToList());
		}

		private void AssertGears(IList<GearData> gears)
		{
			AssertGear(3.364, 1900.SI<NewtonMeter>(), 2500, gears[0]);
			AssertGear(1.909, 1900.SI<NewtonMeter>(), 2500, gears[1]);
			AssertGear(1.421, null, 2500, gears[2]);
			AssertGear(1.000, null, 2500, gears[3]);
			AssertGear(0.720, null, 2500, gears[4]);
			AssertGear(0.615, null, 2500, gears[5]);
		}

		private void AssertGear(double ratio, NewtonMeter maxTorque, double maxSpeed, GearData gear)
		{
			Assert.AreEqual(ratio, gear.Ratio);
			Assert.AreEqual(maxTorque, gear.MaxTorque);
			Assert.AreEqual(maxSpeed, gear.MaxSpeed.AsRPM);
		}

		private void AssertGearsLossmap(IList<GearData> genericGearData, IList<GearData> specificGearData)
		{
			Assert.AreEqual(6, genericGearData.Count);
			Assert.AreEqual(genericGearData.Count, specificGearData.Count);
			for (int i = 0; i < genericGearData.Count; i++) {
				Assert.IsNotNull(genericGearData[i].LossMap);
				Assert.AreEqual(genericGearData[i].LossMap.LossMapSerialized, specificGearData[i].LossMap.LossMapSerialized);
			}
		}

		
		#endregion

		#region Torque Converter Asserts

		private void AssertTorqueConverter(RelatedRun relatedRun)
		{
			var genericTorqueConverterData= relatedRun.VectoRunDataGenericBody.GearboxData.TorqueConverterData;
			var specificTorqueConverterData = relatedRun.VectoRunDataSpezificBody.GearboxData.TorqueConverterData;

			Assert.AreEqual(1000.RPMtoRad(), genericTorqueConverterData.ReferenceSpeed);
			Assert.AreEqual(genericTorqueConverterData.ReferenceSpeed, specificTorqueConverterData.ReferenceSpeed);

			Assert.AreEqual(DeclarationData.TorqueConverter.MaxInputSpeed, genericTorqueConverterData.TorqueConverterSpeedLimit);
			Assert.AreEqual(genericTorqueConverterData.TorqueConverterSpeedLimit, specificTorqueConverterData.TorqueConverterSpeedLimit);

			Assert.AreEqual(0.1.SI<MeterPerSquareSecond>(), genericTorqueConverterData.CCUpshiftMinAcceleration);
			Assert.AreEqual(0.1.SI<MeterPerSquareSecond>(), genericTorqueConverterData.CLUpshiftMinAcceleration);

			Assert.AreEqual(genericTorqueConverterData.CCUpshiftMinAcceleration, specificTorqueConverterData.CCUpshiftMinAcceleration);
			Assert.AreEqual(genericTorqueConverterData.CLUpshiftMinAcceleration, specificTorqueConverterData.CLUpshiftMinAcceleration);

			Assert.AreEqual(genericTorqueConverterData.CharacteristicCurve, specificTorqueConverterData.CharacteristicCurve);
		}


		#endregion

		#region Axlegear Data Asserts

		private void AssertAxlegearData(RelatedRun relatedRun)
		{
			var genericAxlegearData = relatedRun.VectoRunDataGenericBody.AxleGearData;
			var specificAxlegearData = relatedRun.VectoRunDataSpezificBody.AxleGearData;

			Assert.AreEqual(6.500, genericAxlegearData.AxleGear.Ratio);
			Assert.AreEqual(genericAxlegearData.AxleGear.Ratio, specificAxlegearData.AxleGear.Ratio);

			Assert.AreEqual(AxleLineType.SinglePortalAxle, genericAxlegearData.LineType);
			Assert.AreEqual(genericAxlegearData.LineType, specificAxlegearData.LineType);

			Assert.IsNotNull(genericAxlegearData.AxleGear.LossMap);
			AssertAxlegearLossMap(genericAxlegearData.AxleGear.LossMap);
			var zipped =
				genericAxlegearData.AxleGear.LossMap.LossMapSerialized.Zip(specificAxlegearData.AxleGear.LossMap
					.LossMapSerialized);
			Assert.IsTrue(zipped.All(x => x.First.Equals(x.Second, StringComparison.InvariantCultureIgnoreCase)));
			//Assert(genericAxlegearData.AxleGear.LossMap.LossMapSerialized, specificAxlegearData.AxleGear.LossMap.LossMapSerialized);
		}

		private void AssertAxlegearLossMap(TransmissionLossMap lossMap)
		{
			Assert.AreEqual(12, lossMap._entries.Count);

			AssertLossmapEntry(0, -75337.83, 1585.24, lossMap._entries[0]);
			AssertLossmapEntry(0, -22.29, 16.17, lossMap._entries[1]);
			AssertLossmapEntry(0, 54.63, 16.17, lossMap._entries[2]);
			AssertLossmapEntry(0, 78508.32, 1585.24, lossMap._entries[3]);
			AssertLossmapEntry(325, -75337.83, 1585.24, lossMap._entries[4]);
			AssertLossmapEntry(325, -22.29, 16.17, lossMap._entries[5]);
			AssertLossmapEntry(325, 54.63, 16.17, lossMap._entries[6]);
			AssertLossmapEntry(325, 78508.32, 1585.24, lossMap._entries[7]);
			AssertLossmapEntry(32500, -74957.06, 1966.01, lossMap._entries[8]);
			AssertLossmapEntry(32500, 358.48, 396.94, lossMap._entries[9]);
			AssertLossmapEntry(32500, 435.40, 396.94, lossMap._entries[10]);
			AssertLossmapEntry(32500, 78889.09, 1966.01, lossMap._entries[11]);

		}

		private void AssertLossmapEntry(double inputSpeed, double inputTorque, double torqueLoss,
			TransmissionLossMap.GearLossMapEntry entry)
		{
			Assert.AreEqual(inputSpeed.RPMtoRad(), entry.InputSpeed);
			Assert.AreEqual(inputTorque, entry.InputTorque.Value(),1e-4);
			Assert.AreEqual(torqueLoss, entry.TorqueLoss.Value(), 1e-4);
		}

		#endregion
		
		#region Angledrive Data Asserts

		private void AssertAngledriveData(RelatedRun relatedRun)
		{
			var genericAngledriveData = relatedRun.VectoRunDataGenericBody.AngledriveData;
			var specificAngledriveData = relatedRun.VectoRunDataSpezificBody.AngledriveData;

			Assert.AreEqual(null, genericAngledriveData);
			Assert.AreEqual(genericAngledriveData, specificAngledriveData);
		}


		#endregion

		#region Auxiliary Data Asserts

		private void AssertAuxiliaryData(RelatedRun relatedRun)
		{
			var genericAuxiliaryData = relatedRun.VectoRunDataGenericBody.Aux.ToList();
			var specificAuxiliaryData = relatedRun.VectoRunDataSpezificBody.Aux.ToList();

			Assert.AreEqual(2, genericAuxiliaryData.Count());
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", genericAuxiliaryData.First().Technology.First());
			Assert.AreEqual("Variable displacement elec. controlled", genericAuxiliaryData.Last().Technology.First());

			Assert.AreEqual("Hydraulic driven - Constant displacement pump", specificAuxiliaryData.First().Technology.First());
			Assert.AreEqual("Variable displacement elec. controlled", specificAuxiliaryData.Last().Technology.First());
		}

		#endregion

		#region Bus Auxiliary Electrical UserInput Config Asserts

		private void AssertElectricalUserInputConfig(RelatedRun relatedRun, int idx)
		{
			var genericElectric = 
				relatedRun.VectoRunDataGenericBody.BusAuxiliaries.ElectricalUserInputsConfig;
			var specificElectric =
				relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.ElectricalUserInputsConfig;
			
			Assert.AreEqual(AlternatorType.Conventional, genericElectric.AlternatorType);
			Assert.AreEqual(genericElectric.AlternatorType, specificElectric.AlternatorType);

			Assert.AreEqual(null, genericElectric.MaxAlternatorPower);
			Assert.AreEqual(genericElectric.MaxAlternatorPower, specificElectric.MaxAlternatorPower);

			Assert.AreEqual(0.SI<WattSecond>(), genericElectric.ElectricStorageCapacity);
			Assert.AreEqual(genericElectric.ElectricStorageCapacity, specificElectric.ElectricStorageCapacity);

			Assert.AreEqual(0.7 ,genericElectric.AlternatorMap.GetEfficiency(0.RPMtoRad(),0.0.SI<Ampere>()));
			Assert.AreEqual(
				genericElectric.AlternatorMap.GetEfficiency(1000.RPMtoRad(), 100.SI<Ampere>()),
				specificElectric.AlternatorMap.GetEfficiency(1000.RPMtoRad(), 100.SI<Ampere>())); 

			Assert.AreEqual(Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency, genericElectric.AlternatorGearEfficiency);
			Assert.AreEqual(genericElectric.AlternatorGearEfficiency, specificElectric.AlternatorGearEfficiency);

			Assert.AreEqual(Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond, genericElectric.DoorActuationTimeSecond);
			Assert.AreEqual(genericElectric.DoorActuationTimeSecond, specificElectric.DoorActuationTimeSecond);

			switch (idx) {
				case 0:
				case 1:		
					Assert.AreEqual(49.395, genericElectric.AverageCurrentDemandInclBaseLoad(false, false).Value(), 1e-3);
					Assert.AreEqual(16.995, genericElectric.AverageCurrentDemandWithoutBaseLoad(false, false).Value(), 1e-3);

					Assert.AreEqual(54.181, specificElectric.AverageCurrentDemandInclBaseLoad(false, false).Value(), 1e-3);
					Assert.AreEqual(21.781, specificElectric.AverageCurrentDemandWithoutBaseLoad(false, false).Value(), 1e-3);
					break;
				case 2:
				case 3:
					Assert.AreEqual(54.235, genericElectric.AverageCurrentDemandInclBaseLoad(false, false).Value(), 1e-3);
					Assert.AreEqual(21.835, genericElectric.AverageCurrentDemandWithoutBaseLoad(false, false).Value(), 1e-3);

					Assert.AreEqual(59.0091, specificElectric.AverageCurrentDemandInclBaseLoad(false, false).Value(), 1e-3);
					Assert.AreEqual(26.6091, specificElectric.AverageCurrentDemandWithoutBaseLoad(false, false).Value(), 1e-3);
					break;

			}


		}



		#endregion Asserts

		#region Pneumatic User Inputs Config Asserts

		private void AssertPneumaticUserInputsConfig(RelatedRun relatedRun)
		{
			var genericPneumaticUI = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.PneumaticUserInputsConfig;
			var specificPneumaticUI = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.PneumaticUserInputsConfig;

			Assert.IsNotNull(genericPneumaticUI.CompressorMap);
			Assert.AreEqual(genericPneumaticUI.CompressorMap.Technology, specificPneumaticUI.CompressorMap.Technology);
			Assert.AreEqual(
				genericPneumaticUI.CompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate().Value(),
				specificPneumaticUI.CompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate().Value());

			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency, genericPneumaticUI.CompressorGearEfficiency);
			Assert.AreEqual(genericPneumaticUI.CompressorGearEfficiency, specificPneumaticUI.CompressorGearEfficiency);

			Assert.AreEqual(1.0, genericPneumaticUI.CompressorGearRatio);
			Assert.AreEqual(genericPneumaticUI.CompressorGearRatio, specificPneumaticUI.CompressorGearRatio);

			Assert.AreEqual(true, genericPneumaticUI.SmartAirCompression);
			Assert.AreEqual(genericPneumaticUI.SmartAirCompression, specificPneumaticUI.SmartAirCompression);

			Assert.AreEqual(false, genericPneumaticUI.SmartRegeneration);
			Assert.AreEqual(genericPneumaticUI.SmartRegeneration, specificPneumaticUI.SmartRegeneration);

			Assert.AreEqual(VectoMath.Max(0.07.SI<Meter>(), 0.120.SI<Meter>() - Constants.BusParameters.EntranceHeight), genericPneumaticUI.KneelingHeight);
			Assert.AreEqual(VectoMath.Max(0.SI<Meter>(), 0.120.SI<Meter>() - Constants.BusParameters.EntranceHeight), specificPneumaticUI.KneelingHeight);

			Assert.AreEqual(ConsumerTechnology.Electrically, genericPneumaticUI.AirSuspensionControl);
			Assert.AreEqual(genericPneumaticUI.AirSuspensionControl, specificPneumaticUI.AirSuspensionControl);

			Assert.AreEqual(ConsumerTechnology.Pneumatically, genericPneumaticUI.AdBlueDosing);
			Assert.AreEqual(genericPneumaticUI.AdBlueDosing, specificPneumaticUI.AdBlueDosing);

			Assert.AreEqual(ConsumerTechnology.Pneumatically, genericPneumaticUI.Doors);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, specificPneumaticUI.Doors);
		}

		#endregion

		#region Pneumatic Consumer Demand Asserts

		private void AssertPneumaticConsumerDemand(RelatedRun relatedRun)
		{
			var genericConsumer = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.PneumaticAuxiliariesConfig;
			var specificConsumer = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.PneumaticAuxiliariesConfig;
			
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection, genericConsumer.AdBlueInjection);
			Assert.AreEqual( genericConsumer.AdBlueInjection, specificConsumer.AdBlueInjection);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension, genericConsumer.AirControlledSuspension);
			Assert.AreEqual(genericConsumer.AirControlledSuspension, specificConsumer.AirControlledSuspension);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder, genericConsumer.Braking);
			Assert.AreEqual(genericConsumer.Braking, specificConsumer.Braking);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling, genericConsumer.BreakingWithKneeling);
			Assert.AreEqual(genericConsumer.BreakingWithKneeling, specificConsumer.BreakingWithKneeling);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts, genericConsumer.DeadVolBlowOuts);
			Assert.AreEqual(genericConsumer.DeadVolBlowOuts, specificConsumer.DeadVolBlowOuts);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume, genericConsumer.DeadVolume);
			Assert.AreEqual(genericConsumer.DeadVolume, specificConsumer.DeadVolume);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				genericConsumer.NonSmartRegenFractionTotalAirDemand);
			Assert.AreEqual(genericConsumer.NonSmartRegenFractionTotalAirDemand, specificConsumer.NonSmartRegenFractionTotalAirDemand);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				genericConsumer.SmartRegenFractionTotalAirDemand);
			Assert.AreEqual(genericConsumer.SmartRegenFractionTotalAirDemand, specificConsumer.SmartRegenFractionTotalAirDemand);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				genericConsumer.OverrunUtilisationForCompressionFraction);
			Assert.AreEqual(genericConsumer.OverrunUtilisationForCompressionFraction, specificConsumer.OverrunUtilisationForCompressionFraction);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening, genericConsumer.DoorOpening);
			Assert.AreEqual(genericConsumer.DoorOpening, specificConsumer.DoorOpening);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation, genericConsumer.StopBrakeActuation);
			Assert.AreEqual(genericConsumer.StopBrakeActuation, specificConsumer.StopBrakeActuation);
		}

		#endregion
		
		#region SSMBusParameters Asserts

		private void AssertSSMBusParameters(RelatedRun relatedRun, int currentIndex)
		{
			var genericBusParam = (relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).BusParameters;
			var specificBusParam = (relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).BusParameters;
			
			AssertPassengerCount(genericBusParam.NumberOfPassengers, 
				specificBusParam.NumberOfPassengers, currentIndex);
			
			Assert.AreEqual(FloorType.HighFloor, genericBusParam.BusFloorType);
			Assert.AreEqual(FloorType.HighFloor, specificBusParam.BusFloorType);

			Assert.AreEqual(23.00, genericBusParam.BusWindowSurface.Value(), 1e-3);
			Assert.AreEqual(22.745, specificBusParam.BusWindowSurface.Value(), 1e-3);

			Assert.AreEqual(140.865, genericBusParam.BusSurfaceArea.Value(), 1e-3);
			Assert.AreEqual(134.783, specificBusParam.BusSurfaceArea.Value(), 1e-3);

			Assert.AreEqual(81.09, genericBusParam.BusVolumeVentilation.Value(), 1e-3);
			Assert.AreEqual(75.4162, specificBusParam.BusVolumeVentilation.Value(), 1e-3);
		}

		private void AssertPassengerCount(double genericLoading, double specificLoading, int index)
		{
			switch (index) {
				// generic loading values shall match expected values of primary vehicle for IU and CO cycle
				// see TestPrimaryBusGroup41Test
				case 0:
					Assert.AreEqual(16.147, genericLoading, 1e-4);
					Assert.AreEqual(15.908575, specificLoading, 1e-4);
					break;
				case 1:
					Assert.AreEqual(50.572, genericLoading, 1e-4);
					Assert.AreEqual(31, specificLoading, 1e-0);
					break;
				case 2:
					Assert.AreEqual(16.4224, genericLoading, 1e-2);
					Assert.AreEqual(16.1796, specificLoading, 1e-4);
					break;
				case 3:
					Assert.AreEqual(39.556, genericLoading, 1e-2);
					Assert.AreEqual(31, specificLoading, 1e-0);
					break;
			}
		}

		#endregion

		#region Technolgy Benefits Asserts

		private void AssertTechnologyBenefits(RelatedRun relatedRun)
		{
			var genericTechnolgyBenefit = (relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).Technologies;
			var specificTechnolgyBenefit = (relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).Technologies;

			Assert.AreEqual(0.08, genericTechnolgyBenefit.CValueVariation);
			Assert.AreEqual(0.06, genericTechnolgyBenefit.HValueVariation);
			Assert.AreEqual(0.08, genericTechnolgyBenefit.VCValueVariation);
			Assert.AreEqual(0.06, genericTechnolgyBenefit.VHValueVariation);
			Assert.AreEqual(0.04, genericTechnolgyBenefit.VVValueVariation);

			Assert.AreEqual(0.08, specificTechnolgyBenefit.CValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.HValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.VCValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.VHValueVariation);
			Assert.AreEqual(0.04, specificTechnolgyBenefit.VVValueVariation);


		}

		#endregion

		#region Boundary Conditions Asserts

		private void AssertBoundaryConditions(RelatedRun relatedRun)
		{
			var genericBound = (relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).BoundaryConditions;
			var specificBound = (relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).BoundaryConditions;
		
			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.GFactor, genericBound.GFactor);
			Assert.AreEqual(genericBound.GFactor, specificBound.GFactor);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				genericBound.HeatingBoundaryTemperature);
			Assert.AreEqual(genericBound.HeatingBoundaryTemperature, specificBound.HeatingBoundaryTemperature);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature, 
				genericBound.CoolingBoundaryTemperature);
			Assert.AreEqual(genericBound.CoolingBoundaryTemperature, specificBound.CoolingBoundaryTemperature);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower, 
				genericBound.SpecificVentilationPower);
			Assert.AreEqual(genericBound.SpecificVentilationPower, specificBound.SpecificVentilationPower);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				genericBound.AuxHeaterEfficiency);
			Assert.AreEqual(genericBound.AuxHeaterEfficiency, specificBound.AuxHeaterEfficiency);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
				genericBound.MaxPossibleBenefitFromTechnologyList);
			Assert.AreEqual(genericBound.MaxPossibleBenefitFromTechnologyList, 
				specificBound.MaxPossibleBenefitFromTechnologyList);
		}

		#endregion

		#region Environmental Conditions Asserts

		private void AssertEnvironmentalConditions(RelatedRun relatedRun)
		{
			var genericEnv = (relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).EnvironmentalConditions;
			var specificEnv = (relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs).EnvironmentalConditions;

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar, genericEnv.DefaultConditions.Solar);
			Assert.AreEqual(genericEnv.DefaultConditions.Solar, specificEnv.DefaultConditions.Solar);
			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature, genericEnv.DefaultConditions.Temperature);
			Assert.AreEqual(genericEnv.DefaultConditions.Temperature, specificEnv.DefaultConditions.Temperature);
			Assert.AreEqual(1.0, genericEnv.DefaultConditions.Weighting);
			Assert.AreEqual(genericEnv.DefaultConditions.Weighting, specificEnv.DefaultConditions.Weighting);
			Assert.AreEqual(DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions, genericEnv.EnvironmentalConditionsMap);
			Assert.AreEqual(genericEnv.EnvironmentalConditionsMap, specificEnv.EnvironmentalConditionsMap);
		}

		#endregion

		#region SSMInputs Asserts

		private void AssertSSMInputs(RelatedRun relatedRun, int currentIndex)
		{
			var genericSSMInput = (SSMInputs) relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputsCooling;
			var specificSSMInput = (SSMInputs)relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputsCooling;

			AssertPassengerCount(genericSSMInput.NumberOfPassengers,
				specificSSMInput.NumberOfPassengers, currentIndex);

			AssertHVACMaxCoolingPower(genericSSMInput.HVACMaxCoolingPower.Value(),
				specificSSMInput.HVACMaxCoolingPower.Value(), currentIndex);

			//AssertCOP(genericSSMInput.COP, specificSSMInput.COP, currentIndex);

			Assert.AreEqual(true, genericSSMInput.VentilationOnDuringHeating);
			Assert.AreEqual(true, specificSSMInput.VentilationOnDuringHeating);
			Assert.AreEqual(true, genericSSMInput.VentilationWhenBothHeatingAndACInactive);
			Assert.AreEqual(true, specificSSMInput.VentilationWhenBothHeatingAndACInactive);
			Assert.AreEqual(true, genericSSMInput.VentilationDuringAC);
			Assert.AreEqual(true, specificSSMInput.VentilationDuringAC);

			Assert.AreEqual(30000, genericSSMInput.FuelFiredHeaterPower.Value());
			Assert.AreEqual(0, specificSSMInput.FuelFiredHeaterPower.Value());

			Assert.AreEqual(Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				genericSSMInput.FuelEnergyToHeatToCoolant);
			Assert.AreEqual(genericSSMInput.FuelEnergyToHeatToCoolant, specificSSMInput.FuelEnergyToHeatToCoolant);

			Assert.AreEqual(Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				genericSSMInput.CoolantHeatTransferredToAirCabinHeater);
			Assert.AreEqual(genericSSMInput.CoolantHeatTransferredToAirCabinHeater,
				specificSSMInput.CoolantHeatTransferredToAirCabinHeater);
		}


		private void AssertHVACMaxCoolingPower(double genericValue,double specificValue, int currentIndex)
		{
			switch (currentIndex) {
				case 0:
				case 1://Interurban
					Assert.AreEqual(28543.35, genericValue, 1e-3);
					Assert.AreEqual(28718.1875, specificValue, 1e-3);
					break;
				case 2:
				case 3://Coach
					Assert.AreEqual(42639.55, genericValue, 1e-3);
					Assert.AreEqual(41271.4375, specificValue, 1e-3);
					break;
			}
		}

		private void AssertCOP(double genericValue, double specificValue, int currentIndex)
		{
			switch (currentIndex)
			{
				case 0:
				case 1://Interurban
					Assert.AreEqual(3.5, genericValue);
					Assert.AreEqual(3.57, specificValue, 1e-6);
					break;
				case 2:
				case 3://Coach
					Assert.AreEqual(3.5, genericValue);
					Assert.AreEqual(3.57, specificValue, 1e-6);
					break;
			}
		}

		#endregion

		#region Retarder Asserts

		private void AssertRetarder(RelatedRun relatedRun)
		{
			var genericRetarder = relatedRun.VectoRunDataGenericBody.Retarder;
			var specificRetarder = relatedRun.VectoRunDataSpezificBody.Retarder;

			Assert.AreEqual(1, genericRetarder.Ratio);
			Assert.AreEqual( genericRetarder.Ratio, specificRetarder.Ratio);

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, genericRetarder.Type);
			Assert.AreEqual(genericRetarder.Type, specificRetarder.Type);

			var zipped = genericRetarder.LossMap.LossMapSerialized.Zip(specificRetarder.LossMap.LossMapSerialized);
			Assert.IsTrue(zipped.All(x => x.First.Equals(x.Second, StringComparison.InvariantCultureIgnoreCase)));
			//Assert.AreEqual(genericRetarder.LossMap, specificRetarder.LossMap);
		}

		#endregion

		#region  Driver Data Asserts

		private void AssertDriverData(RelatedRun relatedRun)
		{
			var genericDriver = relatedRun.VectoRunDataGenericBody.DriverData;
			var specificDriver = relatedRun.VectoRunDataGenericBody.DriverData;

			Assert.IsNotNull(genericDriver.AccelerationCurve);
			Assert.IsNotNull(specificDriver.AccelerationCurve);
			Assert.AreEqual(genericDriver.AccelerationCurve, specificDriver.AccelerationCurve);

			Assert.AreEqual(false, genericDriver.LookAheadCoasting.Enabled);
			Assert.AreEqual(DeclarationData.Driver.LookAhead.MinimumSpeed, genericDriver.LookAheadCoasting.MinSpeed);
			Assert.IsNotNull(genericDriver.LookAheadCoasting.LookAheadDecisionFactor);
			Assert.AreEqual(DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
				genericDriver.LookAheadCoasting.LookAheadDistanceFactor);
			Assert.AreEqual(genericDriver.LookAheadCoasting, specificDriver.LookAheadCoasting);

			Assert.AreEqual(false, genericDriver.OverSpeed.Enabled);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.MinSpeed, genericDriver.OverSpeed.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.AllowedOverSpeed, genericDriver.OverSpeed.OverSpeed);
			Assert.AreEqual(genericDriver.OverSpeed, specificDriver.OverSpeed);

			AssertStopStartData(genericDriver.EngineStopStart);
			AssertEcoRoll(genericDriver.EcoRoll);
			AssertPccData(genericDriver.PCC);

			AssertStopStartData(specificDriver.EngineStopStart);
			AssertEcoRoll(specificDriver.EcoRoll);
			AssertPccData(specificDriver.PCC);
		}

		private void AssertStopStartData(DriverData.EngineStopStartData engineStopStart)
		{
			var declarationValues = DeclarationData.Driver.GetEngineStopStartBus(
				VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, CompressorDrive.mechanically);

            Assert.AreEqual(declarationValues.ActivationDelay, engineStopStart.EngineOffStandStillActivationDelay);
			Assert.AreEqual(declarationValues.MaxEngineOffTimespan, engineStopStart.MaxEngineOffTimespan);
			Assert.AreEqual(declarationValues.UtilityFactor, engineStopStart.UtilityFactorStandstill);
		}

		private void AssertEcoRoll(DriverData.EcoRollData ecoRoll)
		{
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.UnderspeedThreshold, ecoRoll.UnderspeedThreshold);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.MinSpeed, ecoRoll.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.ActivationDelay, ecoRoll.ActivationPhaseDuration);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.AccelerationLowerLimit, ecoRoll.AccelerationLowerLimit);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.AccelerationUpperLimit, ecoRoll.AccelerationUpperLimit);
		}

		private void AssertPccData(DriverData.PCCData pccData)
		{
			Assert.AreEqual(DeclarationData.Driver.PCC.PCCEnableSpeed, pccData.PCCEnableSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.MinSpeed, pccData.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.PreviewDistanceUseCase1, pccData.PreviewDistanceUseCase1);
			Assert.AreEqual(DeclarationData.Driver.PCC.PreviewDistanceUseCase2, pccData.PreviewDistanceUseCase2);
			Assert.AreEqual(DeclarationData.Driver.PCC.Underspeed, pccData.UnderSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.OverspeedUseCase3, pccData.OverspeedUseCase3);
		}

		#endregion

		private CrosswindCorrectionCdxALookup GetCrosswindCorrection(string crossWindCorrectionParams,
			SquareMeter aerodynamicDragArea, Meter vehicleHeight)
		{
			return new CrosswindCorrectionCdxALookup(
				  aerodynamicDragArea, 0.SI<SquareMeter>(),
				  new AirdragDataAdapter().GetDeclarationAirResistanceCurve(
					  crossWindCorrectionParams,
					  aerodynamicDragArea,
					  vehicleHeight),
				  CrossWindCorrectionMode.DeclarationModeCorrection);
		}



		private List<RelatedRun> SetRelatedVehicleParts(List<VectoRunData> runs)
		{
			var relatedRuns = new List<RelatedRun>();

            for (int i = 0; i < runs.Count; i++)
			{
				
				var relatedRun = new RelatedRun
				{
					VectoRunDataSpezificBody = runs[i],
					VectoRunDataGenericBody = runs[i + 1]
				};
				relatedRuns.Add(relatedRun);
				i++;
			}

			return relatedRuns;
		}


		
		[TestCase(JobFile_Group41, 0, TestName = "PrintVectoRunData CompletedBus Group 41/32b CO/LL"),
		 TestCase(JobFile_Group42, 1, TestName = "PrintVectoRunData CompletedBus Group 42/33b HU/RL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/CompletedBus_41-32b_AT-P.vecto", 0, TestName = "PrintVectoRunData CompletedBus Group 41/32b AT-P CO/LL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/CompletedBus_41-32b_ES-AUX.vecto", 0, TestName = "PrintVectoRunData CompletedBus Group 41/32b ES-Aux CO/LL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/CompletedBus_41-32b_ES-AUX_mixed.vecto", 0, TestName = "PrintVectoRunData CompletedBus Group 41/32b ES-Aux CO/LL mixedDoors"),]
		public void PrintModelParametersCompletedBus(string jobFile, int pairIdx)
		{
			var runs = GetVectoRunData(jobFile);

			var relatedRuns = SetRelatedVehicleParts(runs);
			var pair = relatedRuns[pairIdx];

			File.WriteAllText($"{pair.VectoRunDataGenericBody.JobName}_{pair.VectoRunDataGenericBody.Cycle.Name}{pair.VectoRunDataGenericBody.ModFileSuffix}.json", JsonConvert.SerializeObject(pair.VectoRunDataGenericBody, Formatting.Indented));

			File.WriteAllText($"{pair.VectoRunDataSpezificBody.JobName}_{pair.VectoRunDataGenericBody.Cycle.Name}{pair.VectoRunDataSpezificBody.ModFileSuffix}.json", JsonConvert.SerializeObject(pair.VectoRunDataSpezificBody, Formatting.Indented));
		}


		[TestCase(@"TestData/Integration/Buses/FactorMethod/primary_heavyBus group41_nonSmart.xml", 12, TestName = "PrintVectoRunData PrimaryBus Group41 SD CO/LL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/primary_heavyBus group42_SmartPS.xml", 1, TestName = "PrintVectoRunData PrimaryBus Group42 SD HU/RL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_41-32b.vecto", 0, TestName = "PrintVectoRunData SingleBus Group 41/32b CO/LL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_42-33b.vecto", 1, TestName = "PrintVectoRunData SingleBus Group 42/33b HU/RL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/primary_heavyBus group41_nonSmart_AT-P.xml", 12, TestName = "PrintVectoRunData PrimaryBus Group41 AT-P SD CO/LL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/vecto_vehicle-primary_heavyBus_ESS_electricFanSTP.xml", 13, TestName = "PrintVectoRunData electric STP/Fan ESS IU/RL"),
			]
		public void PrintModelParameters(string jobFile, int runIdx)
		{
			var runs = GetVectoRunData(jobFile);

			var relatedRuns = SetRelatedVehicleParts(runs);
			var run = runs[runIdx];

			File.WriteAllText($"{run.JobName}_{run.Cycle.Name}{run.ModFileSuffix}.json", JsonConvert.SerializeObject(run, Formatting.Indented));
		}

		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_5_generic vehicle_DECL.vecto", 0, TestName = "PrintVectoRunData VTP HeavyTruck"),
		TestCase(@"TestData/Integration/VTPMode/MediumLorry/VTP_MediumLorry.vecto", 0, TestName = "PrintVectoRunData VTP MediumLory"),
		TestCase(@"TestData/Integration/VTPMode/HeavyBus/VTP_PrimaryBus.vecto", 0, TestName = "PrintVectoRunData VTP PrimaryBus"),
		TestCase(@"TestData/Integration/VTPMode/DualFuelVehicle/VTP_DualFuel.vecto", 0, TestName = "PrintVectoRunData VTP DualFuel"),
		]
		public void PrintModelParametersVTP(string jobFile, int runIdx)
		{
			var runs = GetVectoRunData(jobFile);

			//SetRelatedVehicleParts(runs);
			var run = runs[runIdx];

			File.WriteAllText($"{run.JobName}_{run.Cycle.Name}{run.ModFileSuffix}.json", JsonConvert.SerializeObject(run, Formatting.Indented));
		}

		private List<VectoRunData> GetVectoRunData(string jobFile)
		{
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(JobFile_Group41), Path.GetFileName(JobFile_Group41)));
			IInputDataProvider inputData = null;
			if (Path.GetExtension(jobFile).Equals(".xml")) {
				// a complete VIF 
				inputData = xmlInputReader.CreateDeclaration(jobFile);
			} else {
				// Primary VIF and completed XML separate
				var tmp = JSONInputDataFactory.ReadJsonJob(jobFile);

				switch (tmp) {
					case JSONInputDataSingleBusV6 _:
					case JSONVTPInputDataV4 _:
						inputData = tmp;
						break;
					case JSONInputDataCompletedBusFactorMethodV7 completedJson: {
						inputData = CompletedVIF.CreateCompletedVif(completedJson, xmlInputReader);
						break;
					}
				}
			}
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
			//	WriteModalResults = true,

			//	//ActualModalData = true,
			//	Validate = false
			//};

			var runs = factory.RunDataFactory.NextRun().ToList();
			return runs;
		}

		

		[TestCase(JobFile_Group41, TestName = "RunCompletedBusSimulation Group41/32b"),
		TestCase(JobFile_Group42, TestName = "RunCompletedBusSimulation Group42/33b"),
		]
		public void TestRunCompletedBusSimulation(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			switch (inputData) {
				case JSONInputDataSingleBusV6 _:
				case JSONVTPInputDataV4 _:
					break;
				case JSONInputDataCompletedBusFactorMethodV7 completedJson: {
					inputData = CompletedVIF.CreateCompletedVif(completedJson, xmlInputReader);
					break;
				}
			}
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration,  inputData, writer) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));

			//var runs = factory.SimulationRuns().ToArray();
			//var runIdx = 0;
			//runs[runIdx].Run();

			//Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(JobFilePrimary41, TestName = "RunPrimaryBusSimulation Group41"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/primary_heavyBus group41_nonSmart_AT-P.xml", TestName = "RunPrimaryBusSimulation Group41 AT-P"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/vecto_vehicle-primary_heavyBus_ESS_electricFanSTP.xml", TestName = "RunPrimaryBusSimulation Group41 ES-AUX"),
		TestCase(JobFilePrimary42, TestName = "RunPrimaryBusSimulation Group42"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_41-32b.vecto", TestName = "RunSingleBusSimulation Group 41/32b"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_42-33b.vecto", TestName = "RunSingleBusSimulation Group 42/33b"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_41-32b_AT-P.vecto", TestName = "RunSingleBusSimulation Group 41/32b AT-P"),
		]
		public void TestRunPrimaryOrSingleBusSimulation(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));

			//var runs = factory.SimulationRuns().ToArray();
			//var runIdx = 0;
			//runs[runIdx].Run();

			//Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

        [TestCase(@"TestData/Integration/Buses/FactorMethod/SingleBus_41-32b.vecto", TestName = "HVAC_Heating RunSingleBusSimulation Group 41/32b"),]
		public void TestRunPrimaryOrSingleBusSimulationHVACHeating(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));

            var runs = factory.SimulationRuns().ToArray();
            var runIdx = 0;
            runs[runIdx].Run();

            Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);

   //         jobContainer.AddRuns(factory);

			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(@"TestData/Integration/Buses/FactorMethod/vecto_vehicle-primary_heavyBus_ESS_electricFanSTP.xml", 13, TestName = "RunBusSimulation electric STP/Fan ESS IU/RL"),
		TestCase(@"TestData/Integration/Buses/FactorMethod/vecto_vehicle-primary_heavyBus_ESS_electricFanSTP.xml", 17, TestName = "RunBusSimulation electric STP/Fan ESS CO/RL"),

		TestCase(@"TestData/Integration/Buses/primary_heavyBus group P39_40_nonSmart_ESS.xml", 3, TestName = "RunBusSimulation Grp 39/40 P39SD U/RL"),
		TestCase(@"TestData/Integration/Buses/primary_heavyBus group P39_40_nonSmart_ESS.xml", 7, TestName = "RunBusSimulation Grp 39/40 P39SD IU/RL"),
		TestCase(@"TestData/Integration/Buses/primary_heavyBus group P39_40_nonSmart_ESS.xml", 9, TestName = "RunBusSimulation Grp 39/40 P39DD HU/RL"),
		TestCase(@"TestData/Integration/Buses/primary_heavyBus group P39_40_nonSmart_ESS.xml", 19, TestName = "RunBusSimulation Grp 39/40 P40DD IU/RL"), // fails! is intended/known
		TestCase(@"TestData/Integration/Buses/primary_heavyBus group P39_40_nonSmart_ESS.xml", 18, TestName = "RunBusSimulation Grp 39/40 P40DD IU/LL"),  // fails! is intended/known

		//TestCase(@"E:/QUAM/tmp/ESS_Tests/primary_heavyBus group 42_ConvAux_ESS_SmartPS.xml", 10, TestName = "RunBusSimulation ESS P33DD SU/LL ConvAux SmartPS"),
		//TestCase(@"E:/QUAM/tmp/ESS_Tests/primary_heavyBus group 42_ESS_SmartPS.xml", 10, TestName = "RunBusSimulation ESS P33DD SU/LL ES Aux SmartPS"),
		//TestCase(@"E:/QUAM/tmp/ESS_Tests/primary_heavyBus group 42_ESS_SmartPS_SmartES.xml", 10, TestName = "RunBusSimulation ESS P33DD SU/LL ES Aux SmartPS SmartES"),
			]
		public void TestRunPrimaryBusSimulation_ESS(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate:false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));

			var runs = factory.SimulationRuns().ToArray();
			
			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);

			//jobContainer.AddRuns(factory);

			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			//var progress = jobContainer.GetProgress();
			//Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			//Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat<Exception>(jobContainer.Runs.Select(r => r.ExecException)));
		}

		private const string JobGrp32b = @"TestData/Integration/Buses/FactorMethod/CompletedBus_41-32b_ES-AUX.vecto";

		[
		TestCase(JobGrp32b, 2, 20, 5, 17, 9, 51, TestName = "CompleteBus PassengerCount IU specific RL"),
		TestCase(JobGrp32b, 3, 20, 5, 17, 9, 49.572, TestName = "CompleteBus PassengerCount IU generic RL"),
		TestCase(JobGrp32b, 6, 20, 5, 17, 9, 37, TestName = "CompleteBus PassengerCount CO specific RL"),
		TestCase(JobGrp32b, 7, 20, 5, 17, 9, 38.556, TestName = "CompleteBus PassengerCount CO generic RL"),
		Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestPassengerCountAllocationCompletedBus(string jobName, int runIdx, int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper,  double expectedPassengers)
		{
			var inputData = CompletedVIF.CreateCompletedVifXML(
				JSONInputDataFactory.ReadJsonJob(JobFile_Group41) as JSONInputDataCompletedBusFactorMethodV7,
				xmlInputReader);

			var modified = GetModifiedXML(inputData, pSeatsLower, pStdLower, pSeatsUpper, pStdUpper);
			var completedVif = xmlInputReader.CreateDeclaration(XmlReader.Create(new StringReader(modified)));
			
			var writer = new FileOutputWriter("SanityCheckTest");
			//var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);
			//var inputData = new MockCompletedBusInputData(modified);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, new XMLDeclarationVIFInputData(completedVif as IMultistepBusInputDataProvider, null), writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, new XMLDeclarationVIFInputData(completedVif as IMultistageBusInputDataProvider, null), writer) {
			//	WriteModalResults = true,
			//	Validate = false
			//};

			var runs = factory.RunDataFactory.NextRun().ToList();
			var run = runs[runIdx];
			
			Assert.NotNull(run.VehicleData.PassengerCount);
			Assert.AreEqual(expectedPassengers, run.VehicleData.PassengerCount.Value, 1e-3);

			var ssmInputs = run.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs;
			Assert.NotNull(ssmInputs);
			Assert.AreEqual(expectedPassengers + 1, ssmInputs.NumberOfPassengers, 1e-3); // adding driver for SSM
		}

		private const string PrimaryGrp41 = @"TestData/Integration/Buses/FactorMethod/primary_heavyBus group41_nonSmart.xml";
		private const string CompletedGrp41_32b = @"TestData/Integration/Buses/FactorMethod/vecto_vehicle-completed_heavyBus_41.xml";

		[
			TestCase(PrimaryGrp41, CompletedGrp41_32b, 1, 20, 5, 17, 9, 51, TestName = "SingleBus PassengerCount IU RL"),
			TestCase(PrimaryGrp41, CompletedGrp41_32b, 3, 20, 5, 17, 9, 37, TestName = "SingleBus PassengerCount CO RL"),
		]
		public void TestPassengerCountAllocationSingleBus(string primaryFile, string completedFile, int runIdx, int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper, double expectedPassengers)
		{
			var primary = xmlInputReader.CreateDeclaration(primaryFile);

			var completedXml = new XmlDocument();
			completedXml.Load(completedFile);
			var modified = GetModifiedXML(completedXml.OuterXml, pSeatsLower, pStdLower, pSeatsUpper, pStdUpper, VehicleCode.CB);
			var modifiedCompleted = xmlInputReader.CreateDeclaration(XmlReader.Create(new StringReader(modified)));

			var inputData = new MockSingleBusInputDataProvider(primary.JobInputData.Vehicle, modifiedCompleted.JobInputData.Vehicle);
			var factory =
				SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, null, validate: false);
			factory.WriteModalResults = true;

			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, null) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var runs = factory.RunDataFactory.NextRun().ToList();
			var run = runs[runIdx];

			Assert.NotNull(run.VehicleData.PassengerCount);
			Assert.AreEqual(expectedPassengers, run.VehicleData.PassengerCount.Value, 1e-3);

			var ssmInputs = run.BusAuxiliaries.SSMInputsCooling as ISSMDeclarationInputs;
			Assert.NotNull(ssmInputs);
			Assert.AreEqual(expectedPassengers + 1, ssmInputs.NumberOfPassengers, 1e-3); // adding driver for SSM
		}

		public class MockSingleBusInputDataProvider : ISingleBusInputDataProvider, IDeclarationJobInputData
		{
			public MockSingleBusInputDataProvider(IVehicleDeclarationInputData primary, IVehicleDeclarationInputData completed)
			{
				PrimaryVehicle = primary;
				CompletedVehicle = completed;
			}

			#region Implementation of IInputDataProvider

			public DataSource DataSource { get; }

			#endregion

			#region Implementation of IDeclarationInputDataProvider

			public IDeclarationJobInputData JobInputData => this;
			public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }
			public XElement XMLHash { get; }

			#endregion

			#region Implementation of ISingleBusInputDataProvider

			public IVehicleDeclarationInputData PrimaryVehicle { get; set; }
			public IVehicleDeclarationInputData CompletedVehicle { get; set; }
			public XElement XMLHashCompleted { get; }

			#endregion

			#region Implementation of IDeclarationJobInputData

			public bool SavedInDeclarationMode => true;
			public IVehicleDeclarationInputData Vehicle => PrimaryVehicle;
			public string JobName { get; }
			public VectoSimulationJobType JobType => VectoSimulationJobType.ConventionalVehicle;

			#endregion
		}

		private string GetModifiedXML(string vifXML, int pSeatsLower, int pStdLower, int pSeatsUpper, int pStdUpper,
			VehicleCode? vehicleCode = null)
		{
			var vif = new XmlDocument();
			vif.LoadXml(vifXML);

			var pSeatsLowerNode = vif.SelectSingleNode("//*[local-name()='NumberPassengerSeatsLowerDeck']");
			pSeatsLowerNode.InnerText = pSeatsLower.ToString();

			var pStdLowerNode = vif.SelectSingleNode("//*[local-name()='NumberPassengersStandingLowerDeck']");
			pStdLowerNode.InnerText = pStdLower.ToString();

			var pSeatsUpperNode = vif.SelectSingleNode("//*[local-name()='NumberPassengerSeatsUpperDeck']");
			pSeatsUpperNode.InnerText = pSeatsUpper.ToString();

			var pStdUpperNode = vif.SelectSingleNode("//*[local-name()='NumberPassengersStandingUpperDeck']");
			pStdUpperNode.InnerText = pStdUpper.ToString();

			if (vehicleCode != null) {
				var bodyWorkNode = vif.SelectSingleNode("//*[local-name()='BodyworkCode']");
				bodyWorkNode.InnerText = vehicleCode.ToXMLFormat();

			}

			return vif.OuterXml;

			
		}


		//[TestCase(@"E:/QUAM/tmp/primary_heavyBus group 42_SmartPS_spec engine map.xml", 0),]
		public void TestRunPrimaryBusSimulationSngle(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, validate: false);
			factory.WriteModalResults = true;
			//var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
			//	WriteModalResults = true,
			//	//ActualModalData = true,
			//	Validate = false
			//};
			var jobContainer = new JobContainer(new SummaryDataContainer(writer));

			var runs = factory.SimulationRuns().ToArray();

			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);

			//jobContainer.AddRuns(factory);

			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			//var progress = jobContainer.GetProgress();
			//Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			//Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat<Exception>(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}
