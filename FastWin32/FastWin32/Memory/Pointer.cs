using System;

namespace FastWin32.Memory
{
    /// <summary>
    /// 指针
    /// </summary>
    public class Pointer
    {
        private string _mName;
        private int _mOffset;
        private int[] _offset;
        internal IntPtr _lastAddr;

        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName => _mName;

        /// <summary>
        /// 模块偏移
        /// </summary>
        public int ModuleOffset => _mOffset;

        /// <summary>
        /// 偏移
        /// </summary>
        public int[] Offset => _offset;

        /// <summary>
        /// 禁止无参构造函数
        /// </summary>
        private Pointer() { }

        /// <summary>
        /// 实例化指针结构
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="moduleOffset"></param>
        /// <param name="offset"></param>
        public Pointer(string moduleName, int moduleOffset, int[] offset)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentException();

            _mName = moduleName;
            _mOffset = moduleOffset;
            _offset = offset;
            _lastAddr = IntPtr.Zero;
        }
    }
}
