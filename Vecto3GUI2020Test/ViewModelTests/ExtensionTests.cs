using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using EnumHelper = VECTO3GUI2020.Helper.EnumHelper;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class ExtensionTests
	{

		[Test]
		public void TestGetValuesAsObservableCollectionEnumExtension()
		{

			VehicleCode? vehicleCode = null;
			var collection1 = EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, VehicleCode>(VehicleCode.NOT_APPLICABLE, VehicleCode.CF);

			Assert.False(collection1.Contains(VehicleCode.NOT_APPLICABLE));
			Assert.False(collection1.Contains(VehicleCode.CF));

			Assert.True(collection1.Contains(VehicleCode.CA));
		
			Assert.True(collection1.Contains(VehicleCode.CB));
			Assert.True(collection1.Contains(VehicleCode.CC));
			Assert.True(collection1.Contains(VehicleCode.CD));
			Assert.True(collection1.Contains(VehicleCode.CE));
			Assert.True(collection1.Contains(VehicleCode.CG));
			Assert.True(collection1.Contains(VehicleCode.CH));
			Assert.True(collection1.Contains(VehicleCode.CI));
			Assert.True(collection1.Contains(VehicleCode.CJ));



			var collection2 = EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, VehicleCode>(VehicleCode.CA);
			Assert.True(collection2.Contains(VehicleCode.CA));
			Assert.False(collection2.Contains(VehicleCode.NOT_APPLICABLE));
		}

		[Ignore("")]
		[TestCase(1.000, 3U, "1.000")]
		[TestCase(1.210, 3U, "1.210")]
		[TestCase(1.201, 3U, "1.201")]
		[TestCase(1.3, 3U, "1.300")]
		public void ToMinSignificantDigitsExtensionTest(double input, uint significantDigits, string expected)
		{
			var result = input.ToMinSignificantDigits(significantDigits); //Called like this in XMLPrimaryBusVehicleReport;
			Assert.AreEqual(expected, result);
		}

		[TestCase(1.000, 3U, 3U, "1.000")]
		[TestCase(1.210, 3U, 3U, "1.210")]
		[TestCase(1.201, 3U, 3U, "1.201")]
		[TestCase(1.3, 3U, 3U, "1.300")]
		public void ToMinSignificantDigitsExtensionTest2(double input, uint significantDigits, uint decimals, string expected)
		{
			var result = input.ToMinSignificantDigits(significantDigits, decimals); //Called like this in XMLPrimaryBusVehicleReport;
			Assert.AreEqual(expected, result);
		}
	}
}