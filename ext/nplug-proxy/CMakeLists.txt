cmake_minimum_required(VERSION 3.16)
project(nplug_proxy)
include_directories(.)

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib/${CMAKE_BUILD_TYPE})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin/${CMAKE_BUILD_TYPE})

message(STATUS "Variable DOTNET_RID = ${DOTNET_RID}")

if(APPLE)
  if("${DOTNET_RID}" STREQUAL "osx-x64")
    SET(CMAKE_OSX_ARCHITECTURES "x86_64")
  else()
    SET(CMAKE_OSX_ARCHITECTURES "arm64")
  endif()
  message(STATUS "Architecture CMAKE_OSX_ARCHITECTURES = ${CMAKE_OSX_ARCHITECTURES}")
endif(APPLE)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)
link_directories(${CMAKE_CURRENT_SOURCE_DIR}/lib/${DOTNET_RID})

# Compile nethost as a static library
add_definitions(-DNETHOST_USE_AS_STATIC)

if(MSVC)
    # use static linking with CRT
    set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
    #set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Release>:Relesae>")
endif()

add_library(nplug_proxy SHARED plugin.cpp)

if(MSVC)
    target_link_libraries(nplug_proxy PRIVATE libnethost.lib)
else()
    target_link_libraries(nplug_proxy PRIVATE nethost)
endif()
