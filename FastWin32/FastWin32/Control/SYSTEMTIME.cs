using System;
using System.Runtime.InteropServices;

namespace FastWin32.Control
{
    /// <summary>
    /// 日期
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME : IWin32ControlStruct
    {
        /// <summary>
        /// 年
        /// </summary>
        public ushort wYear;

        /// <summary>
        /// 月
        /// </summary>
        public ushort wMonth;

        /// <summary>
        /// 星期，0=星期日，1=星期一...
        /// </summary>
        public ushort wDayOfWeek;

        /// <summary>
        /// 日
        /// </summary>
        public ushort wDay;

        /// <summary>
        /// 时
        /// </summary>
        public ushort wHour;

        /// <summary>
        /// 分
        /// </summary>
        public ushort wMinute;

        /// <summary>
        /// 秒
        /// </summary>
        public ushort wSecond;

        /// <summary>
        /// 毫秒
        /// </summary>
        public ushort wMilliseconds;

        /// <summary>
        /// 将DateTime转换为SYSTEMTIME
        /// </summary>
        /// <param name="dateTime">日期</param>
        public SYSTEMTIME(DateTime dateTime)
        {
            wYear = (ushort)dateTime.Year;
            wMonth = (ushort)dateTime.Month;
            wDayOfWeek = (ushort)dateTime.DayOfWeek;
            wDay = (ushort)dateTime.Day;
            wHour = (ushort)dateTime.Hour;
            wMinute = (ushort)dateTime.Minute;
            wSecond = (ushort)dateTime.Second;
            wMilliseconds = (ushort)dateTime.Millisecond;
        }

        /// <summary>
        /// 将DateTime对象转换为SYSTEMTIME对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator SYSTEMTIME(DateTime value)
        {
            return new SYSTEMTIME(value);
        }

        /// <summary>
        /// 将SYSTEMTIME对象转换为DateTime对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator DateTime(SYSTEMTIME value)
        {
            return new DateTime
            (
                value.wYear,
                value.wMonth,
                value.wDay,
                value.wHour,
                value.wMinute,
                value.wSecond,
                value.wMilliseconds
            );
        }

        /// <summary>
        /// 最小日期
        /// </summary>
        public static readonly SYSTEMTIME MinValue = (SYSTEMTIME)DateTime.MinValue;

        /// <summary>
        /// 最大日期
        /// </summary>
        public static readonly SYSTEMTIME MaxValue = (SYSTEMTIME)DateTime.MaxValue;

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        public static readonly uint Size = (uint)Marshal.SizeOf(typeof(SYSTEMTIME));

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        uint IWin32ControlStruct.Size => Size;

        /// <summary>
        /// 获取一个SYSTEMTIME对象，该对象设置为此计算机上的当前日期和时间，表示为本地时间。
        /// </summary>
        public static SYSTEMTIME Now => new SYSTEMTIME(DateTime.Now);

        /// <summary>
        /// 获取一个SYSTEMTIME对象，该对象设置为此计算机上的当前日期和时间，表示为协调通用时间 (UTC)。
        /// </summary>
        public static SYSTEMTIME UtcNow => new SYSTEMTIME(DateTime.UtcNow);

        /// <summary>
        /// 获取指向当前对象的指针
        /// </summary>
        /// <returns></returns>
        public unsafe void* ToPointer()
        {
            fixed (void* p = &this)
            {
                return p;
            }
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ((DateTime)this).Equals(obj);
        }

        /// <summary>
        /// 返回此实例的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ((DateTime)this).GetHashCode();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ((DateTime)this).ToString();
        }
    }
}
