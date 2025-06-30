using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class WHRMapTest
	{
		public const string SingleFuelWHRVehicle = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.3/vehicle_sampleSingleModeSingleFuel_WHR.xml";

		public const string DualFuelWHRVehicle = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.3/vehicle_sampleSingleModeDualFuel_WHR.xml";

		public const string EngineeringDualFuelWHRVehicle = @"TestData/XML/XMLReaderEngineering/engineering_job-sample_ref_DF_WHR.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;
		private IVectoRunDataFactoryFactory _runDataFactory;
		private IPowertrainBuilder PowertrainBuilder;
		private IEngineeringDataAdapter DataAdapter;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			_runDataFactory = _kernel.Get<IVectoRunDataFactoryFactory>();
			PowertrainBuilder = _kernel.Get<IPowertrainBuilder>();
			DataAdapter = _kernel.Get<IEngineeringDataAdapter>();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRMapCSVDataElectric()
		{

			var data = "engine speed, torque, fuel consumption, whr power electric\n" +
						"600, -100, 0, 100\n" +
						"600, 500, 200, 400\n" +
						"2400, -100, 0, 100\n" +
						"2400, 500, 200, 400";

			var whrMap = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.ElectricalOutput);
			var result = whrMap.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

			Assert.IsFalse(result.Extrapolated);
			Assert.AreEqual(400, result.GeneratedPower.Value());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRMapCSVDataMechanical()
		{

			var data = "engine speed, torque, fuel consumption, whr power mechanical\n" +
						"600, -100, 0, 100\n" +
						"600, 500, 200, 400\n" +
						"2400, -100, 0, 100\n" +
						"2400, 500, 200, 400";

			var whrMap = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.MechanicalOutputDrivetrain);
			var result = whrMap.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

			Assert.IsFalse(result.Extrapolated);
			Assert.AreEqual(400, result.GeneratedPower.Value());
		}


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRMapCSVDataElectricAndMechanical()
		{

			var data = "engine speed, torque, fuel consumption, whr power electric, whr power mechanical\n" +
						" 600, -100,   0,  50, 100\n" +
						" 600,  500, 200, 200, 400\n" +
						"2400, -100,   0, 100, 100\n" +
						"2400,  500, 200, 200, 400";

			var whrMapEl = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.ElectricalOutput);
			var resultEl = whrMapEl.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

			Assert.IsFalse(resultEl.Extrapolated);
			Assert.AreEqual(200, resultEl.GeneratedPower.Value());

			var whrMapMech = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.MechanicalOutputDrivetrain);
			var resultMech = whrMapMech.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

			Assert.IsFalse(resultMech.Extrapolated);
			Assert.AreEqual(400, resultMech.GeneratedPower.Value());

		}


		[TestCase()]
		public void ReadXMLSingleFuelEngWithWHR()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);
			var inputDataProvider = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			//var dao = new DeclarationModeTruckVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());

			var runs = dao.NextRun().ToArray();
			Assert.AreEqual(10, runs.Length);

			Assert.IsTrue(runs.All(x => x.EngineData.ElectricalWHR?.WHRMap != null));
		}


		[TestCase()]
		public void ReadXMLSingleFuelEngWHRWithoutCorrectionFactors()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			whrCfNode.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoXMLException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHR electric power provided but no correction factors found.");
		}

		[TestCase()]
		public void ReadXMLSingleFuelEngWHRMissingWHRPower()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");
			while (whrPwrEntries.MoveNext()) { 
				whrPwrEntries.Current.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoXMLException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHR correction factors provided but electricPower missing for some entries.");
		}

		[TestCase()]
		public void ReadXMLSingleFuelEngWHRMissingWHREntries()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			for (var i = 0; i < 4; i++) {
				whrPwrEntries.MoveNext();
			}
			whrPwrEntries.Current.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoXMLException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHR correction factors provided but electricPower missing for some entries.");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWithWHR()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);
			var inputDataProvider = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());

			var runs = dao.NextRun().ToArray();
			Assert.AreEqual(10, runs.Length);

			Assert.IsTrue(runs.All(x => x.EngineData.ElectricalWHR?.WHRMap != null));
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWHRCorrectionFactorsBothFuels()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			var fuel2 = nav.SelectSingleNode(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']");
			fuel2.InsertBefore(whrCfNode.Clone());

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			var runs = dao.NextRun().ToArray();
			
			Assert.IsTrue(runs.All(x => x.EngineData.ElectricalWHR?.WHRMap != null));
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWHRPowerBothFuels()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var fuel2 = nav.Select(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']");
			while (fuel2.MoveNext()) {
				fuel2.Current.CreateAttribute("", "electricPower", "", "200.00");
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHRData (electricPower) can only be defined for one fuel!");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineWHRCorrectionAndMapForDifferentFuel()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var fuel1Entries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");
			while (fuel1Entries.MoveNext()) {
				fuel1Entries.Current.DeleteSelf();
			}

			var fuel2Entries = nav.Select(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']");
			while (fuel2Entries.MoveNext()) {
				fuel2Entries.Current.CreateAttribute("", "electricPower", "", "200.00");
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoXMLException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHR correction factors provided but electricPower missing for some entries.");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineWHRMissingWHREntries()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			for (var i = 0; i < 4; i++) {
				whrPwrEntries.MoveNext();
			}
			whrPwrEntries.Current.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			AssertHelper.Exception<VectoXMLException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHR correction factors provided but electricPower missing for some entries.");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineNoWHRCorrectionFactors()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			whrCfNode.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			//var runs = dao.NextRun().ToArray();
			AssertHelper.Exception<VectoXMLException>(() => { var tmp = dao.NextRun().ToArray(); });
			
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineNoWHRPowerEntries()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			while (whrPwrEntries.MoveNext()) {
				whrPwrEntries.Current.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = _runDataFactory.CreateDeclarationRunDataFactory(inputDataProvider, new NullDeclarationReport(),
				new NullVTPReport());
			//var runs = dao.NextRun().ToArray();
			//Assert.IsTrue(runs.All(x => x.EngineData.ElectricalWHR == null));
			AssertHelper.Exception<VectoXMLException>(() => { var tmp = dao.NextRun().ToArray(); });
		}


		[TestCase(), Ignore("Engineering XML not maintained")]
		public void ReadEngineeringXMLDualFuel()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringDualFuelWHRVehicle);
			var dao = new EngineeringModeVectoRunDataFactory(inputDataProvider, PowertrainBuilder, DataAdapter);

			var runs = dao.NextRun().ToArray();
			Assert.AreEqual(1, runs.Length);
			Assert.IsTrue(runs.All(x => x.EngineData.ElectricalWHR?.WHRMap != null));
		}
	}
}
