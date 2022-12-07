cmake_minimum_required(VERSION 3.16)
project(nplug_native_proxy)
include_directories(.)

if(MSVC)
    # use static linking with CRT
    set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
endif()

add_library(nplug_native_proxy SHARED plugin.cpp)
