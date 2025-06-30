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
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.MonitoringReport;

namespace TUGraz.VectoCore.OutputData.XML
{
    public class XMLDeclarationReport : DeclarationReport<XMLDeclarationReport.ResultEntry>
	{
		protected IXMLManufacturerReport ManufacturerRpt;

		protected IXMLCustomerReport CustomerRpt;

		protected IXMLMonitoringReport _monitoringReport;

		protected readonly IManufacturerReportFactory _mrfFactory;
		protected readonly ICustomerInformationFileFactory _cifFactory;
		

		protected IDictionary<Tuple<MissionType, LoadingType>, double> _weightingFactors;

		public XMLDeclarationReport(IReportWriter writer, IManufacturerReportFactory mrfFactory, ICustomerInformationFileFactory cifFactory) : base(writer)
		{
			_mrfFactory = mrfFactory;
			_cifFactory = cifFactory;
		}

		protected XMLDeclarationReport(IReportWriter writer, bool dummy) : base(writer) { }

		public class ResultEntry : IResultEntry
		{
			public const double BEGIN_OF_LIFE_DETERIORATION = 0;
			public const double END_OF_LIFE_DETERIORATION = 0.1;

			public ResultEntry()
			{
				Distance = double.MaxValue.SI<Meter>();
			}

			public void Initialize(VectoRunData runData)
			{
				Mission = runData.Mission.MissionType;
				LoadingType = runData.Loading;
				FuelMode = runData.EngineData?.FuelMode ?? 0;
				FuelData = runData.EngineData?.Fuels.Select(x => x.FuelData).ToList() ?? new List<IFuelProperties>();
				Payload = runData.VehicleData.Loading;
				TotalVehicleMass = runData.VehicleData.TotalVehicleMass;
				CargoVolume = runData.VehicleData.CargoVolume;
				VehicleClass = runData.Mission?.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass;
				PassengerCount = runData.VehicleData.PassengerCount;
				MaxChargingPower = runData.MaxChargingPower;
				BatteryData = runData.BatteryData;
				OVCMode = runData.OVCMode;
				OVCIteration = runData.Iteration;
				VectoRunData = runData;
			}

			public void Initialize(VectoRunData runData, IModalDataContainer modalData)
			{
				Mission = runData.Mission.MissionType;
				LoadingType = runData.Loading;
				FuelMode = runData.EngineData?.FuelMode ?? 0;
				Payload = runData.VehicleData.Loading;
				TotalVehicleMass = runData.VehicleData.TotalVehicleMass;
				CargoVolume = runData.VehicleData.CargoVolume;
				VehicleClass = runData.Mission?.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass;
				PassengerCount = runData.VehicleData.PassengerCount;
				MaxChargingPower = runData.MaxChargingPower;
				BatteryData = runData.BatteryData;
				OVCMode = runData.OVCMode;
				VectoRunData = runData;
				OVCIteration = runData.Iteration;

				FuelData = runData.JobType.IsFCHV() ?
					runData.FuelCellSystemData?.Fuel :
					runData.EngineData?.Fuels.Select(x => x.FuelData).ToList() ?? new List<IFuelProperties>();

				SetResultData(runData, modalData, 0.0);
			}

			public VectoRunData VectoRunData { get; private set; }

			public MissionType Mission { get; private set; }
			public LoadingType LoadingType { get; private set; }
			public int FuelMode { get; private set; }
			public IList<IFuelProperties> FuelData { get; set; }


			public Kilogram Payload { get; set; }

			public Kilogram TotalVehicleMass { get; private set; }

			public CubicMeter CargoVolume { get; private set; }

			public double? PassengerCount { get; set; }
			public VehicleClass VehicleClass { get; set; }

			public VehicleClass? PrimaryVehicleClass => throw new NotImplementedException();

			public Watt MaxChargingPower { get; set; }

			public MeterPerSecond AverageSpeed { get; private set; }

			public MeterPerSecond AverageDrivingSpeed { get; private set; }

			public Joule EnergyConsumptionTotal { get; private set; }

            public IFuelConsumptionCorrection FuelConsumptionFinal(FuelType fuelType)
			{
				return CorrectedFinalFuelConsumption.ContainsKey(fuelType) ?  CorrectedFinalFuelConsumption[fuelType] : null;
			}

			public WattSecond ElectricEnergyConsumption { get; private set; }

			public Kilogram CO2Total { get; private set; }

			public Dictionary<FuelType, IFuelConsumptionCorrection> CorrectedFinalFuelConsumption { get; private set; }

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
			public BatterySystemData BatteryData { get; private set; }

			public PerSecond EngineSpeedDrivingMin { get; private set; }
			public PerSecond EngineSpeedDrivingAvg { get; private set; }
			public PerSecond EngineSpeedDrivingMax { get; private set; }

