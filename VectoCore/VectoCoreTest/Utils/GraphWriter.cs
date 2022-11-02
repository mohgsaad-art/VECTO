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
using System.IO;
using System.Drawing;
using System.Linq;
#if !NET5_0_OR_GREATER
using System.Windows.Forms.DataVisualization.Charting;
#endif
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class GraphWriter
	{
		private bool _enabled = true;

		private Size _diagramSize = new Size(2000, 440);

#if !NET5_0_OR_GREATER
		private readonly Font AxisLabelFont = new Font("Consolas", 10);
		private readonly Font AxisTitleFont = new Font("Verdana", 12);
		private readonly Font LegendFont = new Font("Verdana", 14);
#endif

		public string Series2Label { get; set; }

		public string Series1Label { get; set; }

		public ModalResultField[] Yfields { get; set; }

		public ModalResultField[] Xfields { get; set; }

		public bool PlotDrivingMode = false;

		public bool PlotIgnitionState = false;

		public void Enable()
		{
			_enabled = true;
		}

		public void Disable()
		{
			_enabled = false;
		}

		public void Write(string fileNameV3, string fileNameV22 = null)
		{
#if !NET5_0_OR_GREATER
			if (!_enabled) {
				return;
			}

			var modDataV3 = VectoCSVFile.Read(fileNameV3);
			if (!File.Exists(fileNameV22)) {
				LogManager.GetLogger(typeof(GraphWriter).FullName).Error("Modfile V2.2 not found: " + fileNameV22);
				//Write(fileNameV3);
				//return;
			}
			DataTable modDataV22 = null;
			if (!string.IsNullOrWhiteSpace(fileNameV22) && File.Exists(fileNameV22))
				modDataV22 = VectoCSVFile.Read(fileNameV22);

			var titleHeight = (50 * 100.0f) / (_diagramSize.Height * Yfields.Length);

			foreach (var xfield in Xfields) {
				var fileName = $"{Path.GetFileNameWithoutExtension(fileNameV3)}_{xfield.GetName()}.png";

				var x = LoadData(modDataV3, xfield.GetName());
				var x2 = new[] { double.NegativeInfinity };
				if (fileNameV22 != null && modDataV22 != null) {
					x2 = LoadData(modDataV22, xfield.GetName());
				}
				var plotSize = new Size(_diagramSize.Width, _diagramSize.Height * Yfields.Length);
				var maxX = (int)(Math.Ceiling(Math.Max(x.Max(), x2.Max()) * 1.01 / 10.0) * 10.0);
				var minX = (int)(Math.Floor(Math.Max(x.Min(), x2.Min()) / 10.0) * 10.0);
				var chart = new Chart { Size = plotSize };

				for (var i = 0; i < Yfields.Length; i++) {
					var yfield = Yfields[i];
					var y = LoadData(modDataV3, yfield.GetShortCaption());

					var chartArea = AddChartArea(chart, yfield.ToString(), xfield.GetCaption(), maxX, minX,
						yfield.GetCaption(), yfield == ModalResultField.Gear);

					var legend = CreateLegend(chart, yfield.ToString());

					if (yfield == ModalResultField.v_act) {
						var y3 = LoadData(modDataV3, ModalResultField.v_targ.GetShortCaption());
						var series3 = CreateSeries("v_target", legend, chartArea, chart, Color.Green, x, y3);
						var min = Math.Min(y3.Min(), y.Min());
						var max = Math.Max(y3.Max(), y.Max());
						chartArea.AxisY.Minimum = Math.Floor(min - (max - min) * 0.1 - 1);
						chartArea.AxisY.Maximum = Math.Ceiling(max + (max - min) * 0.1 + 1);
					}


					if ((Yfields.Contains(ModalResultField.altitude) && yfield == ModalResultField.altitude) ||
						(!Yfields.Contains(ModalResultField.altitude) && yfield == ModalResultField.v_act)) {

						var grad = LoadData(modDataV3, ModalResultField.grad.GetShortCaption());

						chartArea.AxisY2.Enabled = AxisEnabled.True;
						chartArea.AxisY2.Title = "gradient [%]";
						chartArea.AxisY2.TitleFont = AxisTitleFont;
						chartArea.AxisY2.LabelStyle.Font = AxisLabelFont;
						chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None;
						chartArea.AxisY2.MinorGrid.Enabled = false;
						chartArea.AxisY2.MajorGrid.Enabled = false;
						var max = Math.Max(-Math.Round(grad.Min() * 2), Math.Round(grad.Max() * 2));
						//chartArea.AxisY2.si
						//chartArea.AxisY2.Minimum = -max;
						//chartArea.AxisY2.Maximum = max;
						chartArea.AxisY2.RoundAxisValues();
						chartArea.AxisY2.Interval = Math.Round(max / 5);

						if (modDataV3.Columns.Contains("Alt")) {
							var alt = LoadData(modDataV3, "Alt");
							var seriesAlt = CreateSeries("Altitude", legend, chartArea, chart, Color.Brown, x, alt);
							seriesAlt.YAxisType = AxisType.Secondary;
						}
						var seriesGrad = CreateSeries("Gradient", legend, chartArea, chart, Color.Coral, x, grad);
						seriesGrad.YAxisType = AxisType.Secondary;
					}
					if (PlotDrivingMode && yfield == Yfields.First()) {
						if (modDataV3.Columns.Contains("Action")) {
							var actionMapping = new Dictionary<string, double> {
								{ "Accelerate", 2 - 3 },
								{ "Coast", 1 - 3 },
								{ "Roll", 0 - 3 },
								{ "Brake", -1 - 3 }
							};
							var action = LoadDataMapped(modDataV3, "Action", actionMapping);
							var seriesAction = CreateSeries("Driving Action", legend, chartArea, chart, Color.Magenta, x, action);
							//seriesAction.YAxisType = AxisType.Secondary;
						}

						if (modDataV3.Columns.Contains("DrivingMode")) {
							var modeMapping = new Dictionary<string, double> {
								{ "DrivingModeDrive", -1 },
								{ "DrivingModeBrake", -6 },
							};
							var mode = LoadDataMapped(modDataV3, "DrivingMode", modeMapping);
							var seriesAction = CreateSeries("Driving Mode", legend, chartArea, chart, Color.Maroon, x, mode);
							//seriesAction.YAxisType = AxisType;
						}
					}
					if (PlotIgnitionState && yfield == ModalResultField.P_ice_out) {
						var ignition = LoadData(modDataV3, ModalResultField.ICEOn.GetShortCaption());

						chartArea.AxisY2.Enabled = AxisEnabled.True;
						chartArea.AxisY2.Title = "Engine On [0/1]";
						chartArea.AxisY2.TitleFont = AxisTitleFont;
						chartArea.AxisY2.LabelStyle.Font = AxisLabelFont;
						chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None;
						chartArea.AxisY2.MinorGrid.Enabled = false;
						chartArea.AxisY2.MajorGrid.Enabled = false;

						var seriesIgnition = CreateSeries("Engine On", legend, chartArea, chart, Color.Crimson, x, ignition);
						seriesIgnition.YAxisType = AxisType.Secondary;
					}

					var series1 = CreateSeries(string.Format("{1} - {0}", yfield, Series1Label), legend, chartArea, chart,
						Color.Blue, x, y);

					if (modDataV22 != null) {
						var y2 = LoadData(modDataV22, TranslateFieldname(yfield));
						var series2 = CreateSeries(string.Format("{1} - {0}", yfield, Series2Label), legend, chartArea, chart,
							Color.Red, x2,
							y2);
					}

					PositionChartArea(chartArea, titleHeight, i, Yfields.Length);

					if (i > 0) {
						AlignChart(chart, yfield.ToString(), Yfields[0].ToString());
					}
				}

				AddTitle(chart, Path.GetFileNameWithoutExtension(fileName), Yfields[0].ToString());

				chart.Invalidate();
				chart.SaveImage(Path.Combine(Path.GetDirectoryName(fileNameV3) ?? "", fileName), ChartImageFormat.Png);
			}
#endif
		}

		private string TranslateFieldname(ModalResultField modalResultField)
		{
			switch (modalResultField) {
				case ModalResultField.n_ice_avg:
					return "n";
				case ModalResultField.P_ice_out:
					return "Pe_eng";
				case ModalResultField.T_ice_fcmap:
					return "Tq_eng";
				case ModalResultField.P_aux_mech:
					return "Paux";
				//case ModalResultField.AA_TotalCycleFC_Grams:
				//	return "AA_TotalCycleFC_Grams []";
				default:
					return modalResultField.GetName();
			}
		}

		public bool WriteDistanceSlice(string fileNameV3, string fileNameV22, double start, double end)
		{
#if !NET5_0_OR_GREATER
			if (!_enabled) {
				return true;
			}

			var modDataV3Iput = VectoCSVFile.Read(fileNameV3);
			//var modDataV3View = new DataView(modDataV3Iput) {
			//    RowFilter = string.Format(@"dist > {0} AND dist < {1}", start, end)
			//};
			//var modDataV3 = modDataV3View.ToTable();
			var modDataV3Tmp = modDataV3Iput.Rows.Cast<DataRow>().Where(row => {
				var s = row.ParseDouble("dist");
				return s >= start && s <= end;
			}).ToList();

			if (!File.Exists(fileNameV22)) {
				//LogManager.GetCurrentClassLogger().Error("Modfile V2.2 not found: " + fileNameV22);
				//Write(fileNameV3);
				//return;
			}
			DataTable modDataV22 = null;
			if (fileNameV22 != null) {
				var modDataV22Input = VectoCSVFile.Read(fileNameV22);
				//var modDataV22View = new DataView(modDataV22Input) {
				//    RowFilter = string.Format(@"dist > {0} AND dist < {1}", start, end)
				//};
				var modDataV22Tmp = modDataV22Input.Rows.Cast<DataRow>().Where(row => {
					var s = row.ParseDouble("dist");
					return s >= start && s <= end;
				}).ToList();
				if (!(modDataV3Tmp.Any() || modDataV22Tmp.Any())) {
					return false;
				}

				var d = new DataTable();
				modDataV22Tmp.ForEach(r => d.Rows.Add(r));
				modDataV22 = d;
			} else {
				if (!modDataV3Tmp.Any()) {
					return false;
				}
			}

			var modDataV3 = new DataTable();
			modDataV3Tmp.ForEach(r => modDataV3.Rows.Add(r));

			//var xfields = new[] { ModalResultField.dist };
			var xfield = ModalResultField.dist;
			var yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_ice_avg, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.T_ice_fcmap, ModalResultField.FCMap
			};

			var titleHeight = (50 * 100.0f) / (_diagramSize.Height * yfields.Length);

			//foreach (var xfield in xfields) {
			var fileName = $"{Path.GetFileNameWithoutExtension(fileNameV3)}_{xfield.GetName()}-{(int)(start / 1000):D3}_{(int)(end / 1000):D3}.png";

			var x = LoadData(modDataV3, xfield.GetName());
			var x2 = new[] { double.NegativeInfinity };
			if (fileNameV22 != null && modDataV22 != null) {
				x2 = LoadData(modDataV22, xfield.GetName());
			}
			var plotSize = new Size(_diagramSize.Width, _diagramSize.Height * yfields.Length);
			var maxX = (int)(Math.Ceiling(Math.Max(x.Max(), x2.Max()) * 1.01 / 10.0) * 10.0);
			var minX = (int)(Math.Floor(Math.Max(x.Min(), x2.Min()) / 10.0) * 10.0);
			var chart = new Chart { Size = plotSize };

			for (var i = 0; i < yfields.Length; i++) {
				var yfield = yfields[i];
				var y = LoadData(modDataV3, yfield.GetName());

				var chartArea = AddChartArea(chart, yfield.ToString(), xfield.GetCaption(), maxX, minX,
					yfield.GetCaption(), yfield == ModalResultField.Gear);

				var legend = CreateLegend(chart, yfield.ToString());

				if (yfield == ModalResultField.v_act) {
					var y3 = LoadData(modDataV3, ModalResultField.v_targ.GetName());
					var series3 = CreateSeries("v_target", legend, chartArea, chart, Color.Green, x, y3);

					var grad = LoadData(modDataV3, ModalResultField.grad.GetName());

					chartArea.AxisY2.Enabled = AxisEnabled.True;
					chartArea.AxisY2.Title = "gradient [%]";
					chartArea.AxisY2.TitleFont = AxisTitleFont;
					chartArea.AxisY2.LabelStyle.Font = AxisLabelFont;
					chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None;
					chartArea.AxisY2.MinorGrid.Enabled = false;
					chartArea.AxisY2.MajorGrid.Enabled = false;
					var max = Math.Max(-Math.Round(grad.Min() * 2), Math.Round(grad.Max() * 2));
					//chartArea.AxisY2.si
					chartArea.AxisY2.Minimum = -max;
					chartArea.AxisY2.Maximum = max;
					chartArea.AxisY2.RoundAxisValues();

					var seriesGrad = CreateSeries("Gradient", legend, chartArea, chart, Color.Coral, x, grad);
					seriesGrad.YAxisType = AxisType.Secondary;
				}

				var series1 = CreateSeries($"Vecto 3 - {yfield}", legend, chartArea, chart,
					Color.Blue, x, y);

				if (fileNameV22 != null) {
					var y2 = LoadData(modDataV22, yfield.GetName());
					var series2 = CreateSeries($"Vecto 2.2 - {yfield}", legend, chartArea, chart,
						Color.Red, x2, y2);
				}

				PositionChartArea(chartArea, titleHeight, i, yfields.Length);

				if (i > 0) {
					AlignChart(chart, yfield.ToString(), yfields[0].ToString());
				}
				//}

				AddTitle(chart, Path.GetFileNameWithoutExtension(fileName), yfields[0].ToString());

				chart.Invalidate();
				chart.SaveImage(fileName, ChartImageFormat.Png);
			}
#endif
			return true;
		}

