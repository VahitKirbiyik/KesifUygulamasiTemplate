using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string resxFolder = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Strings");
        var resxFiles = Directory.GetFiles(resxFolder, "AppResources.*.resx");

        bool reportDuplicates = args.Contains("--report-duplicates");
        string logFilePath = "resx-duplicates.log";

        if (reportDuplicates && File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        foreach (var file in resxFiles)
        {
            Console.WriteLine($"Checking {file}...");
            var duplicateKeys = XDocument.Load(file)
                .Descendants("data")
                .GroupBy(x => x.Attribute("name")?.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var key in duplicateKeys)
            {
                Console.WriteLine($"Duplicate key found: {key}");
                if (reportDuplicates)
                {
                    File.AppendAllText(logFilePath, $"{file}: {key}{Environment.NewLine}");
                }
            }
        }

        Console.WriteLine("Validation complete.");
        if (reportDuplicates)
        {
            Console.WriteLine($"Duplicate keys logged to {logFilePath}");
        }
    }
}
