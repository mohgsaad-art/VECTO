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
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using Point = TUGraz.VectoCommon.Utils.Point;
#if !NET5_0_OR_GREATER
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
#endif

namespace TUGraz.VectoCore.Utils
{
	public sealed class DelaunayMap : LoggingObject
	{
		private ICollection<Point> _points = new HashSet<Point>();
		private Triangle[] _triangles;
		private Edge[] _convexHull;

		private readonly string _mapName;
		private double _minY;
		private double _minX;
		private double _maxY;
		private double _maxX;

		public DelaunayMap(string name)
		{
			_mapName = name;
		}

		public string Name => _mapName;

		public void AddPoint(double x, double y, double z)
		{
			_points.Add(new Point(x, y, z));
		}

		public IReadOnlyCollection<Point> Entries
		{
			get {
				var retVal = new Point[_points.Count];
				var i = 0;
				foreach (var pt in _points) {
					retVal[i++] = new Point(pt.X * (_maxX - _minX) + _minX, pt.Y * (_maxY - _minY) + _minY, pt.Z);
				}
				return retVal;
			}
		}

		/// <summary>
		/// Triangulate the points.
		/// </summary>
		/// <remarks>
		/// Triangulation with the Bowyer-Watson algorithm (iteratively insert points into a super triangle).
		/// https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
		/// </remarks>
		public void Triangulate()
		{
			if (_points.Count < 3) {
				throw new ArgumentException($"{_mapName}: Triangulation needs at least 3 Points. Got {_points.Count} Points.");
			}

			SanitycheckInputPoints();

			// The "supertriangle" encompasses all triangulation points.
			// This is just a helper triangle which initializes the algorithm and will be removed in the end of the algorithm.
			_maxX = _points.Max(p => p.X);
			_maxY = _points.Max(p => p.Y);
			_minX = _points.Min(p => p.X);
			_minY = _points.Min(p => p.Y);
			_points =
				_points.Select(p => new Point((p.X - _minX) / (_maxX - _minX), (p.Y - _minY) / (_maxY - _minY), p.Z)).ToList();
			var superTriangle = new Triangle(new Point(-1, -1), new Point(4, -1), new Point(-1, 4));
			var triangles = new List<Triangle> { superTriangle };

			var pointCount = 0;

			var points = _points.ToArray();

			// iteratively add each point into the correct triangle and split up the triangle
			foreach (var point in points) {
				// If the vertex lies inside the circumcircle of a triangle, the edges of this triangle are 
				// added to the edge buffer and the triangle is removed from list.
				// Remove duplicate edges. This leaves the convex hull of the edges.
				// The edges in this convex hull are oriented counterclockwise!

				var newTriangles = triangles.Select((t, i) => Tuple.Create(i, t, t.ContainsInCircumcircle(point)))
					.Where(t => t.Item3)
					.Reverse()
					.SelectMany(t => {
						triangles.RemoveAt(t.Item1);
						return t.Item2.GetEdges();
					})
					.GroupBy(edge => edge)
					.Where(group => group.Count() == 1)
					.Select(group => new Triangle(group.Key.P1, group.Key.P2, point)).ToList();

				triangles.AddRange(newTriangles);

				//DrawGraph(pointCount, triangles, superTriangle, xmin, xmax, ymin, ymax, point);
				pointCount++;

				// check invariant: m = 2n-2-k
				// m...triangle count
				// n...point count (pointCount +3 points on the supertriangle)
				// k...points on convex hull (exactly 3 --> supertriangle)
				if (triangles.Count != 2 * (pointCount + 3) - 2 - 3) {
					throw new VectoException(
						"{0} Delaunay-Triangulation invariant violated! Triangle count and point count doesn't fit together.", _mapName);
				}
			}

#if TRACE
			DrawGraph(pointCount, triangles, superTriangle, points);
#endif
			_convexHull = triangles.FindAll(t => t.SharesVertexWith(superTriangle)).
				SelectMany(t => t.GetEdges()).
				Where(e => !(superTriangle.Contains(e.P1) || superTriangle.Contains(e.P2))).ToArray();

			_triangles = triangles.FindAll(t => !t.SharesVertexWith(superTriangle)).ToArray();
		}

		private void SanitycheckInputPoints()
		{
			var duplicates = _points.GroupBy(pt => new { pt.X, pt.Y }, x => x).Where(g => g.Count() > 1).ToList();

			foreach (var duplicate in duplicates) {
				Log.Error("{0}: Input Point appears twice: x: {1}, y: {2}", duplicate.Key.X, duplicate.Key.Y);
			}
			if (duplicates.Any()) {
				throw new VectoException("{0}: Input Data for Delaunay map contains duplicates! \n{1}", _mapName,
					duplicates.Select(pt => $"{pt.Key.X} / {pt.Key.Y}").Join("\n"));
			}
		}

		public void DrawGraph()
		{
			var superTriangle = new Triangle(new Point(-1, -1), new Point(4, -1), new Point(-1, 4));
			DrawGraph(0, _triangles, superTriangle, _points.ToArray());
		}

