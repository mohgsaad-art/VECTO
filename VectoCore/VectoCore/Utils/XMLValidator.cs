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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public class XMLValidator
	{
		private readonly Action<XmlSeverityType, ValidationEvent> _validationErrorAction;
		private readonly Action<bool> _resultAction;
		private bool _valid;
		private  XmlDocument _doc;
		private List<string> _validationErrors = new List<string>();

		private XMLValidator(Action<bool> resultaction, Action<XmlSeverityType, ValidationEvent> validationErrorAction)
		{
			_validationErrorAction = validationErrorAction ?? ((x, y) => { });
			_resultAction = resultaction ?? (x => { });
			_valid = false;
		}

		public XMLValidator(
			XmlReader document, Action<bool> resultaction = null,
			Action<XmlSeverityType, ValidationEvent> validationErrorAction = null) : this(resultaction, validationErrorAction)
		{
			_doc = new XmlDocument();
			_doc.Load(document);
		}

		public XMLValidator(
			XmlDocument document, Action<bool> resultaction = null,
			Action<XmlSeverityType, ValidationEvent> validationErrorAction = null) : this(resultaction, validationErrorAction)
		{
			_doc = document;
		}

		public bool ValidateXML(XmlDocumentType docType)
		{
			_valid = true;
			_validationErrors.Clear();
			if (_doc.DocumentElement == null) {
				throw new Exception("empty XML document");
			}

			_doc.Schemas = GetXMLSchema(docType);
			
			_doc.Validate(ValidationCallBack);

			if (_doc.SchemaInfo.Validity != XmlSchemaValidity.Valid || 
				_doc.DocumentElement?.SchemaInfo == null ||
				_doc.DocumentElement.SchemaInfo.SchemaType == null) {
				ValidationCallBack(null, null);
				_valid = false;
			}

			return _valid;
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			_resultAction(false);
			_valid = false;
			_validationErrors.Add(args?.Message ?? "no schema found");
			_validationErrorAction(args?.Severity ?? XmlSeverityType.Error, new ValidationEvent { ValidationEventArgs = args });
		}

		public string ValidationError => _validationErrors.Any() ? _validationErrors.Join(Environment.NewLine) : null;

		public static void CallBackExceptionOnError(XmlSeverityType severity, ValidationEvent evt)
		{
			if (severity == XmlSeverityType.Error) {
				throw new VectoException("Validation error: {0}", evt?.ValidationEventArgs?.Message ?? "XML schema not known");
			}
		}

		public static XmlSchemaSet GetXMLSchema(XmlDocumentType docType)
		{
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };
			foreach (var entry in EnumHelper.GetValues<XmlDocumentType>()) {
				if ((entry & docType) == 0) {
					continue;
				}

				var schemaFile = XMLDefinitions.GetSchemaFilename(entry);
				if (schemaFile == null) {
					continue;
				}

				Stream resource;
				try {
					resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, schemaFile);
				} catch (Exception e) {
					throw new Exception(
						$"Missing resource {schemaFile} for XML document type: {entry} ({docType.ToString()})", e);
				}

				var reader = XmlReader.Create(resource, new XmlReaderSettings(), "schema://");
				xset.Add(XmlSchema.Read(reader, null));
			}

			xset.Compile();
			return xset;
		}
	}

	public class ValidationEvent
	{
		public Exception Exception;
		public ValidationEventArgs ValidationEventArgs;
	}
}
