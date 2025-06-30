using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IEngineAuxiliary : IAuxInProvider, IAuxPort
	{
		void Add(string auxId, Func<PerSecond, Second, Second, bool, Watt> powerLossFunction, string columnName = null);

        void AddCycle(string cycle);

		void AddCycle(string cycle, Func<DrivingCycleData.DrivingCycleEntry, Watt> powerLossFunc,
			string columnName = null);

        void AddConstant(string id, Watt powerDemandMech, string columnName = null);
	}
}