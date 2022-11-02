using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AuxiliariesViewModel : ViewModelBase, IComponentViewModel, IAuxiliariesViewModel
    {
        private static readonly string _name = "Auxiliaries";
        public string Name { get { return _name; } }

		protected IXMLAuxiliariesDeclarationInputData _inputData;

		protected IComponentViewModelFactory _componentViewModelFactory;
        private bool _isPresent = true;
        private ObservableCollection<IAuxiliaryViewModel> _auxiliaryViewModels = new ObservableCollection<IAuxiliaryViewModel>();


		public bool IsPresent { get { return _isPresent; } }

        public ObservableCollection<IAuxiliaryViewModel> AuxiliaryViewModels { 
            get => _auxiliaryViewModels; 
            set => SetProperty(ref _auxiliaryViewModels, value); }

		public abstract void CreateAuxiliaries();

        protected AuxiliariesViewModel(IXMLAuxiliariesDeclarationInputData inputData, IComponentViewModelFactory componentViewModelFactory)
        {
            _inputData = inputData;
			_componentViewModelFactory = componentViewModelFactory;

            Debug.Assert(_inputData != null);
            CreateAuxiliaries();


        }

		private bool _savedInDeclarationMode;
		private IList<IAuxiliaryDeclarationInputData> _auxiliaries;
        public virtual bool SavedInDeclarationMode => throw new NotImplementedException();

		public virtual IList<IAuxiliaryDeclarationInputData> Auxiliaries => _auxiliaryViewModels.Cast<IAuxiliaryDeclarationInputData>().ToList();
	}

    public class AuxiliariesViewModel_v1_0 : AuxiliariesViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAuxiliariesDataProviderV10).FullName;

		public AuxiliariesViewModel_v1_0(IXMLAuxiliariesDeclarationInputData inputData, IComponentViewModelFactory componentViewModelFactory) : base(inputData, componentViewModelFactory)
        {
        }

		public override void CreateAuxiliaries()
		{
			foreach (var auxiliary in _inputData.Auxiliaries)
			{
				Debug.Assert(auxiliary.Technology.Count == 1);
				var componentViewModel = _componentViewModelFactory.CreateComponentViewModel(auxiliary as IXMLAuxiliaryDeclarationInputData);
				AuxiliaryViewModels.Add(componentViewModel as IAuxiliaryViewModel);
			}
        }
	}

    public class AuxiliariesViewModel_v2_0 : AuxiliariesViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAuxiliariesDataProviderV20).FullName;

        public AuxiliariesViewModel_v2_0(IXMLAuxiliariesDeclarationInputData inputData, IComponentViewModelFactory componentViewModelFactory) : base(inputData, componentViewModelFactory)
        {
        }

		public override void CreateAuxiliaries()
		{
			base.CreateAuxiliaries();
		}
	}


}
