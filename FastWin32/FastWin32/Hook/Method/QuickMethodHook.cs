using System;
using System.Reflection;
using static FastWin32.NativeMethods;

namespace FastWin32.Hook.Method
{
    /// <summary>
    /// 提供对 <see cref="LocalHook"/> 与 <see cref="RemoteHook"/> 类的一些封装，实现快速Hook方法
    /// </summary>
    public static class QuickMethodHook
    {
        /// <summary>
        /// 杀死函数，让函数不执行任何动作
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="apiName">函数名</param>
        /// <returns></returns>
        public static bool Kill(string moduleName, string apiName)
        {
            if (moduleName == null || apiName == null)
                throw new ArgumentNullException();
            if (moduleName.Length == 0 || apiName.Length == 0)
                throw new ArgumentOutOfRangeException();

            return Kill(Diagnostics.Module.GetFuncAddressInternal(moduleName, apiName));
        }

        /// <summary>
        /// 杀死方法，让方法不执行任何动作
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns></returns>
        public static bool Kill(MethodInfo methodInfo)
        {
            return Kill(methodInfo.MethodHandle.GetFunctionPointer());
        }

        /// <summary>
        /// 杀死函数，让函数不执行任何动作
        /// </summary>
        /// <param name="entry">函数入口地址</param>
        /// <returns></returns>
        public static unsafe bool Kill(IntPtr entry)
        {
            byte ret = 0xC3;

            return WriteProcessMemory(CURRENT_PROCESS, entry, ref ret, 1, null);
        }
    }
}
