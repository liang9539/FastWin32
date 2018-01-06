using System;
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
        /// 删除列表视图控件中指定Item
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">指定Item的索引</param>
        /// <returns></returns>
        public static bool ListView_DeleteItem(IntPtr hWnd, int i)
        {
            return SendMessage(hWnd, LVM_DELETEITEM, (IntPtr)i, IntPtr.Zero) != 0;
        }

        /// <summary>
        /// 获取列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        public static ExtendedListViewStyles ListView_GetExtendedListViewStyle(IntPtr hWnd)
        {
            return (ExtendedListViewStyles)SendMessage(hWnd, LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// 获取LVITEM
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="addr">LVITEM结构在列表视图控件所在进程中的地址</param>
        /// <returns></returns>
        public static bool ListView_GetItem(IntPtr hWnd, IntPtr addr)
        {
            return SendMessage(hWnd, LVM_GETITEM, IntPtr.Zero, addr) != 0;
        }

        /// <summary>
        /// 获取列表视图控件中Item数量
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        public static int ListView_GetItemCount(IntPtr hWnd)
        {
            return (int)SendMessage(hWnd, LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// 获取列表视图控件中Item位置
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">第i个Item</param>
        /// <param name="addr">Point结构在列表视图控件所在进程中的地址</param>
        /// <returns></returns>
        public static bool ListView_GetItemPosition(IntPtr hWnd, int i, IntPtr addr)
        {
            return SendMessage(hWnd, LVM_GETITEMPOSITION, (IntPtr)i, addr) != 0;
        }

        /// <summary>
        /// 获取指定Item的文本，返回文本长度
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="i"></param>
        /// <param name="addr">远程进程的LVITEM地址</param>
        /// <param name="cchTextMax"></param>
        /// <returns></returns>
        public static int ListView_GetItemText(IntPtr hWnd, int i, IntPtr addr, int cchTextMax)
        {
            return (int)SendMessage(hWnd, LVM_GETITEMTEXT, (IntPtr)i, addr);
        }

        /// <summary>
        /// 重绘列表视图控件中指定Item
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="iFirst">索引起始</param>
        /// <param name="iLast">索引结束</param>
        /// <returns></returns>
        public static bool ListView_RedrawItems(IntPtr hWnd, int iFirst, int iLast)
        {
            return SendMessage(hWnd, LVM_REDRAWITEMS, (IntPtr)iFirst, (IntPtr)iLast) != 0;
        }

        /// <summary>
        /// 设置列表视图控件中Item位置
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="i">第i个Item</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        public static bool ListView_SetItemPosition(IntPtr hWnd, int i, int x, int y)
        {
            return SendMessage(hWnd, LVM_SETITEMPOSITION, (IntPtr)i, (IntPtr)CombineXY(x, y)) != 0;
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="dwExStyle">扩展样式</param>
        public static void ListView_SetExtendedListViewStyle(IntPtr hWnd, ExtendedListViewStyles dwExStyle)
        {
            SendMessage(hWnd, LVM_SETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, (IntPtr)dwExStyle);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="dwExMask">（Google翻译，懒得自己翻译了）指定dwExStyle中哪些样式将受到影响的DWORD值。此参数可以是扩展列表视图控件样式的组合。只有dwExMask中的扩展样式将被更改。所有其他样式将保持原样。如果此参数为零，则dwExStyle中的所有样式将受到影响。</param>
        /// <param name="dwExStyle">扩展样式</param>
        public static void ListView_SetExtendedListViewStyleEx(IntPtr hWnd, ExtendedListViewStyles dwExMask, ExtendedListViewStyles dwExStyle)
        {
            SendMessage(hWnd, LVM_SETEXTENDEDLISTVIEWSTYLE, (IntPtr)dwExMask, (IntPtr)dwExStyle);
        }
        #endregion

        #region SysDateTimePick32
        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="lpSysTime">指向SYSTEMTIME结构的指针</param>
        /// <returns></returns>
        public static bool DateTime_GetSystemtime(IntPtr hWnd, IntPtr lpSysTime)
        {
            return SendMessage(hWnd, DTM_GETSYSTEMTIME, IntPtr.Zero, lpSysTime) != GDT_ERROR;
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="flag"></param>
        /// <param name="lpSysTime">指向SYSTEMTIME结构的指针</param>
        /// <returns></returns>
        public static bool DateTime_SetSystemtime(IntPtr hWnd, uint flag, IntPtr lpSysTime)
        {
            return SendMessage(hWnd, DTM_SETSYSTEMTIME, (IntPtr)flag, lpSysTime) != 0;
        }
        #endregion
    }
}
