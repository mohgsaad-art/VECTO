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

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Defines interfaces for a gearbox.
	/// </summary>
	public interface IGearbox : IPowerTrainComponent, IGearboxInfo, IGearboxControl, IUpdateable { }

	public interface ITypedGearbox : IGearbox {}

	public interface IMTGearbox : ITypedGearbox {}

	public interface IAMTGearbox : ITypedGearbox
	{
		GearboxState GetPreviousState { get; }
	}

	public interface IAPTGearbox : ITypedGearbox
	{
		ATGearboxState GetPreviousState { get; }
		bool ShiftToLocked { get; }
		IIdleController IdleController { set; }
		bool TorqueConverterLocked { get; }

		
		
		ResponseDryRun Initialize(GearshiftPosition gear, NewtonMeter outTorque,
			PerSecond outAngularVelocity);

		new bool Disengaged { get; set; }
		KilogramSquareMeter EngineInertia { get; }
		ITorqueConverter TorqueConverter { get; }

		WattSecond ComputeShiftLosses(NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGearPos);
	}

	public interface IAPTNGearbox : ITypedGearbox { }

	public interface IPEVGearbox : ITypedGearbox { }

	public interface IIEPCGearbox : ITypedGearbox { }

	public interface ITorqueConverter : ITorqueConverterInfo, ITorqueConverterControl, IUpdateable
	{
		TorqueConverterOperatingPoint FindOperatingPoint(Second absTime, Second dt, NewtonMeter nextGearboxInTorque, PerSecond nextGearboxInSpeed);
	}
}