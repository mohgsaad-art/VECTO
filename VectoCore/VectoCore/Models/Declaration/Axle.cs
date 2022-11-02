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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	[CustomValidation(typeof(Axle), "ValidateAxleData")]
	public class Axle : SimulationComponentData
	{
		public string WheelsDimension { get; internal set; }

		[Required, SIRange(0, 100)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, SIRange(0.003, 0.015)]
		public double RollResistanceCoefficient { get; internal set; }

		[Required, SIRange(500, 100000)]
		public Newton TyreTestLoad { get; internal set; }

		public string FuelEfficiencyClass { get; internal set; }

		[Required, SIRange(0, 1, ExecutionMode.Engineering), SIRange(double.MinValue, 1, ExecutionMode.Declaration)]
		public double AxleWeightShare { get; internal set; }

		public bool TwinTyres { get; internal set; }

		public AxleType AxleType { get; internal set; }
		

		public static ValidationResult ValidateAxleData(Axle axle, ValidationContext validationContext)
		{
			var execMode = GetExecutionMode(validationContext);
			if (execMode == ExecutionMode.Engineering)
				return ValidationResult.Success;
			try {
				DeclarationData.Wheels.Lookup(axle.WheelsDimension);
			} catch (Exception) {
				return new ValidationResult($"Unknown Tyre dimenstion '{axle.WheelsDimension}'");
			}
			return ValidationResult.Success;
		}
	}
}
