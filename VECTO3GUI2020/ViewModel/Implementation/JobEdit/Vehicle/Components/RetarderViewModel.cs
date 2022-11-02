using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class RetarderViewModel : ViewModelBase, IComponentViewModel, IRetarderViewModel
    {
        private static readonly string _name = "Retarder";
        public string Name { get { return _name; } }

        protected IRetarderInputData _inputData;


        private ICommonComponentViewModel _commonComponentViewModel;


		public ICommonComponentViewModel CommonComponentViewModel { get
                => _commonComponentViewModel;
            set { SetProperty(ref _commonComponentViewModel, value); }
        }

		private bool _isPresent = false;
        public bool IsPresent { get => _isPresent; set => SetProperty(ref _isPresent, value); }

		public DataView LossMapDataView => _lossMap?.DefaultView;


		public abstract void SetProperties();



        public RetarderViewModel(IXMLRetarderInputData inputData, IComponentViewModelFactory vmFactory)
        {
            _inputData = inputData as IXMLRetarderInputData;
            Debug.Assert(_inputData != null);
            IsPresent = _inputData?.DataSource.SourceFile != null;

			

			CommonComponentViewModel = vmFactory.CreateCommonComponentViewModel(inputData);

			

			SetProperties();
		}

		protected RetarderType _type;
		protected double _ratio;
		protected TableData _lossMap;

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

		public virtual RetarderType Type {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public virtual double Ratio {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public virtual TableData LossMap {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual DataTable LossMapDataTable
		{
			get => (DataTable)LossMap;
		}
	}

    public class RetarderViewModel_v1_0 : RetarderViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationRetarderDataProviderV10).FullName;

		public RetarderViewModel_v1_0(IXMLRetarderInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {
            

        }

		public override void SetProperties()
		{
			if (!IsPresent) {
				
				_ratio = 0;
			} else {
				_lossMap = _inputData.LossMap;
				_ratio = _inputData.Ratio;
			}
			_type = _inputData.Type;
		}

		public override RetarderType Type
		{
			get => _type;
			set
			{
				IsPresent = (value != RetarderType.None);
				SetProperty(ref _type, value);
			}
		}

		public override double Ratio
		{
			get => _ratio;
			set => SetProperty(ref _ratio, value);
		}

		public override TableData LossMap
		{
			get => _lossMap;
			set => SetProperty(ref _lossMap, value);
		}
	}

    public class RetarderViewModel_v2_0 : RetarderViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationRetarderDataProviderV20).FullName;

        public RetarderViewModel_v2_0(IXMLRetarderInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			base.SetProperties();
		}
	}
}
