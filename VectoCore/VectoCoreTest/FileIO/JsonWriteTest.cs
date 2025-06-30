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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class JsonWriteTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(@"TestData/Generic Vehicles/Declaration Mode/Class9_RigidTruck_6x2/Class9_RigidTruck.vveh")]
		public void SaveVehicleFileDecl(string vehicleFile)
		{
			var outFile = "json_vehicle_write_test.vveh";

			var input = JSONInputDataFactory.ReadComponentData(vehicleFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var vehicleInput = inputProvider.JobInputData.Vehicle;

			var writer = JSONFileWriter.Instance;

			//VECTO_Global.Cfg = new VECTO.Configuration() {DeclMode = true};
			writer.SaveVehicle(
				vehicleInput, vehicleInput.Components.AirdragInputData, vehicleInput.Components.RetarderInputData,
				vehicleInput.Components.PTOTransmissionInputData, vehicleInput.Components.AngledriveInputData, outFile, true);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);

			//Assert.AreEqual(vehicleInput, savedInprovider.JobInputData.Vehicle);

			AssertHelper.PublicPropertiesEqual(
				typeof(IVehicleDeclarationInputData), vehicleInput, savedInprovider.JobInputData.Vehicle,
				new[] {
					"Source", "GearboxInputData", "EngineInputData", "TorqueConverterInputData", "AxleGearInputData", "Identifier"
				});
		}


		[TestCase(@"TestData/Generic Vehicles/Declaration Mode/Class9_RigidTruck_6x2/Engine_324kW_12.7l.veng")]
		public void SaveEngineFileDecl(string engineFile)
		{
			var outFile = "json_engine_write_test.veng";

			var input = JSONInputDataFactory.ReadComponentData(engineFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var engineInputData = inputProvider.JobInputData.Vehicle.Components.EngineInputData;

			var writer = JSONFileWriter.Instance;

			//VECTO_Global.Cfg = new VECTO.Configuration() { DeclMode = true };
			writer.SaveEngine(engineInputData, outFile, true);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);

			AssertHelper.PublicPropertiesEqual(
				typeof(IEngineDeclarationInputData), engineInputData,
				savedInprovider.JobInputData.Vehicle.Components.EngineInputData,
				new[] { "Source", "EngineModes" });
		}

		[TestCase(@"TestData/Generic Vehicles/Declaration Mode/Class9_RigidTruck_6x2/AMT_12.vgbx")]
		public void SaveGearboxAxlegearFileDecl(string engineFile)
		{
			var outFile = "json_gearbox_write_test.vgbx";

			var input = JSONInputDataFactory.ReadComponentData(engineFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var components = inputProvider.JobInputData.Vehicle.Components;

			var writer = JSONFileWriter.Instance;

			//VECTO_Global.Cfg = new VECTO.Configuration() { DeclMode = true };
			writer.SaveGearbox(
				components.GearboxInputData, components.AxleGearInputData, components.TorqueConverterInputData,
				(IGearshiftEngineeringInputData)components.GearboxInputData, outFile, true);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);

			AssertHelper.PublicPropertiesEqual(
				typeof(IGearboxDeclarationInputData), components.GearboxInputData,
				savedInprovider.JobInputData.Vehicle.Components.GearboxInputData,
				new[] { "Source" });
			AssertHelper.PublicPropertiesEqual(
				typeof(IAxleDeclarationInputData), components.AxleGearInputData,
				savedInprovider.JobInputData.Vehicle.Components.AxleGearInputData,
				new[] { "Source" });
		}
	}
}
