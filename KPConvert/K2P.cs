using PsdSharp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace KPConvert;

internal class K2P
{
    public static void Do(string psdPath, string kraPath)
    {
        CheckFile(psdPath);
        CheckFile(kraPath);

        static void go(List<XElement> kraLayers, List<PsLayer> psLayers)
        {
            psLayers = [.. psLayers.Where(x => x.Name != "</Layer group>")];
            for (int i = 0; i < kraLayers.Count; i++)
            {
                var psLayer = psLayers[i];
                var kraLayer = kraLayers[i];

                if (kraLayer.Attribute("channelflags")!.Value == "1110")
                {
                    psLayer.Clipping = 1;
                }
            }
        }


        string filename = Path.GetFileNameWithoutExtension(psdPath);

        string outPsdFileName = $"./{filename}_converted.psd";

        using FileStream psfs = new(psdPath, FileMode.Open);

        using BinaryReader br = new(psfs);

        using KraOperator kraOperator = KraOperator.Open(kraPath, out XDocument kraDoc);

        using FileStream psoutfs = new(outPsdFileName, FileMode.CreateNew);
        using BinaryWriter bw = new(psoutfs);




        int headLength = 26;

        int colorModeDataSectionHeadLength = 4;

        int colorModeDataSectionLength;

        int imageResourcesSectionHeadLength = 4;

        int imageResourcesSectionLength;

        int layerandMaskInformationSectionHeadLength = 4;

        int layerandMaskInformationSectionLength;

        // 头部分
        var by = br.ReadBytes(headLength);
        bw.Write(by);


        // 
        var by2 = br.ReadBytes(colorModeDataSectionHeadLength);
        colorModeDataSectionLength = BinaryPrimitives.ReadInt32BigEndian(by2);
        bw.Write(by2);
        bw.Write(br.ReadBytes(colorModeDataSectionLength));

        var by3 = br.ReadBytes(imageResourcesSectionHeadLength);
        imageResourcesSectionLength = BinaryPrimitives.ReadInt32BigEndian(by3);
        bw.Write(by3);
        bw.Write(br.ReadBytes(imageResourcesSectionLength));


        // 主要处理的部分
        var by4 = br.ReadBytes(layerandMaskInformationSectionHeadLength);
        layerandMaskInformationSectionLength = BinaryPrimitives.ReadInt32BigEndian(by4);
        bw.Write(by4);
        var layerArea = br.ReadBytes(layerandMaskInformationSectionLength);

        var kraLayers = kraDoc.Descendants().Where(x => x.Name.LocalName == "layer").Reverse().ToList();
        var psLayers = ParseLayers(layerArea);
        go(kraLayers, psLayers);


        bw.Write(layerArea);


        int size = 64 * 1024;
        var buffer = new byte[size];
        int bytesRead;
        while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
        {
            bw.Write(buffer, 0, bytesRead);
        }


        Console.WriteLine($"success! file: {outPsdFileName}");

    }

    private static void CheckFile(string psdPath)
    {
        if (!File.Exists(psdPath))
        {
            throw new Exception($"文件 {psdPath} 不存在");
            throw new Exception($"Can't find {psdPath}");
        }
    }

    static List<PsLayer> ParseLayers(byte[] layerArea)
    {

        //var layerAndMaskInformationLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[0..4]);

        var index = 0;

        var layerInfoLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[index..(index + 4)]);
        index += 4;
        var layerCount = Math.Abs(BinaryPrimitives.ReadInt16BigEndian(layerArea.AsSpan()[index..(index + 2)]));
        index += 2;

        List<PsLayer> layers = new(layerCount);
        for (var i = 0; i < layerCount; i++)
        {
            index += 16;
            var channelsCount = BinaryPrimitives.ReadInt16BigEndian(layerArea.AsSpan()[index..(index + 2)]);
            index += 2;
            index = index + 6 * channelsCount + 4 + 4 + 1;
            var clippingIndex = index;
            var clipping = layerArea[clippingIndex];
            index++;
            index++;
            index++;
            var extraDataFieldLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[(index)..(index + 4)]);
            index += 4;
            var layerMaskDataLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[(index)..(index + 4)]);
            var layerBlendingRangesLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[(index + layerMaskDataLength)..(index + layerMaskDataLength + 4)]);
            var nameIndex = index + 4 + layerMaskDataLength + 4 + layerBlendingRangesLength;
            var nameLength = layerArea[nameIndex];
            var name = Encoding.Latin1.GetString(layerArea.AsSpan()[(nameIndex + 1)..(nameIndex + 1 + nameLength)]);
            var layer = new PsLayer
            {
                LayerBytes = layerArea,
                ClippingIndex = clippingIndex,
                Clipping = clipping,
                Name = name,
            };
            layers.Add(layer);
            index += extraDataFieldLength;
        }

        return layers;


    }
}



class PsLayer
{
    public required byte[] LayerBytes { get; init; }
    public required int ClippingIndex { get; init; }
    public required string Name { get; init; }
    /// <summary>
    /// Clipping: 0 = base, 1 = non-base
    /// </summary>
    public required byte Clipping
    {
        get; set
        {
            LayerBytes[ClippingIndex] = value;
            field = value;
        }
    }
}
