using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class ParallelHybridBatteryOnlyElectricMotorController : IElectricMotorControl
	{
		protected IVehicleContainer DataBus;
		public VectoRunData ModelData;
		//private IElectricSystem ElectricSystem;
		//protected ElectricMotorData ElectricMotorData;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;

		protected ITestPowertrain TestPowertrain;

		public ParallelHybridBatteryOnlyElectricMotorController(IVehicleContainer container, IElectricSystem es)
		{
			DataBus = container;
			ModelData = container.RunData;
			//ElectricMotorData = container.RunData.ElectricMachinesData.FirstOrDefault()?.Item2;
			//ElectricSystem = es;

			ElectricMotorsOff = ModelData.ElectricMachinesSinglePwt
				.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
				.ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null, x.Value));

			//var testContainer = container.SimplePowertrainBuilder.BuildSimpleHybridBatteryOnlyPowertrain(container.RunData);
			//TestPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(testContainer, false);
			TestPowertrain = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false);
		}

		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast ||
				DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				return null;
			}

			if (CannotProvideRecuperationAtLowSpeed(outTorque)) {
				return null;
			}

			if (maxDriveTorque == null) {
				return null;
			}

			if (dryRun) {
				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate)
					return maxDriveTorque;
				//if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
				//	return (outTorque + DataBus.Brakes.BrakePower /
				//			((prevOutAngularVelocity + currOutAngularVelocity) / 2.0))
				//		.LimitTo(0.SI<NewtonMeter>(), maxRecuperationTorque);
				//}
			}

			var emTorqueICEOff = FindEMTorqueICEOff(absTime, dt, outTorque, currOutAngularVelocity);


			var emTorque = emTorqueICEOff.LimitTo(maxDriveTorque,
				maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));


			return emTorque;
		}

		#endregion

		protected virtual bool CannotProvideRecuperationAtLowSpeed(NewtonMeter outTorque)
		{
			var driverIsBraking = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking &&
								DataBus.DriverInfo.DrivingAction == DrivingAction.Brake;

			return driverIsBraking && (DataBus.VehicleInfo.VehicleSpeed ?? 0.SI<MeterPerSecond>()).IsSmallerOrEqual(
										ModelData.GearboxSinglePwt?.DisengageWhenHaltingSpeed ?? Constants.SimulationSettings
											.ATGearboxDisengageWhenHaltingSpeed)
									&& outTorque.IsSmaller(0);
		}

		protected virtual NewtonMeter FindEMTorqueICEOff(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{

			var emOffResponse = GetEmOffResultEntry(absTime, dt, outTorque, outAngularVelocity);

			var gear = DataBus.GearboxInfo().Gear;
			TestPowertrain.UpdateComponents();

			var emPos = ElectricMotorsOff.Keys.First();
			var emCtl = TestPowertrain.ElectricMotors.First(x => x.Position == emPos).Control as ITestPowertrainElectricMotorControl;

            emCtl.EmOff = false;

			var emTorqueICEOff = SearchAlgorithm.Search(
				emOffResponse.ElectricMotor.ElectricMotorPowerMech / emOffResponse.ElectricMotor.AngularVelocity,
				emOffResponse.Engine.TorqueOutDemand, emOffResponse.ElectricMotor.MaxDriveTorque * 0.1,
				getYValue: r => {
					var response = r as IResponse;
					return response.Engine.TorqueOutDemand;
				},
				evaluateFunction: emTq => {
					//var cfg = new HybridStrategyResponse {
					//	CombustionEngineOn = false,
					//	GearboxInNeutral = true,
					//	NextGear = gear,
					//	MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
					//		{ emPos, Tuple.Create(emOffResponse.ElectricMotor.AngularVelocity, emTq) }
					//	}
					//};
					//TestPowertrain.HybridController.ApplyStrategySettings(cfg);
					emCtl.EMTorque = emTq;
					var retVal = TestPowertrain.GetElectricMotor(Constants.NOT_IN_AXLE_POWERTRAIN).Request(absTime, dt, outTorque,
							outAngularVelocity, true);
					//retVal.HybridController.StrategySettings = cfg;
					return retVal;
				},
				criterion: r => {
					var response = r as IResponse;
					return response.Engine.TorqueOutDemand.Value();
				},
				abortCriterion: (r, c) => r == null,
				searcher: this
			);
			return emTorqueICEOff;
		}

		protected virtual IResponse GetEmOffResultEntry(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			
			var gear = DataBus.GearboxInfo().Gear;
			var emPos = ElectricMotorsOff.Keys.First();
			var emCtl = TestPowertrain.ElectricMotors.First(x => x.Position == emPos).Control as ITestPowertrainElectricMotorControl;

            TestPowertrain.UpdateComponents();

			emCtl.EmOff = true;
			emCtl.EMTorque = null;
			
			var emOffResponse =
				TestPowertrain.GetElectricMotor(Constants.NOT_IN_AXLE_POWERTRAIN).Request(absTime, dt, outTorque, outAngularVelocity, true);

			return emOffResponse;
		}
	}
}