// Copyright (c) 2024 Delta Devs. All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to use
// the Software for personal, educational, and research purposes, including the
// rights to use, copy, modify, merge, publish, distribute copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// The Software is provided "as is", without warranty of any kind, express or implied,
// including but not limited to the warranties of merchantability, fitness for a particular
// purpose and noninfringement. In no event shall the authors or copyright holders be liable
// for any claim, damages or other liability, whether in an action of contract, tort or
// otherwise, arising from, out of or in connection with the Software or the use or other
// dealings in the Software.
// 
// Distribution and/or publication of the Software, modified or unmodified, to the public
// is strictly prohibited.
// 
// Developed by Pavel Kalaš 2024-present.

#include "pch.h"
#include "NamedPipeServer.h"
#include <string>
#include <iostream>
#include <tlhelp32.h>

DWORD WINAPI EntryPoint(LPVOID param) {
    StartNamedPipeServer();
    FreeLibraryAndExitThread((HMODULE)param, 0);
}

bool isProcessRunning(const wchar_t* processName) {
    HANDLE hProcessSnap;
    PROCESSENTRY32W pe32;

    hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hProcessSnap == INVALID_HANDLE_VALUE) {
        std::cerr << "Error: CreateToolhelp32Snapshot failed." << std::endl;
        return false;
    }

    pe32.dwSize = sizeof(PROCESSENTRY32W);

    if (!Process32FirstW(hProcessSnap, &pe32)) {
        std::cerr << "Error: Process32First failed." << std::endl;
        CloseHandle(hProcessSnap);
        return false;
    }

    do {
        if (wcscmp(pe32.szExeFile, processName) == 0) {
            CloseHandle(hProcessSnap);
            return true;
        }
    } while (Process32NextW(hProcessSnap, &pe32));

    CloseHandle(hProcessSnap);
    return false;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD callReason, LPVOID lpReserved) {
    if (callReason == DLL_PROCESS_ATTACH) {
        CreateThread(0, 0, EntryPoint, hModule, 0, 0);

    }
    return TRUE;
}
