using System;
using static FastWin32.NativeMethods;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存管理
    /// </summary>
    public static class MemoryManagement
    {
        /// <summary>
        /// 打开进程（内存操作）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessVMOperation(uint processId)
        {
            return OpenProcess(PROCESS_VM_OPERATION, false, processId);
        }

        #region ProtectionFlagsGenerator
        /// <summary>
        /// 所有内存保护选项
        /// </summary>
        private const uint AllMemoryProtectionFlags =
            PAGE_EXECUTE_READ |
            PAGE_EXECUTE_READWRITE |
            PAGE_READONLY |
            PAGE_READWRITE;

        /// <summary>
        /// 根据提供选项生成对应的内存保护标识
        /// </summary>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <returns></returns>
        private static uint ProtectionFlagsGenerator(bool writable, bool executable)
        {
            uint writableFlags;
            uint executableFlags;

            writableFlags =
                PAGE_EXECUTE_READWRITE |
                PAGE_READWRITE;
            //可写
            if (!writable)
                //如果不可写
                writableFlags = AllMemoryProtectionFlags ^ writableFlags;
            executableFlags =
                PAGE_EXECUTE_READ |
                PAGE_EXECUTE_READWRITE;
            //可执行
            if (!executable)
                //如果不可执行
                executableFlags = AllMemoryProtectionFlags ^ executableFlags;
            return writableFlags & executableFlags;
        }
        #endregion

        #region AllocMemory
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
            IntPtr processHandle;

            processHandle = OpenProcessVMOperation(processId);
            if (processHandle == IntPtr.Zero)
                return IntPtr.Zero;
            try
            {
                return AllocMemoryInternal(processHandle, size, ProtectionFlagsGenerator(writable, executable));
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 在当前进程中分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(uint size)
        {
            return VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        }

        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="flags">内存保护选项</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(uint size, uint flags)
        {
            return VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, flags);
        }

        /// <summary>
        /// 分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(IntPtr processHandle, uint size)
        {
            return VirtualAllocEx(processHandle, IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        }

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="flags">内存保护选项</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(IntPtr processHandle, uint size, uint flags)
        {
            return VirtualAllocEx(processHandle, IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, flags);
        }
        #endregion

        #region FreeMemory
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
            IntPtr processHandle;

            processHandle = OpenProcessVMOperation(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            try
            {
                return FreeMemoryInternal(processHandle, addr);
            }
            finally
            {
                CloseHandle(processHandle);
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
            IntPtr processHandle;

            processHandle = OpenProcessVMOperation(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            try
            {
                return FreeMemoryInternal(processHandle, addr, size);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryInternal(IntPtr addr)
        {
            return VirtualFree(addr, 0, MEM_RELEASE);
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryInternal(IntPtr addr, uint size)
        {
            return VirtualFree(addr, size, MEM_DECOMMIT);
        }

        /// <summary>
        /// 释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryInternal(IntPtr processHandle, IntPtr addr)
        {
            return VirtualFreeEx(processHandle, addr, 0, MEM_RELEASE);
        }

        /// <summary>
        /// 释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryInternal(IntPtr processHandle, IntPtr addr, uint size)
        {
            return VirtualFreeEx(processHandle, addr, size, MEM_DECOMMIT);
        }
        #endregion
    }
}
