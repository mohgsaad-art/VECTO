using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.InputData.XML.Factories;

public class XMLBatteryPackFactoryTests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;
	

    [SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		var inputDataFactory = _kernel.Get<IXMLInputDataReader>();
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}

    [TestCase(XMLBatteryPackDeclarationInputDataMeasuredV23.XSD_TYPE,
		XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
		typeof(XMLBatteryPackDeclarationInputDataMeasuredV23))]
	[TestCase(XMLBatteryPackDeclarationInputDataStandardV23.XSD_TYPE,
		XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
		typeof(XMLBatteryPackDeclarationInputDataStandardV23))]
	public void FactoryTest(string type, string ns, Type expType)
	{
		XNamespace nameSpace = ns;
		var provider = _declarationFactory.CreateBatteryPackDeclarationInputData(XMLHelper.CombineNamespace(nameSpace, type), null, null, null);

		Assert.AreEqual(expType, provider.GetType());
		Assert.NotNull(provider);

	}
}