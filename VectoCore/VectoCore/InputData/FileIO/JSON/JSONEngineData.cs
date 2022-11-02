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
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	/// <summary>
	///     Represents the CombustionEngineData. Fileformat: .veng
	/// </summary>
	/// <code>
	/// {
	///  "Header": {
	///    "CreatedBy": " ()",
	///    "Date": "3/4/2015 12:26:24 PM",
	///    "AppVersion": "2.0.4-beta3",
	///    "FileVersion": 2
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///    "Displacement": 12730.0,
	///    "IdlingSpeed": 560.0,
	///    "Inertia": 3.8,
	///    "FullLoadCurves": [
	///      {
	///        "Path": "24t Coach.vfld",
	///        "Gears": "0 - 99"
	///      }
	///    ],
	///    "FuelMap": "24t Coach.vmap",
	///    "WHTC-Urban": 0.0,
	///    "WHTC-Rural": 0.0,
	///    "WHTC-Motorway": 0.0
	///  }
	/// }
	/// </code>
	public class JSONEngineDataV5 : JSONEngineDataV4, IEngineModeEngineeringInputData
	{
		protected IWHRData _elWHRData;
		protected IWHRData _mechWHRData;

		
		public JSONEngineDataV5(JObject data, string fileName, bool tolerateMissing = false) : base(
			data, fileName, tolerateMissing) { }

		#region Overrides of JSONEngineDataV3

		public override IWHRData WasteHeatRecoveryDataElectrical => _elWHRData ?? (_elWHRData = ReadWHRData(Body["WHRCorrectionFactors"]?["Electrical"]));


		public override IWHRData WasteHeatRecoveryDataMechanical => _mechWHRData ?? (_mechWHRData = ReadWHRData(Body["WHRCorrectionFactors"]?["Mechanical"]));

		public override WHRType WHRType
		{
			get {
				var whr = Body["WHRType"];
				var retVal = WHRType.None;
				if (whr == null) {
					return retVal;
				}

				foreach (var entry in whr) {
					retVal |= entry.ToString().ParseEnum<WHRType>(); 
				}
				return retVal;
			}
		}

		protected override IList<IEngineFuelEngineeringInputData> ReadFuels()
		{
			var retVal = new List<IEngineFuelEngineeringInputData>();
			foreach (var jsonFuel in Body["Fuels"]) {
				TableData fuelMap;
				try {
					fuelMap = ReadTableData(jsonFuel.GetEx<string>(JsonKeys.Engine_FuelConsumptionMap), "FuelConsumptionMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					fuelMap = new TableData(
							Path.Combine(BasePath, jsonFuel[JsonKeys.Engine_FuelConsumptionMap].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}

				retVal.Add(new JSONFuelInputData {
					FuelType = jsonFuel.GetEx<string>("FuelType").ParseEnum<FuelType>(),
					ColdHotBalancingFactor = jsonFuel.GetEx<double>("ColdHotBalancingFactor"),
					CorrectionFactorRegPer = jsonFuel.GetEx<double>("CFRegPer"),
					WHTCUrban = jsonFuel.GetEx<double>(JsonKeys.Engine_WHTC_Urban),
					WHTCRural = jsonFuel.GetEx<double>(JsonKeys.Engine_WHTC_Rural),
					WHTCMotorway = jsonFuel.GetEx<double>(JsonKeys.Engine_WHTC_Motorway),
					WHTCEngineering = jsonFuel.GetEx<double>("WHTC-Engineering"),
					FuelConsumptionMap = fuelMap
				});
			}

			return retVal;
		}

		#endregion

		private IWHRData ReadWHRData(JToken correctionFactors)
		{
			TableData whrMap;
			try {
				whrMap = ReadTableData(Body["Fuels"][0].GetEx<string>(JsonKeys.Engine_FuelConsumptionMap), "FuelConsumptionMap");
			} catch (Exception) {
				if (!TolerateMissing) {
					throw;
				}

				whrMap =
					new TableData(
						Path.Combine(BasePath, Body[JsonKeys.Engine_FuelConsumptionMap].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
			}

			var retVal = new JSonWHRData(correctionFactors, whrMap);
			return retVal;
		}

		public class JSONFuelInputData : IEngineFuelEngineeringInputData {
			#region Implementation of IEngineFuelDelcarationInputData

			public FuelType FuelType { get; set; }
			public double WHTCMotorway { get; set; }
			public double WHTCRural { get; set; }
			public double WHTCUrban { get; set; }
			public double ColdHotBalancingFactor { get; set; }
			public double CorrectionFactorRegPer { get; set; }
			public TableData FuelConsumptionMap { get; set; }

			#endregion

			#region Implementation of IEngineFuelEngineeringInputData

			public double WHTCEngineering { get; set; }

			#endregion
		}

		private class JSonWHRData : IWHRData
		{
			protected JToken CorrectionFactors;
			protected TableData WHRMap;

			public JSonWHRData(JToken correctionFactors, TableData whrMap)
			{
				CorrectionFactors = correctionFactors;
				WHRMap = whrMap;
			}

			#region Implementation of IWHRData

			public double UrbanCorrectionFactor => CorrectionFactors?["Urban"]?.ToString().ToDouble() ?? 1.0;

			public double RuralCorrectionFactor => CorrectionFactors?["Rural"]?.ToString().ToDouble() ?? 1.0;

			public double MotorwayCorrectionFactor => CorrectionFactors?["Motorway"]?.ToString().ToDouble() ?? 1.0;

			public double BFColdHot => CorrectionFactors?["ColdHotBalancingFactor"]?.ToString().ToDouble() ?? 1.0;

			public double CFRegPer => CorrectionFactors?["CFRegPer"]?.ToString().ToDouble() ?? 1.0;

			public double EngineeringCorrectionFactor => CorrectionFactors?["EngineeringCorrectionFactor"]?.ToString().ToDouble() ?? 1.0;

			public TableData GeneratedPower => WHRMap;

			#endregion
		}



	}


	public class JSONEngineDataV4 : JSONEngineDataV3
	{
		public JSONEngineDataV4(JObject data, string fileName, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing) { }

		public override Watt RatedPowerDeclared => Body.GetEx<double>("RatedPower").SI<Watt>();

		public override PerSecond RatedSpeedDeclared => Body.GetEx<double>("RatedSpeed").RPMtoRad();

		public override NewtonMeter MaxTorqueDeclared => Body.GetEx<double>("MaxTorque").SI<NewtonMeter>();

		public override double CorrectionFactorRegPer => Body.GetEx<double>("CFRegPer");


		public override FuelType FuelType => Body.GetEx<string>("FuelType").ParseEnum<FuelType>();
	}


	public class JSONEngineDataV3 : JSONFile, IEngineEngineeringInputData, IEngineModeEngineeringInputData,
		IEngineFuelEngineeringInputData
	{
		protected IList<IEngineFuelEngineeringInputData> _fuels;

		public JSONEngineDataV3(JObject data, string fileName, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing) { }

		public virtual CubicMeter Displacement => Body.GetEx<double>(JsonKeys.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>();

		// convert vom ccm to m^3}
		public virtual PerSecond IdleSpeed => Body.GetEx<double>(JsonKeys.Engine_IdleSpeed).RPMtoRad();


		public virtual FuelType FuelType => FuelType.DieselCI;

		public virtual TableData FuelConsumptionMap
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>(JsonKeys.Engine_FuelConsumptionMap), "FuelConsumptionMap");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Path.Combine(BasePath, Body[JsonKeys.Engine_FuelConsumptionMap].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}

		public virtual TableData FullLoadCurve
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>(JsonKeys.Engine_FullLoadCurveFile), "FullLoadCurve");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body[JsonKeys.Engine_FullLoadCurveFile].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		IList<IEngineFuelEngineeringInputData> IEngineModeEngineeringInputData.Fuels => _fuels ?? (_fuels = ReadFuels());

		public virtual IList<IEngineFuelDeclarationInputData> Fuels => (_fuels ?? (_fuels = ReadFuels())).Cast<IEngineFuelDeclarationInputData>().ToList();

		protected virtual IList<IEngineFuelEngineeringInputData> ReadFuels()
		{
			return new IEngineFuelEngineeringInputData[] { this };
		}

		public virtual IWHRData WasteHeatRecoveryDataElectrical => null;

		public virtual IWHRData WasteHeatRecoveryDataMechanical => null;

		public virtual Watt RatedPowerDeclared => 0.SI<Watt>();

		public virtual PerSecond RatedSpeedDeclared => 0.RPMtoRad();

		public virtual NewtonMeter MaxTorqueDeclared => 0.SI<NewtonMeter>();

		IList<IEngineModeEngineeringInputData> IEngineEngineeringInputData.EngineModes
		{
			get { return new IEngineModeEngineeringInputData[] { this }; }
		}

		public virtual IList<IEngineModeDeclarationInputData> EngineModes
		{
			get { return new IEngineModeDeclarationInputData[] { this }; }
		}

		public virtual WHRType WHRType => WHRType.None;

		public virtual KilogramSquareMeter Inertia => Body.GetEx<double>(JsonKeys.Engine_Inertia).SI<KilogramSquareMeter>();

		public virtual double WHTCEngineering
		{
			get {
				if (Body["WHTC-Engineering"] == null) {
					return 1;
				}

				return Body.GetEx<double>("WHTC-Engineering");
			}
		}

		public virtual Second EngineStartTime => null;

		public virtual double WHTCMotorway => Body.GetEx<double>(JsonKeys.Engine_WHTC_Motorway);

		public virtual double WHTCRural => Body.GetEx<double>(JsonKeys.Engine_WHTC_Rural);

		public virtual double WHTCUrban => Body.GetEx<double>(JsonKeys.Engine_WHTC_Urban);

		public double ColdHotBalancingFactor
		{
			get {
				if (Body["ColdHotBalancingFactor"] == null) {
					return 1.0;
				}

				return Body.GetEx<double>("ColdHotBalancingFactor");
			}
		}

		public virtual double CorrectionFactorRegPer => 1;


		public virtual string Manufacturer => Constants.NOT_AVAILABLE;

		public virtual string Model => Body.GetEx<string>(JsonKeys.Engine_ModelName);


		public virtual DateTime Date => DateTime.MinValue;

		public virtual CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public virtual string CertificationNumber => Constants.NOT_AVAILABLE;

		public virtual DigestData DigestValue => null;
	}
}
