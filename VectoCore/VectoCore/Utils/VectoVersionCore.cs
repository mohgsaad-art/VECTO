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

namespace TUGraz.VectoCore.Utils
{
	public static class VectoSimulationCore
	{

#if CERTIFICATION_RELEASE
		private const string SUFFIX = "";
#else
#if RELEASE_CANDIDATE
		private const string SUFFIX = "-RC";
#else
		private const string SUFFIX = "-DEV";
#endif
#endif

		public static string VersionNumber
		{
			get {
				return "3.3.12.2800" + SUFFIX;
			}
		}

		public static string FullVersion
		{
			get {
				return string.Format("VectoCore {0}", VersionNumber);
			}
		}

		public enum VersionPart {
			Major = 0,
			Minor = 1,
			Patch = 2,
			Build = 3
		};

		public static int CompareVersions(string a, string b, VersionPart bound = VersionPart.Build)
		{ 
			var va = a.Split('-')[0].Split('.');
			var vb = b.Split('-')[0].Split('.');
			
			for (int i = 0; (i < va.Length) && (i <= (int)bound); i++) {
				string vbPartStr = (i < vb.Length) ? vb[i] : "0";
				int vbPart = 0;
				int.TryParse(vbPartStr, out vbPart);

				int vaPart = 0;
				int.TryParse(va[i], out vaPart);

				if (vaPart != vbPart) {
					return (vaPart > vbPart) ? 1 : -1;
				}
			}

			return 0;
		}

	}
}