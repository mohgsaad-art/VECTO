using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter.Declaration;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public class DeclarationJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		private IDeclarationInputDataProvider _inputData;

		
		public DeclarationJobViewModel(IKernel kernel, IDeclarationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.Vehicle);
			CreateComponentModel(Component.Cycle);
			CurrentComponent = GetComponentViewModel(Component.Vehicle);
		}

		#region Implementation of IJobEditViewModel

		public string JobFile
		{
			get { return _inputData.JobInputData.JobName; }
		}

		
		#endregion

		public IDeclarationInputDataProvider ModelData
		{
			get { return new DeclarationJobAdapter(this); }
		}


		protected override void DoSaveJob(Window window)
		{
			var writer = new XMLDeclarationWriter("TEST");
			var tmp = writer.GenerateVectoJob(ModelData);
		}

		protected override void DoCloseJob(Window window)
		{
			throw new NotImplementedException();
		}

		protected override void DoSaveAsJob(Window window)
		{
			throw new NotImplementedException();
		}


		public IInputDataProvider InputDataProvider
		{
			get { return _inputData; }
			set {
				value.Switch()
					.If<IDeclarationInputDataProvider>(
						d => {
							SetProperty(ref IsDeclarationMode, true);
							SetProperty(ref _inputData, d);
						}
					);
			}
		}

		public ICommand ValidateInput { get; }
		public ICommand ShowValidationErrors { get; }
		public ICommand RemoveValidationErrors { get; }
	}
}
