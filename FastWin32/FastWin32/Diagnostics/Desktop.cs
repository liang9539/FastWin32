using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Diagnostics
{
    /// <summary>
    /// 桌面
    /// </summary>
    public static class Desktop
    {
        /// <summary>
        /// 显示桌面
        /// </summary>
        /// <returns></returns>
        public static bool ToggleDesktop()
        {
            try
            {
                Type shellType = Type.GetTypeFromProgID("Shell.Application");
                object shellObject = Activator.CreateInstance(shellType);
                shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取Program Manager的窗口句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetProgramManager()
        {
            return GetShellWindow();
        }

        /// <summary>
        /// 重置第二个WorkerW计数
        /// </summary>
        public static void ResetWorkerW()
        {
            IntPtr hWorkerW;
            IntPtr hProgramManager;

            hWorkerW = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                if (FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                    hWorkerW = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);
            //先获取WorkerW的窗口句柄
            hProgramManager = GetShellWindow();
            //获取Program Manager的窗口句柄
            if (hWorkerW == IntPtr.Zero)
            {
                //不存在WorkerW
                do
                {
                    EnumWindows((hWnd, lParam) =>
                    {
                        if (FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                            hWorkerW = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                        return true;
                    }, IntPtr.Zero);
                    //获取WorkerW的窗口句柄
                    SendMessage(hProgramManager, WM_USER + 300, (IntPtr)2, IntPtr.Zero);
                    //增加WorkerW内部计数
                } while (hWorkerW == IntPtr.Zero);
                SendMessage(hProgramManager, WM_USER + 300, (IntPtr)1, IntPtr.Zero);
                //此时创建出了WorkerW，所以再减少内部计数
            }
            else
            {
                //存在WorkerW
                do
                {
                    SendMessage(hProgramManager, WM_USER + 300, (IntPtr)1, IntPtr.Zero);
                    //减少WorkerW内部计数
                } while (IsWindow(hWorkerW));
            }
        }
    }
}
