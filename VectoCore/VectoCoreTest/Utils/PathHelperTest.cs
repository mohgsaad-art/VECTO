
using System;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	[TestFixture]
	public class PathHelperTest
	{
		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void RelativePathTest1()
		{
			var path = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\PrimaryAndStageInput"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/VectoCoreTest/TestData/Integration/Buses/PrimaryAndStageInput";

			var relativeTo = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/VectoCoreTest/TestData/Integration/Buses/";


			var result = PathHelper.GetRelativePath(relativeTo, path);

			Assert.AreEqual("PrimaryAndStageInput", result );

			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));
		}

		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void RelativePathTest2()
		{
			var path = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\PrimaryAndStageInput\\file1.file"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/VectoCoreTest/TestData/Integration/Buses/PrimaryAndStageInput/file1.file";
			
			var relativeTo = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\file3.file"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/VectoCoreTest/TestData/Integration/Buses/file3.file";


			var result = PathHelper.GetRelativePath(relativeTo, path);

			var compareTo = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "PrimaryAndStageInput\\file1.file"
				: "PrimaryAndStageInput/file1.file";

			Assert.AreEqual(compareTo, result);

			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));
		}

		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void RelativePathTest3()
		{
			var path = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\file.file"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/file.file";
			
			var relativeTo = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "C:\\Users\\Harry\\source\\repos\\vecto-dev\\VectoCore\\VectoCoreTest\\TestData\\Integration\\Buses\\file3.file"
				: "/Users/Harry/source/repos/vecto-dev/VectoCore/VectoCoreTest/TestData/Integration/Buses/file3.file";


			var result = PathHelper.GetRelativePath(relativeTo, path);

			var compareTo = (Environment.OSVersion.Platform == PlatformID.Win32NT)
				? "..\\..\\..\\..\\file.file"
				: "../../../../file.file";

			Assert.AreEqual(compareTo, result);
			Assert.AreEqual(path, PathHelper.GetAbsolutePath(relativeTo, result));
		}


		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
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