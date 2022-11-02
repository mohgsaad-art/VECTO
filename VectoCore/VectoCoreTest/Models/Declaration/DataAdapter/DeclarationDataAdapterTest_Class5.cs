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
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationDataAdapterTest_Class5
	{
		public const string Class5TractorPTOJob =
			@"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";

		public const int CurbWeight = 8229;

		public const double CdxA = 5.3;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(Class5TractorPTOJob, 0)]
		public void TestClass5_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(10, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062662,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 1)]
		public void TestClass5_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 19300,
				totalRollResistance: 0.00587322,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 2)]
		public void TestClass5_Vehicle_LongHaul_EMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(10, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 3500,
				totalRollResistance: 0.0062159,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 3)]
		public void TestClass5_Vehicle_LongHaul_EMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 26500,
				totalRollResistance: 0.0058192,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 4)]
		public void TestClass5_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062565,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 5)]
		public void TestClass5_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 12900,
				totalRollResistance: 0.0059836,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 6)]
		public void TestClass5_Vehicle_RegionalDelEMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 3500,
				totalRollResistance: 0.006259,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 7)]
		public void TestClass5_Vehicle_RegionalDelEMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 17500,
				totalRollResistance: 0.0059836,
				aerodynamicDragArea: CdxA + 1.5);
		}
	}
}
