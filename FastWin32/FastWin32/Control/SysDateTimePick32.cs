using System;
using static FastWin32.Macro.CommCtrl;

namespace FastWin32.Control
{
    /// <summary>
    /// 日期控件
    /// </summary>
    public class SysDateTimePick32 : Win32Control
    {
        /// <summary>
        /// 创建一个新的日期控件（未完成）
        /// </summary>
        public SysDateTimePick32() : base() { }

        /// <summary>
        /// 使用已有SysDateTimePick32控件实例化SysDateTimePick32类
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        public SysDateTimePick32(IntPtr hWnd) : base(hWnd) { }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="systemTime">日期</param>
        /// <returns></returns>
        public bool GetSystemTime(out SYSTEMTIME systemTime)
        {
            return Util.ReadStructRemote(_handle, out systemTime, (IntPtr addr) => DateTime_GetSystemtime(_handle, addr));
        }

        /// <summary>
        /// 设置日期
        /// </summary>
        /// <param name="systemTime">日期</param>
        /// <returns></returns>
        public bool SetSystemTime(SYSTEMTIME systemTime)
        {
            return Util.WriteStructRemote(_handle, systemTime, (IntPtr addr) => DateTime_SetSystemtime(_handle, NativeMethods.GDT_VALID, addr));
        }

        /// <summary>
        /// 将DTP控件设置为“无日期”并清除其复选框。当指定此标志时， lParam将被忽略。此标志仅适用于设置为DTS_SHOWNONE样式的DTP控件。
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            return DateTime_SetSystemtime(_handle, NativeMethods.GDT_NONE, IntPtr.Zero);
        }
    }
}
