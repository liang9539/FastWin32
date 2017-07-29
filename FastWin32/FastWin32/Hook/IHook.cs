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
        void Install();

        /// <summary>
        /// 卸载
        /// </summary>
        void Uninstall();
    }
}
