using System;
using System.Threading;
using System.Windows.Forms;
using static FastWin32.NativeMethods;

namespace FastWin32.Hook.WindowMessage
{
    /// <summary>
    /// 键盘消息钩子
    /// </summary>
    public class KeyboardHook : IHook, IDisposable
    {
        private bool _lowLevel;
        private uint _threadId;
        private IntPtr _hHook;
        /// <summary>
        /// 按键按下事件
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// 按键弹起事件
        /// </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// 按键按压事件
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        /// 是否安装
        /// </summary>
        public bool IsInstalled { get; private set; }

        /// <summary>
        /// 是否将钩子消息传递给下一个钩子处理函数，默认是
        /// </summary>
        public bool IsCallNext { get; set; }

        /// <summary>
        /// 创建低级键盘钩子实例
        /// </summary>
        public KeyboardHook()
        {
            _lowLevel = true;
            IsCallNext = true;
        }

        /// <summary>
        /// 创建线程专用键盘钩子实例
        /// </summary>
        /// <param name="threadId">监听的线程ID，0为全局钩子</param>
        public KeyboardHook(uint threadId)
        {
            _threadId = threadId;
            IsCallNext = true;
        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        public bool Install()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(KeyboardHook));
            if (IsInstalled)
                throw new NotSupportedException("无法重复安装钩子");

            bool finished;

            finished = false;
            if (_lowLevel)
            {
                //低级键盘钩子
                if (Application.MessageLoop)
                    //调用此方法的线程如果有消息循环，就不需要开新线程启动消息循环
                    _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, GetModuleHandle(null), 0);
                else
                    new Thread(() =>
                    {
                        _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, GetModuleHandle(null), 0);
                        finished = true;
                        Application.Run();
                    })
                    {
                        IsBackground = true
                    }.Start();
            }
            else
            {
                if (Application.MessageLoop)
                    //调用此方法的线程如果有消息循环，就不需要开新线程启动消息循环
                    _hHook = SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, GetModuleHandle(null), _threadId);
                else
                    new Thread(() =>
                    {
                        _hHook = SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, GetModuleHandle(null), _threadId);
                        finished = true;
                        Application.Run();
                    })
                    {
                        IsBackground = true
                    }.Start();
            }
            while (true)
            {
                //等待设置完成，再判断是否设置成功（因为多线程）
                Thread.Sleep(1);
                if (finished)
                    break;
            }
            if (_hHook == IntPtr.Zero)
                //钩子创建失败
                return false;
            IsInstalled = true;
            return true;
        }

        /// <summary>
        /// 键盘消息回调函数
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果代码小于0，挂钩过程必须将消息传递给CallNextHookEx函数，无需进一步处理，并应返回CallNextHookEx返回的值。</param>
        /// <param name="wParam">产生击键消息的密钥的虚拟密钥代码。</param>
        /// <param name="lParam">重复计数，扫描码，扩展密钥标志，上下文代码，先前的密钥状态标志和转换状态标志。有关lParam参数的更多信息，请参阅按键消息标志。下表描述了该值的位。</param>
        /// <returns></returns>
        private IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || (KeyDown == null && KeyUp == null && KeyPress == null))
                //如果nCode小于零，则钩子过程必须返回CallNextHookEx返回的值并且不对钩子消息做处理。如果3个事件均未被订阅，直接返回
                if (IsCallNext)
                    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                else
                    return (IntPtr)(-1);

            if (IsCallNext)
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            else
                return (IntPtr)(-1);
        }

        /// <summary>
        /// 低级键盘消息回调函数
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果nCode小于零，钩子过程必须将消息传递给CallNextHookEx函数，无需进一步处理，并应返回CallNextHookEx返回的值。</param>
        /// <param name="wParam">键盘消息的标识符。此参数可以是以下消息之一：WM_KEYDOWN，WM_KEYUP，WM_SYSKEYDOWN或WM_SYSKEYUP。</param>
        /// <param name="lParam">指向KBDLLHOOKSTRUCT结构的指针。</param>
        /// <returns></returns>
        private unsafe IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            uint wm;
            KBDLLHOOKSTRUCT kbMsg;

            if (nCode < 0 || (KeyDown == null && KeyUp == null && KeyPress == null))
                //如果nCode小于零，则钩子过程必须返回CallNextHookEx返回的值并且不对钩子消息做处理。如果3个事件均未被订阅，直接返回
                if (IsCallNext)
                    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                else
                    return (IntPtr)(-1);
            wm = (uint)wParam;
            //转换到无符号整型
            kbMsg = *(KBDLLHOOKSTRUCT*)lParam.ToPointer();
            //指针转换结构体
            if (KeyDown != null && (wm == WM_KEYDOWN || wm == WM_SYSKEYUP))
                //事件被订阅且按键按下
                KeyDown(this, new KeyEventArgs((Keys)kbMsg.vkCode));
            if (KeyPress != null && wm == WM_KEYDOWN)
            {
                //事件被订阅且按键按下
                byte[] kbState;
                byte[] buffer;

                kbState = new byte[256];
                GetKeyboardState(kbState);
                //获取按键状态
                buffer = new byte[2];
                if (ToAscii(kbMsg.vkCode, kbMsg.scanCode, kbState, buffer, kbMsg.flags) == 1)
                    //一个字符被复制到缓冲区，转换正常，然后引发事件
                    KeyPress(this, new KeyPressEventArgs((char)buffer[0]));
            }
            if (KeyUp != null && (wm == WM_KEYUP || wm == WM_SYSKEYUP))
                //事件被订阅且按键弹起1
                KeyUp(this, new KeyEventArgs((Keys)kbMsg.vkCode));
            if (IsCallNext)
                return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            else
                return (IntPtr)(-1);
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public bool Uninstall()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(KeyboardHook));
            if (IsInstalled == false)
                throw new NotSupportedException("未安装钩子");

            if (!UnhookWindowsHookEx(_hHook))
                //钩子卸载失败
                return false;
            IsInstalled = false;
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                Uninstall();
                disposedValue = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~KeyboardHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
