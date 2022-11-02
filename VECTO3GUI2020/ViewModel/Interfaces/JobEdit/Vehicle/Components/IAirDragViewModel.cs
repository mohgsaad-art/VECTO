using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
    public interface IAirDragViewModel : IAirdragDeclarationInputData, IComponentViewModel
    {
        bool LabelVisible { get; set; }
        bool IsReadOnly { get; set; }
		XmlNode XMLSource { get; }
	}
}
