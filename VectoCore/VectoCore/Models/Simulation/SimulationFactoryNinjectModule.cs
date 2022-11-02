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

using System.Collections.Generic;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils.Ninject;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.Models.Simulation
{
	public class SimulatorFactoryNinjectModule : VectoNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<ISimulatorFactoryFactory>().ToFactory(() => new UseFirstArgumentAsInstanceProvider());

			Bind<ISimulatorFactory>().To<SimulatorFactoryDeclaration>().Named(ExecutionMode.Declaration.ToString());
			Bind<ISimulatorFactory>().To<SimulatorFactoryEngineering>().Named(ExecutionMode.Engineering.ToString());


			Bind<IDeclarationReport>().To<NullDeclarationReport>();
			Bind<IVTPReport>().To<NullVTPReport>();

		}

		#endregion
	}

	internal class NullDeclarationReport : IDeclarationReport
	{
		#region Implementation of IDeclarationReport

		public void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{

		}

		public void PrepareResult(LoadingType loading, Mission mission, int fuelMode, VectoRunData runData)
		{

		}

		public void AddResult(LoadingType loadingType, Mission mission, int fuelMode, VectoRunData runData, IModalDataContainer modData)
		{

		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryResults { get; set; }

		#endregion
	}

	internal class NullVTPReport : IVTPReport
	{
		#region Implementation of IDeclarationReport

		public void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{

		}

		public void PrepareResult(LoadingType loading, Mission mission, int fuelMode, VectoRunData runData)
		{

		}

		public void AddResult(LoadingType loadingType, Mission mission, int fuelMode, VectoRunData runData, IModalDataContainer modData)
		{

		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryResults { get; set; }

		#endregion

		#region Implementation of IVTPReport

		public IVectoHash InputDataHash { set { } }

		public IManufacturerReport ManufacturerRecord { set { } }

		public IVectoHash ManufacturerRecordHash { set { } }

		#endregion
	}
}
