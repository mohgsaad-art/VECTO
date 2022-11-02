using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration {
	public sealed class BusAlternatorTechnologies : LookupData<string, double>
	{
		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Buses.AlternatorTechnologies.csv";

		protected override string ErrorMessage => "Bus-Alternator Technology Lookup Error: No value found for Technology. Key: '{0}'";

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
						.Select(row => Tuple.Create(row.Field<string>("technology"), row.ParseDouble("efficiency")))
						.ToDictionary(e => e.Item1, e => e.Item2);
		}

		#endregion
	}
}