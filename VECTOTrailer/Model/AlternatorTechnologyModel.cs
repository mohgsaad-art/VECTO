using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Model
{
	public class AlternatorTechnologyModel : ObservableObject
	{
		public string _alternatorTechnology;

		public string AlternatorTechnology
		{
			get { return _alternatorTechnology; }
			set { SetProperty(ref _alternatorTechnology, value); }
		}
	}
}
