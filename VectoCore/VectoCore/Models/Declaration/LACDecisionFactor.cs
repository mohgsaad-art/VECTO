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

using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	/// <summary>
	/// Class for Look Ahead Coasting Decision Factor (DF_coast)
	/// </summary>
	public sealed class LACDecisionFactor
	{
		private readonly LACDecisionFactorVTarget _vTarget;
		private readonly LACDecisionFactorVdrop _vDrop;
		private readonly double _offset;
		private readonly double _scaling;

		public LACDecisionFactor()
		{
			_offset = DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset;
			_scaling = DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling;
			_vTarget = new LACDecisionFactorVTarget();
			_vDrop = new LACDecisionFactorVdrop();
		}

		public LACDecisionFactor(double offset, double scaling, DataTable vTargetLookup, DataTable vDropLookup)
		{
			_offset = offset;
			_scaling = scaling;
			_vTarget = new LACDecisionFactorVTarget(vTargetLookup);
			_vDrop = new LACDecisionFactorVdrop(vDropLookup);
		}

		public double Lookup(MeterPerSecond targetVelocity, MeterPerSecond velocityDrop)
		{
			// normalize values from [0 .. 1] to [2.5 .. 1]
			return _offset - _scaling * _vTarget.Lookup(targetVelocity) * _vDrop.Lookup(velocityDrop);
		}

		public sealed class LACDecisionFactorVdrop : LookupData<MeterPerSecond, double>
		{
			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".LAC-DF-Vdrop.csv";

			protected override string ErrorMessage => throw new System.NotImplementedException();

			public LACDecisionFactorVdrop() {}

			public LACDecisionFactorVdrop(DataTable vDrop)
			{
				ParseData(vDrop ?? ReadCsvResource(ResourceId));
			}

			public override double Lookup(MeterPerSecond targetVelocity)
			{
				var section = Data.GetSection(kv => kv.Key < targetVelocity);
				return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key, section.Item1.Value, section.Item2.Value,
					targetVelocity);
			}

			protected override void ParseData(DataTable table)
			{
				if (table.Columns.Count < 2) {
					throw new VectoException("LAC Decision Factor File for Vdrop must consist of at least 2 columns.");
				}

				if (table.Rows.Count < 2) {
					throw new VectoException(
						"LAC Decision Factor File for Vdrop must consist of at least two lines with numeric values (below file header)");
				}

				if (table.Columns.Contains(Fields.VelocityDrop) && table.Columns.Contains(Fields.DecisionFactor)) {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(Fields.VelocityDrop).KMPHtoMeterPerSecond(),
							r => r.ParseDouble(Fields.DecisionFactor));
				} else {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(0).KMPHtoMeterPerSecond(), r => r.ParseDouble(1));
				}
			}

			public static class Fields
			{
				public const string DecisionFactor = "decision_factor";
				public const string VelocityDrop = "v_drop";
			}
		}

		public sealed class LACDecisionFactorVTarget : LookupData<MeterPerSecond, double>
		{
			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".LAC-DF-Vtarget.csv";

			protected override string ErrorMessage => throw new System.NotImplementedException();

			public LACDecisionFactorVTarget() {}

			public LACDecisionFactorVTarget(DataTable vTargetLookup)
			{
				ParseData(vTargetLookup ?? ReadCsvResource(ResourceId));
			}

			public override double Lookup(MeterPerSecond targetVelocity)
			{
				var section = Data.GetSection(kv => kv.Key < targetVelocity);
				return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key, section.Item1.Value, section.Item2.Value,
					targetVelocity);
			}

			protected override void ParseData(DataTable table)
			{
				if (table.Columns.Count < 2) {
					throw new VectoException("LAC Decision Factor File for Vtarget must consist of at least 2 columns.");
				}

				if (table.Rows.Count < 2) {
					throw new VectoException(
						"LAC Decision Factor File for Vtarget must consist of at least two lines with numeric values (below file header)");
				}

				if (table.Columns.Contains(Fields.TargetVelocity) && table.Columns.Contains(Fields.DecisionFactor)) {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(Fields.TargetVelocity).KMPHtoMeterPerSecond(),
							r => r.ParseDouble(Fields.DecisionFactor));
				} else {
					Data = table.Rows.Cast<DataRow>()
						.ToDictionary(r => r.ParseDouble(0).KMPHtoMeterPerSecond(), r => r.ParseDouble(1));
				}
			}

			public static class Fields
			{
				public const string TargetVelocity = "v_target";
				public const string DecisionFactor = "decision_factor";
			}
		}
	}
}