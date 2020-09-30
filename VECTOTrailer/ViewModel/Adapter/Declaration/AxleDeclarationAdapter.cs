using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter.Declaration {
	public class AxleDeclarationAdapter : IAxleDeclarationInputData
	{
		protected IAxleViewModel ViewModel;
		public AxleDeclarationAdapter(IAxleViewModel axleViewModel)
		{
			ViewModel = axleViewModel;
		}

		#region Implementation of IAxleDeclarationInputData

		public bool TwinTyres { get { return ViewModel.TwinTyres; } }
		public bool Steered { get { return ViewModel.Steered; } }
		public AxleType AxleType { get { return ViewModel.AxleType; } }
		public ITyreDeclarationInputData Tyre { get { return ViewModel.Tyre.ModelData; } }
		public DataSource DataSource { get; }

		#endregion
	}
}