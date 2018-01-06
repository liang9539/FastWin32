namespace FastWin32.Memory
{
    /// <summary>
    /// 读取内存页面模式
    /// </summary>
    public enum ReadPageMode
    {
        /// <summary>
        /// 当前地址之后（包括当前地址）
        /// </summary>
        After,

        /// <summary>
        /// 当前地址之前（包括当前地址）
        /// </summary>
        Before,

        /// <summary>
        /// 整个内存页面
        /// </summary>
        Full
    }
}