			public double AverageGearboxEfficiency { get; private set; }

			public double AverageAxlegearEfficiency { get; private set; }

			public double WeightingFactor { get; private set; }

			public Meter ActualChargeDepletingRange { get; set; }

			public Meter EquivalentAllElectricRange { get; set; }

			public Meter ZeroCO2EmissionsRange { get; set; }

            public Meter HydrogenRange { get; set; }

			public DeclarationData.ElectricRangesPEV BeginOfLifeRanges { get; private set; }

            public DeclarationData.ElectricRangesPEV EndOfLifeRanges { get; private set; }

            public IFuelProperties AuxHeaterFuel { get; set; }

			public Kilogram ZEV_FuelConsumption_AuxHtr { get; set; }

			public Kilogram ZEV_CO2 { get; set; }

			public OvcHevMode OVCMode { get; set; }

			public int OVCIteration { get; private set; }

            public double DeltaSoC { get; private set; }

			// used for factor method
            public IResult PrimaryResult { get; set; }

			public double BatteryEfficiencyDischarge { get; set; }

            public virtual void SetResultData(VectoRunData runData, IModalDataContainer data, double weightingFactor)
			{
				OVCMode = runData.OVCMode;
				OVCIteration = runData.Iteration;
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

				var entriesDriving = data.HasCombustionEngine
					? data.GetValues(
						r => new {
							dt = r.Field<Second>(ModalResultField.simulationInterval.GetName()),
							v = r.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
							nEng = r.Field<PerSecond>(ModalResultField.n_ice_avg.GetName())
						}).Where(x => x.v.IsGreater(0)).ToArray()
					: null;
				if (entriesDriving?.Length > 0) {
					var drivingTime = entriesDriving.Sum(x => x.dt);

					AverageDrivingSpeed = entriesDriving.Sum(x => x.v * x.dt) / drivingTime;
					EngineSpeedDrivingAvg = (entriesDriving.Sum(x => (x.nEng * x.dt).Value()) / drivingTime.Value())
						.SI<PerSecond>();
					EngineSpeedDrivingMin = entriesDriving.Min(x => x.nEng);
					EngineSpeedDrivingMax = entriesDriving.Max(x => x.nEng);
				} else {
					AverageDrivingSpeed = 0.KMPHtoMeterPerSecond();
					EngineSpeedDrivingAvg = 0.RPMtoRad();
					EngineSpeedDrivingMax = 0.RPMtoRad();
					EngineSpeedDrivingMin = 0.RPMtoRad();
				}

				Distance = data.Distance;

				CorrectedFinalFuelConsumption = data.CorrectedModalData.FuelCorrection;
				CO2Total = data.CorrectedModalData.CO2Total;
				EnergyConsumptionTotal = data.CorrectedModalData.FuelEnergyConsumptionTotal;
				ElectricEnergyConsumption = data.CorrectedModalData.ElectricEnergyConsumption_Final;

				if (runData.JobType.IsBatteryElectric())
				{
					var ranges = DeclarationData.CalculateElectricRangesPEV(runData, data);
					ActualChargeDepletingRange = ranges.ActualChargeDepletingRange;
					EquivalentAllElectricRange = ranges.EquivalentAllElectricRange;
					ZeroCO2EmissionsRange = ranges.ZeroCO2EmissionsRange;
					ElectricEnergyConsumption = ranges.ElectricEnergyConsumption;
			
					var fc = data.CorrectedModalData.FuelCorrection.Values.FirstOrDefault();
                    if (fc != null)
                    {
                        ZEV_FuelConsumption_AuxHtr = fc.FC_AUXHTR_KM * Distance;
                        AuxHeaterFuel = fc.Fuel;
                    }
				}

				if (runData.EngineData?.Fuels.Any(x => x.FuelData.FuelType.IsHydrogenFuel()) ?? false)
				{
					var h2Fuel = runData.EngineData?.Fuels.Where(x => x.FuelData.FuelType.IsHydrogenFuel()).Select(x => x.FuelData.FuelType).First();
					var totalFc = FuelConsumptionFinal(h2Fuel.Value)?.TotalFuelConsumptionCorrected;
					
					var range = ((totalFc != null) && (totalFc > 0))
						? Distance * (runData.VehicleData.H2StorageUsableCapacity / totalFc)
						: null;

					HydrogenRange = range;
					ZeroCO2EmissionsRange = range;
                }

				if (runData.JobType.IsFCHV())
				{
					var totalFc = FuelConsumptionFinal(FuelType.H2FC)?.TotalFuelConsumptionCorrected;

                    var range = ((totalFc != null) && (totalFc > 0))
                        ? Distance * (runData.VehicleData.H2StorageUsableCapacity / totalFc)
                        : null;

                    HydrogenRange = range;
                    ZeroCO2EmissionsRange = range;

                    BeginOfLifeRanges = DeclarationData.CalculateElectricRangesFCHV(runData, data, BEGIN_OF_LIFE_DETERIORATION);
                    EndOfLifeRanges = DeclarationData.CalculateElectricRangesFCHV(runData, data, END_OF_LIFE_DETERIORATION);
                    ElectricEnergyConsumption = (BeginOfLifeRanges.ElectricEnergyConsumption + EndOfLifeRanges.ElectricEnergyConsumption) / 2.0;
                }

                if (data.HasGearbox && !runData.JobType.IsOneOf(VectoSimulationJobType.IEPC_E, VectoSimulationJobType.IEPC_S, VectoSimulationJobType.FCHV_IEPC)) {
					var gbxOutSignal = runData.Retarder.Type == RetarderType.TransmissionOutputRetarder
						? ModalResultField.P_retarder_in
						: (runData.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
					var eGbxIn = data.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
					var eGbxOut = data.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
					AverageGearboxEfficiency = eGbxOut.Value() / eGbxIn.Value();
				} else {
					AverageGearboxEfficiency = double.NaN;
				}

				if (data.HasAxlegear) {
					var eAxlIn = data.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
					var eAxlOut = data.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
					AverageAxlegearEfficiency = eAxlOut == null || eAxlIn == null ? double.NaN : eAxlOut.Value() / eAxlIn.Value();
				} else {
					AverageAxlegearEfficiency = double.NaN;
				}

				DeltaSoC = OVCMode == OvcHevMode.ChargeSustaining ? data.REESSDeltaSoc() : 0;
				
				WeightingFactor = weightingFactor;
				PrimaryResult = runData.PrimaryResult;

				if (data.HasBattery) {
					BatteryEfficiencyDischarge = data.BatteryEfficiencyDischarge();

                }
			}

			public void SetResultWeightingFactor(double weightingFactor)
			{
				WeightingFactor = weightingFactor;
			}

			public IResultEntry Clone(OvcHevMode ovcMode)
			{
				return new ResultEntry()
				{
					OVCMode = ovcMode,
					VectoRunData = VectoRunData,
					Mission = Mission,
					LoadingType = LoadingType,
					FuelMode = FuelMode,
					FuelData = FuelData,
					Payload = Payload,
					TotalVehicleMass = TotalVehicleMass,
					CargoVolume = CargoVolume,
					PassengerCount = PassengerCount,
					VehicleClass = VehicleClass,
					MaxChargingPower = MaxChargingPower,
					AverageSpeed = AverageSpeed,
					AverageDrivingSpeed = AverageDrivingSpeed,
					EnergyConsumptionTotal = EnergyConsumptionTotal,
					ElectricEnergyConsumption = ElectricEnergyConsumption,
					CO2Total = CO2Total,
					CorrectedFinalFuelConsumption = CorrectedFinalFuelConsumption,
					Distance = Distance,
					GearshiftCount = GearshiftCount,
					FullLoadPercentage = FullLoadPercentage,
					MaxDeceleration = MaxDeceleration,
					MaxAcceleration = MaxAcceleration,
					MaxSpeed = MaxSpeed,
					MinSpeed = MinSpeed,
					Error = Error,
					Status = Status,
					StackTrace = StackTrace,
					BatteryData = BatteryData,
					EngineSpeedDrivingMin = EngineSpeedDrivingMin,
					EngineSpeedDrivingAvg = EngineSpeedDrivingAvg,
					EngineSpeedDrivingMax = EngineSpeedDrivingMax,
					AverageGearboxEfficiency = AverageGearboxEfficiency,
					AverageAxlegearEfficiency = AverageAxlegearEfficiency,
					WeightingFactor = WeightingFactor,
					ActualChargeDepletingRange = ActualChargeDepletingRange,
					EquivalentAllElectricRange = EquivalentAllElectricRange,
					ZeroCO2EmissionsRange = ZeroCO2EmissionsRange,
					HydrogenRange = HydrogenRange,
					AuxHeaterFuel = AuxHeaterFuel,
					ZEV_FuelConsumption_AuxHtr = ZEV_FuelConsumption_AuxHtr,
					ZEV_CO2 = ZEV_CO2,
					PrimaryResult = PrimaryResult,
					BatteryEfficiencyDischarge = BatteryEfficiencyDischarge,
					BeginOfLifeRanges = BeginOfLifeRanges,
					EndOfLifeRanges = EndOfLifeRanges
				};
			}
		}



