using System;
using System.Buffers.Binary;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
if (File.Exists("./test_converted.psd"))
    File.Delete("./test_converted.psd");
using FileStream psfs = new("./test.psd", FileMode.Open);

using BinaryReader br = new(psfs);

using FileStream psoutfs = new("./test_converted.psd", FileMode.CreateNew);
using BinaryWriter bw = new(psoutfs);


//for (int i = 0; i < 32; i++)
//{
//    var by = br.ReadBytes(4);
//    var bys = Encoding.Latin1.GetString(by);
//    Console.WriteLine(bys);
//}
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
var layers = GetLayers(layerArea);

foreach (var item in layers)
{
    item.Clipping = 1;
}


bw.Write(layerArea);


int size = 64 * 1024;
var buffer = new byte[size];
int bytesRead;
while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
{
    bw.Write(buffer, 0, bytesRead);
}





var ssgd = layerArea[0..4];

Console.WriteLine(string.Join(",", ssgd));

Console.WriteLine(string.Join(",", layerArea[4..6]));

var ssss = layerArea[4..6];


Console.WriteLine($"图层数： {BinaryPrimitives.ReadInt16BigEndian(ssss)}");







static List<PsLayer> GetLayers(byte[] layerArea)
{

    //var layerAndMaskInformationLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[0..4]);

    var index = 0;

    var layerInfoLength = BinaryPrimitives.ReadInt32BigEndian(layerArea.AsSpan()[index..(index + 4)]);
    index += 4;
    var layerCount = BinaryPrimitives.ReadInt16BigEndian(layerArea.AsSpan()[index..(index + 2)]);
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
        Console.WriteLine(layer.Name);
        layers.Add(layer);
        index += extraDataFieldLength;
    }

    return layers;


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
