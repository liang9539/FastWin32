using static FastWin32.Macro.MinWinDef;

namespace FastWin32.Macro
{
    /// <summary>
    /// 对头文件中的宏的扩展
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 使用指定XY合成lParam
        /// </summary>
        /// <param name="xPos">X坐标</param>
        /// <param name="yPos">Y坐标</param>
        /// <returns></returns>
        public static uint CombineXY(int xPos, int yPos)
        {
            return MakeLong((ushort)xPos, (ushort)yPos);
        }
    }
}
