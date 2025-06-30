using Ninject.Activation.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public abstract class GearboxDataAdapterBase : ComponentDataAdapterBase, IGearboxDataAdapter
	{
		#region Helper
		protected internal static NewtonMeter GbxMaxTorque(
			ITransmissionInputData gear, int numGears, NewtonMeter maxEngineTorque
		)
		{
			if (gear.Gear - 1 < numGears / 2)
			{
				// gears count from 1 to n, -> n entries, lower half can always limit
				if (gear.MaxTorque != null)
				{
					return gear.MaxTorque;
				}
			}
			else
			{
				// upper half can only limit if max-torque <= 90% of engine max torque
				if (gear.MaxTorque != null &&
					gear.MaxTorque <= DeclarationData.Engine.TorqueLimitGearboxFactor * maxEngineTorque)
				{
					return gear.MaxTorque;
				}
			}

			return null;
		}
		protected static void CreateTCSecondGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected static void CreateTCFirstGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected virtual void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1, 1, $"TCGear {i + 1}");
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected virtual void CreateATGearData(GearboxType gearboxType, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory, IDrivingCycleData cycle)
		{
			if (gearboxType == GearboxType.ATPowerSplit && i == 0)
			{
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}

			if (gearboxType == GearboxType.ATSerial)
			{
				if (i == 0)
				{
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1) {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}
					else if (gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
					// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
					// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);

						// NOTE: the lower gear in 'gears' dictionary has index i !!
						gears[i].Ratio = double.NaN;
						gears[i].LossMap = null;
					}
				}
				else {
					if ((cycle != null) && ((cycle.CycleType == CycleType.MeasuredSpeedGear) || (cycle.CycleType == CycleType.VTP))) {
						CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					}
				}
			}
        }
        protected virtual TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i,
			bool useEfficiencyFallback, VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			if (gear.LossMap != null)
			{
				return TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, $"Gear {i + 1}", true);
			}
			if (useEfficiencyFallback)
			{
				return TransmissionLossMapReader.Create(gear.Efficiency, gear.Ratio, $"Gear {i + 1}");
			}
			throw new InvalidFileFormatException("Gear {0} LossMap missing.", i + 1);
		}
		#endregion
		protected static void SetDeclarationData(GearboxData retVal)
		{
			retVal.Inertia = DeclarationData.Gearbox.Inertia;
			retVal.TractionInterruption = retVal.Type.TractionInterruption();
		}

		protected static GearboxData SetCommonGearboxData(IGearboxDeclarationInputData data)
		{
			return new GearboxData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationMethod != CertificationMethod.StandardValues ?
					data.CertificationNumber : "",
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				Type = data.Type
			};
		}

		public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes)
		{
			CheckDeclarationMode(inputData.Components.GearboxInputData, "Gearbox");
			CheckGearNumbers(inputData.Components?.GearboxInputData);
			return DoCreateGearboxData(inputData, runData, shiftPolygonCalculator, supportedGearboxTypes);
		}

		protected abstract GearboxData DoCreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes);


		public ShiftStrategyParameters CreateGearshiftData(double axleRatio,
			PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
		{
			var retVal = new ShiftStrategyParameters
			{
				TorqueReserve = DeclarationData.GearboxTCU.TorqueReserve,
				StartTorqueReserve = DeclarationData.GearboxTCU.TorqueReserveStart,
				TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
				StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,

				RatingFactorCurrentGear = gearboxType.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,
				
				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gearboxType.AutomaticTransmission()
					? (gearsCount > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCATSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = engineIdlingSpeed == null ? null : DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
					.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}


		protected virtual void CheckGearNumbers(IGearboxDeclarationInputData gbxInputData)
		{
			if (gbxInputData == null) {
				return;
			}
			var numGears = gbxInputData.Gears.Count;
			var gearNumbers = gbxInputData.Gears.Select(g => g.Gear);

			if (numGears < 1) {
				throw new VectoException("At least one Gear-Entry must be defined in Gearbox!");
			}

            if (gearNumbers.Distinct().Count() != numGears) {
				throw new VectoException("Duplicate gear number");
			}

			if (gearNumbers.Min() < 0 || gearNumbers.Max() > numGears) {
				throw new VectoException("Gear numbers out of range");
			}

		}




		/// <summary>
		/// Filters the gears based on disabling rule: Disable either the last 1 or 2 gears by setting their vehicle-level torque limit to 0.
		/// </summary>
		internal static IList<ITransmissionInputData> FilterDisabledGears(IList<ITorqueLimitInputData> torqueLimits, IGearboxDeclarationInputData gearboxData)
		{
			if (torqueLimits == null || torqueLimits.Count == 0) {
				return gearboxData?.Gears ?? new List<ITransmissionInputData>();
			}

			if (gearboxData == null) {
				return new List<ITransmissionInputData>();
			}

			var gearsInput = gearboxData.Gears;
			var lastGearNumber = gearsInput.Max(g => g.Gear);
			var toRemove = torqueLimits
				.Where(tqLimit => tqLimit.MaxTorque.IsEqual(0))
				.Select(tqLimit => gearsInput.FirstOrDefault(g => g.Gear == tqLimit.Gear))
				.Where(g => g != default)
				.OrderBy(g => g.Gear)
				.ToList();

			if ((toRemove.Count == 1 && toRemove[0].Gear != lastGearNumber)
				|| (toRemove.Count == 2 && (toRemove[0].Gear != lastGearNumber - 1 || toRemove[1].Gear != lastGearNumber))
				|| toRemove.Count > 2) {
				throw new VectoException("Only the last 1 or 2 gears can be disabled. Disabling gear {0} for a {1}-speed gearbox is not allowed.",
					toRemove.Min(g => g.Gear), gearsInput.Count);
			}

			foreach (var entry in toRemove) {
				gearsInput.Remove(entry);
			}

			return gearsInput;
		}
	}

	public class GearboxDataAdapter : GearboxDataAdapterBase
	{
		private readonly ITorqueConverterDataAdapter _torqueConverterDataAdapter;

		public GearboxDataAdapter(ITorqueConverterDataAdapter torqueConverterDataAdapter)
		{
			_torqueConverterDataAdapter = torqueConverterDataAdapter;
		}
		#region Overrides of GearBoxDataAdapterBase

		protected override GearboxData DoCreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes)
		{
			var gearbox = inputData.Components.GearboxInputData;

			var adas = inputData.ADAS;
			var torqueConverter = inputData.Components.TorqueConverterInputData;

			var engine = runData.EngineData;
			var axlegearRatio = runData.AxleGearData?.AxleGear.Ratio ?? 1.0f;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;

			var retVal = SetCommonGearboxData(gearbox);

			var isBatteryElectric = inputData.VehicleType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
				VectoSimulationJobType.SerialHybridVehicle);
			if (isBatteryElectric && (runData.ElectricMachinesData == null || runData.ElectricMachinesData.Count == 0)) {
				throw new VectoException(
					"Electric motor data has to be set for battery electric vehicles before creating gearbox data!");
			}

			if (isBatteryElectric && runData.GearshiftParameters == null) {
				throw new VectoException(
					"Gearshift parameters have to be set for battery electric vehicles before creating gearbox data!");
			}

            if ((inputData.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || inputData.VehicleType == VectoSimulationJobType.SerialHybridVehicle) &&
				gearbox.Type.AutomaticTransmission())
			{

				// PEV with APT-S or APT-P transmission are simulated as APT-N
				if (retVal.Type.IsOneOf(GearboxType.ATPowerSplit, GearboxType.ATSerial)) {
					retVal.Type = GearboxType.APTN;
				}
			}
            if (adas != null && retVal.Type.AutomaticTransmission()  && adas.EcoRoll != EcoRollType.None &&
                           !adas.ATEcoRollReleaseLockupClutch.HasValue)
            {
                throw new VectoException("Input parameter ATEcoRollReleaseLockupClutch required for AT transmission");
            }

            retVal.ATEcoRollReleaseLockupClutch =
				adas != null && adas.EcoRoll != EcoRollType.None && retVal.Type.AutomaticTransmission()
					? (adas.ATEcoRollReleaseLockupClutch.HasValue ? adas.ATEcoRollReleaseLockupClutch.Value : false)
					: false;

			if (!supportedGearboxTypes.Contains(gearbox.Type))
			{
				throw new VectoSimulationException("Unsupported gearbox type: {0}!", gearbox.Type);
			}

			var gearsInput = FilterDisabledGears(inputData.TorqueLimits, gearbox);  //gearbox.Gears;
			if (gearsInput.Count < 1)
			{
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}
			
			SetDeclarationData(retVal);
			
			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			var tcShiftPolygon = engine?.FullLoadCurves != null ? DeclarationData.TorqueConverter.ComputeShiftPolygon(engine.FullLoadCurves[0]) : null;

			
			var vehicleCategory = runData.VehicleData.VehicleCategory == VehicleCategory.GenericBusVehicle
				? VehicleCategory.GenericBusVehicle
				: inputData.VehicleCategory;
			for (uint i = 0; i < gearsInput.Count; i++)
			{
				var gear = gearsInput[(int)i];
				var lossMap = CreateGearLossMap(gear, i, false, vehicleCategory, gearbox.Type);

				var shiftPolygon = shiftPolygonCalculator != null
					? shiftPolygonCalculator.ComputeDeclarationShiftPolygon(
						gearbox.Type, (int)i, engine?.FullLoadCurves[i + 1], gearbox.Gears, engine, axlegearRatio,
						dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault(x => x.Item1 != PowertrainPosition.GEN)?.Item2)
					: DeclarationData.Gearbox.ComputeShiftPolygon(
						gearbox.Type, (int)i, engine?.FullLoadCurves[i + 1],
						gearsInput, engine,
						axlegearRatio, dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);

				ShiftPolygon extendedShiftPolygon = null;
				if (gearbox.Type == GearboxType.MT) {
					extendedShiftPolygon = shiftPolygonCalculator != null
						? shiftPolygonCalculator.ComputeDeclarationExtendedShiftPolygon(
							gearbox.Type, (int)i, engine?.FullLoadCurves[i + 1], gearbox.Gears, engine, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault(x => x.Item1 != PowertrainPosition.GEN)?.Item2)
						: DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygonExtended(
							(int)i, engine?.FullLoadCurves[i + 1],
							gearsInput,
							engine,
							axlegearRatio,
							dynamicTyreRadius);
				}

				var deratedEmShiftPolygon = isBatteryElectric
					? CalculateDeratedEmShiftPolygon(runData, shiftPolygonCalculator, gearbox, i, axlegearRatio,
						dynamicTyreRadius)
					: null;

				var gearData = new GearData
				{
					ShiftPolygon = shiftPolygon,
					DeRatedEmShiftPolygon = deratedEmShiftPolygon,
					ExtendedShiftPolygon = extendedShiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};
				
				CreateATGearData(retVal.Type, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears,
					runData.VehicleData.VehicleCategory, runData.Cycle);
				gears.Add(i + 1, gearData);
			}

			// remove disabled gears (only the last or last two gears may be removed)
			if (inputData.TorqueLimits != null)
			{
				var toRemove = (from tqLimit in inputData.TorqueLimits
								where tqLimit.Gear >= gears.Keys.Max() - 1 && tqLimit.MaxTorque.IsEqual(0)
								select (uint)tqLimit.Gear).ToList();
				if (toRemove.Count > 0 && toRemove.Min() <= gears.Count - toRemove.Count)
				{
					throw new VectoException(
						"Only the last 1 or 2 gears can be disabled. Disabling gear {0} for a {1}-speed gearbox is not allowed.",
						toRemove.Min(), gears.Count);
				}

				foreach (var entry in toRemove)
				{
					gears.Remove(entry);
				}
			}

			retVal.Gears = gears;
			
			if (retVal.Type.AutomaticTransmission() && retVal.Type != GearboxType.APTN && retVal.Type != GearboxType.IHPC)
			{
				if (torqueConverter == null) {
					throw new VectoException("Torque converter data is required for automatic transmission!");
				}
				var ratio = double.IsNaN(retVal.Gears[1].Ratio)
					? 1
					: retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime;

				retVal.TorqueConverterData =
					_torqueConverterDataAdapter.CreateTorqueConverterData(gearbox.Type, torqueConverter, ratio, engine);

				retVal.TorqueConverterData.Manufacturer = torqueConverter.Manufacturer;
				retVal.TorqueConverterData.ModelName = torqueConverter.Model;
				retVal.TorqueConverterData.DigestValueInput = torqueConverter.DigestValue?.DigestValue;
				retVal.TorqueConverterData.CertificationMethod = torqueConverter.CertificationMethod;
				retVal.TorqueConverterData.CertificationNumber = torqueConverter.CertificationNumber;
				retVal.TorqueConverterData.Date = torqueConverter.Date;
				retVal.TorqueConverterData.AppVersion = torqueConverter.AppVersion;
			}

			// update disengageWhenHaltingSpeed
			if (retVal.Type.AutomaticTransmission())
			{
				var firstGear = retVal.GearList.First(x => x.IsLockedGear());
				if (retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any())
				{
					var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
						.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
					var vehicleSpeedDisengage =
						downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
						dynamicTyreRadius;
					retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
				}
			}

			return retVal;
		}

		protected virtual ShiftPolygon CalculateDeratedEmShiftPolygon(VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, IGearboxDeclarationInputData gearbox, uint i, double axlegearRatio,
			Meter dynamicTyreRadius)
		{
			var em = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2;
			var shiftStrategyParameters = runData.GearshiftParameters;
			var emFld = em.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = em.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(em.EfficiencyData.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = em.RatioADC,
			};
			var deratedEmShiftPolygon = shiftPolygonCalculator.ComputeElectricMotorDeclarationShiftPolygon(
				gearbox.Type, (int)i,
				gearbox.Gears, axlegearRatio,
				dynamicTyreRadius, 
				em, 
				limitedEm);
			return deratedEmShiftPolygon;
			//retVal[i + 1] = shiftPolygon;
		}

		#endregion

	}

	public class GenericCompletedBusGearboxDataAdapter : GearboxDataAdapter
	{
		public const double GearEfficiencyDirectGear = 0.98;
		public const double GearEfficiencyIndirectGear = 0.96;
		public const double GearEfficiencyAT = 0.925;

		public GenericCompletedBusGearboxDataAdapter(ITorqueConverterDataAdapter torqueConverterDataAdapter) : base(
			torqueConverterDataAdapter)
		{

		}

		#region Overrides of GearboxDataAdapterBase

		protected override TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, bool useEfficiencyFallback,
			VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			if (gearboxType.AutomaticTransmission())
			{
				return TransmissionLossMapReader.Create(GearEfficiencyAT, gear.Ratio, $"Gear {i + 1}");
			}
			return TransmissionLossMapReader.Create(
				gear.Ratio.IsEqual(1) ? GearEfficiencyDirectGear : GearEfficiencyIndirectGear, gear.Ratio, $"Gear {i + 1}");
		}

		protected override void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			//gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(GearEfficiencyIndirectGear, 1, string.Format("TCGear {0}", i + 1));
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1.0, 1, $"TCGear {i + 1}");
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		#endregion
	}


	public class IEPCGearboxDataAdapter : GearboxDataAdapterBase
	{

		private GearboxData CreateIEPCGearboxData(IVehicleDeclarationInputData vehicle, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc)
		{
			var iepc = vehicle.Components.IEPC;

			var axlegearRatio = runData.AxleGearData?.AxleGear.Ratio ?? 1.0;
			var dynamicTyreRadius = runData.VehicleData.DynamicTyreRadius;

			var count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;

            var retVal = new GearboxData()
			{
				Type = GearboxType.APTN,
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
				InputData = new IEPCGearboxInputData(iepc),
			};


			var gearInput = iepc.Gears.Select((x, idx) => new TransmissionInputData() {
				Gear = idx + 1,
				Ratio = x.Ratio,
				MaxInputSpeed = x.MaxOutputShaftSpeed == null ? null : x.MaxOutputShaftSpeed * x.Ratio,
				MaxTorque = x.MaxOutputShaftTorque == null ? null : x.MaxOutputShaftTorque * count / x.Ratio,
			}).Cast<ITransmissionInputData>().ToList();
			var gears = new Dictionary<uint, GearData>();
			for (uint i = 0; i < iepc.Gears.Count; i++)
			{
				var gear = iepc.Gears[(int)i];
				var lossMap = TransmissionLossMapReader.Create(1, gear.Ratio, $"Gear{i + 1}");

				ShiftPolygon shiftPolygon = null;
                ShiftPolygon deratedEmShiftPolygon = null;
				if (iepc.Gears.Count > 1)
				{
					if (shiftPolygonCalc != null)
					{
						shiftPolygon = shiftPolygonCalc.ComputeDeclarationShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					}
					else
					{
						shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(GearboxType.APTN, (int)i,
							null, gearInput, null, axlegearRatio,
							dynamicTyreRadius, runData.ElectricMachinesData?.FirstOrDefault()?.Item2);
					}
					deratedEmShiftPolygon = CalculateDeratedEmShiftPolygon(runData, shiftPolygonCalc, gearInput, i, axlegearRatio,
						dynamicTyreRadius);
                }

				
                var gearData = new GearData
				{
					ShiftPolygon = shiftPolygon,
					DeRatedEmShiftPolygon = deratedEmShiftPolygon,
					MaxSpeed = gear.MaxOutputShaftSpeed == null ? null : gear.MaxOutputShaftSpeed * gear.Ratio,
					MaxTorque = gear.MaxOutputShaftTorque == null ? null : gear.MaxOutputShaftTorque * count / gear.Ratio,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				gears.Add(i + 1, gearData);
			}

			retVal.Gears = gears;

			// update disengageWhenHaltingSpeed
			var firstGear = retVal.GearList.First(x => x.IsLockedGear());
			if (iepc.Gears.Count > 1 && retVal.Gears[firstGear.Gear].ShiftPolygon.Downshift.Any())
			{
				var downshiftSpeedInc = retVal.Gears[firstGear.Gear].ShiftPolygon
					.InterpolateDownshiftSpeed(0.SI<NewtonMeter>()) * 1.05;
				var vehicleSpeedDisengage = downshiftSpeedInc / axlegearRatio / retVal.Gears[firstGear.Gear].Ratio *
											dynamicTyreRadius;
				retVal.DisengageWhenHaltingSpeed = vehicleSpeedDisengage;
			}

			return retVal;
		}

		protected virtual ShiftPolygon CalculateDeratedEmShiftPolygon(VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, List<ITransmissionInputData> gearInput, uint i,
			double axlegearRatio,
			Meter dynamicTyreRadius)
		{
			var em = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2;
			var shiftStrategyParameters = runData.GearshiftParameters;
			var emFld = em.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = em.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(em.EfficiencyData.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = em.RatioADC,
			};
			var deratedEmShiftPolygon = shiftPolygonCalculator.ComputeElectricMotorDeclarationShiftPolygon(GearboxType.APTN, (int)i,
				gearInput, axlegearRatio,
				dynamicTyreRadius, em, limitedEm);
			return deratedEmShiftPolygon;
			//retVal[i + 1] = shiftPolygon;
		}

        #region Overrides of GearboxDataAdapterBase

        protected override GearboxData DoCreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalculator, GearboxType[] supportedGearboxTypes)
		{
			return CreateIEPCGearboxData(inputData, runData, shiftPolygonCalculator);
		}

		protected override void CheckGearNumbers(IGearboxDeclarationInputData gbxInputData)
		{
			// TODO?
		}

		#endregion
	}

	public class GenericCompletedBusIEPCGearboxDataAdapter : IEPCGearboxDataAdapter
	{

	}


	public class CompletedSpecificBusGearboxDataAdapter : GenericCompletedBusGearboxDataAdapter
	{
		public CompletedSpecificBusGearboxDataAdapter(ITorqueConverterDataAdapter torqueConverterDataAdapter) : base(torqueConverterDataAdapter) { }
	}
}