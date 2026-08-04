using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VTPReport
{

	internal class NullVTPReport : IVTPReport
	{
		#region Implementation of IDeclarationReport

		public void InitializeReport(VectoRunData modelData) { }

		public void PrepareResult(VectoRunData runData) { }

		public void AddResult(VectoRunData runData, IModalDataContainer modData) { }

		public IPrimaryVehicleInformationInputDataProvider PrimaryResults { get; set; }

		#endregion

		#region Implementation of IVTPReport

		public IVectoHash InputDataHash
		{
			set { }
		}

		public IManufacturerReport ManufacturerRecord
		{
			set { }
		}

		public IVectoHash ManufacturerRecordHash
		{
			set { }
		}

		public IVectoHash CustomerFileHash
		{
			set { }
		}

		public IVectoHash PrimaryVIFHash
		{
			set { }
		}

		public IVectoHash CompletedVIFHash
		{
			set { }
		}

		public VTPOBFCMDeclarationData OBFCMDeclarationInputData
		{
			set { }
		}

		#endregion
	}
}