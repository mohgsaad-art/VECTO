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

namespace TUGraz.VectoCommon.Models
{
	public enum CrossWindCorrectionMode
	{
		NoCorrection,
		SpeedDependentCorrectionFactor,
		VAirBetaLookupTable,
		DeclarationModeCorrection
	}

	public static class CrossWindCorrectionModeHelper
	{
		private const string SpeedDependentCorrectionFactor = "CdofVEng";
		private const string DeclarationModeCorrection = "CdofVdecl";
		private const string VAirBetaLookupTable = "CdofBeta";
		private const string NoCorrection = "Off";

		public static CrossWindCorrectionMode Parse(string correctionMode)
		{
			if (correctionMode.Equals(SpeedDependentCorrectionFactor, StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.SpeedDependentCorrectionFactor;
			}
			if (correctionMode.Equals(DeclarationModeCorrection, StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.DeclarationModeCorrection;
			}
			if (correctionMode.Equals(VAirBetaLookupTable, StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.VAirBetaLookupTable;
			}
			if (correctionMode.Equals(NoCorrection, StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.NoCorrection;
			}
			LogManager.GetLogger(typeof(CrossWindCorrectionModeHelper).ToString())
				.Warn("Invalid Crosswind correction Mode given. Ignoring Crosswind Correction!");
			return CrossWindCorrectionMode.NoCorrection;
		}

		public static string GetName(this CrossWindCorrectionMode mode)
		{
			switch (mode) {
				case CrossWindCorrectionMode.NoCorrection:
					return NoCorrection;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					return SpeedDependentCorrectionFactor;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					return VAirBetaLookupTable;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					return DeclarationModeCorrection;
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrectionMode", mode, null);
			}
		}

		public static string GetLabel(this CrossWindCorrectionMode mode)
		{
			switch (mode) {
				case CrossWindCorrectionMode.NoCorrection:
					return "No Correction";
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					return "Speed dependend (User-defined)";
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					return "Vair & beta Input";
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					return "Speed dependent (Declaration Mode)";
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrectionMode", mode, null);
			}
		}

		public static string ToXMLFormat(this CrossWindCorrectionMode mode)
		{
			switch (mode) {
				case CrossWindCorrectionMode.NoCorrection:
					return "No Correction";
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					return "Speed Dependent Correction Factor";
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					return "VAir Beta Lookup Table";
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					return "Declaration Mode Correction";
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", mode, null);
			}
		}
	}
}