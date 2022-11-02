using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Utils {
	class MockElectricMachinesInputData  : IElectricMachinesEngineeringInputData
	{
		IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> IElectricMachinesDeclarationInputData.Entries
		{
			get;
		}

		public IList<ElectricMachineEntry<IElectricMotorEngineeringInputData>> Entries
		{
			get;
			set;
		}
	}
}