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

using System.IO;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;


namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLEngineeringInputRefTest
	{
		public const string EngineeringSampleFile = "TestData/XML/XMLReaderEngineering/engineering_job-sample_ref.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestXMLInputEngineeringReferencedFileAsStream()
		{
			AssertHelper.Exception<VectoException>(() => {
				var reader = File.OpenRead(EngineeringSampleFile);
				var foo = xmlInputReader.CreateEngineering(reader);
			});
		}

		[TestCase]
		public void TestXMLInputEngReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var engineDataProvider = inputDataProvider.JobInputData.Vehicle.Components.EngineInputData;

			Assert.IsFalse(engineDataProvider.SavedInDeclarationMode);

			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engineDataProvider.Model);
			Assert.AreEqual(0.012730, engineDataProvider.Displacement.Value());
			Assert.AreEqual(0.77, engineDataProvider.Inertia.Value());

			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.EngineModes.First().Fuels.First().WHTCMotorway; });
			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.EngineModes.First().Fuels.First().WHTCRural; });
			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.EngineModes.First().Fuels.First().WHTCUrban; });

			var fcMapTable = engineDataProvider.EngineModes.First().Fuels.First().FuelConsumptionMap;
			Assert.AreEqual(112, fcMapTable.Rows.Count);
			Assert.AreEqual("engine speed", fcMapTable.Columns[0].Caption);
			Assert.AreEqual("torque", fcMapTable.Columns[1].Caption);
			Assert.AreEqual("fuel consumption", fcMapTable.Columns[2].Caption);

			Assert.AreEqual("560", fcMapTable.Rows[0][0]);
			var fcMap = FuelConsumptionMapReader.Create(fcMapTable);
			Assert.AreEqual(1256.SI(Unit.SI.Gramm.Per.Hour).Value(), 
				fcMap.GetFuelConsumption(0.SI<NewtonMeter>(), 560.RPMtoRad(), false).Value.Value());

			var fldTable = engineDataProvider.EngineModes.First().FullLoadCurve;
			Assert.AreEqual(10, fldTable.Rows.Count);
			Assert.AreEqual("engine speed", fldTable.Columns[0].Caption);
			Assert.AreEqual("full load torque", fldTable.Columns[1].Caption);
			Assert.AreEqual("motoring torque", fldTable.Columns[2].Caption);
			var fldMap = FullLoadCurveReader.Create(fldTable, true);
		}

		[TestCase]
		public void TestXMLInputGbxReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.GearboxInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", gearboxDataProvider.Model);
			Assert.AreEqual(GearboxType.AMT, gearboxDataProvider.Type);
			var gears = gearboxDataProvider.Gears;
			Assert.AreEqual(12, gears.Count);

			Assert.AreEqual(1, gears.First().Gear);
			Assert.AreEqual(14.93, gears.First().Ratio);
			Assert.AreEqual("0", gears.First().LossMap.Rows[0][0]);
			Assert.AreEqual("-350", gears.First().LossMap.Rows[0][1]);
			Assert.AreEqual("12.06", gears.First().LossMap.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(gears.First().LossMap, gears.First().Ratio,
				gears.First().Gear.ToString());

			Assert.AreEqual("-100", gears.First().ShiftPolygon.Rows[0][0]);
			Assert.AreEqual("700", gears.First().ShiftPolygon.Rows[0][1]);
			Assert.AreEqual("1800", gears.First().ShiftPolygon.Rows[0][2]);

			//Assert.AreEqual("560", gears.First().FullLoadCurve.Rows[0][0]);
			//Assert.AreEqual("2500", gears.First().FullLoadCurve.Rows[0][1]);

			//var fldMap = FullLoadCurveReader.Create(gears.First().FullLoadCurve, true);
		}

		[TestCase]
		public void TestXMLInputGbxTCReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var tcInputDataProvider = inputDataProvider.JobInputData.Vehicle.Components.TorqueConverterInputData;


			Assert.AreEqual("GBX_ShiftPolygons.vgbs", Path.GetFileName(tcInputDataProvider.ShiftPolygon.Source));
			Assert.AreEqual("-100", tcInputDataProvider.ShiftPolygon.Rows[0][0]);
			Assert.AreEqual("700", tcInputDataProvider.ShiftPolygon.Rows[0][1]);
			Assert.AreEqual("1800", tcInputDataProvider.ShiftPolygon.Rows[0][2]);

			Assert.AreEqual("tc_data.vtcc", Path.GetFileName(tcInputDataProvider.TCData.Source));
			Assert.AreEqual(3, tcInputDataProvider.TCData.Rows.Count);
			Assert.AreEqual("300", tcInputDataProvider.TCData.Rows[0][1]);
			Assert.AreEqual("0.9", tcInputDataProvider.TCData.Rows[2][2]);
		}

		[TestCase]
		public void TestXMLInputAngledriveReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var angledriveInputData = inputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData;

			Assert.AreEqual("Generic Angledrive", angledriveInputData.Model);

			var lossMapData = angledriveInputData.LossMap;
			Assert.AreEqual(1.2, angledriveInputData.Ratio);
			Assert.AreEqual("0", lossMapData.Rows[0][0]);
			Assert.AreEqual("-10000", lossMapData.Rows[0][1]);
			Assert.AreEqual("100", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, angledriveInputData.Ratio, "Angledrive");
		}

		[TestCase]
		public void TestXMLInputAxlGReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var axlegearDataProvider = inputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axlegearDataProvider.Model);

			var lossMapData = axlegearDataProvider.LossMap;
			Assert.AreEqual(2.59, axlegearDataProvider.Ratio);
			Assert.AreEqual("0", lossMapData.Rows[0][0]);
			Assert.AreEqual("-5000", lossMapData.Rows[0][1]);
			Assert.AreEqual("115", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, axlegearDataProvider.Ratio, "AxleGear");
		}

		[TestCase]
		public void TestXMLInputRetarderReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var retarderDataProvider = inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData;

			Assert.AreEqual("Generic Retarder", retarderDataProvider.Model);

			var lossMapData = retarderDataProvider.LossMap;

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, retarderDataProvider.Type);

			Assert.AreEqual("0", lossMapData.Rows[0][0]);
			Assert.AreEqual("10", lossMapData.Rows[0][1]);

			var lossMap = RetarderLossMapReader.Create(lossMapData);
		}

		[TestCase]
		public void TestXMLInputAxleWheelsReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			var axles = vehicleDataProvider.Components.AxleWheels.AxlesEngineering;

			var tyre = axles[0].Tyre;
			Assert.AreEqual("315/70 R22.5",tyre.Dimension);
			Assert.AreEqual(0.0055, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());

			tyre = axles[1].Tyre;
			Assert.AreEqual("315/70 R22.5", tyre.Dimension);
			Assert.AreEqual(0.0063, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());

			//AssertHelper.Exception<VectoException>(() => { var tmp = vehicleDataProvider.Rim; });
			Assert.AreEqual(0.488822, vehicleDataProvider.DynamicTyreRadius.Value(), 1e-6);
		}

		[TestCase]
		public void TestXMLInputAuxiliariesReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);
			var auxDataProvider = inputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData;

			//var aux = auxDataProvider.Auxiliaries;
			//var aux1 = aux[0];

			//Assert.AreEqual("ES", aux1.ID);

			//Assert.AreEqual(70, aux1.DemandMap.Rows[0].ParseDouble(AuxiliaryDataReader.Fields.MechPower));
			//Assert.AreEqual(640, aux1.DemandMap.Rows[2].ParseDouble(AuxiliaryDataReader.Fields.SupplyPower));

			//var aux2 = aux[1];

			//Assert.AreEqual("FAN", aux2.ID);
			//Assert.AreEqual(70, aux2.DemandMap.Rows[0].ParseDouble(AuxiliaryDataReader.Fields.MechPower));
			//Assert.AreEqual(3190, aux2.DemandMap.Rows[113].ParseDouble(AuxiliaryDataReader.Fields.SupplyPower));
		}

		[TestCase]
		public void TestXMLInputADASReferencedFile()
		{
			//var reader = XmlReader.Create(EngineeringSampleFile);

			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var adas = inputDataProvider.DriverInputData;

			Assert.IsTrue(adas.OverSpeedData.Enabled);
		}

		[TestCase]
		public void TestVehicleInputReferencedFile()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(18000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.29, inputDataProvider.JobInputData.Vehicle.Components.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1500, vehicleDataProvider.Loading.Value());
			Assert.AreEqual(500, vehicleDataProvider.CurbMassExtra.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.Components.RetarderInputData.Ratio);
		}

		[TestCase, Ignore("Engineering XML not maintained")]
		public void TestXMLPowertrainGenerationReferencedFile()
		{
			var fileWriter = new FileOutputWriter("foo");
			var sumWriter = new FileOutputWriter("vecto_vehicle-sample_xml");
			var sumData = new SummaryDataContainer(sumWriter);
			var jobContainer = new JobContainer(sumData);
			var dataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.WriteModalResults = true;

			Assert.That(() => jobContainer.AddRuns(runsFactory),
				Throws.TypeOf<VectoException>()
					.And.Message.EqualTo("Node IdlingSpeed not found in input data"));

			Assert.AreEqual(6, jobContainer.Runs.Count);
		}


		[TestCase]
		public void TestXMEngineering_DriverModelLACExt()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var driverDataProvider = inputDataProvider.DriverInputData;

			var lac = driverDataProvider.Lookahead;
			Assert.IsTrue(lac.Enabled);

			Assert.AreEqual("lac_speedDependent.csv", Path.GetFileName(lac.CoastingDecisionFactorTargetSpeedLookup.Source));
			Assert.AreEqual("lac_velocityDrop.csv", Path.GetFileName(lac.CoastingDecisionFactorVelocityDropLookup.Source));
		}

		[TestCase]
		public void TestXMEngineering_PTO()
		{
			var inputDataProvider = xmlInputReader.CreateEngineering(EngineeringSampleFile);

			var ptoData = inputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData;

			//Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", ptoData.PTOTransmissionType);
			Assert.AreEqual(2, ptoData.PTOLossMap.Rows.Count);
			Assert.AreEqual("2800", ptoData.PTOLossMap.Rows[1][0]);
			Assert.AreEqual("100", ptoData.PTOLossMap.Rows[1][1]);

			Assert.AreEqual(4, ptoData.PTOCycleDuringStop.Rows.Count);
			Assert.AreEqual("3", ptoData.PTOCycleDuringStop.Rows[3][0]);
			Assert.AreEqual("1200", ptoData.PTOCycleDuringStop.Rows[3][1]);
			Assert.AreEqual("100", ptoData.PTOCycleDuringStop.Rows[3][2]);
		}
	}
}
