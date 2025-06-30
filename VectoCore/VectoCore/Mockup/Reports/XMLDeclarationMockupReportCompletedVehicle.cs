using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;

namespace TUGraz.VectoMockup.Reports
{

    /// <summary>
    /// Create VIF of an interim (or the complete(d) step
    /// </summary>
    public class XMLDeclarationMockupReportInterimVehicle : XMLDeclarationReportInterimVehicle
	{
		private readonly bool _exempted;

		public XMLDeclarationMockupReportInterimVehicle(IReportWriter writer, IManufacturerReportFactory mrfFactory,
			ICustomerInformationFileFactory cifFactory, IVIFReportFactory vifFactory, IVIFReportInterimFactory interimFactory, bool exempted) : base(writer,
			mrfFactory,
			cifFactory, vifFactory, interimFactory)
		{
			_exempted = exempted;
		}
	}


	/// <summary>
		/// Create MRF and CIF of the complete(d) step
		/// </summary>
	public class XMLDeclarationMockupReportCompletedVehicle : XMLDeclarationReportCompletedVehicle
	{
		//private readonly IManufacturerReportFactory _mrfFactory;
		//private readonly ICustomerInformationFileFactory _cifFactory;
		private readonly bool _exempted;
		private VectoRunData _modelData;

		public XMLDeclarationMockupReportCompletedVehicle(IReportWriter writer, IManufacturerReportFactory mrfFactory, 
			ICustomerInformationFileFactory cifFactory, IVIFReportFactory vifFactory, bool exempted) : base(writer, mrfFactory, cifFactory, vifFactory)
		{
			//_mrfFactory = mrfFactory;
			//_cifFactory = cifFactory;

			_exempted = exempted;
		}

		#region Overrides of XMLDeclarationReportCompletedVehicle

		//protected override void InstantiateReports(VectoRunData modelData)
		//{
		//	var inputData = modelData.InputData as IXMLMultistageInputDataProvider;
		//	var primaryVehicle = inputData.JobInputData.PrimaryVehicle.Vehicle;

		//	var ihpc = (primaryVehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
		//	var iepc = (primaryVehicle.Components?.IEPC != null);
		//	ManufacturerRpt = _mrfFactory.GetManufacturerReport(
		//		inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
		//		inputData.JobInputData.JobType,
		//		primaryVehicle.ArchitectureID,
		//		primaryVehicle.ExemptedVehicle,
		//		iepc,
		//		ihpc);

		//	CustomerRpt = _cifFactory.GetCustomerReport(
		//		inputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
		//		inputData.JobInputData.JobType,
		//		primaryVehicle.ArchitectureID,
		//		primaryVehicle.ExemptedVehicle,
		//		iepc,
		//		ihpc);
			
		//}

        #endregion

		#region Overrides of XMLDeclarationReportCompletedVehicle

		public override void InitializeReport(VectoRunData modelData)
		{
			base.InitializeReport(modelData);
			_modelData = modelData;
		}

		#endregion

		protected internal override void DoWriteReport()
        {
			foreach (var result in OrderedResults) {
				WriteResult(result);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}

       protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			
		}

		protected override void WriteResult(ResultEntry result)
		{
			if (_modelData.InputData is IMultistepBusInputDataProvider multistageInput) {
				var primaryResult = multistageInput.JobInputData.PrimaryVehicle.ResultsInputData.Results.FirstOrDefault();
				var tankSystem = multistageInput.JobInputData.ConsolidateManufacturingStage.Vehicle.TankSystem;
				result.FuelData = primaryResult.EnergyConsumption.Keys
					.Select(x => DeclarationData.FuelData.Lookup(x, tankSystem)).Cast<IFuelProperties>().ToList();
			}
			ManufacturerRpt.WriteResult(result);
			CustomerRpt.WriteResult(result);
		}

	}
}
