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

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public static class Formulas
	{
		/// <summary>
		/// Calculates the power from torque and angular velocity.
		/// </summary>
		public static Watt TorqueToPower(NewtonMeter torque, PerSecond angularVelocity)
		{
			return torque * angularVelocity;
		}

		/// <summary>
		/// Calculates the torque from power and angular velocity.
		/// </summary>
		public static NewtonMeter PowerToTorque(Watt power, PerSecond angularVelocity)
		{
			return power / angularVelocity;
		}

		/// <summary>
		/// Calculates power loss caused by inertia.
		/// https://en.wikipedia.org/wiki/Angular_acceleration
		/// alpha = delta_omega / delta_t
		/// tau = I * alpha
		/// </summary>
		/// <param name="currentOmega">The current omega (new angularSpeed).</param>
		/// <param name="previousOmega">The previous omega (old angularSpeed).</param>
		/// <param name="inertia">The inertia parameter.</param>
		/// <param name="dt">The dt.</param>
		/// <returns></returns>
		public static Watt InertiaPower(PerSecond currentOmega, PerSecond previousOmega, KilogramSquareMeter inertia,
			Second dt)
		{
			var deltaOmega = currentOmega.Value() - previousOmega.Value();

			var alpha = deltaOmega / dt.Value();
			var torque = inertia.Value() * alpha;

			var avgOmega = (currentOmega.Value() + previousOmega.Value()) / 2;
			return (torque * avgOmega).SI<Watt>();
		}

		public static Watt InertiaPower(PerSecond omega, PerSquareSecond alpha, KilogramSquareMeter inertia, Second dt)
		{
			var torque = inertia.Value() * alpha.Value();
			var power = torque * (omega.Value() + alpha.Value() / 2 * dt.Value());
			return power.SI<Watt>();
		}
	}
}