using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace VECTOTrailer.ViewModel.Impl
{
	public class CompletedTrailerJobViewModel: AbstractTrailerJobViewModel
	{
		public CompletedTrailerJobViewModel(IKernel kernel, JobType jobType):base(kernel, jobType)
		{
			SetFirstFileLabel();
		}

		public CompletedTrailerJobViewModel(IKernel kernel, JobEntry jobEntry) : base(kernel, jobEntry)
		{
			SetFirstFileLabel();
		}
		
		protected sealed override void SetFirstFileLabel()
		{
			FirstLabelText = $"Select {JobFileType.PIFTrailerFile.GetLable()}";
		}
	}
}
