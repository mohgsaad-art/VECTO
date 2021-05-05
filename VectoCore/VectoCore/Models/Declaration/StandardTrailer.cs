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
	public sealed class StandardTrailer : LookupData<string, StandardTrailerSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.standard_trailer.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. vehicleGroup: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override StandardTrailerSegment Lookup(string code)
		{
			var row = GetTrailerDataRow(code);
			if (row == null) {
				return new StandardTrailerSegment { VehicleGroup = -1 };
			}

			return new StandardTrailerSegment {
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),
				MaxGcm = row.Field<string>("max_gcm"),
				LhA1 = SquareMeter.Create(row.ParseDouble("polar_curve_lh_a1")),
				LhA2 = SquareMeter.Create(row.ParseDouble("polar_curve_lh_a2")),
				LhA3 = SquareMeter.Create(row.ParseDouble("polar_curve_lh_a3")),
				OtherA1 = SquareMeter.Create(row.ParseDouble("polar_curve_o_a1")),
				OtherA2 = SquareMeter.Create(row.ParseDouble("polar_curve_o_a2")),
				OtherA3 = SquareMeter.Create(row.ParseDouble("polar_curve_o_a3")),
				CargoVolumeLh = CubicMeter.Create(row.ParseDouble("cargo_volume_lh")),
				CargoVolumeOther = CubicMeter.Create(row.ParseDouble("cargo_volume_o")),
				CurbMassLh = Kilogram.Create(row.ParseDouble("cargo_volume_o")),
				CurbMassOther = Kilogram.Create(row.ParseDouble("cargo_volume_o")),
				CdxaLh = SquareMeter.Create(row.ParseDouble("curb_mass_trailer_body_lh")),
				CdxaOther = SquareMeter.Create(row.ParseDouble("curb_mass_trailer_body_o")),
				TyreXml = row.Field<string>("tyre_xml").Replace("_", "").Replace("Standard", ""),
				TwinTyres = row.Field<string>("twin_tyres") == "TRUE"
			};

		}

		private DataRow GetTrailerDataRow(string code)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var codeRow = r.Field<string>("code_standard");
						return codeRow == code;
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, code);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

	}
}
