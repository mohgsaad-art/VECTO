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

using System.Collections.Generic;
using System.IO;
using System.Runtime;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public struct Segment
	{
		public bool Found;
		
		public VehicleClass VehicleClass { get; internal set; }

		public VehicleCategory VehicleCategory { get; set; }

		public AxleConfiguration AxleConfiguration { get; set; }

		public Kilogram GrossVehicleWeightMin { get; set; }

		public Kilogram GrossVehicleWeightMax { get; set; }

		//public Kilogram GrossVehicleMassRating { get; set; }

		public Stream AccelerationFile { get; internal set; }

		public Mission[] Missions { get; internal set; }

		//public Meter VehicleHeight { get; internal set; }

		public MeterPerSecond DesignSpeed { get; internal set; }

	}

	public struct TrailerSegment
	{
		[JsonIgnore]
		public Stream AccelerationFile { get; internal set; }

		public bool VehicleGroupActive { get; set; }
		public int VehicleGroup { get; set; }
		public string VehicleGroupAnnex { get; set; }
		public VehicleClass Class { get; set; } 
		public string GenericCode { get; set; }
		public Kilogram MassBody { get; set; }
		public string MaxGcm { get; set; }
		public Mission[] Missions { get; internal set; }
		public MeterPerSecond DesignSpeed { get; set; }
		public ReferenceTrailerSegment ReferenceTrailerSegment { get; set; }
		public SpecificTrailerSegment SpecificTrailerSegment { get; set; }
		public GenericCadSegment GenericCadVehicle { get; set; }
		public SquareMeter A1Cor { get; set; }
		public SquareMeter A2Cor { get; set; }
		public SquareMeter A3Cor { get; set; }
		public SquareMeter A1RefCor { get; set; }
		public SquareMeter A2RefCor { get; set; }
		public SquareMeter A3RefCor { get; set; }
		public string CodeStandard { get; set; }
		public TrailerReferenceRatios ReferenceRatios { get; set; }
		public string TowingVehicle { get; set; }
	}

	public struct ReferenceTrailerSegment
	{
		[JsonIgnore]
		public int VehicleGroup { get; set; }
		public CubicMeter CargoVolume { get; set; }
		public Kilogram CurbMass { get; set; }
		public Meter TotalTrailerHeight { get; set; }
		public Meter ExternalBodyHeight { get; set; }
		public Meter ExternalBodyWidth { get; set; }
		public Meter ExternalBodyLength { get; set; }
		public Meter LengthTrailerFrontToFirstAxle { get; set; }
		public Meter LengthBetweenCentersOfAxles { get; set; }
		public string TyreXml { get; set; }
		public bool TwinTyres { get; set; }
		public bool LengthAxleCorrection { get; set; }
		public bool WidthCorrection { get; set; }
		public bool HeightCorrection { get; set; }
		public bool VolumeCorrection { get; set; }
		public bool AeroCorrection { get; set; }

		public bool CouplingPointCorrection { get; set; }
	}

	public struct SpecificTrailerSegment
	{
		[JsonIgnore]
		public int VehicleGroup { get; set; }
		public bool LengthAxleCorrection { get; set; }
		public bool WidthCorrection  { get; set; }
		public bool HeightCorrection  { get; set; }
		public bool VolumeCorrection { get; set; }
		public bool AeroCorrection { get; set; }
		public bool CouplingPointCorrection { get; set; }
		public double[] Aero0 { get; set; }
		public double[] Aero1 { get; set; }
		public double[] Aero2 { get; set; }
		public double[] Aero3 { get; set; }
		public double[] Aero4 { get; set; }
		public double[] Aero5 { get; set; }
		public double[] Aero6 { get; set; }
		public double[] Aero7 { get; set; }
		public double[] Aero8 { get; set; }
	}
	public struct GenericCadSegment
	{
		[JsonIgnore]
		public int VehicleGroup { get; set; }
		public Meter TotalHeight { get; set; }
		public Meter ExternalBodyHeight { get; set; }
		public SquareMeter AcrBase { get; set; }
		public double CdxA0 { get; set; }
		public double CdxA3 { get; set; }
		public double CdxA6 { get; set; }
		public double CdxA9 { get; set; }
		public SquareMeter A1 { get; set; }
		public SquareMeter A2 { get; set; }
		public SquareMeter A3 { get; set; }
		public Meter Lbf { get; set; }
		public double DCdf0 { get; set; }
		public double DCdf3 { get; set; }
		public double DCdf6 { get; set; }
		public double DCdf9 { get; set; }
		public Meter Lba { get; set; }
		public double DCda0 { get; set; }
		public double DCda3 { get; set; }
		public double DCda6 { get; set; }
		public double DCda9 { get; set; }
		public Meter Lbr { get; set; }
		public double DCdr0 { get; set; }
		public double DCdr3 { get; set; }
		public double DCdr6 { get; set; }
		public double DCdr9 { get; set; }

	}
	public struct StandardTrailerSegment
	{
		[JsonIgnore]
		//public int VehicleGroup { get; set; }
		public double MaxGcm { get; set; }
		public SquareMeter LhA1 { get; set; }
		public SquareMeter LhA2 { get; set; }
		public SquareMeter LhA3 { get; set; }
		public SquareMeter OtherA1 { get; set; }
		public SquareMeter OtherA2 { get; set; }
		public SquareMeter OtherA3 { get; set; }
		public CubicMeter CargoVolumeTrailerLh { get; set; }
		public CubicMeter CargoVolumeTrailerOther { get; set; }
		public CubicMeter CargoVolumeBodyLh { get; set; }
		public CubicMeter CargoVolumeBodyOther { get; set; }
		public SquareMeter CdxaLh { get; set; }
		public SquareMeter CdxaOther { get; set; }
		public string TyreXml { get; set; }
		public bool TwinTyres { get; set; }
		public Kilogram CurbMassTrailerLh { get; set; }
		public Kilogram CurbMassTrailerOther { get; set; }
		public Kilogram CurbMassBodyLh { get; set; }
		public Kilogram CurbMassBodyOther { get; set; }
		public Meter TotalHeight { get; set; }
		public double PayloadLhLow { get; set; }
		public double PayloadLhRef { get; set; }
		public double PayloadOtherLow { get; set; }
		public double PayloadOtherRef { get; set; }
	}

	public struct TrailerReferenceRatios
	{
		public double LongHaulLow { get; set; }
		public double LongHaulRef { get; set; }
		public double RegionalDeliveryLow { get; set; }
		public double RegionalDeliveryRef { get; set; }
		public double UrbanDeliveryLow { get; set; }
		public double UrbanDeliveryRef { get; set; }

	}
}