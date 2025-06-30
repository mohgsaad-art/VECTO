using System.Data;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class CombustionEngineValidationTests
{
    /// <summary>
    /// VECTO-107 Check valid range of input parameters
    /// </summary>
    [TestCase]
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

    [TestCase]
    public void Validation_CombustionEngineData_Engineering()
	{
		var data = GetMockEngineInputData();

		var dao = new EngineeringDataAdapter();

        var engineData = dao.CreateEngineData(data, data.EngineModes.First());

        var results = engineData.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, null, false);
        Assert.IsFalse(results.Any(), "Validation failed: " + results.Select(r => r.ErrorMessage).Join("; "));
        Assert.IsTrue(engineData.IsValid());
    }

    [TestCase]
    public void Validation_CombustionEngineData_Declaration()
	{
		var data = GetMockEngineInputData();
		var vehicle = GetMockVehicleData(data);

        var dao = new DeclarationDataAdapterHeavyLorry.Conventional();

		var engineData = dao.CreateEngineData(vehicle.Object, data.EngineModes.First(), new Mission() { MissionType = MissionType.LongHaul });

        var results = engineData.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
        Assert.IsFalse(results.Any(), "Validation failed: " + results.Select(r => r.ErrorMessage).Join("; "));

        Assert.IsTrue(engineData.IsValid());
    }

	private static Mock<IVehicleDeclarationInputData> GetMockVehicleData(IEngineEngineeringInputData data)
	{
		var gbx = new Mock<IGearboxEngineeringInputData>();
		var components = new Mock<IVehicleComponentsDeclaration>();
		components.Setup(c => c.EngineInputData).Returns(data);
		components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
		var vehicle = new Mock<IVehicleDeclarationInputData>();
		vehicle.Setup(v => v.Components).Returns(components.Object);
		return vehicle;
	}

	private static IEngineEngineeringInputData GetMockEngineInputData()
	{
		var fuelConsumption = InputDataHelper.InputDataAsTableData("", new[] {
			"1, 1, 1",
			"2, 2, 2",
			"3, 3, 3"
		});

		var fullLoad = InputDataHelper.InputDataAsTableData(
			"Engine speed, max torque, drag torque, PT1", new[] {
				"3, 3, -3, 3",
				"4, 3, -3, 3"
			});
		var data = new Mock<IEngineEngineeringInputData>();
		data.Setup(e => e.Model).Returns("asdf");
		data.Setup(e => e.Displacement).Returns(6374.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>());
		data.Setup(d => d.Inertia).Returns(1.SI<KilogramSquareMeter>());
		var engineMode = new Mock<IEngineModeEngineeringInputData>();
		engineMode.Setup(m => m.IdleSpeed).Returns(560.RPMtoRad());
		engineMode.Setup(m => m.FullLoadCurve).Returns(fullLoad);
		var declEngineMode = engineMode.As<IEngineModeDeclarationInputData>();

		var fuel = new Mock<IEngineFuelEngineeringInputData>();
		fuel.Setup(f => f.FuelConsumptionMap).Returns(fuelConsumption);
		fuel.Setup(f => f.WHTCMotorway).Returns(1.1);
		fuel.Setup(f => f.WHTCRural).Returns(1.1);
		fuel.Setup(f => f.WHTCUrban).Returns(1.1);
		engineMode.Setup(m => m.Fuels).Returns(new List<IEngineFuelEngineeringInputData>() { fuel.Object });
		declEngineMode.Setup(m => m.Fuels).Returns(new List<IEngineFuelDeclarationInputData>() { fuel.Object });

		data.Setup(d => d.EngineModes).Returns(new List<IEngineModeEngineeringInputData>() { engineMode.Object });
		var declData = data.As<IEngineDeclarationInputData>();
		declData.Setup(d => d.EngineModes).Returns(new List<IEngineModeDeclarationInputData>() { declEngineMode.Object });
		return data.Object;
	}
}