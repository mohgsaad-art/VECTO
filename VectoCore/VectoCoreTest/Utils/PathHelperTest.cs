
using System;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	[TestFixture]
	public class PathHelperTest
	{
		[Test]
		public void RelativePathTest1()
		{
			var path =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\PrimaryAndStageInput\\";
			var relativeTo =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\";



			var result = PathHelper.GetRelativePath(relativeTo, path);

			Assert.AreEqual("PrimaryAndStageInput", result );

			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));

		}

		[Test]
		public void RelativePathTest2()
		{
			var path =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\PrimaryAndStageInput\\file1.file";
			var relativeTo =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\file3.file";



			var result = PathHelper.GetRelativePath(relativeTo, path);

			Assert.AreEqual("PrimaryAndStageInput\\file1.file", result);

			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));

		}
		[Test]
		public void RelativePathTest3()
		{
			var path =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\file.file";
			var relativeTo =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\file3.file";



			var result = PathHelper.GetRelativePath(relativeTo, path);

			Assert.AreEqual("..\\..\\..\\..\\file.file", result);
			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));

		}


		[Test]
		public void RelativePathDifferentDrives()
		{
			var path =
				"D:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\PrimaryAndStageInput\\";
			var relativeTo =
				"C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\";


			Assert.Throws<ArgumentException>(
				() => {
					PathHelper.GetRelativePath(path, relativeTo);
				});
		}




		
	}
}