#! /usr/bin/env pwsh
#
# Build script automation a CMake shared library, building the following platforms:
#
# On Windows:
#   win-x86
#   win-x64
#   win-arm
#   win-arm64
#
# On Linux:
#   linux-x64
#   linux-arm
#   linux-arm64
#
# On Mac:
#   osx-x64
#   osx-arm64
# -------------------------------------------------------------
param(
    [bool] $bit32 = $true,
    [String] $CMakeSource = ".",
    [String] $CMakeConfig = "Release",
    [String[]] $CMakeArgs = @(),
    [String] $BuildFolder = "build",
    [String] $CMakeRelativeBuildFolder = ".",
    [String] $CMakeExePath = "cmake"
)

$ErrorActionPreference = "Stop"
$PSNativeCommandArgumentPassing = "Standard"

Try {

# Common function used for building x86/x64/arm/arm64
function Build-Project {

    param (
        $NETArch
    )

    # Setup the correct build system and outputs based on platform
    $NETPlatform = "linux"
    $NETSharedLibExtension = "so"
    $CMakeBuilder = "Unix Makefiles"
    $LocalCMakeArgs = $CMakeArgs
    $BuildPlatformSubFolder = $CMakeRelativeBuildFolder
    if ($IsMacOS) {
        $NETPlatform = "osx"
        $NETSharedLibExtension = "dylib"
        $LocalCMakeArgs += "-DCMAKE_BUILD_TYPE=$CMakeConfig"
    }
    elseif ($IsWindows) {
        $MsvcArch = $NETArch
        if ($MsvcArch -eq "x86") {
            $MsvcArch = "win32"
        }
        $NETPlatform = "win"
        $NETSharedLibExtension = "dll"
        $CMakeBuilder = "Visual Studio 17 2022"
        $LocalCMakeArgs += "-A$MsvcArch"
        $BuildPlatformSubFolder = "$BuildPlatformSubFolder/$CMakeConfig"
    } elseif ($IsLinux) {
        $LocalCMakeArgs += "-DCMAKE_BUILD_TYPE=$CMakeConfig"
        if ($NETArch -eq "arm64") {
            $LocalCMakeArgs += "-DCMAKE_TOOLCHAIN_FILE=$PSScriptRoot/toolchains/aarch64-linux-gnu.toolchain.cmake"
        }
        elseif ($NETArch -eq "arm") {
            $LocalCMakeArgs += "-DCMAKE_TOOLCHAIN_FILE=$PSScriptRoot/toolchains/arm-linux-gnueabihf.toolchain.cmake"
        }
    }

    Write-Host "Building $NETPlatform-$NETArch $CMakeConfig" -ForegroundColor Green

    $DotNetRid = "$NETPlatform-$NETArch"
    $LocalCMakeArgs += "-DDOTNET_RID=$DotNetRid"
    $BuildPlatformFolder = "$BuildFolder/$DotNetRid"
    $PackageFolder = "$BuildFolder/package/$DotNetRid/native/"

    #trace-command -PSHOST -Name ParameterBinding { & $CMakeExePath -G $CMakeBuilder -B $BuildPlatformFolder -DDOTNET_RID="$DotNetRid" $CMakeArch @LocalCMakeArgs $CMakeSource  }
    trace-command -PSHOST -Name ParameterBinding { & $CMakeExePath -G $CMakeBuilder -B $BuildPlatformFolder @LocalCMakeArgs $CMakeSource  }    
    if ($LastExitCode -ne 0) {
        throw "error with cmake"
    }
    trace-command -PSHOST -Name ParameterBinding { & $CMakeExePath --build $BuildPlatformFolder --config $CMakeConfig }
    if ($LastExitCode -ne 0) {
        throw "error with cmake --build"
    }

    New-Item -type Directory -Path "$PackageFolder" -Force
    Copy-Item "$BuildPlatformFolder/$BuildPlatformSubFolder/*.$NETSharedLibExtension" -Destination $PackageFolder
}

if (Test-Path "$BuildFolder") {
    Write-Host "Removing folder $BuildFolder"
    # Remove-Item -Path "$BuildFolder" -Recurse
}

if ($bit32 -And $IsWindows) {
    Build-Project x86
}

if ($IsWindows -Or $IsMacOS -Or $IsLinux) {
    Build-Project x64
}

if ($IsWindows -Or $IsLinux) {
    if ($bit32) {
        Build-Project arm
    }
    Build-Project arm64
}

if ($IsMacOS) {
    Build-Project arm64
}

} Catch {
    $message = $_.Exception | Out-String
    $line = $_.InvocationInfo.ScriptLineNumber
    Write-Host "Error at line $line : $message" -ForegroundColor Red
    exit 1
}
