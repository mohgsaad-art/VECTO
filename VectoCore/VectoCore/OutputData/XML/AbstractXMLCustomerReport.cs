using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML
{ 
	public abstract class AbstractXMLCustomerReport
	{
		public XDocument Report { get; protected set; }

		public abstract void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);
		public abstract void GenerateReport(XElement resultSignature);
		public abstract void WriteResult(XMLDeclarationReport.ResultEntry resultEntry);
	}
}
