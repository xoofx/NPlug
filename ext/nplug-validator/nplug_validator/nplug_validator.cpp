#include "validator.h"
#include <streambuf>
#include <iostream>

extern void* moduleHandle;
extern bool InitModule ();
extern bool DeinitModule ();

typedef void (__cdecl *FunctionOutputCharDelegate)(int);

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
__declspec(dllexport) void nplug_validator_initialize()
{
	InitModule ();
}

__declspec(dllexport) int nplug_validator_validate(int argc, char* argv[], FunctionOutputCharDelegate output, FunctionOutputCharDelegate error)
{
	auto result = NPlugValidator(argc, argv, output, error).run();
	return result;
}

__declspec(dllexport) void nplug_validator_destroy()
{
	DeinitModule ();
}

};