using System.IO;

namespace VECTO3GUI2020.Helper
{
	public static class FileHelper
	{
		public static void CreateDirectory(string fileName)
		{
			var dirName = Path.GetDirectoryName(fileName);
			if (!string.IsNullOrEmpty(dirName)) {
				Directory.CreateDirectory(dirName);
			}
		}
	}
}