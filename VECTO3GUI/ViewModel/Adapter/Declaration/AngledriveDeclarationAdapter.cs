using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class AngledriveDeclarationAdapter : AbstractDeclarationAdapter, IAngledriveInputData
	{
		protected IAngledriveViewModel ViewModel;
		public AngledriveDeclarationAdapter(IAngledriveViewModel angledriveViewModel) :base(angledriveViewModel)
		{
			ViewModel = angledriveViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		
		#endregion

		#region Implementation of IAngledriveInputData

		public AngledriveType Type { get { return ViewModel.AngledriveType; } }
		public double Ratio { get { return ViewModel.Ratio; } }

		public TableData LossMap
		{
			get { return TableDataConverter.Convert(ViewModel.LossMap); }
		}

		public double Efficiency { get { throw new NotImplementedException(); } }

		#endregion
	}
}