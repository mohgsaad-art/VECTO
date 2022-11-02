using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration {
	public sealed class SteeringPumpBus : IDeclarationAuxiliaryTable
	{
		private readonly SteeringPumpBaseLine _baseLine = new SteeringPumpBaseLine();

		private readonly SteeringPumpFactors _technologyFactors = new SteeringPumpFactors();

		public Watt LookupMechanicalPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength)
		{
			return LookupPowerDemand(mission, technologies, vehicleLength, false);
		}

		public Watt LookupElectricalPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength)
		{
			return LookupPowerDemand(mission, technologies, vehicleLength, true);
		}

		public Watt LookupPowerDemand(MissionType mission, IList<string> technologies, Meter vehicleLength, bool electrical)
		{
			var powerDemand = 0.SI<Watt>();

			for (var i = 0; i < technologies.Count; i++) {
				var techLookup = _technologyFactors.Lookup(technologies[i], mission);
				if (techLookup.FullyElectric != electrical) {
					continue;
				}
				var baseDemand = _baseLine.Lookup(mission, i + 1).Value;
				var powerDemandTubing = (Constants.BusParameters.Auxiliaries.SteeringPump.TubingLoss * 2 *
										(vehicleLength - Constants.BusParameters.Auxiliaries.SteeringPump.LengthBonus) *
										Constants.BusParameters.Auxiliaries.SteeringPump.VolumeFlow).Cast<Watt>();
				var tubingFactor = i == 0 ? techLookup.TubingFactor : 0;
				var axleFactor = techLookup.AxleFactor;

				powerDemand += baseDemand * axleFactor + powerDemandTubing * tubingFactor;
			}

			return powerDemand;
		}

		#region Implementation of IDeclarationAuxiliaryTable

		public string[] GetTechnologies()
		{
			return _technologyFactors.GetTechnologies();
		}

		#endregion

		private sealed class SteeringPumpBaseLine : LookupData<MissionType, int, LookupValues<Watt>>
		{
			#region Overrides of LookupData

			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.SP-Axles_Bus.csv";
			protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'";

			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var axleNumber = int.Parse(row.Field<string>("axlenbr"));
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "axlenbr" || string.IsNullOrWhiteSpace(row.Field<string>(col.Caption))) {
							continue;
						}

						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), axleNumber)] =
							new LookupValues<Watt> { Value = row.ParseDouble(col.Caption).SI<Watt>() };
					}
				}
			}

			#endregion
		}

		private sealed class SteeringPumpFactors : LookupData<string, MissionType, SteeringPumpTechnologyEntry>, IDeclarationAuxiliaryTable
		{
			#region Overrides of LookupData

			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.SP-Factors_Bus.csv";
			protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'";

			protected override void ParseData(DataTable table)
			{
				var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>().Where(
					m => ((m.IsDeclarationMission() && m != MissionType.ExemptedMission) || m == MissionType.VerificationTest) &&
						table.Columns.Contains("tubing-" + m)).ToList();

				foreach (DataRow row in table.Rows) {
					var axleNumber = row.Field<string>("technology");

					foreach(var mission in missionTypes) {
						Data[Tuple.Create(axleNumber, mission)] =
							 new SteeringPumpTechnologyEntry() {
									FullyElectric = !row.Field<string>("fullyelectric").Equals("0"),
									TubingFactor = row.ParseDouble("tubing-"+mission.ToString().ToLower()),
									AxleFactor = row.ParseDouble("axle-"+mission.ToString().ToLower())
							};
					}
				}
			}

			#endregion

			#region Implementation of IDeclarationAuxiliaryTable

			public string[] GetTechnologies()
			{
				return Data.Keys.Select(x => x.Item1).Distinct().ToArray();
			}

			#endregion
		}

		public struct SteeringPumpTechnologyEntry
		{
			public double TubingFactor { get; set; }
			public double AxleFactor { get; set; }

			public bool FullyElectric { get; set; }
		}
		

		private struct LookupValues<T>
		{
			public T Value;
		}
	}
}