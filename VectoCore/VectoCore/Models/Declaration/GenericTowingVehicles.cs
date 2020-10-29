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
	public sealed class GenericTowingVehicles : LookupData<string, int>
	{
		private DataTable _genericTowingVehiclesTable;

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".GenericTowingVehicles.csv";

		protected override string ErrorMessage => "ERROR: Could not find this vehicle code.";

		protected override void ParseData(DataTable table)
		{
			_genericTowingVehiclesTable = table.Copy();
		}

		public override int Lookup(string vehicleCode)
		{

			var row = GetDataDataRow(vehicleCode);
			return row == null ? 0 : 1; //TODO
		}


		private DataRow GetDataDataRow(string vehicleCode)
		{
			DataRow row;
			try {
				row = _genericTowingVehiclesTable.AsEnumerable().FirstOrDefault(
					r => {
						var code = r.Field<string>("Code");

						return code.Contains(vehicleCode);
					});
			} catch (InvalidOperationException e) {
				var errorMessage = string.Format(ErrorMessage);
				Log.Fatal(errorMessage);
				throw new VectoException(errorMessage, e);
			}

			return row;
		}
	}
}
