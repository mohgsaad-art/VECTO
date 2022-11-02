using System;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class TorqueConverterDeclarationAdapter : AbstractDeclarationAdapter, ITorqueConverterDeclarationInputData
	{
		protected ITorqueConverterViewModel ViewModel;

		public TorqueConverterDeclarationAdapter(ITorqueConverterViewModel torqueConverterViewModel):base(torqueConverterViewModel)
		{
			ViewModel = torqueConverterViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		

		#endregion

		#region Implementation of ITorqueConverterDeclarationInputData

		public TableData TCData { get { return TableDataConverter.Convert(ViewModel.Characteristics); } }

		#endregion
	}
}