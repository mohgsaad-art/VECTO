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
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public abstract class EngineComponentDataAdapter : ComponentDataAdapterBase, IEngineDataAdapter
	{
		internal static EngineFullLoadCurve IntersectFullLoadCurves(EngineFullLoadCurve engineCurve, NewtonMeter maxTorque)
		{
			if (maxTorque == null)
			{
				return engineCurve;
			}

			var entries = new List<EngineFullLoadCurve.FullLoadCurveEntry>();
			var firstEntry = engineCurve.FullLoadEntries.First();
			if (firstEntry.TorqueFullLoad < maxTorque)
			{
				entries.Add(engineCurve.FullLoadEntries.First());
			}
			else
			{
				entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry
				{
					EngineSpeed = firstEntry.EngineSpeed,
					TorqueFullLoad = maxTorque,
					TorqueDrag = firstEntry.TorqueDrag
				});
			}
			foreach (var entry in engineCurve.FullLoadEntries.Pairwise(Tuple.Create))
			{
				if (entry.Item1.TorqueFullLoad <= maxTorque && entry.Item2.TorqueFullLoad <= maxTorque)
				{
					// segment is below maxTorque line -> use directly
					entries.Add(entry.Item2);
				}
				else if (entry.Item1.TorqueFullLoad > maxTorque && entry.Item2.TorqueFullLoad > maxTorque)
				{
					// segment is above maxTorque line -> add limited entry
					entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry
					{
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = maxTorque,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				}
				else
				{
					// segment intersects maxTorque line -> add new entry at intersection
					var edgeFull = Edge.Create(
						new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueFullLoad.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueFullLoad.Value()));
					var edgeDrag = Edge.Create(
						new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueDrag.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueDrag.Value()));
					var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
					if (!entries.Any(x => x.EngineSpeed.IsEqual(intersectionX)) && !intersectionX.IsEqual(entry.Item2.EngineSpeed.Value()))
					{
						entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry
						{
							EngineSpeed = intersectionX.SI<PerSecond>(),
							TorqueFullLoad = maxTorque,
							TorqueDrag = VectoMath.Interpolate(edgeDrag.P1, edgeDrag.P2, intersectionX).SI<NewtonMeter>()
						});
					}

					entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry
					{
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = entry.Item2.TorqueFullLoad > maxTorque ? maxTorque : entry.Item2.TorqueFullLoad,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				}
			}

			var flc = new EngineFullLoadCurve(entries.ToList(), engineCurve.PT1Data)
			{
				EngineData = engineCurve.EngineData,
			};
			return flc;
		}
		
		protected static WHRData CreateWHRData(IWHRData whrInputData, MissionType missionType, WHRType type)
		{
			if (whrInputData == null || whrInputData.GeneratedPower == null)
			{
				throw new VectoException("Missing WHR Data");
			}

			var whr = new WHRData()
			{
				CFUrban = whrInputData.UrbanCorrectionFactor,
				CFRural = whrInputData.RuralCorrectionFactor,
				CFMotorway = whrInputData.MotorwayCorrectionFactor,
				CFColdHot = whrInputData.BFColdHot,
				CFRegPer = whrInputData.CFRegPer,
				WHRMap = WHRPowerReader.Create(whrInputData.GeneratedPower, type)
			};
			whr.WHRCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
				missionType.GetNonEMSMissionType(), whr.CFRural, whr.CFUrban,
				whr.CFMotorway) * whr.CFColdHot * whr.CFRegPer;
			return whr;
		}
		public static CombustionEngineData SetCommonCombustionEngineData(IEngineDeclarationInputData data, TankSystem? tankSystem)
		{
			var retVal = new CombustionEngineData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationNumber = data.CertificationNumber,
				CertificationMethod = CertificationMethod.Measured,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				Displacement = data.Displacement,
				//IdleSpeed = data.IdleSpeed,
				//ConsumptionMap = FuelConsumptionMapReader.Create(data.FuelConsumptionMap),
				RatedPowerDeclared = data.RatedPowerDeclared,
				RatedSpeedDeclared = data.RatedSpeedDeclared,
				MaxTorqueDeclared = data.MaxTorqueDeclared,
				//FuelData = DeclarationData.FuelData.Lookup(data.FuelType, tankSystem)
				MultipleEngineFuelModes = data.EngineModes.Count > 1,
				WHRType = data.WHRType,
			};
			return retVal;
		}

		#region Implementation of IEngineDataAdapter

		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, IEngineModeDeclarationInputData mode,
			Mission mission)
		{
			CheckDeclarationMode(primaryVehicle.Components.EngineInputData, "EngineData");
			return DoCreateEngineData(primaryVehicle, mode, mission);
		}

		public abstract CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);

		protected abstract CombustionEngineData DoCreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission);

		#endregion
	}

	public class CombustionEngineComponentDataAdapter : EngineComponentDataAdapter
	{
		#region Overrides of EngineDataAdapter

		public override CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
		{
			throw new NotImplementedException();
		}

		protected override CombustionEngineData DoCreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
		{
			var engine = vehicle.Components.EngineInputData;
			var gearbox = vehicle.Components.GearboxInputData;

			var retVal = SetCommonCombustionEngineData(engine, vehicle.TankSystem);
			retVal.IdleSpeed = VectoMath.Max(mode.IdleSpeed, vehicle.EngineIdleSpeed);

			retVal.Fuels = new List<CombustionEngineFuelData>();
			foreach (var fuel in mode.Fuels)
			{
				retVal.Fuels.Add(
					new CombustionEngineFuelData()
					{
						WHTCUrban = fuel.WHTCUrban,
						WHTCRural = fuel.WHTCRural,
						WHTCMotorway = fuel.WHTCMotorway,
						ColdHotCorrectionFactor = fuel.ColdHotBalancingFactor,
						CorrectionFactorRegPer = fuel.CorrectionFactorRegPer,
						FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicle.TankSystem),
						ConsumptionMap = FuelConsumptionMapReader.Create(fuel.FuelConsumptionMap),
						FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
							mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban,
							fuel.WHTCMotorway) * fuel.ColdHotBalancingFactor * fuel.CorrectionFactorRegPer,
					});
			}

			retVal.Inertia = DeclarationData.Engine.EngineInertia(vehicle.VehicleType, retVal.Displacement, gearbox?.Type ?? GearboxType.NoGearbox);
			//retVal.Inertia = engine.Inertia +
			//				(gbx != null && gbx.Type.AutomaticTransmission()
			//					? (gbx.Type == GearboxType.APTN || gbx.Type == GearboxType.IHPC ? 0.SI<KilogramSquareMeter>() : torqueConverter.Inertia)
			//					: 0.SI<KilogramSquareMeter>());

			retVal.EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime;
			var limits = vehicle.TorqueLimits?.ToDictionary(e => e.Gear) ??
						new Dictionary<int, ITorqueLimitInputData>();
			//var numGears = gearbox?.Gears.Count ?? 0;
			var gears = GearboxDataAdapterBase.FilterDisabledGears(vehicle.TorqueLimits, gearbox);
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(gears.Count + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(mode.FullLoadCurve, true);
			fullLoadCurves[0].EngineData = retVal;
			
			foreach (var gear in gearbox?.Gears ?? new List<ITransmissionInputData>(0))
			{
				var maxTorque = VectoMath.Min(
					GearboxDataAdapterBase.GbxMaxTorque(gear, gears.Count + 1, fullLoadCurves[0].MaxTorque),
					VehicleDataAdapter.VehMaxTorque(gear, gears.Count + 1, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			//Add first full load curve for every gear present in iepc

			if (vehicle.Components.IEPC?.Gears != null)
				foreach (var gear in vehicle.Components.IEPC.Gears) {
					fullLoadCurves[(uint)gear.GearNumber] = fullLoadCurves[0];
				}


			retVal.FullLoadCurves = fullLoadCurves;

			retVal.WHRType = engine.WHRType;
			if ((retVal.WHRType & WHRType.ElectricalOutput) != 0)
			{
				retVal.ElectricalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataElectrical, mission.MissionType, WHRType.ElectricalOutput);
			}

			if ((retVal.WHRType & WHRType.MechanicalOutputDrivetrain) != 0)
			{
				retVal.MechanicalWHR = CreateWHRData(
					mode.WasteHeatRecoveryDataMechanical, mission.MissionType, WHRType.MechanicalOutputDrivetrain);
			}

			return retVal;
		}


		#endregion
	}

	public class GenericCombustionEngineComponentDataAdapter : EngineComponentDataAdapter
	{
		GenericBusEngineData busEngineData = GenericBusEngineData.Instance;
		#region Overrides of EngineComponentDataAdapter

		protected override CombustionEngineData DoCreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode,
			Mission mission)
		{
			throw new NotImplementedException();
		}

		public override CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
		{
			return busEngineData.CreateGenericBusEngineData(primaryVehicle, modeIdx, mission);
		}

		#endregion
	}


}