		public virtual XDocument FullReport => ManufacturerRpt.Report;

		public virtual XDocument CustomerReport => CustomerRpt?.Report;

		public virtual XDocument MonitoringReport => _monitoringReport.Report;

		public virtual XDocument PrimaryVehicleReport => null;


		protected override void DoStoreResult(ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			var factor = _weightingFactors[Tuple.Create(runData.Mission.MissionType, runData.Loading)];
			entry.SetResultData(runData, modData, factor);
		}

		protected override void WriteResult(ResultEntry result)
		{
			var sumWeightingFactors = _weightingFactors.Values.Sum(x => x);
			bool isNormalWeights = sumWeightingFactors.IsEqual(0) || sumWeightingFactors.IsEqual(1, 1e-12);
			bool isVocationalWeights = sumWeightingFactors.IsEqual(2.0, 1e-12); ;
			if (!isNormalWeights && !isVocationalWeights) {
				throw new VectoException("Mission Profile Weighting factors or Mission Profile Weighting factors for Vocational misisons do not sum up to 1!");
			}

			ManufacturerRpt.WriteResult(result);
			CustomerRpt?.WriteResult(result);
		}

		protected override void GenerateReports()
		{
			ManufacturerRpt.GenerateReport();
			var fullReportHash = GetSignature(ManufacturerRpt.Report);
			CustomerRpt?.GenerateReport(fullReportHash);
			_monitoringReport.GenerateReport();
		}

