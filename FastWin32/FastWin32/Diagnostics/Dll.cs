using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 提供Dll相关操作的静态类
    /// </summary>
    public static class Dll
    {
        /// <summary>
        /// Dll注入
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="dllPath">要注入的Dll的路径</param>
        /// <returns></returns>
        public static unsafe bool Inject(uint processId, string dllPath)
        {
            
            return InjectDInvoker(processId, dllPath);
        }

        /// <summary>
        /// 注入DInvoker，由DInvoker进行调用与启动CLR
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="dllPath">DInvoker的路径</param>
        /// <returns></returns>
        private static unsafe bool InjectDInvoker(uint processId, string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath))
                throw new ArgumentException();
            if (!File.Exists(dllPath))
                throw new DllNotFoundException();

            IntPtr hModule;
            IntPtr pFunc;
            IntPtr hProcess;
            byte[] bytDllPath;
            IntPtr pDllPathRemote;
            IntPtr hThread;

            hModule = GetModuleHandle("kernel32");
            //获取模块句柄
            pFunc = GetProcAddress(hModule, "LoadLibraryW");
            //获取LoadLibrary的函数地址
            hProcess = OpenProcess(ProcAccessFlags.PROCESS_CREATE_THREAD | ProcAccessFlags.PROCESS_VM_OPERATION | ProcAccessFlags.PROCESS_VM_READ | ProcAccessFlags.PROCESS_VM_WRITE, false, processId);
            if (hProcess == IntPtr.Zero)
                return false;
            bytDllPath = Encoding.Unicode.GetBytes(dllPath);
            //以字节数组形式表示Dll的路径
            pDllPathRemote = MemoryManagement.AllocMemoryInternal(hProcess, (uint)bytDllPath.Length, MemoryProtectionFlags.PAGE_EXECUTE_READ);
            //在远程进程中，指向Dll路径的指针
            if (!MemoryRW.WriteBytesInternal(hProcess, pDllPathRemote, bytDllPath))
                //写入Dll路径失败
                return false;
            hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, pFunc, pDllPathRemote, 0, null);
            //创建远程线程
            if (hThread == IntPtr.Zero)
                //创建远程线程失败
                return false;
            return true;
        }
    }
}
