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
*	Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TrailerSegments : LookupData<NumberOfTrailerAxles, TypeTrailer, BodyWorkCode, bool, TrailerSegment>
	{
		private DataTable _segmentTable;
		private static int _numberOfAxles;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".TrailerSegmentationTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. NumberOfAxles: {0}, TrailerType: {1}, BodyWorkCode: {2}, VolumeOrientation: {3}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override TrailerSegment Lookup(NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation)
		{
			_numberOfAxles = int.Parse(numberOfAxles.GetLabel());
			var row = GetTrailerDataRow(numberOfAxles, typeTrailer, bodyWorkCode, volumeOrientation);
			if (row == null) {
				return new TrailerSegment { VehicleGroup = -1 };
			}

			return new TrailerSegment {
				AccelerationFile =
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".VACC.Truck.vacc"), //row.Field<string>(".vaccfile")), TODO - CHECK THIS
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),
				VehicleGroupActive = row.ParseBoolean("Active"),
				GenericCode = row.Field<string>("Generic_code"),
				Class = VehicleClassHelper.Parse(row.Field<string>("Generic_vehicle_group")),
				CrossSectionAreaCorrection = row.ParseBoolean("CS_correction"),
				AcrMin = row.ParseDouble("AcrMin"),
				AcrBase = row.ParseDouble("AcrBase"), 
				CdxABase = row.ParseDouble("CdxA"),
				LengthCorrection = false, //row.ParseBoolean("LC_correction"),
				Aero0 = row.ParseDouble("aero_0"),
				Aero1 = row.ParseDouble("aero_1"),
				Aero2 = row.ParseDouble("aero_2"),
				Aero3 = row.ParseDouble("aero_3"),
				Aero4 = row.ParseDouble("aero_4"),
				Aero5 = row.ParseDouble("aero_5"),
				Aero6 = row.ParseDouble("aero_6"),
				Aero7 = row.ParseDouble("aero_7"),
				Aero8 = row.ParseDouble("aero_8"),
				MaxGcm = row.Field<string>("max_gcm"),
				Missions = CreateMissions(row),
				A1 = SquareMeter.Create(row.ParseDouble("a1")),
				A2 = SquareMeter.Create(row.ParseDouble("a2")),
				A3 = SquareMeter.Create(row.ParseDouble("a3")),
				RefLhA1 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_lh_a1")),
				RefLhA2 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_lh_a2")),
				RefLhA3 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_lh_a3")),
				RefOtherA1 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_other_a1")),
				RefOtherA2 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_other_a2")),
				RefOtherA3 = SquareMeter.Create(row.ParseDouble("ref_polar_curve_other_a3")),
				DesignSpeed = MeterPerSecond.Create(25) //90km/h
			};

		}

		private Mission[] CreateMissions(DataRow row)
		{
			var missionTypes = new List<MissionType>();

			if (!row["Mission_long_haul"].ToString().Contains("-"))
				missionTypes.Add(MissionType.LongHaul);
			if (!row["Mission_regional_delivery"].ToString().Contains("-"))
				missionTypes.Add(MissionType.RegionalDelivery);
			if (!row["Mission_urban_delivery"].ToString().Contains("-"))
				missionTypes.Add(MissionType.UrbanDelivery);

			var missions = new List<Mission>();
			foreach (var missionType in missionTypes) {

				//double maxPayload = 0;
				double lowLoad = 0;
				double refLoad = 0;

				switch (missionType) {
					case MissionType.LongHaul:
						lowLoad = row.ParseDouble("specific_lh_semi_payload_low");
						refLoad = row.ParseDouble("specific_lh_semi_payload_rep");
						break;
					default:
						lowLoad = row.ParseDouble("specific_other_semi_payload_low");
						refLoad = row.ParseDouble("specific_other_semi_payload_rep");
						break;
				}

				var data = new Dictionary<Tuple<MissionType, LoadingType>, double>();
				var tuple = new Tuple<MissionType, LoadingType>(MissionType.LongHaul, LoadingType.LowLoading);
				data.Add(tuple, row["Mission_long_haul"].ToString() == "0" ? 0 : row["Mission_long_haul"].ToString().Split('/')[0].ToDouble());
				tuple = new Tuple<MissionType, LoadingType>(MissionType.LongHaul, LoadingType.ReferenceLoad);
				data.Add(tuple, row["Mission_long_haul"].ToString() == "0" ? 0 : row["Mission_long_haul"].ToString().Split('/')[1].ToDouble());
				tuple = new Tuple<MissionType, LoadingType>(MissionType.RegionalDelivery, LoadingType.LowLoading);
				data.Add(tuple, row["Mission_regional_delivery"].ToString() == "0" ? 0 : row["Mission_regional_delivery"].ToString().Split('/')[0].ToDouble());
				tuple = new Tuple<MissionType, LoadingType>(MissionType.RegionalDelivery, LoadingType.ReferenceLoad);
				data.Add(tuple, row["Mission_regional_delivery"].ToString() == "0" ? 0 : row["Mission_regional_delivery"].ToString().Split('/')[1].ToDouble());

				if (!row["Mission_urban_delivery"].ToString().Contains("-")) {
					tuple = new Tuple<MissionType, LoadingType>(MissionType.UrbanDelivery, LoadingType.LowLoading);
					data.Add(tuple, row["Mission_urban_delivery"].ToString() == "0" ? 0 : row["Mission_urban_delivery"].ToString().Split('/')[0].ToDouble());
					tuple = new Tuple<MissionType, LoadingType>(MissionType.UrbanDelivery, LoadingType.ReferenceLoad);
					data.Add(tuple, row["Mission_urban_delivery"].ToString() == "0" ? 0 : row["Mission_urban_delivery"].ToString().Split('/')[1].ToDouble());
				}

				double refLowLoad = 0;
				double refRefLoad = 0;

				switch (missionType)
				{
					case MissionType.LongHaul:
						refLowLoad = row.ParseDouble("ref_lh_semi_payload_low");
						refRefLoad = row.ParseDouble("ref_lh_semi_payload_rep");
						break;
					default:
						refLowLoad = row.ParseDouble("ref_other_semi_payload_low");
						refRefLoad = row.ParseDouble("ref_other_semi_payload_rep");
						break;
				}

				var referenceTrailers = GetTrailers(row, missionType);
				var specificTrailers = GetTrailers(row, missionType, true);

				var mission = new Mission
				{
					TrailerWeightingFactors = data,
					MissionType = missionType,
					LowLoad = Kilogram.Create(refLowLoad),
					RefLoad = Kilogram.Create(refRefLoad),
					SpecificTrailerLowLoad = Kilogram.Create(lowLoad),
					SpecificTrailerRefLoad = Kilogram.Create(refLoad),
					AxleWeightDistribution = GetTowingAxleWeightDistribution(row, missionType),
					SpecificAxleWeightDistribution = GetTowingAxleWeightDistribution(row, missionType, true),
					CurbMass = Kilogram.Create(missionType == MissionType.LongHaul ? row.ParseDouble("ref_curb_lh") : row.ParseDouble("ref_curb_other")),
					DefaultCDxA = SquareMeter.Create(row.ParseDouble(missionType == MissionType.LongHaul ? "gen_default_CdxA_lh" : "gen_default_CdxA_om")),
					VehicleHeight = Meter.Create(row.ParseDouble("vehicle_height")),
					MinLoad = null,
					MaxLoad = null,
					Trailer = referenceTrailers,
					SpecificTrailer = specificTrailers,
					TotalCargoVolume = CubicMeter.Create(row.ParseDouble(missionType == MissionType.LongHaul ? "generic_cargo_lh" : "generic_cargo_om")),
					CrossWindCorrectionParameters = "TractorSemitrailer", //TODO: Check this
#if USE_EXTENAL_DECLARATION_DATA
					CycleFile = File.OpenRead(Path.Combine("DeclarationMissions", missionType.ToString().Replace("EMS", "") + ".vdri")),
#else
					CycleFile = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
															missionType.ToString().Replace("EMS", "") +
															Constants.FileExtensions.CycleFile),
#endif
				};
				missions.Add(mission);
			}


			return missions.ToArray();
		}


		private static double GetTrailerAxleWeightDistribution(DataRow row, MissionType missionType, bool isSpecific = false)
		{
			var axleDistributionColumn = missionType == MissionType.LongHaul ? "ref_semi_lh_axle" : "ref_semi_other_axle";

			if (isSpecific)
				axleDistributionColumn = missionType == MissionType.LongHaul ? "semi_lh_axle" : "semi_other_axle";

			var axle1 = row.Field<string>(axleDistributionColumn + "1");
			var axle2 = row.Field<string>(axleDistributionColumn + "2");
			var axle3 = row.Field<string>(axleDistributionColumn + "3");

			var axlesDistribution = _numberOfAxles == 1 ? axle1 : (_numberOfAxles == 2 ? axle2 : axle3);
			if (axlesDistribution == "---") return 0;

			double axleDistribution = axlesDistribution.Split('/')[0].ToDouble() / axlesDistribution.Split('/')[1].ToDouble();
			return axleDistribution / 100.0;
		}

		private IList<MissionTrailer> GetTrailers(DataRow row, MissionType missionType, bool isSpecific = false)
		{
			var trailers = new List<MissionTrailer>();

			var trailerColumn = "ref_code" + GetMissionPrefix(missionType);
			var trailerValue = row.Field<string>(trailerColumn);
			if (string.IsNullOrWhiteSpace(trailerValue))
			{
				throw new VectoException("Error in segmentation table: trailer weight share is defined but not trailer type!");
			}

			trailers.Add(CreateTrailer(trailerValue, GetTrailerAxleWeightDistribution(row, missionType, isSpecific), isSpecific, row, missionType));

			return trailers;
		}

		private MissionTrailer CreateTrailer(string trailerValue, double axleWeightShare, bool isSpecific, DataRow row, MissionType missionType)
		{

			Kilogram curbWeight = 0.SI<Kilogram>();
			Kilogram grossVehicleWeight = GetGenericVehicleData(row.Field<string>("Generic_code")).JobInputData.Vehicle.GrossVehicleMassRating;

			CubicMeter cargoVolume = 0.SI<CubicMeter>();
			SquareMeter deltaCDxA = 0.SI<SquareMeter>();

			var wheels = new List<Wheels.Entry>();

			if (!isSpecific) { //for specific these are taken from the input xml document
				curbWeight = Kilogram.Create(missionType == MissionType.LongHaul
					? row.ParseDouble("ref_curb_lh")
					: row.ParseDouble("ref_curb_other"));

				cargoVolume = CubicMeter.Create(row.ParseDouble(missionType == MissionType.LongHaul
					? "generic_cargo_lh"
					: "generic_cargo_om"));

				deltaCDxA = SquareMeter.Create(row.ParseDouble(missionType == MissionType.LongHaul
					? "gen_default_CdxA_lh"
					: "gen_default_CdxA_om"));

				wheels.Add(DeclarationData.Wheels.Lookup(row.Field<string>("ref_TyreXml").Replace("_", "").Replace("Standard", "")));
				if(row.Field<string>("ref_Axles") == "2")
					wheels.Add(DeclarationData.Wheels.Lookup(row.Field<string>("ref_TyreXml").Replace("_", "").Replace("Standard", "")));
				if (row.Field<string>("ref_Axles") == "3") {
					wheels.Add(DeclarationData.Wheels.Lookup(row.Field<string>("ref_TyreXml").Replace("_", "")
						.Replace("Standard", "")));
					wheels.Add(DeclarationData.Wheels.Lookup(row.Field<string>("ref_TyreXml").Replace("_", "")
						.Replace("Standard", "")));

				}
			}

			var trailerType = TrailerTypeHelper.Parse(trailerValue);

			//FOR MARKUS DEBUG
			IKernel _kernel = new StandardKernel(new VectoNinjectModule());
			var xmlInputReader = _kernel.Get<IXMLInputDataReader>();
            var component = xmlInputReader.CreateComponent(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".GenericVehicles." + row.Field<string>("ref_TyreXml").Replace("/","_") + ".xml"));

			
			TyreInputData tyreComponent = new TyreInputData();
			if (component is ITyreDeclarationInputData)
				tyreComponent = (TyreInputData)component;

			//FOR MARKUS DEBUG END

			return new MissionTrailer
			{
				TrailerType = trailerType,
				TrailerWheels = wheels,
				TrailerAxleWeightShare = axleWeightShare,
				TrailerCurbWeight = curbWeight,
				TrailerGrossVehicleWeight = grossVehicleWeight,
				DeltaCdA = deltaCDxA,
				CargoVolume = cargoVolume,
				TwinTyres = row.Field<string>("ref_TwinTyres") == "TRUE",
				TyreComponent = tyreComponent
			};

		}

		private static string GetMissionPrefix(MissionType missionType)
		{
			return missionType == MissionType.LongHaul ? "_lh" : "_other";
		}

		private static double[] GetTowingAxleWeightDistribution(DataRow row, MissionType missionType, bool isSpecific = false)
		{
			var axleDistributionColumn = missionType == MissionType.LongHaul ? "ref_Towing_lh_axle" : "ref_Towing_other_axle";

			if (isSpecific)
				axleDistributionColumn = missionType == MissionType.LongHaul ? "towing_lh_axle" : "towing_other_axle";

			var axle1 = row.Field<string>(axleDistributionColumn + "1");
			axle1 = axle1.Contains("---") || axle1.IsNullOrEmpty() ? "0" : axle1;

			var axle2 = row.Field<string>(axleDistributionColumn + "2");
			axle2 = axle2.Contains("---") || axle2.IsNullOrEmpty() ? "0" : axle2;

			var axle3 = row.Field<string>(axleDistributionColumn + "3");
			axle3 = axle3.Contains("---") || axle3.IsNullOrEmpty() ? "0" : axle3;

			return new [] { axle1.ToDouble() / 100.0, axle2.ToDouble() / 100.0, axle3.ToDouble()};

		}


		private DataRow GetTrailerDataRow(
			NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var numberOfAxlesRow = r.Field<string>("Number_of_axles");
						var typeTrailerRow = r.Field<string>("Trailer_type");
						var bodyWorkCodeRow = r.Field<string>("Bodywork_code");
						var volumeOrientationRow = r.Field<string>("Volume_orientation");
						return numberOfAxlesRow == numberOfAxles.GetLabel()
								&& typeTrailerRow == typeTrailer.ToString()
								&& bodyWorkCodeRow.Contains(bodyWorkCode.GetLabel())
								&& (volumeOrientationRow == "Yes" && volumeOrientation || volumeOrientationRow == "No" && !volumeOrientation);
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, numberOfAxles, typeTrailer, bodyWorkCode);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

		public IDeclarationInputDataProvider GetGenericVehicleData(string vehicleCode)
		{
			var genericXml = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".GenericVehicles." + vehicleCode + ".xml");
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(genericXml);

			IKernel kernel = new StandardKernel(new VectoNinjectModule());
			var inputReader = kernel.Get<IXMLInputDataReader>();

			XmlReader xmlReader = new XmlNodeReader(xmlDoc);
			return inputReader.CreateDeclaration(xmlReader);
		}

	}
}
