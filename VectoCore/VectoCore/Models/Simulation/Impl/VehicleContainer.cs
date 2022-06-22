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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class VehicleContainer : LoggingObject, IVehicleContainer, IPowertainInfo
	{
		private List<Tuple<int, VectoSimulationComponent>> _components =
			new List<Tuple<int, VectoSimulationComponent>>();

		public virtual IEngineInfo EngineInfo { get; protected internal set; }
		public virtual IEngineControl EngineCtl { get; protected set; }
		public virtual IGearboxInfo GearboxInfo { get; protected set; }
		public virtual IGearboxControl GearboxCtl { get; protected set; }
		public virtual IAxlegearInfo AxlegearInfo { get; protected set; }
		public virtual IAngledriveInfo AngledriveInfo { get; protected set; }
		public virtual IVehicleInfo VehicleInfo { get; protected set; }
		public virtual IBrakes Brakes { get; protected set; }
		public virtual IWheelsInfo WheelsInfo { get; protected set; }
		public virtual IDriverInfo DriverInfo { get; protected set; }
		public virtual IHybridController HybridController { get; protected set; }

		public virtual IAuxInProvider BusAux { get; protected set; }

		public virtual IMileageCounter MileageCounter { get; protected set; }

		public virtual IClutchInfo ClutchInfo { get; protected set; }

		public virtual IDrivingCycleInfo DrivingCycleInfo { get; protected set; }

		public IRESSInfo BatteryInfo { get; protected set; }
		public ITorqueConverterInfo TorqueConverterInfo { get; protected set; }

		public virtual ITorqueConverterControl TorqueConverterCtl { get; protected set; }

		public IDCDCConverter DCDCConverter { get; protected set; }

		public WHRCharger WHRCharger { get; protected set; }

		public IElectricSystemInfo ElectricSystemInfo { get; protected set; }

		public virtual bool IsTestPowertrain => false;

		internal ISimulationOutPort Cycle;

		internal IModalDataContainer ModData;

		internal WriteSumData WriteSumData;

		internal readonly IList<ISimulationPreprocessor> Preprocessors = new List<ISimulationPreprocessor>();

		internal readonly Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors =
			new Dictionary<PowertrainPosition, IElectricMotorInfo>();


		public VehicleContainer(ExecutionMode executionMode, IModalDataContainer modData = null,
			WriteSumData writeSumData = null)
		{
			ModData = modData;
			WriteSumData = writeSumData ?? delegate { };
			ExecutionMode = executionMode;
		}

		#region IVehicleContainer

		public virtual IModalDataContainer ModalData => ModData;

		public virtual ISimulationOutPort GetCycleOutPort()
		{
			return Cycle;
		}


		public virtual Second AbsTime { get; set; }
		public IElectricMotorInfo ElectricMotorInfo(PowertrainPosition pos)
		{
			return ElectricMotors.GetVECTOValueOrDefault(pos);
		}



		public IPowertainInfo PowertrainInfo => this;

		public IHybridControllerInfo HybridControllerInfo => HybridController;

		public IHybridControllerCtl HybridControllerCtl => HybridController;


		public virtual void AddComponent(VectoSimulationComponent component)
		{
			var commitPriority = 0;

			if (component is IEngineControl c1) { EngineCtl = c1; }
			if (component is IDriverInfo c2) { DriverInfo = c2; }
			if (component is IGearboxControl c3) { GearboxCtl = c3; }
			if (component is ITorqueConverterInfo c4) { TorqueConverterInfo = c4; }
			if (component is ITorqueConverterControl c5) { TorqueConverterCtl = c5; }
			if (component is IAxlegearInfo c6) { AxlegearInfo = c6; }
			if (component is IAngledriveInfo c7) { AngledriveInfo = c7; }
			if (component is IWheelsInfo c8) { WheelsInfo = c8; }
			if (component is ISimulationOutPort c9) { Cycle = c9; }
			if (component is IMileageCounter c10) { MileageCounter = c10; }
			if (component is IBrakes c11) { Brakes = c11; }
			if (component is IClutchInfo c12) { ClutchInfo = c12; }
			if (component is IHybridController c13) { HybridController = c13; }
			if (component is IRESSInfo c14) { BatteryInfo = c14; }
			if (component is BusAuxiliariesAdapter c15) { BusAux = c15; }
			if (component is IDCDCConverter c16) { DCDCConverter = c16; }
			if (component is IElectricSystemInfo c24) { ElectricSystemInfo = c24; }
			
			if (component is IEngineInfo c17){
				EngineInfo = c17;
				commitPriority = 2;
				HasCombustionEngine = !(component is DummyEngineInfo); // true;
			}
			if (component is IGearboxInfo c18) {
				GearboxInfo = c18;
				commitPriority = 4;
				HasGearbox = true;
			}

			if (component is IVehicleInfo c19) {
				VehicleInfo = c19;
				commitPriority = 5;
			}

			if (component is IDrivingCycleInfo c20) {
				DrivingCycleInfo = c20;
				commitPriority = 6;
			}
			if (component is PTOCycleController c21) { commitPriority = 99; }
			if (component is VTPCycle c22) { commitPriority = 0; }
			if (component is IElectricMotorInfo c23) {
				if (c23.Position == PowertrainPosition.HybridPositionNotSet) {
					return;
				}
				if (ElectricMotors.ContainsKey(c23.Position)) {
					throw new VectoException("There is already an electric machine at position {0}", c23.Position);
				}

				ElectricMotors[c23.Position] = c23;
				HasElectricMotor = true;
			}

			if (component is WHRCharger c25) {
				WHRCharger = c25;
			}
			
			_components.Add(Tuple.Create(commitPriority, component));
			//todo mk20210617 use sorted list with inverse commitPriority (-commitPriority)
			_components = _components.OrderBy(x => x.Item1).Reverse().ToList();
		}



		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			Log.Info("VehicleContainer committing simulation. time: {0}, dist: {1}, speed: {2}", time,
				MileageCounter?.Distance, VehicleInfo?.VehicleSpeed ?? 0.KMPHtoMeterPerSecond());


			foreach (var component in _components) {
				component.Item2.CommitSimulationStep(time, simulationInterval, ModData);
			}

			if (ModData != null) {
				ModData[ModalResultField.drivingBehavior] = DriverInfo.DriverBehavior;
				ModData[ModalResultField.time] = time + simulationInterval / 2;
				ModData[ModalResultField.simulationInterval] = simulationInterval;
				ModData.CommitSimulationStep();
			}
		}

		public virtual void FinishSimulationRun(Exception e = null)
		{
			Log.Info("VehicleContainer finishing simulation.");
			ModData?.Finish(RunStatus, e);

			WriteSumData(ModData);

			ModData?.FinishSimulation();
			DrivingCycleInfo?.FinishSimulation();
		}

		public virtual IEnumerable<ISimulationPreprocessor> GetPreprocessingRuns => new ReadOnlyCollection<ISimulationPreprocessor>(Preprocessors);

		public virtual void AddPreprocessor(ISimulationPreprocessor simulationPreprocessor)
		{
			Preprocessors.Add(simulationPreprocessor);
		}

		public virtual void StartSimulationRun()
		{
			ModData?.Reset();
		}

		public virtual VectoRun.Status RunStatus { get; set; }

		#endregion

		public IReadOnlyCollection<VectoSimulationComponent> SimulationComponents()
		{
			return new ReadOnlyCollection<VectoSimulationComponent>(_components.Select(x => x.Item2).ToList());
		}

		public virtual bool HasElectricMotor { get; private set; }

		public PowertrainPosition[] ElectricMotorPositions => ElectricMotors.Keys.ToArray();

		public VectoSimulationJobType VehicleArchitecutre => RunData.JobType;

		public virtual bool HasCombustionEngine { get; private set; }

		public virtual bool HasGearbox { get; private set; }

		[Required, ValidateObject]
		public virtual VectoRunData RunData { get; set; }
		public virtual ExecutionMode ExecutionMode { get; }



	}

	public class ExemptedRunContainer : VehicleContainer
	{
		private IMileageCounter _mileageCounter;
		private IVehicleInfo _vehicleInfo;

		private IGearboxInfo _gearboxInfo;

		public ExemptedRunContainer(
			ExecutionMode executionMode, IModalDataContainer modData = null, WriteSumData writeSumData = null) : base(
			executionMode, modData, writeSumData)
		{
			_mileageCounter = new ZeroMileageCounter(this);
			_vehicleInfo = new DummyVehicleInfo(this);
			_gearboxInfo = new EngineOnlyGearboxInfo(this);
		}

		#region Overrides of VehicleContainer

		public override IMileageCounter MileageCounter => _mileageCounter;

		#endregion

		#region Overrides of VehicleContainer

		public override IVehicleInfo VehicleInfo => _vehicleInfo;

		#endregion

		#region Overrides of VehicleContainer

		public override IGearboxInfo GearboxInfo => _gearboxInfo;

		#endregion
	}
}