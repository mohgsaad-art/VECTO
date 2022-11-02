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
using System.Collections.Generic;
using System.IO;
using System.Text;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public static class ShiftPolygonExport
	{
		public static void WriteShiftPolygon(ShiftPolygon shiftPolygon, string fileName)
		{
			var lines = new SortedList<double, ShiftPolygonFileEntry>(10);
			foreach (var entry in shiftPolygon.Downshift) {
				lines.Add(Math.Round(entry.Torque.Value(), 4),
					new ShiftPolygonFileEntry {
						Torque = entry.Torque,
						DownShift = entry.AngularSpeed,
						UpShift = shiftPolygon.Upshift.Count > 0 ? shiftPolygon.InterpolateUpshiftSpeed(entry.Torque) : null
					});
			}

			foreach (var entry in shiftPolygon.Upshift) {
				var torque = Math.Round(entry.Torque.Value(), 4);
				var line = lines.GetOrAdd(torque, _ => new ShiftPolygonFileEntry {
					Torque = entry.Torque,
					DownShift = shiftPolygon.Downshift.Count > 0 ? shiftPolygon.InterpolateDownshiftSpeed(entry.Torque) : null
				});
				line.UpShift = entry.AngularSpeed;
			}

			var sb = new StringBuilder(lines.Count + 1);
			sb.AppendLine("engine torque [Nm],downshift rpm [1/min],upshift rpm [1/min]");
			foreach (var line in lines.Values) {
				if (line.DownShift == null)
					sb.AppendLine($"{line.Torque.ToOutputFormat()},,{line.UpShift.AsRPM:0.0000}");
				else if (line.UpShift == null)
					sb.AppendLine($"{line.Torque.ToOutputFormat()},{line.DownShift.AsRPM:0.0000},");
				else
					sb.AppendLine($"{line.Torque.ToOutputFormat()},{line.DownShift.AsRPM:0.0000},{line.UpShift.AsRPM:0.0000}");
			}

			File.WriteAllText(fileName, sb.ToString());
		}

		private class ShiftPolygonFileEntry
		{
			public NewtonMeter Torque;
			public PerSecond DownShift;
			public PerSecond UpShift;
		}
	}
}