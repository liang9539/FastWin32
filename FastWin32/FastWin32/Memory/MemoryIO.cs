using System;
using System.Collections.Generic;
using System.Text;
using FastWin32.Diagnostics;
using static FastWin32.NativeMethods;

namespace FastWin32.Memory
{
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
    /// 内存读写
    /// </summary>
    public static class MemoryIO
    {
        /// <summary>
        /// 打开进程（内存读写+查询）
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns></returns>
        private static IntPtr OpenProcessVMReadWriteQuery(uint processId)
        {
            return OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, processId);
        }

        /// <summary>
        /// 获取指针指向的地址
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="p">指针</param>
        /// <returns></returns>
        internal static bool GetPointerAddrInternal(IntPtr processHandle, Pointer p)
        {
            bool is64;
            int newAddr32 = 0;
            long newAddr64 = 0;

            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (p._type == PointerType.Address_Offset)
            {
                if (is64)
                {
                    if (!ReadInt64Internal(processHandle, p._baseAddr, out newAddr64))
                        return false;
                    p._lastAddr = (IntPtr)newAddr64;
                }
                else
                {
                    if (!ReadInt32Internal(processHandle, p._baseAddr, out newAddr32))
                        return false;
                    p._lastAddr = (IntPtr)newAddr32;
                }
            }
            else
                p._lastAddr = Module32.GetHandleInternal(processHandle, false, p._moduleName);
            if (p._lastAddr == IntPtr.Zero)
                return false;
            p._lastAddr += p._moduleOffset;
            if (p._offset == null)
                return true;
            if (is64)
                for (int i = 0; i < p._offset.Length; i++)
                {
                    if (!ReadInt64Internal(processHandle, p._lastAddr, out newAddr64))
                        return false;
                    p._lastAddr = (IntPtr)(newAddr64 + p._offset[i]);
                }
            else
                for (int i = 0; i < p._offset.Length; i++)
                {
                    if (!ReadInt32Internal(processHandle, p._lastAddr, out newAddr32))
                        return false;
                    p._lastAddr = (IntPtr)(newAddr32 + p._offset[i]);
                }
            return true;
        }

        #region 读写模板
        /// <summary>
        /// 内存读写模板回调函数
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <returns></returns>
        private delegate bool IOTemplateCallback(IntPtr processHandle, IntPtr addr);

        /// <summary>
        /// 内存读写模板回调函数
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private delegate bool IOTemplateCallback<TValue>(IntPtr processHandle, IntPtr addr, out TValue value);

