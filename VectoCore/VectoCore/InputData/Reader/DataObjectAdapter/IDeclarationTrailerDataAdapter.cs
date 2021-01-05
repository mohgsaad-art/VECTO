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
		VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);

		VehicleData CreateTrailerData(IVehicleDeclarationInputData data, ITrailerDeclarationInputData trailer,
			TrailerSegment trailerSegment,
			Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
			TrailerData specificTrailerData);
		AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData, Mission mission);
		AirdragData CreateTrailerAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, TrailerData specificTrailerData, TrailerSegment trailerSegment);
	}
}