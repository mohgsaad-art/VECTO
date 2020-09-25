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
	public sealed class TrailerSegments : LookupData<int, TypeTrailer, BodyWorkCode, bool, int>
	{
		private DataTable _segmentTable;

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".trailerSegmentationTable.csv"; }
		}

		protected override string ErrorMessage
		{
			get {
				return
					"ERROR: Could not find the vehicle group for trailer. NumberOfAxles: {0}, TrailerType: {1}, BodyWorkCode: {2}, VolumeOrientation: {3}";
			}
		}

		protected override void ParseData(DataTable table)
		{
			_segmentTable = table.Copy();
		}

		public override int Lookup(int numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation)
		{

			var row = GetTrailerDataRow(numberOfAxles, typeTrailer, bodyWorkCode, volumeOrientation);
			if (row == null) {
				return 100;
			}

			return (int)row.ParseDouble("Vehicle_group");
		}


		private DataRow GetTrailerDataRow(
			int numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation)
		{
			DataRow row;
			try {
				row = _segmentTable.AsEnumerable().First(
					r => {
						var numberOfAxlesRow = r.ParseDouble("Number_of_axles");
						var typeTrailerRow = r.Field<string>("Trailer_type");
						var bodyWorkCodeRow = r.Field<string>("Bodywork_code");
						var volumeOrientationRow = r.Field<string>("Volume_orientation");
						return (int)numberOfAxlesRow == numberOfAxles 
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
