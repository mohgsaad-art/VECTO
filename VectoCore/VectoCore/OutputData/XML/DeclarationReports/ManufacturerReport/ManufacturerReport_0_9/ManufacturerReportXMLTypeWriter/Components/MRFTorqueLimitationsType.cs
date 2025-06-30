using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    internal class TorqueLimitationsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
    {

		public TorqueLimitationsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			if (inputData.JobInputData.Vehicle.TorqueLimits == null || inputData.JobInputData.Vehicle.TorqueLimits.Count == 0) {

				return null;
			}
			var torqueLimitsElement = new XElement(_mrf + "EngineTorqueLimitations");

			var maxEngineTorque = inputData.JobInputData.Vehicle.Components.EngineInputData.MaxTorqueDeclared;
			foreach (var torqueLimitInputData in inputData.JobInputData.Vehicle.TorqueLimits) {
				var value = (torqueLimitInputData.MaxTorque / maxEngineTorque).Value();
				torqueLimitsElement.Add(new XElement(_mrf + "EngineTorqueLimit",
					new XAttribute("Gear", torqueLimitInputData.Gear), value.ValueAsUnit("%", 0)
					));
			}

			return torqueLimitsElement;
		}

	}
}
