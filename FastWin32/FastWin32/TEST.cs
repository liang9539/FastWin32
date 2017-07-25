#if DEBUG
//仅供测试
using System;
using static FastWin32.NativeMethods;

namespace FastWin32
{
    /// <summary>
    /// 测试
    /// </summary>
    public static class TEST
    {
        /// <summary>
        /// 测试
        /// </summary>
        public static void Test()
        {
            //SendMessage((IntPtr)0x100CC, WM_USER + 458, 3, 0);
            //SendMessage((IntPtr)0x100CC, WM_USER + 458, 4, 1);
            SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            SendMessage(GetShellWindow(), WM_USER + 300, 1, 0);
        }
    }
}
#endif
