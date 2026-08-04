using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public interface IFollowUpSimulatorFactoryCreator
	{ 
		ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries);

		IOutputDataWriter CurrentStageOutputDataWriter { get; }

		IDeclarationReport CurrentStageDeclarationReport { get; }
		IInputDataProvider CurrentStageInputData { get; }

        IVehicleDeclarationInputData CompletedVehicle { get; }
    }


	public abstract class FollowUpSimulatorFactoryCreator : IFollowUpSimulatorFactoryCreator
	{
		protected ISimulatorFactoryFactory _simulatorFactoryFactory;
		protected readonly bool _validate;

		protected FollowUpSimulatorFactoryCreator(ISimulatorFactoryFactory simFactoryFactory, bool validate)
		{
			_simulatorFactoryFactory = simFactoryFactory;
			_validate = validate;
		}

		#region Implementation of IFollowUpSimulatorFactoryCreator

		public abstract ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries);
		public abstract IOutputDataWriter CurrentStageOutputDataWriter { get; }
		public abstract IDeclarationReport CurrentStageDeclarationReport { get; }
		public abstract IInputDataProvider CurrentStageInputData { get; }
		public abstract IVehicleDeclarationInputData CompletedVehicle { get; }
        #endregion
    }


	public class InterimAfterPrimaryFactoryCreator : FollowUpSimulatorFactoryCreator
	{
		protected readonly TempFileOutputWriter _currentStageOutputDataWriter = null;
		private readonly IOutputDataWriter _originalReportWriter = null;
		protected IDeclarationReport _currentStageDeclarationReport = null;
		private readonly IXMLInputDataReader _inputDataReader;
		private readonly IMultistagePrimaryAndStageInputDataProvider _originalStageInputData;
		private readonly IDeclarationReport _originalDeclarationReport;
		private readonly IDeclarationInputDataProvider _currentStageInputData;

		public InterimAfterPrimaryFactoryCreator(IMultistagePrimaryAndStageInputDataProvider originalStageInputData,
			IOutputDataWriter originalReportWriter,
			IDeclarationReport originalDeclarationReport,
			ISimulatorFactoryFactory simFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IXMLInputDataReader inputDataReader, bool validate) : base(simFactoryFactory, validate)
		{

			_originalStageInputData = originalStageInputData;
			_inputDataReader = inputDataReader;
			_originalReportWriter = originalReportWriter;
			_originalDeclarationReport = originalDeclarationReport;
			_currentStageOutputDataWriter =
				new TempFileOutputWriter(
					originalReportWriter, 
					ReportType.DeclarationReportManufacturerXML, 
					ReportType.DeclarationReportMonitoringXML);

			_currentStageDeclarationReport =
				xmlDeclarationReportFactory.CreateReport(originalStageInputData.PrimaryVehicle, _currentStageOutputDataWriter);
			_currentStageInputData = originalStageInputData.PrimaryVehicle;

		}



		#region Implementation of IFollowUpSimulatorFactoryCreator

		public override ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries)
		{
			var aborted = progressEntries.Where(e => e.Value.Error != null).ToArray();
			if (aborted.Any()) {
				throw new VectoException("Not all primary simulation runs finished successfully!");
			}
			//Prepare inputdata for next simulation step
			var primaryInputData = _inputDataReader.CreateDeclaration(_currentStageOutputDataWriter
				.GetDocument(ReportType.DeclarationReportPrimaryVehicleXML).CreateReader()) as IMultistepBusInputDataProvider;
			Debug.Assert(primaryInputData != null);
			var stageInputData = _originalStageInputData.StageInputData;

			var nextStageInput = new XMLDeclarationVIFInputData(primaryInputData, stageInputData, _originalStageInputData.SimulateResultingVIF);

			var manStagesCount =
				nextStageInput.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? -1;

			_originalReportWriter.NumberOfManufacturingStages = manStagesCount;

			return _simulatorFactoryFactory.Factory(ExecutionMode.Declaration, nextStageInput, _originalReportWriter, _originalDeclarationReport,
				null, _validate);
		}

		#endregion


		#region Overrides of FollowUpSimulatorFactoryCreator

		public override IInputDataProvider CurrentStageInputData => _currentStageInputData;
		public override IOutputDataWriter CurrentStageOutputDataWriter => _currentStageOutputDataWriter;
		public override IDeclarationReport CurrentStageDeclarationReport => _currentStageDeclarationReport;
		public override IVehicleDeclarationInputData CompletedVehicle => _originalStageInputData.StageInputData;

        #endregion

    }

	
	public class CompletedAfterInterimPrimaryFactoryCreator : FollowUpSimulatorFactoryCreator
	{
		private readonly IXMLInputDataReader _xmlInputDataReader;


		public CompletedAfterInterimPrimaryFactoryCreator(IMultistageVIFInputData originalInputData,
			IOutputDataWriter originalOutputDataWriter,
			IDeclarationReport originalDeclarationReport,
			IXMLInputDataReader inputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory, 
			bool validate) : base(simulatorFactoryFactory, validate)
		{
			_currentStageOutputDataWriter = originalOutputDataWriter;
			_originalInputData = originalInputData;
			_currentStageDeclarationReport = originalDeclarationReport;
			_currentStageInputData = originalInputData;
			_xmlInputDataReader = inputDataReader;

		}
		public override ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries)
		{
			if (!_originalInputData.SimulateResultingVIF) {
				return null;
			}
			var output = _xmlInputDataReader.CreateDeclaration(
				XmlReader.Create(CurrentStageOutputDataWriter.MultistageXmlReport.ToString().ToStream())) as IMultistepBusInputDataProvider;
            var nextStageInput = new XMLDeclarationVIFInputData(output, null, true, _originalInputData.VehicleInputData.VehicleMonitoringData);

			return _simulatorFactoryFactory.Factory(ExecutionMode.Declaration, nextStageInput, CurrentStageOutputDataWriter, null, null,
				_validate);
		}

		private readonly IOutputDataWriter _currentStageOutputDataWriter;

		public override IOutputDataWriter CurrentStageOutputDataWriter => _currentStageOutputDataWriter;

		private readonly IDeclarationReport _currentStageDeclarationReport;

		public override IDeclarationReport CurrentStageDeclarationReport => _currentStageDeclarationReport;

		private readonly IInputDataProvider _currentStageInputData;
		private readonly IMultistageVIFInputData _originalInputData;

		public override IInputDataProvider CurrentStageInputData => _currentStageInputData;

        public override IVehicleDeclarationInputData CompletedVehicle => null;
    }
}
