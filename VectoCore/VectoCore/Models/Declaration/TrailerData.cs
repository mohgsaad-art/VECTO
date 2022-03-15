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
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class TrailerData
	{
		public bool VolumeOrientation { get; set; }
		public int VehicleGroup { get; set; }
		public bool VehicleGroupActive { get; set; }
		public string GenericVehicleCode { get; set; }
		public Kilogram CurbMassTrailer { get; set; }
		public KilogramSquareMeter Inertia { get; set; }
		public CorrectedTrailerData TotalCorrection { get; set; }
		public CorrectedTrailerData TotalRefCorrection { get; set; }
		public double MaxGcm { get; set; }
		public double CdxABase { get; set; }

	}

	public class CorrectedTrailerData
	{
		public SquareMeter CdxA0 { get; set; }
		public SquareMeter CdxA3 { get; set; }
		public SquareMeter CdxA6 { get; set; }
		public SquareMeter CdxA9 { get; set; }

		public SquareMeter CdxA00 { get; set; }
		public SquareMeter CdxA03 { get; set; }
		public SquareMeter CdxA06 { get; set; }
		public SquareMeter CdxA09 { get; set; }

		public SquareMeter A1 { get; set; }
		public SquareMeter A2 { get; set; }
		public SquareMeter A3 { get; set; }
	}

	public static class CorrectedTrailerDataHelper
	{
		/// <summary>
		/// Updates CdxA3 - CdxA9 based on CdxA0 (e.g. CdxA3 = CdxA0 + (A1 * 3) + (data.A2 * 3 * 3) + (data.A3 * 3 * 3 * 3) 
		/// </summary>
		public static void UpdateCdAxX(this CorrectedTrailerData data)
		{
			data.CdxA3 = data.CdxA0 + (data.A1 * 3) + (data.A2 * 3 * 3) + (data.A3 * 3 * 3 * 3);
			data.CdxA6 = data.CdxA0 + (data.A1 * 6) + (data.A2 * 6 * 6) + (data.A3 * 6 * 6 * 6);
			data.CdxA9 = data.CdxA0 + (data.A1 * 9) + (data.A2 * 9 * 9) + (data.A3 * 9 * 9 * 9);
		}
	}
}
