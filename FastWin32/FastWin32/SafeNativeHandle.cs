using System;
using static FastWin32.NativeMethods;

namespace FastWin32
{
    /// <summary>
    /// 安全句柄
    /// </summary>
    internal struct SafeNativeHandle : IDisposable
    {
        private IntPtr _handle;

        private bool _disposed;

        public static implicit operator SafeNativeHandle(IntPtr value) => new SafeNativeHandle() { _handle = value };

        public static implicit operator IntPtr(SafeNativeHandle value) => value._handle;

        public void Dispose()
        {
            if (_disposed)
                return;

            CloseHandle(_handle);
            _disposed = true;
        }
    }
}
