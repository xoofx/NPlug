#include "validator.h"

extern void* moduleHandle;
extern bool InitModule ();
extern bool DeinitModule ();

extern "C" {
__declspec(dllexport) int validate(int argc, char* argv[])
{
	InitModule ();
	auto result = Steinberg::Vst::Validator (argc, argv).run ();
	DeinitModule ();
	return result;
}
};