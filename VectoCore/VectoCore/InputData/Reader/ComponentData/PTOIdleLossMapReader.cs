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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class PTOIdleLossMapReader
	{

		public static PTOLossMap ReadFromStream(Stream stream)
		{
			try {
				return Create(VectoCSVFile.ReadStream(stream));
			} catch (Exception ex) {
				throw new VectoException("ERROR while loading PTO Idle LossMap: " + ex.Message);
			}
		}

		/// <summary>
		/// Create the pto idle loss map from an appropriate datatable. (2 columns: Engine Speed, PTO Torque)
		/// </summary>
		public static PTOLossMap Create(DataTable data)
		{
			if (data.Columns.Count != 2) {
				throw new VectoException("PTO Idle LossMap Data File must consist of 2 columns: {0}, {1}", Fields.EngineSpeed,
					Fields.PTOTorque);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("PTO Idle LossMap must contain at least 2 entries.");
			}

			if (!(data.Columns.Contains(Fields.EngineSpeed) && data.Columns.Contains(Fields.PTOTorque))) {
				LoggingObject.Logger<RetarderLossMap>().Warn(
					"PTO Idle LossMap: Header Line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.EngineSpeed, 
					Fields.PTOTorque, 
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.EngineSpeed;
				data.Columns[1].ColumnName = Fields.PTOTorque;
			}

			return new PTOLossMap(data.Rows.Cast<DataRow>()
				.Select(row => new PTOLossMap.Entry {
					EngineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					PTOTorque = row.ParseDouble(Fields.PTOTorque).SI<NewtonMeter>()
				}).OrderBy(e => e.EngineSpeed).ToArray());
		}

		public static class Fields
		{
			/// <summary>
			///     [rpm]
			/// </summary>
			public const string EngineSpeed = "Engine speed";

			/// <summary>
			///     [Nm]
			/// </summary>
			public const string PTOTorque = "PTO Torque";
		}

		public static PTOLossMap GetZeroLossMap()
		{
			return new PTOLossMap(new[] {
				new PTOLossMap.Entry { EngineSpeed = 0.RPMtoRad(), PTOTorque = 0.SI<NewtonMeter>() },
				new PTOLossMap.Entry { EngineSpeed = 10000.RPMtoRad(), PTOTorque = 0.SI<NewtonMeter>() },
			});
		}
	}
}