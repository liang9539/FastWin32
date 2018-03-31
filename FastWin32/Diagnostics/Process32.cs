using System;
using System.IO;
using System.Linq;
using System.Text;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 进程
    /// </summary>
    public static unsafe class Process32
    {
        /// <summary>
        /// 打开进程（内存读+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static SafeNativeHandle OpenProcessQuery(uint processId)
        {
            return SafeOpenProcess(FastWin32Settings.SeDebugPrivilege ? PROCESS_ALL_ACCESS : PROCESS_QUERY_INFORMATION, false, processId);
        }

        /// <summary>
        /// 打开进程（进程挂起/恢复）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static SafeNativeHandle OpenProcessProcessSuspendResume(uint processId)
        {
            return SafeOpenProcess(FastWin32Settings.SeDebugPrivilege ? PROCESS_ALL_ACCESS : PROCESS_SUSPEND_RESUME, false, processId);
        }

        /// <summary>
        /// 通过窗口句柄获取进程ID
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        public static uint GetProcessIdByWindowHandle(IntPtr windowHandle)
        {
            uint processId;

            GetWindowThreadProcessId(windowHandle, &processId);
            return processId;
        }

        /// <summary>
        /// 通过线程ID获取进程ID
        /// </summary>
        /// <param name="threadId">线程ID</param>
        /// <returns></returns>
        public static uint GetProcessIdByThreadId(uint threadId)
        {
            SafeNativeHandle threadHandle;

            using (threadHandle = SafeOpenThread(THREAD_QUERY_INFORMATION, false, threadId))
                if (threadHandle.IsValid)
                    return GetProcessIdOfThread(threadHandle);
                else
                    return 0;
        }

        /// <summary>
        /// 获取当前进程ID
        /// </summary>
        /// <returns></returns>
        public static uint GetCurrentProcessId()
        {
            return NativeMethods.GetCurrentProcessId();
        }

        /// <summary>
        /// 获取所有进程ID，失败返回null
        /// </summary>
        /// <returns></returns>
        public static uint[] GetAllProcessIds()
        {
            uint[] processIds;
            uint bytesReturned;

            processIds = null;
            do
            {
                if (processIds == null)
                    processIds = new uint[0x200];
                else
                    processIds = new uint[processIds.Length * 2];
                if (!EnumProcesses(ref processIds[0], (uint)(processIds.Length * 4), out bytesReturned))
                    return null;
            } while (bytesReturned == processIds.Length * 4);
            return processIds.Take((int)bytesReturned / 4).ToArray();
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static string GetProcessName(uint processId)
        {
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessQuery(processId))
                if (processHandle.IsValid)
                    return GetProcessNameInternal(processHandle);
                else
                    return null;
        }

        /// <summary>
        /// 获取进程名
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessNameInternal(IntPtr processHandle)
        {
            StringBuilder filePath;

            filePath = new StringBuilder((int)MAX_MODULE_NAME32);
            if (GetProcessImageFileName(processHandle, filePath, (int)MAX_MODULE_NAME32) == 0)
                return null;
            return Path.GetFileName(filePath.ToString());
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static string GetProcessPath(uint processId)
        {
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessQuery(processId))
                if (processHandle.IsValid)
                    return GetProcessPathInternal(processHandle);
                else
                    return null;
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <returns></returns>
        internal static string GetProcessPathInternal(IntPtr processHandle)
        {
            StringBuilder filePath;

            filePath = new StringBuilder((int)MAX_PATH);
            if (GetProcessImageFileName(processHandle, filePath, MAX_PATH) == 0)
                return null;
            return filePath.ToString();
        }

        /// <summary>
        /// 判断进程是否为64位进程，返回值为方法是否执行成功
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="is64">是否为64位进程</param>
        /// <returns></returns>
        public static bool Is64BitProcess(uint processId, out bool is64)
        {
            SafeNativeHandle processHandle;

            if (!FastWin32Settings.Is64BitOperatingSystem)
            {
                //不是64位系统肯定不会是64位进程
                is64 = false;
                return true;
            }
            using (processHandle = OpenProcessQuery(processId))
                if (processHandle.IsValid)
                    return Is64BitProcessInternal(processHandle, out is64);
                else
                {
                    is64 = false;
                    return false;
                }
        }

        /// <summary>
        /// 判断进程是否为64位进程，返回值为方法是否执行成功
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="is64">是否为64位进程</param>
        /// <returns></returns>
        internal static bool Is64BitProcessInternal(IntPtr processHandle, out bool is64)
        {
            bool isWow64;

            if (!FastWin32Settings.Is64BitOperatingSystem)
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
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessProcessSuspendResume(processId))
                if (processHandle.IsValid)
                    return SuspendProcessInternal(processHandle);
                else
                    return false;
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
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessProcessSuspendResume(processId))
                if (processHandle.IsValid)
                    return ResumeProcessInternal(processHandle);
                else
                    return false;
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

        /// <summary>
        /// 动态提升进程权限，以管理员模式运行当前进程，如果执行成功当前进程将退出，执行失败无反应
        /// </summary>
        /// <param name="windowHandle">主窗口的句柄</param>
        /// <returns></returns>
        public static void SelfElevate(IntPtr windowHandle)
        {
            StringBuilder filePath;
            SHELLEXECUTEINFO shellExecuteInfo;

            filePath = new StringBuilder((int)MAX_PATH);
            if (GetModuleFileName(IntPtr.Zero, filePath, MAX_PATH) == 0)
                return;
            shellExecuteInfo = new SHELLEXECUTEINFO
            {
                cbSize = SHELLEXECUTEINFO.UnmanagedSize,
                hwnd = windowHandle,
                lpVerb = "runas",
                lpFile = filePath.ToString(),
                nShow = 1
            };
            if (ShellExecuteEx(ref shellExecuteInfo))
                Environment.Exit(0);
        }
    }
}
