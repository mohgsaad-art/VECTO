using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public class SimulatorFactoryDeclaration : SimulatorFactory
    {
		private readonly IXMLInputDataReader _xmlInputDataReader;
		private readonly ISimulatorFactoryFactory _simFactoryFactory;
		private IInputDataProvider _currentStageInputData;
		private IDeclarationReport _currentStageDeclarationReport;
		private IVTPReport _currentStageVTPReport;
		private readonly IXMLDeclarationReportFactory _xmlDeclarationReportFactory;


		public SimulatorFactoryDeclaration(IInputDataProvider dataProvider, 
			IOutputDataWriter writer,
			IDeclarationReport declarationReport, 
			IVTPReport vtpReport,
			bool validate,
			IXMLInputDataReader xmlInputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IVectoRunDataFactoryFactory runDataFactoryFactory
			) : base(ExecutionMode.Declaration, writer, validate)
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
				//CreateDeclarationDataReader(_currentStageInputData, _currentStageDeclarationReport, _currentStageVTPReport);
				DataReader = runDataFactoryFactory.CreateDeclarationRunDataFactory(_currentStageInputData, _currentStageDeclarationReport,
					_currentStageVTPReport);
			} else {
				System.Diagnostics.Debug.Assert(_followUpSimulatorFactoryCreator == null,
					"We should not create a followupSimulator factory if we aren't simulating anything in this step");
			}
		}

		private void CreateReport()
		{
			if (_currentStageDeclarationReport != null && _currentStageInputData != null) {

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
			IXMLInputDataReader xmlInputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory, 
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IVectoRunDataFactoryFactory runDataFactoryFactory) : this(
			dataProvider: dataProvider, 
			declarationReport: null,
			writer: writer,
			vtpReport: null, 
			validate: validate,
			xmlInputDataReader: xmlInputDataReader, 
			simulatorFactoryFactory: simulatorFactoryFactory,
			xmlDeclarationReportFactory: xmlDeclarationReportFactory,
			runDataFactoryFactory: runDataFactoryFactory)
		{

		}

		private bool CanBeSimulated(IInputDataProvider dataProvider)
		{
			if (dataProvider is IMultistageVIFInputData multistageVifInputData &&
				multistageVifInputData.VehicleInputData == null) {
				var inputComplete = multistageVifInputData.MultistageJobInputData.JobInputData.InputComplete;
				var declType = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage
					?.Vehicle.VehicleDeclarationType;
				var final = declType == VehicleDeclarationType.final;
				var exempted = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage?
					.Vehicle.ExemptedVehicle == true;

				if (!((final || exempted) && inputComplete)) {
					return false;
				}
			}
			
			return true;
		}

		private IFollowUpSimulatorFactoryCreator CreateFollowUpFactoryCreator(IInputDataProvider currentStageInputData, IDeclarationReport currentStageDeclarationReport)
		{
			switch (currentStageInputData) {
				case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputDataProvider:
					return new InterimAfterPrimaryFactoryCreator(
							multistagePrimaryAndStageInputDataProvider,
							ReportWriter,
							currentStageDeclarationReport,
						_simFactoryFactory, _xmlInputDataReader, Validate);
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

		private void CreateDeclarationDataReader(IInputDataProvider dataProvider, IDeclarationReport declarationReport, IVTPReport vtpReport)
		{
			switch (dataProvider) {
				case IVTPDeclarationInputDataProvider vtpProvider: {
					DataReader = CreateRunDataReader(vtpProvider, vtpReport);
					return;
				}
				case ISingleBusInputDataProvider singleBusProvider: {
					DataReader = CreateRunDataReader(singleBusProvider, declarationReport);
					return;
				}
				case IDeclarationInputDataProvider declDataProvider: {
					DataReader = CreateRunDataReader(declDataProvider, declarationReport);
					return;
				}
				case IMultistageVIFInputData multistageVifInputData: {
					DataReader = CreateRunDataReader(multistageVifInputData, declarationReport);
					return;
				}
				case IMultistagePrimaryAndStageInputDataProvider multiStagePrimaryAndStageInputData: {

					throw new VectoException("Why are we here ? ");
					
                        //Create Temporary Writer to hold the files only in Memory
#pragma warning disable 162
						var tempOutputWriter = new TempFileOutputWriter(ReportWriter, ReportType.DeclarationReportManufacturerXML);
                        var originalReportWriter = ReportWriter;
                        ReportWriter = tempOutputWriter;
                        var tempPrimaryReport = new XMLDeclarationReportPrimaryVehicle(tempOutputWriter, true);


                        DataReader = CreateRunDataReader(multiStagePrimaryAndStageInputData.PrimaryVehicle, tempPrimaryReport);


                        CreateFollowUpSimulatorFactory = true;
                        Func<ISimulatorFactory> _followingSimulatorFactoryCreator = () =>
                        {
                            try
                            {
                                var primaryInputData = _xmlInputDataReader.CreateDeclaration(tempOutputWriter
                                    .GetDocument(ReportType.DeclarationReportPrimaryVehicleXML).CreateReader());

                                var vifInputData = new XMLDeclarationVIFInputData(
                                    primaryInputData as IMultistageBusInputDataProvider,
                                    multiStagePrimaryAndStageInputData.StageInputData);

                                var manStagesCount =
                                    vifInputData.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? -1;

                                originalReportWriter.NumberOfManufacturingStages = manStagesCount;

                                return _simFactoryFactory.Factory(_mode, vifInputData, originalReportWriter, null,
                                    vtpReport, Validate);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Failed to create additional Simulation run: {ex.Message}");
                                return null;
                            }
                        };
                        return;
#pragma warning restore 162

					}
				default:
					throw new VectoException("Unknown InputData for Declaration Mode!");
			}
		}

		private IVectoRunDataFactory CreateRunDataReader(IMultistageVIFInputData multistageVifInputData, IDeclarationReport declarationReport)
		{
			if (multistageVifInputData.VehicleInputData == null)
			{
				//THIS DOESN'T WORK :/
				var reportCompleted = declarationReport; //; new XMLDeclarationReportCompletedVehicle(ReportWriter, true)
				//{
				//	PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
				//};
				return new DeclarationModeCompletedMultistageBusVectoRunDataFactory(
					multistageVifInputData.MultistageJobInputData,
					reportCompleted);
			} else {
				var report = declarationReport;// ?? new XMLDeclarationReportMultistageBusVehicle(ReportWriter);
				return new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);


			}
		}

		private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider,
			IDeclarationReport declarationReport)
		{
			var vehicleCategory = declDataProvider.JobInputData.Vehicle.VehicleCategory;
			if(vehicleCategory.IsLorry()) {
				var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
				return new DeclarationModeTruckVectoRunDataFactory(declDataProvider, report);
			}

			if(vehicleCategory.IsBus())
				switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
				{
					case VehicleCategory.HeavyBusCompletedVehicle:
						var reportCompleted = declarationReport;
											//??
											//new XMLDeclarationReportCompletedVehicle(ReportWriter,
											//	declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
											//{
											//	PrimaryVehicleReportInputData = declDataProvider.PrimaryVehicleData,
											//};
						return new DeclarationModeCompletedBusVectoRunDataFactory(declDataProvider, reportCompleted);
					case VehicleCategory.HeavyBusPrimaryVehicle:
						var reportPrimary = declarationReport;
							//??
							//				new XMLDeclarationReportPrimaryVehicle(ReportWriter,
							//					declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);
						return new DeclarationModePrimaryBusVectoRunDataFactory(declDataProvider, reportPrimary);
						
					default:

						break;
				}

			throw new Exception(
				$"Could not create RunDataFactory for Vehicle Category{vehicleCategory}");
		}

		private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider,
			IDeclarationReport declarationReport)
		{
			var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
			return new DeclarationModeSingleBusVectoRunDataFactory(singleBusProvider, report);
		}

		private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider,
			IVTPReport vtpReport)
		{
			var report = vtpReport ?? new XMLVTPReport(ReportWriter);
			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry()) {
				return new DeclarationVTPModeVectoRunDataFactoryLorries(vtpProvider, report);
			}

			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus()) {
				return new DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider, report);
			}

			throw new Exception(
				$"Could not create RunDataFactory for Vehicle Category{vtpProvider.JobInputData.Vehicle.VehicleCategory}");
		}

	}
}
