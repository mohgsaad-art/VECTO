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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockRunData : VectoRunData
	{
		public MockRunData()
		{
			JobName = "MockJob";
			Retarder = new RetarderData() { Type = RetarderType.None };
			VehicleData = new VehicleData() {
				CurbMass = 0.SI<Kilogram>(),
				BodyAndTrailerMass = 0.SI<Kilogram>(),
				CargoVolume = 0.SI<CubicMeter>(),
				Loading = 0.SI<Kilogram>(),
				TotalRollResistanceCoefficient = 0,
				DynamicTyreRadius = 1.SI<Meter>(),
			};
			AirdragData = new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(1.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(1.SI<SquareMeter>()), CrossWindCorrectionMode.NoCorrection)
			};
			Cycle = new DrivingCycleData() {
				Name = "MockCycle",
			};
			EngineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
				Displacement = 0.SI<CubicMeter>(),
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
					{
						0, new EngineFullLoadCurve(
							new List<EngineFullLoadCurve.FullLoadCurveEntry>() {
								new EngineFullLoadCurve.FullLoadCurveEntry() {
									EngineSpeed = 600.RPMtoRad(),
									TorqueDrag = -100.SI<NewtonMeter>(),
									TorqueFullLoad = 500.SI<NewtonMeter>()
								},
								new EngineFullLoadCurve.FullLoadCurveEntry() {
									EngineSpeed = 1800.RPMtoRad(),
									TorqueDrag = -120.SI<NewtonMeter>(),
									TorqueFullLoad = 1200.SI<NewtonMeter>()
								},
								new EngineFullLoadCurve.FullLoadCurveEntry() {
									EngineSpeed = 2500.RPMtoRad(),
									TorqueDrag = -150.SI<NewtonMeter>(),
									TorqueFullLoad = 400.SI<NewtonMeter>()
								},
							}, null)
					}
				}
			};
			GearboxData = new GearboxData() {
				Type = GearboxType.MT,
				Gears = new Dictionary<uint, GearData>() {
					//{ 1, new GearData() { Ratio = 1 } },
					//{ 2, new GearData() { Ratio = 5 } }
				}
			};
			AxleGearData = new AxleGearData() {
				AxleGear = new GearData() { Ratio = 1 }
			};
		}
	}
}