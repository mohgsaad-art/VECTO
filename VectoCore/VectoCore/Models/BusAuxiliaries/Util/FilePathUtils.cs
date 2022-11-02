using System;
using System.IO;
using System.Linq;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Util {
	public class FilePathUtils
	{

		public static string fPATH(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || path.Length < 3 || path.Substring(1, 2) != @":\") {
				return "";
			}

			return Path.GetDirectoryName(path);
		}

		public static string Left(string str, int length)
		{
			return str.Substring(0, Math.Min(length, str.Length));
		}

		public static string Right(string value, int length)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;

			return value.Length <= length ? value : value.Substring(value.Length - length);
		}

		public static bool ValidateFilePath(string filePath, string expectedExtension, ref string message)
		{
			var illegalFileNameCharacters = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*', '~' };
			var detectedExtention = fileExtentionOnly(filePath);
			var pathOnly = filePathOnly(filePath);
			var fileNameOnlyWithExtension = fileNameOnly(filePath, true);
			var fileNameOnlyNoExtension = fileNameOnly(filePath, false);

			// Is this filePath empty
			if (filePath.Trim().Length == 0 || Right(filePath, 1) == @"\") {
				message = "A filename cannot be empty";
				return false;
			}


			// Extension Expected, but not match
			if (expectedExtension.Trim().Length > 0) {
				if (string.Compare(expectedExtension, detectedExtention, true) != 0) {
					message = $"The file extension type does not match the expected type of {expectedExtension}";
					return false;
				}
			}

			// Extension Not Expected, but was supplied
			if (expectedExtension.Trim().Length > 0) {
				if (detectedExtention.Length == 0) {
					message = $"No Extension was supplied, but an extension of {detectedExtention}, this is not required";
					return false;
				}
			}


			// Illegal characters
			if (!fileNameLegal(fileNameOnlyWithExtension)) {
				message = "The filenames have one or more illegal characters";
				return false;
			}


			message = "OK";
			return true;
		}


		public static bool fileNameLegal(string fileName)
		{
			var illegalFileNameCharacters = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*', '~' };


			// Illegal characters
			foreach (var ch in illegalFileNameCharacters) {
				if (fileName.Contains(ch))
					return false;
			}
			return true;
		}


		public static string ResolveFilePath(string vectoPath, string filename)
		{

			// No Vecto Path supplied
			if (string.IsNullOrEmpty(vectoPath))
				return filename;

			// This is not relative
			if (filename.Contains(@":\"))

				// Filepath is already absolute
				return filename;
			else
				return Path.Combine(vectoPath, filename);// vectoPath & filename
		}


		/// <summary>
		/// 	''' File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
		/// 	''' </summary>
		/// 	''' <param name="filePath"></param>
		/// 	''' <param name="WithExtention"></param>
		/// 	''' <returns>Return file portion of the path, with or without the extension</returns>
		/// 	''' <remarks></remarks>
		public static string fileNameOnly(string filePath, bool WithExtention)
		{
			int x;
			x = filePath.LastIndexOf(@"\") + 1;
			filePath = Right(filePath, filePath.Length - x);
			if (!WithExtention) {
				x = filePath.LastIndexOf(".");
				if (x > 0)
					filePath = Left(filePath, x);
			}
			return filePath;
		}


		/// <summary>
		/// 	''' Extension alone      "C:\temp\TEST.txt" >> ".txt"
		/// 	''' </summary>
		/// 	''' <param name="filePath"></param>
		/// 	''' <returns>Extension alone Including the dot IE  .EXT</returns>
		/// 	''' <remarks></remarks>
		public static string fileExtentionOnly(string filePath)
		{
			int x;
			x = filePath.LastIndexOf(".");
			if (x == -1)
				return "";
			else
				return Right(filePath, filePath.Length - x);
		}

		/// <summary>
		/// 	''' File Path alone   "C:\temp\TEST.txt"  >>  "C:\temp\"
		/// 	'''                   "TEST.txt"          >>  ""
		/// 	''' </summary>
		/// 	''' <param name="filePath"></param>
		/// 	''' <returns>Filepath without the extension</returns>
		/// 	''' <remarks></remarks>
		public static string filePathOnly(string filePath)
		{
			int x;
			if (filePath == null || filePath.Length < 3 || filePath.Substring(1, 2) != @":\")
				return "";
			x = filePath.LastIndexOf(@"\");
			return Left(filePath, x + 1);
		}
	}
}
