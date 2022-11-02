using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Appium;

namespace Vecto3GUITest
{
    [TestClass]
    public class VECTO3GUI2020Session
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string NotepadAppId = @"C:\Windows\System32\notepad.exe";

        protected static WindowsDriver<WindowsElement> session;

		public static void Setup(TestContext context)
        {
            // Launch a new instance of Notepad application
            if (session == null)
            {
                // Create a new session to launch Notepad application
				var appiumOptions = new OpenQA.Selenium.Appium.AppiumOptions();
				appiumOptions.AddAdditionalCapability("app", @"C:\Users\Harry\source\repos\vecto-dev\VECTO3GUI2020\bin\Debug\VECTO3GUI2020.exe");
				appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
                session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);

                // Use the session to control the app
				Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);


				// Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
			}
        }

        public static void TearDown()
		{
			// Close the application and delete the session
            if (session != null)
            {
                session.Close();
				session.Quit();
                session = null;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Select all text and delete to clear the edit box
		}

        protected static string SanitizeBackslashes(string input) => input.Replace("\\", Keys.Alt + Keys.NumberPad9 + Keys.NumberPad2 + Keys.Alt);
    }
}
