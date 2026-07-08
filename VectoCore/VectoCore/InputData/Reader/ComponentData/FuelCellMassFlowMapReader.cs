using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class FuelCellMassFlowMapReader
	{
		public static FuelCellMassFlowMap Create(DataTable data, Watt minPower, Watt maxPower)
		{
			if (data == null) {
				return null;
			}

			if (data.Columns.Count != 2) {
				throw new VectoException($"Mass Flow Map must contain exactly two columns: {Fields.ElectricPower}, {Fields.m_H2}", ErrorCodes.DATATABLE_WRONG_COLUMNS,
					$"Make sure the Mass Flow Map file contains exactly two columns: {Fields.ElectricPower}, {Fields.m_H2}.");
			}
			if (!data.Columns.Contains(Fields.ElectricPower) || !data.Columns.Contains(Fields.m_H2))
			{
				LoggingObject.Logger<FuelCellMassFlowMap>().Warn(
					"Mass Flow Map Header is invalid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.ElectricPower,
					Fields.m_H2,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.ElectricPower;
				data.Columns[1].ColumnName = Fields.m_H2;
			}

			var fcMap = new FuelCellMassFlowMap(data.Rows.Cast<DataRow>().Select(row => new FuelCellMassFlowMap.MassFlowMapEntry() {
				P_el_out = (row.ParseDouble(Fields.ElectricPower) * 1000).SI<Watt>(),
				H2 = (row.ParseDouble(Fields.m_H2) / (1000 * 3600)).SI<KilogramPerSecond>(),
			}).ToArray());
			if (minPower != null) {
				fcMap.MinPower = minPower;
            }

			if (maxPower != null) {
				fcMap.MaxPower = maxPower;
            }

			return fcMap;
		}

		public static class Fields
		{
			public const string ElectricPower = "P_el_out";
			public const string m_H2 = "m_H2";
		}
	}
}