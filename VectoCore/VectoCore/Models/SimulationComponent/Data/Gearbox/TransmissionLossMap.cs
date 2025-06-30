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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public sealed class TransmissionLossMap : LoggingObject
	{
		[ValidateObject] internal readonly IReadOnlyList<GearLossMapEntry> _entries;

		/// <summary>
		/// The Loss map. [X=Output EngineSpeed, Y=Output Torque] => Z=Torque Loss
		/// </summary>
		private readonly DelaunayMap _lossMap;

		/// <summary>
		/// The inverted loss map for range sanity checks. [X=Input EngineSpeed, Y=Input Torque] => Z=Output Torque
		/// </summary>
		private readonly DelaunayMap _invertedLossMap;

		public string GearName { get; private set; }

		public double Ratio { get; }


		public string[] LossMapSerialized
		{
			get { return _entries.Select(x => $"{x.InputSpeed.AsRPM} [rpm], {x.InputTorque}, {x.TorqueLoss}").ToArray(); }
		}

		public TransmissionLossMap(IReadOnlyList<GearLossMapEntry> entries, double gearRatio, string gearName, bool createInnvertedMap)
		{
			GearName = gearName;
			Ratio = gearRatio;
			_entries = entries;
			_lossMap = new DelaunayMap("TransmissionLossMap " + GearName);
			_invertedLossMap = createInnvertedMap ? new DelaunayMap("TransmissionLossMapInv. " + GearName) : null;
			foreach (var entry in _entries) {
				_lossMap.AddPoint(entry.InputSpeed.Value(), (entry.InputTorque - entry.TorqueLoss).Value(), entry.TorqueLoss.Value());
				_invertedLossMap?.AddPoint(entry.InputSpeed.Value(), entry.InputTorque.Value(), entry.TorqueLoss.Value());
			}

			_lossMap.Triangulate();
			_invertedLossMap?.Triangulate();
		}

		/// <summary>
		///	Computes the torque loss (input side) given by the output gearbox speed and the output-torque.
		/// </summary>
		/// <param name="outAngularVelocity">Angular speed at output side.</param>
		/// <param name="outTorque">Torque at output side (as requested by the previous componend towards the wheels).</param>
		/// <returns>Torque loss as seen on input side (towards the engine).</returns>
		public LossMapResult GetTorqueLoss(PerSecond outAngularVelocity, NewtonMeter outTorque)
		{
			var result = new LossMapResult();
			var torqueLoss = _lossMap.Interpolate(outAngularVelocity.Value() * Ratio, outTorque.Value() / Ratio);

			if (torqueLoss.IsNaN()) {
				torqueLoss = _lossMap.Extrapolate(outAngularVelocity.Value() * Ratio, outTorque.Value() / Ratio);
				result.Extrapolated = true;
			}

			result.Value = torqueLoss.SI<NewtonMeter>();

			Log.Debug("GearboxLoss {0}: {1}, outAngularVelocity: {2}, outTorque: {3}", GearName, torqueLoss,
				outAngularVelocity, outTorque);
			return result;
		}

		[DebuggerDisplay("{Value} (extrapolated: {Extrapolated})")]
		public class LossMapResult
		{
			public bool Extrapolated;
			public NewtonMeter Value;
		}
		/// <summary>
		/// Computes the OUTPUT torque and speed given by the input speed and input torque
		/// </summary>
		/// <param name="inAngularVelocity"></param>
		/// <param name="inTorque"></param>
		/// <param name="allowExtrapolation"></param>
		/// <returns></returns>
		public (NewtonMeter outTorque, PerSecond outAngularVelocity) GetOutTorqueAndSpeed(PerSecond inAngularVelocity,
			NewtonMeter inTorque, bool allowExtrapolation = false)
		{
			var outTorque = this.GetOutTorque(inAngularVelocity, inTorque, allowExtrapolation);
			var outAngularVelocity = inAngularVelocity / Ratio;

			var torqueLoss = this.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorqueBackward = GetInTorque(outAngularVelocity, outTorque);

			if(!inTorque.IsEqual(inTorqueBackward, 1E-8.SI<NewtonMeter>()))
			{
				Log.Debug($"{nameof(TransmissionLossMap)}: Forward Calculation and Backward Calculation of DriveTorque do not match...");
				//Search outTorque:
				Log.Debug("Forward Calculation and Backward Calculation do not match...");
				outTorque = SearchAlgorithm.Search(inTorque, (inTorqueBackward - inTorque) * 1e3, outTorque / 10,
					getYValue: r => ((r as NewtonMeter) - inTorque) * 1e3,
					evaluateFunction: x => GetInTorque(outAngularVelocity, x),
					criterion: r => {
						var i = r as NewtonMeter;
						return (i - inTorque).Value() * 1e3;
					},
					searcher: this);
			}

			return (outTorque, outAngularVelocity);
		}

		public NewtonMeter GetInTorque(PerSecond outAngularVelocity, NewtonMeter outTorque)
		{
			var torqueLoss = this.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / Ratio + torqueLoss.Value;
			return inTorque;
		}
		

		///  <summary>
		/// 	Computes the OUTPUT torque given by the input engineSpeed and the input torque.
		///  </summary>
		///  <param name="inAngularVelocity">Angular speed at input side.</param>
		///  <param name="inTorque">Torque at input side.</param>
		/// <param name="allowExtrapolation"></param>
		/// <returns>Torque needed at output side (towards the wheels).</returns>
		public NewtonMeter GetOutTorque(PerSecond inAngularVelocity, NewtonMeter inTorque, bool allowExtrapolation = false)
		{
			if (_invertedLossMap == null) {
				throw new VectoException("Required inverted Loss-Map not available");
			}
			var torqueLoss = _invertedLossMap.Interpolate(inAngularVelocity.Value(), inTorque.Value());
			if (!torqueLoss.IsNaN()) {
				return (inTorque - torqueLoss.SI<NewtonMeter>()) * Ratio;
			}

			if (allowExtrapolation) {
				torqueLoss = _invertedLossMap.Extrapolate(inAngularVelocity.Value(), inTorque.Value());
				return (inTorque - torqueLoss.SI<NewtonMeter>()) * Ratio;
			}

			throw new VectoException("TransmissionLossMap {0}: Interpolation failed. inTorque: {1}, inAngularVelocity: {2}",
				GearName, inTorque,
				inAngularVelocity.AsRPM);
		}

		public GearLossMapEntry this[int i] => _entries[i];

		[DebuggerDisplay("GearLossMapEntry({InputSpeed}, {InputTorque}, {TorqueLoss})")]
		public class GearLossMapEntry
		{
			[Required, SIRange(0, 1000000 * Constants.RPMToRad)]
			public PerSecond InputSpeed { get; private set; }

			//[Required, SIRange(-100000, 100000)]
			public NewtonMeter InputTorque { get; private set; }

			[Required, SIRange(0, 1000000)]
			public NewtonMeter TorqueLoss { get; private set; }

			public GearLossMapEntry(PerSecond inputSpeed, NewtonMeter inputTorque, NewtonMeter torqueLoss)
			{
				InputSpeed = inputSpeed;
				InputTorque = inputTorque;
				TorqueLoss = torqueLoss;
			}
		}
	}
}