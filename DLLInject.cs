using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Holly_X
{
    internal class DLLInject
    {
        // Token: 0x02000003 RID: 3

        // Token: 0x02000004 RID: 4
        public enum DllInjectionResult
        {
            // Token: 0x04000003 RID: 3
            DllNotFound,
            // Token: 0x04000004 RID: 4
            GameProcessNotFound,
            // Token: 0x04000005 RID: 5
            InjectionFailed,
            // Token: 0x04000006 RID: 6
            Success
        }

        // Token: 0x02000005 RID: 5
        public sealed class DllInjector
        {
            // Token: 0x17000001 RID: 1
            // (get) Token: 0x0600000B RID: 11 RVA: 0x0000242F File Offset: 0x0000062F
            public static DLLInject.DllInjector GetInstance
            {
                get
                {
                    if (DLLInject.DllInjector._instance == null)
                    {
                        DLLInject.DllInjector._instance = new DLLInject.DllInjector();
                    }
                    return DLLInject.DllInjector._instance;
                }
            }

            // Token: 0x0600000D RID: 13 RVA: 0x00002454 File Offset: 0x00000654
            private DllInjector()
            {
            }

            // Token: 0x0600000E RID: 14 RVA: 0x0000245C File Offset: 0x0000065C
            private bool bInject(uint pToBeInjected, string sDllPath)
            {
                IntPtr intPtr = DLLInject.DllInjector.OpenProcess(1082u, 1, pToBeInjected);
                if (intPtr == DLLInject.DllInjector.INTPTR_ZERO)
                {
                    return false;
                }
                IntPtr procAddress = DLLInject.DllInjector.GetProcAddress(DLLInject.DllInjector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (procAddress == DLLInject.DllInjector.INTPTR_ZERO)
                {
                    return false;
                }
                IntPtr intPtr2 = DLLInject.DllInjector.VirtualAllocEx(intPtr, (IntPtr)0, (IntPtr)sDllPath.Length, 12288u, 64u);
                if (intPtr2 == DLLInject.DllInjector.INTPTR_ZERO)
                {
                    return false;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);
                if (DLLInject.DllInjector.WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0)
                {
                    return false;
                }
                if (DLLInject.DllInjector.CreateRemoteThread(intPtr, (IntPtr)0, DLLInject.DllInjector.INTPTR_ZERO, procAddress, intPtr2, 0u, (IntPtr)0) == DLLInject.DllInjector.INTPTR_ZERO)
                {
                    return false;
                }
                DLLInject.DllInjector.CloseHandle(intPtr);
                return true;
            }

            // Token: 0x0600000F RID: 15
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int CloseHandle(IntPtr hObject);

            // Token: 0x06000010 RID: 16
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

            // Token: 0x06000011 RID: 17
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            // Token: 0x06000012 RID: 18
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            // Token: 0x06000013 RID: 19 RVA: 0x00002524 File Offset: 0x00000724
            public DLLInject.DllInjectionResult Inject(string sProcName, string sDllPath)
            {
                if (!File.Exists(sDllPath))
                {
                    return DLLInject.DllInjectionResult.DllNotFound;
                }
                uint num = 0u;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    if (!(processes[i].ProcessName != sProcName))
                    {
                        num = (uint)processes[i].Id;
                        break;
                    }
                }
                if (num == 0u)
                {
                    return DLLInject.DllInjectionResult.GameProcessNotFound;
                }
                if (!this.bInject(num, sDllPath))
                {
                    return DLLInject.DllInjectionResult.InjectionFailed;
                }
                return DLLInject.DllInjectionResult.Success;
            }

            // Token: 0x06000014 RID: 20
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

            // Token: 0x06000015 RID: 21
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

            // Token: 0x06000016 RID: 22
            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

            // Token: 0x04000007 RID: 7
            private static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

            // Token: 0x04000008 RID: 8
            private static DLLInject.DllInjector _instance;

        }
    }
}
