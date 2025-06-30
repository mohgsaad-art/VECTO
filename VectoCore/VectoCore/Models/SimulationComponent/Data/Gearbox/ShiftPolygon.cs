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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	[CustomValidation(typeof(ShiftPolygon), "ValidateShiftPolygon")]
	public class ShiftPolygon : SimulationComponentData
	{
		private readonly List<ShiftPolygonEntry> _upShiftPolygon;
		private readonly List<ShiftPolygonEntry> _downShiftPolygon;

		internal ShiftPolygon(IList<ShiftPolygonEntry> downshift, IList<ShiftPolygonEntry> upShift)
		{
			_upShiftPolygon = upShift.ToList();
			_downShiftPolygon = downshift.ToList();
		}

		[JsonIgnore]
		public ReadOnlyCollection<ShiftPolygonEntry> Upshift => _upShiftPolygon.AsReadOnly();

		[JsonIgnore]
		public ReadOnlyCollection<ShiftPolygonEntry> Downshift => _downShiftPolygon.AsReadOnly();

		public string[] DownshiftSerialized
		{
			get { return _downShiftPolygon.Select(x => $"{x.AngularSpeed.AsRPM} [rpm], {x.Torque}").ToArray(); }
		}

		public string[] UpshiftSerialized
		{
			get { return _upShiftPolygon.Select(x => $"{x.AngularSpeed.AsRPM} [rpm], {x.Torque}").ToArray(); }
		}

		public bool IsBelowDownshiftCurve(NewtonMeter inTorque, PerSecond inAngularVelocity)
		{
			if (!Downshift.Any()) {
				return false;}
			var section = Downshift.GetSection(entry => entry.AngularSpeed < inAngularVelocity);
			if (section.Item2.AngularSpeed < inAngularVelocity) {
				return false;
			}
			return IsLeftOf(inAngularVelocity, inTorque, section);
		}

		public bool IsBelowUpshiftCurve(NewtonMeter inTorque, PerSecond inAngularVelocity)
		{
			var section = Upshift.GetSection(entry => entry.AngularSpeed < inAngularVelocity);
			if (section.Item2.AngularSpeed < inAngularVelocity) {
				return false;
			}
			return IsLeftOf(inAngularVelocity, inTorque, section);
		}

		public bool IsAboveDownshiftCurve(NewtonMeter inTorque, PerSecond inAngularVelocity)
		{
			var section = Downshift.GetSection(entry => entry.AngularSpeed < inAngularVelocity);

			if (section.Item2.AngularSpeed < inAngularVelocity) {
				return true;
			}
			return IsRightOf(inAngularVelocity, inTorque, section);
		}

		public bool IsAboveUpshiftCurve(NewtonMeter inTorque, PerSecond inAngularVelocity)
		{
			if (!Upshift.Any()) {
				return false;
			}
			var section = Upshift.GetSection(entry => entry.AngularSpeed < inAngularVelocity);

			if (section.Item2.AngularSpeed < inAngularVelocity) {
				return true;
			}
			return IsRightOf(inAngularVelocity, inTorque, section);
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="segment">Edge of the shift polygon</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is positive (which means that X was on the right side of from--to).</remarks>
		public static bool IsLeftOf(PerSecond angularSpeed, NewtonMeter torque,
			(ShiftPolygonEntry, ShiftPolygonEntry) segment)
		{
			if (segment.Item1.AngularSpeed < angularSpeed && segment.Item2.AngularSpeed < angularSpeed)
				return false;

			var abX = segment.Item2.AngularSpeed.Value() - segment.Item1.AngularSpeed.Value();
			var abY = segment.Item2.Torque.Value() - segment.Item1.Torque.Value();
			var acX = angularSpeed.Value() - segment.Item1.AngularSpeed.Value();
			var acY = torque.Value() - segment.Item1.Torque.Value();
			var z = abX * acY - abY * acX;
			return z.IsGreater(0);
		}

		/// <summary>
		/// Tests if current power request is on the left side of the shiftpolygon segment
		/// </summary>
		/// <param name="angularSpeed">The angular speed.</param>
		/// <param name="torque">The torque.</param>
		/// <param name="segment">Edge of the shift polygon</param>
		/// <returns><c>true</c> if current power request is on the left side of the shiftpolygon segment; otherwise, <c>false</c>.</returns>
		/// <remarks>Computes a simplified cross product for the vectors: from--X, from--to and checks
		/// if the z-component is negative (which means that X was on the left side of from--to).</remarks>
		public static bool IsRightOf(PerSecond angularSpeed, NewtonMeter torque,
			(ShiftPolygonEntry, ShiftPolygonEntry) segment)
		{
			if (segment.Item1.AngularSpeed > angularSpeed && segment.Item2.AngularSpeed > angularSpeed)
				return false;

			var abX = segment.Item2.AngularSpeed.Value() - segment.Item1.AngularSpeed.Value();
			var abY = segment.Item2.Torque.Value() - segment.Item1.Torque.Value();
			var acX = angularSpeed.Value() - segment.Item1.AngularSpeed.Value();
			var acY = torque.Value() - segment.Item1.Torque.Value();
			var z = abX * acY - abY * acX;
			return z.IsSmaller(0);
		}

		// ReSharper disable once UnusedMember.Global -- used via validation
		public static ValidationResult ValidateShiftPolygon(ShiftPolygon shiftPolygon, ValidationContext validationContext)
		{
			var validationService =
				validationContext.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			var gbxType = validationService?.GearboxType;

			if (gbxType == null || gbxType.Value.AutomaticTransmission()) {
				return ValidationResult.Success;
			}

			return shiftPolygon.Downshift.Pairwise().Any(downshiftLine =>
				shiftPolygon.Upshift.Any(upshiftEntry => IsLeftOf(upshiftEntry.AngularSpeed, upshiftEntry.Torque, downshiftLine)))
				? new ValidationResult("upshift line has to be right of the downshift line!")
				: ValidationResult.Success;
		}

		[DebuggerDisplay("{Torque}, {AngularSpeed}")]
		public class ShiftPolygonEntry
		{
			public ShiftPolygonEntry(NewtonMeter torque, PerSecond angularSpeed)
			{
				Torque = torque;
				AngularSpeed = angularSpeed;
			}

			/// <summary>
			///		[Nm] engine torque
			/// </summary>
			public NewtonMeter Torque { get; set; }

			/// <summary>
			///		[1/s] angular velocity threshold
			/// </summary>
			public PerSecond AngularSpeed { get; set; }
		}

		public NewtonMeter InterpolateDownshift(PerSecond inAngularVelocity)
		{
			var section = Downshift.GetSection(entry => entry.AngularSpeed < inAngularVelocity);

			if (section.Item1.AngularSpeed.IsEqual(section.Item2.AngularSpeed)) {
				// vertical line
				return double.MaxValue.SI<NewtonMeter>();
			}
			return VectoMath.Interpolate(section.Item1.AngularSpeed, section.Item2.AngularSpeed, section.Item1.Torque,
				section.Item2.Torque, inAngularVelocity);
		}

		public PerSecond InterpolateDownshiftSpeed(NewtonMeter torque)
		{
			var section = Downshift.GetSection(entry => entry.Torque < torque);

			if (section.Item1.Torque.IsEqual(section.Item2.Torque)) {
				// vertical line
				return double.MaxValue.SI<PerSecond>();
			}
			return VectoMath.Interpolate(section.Item1.Torque, section.Item2.Torque, section.Item1.AngularSpeed,
				section.Item2.AngularSpeed, torque);
		}

		public PerSecond InterpolateUpshiftSpeed(NewtonMeter torque)
		{
			var section = Upshift.GetSection(entry => entry.Torque < torque);

			if (section.Item1.Torque.IsEqual(section.Item2.Torque)) {
				// vertical line
				return double.MaxValue.SI<PerSecond>();
			}
			return VectoMath.Interpolate(section.Item1.Torque, section.Item2.Torque, section.Item1.AngularSpeed,
				section.Item2.AngularSpeed, torque);
		}
	}
}