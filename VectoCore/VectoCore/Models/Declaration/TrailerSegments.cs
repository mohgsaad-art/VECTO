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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TrailerSegments : LookupData<NumberOfTrailerAxles, TypeTrailer, BodyWorkCode, bool, Kilogram, TrailerSegment>
	{
		private DataTable _segmentTable;
		private static int _numberOfAxles;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.trailerSegmentationTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. NumberOfAxles: {0}, TrailerType: {1}, BodyWorkCode: {2}, VolumeOrientation: {3}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public List<int> GetActiveVehicleGroups()
		{
			EnumerableRowCollection<DataRow> rows;
			try
			{
				rows = _segmentTable.AsEnumerable().Where(
					r => {
						var active = (int)r.ParseDouble("Active_Final");
						return active == 1;
					});
			}
			catch (InvalidOperationException e)
			{
				throw new VectoException("Can't get active vehicle groups", e);
			}

			return Enumerable.Select(rows, row => (int)row.ParseDouble("Vehicle_group")).ToList();
		}

		public DataRow GetRow(string vehicleGroup)
		{
			return GetTrailerDataRow(vehicleGroup);
		}
		public override TrailerSegment Lookup(NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation, Kilogram tpmlmAxleAssembly)
		{
			var group = 0;
			DataRow row;
			var rf = new TrailerReferenceFactors();
			_numberOfAxles = int.Parse(numberOfAxles.GetLabel());

			//var rfRun = false;
			row = GetTrailerDataRow(numberOfAxles, typeTrailer, bodyWorkCode, volumeOrientation, tpmlmAxleAssembly);
			@group = (int)row.ParseDouble("Vehicle_group");
			var vehicleGroupAnnex = row.Field<string>("Vehicle_group_acc");
			if (row == null) {
				return new TrailerSegment { VehicleGroup = -1 };
			}

			return new TrailerSegment {
				AccelerationFile =
					RessourceHelper.ReadStream(
						DeclarationData.DeclarationDataResourcePrefix + ".VACC.Truck.vacc"), //row.Field<string>(".vaccfile")), TODO - CHECK THIS
				VehicleGroup = group,
				VehicleGroupAnnex = vehicleGroupAnnex,
				VehicleGroupActive = row.ParseBoolean("Active"),
				GenericCode = row.Field<string>("Generic_code"),
				MassBody = Kilogram.Create(!row.Field<string>("generic_mass_body").Contains("-") ? row.ParseDouble("generic_mass_body") : 0),
				Class = VehicleClassHelper.Parse(row.Field<string>("Generic_vehicle_group").Replace("-RD", "").Replace("-LH", "")),
				TowingVehicle = row.Field<string>("Generic_vehicle_group"),
				MaxGcm = row.Field<string>("max_gcm"),
				Missions = CreateMissions(row),
				ReferenceTrailerSegment = DeclarationData.ReferenceTrailer.Lookup(group),
				SpecificTrailerSegment = DeclarationData.SpecificTrailer.Lookup(group),
				GenericCadVehicle = DeclarationData.GenericCadVehicle.Lookup(group),
				DesignSpeed = MeterPerSecond.Create(25), //90km/h
				CodeStandard = row.Field<string>("ref_code_lh"),
				ReferenceRatios = rf.Lookup(vehicleGroupAnnex),
			};
		}

		private Mission[] CreateMissions(DataRow row)
		{
			var missionTypes = new List<MissionType>();

			var group = (int)row.ParseDouble("Vehicle_group");


			var referenceTrailerSegment = DeclarationData.ReferenceTrailer.Lookup(group);


			if (!row["Mission_long_haul"].ToString().Contains("-"))
				missionTypes.Add(MissionType.LongHaul);
			if (!row["Mission_regional_delivery"].ToString().Contains("-"))
				missionTypes.Add(MissionType.RegionalDelivery);
			if (!row["Mission_urban_delivery"].ToString().Contains("-"))
				missionTypes.Add(MissionType.UrbanDelivery);

			var missions = new List<Mission>();
			foreach (var missionType in missionTypes) {

				Kilogram grossVehicleWeight = GetGenericVehicleData(row.Field<string>("Generic_code")).JobInputData.Vehicle.GrossVehicleMassRating;
				var stdTrailer = DeclarationData.StandardTrailer.Lookup(row.Field<string>("ref_code_lh"), grossVehicleWeight.Value());

				//double maxPayload = 0;
				double lowLoad = 0;
				double refLoad = 0;
				double lowLoadRef = 0;
				double refLoadRef = 0;
				switch (missionType) {
					case MissionType.LongHaul:
						lowLoad = row.ParseDouble("specific_lh_semi_payload_low");
						refLoad = row.ParseDouble("specific_lh_semi_payload_rep");
						lowLoadRef = row.ParseDouble("specific_lh_semi_payload_low");
						refLoadRef = row.ParseDouble("specific_lh_semi_payload_rep");
						break;
					default:
						lowLoad = row.ParseDouble("specific_other_semi_payload_low");
						refLoad = row.ParseDouble("specific_other_semi_payload_rep");
						lowLoadRef = row.ParseDouble("specific_other_semi_payload_low");
						refLoadRef = row.ParseDouble("specific_other_semi_payload_rep");
						break;
				}

				var data = new Dictionary<Tuple<MissionType, LoadingType>, double>();
				var tuple = new Tuple<MissionType, LoadingType>(MissionType.LongHaul, LoadingType.LowLoading);
				if (!row["Mission_long_haul"].ToString().Contains("-")) {
					data.Add(tuple,
						row["Mission_long_haul"].ToString() == "0/0"
							? 0
							: row["Mission_long_haul"].ToString().Split('/')[0].ToDouble());
					tuple = new Tuple<MissionType, LoadingType>(MissionType.LongHaul, LoadingType.ReferenceLoad);
					data.Add(tuple,
						row["Mission_long_haul"].ToString() == "0/0"
							? 0
							: row["Mission_long_haul"].ToString().Split('/')[1].ToDouble());
				}
				if (!row["Mission_regional_delivery"].ToString().Contains("-"))
				{
					tuple = new Tuple<MissionType, LoadingType>(MissionType.RegionalDelivery, LoadingType.LowLoading);
					data.Add(tuple,
						row["Mission_regional_delivery"].ToString() == "0/0"
							? 0
							: row["Mission_regional_delivery"].ToString().Split('/')[0].ToDouble());
					tuple = new Tuple<MissionType, LoadingType>(MissionType.RegionalDelivery,
						LoadingType.ReferenceLoad);
					data.Add(tuple,
						row["Mission_regional_delivery"].ToString() == "0/0"
							? 0
							: row["Mission_regional_delivery"].ToString().Split('/')[1].ToDouble());
				}


				if (!row["Mission_urban_delivery"].ToString().Contains("-")) {
					tuple = new Tuple<MissionType, LoadingType>(MissionType.UrbanDelivery, LoadingType.LowLoading);
					data.Add(tuple, row["Mission_urban_delivery"].ToString() == "0/0" ? 0 : row["Mission_urban_delivery"].ToString().Split('/')[0].ToDouble());
					tuple = new Tuple<MissionType, LoadingType>(MissionType.UrbanDelivery, LoadingType.ReferenceLoad);
					data.Add(tuple, row["Mission_urban_delivery"].ToString() == "0/0" ? 0 : row["Mission_urban_delivery"].ToString().Split('/')[1].ToDouble());
				}

				var referenceTrailers = GetTrailers(row, missionType);
				var specificTrailers = GetTrailers(row, missionType, true);

				var totalCargoVolume = CubicMeter.Create(row["generic_cargo_volume"].ToString() != "---"
					? row.ParseDouble("generic_cargo_volume")
					: 0);

				var curbMass = referenceTrailerSegment.CurbMass;


				var mission = new Mission
				{
					TrailerWeightingFactors = data,
					MissionType = missionType,
					LowLoad = Kilogram.Create(lowLoadRef),
					RefLoad = Kilogram.Create(refLoadRef),
					SpecificTrailerLowLoad = Kilogram.Create(lowLoad),
					SpecificTrailerRefLoad = Kilogram.Create(refLoad),
					AxleWeightDistribution = GetTowingAxleWeightDistribution(row, missionType),
					SpecificTrailerAxleWeightDistribution = GetTowingAxleWeightDistribution(row, missionType, true),
					CurbMass = curbMass,
					VehicleHeight = referenceTrailerSegment.TotalTrailerHeight,
					MinLoad = null,
					MaxLoad = null,
					Trailer = referenceTrailers,
					SpecificTrailer = specificTrailers,
					TotalCargoVolume = totalCargoVolume,
					CrossWindCorrectionParameters = "TractorSemitrailer",
					TrailerLiftableImpact = DeclarationData.SpecificTrailer.GetImpact(group, "Liftable", missionType),
					TrailerSteeredImpact = DeclarationData.SpecificTrailer.GetImpact(group, "Steered", missionType),
					StandardTrailerSegment = stdTrailer,
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
			string axleDistributionColumn;

			axleDistributionColumn = missionType == MissionType.LongHaul ? "semi_lh_axle" : "semi_other_axle";

			var axle1 = row.Field<string>(axleDistributionColumn + "1");
			var axle2 = row.Field<string>(axleDistributionColumn + "2");
			var axle3 = row.Field<string>(axleDistributionColumn + "3");

			var axlesDistribution = _numberOfAxles == 1 ? axle1 : (_numberOfAxles == 2 ? axle2 : axle3);
			if (axlesDistribution == "---") return 0;

			double axleDistribution = axlesDistribution.Contains('/') ? axlesDistribution.Split('/')[0].ToDouble() / axlesDistribution.Split('/')[1].ToDouble() : axlesDistribution.ToDouble();
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

			trailers.Add(CreateTrailer(trailerValue, GetTrailerAxleWeightDistribution(row, missionType, isSpecific), isSpecific, row));

			return trailers;
		}

		private MissionTrailer CreateTrailer(string trailerValue, double axleWeightShare, bool isSpecific, DataRow row)
		{
			var rfRun = false;
			var group = (int)row.ParseDouble("Vehicle_group");

			Kilogram curbWeight = 0.SI<Kilogram>();
			Kilogram grossVehicleWeight = GetGenericVehicleData(row.Field<string>("Generic_code")).JobInputData.Vehicle.GrossVehicleMassRating;

			CubicMeter cargoVolume = 0.SI<CubicMeter>();
			SquareMeter deltaCDxA = 0.SI<SquareMeter>();

			var wheels = new List<Wheels.Entry>();

			var referenceTrailerSegment = DeclarationData.ReferenceTrailer.Lookup(group);

			var twinTyres = referenceTrailerSegment.TwinTyres;

			if (!isSpecific) { //for specific these are taken from the input xml document
				curbWeight = referenceTrailerSegment.CurbMass + Kilogram.Create(!row.Field<string>("generic_mass_body").Contains("-") ? row.ParseDouble("generic_mass_body") : 0);

				cargoVolume = referenceTrailerSegment.CargoVolume;
				//deltaCDxA = referenceTrailerSegment.Cdxa;

				var xml = referenceTrailerSegment.TyreXml.Replace("_", " ").Replace("Standard", "").Trim();

				wheels.Add(DeclarationData.Wheels.Lookup(xml));
				if(row.Field<string>("Number_of_axles") == "2")
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
				if (row.Field<string>("Number_of_axles") == "3") {
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
				}
			} else if (rfRun)
			{
				var rowTrailer = DeclarationData.StandardTrailer.GetRow(row.Field<string>("ref_code_lh"));

				var xml = rowTrailer.Field<string>("tyre_xml").Replace("_", " ").Replace("Standard", "").Trim();
				twinTyres = rowTrailer.Field<string>("twin_tyres") == "TRUE";

				wheels.Add(DeclarationData.Wheels.Lookup(xml));
				if (rowTrailer.Field<string>("Number_of_axles") == "2")
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
				if (rowTrailer.Field<string>("Number_of_axles") == "3")
				{
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
					wheels.Add(DeclarationData.Wheels.Lookup(xml));
				}
			}

			var trailerType = TrailerType.T2a;
			if (trailerValue != "none" && trailerValue !="T2b") //TODO temporary until "none" values are fixed (this is lookup value for standard trailers)
				trailerType = TrailerTypeHelper.Parse(trailerValue);

			IKernel _kernel = new StandardKernel(new VectoNinjectModule());
			var xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			XMLDeclarationTyreDataProviderV23 component = (XMLDeclarationTyreDataProviderV23)xmlInputReader.CreateComponent(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".GenericVehicles." + referenceTrailerSegment.TyreXml.Replace("/", "_") + ".xml"));

			return new MissionTrailer
			{
				TrailerType = trailerType,
				TrailerWheels = wheels,
				TrailerAxleWeightShare = axleWeightShare,
				TrailerCurbWeight = curbWeight,
				TrailerGrossVehicleWeight = grossVehicleWeight,
				DeltaCdA = deltaCDxA,
				CargoVolume = cargoVolume,
				TwinTyres = twinTyres,
				TyreComponent = component
			};

		}

		private static string GetMissionPrefix(MissionType missionType)
		{
			return missionType == MissionType.LongHaul ? "_lh" : "_other";
		}

		private static double[] GetTowingAxleWeightDistribution(DataRow row, MissionType missionType, bool isSpecific = false)
		{
			//var rfRun = false;

			string axleDistributionColumn;

			axleDistributionColumn = missionType == MissionType.LongHaul ? "towing_lh_axle" : "towing_other_axle";

			var axle1 = row.Field<string>(axleDistributionColumn + "1");
			axle1 = axle1.Contains("---") || axle1.IsNullOrEmpty() ? "0" : axle1;

			var axle2 = row.Field<string>(axleDistributionColumn + "2");
			axle2 = axle2.Contains("---") || axle2.IsNullOrEmpty() ? "0" : axle2;

			var axle3 = row.Field<string>(axleDistributionColumn + "3");
			axle3 = axle3.Contains("---") || axle3.IsNullOrEmpty() ? "0" : axle3;

			if (axle1.Contains('/'))
				axle1 = (axle1.Split('/')[0].ToDouble() / axle1.Split('/')[1].ToDouble()).ToString();
			if (axle2.Contains('/'))
				axle2 = (axle2.Split('/')[0].ToDouble() / axle2.Split('/')[1].ToDouble()).ToString();
			if (axle3.Contains('/'))
				axle3 = (axle3.Split('/')[0].ToDouble() / axle3.Split('/')[1].ToDouble()).ToString();

			return new [] { axle1.ToDouble() / 100.0, axle2.ToDouble() / 100.0, axle3.ToDouble() / 100.0 };

		}

		public Func<ConvertedSI, bool> getTPMLMAxleAssemblyFunction(string tpmlmAxleAssemblyRestriction)
		{
			var numberRegex = new Regex(@"\d\d*([.]\d*)?");
			double GetDoubleValue(Match match)
			{
				
				var numberMatch = numberRegex.Match(match.Value);
				var number = double.Parse(numberMatch.Value, NumberStyles.AllowDecimalPoint,
					NumberFormatInfo.InvariantInfo);
				return number;
			}


			List<Func<double, bool>> _functionsToCheck = new List<Func<double, bool>>();
			if (tpmlmAxleAssemblyRestriction == "---" || tpmlmAxleAssemblyRestriction == "all weights") {
				return kilogram => true;
			}

			

			//Greater Regex
			var greaterRegex = new Regex(@">\s*" + numberRegex);
			if (greaterRegex.Match(tpmlmAxleAssemblyRestriction).Success) {
				var match = greaterRegex.Match(tpmlmAxleAssemblyRestriction);
				var number = GetDoubleValue(match);
				_functionsToCheck.Add(val => val > number);
			}

			var greaterEqRegex = new Regex(@"(≥\s*" + numberRegex + @")|(>\s*=" + numberRegex + ")");
			if (greaterRegex.Match(tpmlmAxleAssemblyRestriction).Success)
			{
				var match = greaterRegex.Match(tpmlmAxleAssemblyRestriction);
				var number = GetDoubleValue(match);
				_functionsToCheck.Add(val => val >= number);
			}
			var lessRegex = new Regex(@"<\s*" + numberRegex);
			if (lessRegex.Match(tpmlmAxleAssemblyRestriction).Success)
			{
				var match = lessRegex.Match(tpmlmAxleAssemblyRestriction);
				var number = GetDoubleValue(match);
				_functionsToCheck.Add(val => val < number);
			}
			var lessEqRegex = new Regex(@"(≤\s*" + numberRegex + @")|(<\s*=\s*" + numberRegex + ")");
			if (lessEqRegex.Match(tpmlmAxleAssemblyRestriction).Success)
			{
				var match = lessEqRegex.Match(tpmlmAxleAssemblyRestriction);
				var number = GetDoubleValue(match);
				_functionsToCheck.Add(val => val <= number);
			}




			return to => {
				return _functionsToCheck.TrueForAll(func => func.Invoke(to?.Value ?? 0));
			};
		}

		private DataRow GetTrailerDataRow(
			NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation, Kilogram tpmlmAxleAssembly)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var numberOfAxlesRow = r.Field<string>("Number_of_axles");
						var typeTrailerRow = r.Field<string>("Trailer_type");
						var bodyWorkCodeRow = r.Field<string>("Bodywork_code");
						var volumeOrientationRow = r.Field<string>("Volume_orientation");
						var tpmlmAxleAssemblyRow = r.Field<string>("TPMLM_axle_assembly");
						return numberOfAxlesRow == numberOfAxles.GetLabel()
								&& typeTrailerRow == typeTrailer.ToString()
								&& bodyWorkCodeRow.Contains(bodyWorkCode.GetLabel())
								&& getTPMLMAxleAssemblyFunction(tpmlmAxleAssemblyRow).Invoke(tpmlmAxleAssembly?.ConvertToTon())
								//&& (tpmlmAxleAssemblyRow.Contains("-") ||
								//	tpmlmAxleAssemblyRow.Contains("all weights") ||
								//	(tpmlmAxleAssembly.ConvertToTon().Value > tpmlmMinRow && tpmlmAxleAssembly.ConvertToTon().Value < tpmlmMaxRow))
								&& (volumeOrientationRow == "Yes" && volumeOrientation || volumeOrientationRow == "No" && !volumeOrientation);
					});
			} catch (InvalidOperationException e) {
				// No valid trailer configuration found 
				var errorMessage = $"No valid trailer configuration found! " +
									$"Trailer type: {typeTrailer.ToString()}, " +
									$"Number of Axles: {numberOfAxles.ToXMLFormat()}, " +
									$"Bodywork type: {bodyWorkCode.ToXMLFormat()}, " +
									$"Volume orientation: {volumeOrientation} " +
									$"TPMLM axle assembly: {tpmlmAxleAssembly.ToXMLFormat()})";
				Log.Fatal(errorMessage + ErrorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

		private DataRow GetTrailerDataRow(string vehiclGroup)
		{
			DataRow row;
			try
			{
				row = _segmentTable.AsEnumerable().First(
					r => {
						var VehiclGroupRow = r.Field<string>("Vehicle_group");
						return VehiclGroupRow == vehiclGroup;
					});
			}
			catch (InvalidOperationException e)
			{
				var errorMessage = string.Format(
					ErrorMessage, vehiclGroup);
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
