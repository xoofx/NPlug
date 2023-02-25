#include "validator.h"
#include <streambuf>
#include <iostream>

extern void* moduleHandle;
extern bool InitModule ();
extern bool DeinitModule ();

#if defined(__GNUC__) || defined(__clang__)
    #define NPLUG_NATIVE_DLL_EXPORT __attribute__((__visibility__("default")))
    #define NPLUG_CDECL __attribute__((cdecl))
#elif defined(_MSC_VER) || defined(__INTEL_COMPILER)
    #define NPLUG_NATIVE_DLL_EXPORT __declspec(dllexport)
    #define NPLUG_CDECL __cdecl
#else
    #define NPLUG_NATIVE_DLL_EXPORT
    #define NPLUG_CDECL
#endif

typedef void (NPLUG_CDECL *FunctionOutputCharDelegate)(int);

class RedirectBuffer: public std::streambuf
{
public:
    RedirectBuffer(FunctionOutputCharDelegate output) : _output(output) {}
private:
    // This tee buffer has no buffer. So every character "overflows"
    // and can be put directly into the teed buffers.
    virtual int overflow(int c)
    {
        if (c == EOF)
        {
            return EOF;
        }
        else
        {
            _output(c);
            return c;
        }
    }
private:
    FunctionOutputCharDelegate _output;
};

class RedirectStream : public std::ostream
{
public:
    RedirectStream(FunctionOutputCharDelegate outputDelegate) : std::ostream(&_redirectBuffer), _redirectBuffer(outputDelegate) {}
private:
    RedirectBuffer _redirectBuffer;
};

class NPlugValidator : public Steinberg::Vst::Validator
{
public:
//------------------------------------------------------------------------
	NPlugValidator (int argc, char* argv[], FunctionOutputCharDelegate outputDelegate, FunctionOutputCharDelegate errorDelegate) :
        Validator(argc, argv),
        _outputFuncStream(outputDelegate),
        _errorFuncStream(errorDelegate)
    {
        infoStream = &_outputFuncStream;
        errorStream = &_errorFuncStream;
    }
	~NPlugValidator () override {}

private:
    RedirectStream _outputFuncStream;
    RedirectStream _errorFuncStream;
};

extern "C" {

NPLUG_NATIVE_DLL_EXPORT void NPLUG_CDECL nplug_validator_initialize()
{
	InitModule ();
}

NPLUG_NATIVE_DLL_EXPORT int NPLUG_CDECL nplug_validator_validate(int argc, char* argv[], FunctionOutputCharDelegate output, FunctionOutputCharDelegate error)
{
	auto result = NPlugValidator(argc, argv, output, error).run();
	return result;
}

NPLUG_NATIVE_DLL_EXPORT void NPLUG_CDECL nplug_validator_destroy()
{
	DeinitModule ();
}

};