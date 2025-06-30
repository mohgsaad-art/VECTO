using System.Xml;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.Utils;

public class XMLTestHelper
{
	public static XmlDocument LoadAndValidate(string filePath)
	{
		var document = new XmlDocument();
		using (var reader = XmlReader.Create(filePath.ToStream())) {
			document.Load(reader);
		}
		var xmlValidator = new XMLValidator(document, null, XMLValidator.CallBackExceptionOnError);
		Assert.IsTrue(xmlValidator.ValidateXML(XmlDocumentType.DeclarationComponentData));
		return document;
	}

	public static (string, XmlNode) GetComponent(XmlDocument document, string component)
	{
		var componentNode = document.FirstChild.NextSibling.SelectSingleNode($"./*[local-name()='{component}']");
		
		Assert.NotNull(componentNode);
		var dataNode = componentNode.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
		Assert.NotNull(dataNode);
		var version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);

		return (version, componentNode);
    }
}