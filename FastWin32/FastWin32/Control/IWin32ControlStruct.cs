namespace FastWin32.Control
{
    /// <summary>
    /// 表示一个Win32控件使用的结构体
    /// </summary>
    internal interface IWin32ControlStruct
    {
        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// 获取指向当前对象的指针
        /// </summary>
        /// <returns></returns>
        unsafe void* ToPointer();
    }
}
