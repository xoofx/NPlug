set(target nplug_validator)

add_library(${target} SHARED ${validator_sources} source/nplug_validator.cpp)
if(SDK_IDE_HOSTING_EXAMPLES_FOLDER)
    set_target_properties(${target}
        PROPERTIES
            ${SDK_IDE_HOSTING_EXAMPLES_FOLDER}
    )
endif(SDK_IDE_HOSTING_EXAMPLES_FOLDER)
target_compile_features(${target}
    PUBLIC
        cxx_std_17
)
target_link_libraries(${target}
    PRIVATE
        sdk_hosting
)
smtg_target_codesign(${target})
smtg_target_setup_universal_binary(${target})
if(APPLE AND NOT XCODE)
    find_library(COCOA_FRAMEWORK Cocoa)
    target_link_libraries(${target}
        PRIVATE
            ${COCOA_FRAMEWORK}
    )
endif(APPLE AND NOT XCODE)
