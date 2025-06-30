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
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PTOIdleLossTest
	{
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PTOLossMapCaseSensitiveTest()
		{
			var data = new[] {
				"0, 0",
				"10, 100"
			};
			var tbl = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("pto torque, engine speed", data));

			var pto = PTOIdleLossMapReader.Create(tbl);

			var loss = pto.GetTorqueLoss(100.RPMtoRad());
			Assert.AreEqual(10, loss.Value());
		}


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PTOIdleLosses_FixPoints()
		{
			var entryList = new List<PTOLossMap.Entry>();
			for (var i = 0; i < 2000; i += 200) {
				entryList.Add(new PTOLossMap.Entry {
					EngineSpeed = i.RPMtoRad(),
					PTOTorque = (Math.Sqrt(i) / 10).SI<NewtonMeter>()
				});
			}
			var pto = new PTOLossMap(entryList.ToArray());

			foreach (var entry in entryList) {
				Assert.AreEqual(entry.PTOTorque, pto.GetTorqueLoss(entry.EngineSpeed));
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PTOIdleLosses_Interpolate()
		{
			var entryList = new List<PTOLossMap.Entry>();
			for (var i = 0; i < 2000; i += 200) {
				entryList.Add(new PTOLossMap.Entry {
					EngineSpeed = i.RPMtoRad(),
					PTOTorque = (Math.Sqrt(i) / 10).SI<NewtonMeter>()
				});
			}
			var pto = new PTOLossMap(entryList.ToArray());

			for (var i = 1; i < entryList.Count; i++) {
				var v1 = entryList[i - 1];
				var v2 = entryList[i];

				for (var f = v1.EngineSpeed; f < v2.EngineSpeed; f += 10.RPMtoRad()) {
					AssertHelper.AreRelativeEqual(
						VectoMath.Interpolate(v1.EngineSpeed, v2.EngineSpeed, v1.PTOTorque, v2.PTOTorque, f),
						pto.GetTorqueLoss(f));
				}
			}
		}
	}
}