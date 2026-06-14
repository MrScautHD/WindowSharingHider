﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace WindowSharingHider
{
    public static class WindowHandler
    {
        [DllImport("user32")] static extern Boolean EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32")] static extern Boolean IsWindowVisible(IntPtr hWnd);
        [DllImport("user32", CharSet = CharSet.Auto)] static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
        [DllImport("user32", CharSet = CharSet.Auto)] static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, String className, String windowTitle);
        [DllImport("user32", CharSet = CharSet.Auto)] static extern Int32 GetClassName(IntPtr hWnd, StringBuilder lpClassName, Int32 nMaxCount);
        [DllImport("dwmapi.dll")] static extern Int32 DwmGetWindowAttribute(IntPtr hwnd, Int32 dwAttribute, out Int32 pvAttribute, Int32 cbAttribute);

        [DllImport("user32")] static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);
        [DllImport("user32")] static extern Int32 GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32")] static extern Boolean GetWindowDisplayAffinity(IntPtr hWnd, out Int32 dwAffinity);
        [DllImport("user32")] static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 processId);
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("kernel32")] static extern IntPtr OpenProcess(Int32 dwDesiredAccess, Boolean bInheritHandle, Int32 dwProcessId);
        [DllImport("kernel32")] static extern IntPtr GetModuleHandle(String lpModuleName);
        [DllImport("kernel32")] static extern IntPtr GetProcAddress(IntPtr hModule, String procName);
        [DllImport("kernel32")] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")] static extern Boolean WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, Byte[] lpBuffer, Int32 nSize, out Int32 lpNumberOfBytesWritten);
        [DllImport("kernel32")] static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, UInt32 dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, UInt32 dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32")] static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32")] static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32")] static extern Boolean VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, Int32 dwFreeType);
        [DllImport("kernel32")] static extern Boolean IsWow64Process(IntPtr processHandle, out Boolean wow64Process);
        [DllImport("psapi")] static extern bool EnumProcessModulesEx(IntPtr hProcess, [In][Out] IntPtr[] lphModule, UInt32 cb, out UInt32 lpcbNeeded, UInt32 dwFilterFlag);
        [DllImport("psapi")] static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, UInt32 nSize);
        public static Dictionary<IntPtr, String> GetVisibleWindows()
        {
            var windows = new Dictionary<IntPtr, String>();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (!IsWindowVisible(hWnd)) return true;
                DwmGetWindowAttribute(hWnd, 14, out Int32 pvAttribute, 4);
                if (pvAttribute > 0) return true;
                if (IsTaskbarWindow(hWnd)) return true;
                var length = GetWindowTextLength(hWnd);
                if (length == 0) return true;
                var windowTextBuilder = new StringBuilder(length + 1);
                GetWindowText(hWnd, windowTextBuilder, windowTextBuilder.Capacity);
                var windowTitle = windowTextBuilder.ToString();
                windows[hWnd] = windowTitle;
                return true;
            }, IntPtr.Zero);
            foreach (var desktopWindow in GetDesktopIconsWindows()) windows[desktopWindow] = "Desktop and Icons";
            return windows;
        }

        public static HashSet<IntPtr> GetDesktopIconsWindows()
        {
            var windows = new HashSet<IntPtr>();
            var progman = FindWindow("Progman", "Program Manager");
            if (progman != IntPtr.Zero) windows.Add(progman);

            var workerW = IntPtr.Zero;
            while (true)
            {
                workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
                if (workerW == IntPtr.Zero) break;

                var shellView = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView != IntPtr.Zero) windows.Add(workerW);
            }
            if (windows.Count == 0 && progman != IntPtr.Zero) windows.Add(progman);
            return windows;
        }

        public static HashSet<IntPtr> GetProcessTopLevelWindows(IntPtr sourceWindowHandle)
        {
            var windows = new HashSet<IntPtr>();
            GetWindowThreadProcessId(sourceWindowHandle, out Int32 processId);
            if (processId <= 0)
            {
                windows.Add(sourceWindowHandle);
                return windows;
            }

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                GetWindowThreadProcessId(hWnd, out Int32 candidateProcessId);
                if (candidateProcessId != processId) return true;
                if (!IsWindowVisible(hWnd)) return true;
                DwmGetWindowAttribute(hWnd, 14, out Int32 pvAttribute, 4);
                if (pvAttribute > 0) return true;
                if (IsTaskbarWindow(hWnd)) return true;
                windows.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            if (windows.Count == 0) windows.Add(sourceWindowHandle);
            return windows;
        }

        public static String GetWindowProcessName(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out Int32 processId);
            if (processId <= 0) return String.Empty;
            try
            {
                using (var process = Process.GetProcessById(processId)) return process.ProcessName;
            }
            catch
            {
                return String.Empty;
            }
        }

        public static HashSet<IntPtr> GetTaskbarWindows()
        {
            var windows = new HashSet<IntPtr>();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (IsTaskbarWindow(hWnd)) windows.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return windows;
        }
        public static Int32 GetWindowDisplayAffinity(IntPtr hWnd)
        {
            GetWindowDisplayAffinity(hWnd, out Int32 dwAffinity);
            return dwAffinity;
        }
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowdisplayaffinity current process has to own hWnd.. but does it really matter??
        public static void SetWindowDisplayAffinity(IntPtr hWnd, Int32 dwAffinity)
        {
            GetWindowThreadProcessId(hWnd, out Int32 procId);
            if (procId <= 0) return;
            const Int32 PROCESS_CREATE_THREAD = 0x0002;
            const Int32 PROCESS_VM_OPERATION = 0x0008;
            const Int32 PROCESS_VM_READ = 0x0010;
            const Int32 PROCESS_VM_WRITE = 0x0020;
            const Int32 PROCESS_QUERY_INFORMATION = 0x0400;
            const Int32 PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
            var desiredAccess = PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION | PROCESS_QUERY_LIMITED_INFORMATION;
            var procHandle = OpenProcess(desiredAccess, false, procId);
            if (procHandle == IntPtr.Zero) return;
            try
            {
                if (!IsWow64Process(procHandle, out bool targetIsWow64)) return;
                var targetIs32Bit = targetIsWow64;
                var currentIs32Bit = IntPtr.Size == 4;
                if (currentIs32Bit != targetIs32Bit) return;

                if (!TryGetRemoteModuleBase(procHandle, "user32.dll", out IntPtr remoteUser32)) return;
                var localUser32 = GetModuleHandle("user32.dll");
                if (localUser32 == IntPtr.Zero) return;
                var localSetAffinity = GetProcAddress(localUser32, "SetWindowDisplayAffinity");
                if (localSetAffinity == IntPtr.Zero) return;

                var offset = localSetAffinity.ToInt64() - localUser32.ToInt64();
                var remoteSetAffinity = new IntPtr(remoteUser32.ToInt64() + offset);

                var asm = BuildSetAffinityShellcode(targetIs32Bit, hWnd, dwAffinity, remoteSetAffinity);
                var codePtr = VirtualAllocEx(procHandle, IntPtr.Zero, asm.Length, 0x1000, 0x40);
                if (codePtr == IntPtr.Zero) return;
                if (!WriteProcessMemory(procHandle, codePtr, asm, asm.Length, out Int32 bytesWritten) || bytesWritten != asm.Length)
                {
                    VirtualFreeEx(procHandle, codePtr, 0, 0x8000);
                    return;
                }

                var thread = CreateRemoteThread(procHandle, IntPtr.Zero, 0, codePtr, IntPtr.Zero, 0, IntPtr.Zero);
                if (thread != IntPtr.Zero)
                {
                    WaitForSingleObject(thread, 10000);
                    CloseHandle(thread);
                }
                VirtualFreeEx(procHandle, codePtr, 0, 0x8000);
            }
            finally
            {
                CloseHandle(procHandle);
            }
        }

        private static Boolean TryGetRemoteModuleBase(IntPtr processHandle, String moduleName, out IntPtr remoteBase)
        {
            remoteBase = IntPtr.Zero;
            EnumProcessModulesEx(processHandle, new IntPtr[0], 0, out UInt32 bytesNeeded, 3);
            if (bytesNeeded == 0) return false;

            var ptrSize = Marshal.SizeOf(typeof(IntPtr));
            var moduleCount = bytesNeeded / ptrSize;
            if (moduleCount == 0) return false;

            var modules = new IntPtr[moduleCount];
            if (!EnumProcessModulesEx(processHandle, modules, bytesNeeded, out _, 3)) return false;

            var match = moduleName.ToLowerInvariant();
            foreach (var module in modules)
            {
                var path = new StringBuilder(260);
                if (GetModuleFileNameEx(processHandle, module, path, 260) == 0) continue;
                var fileName = path.ToString().Split('\\').LastOrDefault() ?? String.Empty;
                if (!String.Equals(fileName, match, StringComparison.OrdinalIgnoreCase)) continue;
                remoteBase = module;
                return true;
            }
            return false;
        }

        private static Byte[] BuildSetAffinityShellcode(Boolean targetIs32Bit, IntPtr windowHandle, Int32 affinity, IntPtr setAffinityAddress)
        {
            var asm = new List<Byte>();
            if (targetIs32Bit)
            {
                asm.Add(0x68); // push
                asm.AddRange(BitConverter.GetBytes((UInt32)affinity));
                asm.Add(0x68); // push
                asm.AddRange(BitConverter.GetBytes((UInt32)windowHandle.ToInt32()));
                asm.Add(0xB8); // mov eax
                asm.AddRange(BitConverter.GetBytes((UInt32)setAffinityAddress.ToInt32()));
                asm.AddRange(new Byte[] { 0xFF, 0xD0 }); // call eax
            }
            else
            {
                asm.AddRange(new Byte[] { 0x48, 0x83, 0xEC, 0x28 }); // sub rsp, 0x28
                asm.AddRange(new Byte[] { 0x48, 0xB9 }); // mov rcx, <hWnd>
                asm.AddRange(BitConverter.GetBytes((UInt64)windowHandle.ToInt64()));
                asm.AddRange(new Byte[] { 0x48, 0xBA }); // mov rdx, <affinity>
                asm.AddRange(BitConverter.GetBytes((UInt64)affinity));
                asm.AddRange(new Byte[] { 0x48, 0xB8 }); // mov rax, <SetWindowDisplayAffinity>
                asm.AddRange(BitConverter.GetBytes((UInt64)setAffinityAddress.ToInt64()));
                asm.AddRange(new Byte[] { 0xFF, 0xD0 }); // call rax
                asm.AddRange(new Byte[] { 0x48, 0x83, 0xC4, 0x28 }); // add rsp, 0x28
            }
            asm.Add(0xC3); // ret
            return asm.ToArray();
        }

        private static Boolean IsTaskbarWindow(IntPtr hWnd)
        {
            var className = new StringBuilder(256);
            if (GetClassName(hWnd, className, className.Capacity) <= 0) return false;
            var cls = className.ToString();
            return String.Equals(cls, "Shell_TrayWnd", StringComparison.OrdinalIgnoreCase)
                   || String.Equals(cls, "Shell_SecondaryTrayWnd", StringComparison.OrdinalIgnoreCase);
        }
    }
}
