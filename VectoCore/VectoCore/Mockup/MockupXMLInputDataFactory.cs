using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoMockup
{
    internal class MockupXMLInputDataFactory : XMLInputDataFactory
	{
		private const string prefix = "urn:tugraz:ivt:VectoAPI:";
		private readonly HashSet<string> _supportedNamespaces = new HashSet<string>() {
			prefix + "DeclarationDefinitions:v2.4",
			prefix + "VectoOutputMultistep:v0.1",
			prefix + "DeclarationDefinitions:v2.7"
        };

		#region Overrides of XMLInputDataFactory


		protected override IMultistepBusInputDataProvider ReadMultistageDeclarationJob(XmlDocument xmlDoc, string source)
		{
			var ret =  base.ReadMultistageDeclarationJob(xmlDoc, source);

			return ret;
		}

		protected override IDeclarationInputDataProvider ReadDeclarationJob(XmlDocument xmlDoc, string source, bool allowDeprecated)
		{
			var ret = base.ReadDeclarationJob(xmlDoc, source, allowDeprecated);
			NamespaceSupported(ret.JobInputData.Vehicle.XMLSource.SchemaInfo.SchemaType.QualifiedName.Namespace);
			return ret;
		}

		private void NamespaceSupported(string ns)
		{
			if (!_supportedNamespaces.Contains(ns)) {
				throw new VectoException($"Namespace {ns} not supported in Mockup Vecto!");
			}
		}
		#endregion
	}
}
