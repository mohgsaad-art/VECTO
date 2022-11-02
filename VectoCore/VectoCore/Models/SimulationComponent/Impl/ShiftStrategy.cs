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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Class ShiftStrategy is a base class for shift strategies. Implements some helper methods for checking the shift curves.
	/// </summary>
	public abstract class ShiftStrategy : BaseShiftStrategy
	{
		protected bool SkipGears;
		protected bool EarlyShiftUp;
		protected Gearbox _gearbox;

		protected ShiftStrategy(IVehicleContainer dataBus) : base(dataBus) {}

		public override IGearbox Gearbox
		{
			get => _gearbox;
			set {
				var myGearbox = value as Gearbox;
				if (myGearbox == null) {
					throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
				}
				_gearbox = myGearbox;
			}
		}

		/// <summary>
		/// Tests if the operating point is below the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsAboveDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveDownshiftCurve(inTorque, inEngineSpeed);
		}


		/// <summary>
		/// Tests if the operating point is above the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected bool IsAboveUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasSuccessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsBelowUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasSuccessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowUpshiftCurve(inTorque, inEngineSpeed);
		}
	}
}