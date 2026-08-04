using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IElectricStorageAdapter
    {
        BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData,
            VectoSimulationJobType jobType,
            bool ovc,
            bool batteryOnlyMode,
            double deterioration = DeclarationData.Battery.GenericDeterioration);

        SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData);
    }
}