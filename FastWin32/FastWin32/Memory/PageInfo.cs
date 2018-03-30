using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存页面信息
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 地址
        /// </summary>
        public IntPtr Address { get; }

        /// <summary>
        /// 大小
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// 保护选项
        /// </summary>
        public uint Protect { get; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public uint Type { get; }

        internal PageInfo(MEMORY_BASIC_INFORMATION mbi)
        {
            Address = mbi.BaseAddress;
            Size = (uint)mbi.RegionSize;
            Protect = mbi.Protect;
            Type = mbi.Type;
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            bool is64;

            is64 = (ulong)Address > uint.MaxValue;
            return $"Address=0x{Address.ToString(is64 ? "X16" : "X8")} Size=0x{Size.ToString("X8")}";
        }
    }
}
