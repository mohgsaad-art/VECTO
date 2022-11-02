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

using System.Xml;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLDeclarationInputReaderV10 : AbstractComponentReader, IXMLDeclarationInputDataReader
	{
		protected XmlNode JobNode;
		protected IXMLDeclarationInputData InputData;
		protected IDeclarationJobInputData _jobData;

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLDeclarationInputReaderV10(IXMLDeclarationInputData inputData, XmlNode baseNode) : base(
			inputData, baseNode)
		{
			JobNode = baseNode;
			InputData = inputData;
		}

		#region Implementation of IXMLDeclarationInputReader

		public virtual IDeclarationJobInputData JobData => _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputDeclaration, JobCreator));

		#endregion

		protected virtual IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateJobData(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateJobReader(version, job, JobNode);
			return job;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInputReaderV20 : XMLDeclarationInputReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationInputReaderV20(IXMLDeclarationInputData inputData, XmlNode baseNode) 
			: base(inputData, baseNode) { }
	}
}
