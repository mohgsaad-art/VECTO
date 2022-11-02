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

using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	/// <summary>
	/// LossMap for retarder.
	/// </summary>
	public class RetarderLossMap : SimulationComponentData, ILossMap
	{
		[ValidateObject] private readonly RetarderLossEntry[] _entries;
		private PerSecond _minSpeed;
		private PerSecond _maxSpeed;

		protected internal RetarderLossMap(RetarderLossEntry[] entries)
		{
			_entries = entries;
		}

		/// <summary>
		/// Gets the minimal defined speed of the retarder loss map.
		/// </summary>
		public PerSecond MinSpeed
		{
			get { return _minSpeed ?? (_minSpeed = _entries.Min(e => e.RetarderSpeed)); }
		}

		/// <summary>
		/// Gets the maximal defined speed of the retarder loss map.
		/// </summary>
		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = _entries.Max(e => e.RetarderSpeed)); }
		}

		public string[] LossMapSerialized
		{
			get { return _entries.Select(x => $"{x.RetarderSpeed.AsRPM} [rpm], {x.TorqueLoss}").ToArray(); }
		}

		/// <summary>
		/// Calculates the retarder losses.
		/// </summary>
		public NewtonMeter GetTorqueLoss(PerSecond angularVelocity)
		{
			var s = _entries.GetSection(e => e.RetarderSpeed < angularVelocity);
			return VectoMath.Interpolate(s.Item1.RetarderSpeed, s.Item2.RetarderSpeed, s.Item1.TorqueLoss, s.Item2.TorqueLoss,
				angularVelocity);
		}

		public class RetarderLossEntry
		{
			[Required, SIRange(0, double.MaxValue)] public PerSecond RetarderSpeed;
			[Required, SIRange(0, 500)] public NewtonMeter TorqueLoss;
		}
	}
}