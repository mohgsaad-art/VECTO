using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IAuxiliaryViewModel : IAuxiliaryDeclarationInputData
    {
		string Name { get; set; }

        string TechnologyName { get; set; }

        IList<string> TechnologyList { get; }
        AuxiliaryType Type { get; }
	}
}

