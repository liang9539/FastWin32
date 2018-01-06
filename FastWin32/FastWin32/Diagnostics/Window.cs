using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 窗口
    /// </summary>
    public static class Window
    {
        /// <summary>
        /// 枚举窗口回调函数，继续枚举返回true，否则返回false
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        public delegate bool EnumWindowsCallback(IntPtr hWnd);

        /// <summary>
        /// 获取包含桌面ListView的句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetDesktopView()
        {
            IntPtr workerW;
            IntPtr programManager;
            IntPtr shell;

            workerW = IntPtr.Zero;
            shell = IntPtr.Zero;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                //Vista及以上        
                NativeMethods.EnumWindows((hWnd, lParam) =>
                {
                    shell = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (shell != IntPtr.Zero)
                    {
                        //如果当前窗口存在类名为SHELLDLL_DefView的子窗口
                        workerW = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                        return false;
                    }
                    return true;
                }, IntPtr.Zero);
            }
            else
            {
                //XP及以下
                programManager = GetShellWindow();
                //XP的SHELLDLL_DefView在Program Manager里
                shell = FindWindowEx(programManager, IntPtr.Zero, "SHELLDLL_DefView", null);
            }
            //先获取WorkerW
            return FindWindowEx(shell, IntPtr.Zero, "SysListView32", "FolderView");
        }

        /// <summary>
        /// 将窗口置顶并激活（单次，非永久），非直接调用Win32API SetForegroundWindow，成功率高
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        public static void SetForegroundWindow(IntPtr hWnd)
        {
            if (!IsWindow(hWnd))
                throw new ArgumentException("无效窗口句柄");

            IntPtr hForeWnd;
            uint processId;
            uint idAttach;
            uint idAttachTo;

            hForeWnd = GetForegroundWindow();
            //获取顶端窗口
            idAttach = GetCurrentThreadId();
            //获取当前线程ID
            idAttachTo = GetWindowThreadProcessId(hForeWnd, out processId);
            //获取要附加到的线程的ID
            AttachThreadInput(idAttach, idAttachTo, true);
            //附加到线程
            NativeMethods.SetForegroundWindow(hWnd);
            SetActiveWindow(hWnd);
            SetFocus(hWnd);
            AttachThreadInput(idAttach, idAttachTo, false);
            //分离
        }

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="className">窗口类名</param>
        /// <param name="windowName">窗口标题</param>
        /// <returns></returns>
        public static IntPtr FindWindow(string className, string windowName)
        {
            return NativeMethods.FindWindow(className, windowName);
        }

        /// <summary>
        /// 查找窗口
        /// </summary>
        /// <param name="hParentWnd">父窗口句柄</param>
        /// <param name="hAfterWnd">从此窗口之后开始查找（此窗口必须为父窗口的直接子窗口）</param>
        /// <param name="className">窗口类名</param>
        /// <param name="windowName">窗口标题</param>
        /// <returns></returns>
        public static IntPtr FindWindow(IntPtr hParentWnd, IntPtr hAfterWnd, string className, string windowName)
        {
            return FindWindowEx(hParentWnd, hAfterWnd, className, windowName);
        }

        /// <summary>
        /// 枚举所有顶级窗口
        /// </summary>
        /// <param name="callback">查找到窗口时的回调函数</param>
        /// <returns></returns>
        public static bool EnumWindows(EnumWindowsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException();

            return NativeMethods.EnumWindows((hWnd, lParam) => callback(hWnd), IntPtr.Zero);
        }

        /// <summary>
        /// 枚举所有子窗口
        /// </summary>
        /// <param name="hWndParent">父窗口</param>
        /// <param name="callback">查找到窗口时的回调函数</param>
        /// <returns></returns>
        public static bool EnumChildWindows(IntPtr hWndParent, EnumWindowsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException();

            return NativeMethods.EnumChildWindows(hWndParent, (hWnd, lParam) => callback(hWnd), IntPtr.Zero);
        }
    }
}
