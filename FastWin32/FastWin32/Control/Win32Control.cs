using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Control
{
    /// <summary>
    /// 表示一个Win32控件
    /// </summary>
    public abstract class Win32Control
    {
        /// <summary>
        /// 控件句柄
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// 操作已有Win32控件
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        protected Win32Control(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero || hWnd == INVALID_HANDLE_VALUE || !IsWindow(hWnd))
                throw new ArgumentException("无效的Win32控件窗口句柄");

            Handle = hWnd;
        }
    }
}
