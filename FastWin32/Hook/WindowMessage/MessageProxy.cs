//using System;
//using System.Threading;
//using System.Windows.Forms;
//using static FastWin32.NativeMethods;

//namespace FastWin32.Hook.WindowMessage
//{
//    /// <summary>
//    /// 转发消息
//    /// </summary>
//    internal sealed class MessageProxy
//    {
//        private IntPtr _hookHandle;

//        private Thread _hookThread;

//        /// <summary>
//        /// 启动代理
//        /// </summary>
//        /// <returns></returns>
//        private bool Install()
//        {
//            bool finished;
//            bool result;

//            finished = false;
//            result = false;
//            _hookThread = new Thread(() =>
//            {
//                //_hookHandle = SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, IntPtr.Zero,TargetThreadId);
//                throw new NotImplementedException();
//                result = _hookHandle != IntPtr.Zero;
//                finished = true;
//                Application.Run();
//            })
//            {
//                IsBackground = true
//            };
//            _hookThread.Start();
//            while (!finished)
//                Thread.Sleep(0);
//            if (result)
//                return true;
//            else
//            {
//                _hookThread.Abort();
//                return false;
//            }
//        }

//        private unsafe IntPtr KeyboardProc(int nCode, size_t wParam, size_t lParam)
//        {
//            if (nCode < 0)
//                //如果nCode小于零，则钩子过程必须返回CallNextHookEx返回的值并且不对钩子消息做处理。如果3个事件均未被订阅，直接返回
//                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
//            else
//            {
//                //if (keyboardHook.OnHookProc((uint)wParam, (uint)lParam, (uint)lParam))
//                //    return (IntPtr)(-1);
//                //else
//                //    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
//                throw new NotImplementedException();
//            }
//        }

//        ///// <summary>
//        ///// 停止代理
//        ///// </summary>
//        ///// <returns></returns>
//        //public bool Uninstall()
//        //{
//        //    if (_hookThread.ThreadState == ThreadState.Running)
//        //        _hookThread.Abort();
//        //    return UnhookWindowsHookEx(_hookHandle);
//        //}

//        /// <summary>
//        /// 启动代理，注入DLL使用
//        /// </summary>
//        /// <param name="arg">参数</param>
//        /// <returns></returns>
//        public static int StartServer(string arg)
//        {
//            return new MessageProxy {  }.Install() ? 1 : 0;
//        }
//    }
//}
