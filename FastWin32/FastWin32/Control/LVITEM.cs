using System;
using System.Runtime.InteropServices;

namespace FastWin32.Control
{
    /// <summary>
    /// Specifies or receives the attributes of a list-view item.
    /// This structure has been updated to support a new mask value (LVIF_INDENT) that enables item indenting.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct LVITEM : IWin32ControlStruct
    {
        /// <summary>
        /// 
        /// </summary>
        public uint mask;

        /// <summary>
        /// 
        /// </summary>
        public int iItem;

        /// <summary>
        /// 
        /// </summary>
        public int iSubItem;

        /// <summary>
        /// 
        /// </summary>
        public uint state;

        /// <summary>
        /// 
        /// </summary>
        public uint stateMask;

        /// <summary>
        /// 
        /// </summary>
        public char* pszText;

        /// <summary>
        /// 
        /// </summary>
        public int cchTextMax;

        /// <summary>
        /// 
        /// </summary>
        public int iImage;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr lParam;

        /// <summary>
        /// 
        /// </summary>
        public int iIndent;

        /// <summary>
        /// 
        /// </summary>
        public int iGroupId;

        /// <summary>
        /// 
        /// </summary>
        public uint cColumns;

        /// <summary>
        /// 
        /// </summary>
        public uint* puColumns;

        /// <summary>
        /// 
        /// </summary>
        public int* piColFmt;

        /// <summary>
        /// 
        /// </summary>
        public int iGroup;

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        public static readonly uint Size = (uint)Marshal.SizeOf(typeof(LVITEM));

        /// <summary>
        /// 结构体在非托管内存中的大小
        /// </summary>
        uint IWin32ControlStruct.Size => Size;

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get => new string(pszText);
            set => pszText = (char*)Marshal.StringToHGlobalUni(value);
        }

        /// <summary>
        /// 获取指向当前对象的指针
        /// </summary>
        /// <returns></returns>
        public unsafe void* ToPointer()
        {
            fixed (void* p = &this)
                return p;
        }
    }
}
