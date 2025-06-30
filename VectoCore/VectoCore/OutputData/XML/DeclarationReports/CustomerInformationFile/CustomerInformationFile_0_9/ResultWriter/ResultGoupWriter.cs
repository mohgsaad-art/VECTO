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
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.
    ResultWriter
{

	public class CIFResultMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public CIFResultMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] { new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()) };
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class ResultSimulationParameterCIFBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterCIFBusWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					entry.TotalVehicleMass.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_MassPassengers,
					entry.Payload.ValueAsUnit(XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
			);
		}

		#endregion
	}

	public class VehiclePerformanceCIFWriter : AbstractResultGroupWriter
	{
		public VehiclePerformanceCIFWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed,
				XMLHelper.ValueAsUnit(entry.AverageSpeed, "km/h", 1));
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed,
				XMLHelper.ValueAsUnit(entry.Weighted.AverageSpeed, "km/h", 1));
		}

		#endregion
	}

	

	public class CIFErrorDetailsWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public CIFErrorDetailsWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultSequenceWriter

		public XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Error, entry.Error),
				new XElement(TNS + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
			};
		}

		[ExcludeFromCodeCoverage] // OVC results are not written in case of an error
		public XElement[] GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

