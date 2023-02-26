cmake_minimum_required(VERSION 3.19)

project(nplug_validator)

message(STATUS "Variable DOTNET_RID = ${DOTNET_RID}")
if(APPLE)
  if("${DOTNET_RID}" STREQUAL "osx-x64")
    SET(CMAKE_OSX_ARCHITECTURES "x86_64")
  else()
    SET(CMAKE_OSX_ARCHITECTURES "arm64")
  endif()
  message(STATUS "Architecture CMAKE_OSX_ARCHITECTURES = ${CMAKE_OSX_ARCHITECTURES}")
endif(APPLE)

include(ExternalProject)

set(nplug_validator_source ${CMAKE_CURRENT_SOURCE_DIR}/nplug_validator)
set(nplug_validator_source_files 
    ${nplug_validator_source}/nplug_validator.cpp
    ${nplug_validator_source}/nplug_validator.cmake
)

set(VST_SDK_FOLDER ${CMAKE_CURRENT_SOURCE_DIR}/../vst3sdk)

if(NOT EXISTS ${VST_SDK_FOLDER})
    set(GIT_REPOSITORY https://github.com/steinbergmedia/vst3sdk.git)
    set(GIT_TAG v3.7.7_build_19)
endif()

ExternalProject_Add(vst3sdk
    GIT_REPOSITORY    ${GIT_REPOSITORY}
    GIT_TAG           ${GIT_TAG}
    SOURCE_DIR ${VST_SDK_FOLDER}
    CMAKE_ARGS -DSMTG_CREATE_PLUGIN_LINK=0 -DSMTG_USE_STATIC_CRT=1 -DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=${CMAKE_OSX_ARCHITECTURES} -DSMTG_BUILD_UNIVERSAL_BINARY=0
    BUILD_COMMAND ${CMAKE_COMMAND} --build . --config Release --target nplug_validator
    INSTALL_COMMAND ""
    TEST_COMMAND ""
    PATCH_COMMAND ${CMAKE_COMMAND} -E copy_if_different ${nplug_validator_source_files} ${VST_SDK_FOLDER}/public.sdk/samples/vst-hosting/validator/source
    COMMAND pwsh ${CMAKE_CURRENT_SOURCE_DIR}/nplug_validator_patch_files.ps1 -sdk_folder ${VST_SDK_FOLDER}
)
