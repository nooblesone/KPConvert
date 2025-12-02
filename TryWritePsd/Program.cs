using System;
using System.Buffers.Binary;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization.Formatters;
using System.Text;
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
layerArea = Funnnn(layerArea, 0, 0);
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





static byte[] Funnnn(byte[] layerArea, int start, int end)
{
    var sadf = layerArea[22..24];
    var channelsC = BinaryPrimitives.ReadInt16BigEndian(sadf);
    var index = 6 + 16 + 2 + 6 * channelsC + 4 + 4 + 1;
    layerArea[index] = 1;



    return layerArea;
}