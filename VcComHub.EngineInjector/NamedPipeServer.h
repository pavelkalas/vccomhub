#pragma once

#ifdef NAMEDPIPESERVER_EXPORTS
#define NAMEDPIPESERVER_API __declspec(dllexport)
#else
#define NAMEDPIPESERVER_API __declspec(dllimport)
#endif

extern "C" NAMEDPIPESERVER_API void StartNamedPipeServer();
