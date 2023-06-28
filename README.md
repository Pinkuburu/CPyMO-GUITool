# CPyMO GUI Tool

这是一个CPyMO用户的GUI工具，包含以下功能：

* 启动特定CPyMO游戏
* 在桌面上创建CPyMO游戏的快捷方式
* 将CPyMO游戏转换到特定平台（s60v3/s60v5/pymo/3ds/psp/wii）
* 删除CPyMO游戏中无用的文件
* 将素材文件打包为PyMO PAK包
* 对PyMO PAK包进行解包

## 如何使用（在windows上）

### 下载

1. 选择适合你的版本
    * 如果你是x86或x64的Windows用户，可以下载.NET Framework 4.6.2版本
    * 如果你是ARM64的Windows用户，则可以下载.NET 6版本
1. 下载包后，启动`CPyMO-GUITool.exe`
1. 检查窗口最上方“cpymo路径”、“cpymo-tool路径”、“ffmpeg”路径可否被找到
    * 如果可以找到，则无视这一步
    * 如果“cpymo路径”找不到，则无法启动游戏和创建桌面快捷方式
        * 可以前往[CPyMO Release](https://github.com/Strrationalism/CPyMO/releases/latest)下载`CPyMO.for.Windows.x64.zip`、`CPyMO.for.Windows.x86.zip`或`CPyMO.for.Windows.ARM64.zip`中的适合你的电脑的的版本解压到`CPyMO-GUITool.exe`所在的文件夹下
    * 如果“cpymo-tool路径”找不到，则无法进行转换、精简、打包、解包操作
        * 安装方式同上，压缩包里应该会同时有`cpymo.exe`和`cpymo-tool.exe`
    * 如果“ffmpeg路径”找不到，则无法在转换游戏时转换音频，可能导致转换出的游戏在目标平台上没有声音
        * 前往[FFmpeg Releases](https://github.com/BtbN/FFmpeg-Builds/releases)下载适用于Windows的FFmpeg，并将压缩包中的exe和dll文件解压到`CPyMO-GUITool.exe`所在的文件夹下，确保`ffmpeg.exe`文件在该目录下

### 选择游戏

1. 如果你只是想打包或解包PyMO pak包，则完全不需要这一步
1. 在`CPyMO-GUITool`的主界面，点击“游戏文件夹”右边的“选择”按钮
1. 选择你要处理的游戏文件夹

### 转换游戏

1. 你需要先选择要处理的游戏
1. 在下方标签中选择“转换”
1. 选择你要转换的目标平台，仅以下平台需要转换，其他平台不需要：
    * 任天堂3DS
    * 任天堂Wii
    * 索尼PSP
    * PyMO原版（安卓、PC、塞班S60v3、塞班S60v5）
1. 点击“转换”按钮
1. 选择要输出的目标文件夹，注意你选择一个空的文件夹，转换后的游戏将会被生成到此处
1. 稍等即可转换完成

### 打包PyMO PAK包

1. 在下方标签中选择“打包”
1. 选择要打包的文件夹
1. 点击“打包”，选择要保存的PAK包位置和文件名
1. 稍等即可打包完成

### 解包PyMO PAK包

1. 在下方标签中选择“解包”
1. 选择要解包的PAK文件
1. 这时程序应该可以自动推导出包内文件的扩展名，如果扩展名不对，请修改它
1. 点击“解包”
1. 选择要解包到的目标位置
1. 稍等即可解包完成

