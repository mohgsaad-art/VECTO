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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Interface for the ShiftStrategy. Decides when to shift and which gear to take.
	/// </summary>
	public interface IShiftStrategy 
	{
		/// <summary>
		/// Checks if a shift operation is required.
		/// </summary>
		/// <param name="absTime">The abs time.</param>
		/// <param name="dt">The dt.</param>
		/// <param name="outTorque">The out torque.</param>
		/// <param name="outAngularVelocity">The out angular velocity.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inAngularVelocity">The in angular velocity.</param>
		/// <param name="gear">The current gear.</param>
		/// <param name="lastShiftTime">The last shift time.</param>
		/// <param name="response"></param>
		/// <returns><c>true</c> if a shift is required, <c>false</c> otherwise.</returns>
		bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response);

		/// <summary>
		/// Returns an appropriate starting gear after a vehicle standstill.
		/// </summary>
		/// <param name="absTime">The abs time.</param>
		/// <param name="dt">The dt.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="outAngularVelocity">The angular speed.</param>
		/// <returns>The initial gear.</returns>
		GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity);

		/// <summary>
		/// Engages a gear.
		/// </summary>
		/// <param name="absTime">The abs time.</param>
		/// <param name="dt">The dt.</param>
		/// <param name="outTorque">The out torque.</param>
		/// <param name="outAngularVelocity">The out engine speed.</param>
		/// <returns>The gear to take.</returns>
		GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);

		/// <summary>
		/// Disengages a gear.
		/// </summary>
		/// <param name="absTime">The abs time.</param>
		/// <param name="dt">The dt.</param>
		/// <param name="outTorque">The out torque.</param>
		/// <param name="outAngularVelocity">The out engine speed.</param>
		void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);

		/// <summary>
		/// Gets or sets the gearbox.
		/// </summary>
		/// <value>
		/// The gearbox.
		/// </value>
		IGearbox Gearbox { get; set; }

		GearshiftPosition NextGear { get; }

		bool CheckGearshiftRequired { get; }
		GearshiftPosition MaxStartGear { get; }

		void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);

		void WriteModalResults(IModalDataContainer container);

		VelocityRollingLookup VelocityDropData { get; }
	}

	public interface IHybridControlShiftStrategy : IShiftStrategy
	{
		void SetNextGear(GearshiftPosition nextGear);
    }
}