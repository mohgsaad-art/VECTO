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
	public sealed class TrailerSegments : LookupData<NumberOfTrailerAxles, TypeTrailer, BodyWorkCode, bool, TrailerSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".TrailerSegmentationTable.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. NumberOfAxles: {0}, TrailerType: {1}, BodyWorkCode: {2}, VolumeOrientation: {3}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override TrailerSegment Lookup(NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation)
		{

			var row = GetTrailerDataRow(numberOfAxles, typeTrailer, bodyWorkCode, volumeOrientation);
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
				MaxGcm = row.ParseDouble("max_gcm"),
				Missions = CreateMissions(row)
			};
		}

		private static Mission[] CreateMissions(DataRow row)
		{
			//TODO: get only the missions for the specific trailer, i think they are 4
			var missionTypes = Enum.GetValues(typeof(MissionType)).Cast<MissionType>();
			var missions = new List<Mission>();
			foreach (var missionType in missionTypes) {

				//double maxPayload = 0;
				double minLoad = 0;
				double maxLoad = 0;

				//TODO: get payloads dynamicly from row based on mission
				switch (missionType) {
					case MissionType.LongHaul:
						minLoad = row.ParseDouble("LH_payload_low");
						maxLoad = row.ParseDouble("LH_payload_rep");
						break;
					default:
						minLoad = row.ParseDouble("OM_payload_low");
						maxLoad = row.ParseDouble("OM_payload_rep");
						break;
				}

				var mission = new Mission
				{
					MissionType = missionType,
					//MaxPayload = Kilogram.Create(maxPayload),
					MinLoad = Kilogram.Create(minLoad),
					MaxLoad = Kilogram.Create(maxLoad),
					//DefaultCDxA = (SquareMeter)row[""]
				};
				missions.Add(mission);
			}

			return missions.ToArray();
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
								&& (volumeOrientationRow == "Yes" && volumeOrientation 
									|| volumeOrientationRow == "No" && !volumeOrientation);
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, numberOfAxles, typeTrailer, bodyWorkCode, volumeOrientation);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}
	}
}
