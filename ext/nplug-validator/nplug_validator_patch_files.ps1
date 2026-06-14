param(
    [Parameter(mandatory=$true)][string] $sdk_folder
)

function Patch-File {
    param(
        [Parameter(mandatory=$true)][string] $file_to_patch,
        [Parameter(mandatory=$true)][string] $content_to_append
    )    

    if (Test-Path $file_to_patch -PathType leaf) {
        if ((Select-String -Path $file_to_patch -Pattern "$content_to_append" -SimpleMatch).Count -eq 0) {
            Add-Content $file_to_patch "`n$content_to_append"
        }
    } else {
        #nothing to patch
    }
}

function Patch-FileAfterRegex {
    param(
        [Parameter(mandatory=$true)][string] $file_to_patch,
        [Parameter(mandatory=$true)][string] $pattern,
        [Parameter(mandatory=$true)][string] $content_to_insert,
        [Parameter(mandatory=$true)][string] $already_patched_pattern
    )

    if (Test-Path $file_to_patch -PathType leaf) {
        $content = Get-Content -Path $file_to_patch -Raw
        if ($content -match $already_patched_pattern) {
            return
        }

        $match = [regex]::Match($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
        if (-not $match.Success) {
            throw "Could not find patch location in $file_to_patch"
        }

        $new_content = $content.Insert($match.Index + $match.Length, "`n$content_to_insert")
        Set-Content -Path $file_to_patch -Value $new_content -NoNewline
    } else {
        #nothing to patch
    }
}

# Patch CMakeLists.txt files to allow static linking
Patch-File "$sdk_folder/public.sdk/samples/vst-hosting/validator/CMakeLists.txt" 'include(source/nplug_validator.cmake)'

$vst_sdk_project_pattern = 'project\s*\(\s*vstsdk\s+VERSION\s+[0-9.]+\s+DESCRIPTION\s+"Steinberg VST 3 Software Development Kit"\s+HOMEPAGE_URL\s+"https://www\.steinberg\.net"\s*\)'
$vst_sdk_apple_languages_patch = @'
# The macOS validator targets include .mm sources. The upstream VST3 SDK
# project does not enable Objective-C/C++ for Makefile/Ninja generators, which
# leaves CMAKE_OBJCXX_COMPILE_OBJECT unset during CMake generation.
if(APPLE)
    enable_language(OBJC OBJCXX)
endif()
'@

Patch-FileAfterRegex "$sdk_folder/CMakeLists.txt" $vst_sdk_project_pattern $vst_sdk_apple_languages_patch 'enable_language\s*\(\s*OBJC\s+OBJCXX\s*\)'
