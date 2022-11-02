using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML
{
    public static class XMLDeclarationNamespaces
    {
		public static readonly string DeclarationDefinition = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
		public static readonly XNamespace Xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		public static readonly XNamespace Tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput";
		public static readonly XNamespace Tns_v20 = Tns.NamespaceName + ":v2.0";
		public static readonly XNamespace V26 = DeclarationDefinition + ":DEV:v2.6";
		public static readonly XNamespace V21 = DeclarationDefinition + ":v2.1";
		public static readonly XNamespace V23 = DeclarationDefinition + ":DEV:v2.3";
		public static readonly XNamespace V20 = DeclarationDefinition + ":v2.0";
		public static readonly XNamespace V10 = DeclarationDefinition + ":v1.0";
		public static readonly XNamespace V28 = DeclarationDefinition + ":DEV:v2.8";
		public static readonly XNamespace v2_10_1 = DeclarationDefinition + ":DEV:v2.10.1";
		public static readonly XNamespace v2_10_2 = DeclarationDefinition + ":DEV:v2.10.2";
		public static readonly XNamespace Di = "http://www.w3.org/2000/09/xmldsig#";

		public static readonly string DeclarationRootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationJob";
	}
}
