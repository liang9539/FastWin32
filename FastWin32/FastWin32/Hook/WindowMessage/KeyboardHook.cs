using System;
using System.Threading;
using System.Windows.Forms;
using static FastWin32.NativeMethods;

namespace FastWin32.Hook.WindowMessage
{
    /// <summary>
    /// 参数，触发条件与 <see cref="KeyEventArgs"/> 相同，但此事件有返回值。返回 <see langword="true"/> 表示将此次消息继续发送给下一个钩子，返回 <see langword="false"/> 表示屏蔽此次消息，目标窗口将无法收到此次消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate bool HookKeyEventHandler(KeyboardHook sender, KeyEventArgs e);

    /// <summary>
    /// 参数，触发条件与 <see cref="KeyPressEventArgs"/> 相同，但此事件有返回值。返回 <see langword="true"/> 表示将此次消息继续发送给下一个钩子，返回 <see langword="false"/> 表示屏蔽此次消息，目标窗口将无法收到此次消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate bool HookKeyPressEventHandler(KeyboardHook sender, KeyPressEventArgs e);

    /// <summary>
    /// 键盘消息钩子
    /// </summary>
    public class KeyboardHook : IHook
    {
        private bool _isLowLevel;

        private uint _threadId;

        private IntPtr _hookHandle;

        private byte[] _keyboardState = new byte[256];

        private Thread _hookThread;

        /// <summary>
        /// 按键按下事件
        /// </summary>
        public event HookKeyEventHandler KeyDown;

        /// <summary>
        /// 按键弹起事件
        /// </summary>
        public event HookKeyEventHandler KeyUp;

        /// <summary>
        /// 按键按压事件
        /// </summary>
        public event HookKeyPressEventHandler KeyPress;

        /// <summary>
        /// 是否安装
        /// </summary>
        public bool IsInstalled { get; private set; }

        /// <summary>
        /// 是否将输入处理机制附加到顶端窗口的线程，默认为否。如果大小写获取失败或无法正常挂钩键盘消息，可以将此属性设置为 <see langword="true"/>。
        /// </summary>
        public bool IsAttachInput { get; set; }

        /// <summary>
        /// 创建普通全局键盘钩子实例，非LowLevel
        /// </summary>
        public KeyboardHook() : this(false)
        {
        }

        /// <summary>
        /// 创建全局键盘钩子实例
        /// </summary>
        /// <param name="isLowLevel">是否使用低级键盘钩子</param>
        public KeyboardHook(bool isLowLevel)
        {
            _isLowLevel = true;
        }

        /// <summary>
        /// 对指定线程创建键盘钩子实例
        /// </summary>
        /// <param name="threadId">监听的线程ID</param>
        public KeyboardHook(uint threadId)
        {
            _threadId = threadId;
        }

        /// <summary>
        /// 对指定窗口创建键盘钩子实例
        /// </summary>
        /// <param name="windowHandle">监听的窗口句柄</param>
        public unsafe KeyboardHook(IntPtr windowHandle)
        {
            if (!IsWindow(windowHandle))
                throw new ArgumentNullException("无效窗口句柄");

            _threadId = GetWindowThreadProcessId(windowHandle, null);
        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        public bool Install()
        {
            if (IsInstalled)
                throw new NotSupportedException("无法重复安装钩子");

            bool finished;

            finished = false;
            _hookThread = null;
            if (Application.MessageLoop)
            {
                //调用此方法的线程如果有消息循环，就不需要开新线程启动消息循环
                InstallPrivate();
                finished = true;
            }
            else
            {
                _hookThread = new Thread(() =>
                {
                    InstallPrivate();
                    finished = true;
                    Application.Run();
                })
                {
                    IsBackground = true
                };
                _hookThread.Start();
            }
            while (!finished)
                Thread.Sleep(0);
            if (_hookHandle == IntPtr.Zero)
            {
                _hookThread?.Abort();
                return false;
            }
            IsInstalled = true;
            return true;
        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        private void InstallPrivate()
        {
            if (_threadId == 0)
                _hookHandle = SetWindowsHookEx(_isLowLevel ? WH_KEYBOARD_LL : WH_KEYBOARD, KeyboardHookCallback, GetModuleHandle(null), 0);
            else
            {
            }
        }

        /// <summary>
        /// 键盘消息回调函数
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果代码小于0，挂钩过程必须将消息传递给CallNextHookEx函数，无需进一步处理，并应返回CallNextHookEx返回的值。</param>
        /// <param name="wParam">产生击键消息的密钥的虚拟密钥代码。</param>
        /// <param name="lParam">重复计数，扫描码，扩展密钥标志，上下文代码，先前的密钥状态标志和转换状态标志。有关lParam参数的更多信息，请参阅按键消息标志。下表描述了该值的位。</param>
        /// <returns></returns>
        private unsafe IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT keyboardMessage;
            uint messageType;
            bool isCallNext;
            char keyChar;
            uint currentThreadId;
            uint foregroundThreadId;

            if (nCode < 0 || (KeyDown == null && KeyUp == null && KeyPress == null))
                //如果nCode小于零，则钩子过程必须返回CallNextHookEx返回的值并且不对钩子消息做处理。如果3个事件均未被订阅，直接返回
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            else
            {
                keyboardMessage = *(KBDLLHOOKSTRUCT*)lParam;
                messageType = (uint)wParam;
                isCallNext = true;
                if (KeyDown != null && (messageType == WM_KEYDOWN || messageType == WM_SYSKEYDOWN))
                    isCallNext = KeyDown(this, new KeyEventArgs((Keys)keyboardMessage.vkCode));
                if (KeyUp != null && (messageType == WM_KEYUP || messageType == WM_SYSKEYUP))
                    isCallNext = KeyUp(this, new KeyEventArgs((Keys)keyboardMessage.vkCode));
                if (KeyPress != null && messageType == WM_KEYDOWN)
                {
                    if (IsAttachInput)
                    {
                        //将输入处理机制附加到顶端窗口的线程
                        currentThreadId = GetCurrentThreadId();
                        foregroundThreadId = GetWindowThreadProcessId(GetForegroundWindow(), null);
                        AttachThreadInput(currentThreadId, foregroundThreadId, true);
                        GetKeyState(0);
                        GetKeyboardState(_keyboardState);
                        AttachThreadInput(currentThreadId, foregroundThreadId, false);
                    }
                    else
                    {
                        GetKeyState(0);
                        GetKeyboardState(_keyboardState);
                    }
                    if (ToAscii(keyboardMessage.vkCode, keyboardMessage.scanCode, _keyboardState, out keyChar, 0) == 1)
                        isCallNext = KeyPress(this, new KeyPressEventArgs(keyChar));
                }
                if (isCallNext)
                    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                else
                    return (IntPtr)(-1);
            }
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public bool Uninstall()
        {
            if (!IsInstalled)
                throw new NotSupportedException("未安装钩子");

            _hookThread?.Abort();
            if (!UnhookWindowsHookEx(_hookHandle))
                //钩子卸载失败
                return false;
            IsInstalled = false;
            return true;
        }
    }
}
