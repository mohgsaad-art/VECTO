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

namespace TUGraz.VectoCore.Models.Connector.Ports
{
	/// <summary>
	/// Defines a method to acquire an Tn in port.
	/// </summary>
	public interface ITnInProvider
	{
		/// <summary>
		/// Returns the inport to connect it to another outport.
		/// </summary>
		ITnInPort InPort();
	}

	/// <summary>
	/// Defines a method to acquire an Tn out port.
	/// </summary>
	public interface ITnOutProvider
	{
		/// <summary>
		/// Returns the outport to send requests to.
		/// </summary>
		ITnOutPort OutPort();
	}

	/// <summary>
	/// Defines a connect method to connect the inport to an outport.
	/// </summary>
	public interface ITnInPort
	{
		/// <summary>
		/// Connects the inport to another outport.
		/// </summary>
		void Connect(ITnOutPort other);
	}

	/// <summary>
	/// Defines a request method for a Tn-Out-Port.
	/// </summary>
	public interface ITnOutPort
	{
		/// <summary>
		/// Requests the Outport with the given torque [Nm] and angularVelocity [rad/s].
		/// </summary>
		IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);

		IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);
	}
}