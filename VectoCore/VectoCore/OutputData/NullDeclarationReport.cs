using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	internal class NullDeclarationReport : IDeclarationReport
	{
		#region Implementation of IDeclarationReport
		private int _addedResults = 0;
		public void InitializeReport(VectoRunData modelData)
		{

		}

		public void PrepareResult(VectoRunData runData)
		{

		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddResult(VectoRunData runData, IModalDataContainer modData)
		{
			_addedResults++;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryResults { get; set; }

		#endregion
	}
}