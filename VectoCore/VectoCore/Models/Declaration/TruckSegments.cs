/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TruckSegments : LookupData<VehicleCategory, AxleConfiguration, Kilogram, Kilogram, bool, Segment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".SegmentTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the declaration segment for vehicle. Category: {0}, AxleConfiguration: {1}, GrossVehicleWeight: {2}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override Segment Lookup(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, Kilogram curbWeight, bool vocational)
		{

			return Lookup(vehicleCategory, axleConfiguration, grossVehicleMassRating, curbWeight, vocational, false);
		}

		public VehicleCategory[] GetVehicleCategories(bool declarationOnly = true)
		{
			return _segmentTable.AsEnumerable().Where(r => !declarationOnly || r.Field<string>("valid") == "1")
								.Select(r => EnumHelper.ParseEnum<VehicleCategory>(r.Field<string>("vehiclecategory"))).Distinct().ToArray();
		}

		public IEnumerable<AxleConfiguration> GetAxleConfigurations()
		{
			return _segmentTable.AsEnumerable().Where(row => row.Field<string>("valid") == "1")
								.Select(row => AxleConfigurationHelper.Parse(row.Field<string>("axleconf."))).Distinct();
		}

		public Segment Lookup(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, Kilogram curbWeight, bool vocational, bool considerInvalid)
		{
			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, vocational, considerInvalid);
			if (row == null) {
				return new Segment() { Found = false };
			}

			var vehicleHeight = LookupHeight(vehicleCategory, axleConfiguration, grossVehicleMassRating, vocational);
			var segment = new Segment {
				Found = true,
				GrossVehicleWeightMin = row.ParseDouble("tpmlm_min").SI(Unit.SI.Ton).Cast<Kilogram>(),
				GrossVehicleWeightMax = row.ParseDouble("tpmlm_max").SI(Unit.SI.Ton).Cast<Kilogram>(),
				VehicleCategory = vehicleCategory,
				AxleConfiguration = axleConfiguration,
				VehicleClass = VehicleClassHelper.Parse(row.Field<string>("hdvgroup")),
				AccelerationFile =
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
						row.Field<string>(".vaccfile")),
				Missions = CreateMissions(grossVehicleMassRating, curbWeight, row, vehicleHeight),
				DesignSpeed = row.ParseDouble("designspeed").KMPHtoMeterPerSecond(),

				//GrossVehicleMassRating = grossVehicleMassRating,
			};

			return segment;
		}

		private DataRow GetSegmentDataRow(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, bool vocational, bool considerInvalid)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var isValid = r.Field<string>("valid");
						var isVocational = r.Field<string>("vocational").ToBoolean();
						var category = r.Field<string>("vehiclecategory");
						var axleConf = r.Field<string>("axleconf.");
						var massMin = r.ParseDouble("tpmlm_min").SI(Unit.SI.Ton);
						var massMax = r.ParseDouble("tpmlm_max").SI(Unit.SI.Ton);
						return (considerInvalid || isValid == "1")
								&& vocational == isVocational
								&& category == vehicleCategory.ToString()
								&& axleConf == axleConfiguration.GetName()
								&& grossVehicleMassRating > massMin && grossVehicleMassRating <= massMax;
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, vehicleCategory, axleConfiguration.GetName(),
					grossVehicleMassRating);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

		public Meter LookupHeight(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, bool vocational)
		{
			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, vocational, true);

			var vehicleHeight = row.ParseDouble("height").SI<Meter>();
			var vehicleClass = VehicleClassHelper.Parse(row.Field<string>("hdvgroup"));

			if (vehicleClass == VehicleClass.Class9) {
				// VECTO-471: for class 9 take similar height than rigid with same maximum gross vehicle weight (class 1, 2, 3 or 4).
				var rigidGVWrow = _segmentTable.AsEnumerable().FirstOrDefault(
					r => {
						var massMin = r.ParseDouble("tpmlm_min").SI(Unit.SI.Ton);
						var massMax = r.ParseDouble("tpmlm_max").SI(Unit.SI.Ton);
						return new[] { "1", "2", "3", "4" }.Contains(r.Field<string>("hdvgroup"))
								&& massMin <= grossVehicleMassRating && grossVehicleMassRating <= massMax;
					});
				if (rigidGVWrow != null) {
					vehicleHeight = rigidGVWrow.ParseDouble("height").SI<Meter>();
				}
			}

			return vehicleHeight;
		}

		/// <summary>
		/// Looks up the default CdxA value for the cross wind correction.
		/// </summary>
		public SquareMeter LookupCdA(
			VehicleCategory vehicleCategory, AxleConfiguration axleConfiguration,
			Kilogram grossVehicleMassRating, bool vocational)
		{
			var row = GetSegmentDataRow(vehicleCategory, axleConfiguration, grossVehicleMassRating, vocational, true);
			return row.SI<SquareMeter>("cdxa_default");
		}

		private static Mission[] CreateMissions(
			Kilogram grossVehicleWeight, Kilogram curbWeight, DataRow row, Meter vehicleHeight)
		{
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>();
			var missions = new List<Mission>();
			foreach (var missionType in missionTypes.Where(
				m => m.IsDeclarationMission() && m != MissionType.ExemptedMission && row.Field<string>(m.ToString()) != "-")) {
				var body = GetBody(row, missionType);
				var trailers = GetTrailers(row, missionType);

				var maxGVW = missionType.IsEMS()? Constants.SimulationSettings.MaximumGrossVehicleMassEMS : Constants.SimulationSettings.MaximumGrossVehicleMass;
				
				// limit gvw to MaxGVW (40t)
				var gvw = VectoMath.Min(
					grossVehicleWeight + trailers.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0),
					maxGVW);
				var vehicleWeight = curbWeight + body.CurbWeight;
				var maxLoad = gvw - vehicleWeight -
							trailers.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);

				var payloads = row.Field<string>(missionType.ToString());

				var weight = grossVehicleWeight;
				GetLoadings(
					out var lowLoad, out var refLoad, payloads, (p, l) => GetLoading(p, weight, vehicleWeight, trailers, l), maxLoad);
				
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
					CrossWindCorrectionParameters = row.Field<string>("crosswindcorrection" + GetMissionSuffix(missionType, true)),
					CycleFile = cycle,
					//CycleFile = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
					//							missionType.ToString().Replace("EMS", "") +
					//							Constants.FileExtensions.CycleFile),
					AxleWeightDistribution = GetAxleWeightDistribution(row, missionType),
					BodyCurbWeight = body.CurbWeight,
					Trailer = trailers,
					MaxPayload = maxLoad,
					MinLoad = null,
					MaxLoad = null,
					RefLoad = refLoad,
					LowLoad = lowLoad,
					VehicleHeight = vehicleHeight,
					TotalCargoVolume = body.CargoVolume + trailers.Sum(t => t.CargoVolume).DefaultIfNull(0),
					DefaultCDxA = ReadDefaultAirDragValue(row, missionType)
				};
				missions.Add(mission);
			}

			return missions.ToArray();
		}

		private static void GetLoadings(
			out Kilogram lowLoad, out Kilogram refLoad, string payloadStr, Func<string, bool, Kilogram> loadingParser,
			Kilogram maxLoad)
		{
			var payloads = payloadStr.Split('/');
			if (payloads.Length == 2) {
				lowLoad = loadingParser(payloads[0], true);
				refLoad = loadingParser(payloads[1], false);
			} else {
				lowLoad = 0.SI<Kilogram>();
				refLoad = loadingParser(payloadStr, false);
			}

			refLoad = refLoad.LimitTo(0.SI<Kilogram>(), maxLoad);
			lowLoad = lowLoad.LimitTo(0.SI<Kilogram>(), maxLoad);
		}

		private static IList<MissionTrailer> GetTrailers(DataRow row, MissionType missionType)
		{
			var trailers = new List<MissionTrailer>();
			if (missionType.IsEMS()) {
				//maxGVW = Constants.SimulationSettings.MaximumGrossVehicleWeightEMS;
				var trailerList = row.Field<string>("ems").Split('+');
				var trailerWeightShares = row.Field<string>("traileraxles" + GetMissionSuffix(missionType)).Split('/');
				if (trailerList.Length != trailerWeightShares.Length) {
					throw new VectoException(
						"Error in segmentation table: number of trailers and list of weight shares does not match!");
				}

				trailers.AddRange(
					trailerWeightShares.Select((t, i) => CreateTrailer(trailerList[i], t.ToDouble() / 100.0, i == 0)));
			} else {
				if (ShouldTrailerBeUsed(row, missionType)) {
					var trailerColumn = missionType == MissionType.Construction ? "trailerconstruction" : "trailer";
					var trailerValue = row.Field<string>(trailerColumn);
					if (string.IsNullOrWhiteSpace(trailerValue)) {
						throw new VectoException("Error in segmentation table: trailer weight share is defined but not trailer type!");
					}

					trailers.Add(CreateTrailer(trailerValue, GetTrailerAxleWeightDistribution(row, missionType), true));
				}
			}

			return trailers;
		}

		private static StandardBody GetBody(DataRow row, MissionType missionType)
		{
			var bodyColumn = "body";
			switch (missionType) {
				case MissionType.Construction:
					bodyColumn = "bodyconstruction";
					break;
				case MissionType.MunicipalUtility:
					bodyColumn = "bodymunicipalutility";
					break;
			}

			var body = DeclarationData.StandardBodies.Lookup(row.Field<string>(bodyColumn));
			return body;
		}

		private static SquareMeter ReadDefaultAirDragValue(DataRow row, MissionType missionType)
		{
			var airDragColumn = "cdxa_default";
			if (missionType == MissionType.Construction) {
				airDragColumn = "cdxa_construction";
			}
			var cdxA = string.IsNullOrEmpty(row[airDragColumn].ToString())
				? null
				: row.ParseDouble(airDragColumn).SI<SquareMeter>();
			return cdxA;
		}

		private static Kilogram GetLoading(
			string payloadStr, Kilogram grossVehicleWeight, Kilogram vehicleWeight,
			IEnumerable<MissionTrailer> trailers, bool lowLoading)
		{
			var refLoadValue = payloadStr.ToDouble(double.NaN);
			if (!double.IsNaN(refLoadValue)) {
				return refLoadValue.SI<Kilogram>();
			}

			var vehiclePayload = DeclarationData.GetPayloadForGrossVehicleWeight(grossVehicleWeight, payloadStr)
													.LimitTo(0.SI<Kilogram>(), grossVehicleWeight - vehicleWeight);
			var trailerPayload = trailers.Sum(
												t => DeclarationData.GetPayloadForTrailerWeight(
													t.TrailerGrossVehicleWeight, t.TrailerCurbWeight, lowLoading))
											.DefaultIfNull(0);
				return vehiclePayload + trailerPayload;
		}


		/// <summary>
		/// Checks if a trailer should be used for the current missionType.
		/// </summary>
		private static bool ShouldTrailerBeUsed(DataRow row, MissionType missionType)
		{
			return !string.IsNullOrWhiteSpace(row.Field<string>("traileraxles" + GetMissionSuffix(missionType)));
		}

		private static double GetTrailerAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			var trailerAxles =
				row.Field<string>("traileraxles" + GetMissionSuffix(missionType));
			if (!string.IsNullOrWhiteSpace(trailerAxles)) {
				return trailerAxles.ToDouble() / 100.0;
			}

			return 0;
		}

		private static double[] GetAxleWeightDistribution(DataRow row, MissionType missionType)
		{
			var axleDistribution = row.Field<string>("truckaxles" + GetMissionSuffix(missionType));
			if (string.IsNullOrWhiteSpace(axleDistribution)) {
				return new double[] { };
			}

			return axleDistribution.Split('/').ToDouble().Select(x => x / 100.0).ToArray();
		}

		private static string GetMissionSuffix(MissionType missionType, bool ignoreEMS = false)
		{
			return "-" +
					(missionType.IsEMS() && ignoreEMS
						? ""
						: (missionType.GetNonEMSMissionType() == MissionType.LongHaul ? "longhaul" : "other")) +
					(missionType.IsEMS() ? "ems" : "");
		}

		private static MissionTrailer CreateTrailer(string trailerValue, double axleWeightShare, bool firstTrailer)
		{
			var trailerType = TrailerTypeHelper.Parse(trailerValue);
			var trailer = DeclarationData.StandardBodies.Lookup(trailerType.ToString());
			return new MissionTrailer {
				TrailerType = trailerType,
				TrailerWheels = trailer.Wheels,
				TrailerAxleWeightShare = axleWeightShare,
				TrailerCurbWeight = trailer.CurbWeight,
				TrailerGrossVehicleWeight = trailer.GrossVehicleWeight,
				DeltaCdA = trailer.DeltaCrossWindArea[firstTrailer ? 0 : 1],
				CargoVolume = trailer.CargoVolume
			};
		}
	}
}
