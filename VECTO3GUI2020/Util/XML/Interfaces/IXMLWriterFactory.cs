using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using VECTO3GUI2020.Util.XML.Implementation.ComponentWriter;

namespace VECTO3GUI2020.Util.XML.Interfaces
{
    public interface IXMLWriterFactory
	{
		IXMLDeclarationJobWriter CreateJobWriter(IDeclarationJobInputData inputData);
		IXMLVehicleWriter CreateVehicleWriter(IVehicleDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IComponentInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAuxiliariesDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAxlesDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAxleDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAirdragDeclarationInputData inputData);

		IXMLComponentWriter CreateComponentWriter(IPTOTransmissionInputData inputData);
		IXMLComponentsWriter CreateComponentsWriter(IVehicleComponentsDeclaration inputData);

		IXMLBusAuxiliariesWriter CreateBuxAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData);
	}
}
