using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.Impl;

namespace TUGraz.VectoCore.OutputData.XML
{
    class XMLDeclarationReportFactory : IXMLDeclarationReportFactory
    {
		#region Implementation of IXMLDeclarationReportFactory

		public IDeclarationReport CreateReport(IInputDataProvider input, IOutputDataWriter outputWriter)
		{
			switch (input) {
				case IMultistageBusInputDataProvider multistageBusInputDataProvider:
					break;
				case ISingleBusInputDataProvider singleBusInputDataProvider:
					return new XMLDeclarationReport(outputWriter);
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
			throw new VectoException($"Could not create Declaration Report for {input.GetType()}");
		}

		public IVTPReport CreateVTPReport(IInputDataProvider input, IOutputDataWriter outputWriter)
		{
			return input is IVTPDeclarationInputDataProvider 
				? new XMLVTPReport(outputWriter)
				: null;
		}

		private IDeclarationReport CreateDeclarationReport(IMultistageVIFInputData multistageVifInputData, IOutputDataWriter outputDataWriter)
		{
			if (multistageVifInputData.VehicleInputData == null)
			{
				var reportCompleted = new XMLDeclarationReportCompletedVehicle(outputDataWriter, true)
				{
					PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
				};
				return reportCompleted;
			}
			else {
				var report = new XMLDeclarationReportMultistageBusVehicle(outputDataWriter);
				return report;
			}
		}

		private IDeclarationReport CreateDeclarationReport(IDeclarationInputDataProvider declarationInputDataProvider,
			IOutputDataWriter outputDataWriter)
		{
			var vehicleCategory = declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory;
			if (vehicleCategory.IsLorry())
			{
				return new XMLDeclarationReport(outputDataWriter);
			}

			if (vehicleCategory.IsBus())
				switch (declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory)
				{
					case VehicleCategory.HeavyBusCompletedVehicle:
						return new XMLDeclarationReportCompletedVehicle(outputDataWriter,
												declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
											{
												PrimaryVehicleReportInputData = declarationInputDataProvider.PrimaryVehicleData,
											};
					case VehicleCategory.HeavyBusPrimaryVehicle:
						return new XMLDeclarationReportPrimaryVehicle(outputDataWriter,
												declarationInputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);

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
