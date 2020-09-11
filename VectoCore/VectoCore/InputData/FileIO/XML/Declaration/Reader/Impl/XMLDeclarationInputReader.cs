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

		public virtual IDeclarationJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputDeclaration, JobCreator)); }
		}

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

		public XMLDeclarationInputReaderV20(IXMLDeclarationInputData inputData, XmlNode baseNode) : base(inputData,
			baseNode)
		{

		}
	}

	public class XMLDeclarationInputReaderV27 : AbstractComponentReader, IXMLDeclarationTrailerInputDataReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected XmlNode JobNode;
		protected IXMLDeclarationTrailerInputData InputData;
		protected IDeclarationTrailerJobInputData _jobData;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected virtual IDeclarationTrailerJobInputData JobCreatorTrailer(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateJobDataTrailer(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateJobTrailerReaderTrailer(version, job, JobNode);
			return job;
		}

		public XMLDeclarationInputReaderV27(IXMLDeclarationTrailerInputData inputData, XmlNode baseNode) : base(
			inputData, baseNode)
		{
			JobNode = baseNode;
			InputData = inputData;
		}
		
		public IDeclarationTrailerJobInputData JobData { get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoInputDeclaration, JobCreatorTrailer)); } }
	}


	// ---------------------------------------------------------------------------------------

	public class XMLPrimaryVehicleBusInputReaderV01 : AbstractComponentReader, IXMLDeclarationPrimaryVehicleBusInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_NAMESPACE;

		public const string XSD_TYPE = "PrimaryVehicleHeavyBusType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		

		protected XmlNode JobNode;
		protected IDeclarationJobInputData _jobData;
		protected IXMLPrimaryVehicleBusInputData _primaryInputData;
		protected IApplicationInformation _applicationInformation;
		protected IResultsInputData _resultsInputData;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		
		public XMLPrimaryVehicleBusInputReaderV01(IXMLPrimaryVehicleBusInputData inputData, XmlNode baseNode) : base(inputData,  baseNode)
		{
			JobNode =  baseNode;
			_primaryInputData = inputData;
		}

		public IDeclarationJobInputData JobData
		{
			get
			{
				return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoPrimaryVehicleReport, JobCreator));
			}
		}


		protected  IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreatePrimaryVehicleJobData(version, BaseNode, _primaryInputData,
				(_primaryInputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreatePrimaryVehicleJobReader(version, job, JobNode);
			return job;
		}
		

		public IResultsInputData ResultsInputData
		{
			get{ return _resultsInputData ?? 
						(_resultsInputData = CreateComponent(XMLNames.Report_Results, ResultsInputDataCreator)); }
		}
		
		protected IResultsInputData ResultsInputDataCreator(string version, XmlNode node, string arg3)
		{
			return Factory.CreateResultsInputDataReader(version, node);
		}
		

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}
		

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}
		

		public IApplicationInformation ApplicationInformation
		{
			get { return _applicationInformation ?? 
						(_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator)); }
		}
	}
}
