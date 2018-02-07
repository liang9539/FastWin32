using System;
using System.Collections.Generic;

namespace FastWin32.Asm
{
    /// <summary>
    /// 汇编指令，机器码对应表
    /// </summary>
    public class AsmData
    {
        internal List<byte> _byteList;

        /// <summary>
        /// 汇编指令
        /// </summary>
        public string Opcode { get; internal set; }

        /// <summary>
        /// 机器码
        /// </summary>
        public byte[] Bytes { get; internal set; }

        /// <summary>
        /// 实例化对应表
        /// </summary>
        /// <param name="opcode">汇编指令</param>
        internal AsmData(string opcode)
        {
            Opcode = opcode;
            _byteList = new List<byte>();
        }

        /// <summary>
        /// 更新对应的机器码，设置<see cref="_byteList"/>为<see langword="null"/>，转换为只读模式
        /// </summary>
        internal void AsReadOnly()
        {
            Bytes = _byteList.ToArray();
            _byteList = null;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Opcode} - {BitConverter.ToString(Bytes).Replace("-", string.Empty)}";
        }
    }
}
