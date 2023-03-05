# NPlug [![Build Status](https://github.com/xoofx/NPlug/workflows/ci/badge.svg?branch=main)](https://github.com/xoofx/NPlug/actions) [![NuGet](https://img.shields.io/nuget/v/NPlug.svg)](https://www.nuget.org/packages/NPlug/)

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/xoofx/NPlug/main/img/NPlug.png">

NPlug is a library that allows to easily develop [VST3](https://steinbergmedia.github.io/vst3_dev_portal/pages/) audio native plugins in .NET using NET7+ NativeAOT.

> **What is VST?**
>
> _Virtual Studio Technology (VST) is an audio plug-in software interface that facilitates the integration of software synthesizers and effects in digital audio workstations (DAW)._

## Features

- Purely managed, fast interop, no C++/CLI.
- Compatible with NET7+ NativeAOT
  - Build a native VST3 plugin with NPlug with zero dependencies!
- Exposes the interfaces from VST3 version `3.7.7`.
- Provides builtin support for synchronizing automatically the data model between the `AudioProcessor` and `AudioController`.
- Supports multiple platforms: `win-x64`, `win-arm64`, `osx-x64`, `osx-arm64`, `linux-x64`, `linux-arm64`
  - Please notice that `osx` full native supports will be only possible with .NET8+
- Provides the official VST3 Validator to unit test your plugin developed with NPlug.

## User Guide

The official documentation for VST3 is https://steinbergmedia.github.io/vst3_dev_portal/pages/

For more details on how to use NPlug, please visit the [user guide](https://github.com/xoofx/NPlug/blob/main/doc/readme.md).

## License

The core part of this software is released under the [BSD-2-Clause license](https://opensource.org/licenses/BSD-2-Clause) but you have also to follow the following VST3 license:

> **NOTICE**
> 
> When you are developing a plugin with NPlug, your plugin needs to comply with the [VST 3 Licensing](https://steinbergmedia.github.io/vst3_dev_portal/pages/VST+3+Licensing/Index.html). If your plugin is distributed, it needs to either be published under:
> - The [Proprietary Steinberg VST 3 license](https://steinbergmedia.github.io/vst3_dev_portal/pages/VST+3+Licensing/What+are+the+licensing+options.html#proprietary-steinberg-vst-3-license) if you want to keep your plugin closed source.
> - The [Open-source GPLv3 license](https://steinbergmedia.github.io/vst3_dev_portal/pages/VST+3+Licensing/What+are+the+licensing+options.html#open-source-gplv3-license) if you want to make your plugin OSS.

What it means is that you are allowed to modify and redistribute NPlug (according to the `BSD-2-Clause` license) but you need to publish your plugin under the VST3 dual-license.

## Author

Alexandre Mutel aka [xoofx](https://xoofx.com).
