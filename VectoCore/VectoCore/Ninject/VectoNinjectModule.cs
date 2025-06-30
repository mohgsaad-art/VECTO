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

#if(MOCKUP)
using System.IO;
using System.Reflection;
#endif
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.ComponentWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoMockup.Ninject;

namespace TUGraz.VectoCore
{
	public abstract class AbstractNinjectModule : NinjectModule
	{
		protected virtual void LoadModule<T>() where T : class, INinjectModule, new()
		{
			if (Kernel != null && !Kernel.HasModule(typeof(T).FullName)) {
				Kernel.Load(new[] { new T() });
			}

		}
	}

	public class VectoNinjectModule : AbstractNinjectModule
	{
		private readonly bool _mockup;

		#region Overrides of NinjectModule

		public override void Load()
		{
			// necessary for injecting IShiftStrategyFactory into AbstractSimulationDataAdapter, PrimaryBusBase, CompletedBusDeclarationBase, SingleBusBase
			// as the property there is private
			Kernel.Settings.InjectNonPublic = true;
			Kernel.Settings.InjectParentPrivateProperties = true;

            LoadModule<XMLInputDataNinjectModule>();

			LoadModule<SimulatorFactoryNinjectModule>();

			LoadModule<PowertrainComponentNinjectModule>();

			LoadModule<XMLDeclarationReportFactoryNinjectModule>();	

			LoadModule<VectoRunDataFactoryNinjectModule>();

			LoadModule<ShiftStrategyNinjectModule>();

			LoadModule<DeclarationDataAdapterNinjectModule>();

			LoadModule<EngineeringDataAdapterNinjectModule>();

			LoadModule<GroupWriterNinjectModule>();

			LoadModule<ComponentWriterNinjectModule>();

			LoadModule<ResultsNinjectModule>();

			LoadModule<MRFNinjectModule>();

			LoadModule<MRFResultsNinjectModule>();

			LoadModule<CIFNinjectModule>();

			LoadModule<CIFResultsNinjectModule>();
			
			LoadModule<VIFNinjectModule>();

			LoadModule<VIFResultsNinjectModule>();

			LoadModule<PostProcessingNinjectModule>();


			#if (MOCKUP)  //TODO: add second constant for release
				LoadModule<MockupModule>();
			#endif
		}
		#endregion
	}
}
