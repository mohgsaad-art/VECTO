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
using System.Drawing;
using System.Linq;
#if !NET5_0_OR_GREATER
using System.Windows.Forms.DataVisualization.Charting;
#endif
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using Point = TUGraz.VectoCommon.Utils.Point;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class ShiftPolygonDrawer
	{
#if !NET5_0_OR_GREATER
		private static readonly Font AxisLabelFont = new Font("Consolas", 10);
		private static readonly Font AxisTitleFont = new Font("Verdana", 12);
		private static readonly Font LegendFont = new Font("Verdana", 14);
#endif
		private static Size _diagramSize = new Size(1000, 800);


		public static void DrawShiftPolygons(string title, Dictionary<uint, EngineFullLoadCurve> engineFld,
			List<ShiftPolygon> polygons,
			string imageFileName, PerSecond speed85kmh, List<List<Point>> upshiftOrig = null,
			List<List<Point>> downshiftTransformed = null, List<List<Point>> downshiftOrig = null)
		{
#if !NET5_0_OR_GREATER
			var numRows = Math.Ceiling(polygons.Count / 4.0);
			var numCols = Math.Ceiling(polygons.Count / numRows);

			var chart = new Chart() {
				Size = new Size((int)(_diagramSize.Width * numCols), (int)(_diagramSize.Height * numRows))
			};
			var maxX = engineFld[0].FullLoadEntries.Last().EngineSpeed.Value() / Constants.RPMToRad * 1.1;

			AddLegend(chart);

			var i = 0;
			foreach (var shiftPolygon in polygons) {
				var chartArea = AddChartArea(chart, "Gear " + (i + 1), "Engine Speed", "Torque", 0, maxX);

				PlotPower(engineFld[(uint)(i + 1)], chartArea, chart, "engine power " + i);
				PlotFLD(engineFld[(uint)(i + 1)], speed85kmh, chartArea, chart, "Engine Full Load " + i);

				if (upshiftOrig != null && i < upshiftOrig.Count) {
					PlotShiftLine("UpshiftOrig " + i, chartArea, chart, Color.Gray,
						upshiftOrig[i].Select(pt => pt.X / Constants.RPMToRad).ToList(),
						upshiftOrig[i].Select(pt => pt.Y).ToList(), true);
				}
				if (downshiftTransformed != null && i < downshiftTransformed.Count) {
					PlotShiftLine("DownTransformed " + i, chartArea, chart, Color.BlueViolet,
						downshiftTransformed[i].Select(pt => pt.X / Constants.RPMToRad).ToList(),
						downshiftTransformed[i].Select(pt => pt.Y).ToList(), true);
				}
				if (downshiftOrig != null && i < downshiftOrig.Count) {
					PlotShiftLine("DownshiftOrig" + i, chartArea, chart, Color.Gray,
						downshiftOrig[i].Select(pt => pt.X / Constants.RPMToRad).ToList(),
						downshiftOrig[i].Select(pt => pt.Y).ToList(), true);
				}

				PlotShiftPolygon(i, shiftPolygon, chartArea, chart);

				PositionChartArea(chartArea, 5, i, polygons.Count, 7);


				i++;
			}

			AddTitle(chart, title);
			chart.Invalidate();
			chart.SaveImage(imageFileName, ChartImageFormat.Png);
#endif
		}

#if !NET5_0_OR_GREATER
		private static void PlotPower(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkGoldenrod,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = false,
				ChartArea = chartArea.Name,
				//YAxisType = AxisType.Secondary
			};
			series.BorderDashStyle = ChartDashStyle.Dash;
			series.BorderWidth = 1;

			var x = new List<double>();
			var y = new List<double>();
			for (var i = engineFld.FullLoadEntries.First().EngineSpeed;
				i < engineFld.FullLoadEntries.Last().EngineSpeed;
				i += 5.RPMtoRad()) {
				x.Add(i.Value() / Constants.RPMToRad);
				y.Add(engineFld.FullLoadStationaryPower(i).Value() / 1000);
			}

			chart.Series.Add(series);
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static void AddTitle(Chart chart, string titleText)
		{
			var title = new Title {
				Text = titleText,
				IsDockedInsideChartArea = false,
				Font = new Font("Verdana", 18, FontStyle.Bold)
			};
			//title.DockedToChartArea = dockToChartArea;
			chart.Titles.Add(title);
		}

		private static void PositionChartArea(ChartArea chartArea, float titleHeight, int i, int count, float legendHeight)
		{
			var numRows = Math.Ceiling(count / 4.0);
			var numCols = Math.Ceiling(count / numRows);
			chartArea.Position.Auto = false;
			chartArea.Position.Width = (float)((100.0f) / numCols);
			chartArea.Position.Height = (float)((100.0f - titleHeight) / numRows);
			chartArea.Position.X = (float)(((i) % numCols) * 100.0f / numCols);
			chartArea.Position.Y = (float)(titleHeight + (int)((i) / numCols) * (100 - titleHeight - legendHeight) / numRows);
		}

		private static void PlotShiftPolygon(int gear, ShiftPolygon shiftPolygon, ChartArea chartArea, Chart chart)
		{
			var x = new List<double>();
			var y = new List<double>();
			foreach (var entry in shiftPolygon.Downshift) {
				x.Add(entry.AngularSpeed.Value() / Constants.RPMToRad);
				y.Add(entry.Torque.Value());
			}
			PlotShiftLine("DownShift " + gear, chartArea, chart, Color.DarkRed, x, y);


			var xUp = new List<double>();
			var yUp = new List<double>();
			foreach (var entry in shiftPolygon.Upshift) {
				xUp.Add(entry.AngularSpeed.Value() / Constants.RPMToRad);
				yUp.Add(entry.Torque.Value());
			}
			PlotShiftLine("UpShift " + gear, chartArea, chart, Color.DarkRed, xUp, yUp);


			//return series;
		}

		private static void PlotShiftLine(string name, ChartArea chartArea, Chart chart, Color color, List<double> x,
			List<double> y, bool dashed = false)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = color,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = false,
				ChartArea = chartArea.Name,
			};
			if (dashed) {
				series.BorderDashStyle = ChartDashStyle.Dash;
				series.BorderWidth = 1;
			}

			chart.Series.Add(series);
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static void PlotFLD(EngineFullLoadCurve engineFld, PerSecond speed85kmh, ChartArea chartArea, Chart chart,
			string name)
		{
			PlotFullLoad(engineFld, chartArea, chart, name);

			PlotDragLoad(engineFld, chartArea, chart, name);

			//PlotEngineSpeedLine(engineFld, chartArea, chart, "nPref " + name, Color.DeepSkyBlue,
			//	engineFld.PreferredSpeed.Value() / Constants.RPMToRad);

			//PlotEngineSpeedLine(engineFld, chartArea, chart, "n95h " + name, Color.Red,
			//	engineFld.N95hSpeed.Value() / Constants.RPMToRad);

			//PlotEngineSpeedLine(engineFld, chartArea, chart, "n85kmh " + name, Color.LimeGreen,
			//	speed85kmh.Value() / Constants.RPMToRad);

			PlotEngineSpeedLine(engineFld, chartArea, chart, "nPmax " + name, Color.Coral,
				engineFld.RatedSpeed.Value() / Constants.RPMToRad);
		}


		private static void PlotEngineSpeedLine(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name,
			Color color, double n)
		{
			var seriesNpref = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = color,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = false,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(seriesNpref);
			var xpref =
				new[]
				{ n, n }
					.ToList();
			var ypref = new[] { engineFld.MaxDragTorque.Value(), engineFld.MaxTorque.Value() }.ToList();
			chart.Series[seriesNpref.Name].Points.DataBindXY(xpref, ypref);
		}

		private static void PlotDragLoad(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var seriesDrag = new Series {
				Name = "Drag-" + name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkBlue,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = false,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(seriesDrag);
			var xDrag = new List<double>();
			var yDrag = new List<double>();
			engineFld.FullLoadEntries.ForEach(entry => {
				xDrag.Add(entry.EngineSpeed.Value() / Constants.RPMToRad);
				yDrag.Add(entry.TorqueDrag.Value());
			});
			chart.Series[seriesDrag.Name].Points.DataBindXY(xDrag, yDrag);
		}

		private static void PlotFullLoad(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkBlue,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = false,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(series);
			var x = new List<double>();
			var y = new List<double>();
			engineFld.FullLoadEntries.ForEach(entry => {
				x.Add(entry.EngineSpeed.Value() / Constants.RPMToRad);
				y.Add(entry.TorqueFullLoad.Value());
			});
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static ChartArea AddChartArea(Chart chart, string name, string axisXTitle, string axisYTitle, double xMin,
			double xMax, bool discreteValues = false)
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

		private static void AddLegend(Chart chart)
		{
			var legend = new Legend() {
				Docking = Docking.Bottom,
				Alignment = StringAlignment.Center,
				IsDockedInsideChartArea = false,
				Font = LegendFont,
				//Title = "Legend",
			};
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.DarkBlue,
				Name = "Engine Full Load Curve",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.DarkRed,
				Name = "Upshift / Downshift",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.DeepSkyBlue,
				Name = "n_pref",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.Coral,
				Name = "n_Pmax",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.Red,
				Name = "n_95h",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.LimeGreen,
				Name = "n_85km/h",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.BlueViolet,
				Name = "Downshift next gear",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.Gray,
				Name = "Downshift/Upshift orig.",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			legend.CustomItems.Add(new LegendItem() {
				Color = Color.DarkGoldenrod,
				Name = "P",
				MarkerStyle = MarkerStyle.None,
				ImageStyle = LegendImageStyle.Line,
				BorderWidth = 3,
			});
			chart.Legends.Add(legend);
		}
#endif
	}
}