using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;
using DeclarationDataAdapterHeavyLorry = TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry.DeclarationDataAdapterHeavyLorry;

namespace TUGraz.VectoCore.Models.Declaration
{
    public class GenericBusEngineData
	{
		#region Constants

		private static string GenericEngineCM_Normed_CI =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_CI_normalized.vmap";

		private static string GenericEngineCM_Normed_PI =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_PI_normalized.vmap";

		private static readonly double[] DieselCIFactors = { 1.05, 1.02, 1.0, 1.005, 1.0 };
		private static readonly double[] PIFactors = { 1.05, 1.02, 1.0, 1.005, 1.0 };

		private static GenericBusEngineData _instance;

		public static GenericBusEngineData Instance => _instance ?? (_instance = new GenericBusEngineData());

		#endregion

		protected GenericBusEngineData()
		{
			
		}

		public CombustionEngineData CreateGenericBusEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
		{
			if (modeIdx >= primaryVehicle.Components.EngineInputData.EngineModes.Count) {
				throw new VectoException(
					"requested engine mode {0}, only {1} modes in engine of primary vehicle available!", modeIdx,
					primaryVehicle.Components.EngineInputData.EngineModes.Count);
			}
			var engineData = primaryVehicle.Components.EngineInputData;
			var gearbox = primaryVehicle.Components.GearboxInputData;
			var idleSpeed = VectoMath.Max(engineData.EngineModes[modeIdx].IdleSpeed, primaryVehicle.EngineIdleSpeed);
			var engine = new CombustionEngineData {
				IdleSpeed = idleSpeed,
				Displacement = engineData.Displacement,
				WHRType = WHRType.None,
				Inertia = DeclarationData.Engine.EngineInertia(primaryVehicle.VehicleType, engineData.Displacement, gearbox?.Type),
				EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime,
				RatedPowerDeclared = engineData.RatedPowerDeclared,
				RatedSpeedDeclared = engineData.RatedSpeedDeclared,
				MaxTorqueDeclared = engineData.MaxTorqueDeclared,
			};

			var limits = primaryVehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var gears = GearboxDataAdapterBase.FilterDisabledGears(primaryVehicle.TorqueLimits, gearbox);
			
			var numGears = gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engineData.EngineModes[modeIdx].FullLoadCurve, true);
			fullLoadCurves[0].EngineData = engine;

			foreach (var gear in gears) {
				var maxTorque = VectoMath.Min(
					GearboxDataAdapter.GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehicleDataAdapter.VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}
			// TODO MQ 2024-10-22: IMO IPEC component is not relevant here!
			// a vehicle with an IEPC will never have a combustion engine in the powertrain!
			if (primaryVehicle.Components.IEPC?.Gears != null)
				foreach (var gear in primaryVehicle.Components.IEPC.Gears)
				{
					fullLoadCurves[(uint)gear.GearNumber] = fullLoadCurves[0];
				}


            engine.FullLoadCurves = fullLoadCurves;

			var engineMode = primaryVehicle.Components.EngineInputData.EngineModes[modeIdx];
			var fuel = GetCombustionEngineFuelData(engineMode.Fuels,
				VectoMath.Max(engineMode.IdleSpeed, primaryVehicle.EngineIdleSpeed), fullLoadCurves[0], mission);
			
			engine.Fuels = new List<CombustionEngineFuelData> { fuel };
			
			return engine;
		}


		private bool UseDieselFuel(IList<IEngineFuelDeclarationInputData> fuels)
		{
			var fuelType = fuels.First().FuelType;
			var isDualFuel = fuels.Count > 1;

			if (isDualFuel)
				return true;

			switch (fuelType) {
				case FuelType.DieselCI:
				case FuelType.DieselB100CI:
				case FuelType.EthanolCI:
				case FuelType.NGCI:
					return true;
				default:
					return false;
			}
		}

		private string GetEngineRessourceId(IList<IEngineFuelDeclarationInputData> fuels)
		{
			return UseDieselFuel(fuels) ? GenericEngineCM_Normed_CI : GenericEngineCM_Normed_PI;
		}

		private IFuelProperties GetFuelData(IList<IEngineFuelDeclarationInputData> fuels)
		{
			return UseDieselFuel(fuels)
				? FuelData.Diesel
				: FuelData.Instance().Lookup(FuelType.NGPI, TankSystem.Compressed);
		}

