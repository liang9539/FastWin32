using System;

namespace FastWin32.Memory
{
    /// <summary>
    /// 指针类型
    /// </summary>
    internal enum PointerType
    {
        /// <summary>
        /// 模块名+偏移
        /// </summary>
        ModuleName_Offset,

        /// <summary>
        /// 地址+偏移
        /// </summary>
        Address_Offset
    }

    /// <summary>
    /// 指针
    /// </summary>
    public sealed class Pointer
    {
        internal string _moduleName;

        internal uint _moduleOffset;

        internal IntPtr _baseAddr;

        internal uint[] _offset;

        internal PointerType _type;

        internal IntPtr _lastAddr;

        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName => _type == PointerType.ModuleName_Offset ? _moduleName : throw new NotSupportedException("使用了地址+偏移，未使用模块名");

        /// <summary>
        /// 模块偏移
        /// </summary>
        public uint ModuleOffset => _type == PointerType.ModuleName_Offset ? _moduleOffset : throw new NotSupportedException("使用了地址+偏移，未使用模块偏移");

        /// <summary>
        /// 基础地址
        /// </summary>
        public IntPtr BaseAddr => _type == PointerType.Address_Offset ? _baseAddr : throw new NotSupportedException("使用了模块偏移，未使用地址+偏移");

        /// <summary>
        /// 偏移
        /// </summary>
        public uint[] Offset => _offset;

        /// <summary>
        /// 实例化指针结构
        /// </summary>
        /// <param name="moduleName">模块名</param>
        /// <param name="moduleOffset">模块偏移</param>
        /// <param name="offset">偏移</param>
        public Pointer(string moduleName, uint moduleOffset, params uint[] offset)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentOutOfRangeException();

            _moduleName = moduleName;
            _moduleOffset = moduleOffset;
            _offset = offset;
            _type = PointerType.ModuleName_Offset;
        }

        /// <summary>
        /// 实例化指针结构
        /// </summary>
        /// <param name="baseAddr">基础地址</param>
        /// <param name="offset">偏移</param>
        public Pointer(IntPtr baseAddr, params uint[] offset)
        {
            _baseAddr = baseAddr;
            _offset = offset;
            _type = PointerType.Address_Offset;
        }
    }
}
