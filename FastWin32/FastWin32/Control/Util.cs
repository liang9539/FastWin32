using System;
using FastWin32.Diagnostics;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Control
{
    /// <summary>
    /// 常用方法
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// 在远程进程中读取结构
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="structure">读取出的结构体</param>
        /// <param name="reader">读取器（将需要的数据从远程进程读取到指定远程内存中）</param>
        /// <returns></returns>
        public static unsafe bool ReadStructRemote<TStruct>(IntPtr hWnd, out TStruct structure, Func<IntPtr, bool> reader) where TStruct : IWin32ControlStruct
        {
            uint processId;
            IntPtr hProcess;
            IntPtr remoteAddr;

            structure = default(TStruct);
            processId = ProcessX.GetProcessIdByHWnd(hWnd);
            //获取控件所在进程ID
            hProcess = Memory.Util.OpenProcessRW(processId);
            //打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            try
            {
                remoteAddr = MemoryManagement.AllocMemoryInternal(hProcess, structure.Size);
                //在控件所在进程分配内存，用于储存structure
                try
                {
                    if (!reader(remoteAddr))
                        //在远程进程中获取失败
                        return false;
                    if (!ReadProcessMemory(hProcess, remoteAddr, structure.ToPointer(), structure.Size, null))
                        //从远程进程取回到当前进程失败
                        return false;
                    return true;
                }
                finally
                {
                    MemoryManagement.FreeMemoryInternal(hProcess, remoteAddr);
                    //释放之前分配的内存
                }
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 在远程进程中写入结构
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="structure">将写入的结构体</param>
        /// <param name="writer">写入器（将需要的数据从远程进程写入到指定远程内存中）</param>
        /// <returns></returns>
        public static unsafe bool WriteStructRemote<TStruct>(IntPtr hWnd, TStruct structure, Func<IntPtr, bool> writer) where TStruct : IWin32ControlStruct
        {
            uint processId;
            IntPtr hProcess;
            IntPtr remoteAddr;

            processId = ProcessX.GetProcessIdByHWnd(hWnd);
            //获取控件所在进程ID
            hProcess = Memory.Util.OpenProcessRW(processId);
            //打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            try
            {
                remoteAddr = MemoryManagement.AllocMemoryInternal(hProcess, structure.Size);
                //在控件所在进程分配内存，用于储存structure
                try
                {
                    if (!WriteProcessMemory(hProcess, remoteAddr, structure.ToPointer(), structure.Size, null))
                        return false;
                    if (!writer(remoteAddr))
                        return false;
                    return true;
                }
                finally
                {
                    MemoryManagement.FreeMemoryInternal(hProcess, remoteAddr);
                    //释放之前分配的内存
                }
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }
    }
}
