using System;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Utils
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class VectoVersionTest
    {
        [
        TestCase("1.1.3.400-DEV", "1.2.2.100-RC", VersioningUtil.VersionPart.Major, 0),
        TestCase("1.2.1.400-DEV", "1.1.2.100-RC", VersioningUtil.VersionPart.Major, 0),
        TestCase("1.1.3.400-DEV", "1.1.1.100-RC", VersioningUtil.VersionPart.Major, 0),

        TestCase("1.1.3.400-DEV", "2.1.3.400-DEV", VersioningUtil.VersionPart.Major, -1),
        TestCase("2.2.1.400-DEV", "1.2.1.400-DEV", VersioningUtil.VersionPart.Major, 1),
        TestCase("1.2.3.400-DEV", "1.1.1.400-DEV", VersioningUtil.VersionPart.Major, 0),
        
        TestCase("1.1.3.400-DEV", "1.2.2.100-RC", VersioningUtil.VersionPart.Minor, -1),
        TestCase("1.2.1.400-DEV", "1.1.2.100-RC", VersioningUtil.VersionPart.Minor, 1),
        TestCase("1.1.3.400-DEV", "1.1.1.100-RC", VersioningUtil.VersionPart.Minor, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.2.100-RC", VersioningUtil.VersionPart.Minor, 0),
        TestCase("1.2.1.400-DEV", "1.2.2.100-RC", VersioningUtil.VersionPart.Minor, 0),
        TestCase("1.3.3.400-DEV", "1.3.1.100-RC", VersioningUtil.VersionPart.Minor, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.4.100-RC", VersioningUtil.VersionPart.Patch, -1),
        TestCase("1.2.1.400-DEV", "1.2.0.100-RC", VersioningUtil.VersionPart.Patch, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.100-RC", VersioningUtil.VersionPart.Patch, 0),

        TestCase("1.1.4.400-DEV", "1.1.4.100-RC", VersioningUtil.VersionPart.Patch, 0),
        TestCase("1.2.0.400-DEV", "1.2.0.100-RC", VersioningUtil.VersionPart.Patch, 0),
        
        TestCase("1.1.3.400-DEV", "1.1.3.500-RC", VersioningUtil.VersionPart.Build, -1),
        TestCase("1.2.1.400-DEV", "1.2.1.100-RC", VersioningUtil.VersionPart.Build, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.400-RC", VersioningUtil.VersionPart.Build, 0),

        TestCase("1.1.3.400-DEV", "1.1.4.400-DEV", VersioningUtil.VersionPart.Build, -1),
        TestCase("1.2.2.400-DEV", "1.2.1.400-DEV", VersioningUtil.VersionPart.Build, 1),
        TestCase("1.1.3.400-DEV", "1.1.3.400-DEV", VersioningUtil.VersionPart.Build, 0),
        Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void TestCompareVersionsWithBound(string a, string b, VersioningUtil.VersionPart bound, int result)
        {
            Assert.IsTrue(VersioningUtil.CompareVersions(a, b, bound) == result);
        }

        [
        TestCase("1.1.1.1200-DEV", true),
        TestCase("1.1.1.1200-RC", true),
        TestCase("1.1.1.1200-abcd", true),
        TestCase("1.1.1.1", true),
        TestCase("1.2.3.4", true),
        TestCase("1", false),
        TestCase("1.1", false),
        TestCase("1.1.1", false),
        TestCase("1.1.1.1.1", false),
        TestCase("abc", false),
        TestCase("1.abc", false),
        TestCase("", false),
        TestCase("1.1-DEV", false),
        TestCase("1.1.1-DEV", false),
        TestCase("a.b.c.d-qwe", false),
        Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void TestIsVersion(string text, bool result)
        {
            Assert.IsTrue(VersioningUtil.IsVersion(text) == result);
        }
    }
}
