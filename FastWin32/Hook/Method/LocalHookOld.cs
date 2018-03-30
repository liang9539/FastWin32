//using System;
//using System.ComponentModel;
//using System.Reflection;
//using static FastWin32.NativeMethods;

//namespace FastWin32.Hook.Method
//{
//    /// <summary>
//    /// 改变托管/非托管函数的执行过程与结果，若要Hook其他进程，配合Injector类注入Dll使用
//    /// </summary>
//    public class LocalHookOld : IHook,IDisposable
//    {
//        /// <summary>
//        /// 原函数入口地址
//        /// </summary>
//        private IntPtr _origEntry;
//        /// <summary>
//        /// 新函数入口地址
//        /// </summary>
//        private IntPtr _newEntry;
//        /// <summary>
//        /// 是否已经安装
//        /// </summary>
//        private bool _isInstalled;
//        private byte[] _origBytes;
//        private byte[] _newBytes;
//        private bool _isFirst;

//        /// <summary>
//        /// 实例化API钩子（用非托管函数替换非托管函数）
//        /// </summary>
//        /// <param name="origModuleName">原非托管函数所在模块</param>
//        /// <param name="origApiName">原非托管函数名（如果该参数是序数值，则它必须在低位字中; 高阶字必须为零）</param>
//        /// <param name="newModuleName">新非托管函数所在模块</param>
//        /// <param name="newApiName">新非托管函数名（如果该参数是序数值，则它必须在低位字中; 高阶字必须为零）</param>
//        public LocalHookOld(string origModuleName, string origApiName, string newModuleName, string newApiName) : this(GetProcAddressInternal(origModuleName, origApiName), GetProcAddressInternal(newModuleName, newApiName)) { }

//        /// <summary>
//        /// 实例化API钩子（用托管方法替换非托管函数）
//        /// </summary>
//        /// <param name="origModuleName">原非托管函数所在模块</param>
//        /// <param name="origApiName">原非托管函数名（如果该参数是序数值，则它必须在低位字中; 高阶字必须为零）</param>
//        /// <param name="newMethodInfo">新托管方法元数据</param>
//        public LocalHookOld(string origModuleName, string origApiName, MethodInfo newMethodInfo) : this(GetProcAddressInternal(origModuleName, origApiName), newMethodInfo.MethodHandle.GetFunctionPointer()) { }

//        /// <summary>
//        /// 实例化API钩子（用非托管函数替换托管方法）
//        /// </summary>
//        /// <param name="origMethodInfo">原托管方法元数据</param>
//        /// <param name="newModuleName">新非托管函数所在模块</param>
//        /// <param name="newApiName">新非托管函数名（如果该参数是序数值，则它必须在低位字中; 高阶字必须为零）</param>
//        public LocalHookOld(MethodInfo origMethodInfo, string newModuleName, string newApiName) : this(origMethodInfo.MethodHandle.GetFunctionPointer(), GetProcAddressInternal(newModuleName, newApiName)) { }

//        /// <summary>
//        /// 实例化API钩子（用托管方法数替换托管方法）
//        /// </summary>
//        /// <param name="origMethodInfo">原托管方法元数据</param>
//        /// <param name="newMethodInfo">新托管方法元数据</param>
//        public LocalHookOld(MethodInfo origMethodInfo, MethodInfo newMethodInfo) : this(origMethodInfo.MethodHandle.GetFunctionPointer(), newMethodInfo.MethodHandle.GetFunctionPointer()) { }

//        /// <summary>
//        /// 实例化API钩子
//        /// </summary>
//        /// <param name="origEntry">原入口</param>
//        /// <param name="newEntry">新入口</param>
//        public LocalHookOld(IntPtr origEntry, IntPtr newEntry)
//        {
//            if (origEntry == newEntry)
//                throw new ArgumentException("新入口与原入口一致");

//            _origEntry = origEntry;
//            _newEntry = newEntry;
//        }

//        /// <summary>
//        /// 获取函数地址
//        /// </summary>
//        /// <param name="moduleName">模块名</param>
//        /// <param name="procName">函数名</param>
//        /// <returns></returns>
//        internal static IntPtr GetProcAddressInternal(string moduleName, string procName)
//        {
//            if (moduleName == null || procName == null)
//                throw new ArgumentNullException();
//            if (moduleName.Length == 0 || procName.Length == 0)
//                throw new ArgumentOutOfRangeException();

//            IntPtr moduleHandle;
//            IntPtr pFunction;

//            moduleHandle = GetModuleHandle(moduleName);
//            if (moduleHandle == IntPtr.Zero)
//                throw new Win32Exception();
//            pFunction = GetProcAddress(moduleHandle, procName);
//            if (pFunction == IntPtr.Zero)
//                throw new Win32Exception();
//            return pFunction;
//        }

//        /// <summary>
//        /// 安装钩子
//        /// </summary>
//        public bool Install()
//        {
//            _isInstalled = true;
//        }

//        /// <summary>
//        /// 生成跳转需要的字节数组
//        /// </summary>
//        /// <returns></returns>
//        private byte[] GenBytes()
//        {
//            if (Environment.Is64BitProcess)
//            {
//                byte[] bytAddr;

//                bytAddr = BitConverter.GetBytes((long)_origEntry);
//                //获取地址的字节数组形式
//                return new byte[]
//                {
//                    0x48, 0xB8, bytAddr[0], bytAddr[1], bytAddr[2], bytAddr[3], bytAddr[4], bytAddr[5], bytAddr[6], bytAddr[7],
//                    //mov rax, addr
//                    0x50,
//                    //push rax
//                    0xC3
//                    //ret
//                };
//                //64位麻烦一些，因为push imm64不被支持，也就是不能直接push 1234567812345678h
//            }
//            else
//            {
//                byte[] bytAddr;

//                bytAddr = BitConverter.GetBytes((int)_newEntry);
//                //获取地址的字节数组形式
//                return new byte[]
//                {
//                    0x68, bytAddr[0], bytAddr[1], bytAddr[2], bytAddr[3],
//                     //push addr
//                    0xC3
//                     //ret
//                };
//            }
//        }

//        /// <summary>
//        /// 卸载钩子
//        /// </summary>
//        public bool Uninstall()
//        {
//            if (!_isInstalled)
//                throw new NotSupportedException("")
//        }

//        #region IDisposable Support
//        private bool disposedValue = false; // 要检测冗余调用

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    // TODO: 释放托管状态(托管对象)。
//                }

//                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
//                // TODO: 将大型字段设置为 null。

//                disposedValue = true;
//            }
//        }

//        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
//        // ~LocalHook() {
//        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//        //   Dispose(false);
//        // }

//        // 添加此代码以正确实现可处置模式。
//        public void Dispose()
//        {
//            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//            Dispose(true);
//            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
//            // GC.SuppressFinalize(this);
//        }
//        #endregion
//    }
//}
