// See https://aka.ms/new-console-template for more information

using TES3Lib;

Console.WriteLine("Hello, World!");

var testPlugin = "test.esp";
if (Path.Exists(testPlugin))
{
    var plugin = TES3.TES3Load(testPlugin);
    
    Console.WriteLine($"{testPlugin}");
    foreach (var record in plugin.Records)   
    {
        Console.WriteLine($"\t{record.GetEditorId()}");
    }
}


