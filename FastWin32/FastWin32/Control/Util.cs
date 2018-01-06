using System;
using FastWin32.Diagnostics;
using static FastWin32.NativeMethods;
using static FastWin32.Util;

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
        /// <param name="callbackBeforeRead">读取前回调方法</param>
        /// <param name="callbackAfterRead">读取后回调方法</param>
        /// <returns></returns>
        public static unsafe bool ReadStructRemote<TStruct>(IntPtr hWnd, out TStruct structure, Func<IntPtr, IntPtr, bool> callbackBeforeRead, Func<IntPtr, IntPtr, bool> callbackAfterRead) where TStruct : IWin32ControlStruct
        {
            uint processId;
            IntPtr hProcess;
            bool is64;
            IntPtr remoteAddr;

            structure = default(TStruct);
            processId = Process.GetProcessIdByHWnd(hWnd);
            //获取控件所在进程ID
            hProcess = OpenProcessRWQuery(processId);
            //打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                remoteAddr = VirtualAllocEx(hProcess, IntPtr.Zero, structure.Size, MEM_COMMIT, PAGE_READWRITE);
                //在控件所在进程分配内存，用于储存structure
                try
                {
                    if (callbackBeforeRead != null)
                        if (!callbackBeforeRead(hProcess, remoteAddr))
                            return false;
                    if (!ReadProcessMemory(hProcess, remoteAddr, structure.ToPointer(), structure.Size, null))
                        //从远程进程取回到当前进程失败
                        return false;
                    if (callbackAfterRead != null)
                        if (!callbackAfterRead(hProcess, remoteAddr))
                            return false;
                    return true;
                }
                finally
                {
                    VirtualFreeEx(hProcess, remoteAddr, 0, MEM_RELEASE);
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
        /// <param name="callbackBeforeWrite">写入前回调方法</param>
        /// <param name="callbackAfterWrite">写入后回调方法</param>
        /// <returns></returns>
        public static unsafe bool WriteStructRemote<TStruct>(IntPtr hWnd, ref TStruct structure, Func<IntPtr, IntPtr, bool> callbackBeforeWrite, Func<IntPtr, IntPtr, bool> callbackAfterWrite) where TStruct : IWin32ControlStruct
        {
            uint processId;
            IntPtr hProcess;
            bool is64;
            IntPtr remoteAddr;

            processId = Process.GetProcessIdByHWnd(hWnd);
            //获取控件所在进程ID
            hProcess = OpenProcessRWQuery(processId);
            //打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                remoteAddr = VirtualAllocEx(hProcess, IntPtr.Zero, structure.Size, MEM_COMMIT, PAGE_READWRITE);
                //在控件所在进程分配内存，用于储存structure
                try
                {
                    if (callbackBeforeWrite != null)
                        if (!callbackBeforeWrite(hProcess, remoteAddr))
                            return false;
                    if (!WriteProcessMemory(hProcess, remoteAddr, structure.ToPointer(), structure.Size, null))
                        return false;
                    if (callbackAfterWrite != null)
                        if (!callbackAfterWrite(hProcess, remoteAddr))
                            return false;
                    return true;
                }
                finally
                {
                    VirtualFreeEx(hProcess, remoteAddr, 0, MEM_RELEASE);
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
