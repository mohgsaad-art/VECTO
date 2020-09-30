using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.ViewModel.Adapter.Declaration;
using VECTOTrailer.ViewModel.Interfaces;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Impl {
	public class TyreDeclarationAdapter : AbstractDeclarationAdapter, ITyreDeclarationInputData
	{
		protected ITyreViewModel ViewModel;

		public TyreDeclarationAdapter(ITyreViewModel tyreViewModel) :base(tyreViewModel)
		{
			ViewModel = tyreViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return CertificationMethod.Measured; } }
		
		#endregion

		#region Implementation of ITyreDeclarationInputData

		public string Dimension { get { return ViewModel.Dimension; } }
		public double RollResistanceCoefficient { get { return ViewModel.RollingResistanceCoefficient; } }
		public Newton TyreTestLoad { get { return ViewModel.FzISO; } }
		public string FuelEfficiencyClass { get; }

		#endregion
	}
}