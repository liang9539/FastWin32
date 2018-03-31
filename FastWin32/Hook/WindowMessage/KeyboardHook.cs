//using System;
//using System.Reflection;
//using System.Threading;
//using System.Windows.Forms;
//using FastWin32.Diagnostics;
//using static FastWin32.NativeMethods;

//namespace FastWin32.Hook.WindowMessage
//{
//    /// <summary>
//    /// 参数，触发条件与 <see cref="KeyEventHandler"/> 相同，但此事件有返回值。返回 <see langword="false"/> 表示将此次消息继续发送给下一个钩子，返回 <see langword="true"/> 表示屏蔽此次消息，目标窗口将无法收到此次消息
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    /// <returns></returns>
//    public delegate bool KeyHookEventHandler(KeyboardHook sender, KeyEventArgs e);

//    /// <summary>
//    /// 参数，触发条件与 <see cref="KeyPressEventHandler"/> 相同，但此事件有返回值。返回 <see langword="false"/> 表示将此次消息继续发送给下一个钩子，返回 <see langword="true"/> 表示屏蔽此次消息，目标窗口将无法收到此次消息
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    /// <returns></returns>
//    public delegate bool KeyPressHookEventHandler(KeyboardHook sender, KeyPressEventArgs e);

//    /// <summary>
//    /// 键盘消息钩子
//    /// </summary>
//    public sealed class KeyboardHook
//    {
//        private byte[] _keyboardState = new byte[256];

//        private uint _targetThreadId;

//        private IntPtr _hookHandle;

//        private Thread _hookThread;

//        /// <summary>
//        /// 是否安装
//        /// </summary>
//        public bool IsInstalled { get; private set; }

//        /// <summary>
//        /// 按键按下事件
//        /// </summary>
//        public event KeyHookEventHandler KeyDown;

//        /// <summary>
//        /// 按键弹起事件
//        /// </summary>
//        public event KeyHookEventHandler KeyUp;

//        /// <summary>
//        /// 按键按压事件
//        /// </summary>
//        public event KeyPressHookEventHandler KeyPress;

//        /// <summary>
//        /// 创建全局键盘钩子实例
//        /// </summary>
//        public KeyboardHook()
//        {
//        }

//        /// <summary>
//        /// 对指定线程创建键盘钩子
//        /// </summary>
//        /// <param name="targetThreadId">线程ID</param>
//        public KeyboardHook(uint targetThreadId)
//        {
//            _targetThreadId = targetThreadId;
//        }

//        /// <summary>
//        /// 对指定窗口创建键盘钩子
//        /// </summary>
//        /// <param name="windowHandle">窗口句柄</param>
//        public KeyboardHook(IntPtr windowHandle)
//        {
//            if (!IsWindow(windowHandle))
//                throw new ArgumentNullException("无效窗口句柄");

//            _targetThreadId = GetWindowThreadProcessId(windowHandle, null);
//        }

//        /// <summary>
//        /// 安装钩子
//        /// </summary>
//        public bool Install()
//        {
//            if (IsInstalled)
//                throw new NotSupportedException("无法重复安装钩子");

//            bool finished;
//            bool result;

//            finished = false;
//            result = false;
//            if (Application.MessageLoop)
//            {
//                //调用此方法的线程如果有消息循环，就不需要开新线程启动消息循环
//                result = InstallPrivate();
//                finished = true;
//            }
//            else
//            {
//                _hookThread = new Thread(() =>
//                {
//                    result = InstallPrivate();
//                    finished = true;
//                    Application.Run();
//                })
//                {
//                    IsBackground = true
//                };
//                _hookThread.Start();
//            }
//            while (!finished)
//                Thread.Sleep(0);
//            if (result)
//            {
//                IsInstalled = true;
//                return true;
//            }
//            else
//            {
//                _hookThread?.Abort();
//                return false;
//            }
//        }

//        /// <summary>
//        /// 安装钩子
//        /// </summary>
//        /// <returns></returns>
//        private bool InstallPrivate()
//        {
//            uint processId;
//            string guid;
//            int returnValue;

//            if (_targetThreadId == 0)
//            {
//                _hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardHookProc, IntPtr.Zero, 0);
//                return _hookHandle != IntPtr.Zero;
//            }
//            else
//            {
//                guid = Guid.NewGuid().ToString();
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                //TODO
//                return (processId = Process32.GetProcessIdByThreadId(_targetThreadId)) != 0 && Injector.InjectManaged(processId, Assembly.GetExecutingAssembly().Location, "FastWin32.Hook.WindowMessage.MessageProxy", "StartServer", guid, out returnValue) && returnValue == 1;
//            }
//        }

//        /// <summary>
//        /// 键盘消息回调函数
//        /// </summary>
//        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果代码小于0，挂钩过程必须将消息传递给CallNextHookEx函数，无需进一步处理，并应返回CallNextHookEx返回的值。</param>
//        /// <param name="wParam">产生击键消息的密钥的虚拟密钥代码。</param>
//        /// <param name="lParam">重复计数，扫描码，扩展密钥标志，上下文代码，先前的密钥状态标志和转换状态标志。有关lParam参数的更多信息，请参阅按键消息标志。下表描述了该值的位。</param>
//        /// <returns></returns>
//        private IntPtr LowLevelKeyboardHookProc(int nCode, size_t wParam, size_t lParam)
//        {
//            if (nCode < 0 || (KeyDown == null && KeyUp == null && KeyPress == null))
//                //如果nCode小于零，则钩子过程必须返回CallNextHookEx返回的值并且不对钩子消息做处理。如果3个事件均未被订阅，直接返回
//                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
//            else
//            {
//                if (OnKeyEvent((uint)wParam, ((KBDLLHOOKSTRUCT*)lParam)->vkCode, ((KBDLLHOOKSTRUCT*)lParam)->scanCode))
//                    return (IntPtr)(-1);
//                else
//                    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
//            }
//        }

//        /// <summary>
//        /// 引发事件
//        /// </summary>
//        /// <param name="messageType">消息类型</param>
//        /// <param name="vkCode">虚拟键码</param>
//        /// <param name="scanCode">扫描码</param>
//        /// <returns></returns>
//        private bool OnKeyEvent(uint messageType, uint vkCode, uint scanCode)
//        {
//            bool isBlock;
//            char keyChar;

//            isBlock = false;
//            if (KeyDown != null && (messageType == WM_KEYDOWN || messageType == WM_SYSKEYDOWN))
//                isBlock = KeyDown(this, new KeyEventArgs((Keys)vkCode));
//            if (KeyUp != null && (messageType == WM_KEYUP || messageType == WM_SYSKEYUP))
//                isBlock = KeyUp(this, new KeyEventArgs((Keys)vkCode));
//            if (KeyPress != null && messageType == WM_KEYDOWN)
//            {
//                GetKeyState(0);
//                GetKeyboardState(_keyboardState);
//                if (ToAscii(vkCode, scanCode, _keyboardState, out keyChar, 0) == 1)
//                    isBlock = KeyPress(this, new KeyPressEventArgs(keyChar));
//            }
//            return isBlock;
//        }

//        /// <summary>
//        /// 卸载钩子
//        /// </summary>
//        public bool Uninstall()
//        {
//            if (!IsInstalled)
//                throw new NotSupportedException("未安装钩子");

//            if (_targetThreadId == 0)
//            {
//                //全局钩子
//                if (!UnhookWindowsHookEx(_hookHandle))
//                    //钩子卸载失败
//                    return false;
//            }
//            else
//                //线程钩子
//                _hookThread.Abort();
//            IsInstalled = false;
//            return true;
//        }
//    }
//}
