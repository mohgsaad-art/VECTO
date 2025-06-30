using System.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.UnitTests.Utils.MockComponents;

public class MemorySumDataWriter : ISummaryWriter
{
	public DataTable WrittenData = null;

	#region Implementation of ISummaryWriter

	public void WriteSumData(DataTable sortedAndFilteredTable)
	{
		WrittenData = sortedAndFilteredTable;
	}

	#endregion
}