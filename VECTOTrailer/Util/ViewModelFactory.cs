using System;
using System.Collections.Generic;
using VECTOTrailer.ViewModel.Interfaces;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.Util
{
	public static class ViewModelFactory
	{

		public static Dictionary<Component, Type> ComponentViewModelMapping = new Dictionary<Component, Type>()
		{
			{ Component.CompleteTrailer, typeof(ICompleteTrailerViewModel)},
			{ Component.Cycle ,typeof(ICyclesViewModel) },

		};

		//public static IComponentViewModel CreateViewModel(IKernel kernel, Component component, DeclarationJobViewModel jobEditViewModel)
		//{
		//	throw new System.NotImplementedException();
		//}
	}
}
