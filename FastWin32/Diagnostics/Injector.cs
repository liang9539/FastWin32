using System;
using System.IO;
using System.Text;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 注入
    /// </summary>
    public static class Injector
    {
        #region Constant
        private const int AsmSize = 0x2000;

        private const int MachineCodeSize = 0x200;

        private const int AssemblyPathOffset = 0x200;

        private const int TypeNameOffset = 0x800;

        private const int MethodNameOffset = 0x980;

        private const int ArgumentOffset = 0xA00;

        private const int ReturnValueOffset = 0x1200;

        private const int CLRVersionOffset = 0x1210;

        private const int CLSID_CLRMetaHostOffset = 0x1260;

        private const int IID_ICLRMetaHostOffset = 0x1270;

        private const int IID_ICLRRuntimeInfoOffset = 0x1280;

        private const int CLSID_CLRRuntimeHostOffset = 0x1290;

        private const int IID_ICLRRuntimeHostOffset = 0x12A0;

        private const int AvailableOffset = 0x12B0;

        private readonly static byte[] CLSID_CLRMetaHost = new Guid(0x9280188D, 0x0E8E, 0x4867, 0xB3, 0x0C, 0x7F, 0xA8, 0x38, 0x84, 0xE8, 0xDE).ToByteArray();

        private readonly static byte[] IID_ICLRMetaHost = new Guid(0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16).ToByteArray();

        private readonly static byte[] IID_ICLRRuntimeInfo = new Guid(0xBD39D1D2, 0xBA2F, 0x486A, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91).ToByteArray();

        private readonly static byte[] CLSID_CLRRuntimeHost = new Guid(0x90F1A06E, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02).ToByteArray();

        private readonly static byte[] IID_ICLRRuntimeHost = new Guid(0x90F1A06C, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02).ToByteArray();
        #endregion

        private struct Section
        {
            public uint VirtualSize;

            public uint VirtualAddress;

            public uint SizeOfRawData;

            public uint PointerToRawData;

            public Section(uint virtualSize, uint virtualAddress, uint sizeOfRawData, uint pointerToRawData)
            {
                VirtualSize = virtualSize;
                VirtualAddress = virtualAddress;
                SizeOfRawData = sizeOfRawData;
                PointerToRawData = pointerToRawData;
            }
        }

        /// <summary>
        /// 打开进程（注入使用）
        /// </summary>
        /// <param name="processId">进程句柄</param>
        /// <returns></returns>
        private static SafeNativeHandle OpenProcessInjecting(uint processId)
        {
            return SafeOpenProcess(FastWin32Settings.SeDebugPrivilege ? PROCESS_ALL_ACCESS : PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, processId);
        }

        /// <summary>
        /// 注入非托管DLL
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="assemblyPath">要注入程序集的路径</param>
        /// <param name="typeName">类型名（命名空间+类型名，比如NamespaceA.ClassB）</param>
        /// <param name="methodName">方法名（比如MethodC），该方法必须具有此类签名static int MethodName(string)，比如private static int InjectingMain(string argument)</param>
        /// <param name="argument">参数，长度必须小于1024个字符，可传入null。</param>
        /// <returns></returns>
        public static bool InjectManaged(uint processId, string assemblyPath, string typeName, string methodName, string argument)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentNullException();
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException();
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException();
            if (argument != null && argument.Length >= 1024)
                throw new ArgumentOutOfRangeException(nameof(argument) + "长度必须小于1024个字符");

            SafeNativeHandle processHandle;
            int returnValue;

            using (processHandle = OpenProcessInjecting(processId))
                if (processHandle.IsValid)
                    return InjectManagedInternal(processHandle, assemblyPath, typeName, methodName, argument, out returnValue, false);
                else
                    return false;
        }

        /// <summary>
        /// 注入非托管DLL，并获取被调用方法的返回值（警告：被调用方法返回后才能获取到返回值，<see cref="InjectManaged(uint, string, string, string, string, out int)"/>方法将一直等待到被调用方法返回。如果仅注入程序集而不需要获取返回值，请使用重载版本<see cref="InjectManaged(uint, string, string, string, string)"/>）
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="assemblyPath">要注入程序集的路径</param>
        /// <param name="typeName">类型名（命名空间+类型名，比如NamespaceA.ClassB）</param>
        /// <param name="methodName">方法名（比如MethodC），该方法必须具有此类签名static int MethodName(string)，比如private static int InjectingMain(string argument)</param>
        /// <param name="argument">参数，长度必须小于1024个字符，可传入null。</param>
        /// <param name="returnValue">被调用方法返回的整数值</param>
        /// <returns></returns>
        public static bool InjectManaged(uint processId, string assemblyPath, string typeName, string methodName, string argument, out int returnValue)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentNullException();
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException();
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException();
            if (argument != null && argument.Length >= 1024)
                throw new ArgumentOutOfRangeException(nameof(argument) + "长度必须小于1024个字符");

            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessInjecting(processId))
                if (processHandle.IsValid)
                    return InjectManagedInternal(processHandle, assemblyPath, typeName, methodName, argument, out returnValue, true);
                else
                {
                    returnValue = 0;
                    return false;
                }
        }

        /// <summary>
        /// 注入非托管DLL
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="assemblyPath">要注入程序集的路径</param>
        /// <param name="typeName">类型名（命名空间+类型名，比如NamespaceA.ClassB）</param>
        /// <param name="methodName">方法名（比如MethodC），该方法必须具有此类签名static int MethodName(string)，比如private static int InjectingMain(string argument)</param>
        /// <param name="argument">参数，长度必须小于1024个字符，可传入null。</param>
        /// <param name="returnValue">被调用方法返回的整数值</param>
        /// <param name="wait">是否等待返回值</param>
        /// <returns></returns>
        internal static unsafe bool InjectManagedInternal(IntPtr processHandle, string assemblyPath, string typeName, string methodName, string argument, out int returnValue, bool wait)
        {
            bool isAssembly;
            bool is64;
            string clrVersion;
            IntPtr pFunction;
            IntPtr threadHandle;
            uint exitCode;

            returnValue = 0;
            assemblyPath = Path.GetFullPath(assemblyPath);
            //获取绝对路径
            IsAssembly(assemblyPath, out isAssembly, out clrVersion);
            if (!isAssembly)
                throw new NotSupportedException("将注入的DLL为不是程序集，应该调用InjectUnmanaged方法而非调用InjectManaged方法");
            if (!Process32.Is64BitProcessInternal(processHandle, out is64))
                return false;
            if (!InjectUnmanagedInternal(processHandle, Path.Combine(GetSystemPath(is64), "mscoree.dll")))
                return false;
            //加载对应进程位数的mscoree.dll
            pFunction = WriteAsm(processHandle, clrVersion, assemblyPath, typeName, methodName, argument);
            //获取远程进程中启动CLR的函数指针
            if (pFunction == IntPtr.Zero)
                return false;
            threadHandle = CreateRemoteThread(processHandle, null, 0, pFunction, pFunction + ReturnValueOffset, 0, null);
            if (threadHandle == IntPtr.Zero)
                return false;
            if (wait)
            {
                WaitForSingleObject(threadHandle, INFINITE);
                //等待线程结束
                if (!GetExitCodeThread(threadHandle, out exitCode))
                    return false;
                if (!MemoryIO.ReadInt32Internal(processHandle, pFunction + ReturnValueOffset, out returnValue))
                    return false;
                //获取程序集中被调用方法的返回值
                if (!MemoryManagement.FreeMemoryInternal(processHandle, pFunction))
                    return false;
                return (int)exitCode >= 0;
                //ICLRRuntimeHost::ExecuteInDefaultAppDomain返回S_OK（0）表示成功。HRESULT不能直接比较，大于等于0就是成功
            }
            return true;
        }

        /// <summary>
        /// 注入非托管DLL
        /// </summary>
        /// <param name="processId">要注入的进程ID</param>
        /// <param name="dllPath">要注入DLL的路径</param>
        /// <returns></returns>
        public static bool InjectUnmanaged(uint processId, string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath))
                throw new ArgumentNullException();
            if (!File.Exists(dllPath))
                throw new FileNotFoundException();

            SafeNativeHandle processHandle;
            bool isAssembly;
            string clrVersion;

            using (processHandle = OpenProcessInjecting(processId))
                if (processHandle.IsValid)
                {
                    dllPath = Path.GetFullPath(dllPath);
                    //获取绝对路径
                    IsAssembly(dllPath, out isAssembly, out clrVersion);
                    if (isAssembly)
                        throw new NotSupportedException("将注入的DLL为程序集，应该调用InjectManaged方法而非调用InjectUnmanaged方法");
                    return InjectUnmanagedInternal(processHandle, dllPath);
                    //注入非托管DLL
                }
                else
                    return false;
        }

        /// <summary>
        /// 注入非托管Dll
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="dllPath">要注入的Dll的路径</param>
        /// <returns></returns>
        internal static unsafe bool InjectUnmanagedInternal(IntPtr processHandle, string dllPath)
        {
            bool is64;
            IntPtr pLoadLibrary;
            IntPtr pDllPath;
            IntPtr threadHandle;
            uint exitCode;

            if (!Process32.Is64BitProcessInternal(processHandle, out is64))
                return false;
            pLoadLibrary = Module32.GetProcAddressInternal(processHandle, "kernel32.dll", "LoadLibraryW");
            //获取LoadLibrary的函数地址
            pDllPath = MemoryManagement.AllocMemoryInternal(processHandle, (uint)dllPath.Length * 2 + 2, PAGE_EXECUTE_READ);
            try
            {
                if (pDllPath == IntPtr.Zero)
                    return false;
                if (!MemoryIO.WriteStringInternal(processHandle, pDllPath, dllPath))
                    return false;
                threadHandle = CreateRemoteThread(processHandle, null, 0, pLoadLibrary, pDllPath, 0, null);
                if (threadHandle == IntPtr.Zero)
                    return false;
                WaitForSingleObject(threadHandle, INFINITE);
                //等待线程结束
                GetExitCodeThread(threadHandle, out exitCode);
                return exitCode != 0;
                //LoadLibrary返回值不为0则调用成功，否则失败
            }
            finally
            {
                MemoryManagement.FreeMemoryInternal(processHandle, pDllPath);
            }
        }

        /// <summary>
        /// 获取系统文件夹路径
        /// </summary>
        /// <param name="is64">是否64位</param>
        /// <returns></returns>
        private static string GetSystemPath(bool is64)
        {
            return Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), (!FastWin32Settings.Is64BitOperatingSystem || is64) ? "System32" : "SysWOW64");
        }

        /// <summary>
        /// 写入启动CLR的机器码，返回函数指针
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="clrVersion">CLR版本</param>
        /// <param name="assemblyPath">程序集路径（绝对路径）</param>
        /// <param name="typeName">类型名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="argument">参数（可空，如果非空，长度必须小于2000）</param>
        /// <returns></returns>
        private static unsafe IntPtr WriteAsm(IntPtr processHandle, string clrVersion, string assemblyPath, string typeName, string methodName, string argument)
        {
            bool is64;
            byte[] asm;
            IntPtr pFunction;
            IntPtr pCorBindToRuntimeEx;
            IntPtr pCLRCreateInstance;

            if (!Process32.Is64BitProcessInternal(processHandle, out is64))
                return IntPtr.Zero;
            asm = GetAsmCommon(clrVersion, assemblyPath, typeName, methodName, argument);
            pFunction = MemoryManagement.AllocMemoryInternal(processHandle, AsmSize, PAGE_EXECUTE_READWRITE);
            if (pFunction == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                fixed (byte* p = &asm[0])
                {
                    switch (clrVersion)
                    {
                        case "v2.0.50727":
                            pCorBindToRuntimeEx = Module32.GetProcAddressInternal(processHandle, "mscoree.dll", "CorBindToRuntimeEx");
                            if (pCorBindToRuntimeEx == IntPtr.Zero)
                                return IntPtr.Zero;
                            if (is64)
                                SetAsm64V2(p, (long)pFunction, (long)pCorBindToRuntimeEx);
                            else
                                SetAsm32V2(p, (int)pFunction, (int)pCorBindToRuntimeEx);
                            break;
                        case "v4.0.30319":
                            pCLRCreateInstance = Module32.GetProcAddressInternal(processHandle, "mscoree.dll", "CLRCreateInstance");
                            if (pCLRCreateInstance == IntPtr.Zero)
                                return IntPtr.Zero;
                            if (is64)
                                SetAsm64V4(p, (long)pFunction, (long)pCLRCreateInstance);
                            else
                                SetAsm32V4(p, (int)pFunction, (int)pCLRCreateInstance);
                            break;
                        default:
                            return IntPtr.Zero;
                    }
                }
                if (!MemoryIO.WriteBytesInternal(processHandle, pFunction, asm))
                    return IntPtr.Zero;
            }
            catch
            {
                MemoryManagement.FreeMemoryInternal(processHandle, pFunction);
                return IntPtr.Zero;
            }
            return pFunction;
        }

        /// <summary>
        /// 获取设置好参数的机器码
        /// </summary>
        /// <param name="clrVersion">CLR版本</param>
        /// <param name="assemblyPath">程序集路径（绝对路径）</param>
        /// <param name="typeName">类型名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="argument">参数（可空，如果非空，长度必须小于2000）</param>
        /// <returns></returns>
        private static byte[] GetAsmCommon(string clrVersion, string assemblyPath, string typeName, string methodName, string argument)
        {
            MemoryStream memoryStream;
            byte[] bytes;

            using (memoryStream = new MemoryStream(AsmSize))
            {
                bytes = Encoding.Unicode.GetBytes(assemblyPath);
                memoryStream.Position = AssemblyPathOffset;
                memoryStream.Write(bytes, 0, bytes.Length);
                //assemblyPath
                bytes = Encoding.Unicode.GetBytes(typeName);
                memoryStream.Position = TypeNameOffset;
                memoryStream.Write(bytes, 0, bytes.Length);
                //typeName
                bytes = Encoding.Unicode.GetBytes(methodName);
                memoryStream.Position = MethodNameOffset;
                memoryStream.Write(bytes, 0, bytes.Length);
                //methodName
                bytes = argument == null ? new byte[0] : Encoding.Unicode.GetBytes(argument);
                memoryStream.Position = ArgumentOffset;
                memoryStream.Write(bytes, 0, bytes.Length);
                //argument
                bytes = Encoding.Unicode.GetBytes(clrVersion);
                memoryStream.Position = CLRVersionOffset;
                memoryStream.Write(bytes, 0, bytes.Length);
                //clrVersion
                memoryStream.Position = CLSID_CLRMetaHostOffset;
                memoryStream.Write(CLSID_CLRMetaHost, 0, CLSID_CLRMetaHost.Length);
                memoryStream.Position = IID_ICLRMetaHostOffset;
                memoryStream.Write(IID_ICLRMetaHost, 0, IID_ICLRMetaHost.Length);
                memoryStream.Position = IID_ICLRRuntimeInfoOffset;
                memoryStream.Write(IID_ICLRRuntimeInfo, 0, IID_ICLRRuntimeInfo.Length);
                memoryStream.Position = CLSID_CLRRuntimeHostOffset;
                memoryStream.Write(CLSID_CLRRuntimeHost, 0, CLSID_CLRRuntimeHost.Length);
                memoryStream.Position = IID_ICLRRuntimeHostOffset;
                memoryStream.Write(IID_ICLRRuntimeHost, 0, IID_ICLRRuntimeHost.Length);
                memoryStream.SetLength(AsmSize);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 设置启动32位CLR V2的机器码
        /// </summary>
        /// <param name="p">机器码指针</param>
        /// <param name="pFunction">函数指针</param>
        /// <param name="pCorBindToRuntimeEx">CorBindToRuntimeEx的函数指针</param>
        private static unsafe void SetAsm32V2(byte* p, int pFunction, int pCorBindToRuntimeEx)
        {
            //HRESULT WINAPI LoadCLR2(DWORD *pReturnValue)
            #region {
            p[0] = 0x55;
            p += 1;
            //push ebp
            p[0] = 0x89;
            p[1] = 0xE5;
            p += 2;
            //mov ebp,esp
            p[0] = 0x83;
            p[1] = 0xEC;
            p[2] = 0x44;
            p += 3;
            //sub esp,byte +0x44
            p[0] = 0x53;
            p += 1;
            //push ebx
            p[0] = 0x56;
            p += 1;
            //push esi
            p[0] = 0x57;
            p += 1;
            //push edi
            p[0] = 0xC7;
            p[1] = 0x45;
            p[2] = 0xFC;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            #endregion
            #region ICLRRuntimeHost *pRuntimeHost = nullptr;
            //mov dword [ebp-0x4],0x0
            p[0] = 0x8D;
            p[1] = 0x45;
            p[2] = 0xFC;
            p += 3;
            #endregion
            #region CorBindToRuntimeEx(L"v2.0.50727", nullptr, 0, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntimeHost);
            //lea eax,[ebp-0x4]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + IID_ICLRRuntimeHostOffset;
            p += 5;
            //push dword PIID_ICLRRuntimeHost
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + CLSID_CLRRuntimeHostOffset;
            p += 5;
            //push dword pCLSID_CLRRuntimeHost
            p[0] = 0x6A;
            p[1] = 0x00;
            p += 2;
            //push byte +0x0
            p[0] = 0x6A;
            p[1] = 0x00;
            p += 2;
            //push byte +0x0
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + CLRVersionOffset;
            p += 5;
            //push dword pCLRVersion
            p[0] = 0xB9;
            *(int*)(p + 1) = pCorBindToRuntimeEx;
            p += 5;
            //mov ecx,pCorBindToRuntimeEx
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region pRuntimeHost->Start();
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xFC;
            p += 3;
            //mov eax,[ebp-0x4]
            p[0] = 0x8B;
            p[1] = 0x08;
            p += 2;
            //mov ecx,[eax]
            p[0] = 0x8B;
            p[1] = 0x55;
            p[2] = 0xFC;
            p += 3;
            //mov edx,[ebp-0x4]
            p[0] = 0x52;
            p += 1;
            //push edx
            p[0] = 0x8B;
            p[1] = 0x41;
            p[2] = 0x0C;
            p += 3;
            //mov eax,[ecx+0xc]
            p[0] = 0xFF;
            p[1] = 0xD0;
            p += 2;
            //call eax
            #endregion
            #region return pRuntimeHost->ExecuteInDefaultAppDomain(L"assemblyPath", L"typeName", L"methodName", L"argument", pReturnValue);
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0x08;
            p += 3;
            //mov eax,[ebp+0x8]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + ArgumentOffset;
            p += 5;
            //push dword pArgument
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + MethodNameOffset;
            p += 5;
            //push dword pMethodName
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + TypeNameOffset;
            p += 5;
            //push dword pTypeName
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + AssemblyPathOffset;
            p += 5;
            //push dword pAssemblyPath
            p[0] = 0x8B;
            p[1] = 0x4D;
            p[2] = 0xFC;
            p += 3;
            //mov ecx,[ebp-0x4]
            p[0] = 0x8B;
            p[1] = 0x11;
            p += 2;
            //mov edx,[ecx]
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xFC;
            p += 3;
            //mov eax,[ebp-0x4]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x8B;
            p[1] = 0x4A;
            p[2] = 0x2C;
            p += 3;
            //mov ecx,[edx+0x2c]
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region }
            p[0] = 0x5F;
            p += 1;
            //pop edi
            p[0] = 0x5E;
            p += 1;
            //pop esi
            p[0] = 0x5B;
            p += 1;
            //pop ebx
            p[0] = 0x89;
            p[1] = 0xEC;
            p += 2;
            //mov esp,ebp
            p[0] = 0x5D;
            p += 1;
            //pop ebp
            p[0] = 0xC2;
            p[1] = 0x04;
            p[2] = 0x00;
            p += 3;
            //ret 0x4
            #endregion
        }

        /// <summary>
        /// 设置启动32位CLR V4的机器码
        /// </summary>
        /// <param name="p">机器码指针</param>
        /// <param name="pFunction">函数指针</param>
        /// <param name="pCLRCreateInstance">CLRCreateInstance的函数指针</param>
        private static unsafe void SetAsm32V4(byte* p, int pFunction, int pCLRCreateInstance)
        {
            //HRESULT WINAPI LoadCLR4(DWORD *pReturnValue)
            #region {
            p[0] = 0x55;
            p += 1;
            //push ebp
            p[0] = 0x89;
            p[1] = 0xE5;
            p += 2;
            //mov ebp,esp
            p[0] = 0x83;
            p[1] = 0xEC;
            p[2] = 0x4C;
            p += 3;
            //sub esp,byte +0x4c
            p[0] = 0x53;
            p += 1;
            //push ebx
            p[0] = 0x56;
            p += 1;
            //push esi
            p[0] = 0x57;
            p += 1;
            //push edi
            #endregion
            #region ICLRMetaHost *pMetaHost = nullptr;
            p[0] = 0xC7;
            p[1] = 0x45;
            p[2] = 0xFC;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            //mov dword [ebp-0x4],0x0
            #endregion
            #region ICLRRuntimeInfo *pRuntimeInfo = nullptr;
            p[0] = 0xC7;
            p[1] = 0x45;
            p[2] = 0xF8;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            //mov dword [ebp-0x8],0x0
            #endregion
            #region ICLRRuntimeHost *pRuntimeHost = nullptr;
            p[0] = 0xC7;
            p[1] = 0x45;
            p[2] = 0xF4;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            //mov dword [ebp-0xc],0x0
            #endregion
            #region CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&pMetaHost);
            p[0] = 0x8D;
            p[1] = 0x45;
            p[2] = 0xFC;
            p += 3;
            //lea eax,[ebp-0x4]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + IID_ICLRMetaHostOffset;
            p += 5;
            //push dword pIID_ICLRMetaHost
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + CLSID_CLRMetaHostOffset;
            p += 5;
            //push dword pCLSID_CLRMetaHost
            p[0] = 0xB9;
            *(int*)(p + 1) = pCLRCreateInstance;
            p += 5;
            //mov ecx,pCLRCreateInstance
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region pMetaHost->GetRuntime(L"v4.0.30319", IID_ICLRRuntimeInfo, (LPVOID*)&pRuntimeInfo);
            p[0] = 0x8D;
            p[1] = 0x45;
            p[2] = 0xF8;
            p += 3;
            //lea eax,[ebp-0x8]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + IID_ICLRRuntimeInfoOffset;
            p += 5;
            //push dword pIID_ICLRRuntimeInfo
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + CLRVersionOffset;
            p += 5;
            //push dword pCLRVersion
            p[0] = 0x8B;
            p[1] = 0x4D;
            p[2] = 0xFC;
            p += 3;
            //mov ecx,[ebp-0x4]
            p[0] = 0x8B;
            p[1] = 0x11;
            p += 2;
            //mov edx,[ecx]
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xFC;
            p += 3;
            //mov eax,[ebp-0x4]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x8B;
            p[1] = 0x4A;
            p[2] = 0x0C;
            p += 3;
            //mov ecx,[edx+0xc]
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntimeHost);
            p[0] = 0x8D;
            p[1] = 0x45;
            p[2] = 0xF4;
            p += 3;
            //lea eax,[ebp-0xc]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + IID_ICLRRuntimeHostOffset;
            p += 5;
            //push dword pIID_ICLRRuntimeHost
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + CLSID_CLRRuntimeHostOffset;
            p += 5;
            //push dword pCLSID_CLRRuntimeHost
            p[0] = 0x8B;
            p[1] = 0x4D;
            p[2] = 0xF8;
            p += 3;
            //mov ecx,[ebp-0x8]
            p[0] = 0x8B;
            p[1] = 0x11;
            p += 2;
            //mov edx,[ecx]
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xF8;
            p += 3;
            //mov eax,[ebp-0x8]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x8B;
            p[1] = 0x4A;
            p[2] = 0x24;
            p += 3;
            //mov ecx,[edx+0x24]
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region pRuntimeHost->Start();
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xF4;
            p += 3;
            //mov eax,[ebp-0xc]
            p[0] = 0x8B;
            p[1] = 0x08;
            p += 2;
            //mov ecx,[eax]
            p[0] = 0x8B;
            p[1] = 0x55;
            p[2] = 0xF4;
            p += 3;
            //mov edx,[ebp-0xc]
            p[0] = 0x52;
            p += 1;
            //push edx
            p[0] = 0x8B;
            p[1] = 0x41;
            p[2] = 0x0C;
            p += 3;
            //mov eax,[ecx+0xc]
            p[0] = 0xFF;
            p[1] = 0xD0;
            p += 2;
            //call eax
            #endregion
            #region return pRuntimeHost->ExecuteInDefaultAppDomain(L"assemblyPath", L"typeName", L"methodName", L"argument", pReturnValue);
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0x08;
            p += 3;
            //mov eax,[ebp+0x8]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + ArgumentOffset;
            p += 5;
            //push dword pArgument
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + MethodNameOffset;
            p += 5;
            //push dword pMethodName
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + TypeNameOffset;
            p += 5;
            //push dword pTypeName
            p[0] = 0x68;
            *(int*)(p + 1) = pFunction + AssemblyPathOffset;
            p += 5;
            //push dword pAssemblyPath
            p[0] = 0x8B;
            p[1] = 0x4D;
            p[2] = 0xF4;
            p += 3;
            //mov ecx,[ebp-0xc]
            p[0] = 0x8B;
            p[1] = 0x11;
            p += 2;
            //mov edx,[ecx]
            p[0] = 0x8B;
            p[1] = 0x45;
            p[2] = 0xF4;
            p += 3;
            //mov eax,[ebp-0xc]
            p[0] = 0x50;
            p += 1;
            //push eax
            p[0] = 0x8B;
            p[1] = 0x4A;
            p[2] = 0x2C;
            p += 3;
            //mov ecx,[edx+0x2c]
            p[0] = 0xFF;
            p[1] = 0xD1;
            p += 2;
            //call ecx
            #endregion
            #region }
            p[0] = 0x5F;
            p += 1;
            //pop edi
            p[0] = 0x5E;
            p += 1;
            //pop esi
            p[0] = 0x5B;
            p += 1;
            //pop ebx
            p[0] = 0x89;
            p[1] = 0xEC;
            p += 2;
            //mov esp,ebp
            p[0] = 0x5D;
            p += 1;
            //pop ebp
            p[0] = 0xC2;
            p[1] = 0x04;
            p[2] = 0x00;
            p += 3;
            //ret 0x4
            #endregion
        }

        /// <summary>
        /// 设置启动64位CLR V2的机器码
        /// </summary>
        /// <param name="p">机器码指针</param>
        /// <param name="pFunction">函数指针</param>
        /// <param name="pCorBindToRuntimeEx">CorBindToRuntimeEx的函数指针</param>
        private static unsafe void SetAsm64V2(byte* p, long pFunction, long pCorBindToRuntimeEx)
        {
            //HRESULT WINAPI LoadCLR2(DWORD *pReturnValue)
            #region {
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x08;
            p += 5;
            //mov [rsp+0x8],rcx
            p[0] = 0x55;
            p += 1;
            //push rbp
            p[0] = 0x48;
            p[1] = 0x81;
            p[2] = 0xEC;
            p[3] = 0x80;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            //sub rsp,0x80
            p[0] = 0x48;
            p[1] = 0x8D;
            p[2] = 0x6C;
            p[3] = 0x24;
            p[4] = 0x30;
            p += 5;
            //lea rbp,[rsp+0x30]
            #endregion
            #region ICLRRuntimeHost *pRuntimeHost = nullptr;
            p[0] = 0x48;
            p[1] = 0xC7;
            p[2] = 0x45;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p[7] = 0x00;
            p += 8;
            //mov qword [rbp+0x0],0x0
            #endregion
            #region CorBindToRuntimeEx(L"v2.0.50727", nullptr, 0, CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntimeHost);
            p[0] = 0x48;
            p[1] = 0x8D;
            p[2] = 0x45;
            p[3] = 0x00;
            p += 4;
            //lea rax,[rbp+0x0]
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x44;
            p[3] = 0x24;
            p[4] = 0x28;
            p += 5;
            //mov [rsp+0x28],rax
            p[0] = 0x48;
            p[1] = 0xB8;
            *(long*)(p + 2) = pFunction + IID_ICLRRuntimeHostOffset;
            p += 10;
            //mov rax,pIID_ICLRRuntimeHost
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x44;
            p[3] = 0x24;
            p[4] = 0x20;
            p += 5;
            //mov [rsp+0x20],rax
            p[0] = 0x49;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + CLSID_CLRRuntimeHostOffset;
            p += 10;
            //mov r9,pCLSID_CLRRuntimeHost
            p[0] = 0x45;
            p[1] = 0x31;
            p[2] = 0xC0;
            p += 3;
            //xor r8d,r8d
            p[0] = 0x31;
            p[1] = 0xD2;
            p += 2;
            //xor edx,edx
            p[0] = 0x48;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + CLRVersionOffset;
            p += 10;
            //mov rcx,pCLRVersion
            p[0] = 0x49;
            p[1] = 0xBF;
            *(long*)(p + 2) = pCorBindToRuntimeEx;
            p += 10;
            //mov r15,pCorBindToRuntimeEx
            p[0] = 0x41;
            p[1] = 0xFF;
            p[2] = 0xD7;
            p += 3;
            //call r15
            #endregion
            #region pRuntimeHost->Start();
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x00;
            p += 4;
            //mov rax,[rbp+0x0]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x00;
            p += 4;
            //mov rcx,[rbp+0x0]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x18;
            p += 3;
            //call [rax+0x18]
            #endregion
            #region return pRuntimeHost->ExecuteInDefaultAppDomain(L"assemblyPath", L"typeName", L"methodName", L"argument", pReturnValue);
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x00;
            p += 4;
            //mov rax,[rbp+0x0]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x60;
            p += 4;
            //mov rcx,[rbp+0x60]
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x28;
            p += 5;
            //mov [rsp+0x28],rcx
            p[0] = 0x48;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + ArgumentOffset;
            p += 10;
            //mov rcx,pArgument
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x20;
            p += 5;
            //mov [rsp+0x20],rcx
            p[0] = 0x49;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + MethodNameOffset;
            p += 10;
            //mov r9,pMethodName
            p[0] = 0x49;
            p[1] = 0xB8;
            *(long*)(p + 2) = pFunction + TypeNameOffset;
            p += 10;
            //mov r8,pTypeName
            p[0] = 0x48;
            p[1] = 0xBA;
            *(long*)(p + 2) = pFunction + AssemblyPathOffset;
            p += 10;
            //mov rdx,pAssemblyPath
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x00;
            p += 4;
            //mov rcx,[rbp+0x0]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x58;
            p += 3;
            //call [rax+0x58]
            #endregion
            #region }
            p[0] = 0x48;
            p[1] = 0x8D;
            p[2] = 0x65;
            p[3] = 0x50;
            p += 4;
            //lea rsp,[rbp+0x50]
            p[0] = 0x5D;
            p += 1;
            //pop rbp
            p[0] = 0xC3;
            p += 1;
            //ret
            #endregion
        }

        /// <summary>
        /// 设置启动64位CLR V4的机器码
        /// </summary>
        /// <param name="p">机器码指针</param>
        /// <param name="pFunction">函数指针</param>
        /// <param name="pCLRCreateInstance">CLRCreateInstance的函数指针</param>
        private static unsafe void SetAsm64V4(byte* p, long pFunction, long pCLRCreateInstance)
        {
            //HRESULT WINAPI LoadCLR4(DWORD *pReturnValue)
            #region {
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x08;
            p += 5;
            //mov [rsp+0x8],rcx
            p[0] = 0x55;
            p += 1;
            //push rbp
            p[0] = 0x48;
            p[1] = 0x81;
            p[2] = 0xEC;
            p[3] = 0x90;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p += 7;
            //sub rsp,0x90
            p[0] = 0x48;
            p[1] = 0x8D;
            p[2] = 0x6C;
            p[3] = 0x24;
            p[4] = 0x30;
            p += 5;
            //lea rbp,[rsp+0x30]
            #endregion
            #region ICLRMetaHost *pMetaHost = nullptr;
            p[0] = 0x48;
            p[1] = 0xC7;
            p[2] = 0x45;
            p[3] = 0x00;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p[7] = 0x00;
            p += 8;
            //mov qword [rbp+0x0],0x0
            #endregion
            #region ICLRRuntimeInfo *pRuntimeInfo = nullptr;
            p[0] = 0x48;
            p[1] = 0xC7;
            p[2] = 0x45;
            p[3] = 0x08;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p[7] = 0x00;
            p += 8;
            //mov qword [rbp+0x8],0x0
            #endregion
            #region ICLRRuntimeHost *pRuntimeHost = nullptr;
            p[0] = 0x48;
            p[1] = 0xC7;
            p[2] = 0x45;
            p[3] = 0x10;
            p[4] = 0x00;
            p[5] = 0x00;
            p[6] = 0x00;
            p[7] = 0x00;
            p += 8;
            //mov qword [rbp+0x10],0x0
            #endregion
            #region CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&pMetaHost);
            p[0] = 0x4C;
            p[1] = 0x8D;
            p[2] = 0x45;
            p[3] = 0x00;
            p += 4;
            //lea r8,[rbp+0x0]
            p[0] = 0x48;
            p[1] = 0xBA;
            *(long*)(p + 2) = pFunction + IID_ICLRMetaHostOffset;
            p += 10;
            //mov rdx,pIID_ICLRMetaHost
            p[0] = 0x48;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + CLSID_CLRMetaHostOffset;
            p += 10;
            //mov rcx,pCLSID_CLRMetaHost
            p[0] = 0x49;
            p[1] = 0xBF;
            *(long*)(p + 2) = pCLRCreateInstance;
            p += 10;
            //mov r15,pCLRCreateInstance
            p[0] = 0x41;
            p[1] = 0xFF;
            p[2] = 0xD7;
            p += 3;
            //call r15
            #endregion
            #region pMetaHost->GetRuntime(L"v4.0.30319", IID_ICLRRuntimeInfo, (LPVOID*)&pRuntimeInfo);
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x00;
            p += 4;
            //mov rax,[rbp+0x0]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x4C;
            p[1] = 0x8D;
            p[2] = 0x4D;
            p[3] = 0x08;
            p += 4;
            //lea r9,[rbp+0x8]
            p[0] = 0x49;
            p[1] = 0xB8;
            *(long*)(p + 2) = pFunction + IID_ICLRRuntimeInfoOffset;
            p += 10;
            //mov r8,pIID_ICLRRuntimeInfo
            p[0] = 0x48;
            p[1] = 0xBA;
            *(long*)(p + 2) = pFunction + CLRVersionOffset;
            p += 10;
            //mov rdx,pCLRVersion
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x00;
            p += 4;
            //mov rcx,[rbp+0x0]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x18;
            p += 3;
            //call [rax+0x18]
            #endregion
            #region pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntimeHost);
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x08;
            p += 4;
            //mov rax,[rbp+0x8]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x4C;
            p[1] = 0x8D;
            p[2] = 0x4D;
            p[3] = 0x10;
            p += 4;
            //lea r9,[rbp+0x10]
            p[0] = 0x49;
            p[1] = 0xB8;
            *(long*)(p + 2) = pFunction + IID_ICLRRuntimeHostOffset;
            p += 10;
            //mov r8,pIID_ICLRRuntimeHost
            p[0] = 0x48;
            p[1] = 0xBA;
            *(long*)(p + 2) = pFunction + CLSID_CLRRuntimeHostOffset;
            p += 10;
            //mov rdx,pCLSID_CLRRuntimeHost
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x08;
            p += 4;
            //mov rcx,[rbp+0x8]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x48;
            p += 3;
            //call [rax+0x48]
            #endregion
            #region pRuntimeHost->Start();
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x10;
            p += 4;
            //mov rax,[rbp+0x10]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x10;
            p += 4;
            //mov rcx,[rbp+0x10]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x18;
            p += 3;
            //call [rax+0x18]
            #endregion
            #region return pRuntimeHost->ExecuteInDefaultAppDomain(L"assemblyPath", L"typeName", L"methodName", L"argument", pReturnValue);
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x45;
            p[3] = 0x10;
            p += 4;
            //mov rax,[rbp+0x10]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x00;
            p += 3;
            //mov rax,[rax]
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x70;
            p += 4;
            //mov rcx,[rbp+0x70]
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x28;
            p += 5;
            //mov [rsp+0x28],rcx
            p[0] = 0x48;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + ArgumentOffset;
            p += 10;
            //mov rcx,pArgument
            p[0] = 0x48;
            p[1] = 0x89;
            p[2] = 0x4C;
            p[3] = 0x24;
            p[4] = 0x20;
            p += 5;
            //mov [rsp+0x20],rcx
            p[0] = 0x49;
            p[1] = 0xB9;
            *(long*)(p + 2) = pFunction + MethodNameOffset;
            p += 10;
            //mov r9,pMethodName
            p[0] = 0x49;
            p[1] = 0xB8;
            *(long*)(p + 2) = pFunction + TypeNameOffset;
            p += 10;
            //mov r8,pTypeName
            p[0] = 0x48;
            p[1] = 0xBA;
            *(long*)(p + 2) = pFunction + AssemblyPathOffset;
            p += 10;
            //mov rdx,pAssemblyPath
            p[0] = 0x48;
            p[1] = 0x8B;
            p[2] = 0x4D;
            p[3] = 0x10;
            p += 4;
            //mov rcx,[rbp+0x10]
            p[0] = 0xFF;
            p[1] = 0x50;
            p[2] = 0x58;
            p += 3;
            //call [rax+0x58]
            #endregion
            #region }
            p[0] = 0x48;
            p[1] = 0x8D;
            p[2] = 0x65;
            p[3] = 0x60;
            p += 4;
            //lea rsp,[rbp+0x60]
            p[0] = 0x5D;
            p += 1;
            //pop rbp
            p[0] = 0xC3;
            p += 1;
            //ret
            #endregion
        }

        /// <summary>
        /// 判断是否为程序集，如果是，输出CLR版本
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isAssembly">是否程序集</param>
        /// <param name="clrVersion">CLR版本</param>
        private static void IsAssembly(string path, out bool isAssembly, out string clrVersion)
        {
            BinaryReader binaryReader;

            try
            {
                using (binaryReader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                    clrVersion = GetVersionString(binaryReader);
                isAssembly = true;
            }
            catch
            {
                clrVersion = null;
                isAssembly = false;
            }
        }

        /// <summary>
        /// 获取CLR版本
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        private static string GetVersionString(BinaryReader binaryReader)
        {
            uint peOffset;
            bool is64;
            Section[] sections;
            uint rva;
            Section? section;

            GetPEInfo(binaryReader, out peOffset, out is64);
            binaryReader.BaseStream.Position = peOffset + (is64 ? 0xF8 : 0xE8);
            rva = binaryReader.ReadUInt32();
            //.Net MetaData Directory RVA
            if (rva == 0)
                throw new BadImageFormatException("文件不是程序集");
            sections = GetSections(binaryReader);
            section = GetSection(rva, sections);
            if (section == null)
                throw new InvalidDataException("未知格式的二进制文件");
            binaryReader.BaseStream.Position = section.Value.PointerToRawData + rva - section.Value.VirtualAddress + 0x8;
            //.Net MetaData Directory FileOffset
            rva = binaryReader.ReadUInt32();
            //.Net MetaData RVA
            if (rva == 0)
                throw new BadImageFormatException("文件不是程序集");
            section = GetSection(rva, sections);
            if (section == null)
                throw new InvalidDataException("未知格式的二进制文件");
            binaryReader.BaseStream.Position = section.Value.PointerToRawData + rva - section.Value.VirtualAddress + 0xC;
            //.Net MetaData FileOffset
            return Encoding.UTF8.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32() - 2));
        }

        /// <summary>
        /// 获取PE信息
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="peOffset"></param>
        /// <param name="is64"></param>
        private static void GetPEInfo(BinaryReader binaryReader, out uint peOffset, out bool is64)
        {
            ushort machine;

            binaryReader.BaseStream.Position = 0x3C;
            peOffset = binaryReader.ReadUInt32();
            binaryReader.BaseStream.Position = peOffset + 0x4;
            machine = binaryReader.ReadUInt16();
            if (machine != 0x14C && machine != 0x8664)
                throw new InvalidDataException("未知格式的二进制文件");
            is64 = machine == 0x8664;
        }

        /// <summary>
        /// 获取节
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        private static Section[] GetSections(BinaryReader binaryReader)
        {
            uint ntHeaderOffset;
            bool is64;
            ushort numberOfSections;
            Section[] sections;

            GetPEInfo(binaryReader, out ntHeaderOffset, out is64);
            numberOfSections = binaryReader.ReadUInt16();
            binaryReader.BaseStream.Position = ntHeaderOffset + (is64 ? 0x108 : 0xF8);
            sections = new Section[numberOfSections];
            for (int i = 0; i < numberOfSections; i++)
            {
                binaryReader.BaseStream.Position += 0x8;
                sections[i] = new Section(binaryReader.ReadUInt32(), binaryReader.ReadUInt32(), binaryReader.ReadUInt32(), binaryReader.ReadUInt32());
                binaryReader.BaseStream.Position += 0x10;
            }
            return sections;
        }

        /// <summary>
        /// 获取RVA对应节
        /// </summary>
        /// <param name="rva"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        private static Section? GetSection(uint rva, Section[] sections)
        {
            foreach (Section section in sections)
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + Math.Max(section.VirtualSize, section.SizeOfRawData))
                    return section;
            return null;
        }
    }
}
