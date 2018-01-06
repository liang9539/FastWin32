namespace FastWin32.Hook
{
    /// <summary>
    /// 定义安装/卸载钩子的接口
    /// </summary>
    public interface IHook
    {
        /// <summary>
        /// 安装钩子
        /// </summary>
        bool Install();

        /// <summary>
        /// 卸载钩子
        /// </summary>
        bool Uninstall();
    }
}
