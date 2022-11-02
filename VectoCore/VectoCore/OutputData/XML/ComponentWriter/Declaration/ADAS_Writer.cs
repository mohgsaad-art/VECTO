using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
	public interface IDeclarationAdasWriter
	{
		XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas);

		XElement GetComponent(IAdvancedDriverAssistantSystemDeclarationInputData adas);
	}

    public class AdasConventionalWriter : ComponentWriter, IDeclarationAdasWriter
    {
		public AdasConventionalWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IADASWriter

		public XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var elements = new List<XElement>();
			
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRollWithOutEngineStop()));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRollWithEngineStop()));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_PCC,adas.PredictiveCruiseControl.ToXMLFormat()));
			if (adas.ATEcoRollReleaseLockupClutch != null) {
				elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, adas.ATEcoRollReleaseLockupClutch));
			}

			return elements.ToArray();
		}

		public XElement GetComponent(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return new XElement(_writerNamespace + XMLNames.Vehicle_ADAS,
				new XAttribute(XMLDeclarationNamespaces.Xsi + XMLNames.Attr_Type, "ADAS_Conventional_Type"),
				GetComponentElements(adas));
		}

		#endregion
	}
}
