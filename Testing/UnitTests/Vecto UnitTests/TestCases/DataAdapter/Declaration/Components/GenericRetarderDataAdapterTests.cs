using Microsoft.VisualBasic.CompilerServices;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class GenericRetarderDataAdapterTests
{
	[Test]
	public void TestGenericRetarderDataAdapter_Conventional([Values] RetarderType retarderType)
	{
		var dao = new GenericRetarderDataAdapter();
		var ratio = 3.141;
		var mockRetarderInputData = GetMockRetarderInputData(retarderType, ratio, VectoSimulationJobType.ConventionalVehicle);
		var vehicleData = GetVectoRunData(VectoSimulationJobType.ConventionalVehicle);

		var retarderData = dao.CreateGenericRetarderData(mockRetarderInputData, vehicleData);

		Assert.NotNull(retarderData);
		Assert.AreEqual(retarderType, retarderData.Type);

		if (retarderType.IsDedicatedComponent()) {
			var expectedRetarderSpeeds = new[] {
				0, 200, 400, 600, 800, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000,
				7500
			};
			var expectedLoss = new[] {
				3.18, 3.19, 3.19, 3.21, 3.22, 3.25, 3.33, 3.44, 3.59, 3.76, 3.97, 4.22, 4.49, 4.8, 5.13, 5.51, 5.91,
				6.34, 6.81
			};
			var applicableRatio = ratio / (!retarderType.IsOneOf(RetarderType.TransmissionOutputRetarder, RetarderType.AxlegearInputRetarder)
                ? 1.0
				: vehicleData.GearboxData.Gears[(uint)vehicleData.GearboxData.Gears.Count].Ratio);
			var expectedLossMap = expectedRetarderSpeeds.Zip(expectedLoss)
				.Where(x => x.First < applicableRatio * vehicleData.EngineData.FullLoadCurves[0].MaxSpeed.AsRPM)
				.ToArray();

            Assert.AreEqual(ratio, retarderData.Ratio);
			Assert.AreEqual(expectedLossMap.Length + 1, retarderData.LossMap.LossMapSerialized.Length); // add one to the length, because map includes 1 speed step above max speed
			foreach (var (speed, expected) in expectedLossMap) {
				var loss = retarderData.LossMap.GetTorqueLoss(speed.RPMtoRad());
				Assert.AreEqual(expected, loss.Value(), 1e-2);
			}
		} else {
			Assert.AreEqual(1, retarderData.Ratio);
			Assert.AreEqual(retarderType, retarderData.Type);
			Assert.IsNull(retarderData.LossMap);
		}
	}

	[Test]
	public void TestGenericRetarderDataAdapter_ParallelHybrid([Values] RetarderType retarderType)
	{
		var dao = new GenericRetarderDataAdapter();
		var ratio = 3.141;
		var mockRetarderInputData = GetMockRetarderInputData(retarderType, ratio, VectoSimulationJobType.ParallelHybridVehicle);
		var vehicleData = GetVectoRunData(VectoSimulationJobType.ParallelHybridVehicle);

		var retarderData = dao.CreateGenericRetarderData(mockRetarderInputData, vehicleData);

		Assert.NotNull(retarderData);
		Assert.AreEqual(retarderType, retarderData.Type);

		if (retarderType.IsDedicatedComponent()) {
			var expectedRetarderSpeeds = new[] {
				0, 200, 400, 600, 800, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000,
				7500
			};
			var expectedLoss = new[] {
				3.18, 3.19, 3.19, 3.21, 3.22, 3.25, 3.33, 3.44, 3.59, 3.76, 3.97, 4.22, 4.49, 4.8, 5.13, 5.51, 5.91,
				6.34, 6.81
			};
			var applicableRatio = ratio / (!retarderType.IsOneOf(RetarderType.TransmissionOutputRetarder, RetarderType.AxlegearInputRetarder)
                ? 1.0
				: vehicleData.GearboxData.Gears[(uint)vehicleData.GearboxData.Gears.Count].Ratio);
			var expectedLossMap = expectedRetarderSpeeds.Zip(expectedLoss)
				.Where(x => x.First < applicableRatio * vehicleData.EngineData.FullLoadCurves[0].MaxSpeed.AsRPM)
				.ToArray();

			Assert.AreEqual(ratio, retarderData.Ratio);
			Assert.AreEqual(expectedLossMap.Length + 1, retarderData.LossMap.LossMapSerialized.Length); // add one to the length, because map includes 1 speed step above max speed
			foreach (var (speed, expected) in expectedLossMap) {
				var loss = retarderData.LossMap.GetTorqueLoss(speed.RPMtoRad());
				Assert.AreEqual(expected, loss.Value(), 1e-2);
			}
		} else {
			Assert.AreEqual(1, retarderData.Ratio);
			Assert.AreEqual(retarderType, retarderData.Type);
			Assert.IsNull(retarderData.LossMap);
		}
	}

	[Test]
	public void TestGenericRetarderDataAdapter_SerialHybrid([Values] RetarderType retarderType)
	{
		var dao = new GenericRetarderDataAdapter();
		var ratio = 3.141;
		var mockRetarderInputData = GetMockRetarderInputData(retarderType, ratio, VectoSimulationJobType.SerialHybridVehicle);
		var vehicleData = GetVectoRunData(VectoSimulationJobType.SerialHybridVehicle);

		var retarderData = dao.CreateGenericRetarderData(mockRetarderInputData, vehicleData);

		Assert.NotNull(retarderData);
		Assert.AreEqual(retarderType, retarderData.Type);

		if (retarderType.IsDedicatedComponent()) {
			var expectedRetarderSpeeds = new[] {
				0, 200, 400, 600, 800, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000,
				7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000, 12500, 13000
			};
			var expectedLoss = new[] {
				3.18, 3.19, 3.19, 3.21, 3.22, 3.25, 3.33, 3.44, 3.59, 3.76, 3.97, 4.22, 4.49, 4.8, 5.13, 5.51, 5.91,
				6.34, 6.81, 7.31, 7.85, 8.41, 9.01, 9.64, 10.3, 10.99, 11.72, 12.48, 13.27, 14.09
			};
            var applicableRatio = ratio / (!retarderType.IsOneOf(RetarderType.TransmissionOutputRetarder, RetarderType.AxlegearInputRetarder)
				? 1.0
				: vehicleData.GearboxData.Gears[(uint)vehicleData.GearboxData.Gears.Count].Ratio);
			var expectedLossMap = expectedRetarderSpeeds.Zip(expectedLoss)
				.Where(x => x.First < applicableRatio * vehicleData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2.EfficiencyData.MaxSpeed.AsRPM)
				.ToArray();

			Assert.AreEqual(ratio, retarderData.Ratio);
			Assert.AreEqual(expectedLossMap.Length + 1, retarderData.LossMap.LossMapSerialized.Length); // add one to the length, because map includes 1 speed step above max speed
			foreach (var (speed, expected) in expectedLossMap) {
				var loss = retarderData.LossMap.GetTorqueLoss(speed.RPMtoRad());
				Assert.AreEqual(expected, loss.Value(), 1e-2);
			}
		} else {
			Assert.AreEqual(1, retarderData.Ratio);
			Assert.AreEqual(retarderType, retarderData.Type);
			Assert.IsNull(retarderData.LossMap);
		}
	}

	[Test]
	public void TestGenericRetarderDataAdapter_BatteryElectric([Values] RetarderType retarderType)
	{
		var dao = new GenericRetarderDataAdapter();
		var ratio = 3.141;
		var mockRetarderInputData = GetMockRetarderInputData(retarderType, ratio, VectoSimulationJobType.BatteryElectricVehicle);
		var vehicleData = GetVectoRunData(VectoSimulationJobType.BatteryElectricVehicle);

		var retarderData = dao.CreateGenericRetarderData(mockRetarderInputData, vehicleData);

		Assert.NotNull(retarderData);
		Assert.AreEqual(retarderType, retarderData.Type);

		if (retarderType.IsDedicatedComponent()) {
			var expectedRetarderSpeeds = new[] {
				0, 200, 400, 600, 800, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000,
				7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000, 12500, 13000
			};
			var expectedLoss = new[] {
				3.18, 3.19, 3.19, 3.21, 3.22, 3.25, 3.33, 3.44, 3.59, 3.76, 3.97, 4.22, 4.49, 4.8, 5.13, 5.51, 5.91,
				6.34, 6.81, 7.31, 7.85, 8.41, 9.01, 9.64, 10.3, 10.99, 11.72, 12.48, 13.27, 14.09
			};
			var applicableRatio = ratio / (!retarderType.IsOneOf(RetarderType.TransmissionOutputRetarder, RetarderType.AxlegearInputRetarder)
				? 1.0
				: vehicleData.GearboxData.Gears[(uint)vehicleData.GearboxData.Gears.Count].Ratio);
			var expectedLossMap = expectedRetarderSpeeds.Zip(expectedLoss)
				.Where(x => x.First < applicableRatio * vehicleData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2.EfficiencyData.MaxSpeed.AsRPM)
				.ToArray();

			Assert.AreEqual(ratio, retarderData.Ratio);
			Assert.AreEqual(expectedLossMap.Length + 1, retarderData.LossMap.LossMapSerialized.Length); // add one to the length, because map includes 1 speed step above max speed
			foreach (var (speed, expected) in expectedLossMap) {
				var loss = retarderData.LossMap.GetTorqueLoss(speed.RPMtoRad());
				Assert.AreEqual(expected, loss.Value(), 1e-2);
			}
		} else {
			Assert.AreEqual(1, retarderData.Ratio);
			Assert.AreEqual(retarderType, retarderData.Type);
			Assert.IsNull(retarderData.LossMap);
		}
	}

    private static VectoRunData GetVectoRunData(VectoSimulationJobType jobType)
	{
		var runData = new VectoRunData() {
			JobType = jobType,
			
			GearboxData = new GearboxData() {
				Gears = new Dictionary<uint, GearData>() {
					{ 1u, new GearData() { Ratio = 6 } },
					{ 2u, new GearData() { Ratio = 4 } },
					{ 3u, new GearData() { Ratio = 2 } },
					{ 4u, new GearData() { Ratio = 1.2 } },
				}
			},
			ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>()
        };
		if (jobType != VectoSimulationJobType.BatteryElectricVehicle) {
			runData.EngineData = new CombustionEngineData() {
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
					{
						0u,
						FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICE_FLD_HDR, ICE_FLD_Data), true)
					}
				}
			};
		}

		if (jobType == VectoSimulationJobType.SerialHybridVehicle) {
			runData.ElectricMachinesData.Add(
				Tuple.Create(PowertrainPosition.GEN, new ElectricMotorData()));
		}
		if (jobType != VectoSimulationJobType.ConventionalVehicle) {
			runData.ElectricMachinesData.Add(
				Tuple.Create(PowertrainPosition.BatteryElectricE2, new ElectricMotorData() {
					EfficiencyData = new VoltageLevelData() {
						VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
							new ElectricMotorVoltageLevelData() {
								FullLoadCurve =
									ElectricFullLoadCurveReader.Create(
										InputDataHelper.InputDataAsTableData(EM_FLD_HDR, EM_FLD_Data), 1),
								EfficiencyMap = ElectricMotorMapReader.Create(
									InputDataHelper.InputDataAsTableData(EM_MAP_HDR, EM_MAP_Data), 1,
									ExecutionMode.Engineering),
							}
						}
					},
					RatioADC = 1.0
				}));
		}
		return runData;
	}

	private const string ICE_FLD_HDR = "engine speed [1/min], full load torque [Nm], motoring torque [Nm]";

	private static readonly string[] ICE_FLD_Data = new[] {
		"560, 800, -30",
		"1200, 1800, -30",
		"2300, 800, -30"
	};

	private const string EM_FLD_HDR = "n [rpm] , T_drive [Nm] , T_drag [Nm]";

	private static readonly string[] EM_FLD_Data = new[] {
		"0, 750.00, -750.00",
		"2000, 750.00, -750.00",
		"4000, 50.00, -50.00",
	};

	private const string EM_MAP_HDR = "n [rpm] , T [Nm] , P_el [kW]";

	private static readonly string[] EM_MAP_Data = new[] {
		"0.00,-750.00,0.000",
		"0.00,750.00,0.000",
		"1.00,-750.00,-70",
		"1.00,750.00,80",
		"2000.00,-750.00,-313",
		"2000.00,750.00,320",
        "4000.00,-750.00,-313",
		"4000.00,750.00,320",
	};

    private IRetarderInputData GetMockRetarderInputData(RetarderType retarderType, double ratio,
		VectoSimulationJobType vehicleType)
	{
		var mock = new Mock<IRetarderInputData>();
		mock.Setup(m => m.Type).Returns(retarderType);
		mock.Setup(m => m.Ratio).Returns(ratio);
		
		return mock.Object;
	}
}