		public override void SetWeightingFactors(VectoRunData runData, IEnumerable<IResultEntry> orderedeResults, double? electricRange)
		{
			WeightingGroup = DeclarationData.WeightingGroup.Lookup(
				runData.VehicleData.VehicleClass,
				runData.VehicleData.SleeperCab.Value,
				DeclarationData.GetReferencePropulsionPower(runData.VehicleData.InputData),
				runData.JobType.IsBatteryElectric(),
				electricRange);

			_weightingFactors = WeightingGroup == WeightingGroup.Unknown
				? ZeroWeighting
				: DeclarationData.WeightingFactors.Lookup(WeightingGroup);

			foreach(var result in orderedeResults)
			{
				result.SetResultWeightingFactor(_weightingFactors[Tuple.Create(result.Mission, result.LoadingType)]);
			}
		}

		protected override void OutputReports()
		{
			if (CustomerReport != null) {
				Writer.WriteReport(ReportType.DeclarationReportCustomerXML, CustomerRpt.Report);
			}

			Writer.WriteReport(ReportType.DeclarationReportManufacturerXML, ManufacturerRpt.Report);
			Writer.WriteReport(ReportType.DeclarationReportMonitoringXML, _monitoringReport.Report);
		}


		protected XElement GetSignature(XDocument report)
		{
			return report.XPathSelectElement("/*[local-name()='VectoOutput']/*[local-name()='Signature']/*");
		}


		public override void InitializeReport(VectoRunData modelData)
		{
			if (modelData.Exempted)
			{
				WeightingGroup = WeightingGroup.Unknown;
			}
			else
			{
				if (modelData.VehicleData.SleeperCab == null)
				{
					throw new VectoException("SleeperCab parameter is required");
				}

				var propulsionPower = DeclarationData.GetReferencePropulsionPower(modelData.VehicleData.InputData);
				WeightingGroup = DeclarationData.WeightingGroup.Lookup(
					modelData.VehicleData.VehicleClass,
					modelData.VehicleData.SleeperCab.Value,
					propulsionPower);
			}

			_weightingFactors = WeightingGroup == WeightingGroup.Unknown
				? ZeroWeighting
				: DeclarationData.WeightingFactors.Lookup(WeightingGroup);

			InstantiateReports(modelData);

			ManufacturerRpt.Initialize(modelData);
			CustomerRpt?.Initialize(modelData);
			_monitoringReport.Initialize(modelData);
		}

		public WeightingGroup WeightingGroup { get; protected set; }

		protected virtual void InstantiateReports(VectoRunData modelData)
		{

			var vehicleData = modelData.VehicleData.InputData;
			var iepc = vehicleData.Components?.IEPC != null;
			var ihpc =
				vehicleData.Components?.ElectricMachines?.Entries?.Count(e => e.ElectricMachine.IHPCType != "None") > 0;

			ManufacturerRpt = _mrfFactory.GetManufacturerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
				vehicleData.ExemptedVehicle,
				iepc,
				ihpc);

			CustomerRpt = _cifFactory.GetCustomerReport(vehicleData.VehicleCategory,
				vehicleData.VehicleType,
				vehicleData.ArchitectureID,
                vehicleData.ExemptedVehicle,
				iepc,
				ihpc);

			_monitoringReport = new XMLMonitoringReport(ManufacturerRpt);	
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


		public static IEnumerable<XElement> GetResults(IResultEntry result, XNamespace tns, bool fullOutput)
		{
			//var fuel = result.FuelData;
			var retVal = new List<XElement>();

			foreach (var fuel in result.FuelData) {
				var entry = result.FuelConsumptionFinal(fuel.FuelType);
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
