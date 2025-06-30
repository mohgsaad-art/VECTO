using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class PTOLossMapReaderTests
{
	[TestCase]
	public void PTOLossMapCaseSensitiveTest()
	{
		var data = new[] {
			"0, 0",
			"10, 100"
		};
		var tbl = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("pto torque, engine speed", data));

		var pto = PTOIdleLossMapReader.Create(tbl);

		var loss = pto.GetTorqueLoss(100.RPMtoRad());
		Assert.AreEqual(10, loss.Value());
	}
}