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
	public sealed class Impact : LookupData<string, string, MissionType, KeyValuePair<double,double>>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".liftable_steered_axles.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. Vehicle Group: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override KeyValuePair<double, double> Lookup(string vehicleGroup, string type, MissionType missionType)
		{
			
			var row = GetTrailerDataRow(vehicleGroup);
			if (row == null) {
				return new KeyValuePair<double, double>(1.0, 1.0);
			}

			if (type == "Liftable") {
				if (missionType == MissionType.LongHaul)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Liftaxle_lh_low").Replace("%", "")),
						double.Parse(row.Field<string>("Liftaxle_lh_ref").Replace("%", "")));

				if (missionType == MissionType.RegionalDelivery)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Liftaxle_r_low").Replace("%", "")),
						double.Parse(row.Field<string>("Liftaxle_r_ref").Replace("%", "")));

				if (missionType == MissionType.UrbanDelivery)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Liftaxle_u_low").Replace("%", "")),
						double.Parse(row.Field<string>("Liftaxle_u_ref").Replace("%", "")));
			}
			else if (type == "Steered")
			{
				if (missionType == MissionType.LongHaul)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Steered_lh_low").Replace("%", "")),
						double.Parse(row.Field<string>("Steered_lh_ref").Replace("%", "")));

				if (missionType == MissionType.RegionalDelivery)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Steered_r_low").Replace("%", "")),
						double.Parse(row.Field<string>("Steered_r_ref").Replace("%", "")));

				if (missionType == MissionType.UrbanDelivery)
					return new KeyValuePair<double, double>(
						double.Parse(row.Field<string>("Steered_u_low").Replace("%", "")),
						double.Parse(row.Field<string>("Steered_u_ref").Replace("%", "")));
			}


			return new KeyValuePair<double, double>(1.0, 1.0);
		}

		
		private DataRow GetTrailerDataRow(string vehiclegroup)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var vehiclegroupRow = r.Field<string>("vehicle_group");
						return vehiclegroupRow == vehiclegroup;
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(
					ErrorMessage, vehiclegroup);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}

	}
}
