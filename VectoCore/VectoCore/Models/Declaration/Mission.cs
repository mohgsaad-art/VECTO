/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum LoadingType
	{
		FullLoading,
		ReferenceLoad,
		LowLoading,
		EmptyLoading,
	}

	public class Mission
	{
		public MissionType MissionType;
		public string CrossWindCorrectionParameters;
		public double[] AxleWeightDistribution;

		public Kilogram BodyCurbWeight;

		[JsonIgnore]
		public Stream CycleFile;

		public IList<MissionTrailer> Trailer;

		public Kilogram MinLoad;
		public Kilogram LowLoad;
		public Kilogram RefLoad;
		public Kilogram MaxLoad;

		public SquareMeter DefaultCDxA;

		public CubicMeter TotalCargoVolume;

		public Dictionary<LoadingType, Kilogram> Loadings
		{
			get {
				return new Dictionary<LoadingType, Kilogram> {
					{ LoadingType.LowLoading, LowLoad },
					{ LoadingType.ReferenceLoad, RefLoad },
				};
			}
		}

		public BusParameters BusParameter { get; internal set; }
	}

	public class BusParameters
	{

		public Meter VehicleWidth { get; internal set; }

		public Meter VehicleLength { get; internal set; }

		public Meter BodyHeight { get; internal set; }

		public double NumberPassengersLowerDeck { get; internal set; }

		public double NumberPassengersUpperDeck { get; internal set; }

		public bool DoubleDecker { get; internal set; }

		public bool? LowEntry { get; internal set; }

		// #### HVAC Model Parameters

		public BusHVACSystemConfiguration? HVACConfiguration { get; internal set; }

		public Watt HVACAuxHeaterPower { get; internal set; }

		public HeatPumpType HVACCompressorType { get; internal set; }

		public bool HVACDoubleGlasing { get; internal set; }

		public bool HVACHeatpump { get; internal set; }
		public bool HVACAdjustableAuxHeater { get; internal set; }

		// used for primary bus only
		public bool HVACSeparateAirDistributionDucts { get; internal set; }

		public PerSquareMeter PassengerDensityLow { get; internal set; }
		public PerSquareMeter PassengerDensityRef { get; internal set; }
		public VehicleClass BusGroup { get; internal set; }

		//Completed Bus
		//public VehicleCode VehicleCode { get; internal set; }

		//public double PassengerDensity{ get; internal set; }
		//public double PassengerDensityUrban { get; internal set; }
		//public double PassengersSuburban { get; internal set; }
		//public double PassengersInterurban { get; internal set; }
		//public double PassengersCoach { get; internal set; }

		//public bool?  BodyHeightLowerOrEqual { get; internal set; }
		//public bool? PassengersSeatsLowerOrEqual { get; internal set; }

		public bool AirDragMeasurementAllowed { get; internal set; }

		public Dictionary<string, double> ElectricalConsumers { get; internal set; }

		public Meter DeltaHeight { get; internal set; }
		public Meter EntranceHeight { get; set; }
		public VehicleCode? VehicleCode { get; set; }
		public FloorType FloorType { get; set; }
		public IList<BusHVACSystemConfiguration?> SeparateAirDistributionDuctsHVACCfg { get; set; }
	}


	public class MissionTrailer
	{
		public TrailerType TrailerType;
		public Kilogram TrailerCurbMass;
		public Kilogram TrailerGrossVehicleMass;
		public List<Wheels.Entry> TrailerWheels;
		public double TrailerAxleWeightShare;
		public SquareMeter DeltaCdA;
		public CubicMeter CargoVolume;
	}

	public enum TrailerType
	{
		//None,
		T1,
		T2,
		ST1,
		Dolly,
		STT1,
		STT2
	}

	public static class TrailterTypeHelper
	{
		public static TrailerType Parse(string trailer)
		{
			if ("d".Equals(trailer, StringComparison.InvariantCultureIgnoreCase)) {
				return TrailerType.Dolly;
			}
			return trailer.ParseEnum<TrailerType>();
		}
	}
}