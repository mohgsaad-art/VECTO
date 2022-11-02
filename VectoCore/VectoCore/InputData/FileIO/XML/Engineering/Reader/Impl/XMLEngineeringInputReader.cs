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
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal class XMLEngineeringInputReaderV07 : AbstractExternalResourceReader, IXMLEngineeringInputReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VectoJobEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected XmlNode JobNode;
		protected IXMLEngineeringInputData InputData;
		private IEngineeringJobInputData _jobData;
		private IDriverEngineeringInputData _driverModel;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLEngineeringInputReaderV07(IXMLEngineeringInputData inputData, XmlNode documentElement) :
			base(inputData, documentElement)
		{
			JobNode = documentElement;
			InputData = inputData;
		}

		public IEngineeringJobInputData JobData => _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputEngineering, JobCreator, false, requireDataNode: false));

		public IDriverEngineeringInputData DriverModel => _driverModel ?? (_driverModel = CreateComponent(XMLNames.Component_DriverModel, DriverModelCreator, requireDataNode:false));

		public IEngineeringJobInputData JobCreator(string version, XmlNode baseNode, string filename)
		{
			var job = Factory.CreateJobData(version, JobNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateJobReader(version, job, JobNode);
			return job;
		}

		public IDriverEngineeringInputData DriverModelCreator(string version, XmlNode baseNode, string filename)
		{
			var driverData = Factory.CreateDriverData(version, InputData, baseNode, (InputData as IXMLResource).DataSource.SourceFile);
			driverData.Reader = Factory.CreateDriverReader(version, driverData, baseNode);
			return driverData;
		}
	}

	internal class XMLEngineeringInputReaderV10 : XMLEngineeringInputReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VectoJobEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringInputReaderV10(
			IXMLEngineeringInputData inputData, XmlNode documentElement) :
			base(inputData, documentElement) { }
	}
}
