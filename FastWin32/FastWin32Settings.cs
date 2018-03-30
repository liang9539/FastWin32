using System;
using System.Diagnostics;

namespace FastWin32
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public static class FastWin32Settings
    {
        /// <summary>
        /// 确定当前操作系统是否为 64 位操作系统。
        /// </summary>
        public static readonly bool Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;

        /// <summary>
        /// SeDebugPrivilege特权
        /// </summary>
        public static bool SeDebugPrivilege { get; private set; }

        /// <summary>
        /// 将SeDebugPrivilege特权赋予当前进程
        /// </summary>
        /// <returns></returns>
        public static bool EnableDebugPrivilege()
        {
            if (SeDebugPrivilege)
                return true;
            try
            {
                Process.EnterDebugMode();
                SeDebugPrivilege = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 取消当前进程的SeDebugPrivilege特权
        /// </summary>
        /// <returns></returns>
        public static bool DisableDebugPrivilege()
        {
            if (!SeDebugPrivilege)
                return true;
            try
            {
                Process.LeaveDebugMode();
                SeDebugPrivilege = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
