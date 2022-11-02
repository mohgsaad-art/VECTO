using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AxleWheelsViewModel : ViewModelBase, IComponentViewModel, IAxleWheelsViewModel
    {
        private static readonly string _name = "Axle Wheels";
        public string Name { get { return _name; } }
		private bool _isPresent = true;
		public bool IsPresent { get { return _isPresent; } }

        protected IXMLAxlesDeclarationInputData _inputData;



        public ObservableCollection<IComponentViewModel> AxleViewModels { get; set; } = new ObservableCollection<IComponentViewModel>();
        private IList<IAxleDeclarationInputData> _axlesInputData;
		private XmlNode _xmlSource;

		public AxleWheelsViewModel(IXMLAxlesDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
        {
            _inputData = inputData as IXMLAxlesDeclarationInputData;
            Debug.Assert(_inputData != null);
            _isPresent = (_inputData?.DataSource?.SourceFile != null);

            _axlesInputData = _inputData.AxlesDeclaration as IList<IAxleDeclarationInputData>;
            Debug.Assert(_axlesInputData != null);


            

            for (int i = 0; i < _axlesInputData.Count(); i++)
            {
               AxleViewModels.Add(vmFactory.CreateComponentViewModel(_axlesInputData[i]));
            }      
        }


		public IList<IAxleDeclarationInputData> AxlesDeclaration =>
			AxleViewModels.Cast<IAxleDeclarationInputData>().ToList();

		public int? NumSteeredAxles => null;

		public XmlNode XMLSource => _xmlSource;
	}

    public class AxleWheelsViewModel_v1_0 : AxleWheelsViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAxlesDataProviderV10).FullName;



        public AxleWheelsViewModel_v1_0(IXMLAxlesDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

            
        }
    }

    public class AxleWheelsViewModel_v2_0 : AxleWheelsViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAxlesDataProviderV20).FullName;

        public AxleWheelsViewModel_v2_0(IXMLAxlesDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }
    }
}
