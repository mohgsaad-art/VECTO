using System;
using Microsoft.Win32;
using NLog.MessageTemplates;

namespace VECTOTrailer.Helper
{
	static class BrowserUtils
	{
        public static string GetDefaultBrowserPath()
		{
			var urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
			var browserPathKey = @"$BROWSER$\shell\open\command";


			string browserPath = null;

			//'Read default browser path from userChoiceLKey
			var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);
			if (userChoiceKey == null) {
                //'Read default browser path from Win XP registry key, or try Win Vista (and newer) registry key

				var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);
				if (browserKey == null) {
					browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, false);
				}

				var path = browserKey?.GetValue("").ToString();
				browserKey?.Close();
				browserPath = path;
			} else {
				var progId = userChoiceKey.GetValue("ProgId").ToString();
                userChoiceKey.Close();
				var concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
				var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
				browserPath = kp?.GetValue("").ToString();
                kp?.Close();
				if (browserPath != null && browserPath.Contains(".exe")) {
					browserPath = browserPath?.Substring(1, browserPath.IndexOf(".exe") + 3);
				}
			}



            return browserPath;

		}





        //'Read default browser path from userChoiceLKey
        //Dim userChoiceKey As RegistryKey = Registry.CurrentUser.OpenSubKey(urlAssociation + "\UserChoice", False)

        //If userChoiceKey Is Nothing Then
        //'If user choice was not found, try machine default
        //'Read default browser path from Win XP registry key, or try Win Vista (and newer) registry key
        //Dim browserKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("HTTP\shell\open\command", False)
        //If browserKey Is Nothing Then
        //	browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, False)
        //End If

        //Dim path As String = CType(browserKey.GetValue(""), System.String)
        //browserKey.Close()
        //	If(path.Contains(".exe")) Then
        //	Return path.Substring(1, path.IndexOf(".exe") + 3)
        //Else
        //	Return path
        //	End If

        //	End If
        //' user defined browser choice was found
        //Dim progId As String = userChoiceKey.GetValue("ProgId").ToString()
        //userChoiceKey.Close()

        //' now look up the path of the executable
        //Dim concreteBrowserKey As String = browserPathKey.Replace("$BROWSER$", progId)
        //Dim kp As RegistryKey = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, False)
        //browserPath = CType(kp.GetValue(""), String)
        //kp.Close()
        //	If(browserPath.Contains(".exe")) Then
        //	Return browserPath.Substring(1, browserPath.IndexOf(".exe") + 3)
        //Else
        //	Return browserPath
        //	End If
        //	End Function
        //	End Class
    }
}