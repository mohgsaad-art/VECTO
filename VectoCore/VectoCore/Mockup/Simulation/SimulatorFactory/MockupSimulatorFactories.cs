using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Simulation.SimulatorFactory
{
    internal class MockupEngineeringSimulatorFactory : SimulatorFactoryEngineering
	{
		public MockupEngineeringSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer,
			bool validate, IVectoRunDataFactoryFactory runDataFactoryFactory) : base(dataProvider, writer, validate, runDataFactoryFactory, null, null)
		{
			throw new VectoException("Engineering mode is not supported in Mockup Vecto");
		}
	}


	internal class MockupDeclarationSimulatorFactory : SimulatorFactoryDeclaration
    {
		protected ISimplePowertrainBuilder  SimplePowertrainBuilder { get; private set; }

		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer,
			IDeclarationReport declarationReport, IVTPReport vtpReport, bool validate,
			// the following parameters are injected
			IXMLInputDataReader xmlInputDataReader, ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory, IVectoRunDataFactoryFactory runDataFactoryFactory,
			IPowertrainBuilder ptBuilder, ISimplePowertrainBuilder simplePtBuilder, IModalDataFactory modDataFactory)
			: base(dataProvider, writer, declarationReport, vtpReport, validate, xmlInputDataReader,
				simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory, ptBuilder, modDataFactory)
		{
			SimplePowertrainBuilder = simplePtBuilder;
			CheckInputData(dataProvider);
		}

		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider,
			IOutputDataWriter writer, bool validate,
			// the following parameters are injected
			IXMLInputDataReader xmlInputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IVectoRunDataFactoryFactory runDataFactoryFactory, IPowertrainBuilder ptBuilder, IModalDataFactory modDataFactory)
			: base(dataProvider, writer, validate,
				xmlInputDataReader, simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory,
				ptBuilder, modDataFactory)
		{
			CheckInputData(dataProvider);
		}

        #region Overrides of SimulatorFactory

		private void CheckInputData(IInputDataProvider dataProvider)
		{
			if (dataProvider is JSONFile json && !(dataProvider is JSONInputDataV10_PrimaryAndStageInputBus || dataProvider is JSONInputDataCompletedBusFactorMethodV7)) {
				throw new VectoException($"JSON input data is not supported in Mockup Vecto");
			}
		}

        protected override IVectoRun GetExemptedRun(VectoRunData data)
		{

			if (data.Report != null)
			{
				data.Report.PrepareResult(data);
			}
			return new MockupExemptedRun(new ExemptedVehicleContainer(data, null, null, SimplePowertrainBuilder), modData => {
				if (data.Report != null)
				{
					data.Report.AddResult(data, modData);
				}
			});
		}

		protected override IVectoRun GetNonExemptedRun(VectoRunData data, int current, ref bool warning1Hz, ref bool firstRun)
		{
			var addReportResult = PrepareReport(data);
			return new MockupRun(VehicleContainer.CreateVehicleContainer(data,
					new MockupModalDataContainer(ModDataFactory.CreateModDataContainer(data, ReportWriter, null, null), addReportResult), null));
			
		}
		protected new static Action<IModalDataContainer> PrepareReport(VectoRunData data)
		{
			if (data.Report != null)
			{
				data.Report.PrepareResult(data);
			}
			Action<IModalDataContainer> addReportResult = modData => {
				if (modData is MockupModalDataContainer && data.Report != null) {
					data.Report.AddResult(data, modData);
				}

				return;
				//if (data.Report != null)
				//{
				//	data.Report.AddResult(data.Loading, data.Mission, data.EngineData?.FuelMode ?? 0, data, modData);
				//}
			};

			return addReportResult;
		}

		#region Overrides of SimulatorFactoryDeclaration

		protected override IFollowUpSimulatorFactoryCreator CreateFollowUpFactoryCreator(IInputDataProvider currentStageInputData,
			IDeclarationReport currentStageDeclarationReport)
		{
			switch (currentStageInputData)
			{
				case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputDataProvider:
					return new MockupInterimAfterPrimaryFactoryCreator(
						multistagePrimaryAndStageInputDataProvider,
						ReportWriter,
						currentStageDeclarationReport,
						_simFactoryFactory, 
						//((IMockupDeclarationReportFactory)_xmlDeclarationReportFactory).MrfFactory,
						//((IMockupDeclarationReportFactory)_xmlDeclarationReportFactory).CifFactory, 
						//((IMockupDeclarationReportFactory)_xmlDeclarationReportFactory).VifFactory,
						_xmlDeclarationReportFactory,
						_xmlInputDataReader, 
						Validate);
				case IMultistageVIFInputData multistageVifInputData:
					if (multistageVifInputData.VehicleInputData != null)
					{
						return new MockupCompletedAfterPrimaryFactoryCreator(
							multistageVifInputData,
							ReportWriter,
							currentStageDeclarationReport,
							_xmlInputDataReader,
							_simFactoryFactory, Validate);
					}
					break;
				default:
					return null;
			}
			return null;
		}

		#endregion

		#endregion
	}
}
