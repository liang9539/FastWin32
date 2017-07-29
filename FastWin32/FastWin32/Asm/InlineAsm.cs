using System;

namespace FastWin32.Asm
{
    /// <summary>
    /// 内联汇编（输入机器码/指令并立即执行），与C++语法相似
    /// C#使用此类：
    /// using static FastWin32.Asm.InlineAsm;
    /// 然后_asm(xxxxx);即可
    /// VB.Net使用此类：
    /// Imports FastWin32.Asm.InlineAsm
    /// 然后_asm(xxxxx)即可
    /// </summary>
    public static class InlineAsm
    {
        /// <summary>
        /// 输入机器码并立即执行
        /// </summary>
        /// <param name="bytes">机器码</param>
        public static void _asm(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();
            if (bytes.Length == 0)
                throw new ArgumentOutOfRangeException();

            AsmLib.GetDelegateForAsm<Action>(bytes)();
        }

        /// <summary>
        /// 输入汇编指令并立即执行
        /// </summary>
        /// <param name="opcodes">汇编指令</param>
        public static void _asm(string[] opcodes)
        {
            if (opcodes == null)
                throw new ArgumentNullException();
            if (opcodes.Length == 0)
                throw new ArgumentOutOfRangeException();

            AsmLib.GetDelegateForAsm<Action>(opcodes)();
        }
    }
}
