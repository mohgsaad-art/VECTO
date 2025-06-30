using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Moq;
using NUnit;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;

public class AuxiliaryDataAdapterTest
{
	[TestCase("Small", false, false, TestName = "S_HEV Pass 1")]
	[TestCase("Small + AMS + elec. driven", true, false, TestName = "S_HEV Pass 2")]
	[TestCase("Small", false, false, TestName = "S_HEV Pass 3"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void SHEVPneumaticSystemTest(string psTechnology, bool fullyElectric, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();

		var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", "Crankshaft mounted - Electronically controlled visco clutch", "Electric driven pump");

		var auxData = dataAdapter.CreateAuxiliaryData(auxInput.Object, null, MissionType.LongHaul, VehicleClass.Class2,
			8.SI<Meter>(), 1, VectoSimulationJobType.SerialHybridVehicle);

		var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

		Assert.That(ps.ConnectToREESS == fullyElectric);
		Assert.That(ps.IsFullyElectric == fullyElectric);
	}

	[TestCase("Small + AMS", "", true, 0.0f, true, TestName = "PEV Fail 2")]
	[TestCase("Small + ESS", "", true, 0.0f, true, TestName = "PEV Fail 3")]
	[TestCase("Small + AMS + elec. driven", "Electrically driven - Electronically controlled", true, 0.0f, true, TestName = "PEV Fail 1")]
	[TestCase("Small + AMS + elec. driven", "", true, 675, false, TestName = "PEV Pass 1"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PEVPneumaticSystemTest(string psTechnology, string fanTech, bool fullyElectric, double expectedElectric,
		bool fail)
	{
		var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

		var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", fanTech, "Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1, 8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

		Assert.That(ps.ConnectToREESS == true);
		Assert.That(ps.IsFullyElectric == fullyElectric);
		Assert.That(ps.PowerDemandDataBusFunc == null);
		Assert.That(ps.PowerDemandMechCycleFunc == null);
		Assert.That(ps.PowerDemandElectric.IsEqual(expectedElectric));
	}

	[TestCase("Small", false, 1400, false, TestName = "Conventional Pass 1")]
	[TestCase("Small + AMS + elec. driven", true, 357, false, TestName = "Conventional Pass 2")]
	[TestCase("Small + AMS + elec. xyz", true, 357, true, TestName = "Conventional Fail 1"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void ConventionalPneumaticSystemTest(string psTechnology, bool fullyElectric, double expectedMechanic, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();

		var auxInput = CreateAuxInput(psTechnology, "Default", "Standard technology", "Crankshaft mounted - Electronically controlled visco clutch", "Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1, 8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var ps = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.PneumaticSystem);

		Assert.That(ps.ConnectToREESS == false);
		Assert.That(ps.IsFullyElectric == fullyElectric);
		Assert.That(ps.PowerDemandDataBusFunc == null);
		Assert.That(ps.PowerDemandMechCycleFunc == null);
		Assert.That(ps.PowerDemandMech.IsEqual(expectedMechanic), "wrong value");
	}

	[TestCase("Electrically driven - Electronically controlled", true, false, TestName = "HEV Pass 1")]
	[TestCase("Crankshaft mounted - On/off clutch", false, false, TestName = "HEV Pass 2"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void HEVCoolingFanTest(string fanTech, bool isFullyElectric, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInputData = CreateAuxInput("Small + ESS", "Default", "Standard technology",
			fanTech, "Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInputData, VectoSimulationJobType.SerialHybridVehicle, 1,
			8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var fan = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.Fan);
		
		Assert.That(isFullyElectric == fan.IsFullyElectric);
		Assert.That(isFullyElectric == fan.ConnectToREESS);
		Assert.That(fan.PowerDemandElectric == fan.PowerDemandMech * DeclarationData.AlternatorEfficiency);
		Assert.That(fan.PowerDemandDataBusFunc == null);
		Assert.That(fan.PowerDemandMechCycleFunc == null);
	}

	[TestCase("Electrically driven - Electronically controlled", true, false, TestName = "Conv Pass 1")]
	[TestCase("Crankshaft mounted - On/off clutch", false, false, TestName = "Conv Pass 2"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void ConventionalCoolingFanTest(string fanTech, bool isFullyElectric, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInputData = CreateAuxInput("Small + ESS", "Default", "Standard technology",
			fanTech, "Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInputData, VectoSimulationJobType.ConventionalVehicle, 1,
			8.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var fan = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.Fan);

		Assert.That(isFullyElectric == fan.IsFullyElectric);
		Assert.IsFalse(fan.ConnectToREESS);
		Assert.That(fan.PowerDemandElectric == fan.PowerDemandMech * DeclarationData.AlternatorEfficiency);
		Assert.IsNull(fan.PowerDemandDataBusFunc);
		Assert.IsNull(fan.PowerDemandMechCycleFunc);
	}

	[TestCase("Standard technology", false)]
	[TestCase("asdf", true),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void ConventionalESTest(string esTechnology, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
			"Fixed displacement");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1,
			4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
		Assert.IsFalse(es.ConnectToREESS);
	}

	[TestCase("Standard technology", false)]
	[TestCase("asdf", true),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PHEVEsTest(string esTechnology, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
			"Fixed displacement");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ParallelHybridVehicle, 1,
			4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
		Assert.IsFalse(es.ConnectToREESS);
	}

	[TestCase("Standard technology", false)]
	[TestCase("asdf", true),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void SHEVEsTest(string esTechnology, bool fail)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small", "Default", esTechnology, "Crankshaft mounted - On/off clutch",
			"Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, 1,
			4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
		Assert.IsTrue(es.ConnectToREESS);
	}

	[TestCase("Standard technology", false)]
	[TestCase("asdf", true),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PEVEsTest(string esTechnology, bool fail)
	{
		var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", esTechnology, "",
			"Electric driven pump");
		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1,
			4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.ElectricSystem);
		Assert.IsTrue(es.ConnectToREESS);
	}

	[Test,
	Category(Definitions.TESTCASE_MIGRATED)]
	public void ConventionalHVACTest([Values(VectoSimulationJobType.ConventionalVehicle)] VectoSimulationJobType jobType)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", "Electric driven pump");
		var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
		Assert.IsFalse(es.ConnectToREESS, "Connected to REESS");
	}

	[Test,
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PEVHVACTest([Values(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)] VectoSimulationJobType jobType)
	{
		var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "", "Electric driven pump");
		var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
		Assert.IsTrue(es.ConnectToREESS);
	}

	[Test,
	Category(Definitions.TESTCASE_MIGRATED)]
	public void HEVHVACTest([Values(VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IEPC_S, VectoSimulationJobType.IHPC)] VectoSimulationJobType jobType)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", "Electric driven pump");
		var auxData = GetAuxData(false, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, 1, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.HeatingVentilationAirCondition);
		Assert.IsTrue(es.ConnectToREESS);
	}

	[TestCase(2, false, "Fixed displacement", "Electric driven pump", TestName= "Conventional SteeringPump PASS mixed technologies")]
	[TestCase(2, false, "Fixed displacement", "Fixed displacement", TestName = "Conventional SteeringPump PASS")]
	[TestCase(1, true, "Fixed displacement", "Electric driven pump", TestName= "Conventional SteeringPump Fail"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void ConventionalSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new [] {
			sp1, 
			sp2,
			sp3, 
			sp4
		});

		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ConventionalVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var sp = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
		Assert.IsFalse(auxData.Any(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump_el && data.ConnectToREESS == true), "conventional has el. steering pump");


		Assert.IsFalse(sp.ConnectToREESS, "Connected to REESS");
	}

	[TestCase(2, false, "Electric driven pump", "Full electric steering gear", TestName="SHEV_Pass")]
	[TestCase(2, true, "Fixed displacement", "Full electric steering gear", TestName="SHEV_Fail_mixed"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void SHEVSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new[] {
			sp1,
			sp2,
			sp3,
			sp4
		});

		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.SerialHybridVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		var sp = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
		Assert.IsFalse(auxData.Any(data => data.ID is Constants.Auxiliaries.IDs.SteeringPump_el or Constants.Auxiliaries.IDs.SteeringPump && data.ConnectToREESS == false), "STP not connected to REESS");


		Assert.IsTrue(sp.ConnectToREESS, "not connected to REESS");
	}

	[TestCase(2, true, false, false, "Electric driven pump", "Full electric steering gear", TestName = "PHEV_Electric")]
	[TestCase(2, true, true,false, "Fixed displacement", "Full electric steering gear", TestName = "PHEV_Mixed")]
	[TestCase(2, false, true, false, "Fixed displacement", "Dual displacement", TestName = "PHEV_Mechanical"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PHEVSteeringPumpTest(int steeredAxles, bool hasElectric, bool hasMechanic, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
	{
		var dataAdapter = new HeavyLorryAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "Electrically driven - Electronically controlled", new[] {
			sp1,
			sp2,
			sp3,
			sp4
		});

		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.ParallelHybridVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);

		VectoRunData.AuxData sp = null;
		if (hasElectric && !hasMechanic) {
			sp = auxData.Single(data =>
				data.ID is Constants.Auxiliaries.IDs.SteeringPump or Constants.Auxiliaries.IDs.SteeringPump_el);

			Assert.IsTrue(sp.ConnectToREESS);
			Assert.Pass();
		}

		if (hasElectric && hasMechanic) {
			var spS = auxData.Where(data => data.ID is Constants.Auxiliaries.IDs.SteeringPump);
			Assert.AreEqual(2,spS.Count());

			Assert.IsTrue(spS.All(sp => sp.ConnectToREESS == sp.IsFullyElectric));
			Assert.Pass();
		}

		if (!hasElectric && hasMechanic) {
			sp = auxData.Single(data =>
				data.ID is Constants.Auxiliaries.IDs.SteeringPump);

			Assert.IsFalse(sp.ConnectToREESS);
			Assert.Pass();
		}

		Assert.IsFalse(sp.ConnectToREESS, "not connected to REESS");
	}

	[TestCase(2, false, "Electric driven pump", "Full electric steering gear", TestName = "PEV_Pass")]
	[TestCase(2, true, "Fixed displacement", "Full electric steering gear", TestName = "PEV_Fail_mixed"),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void PEVSteeringPumpTest(int steeredAxles, bool fail, string sp1 = null, string sp2 = null, string sp3 = null, string sp4 = null)
	{
		var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
		var auxInput = CreateAuxInput("Small + elec. driven", "Default", "Standard technology", "", new[] {
			sp1,
			sp2,
			sp3,
			sp4
		});

		var auxData = GetAuxData(fail, dataAdapter, auxInput, VectoSimulationJobType.BatteryElectricVehicle, steeredAxles, 4.SI<Meter>(), VehicleClass.Class2, MissionType.LongHaul);
		
		var es = auxData.Single(data => data.ID == Constants.Auxiliaries.IDs.SteeringPump);
		Assert.IsTrue(es.ConnectToREESS, "not connected to REESS");
	}





	private static IList<VectoRunData.AuxData> GetAuxData(bool fail, HeavyLorryAuxiliaryDataAdapter dataAdapter, Mock<IAuxiliariesDeclarationInputData> auxInput, VectoSimulationJobType vectoSimulationJobType, int numSteeredAxles, Meter vehicleLength, VehicleClass vehicleClass, MissionType missionType)
	{
		IList<VectoRunData.AuxData> auxData = null;
		try {
			auxData = dataAdapter.CreateAuxiliaryData(auxInput.Object, null, missionType, vehicleClass,
				vehicleLength, numSteeredAxles, vectoSimulationJobType);
		} catch (VectoException ex) {
			if (fail) {
				Assert.Pass(ex.Message);
			} else {
				Assert.Fail(ex.Message, "Unexpected exception");
			}
		}
		Assert.IsFalse(fail, "Expected fail");
		return auxData;
	}


	private static Mock<IAuxiliariesDeclarationInputData> CreateAuxInput(string psTechnology, string hvacTechnology, string elSystemTechnology, string fanTech, params string[] spTechnologies)
	{
		Assert.IsTrue(spTechnologies.Length >= 1);
		var auxInput = new Mock<IAuxiliariesDeclarationInputData>();

		Mock<IAuxiliaryDeclarationInputData> steeringSystem = new Mock<IAuxiliaryDeclarationInputData>();
		foreach (var spTechnoly in spTechnologies)
		{
			if (spTechnoly.IsNullOrEmpty()) {
				continue;
				
			}
			steeringSystem.SetType(AuxiliaryType.SteeringPump)
				.AddTechnology(spTechnoly);
		}

		var hvac = new Mock<IAuxiliaryDeclarationInputData>()
			.SetType(AuxiliaryType.HVAC)
			.AddTechnology(hvacTechnology);
		var pneumatic = new Mock<IAuxiliaryDeclarationInputData>()
			.SetType(AuxiliaryType.PneumaticSystem)
			.AddTechnology(psTechnology);
		var elSystem = new Mock<IAuxiliaryDeclarationInputData>()
			.SetType(AuxiliaryType.ElectricSystem)
			.AddTechnology(elSystemTechnology);

		Mock<IAuxiliaryDeclarationInputData> fan = null;
		if (!fanTech.IsNullOrEmpty())
		{
			fan = new Mock<IAuxiliaryDeclarationInputData>().SetType(AuxiliaryType.Fan).AddTechnology(fanTech);
			auxInput.AddAuxiliary(fan.Object);
		}

		auxInput.AddAuxiliaries(steeringSystem.Object, hvac.Object, pneumatic.Object, elSystem.Object);
		return auxInput;
	}
}





public static class AuxiliariesInputMockHelper
{
	public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliary(this Mock<IAuxiliariesDeclarationInputData> mock, IAuxiliaryDeclarationInputData aux)
	{
		var list = mock.Object?.Auxiliaries ?? new List<IAuxiliaryDeclarationInputData>();
		list.Add(aux);
		mock.Setup(aux => aux.Auxiliaries)
			.Returns(list);

		return mock;
	}

	public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliaries(
		this Mock<IAuxiliariesDeclarationInputData> mock, params IAuxiliaryDeclarationInputData[] aux)
	{

		foreach (var auxiliary in aux)
		{
			mock = mock.AddAuxiliary(auxiliary);
		}


		return mock;
	}

	public static Mock<IAuxiliaryDeclarationInputData> SetType(this Mock<IAuxiliaryDeclarationInputData> mock, AuxiliaryType type)
	{
		mock.Setup(aux => aux.Type).Returns(type);
		return mock;
	}

	public static Mock<IAuxiliaryDeclarationInputData> AddTechnology(this Mock<IAuxiliaryDeclarationInputData> mock, string technology)
	{
		var list = mock.Object.Technology ?? new List<string>();
		list.Add(technology);
		mock.Setup(aux => aux.Technology).Returns(list);
		return mock;
	}
}