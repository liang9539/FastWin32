using System;
using System.Runtime.InteropServices;
using FastWin32.Memory;
using static FastWin32.NativeMethods;

namespace FastWin32.Asm
{
    /// <summary>
    /// Asm基础库
    /// </summary>
    public static class AsmLib
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static AsmLib()
        {
            if (Environment.Is64BitProcess)
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// 将机器码写入内存，返回函数指针，如果执行失败。返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static IntPtr GetFunctionPointerForAsm(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            if (bytes.Length == 0)
                throw new ArgumentOutOfRangeException();

            IntPtr pAsm;

            pAsm = MemoryManagement.AllocMemoryInternal((uint)bytes.Length, PAGE_EXECUTE_READ);
            //分配内存（可执行）
            if (!MemoryIO.WriteBytesInternal(CURRENT_PROCESS, pAsm, bytes))
                return IntPtr.Zero;
            return pAsm;
        }

        /// <summary>
        /// 将汇编指令写入内存，返回函数指针，如果执行失败。返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        /// <returns></returns>
        public static IntPtr GetFunctionPointerForAsm(string[] opcodes)
        {
            if (opcodes == null)
                throw new ArgumentNullException();
            if (opcodes.Length == 0)
                throw new ArgumentOutOfRangeException();

            return GetFunctionPointerForAsm(Assembler.OpcodesToBytes(opcodes));
        }

        /// <summary>
        /// 将机器码写入内存，返回对应的委托，如果执行失败。返回空值
        /// </summary>
        /// <typeparam name="TDelegate">委托</typeparam>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static TDelegate GetDelegateForAsm<TDelegate>(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            if (bytes.Length == 0)
                throw new ArgumentOutOfRangeException();

            IntPtr pFunc;

            pFunc = GetFunctionPointerForAsm(bytes);
            if (pFunc == IntPtr.Zero)
                return default(TDelegate);
            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(TDelegate));
        }

        /// <summary>
        /// 将汇编指令写入内存，返回对应的委托
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="opcodes">汇编指令</param>
        /// <returns></returns>
        public static TDelegate GetDelegateForAsm<TDelegate>(string[] opcodes)
        {
            if (opcodes == null)
                throw new ArgumentNullException();
            if (opcodes.Length == 0)
                throw new ArgumentOutOfRangeException();

            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(GetFunctionPointerForAsm(opcodes), typeof(TDelegate));
        }
    }
}
