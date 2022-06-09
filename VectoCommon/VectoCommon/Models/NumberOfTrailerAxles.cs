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
	public enum NumberOfTrailerAxles
	{
		// ReSharper disable InconsistentNaming
		Unknown = -1,
		One = 1,
		Two = 2,
		Three = 3,
		// ReSharper restore InconsistentNaming
	}

	public static class NumberOfTrailerAxlesHelper
	{
		public static string GetLabel(this NumberOfTrailerAxles ttype)
		{
			switch (ttype) {
				case NumberOfTrailerAxles.One:
					return "1";
				case NumberOfTrailerAxles.Two:
					return "2";
				case NumberOfTrailerAxles.Three:
					return "3";
				default:
					throw new ArgumentOutOfRangeException("number of axles", ttype, null);
			}
		}

		public static int ToInt(this NumberOfTrailerAxles nrOfAxles)
		{
			return Convert.ToInt32(nrOfAxles);
		}

		public static string ToXMLFormat(this NumberOfTrailerAxles ttype)
		{
			return ttype.GetLabel();
		}

		public static NumberOfTrailerAxles Parse(string numberOfAxles)
		{
			switch (numberOfAxles) {
				case "1":
					return NumberOfTrailerAxles.One;
				case "2":
					return NumberOfTrailerAxles.Two;
				case "3":
					return NumberOfTrailerAxles.Three;
				default:
					return NumberOfTrailerAxles.Unknown;
			}
		}
	}
}