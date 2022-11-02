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
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public class MeanShiftClustering
	{
		
		public int MaxIterations { get; set; }

		public double UpdateTolerance { get; set; }

		public int ClusterCount { get; set; }

		public MeanShiftClustering()
		{
			MaxIterations = 20;
			UpdateTolerance = 1e-3;
			ClusterCount = 50;
		}

		public double[] FindClusters(double[] numbers, double clusterTolerance)
		{
			var min = numbers.Min();
			var max = numbers.Max();
			var clusterDistance = (max - min) / ClusterCount;
			var clusters = new Cluster[ClusterCount];
			for (var i = 0; i < ClusterCount; i++) {
				clusters[i] = new Cluster(min + (i + 0.5) * clusterDistance);
			}

			IterationCount = 0;
			var updated = true;
			while (IterationCount++ < MaxIterations && updated) {
				for (var clusterIdx = 0; clusterIdx < ClusterCount; clusterIdx++) {
					var minDist = double.MaxValue;
					var minIdx = -1;
					for (var valueIdx = 0; valueIdx < numbers.Length; valueIdx++) {
						if (clusters[clusterIdx].Distance(numbers[valueIdx]) < minDist) {
							minDist = clusters[clusterIdx].Distance(numbers[valueIdx]);
							minIdx = valueIdx;
						}
					}

					if (minIdx >= 0) {
						clusters[clusterIdx].AddValue(numbers[minIdx]);
					}
				}

				updated = false;
				for (int i = 0; i < ClusterCount; i++) {
					updated |= clusters[i].Update(UpdateTolerance);
				}
			}

			return clusters.Select(c => c.Center).Distinct(new ClusterComparer(clusterTolerance)).ToArray();
		}

		public int IterationCount { get; protected set; }

		public struct Cluster
		{
			private int _count;
			private double _sum;

			public Cluster(double center)
			{
				Center = center;
				_sum = 0.0;
				_count = 0;
			}

			public double Center { get; private set; }

			public double Distance(double val)
			{
				return Math.Abs(Center - val);
			}

			public void AddValue(double val)
			{
				_sum += val;
				_count++;
			}

			public double? Mean => _count > 0 ? _sum / _count : (double?)null;

			public override string ToString() => $"[{Center},{Mean}]";

			public bool Update(double tolerance)
			{
				if (Mean == null)
					return false;

				var retVal = Math.Abs(Center - Mean.Value) > tolerance;
				Center = Mean.Value;
				_count = 0;
				_sum = 0;
				return retVal;
			}
		}
	}

	public class ClusterComparer : IEqualityComparer<double>
	{
		protected readonly double Tolerance;

		public ClusterComparer(double updateTolerance)
		{
			Tolerance = updateTolerance;
		}

		#region Implementation of IEqualityComparer<in double>

		public bool Equals(double x, double y)
		{
			return x.IsEqual(y, Tolerance);
		}

		public int GetHashCode(double obj)
		{
			return 0;
		}

		#endregion
	}
}
