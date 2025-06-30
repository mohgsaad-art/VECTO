using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockBatteryInputData : IElectricStorageSystemEngineeringInputData
	{
		protected internal IList<IElectricStorageDeclarationInputData> _electricStorageElements = new List<IElectricStorageDeclarationInputData>();

		public IREESSPackInputData REESSPack
		{
			set
			{
				_electricStorageElements.Add(new MockElectricStorageInputWrapper() {
					REESSPack = value,
					StringId = 0,
					Count = 1,
				});
			}
		}

		#region Implementation of IElectricStorageSystemDeclarationInputData

		IList<IElectricStorageDeclarationInputData> IElectricStorageSystemDeclarationInputData.ElectricStorageElements => _electricStorageElements;

		#endregion

		#region Implementation of IElectricStorageSystemEngineeringInputData

		IList<IElectricStorageEngineeringInputData> IElectricStorageSystemEngineeringInputData.ElectricStorageElements => _electricStorageElements.Cast<IElectricStorageEngineeringInputData>().ToList();

		#endregion

		public class MockElectricStorageInputWrapper : IElectricStorageDeclarationInputData, IElectricStorageEngineeringInputData
		{
			#region Implementation of IElectricStorageDeclarationInputData

			public IREESSPackInputData REESSPack { get; set; }
			public int Count { get; set; }
			public int StringId { get; set; }

			#endregion
		}

	}
}