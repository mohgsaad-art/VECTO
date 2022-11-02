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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class SimulationComponentData : LoggingObject
	{
		public bool SavedInDeclarationMode { get; internal set; }

		public string Manufacturer { get; internal set; }

		public string ModelName { get; internal set; }

		public DateTime Date { get; internal set; }

		public CertificationMethod CertificationMethod { get; internal set; }

		public string CertificationNumber { get; internal set; }

		public string DigestValueInput { get; internal set; }

		protected static ExecutionMode GetExecutionMode(ValidationContext context)
		{
			var validationService =
				context.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			return validationService?.Mode ?? ExecutionMode.Declaration;
		}

		protected static bool GetEmsMode(ValidationContext context)
		{
			var validationService =
				context.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			return validationService != null && validationService.IsEMSCycle;
		}

		protected static VectoSimulationJobType GetSimulationJobType(ValidationContext context)
		{
			var validationService =
				context.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			return validationService?.JobType ?? VectoSimulationJobType.ConventionalVehicle;

		}

		protected static PowertrainPosition? GetEMPosition(ValidationContext context)
		{
			var validationService =
				context.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			return validationService?.EMPowertrainPosition;
		}

	}
}