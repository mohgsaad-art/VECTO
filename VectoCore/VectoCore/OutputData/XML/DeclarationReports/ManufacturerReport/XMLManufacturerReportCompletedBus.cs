using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport {
	public class XMLManufacturerReportCompletedBus : AbstractXMLManufacturerReport
	{
		protected TankSystem? _tankSystem;

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleRecordFile { get; set; }

		public IVehicleDeclarationInputData PrimaryVehicle => PrimaryVehicleRecordFile.Vehicle;


		#region Overrides of AbstractXMLManufacturerReport

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_tankSystem = modelData.VehicleData.InputData.TankSystem;
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleCompletedBusType"),
				GetPrimaryVehicleInformation(),
				new XElement(
					tns + "CompletedVehicle",
					new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
					new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
					new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
					new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
					new XElement(tns + XMLNames.Vehicle_VehicleCategory, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Vehicle_RegisteredClass, modelData.VehicleData.RegisteredClass.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_VehicleCode, modelData.VehicleData.VehicleCode.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
					new XElement(tns + XMLNames.TPMLM,
								XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 2)),
					new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.ZeroEmissionVehicle),
					new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
					new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
					new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),
					new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.DualFuelVehicle),
					
					new XElement(tns + "RegisteredPassengers",
						new XElement(tns + XMLNames.Bus_LowerDeck, modelData.VehicleData.InputData.NumberPassengerSeatsLowerDeck),
						new XElement(tns + XMLNames.Bus_UpperDeck, modelData.VehicleData.InputData.NumberPassengerSeatsUpperDeck)
					),
					new XElement(tns +XMLNames.Bus_LowEntry, modelData.VehicleData.InputData.LowEntry),
					new XElement(tns + XMLNames.Bus_HeightIntegratedBody, modelData.VehicleData.InputData.Height.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Bus_VehicleLength, modelData.VehicleData.InputData.Length.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Bus_VehicleWidth, modelData.VehicleData.InputData.Width.ToXMLFormat(3)),
					new XElement(tns + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology, modelData.VehicleData.InputData.DoorDriveTechnology.ToXMLFormat()),
					
					VehicleComponents(modelData, fuelModes),
					GetInputDataSignature(modelData)
				)
			);
		}

		protected XElement GetPrimaryVehicleInformation()
		{
			return new XElement(
				tns + "PrimaryVehicle",
				new XElement(tns + XMLNames.Component_Manufacturer, PrimaryVehicle.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, PrimaryVehicle.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, PrimaryVehicle.AxleConfiguration.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_InputDataSignature, PrimaryVehicleRecordFile.PrimaryVehicleInputDataHash.ToXML(di)),
				new XElement(tns + "ManufacturerRecordSignature", PrimaryVehicleRecordFile.ManufacturerRecordHash.ToXML(di)),
				new XElement(tns + "VehicleInformationSignature", CreateDummySig()) // PrimaryVehicleRecordFile.XMLHash)
            );
		}

		public virtual void WriteResult(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;
			Results.Add(
				genericResult.Status == VectoRun.Status.Success && specificResult.Status == VectoRun.Status.Success
					? GetSuccessResultEntry(genericResult, specificResult, primaryResult)
					: GetErrorResultEntry(genericResult, specificResult, primaryResult));
		}

		private XElement GetErrorResultEntry(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{

			object[] content = null;
			if (genericResult.Status == VectoRun.Status.Pending || genericResult.Status == VectoRun.Status.Running ||
				specificResult.Status == VectoRun.Status.Pending || specificResult.Status == VectoRun.Status.Running) {
				content =  new object[] {
					new XElement(
						tns + XMLNames.Report_Results_Error,
						$"Simulation not finished! Status: {genericResult.Status} / {specificResult.Status}"),
					new XElement(tns + XMLNames.Report_Results_ErrorDetails, ""),
				}; // should not happen!
			}

			if (genericResult.Status == VectoRun.Status.Canceled || genericResult.Status == VectoRun.Status.Aborted || specificResult.Status == VectoRun.Status.Canceled || specificResult.Status == VectoRun.Status.Aborted) {
				content = new object[] {
					new XElement(tns + XMLNames.Report_Results_Error, (genericResult.Error ?? "") + Environment.NewLine +  (specificResult.Error ?? "")),
					new XElement(tns + XMLNames.Report_Results_ErrorDetails, (genericResult.StackTrace ?? "") + Environment.NewLine + (specificResult.StackTrace ?? "")),
				};
			}

			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				new XElement(tns + XMLNames.Report_Result_Mission, genericResult.Mission.ToXMLFormat()),
				GetSimulationParameters(specificResult),
				content);
		}

		private XElement GetSuccessResultEntry(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultCompletedVehicleSuccessType"),
				new XElement(tns + XMLNames.Report_Result_Mission, genericResult.Mission.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_ResultEntry_Distance, XMLHelper.ValueAsUnit(specificResult.Distance, XMLNames.Unit_km, 3)),
				GetSimulationParametersPrimaryVehicle(primaryResult),
				GetSimulationParameters(specificResult),
				GetVehiclePerformanceResults(specificResult),
				GetFuelConsumptionResults(genericResult, specificResult, primaryResult)
			);

		}

		private XElement GetVehiclePerformanceResults(XMLDeclarationReport.ResultEntry specificResult)
		{
			return new XElement(
				tns + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(
					tns + XMLNames.Report_ResultEntry_AverageSpeed,
					XMLHelper.ValueAsUnit(specificResult.AverageSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_AvgDrivingSpeed,
					XMLHelper.ValueAsUnit(specificResult.AverageDrivingSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MinSpeed, XMLHelper.ValueAsUnit(specificResult.MinSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxSpeed, XMLHelper.ValueAsUnit(specificResult.MaxSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxDeceleration,
					XMLHelper.ValueAsUnit(specificResult.MaxDeceleration, XMLNames.Unit_mps2, 2)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxAcceleration,
					XMLHelper.ValueAsUnit(specificResult.MaxAcceleration, XMLNames.Unit_mps2, 2)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage,
					specificResult.FullLoadPercentage.ToXMLFormat(2)),
				new XElement(tns + XMLNames.Report_ResultEntry_GearshiftCount, specificResult.GearshiftCount.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_EngineSpeedDriving,
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Min,
						XMLHelper.ValueAsUnit(specificResult.EngineSpeedDrivingMin, XMLNames.Unit_RPM, 1)),
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Avg,
						XMLHelper.ValueAsUnit(specificResult.EngineSpeedDrivingAvg, XMLNames.Unit_RPM, 1)),
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Max,
						XMLHelper.ValueAsUnit(specificResult.EngineSpeedDrivingMax, XMLNames.Unit_RPM, 1))
				),
				new XElement(
					tns + XMLNames.Report_Results_AverageGearboxEfficiency,
					XMLHelper.ValueAsUnit(specificResult.AverageGearboxEfficiency, XMLNames.UnitPercent, 2)),
				new XElement(
					tns + XMLNames.Report_Results_AverageAxlegearEfficiency,
					XMLHelper.ValueAsUnit(specificResult.AverageAxlegearEfficiency, XMLNames.UnitPercent, 2))
			);
		}

		private XElement GetSimulationParametersPrimaryVehicle(IResult primaryResult)
		{
			return new XElement(
				tns + "SimulationParametersPrimaryVehicle",
				new XElement(
					tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(primaryResult.SimulationParameter.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(primaryResult.SimulationParameter.Payload, XMLNames.Unit_kg)),
				new XElement(tns + "PassengerCount", primaryResult.SimulationParameter.PassengerCount.ToMinSignificantDigits(3, 1)),
				new XElement(tns + XMLNames.Report_Result_FuelMode, primaryResult.SimulationParameter.FuelMode)
			);
		}

		protected override XElement GetSimulationParameters(XMLDeclarationReport.ResultEntry result)
		{
			return new XElement(
				tns + "SimulationParametersCompletedVehicle",
				new XElement(
					tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				result.PassengerCount.HasValue && result.PassengerCount.Value > 0 ? new XElement(tns + "PassengerCount", result.PassengerCount.Value.ToMinSignificantDigits(3, 1)) : null
			);
		}

		protected internal static double CalculateFactorMethodFactor(IResult primaryResult,
			XMLDeclarationReport.ResultEntry specific, XMLDeclarationReport.ResultEntry generic)
		{
			return specific.EnergyConsumptionTotal.Value() / generic.EnergyConsumptionTotal.Value();
   //         var energyConsumptionPrimary = primaryResult.EnergyConsumption.Sum(x => x.Value);
			//var energyConsumptionCompeted = energyConsumptionPrimary +
			//								specific.EnergyConsumptionTotal / specific.Distance -
			//								generic.EnergyConsumptionTotal / generic.Distance;
			//return energyConsumptionCompeted / energyConsumptionPrimary;
		}

		private XElement[] GetFuelConsumptionResults(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			var factor = CalculateFactorMethodFactor(primaryResult, specificResult, genericResult);
			//var factor = specificResult.EnergyConsumptionTotal.Value() / genericResult.EnergyConsumptionTotal.Value();
			var retVal = new List<XElement>();

			var co2Sum = 0.SI<KilogramPerMeter>();
			//retVal.Add(new XElement(tns + "FuelConsumptionFactor", factor.ToMinSignificantDigits(4)));
			foreach (var entry in primaryResult.EnergyConsumption) {
				var fcEnergy = entry.Value * factor;  // J/m
				var fuelData = FuelData.Instance().Lookup(entry.Key, _tankSystem);
				var fcMass = fcEnergy / fuelData.LowerHeatingValueVecto; // kg/m
				co2Sum += fcMass * fuelData.CO2PerFuelWeight;

				var fcResult = new XElement(tns + XMLNames.Report_Results_Fuel, new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuelData.FuelType.ToXMLFormat()));
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						fcMass.ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 1)),
					//new XElement(
					//	tns + XMLNames.Report_Results_FuelConsumption,
					//	new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					//	(fcMass / specificResult.Payload)
					//	.ConvertToGrammPerTonKilometer().ToMinSignificantDigits(3, 1)),
					specificResult.CargoVolume > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / specificResult.CargoVolume)
							.Value().ToMinSignificantDigits(3, 1))
						: null,
					specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / specificResult.PassengerCount.Value).ToMinSignificantDigits(3, 1))
						: null,
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"),
						fcEnergy.ConvertToMegaJoulePerKilometer().ToMinSignificantDigits(3, 1)),
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/t-km"),
						(fcEnergy.ConvertToMegaJoulePerKilometer() / specificResult.Payload.ConvertToTon())
						.ToMinSignificantDigits(3, 1)),
					specificResult.CargoVolume > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/m³-km"),
							(fcEnergy.ConvertToMegaJoulePerKilometer() / specificResult.CargoVolume.Value()).ToMinSignificantDigits(3, 1))
						: null,
					specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0
						? new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/p-km"),
							(fcEnergy.ConvertToMegaJoulePerKilometer() / specificResult.PassengerCount.Value).ToMinSignificantDigits(3, 1))
						: null
				);
				if (fuelData.FuelDensity != null) {
					fcResult.Add(
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
							(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity * 100).Value().ToMinSignificantDigits(3, 1)),
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
							(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
							specificResult.Payload.ConvertToTon()).Value().ToMinSignificantDigits(3, 1)));
					if (specificResult.CargoVolume > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
								(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
								specificResult.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
					}
					if (specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0) {
						fcResult.Add(
							new XElement(
								tns + XMLNames.Report_Results_FuelConsumption,
								new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/p-km"),
								(fcMass.ConvertToGrammPerKiloMeter() / fuelData.FuelDensity /
								specificResult.PassengerCount.Value).Value().ToMinSignificantDigits(3, 1))
						);
					}
				}
				retVal.Add(fcResult);
			}

			//CO2
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					co2Sum.ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 2)));
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.Payload.ConvertToTon()).ToMinSignificantDigits(3, 2)));
			if (specificResult.CargoVolume > 0)
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.CargoVolume).Value().ToMinSignificantDigits(3, 2)));
			if (specificResult.PassengerCount.HasValue && specificResult.PassengerCount.Value > 0) {
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/p-km"),
						(co2Sum.ConvertToGrammPerKiloMeter() / specificResult.PassengerCount.Value).ToMinSignificantDigits(3, 2)));
			}

			return retVal.ToArray();
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + "type", "ComponentsCompletedBusType"),
				GetAirDragDescription(modelData.AirdragData),
				GetAuxiliariesDescription(modelData)
			);
		}

		

		protected override XElement GetAirDragDescription(AirdragData airdragData)
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
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, airdragData.DigestValueInput),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
			);
		}

		protected override XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var busAuxiliaries = modelData.BusAuxiliaries;
			var busAuxXML = busAuxiliaries.InputData.XMLSource;
			const string auxPrefix = "aux";
			
			if (busAuxiliaries.InputData is ConsolidatedBusAuxiliariesData) {
				var namespaceName = ((XmlElement)busAuxXML.FirstChild).NamespaceURI;
				return new XElement(
					tns + XMLNames.Component_Auxiliaries,
					new XAttribute(XNamespace.Xmlns + auxPrefix, namespaceName),
					new XAttribute(xsi + "type", $"{auxPrefix}:CompletedVehicleAuxiliaryDataDeclarationType"),
					XElement.Parse(busAuxXML.InnerXml).Elements()
				);
			}
			
			var ns = XNamespace.Get(busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Namespace);
			return new XElement(
				tns + XMLNames.Component_Auxiliaries,
				new XAttribute(XNamespace.Xmlns + auxPrefix, ns.NamespaceName),
				new XAttribute(xsi + "type", $"{auxPrefix}:{busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Name}"),
				XElement.Parse(busAuxXML.InnerXml).Elements()
			);
		}

		
		#endregion

		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			throw new NotSupportedException();
		}

		
	}
}