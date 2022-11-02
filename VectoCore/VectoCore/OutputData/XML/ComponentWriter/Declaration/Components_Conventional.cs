using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter.Declaration
{
	public interface IComponentWriter
	{
		XElement[] GetComponentElements(IVehicleComponentsDeclaration components);

		XElement GetComponentsElement(IVehicleComponentsDeclaration components);
    } 

    class Components_Conventional_CompletedBus : ComponentWriter, IComponentWriter
    {
		public Components_Conventional_CompletedBus(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IComponentWriter

		public XElement[] GetComponentElements(IVehicleComponentsDeclaration components)
		{
			throw new NotImplementedException();
		}

		public XElement GetComponentsElement(IVehicleComponentsDeclaration components)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
