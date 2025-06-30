using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy
{

    public class PrimaryBusPostMortemStrategy : IPostMortemAnalyzeStrategy
	{
		protected static readonly MeterPerSecond MinSpeed = 5.KMPHtoMeterPerSecond();

		#region Implementation of IPostMortemAnalyzeStrategy

		public bool AbortSimulation(IVehicleContainer container, Exception exception)
		{
			if (container.RunData.Mission.MissionType != MissionType.Interurban) {
				// for now only consider interurban cycle
				return true;
			}

			if (container.RunData.Loading != LoadingType.ReferenceLoad) {
				// for now only consider reference load
				return true;
			}

			if (!container.DrivingCycleInfo.RoadGradient.IsGreater(0)) {
				// road gradient must be greater than 0
				return true;
			}

			if (container.VehicleInfo.VehicleSpeed.IsGreater(MinSpeed)) {
				// vehicle has to be almost stopped
				return true;
			}

			var maxGradability = GetMaxGradability(container);
			if (maxGradability.IsSmaller(container.DrivingCycleInfo.RoadGradient)) {
				// the vehicle cannot go this steep uphill passage...
				return false;
			}
			return true;
		}

		protected virtual Radian GetMaxGradability(IVehicleContainer container)
		{
			ITestPowertrain testContainer = null;
            switch (container.PowertrainInfo.VehicleArchitecutre) {
				//case VectoSimulationJobType.ConventionalVehicle:
				//	PowertrainBuilder.BuildSimplePowertrain(container.RunData, testContainer);
				//	break;
				//case VectoSimulationJobType.ParallelHybridVehicle:
				//case VectoSimulationJobType.IHPC:
				//	PowertrainBuilder.BuildSimpleHybridPowertrain(container.RunData, testContainer);
				//	break;
				//case VectoSimulationJobType.SerialHybridVehicle:
				//	PowertrainBuilder.BuildSimpleSerialHybridPowertrain(container.RunData, testContainer);
				//	break;
				//case VectoSimulationJobType.IEPC_S:
				//	PowertrainBuilder.BuildSimpleIEPCHybridPowertrain(container.RunData, testContainer);
				//	break;
                case VectoSimulationJobType.FCHV:
                case VectoSimulationJobType.FCHV_IEPC:
                case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					testContainer = container.SimplePowertrainBuilder.CreateTestPowertrain(container, false);
					break;
				default:
					throw new VectoException($"unhandled powertrain architecture {container.PowertrainInfo.VehicleArchitecutre} to calculate gradability");
			}

			testContainer.UpdateComponents();

			var maxSlope = SearchSlope(testContainer);

			return maxSlope;
		}

		protected virtual Radian SearchSlope(ITestPowertrain testPowertrain)
		{
			var simulationInterval = Constants.SimulationSettings.TargetTimeInterval;
			var absTime = 0.SI<Second>();
			var gradient = 0.SI<Radian>();

			var vehicle = testPowertrain.Vehicle;
			var acceleration = DeclarationData.GearboxTCU.StartAcceleration * 0.5;

            foreach (var motor in testPowertrain.ElectricMotors.Values) {
				if (motor.Control is ITestPowertrainElectricMotorControl emCtl) {
					emCtl.EmOff = false;
				}
			}

			if (testPowertrain.Container.HasGearbox) {
				var gbx = testPowertrain.Gearbox;
				gbx.SetGear = testPowertrain.Container.RunData.GearboxData.GearList.First();
			}
			vehicle.Initialize(0.KMPHtoMeterPerSecond(), gradient);

			var architecture = testPowertrain.Container.VehicleArchitecutre;
            var initialResponse = vehicle.Request(absTime, simulationInterval, acceleration, gradient, true);
			var delta = GetDelta(initialResponse as ResponseDryRun, architecture);

			try {
				gradient = SearchAlgorithm.Search(
					gradient, delta, 0.1.SI<Radian>(),
					getYValue: response => GetDelta(response as ResponseDryRun, architecture),
					evaluateFunction: grad => vehicle.Request(absTime, simulationInterval, acceleration, grad, true),
					criterion: response => GetDelta(response as ResponseDryRun, architecture).Value(),
					searcher: this);
			} catch (VectoSearchAbortedException) {
				return double.MaxValue.SI<Radian>();
			}

			return gradient;
		}

		private Watt GetDelta(ResponseDryRun response, VectoSimulationJobType vehicleArchitecutre)
		{
			//return response.DeltaFullLoad;

			switch (vehicleArchitecutre) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.IHPC:
					return response.DeltaFullLoad;
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					return (response.ElectricMotor.TorqueRequest + response.ElectricMotor.MaxDriveTorque) * response.ElectricMotor.AvgDrivetrainSpeed;
				default:
					throw new VectoException($"unhandled powertrain architecture {vehicleArchitecutre} to calculate gradability");
			}
        }

		#endregion
	}
}