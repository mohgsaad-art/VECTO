using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration
{
	public class AirdragDeclarationAdapter : AbstractDeclarationAdapter, IAirdragDeclarationInputData
	{
		protected IAirdragViewModel ViewModel;

		public AirdragDeclarationAdapter(IAirdragViewModel viewModel) : base(viewModel)
		{
			ViewModel = viewModel;
		}

		#region Implementation of IComponentInputData
		
		public DataSource DataSource { get; }

		public DateTime Date { get; }
		public string AppVersion { get; }

		public CertificationMethod CertificationMethod
		{
			get { return ViewModel.UseMeasuredValues ? CertificationMethod.Measured : CertificationMethod.StandardValues; }
		}

		#endregion

		#region Implementation of IAirdragDeclarationInputData

		public SquareMeter AirDragArea
		{
			get { return ViewModel.UseMeasuredValues ? ViewModel.DeclaredCdxA : null; }
		}

		public SquareMeter TransferredAirDragArea
		{
			get { return ViewModel.UseMeasuredValues ? ViewModel.TransferredCdxA : null; }
		}

		public SquareMeter AirdragArea_0
		{
			get { return ViewModel.UseMeasuredValues ? ViewModel.CdxA_0 : null; }
		}

		#endregion
	}
}
