namespace FastWin32.Memory
{
    /// <summary>
    /// 指针类型
    /// </summary>
    internal enum PointerType : int
    {
        /// <summary>
        /// 模块名+偏移
        /// </summary>
        ModuleName_Offset,

        /// <summary>
        /// 地址+偏移
        /// </summary>
        Address_Offset
    }
}
