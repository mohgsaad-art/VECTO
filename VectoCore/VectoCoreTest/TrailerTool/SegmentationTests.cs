using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.TrailerTool
{
    [TestFixture]
	internal class SegmentationTests
	{
		private TrailerSegments _trailerSegments;


		[SetUp]
		public void SetUp()
		{
			_trailerSegments = new TrailerSegments();
		}


		[TestCase("> 8 to", 8, false)]

		[TestCase("> 8 to", 8.1, true)]
		[TestCase(">= 8", 8, true)]
		[TestCase("> = 8", 8, true)]
		[TestCase("≥8 ", 8, true)]
		[TestCase("---", 9, true)]

		[TestCase("≤13.5", 8, true)]
		[TestCase("≤13.5", 13.5, true)]
		[TestCase("≤13.5", 13.6, false)]

		[TestCase("> 8 to up to <= 13.5 to", 13, true)]
		[TestCase("> 8 to up to <= 13.5 to", 13.5, true)]
		[TestCase("> 8 to up to <= 13.5 to", 8.1, true)]
		[TestCase("> 8 to up to <= 13.5 to", 8, false)]
		[TestCase("> 8 to up to <= 13.5 to", 7, false)]
		[TestCase("> 8 to up to <= 13.5 to", 13.51, false)]

		[TestCase("all weights", 13.51, true)]

		public void TPMLMAxleAssemblyTest(string tpmlmAxleAssemblyString, double valueToCheck, bool expectedReturnValue)
		{
			var valueInTons = Kilogram.Create(valueToCheck * 1000).ConvertToTon();
			var func = _trailerSegments.getTPMLMAxleAssemblyFunction(tpmlmAxleAssemblyString);
			var ret = func.Invoke(valueInTons);
			Assert.AreEqual(expectedReturnValue, ret);


		}








	}
}
