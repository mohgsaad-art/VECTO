using System.Text.RegularExpressions;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter
{
	public class GroupWriterNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		private string GetName(string groupName, XNamespace xNamespace)
		{
			return UseFirstTwoArgumentsAsInstanceProvider.GetName(groupName, xNamespace.ToString());
		}
		public override void Load()
		{
			Bind<IGroupWriterFactory>().ToFactory(() => new UseFirstTwoArgumentsAsInstanceProvider(1, false)).InSingletonScope();

			///Vehicle V2_10_2
			var v2102 = XMLDeclarationNamespaces.v2_10_2;
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusGeneralParametersWriterV2_10_2>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, v2102));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusParametersWriterV2_10_2>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBusParametersSequenceGroup, v2102));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusDimensionsWriter_V2_10_2>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, v2102));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusPassengerCountWriter_V2_10_2>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, v2102));

			///BusAuxiliaries_V2_10_2
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemLightsGroupWriter_v2_10_2>().InSingletonScope().
				Named(GetName(GroupNames.BusAuxElectricSystemLightsGroup, v2102));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemSupplyGroupWriter_v2_10_2>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxElectricSystemSupplySequenceGroup, v2102));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACConventionalGroupWriter_v2_10_2>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACConventionalSequenceGroup, v2102));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACHeatPumpWriter_v2_10_2>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACHeatPumpSequenceGroup, v2102));


		}

		#endregion
	}
}