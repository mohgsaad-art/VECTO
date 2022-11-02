using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Util {
	public static class TableDataConverter {

		public static TableData Convert(IEnumerable<GearLossMapEntry> viewModelLossMap)
		{
			var retVal = new TableData();
			var columns = new[] {
				TransmissionLossMapReader.Fields.InputSpeed,
				TransmissionLossMapReader.Fields.InputTorque,
				TransmissionLossMapReader.Fields.TorqeLoss
			};
			foreach (var colName in columns) {
				retVal.Columns.Add(colName);
			}
			foreach (var entry in viewModelLossMap) {
				var row = retVal.NewRow();
				row[TransmissionLossMapReader.Fields.InputSpeed] = entry.InputSpeed.AsRPM;
				row[TransmissionLossMapReader.Fields.InputTorque] = entry.InputTorque.Value();
				row[TransmissionLossMapReader.Fields.TorqeLoss] = entry.TorqueLoss.Value();
				retVal.Rows.Add(row);
			}
			return retVal;
		}

		public static TableData Convert(IEnumerable<RetarderLossMapEntry> viewModelLossMap)
		{
			var retVal = new TableData();
			var columns = new[] {
				RetarderLossMapReader.Fields.RetarderSpeed,
				RetarderLossMapReader.Fields.TorqueLoss
			};
			foreach (var colName in columns) {
				retVal.Columns.Add(colName);
			}
			foreach (var entry in viewModelLossMap) {
				var row = retVal.NewRow();
				row[RetarderLossMapReader.Fields.RetarderSpeed] = entry.RetarderSpeed.AsRPM;
				row[RetarderLossMapReader.Fields.TorqueLoss] = entry.TorqueLoss.Value();
				retVal.Rows.Add(row);
			}
			return retVal;
		}

		public static TableData Convert(IEnumerable<TorqueConverterCharacteristics> viewModelLossMap)
		{
			var retVal = new TableData();
			var columns = new[] {
				TorqueConverterDataReader.Fields.SpeedRatio,
				TorqueConverterDataReader.Fields.TorqueRatio,
				TorqueConverterDataReader.Fields.CharacteristicTorque
			};
			foreach (var colName in columns) {
				retVal.Columns.Add(colName);
			}
			foreach (var entry in viewModelLossMap) {
				var row = retVal.NewRow();
				row[TorqueConverterDataReader.Fields.SpeedRatio] = entry.SpeedRatio;
				row[TorqueConverterDataReader.Fields.TorqueRatio] = entry.TorqueRatio;
				row[TorqueConverterDataReader.Fields.CharacteristicTorque] = entry.InputTorqueRef.Value();
				retVal.Rows.Add(row);
			}
			return retVal;
		}

		public static TableData Convert(IEnumerable<FuelConsumptionEntry> fcMap)
		{
			var retVal = new TableData();
			var columns = new[] {
				FuelConsumptionMapReader.Fields.EngineSpeed,
				FuelConsumptionMapReader.Fields.Torque,
				FuelConsumptionMapReader.Fields.FuelConsumption
			};
			foreach (var colName in columns) {
				retVal.Columns.Add(colName);
			}
			foreach (var entry in fcMap) {
				var row = retVal.NewRow();
				row[FuelConsumptionMapReader.Fields.EngineSpeed] = entry.EngineSpeed.AsRPM;
				row[FuelConsumptionMapReader.Fields.Torque] = entry.Torque.Value();
				row[FuelConsumptionMapReader.Fields.FuelConsumption] = entry.FuelConsumption.ConvertToGrammPerHour().Value;
				retVal.Rows.Add(row);
			}
			return retVal;
		}

		public static TableData Convert(IEnumerable<FullLoadEntry> fullLoadCurve)
		{
			var retVal = new TableData();
			var columns = new[] {
				FullLoadCurveReader.Fields.EngineSpeed,
				FullLoadCurveReader.Fields.TorqueFullLoad,
				FullLoadCurveReader.Fields.TorqueDrag
			};
			foreach (var colName in columns) {
				retVal.Columns.Add(colName);
			}
			foreach (var entry in fullLoadCurve) {
				var row = retVal.NewRow();
				row[FullLoadCurveReader.Fields.EngineSpeed] = entry.EngineSpeed.AsRPM;
				row[FullLoadCurveReader.Fields.TorqueFullLoad] = entry.MaxTorque.Value();
				row[FullLoadCurveReader.Fields.TorqueDrag] = entry.DragTorque.Value();
				retVal.Rows.Add(row);
			}
			return retVal;
		}
	}
}