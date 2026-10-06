using System;
using System.IO;

namespace TUGraz.Vecto.IntegrationTests.Utils;

public class InputDataHelper
{
	public static string CreateUniqueSubfolder(String inputFile)
    {
		string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Guid.NewGuid().ToString(), Path.GetFileName(inputFile));

		if (!Directory.Exists(Path.GetDirectoryName(outputFile))) {
			Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
        }

		return outputFile;
	}
}
