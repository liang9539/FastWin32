using System.Runtime.CompilerServices;

namespace FastWin32.Macro
{
    /// <summary>
    /// minwindef.h
    /// </summary>
    public static class MinWinDef
    {
        /// <summary>
        /// 使用指定高低位创建无符号短整型
        /// </summary>
        /// <param name="low">低位</param>
        /// <param name="high">高位</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort MakeWord(byte low, byte high)
        {
            return unchecked((ushort)(low | high << 8));
        }

        /// <summary>
        /// 使用指定高低位创建无符号整型
        /// </summary>
        /// <param name="low">低位</param>
        /// <param name="high">高位</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MakeLong(ushort low, ushort high)
        {
            return unchecked((uint)(low | high << 16));
        }

        /// <summary>
        /// 从指定值中获取低位
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LowByte(ushort value)
        {
            return unchecked((byte)(value & 0xFF));
        }

        /// <summary>
        /// 从指定值中获取低位
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HighByte(ushort value)
        {
            return unchecked((byte)(value >> 8));
        }

        /// <summary>
        /// 从指定值中获取低位
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LowWord(uint value)
        {
            return unchecked((ushort)(value & 0xFFFF));
        }

        /// <summary>
        /// 从指定值中获取低位
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HighWord(uint value)
        {
            return unchecked((ushort)(value >> 16));
        }
    }
}
