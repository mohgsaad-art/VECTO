using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Omu.ValueInjecter.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.FileIO;

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

		private static IDictionary<Tuple<MissionType, LoadingType>, double> ZeroWeighting =>
			new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(
				new Dictionary<Tuple<MissionType, LoadingType>, double>() {
					{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 0 },
				});

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_weightingFactors = modelData.Mission.TrailerWeightingFactors;

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
				});

		protected internal override void DoWriteReport()
		{
			//TODO: identifies trailer missions with PassengerCount - need to change this probably
			foreach (var resultEntry in Results.OrderBy(x=> x.Mission).ThenBy(x=>x.LoadingType)) {
				var mappings = new Dictionary<ResultEntry, ResultEntry>();
				if (Results.Count(x => x.Mission == resultEntry.Mission && x.LoadingType == resultEntry.LoadingType && x.PassengerCount != null) > 0
				&& Results.Count(x => x.Mission == resultEntry.Mission && x.LoadingType == resultEntry.LoadingType && x.PassengerCount == null) > 0) {
					//Add map trailer simulations (key) with generic (value)
					mappings.Add(
						Results.FirstOrDefault(x => x.Mission == resultEntry.Mission && x.LoadingType == resultEntry.LoadingType && x.PassengerCount != null), 
						Results.FirstOrDefault(x => x.Mission == resultEntry.Mission && x.LoadingType == resultEntry.LoadingType && x.PassengerCount == null));
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

			var writer = Writer as FileOutputWriter;
			WriteStyleSheet(writer?.BasePath);
		}

		private static void WriteStyleSheet(string basePath)
		{
			//write stylesheet file
			if (basePath == null) {
				return;
			}
			var assembly = Assembly.GetExecutingAssembly();
			const string resourceName = DeclarationData.DeclarationDataResourcePrefix + ".Trailer.VectoReportsTrailer.css";
			var styleSheet = string.Empty;

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			using (var reader = new StreamReader(stream ?? throw new Exception("Cant find StyleSheet")))
			{
				styleSheet = reader.ReadToEnd();
			}

			File.WriteAllText(basePath+ "/VectoReportsTrailer.css", styleSheet);

		}
	}
}