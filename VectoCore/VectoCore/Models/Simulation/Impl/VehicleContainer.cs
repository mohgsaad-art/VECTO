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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
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
	public class VehicleContainer : LoggingObject, IVehicleContainer
	{
		private List<Tuple<int, VectoSimulationComponent>> _components =
			new List<Tuple<int, VectoSimulationComponent>>();

		
		public virtual bool IsTestPowertrain => false;

		internal ISimulationOutPort Cycle;

		internal IModalDataContainer ModData;

		internal WriteSumData WriteSumData;

		internal readonly IList<ISimulationPreprocessor> Preprocessors = new List<ISimulationPreprocessor>();

		
		public VehicleContainer(ExecutionMode executionMode, IModalDataContainer modData = null,
			WriteSumData writeSumData = null)
		{
			ModData = modData;
			WriteSumData = writeSumData ?? delegate { };
			ExecutionMode = executionMode;
		}

		#region IVehicleContainer

		public IModalDataContainer ModalData
		{
			get { return ModData; }
		}

		public ISimulationOutPort GetCycleOutPort()
		{
			return Cycle;
		}

		public FuelType FuelType
		{
			get { return ModData.FuelData.FuelType; }
		}

		public virtual Second AbsTime { get; set; }
		public IMileageCounter MileageCounter { get; protected set; }
		public IGearboxInfo GearboxInfo { get; protected set; }
		public IGearboxControl GearboxCtl { get; protected set; }
		public IAxlegearInfo AxlegearInfo { get; protected set; }

		public IAngledriveInfo AngledriveInfo { get; protected set; }
		public IEngineInfo EngineInfo { get; protected internal set; }
		public IEngineControl EngineCtl { get; protected set; }
		public IVehicleInfo VehicleInfo { get; protected set; }
		public IClutchInfo ClutchInfo { get; protected set; }
		public IBrakes Brakes { get; protected set; }
		public IWheelsInfo WheelsInfo { get; protected set; }

		public void AddComponent(VectoSimulationComponent component)
		{
			var commitPriority = 0;

			component.Switch()
				.If<IEngineInfo>(c => {
					EngineInfo = c;
					commitPriority = 2;
				})
				.If<IEngineControl>(c => {
					EngineCtl = c;
				})
				.If<IDriverInfo>(c => DriverInfo = c)
				.If<IGearboxInfo>(c => {
					GearboxInfo = c;
					commitPriority = 4;
				})
				.If<IGearboxControl>(c => {
					GearboxCtl = c;
				})
				.If<IAxlegearInfo>(c => AxlegearInfo = c)
				.If<IAngledriveInfo>(c => AngledriveInfo = c)
				.If<IWheelsInfo>(c => WheelsInfo = c)
				.If<IVehicleInfo>(c => {
					VehicleInfo = c;
					commitPriority = 5;
				})
				.If<ISimulationOutPort>(c => Cycle = c)
				.If<IMileageCounter>(c => MileageCounter = c)
				.If<IBrakes>(c => Brakes = c)
				.If<IClutchInfo>(c => ClutchInfo = c)
				.If<IDrivingCycleInfo>(c => {
					DrivingCycleInfo = c;
					commitPriority = 6;
				})
				.If<PTOCycleController>(c => { commitPriority = 99; })
				.If<VTPCycle>(_ => { commitPriority = 0; });

			_components.Add(Tuple.Create(commitPriority, component));
			_components = _components.OrderBy(x => x.Item1).Reverse().ToList();
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			Log.Info("VehicleContainer committing simulation. time: {0}, dist: {1}, speed: {2}", time,
				MileageCounter?.Distance, VehicleInfo?.VehicleSpeed ?? 0.KMPHtoMeterPerSecond());


			foreach (var component in _components) {
				component.Item2.CommitSimulationStep(ModData);
			}

			if (ModData != null) {
				ModData[ModalResultField.drivingBehavior] = DriverInfo?.DriverBehavior ?? DrivingBehavior.Driving;
				ModData[ModalResultField.time] = time + simulationInterval / 2;
				ModData[ModalResultField.simulationInterval] = simulationInterval;
				ModData.CommitSimulationStep();
			}
		}

		public void FinishSimulationRun(Exception e = null)
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

		public void FinishSimulation()
		{
			throw new NotImplementedException();
		}

		public VectoRun.Status RunStatus { get; set; }

		#endregion

		public IReadOnlyCollection<VectoSimulationComponent> SimulationComponents()
		{
			return new ReadOnlyCollection<VectoSimulationComponent>(_components.Select(x => x.Item2).ToList());
		}




		public virtual IDriverInfo DriverInfo { get; protected set; }

		public IDrivingCycleInfo DrivingCycleInfo { get; protected set; }

		public IPowertainInfo PowertrainInfo { get; }

		

		public VectoRunData RunData { get; set; }

		public ExecutionMode ExecutionMode { get; }

	}
}