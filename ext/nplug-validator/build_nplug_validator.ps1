#! /usr/bin/env pwsh
#
# Build script for Steinberg VST validator shared library, building the following platforms:
#
# On Windows:
#   win-x64
#   win-arm64
#
# On Linux:
#   linux-x64
#   linux-arm64
#
# On Mac:
#   osx-x64
#   osx-arm64
# -------------------------------------------------------------
$cmake_relative_build_folder = "vst3sdk-prefix/src/vst3sdk-build"
if ($IsWindows) {
    $cmake_relative_build_folder = "$cmake_relative_build_folder/bin"
}
else {
    $cmake_relative_build_folder = "$cmake_relative_build_folder/lib"
}

& "$PSScriptRoot/../CMake-Build-Platforms.ps1" -bit32 $false -CMakeConfig Release -CMakeRelativeBuildFolder $cmake_relative_build_folder