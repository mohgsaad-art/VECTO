using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus
{
	public interface IMrfBusAuxGroup
	{
		IList<XElement> GetElements(IBusAuxiliariesDeclarationData busAuxiliaries);

	}
    internal class CompletedBus_HVACSystem_Group : AbstractReportOutputGroup, IMrfBusAuxGroup
    {
		public CompletedBus_HVACSystem_Group(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		[ExcludeFromCodeCoverage] // not used for buses
		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfBusAuxGroup

		public IList<XElement> GetElements(IBusAuxiliariesDeclarationData busAuxiliaries)
		{
			return new List<XElement>() {
				new XElement(_mrf + XMLNames.Bus_SystemConfiguration,
					busAuxiliaries.HVACAux.SystemConfiguration.ToXmlFormat()),
				new XElement(_mrf + XMLNames.Bus_HeatPumpTypeDriver,
					new XElement(_mrf + XMLNames.BusHVACHeatPumpCooling,
						busAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment.GetLabel()),
					new XElement(_mrf + XMLNames.BusHVACHeatPumpHeating,
						busAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment.GetLabel())),
				new XElement(_mrf + XMLNames.Bus_HeatPumpTypePassenger,
					new XElement(_mrf + XMLNames.BusHVACHeatPumpCooling,
						busAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment.GetLabel()),
					new XElement(_mrf + XMLNames.BusHVACHeatPumpHeating,
						busAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment.GetLabel())),

				new XElement(_mrf + XMLNames.Bus_DoubleGlazing, busAuxiliaries.HVACAux.DoubleGlazing),
				new XElement(_mrf + XMLNames.Bus_AdjustableAuxiliaryHeater, busAuxiliaries.HVACAux.AdjustableAuxiliaryHeater),
				new XElement(_mrf + XMLNames.Bus_SeparateAirDistributionDucts, busAuxiliaries.HVACAux.SeparateAirDistributionDucts),
				new XElement(_mrf + XMLNames.Bus_AuxiliaryHeaterPower, busAuxiliaries.HVACAux.AuxHeaterPower.ConvertToKiloWatt().ToXMLFormat(0))
			};
		}

		#endregion
	}

	internal class CompletedBus_xEVHVACSystem_Group : AbstractReportOutputGroup, IMrfBusAuxGroup
	{
		public CompletedBus_xEVHVACSystem_Group(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfBusAuxGroup

		public IList<XElement> GetElements(IBusAuxiliariesDeclarationData busAuxiliaries)
		{
			return new List<XElement>() {
				new XElement(_mrf + XMLNames.Bus_WaterElectricHeater,
					busAuxiliaries.HVACAux.WaterElectricHeater),
				new XElement(_mrf + XMLNames.Bus_AirElectricHeater, busAuxiliaries.HVACAux.AirElectricHeater),
				new XElement(_mrf + XMLNames.Bus_OtherHeatingTechnology, busAuxiliaries.HVACAux.OtherHeatingTechnology)
			};
		}

		#endregion
	}
}
