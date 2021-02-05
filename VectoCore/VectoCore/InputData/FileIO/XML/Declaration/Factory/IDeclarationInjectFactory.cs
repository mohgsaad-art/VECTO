using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory
{
	public interface IDeclarationInjectFactory
	{
		/*
		 * Ninject automatically creates a factory class for this interface
		 * 
		 * the first argument is used to lookup the named binding for the required return type, the remaining
		 * parameters are the constructor arguments
		 * 
		 */

		IXMLDeclarationInputData CreateInputProvider(string version, XmlDocument xmlDoc, string fileName);

		IXMLDeclarationTrailerInputData CreateInputProviderTrailer(string version, XmlDocument xmlDoc, string fileName);

		IXMLPrimaryVehicleBusInputData CreatePrimaryVehicleBusInputProvider(string version, XmlDocument xmlDoc, string fileName);


		IXMLDeclarationJobInputData CreateJobData(
			string version, XmlNode node, IXMLDeclarationInputData inputProvider, string fileName);

		IXMLDeclarationTrailerJobInputData CreateJobDataTrailer(
			string version, XmlNode node, IXMLDeclarationTrailerInputData inputProvider, string fileName);

		IXMLPrimaryVehicleBusJobInputData CreatePrimaryVehicleJobData(
			string version, XmlNode node, IXMLPrimaryVehicleBusInputData inputProvider, string fileName);



		IXMLDeclarationVehicleData CreateVehicleData(
			string version, IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile);

		IXMLTrailerDeclarationVehicleData CreateTrailerVehicleData(
			string version, IXMLDeclarationTrailerJobInputData jobData, XmlNode xmlNode, string sourceFile);

		IXMLDeclarationVehicleData CreatePrimaryVehicleData(
			string version, IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode xmlNode, string sourceFile);



		IXMLVehicleComponentsDeclaration CreateComponentData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);


		IXMLAirdragDeclarationInputData CreateAirdragData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLGearboxDeclarationInputData CreateGearboxData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLGearData CreateGearData(string version, XmlNode gearNode, string sourceFile);


		IXMLTorqueConverterDeclarationInputData CreateTorqueconverterData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAxleGearInputData CreateAxlegearData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAngledriveInputData CreateAngledriveData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLEngineDeclarationInputData CreateEngineData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLRetarderInputData CreateRetarderData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAuxiliariesDeclarationInputData CreateAuxiliariesData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLPTOTransmissionInputData CreatePTOData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAxlesDeclarationInputData CreateAxleWheels(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);
		IXMLAxlesDeclarationInputData CreateTrailerAxleWheels(string version, IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);
		IXMLAxleDeclarationInputData CreateAxleData(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);
		IXMLAxleDeclarationInputData CreateTrailerAxleData(string version, IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLTyreDeclarationInputData CreateTyre(string version, XmlNode componentNode, string sourceFile);

		IXMLTyreDeclarationInputData CreateTrailerTyre(string version, XmlNode componentNode, string sourceFile);

		IXMLAuxiliaryDeclarationInputData CreateAuxiliaryData(string version, XmlNode auxNode, IXMLDeclarationVehicleData vehicle);

		IXMLAdvancedDriverAssistantSystemDeclarationInputData CreateADASData(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLBusAuxiliariesDeclarationData CreateBusAuxiliaires(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);


		// ---------------------------------------------------------------------------------------------

		IXMLDeclarationInputDataReader CreateInputReader(
			string version, IXMLDeclarationInputData inputData, XmlNode baseNode);

		IXMLDeclarationTrailerInputDataReader CreateInputReaderTrailer(
			string version, IXMLDeclarationTrailerInputData inputData, XmlNode baseNode);

		IXMLDeclarationPrimaryVehicleBusInputDataReader CreatePrimaryVehicleBusInputReader(
			string version, IXMLPrimaryVehicleBusInputData inputData, XmlNode baseNode);


		IXMLJobDataReader CreateJobReader(
			string version, IXMLDeclarationJobInputData jobData, XmlNode jobNode);

		IXMLJobTrailerDataReader CreateJobTrailerReaderTrailer(
			string version, IXMLDeclarationTrailerJobInputData jobData, XmlNode jobNode);

		IXMLJobDataReader CreatePrimaryVehicleJobReader(
			string version, IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode jobNode);


		IXMLComponentReader CreateComponentReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLComponentReader CreateTrailerComponentReader(
			string version, IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLADASReader CreateADASReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode);

		IXMLPTOReader CreatePTOReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode);

		IXMLAxlesReader CreateAxlesReader(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLAxlesReader CreateTrailerAxlesReader(string version, IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLAxleReader CreateAxleReader(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLAxleReader CreateTrailerAxleReader(string version, IXMLTrailerDeclarationVehicleData vehicle, XmlNode componentsNode);
		IXMLGearboxReader CreateGearboxReader(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode);
		IXMLAuxiliaryReader CreateAuxiliariesReader(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode);

		IXMLApplicationInformationData CreateApplicationInformationReader(string version, XmlNode applicationNode);

		IXMLResultsInputData CreateResultsInputDataReader(string version, XmlNode resultsNode);

		IXMLComponentInputData CreateComponentInputProvider(string version, XmlDocument xmlDoc, string fileName);
	}

}