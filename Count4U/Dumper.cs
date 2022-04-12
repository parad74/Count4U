﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Count4U
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_EXCEPTION_INFORMATION
    {
        public uint ThreadId;
        public IntPtr ExceptionPointers;
        public int ClientPointers;
    }

    public class Dumper
    {
        public const int MiniDumpNormal = 0x00000000;
        public const int MiniDumpWithDataSegs = 0x00000001;
        public const int MiniDumpWithFullMemory = 0x00000002;
        public const int MiniDumpWithHandleData = 0x00000004;
        public const int MiniDumpFilterMemory = 0x00000008;
        public const int MiniDumpScanMemory = 0x00000010;
        public const int MiniDumpWithUnloadedModules = 0x00000020;
        public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
        public const int MiniDumpFilterModulePaths = 0x00000080;
        public const int MiniDumpWithProcessThreadData = 0x00000100;
        public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
        public const int MiniDumpWithoutOptionalData = 0x00000400;
        public const int MiniDumpWithFullMemoryInfo = 0x00000800;
        public const int MiniDumpWithThreadInfo = 0x00001000;
        public const int MiniDumpWithCodeSegs = 0x00002000; 

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("Dbghelp.dll")]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint ProcessId, 
            IntPtr hFile,
            int DumpType,
            ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);

        public void CreateMiniDump(Version version)
        {
            using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
            {
                DateTime now = DateTime.Now;
                string date = String.Format("{0}_{1}-{2:D2}-{3:D2}_{4:D2}-{5:D2}-{6:D2}", version.Build, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                string fileName = string.Format(@"CRASH_DUMP_{0}.dmp", date);
                string filePath = Path.Combine(BootstrapLogger.GetLogDir(), fileName);

                MINIDUMP_EXCEPTION_INFORMATION Mdinfo = new MINIDUMP_EXCEPTION_INFORMATION();

                Mdinfo.ThreadId = GetCurrentThreadId();
                Mdinfo.ExceptionPointers = Marshal.GetExceptionPointers();
                Mdinfo.ClientPointers = 1;                

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    MiniDumpWriteDump(process.Handle,
                        (uint)process.Id,
                        fs.SafeFileHandle.DangerousGetHandle(),
                        MiniDumpWithDataSegs,
                        ref Mdinfo,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
        }
    }
}