using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusElectricMotorData : GenericBusEMBase
	{
		public GenericBusElectricMotorData()
		{
			GenericEfficiencyMap_ASM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_ASM_normalized.vmap";
			GenericEfficiencyMap_PSM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_PSM_normalized.vmap";
		}


		public ElectricMotorData CreateGenericElectricMotorData(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry,
			IList<Tuple<Volt, TableData>> torqueLimits, Volt averageVoltage)
		{
			var motorData = electricMachineEntry.ElectricMachine;
			if (electricMachineEntry.ElectricMachine.CertificationMethod == CertificationMethod.StandardValues) {
				motorData = new StandardValuesInputData.StandardValuesEmInputData(motorData, 1.SI<Volt>(), 10e9.SI<Volt>());
			}
            var electricMachineType = electricMachineEntry.ElectricMachine.ElectricMachineType;
			var efficiencyData = GetVoltageLevels(motorData, electricMachineEntry.Count, electricMachineType, torqueLimits);
			var powertrainPosition = electricMachineEntry.Position;
            //var adcLossMap = electricMachineEntry.MechanicalTransmissionLossMap;
			var adcRatio = electricMachineEntry.RatioADC;
			var adcLossMap = adcRatio.IsEqual(1) ? 0.98 : 0.96;
			var lossMap = powertrainPosition == PowertrainPosition.IHPC
				? TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC IHPC LossMap Eff")
				: electricMachineEntry.ADC != null
					? TransmissionLossMapReader.CreateEmADCLossMap(adcLossMap, adcRatio, "EM ADC LossMap")
					: TransmissionLossMapReader.CreateEmADCLossMap(DeclarationData.ElectricMachineDefaultMechanicalTransmissionEfficiency, adcRatio, "EM ADC LossMap Eff");

			

            var electricMotorData = new ElectricMotorData {
				RatioPerGear = electricMachineEntry.RatioPerGear,
				EMDragCurve = ElectricMotorDragCurveReader.Create(electricMachineEntry.ElectricMachine.DragCurve,
					electricMachineEntry.Count),
				EfficiencyData = efficiencyData,
				Inertia = electricMachineEntry.ElectricMachine.Inertia * electricMachineEntry.Count,//??
				RatioADC = electricMachineEntry.RatioADC,
				Overload = CalculateOverloadData(motorData, electricMachineEntry.Count, efficiencyData, averageVoltage ),
				TransmissionLossMap = lossMap,
				OverloadRecoveryFactor = DeclarationData.OverloadRecoveryFactor,
			};

			return electricMotorData;
		}




        private OverloadData CalculateOverloadData(IIEPCDeclarationInputData iepc, int count,
            VoltageLevelData voltageLevel, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
		{
			return CalculateOverloadData(count, voltageLevel, averageVoltage, iepc.VoltageLevels, gearRatioUsedForMeasurement);
		}


        private OverloadData CalculateOverloadData(IElectricMotorDeclarationInputData motorData, int count, VoltageLevelData voltageLevel, Volt averageVoltage)
		{
			return CalculateOverloadData(count, voltageLevel, averageVoltage, motorData.VoltageLevels);
		}
		private OverloadData CalculateOverloadData(int count, VoltageLevelData voltageLevel, Volt averageVoltage, IList<IElectricMotorVoltageLevel> electricMotorVoltageLevels, Tuple<uint, double> gearUsedForMeasurement = null)
		{

			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < electricMotorVoltageLevels.Min(x => x.VoltageLevel))
			{
				return CalculateOverloadBuffer(electricMotorVoltageLevels.First(), count, voltageLevel, gearUsedForMeasurement);
			}
			if (averageVoltage > electricMotorVoltageLevels.Max(x => x.VoltageLevel))
			{
				return CalculateOverloadBuffer(electricMotorVoltageLevels.Last(), count, voltageLevel, gearUsedForMeasurement);
			}

			var (vLow, vHigh) = electricMotorVoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel, gearUsedForMeasurement);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel, gearUsedForMeasurement);

			var retVal = new OverloadData()
			{
				OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
				ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
			};
			return retVal;
		}

        private OverloadData CalculateOverloadBuffer(IElectricMotorVoltageLevel voltageEntry,
            int count, VoltageLevelData voltageLevel, Tuple<uint, double> gearUsedForMeasurement = null)
        {
            var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
            var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
            var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
            var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
            var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
            var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;

            //ElectricMotorRatedSpeedHelper.GetRatedSpeed(voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque)
            var FullLoadCurve = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve;
            // FullLoadCurve express torque and speed at the rotor level, not the output shaft

            //var FullLoadEntries = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries; 
            var continuousPower = continuousTorque * continuousTorqueSpeed;

            var continuousTorqueSpeedRef = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque));
            var continuousTorqueSpeedRefGen = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullGenerationTorque));

            var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
            var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

            var peakElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    overloadTestSpeed,
                    -overloadTorque,
                    gear,
                    true)
                    .ElectricalPower;
            var peakPwrLoss = -peakElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive

            var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                -continuousTorque, gear).ElectricalPower ??
                            voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed),
                                gear, true).ElectricalPower;
            var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive
            var overloadBuffer = (peakPwrLoss - continuousPowerLoss) * voltageEntry.OverloadTime;
            return new OverloadData()
            {
                OverloadBuffer = overloadBuffer,
                ContinuousTorque = continuousTorqueRef,
                ContinuousTorqueGen = continuousTorqueRefGen,
                ContinuousPower = continuousPower,
                ContinuousPowerLoss = continuousPowerLoss
            };
        }

        private VoltageLevelData GetVoltageLevels(
			IElectricMotorDeclarationInputData motorData, int count,
			ElectricMachineType electricMachineType, IList<Tuple<Volt, TableData>> torqueLimits)
		{
			var voltageLevels = motorData.VoltageLevels;
			var normalizedMap = GetNormalizedEfficiencyMap(electricMachineType);


			 return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, normalizedMap, torqueLimits)
			};
		}
		

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(
			IList<IElectricMotorVoltageLevel> voltageLevels, int count, TableData normalizedMap,
			IList<Tuple<Volt, TableData>> torqueLimits)
		{
			var result = new List<ElectricMotorVoltageLevelData>();

			foreach (var voltageLevel in voltageLevels) {

				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(voltageLevels[0].FullLoadCurve);
				var efficiencyMap = DeNormalizeData(normalizedMap, ratedPoint);
				
				var electricMotorVoltageLevel = new ElectricMotorVoltageLevelData {
					Voltage = voltageLevel.VoltageLevel,
					FullLoadCurve = GetElectricMotorFullLoadCurve(voltageLevel, count, torqueLimits),
					EfficiencyMap = ElectricMotorMapReader.Create(efficiencyMap, count, ExecutionMode.Declaration)
				};

				result.Add(electricMotorVoltageLevel);
			}
			
			return result;
		}


		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(IElectricMotorVoltageLevel entry,
			int count,
			IList<Tuple<Volt, TableData>> torqueLimits)
		{
			//var entries = new List<ElectricMotorFullLoadCurve.FullLoadEntry>();
			var fullLoadCurve = ElectricFullLoadCurveReader.Create(entry.FullLoadCurve, count);
            var maxTorqueCurve = torqueLimits == null
				? null
				: ElectricFullLoadCurveReader.Create(
					torqueLimits.FirstOrDefault(x => x.Item1 == null || x.Item1.IsEqual(entry.VoltageLevel))?.Item2, count);

			var fullLoadCurveCombined = ElectricMachinesDataAdapter.IntersectEMFullLoadCurves(fullLoadCurve, maxTorqueCurve);

            //         foreach (DataRow row in entry.FullLoadCurve.Rows) {
            //	entries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry {
            //		MotorSpeed = row.ParseDouble("outShaftSpeed").SI<PerSecond>(),
            //		FullGenerationTorque = row.ParseDouble("minTorque").SI<NewtonMeter>(),
            //		FullDriveTorque = row.ParseDouble("maxTorque").SI<NewtonMeter>()
            //	});
            //}

            return fullLoadCurveCombined;
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);
			
			///Efficiency maps are saved in rpm
			foreach (DataRow row in normalizedMap.Rows) {
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated;
				var torque = row.ParseDouble(TorqueNorm) * ratedPoint.TRated;
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;

				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.AsRPM, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}
			
			return result;
		}
	}
}
