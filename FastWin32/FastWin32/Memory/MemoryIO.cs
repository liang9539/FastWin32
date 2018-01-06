using System;
using System.Collections.Generic;
using System.Text;
using FastWin32.Diagnostics;
using static FastWin32.NativeMethods;
using static FastWin32.Util;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存读写
    /// </summary>
    public static class MemoryIO
    {
        #region 获取模块基址
        /// <summary>
        /// 获取模块基址，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="first">是否返回第一个模块基址</param>
        /// <param name="moduleName">模块名</param>
        /// <param name="flag">过滤标识</param>
        /// <returns></returns>
        internal static unsafe IntPtr GetBaseAddrInternal(IntPtr hProcess, bool first, string moduleName, uint flag)
        {
            return Module.GetHandleInternal(hProcess, first, moduleName, flag);
        }

        /// <summary>
        /// 获取主模块基址，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        public static IntPtr GetBaseAddr(uint processId)
        {
            return Module.GetHandle(processId);
        }

        /// <summary>
        /// 获取模块基址，获取失败时返回 <see cref="IntPtr.Zero"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="moduleName">模块名</param>
        /// <returns></returns>
        public static IntPtr GetBaseAddr(uint processId, string moduleName)
        {
            return Module.GetHandle(processId, moduleName);
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
            bool is64;
            int newAddr32 = 0;
            long newAddr64 = 0;

            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            //获取进程位数失败
            if (p._type == PointerType.Address_Offset)
            {
                //地址+偏移
                if (is64)
                {
                    if (!ReadInt64Internal(hProcess, p._baseAddr, out newAddr64))
                        return false;
                    p._lastAddr = (IntPtr)newAddr64;
                }
                else
                {
                    if (!ReadInt32Internal(hProcess, p._baseAddr, out newAddr32))
                        return false;
                    p._lastAddr = (IntPtr)newAddr32;
                }
            }
            else
                p._lastAddr = GetBaseAddrInternal(hProcess, false, p._moduleName, EnumModulesFilterFlag.X86);
            //不使用EnumModulesFilterFlag.ALL是因为无法识别到底是32位模块还是64位模块
            //获取32位模块基址失败
            if (p._lastAddr == IntPtr.Zero)
                if (p._type == PointerType.Address_Offset)
                    return false;
                else
                    goto x64;
            else if (p._type == PointerType.Address_Offset)
                return true;
            //获取基础地址失败
            p._lastAddr += p._moduleOffset;
            //加上模块偏移
            if (p._offset != null)
            {
                //有偏移
                for (int i = 0; i < p._offset.Length; i++)
                {
                    //处理每个偏移
                    if (!ReadInt32Internal(hProcess, p._lastAddr, out newAddr32))
                        //读取新地址失败
                        return false;
                    p._lastAddr = (IntPtr)(newAddr32 + p._offset[i]);
                    //生成新地址
                }
            }
            return true;
            x64:
            p._lastAddr = GetBaseAddrInternal(hProcess, false, p._moduleName, EnumModulesFilterFlag.X64);
            if (p._lastAddr == IntPtr.Zero)
                //获取64位模块基址失败
                return false;
            p._lastAddr += p._moduleOffset;
            //加上模块偏移
            if (p._offset != null)
            {
                //有偏移
                for (int i = 0; i < p._offset.Length; i++)
                {
                    //处理每个偏移
                    if (!ReadInt64Internal(hProcess, p._lastAddr, out newAddr64))
                        //读取新地址失败
                        return false;
                    p._lastAddr = (IntPtr)(newAddr64 + p._offset[i]);
                    //生成新地址
                }
            }
            return true;
        }
        #endregion

        #region 读写模板
        /// <summary>
        /// 内存读取模板回调函数
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private delegate bool TemplateOfReadCallback<TValue>(IntPtr hProcess, IntPtr addr, out TValue value);

        /// <summary>
        /// 内存读取模板回调函数
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        private delegate bool TemplateOfReadRefCallback(IntPtr hProcess, IntPtr addr);

        /// <summary>
        /// 内存写入模板回调函数
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        private delegate bool TemplateOfWriteCallback(IntPtr hProcess, IntPtr addr);

        /// <summary>
        /// 内存读取模板
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfReadOut<TValue>(uint processId, IntPtr addr, out TValue value, TemplateOfReadCallback<TValue> reader)
        {
            IntPtr hProcess;
            bool is64;

            value = default(TValue);
            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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
        /// 内存读取模板
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfReadOut<TValue>(uint processId, Pointer p, out TValue value, TemplateOfReadCallback<TValue> reader)
        {
            IntPtr hProcess;
            bool is64;

            value = default(TValue);
            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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
        /// 内存读取模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfRead(uint processId, IntPtr addr, TemplateOfReadRefCallback reader)
        {
            IntPtr hProcess;
            bool is64;

            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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
        /// 内存读取模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        private static bool TemplateOfRead(uint processId, Pointer p, TemplateOfReadRefCallback reader)
        {
            IntPtr hProcess;
            bool is64;

            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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
        /// 内存写入模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="writer">写入器</param>
        /// <returns></returns>
        private static bool TemplateOfWrite(uint processId, IntPtr addr, TemplateOfWriteCallback writer)
        {
            IntPtr hProcess;
            bool is64;

            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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
        /// 内存写入模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="writer">写入器</param>
        /// <returns></returns>
        private static bool TemplateOfWrite(uint processId, Pointer p, TemplateOfWriteCallback writer)
        {
            IntPtr hProcess;
            bool is64;

            hProcess = OpenProcessRWQuery(processId);
            //读写查询权限打开进程
            if (hProcess == IntPtr.Zero)
                return false;
            if (!Process.Is64ProcessInternal(hProcess, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
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

            return TemplateOfRead(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => ReadBytesInternal(hProcess, addrCallback, value));
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

            return TemplateOfRead(processId, p, (IntPtr hProcess, IntPtr addrCallback) => ReadBytesInternal(hProcess, addrCallback, value));
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
            MEMORY_BASIC_INFORMATION pageInfo;

            value = null;
            if (!VirtualQueryEx(hProcess, addr, out pageInfo, MEMORY_BASIC_INFORMATION.Size))
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out byte buffer) => ReadByteInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out byte buffer) => ReadByteInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out char buffer) => ReadCharInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out char buffer) => ReadCharInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out int buffer) => ReadInt32Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out int buffer) => ReadInt32Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out long buffer) => ReadInt64Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out long buffer) => ReadInt64Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out float buffer) => ReadFloatInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out float buffer) => ReadFloatInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(hProcess, addrCallback, out buffer));
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
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(hProcess, addrCallback, out buffer));
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

        #region 读写指针
        #region 读取指针
        /// <summary>
        /// 读取指针
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadIntPtr(uint processId, IntPtr addr, out IntPtr value)
        {
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out IntPtr buffer) => ReadIntPtrInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取指针
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadIntPtr(uint processId, Pointer p, out IntPtr value)
        {
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out IntPtr buffer) => ReadIntPtrInternal(hProcess, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取指针
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadIntPtrInternal(IntPtr hProcess, IntPtr addr, out IntPtr value)
        {
            return ReadProcessMemory(hProcess, addr, out value, (uint)IntPtr.Size, null);
        }
        #endregion

        #region 写入指针
        /// <summary>
        /// 写入指针
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteIntPtr(uint processId, IntPtr addr, IntPtr value)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteIntPtrInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入指针
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteIntPtr(uint processId, Pointer p, IntPtr value)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteIntPtrInternal(hProcess, addrCallback, value));
        }

        /// <summary>
        /// 写入指针
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteIntPtrInternal(IntPtr hProcess, IntPtr addr, IntPtr value)
        {
            return WriteProcessMemory(hProcess, addr, ref value, (uint)IntPtr.Size, null);
        }
        #endregion
        #endregion

        #region 读写字符串
        #region 读取字符串
        /// <summary>
        /// 读取字符串，使用UTF16编码，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        public static bool ReadString(uint processId, IntPtr addr, string value, bool doubleZero)
        {
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out string buffer) => ReadStringInternal(hProcess, addrCallback, out buffer, 0x1000, doubleZero, Encoding.Unicode));
        }

        /// <summary>
        /// 读取字符串，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool ReadString(uint processId, IntPtr addr, string value, bool doubleZero, Encoding encoding)
        {
            return TemplateOfReadOut(processId, addr, out value, (IntPtr hProcess, IntPtr addrCallback, out string buffer) => ReadStringInternal(hProcess, addrCallback, out buffer, 0x1000, doubleZero, encoding));
        }

        /// <summary>
        /// 读取字符串，使用UTF16编码，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        public static bool ReadString(uint processId, Pointer p, string value, bool doubleZero)
        {
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out string buffer) => ReadStringInternal(hProcess, addrCallback, out buffer, 0x1000, doubleZero, Encoding.Unicode));
        }

        /// <summary>
        /// 读取字符串，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool ReadString(uint processId, Pointer p, out string value, bool doubleZero, Encoding encoding)
        {
            return TemplateOfReadOut(processId, p, out value, (IntPtr hProcess, IntPtr addrCallback, out string buffer) => ReadStringInternal(hProcess, addrCallback, out buffer, 0x1000, doubleZero, encoding));
        }

        /// <summary>
        /// 读取字符串，使用UTF16编码，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="bufferSize">缓存大小</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        internal static bool ReadStringInternal(IntPtr hProcess, IntPtr addr, out string value, int bufferSize, bool doubleZero)
        {
            return ReadStringInternal(hProcess, addr, out value, bufferSize, doubleZero, Encoding.Unicode);
        }

        /// <summary>
        /// 读取字符串，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="bufferSize">缓存大小</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static unsafe bool ReadStringInternal(IntPtr hProcess, IntPtr addr, out string value, int bufferSize, bool doubleZero, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding) + "不能为null");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize) + "小于等于0");

            byte[] buffer;
            uint numberOfBytesRead;
            List<byte> bufferList;
            bool lastByteIsZero;

            lastByteIsZero = false;
            bufferList = new List<byte>(bufferSize);
            while (true)
            {
                buffer = new byte[bufferSize];
                ReadProcessMemory(hProcess, addr, buffer, (uint)bufferSize, &numberOfBytesRead);
                //读取到缓存
                if ((int)numberOfBytesRead == bufferSize)
                {
                    //读取完整
                    for (int i = 0; i < bufferSize; i++)
                    {
                        if (buffer[i] == 0)
                        {
                            //出现\0
                            if (doubleZero)
                            {
                                //如果双\0结尾
                                if (lastByteIsZero)
                                    //上一个字节为\0
                                    goto addLastRange;
                                if (i + 1 != bufferSize)
                                {
                                    //不是缓存区最后一个字节
                                    if (buffer[i + 1] == 0)
                                        //下一个字节也为\0
                                        goto addLastRange;
                                }
                                else
                                    //缓存读完，标记上一个字节为\0
                                    lastByteIsZero = true;
                            }
                            else
                                //不是2个\0结尾，直接跳出
                                goto addLastRange;
                        }
                        else
                        {
                            if (lastByteIsZero)
                                //上一个字节为\0，但当前字节不是
                                lastByteIsZero = false;
                        }
                    }
                }
                else if ((int)numberOfBytesRead == 0)
                {
                    //读取失败
                    value = null;
                    return false;
                }
                else
                {
                    //读取不完整
                    for (int i = 0; i < (int)numberOfBytesRead; i++)
                    {
                        if (buffer[i] == 0)
                        {
                            //出现\0
                            if (doubleZero)
                            {
                                //如果双\0结尾
                                if (lastByteIsZero)
                                    //上一个字节为\0
                                    goto addLastRange;
                                if (i + 1 != (int)numberOfBytesRead && buffer[i + 1] == 0)
                                    //不是缓存区最后一个字节且下一个字节也为\0
                                    goto addLastRange;
                            }
                            else
                                //不是2个\0结尾，直接跳出
                                goto addLastRange;
                        }
                        else
                        {
                            if (lastByteIsZero)
                                //上一个字节为\0，但当前字节不是
                                lastByteIsZero = false;
                        }
                    }
                }
                bufferList.AddRange(buffer);
            };
            addLastRange:
            numberOfBytesRead -= doubleZero ? 2u : 1u;
            for (int i = 0; i < (int)numberOfBytesRead; i++)
                bufferList.Add(buffer[i]);
            if (encoding.CodePage == Encoding.Unicode.CodePage)
                buffer = bufferList.ToArray();
            else
                buffer = Encoding.Convert(encoding, Encoding.Unicode, bufferList.ToArray());
            fixed (void* p = &buffer[0])
                value = new string((char*)p);
            return true;
        }
        #endregion

        #region 写入字符串
        /// <summary>
        /// 写入字符串，使用UTF16编码，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, IntPtr addr, string value, bool doubleZero)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteStringInternal(hProcess, addrCallback, value, doubleZero));
        }

        /// <summary>
        /// 写入字符串，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, IntPtr addr, string value, bool doubleZero, Encoding encoding)
        {
            return TemplateOfWrite(processId, addr, (IntPtr hProcess, IntPtr addrCallback) => WriteStringInternal(hProcess, addrCallback, value, doubleZero, encoding));
        }

        /// <summary>
        /// 写入字符串，使用UTF16编码，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, Pointer p, string value, bool doubleZero)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteStringInternal(hProcess, addrCallback, value, doubleZero));
        }

        /// <summary>
        /// 写入字符串，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, Pointer p, string value, bool doubleZero, Encoding encoding)
        {
            return TemplateOfWrite(processId, p, (IntPtr hProcess, IntPtr addrCallback) => WriteStringInternal(hProcess, addrCallback, value, doubleZero, encoding));
        }

        /// <summary>
        /// 写入字符串，使用UTF16编码，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        internal static unsafe bool WriteStringInternal(IntPtr hProcess, IntPtr addr, string value, bool doubleZero)
        {
            if (value == null)
                throw new ArgumentNullException();

            value += "\0";
            return WriteProcessMemory(hProcess, addr, value, (uint)value.Length * 2, null);
        }

        /// <summary>
        /// 写入字符串，如果写入到非托管进程中，并且写入为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="WriteBytes(uint, Pointer, byte[])"/>，<see cref="WriteBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static unsafe bool WriteStringInternal(IntPtr hProcess, IntPtr addr, string value, bool doubleZero, Encoding encoding)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (encoding == null)
                throw new ArgumentNullException();

            byte[] buffer;

            value += "\0";
            if (encoding.CodePage == Encoding.Unicode.CodePage)
                return WriteProcessMemory(hProcess, addr, value, (uint)value.Length * 2, null);
            buffer = Encoding.Convert(Encoding.Unicode, encoding, Encoding.Unicode.GetBytes(value));
            return WriteProcessMemory(hProcess, addr, buffer, (uint)buffer.Length, null);
        }
        #endregion
        #endregion
    }
}
