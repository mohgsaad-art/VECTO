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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class IdleControllerSwitcher : IIdleController
	{
		private readonly IIdleController _idleController;
		private readonly PTOCycleController _ptoController;
		private IIdleController _currentController;

		public IdleControllerSwitcher(IIdleController idleController, PTOCycleController ptoController)
		{
			_idleController = idleController;
			_ptoController = ptoController;

			// default state is idleController
			_currentController = _idleController;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			return _currentController.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			throw new InvalidOperationException(string.Format("{0} cannot initialize.", GetType().FullName));
		}

		public ITnOutPort RequestPort
		{
			set {
				_idleController.RequestPort = value;
				_ptoController.RequestPort = value;
			}
		}

		public void Reset()
		{
			_idleController.Reset();
			_ptoController.Reset();
			_currentController = _idleController;
		}

		public void ActivatePTO()
		{
			_currentController = _ptoController;
		}

		public void ActivateIdle()
		{
			_currentController = _idleController;
		}

		public Second GetNextCycleTime()
		{
			return _ptoController.GetNextCycleTime();
		}

		public void CommitSimulationStep(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (_currentController == _ptoController) {
				_ptoController.CommitSimulationStep(time, simulationInterval, container);
			}
		}

		public Second Duration
		{
			get {
				if (_ptoController != null) {
					return _ptoController.Duration;
				}
				return 0.SI<Second>();
			}
		}
	}
}