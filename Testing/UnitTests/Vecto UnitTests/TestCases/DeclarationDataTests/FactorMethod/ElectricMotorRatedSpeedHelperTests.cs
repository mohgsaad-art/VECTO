using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.FactorMethod;

public class ElectricMotorRatedSpeedHelperTests
{
    [TestCase(755.11f)]
    public void GetRatedSpeedPass(double expectedSpeedRpm)
    {

        (string speed, string torque)[] entries = new (string speedd, string torque)[] {
                ("0.00	  ", "4027.80"),
                ("14.96   ", "4010.00"),
                ("151.09  ", "3980.00"),
                ("302.19  ", "4010.00"),
                ("452.92  ", "3950.00"),
                ("604.01  ", "3900.00"),
                ("755.11  ", "3950.00"),
                ("906.20  ", "3356.50"),
                ("1057.30 ", "2876.98"),
                ("1208.03 ", "2517.38"),
                ("1359.12 ", "2237.68"),
                ("1510.22 ", "2013.90"),
                ("1661.31 ", "1830.82"),
                ("1812.41 ", "1678.25"),
                ("1963.14 ", "1549.15"),
                ("2114.23 ", "1438.52"),
                ("2265.33 ", "1342.60"),
                ("2416.42", "1258.71"),
                ("2567.52 ", "1184.66"),
                ("2718.25 ", "1118.82"),
                ("2869.34 ", "1059.96"),
                ("3020.44 ", "1006.95"),
            };
        {
            var speedCol = "n";
            var torqueCol = "T_drive";
            var data = CreateDataTable(entries, speedCol, torqueCol);

            var ratedSpeed = ElectricMotorRatedSpeedHelper.GetRatedSpeed(data.AsEnumerable(),
                row => row.ParseDouble(speedCol).RPMtoRad(),
                row => row.ParseDouble(torqueCol).SI<NewtonMeter>());

            Assert.AreEqual(ratedSpeed.AsRPM, expectedSpeedRpm, expectedSpeedRpm);
        }
    }

    [TestCase(755.11f)]
    public void GetRatedSpeedDivisionByZero(double expectedSpeedRpm)
    {
        (string speed, string torque)[] entries = new (string speedd, string torque)[] {
                ("0.00	  ", "4027.00"),
                ("14.96   ", "0.00"), //<- this should lead to a division by zero, 
				("151.09  ", "3980.00"),
                ("302.19  ", "4010.00"),
                ("452.92  ", "3950.00"),
                ("604.01  ", "3900.00"),
                ("755.11  ", "3950.00"),
                ("906.20  ", "3356.50"),
                ("1057.30 ", "2876.98"),
                ("1208.03 ", "2517.38"),
                ("1359.12 ", "2237.68"),
                ("1510.22 ", "2013.90"),
                ("1661.31 ", "1830.82"),
                ("1812.41 ", "1678.25"),
                ("1963.14 ", "1549.15"),
                ("2114.23 ", "1438.52"),
                ("2265.33 ", "1342.60"),
                ("2416.42", "1258.71"),
                ("2567.52 ", "1184.66"),
                ("2718.25 ", "1118.82"),
                ("2869.34 ", "1059.96"),
                ("3020.44 ", "1006.95"),
            };

        var speedCol = "n";
        var torqueCol = "T_drive";
        var data = CreateDataTable(entries, speedCol, torqueCol);

        Assert.Throws<VectoException>(() => {
            ElectricMotorRatedSpeedHelper.GetRatedSpeed(data.AsEnumerable(),
                row => row.ParseDouble(speedCol).RPMtoRad(),
                row => row.ParseDouble(torqueCol).SI<NewtonMeter>());

        });
        //Assert.AreEqual(ratedSpeed.AsRPM, expectedSpeedRpm, expectedSpeedRpm);

    }

    [TestCase(755.11f)]
    public void GetRatedSpeedMissingZeroRPM(double expectedSpeedRpm)
    {
        (string speed, string torque)[] entries = new (string speedd, string torque)[] {
                ("1.00	  ", "4027.00"),
                ("14.96   ", "4027.00"), //<- this should lead to a division by zero, 
				("151.09  ", "3980.00"),
                ("302.19  ", "4010.00"),
                ("452.92  ", "3950.00"),
                ("604.01  ", "3900.00"),
                ("755.11  ", "3950.00"),
                ("906.20  ", "3356.50"),
                ("1057.30 ", "2876.98"),
                ("1208.03 ", "2517.38"),
                ("1359.12 ", "2237.68"),
                ("1510.22 ", "2013.90"),
                ("1661.31 ", "1830.82"),
                ("1812.41 ", "1678.25"),
                ("1963.14 ", "1549.15"),
                ("2114.23 ", "1438.52"),
                ("2265.33 ", "1342.60"),
                ("2416.42", "1258.71"),
                ("2567.52 ", "1184.66"),
                ("2718.25 ", "1118.82"),
                ("2869.34 ", "1059.96"),
                ("3020.44 ", "1006.95"),
            };

        var speedCol = "n";
        var torqueCol = "T_drive";
        var data = CreateDataTable(entries, speedCol, torqueCol);

        Assert.Throws<VectoException>(() => {
            ElectricMotorRatedSpeedHelper.GetRatedSpeed(data.AsEnumerable(),
                row => row.ParseDouble(speedCol).RPMtoRad(),
                row => row.ParseDouble(torqueCol).SI<NewtonMeter>());

        });
        //Assert.AreEqual(ratedSpeed.AsRPM, expectedSpeedRpm, expectedSpeedRpm);

    }

	private static DataTable CreateDataTable((string speed, string torque)[] entries, string firstCol,
		string secondCol)
	{

		var data = new DataTable();
		data.Columns.Add(firstCol, typeof(string));
		data.Columns.Add(secondCol, typeof(string));
		foreach (var entry in entries) {
			var row = data.NewRow();
			row[firstCol] = entry.speed;
			row[secondCol] = entry.torque;
			data.Rows.Add(row);
		}

		return data;
	}
}