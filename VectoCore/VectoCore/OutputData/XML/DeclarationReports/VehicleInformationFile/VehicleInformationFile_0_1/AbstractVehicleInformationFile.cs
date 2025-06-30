using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractVehicleInformationFile : IXMLVehicleInformationFile
	{
		protected XNamespace _tns;

		protected readonly IVIFReportFactory _vifFactory;
		protected readonly IResultsWriterFactory _resultFactory;

		public static XNamespace VIF => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace _di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace _v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace _v21 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1";
		protected XNamespace _v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
		protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";
		protected XNamespace _v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";

		protected XElement Vehicle { get; set; }
		protected IResultsWriter Results { get; set; }

		protected  List<IResultEntry> _results = new List<IResultEntry>();
		protected XElement InputDataIntegrity;

		protected AbstractVehicleInformationFile(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory)
		{
			_vifFactory = vifFactory;
			_resultFactory = resultFactory;
		}

		protected abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);
		
		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			Results = _resultFactory.GetVIFResultsWriter(modelData.InputData, modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(VIF + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? XMLHelper.CreateDummySig(_di) : new XElement(modelData.InputDataHash));

		}

		protected virtual string GetVehicleXMLType(VectoRunData modelData)
		{
			return modelData.VehicleData.InputData.XMLSource.Name;
		}

        public void WriteResult(IResultEntry result)
		{
			_results.Add(result);
		}

		public virtual void GenerateReport(XElement fullReportHash)
		{
			// todo amogoda: vif - generate report for v2.7 and VIF v0.2.
			var retVal = new XDocument(new XElement(VIF + XMLNames.VectoOutputMultistep,
				new XAttribute(XNamespace.Xmlns + "di", _di),
				new XAttribute(XNamespace.Xmlns + "xsi", _xsi.NamespaceName),
				new XAttribute(XNamespace.Xmlns + "vif", VIF),
				new XAttribute(XNamespace.Xmlns + "v1.0", _v10),
				new XAttribute(XNamespace.Xmlns + "v2.0", _v20),
				new XAttribute(XNamespace.Xmlns + "v2.1", _v21),
				new XAttribute(XNamespace.Xmlns + "v2.3", _v23),
				new XAttribute(XNamespace.Xmlns + "v2.4", _v24),
				//new XAttribute(XNamespace.Xmlns + "v2.7", _v27),
				new XAttribute(_xsi + "schemaLocation", $"{_tns.NamespaceName} " + @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputMultistep.0.1.xsd"),
				new XAttribute("xmlns", _tns),

				GeneratePrimaryVehicle(fullReportHash)
			));

			Report = retVal;
		}

		public XDocument Report { get; protected set; }

        #endregion

		protected virtual XElement GeneratePrimaryVehicle(XElement resultSignature)
		{
			var vehicleId = $"{VectoComponents.VectoPrimaryVehicleInformation.HashIdPrefix()}{XMLHelper.GetGUID()}";

			var primaryVehicle = new XElement(VIF + XMLNames.Bus_PrimaryVehicle,
				new XElement(VIF + XMLNames.Report_DataWrap,
					new XAttribute(XMLNames.Component_ID_Attr, vehicleId),
					new XAttribute(_xsi + XMLNames.XSIType, "PrimaryVehicleDataType"),
					Vehicle,
					InputDataIntegrity,
					new XElement(VIF + "ManufacturerRecordSignature", resultSignature),
					Results.GenerateResults(_results),
					XMLHelper.GetApplicationInfo(VIF)
				)
			);

			var sigXElement = GetSignatureElement(primaryVehicle);
			primaryVehicle.LastNode.Parent.Add(sigXElement);
			return primaryVehicle;
		}

		protected XElement GetSignatureElement(XElement stage)
		{
			var stream = new MemoryStream();
			var writer = new XmlTextWriter(stream, Encoding.UTF8);
			stage.WriteTo(writer);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			return new XElement(VIF + XMLNames.DI_Signature,
				VectoHash.Load(stream).ComputeXmlHash
					(VectoHash.DefaultCanonicalizationMethod, VectoHash.DefaultDigestMethod));
		}

	}
}
