using System;
using System.ComponentModel;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 进程
    /// </summary>
    public static class ProcessX
    {
        /// <summary>
        /// 通过窗口句柄获取进程ID
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static uint GetProcessIdByHWnd(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint processId);
            return processId;
        }

        /// <summary>
        /// 判断进程是否为64位进程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <returns></returns>
        internal static bool Is64ProcessInternal(IntPtr hProcess)
        {
            if (!Environment.Is64BitOperatingSystem)
                //不是64位系统肯定不会是64位进程
                return false;
            if (!IsWow64Process(hProcess, out bool isWow64))
                //执行失败
                throw new Win32Exception();
            return !isWow64;
            //是Wow64返回false，反之返回true
        }
    }
}
