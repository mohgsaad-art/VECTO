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
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GetSectionTests
	{
		public class Entry
		{
			public readonly PerSecond EngineSpeed;
			public NewtonMeter Torque;

			public Entry(PerSecond engineSpeed, NewtonMeter torque)
			{
				EngineSpeed = engineSpeed;
				Torque = torque;
			}
		}

		[TestCase]
		public void TestGetSection()
		{
			var entries = new List<Entry>();
			for (var i = 0; i < 10; i++) {
				entries.Add(new Entry(i.RPMtoRad(), i.SI<NewtonMeter>()));
			}
			var entryArr = entries.ToArray();

			foreach (var val in new[] { -1, 0, 1, 5, 8, 9, 10 }) {
				var sw = Stopwatch.StartNew();
				var s = entries.GetSection(e => val > e.EngineSpeed);
				sw.Stop();
				//Console.WriteLine("Iterator: " + sw.Elapsed);

				sw.Restart();
				var s1 = entryArr.GetSection(e => val > e.EngineSpeed);
				sw.Stop();
				//Console.WriteLine("Array:    " + sw.Elapsed);

				Assert.AreSame(s.Item1, s1.Item1);
				Assert.AreSame(s.Item2, s1.Item2);
			}

			foreach (var val in new[] { -1, 0, 1, 5, 8, 9, 10 }) {
				var sw = Stopwatch.StartNew();
				var s = entries.GetSection(e => val < e.EngineSpeed);
				sw.Stop();
				//Console.WriteLine("Iterator: " + sw.Elapsed);

				sw.Restart();
				var s1 = entryArr.GetSection(e => val < e.EngineSpeed);
				sw.Stop();
				//Console.WriteLine("Array:    " + sw.Elapsed);

				Assert.AreSame(s.Item1, s1.Item1);
				Assert.AreSame(s.Item2, s1.Item2);
			}
		}
	}
}