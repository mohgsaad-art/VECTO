using System;
using System.Collections.Generic;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	public class DeclarationSegmentTrailerTest
	{
		[TestCase]
		public void TrailerSegmentLookupTest(IDeclarationTrailerInputDataProvider inputDataProvider)
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false);
			Assert.AreEqual(2111, trailerSegment.VehicleGroup);
			Assert.AreEqual("18000 + TPMLMAxle", trailerSegment.MaxGcm);
			Assert.AreEqual(10.2, trailerSegment.AcrBase);
			Assert.AreEqual(8.5, trailerSegment.AcrMin);
			Assert.AreEqual(5.3, trailerSegment.CdxABase);
			Assert.AreEqual(VehicleClass.Class5, trailerSegment.Class);
			Assert.AreEqual("A2", trailerSegment.GenericCode);
		}

		[TestCase]
		public void TrailerAeroFeaturesTest()
		{
			var aeroFeatures = new List<AeroFeatureTechnology> { AeroFeatureTechnology.SideSkirtsShort };

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			Assert.AreEqual(1, combination);
		}

	}
}
