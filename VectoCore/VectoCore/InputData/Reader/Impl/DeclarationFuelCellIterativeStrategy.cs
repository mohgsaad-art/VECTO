using System;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	internal class DeclarationFuelCellIterativeStrategy
	{
		/// <summary>
		/// Sets up the <see cref="FCHEVIterativeRunStrategy"/>.
		/// </summary>
		/// <param name="runData"></param>
		/// <param name="dataAdapter"></param>
		/// <param name="inputDataProvider"></param>
		/// <param name="fuelCellJobType"></param>
		/// <param name="fcBatteries"></param>
		/// <returns></returns>
		public static FCHEVIterativeRunStrategy SetUpFuelCellIterativeRunStrategy(
			VectoRunData runData,
			IVehicleDeclarationInputData vehicle,
			IDeclarationDataAdapter dataAdapter,
			IDeclarationInputDataProvider inputDataProvider,
			VectoSimulationJobType fuelCellJobType,
			Tuple<int, BatteryData> fcBatteries)
		{
			var iterativeRunStrategy = DeclarationFuelCellIterativeStrategy.SetUpFCHEVIterativeRunStrategy();

			iterativeRunStrategy.Update = (modData, iterationRunData) =>
			{
				var fchvDataAdapter = new FCHVDeclarationDataAdapter(inputDataProvider.DataSource);
				var fuelCellSystemData = dataAdapter.CreateFuelCells(vehicle.Components.FuelCellSystem).ConvertToEngineeringData();

				runData.BatteryData.Batteries = runData.BatteryData.Batteries
					.Where(b => b.Item1 != fcBatteries.Item1)
					.ToList();

				iterationRunData.JobType = fuelCellJobType;
				iterationRunData.ModFileSuffix = string.Empty;
				iterationRunData.FuelCellSystemData = fuelCellSystemData;
				iterationRunData.OVCMode = runData.OVCMode == OvcHevMode.NotApplicable
					? OvcHevMode.NotApplicable : OvcHevMode.ChargeSustaining;

				modData.PostProcessingCorrection = new FCHVPostProcessingCorrection();

				iterationRunData.FuelCellSystemData.FuelCellPowerMap =
					fchvDataAdapter.CreateFuelCellPowerMap(modData, iterationRunData.FuelCellSystemData, iterationRunData.BatteryData);
				iterationRunData.FuelCellSystemData.FuelCellShareMap =
					fchvDataAdapter.CreateFuelCellShareMap(fuelCellSystemData);

				/// Comment from [1]: In the real run we don't use a charge sustaining battery
				runData.BatteryData.ChargeSustainingBatterySystem = false;
				runData.ModFileSuffix += runData.Loading;
				runData.Iteration++;
			};

			return iterativeRunStrategy;
		}

		/// <summary>
		/// Sets up the <see cref="FCHEVIterativeRunStrategy"/> for Completed Buses.
		/// </summary>
		/// <param name="runData"></param>
		/// <param name="dataProvider"></param>
		/// <param name="dataAdapterGeneric"></param>
		/// <param name="GetPrimaryResult"></param>
		/// <param name="fuelCellJobType"></param>
		/// <param name="fcBatteries"></param>
		/// <returns><see cref="FCHEVIterativeRunStrategy"/> for Completed Buses.</returns>
		public static FCHEVIterativeRunStrategy SetUpFuelCellIterativeRunStrategy(
			VectoRunData runData,
			IMultistageVIFInputData dataProvider,
			IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric,
			Func<string, VectoRunData, OvcHevMode, IResult> GetPrimaryResult,
			VectoSimulationJobType fuelCellJobType,
			Tuple<int, BatteryData> fcBatteries)
		{
			var primaryVehicle = dataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;
			var iterativeRunStrategy = DeclarationFuelCellIterativeStrategy.SetUpFCHEVIterativeRunStrategy();
			var fuelCellData = dataAdapterGeneric.CreateFuelCells(primaryVehicle.Components.FuelCellSystem).ConvertToEngineeringData();

			iterativeRunStrategy.Update = (modData, iterationRunData) =>
			{
				var fchvDataAdapter = new FCHVDeclarationDataAdapter(dataProvider.DataSource);

				runData.BatteryData.Batteries = runData.BatteryData.Batteries
					.Where(b => b.Item1 != fcBatteries.Item1)
					.ToList();

				iterationRunData.JobType = fuelCellJobType;
				iterationRunData.ModFileSuffix = string.Empty;
				iterationRunData.FuelCellSystemData = fuelCellData;
				iterationRunData.OVCMode = runData.OVCMode == OvcHevMode.NotApplicable ? OvcHevMode.NotApplicable : OvcHevMode.ChargeSustaining;
				modData.PostProcessingCorrection = new FCHVPostProcessingCorrection();

				if (iterationRunData.PrimaryResult != null)
				{
					iterationRunData.PrimaryResult = GetPrimaryResult(null, iterationRunData, iterationRunData.OVCMode);
				}

				iterationRunData.FuelCellSystemData.FuelCellPowerMap =
					fchvDataAdapter.CreateFuelCellPowerMap(modData, iterationRunData.FuelCellSystemData, iterationRunData.BatteryData);
				iterationRunData.FuelCellSystemData.FuelCellShareMap = fchvDataAdapter.CreateFuelCellShareMap(fuelCellData);

				/// Comment from [1]: In the real run we don't use a charge sustaining battery
				runData.BatteryData.ChargeSustainingBatterySystem = false;
				runData.ModFileSuffix += runData.Loading;
				runData.Iteration++;
			};

			return iterativeRunStrategy;
		}

		public static FCHEVIterativeRunStrategy SetUpFCHEVIterativeRunStrategy()
		{
			return new FCHEVIterativeRunStrategy(
					new[]
					{
							// Pre-run, iteration 0.
							new PreRunOptions()
							{
//#if TRACE_FC
//								WriteModAndSumData = true,
//#else
//								WriteModAndSumData = false
//#endif
								WriteModAndSumData = true,
							},

							// Real run, iteration 1.
							new PreRunOptions()
							{
								WriteModAndSumData = true
							}
					});
		}

	}
}
