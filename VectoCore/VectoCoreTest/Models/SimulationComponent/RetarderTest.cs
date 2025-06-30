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
using NUnit.Framework;
using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class RetarderTest
	{
		private const string RetarderLossMapFile = @"TestData/Components/Retarder.vrlm";
		private const double Delta = 0.0001;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(0, 10, 10.002),
		 TestCase(100, 1000, 12),
			Category(Definitions.TESTCASE_MIGRATED) // new name: RetarderRequestTest
            ]
		public void RetarderBasicTest(double cardanTorque, double cardanSpeed, double expectedRetarderLoss)
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var retarder = new Retarder(vehicle, retarderData, 1.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			// --------
			outPort.Initialize(cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad());
			outPort.Request(absTime, dt, cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad(), false);

			Assert.AreEqual(cardanSpeed.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(cardanTorque + expectedRetarderLoss, nextRequest.Torque.Value(), Delta);

			//// --------
			//outPort.Initialize(100.SI<NewtonMeter>(), 1000.RPMtoRad());
			//outPort.Request(absTime, dt, 100.SI<NewtonMeter>(), 1000.RPMtoRad());

			//Assert.AreEqual(1000.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			//Assert.AreEqual(112, nextRequest.Torque.Value(), Delta);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void RetarderSubsequentRequestTest()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var retarder = new Retarder(vehicle, retarderData, 1.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();
			// --------
			outPort.Initialize(50.SI<NewtonMeter>(), 650.RPMtoRad());
			outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 1550.RPMtoRad(), false);
			retarder.CommitSimulationStep(absTime, dt, new MockModalDataContainer());
			Assert.AreEqual(1550.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);

			// (650+1550)/2 = 1100 => 12.42Nm
			Assert.AreEqual(50 + 12.42, nextRequest.Torque.Value(), Delta);

			//VECTO-307: added an additional request after a commit
			outPort.Request(absTime, dt, 50.SI<NewtonMeter>(), 450.RPMtoRad(), false);
			Assert.AreEqual(450.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			// avg: (1550+450)/2 = 1000 rpm => 12Nm
			Assert.AreEqual(50 + 12, nextRequest.Torque.Value(), Delta);
		}

		[TestCase(0, 10, 20.008),
		 TestCase(100, 1000, 36),
		 TestCase(50, 1550, 55.56), // extrapolated
		Category(Definitions.TESTCASE_MIGRATED) // only migrated non-extrapolating testcases, new name: RetarderRequestTest
        ]
		public void RetarderRatioTest(double cardanTorque, double cardanSpeed, double expectedRetarderLoss)
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var retarder = new Retarder(vehicle, retarderData, 2.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			outPort.Initialize(cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad());
			outPort.Request(absTime, dt, cardanTorque.SI<NewtonMeter>(), cardanSpeed.RPMtoRad(), false);

			Assert.AreEqual(cardanSpeed.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(cardanTorque + expectedRetarderLoss, nextRequest.Torque.Value(), Delta);

		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED) // new name: RetarderDeclarationNoExtrapolationTest
        ]
		public void RetarderDeclarationTest()
		{
			var retarderData = RetarderLossMapReader.ReadFromFile(RetarderLossMapFile);
			var declVehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var retarder = new Retarder(declVehicle, retarderData, 2.0);
			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			outPort.Initialize(50.SI<NewtonMeter>(), 2550.RPMtoRad());
			outPort.Request(0.SI<Second>(), 0.SI<Second>(), 50.SI<NewtonMeter>(), 2550.RPMtoRad(), false);
			AssertHelper.Exception<VectoException>(() => retarder.CommitSimulationStep(0.SI<Second>(), 0.SI<Second>(), new MockModalDataContainer()),
				"Retarder LossMap data was extrapolated in Declaration mode: range for loss map is not sufficient: n:5100 (min:0, max:2300), ratio:2");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void RetarderDataSorting()
		{
			var retarderEntries = new[] {
				"100,10.02",
				"0,10",
				"200,10.08",
				"500,10.5",
				"300,10.18",
				"400,10.32",
			};
			var retarderTbl =
				VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("Retarder Speed [rpm],Loss Torque [Nm]",
					retarderEntries));
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var retarderData = RetarderLossMapReader.Create(retarderTbl);
			var retarder = new Retarder(vehicle, retarderData, 2.0);

			var nextRequest = new MockTnOutPort();

			retarder.InPort().Connect(nextRequest);
			var outPort = retarder.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 0.SI<Second>();

			// --------
			outPort.Initialize(100.SI<NewtonMeter>(), 125.RPMtoRad());
			outPort.Request(absTime, dt, 100.SI<NewtonMeter>(), 125.RPMtoRad(), false);

			Assert.AreEqual(125.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
			Assert.AreEqual(100 + 20.26, nextRequest.Torque.Value(), Delta);
		}


		private class RetarderInputData : IRetarderInputData
		{
			#region Implementation of IRetarderInputData

			public RetarderType Type { get; set; }
			public double Ratio { get; set; }
			public TableData LossMap { get; set; }

			#endregion

			#region Implementation of IComponentInputData

			public DataSource DataSource { get; }
			public bool SavedInDeclarationMode { get; }
			public string Manufacturer { get; }
			public string Model { get; }
			public DateTime Date { get; }
			public string AppVersion { get; }
			public CertificationMethod CertificationMethod { get; }
			public string CertificationNumber { get; }
			public DigestData DigestValue { get; }

			#endregion
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CreateRetarderTest()
		{
			var adapter = new EngineeringDataAdapter();
			var lossMap = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("Retarder Speed [rpm],Loss Torque [Nm]",
				new[] { "0, 10", "1000, 12", "2000, 18", "2300, 20.58" }));

			//----------------------------------
			var pos = PowertrainPosition.HybridPositionNotSet;

			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.None }, pos);

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.TransmissionInputRetarder }, pos));

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.AxlegearInputRetarder }, pos));

			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.EngineRetarder }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.LossesIncludedInTransmission }, pos);

			//----------------------------------
			pos = PowertrainPosition.BatteryElectricE3;
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.None }, pos);
			Assert.Throws<VectoException>(() =>
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.TransmissionInputRetarder, LossMap = lossMap }, pos));
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.AxlegearInputRetarder, LossMap = lossMap }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.EngineRetarder }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.LossesIncludedInTransmission }, pos);

			//----------------------------------
			pos = PowertrainPosition.BatteryElectricE2;
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.None }, pos);

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.TransmissionInputRetarder }, pos));

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.AxlegearInputRetarder }, pos));

			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.EngineRetarder }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.LossesIncludedInTransmission }, pos);

			//----------------------------------
			pos = PowertrainPosition.HybridP3;

			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.None }, pos);

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.TransmissionInputRetarder }, pos));

			Assert.Throws<VectoException>(() =>
				adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.AxlegearInputRetarder }, pos));

			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.EngineRetarder }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.LossesIncludedInTransmission }, pos);

			//----------------------------------
			pos = PowertrainPosition.HybridP2;
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.None }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.TransmissionInputRetarder, LossMap = lossMap }, pos);
			
			Assert.Throws<VectoException>(() =>
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.AxlegearInputRetarder }, pos));
			
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.EngineRetarder }, pos);
			adapter.CreateRetarderData(new RetarderInputData { Type = RetarderType.LossesIncludedInTransmission }, pos);
		}
	}
}