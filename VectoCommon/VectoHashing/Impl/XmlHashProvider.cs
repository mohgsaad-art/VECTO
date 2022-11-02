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
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace TUGraz.VectoHashing.Impl
{
	public class XMLHashProvider
	{
		public const string DigestMethodSha256 = "http://www.w3.org/2001/04/xmlenc#sha256";

		public const string VectoDsigTransform = "urn:vecto:xml:2017:canonicalization";
		public const string DsigExcC14NTransform = "http://www.w3.org/2001/10/xml-exc-c14n#";


		public static ICollection<string> SupportedDigestMethods
		{
			get {
				return new[] {
					DigestMethodSha256
				};
			}
		}

		public static string DefaultDigestMethod => DigestMethodSha256;

		public static ICollection<string> SupportedCanonicalizationMethods
		{
			get {
				return new[] {
					VectoDsigTransform,
					DsigExcC14NTransform,
				};
			}
		}

		public static IEnumerable<string> DefaultCanonicalizationMethod
		{
			get {
				return new[] {
					VectoDsigTransform,
					DsigExcC14NTransform
				};
			}
		}

		public static XmlDocument ComputeHash(XmlDocument doc, string elementId, IEnumerable<string> canonicalization,
			string digestMethod)
		{
			if (doc == null) {
				throw new Exception("Invalid Document");
			}
			var c14N = (canonicalization ?? DefaultCanonicalizationMethod).ToArray();
			digestMethod = digestMethod ?? DefaultDigestMethod;
			if (!SupportedDigestMethods.Contains(digestMethod)) {
				throw new Exception($"DigestMethod '{digestMethod}' not supported.");
			}
			var unsupported = c14N.Where(c => !SupportedCanonicalizationMethods.Contains(c)).ToArray();
			if (unsupported.Any()) {
				throw new Exception($"CanonicalizationMethod(s) {string.Join(", ", unsupported)} not supported!");
			}

			// load any HMAC algorithm so that the key is also available. the HVAC algorithm used is the one set in the Reference object below
			var hmac = HMAC.Create("HMACSHA256"); 
			var signedXml = new SignedXml(doc);
			var reference = new Reference("#" + elementId) {
				DigestMethod = digestMethod
			};
			foreach (var c in c14N) {
				reference.AddTransform(GetTransform(c));
			}

			signedXml.AddReference(reference);
			signedXml.ComputeSignature(hmac);
			var xmlDigitalSignature = reference.GetXml();

			var sigdoc = new XmlDocument();
			sigdoc.CreateElement("Signature");
			sigdoc.AppendChild(sigdoc.ImportNode(xmlDigitalSignature, true));

			return sigdoc;
		}

		private static Transform GetTransform(string transformUrn)
		{
			switch (transformUrn) {
				case VectoDsigTransform:
					return new XmlDsigVectoTransform();
				case DsigExcC14NTransform:
					return new XmlDsigExcC14NTransform();
			}
			throw new Exception($"Unsupported CanonicalizationMethod {transformUrn}");
		}
	}
}
