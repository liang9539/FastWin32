using System.Runtime.CompilerServices;
using static FastWin32.Macro.MinWinDef;

namespace FastWin32.Macro
{
    /// <summary>
    /// windowsx.h
    /// </summary>
    public static class WindowsX
    {
        /// <summary>
        /// 从指定的lParam中获取X坐标
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetX(uint lParam)
        {
            return unchecked((short)LowWord(lParam));
        }

        /// <summary>
        /// 从指定的lParam中获取Y坐标
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetY(uint lParam)
        {
            return unchecked((short)HighWord(lParam));
        }
    }
}
