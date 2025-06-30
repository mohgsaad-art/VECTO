
using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	internal abstract class VehicleDataAdapter : ComponentDataAdapterBase, IVehicleDataAdapter
	{
		public static NewtonMeter VehMaxTorque(
			ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2)
			{
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.TryGetValue(gear.Gear, out var limit) && limit.MaxTorque <=
					DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque)
				{
					return limit.MaxTorque;
				}
			}

			return null;
		} 
		internal static IADASDataAdapter _adasDataAdapter = new ADASDataAdapter();
		internal static VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
				OffVehicleCharging = data.OVC,
				H2StorageUsableCapacity = data.H2StorageUsableCapacity
			};

			return retVal;
		}


		#region Implementation of IVehicleDataAdapter

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			Kilogram loading,
			double? passengerCount,
			bool allowVocational)
		{
			CheckDeclarationMode(data, "Vehicle");
			return DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}
		public VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
			bool allowVocational)
		{
			CheckDeclarationMode(data, "Vehicle");
			return DoCreateVehicleData(data, segment, mission, loading.Value.Item1, loading.Value.Item2, allowVocational);
		}
		protected abstract VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			Kilogram loading,
			double? passengerCount,
			bool allowVocational);

		public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			throw new NotImplementedException("Method only applicable for completed specific bus!");
		}

		protected static VehicleData GetVehicleData(IVehicleDeclarationInputData data,
			Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
			//			retVal.LegislativeClass = data.LegislativeClass;
			retVal.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			retVal.VehicleClass = segment.VehicleClass;
			retVal.SleeperCab = retVal.VehicleClass.IsMediumLorry() ? false : data.SleeperCab;
			retVal.TrailerGrossVehicleMass = mission.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0);

			retVal.BodyAndTrailerMass =
				mission.BodyCurbWeight + mission.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);

			retVal.Loading = loading;
			retVal.PassengerCount = passengerCount;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();
			if (segment.VehicleClass.IsMediumLorry() && segment.VehicleClass.IsVan())
			{
				retVal.CargoVolume = data.CargoVolume;
			}
			else
			{
				retVal.CargoVolume = mission.MissionType != MissionType.Construction
					? mission.TotalCargoVolume
					: 0.SI<CubicMeter>();
			}

			retVal.VocationalVehicle = allowVocational && data.VocationalVehicle;
			retVal.ADAS = _adasDataAdapter.CreateADAS(data.ADAS);

			var axles = data.Components.AxleWheels.AxlesDeclaration;
			if (axles.Count < mission.AxleWeightDistribution.Length)
			{
				throw new VectoException(
					"Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
					axles.Count, mission.AxleWeightDistribution.Length);
			}

			var axleData = new List<Axle>();
			for (var i = 0; i < mission.AxleWeightDistribution.Length; i++)
			{
				var axleInput = axles[i];
				var axle = new Axle
				{
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
					CertificationNumber = axleInput.Tyre.CertificationNumber,
					DigestValueInput = axleInput.Tyre.DigestValue == null
						? ""
						: axleInput.Tyre.DigestValue.DigestValue,
				};
				axleData.Add(axle);
			}

			foreach (var trailer in mission.Trailer)
			{
				axleData.AddRange(
					trailer.TrailerWheels.Select(
						trailerWheel => new Axle
						{
							AxleType = AxleType.Trailer,
							AxleWeightShare = trailer.TrailerAxleWeightShare / trailer.TrailerWheels.Count,
							TwinTyres = DeclarationData.Trailer.TwinTyres,
							RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
							TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
							FuelEfficiencyClass = DeclarationData.Trailer.FuelEfficiencyClass,
							Inertia = trailerWheel.Inertia,
							WheelsDimension = trailerWheel.WheelType
						}));
			}

			retVal.AxleData = axleData;
			return retVal;
		}
		#endregion
	}

	internal class LorryVehicleDataAdapter : VehicleDataAdapter
	{
		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			return GetVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}

	}

	internal class ExemptedLorryVehicleDataAdapter : LorryVehicleDataAdapter
	{
		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			var exempted = SetCommonVehicleData(data);
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			return exempted;
		}
	}

	internal class PrimaryBusVehicleDataAdapter : LorryVehicleDataAdapter
	{
		#region Overrides of VehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal = base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);

			retVal.CurbMass = mission.CurbMass;
			if (mission.BusParameter.BusGroup.IsPrimaryBus() && !mission.BusParameter.CurbMassTPMLMFactor.IsNaN() &&
				data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor < mission.CurbMass) {
				retVal.CurbMass = data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor;
			}
			retVal.GrossVehicleMass = 40000.SI<Kilogram>();
			return retVal;
		}

		#endregion

		protected virtual SIBase<Kilogram> CalculateElectricComponentMass(IVehicleDeclarationInputData data)
		{
			var additionalMass = new List<Tuple<String, Kilogram>>();

			if (data.Components.ElectricMachines != null) {
				foreach (var emData in data.Components.ElectricMachines.Entries) {
					var emVoltage = emData.ElectricMachine.VoltageLevels.OrderBy(x => x.VoltageLevel).Last();
					var emContPwr = emVoltage.ContinuousTorque * emVoltage.ContinuousTorqueSpeed;
					var massEM = emContPwr * emData.Count * DeclarationData.EM_MassPerPower + DeclarationData.EM_MassElectronics +
								DeclarationData.EM_MassInverter;
					additionalMass.Add(Tuple.Create(emData.Position.GetName(),
						VectoMath.Round(massEM, MidpointRounding.AwayFromZero)));
				}
			}

			if (data.Components.IEPC != null) {
				var iepc = data.Components.IEPC;
				var emVoltage = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).Last();
				var emContPwr = emVoltage.ContinuousTorque * emVoltage.ContinuousTorqueSpeed;
				var massEM = emContPwr * DeclarationData.EM_MassPerPower + DeclarationData.EM_MassElectronics +
							DeclarationData.EM_MassInverter;
				additionalMass.Add(Tuple.Create("IEPC",
					VectoMath.Round(massEM, MidpointRounding.AwayFromZero)));
			}

			var count = 0;
			foreach (var reess in data.Components.ElectricStorage.ElectricStorageElements) {
				if (reess.REESSPack is IBatteryPackDeclarationInputData battery) {
					var ocvMap = BatterySOCReader.Create(battery.VoltageCurve);
					var capacity = battery.Capacity * ocvMap.Lookup(0.5);
					var batteryMass = VectoMath.Round(capacity * DeclarationData.Battery_MassPerCapacity, MidpointRounding.AwayFromZero);
					additionalMass.Add(Tuple.Create($"bat_{count}", batteryMass));
					count += 1;
				}
			}

			return additionalMass.Sum(x => x.Item2);
		}
	}

	internal class ExemptedPrimaryBusVehicleDataAdapter : PrimaryBusVehicleDataAdapter
	{
		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment,
			Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational) {
			var exempted = new VehicleData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				//CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.AxleConfiguration = data.AxleConfiguration;
			return exempted;
		}
	}

	internal class PrimaryBusVehicleDataAdapter_HEV : PrimaryBusVehicleDataAdapter
	{
		#region Overrides of PrimaryBusVehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal =  base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);

			if (!mission.BusParameter.CurbMassTPMLMFactor.IsNaN() &&
				data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor < mission.CurbMass) {
				retVal.CurbMass = data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor;
			} else {
				retVal.CurbMass = mission.CurbMass + CalculateElectricComponentMass(data);
			}

			return retVal;
		}

		#endregion

	}

	internal class PrimaryBusVehicleDataAdapter_PEV : PrimaryBusVehicleDataAdapter
	{
		#region Overrides of PrimaryBusVehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal = base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);

			if (!mission.BusParameter.CurbMassTPMLMFactor.IsNaN() &&
				data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor < mission.CurbMass) {
				retVal.CurbMass = data.GrossVehicleMassRating * mission.BusParameter.CurbMassTPMLMFactor;
			} else {
				retVal.CurbMass = mission.CurbMass - mission.GenericMassICE + CalculateElectricComponentMass(data);

			}
			return retVal;
		}

		#endregion
	}


	internal class CompletedBusGenericVehicleDataAdapter : PrimaryBusVehicleDataAdapter
	{
		#region Overrides of PrimaryBusVehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal = base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
			retVal.GrossVehicleMass = data.GrossVehicleMassRating;
			
			return retVal;
		}

		#endregion
	}

	internal class CompletedBusGenericVehicleDataAdapter_HEV : PrimaryBusVehicleDataAdapter_HEV
	{

	}

	internal class CompletedBusGenericVehicleDataAdapter_PEV : PrimaryBusVehicleDataAdapter_PEV
	{

	}


	public class CompletedBusSpecificVehicleDataAdapter : IVehicleDataAdapter
	{
		protected IVehicleDataAdapter completedBusGenericDataAdapter = new CompletedBusGenericVehicleDataAdapter();

		#region Overrides of IVehicleDataAdapter

		public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			throw new NotImplementedException();
		}

		public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			if (completedVehicle.NumberPassengerSeatsLowerDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsLowerDeck input parameter is required");
			}
			if (completedVehicle.NumberPassengerSeatsUpperDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsUpperDeck input parameter is required");
			}
			if (completedVehicle.NumberPassengersStandingLowerDeck == null) {
				throw new VectoException("NumberOfPassengersStandingLowerDeck input parameter is required");
			}
			if (completedVehicle.NumberPassengersStandingUpperDeck == null) {
				throw new VectoException("NumberOfPassengersStandingUpperDeck input parameter is required");
			}
			var passengers = DeclarationData.GetNumberOfPassengers(
				mission, completedVehicle.Length, completedVehicle.Width,
				completedVehicle.NumberPassengerSeatsLowerDeck.Value + completedVehicle.NumberPassengerSeatsUpperDeck.Value,
				completedVehicle.NumberPassengersStandingLowerDeck.Value + completedVehicle.NumberPassengersStandingUpperDeck.Value,
				loading.Key);

			var vehicleData = completedBusGenericDataAdapter.CreateVehicleData(primaryVehicle, segment, mission, loading.Value.Item1, loading.Value.Item2, false);
			vehicleData.InputData = completedVehicle;
			vehicleData.VIN = completedVehicle.VIN;
			vehicleData.LegislativeClass = completedVehicle.LegislativeClass;
			vehicleData.VehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
			vehicleData.Manufacturer = completedVehicle.Manufacturer;
			vehicleData.ModelName = completedVehicle.Model;
			vehicleData.ManufacturerAddress = completedVehicle.ManufacturerAddress;
			vehicleData.CurbMass = completedVehicle.CurbMassChassis;

			vehicleData.Loading = passengers * mission.MissionType.GetAveragePassengerMass();
			vehicleData.PassengerCount = passengers;
			vehicleData.GrossVehicleMass = completedVehicle.GrossVehicleMassRating;
			vehicleData.DigestValueInput = completedVehicle.DigestValue?.DigestValue ?? "";

			vehicleData.RegisteredClass = completedVehicle.RegisteredClass;

			vehicleData.VehicleCode = completedVehicle.VehicleCode;
			if (vehicleData.TotalVehicleMass.IsGreater(vehicleData.GrossVehicleMass)) {
				throw new VectoException("Total Vehicle Mass exceeds Gross Vehicle Mass for completed bus specific ({0}/{1})",
					vehicleData.TotalVehicleMass, vehicleData.GrossVehicleMass);
			}
			return vehicleData;
		}

		#endregion
	}

	internal class ExemptedCompletedBusSpecificVehicleDataAdapter : CompletedBusSpecificVehicleDataAdapter
	{
		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			return new VehicleData() {
				ModelName = completedVehicle.Model,
				Manufacturer = completedVehicle.Manufacturer,
				ManufacturerAddress = completedVehicle.ManufacturerAddress,
				VIN = completedVehicle.VIN,
				LegislativeClass = completedVehicle.LegislativeClass,
				RegisteredClass = completedVehicle.RegisteredClass,
				VehicleCode = completedVehicle.VehicleCode,
				VehicleCategory = VehicleCategory.HeavyBusCompletedVehicle,
				CurbMass = completedVehicle.CurbMassChassis,
				GrossVehicleMass = completedVehicle.GrossVehicleMassRating,
				ZeroEmissionVehicle = primaryVehicle.ZeroEmissionVehicle,
				MaxNetPower1 = primaryVehicle.MaxNetPower1,
				InputData = completedVehicle
			};
		}
	}


	internal class SingleBusVehicleDataAdapter : VehicleDataAdapter
	{
		#region Overrides of VehicleDataAdapter

		public VehicleData CreateVehicleData(ISingleBusInputDataProvider data, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var completedVehicle = data.CompletedVehicle;
			var vehicle = data.JobInputData.Vehicle;
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(
				completedVehicle.Length,
				completedVehicle.Width);
			var passengerCountRef = busFloorArea * (loading.Key == LoadingType.LowLoading
				? mission.BusParameter.PassengerDensityLow
				: mission.BusParameter.PassengerDensityRef);
			var passengerCountDecl = completedVehicle.NumberPassengerSeatsUpperDeck +
									completedVehicle.NumberPassengerSeatsLowerDeck
									+ (mission.MissionType == MissionType.Coach
										? 0
										: completedVehicle.NumberPassengersStandingLowerDeck +
										completedVehicle.NumberPassengersStandingUpperDeck);

			//var refLoad = passengerCount * mission.MissionType.GetAveragePassengerMass();
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading)
			{
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}

			var passengerCountCalc = loading.Key == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, (int)passengerCountDecl)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
			var payload = passengerCountCalc * mission.MissionType.GetAveragePassengerMass();

			var retVal = DoCreateVehicleData(vehicle, segment, mission, payload, passengerCountCalc, allowVocational);
			retVal.CurbMass = completedVehicle.CurbMassChassis;
			return retVal;
		}

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			return GetVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}


		#endregion
	}
}
