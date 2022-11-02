using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class CompletedBusSegments : LookupData<int , VehicleCode?, RegistrationClass?, int?, Meter, bool?, Segment>
	{
		private const string COMPLETED_BUS_SEGMENTS_CSV = ".CompletedBusSegmentationTable.csv";

		private DataTable _segmentTable;

		#region  Overrides of LookupData


		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + COMPLETED_BUS_SEGMENTS_CSV;

		protected override string ErrorMessage =>
			"ERROR: Could not find the declaration segment for vehicle. numberOfAxles: {0}, vehicleCode: {1}, registrationClass: {2}, " +
			"passengerSeatsLowerDeck: {3}, bodyHeight: {4} , lowEntry: {5}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(int numberOfAxles, VehicleCode? vehicleCode, RegistrationClass? registrationClass, int? passengerSeatsLowerDeck, Meter bodyHeight, bool? lowEntry)
		{
			return LookupCompletedBusVehicle(numberOfAxles, vehicleCode, registrationClass, passengerSeatsLowerDeck, bodyHeight, lowEntry);
		}
		
		#endregion


		private Segment LookupCompletedBusVehicle(int numberOfAxles, VehicleCode? vehicleCode, RegistrationClass? registrationClass, int? passengerSeatsLowerDeck, Meter bodyHeight, bool? lowEntry)
		{
			var rows = _segmentTable.AsEnumerable().Where(
				r => {
					var currentNumberOfAxles = r.Field<string>("numaxles").ToInt(0);
					var currentVehicleCode =  r.Field<string>("vehiclecode").ParseEnum<VehicleCode>();
					var registrationClasses = RegistrationClassHelper.Parse(r.Field<string>("registrationclasses"));

					return  currentNumberOfAxles == numberOfAxles 
							&& currentVehicleCode == vehicleCode && registrationClasses.Contains(registrationClass);
				}).ToList();
			if (rows.Count == 0) {
				return new Segment { Found = false };
			}

			if (rows.Count > 1) {
				if (rows.Any(r => r.Field<string>("passengerslowerdeck") != "-")) {
					rows = rows.Where(
						r => {
							var limits = r.Field<string>("passengerslowerdeck").Split('-');
							return ((int)passengerSeatsLowerDeck).IsBetween(limits[0].ToInt(), limits[1].ToInt());
						}).ToList();
				} else if (rows.Any(r => r.Field<string>("bodyheight") != "-")) {
					rows = rows.Where(
						r => {
							var limits = r.Field<string>("bodyheight").Split('-');
							return bodyHeight > limits[0].ToDouble().SI<Meter>() && bodyHeight <= limits[1].ToDouble().SI<Meter>();
						}).ToList();
				} else if (rows.All(r => r.Field<string>("lowentry") != "-")) {
					rows = rows.Where(
						r => {
							var isLowEntry = r.Field<string>("lowentry") == "1";
							return isLowEntry == lowEntry;
						}).ToList();
				} else {
					throw new VectoException("Multiple segments found! {0}", rows.Count);
				}
			}
			
			var row = rows.First();
			if(!row.ParseBoolean("isvalid"))
				throw new VectoException("Only invalid vehicles found!");
			
			var segment = new Segment {
				Found =  true,
				AccelerationFile =
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
						row.Field<string>(".vaccfile")),
				Missions = CreateMissions(rows), 
				VehicleClass = VehicleClassHelper.Parse( row.Field<string>("hdvgroup")),
				DesignSpeed = row.ParseDouble("designspeed").KMPHtoMeterPerSecond(),
			};

			return segment;
		}
		

		private Mission[] CreateMissions(List<DataRow> rows)
		{
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>().Where(
				m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
					rows.First().Table.Columns.Contains(m.ToString())).ToList();
			
			var missions = new List<Mission>();

			foreach (var row in rows) {

				foreach (var missionType in missionTypes) {
					if (string.IsNullOrWhiteSpace(row.Field<string>(missionType.ToString()))){
						continue;
					}

					// TODO: MQ 2021-11-30: REMOVE IN PRODUCTION
					Stream cycle;
					var cycleFile = Path.Combine("DeclarationMissions",
						missionType.ToString().Replace("EMS", "") + ".vdri");
					if (File.Exists(cycleFile)) {
						cycle = File.OpenRead(cycleFile);
					} else {
						cycle = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix +
															".MissionCycles." +
															missionType.ToString().Replace("EMS", "") +
															Constants.FileExtensions.CycleFile);
					}

					var mission = new Mission {
						MissionType = missionType,
						CrossWindCorrectionParameters = row.Field<string>("crosswindcorrection"),
						CycleFile = cycle,
						//CycleFile =
						//	RessourceHelper.ReadStream(
						//		DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
						//		missionType.ToString().Replace("EMS", "") +
						//		Constants.FileExtensions.CycleFile),
						AxleWeightDistribution = GetAxleWeightDistribution(row),
						BodyCurbWeight = 0.SI<Kilogram>(),
						Trailer = new List<MissionTrailer>(),
						MinLoad = null,
						MaxLoad = null,
						LowLoad = 10.SI<Kilogram>(), // dummy value to trigger simulation with low load
						RefLoad = 100.SI<Kilogram>(), // dummy value to trigger simulation with ref load
						PassengersLowLoad = 1,  // dummy value
						PassengersRefLoad = 10, // dummy value
						TotalCargoVolume = 0.SI<CubicMeter>(),
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>(),						
						BusParameter = new BusParameters {
							BusGroup = VehicleClassHelper.Parse(row.Field<string>("hdvgroup")),
							PassengerDensityLow = row.ParseDouble(missionType.ToString()).SI<PerSquareMeter>(),
							PassengerDensityRef = row.ParseDouble(missionType.ToString()).SI<PerSquareMeter>(),
							AirDragMeasurementAllowed = row.ParseBoolean(missionType == MissionType.Interurban ? "airdragmeasurementinterurban" : "airdragmeasurement"),
							ElectricalConsumers = GetVehicleEquipment(row),
							DoubleDecker = ((VehicleCode?) row.Field<string>("vehiclecode").ParseEnum<VehicleCode>()).IsDoubleDeckerBus(),
							DeltaHeight = row.ParseDouble("deltaheight").SI<Meter>(),
							SeparateAirDistributionDuctsHVACCfg = row.Field<string>("sepairdistrductshvaccfg").Split('/').Select(BusHVACSystemConfigurationHelper.Parse).ToArray() 
						}
					};

					missions.Add(mission);
				}
			}
			
			return missions.ToArray();
		}

		private double[] GetAxleWeightDistribution(DataRow row)
		{
			var axleDistribution = row.Field<string>("axleloaddistribution");
			if (string.IsNullOrWhiteSpace(axleDistribution)) {
				return new double[] { };
			}

			return axleDistribution.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}


		private Dictionary<string, double> GetVehicleEquipment(DataRow row)
		{
			var retVal = new Dictionary<string, double>();
			foreach (var electricalConsumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items) {
				if (electricalConsumer.Bonus || electricalConsumer.DefaultConsumer) {
					continue;
				}
				var caption = "es_" + electricalConsumer.ConsumerName.ToLowerInvariant().Replace(" ", "");
				retVal[electricalConsumer.ConsumerName] = row.ParseDoubleOrGetDefault(caption);
			}

			return retVal;
		}
	}
}
