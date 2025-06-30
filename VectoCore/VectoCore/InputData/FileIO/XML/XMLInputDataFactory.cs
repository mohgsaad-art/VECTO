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
using System.IO;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public class XMLInputDataFactory : IXMLInputDataReader
	{
		[Inject]
		public IDeclarationInjectFactory DeclarationFactory { protected get; set; }

		
		public IInputDataProvider Create(string filename)
		{
			using (var reader = XmlReader.Create(filename)) {
				return ReadXmlDoc(reader, filename, false);
			}
		}

		public IInputDataProvider Create(Stream inputData)
		{
			using (var reader = XmlReader.Create(inputData)) {
				return ReadXmlDoc(reader, null, false);
			}
		}

		public IInputDataProvider Create(XmlReader inputData)
		{
			return ReadXmlDoc(inputData, null, false);
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


		public IDeclarationInputDataProvider CreateDeclaration(string filename, bool allowDeprecated = false)
		{
			using (var reader = XmlReader.Create(filename)) {
				return DoCreateDeclaration(reader, filename, allowDeprecated);
			}
		}

		public IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData)
		{
			return DoCreateDeclaration(inputData, null, false);
		}


		private IDeclarationInputDataProvider DoCreateDeclaration(XmlReader inputData, string source, bool allowDeprecated)
		{
			var retVal = ReadXmlDoc(inputData, source, allowDeprecated) as IDeclarationInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in declaration mode!");
			}

			return retVal;
		}

		private IEngineeringInputDataProvider DoCreateEngineering(XmlReader inputData, string source)
		{
			var retVal = ReadXmlDoc(inputData, source, false) as IEngineeringInputDataProvider;
			if (retVal == null) {
				throw new VectoException("Input data is not in engineering mode!");
			}

			return retVal;
		}

		private IInputDataProvider ReadXmlDoc(XmlReader inputData, string source, bool allowDeprecated)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(inputData);
			if (xmlDoc.DocumentElement == null) {
				throw new VectoException("empty xml document!");
			}

			var documentType = XMLHelper.GetDocumentTypeFromRootElement(xmlDoc.DocumentElement.LocalName);
			if (documentType == null) {
				throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			}



			new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType.Value);

			switch (documentType.Value)
			{
				case XmlDocumentType.DeclarationJobData: return ReadDeclarationJob(xmlDoc, source, allowDeprecated);
				//case XmlDocumentType.EngineeringJobData: return ReadEngineeringJob(xmlDoc, source);
				//case XmlDocumentType.PrimaryVehicleBusOutputData: return ReadPrimaryVehicleDeclarationJob(xmlDoc, source);
				case XmlDocumentType.MultistepOutputData: return ReadMultistageDeclarationJob(xmlDoc, source);
				case XmlDocumentType.EngineeringComponentData:
				case XmlDocumentType.DeclarationComponentData:
				case XmlDocumentType.ManufacturerReport:
				case XmlDocumentType.CustomerReport:
					throw new VectoException("XML Document {0} not supported as simulation input!", documentType.Value);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual IMultistepBusInputDataProvider ReadMultistageDeclarationJob(XmlDocument xmlDoc, string source)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreateMultistageInputProvider(versionNumber, xmlDoc, source);
				input.Reader = DeclarationFactory.CreateMultistageInputReader(versionNumber, input, xmlDoc.DocumentElement);
				return input;
			} catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}
		}

		protected virtual IDeclarationInputDataProvider ReadDeclarationJob(XmlDocument xmlDoc, string source, 
			bool allowDeprecated)
		{
			var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
			try {
				var input = DeclarationFactory.CreateInputProvider(versionNumber, xmlDoc, source, allowDeprecated);
				input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement, allowDeprecated);
				return input;
			} catch (Exception e) {
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
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

	public class XMLComponentInputDataFactory : IXMLComponentInputReader
	{
		private readonly IDeclarationInjectFactory _declarationFactory;

		public XMLComponentInputDataFactory(IDeclarationInjectFactory declFactory)
		{
			_declarationFactory = declFactory;
		}
		#region Implementation of IXMLComponentInputReader

		public IAirdragDeclarationInputData CreateAirdrag(string filename)
		{
			return CreateFromFile<IAirdragDeclarationInputData>(filename);
		}

		// when using this method to create component data, make sure to have a binding like this for the component:
		//  Bind<IComponentInputData>().ToConstructor<XMLElectricMotorDeclarationInputDataProviderV23>((syntax) => new XMLElectricMotorDeclarationInputDataProviderV23(syntax.Inject<XmlNode>(), syntax.Inject<string>()))
		//   .Named(XMLElectricMotorDeclarationInputDataProviderV23.QUALIFIED_XSD_TYPE);
		public TOut CreateFromFile<TOut>(string filename) where TOut : class
		{
			using (var reader = XmlReader.Create(filename)) {
				return CreateFromXmlReader<TOut>(reader, filename);
			}
		}

		public TOut CreateFromStream<TOut>(Stream inputData) where TOut : class
		{
			using (var reader = XmlReader.Create(inputData)) {
				return CreateFromXmlReader<TOut>(reader, null);
			}
		}

		private TOut CreateFromXmlReader<TOut>(XmlReader reader, string filename) where TOut : class
		{
			var result = ReadXMLDoc(reader, filename);
			var typedResult = result as TOut;
			if (typedResult is null)
			{
				throw new VectoException("Error creating XMLComponentInput {0}", filename);
			}

			return typedResult;
        }

		public IAirdragDeclarationInputData CreateAirdrag(Stream inputData)
		{
			throw new NotImplementedException();
		}

		public IAirdragDeclarationInputData CreateAirdrag(XmlReader inputData)
		{
			throw new NotImplementedException();
        }

		#endregion

        private IComponentInputData ReadXMLDoc(XmlReader inputData, string fileName)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(inputData);
			if (xmlDoc.DocumentElement == null)
			{
				throw new VectoException("empty xml document!");
			}

			var documentType = XmlDocumentType.DeclarationComponentData; //<- TODO HM 27.03.23: remove hardcoding

			//Cannot handle Declaration Component
			//var documentType = XMLHelper.GetDocumentTypeFromRootElement(xmlDoc.DocumentElement.LocalName);
			//if (documentType == null)
			//{
			//	throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			//}



			bool valid = new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType);
			var xNode = xmlDoc.DocumentElement?.FirstChild;
			//Document -> Component -> ComponentData
			var versionNumber = XMLHelper.GetXsdType(xNode?.FirstChild?.SchemaInfo.SchemaType);
			try
			{
				var input = _declarationFactory.CreateComponentData(versionNumber, xNode, fileName);
				//input.Reader = _declarationFactory.CreateInputReader(versionNumber, input, xmlDoc.FirstChild);
				return input;
			}
			catch (Exception e)
			{
				throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
			}





            return null;
		}

	}
}
