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
	/// Defines a method to acquire an DriverDemand in port.
	/// </summary>
	public interface IDrivingCycleInProvider
	{
		/// <summary>
		/// Returns the inport to connect it to another outport.
		/// </summary>
		/// <returns></returns>
		IDrivingCycleInPort InPort();
	}

	/// <summary>
	/// Defines a method to acquire an DriverDemand out port.
	/// </summary>
	public interface IDrivingCycleOutProvider
	{
		/// <summary>
		/// Returns the outport to send requests to.
		/// </summary>
		/// <returns></returns>
		IDrivingCycleOutPort OutPort();
	}


	//=============================================================


	/// <summary>
	/// Defines a connect method to connect the inport to an outport.
	/// </summary>
	public interface IDrivingCycleInPort
	{
		/// <summary>
		/// Connects the inport to another outport.
		/// </summary>
		void Connect(IDrivingCycleOutPort other);
	}

	/// <summary>
	/// Defines a request method for a DriverDemand-Out-Port.
	/// </summary>
	public interface IDrivingCycleOutPort
	{
		/// <summary>
		/// Requests the Outport with the given velocity [m/s] and road gradient [rad].
		/// </summary>
		/// <param name="absTime">[s]</param>
		/// <param name="ds"></param>
		/// <param name="targetVelocity">[m/s]</param>
		/// <param name="gradient">[rad]</param>
		IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient);

		/// <summary>
		/// Requests the outport to simulate the given time interval 
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="targetVelocity"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient);

		IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient);


		IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration);
	}
}