using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class PTODataAdapterLorry : IPTODataAdapter
	{

        public virtual PTOData CreateDefaultPTOData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
		{
			return new PTOData()
			{
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false),
				TransmissionPowerDemand = DeclarationData.PTOTransmission.Lookup(DeclarationData.PTO.DefaultPTOTechnology).PowerDemand,
				ConsumerType = PTOConsumerType.mechanical,
			};
		}
        public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
		{
			if (pto != null && pto.PTOTransmissionType != "None"){
				var powerDemand = DeclarationData.PTOTransmission.Lookup(pto.PTOTransmissionType).PowerDemand;
				return new PTOData
				{
					TransmissionPowerDemand = powerDemand,
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
					ConsumerType = PTOConsumerType.mechanical,
				};
			}
			return null;
		}
	}


	public class ElectricPTODataAdapter : PTODataAdapterLorry
	{
		public override PTOData CreateDefaultPTOData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
		{
			Watt powerDemand = null;
			string transmissionType = null;
			if (pto != null && gbx != null && pto.PTOTransmissionType != "None") {
				powerDemand = DeclarationData.PTOTransmission.Lookup(pto.PTOTransmissionType).PowerDemand;
				transmissionType = pto.PTOTransmissionType;
			}


			return new PTOData()
			{
				TransmissionType = transmissionType,
				LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultE_PTOActivationCycle),
						CycleType.EPTO, "PTO", false),
				TransmissionPowerDemand = powerDemand,
				ConsumerType = PTOConsumerType.electrical,
			};
		}

		public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx)
		{

			if (gbx == null && pto != null && (pto.PTOTransmissionType != "None")) {
				throw new VectoException("PTO specified but the vehicle has no gearbox", ErrorCodes.PTO_MISSING_GEARBOX);
			} 
				
			if ((gbx != null) && (pto != null) && (pto.PTOTransmissionType != "None"))
			{
				var powerDemand = DeclarationData.PTOTransmission.Lookup(pto.PTOTransmissionType).PowerDemand;
				return new PTOData
				{
					TransmissionPowerDemand = powerDemand,
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
					ConsumerType = PTOConsumerType.electrical,
				};
			}

			return null;
		}
	}
}