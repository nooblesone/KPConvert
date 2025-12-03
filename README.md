# KPConvert: A small tool for handling compatibility issues between Krita's alpha inheritance and PSD clipping masks

## Demo Video
Outdated, please wait for an update
https://www.bilibili.com/video/BV1FNSFB9ELK

# Showcase
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203183519.png)
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203184740.png)

# Quick Start

## PSD to KRA

1. Open the PSD file you want to convert with Krita
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/image.png)

2. Save as a KRA file, in the same directory as the PSD file

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/image-1.png)

3. Drag `KPConvert.exe` into this directory, double-click to run, and input `1`

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203181503.png)

After the program exits, you will see that a `filename_converted.kra` file has been generated.

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/zoomit.png)

Now you can double-click the generated `filename_converted.kra` file to view it. You will notice it has added alpha inheritance and layer groups based on the original PSD file content, and applied the same blend mode as the clipping mask on the layer groups.

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251130000724.png)

Illustration by: 墨帧Studio <a>https://space.bilibili.com/2137267072</a>


## KRA to PSD

1. Export your artwork from Krita as a PSD and place it in the same folder as your KRA file.
   
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203185529.png)

2. Drag `KPConvert.exe` into this directory, double-click to run, and input `2`.
   
![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203181503.png)

After the program exits, you will see that a `filename_converted.kra` file has been generated. Open it with software that supports clipping masks to view the result.

![alt text](https://raw.githubusercontent.com/nooblesone/nooblesone-img/main/image/20251203184740.png)


# Using Command Line Arguments

PSD to KRA:

```shell
$ ./KPConvert p2k --psd ./file.psd --kra ./file.kra
```


KRA to PSD:

```shell
$ ./KPConvert k2p --psd ./file.psd --kra ./file.kra
```

You can use  `./KPConvert --help`  `./KPConvert p2k --help` `./KPConvert k2p --help` to view the help information.

