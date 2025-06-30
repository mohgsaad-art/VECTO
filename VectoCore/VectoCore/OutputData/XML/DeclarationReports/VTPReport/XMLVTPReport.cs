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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using NLog;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.Declaration.Auxiliaries;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using LogManager = NLog.LogManager;
using static TUGraz.VectoCore.Models.Simulation.Data.VectoRunData;
using static TUGraz.VectoCore.Models.Declaration.PT1;
using TUGraz.VectoCore.Models.Simulation.Impl;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.OutputData.XML
{
	internal class XMLVTPReport : DeclarationReport<XMLVTPReport.ResultEntry>, IVTPReport
	{
		public const string CURRENT_SCHEMA_VERSION = "0.21";

		private const string VTPReportTartetName = "VTPReportTarget";
		
		protected XElement VehiclePart;
		protected XElement GeneralPart;
		protected XElement DataIntegrityPart;
		protected XElement TestConditionsPart;

		protected XElement ResultsPart;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace rootNS = "urn:tugraz:ivt:VectoAPI:VTPReport";
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:VTPReport:v" + CURRENT_SCHEMA_VERSION;

		private static List<string> LogList = new List<string>();
		private LoggingRule cycleChecksRule;

		protected VehicleClass VehicleClass = VehicleClass.Unknown;
		protected VehicleCode? VehicleCode = VectoCommon.Models.VehicleCode.NOT_APPLICABLE;

		//protected XNamespace di;
		//private bool allSuccess = true;

		public class ResultEntry : XMLDeclarationReport.ResultEntry
		{
			public Watt AverageFanPower;
			public Dictionary<FuelType, Kilogram> VTPFcFinalSimulated = new Dictionary<FuelType, Kilogram>();
			public WattSecond VTPWorkPWheelPos;
			public Dictionary<FuelType, double> VTPFcCorrectionFactors;
			public Dictionary<FuelType, Kilogram> VTPFcMeasured = new Dictionary<FuelType, Kilogram>();
			public WattSecond VTPWorPWheelSimPos;
			public TankSystem? TankSystem;

			public KilogramPerWattSecond CH4Emissions { protected set; get; } = null;
			public KilogramPerWattSecond COEmissions { protected set; get; } = null;
			public KilogramPerWattSecond NMHCEmissions { protected set; get; } = null;
			public KilogramPerWattSecond NOxEmissions { protected set; get; } = null;
			public KilogramPerWattSecond THCEmissions { protected set; get; } = null;
			public PerWattSecond PMFlow { protected set; get; } = null;
			public KilogramPerWattSecond CO2Emissions { protected set; get; } = null;
			public double PositiveEngineWorkDeviation { protected set; get; } = double.NaN;
			public WattSecond MeasuredPositiveEngineWork { protected set; get; } = null;
			public WattSecond SimulatedPositiveEngineWork { protected set; get; } = null;
			public IList<IFuelNCVData> FuelNCVs { protected set; get; } = null;

			#region Overrides of ResultEntry

			public override void SetResultData(VectoRunData runData, IModalDataContainer data, double factor)
			{
				base.SetResultData(runData, data, factor);
				
				if (runData.SimulationType != SimulationType.VerificationTest) {
					return;
				}

				TankSystem = runData.VehicleData.InputData.TankSystem;
				var aux = data.Auxiliaries.FirstOrDefault(x => x.Key == Constants.Auxiliaries.IDs.Fan);
				AverageFanPower = data.AuxiliaryWork(aux.Value) / data.Duration;
				var cycleEntries = runData.Cycle.Entries.Pairwise().Select(
					x => new {
						PWheel = x.Item1.PWheel > 0 ? x.Item1.PWheel : 0.SI<Watt>(),
						dt = x.Item2.Time - x.Item1.Time,
						FC = x.Item1.Fuelconsumption
					}).ToArray();
				VTPWorPWheelSimPos = data.WorkWheelsPos();
				VTPWorkPWheelPos = cycleEntries.Sum(x => x.PWheel * x.dt).Cast<WattSecond>();
				foreach (var fuel in cycleEntries.First().FC.Keys) {
					VTPFcMeasured[fuel] = cycleEntries.Sum(x => x.FC[fuel] * x.dt);
				}

				foreach (var entry in data.FuelData) {
					var col = data.GetColumnName(entry, ModalResultField.FCFinal);
					var fcSum = data.TimeIntegral<Kilogram>(col);
					VTPFcFinalSimulated[entry.FuelType] = fcSum;
				}

				VTPFcCorrectionFactors = runData.VTPData.CorrectionFactors;
				FuelNCVs = runData.VTPData.FuelNCVs;

				CalculatePositiveEngineWorkData(runData, data);
				CalculateEmissions(runData, MeasuredPositiveEngineWork);
			}

			private WattSecond CalculateMeasuredPositiveEngineWork(VectoRunData runData)
			{
				return runData.Cycle.Entries.Pairwise()
					.Where(x => x.Item1.CombustionEngineTorque.IsGreaterOrEqual(0))
					.Sum(x => (x.Item1.EngineSpeed * x.Item1.CombustionEngineTorque) * (x.Item2.Time - x.Item1.Time));
			}

			private void CalculatePositiveEngineWorkData(VectoRunData runData, IModalDataContainer data)
			{ 
				MeasuredPositiveEngineWork = CalculateMeasuredPositiveEngineWork(runData);
				SimulatedPositiveEngineWork = data.TotalEngineWorkPositive();
				PositiveEngineWorkDeviation = 
					((MeasuredPositiveEngineWork - SimulatedPositiveEngineWork) / MeasuredPositiveEngineWork);
			}

			private void CalculateEmissions(VectoRunData runData, WattSecond measuredPositiveEngineWork)
			{
				var entries = runData.Cycle.Entries;

				if (runData.Cycle.Entries.First().CH4MassFlow != null) {
					var CH4 = entries.Pairwise().Sum(x => (x.Item1.CH4MassFlow) * (x.Item2.Time - x.Item1.Time));
					CH4Emissions = (CH4 / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();
				}

				var CO = entries.Pairwise().Sum(x => x.Item1.COMassFlow * (x.Item2.Time - x.Item1.Time));
				COEmissions = (CO / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();

				if (entries.First().NMHCMassFlow != null) {
					var NMHC = entries.Pairwise().Sum(x => x.Item1.NMHCMassFlow * (x.Item2.Time - x.Item1.Time));
					NMHCEmissions = (NMHC / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();
				}

				var NOx = entries.Pairwise().Sum(x => x.Item1.NOxMassFlow * (x.Item2.Time - x.Item1.Time));
				NOxEmissions = (NOx / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();

				if (entries.First().THCMassFlow != null) {
					var THC = entries.Pairwise().Sum(x => x.Item1.THCMassFlow * (x.Item2.Time - x.Item1.Time));
					THCEmissions = (THC / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();
				}
	
				var PN = entries.Pairwise().Sum(x => x.Item1.PMNumberFlow * (x.Item2.Time - x.Item1.Time));
				PMFlow = PN / measuredPositiveEngineWork;

				var CO2 = entries.Pairwise().Sum(x => x.Item1.CO2MassFlow *  (x.Item2.Time - x.Item1.Time));
				CO2Emissions = (CO2 / measuredPositiveEngineWork).Cast<KilogramPerWattSecond>();
			}

			#endregion
		}

		public XMLVTPReport(IReportWriter writer) : base(writer)
		{
			//di = "http://www.w3.org/2000/09/xmldsig#";

			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			GeneralPart = new XElement(tns + "General");
			DataIntegrityPart = new XElement(tns + "DataIntegrityCheck");
			TestConditionsPart = new XElement(tns + "TestConditions");
			ResultsPart = new XElement(tns + "Results");

			AddLogging();
		}

		private void AddLogging()
		{
			LogList.Clear();
			var target = new MethodCallTarget {
				ClassName = typeof(XMLVTPReport).AssemblyQualifiedName,
				MethodName = "LogMethod"
			};
			target.Parameters.Add(new MethodCallParameter("${level}"));
			target.Parameters.Add(new MethodCallParameter("${message}"));
			var config = LogManager.Configuration;
			if (config == null) {
				config = new LoggingConfiguration();
				LogManager.Configuration = config;
			}
			cycleChecksRule = new LoggingRule(typeof(VTPCycle).FullName, LogLevel.Error, target);
			config.AddTarget(VTPReportTartetName, target);
			config.LoggingRules.Add(cycleChecksRule);
			LogManager.Configuration.Reload();
		}

		// ReSharper disable once UnusedMember.Global -- see AddLogging Method
		public static void LogMethod(string level, string message)
		{
			LogList.Add(message);
		}

		#region Overrides of DeclarationReport<ResultEntry>

		protected override void DoStoreResult(
			ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			entry.SetResultData(runData, modData, 0.0);
		}

		protected internal override void DoWriteReport()
		{
			var config = LogManager.Configuration;
			config.LoggingRules.Remove(cycleChecksRule);
			config.RemoveTarget(VTPReportTartetName);
			LogManager.Configuration.Reload();

			GenerateResults();

			var report = GenerateReport();
			if (Writer != null) {
				Writer.WriteReport(ReportType.DeclarationVTPReportXML, report);
			}
		}

		protected override void OutputReports()
		{
			throw new NotImplementedException();
		}

		protected override void GenerateReports()
		{
			throw new NotImplementedException();
		}

		protected override void WriteResult(ResultEntry result)
		{
			throw new NotImplementedException();
		}

		private MissionType GetSelectedMission()
		{ 
			return VehicleClass.IsBus()
				? (VehicleCode.GetFloorType() == FloorType.LowFloor
					? DeclarationData.VTPMode.SelectedMissionLowFloorBus
					: DeclarationData.VTPMode.SelectedMissionHighFloorBus)
				: (VehicleClass.IsMediumLorry()
					? DeclarationData.VTPMode.SelectedMissionMediumLorry
					: DeclarationData.VTPMode.GetSelectedMissionHeavyLorry(VehicleClass));
		}

		private void GenerateResults()
		{
			var vtpResult = Results.OrderBy(x => x.FuelMode).FirstOrDefault(x => x.Mission == MissionType.VerificationTest);

			if (vtpResult == null) {
				throw new VectoException("no vtp result found for generating vtp report");
			}

			var selectedMission = GetSelectedMission();
			
			var vtpFcMeasured = vtpResult.VTPFcMeasured.Select(x => Tuple.Create(x.Key, x.Value / vtpResult.VTPWorkPWheelPos)).ToDictionary(x => x.Item1, x => x.Item2);
			
			var vtpFcMeasuredCorr = vtpResult.VTPFcMeasured
				.Select(x =>
					Tuple.Create(x.Key, x.Value / vtpResult.VTPWorkPWheelPos * vtpResult.VTPFcCorrectionFactors[x.Key]))
				.ToDictionary(x => x.Item1, x => x.Item2);
			
			var vtpFcSimulated = vtpResult.VTPFcFinalSimulated
				.Select(x => Tuple.Create(x.Key, x.Value / vtpResult.VTPWorPWheelSimPos))
				.ToDictionary(x => x.Item1, x => x.Item2);

			var fuels = DeclarationData.FuelData;

			var CO2MeasuredPerFuel = vtpFcMeasured.Select(e => new {
					fuelType = e.Key,
					co2 = e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP
				}).ToDictionary(x => x.fuelType, x => x.co2);

			var CO2MeasuredCorrectedPerFuel = vtpFcMeasuredCorr.Select(e => new {
					fuelType = e.Key,
					co2 = e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP
				}).ToDictionary(x => x.fuelType, x => x.co2);

			var CO2SimulatedPerFuel = vtpFcSimulated.Select(e => new {
					fuelType = e.Key,
					co2 = e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP
				}).ToDictionary(x => x.fuelType, x => x.co2);

			var CO2Measured = vtpFcMeasured.Sum(e => e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP);
			var CO2MeasuredCorrected = vtpFcMeasuredCorr.Sum(e => e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP);
			var CO2Simulated = vtpFcSimulated.Sum(e => e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP);
					
			var cVtp = CO2MeasuredCorrected / CO2Simulated;


			var result = ManufacturerRecord.Results.Results.Where(x => x.Mission == selectedMission)
											.MaxBy(x => x.SimulationParameter.Payload);
			var key = VehicleClass.IsBus() ? "g/p-km" : "g/t-km";
			var declaredCO2 = result.CO2[key];

			if (result == null) {
				throw new VectoException("no corresponding simulation result found for generating vtp report");
			}
			
			var verifiedCO2 = declaredCO2 * cVtp.Value();
			
			ResultsPart.Add(
				new XElement(tns + "Status", 
					(cVtp < 1.075) && (vtpResult.Status == VectoRun.Status.Success) ? "Passed" : "Failed"),
				new XElement(
					tns + "AverageFanPower",
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "kW"),
					vtpResult.AverageFanPower.ConvertToKiloWatt().ToXMLFormat(3)),
				new XElement(
					tns + "WorkPosVT", new XAttribute(XMLNames.Report_Results_Unit_Attr, "kWh"),
					vtpResult.VTPWorkPWheelPos.ConvertToKiloWattHour().ToXMLFormat(3)),
				new XElement(
					tns + "SimulatedWorkPosVT", new XAttribute(XMLNames.Report_Results_Unit_Attr, "kWh"),
					vtpResult.VTPWorPWheelSimPos.ConvertToKiloWattHour().ToXMLFormat(3)),
				vtpResult.FuelNCVs.Select(x => 
				new XElement(
					tns + "FuelNCV",
					new XAttribute("fuelType", x.Type.ToXMLFormat()),
					new XAttribute(XMLNames.Report_Results_Unit_Attr, x.NCV.ConvertToMegaJoulePerKilogram().Units),
					x.NCV.ConvertToMegaJoulePerKilogram().ToXMLFormat(3)
				)),
				CreateConsumptionElements(vtpFcMeasured, vtpFcMeasuredCorr, vtpFcSimulated, 
					CO2MeasuredPerFuel, CO2MeasuredCorrectedPerFuel, CO2SimulatedPerFuel),
				CreateCO2Element(cVtp, CO2Measured, CO2MeasuredCorrected, CO2Simulated, 
					CO2MeasuredPerFuel, CO2MeasuredCorrectedPerFuel, CO2SimulatedPerFuel),
				new XElement(tns + "C_VTP", cVtp.ToXMLFormat(4)),
				CreatePollutantsElement(vtpResult, CO2MeasuredCorrected));

			var threshold = Constants.SimulationSettings.VTPEngineWorkDeviationThreshold;

			if (Math.Abs(vtpResult.PositiveEngineWorkDeviation) > threshold) {
				LogList.Add($@"Simulated positive engine work is deviating more than {threshold * 100}% 
					from measured positive engine work through the vdri. 
					Deviation = {(vtpResult.PositiveEngineWorkDeviation * 100).ToXMLFormat(2)}%");
			}

			if (LogList.Any()) {
				ResultsPart.Add(new XElement(tns + "Warnings", LogList.Select(x => new XElement(tns + "Warning", x))));
			}
		}

		private XElement CreateCO2Element(Scalar cVtp, 
			SpecificFuelConsumption CO2Measured, 
			SpecificFuelConsumption CO2MeasuredCorrected,
			SpecificFuelConsumption CO2Simulated, 
			Dictionary<FuelType, SpecificFuelConsumption> CO2MeasuredPerFuel,
			Dictionary<FuelType, SpecificFuelConsumption> CO2MeasuredCorrectedPerFuel, 
			Dictionary<FuelType, SpecificFuelConsumption> CO2SimulatedPerFuel)
		{
			var vtpResult = Results.OrderBy(x => x.FuelMode).FirstOrDefault(x => x.Mission == MissionType.VerificationTest);

			var selectedMission = GetSelectedMission();

			const LoadingType selectedLoading = DeclarationData.VTPMode.SelectedLoading;
			var result = ManufacturerRecord.Results.Results.Where(x => x.Mission == selectedMission)
											.MaxBy(x => x.SimulationParameter.Payload);

			var key = VehicleClass.IsBus() ? "g/p-km" : "g/t-km";
			var declaredCO2 = result.CO2[key];

			var verifiedCO2 = declaredCO2 * cVtp.Value();
			var CO2Unit = "g/t-km";
			
			return new XElement(
				tns + "CO2",
				new XElement(
					tns + "Certification",
					new XElement(
						tns + "Mission",
						$"{selectedMission.ToXMLFormat()}, {selectedLoading.ToString()}"
					),
					new XElement(
						tns + "Declared", 
						new XAttribute(XMLNames.Report_Results_Unit_Attr, CO2Unit),
						declaredCO2.ToMinSignificantDigits(3, 1)
					),
					new XElement(
						tns + "Verified", 
						new XAttribute(XMLNames.Report_Results_Unit_Attr, CO2Unit),
						verifiedCO2.ToMinSignificantDigits(3, 1)
					)
				),
				new XElement(
					tns + "VTP",
					new XElement(
						tns + "TotalEmissions",
						new XElement(
							tns + "Measured",  
							new XAttribute(XMLNames.Report_Results_Unit_Attr, CO2Measured.ConvertToGramPerKiloWattHour().Units),
							CO2Measured.ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
						),
						new XElement(
							tns + "MeasuredCorrected",  
							new XAttribute(XMLNames.Report_Results_Unit_Attr, CO2MeasuredCorrected.ConvertToGramPerKiloWattHour().Units),
							CO2MeasuredCorrected.ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
						),
						new XElement(
							tns + "Simulated",  
							new XAttribute(XMLNames.Report_Results_Unit_Attr, CO2Simulated.ConvertToGramPerKiloWattHour().Units),
							CO2Simulated.ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
						)
					),
					vtpResult.FuelData.Select(f => new [] {
						new XElement(
							tns + "Emissions",
							new XAttribute("fuelType", f.FuelType.ToXMLFormat()),
							new XElement(
								tns + "Measured", 
								new XAttribute(XMLNames.Report_Results_Unit_Attr, 
									CO2MeasuredPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Units),
								CO2MeasuredPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
							),
							new XElement(
								tns + "MeasuredCorrected", 
								new XAttribute(XMLNames.Report_Results_Unit_Attr, 
									CO2MeasuredCorrectedPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Units),
								CO2MeasuredCorrectedPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
							),
							new XElement(
								tns + "Simulated", 
								new XAttribute(XMLNames.Report_Results_Unit_Attr, 
									CO2SimulatedPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Units),
								CO2SimulatedPerFuel[f.FuelType].ConvertToGramPerKiloWattHour().Value.ToXMLFormat(3)
							)
						)
					}).ToArray()
				));
		}

		private XElement[] CreateConsumptionElements(
			Dictionary<FuelType, SpecificFuelConsumption> vtpFcMeasured,
			Dictionary<FuelType, SpecificFuelConsumption> vtpFcMeasuredCorr, 
			Dictionary<FuelType, SpecificFuelConsumption> vtpFcSimulated,
			Dictionary<FuelType, SpecificFuelConsumption> CO2MeasuredPerFuel,
			Dictionary<FuelType, SpecificFuelConsumption> CO2MeasuredCorrectedPerFuel, 
			Dictionary<FuelType, SpecificFuelConsumption> CO2SimulatedPerFuel)
		{
			var fuels = DeclarationData.FuelData;
			var vtpResult = Results.OrderBy(x => x.FuelMode).FirstOrDefault(x => x.Mission == MissionType.VerificationTest);
			
			return vtpFcMeasured.Select(x =>
				new XElement(
					tns + "FuelConsumption",
					new XAttribute("fuelType", x.Key.ToXMLFormat()),
					new XElement(
						tns + "Measured",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcMeasured[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "MeasuredCorrected",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcMeasuredCorr[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "Simulated",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcSimulated[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					)
				)
			).Concat(
				new XElement(
					tns + "TotalFuelConsumption",
					new XElement(
						tns + "Measured",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						CalculateFuelConsumptionThroughCO2(vtpFcMeasured, CO2MeasuredPerFuel, fuels, vtpResult.TankSystem)
							.ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "MeasuredCorrected",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						CalculateFuelConsumptionThroughCO2(vtpFcMeasuredCorr, CO2MeasuredCorrectedPerFuel, fuels, vtpResult.TankSystem)
							.ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "Simulated",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						CalculateFuelConsumptionThroughCO2(vtpFcSimulated, CO2SimulatedPerFuel, fuels, vtpResult.TankSystem)
							.ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					)
				).ToEnumerable()
			).ToArray();
		}

		private SpecificFuelConsumption CalculateFuelConsumptionThroughCO2(Dictionary<FuelType, SpecificFuelConsumption> fc,
			Dictionary<FuelType, SpecificFuelConsumption> co2, FuelData fuelData, TankSystem? tankSystem)
		{
			var totalFC = fc.Sum(x => x.Value);
			var totalCO2 = co2.Sum(x => x.Value);

			var co2ToFuel = co2.Sum(x => fuelData.Lookup(x.Key, tankSystem).CO2PerFuelWeightVTP * (fc[x.Key] / totalFC));
			
			return totalCO2 / co2ToFuel;
		}

		private XElement CreatePollutantsElement(ResultEntry vtpResult, SpecificFuelConsumption CO2MeasuredCorrected)
		{ 
			var pollutantsElement = new XElement(
				tns + "Pollutants",
				new XElement(tns + "CO", XMLHelper.ValueAsUnit(vtpResult.COEmissions, "mg/kWh", 3)
				),
				new XElement(tns + "NOx", XMLHelper.ValueAsUnit(vtpResult.NOxEmissions, "mg/kWh", 3)
				),
				new XElement(tns + "CO2", XMLHelper.ValueAsUnit(vtpResult.CO2Emissions, "g/kWh", 3)
				)
			);

			if (vtpResult.THCEmissions != null) {
				pollutantsElement.Add(
					new XElement(tns + "THC", XMLHelper.ValueAsUnit(vtpResult.THCEmissions, "mg/kWh", 3)
					));
			}

			if (vtpResult.CH4Emissions != null) {
				pollutantsElement.Add(
					new XElement(tns + "CH4", XMLHelper.ValueAsUnit(vtpResult.CH4Emissions, "mg/kWh", 3)
					));
			}

			if (vtpResult.NMHCEmissions != null) {
				pollutantsElement.Add(
					new XElement(tns + "NMHC", XMLHelper.ValueAsUnit(vtpResult.NMHCEmissions, "mg/kWh", 3)
					));
			}

			pollutantsElement.Add(
				new XElement(tns + "PM", XMLHelper.ValueAsUnit(vtpResult.PMFlow, "#/kWh", 3)
				));

			pollutantsElement.Add(
				new XElement(tns + "PositiveEngineWork",
					new XElement(tns + "Measured", XMLHelper.ValueAsUnit(vtpResult.MeasuredPositiveEngineWork, "kWh", 3)
					),
					new XElement(tns + "Simulated", XMLHelper.ValueAsUnit(vtpResult.SimulatedPositiveEngineWork, "kWh", 3)
					)
				));

			return pollutantsElement;
		}

		private XDocument GenerateReport()
		{

			var retVal = new XDocument();
			retVal.Add(
				new XProcessingInstruction(
					"xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(
				new XElement(
					rootNS + "VectoVTPReport",
					//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "tns", rootNS),

					//new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(
						xsi + "schemaLocation",
						$"{rootNS} {AbstractXMLWriter.SchemaLocationBaseUrl}VTPReport.xsd"),
					new XElement(
						rootNS + "Data",
						new XAttribute(xsi + XMLNames.XSIType, "VTPReportDataType"),
						new XElement(GeneralPart),
						new XElement(VehiclePart),
						new XElement(DataIntegrityPart),
						new XElement(TestConditionsPart),
						new XElement(ResultsPart),
						GetApplicationInfo()
					)
				)
			);

			return retVal;
		}

		public override void InitializeReport(VectoRunData modelData)
		{
			VehicleClass = modelData.VehicleData.VehicleClass;
			if (VehicleClass.IsBus()) {
				VehicleCode = modelData.VehicleData.VehicleCode;
			}
			GeneralPart.Add(
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress));
			VehiclePart.Add(
				new XAttribute(xsi + XMLNames.XSIType, "VehicleType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, modelData.VehicleData.GrossVehicleMass.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, modelData.VehicleData.CurbMass.ToXMLFormat(0)),
				modelData.Retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, modelData.Retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null));
			if (modelData.VehicleData.AxleConfiguration.AxlegearIncludedInGearbox()) {
				VehiclePart.Add(
					new XElement(
						tns + XMLNames.Vehicle_Components,
						new XAttribute(xsi + XMLNames.XSIType, "ComponentsTruckFWDType"),
						GetEngineDescription(modelData.EngineData, modelData.InputData.JobInputData.Vehicle.TankSystem),
						GetGearboxDescription(modelData.GearboxData, modelData.AxleGearData.AxleGear.Ratio),
						GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
						GetRetarderDescription(modelData.Retarder),
						GetAngledriveDescription(modelData.AngledriveData),
						GetAirDragDescription(modelData.AirdragData),
						GetAxleWheelsDescription(modelData.VehicleData),
						GetAuxiliariesDescription(modelData.Aux)
					));
			} else {
				VehiclePart.Add(
					new XElement(
						tns + XMLNames.Vehicle_Components,
						new XAttribute(xsi + XMLNames.XSIType, "ComponentsTruckType"),
						GetEngineDescription(modelData.EngineData, modelData.VehicleData.InputData.TankSystem),
						GetGearboxDescription(modelData.GearboxData),
						GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
						GetRetarderDescription(modelData.Retarder),
						GetAngledriveDescription(modelData.AngledriveData),
						GetAxlegearDescription(modelData.AxleGearData),
						GetAirDragDescription(modelData.AirdragData),
						GetAxleWheelsDescription(modelData.VehicleData),
						GetAuxiliariesDescription(modelData.Aux)
					));
			}

			if (InputDataHash == null) {
				return;
			}

			ManufacturerRecord.ValidateSimulationToolVersion();
			ManufacturerRecord.ValidateHash();

			var allSuccess = true;

			var componentChecks = ComponentIntegrityChecks(ref allSuccess);
			var jobIntegrity = JobIntegrityChecks(ref allSuccess);
			var manufacturerReportIntegrity = ManufacturerReportIntegrityChecks(ref allSuccess);

			DataIntegrityPart.Add(
				new XAttribute("status", allSuccess ? XMLNames.Report_Results_Status_Success_Val : "failed"),
				new XElement(
					tns + "Components",
					componentChecks.ToArray()
				),
				manufacturerReportIntegrity,
				jobIntegrity
			);
		}

		private List<object> ComponentIntegrityChecks(ref bool allSuccess)
		{
			var componentChecks = new List<object>();
			var components = InputDataHash.GetContainigComponents().GroupBy(s => s)
										.Select(g => new { Entry = g.Key, Count = g.Count() });
			foreach (var component in components) {
				if (component.Entry == VectoComponents.Vehicle) {
					continue;
				}

				for (var i = 0; i < component.Count; i++) {
					componentChecks.Add(CheckComponent(ref allSuccess, component.Entry, i, component.Count));
				}
			}

			return componentChecks;
		}

		private XElement CheckComponent(ref bool allSuccess, VectoComponents component, int i, int count)
		{
			bool status;
			var componentName = count == 1
				? VectoComponentsExtensionMethods.XMLElementName(component)
				: $"{VectoComponentsExtensionMethods.XMLElementName(component)} ({i + 1})";
			XElement retVal;
			try {
				var recomputed = InputDataHash.ComputeHash(component, i);
				var readJob = InputDataHash.ReadHash(component, i);
				var read = ManufacturerRecord.ComponentDigests[component][i];
				status = string.Equals(readJob, recomputed) && string.Equals(recomputed, read);
				retVal = new XElement(
						tns + "Component",
						new XAttribute("componentName", componentName),
						new XAttribute("status", status ? XMLNames.Report_Results_Status_Success_Val : "failed"),
						new XElement(tns + "DigestValueRecomputed", recomputed),
						new XElement(
							tns + "DigestValueRead",
							new XAttribute("source", "JobData"),
							readJob
						),
						new XElement(
							tns + "DigestValueRead",
							new XAttribute("source", "ManufacturerRecord"),
							read
						)
					);
			} catch (Exception e) {
				status = false;
				retVal = new XElement(
						tns + "Component",
						new XAttribute("componentName", componentName),
						new XAttribute("status", "failed"),
						new XElement(tns + "Error", e.Message));
			}
			allSuccess = allSuccess && status;
			return retVal;
		}

		private XElement ManufacturerReportIntegrityChecks(ref bool allSuccess)
		{
			bool mrStatus;
			XElement manufacturerReportIntegrity;
			try {
				var mrHashRead = ManufacturerRecordHash.ReadHash();
				var mrHashRecomputed = ManufacturerRecordHash.ComputeHash();
				mrStatus = ManufacturerRecordHash.ValidateHash();
				manufacturerReportIntegrity = new XElement(
					tns + "ManufacturerReport",
					new XAttribute("status", mrStatus ? XMLNames.Report_Results_Status_Success_Val : "failed"),
					new XElement(tns + "DigestValueRecomputed", mrHashRecomputed),
					new XElement(
						tns + "DigestValueRead",
						new XAttribute("source", "ManufacturerRecord"), mrHashRead)
				);
			} catch (Exception e) {
				mrStatus = false;
				var mrError = e.Message;
				manufacturerReportIntegrity = new XElement(
					tns + "ManufacturerReport",
					new XAttribute("status", "failed"),
					new XElement(tns + "Error", mrError)
				);
			}
			allSuccess = allSuccess && mrStatus;
			return manufacturerReportIntegrity;
		}

		private XElement JobIntegrityChecks(ref bool allSuccess)
		{
			bool jobStatus;
			XElement jobIntegrity;
			try {
				var jobHashMethods = ManufacturerRecord.JobDigest;
				var jobHashRecomputed = InputDataHash.ComputeHash(
					jobHashMethods.CanonicalizationMethods, jobHashMethods.DigestMethod);
				var jobHashRead = jobHashMethods.DigestValue;
				jobStatus = string.Equals(jobHashRecomputed, jobHashRead);
				jobIntegrity = new XElement(
					tns + "JobData",
					new XAttribute("status", jobStatus ? XMLNames.Report_Results_Status_Success_Val : "failed"),
					new XElement(
						tns + "DigestValueRecomputed",
						jobHashRecomputed),
					new XElement(
						tns + "DigestValueRead",
						new XAttribute("source", "ManufacturerRecord"),
						jobHashRead)
				);
			} catch (Exception e) {
				jobStatus = false;
				var jobError = e.Message;
				jobIntegrity = new XElement(
					tns + "JobData",
					new XAttribute("status", "failed"),
					new XElement(tns + "Error", jobError)
				);
			}
			allSuccess = allSuccess && jobStatus;
			return jobIntegrity;
		}

		#endregion

		private XElement GetApplicationInfo()
		{
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, VectoSimulationCore.VersionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		private XElement GetEngineDescription(CombustionEngineData engineData, TankSystem? tankSystem)
		{
			var fuelModes = engineData.InputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, tankSystem)).ToList())
				.ToList();
			return new XElement(
				tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Engine_Displacement,
					engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
				fuelModes.SelectMany(x => x.Select(f => f.FuelType.ToXMLFormat())).Distinct().Select(
					x => new XElement(tns + XMLNames.Engine_FuelType, x))
			);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears.Last().Value.Ratio.ToXMLFormat(3))
			);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData, double axlegearRatio)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(tns + XMLNames.Gearbox_AxlegearRatio, axlegearRatio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears.Last().Value.Ratio.ToXMLFormat(3))
			);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverterData)
		{
			if (torqueConverterData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_TorqueConverter,
				GetCommonDescription(torqueConverterData));
		}

		private XElement GetRetarderDescription(RetarderData retarder)
		{
			return new XElement(
				tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent() ? GetCommonDescription(retarder) : null);
		}

		private object GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio));
		}

		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return new XElement(
				tns + XMLNames.Component_Axlegear,
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAirDragDescription(AirdragData airdragData)
		{
			if (airdragData.CertificationMethod == CertificationMethod.StandardValues) {
				return new XElement(
					tns + XMLNames.Component_AirDrag,
					new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
				);
			}

			return new XElement(
				tns + XMLNames.Component_AirDrag,
				new XElement(tns + XMLNames.Component_Model, airdragData.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, airdragData.CertificationNumber),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
			);
		}

		private XElement GetAxleWheelsDescription(VehicleData vehicleData)
		{
			var retVal = new XElement(tns + XMLNames.Component_AxleWheels);
			var axleData = vehicleData.AxleData;
			for (var i = 0; i < axleData.Count; i++) {
				if (axleData[i].AxleType == AxleType.Trailer) {
					continue;
				}

				retVal.Add(GetAxleDescription(i + 1, axleData[i]));
			}

			return retVal;
		}

		private XElement GetAxleDescription(int i, Axle axle)
		{
			return new XElement(
				tns + XMLNames.AxleWheels_Axles_Axle,
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i),
				new XElement(tns + XMLNames.Report_Tyre_TyreDimension, axle.WheelsDimension),
				new XElement(tns + XMLNames.Component_CertificationNumber, axle.CertificationNumber),
				new XElement(tns + XMLNames.Report_Tyre_TyreRRCDeclared, axle.RollResistanceCoefficient.ToXMLFormat(4)),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axle.TwinTyres));
		}

		private XElement GetAuxiliariesDescription(IEnumerable<VectoRunData.AuxData> aux)
		{
			var auxData = aux.ToDictionary(a => a.ID);
			var auxList = new[] {
				AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
				AuxiliaryType.HVAC
			};
			var retVal = new XElement(tns + XMLNames.Component_Auxiliaries);
			foreach (var auxId in auxList) {
				if (auxData.TryGetValue(auxId.Key(), out var auxValue)) {
					foreach (var entry in auxValue.Technology) {
						retVal.Add(new XElement(tns + GetTagName(auxId), entry));
					}
				}
			}
			return retVal;
		}

		private string GetTagName(AuxiliaryType auxId)
		{
			return auxId + "Technology";
		}

		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
			};
		}

		private object[] GetCommonDescription(SimulationComponentData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
			};
		}

		public override void SetWeightingFactors(VectoRunData runData, IEnumerable<IResultEntry> orderedeResults, double? electricRange)
		{
			throw new NotImplementedException();
		}

		#region Implementation of IVTPReport

		public IVectoHash InputDataHash { protected get; set; }

		public IManufacturerReport ManufacturerRecord { protected get; set; }

		public IVectoHash ManufacturerRecordHash { protected get; set; }

		#endregion
	}
}
