using KPConvert;
using PsdSharp;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;


Option<FileInfo> psdFileOption = new("--psd")
{
    Description = "The file to read psd file on the console.",
    Required = true,
};

Option<FileInfo> kraFileOption = new("--kra")
{
    Description = "The file to read kra file on the console.",
    Required = true,
};

RootCommand rootCommand = new("快速处理kra和psd剪切蒙版不兼容问题的小工具");

Command p2kCommand = new("p2k","psd to kra")
{
   psdFileOption,
   kraFileOption
};

p2kCommand.SetAction(parseResult =>
{
    var psdF = parseResult.GetValue(psdFileOption);
    var kraF = parseResult.GetValue(kraFileOption);
    P2K.Do(psdF.FullName, kraF.FullName);
});




Command k2pCommand = new("k2p","kra to psd")
{
   psdFileOption,
   kraFileOption
};


k2pCommand.SetAction(parseResult =>
{
    var psdF = parseResult.GetValue(psdFileOption);
    var kraF = parseResult.GetValue(kraFileOption);
    K2P.Do(psdF.FullName, kraF.FullName);
});

rootCommand.Subcommands.Add(p2kCommand);
rootCommand.Subcommands.Add(k2pCommand);


rootCommand.SetAction(parseResult =>
{
    NoParamDo();
    return 0;
});

ParseResult parseResult = rootCommand.Parse(args);
return parseResult.Invoke();



static void NoParamDo()
{
    ConsoleKeyInfo mode;
    do
    {
        string pr = """

        1: Psd to Kra
        2: Kra to Psd
        """;
        Console.WriteLine(pr);
        mode = Console.ReadKey();
    }
    while (!(mode.Key == ConsoleKey.NumPad1 || mode.Key == ConsoleKey.NumPad2 || mode.Key == ConsoleKey.D1 || mode.Key == ConsoleKey.D2));


    #region 1. 找到当前文件夹下的所有psd kra文件并根据文件名分组整合为元组 （同名.psd,同名.kra）

    var files = Directory.GetFiles("./");


    // 待处理列表 元组内存储路径字符串
    List<(string Psd, string Kra)> waitList = [];
    waitList = [..files.Where(x => x.EndsWith(".psd"))
                    .Join(files,
                          psd => psd.Replace(".psd", ".kra"),
                          kra => kra,
                          (psd, kra) => (psd, kra))];

    Console.WriteLine(string.Join(",", waitList));


    #endregion

    #region 2. 循环元组列表，挨个处理文件

    foreach (var (Psd, Kra) in waitList)
    {
        if (mode.Key == ConsoleKey.D1 || mode.Key == ConsoleKey.NumPad1)
            P2K.Do(Psd, Kra);
        else
            K2P.Do(Psd, Kra);
    }
    #endregion
}

