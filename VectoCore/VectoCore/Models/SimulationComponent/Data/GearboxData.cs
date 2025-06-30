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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// Class for Gearbox Data. Gears can be accessed via Gears-Dictionary and range from 1 upwards.
	/// </summary>
	/// <remarks>The Axle Gear has its own Property "AxleGearData" and is *not included* in the Gears-Dictionary.</remarks>
	[CustomValidation(typeof(GearboxData), "ValidateGearboxData")]
	[DebuggerDisplay("GearboxData({Type}, #Gears: {Gears.Count}, ...)")]
	public class GearboxData : SimulationComponentData
	{
		public GearboxData() {}

		public GearboxType Type { get; internal set; }

		[Required, ValidateObject] public Dictionary<uint, GearData> Gears = new Dictionary<uint, GearData>();
		
		private GearList _gearlist;
		private MeterPerSecond _disengageWhenHaltingSpeed;

		public TorqueConverterData TorqueConverterData { get; internal set; } 

		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, SIRange(0, 5)]
		public Second TractionInterruption { get; internal set; }

		/// <summary>
		///	[%] (0-1) The torque reserve for shift strategy (early upshift, skipgears)
		/// </summary>
		

		[SIRange(0.5, 1)]
		public Second PowershiftShiftTime { get; internal set; }

		public bool ATEcoRollReleaseLockupClutch { get; internal set; }

		[JsonIgnore]
		public IGearboxDeclarationInputData InputData { get; internal set; }


		[JsonIgnore]
		public GearList GearList => _gearlist ?? (_gearlist = CreateGearList());

		public MeterPerSecond DisengageWhenHaltingSpeed
		{
			get {
				if (_disengageWhenHaltingSpeed != null)
					return _disengageWhenHaltingSpeed;
				else if (Type.AutomaticTransmission())
					return Constants.SimulationSettings.ATGearboxDisengageWhenHaltingSpeed;
				else
					return Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed;
			}
			internal set => _disengageWhenHaltingSpeed = value;
		}

		public string ShiftStrategy { get; set; }

		private GearList CreateGearList()
		{
			var gearList = new List<GearshiftPosition>();
			foreach (var gear in Gears) {
				if (Type.AutomaticTransmission() && (Type != GearboxType.IHPC)) {
					if (gear.Value.HasTorqueConverter) {
						gearList.Add(new GearshiftPosition(gear.Key, false));
					}
					if (gear.Value.HasLockedGear) {
						gearList.Add(new GearshiftPosition(gear.Key, true));
					}
				} else {
					gearList.Add(new GearshiftPosition(gear.Key));
				}
			}

			return new GearList(gearList.ToArray());
		}

		// ReSharper disable once UnusedMember.Global -- used via Validation
		public static ValidationResult ValidateGearboxData(GearboxData gearboxData, ValidationContext validationContext)
		{
			var mode = GetExecutionMode(validationContext);
			//var gbxType = GetGearboxType(validationContext);
			var emsMission = GetEmsMode(validationContext);
			var jobType = GetSimulationJobType(validationContext);
			var emPos = GetEMPosition(validationContext);
			
			var result = new List<ValidationResult>();

			if (gearboxData.Gears.Any(g => g.Value.HasTorqueConverter)) {
				if (!gearboxData.Type.AutomaticTransmission() || gearboxData.Type == GearboxType.APTN) {
					return new ValidationResult("Torque Converter can only be used with AT gearbox model");
				}
			} else {
				if (gearboxData.Type.AutomaticTransmission() && gearboxData.Type != GearboxType.APTN && gearboxData.Type != GearboxType.IHPC) {
					return new ValidationResult("AT gearbox model requires torque converter");
				}
			}
			if (gearboxData.Type.AutomaticTransmission() && gearboxData.Type != GearboxType.APTN && gearboxData.Type != GearboxType.IHPC) {
				gearboxData.TorqueConverterData.RequiredSpeedRatio =
					Math.Round(gearboxData.Gears[1].TorqueConverterRatio / gearboxData.Gears[1].Ratio, 4) * 0.95;
				result.AddRange(gearboxData.TorqueConverterData.Validate(mode, jobType, emPos, gearboxData.Type, emsMission));
				//result.AddRange(gearboxData.PowershiftShiftTime.Validate(mode, gearboxData.Type));
				//result.AddRange(gearboxData.PowershiftInertiaFactor.Validate(mode, gearboxData.Type));
				validationContext.MemberName = "PowershiftShiftTime";
				Validator.TryValidateProperty(gearboxData.PowershiftShiftTime, validationContext, result);
			}

			if (result.Any()) {
				return new ValidationResult("Validation of Gearbox Data failed", result.Select(x => x.ErrorMessage));
			}
			return ValidationResult.Success;
		}
	}
}