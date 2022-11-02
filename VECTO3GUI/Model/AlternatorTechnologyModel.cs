using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.Model
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
