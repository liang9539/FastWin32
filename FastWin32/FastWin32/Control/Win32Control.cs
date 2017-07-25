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
        protected IntPtr _handle;

        /// <summary>
        /// 启用Unicode编码（针对部分方法有效）
        /// </summary>
        protected static bool _unicode = true;

        /// <summary>
        /// 控件句柄
        /// </summary>
        public IntPtr Handle => _handle;

        /// <summary>
        /// 启用Unicode编码（默认为是）（针对部分方法有效）
        /// </summary>
        public bool Unicode { get => _unicode; set => _unicode = value; }

        /// <summary>
        /// 创建新的Win32控件
        /// </summary>
        protected Win32Control()
        {
            throw new Exception("未实现");
        }

        /// <summary>
        /// 操作已有Win32控件
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        protected Win32Control(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero || hWnd == INVALID_HANDLE_VALUE || !IsWindow(hWnd))
            {
                throw new ArgumentException();
            }

            _handle = hWnd;
        }
    }
}
