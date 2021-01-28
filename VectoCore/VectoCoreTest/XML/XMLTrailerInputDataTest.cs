using System.Collections.Generic;
using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.XML
{

	public class XMLTrailerInputDataTest
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\test4.xml";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestXMLInputDeclTrailer()
		{
			var reader = XmlReader.Create(SampleTrailer);

			var inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);

			//general info
			Assert.AreEqual("testing123", inputDataProvider.JobInputData.Trailer.Manufacturer);
			Assert.AreEqual("testing123", inputDataProvider.JobInputData.Trailer.ManufacturerAddress);
			Assert.AreEqual("Generic Tyre Model", inputDataProvider.JobInputData.Trailer.Model);
			Assert.AreEqual("123-123-123", inputDataProvider.JobInputData.Trailer.VIN);
			Assert.AreEqual("O4", inputDataProvider.JobInputData.Trailer.LegislativeCategory);
			Assert.AreEqual(NumberOfTrailerAxles.Two, inputDataProvider.JobInputData.Trailer.NumberOfAxles);
			Assert.AreEqual(TypeTrailer.DA, inputDataProvider.JobInputData.Trailer.TrailerType);
			Assert.AreEqual(BodyWorkCode.Drybody, inputDataProvider.JobInputData.Trailer.BodyworkCode);

			//weights
			Assert.AreEqual(Kilogram.Create(7000), inputDataProvider.JobInputData.Trailer.CurbMass);
			Assert.AreEqual(Kilogram.Create(18000), inputDataProvider.JobInputData.Trailer.TPMLMTotalTrailer);
			Assert.AreEqual(Kilogram.Create(17000), inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			//lengths
			Assert.AreEqual(Meter.Create(8.740), inputDataProvider.JobInputData.Trailer.ExternalBodyLength);
			Assert.AreEqual(Meter.Create(2.550), inputDataProvider.JobInputData.Trailer.ExternalBodyWidth);
			Assert.AreEqual(Meter.Create(4.250), inputDataProvider.JobInputData.Trailer.ExternalBodyHeight);
			Assert.AreEqual(Meter.Create(5.250), inputDataProvider.JobInputData.Trailer.TotalTrailerHeight);
			Assert.AreEqual(Meter.Create(5.333), inputDataProvider.JobInputData.Trailer.LengthTrailerFrontToFirstAxleCenter);
			Assert.AreEqual(Meter.Create(7.550), inputDataProvider.JobInputData.Trailer.LengthTrailerFrontToLastAxleCenter);
			Assert.AreEqual(Meter.Create(4.000), inputDataProvider.JobInputData.Trailer.InternalBodyHeight);

			Assert.AreEqual(CubicMeter.Create(86.700), inputDataProvider.JobInputData.Trailer.CargoVolume);

			//category lookup
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false);
			Assert.AreEqual(2111, trailerSegment.VehicleGroup);

			//aerofeature lookup
			var aeroFeatures = new List<AeroFeatureTechnology> { AeroFeatureTechnology.SideSkirtsShort };

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			Assert.AreEqual(1, combination);
		}
	}
}
