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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SumWriterTest
	{

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
		public void TestSumCalcFixedTime()
		{
			var writer = new FileOutputWriter("testsumcalc_fixed");
			var sumWriter = new SummaryDataContainer(writer);
			var rundata = new VectoRunData() {
				JobName = "AuxWriteModFileSumFile",
				EngineData = new CombustionEngineData() {
					Fuels = new[] {new CombustionEngineFuelData {
						FuelData = FuelData.Diesel,
						ConsumptionMap = FuelConsumptionMapReader.ReadFromFile(@"TestData\Components\12t Delivery Truck.vmap")
					}}.ToList(),
					IdleSpeed = 600.RPMtoRad()
				},
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				Cycle = new DrivingCycleData() {
					Name = "MockCycle",
				},
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData()
				}
            };
			var modData = new ModalDataContainer(rundata, writer, null);

			modData.AddAuxiliary("FAN");

			for (var i = 0; i < 500; i++) {
				modData[ModalResultField.simulationInterval] = 1.SI<Second>();
				modData[ModalResultField.n_ice_avg] = 600.RPMtoRad();
				modData[ModalResultField.v_act] = 1.SI<MeterPerSecond>(); //20.KMPHtoMeterPerSecond();
				modData[ModalResultField.drivingBehavior] = DrivingBehavior.Driving;
				modData[ModalResultField.time] = i.SI<Second>();
				modData[ModalResultField.dist] = i.SI<Meter>();
				modData["FAN"] = 3000.SI<Watt>();
				modData[ModalResultField.P_air] = 3000.SI<Watt>();
				modData[ModalResultField.P_roll] = 3000.SI<Watt>();
				modData[ModalResultField.P_slope] = 3000.SI<Watt>();
				modData[ModalResultField.P_aux_mech] = 3000.SI<Watt>();
				modData[ModalResultField.P_brake_loss] = 3000.SI<Watt>();

				modData[ModalResultField.FCMap] = 1e-4.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 1e-4.SI<KilogramPerSecond>();
				modData[ModalResultField.ICEOn] = false;
				modData[ModalResultField.altitude] = 0.SI<Meter>();
				modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
				modData[ModalResultField.P_ice_out] = (i % 2 == 0 ? 1 : -1) * 3000.SI<Watt>();

				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();

				modData.CommitSimulationStep();
			}

			sumWriter.Write(modData, 0, 0, rundata);

			modData.Finish(VectoRun.Status.Success);
			sumWriter.Finish();

			var sumData = VectoCSVFile.Read("testsumcalc_fixed.vsum", false, true);

			// duration: 500s, distance: 500m
			Assert.AreEqual(500, modData.Duration.Value());
			Assert.AreEqual(500, modData.Distance.Value());

			// 3kW * 500s => to kWh
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_air [kWh]"), 1e-3);
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_aux_FAN [kWh]"), 1e-3);
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_roll [kWh]"), 1e-3);
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_grad [kWh]"), 1e-3);
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_aux_sum [kWh]"), 1e-3);
			Assert.AreEqual(500.0 * 3000.0 / 1000 / 3600, sumData.Rows[0].ParseDouble("E_brake [kWh]"), 1e-3);

			// 500s * 1e-4 kg/s = 0.05kg  => 0.05kg / 500 => to g/h
			Assert.AreEqual((500.0 * 1e-4) * 1000 * 3600 / 500.0, sumData.Rows[0].ParseDouble("FC-Map [g/h]"), 1e-3);
			// 500s * 1e-4 kg/s = 0.05kg => 0.05kg / 500m => to g/km
			Assert.AreEqual((500.0 * 1e-4) * 1000 * 1000 / 500, sumData.Rows[0].ParseDouble("FC-Map [g/km]"), 1e-3);
		}

		[TestCase]
		public void TestSumCalcVariableTime()
		{
			var writer = new FileOutputWriter("testsumcalc_var");
			var sumWriter = new SummaryDataContainer(writer);
			var rundata = new VectoRunData() {
				JobName = "testsumcalc_var",
				EngineData = new CombustionEngineData() {
					Fuels = new[] {new CombustionEngineFuelData {
						FuelData = FuelData.Diesel,
						ConsumptionMap = FuelConsumptionMapReader.ReadFromFile(@"TestData\Components\12t Delivery Truck.vmap")
					}}.ToList(),
					IdleSpeed = 600.RPMtoRad()
				},
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				Cycle = new DrivingCycleData() {
					Name = "MockCycle",
				},
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData()
				}
			};
			var modData = new ModalDataContainer(rundata, writer, null);
			modData.AddAuxiliary("FAN");

			var timeSteps = new[]
			{ 0.5.SI<Second>(), 0.3.SI<Second>(), 1.2.SI<Second>(), 12.SI<Second>(), 0.1.SI<Second>() };
			var powerDemand = new[]
			{ 1000.SI<Watt>(), 1500.SI<Watt>(), 2000.SI<Watt>(), 2500.SI<Watt>(), 3000.SI<Watt>() };

			for (var i = 0; i < 500; i++) {
				modData[ModalResultField.simulationInterval] = timeSteps[i % timeSteps.Length];
				modData[ModalResultField.time] = i.SI<Second>();
				modData[ModalResultField.dist] = i.SI<Meter>();
				modData[ModalResultField.n_ice_avg] = 600.RPMtoRad();
				modData[ModalResultField.v_act] = 20.KMPHtoMeterPerSecond();
				modData[ModalResultField.drivingBehavior] = DrivingBehavior.Driving;
				modData["FAN"] = powerDemand[i % powerDemand.Length];
				modData[ModalResultField.P_air] = powerDemand[i % powerDemand.Length];
				modData[ModalResultField.P_roll] = powerDemand[i % powerDemand.Length];
				modData[ModalResultField.P_slope] = powerDemand[i % powerDemand.Length];
				modData[ModalResultField.P_aux_mech] = powerDemand[i % powerDemand.Length];
				modData[ModalResultField.P_brake_loss] = powerDemand[i % powerDemand.Length];

				modData[ModalResultField.altitude] = 0.SI<Meter>();
				modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
				modData[ModalResultField.P_ice_out] = (i % 2 == 0 ? 1 : -1) * powerDemand[i % powerDemand.Length];

				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.ICEOn] = false;
				modData.CommitSimulationStep();
			}

			sumWriter.Write(modData, 0, 0, rundata);

			modData.Finish(VectoRun.Status.Success);
			sumWriter.Finish();

			var sumData = VectoCSVFile.Read("testsumcalc_var.vsum", false, true);

			// sum(dt * p) => to kWh
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_air [kWh]"), 1e-3);
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_aux_FAN [kWh]"), 1e-3);
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_roll [kWh]"), 1e-3);
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_grad [kWh]"), 1e-3);
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_aux_sum [kWh]"), 1e-3);
			Assert.AreEqual(0.934722222, sumData.Rows[0].ParseDouble("E_brake [kWh]"), 1e-3);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestSumDataMetaInformation()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = xmlInputReader.CreateDeclaration(jobfile);
			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport);
			runsFactory.WriteModalResults = false;
			runsFactory.Validate = false;
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var sumRow = sumData.Table.Rows[1];
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Manufacturer, sumRow[SummaryDataContainer.Fields.VEHICLE_MANUFACTURER]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Model, sumRow[SummaryDataContainer.Fields.VEHICLE_MODEL]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.VIN, sumRow[SummaryDataContainer.Fields.VIN_NUMBER]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.Manufacturer,
				sumRow[SummaryDataContainer.Fields.ENGINE_MANUFACTURER]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.Model, sumRow[SummaryDataContainer.Fields.ENGINE_MODEL]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().FuelType.ToXMLFormat(),
				sumRow[SummaryDataContainer.Fields.ENGINE_FUEL_TYPE]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().FuelType.ToXMLFormat(),
				sumRow[SummaryDataContainer.Fields.VEHICLE_FUEL_TYPE]);
			Assert.AreEqual((dataProvider.JobInputData.Vehicle.Components.EngineInputData.RatedPowerDeclared.ConvertToKiloWatt()),
				((ConvertedSI)sumRow[SummaryDataContainer.Fields.ENGINE_RATED_POWER]));
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.RatedSpeedDeclared.AsRPM,
				(double)((ConvertedSI)sumRow[SummaryDataContainer.Fields.ENGINE_RATED_SPEED]));
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.Displacement.ConvertToCubicCentiMeter(),
				((ConvertedSI)sumRow[SummaryDataContainer.Fields.ENGINE_DISPLACEMENT]));
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.GearboxInputData.Manufacturer,
				sumRow[SummaryDataContainer.Fields.GEARBOX_MANUFACTURER]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.GearboxInputData.Model, sumRow[SummaryDataContainer.Fields.GEARBOX_MODEL]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.AxleGearInputData.Manufacturer,
				sumRow[SummaryDataContainer.Fields.AXLE_MANUFACTURER]);
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.AxleGearInputData.Model, sumRow[SummaryDataContainer.Fields.AXLE_MODEL]);

			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().ColdHotBalancingFactor,
				sumRow[SummaryDataContainer.Fields.ENGINE_BF_COLD_HOT].ToString().ToDouble());
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().CorrectionFactorRegPer,
				sumRow[SummaryDataContainer.Fields.ENGINE_CF_REG_PER].ToString().ToDouble());
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().WHTCRural,
				sumRow[SummaryDataContainer.Fields.ENGINE_WHTC_RURAL].ToString().ToDouble());
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().WHTCUrban,
				sumRow[SummaryDataContainer.Fields.ENGINE_WHTC_URBAN].ToString().ToDouble());
			Assert.AreEqual(dataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().WHTCMotorway,
				sumRow[SummaryDataContainer.Fields.ENGINE_WHTC_MOTORWAY].ToString().ToDouble());
		}


		[TestCase()]
		public void TestSumDataIsCompleteEvenIfModFileCannotBeWritten()
		{
			

			var jobFile = @"TestData\Integration\DeclarationMode\Class5_Vocational\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

			var modFilename = Path.Combine(Path.GetDirectoryName(jobFile), "VEH-Class5_ConstructionReferenceLoad_sim.vmod");

			// lock modfile so it can't be written
			Stream fh = !File.Exists(modFilename) ? File.Create(modFilename) : File.OpenRead(modFilename);

			var writer = new FileOutputWriter(jobFile);
			var inputData = xmlInputReader.CreateDeclaration(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var sumWriter = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumWriter);

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(2, sumWriter.Table.Rows.Count);

			fh.Close();
		}


		[TestCase()]
		public void TestSumDataFileIsLocked()
		{


			var jobFile = @"TestData\Integration\DeclarationMode\Class5_Vocational\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

			var sumFilename = Path.Combine(Path.GetDirectoryName(jobFile), "Tractor_4x2_vehicle-class-5_EURO6_2018.vsum");

			// lock modfile so it can't be written
			Stream fh = !File.Exists(sumFilename) ? File.Create(sumFilename) : File.OpenRead(sumFilename);

			var writer = new FileOutputWriter(jobFile);
			var inputData = xmlInputReader.CreateDeclaration(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;
			var sumWriter = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumWriter);

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(2, sumWriter.Table.Rows.Count);

			fh.Close();
		}
	}
}
