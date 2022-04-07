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
	public sealed class SpecificTrailer : LookupData<int, SpecificTrailerSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.specific_trailer.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. vehicleGroup: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override SpecificTrailerSegment Lookup(int vehicleGroup)
		{
			var row = GetTrailerDataRow(vehicleGroup);
			if (row == null) {
				return new SpecificTrailerSegment { VehicleGroup = -1 };
			}

			return new SpecificTrailerSegment
			{
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),

				LengthAxleCorrection = row.Field<string>("length_axle_correction") == "1",
				WidthCorrection = row.Field<string>("width_correction") == "1",
				HeightCorrection = row.Field<string>("height_correction") == "1",
				VolumeCorrection = row.Field<string>("volume_correction") == "1",   
				AeroCorrection = row.Field<string>("aero_correction") == "1",
				CouplingPointCorrection = row.Field<string>("coupling_point_correction") == "1",

				Aero0 = new[] {
					row.ParseDoubleOrGetDefault("aero0_0"),
					row.ParseDoubleOrGetDefault("aero0_3"),
					row.ParseDoubleOrGetDefault("aero0_6"), 
					row.ParseDoubleOrGetDefault("aero0_9")
				},
				Aero1 = new[] { 
					row.ParseDoubleOrGetDefault("aero1_0"), 
					row.ParseDoubleOrGetDefault("aero1_3"), 
					row.ParseDoubleOrGetDefault("aero1_6"), 
					row.ParseDoubleOrGetDefault("aero1_9") },
				Aero2 = new[] { 
					row.ParseDoubleOrGetDefault("aero2_0"), 
					row.ParseDoubleOrGetDefault("aero2_3"), 
					row.ParseDoubleOrGetDefault("aero2_6"), 
					row.ParseDoubleOrGetDefault("aero2_9") },
				Aero3 = new[] { 
					row.ParseDoubleOrGetDefault("aero3_0"), 
					row.ParseDoubleOrGetDefault("aero3_3"), 
					row.ParseDoubleOrGetDefault("aero3_6"), 
					row.ParseDoubleOrGetDefault("aero3_9") },
				Aero4 = new[] { 
					row.ParseDoubleOrGetDefault("aero4_0"), 
					row.ParseDoubleOrGetDefault("aero4_3"), 
					row.ParseDoubleOrGetDefault("aero4_6"), 
					row.ParseDoubleOrGetDefault("aero4_9") },
				Aero5 = new[] { 
					row.ParseDoubleOrGetDefault("aero5_0"), 
					row.ParseDoubleOrGetDefault("aero5_3"), 
					row.ParseDoubleOrGetDefault("aero5_6"), 
					row.ParseDoubleOrGetDefault("aero5_9") },
				Aero6 = new[] { 
					row.ParseDoubleOrGetDefault("aero6_0"), 
					row.ParseDoubleOrGetDefault("aero6_3"), 
					row.ParseDoubleOrGetDefault("aero6_6"), 
					row.ParseDoubleOrGetDefault("aero6_9") },
				Aero7 = new[] { 
					row.ParseDoubleOrGetDefault("aero7_0"), 
					row.ParseDoubleOrGetDefault("aero7_3"), 
					row.ParseDoubleOrGetDefault("aero7_6"), 
					row.ParseDoubleOrGetDefault("aero7_9") },
				Aero8 = new[] { 
					row.ParseDoubleOrGetDefault("aero8_0"), 
					row.ParseDoubleOrGetDefault("aero8_3"), 
					row.ParseDoubleOrGetDefault("aero8_6"), 
					row.ParseDoubleOrGetDefault("aero8_9") },

			};

		}

		public Mission.AxleImpact GetImpact(int vehicleGroup, string type, MissionType missionType)
		{
			var row = GetTrailerDataRow(vehicleGroup);
			if (row == null)
			{
				return new Mission.AxleImpact();
			}

			if (type == "Liftable")
			{
				switch (missionType) {
					case MissionType.LongHaul:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("liftable_lh_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("liftable_lh_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
					case MissionType.RegionalDelivery:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("liftable_r_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("liftable_r_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
					case MissionType.UrbanDelivery:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("liftable_u_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("liftable_u_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
				}
			}
			else if (type == "Steered")
			{
				switch (missionType) {
					case MissionType.LongHaul:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("steered_lh_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("steered_lh_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
					case MissionType.RegionalDelivery:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("steered_r_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("steered_r_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
					case MissionType.UrbanDelivery:
						return new Mission.AxleImpact(
							double.Parse(row.Field<string>("steered_u_payload_low").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture),
							double.Parse(row.Field<string>("steered_u_payload_rep").Replace("%", "").Replace("---", "0"), CultureInfo.InvariantCulture));
				}
			}


			return new Mission.AxleImpact();
		}

		private DataRow GetTrailerDataRow(int vehicleGroup)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var vehicleGroupRow = r.Field<string>("Vehicle_group");
						return vehicleGroupRow == vehicleGroup.ToString();
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, vehicleGroup);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

	}
}
