using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public static class TrailerDataHelper
	{
		public static TrailerSegment GetTrailerSegment(this ITrailerDeclarationInputData trailer)
		{
			return DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType, trailer.BodyworkCode,
				trailer.VolumeOrientation, trailer.TPMLMAxleAssembly);
		}

		public static int GetAeroFeatureCombination(this ITrailerDeclarationInputData trailer)
		{
			var aeroFeatures = CreateAeroFeatureList(trailer);

			return DeclarationData.AeroFeatures.Lookup(aeroFeatures);
		}

		public static List<AeroFeatureTechnology> CreateAeroFeatureList(this ITrailerDeclarationInputData trailer)
		{
			var aeroFeatures = new List<AeroFeatureTechnology>();
			if (trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsShort)) {
				aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsShort);
			}

			if (trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsLong)) {
				aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsLong);
			}

			if (trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailShort)) {
				aeroFeatures.Add(AeroFeatureTechnology.BoatTailShort);
			}

			;
			if (trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailLong)) {
				aeroFeatures.Add(AeroFeatureTechnology.BoatTailLong);
			}

			return aeroFeatures;
		}
	}
}