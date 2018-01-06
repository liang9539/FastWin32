using System;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 模块
    /// </summary>
    public static class Module
    {
        #region 打开进程
        /// <summary>
        /// 打开进程（读取+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessRQuery(uint processId)
        {
            return OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);
        }
        #endregion

        #region 获取模块句柄
        /// <summary>
        /// 获取模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="first">是否返回第一个模块句柄</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="flag">过滤标识</param>
        /// <returns></returns>
        internal static unsafe IntPtr GetHandleInternal(IntPtr hProcess, bool first, string moduleName, uint flag)
        {
            if (!first && string.IsNullOrEmpty(moduleName))
                throw new ArgumentOutOfRangeException("first为false时moduleName不能为空");

            bool is64;
            IntPtr hModule;
            uint cb;
            IntPtr[] hModules;
            StringBuilder baseName;
            string normalizedName;

            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return IntPtr.Zero;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            if (is64 && flag == EnumModulesFilterFlag.X86)
                throw new NotSupportedException("尝试在64位进程中枚举32位模块");
            hModule = IntPtr.Zero;
            if (!EnumProcessModulesEx(hProcess, &hModule, (uint)IntPtr.Size, out cb, flag))
                //先获取储存所有模块句柄所需的字节数
                return IntPtr.Zero;
            if (first)
                //返回第一个模块句柄
                return hModule;
            hModules = new IntPtr[cb / IntPtr.Size];
            //根据所需字节数创建数组
            fixed (IntPtr* p = &hModules[0])
            {
                if (!EnumProcessModulesEx(hProcess, p, cb, out cb, flag))
                    //获取所有模块句柄
                    return IntPtr.Zero;
            }
            baseName = new StringBuilder((int)MAX_MODULE_NAME32);
            //储存模块名
            normalizedName = moduleName.ToUpperInvariant();
            //获取大写模块名
            for (int i = 0; i < hModules.Length; i++)
            {
                //遍历所有模块名
                if (!GetModuleBaseName(hProcess, hModules[i], baseName, MAX_MODULE_NAME32))
                    //获取模块名失败
                    return IntPtr.Zero;
                if (baseName.ToString().ToUpperInvariant() == normalizedName)
                    //比较模块名
                    return hModules[i];
            }
            return IntPtr.Zero;
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
                throw new ArgumentOutOfRangeException();

            return GetModuleHandle(moduleName);
        }

        /// <summary>
        /// 获取主模块句柄，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr GetHandle(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRQuery(processId);
            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return GetHandleInternal(hProcess, true, null, EnumModulesFilterFlag.DEFAULT);
            }
            finally
            {
                CloseHandle(hProcess);
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
                throw new ArgumentOutOfRangeException();

            IntPtr hProcess;

            hProcess = OpenProcessRQuery(processId);
            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return GetHandleInternal(hProcess, false, moduleName, EnumModulesFilterFlag.ALL);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }
        #endregion

        /// <summary>
        /// 获取函数地址
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="funcName">函数名</param>
        /// <returns></returns>
        internal static IntPtr GetFuncAddressInternal(string moduleName, string funcName)
        {
            IntPtr hModule;
            IntPtr pFunc;

            hModule = GetModuleHandle(moduleName);
            if (hModule == IntPtr.Zero)
                return IntPtr.Zero;
            pFunc = GetProcAddress(hModule, funcName);
            if (pFunc == IntPtr.Zero)
                return IntPtr.Zero;
            return pFunc;
        }
        /// <summary>
        /// 获取函数地址
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="funcName">函数名</param>
        /// <returns></returns>
        public static IntPtr GetFuncAddress(string moduleName, string funcName)
        {
            if (moduleName == null || funcName == null)
                throw new ArgumentNullException();
            if (moduleName.Length == 0 || funcName.Length == 0)
                throw new ArgumentOutOfRangeException();

            return GetFuncAddressInternal(moduleName, funcName);
        }
    }
}
