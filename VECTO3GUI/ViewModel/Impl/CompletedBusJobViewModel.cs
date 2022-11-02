using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompletedBusJobViewModel: AbstractBusJobViewModel
	{
		public CompletedBusJobViewModel(IKernel kernel, JobType jobType):base(kernel, jobType)
		{
			SetFirstFileLabel();
		}

		public CompletedBusJobViewModel(IKernel kernel, JobEntry jobEntry) : base(kernel, jobEntry)
		{
			SetFirstFileLabel();
		}
		
		protected sealed override void SetFirstFileLabel()
		{
			FirstLabelText = $"Select {JobFileType.PIFBusFile.GetLable()}";
		}
	}
}
