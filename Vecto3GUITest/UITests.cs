using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using OpenQA.Selenium;

namespace Vecto3GUITest
{
    [TestClass]
    public class StartUpScenario : VECTO3GUI2020Session
    {
        [TestMethod]
        public void LoadFile()
		{
			//session.FindElementByAccessibilityId("Register").FindElementByClassName("Button").Click();
			session.FindElementByName("New Multistage File").Click();


		}

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			Setup(context);
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
			TearDown();
		}
    }
}
