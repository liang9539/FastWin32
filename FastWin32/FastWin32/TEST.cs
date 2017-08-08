#if DEBUG
//仅供测试
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using FastWin32.Asm;
using FastWin32.Diagnostics;
using FastWin32.Hook;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32
{
    /// <summary>
    /// 测试
    /// </summary>
    public static class TEST
    {
        static uint currentPid = (uint)Process.GetCurrentProcess().Id;

        /// <summary>
        /// 测试
        /// </summary>
        public static unsafe void Test()
        {
            //SendMessage((IntPtr)0x100CC, WM_USER + 458, 3, 0);
            //SendMessage((IntPtr)0x100CC, WM_USER + 458, 4, 1);

            //SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            //SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            //SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            //SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            //SendMessage(GetShellWindow(), WM_USER + 300, 2, 0);
            //SendMessage(GetShellWindow(), WM_USER + 300, 1, 0);
        }
    }
}
#endif
