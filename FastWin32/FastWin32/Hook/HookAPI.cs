using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastWin32.Hook
{
    /// <summary>
    /// 改变托管/非托管函数的执行过程与结果
    /// </summary>
    public class HookAPI : IHook
    {
        /// <summary>
        /// 原函数入口地址
        /// </summary>
        private IntPtr _origEntry;
        /// <summary>
        /// 新函数入口地址
        /// </summary>
        private IntPtr _newEntry;
        /// <summary>
        /// 是否已经安装
        /// </summary>
        private bool _isInstalled;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dllName1"></param>
        /// <param name="apiName1"></param>
        /// <param name="dllName2"></param>
        /// <param name="apiName2"></param>
        public HookAPI(string dllName1, string apiName1, string dllName2, string apiName2)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="apiName"></param>
        /// <param name="methodInfo"></param>
        public HookAPI(string dllName, string apiName, MethodInfo methodInfo)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="dllName"></param>
        /// <param name="apiName"></param>
        public HookAPI(MethodInfo methodInfo,string dllName,string apiName)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origEntry"></param>
        /// <param name="newEntry"></param>
        public HookAPI(IntPtr origEntry, IntPtr newEntry)
        {

        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        public void Install()
        {
            _isInstalled = true;
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void Uninstall()
        {
            if (!_isInstalled)
                return;
        }
    }
}
