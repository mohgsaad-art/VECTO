using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public class SimulatorFactoryDeclaration : SimulatorFactory
    {
		protected readonly IXMLInputDataReader _xmlInputDataReader;
		protected readonly ISimulatorFactoryFactory _simFactoryFactory;
		private IInputDataProvider _currentStageInputData;
		private IDeclarationReport _currentStageDeclarationReport;
		private IVTPReport _currentStageVTPReport;
		protected readonly IXMLDeclarationReportFactory _xmlDeclarationReportFactory;
		private IVIFReportFactory _vifFactory;
		private IManufacturerReportFactory _mrfFactory;

		

		public SimulatorFactoryDeclaration(IInputDataProvider dataProvider, 
			IOutputDataWriter writer,
			IDeclarationReport declarationReport, 
			IVTPReport vtpReport,
			bool validate,
			// the following parameters are injected
			IXMLInputDataReader xmlInputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IVectoRunDataFactoryFactory runDataFactoryFactory,
			IPowertrainBuilder ptBuilder,
			IModalDataFactory modDataFactory
		) : base(ExecutionMode.Declaration, writer, validate, ptBuilder, modDataFactory)
		{
			_xmlInputDataReader = xmlInputDataReader;
			_simFactoryFactory = simulatorFactoryFactory;
			_currentStageInputData = dataProvider;
			_xmlDeclarationReportFactory = xmlDeclarationReportFactory;

			_currentStageDeclarationReport = declarationReport ?? xmlDeclarationReportFactory.CreateReport(dataProvider, writer);
			_currentStageVTPReport = vtpReport ?? xmlDeclarationReportFactory.CreateVTPReport(dataProvider, writer);
			_followUpSimulatorFactoryCreator = CreateFollowUpFactoryCreator(
				_currentStageInputData, 
				_currentStageDeclarationReport);

			UpdateCurrentStage();

			_simulate = CanBeSimulated(dataProvider);
			if (_simulate) {
				RunDataFactory = runDataFactoryFactory.CreateDeclarationRunDataFactory(_currentStageInputData, _currentStageDeclarationReport,
					_currentStageVTPReport);

				RunDataFactory.CompletedVehicle = _followUpSimulatorFactoryCreator?.CompletedVehicle;
			} else {
				System.Diagnostics.Debug.Assert(_followUpSimulatorFactoryCreator == null,
					"We should not create a followupSimulator factory if we aren't simulating anything in this step");
			}
		}



		/// <summary>
		/// Modifies the input and output of the current simulation step based on the Following step
		/// </summary>
		private void UpdateCurrentStage()
		{
			ReportWriter = _followUpSimulatorFactoryCreator?.CurrentStageOutputDataWriter ??
										ReportWriter;

			_currentStageDeclarationReport = _followUpSimulatorFactoryCreator?.CurrentStageDeclarationReport ??
											_currentStageDeclarationReport;

			_currentStageInputData = _followUpSimulatorFactoryCreator?.CurrentStageInputData ??
									_currentStageInputData;
		}

		public SimulatorFactoryDeclaration(
			IInputDataProvider dataProvider,
			IOutputDataWriter writer,
			bool validate,
			// the following parameters are injected
			IXMLInputDataReader xmlInputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IVectoRunDataFactoryFactory runDataFactoryFactory,
			IPowertrainBuilder ptBuilder,
			IModalDataFactory modDataFactory
		) : this(
			dataProvider: dataProvider,
			declarationReport: null,
			writer: writer,
			vtpReport: null,
			validate: validate,
			xmlInputDataReader: xmlInputDataReader,
			simulatorFactoryFactory: simulatorFactoryFactory,
			xmlDeclarationReportFactory: xmlDeclarationReportFactory,
			runDataFactoryFactory: runDataFactoryFactory,
			ptBuilder: ptBuilder,
			modDataFactory: modDataFactory) { }

		private bool CanBeSimulated(IInputDataProvider dataProvider)
		{
			if (dataProvider is IMultistageVIFInputData multistageVifInputData &&
				multistageVifInputData.VehicleInputData == null) {
				var inputComplete = multistageVifInputData.MultistageJobInputData.JobInputData.InputComplete;
                var declType = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage
					?.Vehicle.VehicleDeclarationType;
				var final = declType == VehicleDeclarationType.final;

				if (final) {
                    MissingInputEntries = multistageVifInputData.MultistageJobInputData.JobInputData.InvalidEntries
						.Distinct().Where(x => !x.StartsWith("_")).ToList();
                }

                var exempted = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage?
					.Vehicle.ExemptedVehicle == true;

				if (!((final || exempted) && inputComplete)) {
					return false;
				}
			}
			
			return true;
		}

		protected virtual IFollowUpSimulatorFactoryCreator CreateFollowUpFactoryCreator(IInputDataProvider currentStageInputData, IDeclarationReport currentStageDeclarationReport)
		{
			switch (currentStageInputData) {
				case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputDataProvider:
					return new InterimAfterPrimaryFactoryCreator(
							multistagePrimaryAndStageInputDataProvider,
							ReportWriter,
							currentStageDeclarationReport,
						_simFactoryFactory, _xmlDeclarationReportFactory, _xmlInputDataReader, Validate);
				case IMultistageVIFInputData multistageVifInputData:
					if (multistageVifInputData.VehicleInputData != null) {
						return new CompletedAfterInterimPrimaryFactoryCreator(
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
	}
}
