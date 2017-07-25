using System.Runtime.InteropServices;
using static FastWin32.Macro.Extension;

namespace FastWin32.Control
{
    /// <summary>
    /// 表示一个点的坐标
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IWin32ControlStruct
    {
        /// <summary>
        /// x坐标
        /// </summary>
        public int x;

        /// <summary>
        /// y坐标
        /// </summary>
        public int y;

        /// <summary>
        /// 使用指定(x,y)实例化Point
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 使用System.Drawing.Point对象实例化Point
        /// </summary>
        /// <param name="point"></param>
        public Point(System.Drawing.Point point)
        {
            x = point.X;
            y = point.Y;
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Point(System.Drawing.Point value)
        {
            return new Point(value.X, value.Y);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator System.Drawing.Point(Point value)
        {
            return new System.Drawing.Point(value.x, value.y);
        }

        /// <summary>
        /// 坐标(0, 0)
        /// </summary>
        public static readonly Point Empty;

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        public static readonly uint Size = (uint)Marshal.SizeOf(typeof(Point));

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        uint IWin32ControlStruct.Size => Size;

        /// <summary>
        /// 转换为lParam
        /// </summary>
        /// <returns></returns>
        public uint ToLParam()
        {
            return CombineXY(x, y);
        }

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
            return ((System.Drawing.Point)this).Equals(obj);
        }

        /// <summary>
        /// 返回此实例的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ((System.Drawing.Point)this).GetHashCode();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ((System.Drawing.Point)this).ToString();
        }
    }
}
