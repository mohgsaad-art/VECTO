using System;
using System.Windows.Input;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICompleteTrailerViewModel : ICompleteTrailer,  IComponentViewModel 
	{
		AllowedEntry<TypeTrailer>[] AllowedTrailerTypes { get; }
		AllowedEntry<BodyWorkCode>[] AllowedBodyCodes { get; }
		AllowedEntry<AeroFeatureTechnology>[] AllowedTechnologies { get; }
		AllowedEntry<NumberOfTrailerAxles>[] AllowedNumberOfAxles { get; }

		AllowedEntry<string>[] AllowedLegislativeCategory { get; }
	}
}
