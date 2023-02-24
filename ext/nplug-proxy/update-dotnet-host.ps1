#! /usr/bin/env pwsh

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

function DownloadSDK([string]$OS, [string]$Architecture) {
    $aka_ms_link = "https://aka.ms/dotnet/STS/"

    # We download only the x64 version, as it should contain all the other architectures
    $dotnet_sdk_file = "dotnet-sdk-$OS-$Architecture"
    if ($OS -eq "win") {
        $dotnet_sdk_file += ".zip"
    }
    else {
        $dotnet_sdk_file += ".tar.gz"
    }
    $aka_ms_link += $dotnet_sdk_file

    if (!(Test-Path -Path $dotnet_sdk_file)) {
        Write-Output "Downloading $dotnet_sdk_file..."
        Invoke-WebRequest $aka_ms_link -OutFile $dotnet_sdk_file
    }

    $tmpsdk = "tmp/$OS-$Architecture"
    if (!(Test-Path -Path $tmpsdk)) {
        New-Item -ItemType Directory -Force -Path $tmpsdk | Out-Null

        if ($OS -eq "win") {
            Expand-Archive $dotnet_sdk_file -DestinationPath $tmpsdk -Force
        }
        else {
            & tar xof $dotnet_sdk_file -C $tmpsdk
        }
    }

    New-Item -ItemType Directory -Force -Path "include" | Out-Null

    # $dotnet\packs\Microsoft.NETCore.App.Host.win-arm64\7.0.3\runtimes\win-arm64\native
    $dotnet_version = (Get-ChildItem "$tmpsdk\packs\Microsoft.NETCore.App.Host.$OS-$Architecture\")[0].Name

    Copy-Item "$tmpsdk\packs\Microsoft.NETCore.App.Host.$OS-$Architecture\$dotnet_version\runtimes\$OS-$Architecture\native\*.h" -Destination "include"

    $libPath = "lib/$OS-$Architecture"
    New-Item -ItemType Directory -Force -Path $libPath | Out-Null

    Copy-Item "$tmpsdk\packs\Microsoft.NETCore.App.Host.$OS-$Architecture\$dotnet_version\runtimes\$OS-$Architecture\native\libnethost.*" -Destination $libPath
}

DownloadSDK -OS "win" -Architecture "x64"
DownloadSDK -OS "win" -Architecture "arm64"

DownloadSDK -OS "osx" -Architecture "x64"
DownloadSDK -OS "osx"  -Architecture "arm64"

DownloadSDK -OS "linux" -Architecture "x64"
DownloadSDK -OS "linux" -Architecture "arm64"

Remove-Item "tmp" -Recurse -Force
