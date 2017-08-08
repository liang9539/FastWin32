//*****************************************************
//
//C# Dll注入模板
//将此文件复制到你的C#项目即可
//
//*****************************************************

namespace Injecting
{
    /// <summary>
    /// Dll注入使用
    /// </summary>
    public static class Injector
    {
        /// <summary>
        /// DllMain
        /// </summary>
        /// <param name="arg">无效参数，忽视，但此形参不能删除！！！</param>
        /// <returns></returns>
        public static int DllMain(string arg)
        {
            System.Windows.Forms.MessageBox.Show("CSInjector");
            //这句话改成你的一段代码，比如Hook 读写内存什么的
            return 0;
        }
    }
}
