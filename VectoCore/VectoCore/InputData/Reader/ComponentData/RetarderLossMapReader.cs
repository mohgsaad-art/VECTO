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
*/

using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class RetarderLossMapReader
	{
		/// <summary>
		/// Read the retarder loss map from a file.
		/// </summary>
		public static RetarderLossMap ReadFromFile(string fileName)
		{
			try {
				return Create(VectoCSVFile.Read(fileName));
			} catch (Exception ex) {
				throw new VectoException("ERROR while loading RetarderLossMap: " + ex.Message);
			}
		}

		/// <summary>
		/// Create the retarder loss map from an appropriate datatable. (2 columns: Retarder Speed, Torque Loss)
		/// </summary>
		public static RetarderLossMap Create(DataTable data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("RetarderLossMap Data File must consist of 2 columns: {0}, {1}", Fields.RetarderSpeed,
					Fields.TorqueLoss);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("RetarderLossMap must contain at least 2 entries.");
			}

			if (!data.Columns.Contains(Fields.RetarderSpeed) || !data.Columns.Contains(Fields.TorqueLoss)) {
				LoggingObject.Logger<RetarderLossMap>().Warn(
					"RetarderLossMap: Header Line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.RetarderSpeed,
					Fields.TorqueLoss,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.RetarderSpeed;
				data.Columns[1].ColumnName = Fields.TorqueLoss;
			}

			return new RetarderLossMap(data.Rows.Cast<DataRow>()
				.Select(row => new RetarderLossMap.RetarderLossEntry {
					RetarderSpeed = row.ParseDouble(Fields.RetarderSpeed).RPMtoRad(),
					TorqueLoss = row.ParseDouble(Fields.TorqueLoss).SI<NewtonMeter>()
				}).OrderBy(e => e.RetarderSpeed).ToArray());
		}

		public static class Fields
		{
			/// <summary>
			///     [rpm]
			/// </summary>
			public const string RetarderSpeed = "Retarder Speed";

			/// <summary>
			///     [Nm]
			/// </summary>
			public const string TorqueLoss = "Torque Loss";
		}
	}
}