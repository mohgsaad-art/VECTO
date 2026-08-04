using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SingleBus;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus;

namespace TUGraz.VectoCore.Ninject
{
	public class DeclarationDataAdapterNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule
		
		public override void Load()
		{
			#region HeavyLorry
			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Conventional>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S2>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S3>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S4>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_F2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_F2>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_F3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_F3>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_F4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_F4>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_F_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_F_IEPC>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P1>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P3>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P4>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.HEV_P_IHPC>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E2>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E3>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E4>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.Exempted>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.MultipleFCHVLorryDataAdapter>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.MultipleFCHV>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.MultiplePEVLorryDataAdapter>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.MultiplePEV>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.MultipleSHEVLorryDataAdapter>()
				.WhenInjectedExactlyInto<DeclarationModeHeavyLorryRunDataFactory.MultipleSHEV>();

			Bind<ILorryDeclarationDataAdapter>().To<DeclarationDataAdapterHeavyLorry.Conventional>()
				.WhenInjectedExactlyInto<DeclarationVTPModeVectoRunDataFactoryLorries>();
            #endregion HeavyLorry

            #region PrimaryBus
            Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.Conventional>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S2>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S3>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S4>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_F2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_F2>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_F3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_F3>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_F4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_F4>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_F_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_F_IEPC>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P1>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P2>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P2_5>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P3>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P4>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.HEV_P_IHPC>();

            Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E2>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E3>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E4>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.MultiplePEV>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.MultiplePEV>();

            Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.MultipleFCHV>()
                .WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.MultipleFCHV>();

            Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.MultipleSHEV>()
                .WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.MultipleSHEV>();

            Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModePrimaryBusRunDataFactory.Exempted>();

			Bind<IPrimaryBusDeclarationDataAdapter>().To<DeclarationDataAdapterPrimaryBus.Conventional>()
				.WhenInjectedExactlyInto<DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary>();
            #endregion

            #region CompletedBus Generic
            Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Conventional>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S2>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S3>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P1>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P3>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P4>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P_IHPC>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E2>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E3>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E4>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.FCHV_F2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F2>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.FCHV_F3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F3>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.FCHV_F4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F4>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.FCHV_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_IEPC>();

			Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.MultiplePEV>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.MultipleFCHV>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.MultipleSHEV>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>();

            Bind<IGenericCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Exempted>();
			#endregion

			#region CompletedBus Specific
			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
			    .To<DeclarationDataAdapterSpecificCompletedBus.Conventional>()
			    .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Conventional>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S2>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S3>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S4>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_S_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P1>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P1>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P2_5>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P2_5>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P3>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.HEV_P4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P4>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDataAdapterSpecificCompletedBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.HEV_P_IHPC>();

			Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E2>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E3>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E4>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.PEV_E_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.FCHV_F2>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F2>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.FCHV_F3>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F3>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.FCHV_F4>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_F4>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.FCHV_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.FCHV_IEPC>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.MultiplePEV>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultiplePEV>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.MultipleFCHV>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultipleFCHV>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
                .To<DeclarationDataAdapterSpecificCompletedBus.MultipleSHEV>()
                .WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.MultipleSHEV>();

            Bind<ISpecificCompletedBusDeclarationDataAdapter>()
				.To<DeclarationDataAdapterSpecificCompletedBus.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModeCompletedBusRunDataFactory.Exempted>();
			#endregion

			#region SingleBus
			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.Conventional>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.Conventional>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_S2>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_S2>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_S3>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_S3>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_S4>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_S4>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_S_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_S_IEPC>(); 

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P1>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P1>(); 

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P2>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P2_5>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P2_5>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P3>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P3>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P4>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P4>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.HEV_P2>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.HEV_P_IHPC>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.PEV_E2>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.PEV_E2>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.PEV_E3>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.PEV_E3>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.PEV_E4>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.PEV_E4>();

			Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.PEV_E_IEPC>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.PEV_E_IEPC>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.FCHV_F2>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.FCHV_F2>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.FCHV_F3>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.FCHV_F3>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.FCHV_F4>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.FCHV_F4>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.FCHV_IEPC>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.FCHV_IEPC>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.MultiplePEV>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.MultiplePEV>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.MultipleFCHV>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.MultipleFCHV>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.MultipleSHEV>()
                .WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.MultipleSHEV>();

            Bind<ISingleBusDeclarationDataAdapter>().To<DeclarationDataAdapterSingleBus.Exempted>()
				.WhenInjectedExactlyInto<DeclarationModeSingleBusRunDataFactory.Exempted>();
            #endregion
		}

		#endregion
	}
}

