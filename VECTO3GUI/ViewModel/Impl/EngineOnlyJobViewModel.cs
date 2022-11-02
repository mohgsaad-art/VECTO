using System.Windows;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class EngineOnlyJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		private IEngineeringInputDataProvider _inputData;

		public EngineOnlyJobViewModel(IKernel kernel, IEngineeringInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.Engine);
			CreateComponentModel(Component.Cycle);
			CurrentComponent = GetComponentViewModel(Component.Engine);
		}

		
		#region Implementation of IJobEditViewModel

		public string JobFile
		{
			get { return _inputData.JobInputData.JobName; }
		}

		public IInputDataProvider InputDataProvider
		{
			get { return _inputData; }
			set {
				value.Switch()
					.If<IEngineeringInputDataProvider>(
						d => {
							SetProperty(ref IsDeclarationMode, false);
							SetProperty(ref _inputData, d);
						}
					);
			}
		}

		public ICommand ValidateInput { get; }
		public ICommand ShowValidationErrors { get; }
		public ICommand RemoveValidationErrors { get; }

		#endregion

		#region Overrides of AbstractJobViewModel

		protected override void DoSaveJob(Window window)
		{
			throw new System.NotImplementedException();
		}

		protected override void DoCloseJob(Window window)
		{
			throw new System.NotImplementedException();
		}

		protected override void DoSaveAsJob(Window window)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
