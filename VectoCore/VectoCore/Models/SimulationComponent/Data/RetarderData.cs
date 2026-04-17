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

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class RetarderData : SimulationComponentData, IEquatable<RetarderData>
	{
		[ValidateObject]
		public RetarderLossMap LossMap { get; internal set; }

		public RetarderType Type { get; internal set; }

		[SIRange(double.Epsilon, double.MaxValue)]
		public double Ratio { get; internal set; }

        public override bool Equals(object obj) => Equals(obj as RetarderData);

        public override int GetHashCode()
        {
            return (LossMap, Type, Ratio).GetHashCode();
        }

        public bool Equals(RetarderData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return (LossMap == other.LossMap) && (Type == other.Type) && Ratio.IsEqual(other.Ratio);
        }

        public static bool operator ==(RetarderData rd1, RetarderData rd2)
        {
            if (ReferenceEquals(rd1, rd2))
            {
                return true;
            }

            if (ReferenceEquals(rd1, null) || ReferenceEquals(rd2, null))
            {
                return false;
            }

            return rd1.Equals(rd2);
        }

        public static bool operator !=(RetarderData rd1, RetarderData rd2) => !(rd1 == rd2);
    }
}