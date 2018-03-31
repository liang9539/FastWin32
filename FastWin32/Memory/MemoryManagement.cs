using System;
using static FastWin32.NativeMethods;
using size_t = System.IntPtr;

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
        private static SafeNativeHandle OpenProcessVMOperation(uint processId)
        {
            return SafeOpenProcess(FastWin32Settings.SeDebugPrivilege ? PROCESS_ALL_ACCESS : PROCESS_VM_OPERATION, false, processId);
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
                writableFlags ^= AllMemoryProtectionFlags;
            executableFlags =
                PAGE_EXECUTE_READ |
                PAGE_EXECUTE_READWRITE;
            //可执行
            if (!executable)
                //如果不可执行
                executableFlags ^= AllMemoryProtectionFlags;
            return writableFlags & executableFlags;
        }
        #endregion

        #region AllocMemory
        /// <summary>
        /// 在当前进程中分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemory(size_t size)
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
        public static IntPtr AllocMemory(size_t size, bool writable, bool executable)
        {
            return AllocMemoryInternal(size, ProtectionFlagsGenerator(writable, executable));
        }

        /// <summary>
        /// 分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemoryEx(uint processId, size_t size)
        {
            return AllocMemoryEx(processId, size, true, false);
        }

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <returns>分配得到的内存所在地址</returns>
        public static IntPtr AllocMemoryEx(uint processId, size_t size, bool writable, bool executable)
        {
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessVMOperation(processId))
                if (processHandle.IsValid)
                    return AllocMemoryExInternal(processHandle, size, ProtectionFlagsGenerator(writable, executable));
                else
                    return IntPtr.Zero;
        }

        /// <summary>
        /// 在当前进程中分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(size_t size)
        {
            return VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        }

        /// <summary>
        /// 在当前进程中分配内存
        /// </summary>
        /// <param name="size">要分配内存的大小</param>
        /// <param name="flags">内存保护选项</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryInternal(size_t size, uint flags)
        {
            return VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, flags);
        }

        /// <summary>
        /// 分配内存（默认可写，不可执行）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="size">要分配内存的大小</param>
        /// <returns>分配得到的内存所在地址</returns>
        internal static IntPtr AllocMemoryExInternal(IntPtr processHandle, size_t size)
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
        internal static IntPtr AllocMemoryExInternal(IntPtr processHandle, size_t size, uint flags)
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
        public static bool FreeMemory(IntPtr addr, size_t size)
        {
            return FreeMemoryInternal(addr, size);
        }

        /// <summary>
        /// 释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        public static bool FreeMemoryEx(uint processId, IntPtr addr)
        {
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessVMOperation(processId))
                if (processHandle.IsValid)
                    return FreeMemoryInternal(processHandle, addr);
                else
                    return false;
        }

        /// <summary>
        /// 释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        public static bool FreeMemoryEx(uint processId, IntPtr addr, size_t size)
        {
            SafeNativeHandle processHandle;

            using (processHandle = OpenProcessVMOperation(processId))
                if (processHandle.IsValid)
                    return FreeMemoryExInternal(processHandle, addr, size);
                else
                    return false;
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryInternal(IntPtr addr)
        {
            return VirtualFree(addr, size_t.Zero, MEM_RELEASE);
        }

        /// <summary>
        /// 在当前进程中释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryInternal(IntPtr addr, size_t size)
        {
            return VirtualFree(addr, size, MEM_DECOMMIT);
        }

        /// <summary>
        /// 释放内存（MEM_RELEASE）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <returns></returns>
        internal static bool FreeMemoryExInternal(IntPtr processHandle, IntPtr addr)
        {
            return VirtualFreeEx(processHandle, addr, size_t.Zero, MEM_RELEASE);
        }

        /// <summary>
        /// 释放内存（MEM_DECOMMIT）
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">指定释放内存的地址</param>
        /// <param name="size">要释放内存的大小</param>
        internal static bool FreeMemoryExInternal(IntPtr processHandle, IntPtr addr, size_t size)
        {
            return VirtualFreeEx(processHandle, addr, size, MEM_DECOMMIT);
        }
        #endregion
    }
}
