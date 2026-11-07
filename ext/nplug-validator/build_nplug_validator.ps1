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
$ErrorActionPreference = "Stop"
$cmake_relative_build_folder = "vst3sdk-prefix/src/vst3sdk-build"
if ($IsWindows) {
    $cmake_relative_build_folder = "$cmake_relative_build_folder/bin"
}
else {
    $cmake_relative_build_folder = "$cmake_relative_build_folder/lib"
}

& "$PSScriptRoot/../CMake-Build-Platforms.ps1" -bit32 $false -CMakeConfig Release -CMakeRelativeBuildFolder $cmake_relative_build_folder
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

# An ExternalProject must inherit the cross-compilation toolchain, not just the RID folder name.
if ($IsLinux) {
    foreach ($rid in @("linux-x64", "linux-arm64")) {
        $libraryPath = Join-Path $PSScriptRoot "build/package/$rid/native/libnplug_validator.so"
        $header = [System.IO.File]::ReadAllBytes($libraryPath)
        if ($header.Length -lt 20 -or $header[0] -ne 0x7F -or $header[1] -ne 0x45 -or
            $header[2] -ne 0x4C -or $header[3] -ne 0x46 -or $header[4] -ne 2 -or $header[5] -ne 1) {
            throw "Expected a 64-bit little-endian ELF library: $libraryPath"
        }
        $expectedMachine = if ($rid -eq "linux-arm64") { 183 } else { 62 } # EM_AARCH64 / EM_X86_64
        $machine = [System.BitConverter]::ToUInt16($header, 18)
        if ($machine -ne $expectedMachine) {
            throw "Wrong architecture for ${rid}: ELF machine $machine, expected $expectedMachine"
        }
    }
}
