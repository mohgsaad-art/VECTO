using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl {
	internal class EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary : DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary
	{
		public EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(IVTPEngineeringInputDataProvider ivtpProvider) : base(
			ivtpProvider.JobInputData, null)
		{
			throw new Exception("VTP Simulation in engineering mode for heavy buses not supported");
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