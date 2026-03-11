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
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TransmissionData : IEquatable<TransmissionData>

    {
		[ValidateObject]
		public TransmissionLossMap LossMap { get; internal set; }

		[Required, RangeOrNaN(double.Epsilon, 500)]
		public double Ratio { get; internal set; }

        public override bool Equals(object obj) => Equals(obj as TransmissionData);

        public override int GetHashCode()
        {
            return (LossMap, Ratio).GetHashCode();
        }

        public bool Equals(TransmissionData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

			return (other.LossMap == LossMap) && other.Ratio.IsEqual(Ratio);
        }

        public static bool operator ==(TransmissionData transmission1, TransmissionData transmission2)
        {
            if (ReferenceEquals(transmission1, transmission2))
            {
                return true;
            }

            if (ReferenceEquals(transmission1, null) || ReferenceEquals(transmission2, null))
            {
                return false;
            }

            return transmission1.Equals(transmission2);
        }

        public static bool operator !=(TransmissionData transmission1, TransmissionData transmission2) => !(transmission1 == transmission2);
    }

	[CustomValidation(typeof(GearData), "ValidateGearData")]
	public class GearData : TransmissionData, IEquatable<GearData>
	{
		public GearData()
		{
			TorqueConverterRatio = double.NaN;
		}

		public bool HasTorqueConverter => !double.IsNaN(TorqueConverterRatio) && TorqueConverterGearLossMap != null;

		public bool HasLockedGear => !double.IsNaN(Ratio) && LossMap != null;

		[ValidateObject]
		public ShiftPolygon ShiftPolygon { get; internal set; }

		public ShiftPolygon ExtendedShiftPolygon { get; internal set; }

		public double TorqueConverterRatio { get; internal set; }

		public TransmissionLossMap TorqueConverterGearLossMap { get; internal set; }

		public PerSecond MaxSpeed { get; internal set; }

		public NewtonMeter MaxTorque { get; internal set; }

		public ShiftPolygon TorqueConverterShiftPolygon { get; set; }

		public ShiftPolygon DeRatedEmShiftPolygon { get; set; }

		// ReSharper disable once UnusedMember.Global -- used via validation
		public static ValidationResult ValidateGearData(GearData gearData, ValidationContext context)
		{
			var validationService =
				context.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			var mode = validationService?.Mode ?? ExecutionMode.Declaration;
			var gbxType = validationService != null ? validationService.GearboxType : GearboxType.MT;
			var emsMission = validationService != null && validationService.IsEMSCycle;
			var jobType = validationService?.JobType ?? VectoSimulationJobType.ConventionalVehicle;
			var emPos = validationService?.EMPowertrainPosition;

			if (gearData.HasTorqueConverter)
			{
				if (gearData.TorqueConverterShiftPolygon == null)
				{
					return new ValidationResult("Shift Polygon for Torque Converter Gear required!");
				}
				var result = gearData.TorqueConverterShiftPolygon.Validate(mode, jobType, emPos, gbxType, emsMission);
				if (result.Any())
				{
					return new ValidationResult("Validation of GearData failed", result.Select(x => x.ErrorMessage));
				}
			}
			return ValidationResult.Success;
		}

		public override bool Equals(object obj) => Equals(obj as GearData);

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(GearData other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return base.Equals(other)
				&& TorqueConverterRatio.IsEqual(other.TorqueConverterRatio)
				&& (TorqueConverterGearLossMap == other.TorqueConverterGearLossMap)
				&& (MaxSpeed == other.MaxSpeed)
				&& (MaxTorque == other.MaxTorque)
				&& (ShiftPolygon == other.ShiftPolygon)
				&& (ExtendedShiftPolygon == other.ExtendedShiftPolygon)
				&& (TorqueConverterShiftPolygon == other.TorqueConverterShiftPolygon)
				&& (DeRatedEmShiftPolygon == other.DeRatedEmShiftPolygon);
		}

        public static bool operator ==(GearData gear1, GearData gear2)
        {
            if (ReferenceEquals(gear1, gear2))
            {
                return true;
            }

            if (ReferenceEquals(gear1, null) || ReferenceEquals(gear2, null))
            {
                return false;
            }

            return gear1.Equals(gear2);
        }

        public static bool operator !=(GearData gear1, GearData gear2) => !(gear1 == gear2);

    }
}