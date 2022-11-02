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

using NUnit.Framework;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SimulationDataReaderTest
	{
		protected const string DeclarationJob = @"TestData\Jobs\12t Delivery Truck.vecto";
		protected const double Tolerance = 0.0001;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[Category("LongRunning")]
		[TestCase]
		public void ReadDeclarationJobFile()
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(DeclarationJob);
			var declarationProvider = dataProvider as IDeclarationInputDataProvider;
			if (declarationProvider == null) {
				throw new VectoException("Failed to cas to Engineering InputDataProvider");
			}
			var reader = new DeclarationModeTruckVectoRunDataFactory(declarationProvider, null);
			//reader.SetJobFile(DeclarationJob);

			var runData = reader.NextRun().First();

			Assert.AreEqual(ExecutionMode.Declaration, runData.ExecutionMode);

			Assert.AreEqual(Path.GetFileNameWithoutExtension(DeclarationJob), runData.JobName);

			// curbweight + bodyCurbWeight + trailerCurbWeight (for Long Haul only)
			Assert.AreEqual(5850 + 1900 + 3400, runData.VehicleData.TotalCurbMass.Value());

			Assert.AreEqual(11900, runData.VehicleData.GrossVehicleMass.Value());
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, runData.VehicleData.AxleConfiguration);
			AssertHelper.AreRelativeEqual(0.40726954, runData.VehicleData.DynamicTyreRadius);

			Assert.AreEqual(VehicleClass.Class2, runData.VehicleData.VehicleClass);
			Assert.AreEqual(4, runData.VehicleData.AxleData.Count);
			Assert.AreEqual(6, runData.VehicleData.AxleData[0].Inertia.Value(), Tolerance);

			Assert.AreEqual(true, runData.DriverData.LookAheadCoasting.Enabled);
			//Assert.AreEqual(DeclarationData.Driver.LookAhead.MinimumSpeed.Value(),
			//	runData.DriverData.LookAheadCoasting.MinSpeed.Value(), Tolerance);
			//Assert.AreEqual(DeclarationData.Driver.LookAhead.Deceleration.Value(),
			//	runData.DriverData.LookAheadCoasting.Deceleration.Value(), Tolerance);

			Assert.IsTrue(runData.DriverData.OverSpeed.Enabled);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.MinSpeed.Value(),
				runData.DriverData.OverSpeed.MinSpeed.Value(), Tolerance);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.AllowedOverSpeed.Value(),
				runData.DriverData.OverSpeed.OverSpeed.Value(), Tolerance);

			Assert.AreEqual(3.7890, runData.EngineData.Inertia.Value(), 1e-6);

			var downshiftSpeeds = new[] { 660, 660, 800, 1000, 1087.625, 1087.625};
			var downshiftTorque = new[] { -163.9, 623.966, 725.102, 848.0332, 872.2098, 988.9 };

			Assert.AreEqual(downshiftSpeeds.Length, runData.GearboxData.Gears[2].ShiftPolygon.Downshift.Count);
			for (var i = 0; i < downshiftSpeeds.Length; i++) {
				Assert.AreEqual(downshiftSpeeds[i].RPMtoRad().Value(),
					runData.GearboxData.Gears[2].ShiftPolygon.Downshift[i].AngularSpeed.Value(), Tolerance, "i: " + i);
				Assert.AreEqual(downshiftTorque[i], runData.GearboxData.Gears[2].ShiftPolygon.Downshift[i].Torque.Value(), Tolerance,
					"i: " + i);
			}

			var upshiftSpeed = new[] {2318.28077, 2318.28077 };
			var upshiftTorque = new[] { -163.9, 988.9 };

			Assert.AreEqual(upshiftSpeed.Length, runData.GearboxData.Gears[2].ShiftPolygon.Upshift.Count);
			for (var i = 0; i < upshiftSpeed.Length; i++) {
				Assert.AreEqual(upshiftSpeed[i].RPMtoRad().Value(),
					runData.GearboxData.Gears[1].ShiftPolygon.Upshift[i].AngularSpeed.Value(), Tolerance);
				Assert.AreEqual(upshiftTorque[i], runData.GearboxData.Gears[1].ShiftPolygon.Upshift[i].Torque.Value(), Tolerance);
			}
			//Assert.AreEqual(, runData.DriverData.LookAheadCoasting.);
		}
	}
}