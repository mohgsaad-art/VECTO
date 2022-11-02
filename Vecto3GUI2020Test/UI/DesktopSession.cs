using System;
using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Vecto3GUI2020Test
{
	public class DesktopSession
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723/";
		WindowsDriver<WindowsElement> desktopSession;

		public DesktopSession()
		{
			var process = Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");

			var appCapabilities = new AppiumOptions();
			appCapabilities.AddAdditionalCapability("app", "Root");
			appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
			desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
		}

		~DesktopSession()
		{
			desktopSession.Quit();
		}

		public WindowsDriver<WindowsElement> DesktopSessionElement
		{
			get { return desktopSession; }
		}

		public WindowsElement FindElementByAbsoluteXPath(string xPath, int nTryCount = 15)
		{
			WindowsElement uiTarget = null;

			while (nTryCount-- > 0) {
				try {
					uiTarget = desktopSession.FindElementByXPath(xPath);
				} catch {
				}

				if (uiTarget != null) {
					break;
				} else {
					System.Threading.Thread.Sleep(200);
				}
			}

			return uiTarget;
		}
	}
}
