using System;
using System.Runtime.CompilerServices;
using FastWin32.Control;
using static FastWin32.Macro.Extension;
using static FastWin32.NativeMethods;

namespace FastWin32.Macro
{
    /// <summary>
    /// CommCtrl.h
    /// </summary>
    public static class CommCtrl
    {
        #region SysListView32
        /// <summary>
        /// 获取列表视图控件中Item数量
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ListView_GetItemCount(IntPtr hWnd)
        {
            return (int)SendMessage(hWnd, LVM_GETITEMCOUNT, 0, 0);
        }

        /// <summary>
        /// 删除列表视图控件中指定Item
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">指定Item的索引</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ListView_DeleteItem(IntPtr hWnd, int i)
        {
            return SendMessage(hWnd, LVM_DELETEITEM, checked((uint)i), 0) != 0;
        }

        /// <summary>
        /// 设置列表视图控件中Item位置
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">第i个Item</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ListView_SetItemPosition(IntPtr hWnd, int i, int x, int y)
        {
            return SendMessage(hWnd, LVM_SETITEMPOSITION, checked((uint)i), CombineXY(x, y)) != 0;
        }

        /// <summary>
        /// 获取列表视图控件中Item位置
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">第i个Item</param>
        /// <param name="addr">Point结构在列表视图控件所在进程中的地址</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ListView_GetItemPosition(IntPtr hWnd, int i, IntPtr addr)
        {
            return SendMessage(hWnd, LVM_GETITEMPOSITION, checked((uint)i), (uint)addr) != 0;
        }

        /// <summary>
        /// 重绘列表视图控件中指定Item
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="iFirst">索引起始</param>
        /// <param name="iLast">索引结束</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ListView_RedrawItems(IntPtr hWnd, int iFirst, int iLast)
        {
            return SendMessage(hWnd, LVM_REDRAWITEMS, checked((uint)iFirst), checked((uint)iLast)) != 0;
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="dwExStyle">扩展样式</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ListView_SetExtendedListViewStyle(IntPtr hWnd, ExtendedListViewStyles dwExStyle)
        {
            SendMessage(hWnd, LVM_SETEXTENDEDLISTVIEWSTYLE, 0, (uint)dwExStyle);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="dwExMask">（Google翻译，懒得自己翻译了）指定dwExStyle中哪些样式将受到影响的DWORD值。此参数可以是扩展列表视图控件样式的组合。只有dwExMask中的扩展样式将被更改。所有其他样式将保持原样。如果此参数为零，则dwExStyle中的所有样式将受到影响。</param>
        /// <param name="dwExStyle">扩展样式</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ListView_SetExtendedListViewStyleEx(IntPtr hWnd, ExtendedListViewStyles dwExMask, ExtendedListViewStyles dwExStyle)
        {
            SendMessage(hWnd, LVM_SETEXTENDEDLISTVIEWSTYLE, (uint)dwExMask, (uint)dwExStyle);
        }

        /// <summary>
        /// 获取列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExtendedListViewStyles ListView_GetExtendedListViewStyle(IntPtr hWnd)
        {
            return (ExtendedListViewStyles)SendMessage(hWnd, LVM_GETEXTENDEDLISTVIEWSTYLE, 0, 0);
        }
        #endregion

        #region SysDateTimePick32
        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="lpSysTime">指向SYSTEMTIME结构的指针</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DateTime_GetSystemtime(IntPtr hWnd, IntPtr lpSysTime)
        {
            return SendMessage(hWnd, DTM_GETSYSTEMTIME, 0, (uint)lpSysTime) != GDT_ERROR;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="flag"></param>
        /// <param name="lpSysTime">指向SYSTEMTIME结构的指针</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DateTime_SetSystemtime(IntPtr hWnd, uint flag, IntPtr lpSysTime)
        {
            return SendMessage(hWnd, DTM_SETSYSTEMTIME, flag, (uint)lpSysTime) != 0;
        }
        #endregion
    }
}
