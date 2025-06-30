using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAxleWheelsType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFAxleWheelsType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var axleWheels = inputData.JobInputData.Vehicle.Components.AxleWheels;
			if (axleWheels == null)
				return null;

			return new XElement(_vif + XMLNames.Component_AxleWheels,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
                    new XAttribute(_xsi + XMLNames.XSIType, "AxleWheelsDataVIFType"),
                    new XElement(_vif + XMLNames.AxleWheels_Axles,
						GetAxleData(axleWheels.AxlesDeclaration)
					)
				)
			);
		}
		
		private List<XElement> GetAxleData(IList<IAxleDeclarationInputData> axleInput)
		{
			var axles = new List<XElement>();
			
			var axleNumber = 1;
			foreach (var currentAxle in axleInput) {
				var axle = new XElement(_vif + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, axleNumber++),
                    new XAttribute("xmlns", _v20),
                    new XAttribute(_xsi + XMLNames.XSIType, "AxleDataDeclarationType"),
					new XElement(_v20 + XMLNames.AxleWheels_Axles_Axle_AxleType, currentAxle.AxleType.ToXMLFormat()),
					new XElement(_v20 + XMLNames.AxleWheels_Axles_Axle_TwinTyres, currentAxle.TwinTyres),
					new XElement(_v20 + XMLNames.AxleWheels_Axles_Axle_Steered, currentAxle.Steered),
                    GetTyre(currentAxle.Tyre));
				axles.Add(axle);
			}

			return axles;
		}

		private XElement GetTyre(ITyreDeclarationInputData tyre)
		{
			var xmlTyre = tyre as IXMLTyreDeclarationInputData;
			if (xmlTyre == null) {
				throw new VectoException("Tyre input data must be in XML format");
			}

			var retVal = XElement.Load(xmlTyre.GetXmlNode.CreateNavigator().ReadSubtree());
			var ptr = retVal.DescendantNodes().OfType<XElement>().First().FirstAttribute;
			while (ptr != null) {
				if (!ptr.IsNamespaceDeclaration && ptr.Name.LocalName == "type" && ptr.Name.Namespace == _xsi && ptr.Value.Contains(':')) {
					var parts = ptr.Value.Split(':');
					ptr.Value = parts.Last();
					var defaultNsAttr = ptr.Parent.Attributes()
						.FirstOrDefault(x => x.IsNamespaceDeclaration && x.Name.LocalName == "xmlns");
					if (defaultNsAttr != null) {
						defaultNsAttr.Value = xmlTyre.GetXmlNode.GetNamespaceOfPrefix(parts.First());
					} else {
						var ns = ptr.Parent.GetNamespaceOfPrefix(parts.First()) 
							?? xmlTyre.GetXmlNode.GetNamespaceOfPrefix(parts.First());
						
						ptr.Parent.Add(new XAttribute("xmlns", ns));
					}
				}
				ptr = ptr.NextAttribute;
			}

			return retVal;
		}
		
		#endregion
	}
}
