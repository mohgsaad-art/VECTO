using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;

public class BatteryDataAdapterTest
{
	private ElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();



	[OneTimeSetUp]
	public void OneTimeSetup()
	{

	}

    //PEV
    [TestCase(0.1, 0.95, 0.12125, 0.92875, 0.8075, VectoSimulationJobType.BatteryElectricVehicle, true, true)]

    //HEV Ovc
    [TestCase(0.23, 0.77, 0.2435, 0.7565, 0.513, VectoSimulationJobType.SerialHybridVehicle, true, false)] //input inside default limits -> use input
    [TestCase(null, null, 0.1675, 0.8325, 0.6650, VectoSimulationJobType.SerialHybridVehicle, true, false)] //no input -> use default limits
    [TestCase(0.1, 0.9, 0.1675, 0.8325, 0.6650, VectoSimulationJobType.SerialHybridVehicle, true, false)] //input outside default limits -> use default limits
    [TestCase(0.05, 0.97, 0.073, 0.947, 0.874, VectoSimulationJobType.SerialHybridVehicle, true, true)] //no default limits -> use input

    //HEV Non Ovc
    [TestCase(0.40, 0.60, 0.4050, 0.5950, 0.1900, VectoSimulationJobType.SerialHybridVehicle, false, false)] //input inside default limits -> use input
    [TestCase(null, null, 0.2625, 0.7375, 0.4750, VectoSimulationJobType.SerialHybridVehicle, false, false)] //no input -> use default limits
    [TestCase(0.15, 0.85, 0.2625, 0.7375, 0.4750, VectoSimulationJobType.SerialHybridVehicle, false, false)] //input outside default limits -> use default limits
    [Category(Definitions.TESTCASE_MIGRATED)]
	public void GenericSOCTest(
		double inputMinSoc,
		double inputMaxSoc,
		double expectedMinSoc,
		double expectedMaxSoc,
		double usableSocRange, VectoSimulationJobType vectoSimulationJobType, bool ovc, bool batteryOnlyMode)
	{

			var elStorage = CreateElectricStorage(inputMinSoc, inputMaxSoc);
			var inputData = CreateElectricStorageSystem(elStorage.Object);


			BatterySystemData batteryData;
			if (vectoSimulationJobType == VectoSimulationJobType.BatteryElectricVehicle && !ovc) {

				Assert.Throws<VectoException>(() => _electricStorageAdapter.CreateBatteryData(inputData.Object, vectoSimulationJobType, ovc, batteryOnlyMode));
				Assert.Pass();
			}

			batteryData = _electricStorageAdapter.CreateBatteryData(inputData.Object, vectoSimulationJobType, ovc, batteryOnlyMode);



			Assert.AreEqual(1, batteryData.Batteries.Count);

			var battery = batteryData.Batteries.FirstOrDefault().Item2;

			Assert.IsTrue(battery.MinSOC.IsEqual(expectedMinSoc), $"Expected: {expectedMinSoc}, Actual{battery.MinSOC}");
			Assert.IsTrue(battery.MaxSOC.IsEqual(expectedMaxSoc), $"Expected: {expectedMaxSoc}, Actual{battery.MaxSOC}");

			Assert.IsTrue(usableSocRange.IsEqual(battery.GetUsableSocRange()),
				$"Invalid {nameof(usableSocRange)} expected {usableSocRange} got {battery.GetUsableSocRange()}");
	}

	Mock<IElectricStorageSystemDeclarationInputData> CreateElectricStorageSystem(params IElectricStorageDeclarationInputData[] elStorageInputData)
	{
		var result = new Mock<IElectricStorageSystemDeclarationInputData>();
		result.Setup((m) => m.ElectricStorageElements)
			.Returns(new List<IElectricStorageDeclarationInputData>(elStorageInputData));

		return result;
	}

	static TableData GetMockTableData(string[][] values)
	{
		var result = new TableData();


		foreach(var col in values.First()) {
			result.Columns.Add(new DataColumn());
		}

		foreach (var row in values) {
			result.Rows.Add(result.NewRow().ItemArray = row);
		}
		return result;

	}


	private static Mock<IElectricStorageDeclarationInputData> CreateElectricStorage(double? minSoc, double? maxSoc)
	{
		var elStorage = new Mock<IElectricStorageDeclarationInputData>();
		var ressPack = new Mock<IBatteryPackDeclarationInputData>();

		ressPack.Setup(m => m.Capacity).Returns((1000).SI<AmpereSecond>());

		ressPack.Setup(m => m.MinSOC).Returns(() => minSoc);
		ressPack.Setup(m => m.MaxSOC).Returns(() => maxSoc);
		ressPack.Setup(m => m.MaxCurrentMap).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0", "0.0"},
				new[]{"0.0", "0.0", "0.0"},
				new[]{"0.0", "0.0", "0.0"}
			}));

		ressPack.Setup(m => m.InternalResistanceCurve).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"}
			}));
		ressPack.Setup(m => m.VoltageCurve).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"}
			}));
		ressPack.Setup(m => m.DataSource).Returns(new DataSource() {SourceType = DataSourceType.XMLEmbedded});
		elStorage.Setup(m => m.REESSPack).Returns(() => ressPack.Object);
		return elStorage;
	}

}

internal static class BatteryDataExtension
{
	public static double GetUsableSocRange(this BatteryData battery)
	{
		return battery.MaxSOC - battery.MinSOC;
	}
}
