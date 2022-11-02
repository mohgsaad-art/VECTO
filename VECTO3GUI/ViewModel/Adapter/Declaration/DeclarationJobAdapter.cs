using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class DeclarationJobAdapter : IDeclarationInputDataProvider, IDeclarationJobInputData
	{
		protected readonly DeclarationJobViewModel Model;
		public DeclarationJobAdapter(DeclarationJobViewModel declarationJobViewModel)
		{
			Model = declarationJobViewModel;
		}

		#region Implementation of IDeclarationInputDataProvider

		public IDeclarationJobInputData JobInputData
		{
			get { return this; }
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }

		public XElement XMLHash { get { return null; } }

		#endregion

		#region Implementation of IDeclarationJobInputData

		public bool SavedInDeclarationMode { get { return Model.DeclarationMode; } }

		public IVehicleDeclarationInputData Vehicle
		{
			get {
				var vehiclevm = Model.GetComponentViewModel(Component.Vehicle) as IVehicleViewModel;
				return vehiclevm?.ModelData;
			}
		}

		public string JobName { get { return Model.JobFile; } }
		public string ShiftStrategy { get; }

		#endregion

		#region Implementation of IInputDataProvider

		public DataSource DataSource { get; }

		#endregion
	}
}