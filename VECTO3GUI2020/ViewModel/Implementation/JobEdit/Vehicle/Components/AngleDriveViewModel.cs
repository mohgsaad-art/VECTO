using System;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AngleDriveViewModel : ViewModelBase, IComponentViewModel, IAngleDriveViewModel
    {
		protected IXMLAngledriveInputData _inputData;

        private ICommonComponentViewModel _commonComponentViewModel;

        public ICommonComponentViewModel CommonComponentViewModel
        {
            get => _commonComponentViewModel;
			set => SetProperty(ref _commonComponentViewModel, value);
		}

		public string Name => "Angle Drive";

		private bool _isPresent;
		public bool IsPresent { get => _isPresent; set => SetProperty(ref _isPresent, value); }

		public abstract void SetProperties();

        public AngleDriveViewModel(IXMLAngledriveInputData inputData, IComponentViewModelFactory viewModelFactory)
        {
            _inputData = inputData;
			if(((inputData as IXMLResource)?.DataSource.SourceFile == null) || (_inputData.Type == AngledriveType.None)) {
				IsPresent = false;
			}

            _commonComponentViewModel = viewModelFactory.CreateCommonComponentViewModel(inputData);
		}

		#region implementation of IAngleDriveInputData

		protected AngledriveType _type;
		protected double _ratio;
		protected TableData _lossMap;
		protected double _efficiency;
		public DataSource DataSource
		{
			get => _commonComponentViewModel.DataSource;
			set => _commonComponentViewModel.DataSource = value;
		}

		public string Manufacturer
		{
			get => _commonComponentViewModel.Manufacturer;
			set => _commonComponentViewModel.Manufacturer = value;
		}

		public string Model
		{
			get => _commonComponentViewModel.Model;
			set => _commonComponentViewModel.Model = value;
		}

		public DateTime Date
		{
			get => _commonComponentViewModel.Date;
			set => _commonComponentViewModel.Date = value;
		}

		public string CertificationNumber
		{
			get => _commonComponentViewModel.CertificationNumber;
			set => _commonComponentViewModel.CertificationNumber = value;
		}

		public CertificationMethod CertificationMethod
		{
			get => _commonComponentViewModel.CertificationMethod;
			set => _commonComponentViewModel.CertificationMethod = value;
		}

		public bool SavedInDeclarationMode
		{
			get => _commonComponentViewModel.SavedInDeclarationMode;
			set => _commonComponentViewModel.SavedInDeclarationMode = value;
		}

		public DigestData DigestValue
		{
			get => _commonComponentViewModel.DigestValue;
			set => _commonComponentViewModel.DigestValue = value;
		}

		public string AppVersion => _commonComponentViewModel.AppVersion;

		public virtual AngledriveType Type
		{
			get => _type;
			set{
				Debug.WriteLine(value.ToString());
				SetProperty(ref _type, value);
				IsPresent = value != AngledriveType.None;
			} 
		}

		public virtual double Ratio {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual TableData LossMap {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual double Efficiency {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		#endregion
	}


    public class AngleDriveViewModel_v1_0 : AngleDriveViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAngledriveDataProviderV10).FullName;

		public AngleDriveViewModel_v1_0(IXMLAngledriveInputData inputData, IComponentViewModelFactory viewModelFactory) : base(inputData, viewModelFactory)
        {
        }

		public override void SetProperties()
		{
			_type = _inputData.Type;
			_ratio = _inputData.Ratio;
			_lossMap = _inputData.LossMap;
			_efficiency = _inputData.Efficiency;
		}

		public override double Ratio
		{
			get => _ratio;
			set => SetProperty(ref _ratio, value);
		}

		public override TableData LossMap
		{
			get => _lossMap;
			set => SetProperty(ref _lossMap , value);
		}

		public override double Efficiency
		{
			get => _efficiency;
			set => SetProperty(ref _efficiency, value);
		}
	}

    public class AngleDriveViewModel_v2_0 : AngleDriveViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAngledriveDataProviderV20).FullName;

        public AngleDriveViewModel_v2_0(IXMLAngledriveInputData inputData, IComponentViewModelFactory viewModelFactory) : base(inputData, viewModelFactory)
        {
        }

		public override void SetProperties()
		{
			base.SetProperties();
		}
	}




}

