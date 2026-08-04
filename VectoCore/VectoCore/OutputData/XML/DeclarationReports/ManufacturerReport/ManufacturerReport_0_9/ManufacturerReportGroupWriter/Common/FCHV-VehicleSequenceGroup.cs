using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.Lorry
{
    internal class FCHV_VehicleSequenceGroup : AbstractReportOutputGroup
    {
		public FCHV_VehicleSequenceGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
            var result = new List<XElement>();
            var vehicle = inputData.JobInputData.Vehicle;
            
            result.AddRange(vehicle.VehicleType.IsMultiplePowertrains()
                ? vehicle.Components.AxlePowertrainInputData.Select(x => new XElement(_mrf + "FCHVArchitecture", new XAttribute("axleNumber", x.AxleNumber), x.Architecture.GetLabel()))
                : new List<XElement>() { new XElement(_mrf + "FCHVArchitecture", vehicle.ArchitectureID.GetLabel()) });

            result.AddRange(new List<XElement>() 
            {
                new XElement(_mrf + "OffVehicleChargingCapability", vehicle.OVC),
                new XElement(_mrf + "DynamicChargingTechnology", vehicle.DynamicChargingTechnology.ToXMLFormat())
            });

            result.Add(_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
            result.Add(_mrfFactory.GetBoostingLimitationsType().GetElement(inputData.JobInputData.Vehicle));
            return result;
        }

		
	}
}
