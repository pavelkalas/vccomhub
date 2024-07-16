using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

namespace VcComHub.Automation
{
    /// <summary>
    /// Třída sloužící pro manipulaci s GUI oknama.
    /// </summary>
    public class Window
    {
        [DllImport("user32.dll", SetLastError = true)] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool SetWindowText(IntPtr hWnd, string lpString);
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);

        public const int BM_CLICK = 0x00F5;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Nastaví vlastní nadpis okna.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="newTitle"></param>
        public static void ChangeWindowTitle(IntPtr windowHandle, string newTitle)
        {
            if (windowHandle != IntPtr.Zero)
            {
                SetWindowText(windowHandle, newTitle);
            }
        }

        /// <summary>
        /// Nastaví okno do popředí.
        /// </summary>
        /// <param name="processId"></param>
        public static void FocusWindow(int processId)
        {
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.Id == processId)
                {
                    SetForegroundWindow(proc.MainWindowHandle);
                    break;
                }
            }
        }

        /// <summary>
        /// Získá nadpis pomocí pID procesu.
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static string GetWindowTitle(int processId)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.Id == processId)
                {
                    return process.MainWindowTitle.Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Získá nadpis okna pomocí handle okna.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static string GetWindowTitle(IntPtr windowHandle)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.MainWindowHandle == windowHandle)
                {
                    return process.MainWindowTitle.Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Klikne na tlačítko na dialogovém okně podle jména.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool ClickButton(IntPtr windowHandle, string buttonName)
        {
            if (windowHandle == IntPtr.Zero)
            {
                Console.WriteLine("Window not found!");
                return false;
            }

            IntPtr hButton = FindWindowEx(windowHandle, IntPtr.Zero, null, buttonName);

            if (hButton == IntPtr.Zero)
            {
                Console.WriteLine("Button not found!");
                return false;
            }

            SendMessage(hButton, BM_CLICK, IntPtr.Zero, IntPtr.Zero);

            return true;
        }

        /// <summary>
        /// Přemístí okno doprostřed obrazovky.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static bool MoveWindowToCenter(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return false;
            }

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            if (!GetWindowRect(windowHandle, out RECT rect))
            {
                return false;
            }

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            int newX = (screenWidth - windowWidth) / 2;
            int newY = (screenHeight - windowHeight) / 2;

            return MoveWindow(windowHandle, newX, newY, windowWidth, windowHeight, true);
        }
    }
}
