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
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Utils
{
	public static class ProviderExtensions
	{
		public static void AddAuxiliaries(this CombustionEngine engine, IVehicleContainer container,
			VectoRunData data)
		{
			// aux --> engine
			if (data.BusAuxiliaries != null) {
				engine.Connect(PowertrainBuilder.CreateAdvancedAuxiliaries(data, container).Port());
			} else {
				if (data.Aux != null) {
					engine.Connect(PowertrainBuilder.CreateAuxiliaries(data, container).Port());
				}
			}
		}

		public static IDriver AddComponent(this IDrivingCycleInProvider prev, IDriver next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IVehicle AddComponent(this IDriverDemandInProvider prev, IVehicle next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IWheels AddComponent(this IFvInProvider prev, IWheels next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IPowerTrainComponent AddComponent(this ITnInProvider prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IPowerTrainComponent AddComponent(this IPowerTrainComponent prev, IPowerTrainComponent next)
		{
			if (next == null) {
				return prev;
			}

			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static CombustionEngine AddComponent(this IPowerTrainComponent prev, CombustionEngine next,
			IIdleController idleController = null)
		{
			prev.InPort().Connect(next.OutPort());

			if (idleController == null) {
				idleController = next.IdleController;
			}

			var clutch = prev as IClutch;
			if (clutch != null) {
				clutch.IdleController = idleController;
			}
			var atGbx = prev as ATGearbox;
			if (atGbx != null) {
				atGbx.IdleController = idleController;
			}

			return next;
		}

		public static IPowerTrainComponent AddComponent(this IPowerTrainComponent prev, IGearbox gearbox, RetarderData data,
			IVehicleContainer container)
		{
			switch (data.Type) {
				case RetarderType.TransmissionOutputRetarder:
					return prev.AddComponent(new Retarder(container, data.LossMap, data.Ratio)).AddComponent(gearbox);
				case RetarderType.TransmissionInputRetarder:
					return prev.AddComponent(gearbox).AddComponent(new Retarder(container, data.LossMap, data.Ratio));
				case RetarderType.None:
				case RetarderType.LossesIncludedInTransmission:
				case RetarderType.EngineRetarder:
					return prev.AddComponent(new DummyRetarder(container)).AddComponent(gearbox);
				default:
					// ReSharper disable once NotResolvedInText
					// ReSharper disable once LocalizableElement
					throw new ArgumentOutOfRangeException("retarderdata.Type", data.Type.ToString(), "Retardertype unknown");
			}
		}
	}
}
