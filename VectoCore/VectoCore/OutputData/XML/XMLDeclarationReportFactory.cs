using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoCore.OutputData.XML
{
    class XMLDeclarationReportFactory : IXMLDeclarationReportFactory
    {
		private readonly IManufacturerReportFactory _mrfFactory;
		private readonly ICustomerInformationFileFactory _cifFactory;
		private readonly IVIFReportFactory _vifFactory;
		private readonly IVIFReportInterimFactory _vifInterimFactory;

		#region Implementation of IXMLDeclarationReportFactory

		
		public XMLDeclarationReportFactory(IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory, IVIFReportFactory vifFactory, IVIFReportInterimFactory vifInterimFactory)
		{
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
			_vifFactory = vifFactory;
			_vifInterimFactory = vifInterimFactory;
		}
		public IDeclarationReport CreateReport(IInputDataProvider input, IOutputDataWriter outputWriter)
		{
			switch (input) {
				case IMultistepBusInputDataProvider multistageBusInputDataProvider:
					break;
				case ISingleBusInputDataProvider singleBusInputDataProvider:
					return null;
					//throw new NotImplementedException("Single Bus not implemented");
					//return new XMLDeclarationReportSingleBus(outputWriter, _mrfFactory, _cifFactory);
				case IDeclarationInputDataProvider declarationInputDataProvider:
					return CreateDeclarationReport(declarationInputDataProvider, outputWriter);
				case IMultiStageTypeInputData multiStageTypeInputData:
					break;
				case IMultistageVIFInputData multistageVifInputData:
					return CreateDeclarationReport(multistageVifInputData, outputWriter);
				case IPrimaryVehicleInformationInputDataProvider primaryVehicleInformationInputDataProvider:
					break;
				case IVTPDeclarationInputDataProvider vtpProvider: {
					return null;
				}
				case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputData: {
					return null; //No Report for this type (handled in two simulation steps)
				}
				default:
					break;
					
			}
			throw new VectoException($"Could not create Declaration Report for {input.GetType()}", ErrorCodes.REPORT_INVALID_INPUT);
		}

		public IVTPReport CreateVTPReport(IInputDataProvider input, IOutputDataWriter outputWriter)
		{
			return input is IVTPDeclarationInputDataProvider 
				? new XMLVTPReport(outputWriter)
				: null;
		}

		private IDeclarationReport CreateDeclarationReport(IMultistageVIFInputData multiStepVifInputData, IOutputDataWriter outputDataWriter)
		{
			if (multiStepVifInputData.VehicleInputData == null)
			{
				var reportCompleted = new XMLDeclarationReportCompletedVehicle(outputDataWriter, _mrfFactory, _cifFactory, _vifFactory)
				{
					PrimaryVehicleReportInputData = multiStepVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
				};
				return reportCompleted;
			}
			else {
				var report = new XMLDeclarationReportInterimVehicle(outputDataWriter, _mrfFactory, _cifFactory, _vifFactory, _vifInterimFactory);
				return report;
			}
		}

		private IDeclarationReport CreateDeclarationReport(IDeclarationInputDataProvider declarationInputDataProvider,
			IOutputDataWriter outputDataWriter)
		{
			var vehicleCategory = declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory;
			if (vehicleCategory.IsLorry())
			{
				return new XMLDeclarationReport(outputDataWriter, _mrfFactory, _cifFactory);
			}

			if (vehicleCategory.IsBus())
				switch (declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory)
				{
					case VehicleCategory.HeavyBusCompletedVehicle:
						return new XMLDeclarationReportCompletedVehicle(outputDataWriter,_mrfFactory,_cifFactory,_vifFactory)
											{
												PrimaryVehicleReportInputData = declarationInputDataProvider.PrimaryVehicleData,
											};
					case VehicleCategory.HeavyBusPrimaryVehicle:
						return new XMLDeclarationReportPrimaryVehicle(outputDataWriter, _mrfFactory, _vifFactory);

					default:
						break;
				}

			throw new Exception(
				$"Could not create DeclarationReport for Vehicle Category{vehicleCategory}");
		}

		public IVTPReport CreateVTPReport(IVTPDeclarationInputDataProvider input, IOutputDataWriter outputWriter)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
