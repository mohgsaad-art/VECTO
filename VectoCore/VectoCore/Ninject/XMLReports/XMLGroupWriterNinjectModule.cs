using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Ninject.XMLReports
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

			///Vehicle V2_7
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusGeneralParametersWriter>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusParametersWriter>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBusParametersSequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusDimensionsWriter>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusPassengerCountWriter>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, XMLDeclarationNamespaces.V27));

			///BusAuxiliaries_V2_7
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemLightsGroupWriter>().InSingletonScope().
				Named(GetName(GroupNames.BusAuxElectricSystemLightsGroup, XMLDeclarationNamespaces.V27));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemSupplyGroupWriter>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxElectricSystemSupplySequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACConventionalGroupWriter>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACConventionalSequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACxEVGroupWriter>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACxEVSequenceGroup, XMLDeclarationNamespaces.V27));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACHeatPumpWriter>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACHeatPumpSequenceGroup, XMLDeclarationNamespaces.V27));
		}

		#endregion
	}
}