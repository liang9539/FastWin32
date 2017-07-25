using System;

namespace FastWin32.Memory
{
    /// <summary>
    /// 在内存中运行exe
    /// </summary>
    public class MemoryExecute
    {
        /// <summary>
        /// 禁用无参构造函数
        /// </summary>
        private MemoryExecute() { }

        /// <summary>
        /// 使用指定参数运行内存中的exe
        /// </summary>
        /// <param name="buffer">内存中exe的地址</param>
        /// <param name="length">内存中exe占用的长度</param>
        public MemoryExecute(IntPtr buffer, uint length)
        {
            //TODO 未完成
        }

        /// <summary>
        /// 使用指定参数运行内存中的exe
        /// </summary>
        /// <param name="buffer">内存中exe的地址</param>
        /// <param name="length">内存中exe占用的长度</param>
        /// <param name="parameter">exe的启动参数</param>
        public MemoryExecute(IntPtr buffer,uint length, string parameter)
        {
            //TODO 未完成
        }

        /// <summary>
        /// 使用指定参数运行内存中的exe
        /// </summary>
        /// <param name="buffer">内存中exe的地址</param>
        /// <param name="length">内存中exe占用的长度</param>
        public unsafe MemoryExecute(byte* buffer, uint length)
        {
            //TODO 未完成
        }

        /// <summary>
        /// 使用指定参数运行内存中的exe
        /// </summary>
        /// <param name="buffer">内存中exe的地址</param>
        /// <param name="length">内存中exe占用的长度</param>
        /// <param name="parameter">exe的启动参数</param>
        public unsafe MemoryExecute(byte* buffer, uint length, string parameter)
        {
            //TODO 未完成
        }

        /// <summary>
        /// 获取对齐后大小
        /// </summary>
        /// <param name="origin">要对齐的数</param>
        /// <param name="alignment">对齐倍数</param>
        /// <returns></returns>
        private static uint GetAlignedSize(uint origin, uint alignment)
        {
            return (origin + alignment - 1) / alignment * alignment;
        }
    }
}
