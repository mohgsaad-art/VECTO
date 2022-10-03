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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using DriverData = TUGraz.VectoCore.Models.SimulationComponent.Data.DriverData;

namespace TUGraz.VectoCore.Models.Simulation.Data
{
	[CustomValidation(typeof(VectoRunData), "ValidateRunData")]
	public class VectoRunData : SimulationComponentData
	{

		public VectoRunData()
		{
			Exempted = false;
			JobType = VectoSimulationJobType.ConventionalVehicle;
		}

		public VectoSimulationJobType JobType { get; internal set; }

		public MeterPerSecond VehicleDesignSpeed { get; internal set; }

		[ValidateObject]
		public VehicleData VehicleData { get; internal set; }

		[ValidateObject]
		public AirdragData AirdragData { get; internal set; }

		[ValidateObject]
		public CombustionEngineData EngineData { get; internal set; }

		[ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[ValidateObject]
		public AngledriveData AngledriveData { get; internal set; }

		[Required, ValidateObject]
		public IDrivingCycleData Cycle { get; internal set; }

		[ValidateObject]
		public IEnumerable<AuxData> Aux { get; internal set; }

		public IAuxiliaryConfig BusAuxiliaries { get; internal set; }

		[ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[ValidateObject]
		public PTOData PTO { get; internal set; }

		[ValidateObject]
		public DriverData DriverData { get; internal set; }

		public ExecutionMode ExecutionMode { get; internal set; }

		[Required, MinLength(1)]
		public string JobName { get; internal set; }

		public string ModFileSuffix { get; internal set; }

		[ValidateObject]
		[JsonIgnore]
		public IDeclarationReport Report { get; internal set; }

		[Required, ValidateObject]
		public LoadingType Loading { get; internal set; }

		[ValidateObject]
		[JsonIgnore]
		public Mission Mission { get; internal set; }

		[JsonIgnore]
		public XElement InputDataHash { get; internal set; }

		public int JobRunId { get; internal set; }

		public AuxFanData FanData { get; internal set; }

		public SimulationType SimulationType { get; set; }

		public VTPData VTPData { get; set; }

		public ShiftStrategyParameters GearshiftParameters { get; internal set; }

		public bool Exempted { get; set; }

		public IDrivingCycleData PTOCycleWhileDrive { get; internal set; }

		public string ShiftStrategy { get; set; }

		public Watt ElectricAuxDemand { get; internal set; }

		public NewtonMeter TorqueDriftLeftWheel { get; internal set; }

		public NewtonMeter TorqueDriftRightWheel { get; internal set; }

		public class AuxData
		{
			// ReSharper disable once InconsistentNaming
			public string ID;

			public IList<string> Technology;

			[SIRange(0, 100 * Constants.Kilo)] public Watt PowerDemand;

			[Required] public AuxiliaryDemandType DemandType;

			public MissionType? MissionType;
		}

		public static ValidationResult ValidateRunData(VectoRunData runData, ValidationContext validationContext)
		{
			var gearboxData = runData.GearboxData;
			var engineData = runData.EngineData;

			if (gearboxData != null) {
				var validationResult = CheckPowertrainLossMapsSize(runData, gearboxData, engineData);
				if (validationResult != null) {
					return validationResult;
				}
			}

			if (runData.Cycle != null && runData.Cycle.Entries.Any(e => e.PTOActive == PTOActivity.PTOActivityDuringStop)) {
				if (runData.PTO == null || runData.PTO.PTOCycle == null) {
					return new ValidationResult("PTOCycle is used in DrivingCycle, but is not defined in Vehicle-Data.");
				}
			}

			if (runData.Cycle != null && runData.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) {
				if (runData.EngineData.PTORoadSweepEngineSpeed == null) {
					return new ValidationResult("RoadSweeping PTO activity detected in cycle but no min. engine speed during road sweeping provided");
				}

				if (runData.DriverData.PTODriveRoadsweepingGear == null || runData.DriverData.PTODriveRoadsweepingGear.Equals(new GearshiftPosition(0))) {
					return new ValidationResult("RoadSweeping PTO activity detected in cycle but no gear during road sweeping provided");
				}
			}

			if (runData.Cycle != null && runData.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) {
				if (runData.PTOCycleWhileDrive == null || runData.PTOCycleWhileDrive.Entries.Count == 0) {
					return new ValidationResult("PTO activity while driving detected in cycle but PTO cycle provided");
				}
			}

			if (runData.EngineData.PTORoadSweepEngineSpeed != null) {
				if (runData.EngineData.IdleSpeed.IsGreater(runData.EngineData.PTORoadSweepEngineSpeed)) {
					return new ValidationResult("PTO Operating enginespeed is below engine idling speed");
				}

				if (runData.EngineData.FullLoadCurves[0].N95hSpeed.IsSmaller(runData.EngineData.PTORoadSweepEngineSpeed)) {
					return new ValidationResult("PTO operating enginespeed is above n_95h");
				}
			}

			return ValidationResult.Success;
		}

		private static ValidationResult CheckPowertrainLossMapsSize(VectoRunData runData, GearboxData gearboxData,
			CombustionEngineData engineData)
		{
			
			var axleGearData = runData.AxleGearData;
			var angledriveData = runData.AngledriveData;
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;
			var axlegearRatio = axleGearData != null ? axleGearData.AxleGear.Ratio : 1.0;
			var dynamicTyreRadius = runData.VehicleData != null ? runData.VehicleData.DynamicTyreRadius : 0.0.SI<Meter>();

			var vehicleMaxSpeed = runData.EngineData.FullLoadCurves[0].N95hSpeed /
								runData.GearboxData.Gears[runData.GearboxData.Gears.Keys.Max()].Ratio / axlegearRatio /
								angledriveRatio * dynamicTyreRadius;
			var maxSpeed = VectoMath.Min(vehicleMaxSpeed, (runData.VehicleDesignSpeed ?? 90.KMPHtoMeterPerSecond()) + (runData.DriverData?.OverSpeed?.OverSpeed ?? 0.KMPHtoMeterPerSecond()));

			if (gearboxData.Gears.Count + 1 != engineData.FullLoadCurves.Count) {
				return
					new ValidationResult(
						string.Format("number of full-load curves in engine does not match gear count. engine fld: {0}, gears: {1}",
							engineData.FullLoadCurves.Count, gearboxData.Gears.Count));
			}

			foreach (var gear in gearboxData.Gears) {
				var maxEngineSpeed = VectoMath.Min(engineData.FullLoadCurves[gear.Key].RatedSpeed, gear.Value.MaxSpeed);
				for (var angularVelocity = engineData.IdleSpeed;
					angularVelocity < maxEngineSpeed;
					angularVelocity += 2.0 / 3.0 * (maxEngineSpeed - engineData.IdleSpeed) / 10.0) {
					if (!gear.Value.HasLockedGear) {
						continue;
					}

					var velocity = angularVelocity / gear.Value.Ratio / angledriveRatio / axlegearRatio * dynamicTyreRadius;

					if (velocity > maxSpeed) {
						continue;
					}

					for (var inTorque = engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity) / 3;
						inTorque < engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity);
						inTorque += 2.0 / 3.0 * engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity) / 10.0) {
						var validateRunData = CheckLossMapsEntries(gear, angularVelocity, inTorque, angledriveData, axleGearData, velocity);
						if (validateRunData != null) {
							return validateRunData;
						}
					}
				}
			}
			return null;
		}

