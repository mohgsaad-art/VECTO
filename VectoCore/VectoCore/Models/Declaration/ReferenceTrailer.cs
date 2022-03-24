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
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class ReferenceTrailer : LookupData<int, ReferenceTrailerSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.reference_trailer.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. vehicleGroup: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override ReferenceTrailerSegment Lookup(int vehicleGroup)
		{
			var row = GetTrailerDataRow(vehicleGroup);
			if (row == null) {
				return new ReferenceTrailerSegment { VehicleGroup = -1 };
			}

			return new ReferenceTrailerSegment
			{
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),
				CargoVolume = CubicMeter.Create(row["cargo_volume_semi_trailer"].ToString() == "---" ? 0 : row.ParseDouble("cargo_volume_semi_trailer")),
				CurbMass = Kilogram.Create(!row.Field<string>("curb_mass_semi_trailer").Contains("-") ? row.ParseDouble("curb_mass_semi_trailer") : 0),
				TyreXml = row.Field<string>("tyre_xml"),
				TwinTyres = string.Equals(row.Field<string>("twin_tyres"), "TRUE", StringComparison.InvariantCultureIgnoreCase),
				ExternalBodyHeight = Meter.Create(row.ParseDouble("external_body_height")),
				ExternalBodyLength = Meter.Create(row.ParseDouble("external_body_length")),
				ExternalBodyWidth = Meter.Create(row.ParseDouble("external_body_width")),
				TotalTrailerHeight = Meter.Create(row.ParseDouble("total_trailer_height")),
				LengthTrailerFrontToFirstAxle = Meter.Create(row.ParseDouble("length_trailer_front_first_axle")),
				LengthBetweenCentersOfAxles = Meter.Create(row.ParseDouble("distance_first_last_axle")),
				LengthAxleCorrection = row.Field<string>("length_axle_correction") == "1",
				WidthCorrection = row.Field<string>("width_correction") == "1",
				HeightCorrection = row.Field<string>("height_correction") == "1",
				VolumeCorrection = row.Field<string>("volume_correction") == "1",
				AeroCorrection = row.Field<string>("aero_correction") == "1",
				CouplingPointCorrection = row.Field<string>("coupling_point_correction") == "1",
			};
			
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
