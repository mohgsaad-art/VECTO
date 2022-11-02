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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class Wheels : LookupData<string, Wheels.Entry>
	{
		private string[] _dimensions;
		public TyreClass TyreClass = new TyreClass();

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Wheels.csv";

		protected override string ErrorMessage => "Wheels Lookup Error: No value found for Wheels. Key: '{0}'";

		public override Entry Lookup(string key)
		{
			return base.Lookup(key.RemoveWhitespace());
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
				.Select(row => new Entry {
					WheelType = row.Field<string>(0).RemoveWhitespace(),
					Inertia = row.ParseDouble("inertia").SI<KilogramSquareMeter>(),
			        WheelsDiameter = row.ParseDouble("d").SI(Unit.SI.Milli.Meter).Cast<Meter>(),
					CircumferenceFactor = row.ParseDouble("f")
				}).ToDictionary(e => e.WheelType);
			_dimensions = table.Rows.Cast<DataRow>().Select(row => row.Field<string>(0)).ToArray();
		}

		public struct Entry
		{
			public string WheelType;
			public KilogramSquareMeter Inertia;
			public Meter WheelsDiameter;
			public double CircumferenceFactor;

			public Meter DynamicTyreRadius => WheelsDiameter * CircumferenceFactor / (2 * Math.PI);
		}

		public string[] GetWheelsDimensions()
		{
			return _dimensions;
		}
	}
}