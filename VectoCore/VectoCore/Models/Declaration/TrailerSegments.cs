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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TrailerSegments : LookupData<NumberOfTrailerAxles, TypeTrailer, BodyWorkCode, TrailerSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".TrailerSegmentationTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. NumberOfAxles: {0}, TrailerType: {1}, BodyWorkCode: {2}, VolumeOrientation: {3}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override TrailerSegment Lookup(NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode)
		{

			var row = GetTrailerDataRow(numberOfAxles, typeTrailer, bodyWorkCode);
			if (row == null) {
				return new TrailerSegment { VehicleGroup = -1 };
			}

			return new TrailerSegment {
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),
				VehicleGroupActive = row.ParseBoolean("Active"),
				GenericCode = row.Field<string>("Generic_code"),
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
				A3 = SquareMeter.Create(row.ParseDouble("a3"))
			};


		}

		private static Mission[] CreateMissions(DataRow row)
		{
			var missionTypes = new List<MissionType>();

			if (row["Mission_long_haul"].ToString().Contains("X"))
				missionTypes.Add(MissionType.LongHaul);
			if (row["Mission_regional_delivery"].ToString().Contains("X"))
				missionTypes.Add(MissionType.RegionalDelivery);
			if (row["Mission_urban_delivery"].ToString().Contains("X"))
				missionTypes.Add(MissionType.UrbanDelivery);

			var missions = new List<Mission>();
			foreach (var missionType in missionTypes) {

				//double maxPayload = 0;
				double minLoad = 0;
				double maxLoad = 0;

				switch (missionType) {
					case MissionType.LongHaul:
						minLoad = row.ParseDouble("specific_lh_semi_payload_low");
						maxLoad = row.ParseDouble("specific_lh_semi_payload_rep");
						break;
					default:
						minLoad = row.ParseDouble("specific_other_semi_payload_low");
						maxLoad = row.ParseDouble("specific_other_semi_payload_rep");
						break;
				}

				var numberOfAxles = (int)row.ParseDouble("Number_of_axles");
				var axleDistributionColumn =
					missionType == MissionType.LongHaul ? "towing_lh_axle" : "towing_other_axle";

				double.TryParse(row.Field<string>(axleDistributionColumn + "1"), out var axle1);
				double.TryParse(row.Field<string>(axleDistributionColumn + "2"), out var axle2);
				double.TryParse(row.Field<string>(axleDistributionColumn + "3"), out var axle3);

				var mission = new Mission
				{
					MissionType = missionType,
					MinLoad = Kilogram.Create(minLoad),
					MaxLoad = Kilogram.Create(maxLoad),
					AxleWeightDistribution = (numberOfAxles == 1 ? 
						new [] { axle1 } : (numberOfAxles == 2 ? new[] { axle1, axle2 } : new[] { axle1, axle2, axle3 })), 
					CurbMass = Kilogram.Create(missionType == MissionType.LongHaul ? row.ParseDouble("ref_curb_lh") : row.ParseDouble("ref_curb_other"))
				};
				missions.Add(mission);
			}

			return missions.ToArray();
		}


		private DataRow GetTrailerDataRow(
			NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode)
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
								&& bodyWorkCodeRow.Contains(bodyWorkCode.GetLabel());
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, numberOfAxles, typeTrailer, bodyWorkCode);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}
	}
}
