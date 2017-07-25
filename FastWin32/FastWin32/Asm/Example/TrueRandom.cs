using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Asm.Example
{
    /// <summary>
    /// 随机数生成器（需要CPU支持，可用于Intel Ivy Bridge以及之后架构的处理器，AMD 2015年6月以及之后架构的处理器）
    /// </summary>
    public class TrueRandom
    {
        /******************************
        * 参考资料：
        * https://software.intel.com/en-us/articles/intel-digital-random-number-generator-drng-software-implementation-guide
        * http://www.cnblogs.com/gc2013/p/4660430.html
        * https://en.wikipedia.org/wiki/RdRand
        ******************************/

        private static bool _isInitialized;
        private static bool _isSupported;
        private delegate uint GetEcxNativeCall();
        private delegate ushort Rand16NativeCall();
        private static Rand16NativeCall Rand16Native;
        private delegate uint Rand32NativeCall();
        private static Rand32NativeCall Rand32Native;
        private delegate ulong Rand64NativeCall();
        private static Rand64NativeCall Rand64Native;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>CPU支持将返回true，cpu不支持将返回false</returns>
        public static unsafe bool Initialize()
        {
            if (_isInitialized)
                //已经初始化过，返回是否CPU支持指令
                return _isSupported;

            IntPtr pAsm;
            byte[] bytAsm;

            _isInitialized = true;
            IsSupported();
            //获取是否支持
            if (!_isSupported)
                return false;
            pAsm = MemoryManagement.AllocMemoryInternal(100, MemoryProtectionFlags.PAGE_EXECUTE_READ);
            //分配内存，用于存储机器码
            #region short
            bytAsm = new byte[]
            {
                0xF, 0xC7, 0xF0,
                //RDRAND eax
                0xC3
                //ret
            };
            if (!MemoryRW.WriteBytesInternal((IntPtr)(-1), pAsm, bytAsm))
                //写入失败
                throw new Win32Exception();
            Rand16Native = (Rand16NativeCall)Marshal.GetDelegateForFunctionPointer(pAsm, typeof(Rand16NativeCall));
            //通过函数指针获取委托
            #endregion
            #region int
            pAsm += bytAsm.Length;
            bytAsm = new byte[]
            {
                0xF, 0xC7, 0xF0,
                //RDRAND eax
                0xC3
                //ret
            };
            if (!MemoryRW.WriteBytesInternal((IntPtr)(-1), pAsm, bytAsm))
                //写入失败
                throw new Win32Exception();
            Rand32Native = (Rand32NativeCall)Marshal.GetDelegateForFunctionPointer(pAsm, typeof(Rand32NativeCall));
            //通过函数指针获取委托
            #endregion
            #region short
            pAsm += bytAsm.Length;
            bytAsm = new byte[]
            {
                0x48, 0xF, 0xC7, 0xF0,
                //RDRAND rax
                0xC3
                //ret
            };
            if (!MemoryRW.WriteBytesInternal((IntPtr)(-1), pAsm, bytAsm))
                //写入失败
                throw new Win32Exception();
            Rand64Native = (Rand64NativeCall)Marshal.GetDelegateForFunctionPointer(pAsm, typeof(Rand64NativeCall));
            //通过函数指针获取委托
            #endregion
            return true;
        }

        /// <summary>
        /// 获取CPU是否支持RdRand
        /// </summary>
        private static unsafe void IsSupported()
        {
            IntPtr pAsm;
            byte[] bytAsm;
            uint ecx;

            ecx = 0;
            if (Environment.Is64BitProcess)
            {
                //664位汇编
                bytAsm = new byte[] { 0x40, 0x55, 0x53, 0x48, 0x83, 0xEC, 0x68, 0x48, 0x8B, 0xEC, 0xB8, 0x04, 0x00, 0x00, 0x00, 0x48, 0x6B, 0xC0, 0x00, 0x48, 0x8D, 0x44, 0x05, 0x00, 0x48, 0x89, 0x45, 0x50, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x33, 0xC9, 0x0F, 0xA2, 0x4C, 0x8B, 0x45, 0x50, 0x41, 0x89, 0x00, 0x41, 0x89, 0x58, 0x04, 0x41, 0x89, 0x48, 0x08, 0x41, 0x89, 0x50, 0x0C, 0xB8, 0x04, 0x00, 0x00, 0x00, 0x48, 0x6B, 0xC0, 0x02, 0x8B, 0x44, 0x05, 0x00, 0x48, 0x8D, 0x65, 0x68, 0x5B, 0x5D, 0xC3 };
                //真·C++代码（生成的汇编转换成的机器码）
            }
            else
            {
                //32位汇编
                bytAsm = new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x50, 0x53, 0x56, 0x57, 0xB8, 0x04, 0x00, 0x00, 0x00, 0x6B, 0xC8, 0x00, 0x8D, 0x74, 0x0D, 0xF0, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x33, 0xC9, 0x0F, 0xA2, 0x89, 0x06, 0x89, 0x5E, 0x04, 0x89, 0x4E, 0x08, 0x89, 0x56, 0x0C, 0xB8, 0x04, 0x00, 0x00, 0x00, 0xD1, 0xE0, 0x8B, 0x44, 0x05, 0xF0, 0x5F, 0x5E, 0x5B, 0x8B, 0xE5, 0x5D, 0xC3 };
            }
            pAsm = MemoryManagement.AllocMemoryInternal((uint)bytAsm.Length, MemoryProtectionFlags.PAGE_EXECUTE_READ);
            //分配内存，用于存储机器码
            if (!MemoryRW.WriteBytesInternal((IntPtr)(-1), pAsm, bytAsm))
                //写入失败
                throw new Win32Exception();
            GetEcxNativeCall GetEcxNative = (GetEcxNativeCall)Marshal.GetDelegateForFunctionPointer(pAsm, typeof(GetEcxNativeCall));
            ecx = GetEcxNative();
            //获取ecx寄存器的值
            _isSupported = (ecx & 0x40000000) == 0x40000000;
        }

        /// <summary>
        /// 产生一个16位随机数
        /// </summary>
        /// <returns></returns>
        public static ushort Rand16()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();
            if (!_isSupported)
                throw new NotSupportedException();

            return Rand16Native();
        }

        /// <summary>
        /// 产生一个32位随机数
        /// </summary>
        /// <returns></returns>
        public static uint Rand32()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();
            if (!_isSupported)
                throw new NotSupportedException();

            return Rand32Native();
        }

        /// <summary>
        /// 产生一个64位随机数
        /// </summary>
        /// <returns></returns>
        public static ulong Rand64()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();
            if (!_isSupported)
                throw new NotSupportedException();

            if (Environment.Is64BitProcess)
            {
                return Rand64Native();
            }
            else
            {
                byte[] bytLong;

                bytLong = new byte[8];
                BitConverter.GetBytes(Rand32Native()).CopyTo(bytLong, 0);
                BitConverter.GetBytes(Rand32Native()).CopyTo(bytLong, 4);
                return BitConverter.ToUInt64(bytLong, 0);
            }
        }
    }
}
