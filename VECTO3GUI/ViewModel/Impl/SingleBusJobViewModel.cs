using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace VECTO3GUI.ViewModel.Impl
{
	public class SingleBusJobViewModel : AbstractBusJobViewModel
	{
		public SingleBusJobViewModel(IKernel kernel, JobType jobType): base(kernel, jobType)
		{
			SetFirstFileLabel();
		}

		public SingleBusJobViewModel(IKernel kernel, JobEntry jobEntry) : base(kernel, jobEntry)
		{
			SetFirstFileLabel();
		}
		
		protected sealed override void SetFirstFileLabel()
		{
			FirstLabelText = $"Select {JobFileType.PrimaryBusFile.GetLable()}";
		}
	}
}
