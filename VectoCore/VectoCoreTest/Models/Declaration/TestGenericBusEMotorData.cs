using System.Data;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{

	[TestFixture]
	public class TestGenericBusEMotorData
	{
		private const string OutputShaftSpeedColumn = "outShaftSpeed";
		private const string MaxTorqueColumn = "maxTorque";
		private const string MinTorqueColumn = "minTorque";
		private TableData fullLoadCurve;


		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void Init()
		{
			SetFullLoadCurveData();
			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private void SetFullLoadCurveData()
		{
			fullLoadCurve = new TableData();
			fullLoadCurve.Columns.Add(OutputShaftSpeedColumn);
			fullLoadCurve.Columns.Add(MaxTorqueColumn);
			fullLoadCurve.Columns.Add(MinTorqueColumn);
			for (int i = 0; i < 22; i++) {
				fullLoadCurve.Rows.Add(fullLoadCurve.NewRow());
			}

			fullLoadCurve.Rows[0][OutputShaftSpeedColumn] = "0.00";
			fullLoadCurve.Rows[1][OutputShaftSpeedColumn] = "14.96";
			fullLoadCurve.Rows[2][OutputShaftSpeedColumn] = "151.09";
			fullLoadCurve.Rows[3][OutputShaftSpeedColumn] = "302.19";
			fullLoadCurve.Rows[4][OutputShaftSpeedColumn] = "452.92";
			fullLoadCurve.Rows[5][OutputShaftSpeedColumn] = "604.01";
			fullLoadCurve.Rows[6][OutputShaftSpeedColumn] = "755.11";
			fullLoadCurve.Rows[7][OutputShaftSpeedColumn] = "906.20";
			fullLoadCurve.Rows[8][OutputShaftSpeedColumn] = "1057.30";
			fullLoadCurve.Rows[9][OutputShaftSpeedColumn] = "1208.03";
			fullLoadCurve.Rows[10][OutputShaftSpeedColumn] = "1359.12";
			fullLoadCurve.Rows[11][OutputShaftSpeedColumn] = "1510.22";
			fullLoadCurve.Rows[12][OutputShaftSpeedColumn] = "1661.31";
			fullLoadCurve.Rows[13][OutputShaftSpeedColumn] = "1812.41";
			fullLoadCurve.Rows[14][OutputShaftSpeedColumn] = "1963.14";
			fullLoadCurve.Rows[15][OutputShaftSpeedColumn] = "2114.23";
			fullLoadCurve.Rows[16][OutputShaftSpeedColumn] = "2265.33";
			fullLoadCurve.Rows[17][OutputShaftSpeedColumn] = "2416.42";
			fullLoadCurve.Rows[18][OutputShaftSpeedColumn] = "2567.52";
			fullLoadCurve.Rows[19][OutputShaftSpeedColumn] = "2718.25";
			fullLoadCurve.Rows[20][OutputShaftSpeedColumn] = "2869.34";
			fullLoadCurve.Rows[21][OutputShaftSpeedColumn] = "3020.44";
			
			fullLoadCurve.Rows[0][MaxTorqueColumn] = "4027.80";
			fullLoadCurve.Rows[1][MaxTorqueColumn] = "4010.00";
			fullLoadCurve.Rows[2][MaxTorqueColumn] = "3980.00";
			fullLoadCurve.Rows[3][MaxTorqueColumn] = "4010.00";
			fullLoadCurve.Rows[4][MaxTorqueColumn] = "3950.00";
			fullLoadCurve.Rows[5][MaxTorqueColumn] = "3900.00";
			fullLoadCurve.Rows[6][MaxTorqueColumn] = "3950.00";
			fullLoadCurve.Rows[7][MaxTorqueColumn] = "3356.50";
			fullLoadCurve.Rows[8][MaxTorqueColumn] = "2876.98";
			fullLoadCurve.Rows[9][MaxTorqueColumn] = "2517.38";
			fullLoadCurve.Rows[10][MaxTorqueColumn] = "2237.68";
			fullLoadCurve.Rows[11][MaxTorqueColumn] = "2013.90";
			fullLoadCurve.Rows[12][MaxTorqueColumn] = "1830.82";
			fullLoadCurve.Rows[13][MaxTorqueColumn] = "1678.25";
			fullLoadCurve.Rows[14][MaxTorqueColumn] = "1549.15";
			fullLoadCurve.Rows[15][MaxTorqueColumn] = "1438.52";
			fullLoadCurve.Rows[16][MaxTorqueColumn] = "1342.60";
			fullLoadCurve.Rows[17][MaxTorqueColumn] = "1258.71";
			fullLoadCurve.Rows[18][MaxTorqueColumn] = "1184.66";
			fullLoadCurve.Rows[19][MaxTorqueColumn] = "1118.82";
			fullLoadCurve.Rows[20][MaxTorqueColumn] = "1059.96";
			fullLoadCurve.Rows[21][MaxTorqueColumn] = "1006.95";

			foreach (DataRow row in fullLoadCurve.Rows) {
				row[MinTorqueColumn] = row.ParseDouble(MaxTorqueColumn) * -1;
			}
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestFullLoadCurveRatedPointSearch()
		{
			var emResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(fullLoadCurve);
			Assert.IsNotNull(emResult); 
			Assert.AreEqual(755.11 , emResult.NRated.AsRPM, 1e-2, "Wrong speed");
			Assert.AreEqual(4027.8000, emResult.TRated.Value(), 1e-4, "Wrong torque");
			Assert.AreEqual(318498.02032 , emResult.PRated.Value(), 1e-4 , "Wrong power");

			var iepcResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(fullLoadCurve, 1, 1, 0.95, 1);
			Assert.IsNotNull(iepcResult);
			Assert.AreEqual(755.11, iepcResult.NRated.AsRPM, 1e-2);
			Assert.AreEqual(4239.7894, iepcResult.TRated.Value(), 1e-4);
			Assert.AreEqual(335261.0740, iepcResult.PRated.Value(), 1e-2);
		}


		[TestCase(@"TestData/XML/XMLVIFBusReport/IHPC_HEV_completedBus_2.VIF_Report_1.xml"),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericBusElectricMotorData(string filePath)
		{
			var multistepBusInputData = xmlInputReader.Create(filePath) as IMultistepBusInputDataProvider;
			var em = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricMachines;

			var genericElectricMotor = new GenericBusElectricMotorData();
			var electricMotorData = genericElectricMotor.CreateGenericElectricMotorData(em.Entries[0], null,
				em.Entries[0].ElectricMachine.VoltageLevels.Average(v => v.VoltageLevel.Value()).SI<Volt>());

			Assert.AreEqual(2, electricMotorData.EfficiencyData.VoltageLevels.Count);
		}


		[TestCase(@"TestData/XML/XMLVIFBusReport/IEPC_completedBus_2.VIF_Report_2.xml"),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericIEPCElectricMotorData(string iepcFilePath)
		{
			var multistepBusInputData = xmlInputReader.Create(iepcFilePath) as IMultistepBusInputDataProvider;
			var iepcData = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.IEPC;
			var axleGear = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.AxleGearInputData;

			var genericIEPCData = new GenericBusIEPCData();
			var iepcMotorData = genericIEPCData.CreateIEPCElectricMotorData(iepcData);

			Assert.AreEqual(1, iepcMotorData.EfficiencyData.VoltageLevels.Count);
		}


		[TestCase(@"TestData/XML/XMLVIFBusReport/IHPC_HEV_completedBus_2.VIF_Report_1.xml"),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericIHPCElectricMotorData(string ihpcFilePath)
		{
			var multistepBusInputData = xmlInputReader.Create(ihpcFilePath) as IMultistepBusInputDataProvider;
			var electricMachineEntry = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricMachines.Entries.First() ;
			var transmission = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.GearboxInputData;
			var machineType = electricMachineEntry.ElectricMachine.ElectricMachineType;

			var genericBusIHPCData = new GenericBusIHPCData(); 
			var ihpcData = genericBusIHPCData.CreateGenericBusIHPCData(electricMachineEntry, machineType, transmission);

			Assert.AreEqual(2, ihpcData.EfficiencyData.VoltageLevels.Count);
		}

		[TestCase(@"TestData/XML/XMLVIFBusReport/IEPC_completedBus_2.VIF_Report_2.xml", 0.5),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericBatteryData(string vifFilePath, double initialSoC)
		{
			var multistepBusInputData = xmlInputReader.Create(vifFilePath) as IMultistepBusInputDataProvider;
			var electricStorage = multistepBusInputData.JobInputData.PrimaryVehicle.Vehicle.Components.ElectricStorage;
			
			var genericBusBatteryData = new GenericBusBatteryData();
			var batterySystemData = genericBusBatteryData.CreateBatteryData(electricStorage, VectoSimulationJobType.BatteryElectricVehicle, true);

			Assert.AreEqual(initialSoC, batterySystemData.InitialSoC);
			Assert.AreEqual(2, batterySystemData.Batteries.Count);

			var battery0 = batterySystemData.Batteries[0];
			Assert.AreEqual(0.785, battery0.Item2.MaxSOC, 1e-6);
			Assert.AreEqual(0.215, battery0.Item2.MinSOC, 1e-6);
			Assert.AreEqual(72, battery0.Item2.Capacity.AsAmpHour);
			Assert.AreEqual(2, battery0.Item2.InternalResistance.Entries.Length);
			
			Assert.AreEqual(0, battery0.Item2.InternalResistance.Entries[0].SoC);
			Assert.AreEqual(3, battery0.Item2.InternalResistance.Entries[0].Resistance.Count);

			var resistance = battery0.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value();
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value());
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[1].Item2.Value());
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[0].Resistance[2].Item2.Value());
			Assert.AreEqual(1, battery0.Item2.InternalResistance.Entries[1].SoC);
			Assert.AreEqual(3, battery0.Item2.InternalResistance.Entries[1].Resistance.Count);
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[0].Item2.Value());
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[1].Item2.Value());
			Assert.AreEqual(resistance, battery0.Item2.InternalResistance.Entries[1].Resistance[2].Item2.Value());

			var battery1 = batterySystemData.Batteries[1];
			Assert.AreEqual(0.785, battery1.Item2.MaxSOC, 1e-6);
			Assert.AreEqual(0.215, battery1.Item2.MinSOC, 1e-6);
			Assert.AreEqual(72, battery1.Item2.Capacity.AsAmpHour);
			Assert.AreEqual(2, battery1.Item2.InternalResistance.Entries.Length);
			
			Assert.AreEqual(0, battery1.Item2.InternalResistance.Entries[0].SoC);
			Assert.AreEqual(3, battery1.Item2.InternalResistance.Entries[0].Resistance.Count);

			resistance = battery1.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value();
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[0].Item2.Value());
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[1].Item2.Value());
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[0].Resistance[2].Item2.Value());
			Assert.AreEqual(1, battery1.Item2.InternalResistance.Entries[1].SoC);
			Assert.AreEqual(3, battery1.Item2.InternalResistance.Entries[1].Resistance.Count);
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[0].Item2.Value());
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[1].Item2.Value());
			Assert.AreEqual(resistance, battery1.Item2.InternalResistance.Entries[1].Resistance[2].Item2.Value());
		}

		
		[TestCase(@"TestData/Hybrids/Hyb_P2_Group2SuperCapOvl/SuperCap.vreess", 1),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericSuperCapData(string superCapFilePath, double initialSoC)
		{
			var superCap = JSONInputDataFactory.ReadREESSData(superCapFilePath, false) as ISuperCapDeclarationInputData; 
			var genericBusSuperCapData = new GenericBusSuperCapData();
			var superCapData = genericBusSuperCapData.CreateGenericSuperCapData(superCap);

			Assert.AreEqual(37.0, superCapData.Capacity.Value());
			Assert.AreEqual(0, superCapData.MinVoltage.Value());
			Assert.AreEqual(330.0, superCapData.MaxVoltage.Value());
			Assert.AreEqual(100, superCapData.MaxCurrentCharge.Value());
			Assert.AreEqual(-100, superCapData.MaxCurrentDischarge.Value());
			Assert.AreEqual(initialSoC , superCapData.InitialSoC);
			Assert.AreEqual(0.55282, superCapData.InternalResistance.Value(), 1e-4);
		}
	}
}
