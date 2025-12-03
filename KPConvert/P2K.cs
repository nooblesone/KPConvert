using PsdSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace KPConvert;

public class P2K
{
    public static void Do(string psdPath, string kraPath)
    {
        CheckFile(psdPath);
        CheckFile(kraPath);

        // 打开并定义各种需要的变量
        using var fileStream = File.Open(psdPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 扫描psd
        string filename = Path.GetFileNameWithoutExtension(psdPath);
        // 临时解压kra文件夹路径
        string tempPath = @$"./{filename}_converted";
        // 解压后的 maindoc.xml 路径
        string maindocPath = @$"{tempPath}/maindoc.xml";
        // 生成的 kra 文件路径
        string resultKraFile = @$"{tempPath}.kra";
        var psdFile = PsdFile.Open(fileStream);

        // 解压 kra 文件
        ZipFile.ExtractToDirectory(kraPath, tempPath);

        // 读取 maindoc.xml
        XDocument rootDoc = XDocument.Load(maindocPath);
        // 筛掉 psd 中的图层组结束图层
        var psdLayers = psdFile.Layers.Where(x => x.Name != "</Layer group>").Reverse().ToList();
        // 获取 kra 文件的图层列表
        var kraLayers = rootDoc.Descendants().Where(x => x.Name.LocalName == "layer").ToList();

        // psd文件的编码不可控，暂不使用图层名检查
        /**
        for (int i = 0; i < psdLayers.Count; i++)
        {
            //var b = Encoding.Latin1.GetBytes(psdLayers[i].Name);
            //string psdName = Encoding.UTF8.GetString(b);

            string psdName = psdLayers[i].Name;
            var b = Encoding.UTF8.GetBytes(kraLayers[i].Attribute("name")!.Value);
            string kraName = Encoding.Latin1.GetString(b);
            //string kraName = kraLayers[i].Attribute("name")!.Value;
            Console.WriteLine($"{psdName} {kraName} {psdName == kraName}");
        }
        */
        // 检查图层数量是否相同
        if (psdLayers.Count != kraLayers.Count)
        {
            Console.Error.WriteLine($"{psdPath} 与 {kraPath} 图层数不同，处理失败");
            return;
        }

        // 使用了剪切蒙版的图层列表
        List<List<XElement>> cilpGroups = [];

        List<XElement> cilpGroup = [];
        // 找到使用剪切蒙版的图层组
        bool flag = false;
        for (int i = 0; i < psdLayers.Count; i++)
        {
            var psdLayer = psdLayers[i];
            var kraLayer = kraLayers[i];

            if (psdLayer.Clipping || flag)
            {
                flag = true;
                // 为剪切蒙版图层添加锁定透明度
                if (psdLayer.Clipping)
                {
                    kraLayer.SetAttributeValue("channelflags", "1110");
                }

                cilpGroup.Add(kraLayer);
                if (!psdLayer.Clipping)
                {
                    flag = false;
                    cilpGroups.Add(cilpGroup);
                    cilpGroup = [];
                }
            }
        }

        // 为筛出的图层套图层组
        foreach (var item in cilpGroups)
        {
            XElement f = item.First();
            f.AddBeforeSelf(GetKraGroup(item));
            foreach (var item2 in item)
            {
                item2.Remove();
            }
        }
        // 保存 maindoc.xml
        rootDoc.Save(maindocPath);
        // 导出结果文件
        ZipFile.CreateFromDirectory(tempPath, resultKraFile, CompressionLevel.Fastest, false);
        // 删除临时文件夹
        Directory.Delete(tempPath, true);

        Console.Out.WriteLine($"转换成功，文件 {resultKraFile}");

    }

    private static void CheckFile(string psdPath)
    {
        if (!File.Exists(psdPath))
        {
            throw new Exception($"文件 {psdPath} 不存在");
            throw new Exception($"Can't find {psdPath}");
        }
    }


    // 生成 kra xml 图层组的 XElement 并放入child，将最下层图层的混合模式应用的图层组
    static XElement GetKraGroup(List<XElement> child)
    {
        var xml = $$"""
        <layer uuid="{{{Guid.NewGuid()}}}" collapsed="1"
        compositeop="{{child.Last().Attribute("compositeop")!.Value}}" locked="0" y="0" x="0" opacity="255" colorlabel="0" channelflags="" visible="1" nodetype="grouplayer" filename="layer12"
        name="group_{{child.Last().Attribute("name")!.Value}}" intimeline="0" passthrough="0">
        <layers></layers>
        </layer>
        """;
        var result = XElement.Parse(xml);
        result.Element("layers")!.Add(child);
        return result;
    }

}
