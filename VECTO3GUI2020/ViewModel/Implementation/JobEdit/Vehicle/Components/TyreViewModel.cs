using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class TyreViewModel : ViewModelBase, IComponentViewModel, ITyreViewModel
    {

		public string Name => "Tyre View Model";

		public bool IsPresent => true;


		protected IXMLTyreDeclarationInputData _inputData;

        private ICommonComponentViewModel _commonComponentViewModel;


		public ICommonComponentViewModel CommonComponentViewModel { 
            get => _commonComponentViewModel;
            set => SetProperty(ref _commonComponentViewModel, value);
        }

        private static Wheels _wheels = new Wheels();

		public static ObservableCollection<string> AllowedDimensions { get; } = new ObservableCollection<string>(new Wheels().GetWheelsDimensions());

		public abstract void SetProperties();

		public TyreViewModel(IXMLTyreDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
		{
			_inputData = inputData as IXMLTyreDeclarationInputData;
			Debug.Assert(_inputData != null);

			CommonComponentViewModel = vmFactory.CreateCommonComponentViewModel(_inputData);


			SetProperties();
		}


		protected string _dimension;
		protected double _rollResistanceCoefficient;
		protected Newton _tyreTestLoad;
		protected string _fuelEfficiencyClass;

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

		public virtual string Dimension {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual double RollResistanceCoefficient {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual Newton TyreTestLoad {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual string FuelEfficiencyClass {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


	}


    public class TyreViewModel_v1_0 : TyreViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationTyreDataProviderV10).FullName;


		public TyreViewModel_v1_0(IXMLTyreDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {
            

        }

		public override void SetProperties()
		{ 
			_dimension = _inputData.Dimension;
			_rollResistanceCoefficient = _inputData.RollResistanceCoefficient;
			_tyreTestLoad = _inputData.TyreTestLoad;
		}

		public override string Dimension
		{
			get => _dimension;
			set => SetProperty(ref _dimension, value);
		}

		public override double RollResistanceCoefficient
		{
			get => _rollResistanceCoefficient;
			set => SetProperty(ref _rollResistanceCoefficient, value);
		}

		public override Newton TyreTestLoad
		{
			get => _tyreTestLoad;
			set => SetProperty(ref _tyreTestLoad, value);
		}

	}


    public class TyreViewModel_v2_0 : TyreViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationTyreDataProviderV20).FullName;
        public TyreViewModel_v2_0(IXMLTyreDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {
            
        }

		public override void SetProperties()
		{
			base.SetProperties();
		}
	}

    public class TyreViewModel_v2_2 : TyreViewModel_v2_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationTyreDataProviderV22).ToString();
        public TyreViewModel_v2_2(IXMLTyreDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			base.SetProperties();
		}
	}

    public class TyreViewModel_v2_3 : TyreViewModel_v2_2
    {
        public static new readonly string VERSION = typeof(XMLDeclarationTyreDataProviderV23).ToString();

			
		private string _tyreClass;
		public  string TyreClass
        {
            get { return _tyreClass; }
            set { SetProperty(ref _tyreClass, value); }
        }


        public override string FuelEfficiencyClass
        {
            get { return _fuelEfficiencyClass; }
            set { SetProperty(ref _fuelEfficiencyClass, value); }
        }



        public TyreViewModel_v2_3(IXMLTyreDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {
		}

		public override void SetProperties()
		{
			base.SetProperties();
			_tyreClass = "TODO: TyreClass";
			_fuelEfficiencyClass = _inputData.FuelEfficiencyClass;
		}
	}
}
