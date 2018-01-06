using System;
using System.IO;
using System.Reflection;
using System.Text;
using FastWin32.Memory;
using LzmaSharp;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 注入
    /// </summary>
    public static class Injector
    {
        private readonly static byte[] _invoker = LzmaCompressor.Decompress(Resources.DInvoker);

        /// <summary>
        /// Dll注入
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="dllPath">要注入的Dll的路径</param>
        /// <returns></returns>
        public static bool Inject(uint processId, string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath))
                throw new ArgumentException();
            if (!File.Exists(dllPath))
                throw new DllNotFoundException();

            IntPtr hProcess;
            bool is64;
            bool isAssembly;
            string clrVer;

            hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, processId);
            //打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            dllPath = Path.GetFullPath(dllPath);
            //获取绝对路径
            IsAssembly(dllPath, out isAssembly, out clrVer);
            //获取是否程序集
            if (isAssembly)
            {
                string tempDir;
                byte[] bytDInvoker;

                tempDir = Path.Combine(Path.GetTempPath(), "DInvoker", Guid.NewGuid().ToString());
                //获取临时文件夹
                Directory.CreateDirectory(tempDir);
                //创建临时文件夹
                File.Copy(dllPath, Path.Combine(tempDir, "netdll.dll"), true);
                //复制.Net Dll到rootDir并重命名
                if (!Process.Is64ProcessInternal(hProcess, out is64))
                    return false;
                if (is64)
                {
                    if (!Environment.Is64BitProcess)
                        throw new NotSupportedException("注入64位进程但当前进程为32位");
                    bytDInvoker = new byte[18944];
                    Buffer.BlockCopy(_invoker, 14336, bytDInvoker, 0, bytDInvoker.Length);
                    //读取64位DInvoker
                }
                else
                {
                    if (Environment.Is64BitProcess)
                        throw new NotSupportedException("注入32位进程但当前进程为64位");
                    bytDInvoker = new byte[14336];
                    Buffer.BlockCopy(_invoker, 0, bytDInvoker, 0, bytDInvoker.Length);
                    //读取32位DInvoker
                }
                if (clrVer == "v4.0.30319")
                    dllPath = Path.Combine(tempDir, "DInvoker4.dll");
                else if (clrVer == "v2.0.50727")
                    dllPath = Path.Combine(tempDir, "DInvoker2.dll");
                else
                    throw new NotSupportedException("不支持的CLR版本");
                //使用DInvoker启动CLR，并调用.Net Dll
                File.WriteAllBytes(dllPath, bytDInvoker);
                //写入文件
            }
            return InjectInternal(hProcess, dllPath);
        }

        /// <summary>
        /// 注入Dll
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="dllPath">要注入的Dll的路径，必须使用绝对路径！！！不支持相对路径！！！</param>
        /// <returns></returns>
        internal static unsafe bool InjectInternal(IntPtr hProcess, string dllPath)
        {
            IntPtr hModule;
            IntPtr pFunc;
            byte[] bytDllPath;
            IntPtr pDllPathRemote;
            IntPtr hThread;

            hModule = GetModuleHandle("kernel32");
            //获取模块句柄
            pFunc = GetProcAddress(hModule, "LoadLibraryW");
            //获取LoadLibrary的函数地址
            bytDllPath = Encoding.Unicode.GetBytes(dllPath);
            //以字节数组形式表示Dll的路径
            pDllPathRemote = MemoryManagement.AllocMemoryInternal(hProcess, (uint)bytDllPath.Length, PAGE_EXECUTE_READ);
            //在远程进程中，指向Dll路径的指针
            if (!MemoryIO.WriteBytesInternal(hProcess, pDllPathRemote, bytDllPath))
                //写入Dll路径失败
                return false;
            hThread = CreateRemoteThread(hProcess, null, 0, pFunc, pDllPathRemote, CREATE_RUNNING, null);
            //创建远程线程
            if (hThread == IntPtr.Zero)
                //创建远程线程失败
                return false;
            WaitForSingleObject(hThread, INFINITE);
            //等待
            CloseHandle(hThread);
            //关闭句柄
            return true;
        }

        /// <summary>
        /// 判断是否为程序集，如果是，输出CLR版本
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isAssembly">是否程序集</param>
        /// <param name="clrVer">CLR版本</param>
        private static void IsAssembly(string path, out bool isAssembly, out string clrVer)
        {
            try
            {
                clrVer = Assembly.LoadFile(path).ImageRuntimeVersion;
                isAssembly = true;
            }
            catch (BadImageFormatException)
            {
                clrVer = null;
                isAssembly = false;
            }
        }
    }
}
