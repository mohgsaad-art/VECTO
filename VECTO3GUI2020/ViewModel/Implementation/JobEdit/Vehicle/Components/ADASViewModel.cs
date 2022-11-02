using System;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class ADASViewModel : ViewModelBase, IComponentViewModel, IAdasViewModel
    {
        public string Name => "ADAS";

        public bool IsPresent { get; set; } = false;

		protected IAdvancedDriverAssistantSystemDeclarationInputData _inputData;


        public ADASViewModel(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			_inputData = inputData;
			IsPresent = true;

			
			if (!IsPresent)
            {
                return;
            }
            SetProperties();
		}

		public abstract void SetProperties();

		#region implementation of IAdvancedDriverAssistantSystemDeclarationInputData
		protected bool _engineStopStart;
		protected EcoRollType _ecoRoll;
		protected PredictiveCruiseControlType _predictiveCruiseControl;
		protected bool? _atEcoRollReleaseLockupClutch;
		protected XmlNode _xmlSource;
        public virtual bool EngineStopStart {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual EcoRollType EcoRoll {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual PredictiveCruiseControlType PredictiveCruiseControl {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual bool? ATEcoRollReleaseLockupClutch {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual XmlNode XMLSource {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion
    }


    public class ADASViewModel_v1_0 : ADASViewModel
    {
        
        public static readonly string VERSION = typeof(XMLDeclarationADASDataProviderV10).FullName;

		public ADASViewModel_v1_0(IAdvancedDriverAssistantSystemDeclarationInputData inputData) : base(inputData)
        {
            
        }

		public override void SetProperties()
		{
			_engineStopStart = _inputData.EngineStopStart;
			_ecoRoll = _inputData.EcoRoll;
			_predictiveCruiseControl = _inputData.PredictiveCruiseControl;
		}

#region implementation Overrides of Properties
        public override bool EngineStopStart
		{
			get => _engineStopStart;
			set => SetProperty(ref _engineStopStart, value);
		}
		public override EcoRollType EcoRoll
		{
			get => _ecoRoll;
			set => SetProperty(ref _ecoRoll, value);
		}
		public override PredictiveCruiseControlType PredictiveCruiseControl
		{
			get => _predictiveCruiseControl;
			set => SetProperty(ref _predictiveCruiseControl, value);
		}
#endregion
    }


    public class ADASViewModel_v2_1 : ADASViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationADASDataProviderV21).FullName;
        public ADASViewModel_v2_1(IAdvancedDriverAssistantSystemDeclarationInputData inputData) : base(inputData)
        {
            
        }

		public override void SetProperties()
		{
			base.SetProperties();
		}
	}

}
