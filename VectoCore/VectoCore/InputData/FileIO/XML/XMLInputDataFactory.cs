using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public class XMLInputDataFactory : IXMLInputDataReader
	{
		[Inject]
		public IDeclarationInjectFactory DeclarationFactory { protected get; set; }

		[Inject]
		public IEngineeringInjectFactory EngineeringFactory { protected get; set; }

		public IInputDataProvider Create(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return ReadXmlDoc(reader, filename);
			}
		}

		public IInputDataProvider Create(Stream inputData)
		{
			using (var reader = XmlReader.Create(inputData)) {
				return ReadXmlDoc(reader, null);
			}
		}

		public IInputDataProvider Create(XmlReader inputData)
		{
			
			return ReadXmlDoc(inputData, null);
		}

		public IEngineeringInputDataProvider CreateEngineering(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return DoCreateEngineering(reader, filename);
			}
		}


		public IEngineeringInputDataProvider CreateEngineering(Stream inputData)
		{
			using (var reader = XmlReader.Create(inputData)) {
				return DoCreateEngineering(reader, null);
			}
		}

		public IEngineeringInputDataProvider CreateEngineering(XmlReader inputData)
		{
			return DoCreateEngineering(inputData, null);
		}


		public IDeclarationInputDataProvider CreateDeclaration(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return DoCreateDeclaration(reader, filename);
			}
		}

		public IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData)
		{
			return DoCreateDeclaration(inputData, null);
		}

		public IComponentInputData CreateComponent(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return ReadXmlComponentDoc(reader, filename);
			}
		}

		public IComponentInputData CreateComponent(Stream inputData)
		{
			using (var reader = XmlReader.Create(inputData)) {
				return ReadXmlComponentDoc(reader, null);
			}
		}

		public IComponentInputData CreateComponent(XmlReader inputData)
		{
			return ReadXmlComponentDoc(inputData, null);
		}

		public IDeclarationTrailerInputDataProvider CreateDeclarationTrailer(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return DoCreateDeclarationTrailer(reader, filename);
			}
		}

		public IDeclarationTrailerInputDataProvider CreateDeclarationTrailer(XmlReader inputData)
		{
			return DoCreateDeclarationTrailer(inputData, null);
		}


		private IDeclarationInputDataProvider DoCreateDeclaration(XmlReader inputData, string source)
		{
			var retVal = ReadXmlDoc(inputData, source) as IDeclarationInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in declaration mode!");
			}

			return retVal;
		}

		private IDeclarationTrailerInputDataProvider DoCreateDeclarationTrailer(XmlReader inputData, string source)
		{
			var retVal = ReadXmlDoc(inputData, source) as IDeclarationTrailerInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in declaration mode!");
			}

			return retVal;
		}


		private IEngineeringInputDataProvider DoCreateEngineering(XmlReader inputData, string source)
		{
			var retVal = ReadXmlDoc(inputData, source) as IEngineeringInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in engineering mode!");
			}

			return retVal;
		}

		private IInputDataProvider ReadXmlDoc(XmlReader inputData, string source)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(inputData);
			if (xmlDoc.DocumentElement == null) {
				throw new VectoException("empty xml document!");
			}

			new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(
				XmlDocumentType.DeclarationJobData | XmlDocumentType.EngineeringJobData);

			var documentType = XMLHelper.GetDocumentType(xmlDoc);
			if (documentType == null) {
				throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			}

			switch (documentType.Value)
			{
				case XmlDocumentType.DeclarationJobData: return ReadDeclarationJob(xmlDoc, source); 
				case XmlDocumentType.DeclarationTrailerJobData: return ReadDeclarationTrailerJob(xmlDoc, source);
				case XmlDocumentType.EngineeringJobData: return ReadEngineeringJob(xmlDoc, source);
				case XmlDocumentType.PrimaryVehicleBusOutputData: return ReadPrimaryVehicleDeclarationJob(xmlDoc, source);
				case XmlDocumentType.EngineeringComponentData:
				case XmlDocumentType.DeclarationComponentData:
				case XmlDocumentType.ManufacturerReport:
				case XmlDocumentType.CustomerReport:
					throw new VectoException("XML Document {0} not supported as simulation input!", documentType.Value);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private IEngineeringInputDataProvider ReadEngineeringJob(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);

			var input = EngineeringFactory.CreateInputProvider(versionNumber, xmlDoc, source);
			input.Reader = EngineeringFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement);
			return input;
		}

		private IDeclarationInputDataProvider ReadDeclarationJob(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreateInputProvider(versionNumber, xmlDoc, source);
				input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement);
				return input;
			} catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}
		}


		private IComponentInputData ReadXmlComponentDoc(XmlReader inputData, string source)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(inputData);
			if (xmlDoc.DocumentElement == null) {
				throw new VectoException("empty xml document!");
			}

			new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(
				XmlDocumentType.DeclarationComponentData | XmlDocumentType.EngineeringComponentData);

			var documentType = XMLHelper.GetDocumentType(xmlDoc);
			if (documentType == null) {
				throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			}

			switch (documentType.Value) {
				case XmlDocumentType.DeclarationComponentData:
					return ReadDeclarationComponent(xmlDoc, source);
				case XmlDocumentType.DeclarationJobData:
				case XmlDocumentType.DeclarationTrailerJobData:
				case XmlDocumentType.EngineeringJobData:
				case XmlDocumentType.PrimaryVehicleBusOutputData:
				case XmlDocumentType.EngineeringComponentData:
				case XmlDocumentType.ManufacturerReport:
				case XmlDocumentType.CustomerReport:
					throw new VectoException("XML Document {0} not supported as component!", documentType.Value);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public IComponentInputData ReadDeclarationComponent(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			if (versionNumber.Equals(":")) {
				versionNumber = xmlDoc.DocumentElement?.NamespaceURI;
			}
			try {
				var input = DeclarationFactory.CreateComponentInputProvider(versionNumber, xmlDoc, source);
				//input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement);
				return input.Component;
			} catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}
		}


		private IDeclarationTrailerInputDataProvider ReadDeclarationTrailerJob(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreateInputProviderTrailer(versionNumber, xmlDoc, source);
				input.Reader = DeclarationFactory.CreateInputReaderTrailer(versionNumber, input, xmlDoc.DocumentElement);
				return input;
			} catch (Exception e) {
				throw new VectoException($"Failed to read Declaration job version {versionNumber} {e.Message}", e, versionNumber );
			}
		}

		private IPrimaryVehicleInformationInputDataProvider ReadPrimaryVehicleDeclarationJob(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreatePrimaryVehicleBusInputProvider(versionNumber, xmlDoc, source);
				input.Reader = DeclarationFactory.CreatePrimaryVehicleBusInputReader(versionNumber, input, xmlDoc.DocumentElement);
				return input;
			}
			catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}
		}


	}
}
