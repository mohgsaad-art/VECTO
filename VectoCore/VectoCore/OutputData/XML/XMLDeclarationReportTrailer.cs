using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLDeclarationReportTrailer : XMLDeclarationReport
	{
		public XMLDeclarationReportTrailer(IReportWriter writer) : base(writer) { }

		#region Overrides of XMLDeclarationReport

		protected override void InstantiateReports(VectoRunData modelData)
		{
			ManufacturerRpt = new XMLManufacturerReportTrailer();
			CustomerRpt = new XMLCustomerReportTrailer();
		}

		private static IDictionary<Tuple<MissionType, LoadingType>, double> ZeroWeighting
		{
			get
			{
				return new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(
					new Dictionary<Tuple<MissionType, LoadingType>, double>() {
						{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.Construction, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.HeavyUrban, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.HeavyUrban, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.Urban, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.Urban, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.Suburban, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.Suburban, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.Interurban, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.Interurban, LoadingType.ReferenceLoad), 0 },
						{ Tuple.Create(MissionType.Coach, LoadingType.LowLoading), 0 },
						{ Tuple.Create(MissionType.Coach, LoadingType.ReferenceLoad), 0 },
					});
			}
		}
		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			var weightingGroup = modelData.Exempted
				? WeightingGroup.Unknown
				: DeclarationData.WeightingGroup.Lookup(
					modelData.VehicleData.VehicleClass, modelData.VehicleData.SleeperCab,
					modelData.EngineData.RatedPowerDeclared);
			_weightingFactors = weightingGroup == WeightingGroup.Unknown
				? ZeroWeighting
				: DeclarationData.WeightingFactors.Lookup(weightingGroup);

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
			//TODO: identifies trailer missions with PassengerCount - need to change this probably
			foreach (var resultEntry in Results) {
				Dictionary<ResultEntry, ResultEntry> mappings = new Dictionary<ResultEntry, ResultEntry>();
				if (Results.Count(x => x.Mission == resultEntry.Mission && x.Payload == resultEntry.Payload && x.PassengerCount != null) > 0
				&& Results.Count(x => x.Mission == resultEntry.Mission && x.Payload == resultEntry.Payload && x.PassengerCount == null) > 0) {
					//Add map trailer simulations (key) with generic (value)
					mappings.Add(
						Results.FirstOrDefault(x => x.Mission == resultEntry.Mission && x.Payload == resultEntry.Payload && x.PassengerCount != null), 
						Results.FirstOrDefault(x => x.Mission == resultEntry.Mission && x.Payload == resultEntry.Payload && x.PassengerCount == null));
				}

				((XMLManufacturerReportTrailer)ManufacturerRpt).Mappings = mappings;
				ManufacturerRpt.WriteResult(resultEntry);
				((XMLCustomerReportTrailer)CustomerRpt).Mappings = mappings;
				CustomerRpt.WriteResult(resultEntry);
			}

			GenerateReports();

			if (Writer != null) {
				OutputReports();
			}
		}
	}
}