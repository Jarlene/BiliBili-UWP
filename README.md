# BiliBili-UWP
## 简介
 这里是Bilibili第三方UWP客户端的Github项目；该客户端旨在为Windows™ 10用户创造最好的使用Bilibili各项服务的体验，同时支持PC和Mobile设备；最新的稳定版本为3.1.2，请前往Windows应用商店搜索“BilibiliUWP”。

## 代码说明
### 分支说明
 仓库中包含有若干分支；若无特殊声明或特殊情况，master分支中即为最新的稳定版本的代码，feature分支为测试中的代码，以feature_开头或以bugfix_开头的分支为正在开发的代码；除master分支外，均为不稳定版本。
### 编译
 代码编译需要Visual Studio 2015和Windows 10 SDK 14393或更高版本，且FFMpeg相关代码请自行获取编译；为便于管理，仓库不给出FFMpeg相关代码或编译结果。

## 由于不再提供应用程序捆绑包，故删除下述内容
## ~~安装发行包~~
### ~~证书安装~~
 ~~安全证书请安装在**“受信任人”**位置下，否则无效；相关方法请自行搜索。~~
### ~~发行包安装~~
 ~~发行包一般包含主安装包和依赖包，请先安装位于Dependencies目录下、对应计算机配置的依赖包，依次安装完毕，再安装主安装包；对于RS1或更高版本的用户来说，安装通过双击就可完成；否则需要PowerShell，请自行搜索“PowerShell安装Appx”。~~

## 反馈
 若master分支代码或Releases最新的发行包*（非master分支代码或非最新发行包的问题请不要反馈）*出现功能性问题或者崩溃现象，欢迎新建Issue或发送邮件至771102271@qq.com（逍遥橙子）或ThomasW.Fan@outlook.com（DotNet码农）进行反馈。
 
## 代码开放协议（MIT许可协议）
Copyright (C) <year> <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
