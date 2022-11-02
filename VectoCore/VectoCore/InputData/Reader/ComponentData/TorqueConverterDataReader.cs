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

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class TorqueConverterDataReader
	{
		public static TorqueConverterData ReadFromFile(string filename, PerSecond referenceRpm, PerSecond maxRpm,
			ExecutionMode mode, double ratio, MeterPerSquareSecond lcMinAcceleration, MeterPerSquareSecond ccMinAcceleration)
		{
			return Create(VectoCSVFile.Read(filename), referenceRpm, maxRpm, mode, ratio, lcMinAcceleration, ccMinAcceleration);
		}

		public static TorqueConverterData ReadFromStream(Stream stream, PerSecond referenceRpm, PerSecond maxRpm,
			ExecutionMode mode, double ratio, MeterPerSquareSecond lcMinAcceleration, MeterPerSquareSecond ccMinAcceleration)
		{
			return Create(VectoCSVFile.ReadStream(stream), referenceRpm, maxRpm, mode, ratio, lcMinAcceleration,
				ccMinAcceleration);
		}

		public static TorqueConverterData Create(DataTable data, PerSecond referenceRpm, PerSecond maxRpm, ExecutionMode mode,
			double ratio, MeterPerSquareSecond lcMinAcceleration, MeterPerSquareSecond ccMinAcceleration)
		{
			if (data == null) {
				throw new VectoException("TorqueConverter Characteristics data is missing.");
			}

			if (data.Columns.Count != 3) {
				throw new VectoException("TorqueConverter Characteristics data must consist of 3 columns");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("TorqueConverter Characteristics data must contain at least 2 lines with numeric values");
			}

			IEnumerable<TorqueConverterEntry> characteristicTorque;
			if (HeaderIsValid(data.Columns)) {
				characteristicTorque = (from DataRow row in data.Rows
					select
						new TorqueConverterEntry() {
							SpeedRatio = row.ParseDouble(Fields.SpeedRatio),
							Torque = row.ParseDouble(Fields.CharacteristicTorque).SI<NewtonMeter>(),
							TorqueRatio = row.ParseDouble(Fields.TorqueRatio)
						}).ToArray();
			} else {
				characteristicTorque = (from DataRow row in data.Rows
					select
						new TorqueConverterEntry() {
							SpeedRatio = row.ParseDouble(0),
							Torque = row.ParseDouble(2).SI<NewtonMeter>(),
							TorqueRatio = row.ParseDouble(1)
						}).ToArray();
			}
			if (mode == ExecutionMode.Declaration) {
				var tcDrag = DeclarationData.TorqueConverter.GetTorqueConverterDragCurve(ratio, characteristicTorque.First(), characteristicTorque.Last(x => x.SpeedRatio < ratio));
				characteristicTorque =
					characteristicTorque.Where(x => x.SpeedRatio < ratio)
						.Concat(tcDrag)
						.ToArray();
			} else {
				if (!characteristicTorque.Any(x => x.SpeedRatio > ratio)) {
					characteristicTorque =
						characteristicTorque.Where(x => x.SpeedRatio < ratio)
							.Concat(DeclarationData.TorqueConverter.GetTorqueConverterDragCurve(ratio, characteristicTorque.First(), characteristicTorque.Last()))
							.ToArray();
				}
			}
			return new TorqueConverterData(characteristicTorque, referenceRpm, maxRpm, lcMinAcceleration, ccMinAcceleration);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.SpeedRatio) && columns.Contains(Fields.TorqueRatio) &&
					columns.Contains(Fields.CharacteristicTorque);
		}

		public static class Fields
		{
			public const string SpeedRatio = "Speed Ratio";
			public const string TorqueRatio = "Torque Ratio";
			public const string CharacteristicTorque = "MP1000";
		}
	}
}