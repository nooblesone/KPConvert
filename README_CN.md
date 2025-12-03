# KPConvert: 一个简单的处理 krita 继承透明度(alpha inheritance)与psd 剪切蒙版兼容性问题的小工具

## 演示视频
已过时，请等待更新
https://www.bilibili.com/video/BV1FNSFB9ELK

# 效果展示
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203183519.png)
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203184740.png)

# 快速开始

## psd 转换 kra

1. 用 krita 打开需要转换的 psd 文件
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/image.png)

2. 另存为 kra 文件，保存在与 psd 文件同一目录

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/image-1.png)

3. 将 `KPConvert.exe` 拖入此目录，并双击运行，输入 `1`

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203181503.png)

程序退出，可以看到已经生成了 `filename_converted.kra` 文件

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/zoomit.png)

现在就可以双击生成的 `filename_converted.kra` 文件查看了，可以注意到它根据原psd文件内容添加了继承透明度和图层组，并在图层组上使用了和剪切蒙版相同的混合模式。

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251130000724.png)

插图作者: 墨帧Studio <a>https://space.bilibili.com/2137267072</a>


## kra 转换 psd

1. 用krita把你的画导出成psd，放到与你的kra文件同一文件夹
   
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203185529.png)

1. 将 `KPConvert.exe` 拖入此目录，并双击运行，输入 `2` 
   
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203181503.png)

程序退出，可以看到已经生成了 `filename_converted.kra` 文件，使用支持剪切蒙版的软件打开，查看结果

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203184740.png)


# 使用命令行参数

psd转换kra：

```shell
$ ./KPConvert p2k --psd ./file.psd --kra ./file.kra
```

kra转换psd：

```shell
$ ./KPConvert k2p --psd ./file.psd --kra ./file.kra
```

可以使用 `./KPConvert --help`  `./KPConvert p2k --help` `./KPConvert k2p --help` 查看帮助
