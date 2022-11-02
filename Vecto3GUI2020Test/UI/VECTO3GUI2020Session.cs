using System;
using System.Diagnostics;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System.Reflection;
using VECTO3GUI2020.Properties;

namespace Vecto3GUI2020Test.UI
{
    [TestFixture]
    public class VECTO3GUI2020Session
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string NotepadAppId = @"C:\Windows\System32\notepad.exe";

		private static Process winappDriverProcess;

        internal static WindowsDriver<WindowsElement> session;
		internal static DesktopSession desktopSession;

		public WindowsDriver<WindowsElement> DesktopSessionElement
		{
			get { return session; }
		}

		public static void Setup(TestContext context)
        {
            // Launch a new instance of VECTO application
            if (session == null) {
				winappDriverProcess = Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");
				// Create a new session to launch Notepad application


				var appiumOptions = new OpenQA.Selenium.Appium.AppiumOptions();
				appiumOptions.AddAdditionalCapability("app", @"C:\Users\Harry\source\repos\vecto-dev\VECTO3GUI2020\bin\Debug\VECTO3GUI2020.exe");
				appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
                session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
				session.ActivateApp(Assembly.GetExecutingAssembly().Location);

				// Use the session to control the app
				Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);


				// Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
			}

			desktopSession = new DesktopSession();
		}

        
        public static void TearDown()
		{
			// Close the application and delete the session
            if (session != null)
            {
                session.CloseApp();
				winappDriverProcess.Kill();
				
				session = null;
            }
        }

		public void TestInitialize()
        {
            // Select all text and delete to clear the edit box
		}

        protected static string SanitizeBackslashes(string input) => input.Replace("\\", Keys.Alt + Keys.NumberPad9 + Keys.NumberPad2 + Keys.Alt);

    }

	public static class SessionExtensions{

		public static void DoubleCLick(this WindowsElement element)
		{
			element.Click();
			element.Click();
		}
	}
}