		/// <summary>
		/// Draws the delaunay map (except supertriangle).
		/// </summary>
		private static void DrawGraph(int i, IEnumerable<Triangle> triangles, Triangle superTriangle, Point[] points,
			Point lastPoint = null)
		{
#if !NET5_0_OR_GREATER
			var xmin = Math.Min(points.Min(p => p.X), lastPoint?.X ?? double.NaN);
			var xmax = Math.Max(points.Max(p => p.X), lastPoint?.X ?? double.NaN);
			var ymin = Math.Min(points.Min(p => p.Y), lastPoint?.Y ?? double.NaN);
			var ymax = Math.Max(points.Max(p => p.Y), lastPoint?.Y ?? double.NaN);


			using (var chart = new Chart()) {
				chart.Size = new Size(1000, 1000);
				chart.ChartAreas.Add(new ChartArea("main") {
					AxisX = new Axis { Minimum = Math.Min(xmin, xmin), Maximum = Math.Max(xmax, xmax) },
					AxisY = new Axis { Minimum = Math.Min(ymin, ymin), Maximum = Math.Max(ymax, ymax) }
				});

				foreach (var tr in triangles) {
					if (tr.SharesVertexWith(superTriangle)) {
						continue;
					}

					var series = new Series {
						ChartType = SeriesChartType.FastLine,
						Color = lastPoint != null && tr.Contains(lastPoint) ? Color.Red : Color.Blue
					};
					series.Points.AddXY(tr.P1.X, tr.P1.Y);
					series.Points.AddXY(tr.P2.X, tr.P2.Y);
					series.Points.AddXY(tr.P3.X, tr.P3.Y);
					series.Points.AddXY(tr.P1.X, tr.P1.Y);
					chart.Series.Add(series);
				}

				if (lastPoint != null) {
					var series = new Series {
						ChartType = SeriesChartType.Point,
						Color = Color.Red,
						MarkerSize = 5,
						MarkerStyle = MarkerStyle.Circle
					};
					series.Points.AddXY(lastPoint.X, lastPoint.Y);
					chart.Series.Add(series);
				}

				var frame = new StackFrame(2);
				var method = frame.GetMethod();
				System.Diagnostics.Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
				var type = method.DeclaringType.Name.Split(Path.GetInvalidFileNameChars()).Join("");
				var methodName = method.Name.Split(Path.GetInvalidFileNameChars()).Join("");
				Directory.CreateDirectory("delaunay");
				chart.SaveImage($"delaunay\\{type}_{methodName}_{superTriangle.GetHashCode()}_{i}.png",
					ChartImageFormat.Png);
			}
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Interpolate(SI x, SI y)
		{
			return Interpolate(x.Value(), y.Value());
		}

		/// <summary>
		/// Interpolates the value of an point in the delaunay map.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>a value if interpolation is successfull, 
		///          null if interpolation has failed.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Interpolate(double x, double y)
		{
			if (_triangles == null) {
				throw new VectoException("Interpolation not possible. Call DelaunayMap.Triangulate first.");
			}

			x = (x - _minX) / (_maxX - _minX);
			y = (y - _minY) / (_maxY - _minY);

			var i = 0;
			while (i < _triangles.Length && !_triangles[i].IsInside(x, y, true)) {
				i++;
			}
			if (i == _triangles.Length) {
				i = 0;
				while (i < _triangles.Length && !_triangles[i].IsInside(x, y, false)) {
					i++;
				}
			}

			if (i == _triangles.Length) {
				return double.NaN;
			}

			var tr = _triangles[i];
			var plane = new Plane(tr);
			return (plane.W - plane.X * x - plane.Y * y) / plane.Z;
		}

		public double Extrapolate(SI x, SI y)
		{
			return Extrapolate(x.Value(), y.Value());
		}

		/// <summary>
		/// Extrapolates the value of an point on the edges of a delaunay map.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public double Extrapolate(double x, double y)
		{
			x = (x - _minX) / (_maxX - _minX);
			y = (y - _minY) / (_maxY - _minY);
			var point = new Point(x, y);

			// get nearest point on convex hull
			var nearestPoint = _convexHull.Select(e => e.P1).MinBy(p => Math.Pow(p.X - x, 2) + Math.Pow(p.Y - y, 2));

			// test if point is on left side of the perpendicular vector (to x,y coordinates) of edge1 in the nearest point
			//                            ^
			//                 (point)    |
			//                            |
			// (p1)--edge1-->(nearestPoint)
			var edge1 = _convexHull.First(e => e.P2.Equals(nearestPoint));
			if (point.IsLeftOf(new Edge(nearestPoint, edge1.Vector.Perpendicular() + nearestPoint))) {
				return ExtrapolateOnEdge(x, y, edge1);
			}

			// test if point is on right side of the perpendicular vector of edge2 in the nearest point
			// ^
			// |   (point)
			// |        
			// (nearestPoint)--edge2-->(p2)
			var edge2 = _convexHull.First(e => e.P1.Equals(nearestPoint));
			if (!point.IsLeftOf(new Edge(nearestPoint, edge2.Vector.Perpendicular() + nearestPoint))) {
				return ExtrapolateOnEdge(x, y, edge2);
			}

			// if point is right of perpendicular vector of edge1 and left of perpendicular vector of edge2: take the nearest point z-value
			return nearestPoint.Z;
		}

		/// <summary>
		/// Constant z-axis-extrapolation of a point from a line
		/// </summary>
		/// <remarks>
		/// https://en.wikibooks.org/wiki/Linear_Algebra/Orthogonal_Projection_Onto_a_Line
		/// </remarks>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="edge"></param>
		/// <returns></returns>
		private static double ExtrapolateOnEdge(double x, double y, Edge edge)
		{
			// shortcut if edge end points have same Z values
			if (edge.P1.Z.IsEqual(edge.P2.Z)) {
				return edge.P1.Z;
			}

			// 2d vector of the edge:  A--->B
			var ab = new Point(edge.Vector.X, edge.Vector.Y);

			// 2d vector of the point: A---->P
			var ap = new Point(x - edge.P1.X, y - edge.P1.Y);

			// projection of point (x,y) onto the edge
			var z = edge.P1.Z + edge.Vector.Z * (ap.Dot(ab) / ab.Dot(ab));
			return z;
		}
	}
}