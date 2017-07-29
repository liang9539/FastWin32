using System;
using System.ComponentModel;
using FastWin32.Diagnostics;
using static FastWin32.Memory.MemoryRW;
using static FastWin32.Memory.Util;
using static FastWin32.NativeMethods;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存扫描
    /// </summary>
    public static class MemoryScan
    {
        #region MemoryProtectionFlags生成器
        /// <summary>
        /// 所有内存保护选项
        /// </summary>
        private const MemoryProtectionFlags AllMemoryProtectionFlags =
            MemoryProtectionFlags.PAGE_EXECUTE |
            MemoryProtectionFlags.PAGE_EXECUTE_READ |
            MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
            MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
            MemoryProtectionFlags.PAGE_GUARD |
            MemoryProtectionFlags.PAGE_NOACCESS |
            MemoryProtectionFlags.PAGE_NOCACHE |
            MemoryProtectionFlags.PAGE_READONLY |
            MemoryProtectionFlags.PAGE_READWRITE |
            MemoryProtectionFlags.PAGE_WRITECOMBINE |
            MemoryProtectionFlags.PAGE_WRITECOPY;

        /// <summary>
        /// 根据提供选项生成对应的MemoryProtectionFlags
        /// </summary>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <param name="copyOnWrite">写时复制</param>
        /// <returns></returns>
        private static MemoryProtectionFlags ProtectionFlagsGenerator(Tristate writable, Tristate executable, Tristate copyOnWrite)
        {
            MemoryProtectionFlags writableFlags;
            MemoryProtectionFlags executableFlags;
            MemoryProtectionFlags copyOnWriteFlags;

            switch (writable)
            {
                case Tristate.Yes:
                    writableFlags =
                        MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
                        MemoryProtectionFlags.PAGE_READWRITE |
                        MemoryProtectionFlags.PAGE_WRITECOMBINE |
                        MemoryProtectionFlags.PAGE_WRITECOPY;
                    //可写
                    break;
                case Tristate.No:
                    writableFlags =
                        AllMemoryProtectionFlags ^ (
                        MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
                        MemoryProtectionFlags.PAGE_READWRITE |
                        MemoryProtectionFlags.PAGE_WRITECOMBINE |
                        MemoryProtectionFlags.PAGE_WRITECOPY);
                    //不可写
                    break;
                case Tristate.Mix:
                    writableFlags = AllMemoryProtectionFlags;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (executable)
            {
                case Tristate.Yes:
                    executableFlags =
                        MemoryProtectionFlags.PAGE_EXECUTE |
                        MemoryProtectionFlags.PAGE_EXECUTE_READ |
                        MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY;
                    //可执行
                    break;
                case Tristate.No:
                    executableFlags =
                        AllMemoryProtectionFlags ^ (
                        MemoryProtectionFlags.PAGE_EXECUTE |
                        MemoryProtectionFlags.PAGE_EXECUTE_READ |
                        MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY);
                    break;
                case Tristate.Mix:
                    executableFlags = AllMemoryProtectionFlags;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (copyOnWrite)
            {
                case Tristate.Yes:
                    copyOnWriteFlags =
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
                        MemoryProtectionFlags.PAGE_WRITECOPY;
                    //写时复制
                    break;
                case Tristate.No:
                    copyOnWriteFlags =
                        AllMemoryProtectionFlags ^ (
                        MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
                        MemoryProtectionFlags.PAGE_WRITECOPY);
                    //写时不复制
                    break;
                case Tristate.Mix:
                    copyOnWriteFlags = AllMemoryProtectionFlags;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return writableFlags & executableFlags & copyOnWriteFlags;
        }
        #endregion

        #region 内存页面查询
        /// <summary>
        /// 获取有效内存范围
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="protectionFlags">允许的内存保护选项</param>
        /// <returns></returns>
        private static PagePool GetValidRange(IntPtr hProcess, MemoryProtectionFlags protectionFlags)
        {
            long startAddr;
            long stopAddr;
            uint size;
            PagePool result;

            if (ProcessX.Is64ProcessInternal(hProcess))
            {
                //64位进程
                startAddr = 0x0;
                stopAddr = 0x7FFF_FFFF_FFFF_FFFF;
            }
            else
            {
                //32位进程
                startAddr = 0x0;
                stopAddr = 0x7FFF_FFFF;
            }
            size = MEMORY_BASIC_INFORMATION.Size;
            //获取MEMORY_BASIC_INFORMATION结构的大小（32位与64位大小不同，因为用了IntPtr）
            result = new PagePool();
            do
            {
                if (VirtualQueryEx(hProcess, (IntPtr)startAddr, out MEMORY_BASIC_INFORMATION regionInfo, size) != size)
                    //查询内存页面信息失败
                    throw new Win32Exception();
                if (((regionInfo.State & PageState.MEM_COMMIT) == PageState.MEM_COMMIT) && ((regionInfo.Protect & protectionFlags) != 0))
                    //保护选项存在交集且存在PageState.MEM_COMMIT
                    result.Add(new Tuple<IntPtr, long>(regionInfo.BaseAddress, (long)regionInfo.RegionSize));
                //添加到结果
                startAddr += (long)regionInfo.RegionSize;
                //新的基址为老基址+老内存页面大小
            } while (startAddr <= stopAddr);
            return result;
        }
        #endregion

        #region 查找/替换字节数组
        #region 查找字节数组
        /// <summary>
        /// 查找字节数组
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="src">要查找的内容</param>
        public static void FindBytes(uint processId, byte[] src)
        {
            if (src == null || src.Length == 0)
                throw new ArgumentException();

            FindBytes(processId, src, Tristate.Yes, Tristate.Mix, Tristate.No, 1);
        }

        /// <summary>
        /// 查找字节数组
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="src">要查找的内容</param>
        /// <param name="writable">可写</param>
        /// <param name="executable">可执行</param>
        /// <param name="copyOnWrite">写时复制</param>
        /// <param name="alignment">对齐到倍数</param>
        public static void FindBytes(uint processId, byte[] src, Tristate writable, Tristate executable, Tristate copyOnWrite, int alignment)
        {
            if (src == null || src.Length == 0)
                throw new ArgumentException();

            IntPtr hProcess;
            MemoryProtectionFlags protectionFlags;

            hProcess = OpenProcessRWQuery(processId);
            if (hProcess == IntPtr.Zero)
                return;
            protectionFlags = ProtectionFlagsGenerator(writable, executable, copyOnWrite);
            //生成对应的Flags
            try
            {
                FindBytesInternal(hProcess, src, protectionFlags, alignment);
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 查找字节数组
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="src">要查找的内容</param>
        /// <param name="protectionFlags">可写</param>
        /// <param name="alignment">对齐到倍数</param>
        internal static unsafe void FindBytesInternal(IntPtr hProcess, byte[] src, MemoryProtectionFlags protectionFlags, int alignment)
        {
            PagePool pool;

            pool = GetValidRange(hProcess, protectionFlags);
            Console.WriteLine(pool.Total);
        }
        #endregion

        #region 替换字节数组

        #endregion
        #endregion

        #region 单个搜索线程
        /// <summary>
        /// 扫描线程
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="pool">池</param>
        /// <param name="src">要搜索的内容</param>
        /// <param name="alignment">对齐到倍数</param>
        /// <returns></returns>
        private static void ScanThread(IntPtr hProcess,PagePool pool, byte[] src, int alignment)
        {
            Tuple<IntPtr, long> page;
            byte[] bytPage;

            do
            {
                page = pool.Next();
                //取出
                if (page == null)
                    //取完了
                    return;
                if (page.Item2 < src.LongLength)
                    //页面大小小于要搜索内容的长度
                    continue;
                bytPage = new byte[page.Item2];
                //储存页面中所有字节
                if (ReadBytesInternal(hProcess, page.Item1, bytPage) == false)
                    //读取失败
                    continue;
                for (long i = 0; i < bytPage.LongLength; i++)
                {

                }
            } while (true);
        }
        #endregion
    }
}
