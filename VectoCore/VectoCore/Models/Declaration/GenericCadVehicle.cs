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
	public sealed class GenericCadVehicle : LookupData<int, GenericCadSegment>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.generic_cad_vehicle.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. vehicleGroup: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override GenericCadSegment Lookup(int vehicleGroup)
		{
			var row = GetTrailerDataRow(vehicleGroup);
			if (row == null) {
				return new GenericCadSegment { VehicleGroup = -1 };
			}

			return new GenericCadSegment
			{
				VehicleGroup = (int)row.ParseDouble("Vehicle_group"),
				TotalHeight = Meter.Create(row.ParseDouble("total_trailer_height")),
				ExternalBodyHeight = Meter.Create(row.ParseDouble("External_body_height")),
				AcrBase = SquareMeter.Create(row.ParseDouble("AcrBase")),
				CdxABase = row.ParseDouble("Yaw0"),
				A1 = SquareMeter.Create(row.ParseDouble("a1")),
				A2 = SquareMeter.Create(row.ParseDouble("a2")),
				A3 = SquareMeter.Create(row.ParseDouble("a3")),
				Lbf = Meter.Create(row.ParseDouble("Lbf")),
				DCdf0 = row.ParseDouble("dCdf0"),
				Lba = Meter.Create(row.ParseDouble("Lba")),
				DCda0 = row.ParseDouble("dCdf0"),
				Lbr = Meter.Create(row.ParseDouble("Lbr")),
				DCdr0 = row.ParseDouble("dCdr0"),
				
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
