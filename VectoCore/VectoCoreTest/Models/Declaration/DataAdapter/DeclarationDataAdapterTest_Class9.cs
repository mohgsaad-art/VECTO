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

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationDataAdapterTest_Class9
	{
		public const string Class9RigidTruckJob =
			@"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";

		public const int CurbWeight = 9300;
		public const double CdxA = 5.2;

		[TestCase(Class9RigidTruckJob, 0)]
		public void TestClass9_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(10, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 196,
				totalVehicleWeight: CurbWeight + 2200 + 5400 + 2600,
				totalRollResistance: 0.0059426,
				aerodynamicDragArea: CdxA + 1.5);
		}


		[TestCase(Class9RigidTruckJob, 1)]
		public void TestClass9_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 196,
				totalVehicleWeight: CurbWeight + 2200 + 5400 + 19300,
				totalRollResistance: 0.00558611,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class9RigidTruckJob, 2)]
		public void TestClass9_Vehicle_LongHaul_EMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 294,
				totalVehicleWeight: CurbWeight + 2200 + 2500 + 7500 + 3500,
				totalRollResistance: 0.0060500,
				aerodynamicDragArea: CdxA + 2.1);
		}

		[TestCase(Class9RigidTruckJob, 3)]
		public void TestClass9_Vehicle_LongHaul_EMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 294,
				totalVehicleWeight: CurbWeight + 2200 + 2500 + 7500 + 26500,
				totalRollResistance: 0.0056680,
				aerodynamicDragArea: CdxA + 2.1);
		}

		[TestCase(Class9RigidTruckJob, 4)]
		public void TestClass9_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: CurbWeight + 2200 + 1400,
				totalRollResistance: 0.0059109,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class9RigidTruckJob, 5)]
		public void TestClass9_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: CurbWeight + 2200 + 7100,
				totalRollResistance: 0.0056986,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class9RigidTruckJob, 6)]
		public void TestClass9_Vehicle_RegionalDel_EMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 294,
				totalVehicleWeight: CurbWeight + 2200 + 2500 + 7500 + 3500,
				totalRollResistance: 0.0060425,
				aerodynamicDragArea: CdxA + 2.1);
		}

		[TestCase(Class9RigidTruckJob, 7)]
		public void TestClass9_Vehicle_RegionalDel_EMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 294,
				totalVehicleWeight: CurbWeight + 2200 + 2500 + 7500 + 17500,
				totalRollResistance: 0.0057797,
				aerodynamicDragArea: CdxA + 2.1);
		}

		[TestCase(Class9RigidTruckJob, 8)]
		public void TestClass9_Vehicle_Municipal_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: CurbWeight + 6750 + 1200,
				totalRollResistance: 0.0057417,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class9RigidTruckJob, 9)]
		public void TestClass9_Vehicle_Municipal_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: CurbWeight + 6750 + 6000,
				totalRollResistance: 0.005602,
				aerodynamicDragArea: CdxA);
		}
	}
}
