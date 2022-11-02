using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxElectricSystemSupplyGroupWriter_v2_10_2 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxElectricSystemSupplyGroupWriter_v2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
