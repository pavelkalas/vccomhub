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
#include <windows.h>
#include <iostream>
#include <string>
#include <locale>
#include <codecvt>
#include <tlhelp32.h>
#include <tchar.h>
#include <algorithm>
#include <cctype>

typedef void(__cdecl* _CNS_AddTxt)(char*);
_CNS_AddTxt CNS_AddTxt;

typedef void(__cdecl* _CNS_AddCmdTxt)(char*);
_CNS_AddCmdTxt CNS_AddCmdTxt;

typedef void(__cdecl* _CNS_Clear)(void);
_CNS_Clear CNS_Clear;

typedef void(__cdecl* _CNS_Open)(int);
_CNS_Open CNS_Open;

typedef void(__cdecl* _CNS_Close)(int);
_CNS_Close CNS_Close;

char* toPointerChar(const std::string& str) {
    size_t buffer_size = str.size() + 1;
    char* buffer = new char[buffer_size];

    errno_t err = memcpy_s(buffer, buffer_size, str.c_str(), str.size());

    if (err == 0) {
        buffer[str.size()] = '\0';
        return buffer;
    }
    else {
        delete[] buffer;
        return nullptr;
    }
}

std::string wcharToString(const wchar_t* wstr)
{
    std::wstring wstring(wstr);
    std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> converter;
    return converter.to_bytes(wstring);
}

std::string getLastErrorAsString()
{
    DWORD errorMessageID = ::GetLastError();
    if (errorMessageID == 0)
        return std::string("No error");

    LPSTR messageBuffer = nullptr;
    size_t size = FormatMessageA(
        FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        errorMessageID,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPSTR)&messageBuffer,
        0,
        NULL
    );

    std::string message(messageBuffer, size);
    LocalFree(messageBuffer);
    return message;
}

DWORD GetProcessIdByName(const TCHAR* processName) {
    DWORD processId = 0;
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (snapshot != INVALID_HANDLE_VALUE) {
        PROCESSENTRY32 processEntry{};
        processEntry.dwSize = sizeof(PROCESSENTRY32);
        if (Process32First(snapshot, &processEntry)) {
            do {
                if (_tcsicmp(processEntry.szExeFile, processName) == 0) {
                    processId = processEntry.th32ProcessID;
                    break;
                }
            } while (Process32Next(snapshot, &processEntry));
        }
        CloseHandle(snapshot);
    }
    return processId;
}

std::string wcharToUTF8(const wchar_t* wcharString)
{
    int charLen = WideCharToMultiByte(CP_UTF8, 0, wcharString, -1, NULL, 0, NULL, NULL);
    if (charLen == 0) {
        return "";
    }

    char* charString = new char[charLen];
    WideCharToMultiByte(CP_UTF8, 0, wcharString, -1, charString, charLen, NULL, NULL);

    std::string result(charString);

    delete[] charString;

    return result;
}

std::string filterViolatedSymbols(const std::string& str)
{
    std::string filtered;
    for (char ch : str)
    {
        if (isalnum(ch) || isspace(ch) || ispunct(ch))
        {
            filtered += ch;
        }
    }
    return filtered;
}

std::string trim(const std::string& str)
{
    size_t first = str.find_first_not_of(" \t\n\r");
    if (std::string::npos == first)
    {
        return "";
    }
    size_t last = str.find_last_not_of(" \t\n\r");
    return str.substr(first, (last - first + 1));
}

bool replace(std::string& str, const std::string& from, const std::string& to) {
    size_t start_pos = str.find(from);
    if (start_pos == std::string::npos)
        return false;
    str.replace(start_pos, from.length(), to);
    return true;
}

void StartNamedPipeServer()
{
    HMODULE gameModule = LoadLibrary(L"logs.dll");

    if (!gameModule)
    {
        return;
    }

    CNS_AddTxt = (_CNS_AddTxt)GetProcAddress(gameModule, "?CNS_AddTxt@@YAXPAD@Z");
    CNS_AddCmdTxt = (_CNS_AddCmdTxt)GetProcAddress(gameModule, "?CNS_AddCmdTxt@@YAXPAD@Z");
    CNS_Clear = (_CNS_Clear)GetProcAddress(gameModule, "?CNS_Clear@@YAXXZ");
    CNS_Open = (_CNS_Open)GetProcAddress(gameModule, "?CNS_Open@@YAXH@Z");
    CNS_Close = (_CNS_Close)GetProcAddress(gameModule, "?CNS_Close@@YAXH@Z");

    if (!CNS_AddTxt || !CNS_AddCmdTxt || !CNS_Clear || !CNS_Open || !CNS_Close)
    {
        return;
    }

    DWORD processID;
    GetWindowThreadProcessId(GetForegroundWindow(), &processID);
    std::string pipeName = "\\\\.\\pipe\\" + std::to_string(processID);

    HANDLE hPipe = INVALID_HANDLE_VALUE;

    while (true)
    {
        hPipe = CreateNamedPipeA(
            pipeName.c_str(),             
            PIPE_ACCESS_DUPLEX,           
            PIPE_TYPE_MESSAGE |           
            PIPE_READMODE_MESSAGE |       
            PIPE_WAIT,
            1,
            4096,                         
            4096,                         
            NMPWAIT_USE_DEFAULT_WAIT,     
            NULL                          
        );

        if (hPipe == INVALID_HANDLE_VALUE)
        {
            return;
        }

        if (ConnectNamedPipe(hPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED))
        {
            DWORD bytesRead = 0;
            char buffer[4096] = { 0 };

            while (true)
            {
                BOOL result = ReadFile(
                    hPipe,
                    buffer,
                    sizeof(buffer) - 1,
                    &bytesRead,
                    NULL
                );

                if (!result || bytesRead == 0)
                {
                    break;
                }

                buffer[bytesRead] = '\0';

                std::string output(buffer);
                output = trim(output);
                output = filterViolatedSymbols(output);

                Sleep(20);

                if (output.find("cmd+") != std::string::npos) {
                    output = output.substr(4);
                    CNS_AddCmdTxt(toPointerChar(output.c_str()));
                }
                else if (output.find("clear+") != std::string::npos) {
                    CNS_Clear();
                }
                else if (output.find("open+") != std::string::npos) {
                    CNS_Open(1);
                }
                else if (output.find("close+") != std::string::npos) {
                    CNS_Close(1);
                }
                else {
                    CNS_AddTxt(toPointerChar(output.c_str()));
                }
            }
        }

        DisconnectNamedPipe(hPipe);
        CloseHandle(hPipe);
    }
}