using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IAirdrag : ICommonComponentParameters
	{
		bool NoAirdragData { get; set; }
		bool UseMeasurementData { get; set; }
		bool UseMeasuredValues { get; set; }
		SquareMeter CdxA_0 { get; set; }
		SquareMeter TransferredCdxA { get; set; }
		SquareMeter DeclaredCdxA { get; set; }
		string AppVersion { get; set; }
		DigestData DigestValue { get; set; }
	}
}
