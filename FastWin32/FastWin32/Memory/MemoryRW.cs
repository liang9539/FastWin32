using System;
using FastWin32.Diagnostics;
using static FastWin32.NativeMethods;
using static FastWin32.Util;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存读写
    /// </summary>
    public static class MemoryRW
    {
        #region 获取模块基址
        /// <summary>
        /// 获取模块基址
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="first">是否返回第一个模块基址</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="flag">过滤标识</param>
        /// <param name="value">模块基址</param>
        /// <returns></returns>
        internal static unsafe bool GetBaseAddrInternal(IntPtr hProcess, bool first, string moduleName, EnumModulesFilterFlag flag, out IntPtr value)
        {
            return ModuleX.GetHandleInternal(hProcess, first, moduleName, flag, out value);
        }

        /// <summary>
        /// 获取主模块基址
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr GetBaseAddr(uint processId)
        {
            return ModuleX.GetHandle(processId);
        }

        /// <summary>
        /// 获取模块基址
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static IntPtr GetBaseAddr(uint processId, string moduleName)
        {
            return ModuleX.GetHandle(processId, moduleName);
        }
        #endregion

        #region 获取指针指向的地址
        /// <summary>
        /// 获取指针指向的地址
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="p">指针</param>
        /// <returns></returns>
        internal static bool GetPointerAddrInternal(IntPtr hProcess, Pointer p)
        {
            int newAddr32 = 0;
            long newAddr64 = 0;

            p._lastAddr = IntPtr.Zero;
            if (!GetBaseAddrInternal(hProcess, false, p.ModuleName, EnumModulesFilterFlag.X86, out p._lastAddr))
            {
                //不使用EnumModulesFilterFlag.ALL是因为无法识别到底是32位模块还是64位模块
                //获取32位模块基址失败
                goto x64;
            }
            p._lastAddr += p.ModuleOffset;
            //加上模块偏移
            if (p.Offset != null)
            {
                //有偏移
                for (int i = 0; i < p.Offset.Length; i++)
                {
                    //处理每个偏移
                    if (!ReadInt32Internal(hProcess, p._lastAddr, out newAddr32))
                        //读取新地址失败
                        return false;
                    p._lastAddr = (IntPtr)(newAddr32 + p.Offset[i]);
                    //生成新地址
                }
            }
            return true;
            x64:
            if (!GetBaseAddrInternal(hProcess, false, p.ModuleName, EnumModulesFilterFlag.X64, out p._lastAddr))
                //获取64位模块基址失败
                return false;
            p._lastAddr += p.ModuleOffset;
            //加上模块偏移
            if (p.Offset != null)
            {
                //有偏移
                for (int i = 0; i < p.Offset.Length; i++)
                {
                    //处理每个偏移
                    if (!ReadInt64Internal(hProcess, p._lastAddr, out newAddr64))
                        //读取新地址失败
                        return false;
                    p._lastAddr = (IntPtr)(newAddr64 + p.Offset[i]);
                    //生成新地址
                }
            }
            return true;
        }
        #endregion

        #region 读写模板
        /// <summary>
        /// 内存读取模板回调函数（对于值类型）
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private delegate bool TemplateOfReadCallback<TValue>(IntPtr hProcess, IntPtr addr, out TValue value);

        /// <summary>
        /// 内存读取模板回调函数（对于引用类型）
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        private delegate bool TemplateOfReadRefCallback(IntPtr hProcess, IntPtr addr);

        /// <summary>
        /// 内存写入模板回调函数（对于值/引用类型）
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        private delegate bool TemplateOfWriteCallback(IntPtr hProcess, IntPtr addr);

        /// <summary>
        /// 内存读取模板（对于值类型）
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfRead<TValue>(uint processId, IntPtr addr, out TValue value, TemplateOfReadCallback<TValue> reader)
        {
            IntPtr hProcess;

            value = default(TValue);
            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                return reader(hProcess, addr, out value);
                //读取
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 内存读取模板（对于值类型）
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfRead<TValue>(uint processId, Pointer p, out TValue value, TemplateOfReadCallback<TValue> reader)
        {
            IntPtr hProcess;

            value = default(TValue);
            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                if (!GetPointerAddrInternal(hProcess, p))
                {
                    //获取指针指向的地址
                    value = default(TValue);
                    return false;
                }
                return reader(hProcess, p._lastAddr, out value);
                //读取
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 内存读取模板（对于引用类型）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfReadRef(uint processId, IntPtr addr, TemplateOfReadRefCallback reader)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                return reader(hProcess, addr);
                //读取
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 内存读取模板（对于引用类型）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfReadRef(uint processId, Pointer p, TemplateOfReadRefCallback reader)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                if (!GetPointerAddrInternal(hProcess, p))
                    //获取指针指向的地址
                    return false;
                return reader(hProcess, p._lastAddr);
                //读取
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 内存写入模板（对于值/引用类型）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="writer">写入器</param>
        /// <returns></returns>
        private static bool TemplateOfWrite(uint processId, IntPtr addr, TemplateOfWriteCallback writer)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                return writer(hProcess, addr);
                //写入
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }

        /// <summary>
        /// 内存写入模板（对于值/引用类型）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="writer">写入器</param>
        /// <returns></returns>
        private static bool TemplateOfWrite(uint processId, Pointer p, TemplateOfWriteCallback writer)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRW(processId);
            //读写权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (ProcessX.Is64ProcessInternal(hProcess) && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                if (!GetPointerAddrInternal(hProcess, p))
                    //获取指针指向的地址
                    return false;
                return writer(hProcess, p._lastAddr);
                //写入
            }
            finally
            {
                CloseHandle(hProcess);
                //关闭句柄
            }
        }
        #endregion

        #region 读写字节数组
        #region 读取字节数组
        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadBytes(uint processId, IntPtr addr, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return TemplateOfReadRef(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => ReadBytesInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadBytes(uint processId, Pointer p, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return TemplateOfReadRef(processId, p, (IntPtr hProcess, IntPtr addrCallback) => ReadBytesInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadBytesInternal(IntPtr hProcess, IntPtr addr, byte[] value)
        {
            return ReadProcessMemory(hProcess, addr, value, (uint)value.Length, null);
        }
        #endregion

        #region 写入字节数组
        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteBytes(uint processId, IntPtr addr, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteBytesInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteBytes(uint processId, Pointer p, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteBytesInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteBytesInternal(IntPtr hProcess, IntPtr addr, byte[] value)
        {
            return WriteProcessMemory(hProcess, addr, value, (uint)value.Length, null);
        }
        #endregion
        #endregion

        #region 读取区域
        /// <summary>
        /// 读取内存页面模式
        /// </summary>
        public enum ReadPageMode
        {
            /// <summary>
            /// 当前地址之后（包括当前地址）
            /// </summary>
            After,
            /// <summary>
            /// 当前地址之前（包括当前地址）
            /// </summary>
            Before,
            /// <summary>
            /// 整个内存页面
            /// </summary>
            Full
        }

        /// <summary>
        /// 读取地址所在内存页面，读取长度由页面大小以及mode决定决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="mode">读取模式</param>
        /// <returns></returns>
        public static unsafe bool ReadPage(uint processId, IntPtr addr, out byte[] value, ReadPageMode mode)
        {
            IntPtr hProcess;

            hProcess = OpenProcessRWQuery(processId);
            if (hProcess == IntPtr.Zero)
            {
                value = null;
                return false;
            }
            return ReadPageInternal(hProcess, addr, out value, mode);
        }

        /// <summary>
        /// 读取地址所在内存页面，读取长度由页面大小以及mode决定决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="mode">读取模式</param>
        /// <returns></returns>
        public static unsafe bool ReadPage(uint processId, Pointer p, out byte[] value, ReadPageMode mode)
        {
            IntPtr hProcess;

            value = null;
            hProcess = OpenProcessRWQuery(processId);
            if (hProcess == IntPtr.Zero)
                return false;
            if (!GetPointerAddrInternal(hProcess, p))
                return false;
            return ReadPageInternal(hProcess, p._lastAddr, out value, mode);
        }

        /// <summary>
        /// 读取地址所在内存页面，读取长度由页面大小以及mode决定决定
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="mode">读取模式</param>
        /// <returns></returns>
        internal static unsafe bool ReadPageInternal(IntPtr hProcess, IntPtr addr, out byte[] value, ReadPageMode mode)
        {
            value = null;
            if (VirtualQueryEx(hProcess, addr, out MEMORY_BASIC_INFORMATION pageInfo, MEMORY_BASIC_INFORMATION.Size) != MEMORY_BASIC_INFORMATION.Size)
                //查询失败
                return false;
            switch (mode)
            {
                case ReadPageMode.After:
                    value = new byte[(int)pageInfo.BaseAddress + (int)pageInfo.RegionSize - (int)addr];
                    //读取长度=页面基址+页面大小-当前地址
                    break;
                case ReadPageMode.Before:
                    value = new byte[(int)addr - (int)pageInfo.BaseAddress + 1];
                    //读取长度=当前地址-页面基址+1
                    break;
                case ReadPageMode.Full:
                    value = new byte[(int)pageInfo.RegionSize];
                    //读取长度=页面大小
                    addr = pageInfo.BaseAddress;
                    break;
            }
            return ReadProcessMemory(hProcess, addr, value, (uint)value.Length, null);
        }
        #endregion

        #region 读写字节
        #region 读取字节
        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadByte(uint processId, IntPtr addr, out byte value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out byte buffer) => ReadByteInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadByte(uint processId, Pointer p, out byte value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out byte buffer) => ReadByteInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadByteInternal(IntPtr hProcess, IntPtr addr, out byte value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 1, null);
        }
        #endregion

        #region 写入字节
        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteByte(uint processId, IntPtr addr, byte value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteByteInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteByte(uint processId, Pointer p, byte value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteByteInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteByteInternal(IntPtr hProcess, IntPtr addr, byte value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 1, null);
        }
        #endregion
        #endregion

        #region 读写布尔值
        #region 读取布尔值
        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadBoolean(uint processId, IntPtr addr, out bool value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadBoolean(uint processId, Pointer p, out bool value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadBooleanInternal(IntPtr hProcess, IntPtr addr, out bool value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 1, null);
        }
        #endregion

        #region 写入布尔值
        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteBoolean(uint processId, IntPtr addr, bool value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteBooleanInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteBoolean(uint processId, Pointer p, bool value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteBooleanInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteBooleanInternal(IntPtr hProcess, IntPtr addr, bool value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 1, null);
        }
        #endregion
        #endregion

        #region 读写短整形
        #region 读取短整形
        /// <summary>
        /// 读取短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadChar(uint processId, IntPtr addr, out char value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out char buffer) => ReadCharInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadChar(uint processId, Pointer p, out char value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out char buffer) => ReadCharInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取短整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadCharInternal(IntPtr hProcess, IntPtr addr, out char value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 2, null);
        }
        #endregion

        #region 写入短整形
        /// <summary>
        /// 写入短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteChar(uint processId, IntPtr addr, char value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteCharInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteChar(uint processId, Pointer p, char value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteCharInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入短整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteCharInternal(IntPtr hProcess, IntPtr addr, char value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 2, null);
        }
        #endregion
        #endregion

        #region 读写无符号短整形
        #region 读取无符号短整形
        /// <summary>
        /// 读取无符号短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt16(uint processId, IntPtr addr, out ushort value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt16(uint processId, Pointer p, out ushort value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号短整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt16Internal(IntPtr hProcess, IntPtr addr, out ushort value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 2, null);
        }
        #endregion

        #region 写入无符号短整形
        /// <summary>
        /// 写入无符号短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt16(uint processId, IntPtr addr, ushort value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt16Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt16(uint processId, Pointer p, ushort value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt16Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号短整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt16Internal(IntPtr hProcess, IntPtr addr, ushort value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 2, null);
        }
        #endregion
        #endregion

        #region 读写整形
        #region 读取整形
        /// <summary>
        /// 读取整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadInt32(uint processId, IntPtr addr, out int value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out int buffer) => ReadInt32Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadInt32(uint processId, Pointer p, out int value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out int buffer) => ReadInt32Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadInt32Internal(IntPtr hProcess, IntPtr addr, out int value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 4, null);
        }
        #endregion

        #region 写入整形
        /// <summary>
        /// 写入整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteInt32(uint processId, IntPtr addr, int value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteInt32Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteInt32(uint processId, Pointer p, int value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteInt32Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteInt32Internal(IntPtr hProcess, IntPtr addr, int value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 4, null);
        }
        #endregion
        #endregion

        #region 读写无符号整形
        #region 读取无符号整形
        /// <summary>
        /// 读取无符号整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt32(uint processId, IntPtr addr, out uint value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt32(uint processId, Pointer p, out uint value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt32Internal(IntPtr hProcess, IntPtr addr, out uint value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 4, null);
        }
        #endregion

        #region 写入无符号整形
        /// <summary>
        /// 写入无符号整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt32(uint processId, IntPtr addr, uint value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt32Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt32(uint processId, Pointer p, uint value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt32Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt32Internal(IntPtr hProcess, IntPtr addr, uint value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 4, null);
        }
        #endregion
        #endregion

        #region 读写长整形
        #region 读取长整形
        /// <summary>
        /// 读取长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadInt64(uint processId, IntPtr addr, out long value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out long buffer) => ReadInt64Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadInt64(uint processId, Pointer p, out long value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out long buffer) => ReadInt64Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取长整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadInt64Internal(IntPtr hProcess, IntPtr addr, out long value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 8, null);
        }
        #endregion

        #region 写入长整形
        /// <summary>
        /// 写入长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteInt64(uint processId, IntPtr addr, long value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteInt64Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteInt64(uint processId, Pointer p, long value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteInt64Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入长整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteInt64Internal(IntPtr hProcess, IntPtr addr, long value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 8, null);
        }
        #endregion
        #endregion

        #region 读写无符号长整形
        #region 读取无符号长整形
        /// <summary>
        /// 读取无符号长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt64(uint processId, IntPtr addr, out ulong value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadUInt64(uint processId, Pointer p, out ulong value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号长整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt64Internal(IntPtr hProcess, IntPtr addr, out ulong value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 8, null);
        }
        #endregion

        #region 写入无符号长整形
        /// <summary>
        /// 写入无符号长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt64(uint processId, IntPtr addr, ulong value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt64Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号长整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteUInt64(uint processId, Pointer p, ulong value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteUInt64Internal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号长整形
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt64Internal(IntPtr hProcess, IntPtr addr, ulong value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 8, null);
        }
        #endregion
        #endregion

        #region 读写单精度浮点型
        #region 读取单精度浮点型
        /// <summary>
        /// 读取单精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadFloat(uint processId, IntPtr addr, out float value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out float buffer) => ReadFloatInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取单精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadFloat(uint processId, Pointer p, out float value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out float buffer) => ReadFloatInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取单精度浮点型
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadFloatInternal(IntPtr hProcess, IntPtr addr, out float value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 4, null);
        }
        #endregion

        #region 写入单精度浮点型
        /// <summary>
        /// 写入单精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteFloat(uint processId, IntPtr addr, float value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteFloatInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入单精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteFloat(uint processId, Pointer p, float value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteFloatInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入单精度浮点型
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteFloatInternal(IntPtr hProcess, IntPtr addr, float value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 4, null);
        }
        #endregion
        #endregion

        #region 读写双精度浮点型
        #region 读取双精度浮点型
        /// <summary>
        /// 读取双精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadDouble(uint processId, IntPtr addr, out double value)
        {
            return TemplateOfRead(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取双精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadDouble(uint processId, Pointer p, out double value)
        {
            return TemplateOfRead(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取双精度浮点型
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadDoubleInternal(IntPtr hProcess, IntPtr addr, out double value)
        {
            return ReadProcessMemory(hProcess, addr, out value, 8, null);
        }
        #endregion

        #region 写入双精度浮点型
        /// <summary>
        /// 写入双精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteDouble(uint processId, IntPtr addr, double value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteDoubleInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入双精度浮点型
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteDouble(uint processId, Pointer p, double value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteDoubleInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入双精度浮点型
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteDoubleInternal(IntPtr hProcess, IntPtr addr, double value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, 8, null);
        }
        #endregion
        #endregion

        #region 读写字符串

        #endregion
    }
}
