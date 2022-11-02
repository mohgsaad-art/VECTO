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

using System.Diagnostics.CodeAnalysis;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class SimpleDrivingCycles
	{
		public static DrivingCycleData CreateCycleData(string[] entries)
		{
			var cycleData = InputDataHelper.InputDataAsStream("<s>,<v>,<grad>,<stop>", entries);
			return DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);
		}

		public static DrivingCycleData CreateCycleData(string entries)
		{
			var cycleData = InputDataHelper.InputDataAsStream("<s>,<v>,<grad>,<stop>", entries.Split('\n'));
			return DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);
		}

		#region Misc

		public const string CycleDrive_80_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80, 0,    0
			 100,  80, 0.25, 0
			 200,  80, 0.5,  0
			 300,  80, 0.75, 0
			 400,  80, 1,    0
			 500,  80, 1.25, 0
			 600,  80, 1.5,  0
			 700,  80, 1.75, 0
			 800,  80, 2,    0
			 900,  80, 2.25, 0
			1000,  80, 2.5,  0
			1100,  80, 0,    0";

		public const string CycleDrive_50_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  50, 0,    0
			 100,  50, 0.25, 0
			 200,  50, 0.5,  0
			 300,  50, 0.75, 0
			 400,  50, 1,    0
			 500,  50, 1.25, 0
			 600,  50, 1.5,  0
			 700,  50, 1.75, 0
			 800,  50, 2,    0
			 900,  50, 2.25, 0
			1000,  50, 2.5,  0
			1100,  50, 0,    0";

		public const string CycleDrive_30_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  30, 0,    0
			 100,  30, 0.25, 0
			 200,  30, 0.5,  0
			 300,  30, 0.75, 0
			 400,  30, 1,    0
			 500,  30, 1.25, 0
			 600,  30, 1.5,  0
			 700,  30, 1.75, 0
			 800,  30, 2,    0
			 900,  30, 2.25, 0
			1000,  30, 2.5,  0
			1100,  30, 0,    0";

		public const string CycleDrive_80_Decreasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80,  0,    0
			 100,  80, -0.25, 0
			 200,  80, -0.5,  0
			 300,  80, -0.75, 0
			 400,  80, -1,    0
			 500,  80, -1.25, 0
			 600,  80, -1.5,  0
			 700,  80, -1.75, 0
			 800,  80, -2,    0
			 900,  80, -2.25, 0
			1000,  80, -2.5,  0
			1100,  80,  0,    0";

		public const string CycleDrive_50_Decreasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  50,  0,    0
			 100,  50, -0.25, 0
			 200,  50, -0.5,  0
			 300,  50, -0.75, 0
			 400,  50, -1,    0
			 500,  50, -1.25, 0
			 600,  50, -1.5,  0
			 700,  50, -1.75, 0
			 800,  50, -2,    0
			 900,  50, -2.25, 0
			1000,  50, -2.5,  0
			1100,  50,  0,    0  ";

		public const string CycleDrive_30_Decreasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  30,  0,    0
			 100,  30, -0.25, 0
			 200,  30, -0.5,  0
			 300,  30, -0.75, 0
			 400,  30, -1,    0
			 500,  30, -1.25, 0
			 600,  30, -1.5,  0
			 700,  30, -1.75, 0
			 800,  30, -2,    0
			 900,  30, -2.25, 0
			1000,  30, -2.5,  0
			1100,  30,  0,    0	";

		public const string CycleDrive_80_Dec_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80,  0,    0
			 100,  80, -0.25, 0
			 200,  80, -0.5,  0
			 300,  80, -0.75, 0
			 400,  80, -1,    0
			 500,  80, -1.25, 0
			 600,  80, -1.5,  0
			 700,  80, -1.75, 0
			 800,  80, -2,    0
			 900,  80, -2.25, 0
			1000,  80, -2.5,  0
			1100,  80,  0,    0
			1200,  80, 0,    0
			1300,  80, 0.25, 0
			1400,  80, 0.5,  0
			1500,  80, 0.75, 0
			1600,  80, 1,    0
			1700,  80, 1.25, 0
			1800,  80, 1.5,  0
			1900,  80, 1.75, 0
			2000,  80, 2,    0
			2100,  80, 2.25, 0
			2200,  80, 2.5,  0
			2300,  80, 0,    0		";

		public const string CycleDrive_50_Dec_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  50,  0,    0
			  50,  50, -0.25, 0
			 100,  50, -0.5,  0
			 150,  50, -0.75, 0
			 200,  50, -1,    0
			 250,  50, -1.25, 0
			 300,  50, -1.5,  0
			 350,  50, -1.75, 0
			 400,  50, -2,    0
			 450,  50, -2.25, 0
			 500,  50, -2.5,  0
			 550,  50,  0,    0
			 600,  50, 0,    0
			 650,  50, 0.25, 0
			 700,  50, 0.5,  0
			 750,  50, 0.75, 0
			 800,  50, 1,    0
			 850,  50, 1.25, 0
			 900,  50, 1.5,  0
			 950,  50, 1.75, 0
			1000,  50, 2,    0
			1050,  50, 2.25, 0
			1100,  50, 2.5,  0
			1150,  50, 5,    0
			1200,  50, 0,    0	  ";

		public const string CycleDrive_30_Dec_Increasing_Slope =
			// <s>,<v>,<grad>,<stop>
			@"   0,  30,  0,    0
			  50,  30, -0.25, 0
			 100,  30, -0.5,  0
			 150,  30, -0.75, 0
			 200,  30, -1,    0
			 250,  30, -1.25, 0
			 300,  30, -1.5,  0
			 350,  30, -1.75, 0
			 400,  30, -2,    0
			 450,  30, -2.25, 0
			 500,  30, -2.5,  0
			 550,  30,  0,    0
			 600,  30, 0,    0
			 650,  30, 0.25, 0
			 700,  30, 0.5,  0
			 750,  30, 0.75, 0
			 800,  30, 1,    0
			 850,  30, 1.25, 0
			 900,  30, 1.5,  0
			 950,  30, 1.75, 0
			1000,  30, 2,    0
			1050,  30, 2.25, 0
			1100,  30, 2.5,  0
			1150,  30, 0,    0";

		public const string CycleDecelerateWhileBrake_80_0_level =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80, 0,    0
			 990,  65, 0,    0
			1000,   0, 0,    2";

		public const string CycleAccelerateWhileBrake_80_0_level =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80, 0,    0
				 800,  90, 0,    0
				 950,  80, 0,    0
				1000,   0, 0,    2";

		public const string CycleAccelerateAtBrake_80_0_level =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80, 0,    0
				 505,  90, 0,    0
				 650,  80, 0,    0
				1000,   0, 0,    2";

		public const string CycleAccelerateBeforeBrake_80_0_level =
			// <s>,<v>,<grad>,<stop>
			@"   0,  80, 0,    0
			 450,  90, 0,    0
			 650,  80, 0,    0
			1000,   0, 0,    2";

		public const string CycleDrive_stop_85_stop_85_level =
			// <s>,<v>,<grad>,<stop>
			@"  0,   0, 0,     2
			1000, 85, 0,     0
			2000,   0, 0,     2
			3000, 85, 0,     0";

		public const string CycleDrive_SlopeChangeBeforeStop =
			// <s>,<v>,<grad>,<stop>
			@"  0,  60, -1.4,     0
				298,  60, -1.7,     0
			    300,   0, -1.7,     4";

		public const string CycleDriver_FrequentSlopChange =
			// <s>,<v>,<grad>,<stop>
			@"  0,  60,  0,     0
				 10,  60, -6,     0
				100,  55, -6,     0
				300,  55, -6,     0";

		#endregion

		public static DrivingCycleData ReadDeclarationCycle(string missionType)
		{
			var cycleData =
				RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." + missionType +
											Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);
			return cycle;
		}
	}
}