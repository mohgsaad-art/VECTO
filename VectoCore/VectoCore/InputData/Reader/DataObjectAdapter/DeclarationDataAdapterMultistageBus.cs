using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;


namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterMultistageBus : DeclarationDataAdapterCompletedBusSpecific
	{

		public override IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission,
			IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
			VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			return new AuxiliaryConfig
			{
				InputData = completedVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = CreateElectricsUserInputsConfig(
					primaryVehicle, completedVehicle, mission, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputs = GetCompletedSSMInput(mission, completedVehicle, primaryVehicle, runData.Loading),
				VehicleData = runData.VehicleData
			};
		}

		protected override ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, completedVehicle, actuations, vehicleClass);

			// add electrical steering pump or electric fan defined in primary vehicle
			foreach (var entry in GetElectricAuxConsumersPrimary(mission, completedVehicle, vehicleClass, primaryVehicle.Components.BusAuxiliaries))
			{
				currentDemand[entry.Key] = entry.Value;
			}

			var retVal = GetDefaultElectricalUserConfig();

			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;
			retVal.AlternatorType = primaryBusAuxiliaries.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = currentDemand;

			retVal.AlternatorMap = new SimpleAlternator(
				CalculateAlternatorEfficiency(primaryBusAuxiliaries.ElectricSupply.Alternators));

			switch (retVal.AlternatorType) {
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = primaryBusAuxiliaries.ElectricSupply.Alternators.Sum(x => x.RatedVoltage * x.RatedCurrent);
			retVal.ElectricStorageCapacity = primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Sum(x => x.ElectricStorageCapacity) ?? 0.SI<WattSecond>();

			return retVal;
		}
	}
}
