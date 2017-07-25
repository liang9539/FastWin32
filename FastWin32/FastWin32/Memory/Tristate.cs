namespace FastWin32.Memory
{
    /// <summary>
    /// 三态
    /// </summary>
    public enum Tristate : int
    {
        /// <summary>
        /// 是
        /// </summary>
        Yes = -1,

        /// <summary>
        /// 否
        /// </summary>
        No = 0,

        /// <summary>
        /// "是"与"否"均可
        /// </summary>
        Mix = 1
    }
}
