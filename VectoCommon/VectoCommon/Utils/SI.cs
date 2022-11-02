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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;

// ReSharper disable ClassNeverInstantiated.Global

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// SI Class for Scalar Values. Converts implicitely to double and is only castable if the SI value has no units.
	/// </summary>
	public class Scalar : SIBase<Scalar>
	{
		private static readonly int[] Units = { 0, 0, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Scalar(double val) : base(val, Units) { }

		public static implicit operator double(Scalar self)
		{
			return self.Val;
		}

		[DebuggerHidden]
		public static Scalar operator +(Scalar si1, Scalar si2)
		{
			return Create(si1.Val + si2.Val);
		}

		[DebuggerHidden]
		public static Scalar operator +(Scalar si1, double si2)
		{
			return Create(si1.Val + si2);
		}

		[DebuggerHidden]
		public static Scalar operator +(double si1, Scalar si2)
		{
			return Create(si1 + si2.Val);
		}

		[DebuggerHidden]
		public static Scalar operator -(Scalar si1, Scalar si2)
		{
			return Create(si1.Val - si2.Val);
		}

		[DebuggerHidden]
		public static Scalar operator -(Scalar si1, double si2)
		{
			return Create(si1.Val - si2);
		}

		[DebuggerHidden]
		public static Scalar operator -(double si1, Scalar si2)
		{
			return Create(si1 - si2.Val);
		}
	}

	/// <summary>
	/// SI Class for Newton [N].
	/// </summary>
	public class Newton : SIBase<Newton>
	{
		private static readonly int[] Units = { 1, 1, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Newton(double val) : base(val, Units) { }

		public override string UnitString => "N";

		[DebuggerHidden]
		public static NewtonMeter operator *(Newton newton, Meter meter)
		{
			return SIBase<NewtonMeter>.Create(newton.Val * meter.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(Newton newton, MeterPerSecond meterPerSecond)
		{
			return SIBase<Watt>.Create(newton.Val * meterPerSecond.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(MeterPerSecond meterPerSecond, Newton newton)
		{
			return SIBase<Watt>.Create(newton.Val * meterPerSecond.Value());
		}
	}

	/// <summary>
	/// SI Class for Radian [] (rad).
	/// </summary>
	public class Radian : SIBase<Radian>
	{
		private static readonly int[] Units = { 0, 0, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Radian(double val) : base(val, Units) { }

		public double ToInclinationPercent() {
			return Math.Tan(Val);
		}
	}

	/// <summary>
	/// SI Class for PerSquareSecond [1/s^2].
	/// </summary>
	public class PerSquareSecond : SIBase<PerSquareSecond>
	{
		private static readonly int[] Units = { 0, 0, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private PerSquareSecond(double val) : base(val, Units) { }

		[DebuggerHidden]
		public static PerSecond operator *(PerSquareSecond perSquareSecond, Second second)
		{
			return SIBase<PerSecond>.Create(perSquareSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Meter per square second [m/s^2].
	/// </summary>
	public class MeterPerSquareSecond : SIBase<MeterPerSquareSecond>
	{
		private static readonly int[] Units = { 0, 1, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private MeterPerSquareSecond(double val) : base(val, Units) { }

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSecond operator *(MeterPerSquareSecond meterPerSecond, Second second)
		{
			return SIBase<MeterPerSecond>.Create(meterPerSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Second [s].
	/// </summary>
	public class Second : SIBase<Second>
	{
		private static readonly int[] Units = { 0, 0, 1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Second(double val) : base(val, Units) { }
	}

	/// <summary>
	/// SI Class for Meter [m].
	/// </summary>
	public class Meter : SIBase<Meter>
	{
		private static readonly int[] Units = { 0, 1, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Meter(double val) : base(val, Units) { }

		[DebuggerHidden]
		public static MeterPerSecond operator /(Meter meter, Second second)
		{
			return SIBase<MeterPerSecond>.Create(meter.Val / second.Value());
		}

		[DebuggerHidden]
		public static MeterPerSecond operator *(Meter meter, PerSecond perSecond)
		{
			return SIBase<MeterPerSecond>.Create(meter.Val * perSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static Second operator /(Meter second, MeterPerSecond meterPerSecond)
		{
			return SIBase<Second>.Create(second.Val / meterPerSecond.Value());
		}

		public static SquareMeter operator *(Meter m1, Meter m2)
		{
			return SIBase<SquareMeter>.Create(m1.Val * m2.Val);
		}

		public static CubicMeter operator *(SquareMeter m1, Meter m2)
		{
			return SIBase<CubicMeter>.Create(m1.Value() * m2.Val);
		}
	}

	public class PerSquareMeter : SIBase<PerSquareMeter>
	{
		private static readonly int[] Units = { 0, -2, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private PerSquareMeter(double val) : base(val, Units) { }

		
	}

	public class SquareMeterPerMeter : SIBase<SquareMeterPerMeter>
	{
		private static readonly int[] Units = { 0, 1, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private SquareMeterPerMeter(double val) : base(val, Units) { }

		public static SquareMeter operator *(SquareMeterPerMeter m1, Meter m2)
		{
			return SIBase<SquareMeter>.Create(m1.Val * m2.Value());
		}
	}

	/// <summary>
	///  SI Class for KilogramPerMeter [kg/m].
	/// </summary>
	public class KilogramPerMeter : SIBase<KilogramPerMeter>
	{
		private static readonly int[] Units = { 1, -1, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private KilogramPerMeter(double val) : base(val, Units) { }

		public override string UnitString => "kg/m";

		public static KilogramPerMeterMass operator /(KilogramPerMeter kpm, Kilogram kg)
		{
			return SIBase<KilogramPerMeterMass>.Create(kpm.Val / kg.Value());
		}

		public static KilogramPerMeterCubicMeter operator /(KilogramPerMeter kpm, CubicMeter vol)
		{
			return SIBase<KilogramPerMeterCubicMeter>.Create(kpm.Val / vol.Value());
		}
	}

	/// <summary>
	/// SI Class for Liter per Second [l/s].
	/// </summary>
	public class LiterPerSecond : SIBase<LiterPerSecond>
	{
		private static readonly int[] Units = { 0, 3, -1, 0, 0, 0, 0 };

		private LiterPerSecond(double val) : base(val, 0.001, Units) { }

		public override string UnitString => "l/s";

		[DebuggerHidden]
		public static Liter operator *(LiterPerSecond l, Second second)
		{
			return SIBase<Liter>.Create(l.Val * second.Value());
		}

		[DebuggerHidden]
		public static Liter operator *(Second second, LiterPerSecond l)
		{
			return SIBase<Liter>.Create(l.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Kilogram [kg].
	/// </summary>
	public class Kilogram : SIBase<Kilogram>
	{
		private static readonly int[] Units = { 1, 0, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Kilogram(double val) : base(val, Units) { }

		[DebuggerHidden]
		public static KilogramPerSecond operator /(Kilogram kg, Second second)
		{
			return SIBase<KilogramPerSecond>.Create(kg.Val / second.Value());
		}

		//[DebuggerHidden]
		//public static SI operator /(Kilogram kg, Joule j)
		//{
		//    return (kg as SI) / j;
		//}

		[DebuggerHidden]
		public static Scalar operator /(Kilogram kg, Kilogram kg2)
		{
			return SIBase<Scalar>.Create(kg.Val / kg2.Val);
		}

		[DebuggerHidden]
		public static KilogramPerMeter operator /(Kilogram kg, Meter m)
		{
			return SIBase<KilogramPerMeter>.Create(kg.Val / m.Value());
		}

		[DebuggerHidden]
		public static SpecificFuelConsumption operator /(Kilogram kg, WattSecond ws)
		{
			return SIBase<SpecificFuelConsumption>.Create(kg.Val / ws.Value());
		}

		[DebuggerHidden]
		public static Newton operator *(Kilogram kg, MeterPerSquareSecond m)
		{
			return SIBase<Newton>.Create(kg.Val * m.Value());
		}

		[DebuggerHidden]
		public static Kilogram operator *(Kilogram kg, double d)
		{
			return new Kilogram(kg.Val * d);
		}

		[DebuggerHidden]
		public static Kilogram operator *(double d, Kilogram kg)
		{
			return new Kilogram(d * kg.Val);
		}

		public static CubicMeter operator /(Kilogram kilogram, KilogramPerCubicMeter kilogramPerCubicMeter)
		{
			return SIBase<CubicMeter>.Create(kilogram.Value() / kilogramPerCubicMeter.Value());
		}
	}

	public class Liter : SIBase<Liter>
	{
		private static readonly int[] Units = { 0, 3, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		//[DebuggerHidden]
		private Liter(double val) : base(val , 0.001, Units) { }

		public override string UnitString => "l";

		public static Kilogram operator *(Liter liter, KilogramPerCubicMeter kilogramPerCubicMeter)
		{
			return SIBase<Kilogram>.Create(liter.AsBasicUnit * kilogramPerCubicMeter.Value());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NormLiter : SIBase<NormLiter>
	{
		private static readonly int[] Units = { 0, 3, 0, 0, 0, 0, 0 };

		//[DebuggerHidden]
		private NormLiter(double val) : base(val , 0.001, Units) { }

		public override string UnitString => "Nl";

		public static NormLiterPerSecond operator /(NormLiter nl, Second s)
		{
			return SIBase<NormLiterPerSecond>.Create(nl.Val / s.Value());
		}
	}

	public class NormLiterPerKilogram : SIBase<NormLiterPerKilogram>
	{
		private static readonly int[] Units = { -1, 3, 0, 0, 0, 0, 0 };

		//[DebuggerHidden]
		private NormLiterPerKilogram(double val) : base(val , 0.001, Units) { }

		public override string UnitString => "Nl/kg";

		public static NormLiter operator *(NormLiterPerKilogram nlpkg, Kilogram kg)
		{
			return SIBase<NormLiter>.Create(nlpkg.Val * kg.Value());
		}
	}

	
	public class NormLiterPerKilogramMeter : SIBase<NormLiterPerKilogramMeter>
	{
		private static readonly int[] Units = { -1, 2, 0, 0, 0, 0, 0 };

		//[DebuggerHidden]
		private NormLiterPerKilogramMeter(double val) : base(val , 0.001, Units) { }

		public override string UnitString => "Nl/kgm";

		public static NormLiterPerKilogram operator *(NormLiterPerKilogramMeter nlpkgm, Meter m)
		{
			return SIBase<NormLiterPerKilogram>.Create(nlpkgm.Val * m.Value());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NormLiterPerSecond : SIBase<NormLiterPerSecond>
	{
		private static readonly int[] Units = { 0, 3, -1, 0, 0, 0, 0 };

		//[DebuggerHidden]
		private NormLiterPerSecond(double val) : base(val, 0.001, Units) { }

		public override string UnitString => "Nl/s";

		public static NormLiter operator *(NormLiterPerSecond nips, Second s)
		{
			return SIBase<NormLiter>.Create(nips.Val * s.Value());
		}

		public static NormLiterPerSecond operator *(NormLiterPerSecond nps, double val)
		{
			return Create(nps.Val  * val);
		}

		public static NormLiterPerSecond operator /(NormLiterPerSecond nps, double val)
		{
			return Create(nps.Val / val);
		}
	}

	/// <summary>
	/// SI Class for Kilogram per Second [kg].
	/// </summary>
	public class KilogramPerSecond : SIBase<KilogramPerSecond>
	{
		private static readonly int[] Units = { 1, 0, -1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private KilogramPerSecond(double value) : base(value, Units) { }

		[DebuggerHidden]
		public static Kilogram operator *(KilogramPerSecond kilogramPerSecond, Second second)
		{
			return SIBase<Kilogram>.Create(kilogramPerSecond.Val * second.Value());
		}

		[DebuggerHidden]
		public static Kilogram operator *(Second second, KilogramPerSecond kilogramPerSecond)
		{
			return SIBase<Kilogram>.Create(kilogramPerSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Square meter [m^2].
	/// </summary>
	public class SquareMeter : SIBase<SquareMeter>
	{
		private static readonly int[] Units = { 0, 2, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private SquareMeter(double value) : base(value, Units) { }

		public static double operator *(SquareMeter sqm, PerSquareMeter psqm)
		{
			return sqm.Val * psqm.Value();
		}
	}

	/// <summary>
	/// SI Class for cubic meter [m^3].
	/// </summary>
	public class CubicMeter : SIBase<CubicMeter>
	{
		private static readonly int[] Units = { 0, 3, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private CubicMeter(double value)
			: base(value, Units) { }

		public static CubicMeterPerSecond operator *(CubicMeter m3, PerSecond ps)
		{
			return SIBase<CubicMeterPerSecond>.Create(m3.Val * ps.Value());
		}
	}

	public class CubicMeterPerSecond : SIBase<CubicMeterPerSecond>
	{
		private static readonly int[] Units = { 0, 3, -1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private CubicMeterPerSecond(double value)
			: base(value, Units) { }

		public static Watt operator *(CubicMeterPerSecond m3ps, WattSecondPerCubicMeter wspm3)
		{
			return SIBase<Watt>.Create(m3ps.Val * wspm3.Value());
		}

		public static Watt operator *(CubicMeterPerSecond m3ps, JoulePerCubicMeter jpm3)
		{
			return SIBase<Watt>.Create(m3ps.Val * jpm3.Value());
		}
	}

	/// <summary>
	/// SI Class for Kilogram Square Meter [kgm^2].
	/// </summary>
	public class KilogramSquareMeter : SIBase<KilogramSquareMeter>
	{
		private static readonly int[] Units = { 1, 2, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private KilogramSquareMeter(double value) : base(value, Units) { }

		[DebuggerHidden]
		public static NewtonMeter operator *(KilogramSquareMeter kilogramSquareMeter, PerSquareSecond perSquareSecond)
		{
			return SIBase<NewtonMeter>.Create(kilogramSquareMeter.Val * perSquareSecond.Value());
		}
	}

	/// <summary>
	/// SI Class for Kilogram per Cubic Meter [kg/m^3].
	/// </summary>
	public class KilogramPerCubicMeter : SIBase<KilogramPerCubicMeter>
	{
		private static readonly int[] Units = { 1, -3, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private KilogramPerCubicMeter(double value) : base(value, Units) { }

		public override string UnitString => "kg/m^3";

		[DebuggerHidden]
		public static Kilogram operator *(KilogramPerCubicMeter kilogramPerCubicMeter, CubicMeter cubicMeter)
		{
			return SIBase<Kilogram>.Create(kilogramPerCubicMeter.Val * cubicMeter.Value());
		}

	}

	/// <summary>
	/// SI Class for Kilogramm per watt second [kg/Ws].
	/// W = kgm^2/s^3
	/// </summary>
	public class KilogramPerWattSecond : SIBase<KilogramPerWattSecond>
	{
		private static readonly int[] Units = { 0, -2, 2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private KilogramPerWattSecond(double val) : base(val, Units) { }

		public override string UnitString => "kg/Ws";

		public static Kilogram operator *(KilogramPerWattSecond kpws, WattSecond ws)
		{
			return SIBase<Kilogram>.Create(kpws.Val * ws.Value());
		}
	}

	/// <summary>
	/// SI Class for watt second [Ws].
	/// W = kgm^2/s^3
	/// </summary>
	public class WattSecond : SIBase<WattSecond>
	{
		private static readonly int[] Units = { 1, 2, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private WattSecond(double val) : base(val, Units) { }

		public override string UnitString => "Ws";

		[DebuggerHidden]
		public static Watt operator /(WattSecond wattSecond, Second second)
		{
			return SIBase<Watt>.Create(wattSecond.Val / second.Value());
		}

		[DebuggerHidden]
		public static AmpereSecond operator /(WattSecond wattSecond, Volt volt)
		{
			return SIBase<AmpereSecond>.Create(wattSecond.Val / volt.Value());
		}

		public static Kilogram operator *(WattSecond ws, KilogramPerWattSecond kpws)
		{
			return SIBase<Kilogram>.Create(ws.Val * kpws.Value());
		}
	}

	public class WattSecondPerCubicMeter : SIBase<WattSecondPerCubicMeter>
	{
		private static readonly int[] Units = { 1, -1, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private WattSecondPerCubicMeter(double val) : base(val, Units) { }
	}

	public class WattPerKelvinSquareMeter : SIBase<WattPerKelvinSquareMeter>
	{
		private static readonly int[] Units = { 1, 0, -3, 0, -1, 0, 0 };

		private WattPerKelvinSquareMeter(double val) : base(val, Units)
		{ }

		public override string UnitString => "W/Km^2";
	}

	public class WattPerSquareMeter : SIBase<WattPerSquareMeter>
	{
		private static readonly int[] Units = { 1, 0, -3, 0, 0, 0, 0 };

		private WattPerSquareMeter(double val) : base(val, Units)
		{ }

		public static Watt operator *(WattPerSquareMeter wpsqm, SquareMeter sqm)
		{
			return SIBase<Watt>.Create(wpsqm.Val * sqm.Value());
		}
		public override string UnitString => "W/m^2";
	}


	public class WattPerCubicMeter : SIBase<WattPerCubicMeter>
	{
		private static readonly int[] Units = { 1, -1, -3, 0, 0, 0, 0 };

		private WattPerCubicMeter(double val) : base(val, Units)
		{ }

		public override string UnitString => "W/m^3";

		public static Watt operator *(WattPerCubicMeter wpcm, CubicMeter cm)
		{
			return SIBase<Watt>.Create(wpcm.Val * cm.Value());
		}
	}

	/// <summary>
	/// SI Class for Watt [W].
	/// </summary>
	public class Watt : SIBase<Watt>
	{
		private static readonly int[] Units = { 1, 2, -3, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Watt(double val) : base(val, Units) { }

		public override string UnitString => "W";

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		/// <param name="watt">The watt.</param>
		/// <param name="newtonMeter">The newton meter.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static PerSecond operator /(Watt watt, NewtonMeter newtonMeter)
		{
			return SIBase<PerSecond>.Create(watt.Val / newtonMeter.Value());
		}

		[DebuggerHidden]
		public static Newton operator /(Watt watt, MeterPerSecond meterPerSecond)
		{
			return SIBase<Newton>.Create(watt.Val / meterPerSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		/// <param name="watt">The watt.</param>
		/// <param name="perSecond">The per second.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static NewtonMeter operator /(Watt watt, PerSecond perSecond)
		{
			return SIBase<NewtonMeter>.Create(watt.Val / perSecond.Value());
		}

		[DebuggerHidden]
		public static WattSecond operator *(Watt watt, Second second)
		{
			return SIBase<WattSecond>.Create(watt.Val * second.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(Watt watt, double val)
		{
			return Create(watt.Val * val);
		}

		[DebuggerHidden]
		public static KilogramPerSecond operator /(Watt watt, JoulePerKilogramm jpkg)
		{
			return SIBase<KilogramPerSecond>.Create(watt.Val / jpkg.Value());
		}

		[DebuggerHidden]
		public static JoulePerNormLiter operator /(Watt watt, NormLiterPerSecond nlps)
		{
			return SIBase<JoulePerNormLiter>.Create(watt.Val / nlps.Value());
		}
	}

	/// <summary>
	/// SI Class for Joule [J].
	/// J = Ws = kgm^2/s^2
	/// </summary>
	public class Joule : SIBase<Joule>
	{
		private static readonly int[] Units = { 1, 2, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Joule(double val) : base(val, Units) { }

		public override string UnitString => "J";

		public static implicit operator Joule(WattSecond self)
		{
			return Create(self.Value());
		}

		public static Joule operator +(Joule joule, WattSecond ws)
		{
			return Create(joule.Val + ws.Value());
		}

		public static Watt operator /(Joule joule, Second s)
		{
			return SIBase<Watt>.Create(joule.Val / s.Value());
		}

		public static JoulePerMeter operator /(Joule joule, Meter meter)
		{
			return SIBase<JoulePerMeter>.Create(joule.Val / meter.Value());
		}

		public static Kilogram operator /(Joule j, JoulePerKilogramm jpkg)
		{
			return SIBase<Kilogram>.Create(j.Val /  jpkg.Value());
		}
	}

	public class JoulePerNormLiter : SIBase<JoulePerNormLiter>
	{
		private static readonly int[] Units = { 1, -1, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private JoulePerNormLiter(double val) : base(val, Units) { }

		public override string UnitString => "J/Nl";

		public static Watt operator *(JoulePerNormLiter jpnl, NormLiterPerSecond nlps)
		{
			return SIBase<Watt>.Create(jpnl.Val * nlps.Value());
		}
	}

	/// <summary>
		/// SI Class for Joule / kg.
		/// </summary>
		public class JoulePerKilogramm : SIBase<JoulePerKilogramm>
	{
		private static readonly int[] Units = { 0, 2, -2, 0, 0, 0, 0 };

		private JoulePerKilogramm(double val) : base(val, Units) { }

		public override string UnitString => "J/kg";

		public static Joule operator *(Kilogram kg, JoulePerKilogramm jpg)
		{
			return SIBase<Joule>.Create(kg.Value() * jpg.Val);
		}
		
		public static JoulePerCubicMeter operator *(JoulePerKilogramm jpk, KilogramPerCubicMeter kpm3)
		{
			return SIBase<JoulePerCubicMeter>.Create(jpk.Val * kpm3.Value());
		}
	}

	public class JoulePerCubicMeter : SIBase<JoulePerCubicMeter>
	{
		private static readonly int[] Units = { 1, -1, -2, 0, 0, 0, 0 };

		private JoulePerCubicMeter(double val) : base(val, Units) { }

		//public static CubicMeterPerSecond operator /(Watt w, JoulePerCubicMeter cpm3)
		//{
		//	return SIBase<CubicMeterPerSecond>.Create(w.Value() * cpm3.Val);
		//}
	}

	/// <summary>
	///  SI Class for Joule per Meter [J/m].
	///  J = Ws
	///  W = kgm^2/s^3
	/// </summary>
	public class JoulePerMeter : SIBase<JoulePerMeter>
	{
		private static readonly int[] Units = { 1, 1, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private JoulePerMeter(double val) : base(val, Units) { }

		public override string UnitString => "J/m";

		public static KilogramPerMeter operator /(JoulePerMeter jpm, JoulePerKilogramm jpkg)
		{
			return SIBase<KilogramPerMeter>.Create(jpm.Val / jpkg.Value());
		}
	}

	/// <summary>
	/// SI Class for one per second [1/s].
	/// </summary>
	[DebuggerDisplay("{Val.ToString(\"F1\"),nq} [rad/s] ({AsRPM.ToString(\"F1\"),nq} [rpm)")]
	public class PerSecond : SIBase<PerSecond>
	{
		private static readonly int[] Units = { 0, 0, -1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private PerSecond(double val) : base(val, Units) { }

		[DebuggerHidden]
		public static PerSquareSecond operator /(PerSecond perSecond, Second second)
		{
			return SIBase<PerSquareSecond>.Create(perSecond.Val / second.Value());
		}

		public static MeterPerSecond operator *(PerSecond perSecond, Meter meter)
		{
			return SIBase<MeterPerSecond>.Create(perSecond.Val * meter.Value());
		}
		
		public double AsRPM => Val * 60 / (2 * Math.PI);
	}

	/// <summary>
	/// SI Class for Meter per second [m/s].
	/// </summary>
	[DebuggerDisplay("{Val.ToString(\"G4\"),nq} [m/s] ({AsKmph.ToString(\"G4\"),nq} [km/h])")]
	public class MeterPerSecond : SIBase<MeterPerSecond>
	{
		private static readonly int[] Units = { 0, 1, -1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private MeterPerSecond(double val) : base(val, Units) { }

		public double AsKmph => Val * 3.6;

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static PerSecond operator /(MeterPerSecond meterPerSecond, Meter meter)
		{
			return SIBase<PerSecond>.Create(meterPerSecond.Val / meter.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static Second operator /(MeterPerSecond meterPerSecond, MeterPerSquareSecond meterPerSquareSecond)
		{
			return SIBase<Second>.Create(meterPerSecond.Val / meterPerSquareSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSquareSecond operator /(MeterPerSecond meterPerSecond, Second second)
		{
			return SIBase<MeterPerSquareSecond>.Create(meterPerSecond.Val / second.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static Meter operator *(MeterPerSecond meterPerSecond, Second second)
		{
			return SIBase<Meter>.Create(meterPerSecond.Val * second.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSquareSecond operator *(MeterPerSecond meterPerSecond, PerSecond perSecond)
		{
			return SIBase<MeterPerSquareSecond>.Create(meterPerSecond.Val * perSecond.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static Meter operator *(Second second, MeterPerSecond meterPerSecond)
		{
			return SIBase<Meter>.Create(second.Value() * meterPerSecond.Val);
		}
	}

	/// <summary>
	/// SI Class for NewtonMeter [Nm].
	/// Nm = kgm^2/s^2
	/// </summary>
	public class NewtonMeter : SIBase<NewtonMeter>
	{
		private static readonly int[] Units = { 1, 2, -2, 0, 0, 0, 0 };

		[DebuggerHidden]
		private NewtonMeter(double val) : base(val, Units) { }

		public override string UnitString => "Nm";

		[DebuggerHidden]
		public static Watt operator *(NewtonMeter newtonMeter, PerSecond perSecond)
		{
			return SIBase<Watt>.Create(newtonMeter.Val * perSecond.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(PerSecond perSecond, NewtonMeter newtonMeter)
		{
			return SIBase<Watt>.Create(perSecond.Value() * newtonMeter.Val);
		}

		[DebuggerHidden]
		public static Second operator /(NewtonMeter newtonMeter, Watt watt)
		{
			return SIBase<Second>.Create(newtonMeter.Val / watt.Value());
		}

		[DebuggerHidden]
		public static PerSquareSecond operator /(NewtonMeter newtonMeter, KilogramSquareMeter kgKilogramSquareMeter)
		{
			return SIBase<PerSquareSecond>.Create(newtonMeter.Val / kgKilogramSquareMeter.Value());
		}

		[DebuggerHidden]
		public static PerSecond operator /(NewtonMeter newtonMeter, NewtonMeterSecond newtonMeterSecond)
		{
			return SIBase<PerSecond>.Create(newtonMeter.Val / newtonMeterSecond.Value());
		}

		[DebuggerHidden]
		public static Newton operator /(NewtonMeter newtonMeter, Meter meter)
		{
			return SIBase<Newton>.Create(newtonMeter.Val / meter.Value());
		}

		[DebuggerHidden]
		public static NewtonMeterSecond operator /(NewtonMeter newtonMeter, PerSecond perSecond)
		{
			return SIBase<NewtonMeterSecond>.Create(newtonMeter.Val / perSecond.Value());
		}

		public static implicit operator Joule(NewtonMeter self) {
			return SIBase<Joule>.Create(self.Val);
		}
	}

	/// <summary>
	/// SI Class for NewtonMeterSecond [Nms].
	/// N = kgm/s^2
	/// </summary>
	public class NewtonMeterSecond : SIBase<NewtonMeterSecond>
	{
		private static readonly int[] Units = { 1, 2, -1, 0, 0, 0, 0 };
		private NewtonMeterSecond(double val) : base(val, Units) { }

		public override string UnitString => "Nms";
	}

	public class Kelvin : SIBase<Kelvin>
	{
		private static readonly int[] Units = { 0, 0, 0, 0, 1, 0, 0 };

		private Kelvin(double val) : base(val, Units) { }

		public double AsDegCelsius => Val - 273.16;

		public static KelvinSquareMeter operator *(Kelvin k, SquareMeter sq)
		{
			return SIBase<KelvinSquareMeter>.Create(k.Val * sq.Value());
		}
	}

	public class KelvinSquareMeter : SIBase<KelvinSquareMeter>
	{
		private static readonly int[] Units = { 0, 2, 0, 0, 1, 0, 0 };

		private KelvinSquareMeter(double val) : base(val, Units) { }

		public static Watt operator *(KelvinSquareMeter ksqm, WattPerKelvinSquareMeter wpksqm)
		{
			return SIBase<Watt>.Create(ksqm.Val * wpksqm.Value());
		}
	}

	/// <summary>
	/// SI Class for Amperer [A].
	/// </summary>
	public class Ampere : SIBase<Ampere>
	{
		private static readonly int[] Units = { 0, 0, 0, 1, 0, 0, 0 };
		private Ampere(double val) : base(val, Units) { }

		public static Watt operator *(Ampere ampere, Volt volt)
		{
			return SIBase<Watt>.Create(volt.Value() * ampere.Val);
		}

		public static Ampere operator *(Ampere ampere, double val)
		{
			return Create(ampere.Val * val);
		}
		
		public static Volt operator /(Watt watt, Ampere ampere)
		{
			return SIBase<Volt>.Create(watt.Value() / ampere.Value());
		}

		public static Volt operator *(Ampere i, Ohm r)
		{
			return SIBase<Volt>.Create(i.Value() * r.Value());
		}

		public static Volt operator *(Ohm r, Ampere i)
		{
			return SIBase<Volt>.Create(i.Value() * r.Value());
		}

		public static AmpereSecond operator *(Ampere i, Second t)
		{
			return SIBase<AmpereSecond>.Create(i.Value() * t.Value());
		}

		
	}

	public class AmpereSecond : SIBase<AmpereSecond>
	{
		private static readonly int[] Units = { 0, 0, 1, 1, 0, 0, 0 };

		private AmpereSecond(double val) : base(val, Units) { }

		public override string UnitString => "As";

		public double AsAmpHour => Val / 3600.0;

		public static Ampere operator /(AmpereSecond ampereSecond, Second t)
		{
			return SIBase<Ampere>.Create(ampereSecond.Value() / t.Value());
		}

		public static WattSecond operator *(AmpereSecond ampereSeconds, Volt v)
		{
			return SIBase<WattSecond>.Create(ampereSeconds.Val * v.Value());
		}
		public static Ampere operator *(AmpereSecond i, PerSecond ps)
		{
			return SIBase<Ampere>.Create(i.Value() * ps.Value());
		}
	}

	/// <summary>
	/// SI Class for Amperer [V].
	/// V = kgm^2/As^2
	/// </summary>
	public class Volt : SIBase<Volt>
	{
		private static readonly int[] Units = { 1, 2, -3, -1, 0, 0, 0 };
		private Volt(double val) : base(val, Units) { }

		public override string UnitString => "V";

		public static Watt operator *(Volt volt, Ampere ampere)
		{
			return SIBase<Watt>.Create(volt.Val * ampere.Value());
		}

		public static Ampere operator /(Watt watt, Volt volt)
		{
			return SIBase<Ampere>.Create(watt.Value() / volt.Value());
		}

		public static Ohm operator /(Volt volt, Ampere current)
		{
			return SIBase<Ohm>.Create(volt.Value() / current.Value());
		}

		public static Ampere operator /(Volt volt, Ohm r)
		{
			return SIBase<Ampere>.Create(volt.Value() / r.Value());
		}
	}

	public class Ohm : SIBase<Ohm>
	{
		private static readonly int[] Units = { 1, 2, -3, -2, 0, 0, 0 };

		private Ohm(double val) : base(val, Units) { }

		public override string UnitString => "Ω";
	}

	public class Farad : SIBase<Farad>
	{
		private static readonly int[] Units = { -1, -2, 4, 2, 0, 0, 0 };

		private Farad(double val) : base(val, Units) {}

		public override  string UnitString => "F";

		public static Volt operator /(AmpereSecond charge, Farad capacity)
		{
			return SIBase<Volt>.Create(charge.Value() / capacity.Val);
		}

		public static AmpereSecond operator *(Farad capacity, Volt v)
		{
			return SIBase<AmpereSecond>.Create(capacity.Val * v.Value());
		}
	}

	public class VolumePerMeter : SIBase<VolumePerMeter>
	{
		private static readonly int[] Units = { 0, 2, 0, 0, 0, 0, 0 };
		private VolumePerMeter(double val) : base(val, Units) { }

		public override string UnitString => "m^3/m";

		public static VolumePerMeterMass operator /(VolumePerMeter vpm, Kilogram kg)
		{
			return SIBase<VolumePerMeterMass>.Create(vpm.Val / kg.Value());
		}

		public static VolumePerMeterVolume operator /(VolumePerMeter vpm, CubicMeter vol)
		{
			return SIBase<VolumePerMeterVolume>.Create(vpm.Val / vol.Value());
		}

	}

	public class VolumePerMeterMass : SIBase<VolumePerMeterMass>
	{
		private static readonly int[] Units = { -1, 2, 0, 0, 0, 0, 0 };

		private VolumePerMeterMass(double val) : base (val, Units) { }

		public override string UnitString => "m^3/kgm";
	}

	public class VolumePerMeterVolume : SIBase<VolumePerMeterVolume>
	{
		private static readonly int[] Units = { 0, -1, 0, 0, 0, 0, 0 };

		private VolumePerMeterVolume(double val) : base (val, Units) { }

		public override string UnitString => "m^3/kgm^3";
	}

	public class KilogramPerMeterCubicMeter : SIBase<KilogramPerMeterCubicMeter>
	{
		private static readonly int[] Units = { 1, -4, 0, 0, 0, 0, 0 };

		private KilogramPerMeterCubicMeter(double val) : base(val, Units) { }

		public override string UnitString => "kg/(m m^3)";
	}


	public class KilogramPerMeterMass : SIBase<KilogramPerMeterMass>
	{
		private static readonly int[] Units = { 0, -1, 0, 0, 0, 0, 0 };

		private KilogramPerMeterMass(double val) : base(val, Units) { }

		public override string UnitString => "kg/(m kg)";
	}

	public class SpecificFuelConsumption : SIBase<SpecificFuelConsumption>
	{
		private static readonly int[] Units = { 0, -2,2, 0, 0, 0, 0 };

		private SpecificFuelConsumption(double val) : base(val, Units) { }

		public override string UnitString => "kg/Ws";
	}

	/// <summary>
	/// Base Class for all special SI Classes. Not intended to be used directly.
	/// Implements templated operators for type safety and convenience.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SIBase<T> : SI where T : SIBase<T>
	{
		private static readonly T ZeroPrototype;

		static SIBase()
		{
			const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			var constructorInfo = typeof(T).GetConstructor(bindingFlags, null, new[] { typeof(double) }, null);
			var parameter = Expression.Parameter(typeof(double));
			var lambda = Expression.Lambda<Func<double, T>>(Expression.New(constructorInfo, parameter), parameter);
			Constructor = lambda.Compile();
			ZeroPrototype = Constructor(0);
		}

		/// <summary>
		/// The constructor for the generic type T.
		/// </summary>
		private static readonly Func<double, T> Constructor;

		/// <summary>
		/// Creates the specified special SI object.
		/// </summary>
		/// <param name="val">The value of the SI object.</param>
		[DebuggerStepThrough]
		public static T Create(double val)
		{
			if (val == 0) {
				return ZeroPrototype;
			}

			return Constructor(val);
		}

		[DebuggerStepThrough]
		protected SIBase(double value, int[] units) : base(value, units) { }

		protected SIBase(double value, double unitFactor, int[] units) : base(value, unitFactor, units) { }

		[DebuggerStepThrough]
		public new T Abs()
		{
			return Create(Math.Abs(Val));
		}

		#region Operators

		/// <summary>
		/// Implements the operator + for two specialized SI Classes.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SIBase<T> si1, SIBase<T> si2)
		{
			return Create(si1.Val + si2.Val);
		}

		/// <summary>
		/// Implements the operator + for a specialized SI Class and a generic SI Class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SIBase<T> si1, SI si2)
		{
			return ((si1 as SI) + si2).Cast<T>();
		}

		/// <summary>
		/// Implements the operator + for a generic SI Class and a specialized SI Class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SI si1, SIBase<T> si2)
		{
			return (si1 + (si2 as SI)).Cast<T>();
		}

		/// <summary>
		/// Implements the unary operator -.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1)
		{
			return Create(-si1.Val);
		}

		/// <summary>
		/// Implements the operator - for two specialized SI classes.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1, SIBase<T> si2)
		{
			return Create(si1.Val - si2.Val);
		}

		/// <summary>
		/// Implements the operator - for a specialized SI class and a generic SI class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1, SI si2)
		{
			return ((si1 as SI) - si2).Cast<T>();
		}

		/// <summary>
		/// Implements the operator - for a generic SI class and a specialized SI class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SI si1, SIBase<T> si2)
		{
			return (si1 - (si2 as SI)).Cast<T>();
		}

		/// <summary>
		/// Implements the operator * for a double and a specialized SI class.
		/// </summary>
		/// <param name="d">The double value.</param>
		/// <param name="si">The si.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator *(double d, SIBase<T> si)
		{
			return Create(d * si.Val);
		}

		/// <summary>
		/// Implements the operator * for a specialized SI class and a double.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="d">The double.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator *(SIBase<T> si, double d)
		{
			return Create(si.Val * d);
		}

		/// <summary>
		/// Implements the operator / for a specialized SI class and a double.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="d">The double.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator /(SIBase<T> si, double d)
		{
			return Create(si.Val / d);
		}

		[DebuggerHidden]
		public static Scalar operator /(SIBase<T> si, SIBase<T> si2)
		{
			return SIBase<Scalar>.Create(si.Val / si2.Val);
		}

		#endregion
	}

	/// <summary>
	/// Class for representing generic SI Units.
	/// </summary>
	/// <remarks>
	/// Usage: new SI(1.0).Newton.Meter, new SI(2.3).Rounds.Per.Minute
	/// </remarks>
	[DebuggerDisplay("{SerializedValue,nq}")]
	public class SI : IComparable
	{
		/// <summary>
		/// The basic scalar value of the SI.
		/// </summary>
		protected readonly double Val;

		/// <summary>
		/// The array of the SI units.
		/// </summary>
		private readonly int[] _units;

		private double UnitFactor;

		/// <summary>
		/// Initializes a new instance of the <see cref="SI"/> class which allows to construct a new SI with all parameters.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="unitFactor"></param>
		/// <param name="units">The units.</param>
		//[DebuggerHidden]
		protected SI(double val, double unitFactor, int[] units)
		{
			Val = val;
			_units = units;
			UnitFactor = unitFactor;

			if (double.IsNaN(Val)) {
				throw new VectoException("NaN [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
			}

			if (double.IsInfinity(Val)) {
				throw new VectoException("Infinity [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
			}
		}
		[DebuggerHidden]
		protected SI(double val, int[] units) : this(val, 1, units) { }

		[DebuggerHidden]
		public SI(UnitInstance si, double val = 0) : this(val * si.Factor, si.GetSIUnits()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SI"/> class which copies the units from an already existing SI.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="unit">The unit.</param>
		[DebuggerHidden]
		private SI(double val, SI unit) : this(val, unit.UnitFactor,unit._units) { }

		/// <summary>
		/// Casts the SI Unit to the concrete unit type (if the units allow such an cast).
		/// </summary>
		/// <typeparam name="T">the specialized SI unit. e.g. Watt, NewtonMeter, Second</typeparam>
		[DebuggerHidden]
		public T Cast<T>() where T : SIBase<T> {
			if (this is T t)
				return t;

			var si = ToBasicUnits();
			var zero = SIBase<T>.Create(0);
			var casted = SIBase<T>.Create(si.Val / zero.UnitFactor);
			if (!si.HasEqualUnit(casted)) {
				throw new VectoException("SI Unit Conversion failed: From {0} to {1}", si, casted);
			}
			return casted;
		}
		
		/// <summary>
		/// Converts the derived SI units to the basic units and returns this as a new SI object.
		/// </summary>
		[DebuggerHidden]
		public SI ToBasicUnits()
		{
			return new SI(Val * UnitFactor, _units);
		}

		[DebuggerHidden]
		protected double AsBasicUnit => Val * UnitFactor;


		/// <summary>
		/// Gets the underlying scalar double value.
		/// </summary>
		[DebuggerHidden]
		public double Value()
		{
			return Val;
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		[DebuggerHidden]
		public SI Clone()
		{
			return new SI(Val, _units);
		}

		/// <summary>
		/// Returns the absolute value.
		/// </summary>
		[DebuggerHidden]
		public SI Abs()
		{
			return new SI(Math.Abs(Val), this);
		}

		/// <summary>
		/// Returns the numerical sign of the SI.
		/// </summary>
		/// <returns>-1 if si &lt; 0. 0 if si==0, 1 if si &gt; 0.</returns>
		[DebuggerHidden]
		public int Sign()
		{
			return Math.Sign(Val);
		}

		#region Operators

		[DebuggerHidden]
		public static SI operator +(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '+' can only operate on SI Objects with the same unit. Got: {0} + {1}", si1, si2);
			}


			return new SI(si1.Val + si2.Val, si1);
		}

		[DebuggerHidden]
		public static SI operator -(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '-' can only operate on SI Objects with the same unit. Got: {0} - {1}", si1, si2);
			}
			return new SI(si1.Val - si2.Val, si1);
		}

		[DebuggerHidden]
		public static SI operator -(SI si1)
		{
			return new SI(-si1.Val, si1);
		}

		public static SI operator *(SI si1, SI si2)
		{
			var unitArray = SIUtils.CombineUnits(si1._units, si2._units);
			return new SI(si1.AsBasicUnit * si2.AsBasicUnit, unitArray);
		}

		[DebuggerHidden]
		public static SI operator *(SI si1, double d)
		{
			return new SI(si1.Val * d, si1);
		}

		[DebuggerHidden]
		public static SI operator *(double d, SI si1)
		{
			return new SI(d * si1.Val, si1);
		}

		public static SI operator /(SI si1, SI si2)
		{
			double result;
			try {
				result = si1.AsBasicUnit / si2.AsBasicUnit;

				// bad cases: Infinity = x / 0.0  (for x != 0), NaN = 0.0 / 0.0
				if (double.IsInfinity(result) || double.IsNaN(result)) {
					throw new DivideByZeroException();
				}
			} catch (DivideByZeroException ex) {
				throw new VectoException(
					$"Can not compute division by zero ([{si1.UnitString}] / 0[{si2.UnitString}])", ex);
			}

			var unitArray = SIUtils.CombineUnits(si1._units, SIUtils.MultiplyUnits(si2._units, -1));

			return new SI(result, unitArray);
		}

		[DebuggerHidden]
		public static SI operator /(SI si1, double d)
		{
			if (d.IsEqual(0)) {
				throw new VectoException($"Can not compute division by zero ([{si1.UnitString}] / 0)", new DivideByZeroException());
			}

			return new SI(si1.Val / d, si1);
		}

		[DebuggerHidden]
		public static SI operator /(double d, SI si1)
		{
			if (si1.IsEqual(0)) {
				throw new VectoException($"Can not compute division by zero (x / 0[{si1.UnitString}])",
					new DivideByZeroException());
			}

			return new SI(d / si1.AsBasicUnit, si1._units.Select(u => -u).ToArray());
		}

		[DebuggerHidden]
		public static bool operator <(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '<' can only operate on SI Objects with the same unit. Got: {0} < {1}", si1, si2);
			}
			return si1.AsBasicUnit < si2.AsBasicUnit;
		}

		[DebuggerHidden]
		public static bool operator <(SI si1, double d)
		{
			return si1 != null && si1.Val < d;
		}

		[DebuggerHidden]
		public static bool operator >(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '>' can only operate on SI Objects with the same unit. Got: {0} > {1}", si1, si2);
			}
			return si1.AsBasicUnit > si2.AsBasicUnit;
		}

		[DebuggerHidden]
		public static bool operator >(SI si1, double d)
		{
			return si1 != null && si1.Val > d;
		}

		[DebuggerHidden]
		public static bool operator >(double d, SI si1)
		{
			return si1 != null && d > si1.Val;
		}

		[DebuggerHidden]
		public static bool operator <(double d, SI si1)
		{
			return si1 != null && d < si1.Val;
		}

		[DebuggerHidden]
		public static bool operator <=(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '<=' can only operate on SI Objects with the same unit. Got: {0} <= {1}", si1,
					si2);
			}
			return si1.AsBasicUnit <= si2.AsBasicUnit;
		}

		[DebuggerHidden]
		public static bool operator <=(SI si1, double d)
		{
			return si1 != null && si1.Val <= d;
		}

		[DebuggerHidden]
		public static bool operator >=(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '>=' can only operate on SI Objects with the same unit. Got: {0} >= {1}", si1,
					si2);
			}
			return si1.AsBasicUnit >= si2.AsBasicUnit;
		}

		[DebuggerHidden]
		public static bool operator >=(SI si1, double d)
		{
			return si1 != null && si1.Val >= d;
		}

		[DebuggerHidden]
		public static bool operator >=(double d, SI si1)
		{
			return si1 != null && d >= si1.Val;
		}

		[DebuggerHidden]
		public static bool operator <=(double d, SI si1)
		{
			return si1 != null && d <= si1.Val;
		}

		/// <summary>
		/// Determines whether the SI is between lower and uppper bound.
		/// </summary>
		/// <param name="lower">The lower bound.</param>
		/// <param name="upper">The upper bound.</param>
		/// <returns></returns>
		public bool IsBetween(SI lower, SI upper)
		{
			return VectoMath.Min(lower, upper) <= Val && Val <= VectoMath.Max(lower, upper);
		}

		/// <summary>
		/// Determines whether the SI is between lower and upper bound.
		/// </summary>
		/// <param name="lower">The lower bound.</param>
		/// <param name="upper">The upper bound.</param>
		/// <returns></returns>
		public bool IsBetween(double lower, double upper)
		{
			return Math.Min(lower, upper) <= Val && Val <= Math.Max(lower, upper);
		}

		#endregion

		#region ToString

		/// <summary>
		///     Returns the Unit Part of the SI Unit Expression.
		/// </summary>
		public static string GetUnitString(int[] units = null)
		{
			if (units == null) {
				return "";
			}
			return Unit.GetUnitString(units);
		}

		public override string ToString()
		{
			return ToString(null);
		}


		public virtual string SerializedValue => ToString();

		[JsonIgnore]
		public virtual string UnitString => GetUnitString(_units);

		private string ToString(string format)
		{
			if (string.IsNullOrEmpty(format)) {
				format = "F4";
			}

			return string.Format(CultureInfo.InvariantCulture, "{0:" + format + "} [{2}]", Val, format, UnitString);
		}

		#endregion

		#region Equality members

		/// <summary>
		/// Compares the Unit-Parts of two SI Units.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public bool HasEqualUnit(SI si)
		{
			return SIUtils.CompareUnits(_units, si._units);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			var other = obj as SI;

			return other != null && AsBasicUnit.Equals(other.AsBasicUnit) && HasEqualUnit(other);
		}

		/// <summary>
		/// Determines whether the specified si is equal.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsEqual(SI si, SI tolerance = null)
		{
			return (tolerance == null || HasEqualUnit(tolerance)) && HasEqualUnit(si) &&
				   AsBasicUnit.IsEqual(si.AsBasicUnit, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified value is equal.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public bool IsEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsEqual(val, tolerance);
		}

		/// <summary>
		/// Determines whether the specified si is smaller.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsSmaller(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return AsBasicUnit.IsSmaller(si.AsBasicUnit, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is smaller.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsSmaller(SI si, double tolerance)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}

			return Val.IsSmaller(si.Val, tolerance);
		}

		/// <summary>
		/// Determines whether [is smaller or equal] [the specified si].
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsSmallerOrEqual(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return AsBasicUnit.IsSmallerOrEqual(si.AsBasicUnit, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is greater.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsGreater(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return AsBasicUnit.IsGreater(si.AsBasicUnit, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is greater.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreater(SI si, double tolerance)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}

			return Val.IsGreater(si.Val, tolerance);
		}

		/// <summary>
		/// Determines whether [is greater or equal] [the specified si].
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreaterOrEqual(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return AsBasicUnit.IsGreaterOrEqual(si.AsBasicUnit, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified value is smaller.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsSmaller(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsSmaller(val, tolerance);
		}

		/// <summary>
		/// Determines whether [is smaller or equal] [the specified value].
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsSmallerOrEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsSmallerOrEqual(val, tolerance);
		}

		/// <summary>
		/// Determines whether the specified value is greater.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreater(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsGreater(val, tolerance);
		}

		/// <summary>
		/// Determines whether [is greater or equal] [the specified value].
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreaterOrEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsGreaterOrEqual(val, tolerance);
		}

		public override int GetHashCode()
		{
			unchecked {
				// ReSharper disable once NonReadonlyMemberInGetHashCode
				var hashCode = Val.GetHashCode();
				hashCode = (hashCode * 397) ^ (_units != null ? _units.GetHashCode() : 0);
				return hashCode;
			}
		}

		[DebuggerStepThrough]
		public int CompareTo(object obj)
		{
			var si = obj as SI;
			if (si == null) {
				return 1;
			}

			if (!HasEqualUnit(si)) {
				// TODO: thow exception!
				var sum1 = 0;
				var sum2 = 0;
				for (var i = 0; i < _units.Length; i++) {
					sum1 += Math.Abs(si._units[i]);
					sum2 += Math.Abs(_units[i]);
				}
				return sum1 >= sum2 ? -1 : 1;
			}

			if (this > si) {
				return 1;
			}

			if (this < si) {
				return -1;
			}

			return 0;
		}

		public static bool operator ==(SI left, SI right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SI left, SI right)
		{
			return !Equals(left, right);
		}

		#endregion

		/// <summary>
		/// Convert the SI to a string in the wished output format.
		/// </summary>
		/// <param name="decimals">The decimals.</param>
		/// <param name="outputFactor">The output factor.</param>
		/// <param name="showUnit">The show unit.</param>
		/// <returns></returns>
		public string ToOutputFormat(uint? decimals = null, double? outputFactor = null, bool? showUnit = null)
		{
			decimals = decimals ?? 4;
			outputFactor = outputFactor ?? 1.0;
			showUnit = showUnit ?? false;

			if (showUnit.Value) {
				return (Val * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture) + " [" +
					   UnitString + "]";
			}

			return (Val * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		public string ToGUIFormat()
		{
			return Val.ToGUIFormat();
		}

		public string ToXMLFormat(uint? decimals = null)
		{
			decimals = decimals ?? 2;
			return Val.ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		public class EqualityComparer<T> : IEqualityComparer<T> where T : SI
		{
			private readonly double _precision;

			public EqualityComparer(double precision = DoubleExtensionMethods.Tolerance)
			{
				_precision = precision;
			}

			public bool Equals(T x, T y)
			{
				return y != null && x != null && x.IsEqual(y.Value(), _precision);
			}

			public int GetHashCode(T obj)
			{
				return obj.Value().GetHashCode();
			}
		}
	}
}