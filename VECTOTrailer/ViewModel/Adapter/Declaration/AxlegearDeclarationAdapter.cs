using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter.Declaration {
	public class AxlegearDeclarationAdapter : AbstractDeclarationAdapter, IAxleGearInputData
	{
		protected IAxlegearViewModel ViewModel;

		public AxlegearDeclarationAdapter(IAxlegearViewModel axlegearViewModel):base(axlegearViewModel)
		{
			ViewModel = axlegearViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		
		#endregion

		#region Implementation of IAxleGearInputData

		public double Ratio { get { return ViewModel.Ratio; } }
		public TableData LossMap { get { return TableDataConverter.Convert(ViewModel.LossMap); } }
		public double Efficiency { get { throw new NotImplementedException();} }
		public AxleLineType LineType { get { return ViewModel.LineType; } }

		#endregion
	}
}