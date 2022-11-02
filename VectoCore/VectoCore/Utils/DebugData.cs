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
using System.Diagnostics;
using System.Threading;

namespace TUGraz.VectoCore.Utils
{
	public class DebugData
	{
		private const int Capacity = 128;

		private static readonly ThreadLocal<Queue<dynamic>> _data = new ThreadLocal<Queue<dynamic>>(
			() => new Queue<dynamic>(Capacity));

		internal static Queue<dynamic> GlobalData => _data.Value;

		internal Queue<dynamic> LocalData { get; } = new Queue<dynamic>();

		[Conditional("DEBUG")]
		public static void Clear() => GlobalData.Clear();
		
		[Conditional("DEBUG")]
		public void Add(dynamic value)
		{
			while (GlobalData.Count >= Capacity) {
				GlobalData.Dequeue();
			}
			GlobalData.Enqueue(value);

			LocalData.Enqueue(value);
		}

		[Conditional("DEBUG")]
		public void Add(dynamic a, dynamic b) => Add(new { a, b });

		[Conditional("DEBUG")]
		public void Add(dynamic a, dynamic b, dynamic c) => Add(new { a, b, c });

		[Conditional("DEBUG")]
		public void Add(dynamic a, dynamic b, dynamic c, dynamic d) => Add(new { a, b, c, d });
	}
}