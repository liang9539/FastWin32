using System;
using System.Reflection;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Hook.Method
{
    /// <summary>
    /// 挂钩当前进程API
    /// </summary>
    public class LocalHook
    {
        /// <summary>
        /// 杀死函数，让函数不执行任何动作
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="apiName">函数名</param>
        /// <returns></returns>
        public static bool Kill(string moduleName, string apiName)
        {
            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(apiName))
                throw new ArgumentNullException();

            return Kill(Diagnostics.Module32.GetProcAddressInternal(moduleName, apiName));
        }

        /// <summary>
        /// 杀死方法，让方法不执行任何动作
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns></returns>
        public static bool Kill(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException();

            return Kill(methodInfo.MethodHandle.GetFunctionPointer());
        }

        /// <summary>
        /// 杀死函数，让函数不执行任何动作
        /// </summary>
        /// <param name="entry">函数入口地址</param>
        /// <returns></returns>
        public static bool Kill(IntPtr entry)
        {
            return MemoryIO.WriteByteInternal(CURRENT_PROCESS, entry, 0xC3);
        }
    }
}
