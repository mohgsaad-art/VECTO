/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

#pragma warning disable 169

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	public class ValidationTestClass
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Validation_CombustionEngineData()
		{
			var fuelConsumption = new DataTable();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new DataTable();
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("3", "3", "-3", "3");
			fullLoad.Rows.Add("4", "3", "-3", "3");

			var data = new CombustionEngineData {
				ModelName = "asdf",
			    Displacement = 6374.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(),
				IdleSpeed = 560.RPMtoRad(),
				Inertia = 1.SI<KilogramSquareMeter>(),
				Fuels = new List<CombustionEngineFuelData>() {
					new CombustionEngineFuelData() {
						WHTCUrban = 1,
						WHTCRural = 1,
						WHTCMotorway = 1,
						ConsumptionMap = FuelConsumptionMapReader.Create(fuelConsumption)
					}
				},
				EngineStartTime = 1.SI<Second>(),
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() { { 0, FullLoadCurveReader.Create(fullLoad) } },
			};
			data.FullLoadCurves[0].EngineData = data;

			var results = data.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsFalse(results.Any(), "Validation Failed: " + results.Select(r => r.ErrorMessage).Join("; "));
			Assert.IsTrue(data.IsValid());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Validation_CombustionEngineData_Engineering()
		{
			var fuelConsumption = new TableData();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new TableData();
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("3", "3", "-3", "3");
			fullLoad.Rows.Add("4", "3", "-3", "3");
			IEngineEngineeringInputData data = new MockEngineDataProvider {
				Model = "asdf",
			    Displacement = 6374.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(),
				IdleSpeed = 560.RPMtoRad(),
				Inertia = 1.SI<KilogramSquareMeter>(),
				FullLoadCurve = fullLoad,
				FuelConsumptionMap = fuelConsumption,
				
			};
			var dao = new EngineeringDataAdapter();

			var engineData = dao.CreateEngineData(data, data.EngineModes.First());

			var results = engineData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsFalse(results.Any(), "Validation failed: " + results.Select(r => r.ErrorMessage).Join("; "));
			Assert.IsTrue(engineData.IsValid());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Validation_CombustionEngineData_Declaration()
		{
			var fuelConsumption = new TableData();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new TableData();
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("3", "3", "-3", "3");
			fullLoad.Rows.Add("4", "3", "-3", "3");
			var data = new MockEngineDataProvider {
				Model = "asdf",
			    Displacement = 6374.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(),
				IdleSpeed = 560.RPMtoRad(),
				Inertia = 1.SI<KilogramSquareMeter>(),
				FullLoadCurve = fullLoad,
				FuelConsumptionMap = fuelConsumption,
				WHTCMotorway = 1.1,
				WHTCRural = 1.1,
				WHTCUrban = 1.1
			};
			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();

			var dummyGearbox = new DummyGearboxData() {
				Type = GearboxType.AMT,
				Gears = new List<ITransmissionInputData>()
			};
			var vehicle = new MockDeclarationVehicleInputData() {
				EngineInputData = data,
				GearboxInputData = dummyGearbox
			};
			var engineData = dao.CreateEngineData(vehicle, data.EngineModes.First(), new Mission() {MissionType = MissionType.LongHaul});

			var results = engineData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsFalse(results.Any(), "Validation failed: " + results.Select(r => r.ErrorMessage).Join("; "));

			Assert.IsTrue(engineData.IsValid());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ValidationModeVehicleDataTest()
		{
			var vehicleData = new VehicleData {
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				AirDensity = DeclarationData.AirDensity,
				CurbMass = 7500.SI<Kilogram>(),
				DynamicTyreRadius = 0.5.SI<Meter>(),
				//CurbWeigthExtra = 0.SI<Kilogram>(),
				Loading = 12000.SI<Kilogram>(),
				GrossVehicleMass = 16000.SI<Kilogram>(),
				TrailerGrossVehicleMass = 0.SI<Kilogram>(),
				AxleData = new List<Axle> {
					new Axle {
						AxleType = AxleType.VehicleNonDriven,
						AxleWeightShare = 0.4,
							Inertia = 0.5.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.00555,
						TyreTestLoad = 33000.SI<Newton>()
					},
					new Axle {
						AxleType = AxleType.VehicleNonDriven,
						AxleWeightShare = 0.6,
						Inertia = 0.5.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.00555,
						TyreTestLoad = 33000.SI<Newton>()
					},
				}
			};
			var result = vehicleData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsTrue(!result.Any(), "validation should have succeded but failed." + string.Concat(result));
			// Clear of History -> Normally happens inside the SimulatorFactory
			ValidationHelper.ClearValHistory();
			result = vehicleData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsTrue(result.Any(), "validation should have failed, but succeeded." + string.Concat(result));
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ValidationModeVectoRunDataTest()
		{
			var engineData = new CombustionEngineData {
				FullLoadCurves =
					new Dictionary<uint, EngineFullLoadCurve>() {
						{ 0, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") },
						{ 1, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") },
					},
				IdleSpeed = 560.RPMtoRad()
			};

			var gearboxData = new GearboxData();
			gearboxData.Gears[1] = new GearData {
				LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/Direct Gear.vtlm", 1, "1"),
				Ratio = 1
			};

			var axleGearData = new AxleGearData {
				AxleGear = new GearData {
					Ratio = 1,
					LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/limited.vtlm", 1, "1"),
				}
			};
			var vehicleData = new VehicleData {
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				AirDensity = DeclarationData.AirDensity,
				CurbMass = 7500.SI<Kilogram>(),
				DynamicTyreRadius = 0.5.SI<Meter>(),
				//CurbWeigthExtra = 0.SI<Kilogram>(),
				Loading = 12000.SI<Kilogram>(),
				GrossVehicleMass = 16000.SI<Kilogram>(),
				TrailerGrossVehicleMass = 0.SI<Kilogram>(),
				AxleData = new List<Axle> {
					new Axle {
						AxleType = AxleType.VehicleNonDriven,
						AxleWeightShare = 0.4,
						Inertia = 0.5.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.00555,
						TyreTestLoad = 33000.SI<Newton>()
					},
					new Axle {
						AxleType = AxleType.VehicleNonDriven,
						AxleWeightShare = 0.6,
						Inertia = 0.5.SI<KilogramSquareMeter>(),
						RollResistanceCoefficient = 0.00555,
						TyreTestLoad = 33000.SI<Newton>()
					},
				}
			};
			var runData = new VectoRunData {
				JobRunId = 0,
				VehicleData = vehicleData,
				AirdragData = new AirdragData() {
					CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection,
					CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(5.SI<SquareMeter>(), 0.SI<SquareMeter>(),
                            CrossWindCorrectionCurveReader.GetNoCorrectionCurve(5.SI<SquareMeter>()),
							CrossWindCorrectionMode.NoCorrection)
				},
				GearboxData = gearboxData,
				EngineData = engineData,
				AxleGearData = axleGearData,
				ExecutionMode = ExecutionMode.Engineering,
            };

            var container = VehicleContainer.CreateVehicleContainer(runData, null, null);
			var data = new DistanceRun(container);

			var results = data.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsTrue(results.Any(), "Validation should have failed, but succeded.");

			ValidationHelper.ClearValHistory();
			results = vehicleData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsTrue(!results.Any());
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
		[TestCase,
		Category(Definitions.DUPLICATE)]
		public void Validation_VectoRun()
		{
			
			var engineData = new CombustionEngineData {
				FullLoadCurves =
					new Dictionary<uint, EngineFullLoadCurve>() {
						{ 0, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") },
						{ 1, FullLoadCurveReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vfld") }
					},
				IdleSpeed = 560.RPMtoRad()
			};

			var gearboxData = new GearboxData();
			gearboxData.Gears[1] = new GearData {
				LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/Direct Gear.vtlm", 1, "1"),
				Ratio = 1,
			};

			var axleGearData = new AxleGearData {
				AxleGear = new GearData {
					LossMap = TransmissionLossMapReader.ReadFromFile(@"TestData/Components/limited.vtlm", 1, "1"),
					Ratio = 1,
				}
			};

			var container = VehicleContainer.CreateVehicleContainer(
				new VectoRunData {
					JobRunId = 0,
					GearboxData = gearboxData,
					EngineData = engineData,
					AxleGearData = axleGearData
				}, null, null);
			var data = new DistanceRun(container);

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var results = data.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.IsTrue(results.Any(), "Validation should have failed, but succeded.");

			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed + " " + stopwatch.ElapsedMilliseconds);
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Validation_Test()
		{
			var results = new DataObject().Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);

			// every field and property should be tested except private parent fields and properties and 
			// (4*4+1) * 2 = 17*2= 34 - 4 private parent fields (+2 public field and property which are tested twice) = 32
			Assert.AreEqual(32, results.Count, "Validation Error: " + results.Select(r => r.ErrorMessage).Join("\n_eng_avg"));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ValidateDictionaryTest()
		{
			var container = new ContainerObject() {
				Elements = new Dictionary<int, WrapperObject>() {
					{ 2, new WrapperObject() { Value = 41 } },
					{ 4, new WrapperObject() { Value = -30 } }
				}
			};

			var results = container.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.AreEqual(1, results.Count);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ValidateDoubleErrorTest()
		{
			var wrap = new WrapperObject() { Value = 101 };
			var container = new ContainerObject()
			{
				Elements = new Dictionary<int, WrapperObject>() {
					{ 1, wrap },
					{ 2, wrap }
				}
			};

			var results = container.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.AreEqual(1, results.Count);

		}

		/// <summary>
		/// VECTO-249: check upshift is above downshift
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ShiftPolygonValidationTest()
		{
			var vgbs = new[] {
				"-116,600,1508						",
				"0,600,1508							",
				"293,600,1508						",
				"494,806,1508						",
				"956,1278,2355						",
			};

			var shiftPolygon =
				ShiftPolygonReader.Create(
					VectoCSVFile.ReadStream(
						InputDataHelper.InputDataAsStream("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs)));

			var results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
			Assert.IsFalse(results.Any());

			// change columns
			shiftPolygon =
				ShiftPolygonReader.Create(
					VectoCSVFile.ReadStream(
						InputDataHelper.InputDataAsStream("engine torque,upshift rpm [rpm], downshift rpm [rpm]	", vgbs)));

			results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
			Assert.IsTrue(results.Any());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ShiftPolygonValidationATTest()
		{
			var vgbs = new[] {
				"-116,600,1508						",
				"0,600,1508							",
				"293,600,1508						",
				"494,806,1508						",
				"956,1278,2355						",
			};

			var shiftPolygon =
				ShiftPolygonReader.Create(
					VectoCSVFile.ReadStream(
						InputDataHelper.InputDataAsStream("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs)));

			var results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.ATSerial, false);
			Assert.IsFalse(results.Any());

			// change columns
			shiftPolygon =
				ShiftPolygonReader.Create(
					VectoCSVFile.ReadStream(
						InputDataHelper.InputDataAsStream("engine torque,upshift rpm [rpm], downshift rpm [rpm]	", vgbs)));

			results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.ATSerial, false);
			Assert.IsFalse(results.Any());
		}

		public class ContainerObject
		{
			[Required, ValidateObject] public Dictionary<int, WrapperObject> Elements;
		}

		public class WrapperObject
		{
			[Required, System.ComponentModel.DataAnnotations.Range(0, 100)] public int Value = 0;
		}

		public class DeepDataObject
		{
			[Required, System.ComponentModel.DataAnnotations.Range(41, 42)] protected int public_field = 5;
		}

		public abstract class ParentDataObject
		{
			#region 4 parent instance fields

			// ReSharper disable once NotAccessedField.Local
			[Required, System.ComponentModel.DataAnnotations.Range(1, 2)] private int private_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(3, 4)] protected int protected_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(5, 6)] internal int internal_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(7, 8)] public int public_parent_field = 5;

			#endregion

			#region 4 parent static field

			[Required, System.ComponentModel.DataAnnotations.Range(43, 44)] private static int private_static_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(43, 44)] protected static int protected_static_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(50, 51)] internal static int internal_static_parent_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(45, 46)] public static int public_static_parent_field = 7;

			#endregion

			#region 4 parent instance properties

			[Required, System.ComponentModel.DataAnnotations.Range(11, 12)]
			private int private_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(13, 14)]
			protected int protected_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(15, 16)]
			internal int internal_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(17, 18)]
			public int public_parent_property => 7;

			#endregion

			#region 4 parent static properties

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			private static int private_static_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			protected static int protected_static_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			internal static int internal_static_parent_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			public static int public_static_parent_property => 7;

			#endregion

			#region 1 parent sub objects

			[Required, ValidateObject] public DeepDataObject parent_sub_object = new DeepDataObject();

			#endregion

			private void just_to_remove_compiler_warnings()
			{
				private_parent_field = private_static_parent_field;
			}
		}

		public class DataObject : ParentDataObject
		{
			#region 4 instance fields

			// ReSharper disable once NotAccessedField.Local
			[Required, System.ComponentModel.DataAnnotations.Range(1, 2)] private int private_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(3, 4)] protected int protected_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(5, 6)] internal int internal_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(7, 8)] public int public_field = 5;

			#endregion

			#region 4 static field

			[Required, System.ComponentModel.DataAnnotations.Range(43, 44)] private static int private_static_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(43, 44)] protected static int protected_static_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(50, 51)] internal static int internal_static_field = 7;
			[Required, System.ComponentModel.DataAnnotations.Range(45, 46)] public static int public_static_field = 7;

			#endregion

			#region 4 instance properties

			[Required, System.ComponentModel.DataAnnotations.Range(11, 12)]
			private int private_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(13, 14)]
			protected int protected_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(15, 16)]
			internal int internal_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(17, 18)]
			public int public_property => 7;

			#endregion

			#region 4 static properties

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			private static int private_static_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			protected static int protected_static_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			internal static int internal_static_property => 7;

			[Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
			public static int public_static_property => 7;

			#endregion

			#region 1 sub objects

			[Required, ValidateObject] public DeepDataObject sub_object = new DeepDataObject();

			#endregion

			private void just_to_remove_compiler_warnings()
			{
				private_field = private_static_field;
			}
		}
	}

	public class DummyGearboxData : IGearboxEngineeringInputData
	{
		public DataSource DataSource { get; set; }
		public string Source { get; set; }
		public bool SavedInDeclarationMode { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public string Creator { get; set; }
		public DateTime Date { get; set; }
		public string AppVersion { get; set; }
		public string TechnicalReportId { get; set; }

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber { get; set; }
		public DigestData DigestValue { get; set; }
		public GearboxType Type { get; set; }
		public IList<ITransmissionInputData> Gears { get; set; }
		public bool DifferentialIncluded { get; }
		public double AxlegearRatio { get; }

		public KilogramSquareMeter Inertia { get; set; }
		public Second TractionInterruption { get; set; }
		public Second MinTimeBetweenGearshift { get; set; }
		public double TorqueReserve { get; set; }
		public MeterPerSecond StartSpeed { get; set; }
		public MeterPerSquareSecond StartAcceleration { get; set; }
		public double StartTorqueReserve { get; set; }
		public ITorqueConverterEngineeringInputData TorqueConverter { get; set; }
		public Second DownshiftAfterUpshiftDelay { get; set; }
		public Second UpshiftAfterDownshiftDelay { get; set; }
		public MeterPerSquareSecond UpshiftMinAcceleration { get; set; }
		public Second PowershiftShiftTime { get; set; }
	}
}