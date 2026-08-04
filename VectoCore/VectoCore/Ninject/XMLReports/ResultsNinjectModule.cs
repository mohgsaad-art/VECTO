using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Ninject.XMLReports
{
	public class ResultsNinjectModule : NinjectModule
	{

		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		private HydrogenRangeWriterNamingHelper _hydrogenRangeHelper = new HydrogenRangeWriterNamingHelper();

		private IList<IFuelProperties> NoH2Fuel = new List<IFuelProperties>();

		private IList<IFuelProperties> WithH2Fuel = new List<IFuelProperties>()
			{ DeclarationData.FuelData.Lookup(FuelType.H2CI, null) };

		private const bool NonOVC = false;
		private const bool WithOVC = true;

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IResultsWriterFactory>().To<ResultWriterFactory>().InSingletonScope();

			Bind<IInternalResultWriterFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = _namingHelper.CreateName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetCIFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetMRFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetVIFResultsWriter))

					}
				})).Named(ResultWriterNamingHelper.RESULT_WRITER_2nd_AMDM);

			Bind<IInternalResultWriterFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = _namingHelper.CreateName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetCIFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetMRFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetVIFResultsWriter))

					}
				})).Named(ResultWriterNamingHelper.RESULT_WRITER_3rd_AMDM);

			Bind<IInternalHydrogenRangeWriterFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = _hydrogenRangeHelper.CreateName,
					skipArguments = 3,
					takeArguments = 3,
					methods = new[] {
						typeof(IInternalHydrogenRangeWriterFactory).GetMethod(nameof(IInternalHydrogenRangeWriterFactory
							.GetHydrogenRangeWriter))
					}
				}));

            // OVC mode is only relevant for HydrogenRangeWriter for vehicles with hydrogen fuel and OVC
            
            // NonOVC, NoH2Fuel
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ConventionalVehicle, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.SerialHybridVehicle, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_S, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ParallelHybridVehicle, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IHPC, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.BatteryElectricVehicle, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_E, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV_IEPC, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_PEV, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_SHEV, NonOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_FCHV, NonOVC, NoH2Fuel));

            //NonOVC, WithH2Fuel
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterICE>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ConventionalVehicle, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.SerialHybridVehicle, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_S, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ParallelHybridVehicle, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IHPC, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV_IEPC, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_SHEV, NonOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_FCHV, NonOVC, WithH2Fuel));

            //WithOVC, NoH2Fuel
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.SerialHybridVehicle, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_S, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ParallelHybridVehicle, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IHPC, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.BatteryElectricVehicle, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_E, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV_IEPC, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_PEV, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<NullHydrogenRangeWriter>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_SHEV, WithOVC, NoH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_FCHV, WithOVC, NoH2Fuel));

            //WithOVC, WithH2Fuel
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.SerialHybridVehicle, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IEPC_S, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.ParallelHybridVehicle, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.IHPC, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.FCHV_IEPC, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterHEV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_SHEV, WithOVC, WithH2Fuel));
            Bind<IHydrogenRangeWriter>().To<HydrogenRangeWriterFCHV_OVC>()
                .Named(_hydrogenRangeHelper.GetName(VectoSimulationJobType.Multiple_FCHV, WithOVC, WithH2Fuel));

        }



        #endregion
    }

	public class HydrogenRangeWriterNamingHelper
	{
		public string CreateName(object[] arguments)
		{
			if (arguments.Length == 3 && arguments[0] is VectoSimulationJobType jobType &&
				arguments[1] is bool offVehicleCharging &&
				arguments[2] is IList<IFuelProperties> fuels) {
				return GetName(jobType, offVehicleCharging, fuels);
			}
			throw new ArgumentException($"exactly two arguments expected for HydrogenRangeWriterNamingHelper::CreateName: {typeof(VectoSimulationJobType).Name}, {typeof(IList<IFuelProperties>).Name}");
        }

		public string GetName(VectoSimulationJobType jobType, bool offVehicleCharging, IList<IFuelProperties> fuels)
		{
			var hasH2Fuel = fuels.Count(x => x.FuelType.IsHydrogenFuel());
			var h2Suffix = hasH2Fuel > 0 ? "H2" : "NonH2";
			var ovcSuffix = offVehicleCharging ? "OVC" : "NonOVC";
			return $"{jobType}_{h2Suffix}_{ovcSuffix}";
		}
	}
}