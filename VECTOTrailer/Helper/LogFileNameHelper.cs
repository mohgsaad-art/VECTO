using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace VECTOTrailer.Helper
{
	public static class LogFileHelper
	{
		private static string _basePath = null;

		public static bool SetBasePath(string basePath)
		{
			if (_basePath == null) {
				_basePath = basePath;
				return true;
			} else {
				return false;
			}
		}
	


		public static string GetFileNameWithBasePath(string fileName)
		{
			return Path.Combine(_basePath, fileName);
		}
		
	}
}