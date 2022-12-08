cmake_minimum_required(VERSION 3.16)
project(nplug_proxy)
include_directories(.)

message(STATUS "Variable DOTNET_RID = ${DOTNET_RID}")
message(STATUS "Variable DOTNET_PACK_FOLDER = ${DOTNET_PACK_FOLDER}")
message(STATUS "Variable DOTNET_PACK_VERSION = ${DOTNET_PACK_VERSION}")

set(DOTNET_HOST_DIR "${DOTNET_PACK_FOLDER}/Microsoft.NETCore.App.Host.${DOTNET_RID}/${DOTNET_PACK_VERSION}/runtimes/${DOTNET_RID}/native")

message(STATUS "Variable DOTNET_HOST_DIR = ${DOTNET_HOST_DIR}")
include_directories(${DOTNET_HOST_DIR})
link_directories(${DOTNET_HOST_DIR})
# Compile nethost as a static library
add_definitions(-DNETHOST_USE_AS_STATIC)

if(MSVC)
    # use static linking with CRT
    set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
endif()

add_library(nplug_proxy SHARED plugin.cpp)
target_link_libraries(nplug_proxy PRIVATE libnethost.lib)