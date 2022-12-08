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

$result = & dotnet msbuild -nologo "$PSScriptRoot/NPlug.Proxy.msbuildproj" -t:Get_nplug_proxy_Target
if ($? -eq $false) {
    Write-Error "Calling msbuild failed"
    exit 1
}
$result = $result.Trim()
$DotNetPackInfo = $result.Split(";")

if ($DotNetPackInfo.Count -ne 3) {
    Write-Error "Invalid msbuild output: $result`nExpecting 3 items separated by ;";
    exit 1
}

Write-Host $DotNetPackInfo

$dotnet_pack_version = $DotNetPackInfo[1]
$dotnet_pack_folder = $DotNetPackInfo[2]

& "$PSScriptRoot/../../scripts/CMake-Build-Platforms.ps1" -bit32 $false -CMakeConfig RelWithDebInfo -CMakeArgs """-DDOTNET_PACK_FOLDER=$dotnet_pack_folder""","""-DDOTNET_PACK_VERSION=$dotnet_pack_version"""