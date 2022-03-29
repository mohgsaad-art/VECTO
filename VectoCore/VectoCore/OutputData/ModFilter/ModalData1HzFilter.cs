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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.ModFilter
{
	public class ModalData1HzFilter : IModalDataFilter
	{
		public ModalResults Filter(ModalResults data)
		{
			var results = (ModalResults)data.Clone();

			object[] remainingRow = null;
			var gearsList = new Dictionary<object, Second>(3);

			var absTime = 0.SI<Second>();
			var distance = 0.SI<Meter>();
			var v = data.Rows[0].Field<MeterPerSecond>(ModalResultField.v_act.GetName());
			var remainingDt = 0.SI<Second>();
			var vPrevious = v;

			for (var i = 0; i < data.Rows.Count; i++) {
				var row = data.Rows[i];

				var currentDt = row.Field<Second>(ModalResultField.simulationInterval.GetName());

				if (row.Field<Meter>(ModalResultField.dist.GetName()).IsSmaller(distance, 1e-3)) {
					LogManager.GetLogger(typeof(ModalData1HzFilter).FullName).Error("1Hz-Filter: distance must always be increasing.");
				}

				// if a remainder and currentDt would exceed 1 second: take remainder and take current row to fill up to 1 second.
				if (remainingDt > 0 && remainingDt + currentDt >= 1) {
					// calculate values
					var dt = 1.SI<Second>() - remainingDt;
					var gear = row[ModalResultField.Gear.GetName()];
					gearsList[gear] = gearsList.GetValueOrZero(gear) + dt;
					var a = (MeterPerSquareSecond)row[ModalResultField.acc.GetName()];
					var ds = dt * v + a / 2 * dt * dt;
					if (ds.IsSmaller(0)) {
						throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative. ds: {0}  {1}", ds, "1");
					}
					absTime += dt;
					v += dt * a;
					distance += ds;

					// write a new row for the combined 1 second
					var r = results.NewRow();
					r.ItemArray = AddRow(remainingRow, MultiplyRow(row.ItemArray, dt));
					r[ModalResultField.time.GetName()] = absTime;
					r[ModalResultField.simulationInterval.GetName()] = 1.SI<Second>();
					r[ModalResultField.Gear.GetName()] = gearsList.MaxBy(kv => kv.Value).Key;
					r[ModalResultField.dist.GetName()] = distance;
					r[ModalResultField.v_act.GetName()] = (v + vPrevious) / 2;
					vPrevious = v;
					results.Rows.Add(r);

					// reset remainder
					// reduce current dt by already taken diff
					gearsList.Clear();
					currentDt -= dt;
					remainingDt = 0.SI<Second>();
					remainingRow = null;
				}

				// if current row still longer than 1 second: split it to 1 second slices until it is < 1 second
				while (currentDt >= 1) {
					// calculate values
					var dt = 1.SI<Second>();
					currentDt = currentDt - 1.SI<Second>();
					var a = (MeterPerSquareSecond)row[ModalResultField.acc.GetName()];
					var ds = v * dt + a / 2 * dt * dt;
					if (ds.IsSmaller(0)) {
						throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative. ds: {0}  {1}", ds, "2");
					}
					absTime += dt;
					v += a * dt;
					distance += ds;

					// write a new row for the sliced 1 second
					var r = results.NewRow();
					r.ItemArray = row.ItemArray;
					r[ModalResultField.time.GetName()] = absTime;
					r[ModalResultField.simulationInterval.GetName()] = dt;
					r[ModalResultField.dist.GetName()] = distance;
					r[ModalResultField.v_act.GetName()] = (v + vPrevious) / 2;
					vPrevious = v;
					results.Rows.Add(r);
				}

				// if there still is something left in current row: add to weighted values to remainder
				if (currentDt > 0) {
					// calculate values
					var dt = currentDt;
					var gear = row[ModalResultField.Gear.GetName()];
					gearsList[gear] = gearsList.GetValueOrZero(gear) + dt;
					var a = (MeterPerSquareSecond)row[ModalResultField.acc.GetName()];
					var ds = v * dt + a / 2 * dt * dt;
					if (ds.IsSmaller(0)) {
						throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative. ds: {0}  {1}", ds, "3");
					}
					absTime += dt;
					v += a * dt;
					distance += ds;

					// add to remainder
					remainingRow = AddRow(remainingRow, MultiplyRow(row.ItemArray, dt));
					remainingDt += dt;
				} else {
					// reset remainder (just to be sure!)
					remainingRow = null;
					remainingDt = 0.SI<Second>();
					gearsList.Clear();
				}
			}

			// if last row was not enough to full second: take last row as whole second
			if (remainingDt > 0) {
				// calculate values
				var last = data.Rows.Cast<DataRow>().Last();
				var dt = remainingDt;
				var a = (MeterPerSquareSecond)last[ModalResultField.acc.GetName()];
				var ds = v * dt + a / 2 * dt * dt;
				if (v.IsEqual(0)) {
					ds = 0.SI<Meter>();
					a = 0.SI<MeterPerSquareSecond>();
				}
				if (ds.IsSmaller(0)) {
					throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative. ds: {0}  {1}", ds, "4");
				}
				v += a * dt;
				distance += ds;

				// write a new row for the last second
				var r = results.NewRow();
				r.ItemArray = MultiplyRow(remainingRow, 1 / dt).ToArray();
				r[ModalResultField.time.GetName()] = VectoMath.Ceiling(absTime);
				r[ModalResultField.simulationInterval.GetName()] = 1.SI<Second>();
				r[ModalResultField.Gear.GetName()] = gearsList.MaxBy(kv => kv.Value).Key;
				r[ModalResultField.dist.GetName()] = distance;
				r[ModalResultField.v_act.GetName()] = (v + vPrevious) / 2;
				results.Rows.Add(r);
			}

			return results;
		}

		private static IEnumerable<object> MultiplyRow(IEnumerable<object> row, SI dt)
		{
			return row.Select(val => {
				if (val is SI) {
					val = (SI)val * dt.Value();
				} else {
					val.Switch()
						.Case<int>(i => val = i * dt.Value())
						.Case<double>(d => val = d * dt.Value())
						.Case<float>(f => val = f * dt.Value())
						.Case<uint>(ui => val = ui * dt.Value());
				}
				return val;
			});
		}

		private static object[] AddRow(IEnumerable<object> row, IEnumerable<object> addRow)
		{
			if (row == null) {
				return addRow.ToArray();
			}
			if (addRow == null) {
				return row.ToArray();
			}

			return row.ZipAll(addRow, (val, addVal) => {
				if (val is SI || addVal is SI) {
					if (DBNull.Value == val) {
						val = addVal;
					} else if (DBNull.Value != addVal) {
						val = (SI)val + (SI)addVal;
					}
				} else {
					val.Switch()
						.Case<int>(i => val = i + (int)addVal)
						.Case<double>(d => val = d + (double)addVal)
						.Case<float>(f => val = f + (float)addVal)
						.Case<uint>(ui => val = ui + (uint)addVal);
				}
				return val;
			}).ToArray();
		}

		public string ID
		{
			get { return "1Hz"; }
		}
	}
}