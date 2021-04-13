using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader
{
	public interface IXMLComponentReader
	{
		IVehicleComponentsDeclaration ComponentInputData { get; }

		IAirdragDeclarationInputData AirdragInputData { get; }
		IGearboxDeclarationInputData GearboxInputData { get; }
		IAxleGearInputData AxleGearInputData { get; }
		IAngledriveInputData AngledriveInputData { get; }
		IEngineDeclarationInputData EngineInputData { get; }
		IAuxiliariesDeclarationInputData AuxiliaryData { get; }
		IRetarderInputData RetarderInputData { get; }
		IAxlesDeclarationInputData AxlesDeclarationInputData { get; }

		ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }

		IBusAuxiliariesDeclarationData BusAuxiliariesInputData { get; }
	}

	public interface IXMLAxlesReader
	{
		IAxleDeclarationInputData CreateAxle(XmlNode axleNode);
	}

	public interface IXMLTrailerAxlesReader
	{
		ITrailerAxleDeclarationInputData CreateAxle(XmlNode axleNode);
	}


	public interface IXMLAxleReader
	{
		ITyreDeclarationInputData Tyre { get; }
	}

	public interface IXMLTrailerAxleReader
	{
		ITyreDeclarationInputData Tyre { get; }
	}

	public interface IXMLGearboxReader
	{
		ITransmissionInputData CreateGear(XmlNode gearNode);
	}

	public interface IXMLAuxiliaryReader
	{
		IAuxiliaryDeclarationInputData CreateAuxiliary(XmlNode auxNode);

	}
}
