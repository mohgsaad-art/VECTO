using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public interface IDeclarationTrailerDataAdapter
	{
		VehicleData CreateTrailerData(IVehicleDeclarationInputData vehicle, ITrailerDeclarationInputData trailer, TrailerSegment trailerSegment, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, TrailerData specificTrailerData);
		AirdragData CreateTrailerAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, TrailerSegment segment, TrailerData specificTrailerData);
	}
}