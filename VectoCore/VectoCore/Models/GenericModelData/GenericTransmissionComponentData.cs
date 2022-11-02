using System;
using System.Data;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericTransmissionComponentData
	{
		
		//public GearboxData CreateGearboxData(
		//	IVehicleDeclarationInputData primaryVehicle, VectoRunData runData,
		//	IShiftPolygonCalculator shiftPolygonCalc)
		//{
		//	return DeclarationDataAdapterHeavyLorry.DoCreateGearboxData(primaryVehicle, runData, shiftPolygonCalc);
		//}

		public AxleGearData CreateGenericBusAxlegearData(IAxleGearInputData axlegearData)
		{
			var axleGear = new AxleGearData
			{
				LineType = axlegearData.LineType,
				InputData = axlegearData
			};

			var ratio = axlegearData.Ratio;

			var outputLossMap = CreateAxlegearOutputLossMap(ratio);
			var axleGearInputLossMap = CalculateTransmissionLossMapInput(outputLossMap, ratio, 1.0);

			var transmissionData = new TransmissionData
			{
				Ratio = axlegearData.Ratio,
				LossMap = TransmissionLossMapReader.Create(axleGearInputLossMap, ratio, "Axlegear")
			};

			axleGear.AxleGear = transmissionData;

			return axleGear;
		}

		public AngledriveData CreateGenericBusAngledriveData(IAngledriveInputData angledriveData)
		{
			if (angledriveData == null || angledriveData.Type == AngledriveType.None || angledriveData.Type == AngledriveType.LossesIncludedInGearbox) {
				return null;
			}

			var axleGear = new AngledriveData() {
				InputData = angledriveData
			};

			var outputLossMap = CreateAxlegearOutputLossMap(angledriveData.Ratio);
			var axleGearInputLossMap = CalculateTransmissionLossMapInput(outputLossMap, angledriveData.Ratio, Constants.GenericLossMapSettings.FactorAngleDrive);

			var transmissionData = new TransmissionData {
				Ratio = angledriveData.Ratio,
				LossMap = TransmissionLossMapReader.Create(axleGearInputLossMap, angledriveData.Ratio, "Angledrive")
			};

			axleGear.Angledrive = transmissionData;

			return axleGear;
		}

		private DataTable CreateAxlegearOutputLossMap(double axleRatio)
		{
			var torques = new[] {
					Constants.GenericLossMapSettings.OutputTorqueEnd * -1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart *-1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart,
					Constants.GenericLossMapSettings.OutputTorqueEnd
				};

			var outputSpeeds = new[] {
				0,
				Constants.GenericLossMapSettings.OutputSpeedStart,
				Constants.GenericLossMapSettings.OutputSpeedEnd
			};

			var td0 = Constants.GenericLossMapSettings.T0 +
					  axleRatio * Constants.GenericLossMapSettings.T1;

			var td0_ = td0 * 0.5;
			var td150_ = td0 * 0.5;
			var td_n = Constants.GenericLossMapSettings.Td_n;
			var efficiency = Constants.GenericLossMapSettings.Efficiency;

			var lossMap = new DataTable();
			lossMap.Columns.Add("output speed");
			lossMap.Columns.Add("output torque");
			lossMap.Columns.Add("output torque loss");

			foreach (var outputSpeed in outputSpeeds) {
				foreach (var torque in torques) {
					var calculationSpeed = outputSpeed.IsEqual(0)
						? Constants.GenericLossMapSettings.OutputSpeedStart
						: outputSpeed;
					
					var newRow = lossMap.NewRow();
					newRow[lossMap.Columns[0]] = outputSpeed.ToXMLFormat(2);
					newRow[lossMap.Columns[1]] = torque.ToXMLFormat(2);
					newRow[lossMap.Columns[2]] = CalculateOutputTorqueLoss(td0_, td150_, td_n, calculationSpeed, torque, efficiency).ToXMLFormat(2);

					lossMap.Rows.Add(newRow);
				}
			}

			return lossMap;
		}

		private double CalculateOutputTorqueLoss(double td0_, double td150_, double td_n,
			double outputspeed, double ouputTorque, double efficiency)
		{
			if (ouputTorque < 0)
				ouputTorque = ouputTorque * -1.0;

			return td0_ + td150_ * outputspeed / td_n + ouputTorque / efficiency - ouputTorque;
		}

		private DataTable CalculateTransmissionLossMapInput(DataTable outputLossMap, double axleRatio, double lossCorrectionFactor)
		{
			var inputLossMap = new DataTable();

			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputSpeed);
			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputTorque);
			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.TorqeLoss);

			foreach (DataRow row in outputLossMap.Rows)
			{
				var outputSpeed = row[0].ToString().ToDouble();
				var outputTorque = row[1].ToString().ToDouble();
				var outputLoss = row[2].ToString().ToDouble();

				var newRow = inputLossMap.NewRow();
				newRow[0] = Math.Round(GetInputSpeed(outputSpeed, axleRatio), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[1] = Math.Round(GetInputTorque(outputTorque, outputLoss, axleRatio), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[2] = Math.Round(GetInputTorqueLoss(outputLoss, axleRatio, lossCorrectionFactor), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				inputLossMap.Rows.Add(newRow);
			}

			return inputLossMap;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputSpeed(double outputSpeed, double iAxle)
		{
			return outputSpeed * iAxle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputTorque(double outputTorque, double outputLoss, double iAxle)
		{
			return (outputTorque + outputLoss) / iAxle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputTorqueLoss(double outputLoss, double iAxle, double lossCorrectionFactor)
		{
			return outputLoss / iAxle * lossCorrectionFactor;
		}
	}
}
