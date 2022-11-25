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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineFanAuxiliary
	{
		protected readonly double[] FanCoefficients;

		protected readonly double FanDiameter;

		public EngineFanAuxiliary(double[] fanParameters, Meter fanDiameter)
		{
			if (fanParameters.Length < 3) {
				throw new ArgumentException("Three fan parameters are required!");
			}
			FanCoefficients = fanParameters;
			FanDiameter = fanDiameter.ConvertToMilliMeter();
		}

		public Watt PowerDemand(PerSecond fanSpeed)
		{
			return (FanCoefficients[0] * Math.Pow(fanSpeed.AsRPM / FanCoefficients[1], 3) * Math.Pow(FanDiameter / FanCoefficients[2], 5) * 1000).SI<Watt>();
        }

        public Watt PowerDemand(DrivingCycleData.DrivingCycleEntry entry)
        { 
			return (entry.FanElectricalPower != null)
				? PowerDemand(entry.FanElectricalPower)
				: PowerDemand(entry.FanSpeed);
        }

        public Watt PowerDemand(Watt fanElectricalPower)
		{
			return fanElectricalPower / DeclarationData.AlternatorEfficiency;
		}

    }
}