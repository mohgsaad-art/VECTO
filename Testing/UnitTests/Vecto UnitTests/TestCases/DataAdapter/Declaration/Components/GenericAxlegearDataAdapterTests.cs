using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class GenericAxlegearDataAdapterTests
{

	[Test]
	public void GenericAxlegearDataTests([Values] AxleLineType linetype)
	{
		var dao = new GenericCompletedBusAxleGearDataAdapter();
		var ratio = 4.18;
		var mockInputData = GetMockInputData(ratio, linetype);
		var axlegearData = dao.CreateAxleGearData(mockInputData);

		Assert.IsNotNull(axlegearData);
		Assert.AreEqual(linetype, axlegearData.LineType);

		var expected = new[] {
			new[] { 0, -117157.68, 2459.54 },
			new[] { 0, -40.22, 19.59 },
			new[] { 0, 79.40, 19.59 },
			new[] { 0, 122076.77, 2459.54 },
			new[] { 209, -117157.68, 2459.54 },
			new[] { 209, -40.22, 19.59 },
			new[] { 209, 79.40, 19.59 },
			new[] { 209, 122076.77, 2459.54 },
			new[] { 20900, -116702.95, 2914.28 },
			new[] { 20900, 414.52, 474.33 },
			new[] { 20900, 534.14, 474.33 },
			new[] { 20900, 122531.50, 2914.28 },
		};
		foreach (var entry in expected) {
			// expected values are related to the input side. convert to output side for the lookup.
			var loss = axlegearData.AxleGear.LossMap.GetTorqueLoss(entry[0].RPMtoRad() / ratio, (entry[1].SI<NewtonMeter>() - entry[2].SI<NewtonMeter>()) * ratio );
			Assert.AreEqual(entry[2], loss.Value.Value(), 1e-2);
		}
	}

	private IAxleGearInputData GetMockInputData(double ratio, AxleLineType linetype)
	{
		var axl = new Mock<IAxleGearInputData>();
		axl.Setup(a => a.Ratio).Returns(ratio);
		axl.Setup(a => a.LineType).Returns(linetype);
		return axl.Object;
	}
}