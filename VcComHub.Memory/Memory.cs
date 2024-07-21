using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

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

namespace VcComHub.Memory
{
    /// <summary>
    /// Tato třída slouží pro injecting DLL souborů a práci s pamětí.
    /// </summary>
    public class Memory
    {
        [DllImport("kernel32.dll")] private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)] private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")] private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true)][ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)][SuppressUnmanagedCodeSecurity][return: MarshalAs(UnmanagedType.Bool)] private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        const uint PROCESS_VM_READ = 0x0010;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_ALL_ACCESS = 0x001F0FFF;

        const uint PAGE_EXECUTE_READWRITE = 0x40;

        /// <summary>
        /// Injectne dll soubor do procesu.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="dllPath"></param>
        public static void InjectDllToProcess(Process processHandle, string dllPath)
        {
            IntPtr handle = OpenProcess(0x001F0FFF, false, processHandle.Id);
            IntPtr LibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr AllocatedMemory = VirtualAllocEx(handle, IntPtr.Zero, (uint)dllPath.Length + 1, 0x00001000, 4);
            WriteProcessMemory(handle, AllocatedMemory, Encoding.Default.GetBytes(dllPath), (uint)dllPath.Length + 1, out _);

            IntPtr threadHandle = CreateRemoteThread(handle, IntPtr.Zero, 0, LibraryAddress, AllocatedMemory, 0, IntPtr.Zero);

            WaitForSingleObject(handle, 200);
            CloseHandle(threadHandle);
            VirtualFreeEx(handle, AllocatedMemory, dllPath.Length + 1, 0x8000);
            CloseHandle(handle);
        }

        /// <summary>
        /// Zapíše string do paměti na určitou adresu.
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="address"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool WriteStringToAddress(int processId, string moduleName, int offset, string text, int size)
        {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open process.");
                return false;
            }

            Process process = Process.GetProcessById(processId);
            IntPtr moduleBase = IntPtr.Zero;
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    moduleBase = module.BaseAddress;
                    break;
                }
            }

            if (moduleBase == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to find module: {moduleName}");
                CloseHandle(hProcess);
                return false;
            }

            IntPtr address = IntPtr.Add(moduleBase, offset);

            byte[] buffer = new byte[size];
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            Array.Copy(textBytes, buffer, textBytes.Length);

            if (!VirtualProtectEx(hProcess, address, (uint)buffer.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
            {
                Console.WriteLine("Failed to change memory protection.");
                CloseHandle(hProcess);
                return false;
            }

            bool result = WriteProcessMemory(hProcess, address, buffer, (uint)buffer.Length, out _);

            VirtualProtectEx(hProcess, address, (uint)buffer.Length, oldProtect, out _);

            CloseHandle(hProcess);

            return result;
        }

        /// <summary>
        /// Získá adresu .dll knihovny načtený ve hře
        /// </summary>
        /// <param name="process"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static IntPtr GetModuleBaseAddress(Process process, string moduleName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return module.BaseAddress;
                }
            }
            throw new ArgumentException($"Module {moduleName} not found in process {process.ProcessName}.");
        }

        /// <summary>
        /// Přečte string z paměti na určité adrese.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="address"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string ReadFromMemoryStr(Process processHandle, IntPtr address, string module)
        {
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processHandle.Id);

            if (hProcess == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to open process for reading.");
            }

            try
            {
                IntPtr moduleBaseAddress = GetModuleBaseAddress(processHandle, module);
                IntPtr finalAddress = IntPtr.Add(moduleBaseAddress, address.ToInt32());

                const int stringLength = 256;
                byte[] buffer = new byte[stringLength];

                if (!ReadProcessMemory(hProcess, finalAddress, buffer, buffer.Length, out int bytesRead) || bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read memory.");
                }

                return Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Přečte číslo z paměti na učtité adrese (tato funkce je méně bezpečná než ReadFromMemoryStr() protože se doufá že se na adrese nachází int.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="address"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static int ReadFromMemoryInt(Process processHandle, IntPtr address, string module)
        {
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processHandle.Id);

            if (hProcess == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to open process for reading.");
            }

            try
            {
                IntPtr moduleBaseAddress = GetModuleBaseAddress(processHandle, module);
                IntPtr finalAddress = IntPtr.Add(moduleBaseAddress, address.ToInt32());

                byte[] buffer = new byte[sizeof(int)];

                if (!ReadProcessMemory(hProcess, finalAddress, buffer, buffer.Length, out int bytesRead) || bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read memory.");
                }

                return BitConverter.ToInt32(buffer, 0);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Přečte string z paměti na určité adrese.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string ReadFromMemoryStr(Process processHandle, IntPtr address)
        {
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processHandle.Id);

            if (hProcess == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to open process for reading.");
            }

            try
            {
                const int stringLength = 256;
                byte[] buffer = new byte[stringLength];

                if (!ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead) || bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read memory.");
                }

                return Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\0');
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Přečte číslo z paměti na učtité adrese (tato funkce je méně bezpečná než ReadFromMemoryStr() protože se doufá že se na adrese nachází int.
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static int ReadFromMemoryInt(Process processHandle, IntPtr address)
        {
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processHandle.Id);

            if (hProcess == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to open process for reading.");
            }

            try
            {
                byte[] buffer = new byte[sizeof(int)];

                if (!ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead) || bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read memory.");
                }

                return BitConverter.ToInt32(buffer, 0);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Zapíše int na adresu v paměti.
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="moduleName"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteIntToAddress(int processId, string moduleName, int offset, int value)
        {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                return false;
            }

            Process process = Process.GetProcessById(processId);
            IntPtr moduleBase = IntPtr.Zero;
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    moduleBase = module.BaseAddress;
                    break;
                }
            }

            if (moduleBase == IntPtr.Zero)
            {
                CloseHandle(hProcess);
                return false;
            }

            IntPtr address = IntPtr.Add(moduleBase, offset);

            byte[] buffer = BitConverter.GetBytes(value);

            if (!VirtualProtectEx(hProcess, address, (uint)buffer.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
            {
                CloseHandle(hProcess);
                return false;
            }

            bool result = WriteProcessMemory(hProcess, address, buffer, (uint)buffer.Length, out _);

            VirtualProtectEx(hProcess, address, (uint)buffer.Length, oldProtect, out _);

            CloseHandle(hProcess);

            return result;
        }
    }
}
