using System;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class RetarderDeclarationAdapter : AbstractDeclarationAdapter, IRetarderInputData
	{
		protected IRetarderViewModel ViewModel;

		public RetarderDeclarationAdapter(IRetarderViewModel retarderViewModel) :base(retarderViewModel)
		{
			ViewModel = retarderViewModel;
		}

		#region Implementation of IComponentInputData

		
		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		
		#endregion

		#region Implementation of IRetarderInputData

		public RetarderType Type { get { return ViewModel.Type; } }
		public double Ratio { get { return ViewModel.Ratio; } }
		public TableData LossMap { get { return TableDataConverter.Convert(ViewModel.LossMap); } }

		#endregion
	}
}