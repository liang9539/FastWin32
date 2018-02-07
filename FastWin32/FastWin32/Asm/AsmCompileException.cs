using System;

namespace FastWin32.Asm
{
    /// <summary>
    /// Asm编译/反编译错误
    /// </summary>
    public class AsmCompilerException : Exception
    {
        /// <summary>
        /// 用指定的错误消息创建新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        internal AsmCompilerException(string message) : base(message)
        {
        }
    }
}
