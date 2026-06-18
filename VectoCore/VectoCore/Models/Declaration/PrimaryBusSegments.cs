using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class PrimaryBusSegments : LookupData<VehicleCategory, AxleConfiguration, bool, Segment>
	{
		private DataTable _segmentTable;


		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".PrimaryBusSegmentationTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the declaration segment for vehicle. Category: {0}, AxleConfiguration: {1}, GrossVehicleWeight: {2}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration, bool articulated)
		{
			return LookupPrimaryVehicle(vehicleCategory, axleConfiguration, articulated);
		}

		#endregion

		/// <summary>
		/// Look up the hdv group based on the supergroup
		/// </summary>
		public VehicleClass Lookup(VehicleClass hdvSuperGroup, VehicleCode vehicleCode)
		{
			var doubleDecker = vehicleCode.IsDoubleDeckerBus();
			var floorTyoe = vehicleCode.GetFloorType();

			var row = _segmentTable.AsEnumerable().Where(r => {
				bool doubleDeckerLookedup = r.Field<string>("doubledecker") == "1" ? true : false;
				string floor = r.Field<string>("floortype");
				var floorMatches = false;
                switch (floor) {
					case "high floor":
						floorMatches = floorTyoe == FloorType.HighFloor; break;
					case "low floor":
						floorMatches = floorTyoe == FloorType.LowFloor; break;
					default:
						throw new VectoException($"Unexpected value in column floor type {floor}", ErrorCodes.DECLARATIONDATA_INVALID_FLOOR_TYPE);
				}

				VehicleClass hdvSuperGroupLookedUp = VehicleClassHelper.Parse(r.Field<string>("hdvsupergroup"));
				return floorMatches && doubleDecker == doubleDeckerLookedup &&
						hdvSuperGroupLookedUp == hdvSuperGroup;
			}).Single();
			return VehicleClassHelper.Parse(row.Field<string>("hdvgroup"));
        }


		private Segment LookupPrimaryVehicle(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration, bool articulated)
		{
			var rows = _segmentTable.AsEnumerable().Where(
				r => {
					var productionStage = r.Field<string>("productionstage").ToInt(0);
					var articulatedStr = r.Field<string>("articulated");
					var articulatedB = articulatedStr == "-" ? (bool?)null : int.Parse(articulatedStr) != 0;
					var numAxles = r.Field<string>("numaxles").ToInt(0);
					return productionStage == 1 &&
							(!articulatedB.HasValue || articulatedB == articulated) &&
							axleConfiguration.NumAxles() == numAxles;
				}).ToList();
			if (rows.Count == 0) {
				return new Segment() { Found = false };
			}

			var firstRow = rows.First();
			var segment = new Segment {
				Found = true,
				GrossVehicleWeightMin = 0.SI<Kilogram>(),
				GrossVehicleWeightMax = firstRow.ParseDouble("tpmlm_max").SI(Unit.SI.Ton).Cast<Kilogram>(),
				VehicleCategory = vehicleCategory,
				AxleConfiguration = axleConfiguration,
				VehicleClass = VehicleClassHelper.Parse(firstRow.Field<string>("hdvsupergroup")),
				AccelerationFile =
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
						firstRow.Field<string>(".vaccfile")),
				Missions = CreateMissions(rows),
				DesignSpeed = firstRow.ParseDouble("designspeed").KMPHtoMeterPerSecond(),
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
					if (string.IsNullOrWhiteSpace(row.Field<string>(missionType.ToString()))) {
						continue;
					}

					var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(row.ParseDouble("length").SI<Meter>(),
								row.ParseDouble("width").SI<Meter>());
					var passDensities = row.Field<string>(missionType.ToString()).Split('/');
					
                    var passengerDensityRef = passDensities.Last().ToDouble().SI<PerSquareMeter>();
					var passengerDensityLow = passDensities.First().ToDouble().SI<PerSquareMeter>();
					var passengerCountLow = busFloorArea * passengerDensityLow; // weight of driver is included in curb mass
					var passengerCountRef = busFloorArea * passengerDensityRef; // weight of driver is included in curb mass
																				//var refLoad = passengerCountRef * missionType.GetAveragePassengerMass();
																				
					var iceDisplacement = row.ParseDouble("icedisplacement")
																						.SI(Unit.SI.Liter)
																						.Cast<CubicMeter>();
					var fuelCapacity = row.ParseDouble("fuelcapacity").SI<Liter>();

					var genericMassICEAndFuelTank = iceDisplacement * DeclarationData.ICE_MassPerDisplacement +
													fuelCapacity / 2.0 * FuelData.Diesel.FuelDensity;

					var mission = new Mission {
						MissionType = missionType,
						CrossWindCorrectionParameters = row.Field<string>("crosswindcorrection"),
						AxleWeightDistribution = GetAxleWeightDistribution(row),
						CurbMass = row.ParseDouble("curbmass").SI<Kilogram>(),
						BodyCurbWeight = 0.SI<Kilogram>(),
						Trailer = new List<MissionTrailer>(),
						MinLoad = null,
						MaxLoad = null,
						LowLoad = passengerCountLow * missionType.GetAveragePassengerMass() * missionType.GetLowLoadFactorBus(),
						RefLoad = passengerCountRef * missionType.GetAveragePassengerMass(),
						VehicleHeight = row.ParseDouble("bodyheight").SI<Meter>(),
						PassengersRefLoad = passengerCountRef,
						PassengersLowLoad = passengerCountLow * missionType.GetLowLoadFactorBus(),
						TotalCargoVolume = 0.SI<CubicMeter>(),
						GenericMassICE = VectoMath.Round(genericMassICEAndFuelTank, MidpointRounding.AwayFromZero),
						DefaultCDxA = row.ParseDouble("cdxastandard").SI<SquareMeter>(),
						BusParameter = new BusParameters() {
							BusGroup = VehicleClassHelper.Parse(row.Field<string>("hdvgroup")),
							VehicleLength = row.ParseDouble("length").SI<Meter>(),
							VehicleWidth = row.ParseDouble("width").SI<Meter>(),
							CurbMassTPMLMFactor = row.Field<string>("curbmasstpmlmfactor").ToDouble(double.NaN),
							BodyHeight = row.ParseDouble("bodyheight").SI<Meter>(),
							DeltaHeight = 0.3.SI<Meter>(), //row.ParseDouble("height").SI<Meter>(),
							NumberPassengersLowerDeck = row.ParseDouble("passengerslowerdeck"),
							NumberPassengersUpperDeck = row.ParseDouble("passengersupperdeck"),
							PassengerDensityLow = passengerDensityLow,
							PassengerDensityRef = passengerDensityRef,
							DoubleDecker = row.ParseBoolean("doubledecker"),
							LowEntry = GetLowEntry(row.Field<string>("lowentry")),
							FloorType = row.Field<string>("floortype").ParseEnum<FloorType>(),
							EntranceHeight =  row.ParseDouble("entranceheight").SI(Unit.SI.Milli.Meter).Cast<Meter>(),
							VehicleCode = row.Field<string>("vehiclecode").ParseEnum<VehicleCode>(),
							HVACConventional = new HVACParameters() {
								HVACConfiguration = BusHVACSystemConfigurationHelper.Parse(row.Field<string>("hvacsystemconfiguration")),
								HVACAuxHeaterPower = row.ParseDouble("hvacauxheaterconventional").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
								HeatPumpTypeDriverCompartmentCooling = HeatPumpType.none,
								HeatPumpTypeDriverCompartmentHeating = HeatPumpType.none,
								HeatPumpTypePassengerCompartmentCooling = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpcoolingpassengerconventional")),
								HeatPumpTypePassengerCompartmentHeating = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpcoolingpassengerconventional")),
								HVACDoubleGlasing = row.ParseBoolean("hvacdoubleglasing"),
								WaterElectricHeater = row.ParseBoolean("waterelectricheaterconventional"),
								HVACAdjustableAuxHeater = row.ParseBoolean("hvacadjustableauxiliaryheater"),
								HVACSeparateAirDistributionDucts = row.ParseBoolean("hvacseparateairdistributionducts"),
							},
							HVACHEV = new HVACParameters() {
								HVACConfiguration = BusHVACSystemConfigurationHelper.Parse(row.Field<string>("hvacsystemconfiguration")),
								HVACAuxHeaterPower = row.ParseDouble("hvacauxheaterhev").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
								HeatPumpTypeDriverCompartmentCooling = HeatPumpType.none,
								HeatPumpTypeDriverCompartmentHeating = HeatPumpType.none,
								HeatPumpTypePassengerCompartmentCooling = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpcoolingpassengerhev")),
								HeatPumpTypePassengerCompartmentHeating = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpheatingpassengerhev")),
								HVACDoubleGlasing = row.ParseBoolean("hvacdoubleglasing"),
								WaterElectricHeater = row.ParseBoolean("waterelectricheaterhev"),
								HVACAdjustableAuxHeater = row.ParseBoolean("hvacadjustableauxiliaryheater"),
								HVACSeparateAirDistributionDucts = row.ParseBoolean("hvacseparateairdistributionducts"),
							},
							HVACPEV = new HVACParameters() {
								HVACConfiguration = BusHVACSystemConfigurationHelper.Parse(row.Field<string>("hvacsystemconfiguration")),
								HVACAuxHeaterPower = row.ParseDouble("hvacauxheaterpev").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
								HeatPumpTypeDriverCompartmentCooling = HeatPumpType.none,
								HeatPumpTypeDriverCompartmentHeating = HeatPumpType.none,
								HeatPumpTypePassengerCompartmentCooling = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpcoolingpassengerpev")),
								HeatPumpTypePassengerCompartmentHeating = HeatPumpTypeHelper.Parse(row.Field<string>("heatpumpheatingpassengerpev")),
								HVACDoubleGlasing = row.ParseBoolean("hvacdoubleglasing"),
								WaterElectricHeater = row.ParseBoolean("waterelectricheaterpev"),
								HVACAdjustableAuxHeater = row.ParseBoolean("hvacadjustableauxiliaryheater"),
								HVACSeparateAirDistributionDucts = row.ParseBoolean("hvacseparateairdistributionducts"),
							},
							ElectricalConsumers = GetVehicleEquipment(row)
						}
					};
					missions.Add(mission);
				}
			}

			return missions.ToArray();
		}

		private bool? GetLowEntry(string field)
		{
			switch (field) {
				case "-": return null;
				case "0": return false;
				case "1": return true;
				//case "semilowfloor":
				//case "semilow": return FloorType.SemiLowFloor;
				default: return null;
			}
		}

		private static double[] GetAxleWeightDistribution(DataRow row)
		{
			var axleDistribution = row.Field<string>("AxlesWeights");
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
