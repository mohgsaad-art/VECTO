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

using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace DeclarationCycleZip
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var cycleData = DrivingCycleDataReader.ReadFromFile(args[0], CycleType.DistanceBased, false);

			var table = new DataTable();
			table.Columns.Add("<s>");
			table.Columns.Add("<v>");
			table.Columns.Add("<grad>");
			table.Columns.Add("<stop>");

			var lastDistance = cycleData.Entries.First().Distance - 1.SI<Meter>();
			DrivingCycleData.DrivingCycleEntry prevEntry = null;
			foreach (var x in cycleData.Entries) {
				if (x.Distance.IsEqual(lastDistance)) {
					if (prevEntry != null && prevEntry.VehicleTargetSpeed.IsEqual(0.SI<MeterPerSecond>())) {
						x.Distance = x.Distance + 1.SI<Meter>();
					} else {
						continue;
					}
				}
				var row = table.NewRow();
				row["<s>"] = x.Distance.Value().ToString(CultureInfo.InvariantCulture);
				row["<v>"] = x.VehicleTargetSpeed.ConvertToKiloMeterPerHour().Value.ToString(CultureInfo.InvariantCulture);
				row["<grad>"] = x.RoadGradientPercent.Value().ToString(CultureInfo.InvariantCulture);
				row["<stop>"] = x.StoppingTime.Value().ToString(CultureInfo.InvariantCulture);
				table.Rows.Add(row);
				lastDistance = x.Distance;
				prevEntry = x;
			}

			VectoCSVFile.Write(Path.GetFileName(args[0]), table);
		}
	}
}