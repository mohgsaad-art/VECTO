using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData
{
    public interface IVectoRunDataFactoryFactory
    {
		IVectoRunDataFactory CreateEngineeringRunDataFactory(IEngineeringInputDataProvider inputDataProvider);

		/// <summary>
		/// Creates a VectoRunDataFactory based on the type of inputDataProvider
		/// </summary>
		/// <param name="inputDataProvider"></param>
		/// <param name="report"></param>
		/// <returns></returns>
		IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider,
			IDeclarationReport report, IVTPReport vtpReport);
	}
}
