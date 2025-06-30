using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData
{
    public interface IVectoRunDataFactoryFactory
    {
		/// <summary>
		/// Creates a VectoRunDataFactory for engineering mode simulation based on the type of inputDataProvider
		/// </summary>
		/// <param name="inputDataProvider"></param>
		/// <returns></returns>
        IVectoRunDataFactory CreateEngineeringRunDataFactory(IInputDataProvider inputDataProvider);


		/// <summary>
		/// Creates a VectoRunDataFactory for declaration mode simulation based on the type of inputDataProvider
		/// </summary>
		/// <param name="inputDataProvider"></param>
		/// <param name="report"></param>
		/// <param name="vtpReport"></param>
		/// <returns></returns>
		IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider,
			IDeclarationReport report, IVTPReport vtpReport);
	}
}
