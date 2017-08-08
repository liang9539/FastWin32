using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static FastWin32.NativeMethods;

namespace FastWin32.Hook
{
    /// <summary>
    /// 提供对ApiHook类的一些封装，实现快速HookAPI
    /// </summary>
    public static class QuickApiHook
    {
        /// <summary>
        /// 杀死函数，让函数不执行任何动作,如果函数有返回值，此时的返回值是无效的，不可信的
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="apiName">函数名</param>
        /// <returns></returns>
        public static bool Kill(string moduleName, string apiName)
        {
            return Kill(ApiHook.GetProcAddressInternal(moduleName, apiName));
        }

        /// <summary>
        /// 杀死方法，让方法不执行任何动作,如果方法有返回值，此时的返回值是无效的，不可信的
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns></returns>
        public static bool Kill(MethodInfo methodInfo)
        {
            return Kill(methodInfo.MethodHandle.GetFunctionPointer());
        }

        /// <summary>
        /// 杀死函数，让函数不执行任何动作,如果函数有返回值，此时的返回值是无效的，不可信的
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
