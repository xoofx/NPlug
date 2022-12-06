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

# Patch CMakeLists.txt files to allow static linking
Patch-File "$sdk_folder/public.sdk/samples/vst-hosting/validator/CMakeLists.txt" 'include(source/nplug_validator.cmake)'