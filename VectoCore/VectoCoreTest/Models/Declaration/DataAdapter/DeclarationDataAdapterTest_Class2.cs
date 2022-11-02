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
using TUGraz.VectoCommon.Models;
using System.IO;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationDataAdapterTest_Class2
	{
		public const string Class2RigidTruckNoEMSJob =
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto";

		public const int CurbWeight = 4670;
		public const double CdxA = 4.83;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 0)]
		public void TestClass2_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(6, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 603.917 + 700,
				totalRollResistance: 0.0069554,
				aerodynamicDragArea: CdxA + 1.3);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 1)]
		public void TestClass2_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 4541.176 + 5300,
				totalRollResistance: 0.0065733,
				aerodynamicDragArea: CdxA + 1.3);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 2)]
		public void TestClass2_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 3)]
		public void TestClass2_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248,
				aerodynamicDragArea: CdxA);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 4)]
		public void TestClass2_Vehicle_UrbanDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);
			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 5)]
		public void TestClass2_Vehicle_UrbanDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248,
				aerodynamicDragArea: CdxA);
		}
	}
}
