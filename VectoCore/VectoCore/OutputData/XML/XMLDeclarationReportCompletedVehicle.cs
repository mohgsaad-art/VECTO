using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportCompletedVehicle : XMLDeclarationReport
	{
		public XMLDeclarationReportCompletedVehicle(IReportWriter writer, bool writePIF = false) : base(writer) { }

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleReportInputData { get; set; }

		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			ManufacturerRpt = modelData.Exempted
				? new XMLManufacturerReportExemptedCompletedBus() {
					PrimaryVehicleRecordFile = PrimaryVehicleReportInputData
				}
				: new XMLManufacturerReportCompletedBus() {
					PrimaryVehicleRecordFile = PrimaryVehicleReportInputData
				};
			CustomerRpt = modelData.Exempted
				? new XMLCustomerReportExemptedCompletedBus() {
					PrimaryVehicleRecordFile = PrimaryVehicleReportInputData
				}
				: new XMLCustomerReportCompletedBus() {
					PrimaryVehicleRecordFile = PrimaryVehicleReportInputData
				};
		}

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_weightingFactors = EqualWeighting;

			InstantiateReports(modelData);

			ManufacturerRpt.Initialize(modelData, fuelModes);
			CustomerRpt.Initialize(modelData, fuelModes);
		}
		#endregion

		private static IDictionary<Tuple<MissionType, LoadingType>, double> EqualWeighting =>
			new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(
				new Dictionary<Tuple<MissionType, LoadingType>, double>() {
					{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Construction, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Urban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Urban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.ReferenceLoad), 1 },
					{ Tuple.Create(MissionType.Coach, LoadingType.LowLoading), 1 },
					{ Tuple.Create(MissionType.Coach, LoadingType.ReferenceLoad), 1 },
				});

		protected internal override void DoWriteReport()
		{
			foreach (var specificResult in Results.Where(x => VehicleClassHelper.IsCompletedBus(x.VehicleClass)).OrderBy(x => x.VehicleClass)
												.ThenBy(x => x.FuelMode).ThenBy(x => x.Mission)) {

				var genericResult = Results.First(x => x.VehicleClass.IsPrimaryBus() && x.FuelMode == specificResult.FuelMode &&
						x.Mission == specificResult.Mission && x.LoadingType == specificResult.LoadingType);
				var primaryResult = genericResult.PrimaryResult ?? specificResult.PrimaryResult;
				if (primaryResult == null) {
					throw new VectoException(
						"no primary result entry set for simulation run vehicle class: {0}, mission: {1}, payload: {2}",
						genericResult.VehicleClass, genericResult.Mission, genericResult.Payload);
				}

				(ManufacturerRpt as XMLManufacturerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
				(CustomerRpt as XMLCustomerReportCompletedBus).WriteResult(genericResult, specificResult, primaryResult);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}
	}
}