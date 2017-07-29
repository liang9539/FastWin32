using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        /// 将机器码写入内存，返回函数指针
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

            pAsm = MemoryManagement.AllocMemoryInternal((uint)bytes.Length, MemoryProtectionFlags.PAGE_EXECUTE_READ);
            //分配内存（可执行）
            if (!MemoryRW.WriteBytesInternal(CURRENT_PROCESS, pAsm, bytes))
                throw new Win32Exception();
            return pAsm;
        }

        /// <summary>
        /// 将汇编指令写入内存，返回函数指针
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
        /// 将机器码写入内存，返回对应的委托
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="bytes">机器码</param>
        /// <returns></returns>
        public static TDelegate GetDelegateForAsm<TDelegate>(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            if (bytes.Length == 0)
                throw new ArgumentOutOfRangeException();

            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(GetFunctionPointerForAsm(bytes), typeof(TDelegate));
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