        /// <summary>
        /// 内存读写模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="callback">读写器</param>
        /// <returns></returns>
        private static bool IOTemplate(uint processId, IntPtr addr, IOTemplateCallback callback)
        {
            IntPtr processHandle;
            bool is64;

            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                return callback(processHandle, addr);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 内存读写模板
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="callback">读写器</param>
        /// <returns></returns>
        private static bool IOTemplate(uint processId, Pointer p, IOTemplateCallback callback)
        {
            IntPtr processHandle;
            bool is64;

            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                if (!GetPointerAddrInternal(processHandle, p))
                    return false;
                return callback(processHandle, p._lastAddr);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 内存读取模板
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="callback">读写器</param>
        /// <returns></returns>
        private static bool IOTemplate<TValue>(uint processId, IntPtr addr, out TValue value, IOTemplateCallback<TValue> callback)
        {
            IntPtr processHandle;
            bool is64;

            value = default(TValue);
            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                return callback(processHandle, addr, out value);
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// 内存读写模板
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="callback">读写器</param>
        /// <returns></returns>
        private static bool IOTemplate<TValue>(uint processId, Pointer p, out TValue value, IOTemplateCallback<TValue> callback)
        {
            IntPtr processHandle;
            bool is64;

            value = default(TValue);
            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            if (!Process32.Is64ProcessInternal(processHandle, out is64))
                return false;
            if (is64 && !Environment.Is64BitProcess)
                throw new NotSupportedException("目标进程为64位但当前进程为32位");
            try
            {
                if (!GetPointerAddrInternal(processHandle, p))
                {
                    value = default(TValue);
                    return false;
                }
                return callback(processHandle, p._lastAddr, out value);
            }
            finally
            {
                CloseHandle(processHandle);
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
            if (value == null || value.Length == 0)
                throw new ArgumentNullException();

            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => ReadBytesInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="numOfRead">实际读取的字节数</param>
        /// <returns></returns>
        public static bool ReadBytes(uint processId, IntPtr addr, byte[] value, out uint numOfRead)
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException();

            return IOTemplate(processId, addr, out numOfRead, (IntPtr processHandle, IntPtr addrCallback, out uint numOfReadCallback) => ReadBytesInternal(processHandle, addrCallback, value, out numOfReadCallback));
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
            if (value == null || value.Length == 0)
                throw new ArgumentNullException();

            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => ReadBytesInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="numOfRead">实际读取的字节数</param>
        /// <returns></returns>
        public static bool ReadBytes(uint processId, Pointer p, byte[] value, out uint numOfRead)
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException();

            return IOTemplate(processId, p, out numOfRead, (IntPtr processHandle, IntPtr addrCallback, out uint numOfReadCallback) => ReadBytesInternal(processHandle, addrCallback, value, out numOfReadCallback));
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadBytesInternal(IntPtr processHandle, IntPtr addr, byte[] value)
        {
            return ReadProcessMemory(processHandle, addr, value, (uint)value.Length, null);
        }

        /// <summary>
        /// 读取字节数组，读取的长度由value的长度决定
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="numOfRead">实际读取的字节数</param>
        /// <returns></returns>
        internal static unsafe bool ReadBytesInternal(IntPtr processHandle, IntPtr addr, byte[] value, out uint numOfRead)
        {
            return ReadProcessMemory(processHandle, addr, value, (uint)value.Length, out numOfRead);
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

            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteBytesInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="numOfWritten">实际写入的字节数</param>
        /// <returns></returns>
        public static bool WriteBytes(uint processId, IntPtr addr, byte[] value, out uint numOfWritten)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return IOTemplate(processId, addr, out numOfWritten, (IntPtr processHandle, IntPtr addrCallback, out uint numOfWrittenCallback) => WriteBytesInternal(processHandle, addrCallback, value, out numOfWrittenCallback));
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

            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteBytesInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="numOfWritten">实际写入的字节数</param>
        /// <returns></returns>
        public static bool WriteBytes(uint processId, Pointer p, byte[] value, out uint numOfWritten)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length == 0)
                throw new ArgumentException();

            return IOTemplate(processId, p, out numOfWritten, (IntPtr processHandle, IntPtr addrCallback, out uint numOfWrittenCallback) => WriteBytesInternal(processHandle, addrCallback, value, out numOfWrittenCallback));
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteBytesInternal(IntPtr processHandle, IntPtr addr, byte[] value)
        {
            return WriteProcessMemory(processHandle, addr, value, (uint)value.Length, null);
        }

        /// <summary>
        /// 写入字节数组，写入的长度由value的长度决定
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="numOfWritten">实际写入的字节数</param>
        /// <returns></returns>
        internal static unsafe bool WriteBytesInternal(IntPtr processHandle, IntPtr addr, byte[] value, out uint numOfWritten)
        {
            return WriteProcessMemory(processHandle, addr, value, (uint)value.Length, out numOfWritten);
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
            IntPtr processHandle;

            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
            {
                value = null;
                return false;
            }
            return ReadPageInternal(processHandle, addr, out value, mode);
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
            IntPtr processHandle;

            value = null;
            processHandle = OpenProcessVMReadWriteQuery(processId);
            if (processHandle == IntPtr.Zero)
                return false;
            if (!GetPointerAddrInternal(processHandle, p))
                return false;
            return ReadPageInternal(processHandle, p._lastAddr, out value, mode);
        }

        /// <summary>
        /// 读取地址所在内存页面，读取长度由页面大小以及mode决定决定
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="mode">读取模式</param>
        /// <returns></returns>
        internal static unsafe bool ReadPageInternal(IntPtr processHandle, IntPtr addr, out byte[] value, ReadPageMode mode)
        {
            MEMORY_BASIC_INFORMATION pageInfo;

            value = null;
            if (!VirtualQueryEx(processHandle, addr, out pageInfo, MEMORY_BASIC_INFORMATION.Size))
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
            return ReadProcessMemory(processHandle, addr, value, (uint)value.Length, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out byte buffer) => ReadByteInternal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out byte buffer) => ReadByteInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadByteInternal(IntPtr processHandle, IntPtr addr, out byte value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 1, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteByteInternal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteByteInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteByteInternal(IntPtr processHandle, IntPtr addr, byte value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 1, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out bool buffer) => ReadBooleanInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadBooleanInternal(IntPtr processHandle, IntPtr addr, out bool value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 1, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteBooleanInternal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteBooleanInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteBooleanInternal(IntPtr processHandle, IntPtr addr, bool value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 1, null);
        }
        #endregion
        #endregion

        #region 读写字符
        #region 读取字符
        /// <summary>
        /// 读取字符
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadChar(uint processId, IntPtr addr, out char value)
        {
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out char buffer) => ReadCharInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取字符
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadChar(uint processId, Pointer p, out char value)
        {
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out char buffer) => ReadCharInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取字符
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadCharInternal(IntPtr processHandle, IntPtr addr, out char value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 2, null);
        }
        #endregion

        #region 写入字符
        /// <summary>
        /// 写入字符
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteChar(uint processId, IntPtr addr, char value)
        {
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteCharInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字符
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteChar(uint processId, Pointer p, char value)
        {
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteCharInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字符
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteCharInternal(IntPtr processHandle, IntPtr addr, char value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 2, null);
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
        public static bool ReadInt16(uint processId, IntPtr addr, out short value)
        {
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out short buffer) => ReadInt16Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool ReadInt16(uint processId, Pointer p, out short value)
        {
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out short buffer) => ReadInt16Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取短整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadInt16Internal(IntPtr processHandle, IntPtr addr, out short value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 2, null);
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
        public static bool WriteInt16(uint processId, IntPtr addr, short value)
        {
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteInt16Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入短整形
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteInt16(uint processId, Pointer p, short value)
        {
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteInt16Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入短整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteInt16Internal(IntPtr processHandle, IntPtr addr, short value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 2, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out ushort buffer) => ReadUInt16Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号短整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt16Internal(IntPtr processHandle, IntPtr addr, out ushort value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 2, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt16Internal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt16Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号短整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt16Internal(IntPtr processHandle, IntPtr addr, ushort value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 2, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out int buffer) => ReadInt32Internal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out int buffer) => ReadInt32Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadInt32Internal(IntPtr processHandle, IntPtr addr, out int value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 4, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteInt32Internal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteInt32Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteInt32Internal(IntPtr processHandle, IntPtr addr, int value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 4, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out uint buffer) => ReadUInt32Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt32Internal(IntPtr processHandle, IntPtr addr, out uint value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 4, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt32Internal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt32Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt32Internal(IntPtr processHandle, IntPtr addr, uint value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 4, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out long buffer) => ReadInt64Internal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out long buffer) => ReadInt64Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取长整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadInt64Internal(IntPtr processHandle, IntPtr addr, out long value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 8, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteInt64Internal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteInt64Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入长整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteInt64Internal(IntPtr processHandle, IntPtr addr, long value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 8, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out ulong buffer) => ReadUInt64Internal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取无符号长整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadUInt64Internal(IntPtr processHandle, IntPtr addr, out ulong value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 8, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt64Internal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteUInt64Internal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入无符号长整形
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteUInt64Internal(IntPtr processHandle, IntPtr addr, ulong value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 8, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out float buffer) => ReadFloatInternal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out float buffer) => ReadFloatInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取单精度浮点型
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadFloatInternal(IntPtr processHandle, IntPtr addr, out float value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 4, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteFloatInternal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteFloatInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入单精度浮点型
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteFloatInternal(IntPtr processHandle, IntPtr addr, float value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 4, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out double buffer) => ReadDoubleInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取双精度浮点型
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadDoubleInternal(IntPtr processHandle, IntPtr addr, out double value)
        {
            return ReadProcessMemory(processHandle, addr, out value, 8, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteDoubleInternal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteDoubleInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入双精度浮点型
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteDoubleInternal(IntPtr processHandle, IntPtr addr, double value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, 8, null);
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
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out IntPtr buffer) => ReadIntPtrInternal(processHandle, addrCallback, out buffer));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out IntPtr buffer) => ReadIntPtrInternal(processHandle, addrCallback, out buffer));
        }

        /// <summary>
        /// 读取指针
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool ReadIntPtrInternal(IntPtr processHandle, IntPtr addr, out IntPtr value)
        {
            return ReadProcessMemory(processHandle, addr, out value, (uint)IntPtr.Size, null);
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
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteIntPtrInternal(processHandle, addrCallback, value));
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
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteIntPtrInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入指针
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteIntPtrInternal(IntPtr processHandle, IntPtr addr, IntPtr value)
        {
            return WriteProcessMemory(processHandle, addr, ref value, (uint)IntPtr.Size, null);
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
        public static bool ReadString(uint processId, IntPtr addr, out string value, bool doubleZero)
        {
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out string buffer) => ReadStringInternal(processHandle, addrCallback, out buffer, 0x1000, doubleZero, Encoding.Unicode));
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
        public static bool ReadString(uint processId, IntPtr addr, out string value, bool doubleZero, Encoding encoding)
        {
            return IOTemplate(processId, addr, out value, (IntPtr processHandle, IntPtr addrCallback, out string buffer) => ReadStringInternal(processHandle, addrCallback, out buffer, 0x1000, doubleZero, encoding));
        }

        /// <summary>
        /// 读取字符串，使用UTF16编码，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        public static bool ReadString(uint processId, Pointer p, out string value, bool doubleZero)
        {
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out string buffer) => ReadStringInternal(processHandle, addrCallback, out buffer, 0x1000, doubleZero, Encoding.Unicode));
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
            return IOTemplate(processId, p, out value, (IntPtr processHandle, IntPtr addrCallback, out string buffer) => ReadStringInternal(processHandle, addrCallback, out buffer, 0x1000, doubleZero, encoding));
        }

        /// <summary>
        /// 读取字符串，使用UTF16编码，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="bufferSize">缓存大小</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <returns></returns>
        internal static bool ReadStringInternal(IntPtr processHandle, IntPtr addr, out string value, int bufferSize, bool doubleZero)
        {
            return ReadStringInternal(processHandle, addr, out value, bufferSize, doubleZero, Encoding.Unicode);
        }

        /// <summary>
        /// 读取字符串，如果读取到非托管进程中，并且读取为LPSTR LPWSTTR BSTR等字符串类型，请自行转换为byte[]并使用<see cref="ReadBytes(uint, Pointer, byte[])"/>，<see cref="ReadBytes(uint, IntPtr, byte[])"/>
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="bufferSize">缓存大小</param>
        /// <param name="doubleZero">是否以2个\0结尾（比如LPWSTR以2个字节\0结尾，而LPSTR以1个字节\0结尾）</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static unsafe bool ReadStringInternal(IntPtr processHandle, IntPtr addr, out string value, int bufferSize, bool doubleZero, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding) + "不能为null");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize) + "小于等于0");

            byte[] buffer;
            uint numberOfBytesRead;
            List<byte> bufferList;
            bool lastByteIsZero;

            buffer = null;
            numberOfBytesRead = 0;
            bufferList = new List<byte>(bufferSize);
            lastByteIsZero = false;
            for (int i = 0; i < int.MaxValue; i++)
            {
                buffer = new byte[bufferSize];
                ReadProcessMemory(processHandle, addr + bufferSize * i, buffer, (uint)bufferSize, &numberOfBytesRead);
                //读取到缓存
                if ((int)numberOfBytesRead == bufferSize)
                {
                    //读取完整
                    for (int j = 0; j < bufferSize; j++)
                    {
                        if (buffer[j] == 0)
                        {
                            //出现\0
                            if (doubleZero)
                            {
                                //如果双\0结尾
                                if (lastByteIsZero)
                                    //上一个字节为\0
                                    goto addLastRange;
                                if (j + 1 != bufferSize)
                                {
                                    //不是缓存区最后一个字节
                                    if (buffer[j + 1] == 0)
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
                else if (numberOfBytesRead == 0)
                {
                    //读取失败
                    value = null;
                    return false;
                }
                else
                {
                    //读取不完整
                    for (int j = 0; j < (int)numberOfBytesRead; j++)
                    {
                        if (buffer[j] == 0)
                        {
                            //出现\0
                            if (doubleZero)
                            {
                                //如果双\0结尾
                                if (lastByteIsZero)
                                    //上一个字节为\0
                                    goto addLastRange;
                                if (j + 1 != (int)numberOfBytesRead && buffer[j + 1] == 0)
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
        /// 写入字符串，使用UTF16编码
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, IntPtr addr, string value)
        {
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteStringInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, IntPtr addr, string value, Encoding encoding)
        {
            return IOTemplate(processId, addr, (IntPtr processHandle, IntPtr addrCallback) => WriteStringInternal(processHandle, addrCallback, value, encoding));
        }

        /// <summary>
        /// 写入字符串，使用UTF16编码
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, Pointer p, string value)
        {
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteStringInternal(processHandle, addrCallback, value));
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="p">指针</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static bool WriteString(uint processId, Pointer p, string value, Encoding encoding)
        {
            return IOTemplate(processId, p, (IntPtr processHandle, IntPtr addrCallback) => WriteStringInternal(processHandle, addrCallback, value, encoding));
        }

        /// <summary>
        /// 写入字符串，使用UTF16编码
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal static unsafe bool WriteStringInternal(IntPtr processHandle, IntPtr addr, string value)
        {
            if (value == null)
                throw new ArgumentNullException();

            value += "\0";
            return WriteProcessMemory(processHandle, addr, value, (uint)value.Length * 2, null);
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="processHandle">进程句柄</param>
        /// <param name="addr">地址</param>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        internal static unsafe bool WriteStringInternal(IntPtr processHandle, IntPtr addr, string value, Encoding encoding)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (encoding == null)
                throw new ArgumentNullException();

            byte[] buffer;

            value += "\0";
            if (encoding.CodePage == Encoding.Unicode.CodePage)
                return WriteProcessMemory(processHandle, addr, value, (uint)value.Length * 2, null);
            buffer = Encoding.Convert(Encoding.Unicode, encoding, Encoding.Unicode.GetBytes(value));
            return WriteProcessMemory(processHandle, addr, buffer, (uint)buffer.Length, null);
        }
        #endregion
        #endregion
    }
}