		private static ValidationResult CheckLossMapsEntries(KeyValuePair<uint, GearData> gear, PerSecond engineSpeed,
			NewtonMeter inTorque, AngledriveData angledriveData, AxleGearData axleGearData, MeterPerSecond velocity)
		{
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;

			var tqLoss = gear.Value.LossMap.GetTorqueLoss(engineSpeed / gear.Value.Ratio, inTorque * gear.Value.Ratio);
			if (tqLoss.Extrapolated) {
				return new ValidationResult(
					string.Format("Interpolation of Gear-{0}-LossMap failed with torque={1} and angularSpeed={2}", gear.Key,
								inTorque, engineSpeed.ConvertToRoundsPerMinute()));

			}
			var angledriveTorque = (inTorque - tqLoss.Value) / gear.Value.Ratio;


			var axlegearTorque = angledriveTorque;
			if (hasAngleDrive) {
				var anglTqLoss = angledriveData.Angledrive.LossMap.GetTorqueLoss(
					engineSpeed / gear.Value.Ratio / angledriveRatio,
					angledriveTorque * angledriveRatio);
				if (anglTqLoss.Extrapolated) {
					return new ValidationResult(
						string.Format(
							"Interpolation of Angledrive-LossMap failed with torque={0} and angularSpeed={1}",
							angledriveTorque, (engineSpeed / gear.Value.Ratio).ConvertToRoundsPerMinute()));
				}
			}

			if (axleGearData != null) {
				var axleAngularVelocity = engineSpeed / gear.Value.Ratio / angledriveRatio / axleGearData.AxleGear.Ratio;
				
				var axlTqLoss = axleGearData.AxleGear.LossMap.GetTorqueLoss(axleAngularVelocity, axlegearTorque * axleGearData.AxleGear.Ratio);
				if (axlTqLoss.Extrapolated) { 
				return
						new ValidationResult(
							string.Format(
								"Interpolation of AxleGear-LossMap failed with torque={0} and angularSpeed={1} (gear={2}, velocity={3})",
								axlegearTorque, axleAngularVelocity.ConvertToRoundsPerMinute(), gear.Key, velocity));
				}
			}
			return null;
		}
	}

	public class VTPData
	{
		public Dictionary<FuelType, double> CorrectionFactors;

		public IList<IFuelNCVData> FuelNCVs;
	}

	public class AuxFanData
	{
		public double[] FanCoefficients;

		public Meter FanDiameter;
	}
}