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

using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	public class DeclarationAdapterTestHelper
	{
		public static VectoRunData[] CreateVectoRunData(string file)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeTruckVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();
			return runData;
		}

		public static void AssertVehicleData(VehicleData vehicleData, AirdragData airdragData, VehicleCategory vehicleCategory,
			VehicleClass vehicleClass, AxleConfiguration axleConfiguration, double wheelsInertia, double totalVehicleWeight,
			double totalRollResistance, double aerodynamicDragArea)
		{
			Assert.AreEqual(vehicleCategory, vehicleData.VehicleCategory, "VehicleCategory");
			Assert.AreEqual(vehicleClass, vehicleData.VehicleClass, "VehicleClass");
			Assert.AreEqual(axleConfiguration, vehicleData.AxleConfiguration, "AxleConfiguration");
			Assert.AreEqual(totalVehicleWeight, vehicleData.TotalVehicleMass.Value(), 1e-3, "TotalVehicleWeight");
			Assert.AreEqual(wheelsInertia, vehicleData.WheelsInertia.Value(), 1e-6, "WheelsInertia");
			Assert.AreEqual(totalRollResistance, vehicleData.TotalRollResistanceCoefficient, 1e-6, "TotalRollResistance");

			Assert.AreEqual(aerodynamicDragArea, airdragData.CrossWindCorrectionCurve.AirDragArea.Value(), 1e-6, "Cd x A");
		}
	}
}