		private double[] GetEngineCorrectionFactors(IList<IEngineFuelDeclarationInputData> fuels)
		{
			return UseDieselFuel(fuels) ? DieselCIFactors : PIFactors;
		}

		private CombustionEngineFuelData GetCombustionEngineFuelData(IList<IEngineFuelDeclarationInputData> fuels, PerSecond idleSpeed, EngineFullLoadCurve fullLoadCurve, Mission mission)
		{
			var ressourceId = GetEngineRessourceId(fuels);

			var denormalizedData = DenormalizeData(ressourceId, idleSpeed, fullLoadCurve.N95hSpeed, fullLoadCurve.MaxPower);
			
			var engineSpeed = denormalizedData.AsEnumerable().Select(r => 
				r.Field<string>(FuelConsumptionMapReader.Fields.EngineSpeed).ToDouble()).ToArray();

			var clusterResult = new MeanShiftClustering().FindClusters(engineSpeed, 1);

			foreach (var entry in clusterResult) {
				var dragTorque = fullLoadCurve.DragLoadStationaryTorque(entry.RPMtoRad()).Value();
				var newRow = denormalizedData.NewRow();
				newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = Math.Round(entry, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[FuelConsumptionMapReader.Fields.Torque] =  Math.Round(dragTorque, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = 0;
				denormalizedData.Rows.Add(newRow);
				var newRow2 = denormalizedData.NewRow();
				newRow2[FuelConsumptionMapReader.Fields.EngineSpeed] = Math.Round(entry, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow2[FuelConsumptionMapReader.Fields.Torque] = Math.Round(dragTorque - 100, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow2[FuelConsumptionMapReader.Fields.FuelConsumption] = 0;
				denormalizedData.Rows.Add(newRow2);
			}

			var fcMap = FuelConsumptionMapReader.Create(denormalizedData.AsEnumerable().OrderBy(r => r.Field<string>(FuelConsumptionMapReader.Fields.EngineSpeed).ToDouble())
																		.ThenBy(r => r.Field<string>(FuelConsumptionMapReader.Fields.Torque).ToDouble()).CopyToDataTable());
			var engineCF = GetEngineCorrectionFactors(fuels);

			var fuel = new CombustionEngineFuelData
			{
				WHTCUrban = engineCF[0],
				WHTCRural = engineCF[1],
				WHTCMotorway = engineCF[2],
				ColdHotCorrectionFactor = engineCF[3],
				CorrectionFactorRegPer = engineCF[4],
				ConsumptionMap = fcMap,
				FuelData = GetFuelData(fuels)
			};
			fuel.FuelConsumptionCorrectionFactor = DeclarationData.WHTCCorrection.Lookup(
														mission.MissionType.GetNonEMSMissionType(), fuel.WHTCRural, fuel.WHTCUrban,
														fuel.WHTCMotorway) * fuel.ColdHotCorrectionFactor * fuel.CorrectionFactorRegPer;

			return fuel;
		}

		


		private DataTable DenormalizeData(string ressourceId, PerSecond nIdle, PerSecond n95h, Watt ratedPower)
		{
			var normalized = ReadCsvResource(ressourceId);

			var result = new DataTable();
			result.Columns.Add(FuelConsumptionMapReader.Fields.EngineSpeed);
			result.Columns.Add(FuelConsumptionMapReader.Fields.Torque);
			result.Columns.Add(FuelConsumptionMapReader.Fields.FuelConsumption);

			foreach (DataRow row in normalized.Rows) {
				var engineSpeed = row.ParseDouble("n_norm") * (n95h - nIdle) + nIdle;
				var pwr = row.ParseDouble("P_norm") * ratedPower;
				var torque = pwr / engineSpeed;
				var fc = (row.ParseDouble("FC_norm") * ratedPower.Value() / 1000).SI(Unit.SI.Gramm.Per.Hour) .Cast<KilogramPerSecond>();

				var newRow = result.NewRow();
				newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = Math.Round(engineSpeed.AsRPM,2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[FuelConsumptionMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = Math.Round(fc.ConvertToGrammPerHour().Value, 2, MidpointRounding.AwayFromZero).ToXMLFormat();
				result.Rows.Add(newRow);
			}

			return result;
		}

		private static TableData ReadCsvResource(string ressourceId)
		{
			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);
		}
	}
}
