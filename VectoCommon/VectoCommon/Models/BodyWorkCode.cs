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
*   Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;

namespace TUGraz.VectoCommon.Models
{
	public enum BodyWorkCode
	{
		// ReSharper disable InconsistentNaming
		Unknown,
		Drybody,
		Refrigerated,
		Conditioned,
		CurtainSided,
		DropSide
		// ReSharper restore InconsistentNaming
	}

	public static class BodyWorkCodeHelper
	{

		public static string GetLabel(this BodyWorkCode ttype)
		{
			switch (ttype)
			{
				case BodyWorkCode.Drybody:
					return "dry box";
				case BodyWorkCode.Refrigerated:
					return "refrigerated";
				case BodyWorkCode.Conditioned:
					return "conditioned";
				case BodyWorkCode.CurtainSided:
					return "curtainsider";
				case BodyWorkCode.DropSide:
					return "drop-side with tarpaulin body"; 
				default:
					throw new ArgumentOutOfRangeException("trailer bodywork code", ttype, null);
			}
		}

		public static string ToXMLFormat(this BodyWorkCode ttype)
		{
			return ttype.GetLabel();
		}

		public static BodyWorkCode Parse(string vehicleCategory)
		{
			switch (vehicleCategory) {
				case "dry box":
					return BodyWorkCode.Drybody;
				case "refrigerated":
					return BodyWorkCode.Refrigerated;
				case "conditioned":
					return BodyWorkCode.Conditioned;
				case "curtainsider":
					return BodyWorkCode.CurtainSided;
				case "drop-side with tarpaulin body":
					return BodyWorkCode.DropSide;
				default:
					return BodyWorkCode.Unknown;
			}
		}
	}
}