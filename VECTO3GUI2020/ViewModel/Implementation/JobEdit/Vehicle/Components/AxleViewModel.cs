using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AxleViewModel : ViewModelBase, IComponentViewModel, IAxleViewModel
    {
        public string Name => "Axle Wheels";

        public abstract string Version { get; }

        public bool IsPresent => true;

        protected IXMLAxleDeclarationInputData _inputData;

		private bool _twinTyres;

        public bool TwinTyres { get => _twinTyres; set => SetProperty(ref _twinTyres, value); }



        private AxleType _axleType;
        public AxleType AxleType { get => _axleType; set => SetProperty(ref _axleType, value); }

		private int _axleNumber;
		public int AxleNumber { get => _axleNumber;
			set => SetProperty(ref _axleNumber, value);
		}
		public DataSource DataSource => _dataSource;

		private ITyreViewModel _tyreViewModel;
		public ITyreViewModel TyreViewModel { 
			get => _tyreViewModel;
			set => SetProperty(ref _tyreViewModel, value);
		}
		ITyreDeclarationInputData IAxleDeclarationInputData.Tyre => _tyreViewModel;

        private bool _steered;
		private DataSource _dataSource;
		public bool Steered { get => _steered; set => SetProperty(ref _steered, value); }



        public AxleViewModel(IXMLAxleDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
        {
            _inputData = inputData;
            Debug.Assert(_inputData != null);

            _axleType = _inputData.AxleType;
            _twinTyres = _inputData.TwinTyres;
            _axleNumber = 0; //_inputData.AxleNumber;
			_steered = false; //_inputData.Steered;
			_tyreViewModel = (ITyreViewModel)vmFactory.CreateComponentViewModel(_inputData.Tyre);
		}

        public override string ToString()
        {
            return _inputData.AxleType.ToString();
        }
    }


    public class AxleViewModel_v1_0 : AxleViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAxleDataProviderV10).FullName;
        

        public AxleViewModel_v1_0(IXMLAxleDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {
            

        }

        public override string Version => VERSION;
    }

    public class AxleViewModel_v2_0 : AxleViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAxleDataProviderV20).FullName;
        public AxleViewModel_v2_0(IXMLAxleDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }
    }
}
