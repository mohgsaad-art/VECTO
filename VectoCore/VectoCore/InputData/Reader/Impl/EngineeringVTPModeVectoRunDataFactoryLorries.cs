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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	internal class EngineeringVTPModeVectoRunDataFactoryLorries : DeclarationVTPModeVectoRunDataFactoryLorries
	{
		
		private EngineeringDataAdapter _engineeringDao = new EngineeringDataAdapter();

		public EngineeringVTPModeVectoRunDataFactoryLorries(IVTPEngineeringInputDataProvider ivtpProvider) : base(ivtpProvider.JobInputData, null)
		{
			
		}

		public override IEnumerable<VectoRunData> NextRun()
		{
			if (InitException != null) {
				throw InitException;
			}
			return JobInputData.Cycles.Select(
				cycle => {
					var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, false);
					// loading is not relevant as we use P_wheel
					var runData = CreateVectoRunData(Segment, Segment.Missions.First(), new Tuple<Kilogram, double?>(0.SI<Kilogram>(), null));
					runData.Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name);
					runData.Aux = AuxVTP;
					runData.FanDataVTP = GetFanData();
					runData.ExecutionMode = ExecutionMode.Engineering;
					runData.SimulationType = SimulationType.VerificationTest;
					runData.Mission = new Mission() {
						MissionType = MissionType.VerificationTest
					};
					runData.DriverData = Driverdata;
					return runData;
				});
		}

		protected override AuxFanData GetFanData()
		{
			return new AuxFanData() {
				FanCoefficients = JobInputData.FanPowerCoefficents.ToArray(),
				FanDiameter = JobInputData.FanDiameter,
			};
		}
	}
}
