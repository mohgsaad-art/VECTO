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

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public enum PTOConsumerType
	{
		mechanical,
		electrical
	}

	public class PTOData : SimulationComponentData, IEquatable<PTOData>
	{
		public string TransmissionType;
		//Transmission powerdemand mechanical
		public Watt TransmissionPowerDemand;

		//Transmission powerdemand electrical
		public Watt TransmissionPowerDemandElectrical;

		public PTOLossMap LossMap;
		public DrivingCycleData PTOCycle;
		public PTOConsumerType ConsumerType = PTOConsumerType.mechanical;

        public override bool Equals(object obj) => Equals(obj as PTOData);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(PTOData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return (TransmissionType == other.TransmissionType) 
                && (TransmissionPowerDemand == other.TransmissionPowerDemand)
                && (TransmissionPowerDemandElectrical == other.TransmissionPowerDemandElectrical)
                && (LossMap == other.LossMap)
                && (ConsumerType == other.ConsumerType);
        }

        public static bool operator ==(PTOData pto1, PTOData pto2)
        {
            if (ReferenceEquals(pto1, pto2))
            {
                return true;
            }

            if (ReferenceEquals(pto1, null) || ReferenceEquals(pto2, null))
            {
                return false;
            }

            return pto1.Equals(pto2);
        }

        public static bool operator !=(PTOData pto1, PTOData pto2) => !(pto1 == pto2);
    }
}