#if !NET5_0_OR_GREATER
		private static void AddTitle(Chart chart, string titleText, string dockToChartArea)
		{
			var title = new Title {
				Text = titleText,
				DockedToChartArea = dockToChartArea,
				IsDockedInsideChartArea = false,
				Font = new Font("Verdana", 18, FontStyle.Bold)
			};
			chart.Titles.Add(title);
		}

		private static double[] LoadData(DataTable modDataV3, string fieldA)
		{
			var field = string.Format(fieldA, "");
			return modDataV3.Rows.Cast<DataRow>()
				.Select(v => ((string)v[field]).Length == 0
					? double.NaN
					: ((string)v[field]).ToDouble())
				.ToArray();
		}

		//private static double[] LoadDataMapped(DataTable modDataV3, string field, Dictionary<string, double> mapping) =>
		//	modDataV3.Rows.Cast<DataRow>().Select(x => mapping.GetValueOrDefault(((string)x[field]), double.NaN)).ToArray();
		private static double[] LoadDataMapped(DataTable modDataV3, string field, Dictionary<string, double> mapping)
		{
			return (from x in modDataV3.Rows.Cast<DataRow>()
				let val = x.Field<string>(field)
				select mapping.ContainsKey(val) ? mapping[val] : double.NaN).ToArray();
		}

		private static void AlignChart(Chart chart, string chartToAlign, string chartToAlignWith)
		{
			chart.ChartAreas[chartToAlign].AlignWithChartArea = chartToAlignWith;
			chart.ChartAreas[chartToAlign].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
			chart.ChartAreas[chartToAlign].AlignmentStyle = AreaAlignmentStyles.All;
		}

		private static void PositionChartArea(ChartArea chartArea, float titleHeight, int i, int numCharts)
		{
			chartArea.Position.Auto = false;
			chartArea.Position.Width = 85;
			chartArea.Position.Height = (100.0f - titleHeight) / numCharts;
			chartArea.Position.X = 0;
			chartArea.Position.Y = (i * (100.0f - titleHeight)) / numCharts + titleHeight;
		}

		private ChartArea AddChartArea(Chart chart, string name, string axisXTitle, int xMax, int xMin,
			string axisYTitle, bool discreteValues)
		{
			var chartArea = new ChartArea { Name = name };
			chartArea.AxisX.MajorGrid.LineColor = Color.DarkGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.DarkGray;
			chartArea.AxisX.LabelStyle.Font = AxisLabelFont;
			chartArea.AxisY.LabelStyle.Font = AxisLabelFont;

			chartArea.AxisX.Interval = xMax / 20.0;
			chartArea.AxisX.Maximum = xMax;
			chartArea.AxisX.Minimum = xMin;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.Interval = (xMax - xMin) / 100.0;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.Title = axisXTitle;
			chartArea.AxisX.TitleFont = AxisTitleFont;
			chartArea.AxisX.RoundAxisValues();
			chartArea.AxisX.MajorTickMark.Size = 2 * 100.0f / _diagramSize.Height;

			chartArea.AxisY.Title = axisYTitle;
			chartArea.AxisY.TitleFont = AxisTitleFont;
			chartArea.AxisY.RoundAxisValues();
			if (discreteValues) {
				chartArea.AxisY.MajorGrid.Interval = 1;
				chartArea.AxisY.MinorGrid.Enabled = false;
			} else {
				chartArea.AxisY.MinorGrid.Enabled = true;
			}
			chartArea.AxisY.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorTickMark.Size = 5 * 100.0f / _diagramSize.Width;

			chart.ChartAreas.Add(chartArea);
			return chartArea;
		}

		private Legend CreateLegend(Chart chart, string dockToChartArea)
		{
			var legend = new Legend(dockToChartArea) {
				Docking = Docking.Right,
				IsDockedInsideChartArea = false,
				DockedToChartArea = dockToChartArea,
				Font = LegendFont,
			};
			chart.Legends.Add(legend);
			return legend;
		}

		private static Series CreateSeries(String name, Legend legend, ChartArea chartArea, Chart chart, Color color,
			IEnumerable<double> x, IEnumerable<double> y)
		{
			//ModalResultField yfield;
			var series1 = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = color,
				BorderWidth = 2,
				Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};

			chart.Series.Add(series1);
			chart.Series[series1.Name].Points.DataBindXY(x, y);
			return series1;
		}
#endif
	}
}