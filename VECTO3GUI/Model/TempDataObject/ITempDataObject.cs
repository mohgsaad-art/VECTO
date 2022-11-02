using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI.Model.TempDataObject
{
	public interface ITempDataObject<in T> where T : class
	{
		void UpdateCurrentValues(T viewModel);
		void ResetToComponentValues(T viewModel);
		void ClearValues(T viewModel);
	}

}
