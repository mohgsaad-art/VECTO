using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Helper;

public class IEPCGearboxInputDataTests
{

	[TestCase]
	public void TestIEPCGearboxInputata()
	{
		var iepcInputMock = GetIEPCInputMock();

		var iepcGbx = new IEPCGearboxInputData(iepcInputMock);

		Assert.IsNotNull(iepcGbx);
		Assert.AreEqual(iepcInputMock.Manufacturer, iepcGbx.Manufacturer);
		Assert.AreEqual(iepcInputMock.Model, iepcGbx.Model);
		Assert.AreEqual(iepcInputMock.CertificationMethod, iepcGbx.CertificationMethod);
		Assert.AreEqual(GearboxType.APTN, iepcGbx.Type);
		Assert.IsFalse(iepcGbx.DifferentialIncluded);
		Assert.AreEqual(double.NaN, iepcGbx.AxlegearRatio);

		for (var i = 0; i < iepcInputMock.Gears.Count; i++) {
			var g1 = iepcInputMock.Gears[i];
			var g2 = iepcGbx.Gears[i];
			Assert.IsNotNull(g1);
			Assert.IsNotNull(g2);
			Assert.AreEqual(g1.GearNumber, g2.Gear);
			Assert.AreEqual(g1.Ratio, g2.Ratio, 1e-9);
			var ratio = g1.Ratio;
			if (g1.MaxOutputShaftSpeed != null) {
				Assert.AreEqual(g1.MaxOutputShaftSpeed.Value(), g2.MaxInputSpeed.Value() / ratio, 1e-6);
			} else {
				Assert.IsNull(g2.MaxInputSpeed);
			}

			if (g1.MaxOutputShaftTorque != null) {
				Assert.AreEqual(g1.MaxOutputShaftTorque.Value(), g2.MaxTorque.Value() * ratio, 1e-6);
			} else {
				Assert.IsNull(g2.MaxTorque);
			}
		}
	}

    private IIEPCDeclarationInputData GetIEPCInputMock()
	{
		var iepc = new Mock<IIEPCDeclarationInputData>();

		iepc.Setup(i => i.Manufacturer).Returns("Some Manufacturer");
		iepc.Setup(i => i.Model).Returns("IEPC Model");
		iepc.Setup(i => i.CertificationMethod).Returns(CertificationMethod.Measured);

		var gear1 = new Mock<IGearEntry>();
		gear1.Setup(g => g.Ratio).Returns(3);

		var gear2 = new Mock<IGearEntry>();
		gear2.Setup(g => g.Ratio).Returns(1);
		gear2.Setup(g => g.MaxOutputShaftTorque).Returns(2000.SI<NewtonMeter>());

		var gear3 = new Mock<IGearEntry>();
		gear3.Setup(g => g.Ratio).Returns(0.5);
		gear3.Setup(g => g.MaxOutputShaftSpeed).Returns(3000.RPMtoRad());

		iepc.Setup(i => i.Gears).Returns(new List<IGearEntry>() { gear1.Object, gear2.Object, gear3.Object });

		return iepc.Object;
	}
}