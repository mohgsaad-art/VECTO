using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit
{
    public interface IJobEditViewModelFactory
    {


        /// <summary>
        /// Creates a JobEditViewModel dependent on the type of the inputdataprovider
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        IJobEditViewModel CreateJobEditViewModel(IInputDataProvider inputData);
                                             
    }
}
