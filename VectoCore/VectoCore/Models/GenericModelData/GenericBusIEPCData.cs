using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;


namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusIEPCData : GenericBusEMBase
	{
		#region Constant

		public const double GenericGearEfficiency = 0.96;
		public const double GenericAxleGearEfficiency = 0.96;

		#endregion

		private readonly GenericTransmissionComponentData _genericTransmission = new GenericTransmissionComponentData();

		private double _axleEfficiency;
		private Dictionary<int, double> _gearRatios;
		private double _axleRatio;
		private KeyValuePair<int, double> _gearRatioAtMeasurement;

		public GenericBusIEPCData()
		{
			GenericEfficiencyMap_ASM = 
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_ASM_normalized.vmap";
			GenericEfficiencyMap_PSM = 
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_PSM_normalized.vmap";
		}

		public IEPCElectricMotorData CreateIEPCElectricMotorData(IIEPCDeclarationInputData iepcData)
		{
			InitData(iepcData);

			var count = iepcData.DesignTypeWheelMotor && iepcData.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1; //? also for declaration mode valid
			
			var iepcEM = new IEPCElectricMotorData {
				IEPCDragCurves = GetIEPCDragCurves(iepcData, count),
				EfficiencyData = GetIEPCVoltageLevelData(iepcData.VoltageLevels, count, iepcData.ElectricMachineType),
				Inertia = iepcData.Inertia * count,
				RatioPerGear = _gearRatios.Select(x => x.Value).ToArray(),
				//iepcEM.RatioADC
                //iepcEM.TransmissionLossMap
                //iepcEM.EMDragCurve
                //iepcEM.Overload
            };
			
			return iepcEM;
		}

		private void InitData(IIEPCDeclarationInputData iepcData)
		{
			if (iepcData.DifferentialIncluded) {
				_axleEfficiency = GenericAxleGearEfficiency;
				_axleRatio = 1;
			} else {
				_axleEfficiency = 1;
				_axleRatio = 1;
			}

			_gearRatios = new Dictionary<int, double>();
			foreach (var gear in iepcData.Gears.OrderBy(x => x.GearNumber)) {
				_gearRatios.Add(gear.GearNumber, gear.Ratio);
			}

			_gearRatioAtMeasurement = GetGearRatioAtMeasurement();
		}

		private VoltageLevelData GetIEPCVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, electricMachineType)
			};
		}

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			var result = new List<ElectricMotorVoltageLevelData>();
			foreach (var entry in voltageLevels.OrderBy(x => x.VoltageLevel)) {
				var fld = GetElectricMotorFullLoadCurve(entry, count);
				var iepcVoltageLevel = new IEPCVoltageLevelData {
					EfficiencyMaps = GetEfficiencyMaps(entry, count, electricMachineType, fld),
					Voltage = entry.VoltageLevel,
					FullLoadCurve = fld
				};
				result.Add(iepcVoltageLevel);
			}

			return result;
		}

		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(IElectricMotorVoltageLevel voltageLevel,
			int count)
		{
			return IEPCFullLoadCurveReader.Create(voltageLevel.FullLoadCurve, count, _gearRatioAtMeasurement.Value);
		}

		
		private Dictionary<uint, EfficiencyMap> GetEfficiencyMaps(IElectricMotorVoltageLevel voltageLevel, int count,
			ElectricMachineType electricMachineType, ElectricMotorFullLoadCurve fullLoadCurve)
		{
			var result = new Dictionary<uint, EfficiencyMap>();

			foreach (var gearEntry in _gearRatios) {

				var gearRatio = gearEntry.Value;
				
				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(voltageLevel.FullLoadCurve,
					_axleRatio, _gearRatioAtMeasurement.Value, GenericGearEfficiency, _axleEfficiency);
				
				var deNormalizedMap = DeNormalizeData(GetNormalizedEfficiencyMap(electricMachineType), ratedPoint, gearRatio);
				result.Add((uint) gearEntry.Key, IEPCMapReader.Create(deNormalizedMap, count, gearRatio, fullLoadCurve, ExecutionMode.Declaration));
			}

			return result;
		}
		

		private Dictionary<uint, DragCurve> GetIEPCDragCurves(IIEPCDeclarationInputData iepcData, int count)
		{
			var dragCurves = new Dictionary<uint, DragCurve>();

			if (iepcData.DragCurves.Count > 1) {
				for (var i = 0u; i < iepcData.DragCurves.Count; i++) {
					var ratio = iepcData.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(iepcData.DragCurves[(int)i].DragCurve, count, ratio));
				}
			} else {
				var dragCurve = iepcData.DragCurves.First().DragCurve;
				for (var i = 0u; i < iepcData.Gears.Count; i++) {
					var ratio = iepcData.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(dragCurve, count, ratio));
				}
			}

            return dragCurves;
		}


		private KeyValuePair<int, double> GetGearRatioAtMeasurement()
		{
			var gear = _gearRatios.Select(x => new {
				Gear = x.Key,
				Ratio = x.Value,
				Distance = Math.Abs(x.Value - 1)
			}).OrderBy(x => x.Distance).GroupBy(x => x.Distance).First().MaxBy(x => x.Ratio);

			return new KeyValuePair<int, double>(gear.Gear, gear.Ratio);
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint, double gearRatio)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);

			foreach (DataRow row in normalizedMap.Rows) {
				var torqueNormValue = row.ParseDouble(TorqueNorm);
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated / gearRatio / _axleRatio;
				var torque = torqueNormValue * ratedPoint.TRated * gearRatio * _axleRatio * 
							( torqueNormValue > 0 ? GenericGearEfficiency * _axleEfficiency : 1 / (GenericGearEfficiency * _axleEfficiency));
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;
				
				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.AsRPM, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}

			return result;
		}

		private OverloadData CalculateOverloadData(IIEPCDeclarationInputData iepc, int count,
			VoltageLevelData voltageLevel, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
		{

			// if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
			if (averageVoltage < iepc.VoltageLevels.Min(x => x.VoltageLevel))
			{
				return CalculateOverloadBuffer(iepc.VoltageLevels.First(), count, voltageLevel, gearRatioUsedForMeasurement);
			}
			if (averageVoltage > iepc.VoltageLevels.Max(x => x.VoltageLevel))
			{
				return CalculateOverloadBuffer(iepc.VoltageLevels.Last(), count, voltageLevel, gearRatioUsedForMeasurement);
			}

			var (vLow, vHigh) = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
			var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel, gearRatioUsedForMeasurement);
			var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel, gearRatioUsedForMeasurement);

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

            var FullLoadCurve = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve;

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
    }
}
