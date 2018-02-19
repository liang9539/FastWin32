using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Windowing
{
    /// <summary>
    /// 窗口
    /// </summary>
    public static class Window
    {
        /// <summary>
        /// 枚举窗口回调函数，继续枚举返回true，否则返回false
        /// </summary>
        /// <param name="windowHandle">窗口句柄</param>
        /// <returns></returns>
        public delegate bool EnumWindowsCallback(IntPtr windowHandle);

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
                NativeMethods.EnumWindows((windowHandle, lParam) =>
                {
                    shell = FindWindowEx(windowHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (shell != IntPtr.Zero)
                    {
                        //如果当前窗口存在类名为SHELLDLL_DefView的子窗口
                        workerW = FindWindowEx(IntPtr.Zero, windowHandle, "WorkerW", null);
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
        /// <param name="windowHandle">窗口句柄</param>
        public static unsafe void SetForegroundWindow(IntPtr windowHandle)
        {
            if (!IsWindow(windowHandle))
                throw new ArgumentException("无效窗口句柄");

            uint currentThreadId;
            uint foregroundThreadId;

            currentThreadId = GetCurrentThreadId();
            //获取当前线程ID
            foregroundThreadId = GetWindowThreadProcessId(GetForegroundWindow(), null);
            //获取要附加到的线程的ID
            AttachThreadInput(currentThreadId, foregroundThreadId, true);
            //附加到线程
            NativeMethods.SetForegroundWindow(windowHandle);
            SetActiveWindow(windowHandle);
            SetFocus(windowHandle);
            AttachThreadInput(currentThreadId, foregroundThreadId, false);
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
        /// <param name="parentWindowHandle">父窗口句柄</param>
        /// <param name="afterWindowHandle">从此窗口之后开始查找（此窗口必须为父窗口的直接子窗口）</param>
        /// <param name="className">窗口类名</param>
        /// <param name="windowName">窗口标题</param>
        /// <returns></returns>
        public static IntPtr FindWindow(IntPtr parentWindowHandle, IntPtr afterWindowHandle, string className, string windowName)
        {
            return FindWindowEx(parentWindowHandle, afterWindowHandle, className, windowName);
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

            return NativeMethods.EnumWindows((windowHandle, lParam) => callback(windowHandle), IntPtr.Zero);
        }

        /// <summary>
        /// 枚举所有子窗口
        /// </summary>
        /// <param name="windowHandleParent">父窗口</param>
        /// <param name="callback">查找到窗口时的回调函数</param>
        /// <returns></returns>
        public static bool EnumChildWindows(IntPtr windowHandleParent, EnumWindowsCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException();

            return NativeMethods.EnumChildWindows(windowHandleParent, (windowHandle, lParam) => callback(windowHandle), IntPtr.Zero);
        }
    }
}
