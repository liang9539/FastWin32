using System;
using System.IO;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 进程
    /// </summary>
    public static class Process32
    {
        /// <summary>
        /// 打开进程（内存读+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessVMReadQuery(uint processId)
        {
            return OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);
        }

        /// <summary>
        /// 打开进程（进程挂起/恢复）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessProcessSuspendResume(uint processId)
        {
            return OpenProcess(PROCESS_SUSPEND_RESUME, false, processId);
        }

        /// <summary>
        /// 通过窗口句柄获取进程ID
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static unsafe uint GetProcessIdByWindowHandle(IntPtr windowHandle)
        {
            uint processId;

            GetWindowThreadProcessId(windowHandle, &processId);
            return processId;
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static string GetProcessName(uint processId)
        {
            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return null;
            return GetProcessNameInternal(processHandle);
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessNameInternal(IntPtr processHandle)
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder((int)MODULENAME_MAX_LENGTH);
            if (GetProcessImageFileName(processHandle, stringBuilder, (int)MODULENAME_MAX_LENGTH) == 0)
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
            IntPtr processHandle;

            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
                return null;
            return GetProcessNameInternal(processHandle);
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessPathInternal(IntPtr processHandle)
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder((int)MAX_PATH);
            if (GetProcessImageFileName(processHandle, stringBuilder, MAX_PATH) == 0)
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
            IntPtr processHandle;

            if (!Environment.Is64BitOperatingSystem)
            {
                //不是64位系统肯定不会是64位进程
                is64 = false;
                return true;
            }
            processHandle = OpenProcessVMReadQuery(processId);
            if (processHandle == IntPtr.Zero)
            {
                is64 = false;
                return false;
            }
            return Is64ProcessInternal(processHandle, out is64);
        }

        /// <summary>
        /// 判断进程是否为64位进程，返回值为方法是否执行成功
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="is64">是否为64位进程</param>
        /// <returns></returns>
        internal static bool Is64ProcessInternal(IntPtr processHandle, out bool is64)
        {
            bool isWow64;

            if (!Environment.Is64BitOperatingSystem)
            {
                //不是64位系统肯定不会是64位进程
                is64 = false;
                return true;
            }
            if (!IsWow64Process(processHandle, out isWow64))
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
            IntPtr processHandle;

            processHandle = OpenProcessProcessSuspendResume(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            return SuspendProcessInternal(processHandle);
        }

        /// <summary>
        /// 暂停进程
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static bool SuspendProcessInternal(IntPtr processHandle)
        {
            return ZwSuspendProcess(processHandle) != unchecked((uint)-1);
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static bool ResumeProcess(uint processId)
        {
            IntPtr processHandle;

            processHandle = OpenProcessProcessSuspendResume(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            return ResumeProcessInternal(processHandle);
        }

        /// <summary>
        /// 恢复进程
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static bool ResumeProcessInternal(IntPtr processHandle)
        {
            return ZwResumeProcess(processHandle) != unchecked((uint)-1);
        }
    }
}
