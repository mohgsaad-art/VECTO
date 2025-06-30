using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Mockup;
using TUGraz.VectoCore.Mockup.Simulation.RundataFactories;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Ninject
{
    public class MockupModule : AbstractNinjectModule
    {
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<CIFMockupModule>();
			LoadModule<MRFMockupModule>();
			LoadModule<SimulatorFactoryModule>();
			LoadModule<VIFMockupModule>();

			Rebind<IVectoRunDataFactoryFactory>().To<VectoMockUpRunDataFactoryFactory>();
			Rebind<IXMLDeclarationReportFactory>().To<MockupReportFactory>();
			Rebind<IXMLInputDataReader>().To<MockupXMLInputDataFactory>();
			Rebind<IResultsWriterFactory>().To<MockupReportResultsFactory>().InSingletonScope();

			Rebind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>();

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.BatteryElectricVehicle.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IEPC_E.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
                .Named(VectoSimulationJobType.Multiple_FCHV.ToString());

            Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
                .Named(VectoSimulationJobType.Multiple_PEV.ToString());

            Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
                .Named(VectoSimulationJobType.Multiple_SHEV.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.SerialHybridVehicle.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IEPC_S.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.FCHV.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.FCHV_IEPC.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.ParallelHybridVehicle.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.IHPC.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.EngineOnlySimulation.ToString());

			Bind<IModalDataPostProcessor>().To<MockupModalDataPostprocessingCorrection>()
				.Named(VectoSimulationJobType.ConventionalVehicle.ToString());
        }

		#endregion
	}

}
