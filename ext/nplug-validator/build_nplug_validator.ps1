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
& "$PSScriptRoot/../CMake-Build-Platforms.ps1" -bit32 $false -CMakeConfig Release -CMakeRelativeBuildFolder vst3sdk-prefix/src/vst3sdk-build/bin