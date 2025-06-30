using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter
{
    public class VIFErrorResultWriter : ErrorResultWriter
	{
		public VIFErrorResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of ErrorResultWriter

		protected override string ResultXMLType => null;

		#endregion
	}

	public class ResultSimulationParameterVIFBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterVIFBusWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					entry.TotalVehicleMass.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_Payload,
					entry.Payload.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
			);
		}

		#endregion
	}

	public class VIFResultErrorMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public VIFResultErrorMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_PrimaryVehicleSubgroup, entry.VehicleClass.ToXML()),
				new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat())
			};
		}

		[ExcludeFromCodeCoverage] // no ovc results in VIF
		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class VIFResultSimulationParameterErrorWriter : AbstractResultGroupWriter
	{
		public VIFResultSimulationParameterErrorWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_ResultEntry_PassengerCount,
					entry.PassengerCount?.ToXMLFormat(2) ?? "NaN")
				);
		}

		#endregion
	}


	public class VIFFuelConsumptionWriter : FuelConsumptionWriterBase
    {
		public VIFFuelConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override string FCElementName { get; } = XMLNames.Report_Result_EnergyConsumption;

		public override IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc, IFuelProperties fuel,
			Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
		{
			return new List<ConvertedSI> {
				(fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
			};
		}

		public override IList<ConvertedSI> GetFuelConsumptionEntries(KilogramPerMeter fcPerMeter, IFuelProperties fuel, Kilogram payload, CubicMeter volume, double? passenger)
		{
			JoulePerMeter fcPerMeterlowerHeatingValue = (fcPerMeter.Value() * fuel.LowerHeatingValueVecto.Value()).SI<JoulePerMeter>();
			return new List<ConvertedSI> {
				(fcPerMeterlowerHeatingValue).ConvertToMegaJoulePerKilometer(),
			};
		}
	}

	public class VIFElectricEnergyConsumptionWriter : ElectricEnergyConsumptionWriterBase
	{
		public VIFElectricEnergyConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of ElectricEnergyConsumptionWriterBase

		protected override string ElectricEnergyConsumptionXMLElementName => XMLNames.Report_ResultEntry_VIF_ElectricEnergyConsumption;


		protected override IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
			Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(elEnergy / distance).ConvertToMegaJoulePerKiloMeter(),
			};
		}

		#endregion
	}

	public class VIFResultSuccessMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public VIFResultSuccessMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_PrimaryVehicleSubgroup, entry.VehicleClass.ToXML()),
				new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat())
			};
		}

		[ExcludeFromCodeCoverage] // no ovc results in VIF
		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class VIFErrorDetailsWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public VIFErrorDetailsWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultSequenceWriter

		public XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Error, entry.Error),
				new XElement(TNS + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
			};
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}