using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: MassXmlEditor <folder-path>");
            return;
        }

        string folderPath = args[0];

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Error: Folder '{folderPath}' does not exist.");
            return;
        }

        // Recursively find all .xml files
        string[] xmlFiles = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);

        if (xmlFiles.Length == 0)
        {
            Console.WriteLine("No XML files found.");
            return;
        }

        Console.WriteLine($"Found {xmlFiles.Length} XML file(s). Processing...");

        // Prefix may contain letters, digits, underscore, hyphen and dot (e.g. "v2.0:")
        // Group 1 = opening tag, Group 2 = prefix (including colon or empty),
        // Group 3 = content, Group 4 = closing tag (must use the same prefix)
        Regex manufacturerRegex = new Regex(
            @"(<((?:[\w.-]+:)?)Manufacturer\b[^>]*>)(.*?)(</\2Manufacturer\s*>)",
            RegexOptions.Singleline);

        Regex certificationRegex = new Regex(
            @"(<((?:[\w.-]+:)?)CertificationNumber\b[^>]*>)(.*?)(</\2CertificationNumber\s*>)",
            RegexOptions.Singleline);

        foreach (string filePath in xmlFiles)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                string original = content;

                // Replace Manufacturer content
                content = manufacturerRegex.Replace(content, m =>
                    m.Groups[1].Value + "manufacturer_name" + m.Groups[4].Value);

                // Replace CertificationNumber content
                content = certificationRegex.Replace(content, m =>
                    m.Groups[1].Value + "certification_number" + m.Groups[4].Value);

                if (content != original)
                {
                    File.WriteAllText(filePath, content);
                    Console.WriteLine($"Updated: {filePath}");
                }
                else
                {
                    //Console.WriteLine($"No changes needed: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing '{filePath}': {ex.Message}");
            }
        }

        Console.WriteLine("Done.");
    }
}