using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.Util;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICompleteTrailerViewModel : ICompleteTrailer,  IComponentViewModel 
	{
		AllowedEntry<LegislativeClass>[] AllowedLegislativeClasses { get; }
		AllowedEntry<TypeTrailer>[] AllowedTrailerTypes { get; }
		AllowedEntry<BodyWorkCode>[] AllowedBodyCodes { get; }
		AllowedEntry<AeroFeatureTechnologies>[] AllowedTechnologies { get; }
	}
}
