using System;
using System.Text;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 遍历模块回调方法，要继续遍历,返回true;要停止遍历,返回false
    /// </summary>
    /// <param name="moduleHandle">模块句柄</param>
    /// <param name="moduleName">模块名</param>
    /// <param name="filePath">模块文件所在路径</param>
    /// <returns></returns>
    public delegate bool EnumModulesCallback(IntPtr moduleHandle, string moduleName, string filePath);

    /// <summary>
    /// 遍历模块导出函数回调方法，要继续遍历,返回true;要停止遍历,返回false
    /// </summary>
    /// <param name="pFunction">函数指针</param>
    /// <param name="functionName">函数名，当函数以序号方式导出时，此参数为null</param>
    /// <param name="ordinal">函数导出序号</param>
    /// <returns></returns>
    public delegate bool EnumFunctionsCallback(IntPtr pFunction, string functionName, short ordinal);

    /// <summary>
    /// 模块
    /// </summary>
    public static class Module32
    {
        /// <summary>
        /// 打开进程（内存读+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessVMReadQuery(uint processId)
        {
            return OpenProcess(FastWin32Settings.SeDebugPrivilege ? PROCESS_ALL_ACCESS : PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);
        }

        /// <summary>
        /// 获取当前进程主模块句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetHandle()
        {
            return GetModuleHandle(null);
        }

        /// <summary>
        /// 获取当前进程模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static IntPtr GetHandle(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException();

            return GetModuleHandle(moduleName);
        }

        /// <summary>
        /// 获取主模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr GetHandle(uint processId)
        {
            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return GetHandleInternal(processHandle, true, null);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 获取模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static IntPtr GetHandle(uint processId, string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException();

            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return GetHandleInternal(processHandle, false, moduleName);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 获取模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="first">是否返回第一个模块句柄</param>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        internal static unsafe IntPtr GetHandleInternal(IntPtr processHandle, bool first, string moduleName)
        {
            bool is64;
            bool isXP;
            IntPtr moduleHandle;
            uint size;
            IntPtr[] moduleHandles;
            StringBuilder moduleNameBuffer;

            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return IntPtr.Zero;
            isXP = Environment.OSVersion.Version.Major == 5;
            if (isXP)
            {
                //XP兼容
                if (!EnumProcessModules(processHandle, &moduleHandle, (uint)IntPtr.Size, out size))
                    return IntPtr.Zero;
            }
            else
            {
                if (!EnumProcessModulesEx(processHandle, &moduleHandle, (uint)IntPtr.Size, out size, is64 ? LIST_MODULES_64BIT : LIST_MODULES_32BIT))
                    //先获取储存所有模块句柄所需的字节数
                    return IntPtr.Zero;
            }
            if (first)
                //返回第一个模块句柄
                return moduleHandle;
            moduleHandles = new IntPtr[size / IntPtr.Size];
            fixed (IntPtr* p = &moduleHandles[0])
                if (isXP)
                {
                    //XP兼容
                    if (!EnumProcessModules(processHandle, p, size, out size))
                        return IntPtr.Zero;
                }
                else
                {
                    if (!EnumProcessModulesEx(processHandle, p, size, out size, is64 ? LIST_MODULES_64BIT : LIST_MODULES_32BIT))
                        //获取所有模块句柄
                        return IntPtr.Zero;
                }
            moduleNameBuffer = new StringBuilder((int)MODULENAME_MAX_LENGTH);
            for (int i = 0; i < moduleHandles.Length; i++)
            {
                if (!GetModuleBaseName(processHandle, moduleHandles[i], moduleNameBuffer, MODULENAME_MAX_LENGTH))
                    return IntPtr.Zero;
                if (moduleNameBuffer.ToString().Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                    return moduleHandles[i];
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 遍历模块，遍历成功返回true，失败返回false（返回值与回调方法的返回值无关）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="callback">回调方法，不能为空</param>
        /// <param name="getModuleName">是否向回调方法提供模块名，默认为是</param>
        /// <param name="getFilePath">是否向回调方法提供模块文件路径，默认为否</param>
        /// <returns></returns>
        public static unsafe bool EnumModules(uint processId, EnumModulesCallback callback, bool getModuleName = true, bool getFilePath = false)
        {
            if (callback == null)
                throw new ArgumentNullException();

            IntPtr processHandle;
            bool is64;
            bool isXP;
            IntPtr moduleHandle;
            uint size;
            IntPtr[] moduleHandles;
            StringBuilder moduleName;
            StringBuilder filePath;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            try
            {
                if (!Process32.Is64ProcessInternal(processHandle, out is64))
                    return false;
                isXP = Environment.OSVersion.Version.Major == 5;
                if (isXP)
                {
                    //XP兼容
                    if (!EnumProcessModules(processHandle, &moduleHandle, (uint)IntPtr.Size, out size))
                        return false;
                }
                else
                {
                    if (!EnumProcessModulesEx(processHandle, &moduleHandle, (uint)IntPtr.Size, out size, is64 ? LIST_MODULES_64BIT : LIST_MODULES_32BIT))
                        //先获取储存所有模块句柄所需的字节数
                        return false;
                }
                moduleHandles = new IntPtr[size / IntPtr.Size];
                fixed (IntPtr* p = &moduleHandles[0])
                    if (isXP)
                    {
                        //XP兼容
                        if (!EnumProcessModules(processHandle, p, size, out size))
                            return false;
                    }
                    else
                    {
                        if (!EnumProcessModulesEx(processHandle, p, size, out size, is64 ? LIST_MODULES_64BIT : LIST_MODULES_32BIT))
                            //获取所有模块句柄
                            return false;
                    }
                moduleName = getModuleName ? new StringBuilder((int)MODULENAME_MAX_LENGTH) : null;
                filePath = getFilePath ? new StringBuilder((int)MAX_PATH) : null;
                for (int i = 0; i < moduleHandles.Length; i++)
                {
                    if (getModuleName && !GetModuleBaseName(processHandle, moduleHandles[i], moduleName, MODULENAME_MAX_LENGTH))
                        return false;
                    if (getFilePath && GetModuleFileName(processHandle, filePath, MAX_PATH) == 0)
                        return false;
                    if (!callback(moduleHandles[i], getModuleName ? moduleName.ToString() : null, getFilePath ? filePath.ToString() : null))
                        return true;
                }
                return true;
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 获取函数地址
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="functionName">函数名</param>
        /// <returns></returns>
        public static IntPtr GetProcAddress(string moduleName, string functionName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException();

            return GetProcAddressInternal(moduleName, functionName);
        }

        /// <summary>
        /// 获取远程进程函数地址
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="functionName">函数名</param>
        /// <returns></returns>
        public static IntPtr GetProcAddress(uint processId, string moduleName, string functionName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException();

            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return GetProcAddressInternal(processHandle, moduleName, functionName);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 获取函数地址
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="functionName">函数名</param>
        /// <returns></returns>
        internal static IntPtr GetProcAddressInternal(string moduleName, string functionName)
        {
            IntPtr moduleHandle;

            moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
                return IntPtr.Zero;
            return NativeMethods.GetProcAddress(moduleHandle, functionName);
        }

        /// <summary>
        /// 获取远程进程函数地址
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="functionName">函数名</param>
        /// <returns></returns>
        internal static unsafe IntPtr GetProcAddressInternal(IntPtr processHandle, string moduleName, string functionName)
        {
            IntPtr moduleHandle;
            int ntHeaderOffset;
            bool is64;
            int iedRVA;
            IMAGE_EXPORT_DIRECTORY ied;
            int[] nameOffsets;
            string name;
            short ordinal;
            int addressOffset;

            moduleHandle = GetHandleInternal(processHandle, false, moduleName);
            if (moduleHandle == IntPtr.Zero)
                return IntPtr.Zero;
            if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + 0x3C, out ntHeaderOffset))
                return IntPtr.Zero;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return IntPtr.Zero;
            if (is64)
            {
                if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + ntHeaderOffset + 0x88, out iedRVA))
                    return IntPtr.Zero;
            }
            else
            {
                if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + ntHeaderOffset + 0x78, out iedRVA))
                    return IntPtr.Zero;
            }
            if (!ReadProcessMemory(processHandle, moduleHandle + iedRVA, &ied, 40, null))
                return IntPtr.Zero;
            nameOffsets = new int[ied.NumberOfNames];
            fixed (void* p = &nameOffsets[0])
                if (!ReadProcessMemory(processHandle, moduleHandle + (int)ied.AddressOfNames, p, ied.NumberOfNames * 4, null))
                    return IntPtr.Zero;
            for (int i = 0; i < ied.NumberOfNames; i++)
            {
                if (!MemoryIO.ReadStringInternal(processHandle, moduleHandle + nameOffsets[i], out name, 40, false, Encoding.ASCII))
                    return IntPtr.Zero;
                if (name == functionName)
                {
                    if (!MemoryIO.ReadInt16Internal(processHandle, moduleHandle + (int)ied.AddressOfNameOrdinals + i * 2, out ordinal))
                        return IntPtr.Zero;
                    if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + (int)ied.AddressOfFunctions + ordinal * 4, out addressOffset))
                        return IntPtr.Zero;
                    return moduleHandle + addressOffset;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 枚举模块导出函数
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        public static bool EnumFunctions(uint processId, string moduleName, EnumFunctionsCallback callback)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException();
            if (callback == null)
                throw new ArgumentNullException();

            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            try
            {
                return EnumFunctions(processHandle, moduleName, callback);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 枚举模块导出函数
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleHandle">模块句柄</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        public static bool EnumFunctions(uint processId, IntPtr moduleHandle, EnumFunctionsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException();

            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            try
            {
                return EnumFunctions(processHandle, moduleHandle, callback);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 枚举模块导出函数
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        internal static unsafe bool EnumFunctions(IntPtr processHandle, string moduleName, EnumFunctionsCallback callback)
        {
            IntPtr moduleHandle;

            moduleHandle = GetHandleInternal(processHandle, false, moduleName);
            if (moduleHandle == IntPtr.Zero)
                return false;
            return EnumFunctions(processHandle, moduleHandle, callback);
        }

        /// <summary>
        /// 枚举模块导出函数
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="moduleHandle">模块句柄</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        internal static unsafe bool EnumFunctions(IntPtr processHandle, IntPtr moduleHandle, EnumFunctionsCallback callback)
        {
            int ntHeaderOffset;
            bool is64;
            int iedRVA;
            IMAGE_EXPORT_DIRECTORY ied;
            int[] nameOffsets;
            string functionName;
            short ordinal;
            int addressOffset;

            if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + 0x3C, out ntHeaderOffset))
                return false;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (is64)
            {
                if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + ntHeaderOffset + 0x88, out iedRVA))
                    return false;
            }
            else
            {
                if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + ntHeaderOffset + 0x78, out iedRVA))
                    return false;
            }
            if (!ReadProcessMemory(processHandle, moduleHandle + iedRVA, &ied, 40, null))
                return false;
            if (ied.NumberOfNames == 0)
                //无按名称导出函数
                return true;
            nameOffsets = new int[ied.NumberOfNames];
            fixed (void* p = &nameOffsets[0])
                if (!ReadProcessMemory(processHandle, moduleHandle + (int)ied.AddressOfNames, p, ied.NumberOfNames * 4, null))
                    return false;
            for (int i = 0; i < ied.NumberOfNames; i++)
            {
                if (!MemoryIO.ReadStringInternal(processHandle, moduleHandle + nameOffsets[i], out functionName, 40, false, Encoding.ASCII))
                    return false;
                if (!MemoryIO.ReadInt16Internal(processHandle, moduleHandle + ((int)ied.AddressOfNameOrdinals + i * 2), out ordinal))
                    return false;
                if (!MemoryIO.ReadInt32Internal(processHandle, moduleHandle + ((int)ied.AddressOfFunctions + ordinal * 4), out addressOffset))
                    return false;
                if (!callback(moduleHandle + addressOffset, functionName, ordinal))
                    return true;
            }
            return true;
        }
    }
}
