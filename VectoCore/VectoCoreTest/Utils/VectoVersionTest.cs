using System;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;
using static TUGraz.VectoCore.Utils.VersioningUtil;

namespace TUGraz.VectoCore.Tests.Utils
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class VectoVersionTest
    {
        [
        TestCase("1.1.3.400-DEV", "1.2.2.100-RC", VersionPart.Major, 0),
        TestCase("1.2.1.400-DEV", "1.1.2.100-RC", VersionPart.Major, 0),
        TestCase("1.1.3.400-DEV", "1.1.1.100-RC", VersionPart.Major, 0),

        TestCase("1.1.3.400-DEV", "2.1.3.400-DEV", VersionPart.Major, -1),
        TestCase("2.2.1.400-DEV", "1.2.1.400-DEV", VersionPart.Major, 1),
        TestCase("1.2.3.400-DEV", "1.1.1.400-DEV", VersionPart.Major, 0),
        
        TestCase("1.1.3.400-DEV", "1.2.2.100-RC", VersionPart.Minor, -1),
        TestCase("1.2.1.400-DEV", "1.1.2.100-RC", VersionPart.Minor, 1),
        TestCase("1.1.3.400-DEV", "1.1.1.100-RC", VersionPart.Minor, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.2.100-RC", VersionPart.Minor, 0),
        TestCase("1.2.1.400-DEV", "1.2.2.100-RC", VersionPart.Minor, 0),
        TestCase("1.3.3.400-DEV", "1.3.1.100-RC", VersionPart.Minor, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.4.100-RC", VersionPart.Patch, -1),
        TestCase("1.2.1.400-DEV", "1.2.0.100-RC", VersionPart.Patch, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.100-RC", VersionPart.Patch, 0),

        TestCase("1.1.4.400-DEV", "1.1.4.100-RC", VersionPart.Patch, 0),
        TestCase("1.2.0.400-DEV", "1.2.0.100-RC", VersionPart.Patch, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.3.500-RC", VersionPart.Build, -1),
        TestCase("1.2.1.400-DEV", "1.2.1.100-RC", VersionPart.Build, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.400-RC", VersionPart.Build, 0),

        TestCase("1.1.3.400-DEV", "1.1.4.400-DEV", VersionPart.Build, -1),
        TestCase("1.2.2.400-DEV", "1.2.1.400-DEV", VersionPart.Build, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.400-DEV", VersionPart.Build, 0),
        ]
        public void TestCompareVersionsWithBound(string a, string b, VersionPart bound, int result)
        {
            Assert.IsTrue(CompareVersions(a, b, bound) == result);
        }
    }
}
