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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	[CustomValidation(typeof(AirdragData), "ValidateAirDragData")]
	public class AirdragData : SimulationComponentData
	{
		public CrossWindCorrectionMode CrossWindCorrectionMode { get; set; }

		[Required, ValidateObject]
		public ICrossWindCorrection CrossWindCorrectionCurve { get; internal set; }

		public SquareMeter DeclaredAirdragArea { get; internal set; }

		public SquareMeter DeclaredAirdragAreaInput { get; internal set; }

		// ReSharper disable once UnusedMember.Global  -- used via Validation
		public static ValidationResult ValidateAirDragData(AirdragData airDragData, ValidationContext validationContext)
		{
			if (airDragData.CrossWindCorrectionMode != CrossWindCorrectionMode.DeclarationModeCorrection &&
				airDragData.CrossWindCorrectionCurve.AirDragArea == null) {
				return new ValidationResult(
					"AirDrag Area (CdxA) must not be empty when the cross wind correction mode is not \"Speed dependent (Declaration Mode)\"");
			}

			return ValidationResult.Success;
		}
	}

	/// <summary>
	/// Data Class for the Vehicle
	/// </summary>
	[CustomValidation(typeof(VehicleData), "ValidateVehicleData")]
	public class VehicleData : SimulationComponentData, IVehicleData
	{
		public string VIN { get; internal set; }

		public LegislativeClass? LegislativeClass { get; internal set; }

		public VehicleCategory VehicleCategory { get; internal set; }

		public VehicleClass VehicleClass { get; internal set; }

		public AxleConfiguration AxleConfiguration { get; internal set; }

		public string ManufacturerAddress { get; internal set; }


		[Required, ValidateObject] private List<Axle> _axleData;

		private KilogramSquareMeter _wheelsInertia;
		private double? _totalRollResistanceCoefficient;
		private double? _rollResistanceCoefficientWithoutTrailer;
		private double? _averageRollingResistanceTruck;

		public List<Axle> AxleData
		{
			get => _axleData;
			internal set {
				_axleData = value;
				_wheelsInertia = null;
				_totalRollResistanceCoefficient = null;
			}
		}

		/// <summary>
		/// The Curb mass of the vehicle 
		/// (+ Curb mass of Standard-Body if it has one)
		/// (+ Curb mass of Trailer if it has one)
		/// </summary>
		[Required, SIRange(500, 40000, emsMission: false),
		SIRange(0, 60000, emsMission: true)]
		public Kilogram CurbMass { get; internal set; }

		/// <summary>
		/// Curb mass of Standard-Body (if it has one)
		/// + Curb mass of Trailer (if it has one)
		/// </summary>
		public Kilogram BodyAndTrailerMass { get; internal set; }

		[Required, SIRange(0, 40000, emsMission: false),
		SIRange(0, 60000, emsMission: true)]
		public Kilogram Loading { get; internal set; }

		public double? PassengerCount { get; internal set; }


		[SIRange(0, 500)]
		public CubicMeter CargoVolume { get; internal set; }

		/// <summary>
		/// The Gross Vehicle mass of the Vehicle.
		/// </summary>
		[Required,
		SIRange(3500, 40000, ExecutionMode.Declaration, emsMission: false),
		SIRange(0, 60000, ExecutionMode.Declaration, emsMission: true),
		SIRange(0, 1000000, ExecutionMode.Engineering)]
		public Kilogram GrossVehicleMass { get; internal set; }

		/// <summary>
		/// The Gross Vehicle mass of the Trailer (if the vehicle has one).
		/// </summary>
		[Required, SIRange(0, 40000, emsMission: false),
		SIRange(0, 60000, emsMission: true)]
		public Kilogram TrailerGrossVehicleMass { get; internal set; }

		[Required, SIRange(0.1, 2)]
		public Meter DynamicTyreRadius { get; internal set; }

		[Required, SIRange(0.1, 100)]
		public KilogramPerCubicMeter AirDensity { get; internal set; }

		public KilogramSquareMeter WheelsInertia
		{
			get {
				if (_wheelsInertia == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _wheelsInertia;
			}
			internal set => _wheelsInertia = value;
		}

		//[Required, SIRange(0, 1E12)]
		public double TotalRollResistanceCoefficient
		{
			get {
				if (_totalRollResistanceCoefficient == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _totalRollResistanceCoefficient.GetValueOrDefault();
			}
			protected internal set => _totalRollResistanceCoefficient = value;
		}

		public double RollResistanceCoefficientWithoutTrailer
		{
			get {
				if (_rollResistanceCoefficientWithoutTrailer == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _rollResistanceCoefficientWithoutTrailer.GetValueOrDefault();
			}
			protected internal set => _rollResistanceCoefficientWithoutTrailer = value;
		}

		public Kilogram TotalVehicleMass
		{
			get {
				var retVal = 0.0.SI<Kilogram>();
				retVal += CurbMass ?? 0.SI<Kilogram>();
				retVal += BodyAndTrailerMass ?? 0.SI<Kilogram>();
				retVal += Loading ?? 0.SI<Kilogram>();
				return retVal;
			}
		}

		public Kilogram TotalCurbMass => (CurbMass ?? 0.SI<Kilogram>()) + (BodyAndTrailerMass ?? 0.SI<Kilogram>());

		public Kilogram MinimumVehicleMass
		{
			get {
				var retVal = 0.0.SI<Kilogram>();
				retVal += CurbMass ?? 0.SI<Kilogram>();
				retVal += BodyAndTrailerMass ?? 0.SI<Kilogram>();
				return retVal;
			}
		}

		public Kilogram MaximumVehicleMass => GrossVehicleMass;

		public double AverageRollingResistanceTruck
		{
			get {
				if (_averageRollingResistanceTruck == null) {
					ComputeRollResistanceAndReducedMassWheels();
				}
				return _averageRollingResistanceTruck.GetValueOrDefault();
			}
			protected internal set => _averageRollingResistanceTruck = value;
		}

		public bool ZeroEmissionVehicle { get; internal set; }
		public bool HybridElectricHDV { get; internal set; }
		public bool DualFuelVehicle { get; internal set; }
		public Watt MaxNetPower1 { get; internal set; }
		public Watt MaxNetPower2 { get; internal set; }
		public bool? SleeperCab { get; internal set; }
		public ADASData ADAS { get; internal set; }
		public bool VocationalVehicle { get; internal set; }

		public class ADASData
		{
			public bool EngineStopStart { get; internal set; }
			public EcoRollType EcoRoll { get; internal set; }
			public PredictiveCruiseControlType PredictiveCruiseControl { get; internal set; }

			[JsonIgnore]
			public IAdvancedDriverAssistantSystemDeclarationInputData InputData { get; internal set; }
		}

		[JsonIgnore]
		public IVehicleDeclarationInputData InputData { get; internal set; }

		public RegistrationClass? RegisteredClass { get; internal set; }
		public VehicleCode? VehicleCode { get; internal  set; }


		//		#region "Bus Parameters"
//		public double PassengerCount { get; internal set; }

//		public FloorType FloorType { get; internal set; }

//		public bool DoubleDecker { get; internal set; }

//		public Meter Length { get;internal set; }
		 
//		public Meter Width { get; internal set; }

//		public Meter Height { get; internal set; }

//#endregion


		protected void ComputeRollResistanceAndReducedMassWheels()
		{
			if (TotalVehicleMass == 0.SI<Kilogram>()) {
				throw new VectoException("Total vehicle weight must be greater than 0! Set CurbWeight and Loading before!");
			}
			if (DynamicTyreRadius == null) {
				throw new VectoException("Dynamic tyre radius must be set before axles!");
			}

			var g = Physics.GravityAccelleration;

			var rrc = 0.0.SI<Scalar>();
			var rrcVehicle = 0.0.SI<Scalar>();
			var averageRRC = 0.0;
			var vehicleWheels = 0;
			var wheelsInertia = 0.0.SI<KilogramSquareMeter>();
			var vehicleWeightShare = 0.0;
			foreach (var axle in _axleData) {
				if (axle.AxleWeightShare.IsEqual(0, 1e-12)) {
					continue;
				}
				var nrWheels = axle.TwinTyres ? 4 : 2;
				var baseValue = (axle.AxleWeightShare * TotalVehicleMass * g / axle.TyreTestLoad / nrWheels).Value();

				var rrcShare = axle.AxleWeightShare * axle.RollResistanceCoefficient *
								Math.Pow(baseValue, Physics.RollResistanceExponent - 1);
				if (axle.AxleType != AxleType.Trailer) {
					rrcVehicle += rrcShare;
					vehicleWeightShare += axle.AxleWeightShare;
					averageRRC += axle.RollResistanceCoefficient * nrWheels;
					vehicleWheels += nrWheels;
				}
				rrc += rrcShare;
				wheelsInertia += nrWheels * axle.Inertia;
			}
			RollResistanceCoefficientWithoutTrailer = rrcVehicle / vehicleWeightShare;
			TotalRollResistanceCoefficient = rrc;
			AverageRollingResistanceTruck = averageRRC / vehicleWheels;
			WheelsInertia = wheelsInertia;
		}

		// ReSharper disable once UnusedMember.Global  -- used via Validation
		public static ValidationResult ValidateVehicleData(VehicleData vehicleData, ValidationContext validationContext)
		{
			var mode = GetExecutionMode(validationContext);
			var emsCycle = GetEmsMode(validationContext);

			if (vehicleData.AxleData.Count < 1) {
				return new ValidationResult("At least two axles need to be specified");
			}

			var weightShareSum = vehicleData.AxleData.Sum(axle => axle.AxleWeightShare);
			if (!weightShareSum.IsEqual(1.0, 1E-10)) {
				return new ValidationResult(
					$"Sum of axle weight share is not 1! sum: {weightShareSum}, difference: {1 - weightShareSum}");
			}
			for (var i = 0; i < vehicleData.AxleData.Count; i++) {
				if (vehicleData.AxleData[i].TyreTestLoad.IsSmallerOrEqual(0)) {
					return new ValidationResult($"Tyre test load (FzISO) for axle {i} must be greater than 0.");
				}
			}

			if (vehicleData.TotalRollResistanceCoefficient <= 0) {
				return
					new ValidationResult($"Total rolling resistance must be greater than 0! {vehicleData.TotalRollResistanceCoefficient}");
			}

			// total gvw is limited by max gvw (40t)
			var gvwTotal = VectoMath.Min(vehicleData.GrossVehicleMass + vehicleData.TrailerGrossVehicleMass,
				emsCycle
					? Constants.SimulationSettings.MaximumGrossVehicleMassEMS
					: Constants.SimulationSettings.MaximumGrossVehicleMass);
			if (mode != ExecutionMode.Declaration) {
				return ValidationResult.Success;
			}
			// vvvvvvv these checks apply only for declaration mode! vvvvvv

			//if (vehicleData.AxleConfiguration.NumAxles() != vehicleData.AxleData.Count) {
			//	return
			//		new ValidationResult(
			//			string.Format("For a {0} type vehicle exactly {1} number of axles have to pe specified. Found {2}",
			//				vehicleData.AxleConfiguration.GetName(), vehicleData.AxleConfiguration.NumAxles(), vehicleData.AxleData.Count));
			//}

			if (vehicleData.TotalVehicleMass > gvwTotal) {
				return new ValidationResult(
					$"Total Vehicle mass is greater than GrossVehicleMass! Mass: {vehicleData.TotalVehicleMass},  GVM: {gvwTotal}");
			}

			var numDrivenAxles = vehicleData._axleData.Count(x => x.AxleType == AxleType.VehicleDriven);
			if (numDrivenAxles != vehicleData.AxleConfiguration.NumDrivenAxles()) {
				return
					new ValidationResult(string.Format(
						vehicleData.AxleConfiguration.NumAxles() == 1
							? "Exactly {0} axle has to be defined as driven, given {1}!"
							: "Exactly {0} axles have to be defined as driven, given {1}!", vehicleData.AxleConfiguration.NumDrivenAxles(),
						numDrivenAxles));
			}

			return ValidationResult.Success;
		}
	}
}