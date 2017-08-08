using System;
using static FastWin32.NativeMethods;
using static FastWin32.Util;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存管理
    /// </summary>
    public static class MemoryManagement
    {
        //TODO 分配内存 释放内存 加载文件到内存

        #region MemoryProtectionFlags生成器
        /// <summary>
        /// 所有内存保护选项
        /// </summary>
        private const MemoryProtectionFlags AllMemoryProtectionFlags =
            MemoryProtectionFlags.PAGE_EXECUTE_READ |
            MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
            MemoryProtectionFlags.PAGE_READONLY |
            MemoryProtectionFlags.PAGE_READWRITE;

        /// <summary>
        /// 根据提供选项生成对应的MemoryProtectionFlags
        /// </summary>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <returns></returns>
        private static MemoryProtectionFlags ProtectionFlagsGenerator(bool writable, bool executable)
        {
            MemoryProtectionFlags writableFlags;
            MemoryProtectionFlags executableFlags;

            writableFlags =
                MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                MemoryProtectionFlags.PAGE_READWRITE;
            //可写
            if (!writable)
                //如果不可写
                writableFlags = AllMemoryProtectionFlags ^ writableFlags;
            executableFlags =
                MemoryProtectionFlags.PAGE_EXECUTE_READ |
                MemoryProtectionFlags.PAGE_EXECUTE_READWRITE;
            //可执行
            if (!executable)
                //如果不可执行
                executableFlags = AllMemoryProtectionFlags ^ executableFlags;
            return writableFlags & executableFlags;
        }
        #endregion

        #region alloc
        /// <summary>
        /// 在当前进程中分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemory(uint size)
        {
            return AllocMemory(size, true, false);
        }

        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemory(uint size, bool writable, bool executable)
        {
            return AllocMemoryInternal(size, ProtectionFlagsGenerator(writable, executable));
        }

        /// <summary>
        /// 分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemory(uint processId, uint size)
        {
            return AllocMemory(processId, size, true, false);
        }

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemory(uint processId, uint size, bool writable, bool executable)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            if (hProcess == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return AllocMemoryInternal(hProcess, size, ProtectionFlagsGenerator(writable, executable));
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 在当前进程中分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(uint size)
        {
            return VirtualAlloc(IntPtr.Zero, size, MemoryAllocationFlags.MEM_COMMIT | MemoryAllocationFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);
        }

        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="flags">内存保护选项</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(uint size, MemoryProtectionFlags flags)
        {
            return VirtualAlloc(IntPtr.Zero, size, MemoryAllocationFlags.MEM_COMMIT | MemoryAllocationFlags.MEM_RESERVE, flags);
        }

        /// <summary>
        /// 分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(IntPtr hProcess, uint size)
        {
            return VirtualAllocEx(hProcess, IntPtr.Zero, size, MemoryAllocationFlags.MEM_COMMIT | MemoryAllocationFlags.MEM_RESERVE, MemoryProtectionFlags.PAGE_READWRITE);
        }

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="flags">内存保护选项</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(IntPtr hProcess, uint size, MemoryProtectionFlags flags)
        {
            return VirtualAllocEx(hProcess, IntPtr.Zero, size, MemoryAllocationFlags.MEM_COMMIT | MemoryAllocationFlags.MEM_RESERVE, flags);
        }
        #endregion

        #region free
        /// <summary>
        /// 在当前进程中释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        public static bool FreeMemory(IntPtr addr)
        {
            return FreeMemoryInternal(addr);
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        public static bool FreeMemory(IntPtr addr, uint size)
        {
            return FreeMemoryInternal(addr, size);
        }

        /// <summary>
        /// 释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        public static bool FreeMemory(uint processId, IntPtr addr)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            if (hProcess == IntPtr.Zero)
                return false;
            try
            {
                return FreeMemoryInternal(hProcess, addr);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        public static bool FreeMemory(uint processId, IntPtr addr, uint size)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            if (hProcess == IntPtr.Zero)
                return false;
            try
            {
                return FreeMemoryInternal(hProcess, addr, size);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryInternal(IntPtr addr)
        {
            return VirtualFree(addr, 0, MemoryFreeFlag.MEM_RELEASE);
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryInternal(IntPtr addr, uint size)
        {
            return VirtualFree(addr, size, MemoryFreeFlag.MEM_DECOMMIT);
        }

        /// <summary>
        /// 释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryInternal(IntPtr hProcess, IntPtr addr)
        {
            return VirtualFreeEx(hProcess, addr, 0, MemoryFreeFlag.MEM_RELEASE);
        }

        /// <summary>
        /// 释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryInternal(IntPtr hProcess, IntPtr addr, uint size)
        {
            return VirtualFreeEx(hProcess, addr, size, MemoryFreeFlag.MEM_DECOMMIT);
        }
        #endregion
    }
}
