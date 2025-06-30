using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class ElectricMotorData
	{
		[SIRange(double.MinValue, double.MaxValue)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[SIRange(0, 1)]
		public double OverloadRecoveryFactor { get; internal set; }

		public double RatioADC { get; internal set; }

		public TransmissionLossMap TransmissionLossMap { get; internal set; }

		public double[] RatioPerGear { get; set; }

		[ValidateObject]
		public VoltageLevelData EfficiencyData { get; internal set; }

		protected internal DragCurve EMDragCurve { private get; set; }

		public virtual NewtonMeter DragCurveLookup(PerSecond emSpeed, uint gear)
		{
			return EMDragCurve.Lookup(emSpeed);
		}

		public virtual NewtonMeter DragCurveLookup(PerSecond emSpeed, GearshiftPosition gear)
		{
			return EMDragCurve.Lookup(emSpeed);
		}

		// not read direcly from input but calculated in a pre-processing step
		public OverloadData Overload { get; internal set; }
	}

	public class IEPCElectricMotorData : ElectricMotorData
	{
		protected internal Dictionary<uint, DragCurve> IEPCDragCurves { private get; set; }

        #region Overrides of ElectricMotorData

        public override NewtonMeter DragCurveLookup(PerSecond emSpeed, uint gear)
        {
            return IEPCDragCurves[gear].Lookup(emSpeed);
        }

		public override NewtonMeter DragCurveLookup(PerSecond emSpeed, GearshiftPosition gear)
		{
			return IEPCDragCurves[gear.Gear].Lookup(emSpeed);
		}
		#endregion
	}

	public class VoltageLevelData
	{
		private PerSecond _maxSpeed;

		public IList<ElectricMotorVoltageLevelData> VoltageLevels { get; internal set; }


		public PerSecond MaxSpeed =>
			_maxSpeed ?? (_maxSpeed = VoltageLevels
				.Min(v => v.MaxSpeed));

		
		public NewtonMeter EfficiencyMapLookupTorque(Volt voltage, Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque, GearshiftPosition gear)
		{
			if (avgSpeed.IsEqual(0.RPMtoRad()) || avgSpeed.IsGreater(MaxSpeed)) {
				return 0.SI<NewtonMeter>();
			}
			var (vLow, vHigh) = GetSection(voltage);
			var r1 = vLow.LookupTorque(electricPower, avgSpeed, maxEmTorque, gear.Gear);
			var r2 = vHigh.LookupTorque(electricPower, avgSpeed, maxEmTorque, gear.Gear);

			if (r1 is null && r2 is null) {
				return null;
			}

			// if one of the values is limited by EM, but the other is not (is null): use maxEmTorque instead
			if (r1 is null) {
				r1 = maxEmTorque;
			}
			if (r2 is null) {
				r2 = maxEmTorque;
			}

			var retVal = vLow.Voltage.IsEqual(vHigh.Voltage)
				? r1
				: VectoMath.Interpolate(vLow.Voltage, vHigh.Voltage,
					r1, r2, voltage);
			var elPwr = LookupElectricPower(voltage, avgSpeed, retVal, gear, true);
			if (elPwr.ElectricalPower != null && electricPower.IsEqual(elPwr.ElectricalPower, 1e-3.SI<Watt>())) {
				return retVal;
			}

			var searchResult = SearchAlgorithm.Search(retVal, electricPower - elPwr.ElectricalPower,
				interval: 10.SI<NewtonMeter>(),
				getYValue: x => (Watt)x - electricPower,
				evaluateFunction: x => LookupElectricPower(voltage, avgSpeed, x, gear, true).ElectricalPower,
				criterion: x => ((Watt)x - electricPower).Value(),
				searcher: this
			);

			return searchResult;
			//return null;
			//throw new NotImplementedException("EfficientyMapLookupTorque");
		}

		public EfficiencyMap.EfficiencyResult LookupElectricPower(Volt voltage, PerSecond avgSpeed, NewtonMeter torque, GearshiftPosition gear, bool allowExtrapolation = false)
		{
			var (vLow, vHigh) = GetSection(voltage);

			var r1 = vLow.LookupElectricPower(avgSpeed, torque, gear.Gear, allowExtrapolation);
			var r2 = vHigh.LookupElectricPower(avgSpeed, torque, gear.Gear, allowExtrapolation);

			if (r1 == null || r2 == null || r1.ElectricalPower == null || r2.ElectricalPower == null) {
				return new EfficiencyMap.EfficiencyResult() {
					ElectricalPower = null,
					Speed = avgSpeed,
					Torque = torque
				};
			}

			var pwr = vLow.Voltage.IsEqual(vHigh.Voltage)
				? r1.ElectricalPower
				: VectoMath.Interpolate(vLow.Voltage, vHigh.Voltage, r1.ElectricalPower,
					r2.ElectricalPower, voltage);

			return new EfficiencyMap.EfficiencyResult() {
				ElectricalPower = pwr,
				Extrapolated = r1.Extrapolated || r2.Extrapolated,
				Speed = avgSpeed,
				Torque = torque
			};
		}

		public NewtonMeter FullGenerationTorque(Volt voltage, PerSecond avgSpeed, uint gear)
		{
			var (vLow, vHigh) = GetSection(voltage);

			if (vLow.Voltage.IsEqual(vHigh.Voltage)) {
				return vLow.FullGenerationTorque(avgSpeed, gear);
			}

			return VectoMath.Interpolate(vLow.Voltage, vHigh.Voltage,
				vLow.FullGenerationTorque(avgSpeed, gear),
				vHigh.FullGenerationTorque(avgSpeed, gear), voltage);
		}

		public NewtonMeter FullLoadDriveTorque(Volt voltage, PerSecond avgSpeed, uint gear)
		{
			var (vLow, vHigh) = GetSection(voltage);

			if (vLow.Voltage.IsEqual(vHigh.Voltage)) {
				return vLow.FullLoadDriveTorque(avgSpeed, gear);
			}

			return VectoMath.Interpolate(vLow.Voltage, vHigh.Voltage,
				vLow.FullLoadDriveTorque(avgSpeed, gear),
				vHigh.FullLoadDriveTorque(avgSpeed, gear), voltage);
		}

		protected (ElectricMotorVoltageLevelData, ElectricMotorVoltageLevelData) GetSection(Volt voltage)
		{
			if (voltage < VoltageLevels.First().Voltage) {
				return (VoltageLevels.First(), VoltageLevels.First());
			}

			if (voltage > VoltageLevels.Last().Voltage) {
				return (VoltageLevels.Last(), VoltageLevels.Last());
			}
			return VoltageLevels.GetSection(x => voltage > x.Voltage);
		}
	}

	public class ElectricMotorVoltageLevelData
	{
		protected PerSecond _maxSpeed;

		[SIRange(0, double.MaxValue)]
		public Volt Voltage { get; internal set; }
		
		[ValidateObject]
		public ElectricMotorFullLoadCurve FullLoadCurve { get; protected internal set; }


		[ValidateObject]
		public EfficiencyMap EfficiencyMap { get; set; }

		public virtual PerSecond MaxSpeed => _maxSpeed ?? (_maxSpeed = VectoMath.Min(EfficiencyMap.MaxSpeed, FullLoadCurve.MaxSpeed));

		public virtual EfficiencyMap.EfficiencyResult LookupElectricPower(PerSecond avgSpeed, NewtonMeter torque, uint gear, bool allowExtrapolation)
		{
			return EfficiencyMap.LookupElectricPower(avgSpeed, torque, allowExtrapolation);
		}

		public virtual NewtonMeter LookupTorque(Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque, uint gear)
		{
			return EfficiencyMap.LookupTorque(electricPower, avgSpeed, maxEmTorque);
		}

		public virtual NewtonMeter FullLoadDriveTorque(PerSecond avgSpeed, uint gear)
		{
			return FullLoadCurve.FullLoadDriveTorque(avgSpeed);
		}

		public virtual NewtonMeter FullGenerationTorque(PerSecond avgSpeed, uint gear)
		{
			return FullLoadCurve.FullGenerationTorque(avgSpeed);
		}
	}

	public class IEPCVoltageLevelData : ElectricMotorVoltageLevelData
	{
		[ValidateObject]
		public Dictionary<uint, EfficiencyMap> EfficiencyMaps {  get; set; }

        public Dictionary<uint, ElectricMotorFullLoadCurve> FullLoadCurves { get; set; } = new Dictionary<uint, ElectricMotorFullLoadCurve>();

        public override PerSecond MaxSpeed => _maxSpeed ?? (_maxSpeed = VectoMath.Min(
			FullLoadCurve?.MaxSpeed ?? FullLoadCurves.Min(x => x.Value.MaxSpeed), EfficiencyMaps.Values.Min(x => x.MaxSpeed)));

		public override EfficiencyMap.EfficiencyResult LookupElectricPower(PerSecond avgSpeed, NewtonMeter torque, uint gear, bool allowExtrapolation)
		{
			return EfficiencyMaps[gear].LookupElectricPower(avgSpeed, torque, allowExtrapolation);
		}

		public override NewtonMeter LookupTorque(Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque, uint gear)
		{
			return EfficiencyMaps[gear].LookupTorque(electricPower, avgSpeed, maxEmTorque);
		}

        public override NewtonMeter FullGenerationTorque(PerSecond avgSpeed, uint gear)
        {
            return (FullLoadCurves.Count == 0)
				? base.FullGenerationTorque(avgSpeed, gear)
				: FullLoadCurves[gear].FullGenerationTorque(avgSpeed);
        }

        public override NewtonMeter FullLoadDriveTorque(PerSecond avgSpeed, uint gear)
        {
            return (FullLoadCurves.Count == 0)
				? base.FullLoadDriveTorque(avgSpeed, gear)
				: FullLoadCurves[gear].FullLoadDriveTorque(avgSpeed);
        }
    }

	public class IHPCVoltageLevelData : IEPCVoltageLevelData
	{

	}
	
	public class DeratedVoltageLevelData : ElectricMotorVoltageLevelData
	{
		public DeratedVoltageLevelData(PerSecond maxSpeed)
		{
			_maxSpeed = maxSpeed;
		}
		
		#region Overrides of ElectricMotorVoltageLevelData
		public override EfficiencyMap.EfficiencyResult LookupElectricPower(PerSecond avgSpeed, NewtonMeter torque, uint gear, bool allowExtrapolation)
		{
			throw new NotImplementedException();
		}
		public override NewtonMeter LookupTorque(Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque, uint gear)
		{
			throw new NotImplementedException();
		}
		public override NewtonMeter FullLoadDriveTorque(PerSecond avgSpeed, uint gear)
		{
			throw new NotImplementedException();
		}
		public override NewtonMeter FullGenerationTorque(PerSecond avgSpeed, uint gear)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	

	public class OverloadData
	{
		public NewtonMeter ContinuousTorque { get; internal set; }

		public NewtonMeter ContinuousTorqueGen { get; internal set; }

		public Watt ContinuousPower { get; internal set; }

		public Joule OverloadBuffer { get; internal set; }

		public Watt ContinuousPowerLoss { get; internal set; }
	}
}