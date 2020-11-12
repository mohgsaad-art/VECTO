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
			foreach (var resultEntry in Results) {
				Dictionary<ResultEntry, ResultEntry> mappings = new Dictionary<ResultEntry, ResultEntry>();

				if (Results.Count(x => x.Mission == resultEntry.Mission) > 1) {
					mappings.Add(Results.Where(x => x.Mission == resultEntry.Mission && resultEntry.CdxA != null).ToArray()[0], 
						Results.Where(x => x.Mission == resultEntry.Mission).ToArray()[1]);
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