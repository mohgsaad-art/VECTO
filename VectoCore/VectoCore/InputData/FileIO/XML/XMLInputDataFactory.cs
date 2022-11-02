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


		private IDeclarationInputDataProvider DoCreateDeclaration(XmlReader inputData, string source)
		{
			var retVal = ReadXmlDoc(inputData, source) as IDeclarationInputDataProvider;
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

			var documentType = XMLHelper.GetDocumentType(xmlDoc.DocumentElement.LocalName);
			if (documentType == null) {
				throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
			}



			new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType.Value);

			switch (documentType.Value)
			{
				case XmlDocumentType.DeclarationJobData: return ReadDeclarationJob(xmlDoc, source);
				case XmlDocumentType.EngineeringJobData: return ReadEngineeringJob(xmlDoc, source);
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

		private IMultistageBusInputDataProvider ReadMultistageDeclarationJob(XmlDocument xmlDoc, string source)
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
