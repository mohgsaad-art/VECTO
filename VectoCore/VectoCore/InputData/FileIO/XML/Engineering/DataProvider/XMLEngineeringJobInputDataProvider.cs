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
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringJobInputDataProviderV07 : AbstractXMLResource, IXMLEngineeringJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VectoJobEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected string FileName;
		protected IXMLEngineeringInputData InputProvider;

		private IEngineEngineeringInputData _engineOnly;
		private IVehicleEngineeringInputData _vehicle;

		public IXMLJobDataReader Reader { protected get; set; }
		private IXMLCyclesDataProvider _cycles;


		public XMLEngineeringJobInputDataProviderV07(XmlNode node, IXMLEngineeringInputData inputProvider, string fileName) :
			base(node, fileName)
		{
			InputProvider = inputProvider;
			FileName = fileName;

			EngineOnlyMode = GetBool(XMLNames.VectoJob_EngineOnlyMode);
			SourceType = (inputProvider as IXMLResource).DataSource.SourceFile == fileName ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}


		public virtual IList<ICycleData> Cycles
		{
			get { return (_cycles ?? (_cycles = Reader.CreateCycles)).Cycles; }
		}

		public virtual IEngineEngineeringInputData EngineOnly
		{
			get { return _engineOnly ?? (_engineOnly = Reader.CreateEngineOnly); }
		}

		public virtual TableData PTOCycleWhileDrive { get { return null; } }

		public string ShiftStrategy => null;

		public virtual VectoSimulationJobType JobType { get; }

		public virtual bool EngineOnlyMode { get; }


		public virtual string JobName
		{
			get {
				return EngineOnlyMode
					? EngineOnly.Model
					: (GetAttribute(BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Component_Vehicle)), "id") ??
						Vehicle.Model + " " + Vehicle.Manufacturer);
			}
		}

		public virtual bool SavedInDeclarationMode
		{
			get { return false; }
		}

		public virtual IVehicleEngineeringInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return Vehicle; }
		}

		public string SourePath
		{
			get { return FileName == null ? null : Path.GetDirectoryName(Path.GetFullPath(FileName)); }
		}

		public IXMLEngineeringInputData InputData
		{
			get { return InputProvider; }
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }
		protected override DataSourceType SourceType { get; }

		#endregion
	}


	internal class XMLEngineeringJobInputDataProviderV10 : XMLEngineeringJobInputDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VectoJobEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringJobInputDataProviderV10(XmlNode node, IXMLEngineeringInputData inputProvider, string fileName) :
			base(node, inputProvider, fileName) { }

		#region Overrides of XMLEngineeringJobInputDataProviderV07

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }

		#endregion
	}
}
