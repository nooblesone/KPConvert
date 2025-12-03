using System.CommandLine;
using System.CommandLine.Parsing;

Option<FileInfo> fileOption = new("--psd")
{
    Description = "The file to read and display on the console.",
};

Option<FileInfo> fileOption2 = new("--kra")
{
    Description = "The file to read and display on the console.",
};

RootCommand rootCommand = new("Sample app for System.CommandLine");
rootCommand.Options.Add(fileOption);
rootCommand.Options.Add(fileOption2);

rootCommand.SetAction(parseResult =>
{
    FileInfo parsedFile = parseResult.GetValue(fileOption);
    FileInfo parsedFile2 = parseResult.GetValue(fileOption2);
    ReadFile(parsedFile);
    ReadFile(parsedFile2);
    return 0;
});

ParseResult parseResult = rootCommand.Parse(args);
return parseResult.Invoke();

static void ReadFile(FileInfo file)
{
    foreach (string line in File.ReadLines(file.FullName))
    {
        Console.WriteLine(line);
    }
}