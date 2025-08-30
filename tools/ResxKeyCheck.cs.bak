// .resx anahtar bütünlüğü kontrol aracı
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

class ResxKeyCheck
{
    static string[] RequiredKeys = new[] { "POIShow", "TransportationMode", "RealTimeTraffic", "Donate", "AR", "Offline Map", "Voice Navigation", "TestKey" };
    static void Main()
    {
        var resxFiles = Directory.GetFiles("Resources/Strings", "AppResources.*.resx");
        foreach (var file in resxFiles)
        {
            var doc = XDocument.Load(file);
            var keys = doc.Descendants("data").Select(x => (string)x.Attribute("name")).ToList();
            var missing = RequiredKeys.Except(keys).ToList();
            if (missing.Any())
            {
                Console.WriteLine($"{file} eksik anahtarlar: {string.Join(", ", missing)}");
                Environment.Exit(1);
            }
        }
        Console.WriteLine("Tüm .resx dosyalarında anahtar bütünlüğü sağlandı.");
    }
}
