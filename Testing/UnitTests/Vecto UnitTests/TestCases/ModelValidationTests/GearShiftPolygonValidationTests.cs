using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class GearShiftPolygonValidationTests
{
    /// <summary>
    /// VECTO-517 Shiftpolygon is considered invalid
    /// </summary>
    [TestCase]
    public void ShiftCurve_ShiftPolygon_Validation_Test()
    {
        var vgbs = new[] {
            "-50,685,1537",
            "550,685,1537",
            "678,763,1537",
            "1080,1008,2092",
            "1200,1081,2092",
            "1200,1081,2092",
            "3000,1081,2092"
        };

        var shiftPolygon =
            ShiftPolygonReader.Create(
				InputDataHelper.InputDataAsTableData("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs));

        var results = shiftPolygon.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
        Assert.IsFalse(results.Any(), results.Select(r => r.ErrorMessage).Join("\n"));
    }

    /// <summary>
    /// VECTO-249: check upshift is above downshift
    /// </summary>
    [TestCase]
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
				InputDataHelper.InputDataAsTableData("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs));

        var results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
        Assert.IsFalse(results.Any());

        // change columns
        shiftPolygon =
            ShiftPolygonReader.Create(
				InputDataHelper.InputDataAsTableData("engine torque,upshift rpm [rpm], downshift rpm [rpm]	", vgbs));

        results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
        Assert.IsTrue(results.Any());
    }

    [TestCase]
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
				InputDataHelper.InputDataAsTableData("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs));

        var results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.ATSerial, false);
        Assert.IsFalse(results.Any());

        // change columns
        shiftPolygon =
            ShiftPolygonReader.Create(
				InputDataHelper.InputDataAsTableData("engine torque,upshift rpm [rpm], downshift rpm [rpm]	", vgbs));

        results = shiftPolygon.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.ATSerial, false);
        Assert.IsFalse(results.Any());
    }

}