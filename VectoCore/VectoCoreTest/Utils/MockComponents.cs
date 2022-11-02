using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockComponents : IVehicleComponentsDeclaration
	{
		public IAirdragDeclarationInputData AirdragInputData { get; set; }

		public IGearboxDeclarationInputData GearboxInputData { get; set; }

		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get; set; }

		public IAxleGearInputData AxleGearInputData { get; set; }

		public IAngledriveInputData AngledriveInputData { get; set; }

		public IEngineDeclarationInputData EngineInputData { get; set; }

		public IAuxiliariesDeclarationInputData AuxiliaryInputData { get; set; }

		public IRetarderInputData RetarderInputData { get; set; }

		public IPTOTransmissionInputData PTOTransmissionInputData { get; set; }

		public IAxlesDeclarationInputData AxleWheels { get; set; }

		public IBusAuxiliariesDeclarationData BusAuxiliaries { get; }

		public IElectricStorageSystemDeclarationInputData ElectricStorage { get; }

		public IElectricMachinesDeclarationInputData ElectricMachines { get; }

		public IIEPCDeclarationInputData IEPC { get; }
	}
}