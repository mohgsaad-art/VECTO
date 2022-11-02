using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.VectoCore.Tests.XML
{

	public class XMLPrimaryBusInputDataTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		const string SampleVehicleDecl = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\vecto_vehicle-primary_heavyBus-sample.xml";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestXMLInputDeclPrimaryBus()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);

			Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, inputDataProvider.JobInputData.Vehicle.VehicleCategory);
		}
	}
}
