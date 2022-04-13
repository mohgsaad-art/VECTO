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
using Microsoft.SqlServer.Server;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class TrailerReferenceFactors : LookupData<string, TrailerReferenceRatios>
	{
		private DataTable _segmentTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Trailer.reference_ratios.csv";

		protected override string ErrorMessage => "ERROR: Could not find the vehicle group for trailer. Vehicle Group: {0}";

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override TrailerReferenceRatios Lookup(string vehicleGroupAnnex1)
		{
			System.Diagnostics.Debug.WriteLine(vehicleGroupAnnex1);
			var row = GetTrailerDataRow(vehicleGroupAnnex1);
			if (row == null) {
				return new TrailerReferenceRatios();
			}

			try {
				return new TrailerReferenceRatios {
					LongHaulLow = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_LongHaul, 0, row),
					LongHaulRef = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_LongHaul, 1, row),
					RegionalDeliveryLow = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_Regional_Delivery, 0, row),
					RegionalDeliveryRef = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_Regional_Delivery, 1, row),
					UrbanDeliveryLow = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_Urban_Delivery, 0, row),
					UrbanDeliveryRef = GetDouble(CSVNames.TrailerReferenceFactors_Ref_Ratio_Urban_Delivery, 1, row)
				};
			} catch (Exception ex) {
				Log.Error(ex.Message);
				//TODO: ignored
				return new TrailerReferenceRatios();
			}


		}

		private double GetDouble(string rowName, int elementIndex, DataRow row)
		{
			var refRatioString = row[rowName].ToString();
			if (refRatioString == "---") {
				return 0;
			}
			return double.Parse(refRatioString.Split('/')[elementIndex], NumberStyles.AllowDecimalPoint,
				NumberFormatInfo.InvariantInfo);
		}

		private DataRow GetTrailerDataRow(string vehicleGroupAnnex1)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var group = r.Field<string>(CSVNames.TrailerReferenceFactors_Vehicle_group_annex1);
						//var group = r.Field<string>("Vehicle_Group");
						return vehicleGroupAnnex1 == group;
					});
			} catch (Exception e) {
				var errorMessage = string.Format(
					ErrorMessage, vehicleGroupAnnex1);
				Log.Fatal(errorMessage + e.Message);
				row = null;
			}

			return row;
		}

	}
}
