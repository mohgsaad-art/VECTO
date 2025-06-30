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

using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class LACDecisionFactorTest
	{
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void LAC_DF_Test()
		{
			for (var vVehicle = 0.SI<MeterPerSecond>();
				vVehicle < 100.KMPHtoMeterPerSecond();
				vVehicle += 1.KMPHtoMeterPerSecond()) {
				for (var vTarget = vVehicle; vTarget > 0; vTarget -= 1.KMPHtoMeterPerSecond()) {
					var df_coast = new LACDecisionFactor().Lookup(vTarget, vVehicle - vTarget);
					if (vTarget < 48.KMPHtoMeterPerSecond()) {
						AssertHelper.AreRelativeEqual(df_coast, 2.5, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
					}

					if (vVehicle - vTarget > 11) {
						AssertHelper.AreRelativeEqual(df_coast, 2.5, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
					}

					if (vTarget > 52.KMPHtoMeterPerSecond() && vVehicle - vTarget < 9.KMPHtoMeterPerSecond()) {
						AssertHelper.AreRelativeEqual(df_coast, 1.0, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
					}
				}
			}
		}
	}
}