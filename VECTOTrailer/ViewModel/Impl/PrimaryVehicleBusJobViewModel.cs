using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.InputData;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Impl
{
	public class PrimaryVehicleBusJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{

		public PrimaryVehicleBusJobViewModel(IKernel kernel, IPrimaryVehicleInformationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.PrimaryBusVehicle);
			CurrentComponent = GetComponentViewModel(Component.PrimaryBusVehicle);
		}


		protected override void DoSaveJob(Window window)
		{
			throw new NotImplementedException();
		}

		protected override void DoCloseJob(Window window)
		{
			throw new NotImplementedException();
		}

		protected override void DoSaveAsJob(Window window)
		{
			throw new NotImplementedException();
		}

		public string JobFile
		{
			get { return string.Empty; }
		}
		public IInputDataProvider InputDataProvider { get; set; }
		public ICommand ValidateInput { get; }
		public ICommand ShowValidationErrors { get; }
		public ICommand RemoveValidationErrors { get; }
	}
}
