/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Linq;
using System.Xml;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing;

namespace HashingTool.Helper
{
	public static class HashingHelper
	{
		public const string ToolTipInvalidFileType = "Invalid File type!";
		public const string ToolTipXMLValidationFailed = "XML validation failed!";
		public const string ToolTipOk = "Correct file selected";
		public const string ToolTipHashInvalid = "Incorrect digest value!";
		public const string ToolTipHashValid = "File integrity verified";
		public const string ToolTipNone = "";
		public static string ToolTipComponentHashInvalid = "Job-Data validation failed!";

		public static bool? IsManufacturerReport(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoManufacturerReport;
			if (!valid) {
				errorLog.LogError($"Invalid XML file given ({x.DocumentElement.LocalName}). " +
								$"Expected Manufacturer Report XML ({XMLNames.VectoManufacturerReport})!");
			}
			return valid;
		}

		public static bool? IsCustomerReport(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement != null && x.DocumentElement.LocalName == XMLNames.VectoCustomerReport;
			if (!valid) {
				errorLog.LogError($"Invalid XML file given ({x.DocumentElement.LocalName}). " +
								$"Expected Customer Report XML ({XMLNames.VectoCustomerReport})!");
			}
			return valid;
		}

		public static bool? IsJobFile(XmlDocument x, IErrorLogger errorLog)
		{
			if (x == null || x.DocumentElement == null) {
				return null;
			}
			var valid = x.DocumentElement.LocalName == XMLNames.VectoInputDeclaration &&
						x.DocumentElement.FirstChild.LocalName == XMLNames.Component_Vehicle;
			if (!valid) {
				errorLog.LogError($"Invalid XML file given ({x.DocumentElement.LocalName}/{x.DocumentElement.FirstChild.LocalName}). " +
								$"Expected Vehicle XML ({XMLNames.VectoInputDeclaration}/{XMLNames.Component_Vehicle})!");
			}
			return valid;
		}

		public static bool? IsComponentFile(XmlDocument x, IErrorLogger errorLog)
		{
			if (x.DocumentElement == null) {
				return null;
			}

			if (x.DocumentElement.LocalName != XMLNames.VectoInputDeclaration) {
				errorLog.LogError($"Invalid XML file given ({x.DocumentElement.LocalName}). " +
								$"Expected Component XML ({XMLNames.VectoInputDeclaration})!");

				return false;
			}

			var localName = x.DocumentElement.FirstChild.LocalName;
			var components = new[] {
				VectoComponents.Engine, VectoComponents.Airdrag, VectoComponents.Angledrive, VectoComponents.Axlegear,
				VectoComponents.Gearbox, VectoComponents.Retarder, VectoComponents.TorqueConverter, VectoComponents.Tyre,
				VectoComponents.BatterySystem, VectoComponents.CapacitorSystem, VectoComponents.ElectricMachineSystem,
				VectoComponents.IEPC, VectoComponents.ADC
			};
			var valid = components.Where(c => c.XMLElementName() == localName).Any();
			if (!valid) {
				errorLog.LogError($"Invalid XML file given ({localName}). " +
								$"Expected Component XML ({String.Join(", ", components.Select(c => c.XMLElementName()))})!");
			}
			return valid;
		}


		public static void HashJobFile(XmlDocument xml, VectoXMLFile xmlViewModel)
		{
			try {
				var h = VectoHash.Load(xml);
				xmlViewModel.DigestValueComputed = h.ComputeHash();
				xmlViewModel.DigestMethod = h.GetDigestMethod();
				xmlViewModel.SetCanonicalizationMethod(h.GetCanonicalizationMethods());
			} catch (Exception e) {
				xmlViewModel.XMLFile.LogError(e.Message);
				xmlViewModel.DigestValueComputed = "";
			}
		}

		public static void ValidateDocumentHash(XmlDocument xml, VectoXMLFile xmlFileModel)
		{
			var hashedXML = xmlFileModel as HashedXMLFile;
			if (hashedXML == null) {
				return;
			}

			try {
				var h = VectoHash.Load(xml);
				hashedXML.DigestMethod = h.GetDigestMethod();
				hashedXML.SetCanonicalizationMethod(h.GetCanonicalizationMethods());
				try {
					hashedXML.DigestValueRead = h.ReadHash();
					var dateNode = xml.SelectSingleNode("//*[local-name()='Date']");
					hashedXML.Date = dateNode != null
						? XmlConvert.ToDateTime(dateNode.InnerText, XmlDateTimeSerializationMode.Local)
						: (DateTime?)null;
				} catch {
					hashedXML.DigestValueRead = "";
				}
				try {
					hashedXML.DigestValueComputed = h.ComputeHash();
				} catch {
					hashedXML.DigestValueComputed = "";
				}
				var valid = h.ValidateHash();
				hashedXML.FileIntegrityTooltip = valid ? ToolTipHashValid : ToolTipHashInvalid;
				hashedXML.FileIntegrityValid = valid;
			} catch (Exception e) {
				hashedXML.XMLFile.LogError(e.Message);
				hashedXML.FileIntegrityValid = false;
			}
		}
	}
}
