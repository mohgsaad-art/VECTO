using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VECTO3GUI2020.Helper
{
    public static class ProcessHelper
    {

		public static void OpenFolder(string path)
		{
			if (path == null)
			{
				return;
			}

			path = Path.GetFullPath(path);

			var explorerCommandStrBuilder = new StringBuilder();
			explorerCommandStrBuilder.Append("explorer.exe");
			explorerCommandStrBuilder.Append(" /select ");
			explorerCommandStrBuilder.Append(path);

			StartProcess("explorer.exe", "/select," + path);
		}



		public static void OpenFile(string path)
		{
			if (path == null)
			{
				return;
			}

			StartProcess(path);


		}

		private static void StartProcess(string command, params string[] arguments)
		{
			string argumentsString = "";
			if (arguments != null)
			{
				var argumentsStrBuilder = new StringBuilder();
				foreach (var argument in arguments)
				{
					argumentsStrBuilder.Append(argument);
					if (argument != arguments.Last())
					{
						argumentsStrBuilder.Append(" ");
					}
				}

				argumentsString = SanitizeInput(argumentsStrBuilder.ToString());
				Debug.WriteLine(argumentsString);
			}

			try
			{
				Process.Start(new ProcessStartInfo(command, argumentsString) { UseShellExecute = true});
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

        public static string SanitizeInput(string input)
        {
            var disallowedChars = new char[] { '&', ';', '|', '$' };

            foreach (var c in disallowedChars)
            {
                input = input.Replace(c.ToString(), string.Empty);
            }

            return input;
        }

    }
}
