using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class PTOViewModel : ViewModelBase, IComponentViewModel, IPTOViewModel
    {

		
        protected IPTOTransmissionInputData _inputData;

		protected PTOTransmission ptoTransmission = new PTOTransmission();

		private bool _isPresent = true;
        public bool IsPresent { get => _isPresent; set => SetProperty(ref _isPresent, value); }

		private static readonly string _name = "PTO";
		public string Name { get { return _name; } }

		protected abstract void SetProperties();
        protected PTOViewModel(IXMLPTOTransmissionInputData inputData)
        {
            _inputData = inputData;
            _ptoTransmissionType= _inputData.PTOTransmissionType;
			if(_ptoTransmissionType == "None")
            {
                _isPresent = false;
                return;
            }

			ptoTransmission.GetTechnologies();

			SetProperties();
		}

		
		#region implementation of IPTOTransmissionInputData

		protected string _ptoTransmissionType;
		protected TableData _ptoLossMap;
		protected TableData _ptoCycle;
        public virtual string PTOTransmissionType {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual TableData PTOLossMap {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public TableData PTOCycleDuringStop { get; }
		public TableData PTOCycleWhileDriving { get; }
		public virtual TableData PTOCycle {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual DataView PTOLossMapDataView
		{
			get => PTOLossMap?.DefaultView;
		}

		public virtual DataView PTOCycleDataView
		{
			get => PTOCycle?.DefaultView;
		}

#endregion
    }

    public class PTOViewModel_V1_0 : PTOViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationPTODataProviderV10).FullName;

		public PTOViewModel_V1_0(IXMLPTOTransmissionInputData inputData) : base(inputData)
        {

        }

		protected override void SetProperties()
		{
			_ptoTransmissionType = _inputData.PTOTransmissionType;
			_ptoCycle = _inputData.PTOCycleDuringStop;
			_ptoLossMap = _inputData.PTOLossMap;
		}

		public override string PTOTransmissionType
		{
			get => _ptoTransmissionType;
			set => SetProperty(ref _ptoTransmissionType, value);
		}

		public override TableData PTOLossMap
		{
			get => _ptoLossMap;
			set => SetProperty(ref _ptoLossMap, value);
		}

		public override TableData PTOCycle
		{
			get => _ptoCycle;
			set => SetProperty(ref _ptoCycle, value);
		}
	}

    public class PTOViewModel_V2_0 : PTOViewModel_V1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationPTODataProviderV20).FullName;
        public PTOViewModel_V2_0(IXMLPTOTransmissionInputData inputData) : base(inputData)
        {
            
        }

		protected override void SetProperties()
		{
			base.SetProperties();
		}
	}
}
