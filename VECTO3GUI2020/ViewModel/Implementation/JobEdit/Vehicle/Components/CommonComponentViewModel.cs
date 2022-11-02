using Ninject;
using System;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public class CommonComponentViewModel : ViewModelBase, ICommonComponentViewModel
    {
        private string _manufacturer = "No Manufacturer found";
        private string _model = "No Model found";
		private DateTime _date = new DateTime(1985, 10, 26, 01, 21, 00);
		private DataSource _dataSource;
		private bool _savedInDeclarationMode;
		private string _appVersion;
		private CertificationMethod _certificationMethod;
		private string _certificationNumber;
		private DigestData _digestValue;

		#region Implementation of IComponentInputData


		public string Manufacturer { get => _manufacturer; set => SetProperty(ref _manufacturer, value); }
        public string Model { get => _model; set => SetProperty(ref _model,  value); }
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

		public DataSource DataSource
		{
			get { return _dataSource; }
			set { SetProperty(ref _dataSource, value); }
		}

		public bool SavedInDeclarationMode
		{
			get => _savedInDeclarationMode;
			set => SetProperty(ref _savedInDeclarationMode, value);
		}

		public string AppVersion
		{
			get => _appVersion;
			set => SetProperty(ref _appVersion, value);
		}

		public CertificationMethod CertificationMethod
		{
			get => _certificationMethod;
			set => SetProperty(ref _certificationMethod, value);
		}

		public string CertificationNumber
		{
			get => _certificationNumber;
			set => SetProperty(ref _certificationNumber, value);
		}

		public DigestData DigestValue
		{
			get => _digestValue;
			set => SetProperty(ref _digestValue, value);
		}

		#endregion

		[Inject]
        public CommonComponentViewModel(IComponentInputData inputData) {
            Debug.Assert(inputData != null);
			try {
				_manufacturer = inputData.Manufacturer;
				_model = inputData.Model;
				_date = inputData.Date;
				_dataSource = inputData.DataSource;
				_savedInDeclarationMode = inputData.SavedInDeclarationMode;
				_appVersion = inputData.AppVersion;
				_certificationMethod = inputData.CertificationMethod;
				_certificationNumber = inputData.CertificationNumber;
				_digestValue = inputData.DigestValue;
	
			} catch (Exception e){
				//TODO ignoring for now
			}
		}
	}
}
