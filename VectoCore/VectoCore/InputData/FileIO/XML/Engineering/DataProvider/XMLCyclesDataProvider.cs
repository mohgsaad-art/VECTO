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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLCyclesDataProviderV07 : AbstractXMLType, IXMLCyclesDataProvider
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "MissionCyclesType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected string BasePath;

		public XMLCyclesDataProviderV07(IEngineeringJobInputData jobData, XmlNode baseNode, string basePath) : base(baseNode)
		{
			BasePath = basePath;
		}


		public IList<ICycleData> Cycles
		{
			get {
				var retVal = new List<ICycleData>();
				var cycleNodes = GetNodes(XMLNames.Missions_Cycle);
				if (cycleNodes == null || cycleNodes.Count == 0) {
					return retVal;
				}

				foreach (XmlNode cycleNode in cycleNodes) {
					var file = cycleNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
					if (string.IsNullOrWhiteSpace(file)) {
						continue;
					}

					var fullPath = Path.Combine(BasePath ?? "", file);
					if (File.Exists(fullPath)) {
						retVal.Add(
							new CycleInputData() {
								Name = Path.GetFileNameWithoutExtension(fullPath),
								CycleData = VectoCSVFile.Read(fullPath)
							});
					} else {
						try {
							var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." + file +
												Constants.FileExtensions.CycleFile;
							retVal.Add(
								new CycleInputData() {
									Name = Path.GetFileNameWithoutExtension(file),
									CycleData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName),
								});
						} catch {
							//Log.Debug("Driving Cycle could not be read: " + cycleFile);
							throw new VectoException("Driving Cycle could not be read: " + file);
						}
					}
				}

				return retVal;
			}
		}
	}


	internal class XMLCyclesDataProviderV10 : XMLCyclesDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "MissionCyclesType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLCyclesDataProviderV10(IEngineeringJobInputData jobData, XmlNode baseNode, string basePath) : base(
			jobData, baseNode, basePath) { }
	}
}
