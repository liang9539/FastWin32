using System;
using System.ComponentModel;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 模块
    /// </summary>
    public static class ModuleX
    {
        #region 打开进程
        /// <summary>
        /// 打开进程（读取+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcess(uint processId)
        {
            return NativeMethods.OpenProcess(ProcAccessFlags.PROCESS_VM_READ | ProcAccessFlags.PROCESS_QUERY_INFORMATION, false, processId);
        }
        #endregion

        #region 获取模块句柄
        /// <summary>
        /// 获取模块句柄
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="first">是否返回第一个模块句柄</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="flag">过滤标识</param>
        /// <param name="value">模块句柄</param>
        /// <returns></returns>
        internal static unsafe bool GetHandleInternal(IntPtr hProcess, bool first, string moduleName, EnumModulesFilterFlag flag, out IntPtr value)
        {
            IntPtr hModule = IntPtr.Zero;
            IntPtr[] hModules;
            StringBuilder baseName;
            string lowerName;
            uint cb = 0;

            value = IntPtr.Zero;
            if (!EnumProcessModulesEx(hProcess, &hModule, (uint)IntPtr.Size, ref cb, flag))
                //先获取储存所有模块句柄所需的字节数
                return false;
            if (first)
            {
                //返回第一个模块句柄
                value = hModule;
                return true;
            }
            hModules = new IntPtr[cb / IntPtr.Size];
            //根据所需字节数创建数组
            fixed (IntPtr* p = &hModules[0])
            {
                if (!EnumProcessModulesEx(hProcess, p, cb, ref cb, flag))
                    //获取所有模块句柄
                    return false;
            }
            baseName = new StringBuilder((int)MAX_MODULE_NAME32);
            //储存模块名
            lowerName = moduleName.ToLower();
            //获取小写模块名
            for (int i = 0; i < hModules.Length; i++)
            {
                //遍历所有模块名
                if (!GetModuleBaseName(hProcess, hModules[i], baseName, MAX_MODULE_NAME32))
                    //获取模块名失败
                    throw new Win32Exception();
                if (baseName.ToString().ToLower() == lowerName)
                {
                    //如果相等
                    value = hModules[i];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取主模块句柄
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr GetHandle(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(processId);
            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            GetHandleInternal(hProcess, true, string.Empty, EnumModulesFilterFlag.DEFAULT, out IntPtr Handle);
            CloseHandle(hProcess);
            return Handle;
        }

        /// <summary>
        /// 获取模块句柄
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static IntPtr GetHandle(uint processId, string moduleName)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(processId);
            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            GetHandleInternal(hProcess, false, moduleName, EnumModulesFilterFlag.ALL, out IntPtr Handle);
            CloseHandle(hProcess);
            return Handle;
        }
        #endregion
    }
}
