using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace VECTO3GUI2020.Helper
{
	public static class ExceptionExtension
	{
		public static string GetInnerExceptionMessages(this Exception ex, string separator = "\n")
		{
			IList<string> errorStrings = new List<string>();


			do {
				errorStrings.Add(ex.Message);
				ex = ex.InnerException;
			} while (ex != null);

			var errorString = String.Join(separator, errorStrings);


			return errorString;
		}



	}
}