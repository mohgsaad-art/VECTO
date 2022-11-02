using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TUGraz.VectoCore.Utils
{
	public static class PathHelper
	{
		public static string GetRelativePath(string relativeTo, string path)
		{
			
			
			var relativeFullPath = Path.GetFullPath(relativeTo);
			var relativeToFileName = Path.GetFileName(relativeFullPath);
			relativeFullPath = relativeFullPath.RemoveLastOccurence(relativeToFileName);

			var pathFullPath = Path.GetFullPath(path);
			var pathFileName = Path.GetFileName(pathFullPath);
			pathFullPath = pathFullPath.RemoveLastOccurence(pathFileName);

			var rootEqual = Path.GetPathRoot(relativeFullPath) == Path.GetPathRoot(path);
			if (!rootEqual)
			{
				throw new ArgumentException("Paths must be on the same drive");
			}


			var pathArray = pathFullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s));
			var relativeToArray = relativeFullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s));
			var result = GetRelativePath(relativeToArray, pathArray);


			return Path.Combine(result, pathFileName);
		}

		public static string GetAbsolutePath(string relativeTo, string relativePath)
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(relativeTo), relativePath));
			//var uri = new Uri(baseUri: new Uri(relativeTo), relativeUri: relativePath);
			//return Path.GetFullPath(uri.AbsolutePath);
		}

		private static string GetRelativePath(IEnumerable<string> relativeTo, IEnumerable<string> path)
		{
			var relList = relativeTo.ToList();
			var pathList = path.ToList();
			var resultList = new LinkedList<string>();
			for (int i = 0; i < Math.Max(relList.Count, pathList.Count); i++) {
				var relRemaining = i < relList.Count;
				var pathRemaining = i < pathList.Count;
				
				if ((relRemaining && pathRemaining) && relList[i] == pathList[i]) {
					//ignore CommonNode
					continue;
				}

				if (relRemaining) {
					resultList.AddFirst("..");
				}

				if (pathRemaining) {
					resultList.AddLast(pathList[i]);
				}
			}
			return Path.Combine(resultList.ToArray());
		}

		private static string RemoveLastOccurence(this string original, string remove)
		{
			
			var lastIndex = original.LastIndexOf(remove, StringComparison.InvariantCulture);


			return lastIndex != -1 ? original.Remove(lastIndex, remove.Length) : original;
		}
	}
}