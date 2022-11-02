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
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		protected IXMLManufacturerReport ManufacturerRpt;

		protected IXMLCustomerReport CustomerRpt;


		protected IDictionary<Tuple<MissionType, LoadingType>, double> _weightingFactors;

		public XMLDeclarationReport(IReportWriter writer) : base(writer) { }

		public class ResultEntry : IResultEntry
		{
			public ResultEntry()
			{
				Distance = double.MaxValue.SI<Meter>();
			}

			public MissionType Mission { get; set; }
			public LoadingType LoadingType { get; set; }
			public int FuelMode { get; set; }
			public IList<IFuelProperties> FuelData { get; set; }


			public Kilogram Payload { get; set; }

			public Kilogram TotalVehicleMass { get; set; }

			public CubicMeter CargoVolume { get; set; }

			public double? PassengerCount { get; set; }
			public VehicleClass VehicleClass { get; set; }

			public MeterPerSecond AverageSpeed { get; private set; }

			public MeterPerSecond AverageDrivingSpeed { get; private set; }

			public Joule EnergyConsumptionTotal { get; private set; }

			public Kilogram CO2Total { get; private set; }

			public Dictionary<FuelType, IFuelConsumptionCorrection> FuelConsumptionFinal { get; private set; }

			public Meter Distance { get; private set; }

			public Scalar GearshiftCount { get; private set; }

			public Scalar FullLoadPercentage { get; private set; }

			public MeterPerSquareSecond MaxDeceleration { get; private set; }

			public MeterPerSquareSecond MaxAcceleration { get; private set; }

			public MeterPerSecond MaxSpeed { get; private set; }

			public MeterPerSecond MinSpeed { get; private set; }

			public string Error { get; private set; }

			public VectoRun.Status Status { get; private set; }

			public string StackTrace { get; private set; }

			public PerSecond EngineSpeedDrivingMin { get; private set; }
			public PerSecond EngineSpeedDrivingAvg { get; private set; }
			public PerSecond EngineSpeedDrivingMax { get; private set; }

			public double AverageGearboxEfficiency { get; private set; }

			public double AverageAxlegearEfficiency { get; private set; }

			public double WeightingFactor { get; set; }

			// used for factor method
			public IResult PrimaryResult { get; set; }


			public virtual void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor)
			{
				Status = data.RunStatus;
				Error = data.Error;
				StackTrace = data.StackTrace;
				AverageSpeed = data.Speed();

				MinSpeed = data.MinSpeed();
				MaxSpeed = data.MaxSpeed();
				MaxAcceleration = data.MaxAcceleration();
				MaxDeceleration = data.MaxDeceleration();
				FullLoadPercentage = data.ICEMaxLoadTimeShare();
				GearshiftCount = data.GearshiftCount();

				var entriesDriving = data.GetValues(
					r => new {
						dt = r.Field<Second>(ModalResultField.simulationInterval.GetName()),
						v = r.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
						nEng = r.Field<PerSecond>(ModalResultField.n_ice_avg.GetName())
					}).Where(x => x.v.IsGreater(0)).ToArray();
				var drivingTime = entriesDriving.Sum(x => x.dt);

				AverageDrivingSpeed = entriesDriving.Sum(x => x.v * x.dt) / drivingTime;
				EngineSpeedDrivingAvg = (entriesDriving.Sum(x => (x.nEng * x.dt).Value()) / drivingTime.Value()).SI<PerSecond>();
				EngineSpeedDrivingMin = entriesDriving.Min(x => x.nEng);
				EngineSpeedDrivingMax = entriesDriving.Max(x => x.nEng);
				Distance = data.Distance;

				FuelConsumptionFinal = data.CorrectedModalData.FuelCorrection;
				CO2Total = data.CorrectedModalData.CO2Total;
				EnergyConsumptionTotal = data.CorrectedModalData.EnergyConsumptionTotal;

				var gbxOutSignal = runData.Retarder.Type == RetarderType.TransmissionOutputRetarder
					? ModalResultField.P_retarder_in
					: (runData.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
				var eGbxIn = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
				var eGbxOut = data.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
				AverageGearboxEfficiency = eGbxOut / eGbxIn;

				var eAxlIn = data.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
				var eAxlOut = data.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
				AverageAxlegearEfficiency = eAxlOut / eAxlIn;

				WeightingFactor = weightingFactor;

				PrimaryResult = runData.PrimaryResult;

			}

		}



		public virtual XDocument FullReport => ManufacturerRpt.Report;

		public virtual XDocument CustomerReport => CustomerRpt.Report;

		public virtual XDocument PrimaryVehicleReport => null;


		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			var factor = _weightingFactors[Tuple.Create(runData.Mission.MissionType, runData.Loading)];
			entry.SetResultData(runData, modData, factor);
		}

		protected override void WriteResult(ResultEntry result)
		{
			var sumWeightinFactors = _weightingFactors.Values.Sum(x => x);
			if (!sumWeightinFactors.IsEqual(0) && !sumWeightinFactors.IsEqual(1)) {
				throw new VectoException("Mission Profile Weighting factors do not sum up to 1!");
			}

			ManufacturerRpt.WriteResult(result);
			CustomerRpt.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			CustomerRpt.GenerateReport(fullReportHash);
		}


		protected override void OutputReports()
		{
			Writer.WriteReport(ReportType.DeclarationReportCustomerXML, CustomerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
		}


		protected XElement GetSignature(XDocument report)
		{
			return report.XPathSelectElement("/*[local-name()='VectoOutput']/*[local-name()='Signature']/*");
		}


		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			if (modelData.Exempted) {
				WeightingGroup = WeightingGroup.Unknown;
			} else {
				if (modelData.VehicleData.SleeperCab == null) {
					throw new VectoException("SleeperCab parameter is required");
				}

				WeightingGroup = DeclarationData.WeightingGroup.Lookup(
						modelData.VehicleData.VehicleClass, modelData.VehicleData.SleeperCab.Value,
						modelData.EngineData.RatedPowerDeclared);
			}

			_weightingFactors = WeightingGroup == WeightingGroup.Unknown
				? ZeroWeighting
				: DeclarationData.WeightingFactors.Lookup(WeightingGroup);


			InstantiateReports(modelData);

			ManufacturerRpt.Initialize(modelData, fuelModes);
			CustomerRpt.Initialize(modelData, fuelModes);
		}

		public WeightingGroup WeightingGroup { get; protected set; }

		protected virtual void InstantiateReports(VectoRunData modelData)
		{
			if (modelData.Exempted) {
				ManufacturerRpt = new XMLManufacturerReportExemptedTruck();
			} else {
				ManufacturerRpt = new XMLManufacturerReportTruck();
			}
			CustomerRpt = new XMLCustomerReport();
		}

		private static IDictionary<Tuple<MissionType, LoadingType>, double> ZeroWeighting =>
			new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(
				new Dictionary<Tuple<MissionType, LoadingType>, double>() {
					{ Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.LongHaulEMS, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.Construction, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.HeavyUrban, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.Urban, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.Urban, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.Suburban, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.Interurban, LoadingType.ReferenceLoad), 0 },
					{ Tuple.Create(MissionType.Coach, LoadingType.LowLoading), 0 },
					{ Tuple.Create(MissionType.Coach, LoadingType.ReferenceLoad), 0 },
				});


		public static IEnumerable<XElement> GetResults(ResultEntry result, XNamespace tns, bool fullOutput)
		{
			//var fuel = result.FuelData;
			var retVal = new List<XElement>();

			foreach (var fuel in result.FuelData) {
				var entry = result.FuelConsumptionFinal[fuel.FuelType];
				var fcResult = new XElement(tns + XMLNames.Report_Results_Fuel, new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()));
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						entry.FC_FINAL_KM
						.ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 1)),
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						(entry.FC_FINAL_KM / result.Payload)
						.ConvertToGrammPerTonKilometer().ToMinSignificantDigits(3, 1)),
					result.CargoVolume > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
							(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / result.Distance.ConvertToKiloMeter() /
							result.CargoVolume)
							.Value().ToMinSignificantDigits(3, 1))
						: null,
					result.PassengerCount.HasValue && result.PassengerCount.Value > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
							(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / result.Distance.ConvertToKiloMeter() /
							result.PassengerCount.Value).ToMinSignificantDigits(3, 1))
						: null
				);

				//FC
				// TODO: MQ 2019-07-31 - per fuel or overall?
				if (fullOutput) {
					fcResult.Add(
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"),
							(entry.EnergyDemand /
							result.Distance.ConvertToKiloMeter() / 1e6)
							.Value().ToMinSignificantDigits(3, 1)),
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/t-km"),
							(entry.EnergyDemand /
							result.Distance.ConvertToKiloMeter() / result.Payload.ConvertToTon() / 1e6)
							.Value().ToMinSignificantDigits(3, 1)));
					if (result.CargoVolume > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/m³-km"),
								(entry.EnergyDemand /
								result.Distance.ConvertToKiloMeter() / result.CargoVolume / 1e6).Value().ToMinSignificantDigits(3, 1)));
					}
					if (result.PassengerCount.HasValue) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/p-km"),
								(entry.EnergyDemand /
								result.Distance.ConvertToKiloMeter() / result.PassengerCount.Value / 1e6).Value().ToMinSignificantDigits(3, 1))
						);
					}
				}
				if (fuel.FuelDensity != null) {
					fcResult.Add(
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
							(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / fuel.FuelDensity /
							result.Distance.ConvertToKiloMeter() * 100)
							.Value().ToMinSignificantDigits(3, 1)),
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
							(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / fuel.FuelDensity /
							result.Distance.ConvertToKiloMeter() /
							result.Payload.ConvertToTon()).Value().ToMinSignificantDigits(3, 1)));
					if (result.CargoVolume > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
								(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / fuel.FuelDensity /
								result.Distance.ConvertToKiloMeter() /
								result.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
					}
					if (result.PassengerCount.HasValue && result.PassengerCount.Value > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/p-km"),
								(entry.TotalFuelConsumptionCorrected.ConvertToGramm() / fuel.FuelDensity /
								result.Distance.ConvertToKiloMeter() / result.PassengerCount.Value).Value().ToMinSignificantDigits(3, 1))
						);
					}
				}
				retVal.Add(fcResult);
			}

			//CO2
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 2)));
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() /
					result.Payload.ConvertToTon()).ToMinSignificantDigits(3, 2)));
			if (result.CargoVolume > 0)
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() / result.CargoVolume).Value()
																													.ToMinSignificantDigits(3, 2)));
			if (result.PassengerCount.HasValue && result.PassengerCount.Value > 0) {
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
						(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() / result.PassengerCount.Value)
																													.ToMinSignificantDigits(3, 2)));
			}
			return retVal;
		}
	}
}
