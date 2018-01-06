using System;
using System.IO;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 进程
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// 通过窗口句柄获取进程ID
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static uint GetProcessIdByHWnd(IntPtr hWnd)
        {
            uint processId;

            GetWindowThreadProcessId(hWnd, out processId);
            return processId;
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static string GetProcessName(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess == IntPtr.Zero)
                return null;
            return GetProcessNameInternal(hProcess);
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessNameInternal(IntPtr hProcess)
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder((int)MAX_MODULE_NAME32);
            if (GetProcessImageFileName(hProcess, stringBuilder, (int)MAX_MODULE_NAME32) == 0)
                return null;
            return Path.GetFileName(stringBuilder.ToString());
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static string GetProcessPath(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess == IntPtr.Zero)
                return null;
            return GetProcessNameInternal(hProcess);
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessPathInternal(IntPtr hProcess)
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder(100);
            if (GetProcessImageFileName(hProcess, stringBuilder, 100) == 0)
                return null;
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 判断进程是否为64位进程，返回值为方法是否执行成功
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="is64">是否为64位进程</param>
        /// <returns></returns>
        public static bool Is64Process(uint processId, out bool is64)
        {
            IntPtr hProcess;

            if (!Environment.Is64BitOperatingSystem)
            {
                //不是64位系统肯定不会是64位进程
                is64 = false;
                return true;
            }
            hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                is64 = false;
                return false;
            }
            return Is64ProcessInternal(hProcess, out is64);
        }

        /// <summary>
        /// 判断进程是否为64位进程，返回值为方法是否执行成功
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="is64">是否为64位进程</param>
        /// <returns></returns>
        internal static bool Is64ProcessInternal(IntPtr hProcess, out bool is64)
        {
            bool isWow64;

            if (!Environment.Is64BitOperatingSystem)
            {
                //不是64位系统肯定不会是64位进程
                is64 = false;
                return true;
            }
            if (!IsWow64Process(hProcess, out isWow64))
            {
                //执行失败
                is64 = false;
                return false;
            }
            is64 = !isWow64;
            return true;
        }

        /// <summary>
        /// 暂停进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static bool SuspendProcess(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(PROCESS_SUSPEND_RESUME, false, processId);
            if (hProcess == IntPtr.Zero)
                return false;
            return SuspendProcessInternal(hProcess);
        }

        /// <summary>
        /// 暂停进程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns></returns>
        internal static bool SuspendProcessInternal(IntPtr hProcess)
        {
            return ZwSuspendProcess(hProcess) != unchecked((uint)-1);
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static bool ResumeProcess(uint processId)
        {
            IntPtr hProcess;

            hProcess = OpenProcess(PROCESS_SUSPEND_RESUME, false, processId);
            if (hProcess == IntPtr.Zero)
                return false;
            return ResumeProcessInternal(hProcess);
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns></returns>
        internal static bool ResumeProcessInternal(IntPtr hProcess)
        {
            return ZwResumeProcess(hProcess) != unchecked((uint)-1);
        }
    }
}
