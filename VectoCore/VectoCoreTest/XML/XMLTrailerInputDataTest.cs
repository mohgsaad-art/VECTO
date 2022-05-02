using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;

namespace TUGraz.VectoCore.Tests.XML
{

	public class XMLTrailerInputDataTest
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\TEST_CASE.xml";

		private IDeclarationTrailerInputDataProvider inputDataProvider;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			var reader = XmlReader.Create(SampleTrailer);
			inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		[TestCase]
		public void TrailerFullTest()
		{
			//InputData
			TestXMLInputDeclTrailer();

			//Trailer Segment lookup
			var declarationSegmentTrailerTest = new DeclarationSegmentTrailerTest();
			declarationSegmentTrailerTest.TrailerLookupTest(inputDataProvider);

			//Aerofeature lookup
			declarationSegmentTrailerTest.TrailerAeroFeaturesTest();

			//Corrections
			TrailerCorrectionsTest();

			//Missions Test
			TrailerMissionsTest();

			//Report XML schema
			var xmlReportTrailerTest = new XMLReportTrailerTest();
			xmlReportTrailerTest.RunBeforeAnyTests();
			xmlReportTrailerTest.RunTrailerDeclarationJob(SampleTrailer);
		}

		[TestCase]
		public void TestXMLInputDeclTrailer()
		{
			//general info
			Assert.AreEqual("fds", inputDataProvider.JobInputData.Trailer.Manufacturer);
			Assert.AreEqual("fds", inputDataProvider.JobInputData.Trailer.ManufacturerAddress);
			//Assert.AreEqual("Model", inputDataProvider.JobInputData.Trailer.TrailerModel);
			Assert.AreEqual("fds", inputDataProvider.JobInputData.Trailer.VIN);
			Assert.AreEqual("O4", inputDataProvider.JobInputData.Trailer.LegislativeCategory);
			Assert.AreEqual(NumberOfTrailerAxles.Two, inputDataProvider.JobInputData.Trailer.NumberOfAxles);
			Assert.AreEqual(TypeTrailer.DC, inputDataProvider.JobInputData.Trailer.TrailerType);
			Assert.AreEqual(BodyWorkCode.CurtainSided, inputDataProvider.JobInputData.Trailer.BodyworkCode);
			Assert.AreEqual(TrailerCouplingPoint.Low, inputDataProvider.JobInputData.Trailer.TrailerCouplingPoint);

			//weights
			Assert.AreEqual(Kilogram.Create(5600), inputDataProvider.JobInputData.Trailer.MassInRunningOrder);
			Assert.AreEqual(Kilogram.Create(18000), inputDataProvider.JobInputData.Trailer.TPMLMTotalTrailer);
			Assert.AreEqual(Kilogram.Create(15000), inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			//lengths
			Assert.AreEqual(Meter.Create(7.820), inputDataProvider.JobInputData.Trailer.ExternalBodyLength);
			Assert.AreEqual(Meter.Create(2.550), inputDataProvider.JobInputData.Trailer.ExternalBodyWidth);
			Assert.AreEqual(Meter.Create(2.730), inputDataProvider.JobInputData.Trailer.ExternalBodyHeight);
			Assert.AreEqual(Meter.Create(4.000), inputDataProvider.JobInputData.Trailer.TotalTrailerHeight);
			Assert.AreEqual(Meter.Create(3.695), inputDataProvider.JobInputData.Trailer.LengthFromFrontToFirstAxle);
			Assert.AreEqual(Meter.Create(1.310), inputDataProvider.JobInputData.Trailer.LengthBetweenCentersOfAxles);

			Assert.AreEqual(CubicMeter.Create(49.500), inputDataProvider.JobInputData.Trailer.CargoVolume);

			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle0);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle3);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle6);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle9);
		}

		[TestCase]
		public void TrailerCorrectionsTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false, inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			var trailerDataAdapter = new DeclarationTrailerDataAdapter();

			var genericInputDataProvider = DeclarationData.TrailerSegments.GetGenericVehicleData(trailerSegment.GenericCode);

			var specificTrailerData = trailerDataAdapter.GetTrailerData(trailerSegment, inputDataProvider.JobInputData.Trailer, genericInputDataProvider.JobInputData.Vehicle);

			Assert.AreEqual(SquareMeter.Create(-0.014151343726624403), specificTrailerData.TotalCorrection.A1);
			Assert.AreEqual(SquareMeter.Create(0.0038345680933802003), specificTrailerData.TotalCorrection.A2);
			Assert.AreEqual(SquareMeter.Create(-0.00017676620088870912), specificTrailerData.TotalCorrection.A3);
			Assert.AreEqual(SquareMeter.Create(5.0386476237653435), specificTrailerData.TotalCorrection.CdxA0);
			Assert.AreEqual(SquareMeter.Create(5.0259320180014724), specificTrailerData.TotalCorrection.CdxA3);
			Assert.AreEqual(SquareMeter.Create(5.0536025133813762), specificTrailerData.TotalCorrection.CdxA6);
			Assert.AreEqual(SquareMeter.Create(5.093022985332448), specificTrailerData.TotalCorrection.CdxA9);
			Assert.AreEqual(SquareMeter.Create(0.00), specificTrailerData.TotalCorrection.CdxA00);
			Assert.AreEqual(SquareMeter.Create(-0.012715605763871096), specificTrailerData.TotalCorrection.CdxA03);
			Assert.AreEqual(SquareMeter.Create(0.014954889616032752), specificTrailerData.TotalCorrection.CdxA06);
			Assert.AreEqual(SquareMeter.Create(0.054375361567104541), specificTrailerData.TotalCorrection.CdxA09);

		}

		[TestCase]
		public void TrailerMissionsTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(
				inputDataProvider.JobInputData.Trailer.NumberOfAxles,
				inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode,
				false, inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			Assert.AreEqual(3, trailerSegment.Missions.Count());
			Assert.AreEqual(1, trailerSegment.Missions.Count(x=> x.MissionType == MissionType.LongHaul));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.RegionalDelivery));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.UrbanDelivery));

			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(19300), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(12900), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(12900), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerRefLoad);

		}
	}
}
