using System;
using FastWin32.Memory;
using static FastWin32.Macro.CommCtrl;
using static FastWin32.NativeMethods;

namespace FastWin32.Control
{
    /// <summary>
    /// 列表视图控件
    /// </summary>
    public class SysListView32 : Win32Control
    {
        /// <summary>
        /// 使用已有SysListView32控件实例化SysListView32类
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        public SysListView32(IntPtr hWnd) : base(hWnd) { }

        /// <summary>
        /// 删除列表视图控件中指定Item
        /// </summary>
        /// <param name="i">指定Item的索引</param>
        /// <returns></returns>
        public bool DeleteItem(int i)
        {
            return ListView_DeleteItem(Handle, i);
        }

        /// <summary>
        /// 获取列表视图控件扩展样式
        /// </summary>
        /// <returns></returns>
        public ExtendedListViewStyles GetExtendedListViewStyle()
        {
            return ListView_GetExtendedListViewStyle(Handle);
        }

        /// <summary>
        /// 获取LVITEM
        /// </summary>
        /// <param name="item">LVITEM</param>
        /// <returns></returns>
        public bool GetItem(out LVITEM item)
        {
            return Util.ReadStructRemote(Handle, out item, (IntPtr hProcess, IntPtr addr) => ListView_GetItem(Handle, addr), null);
        }

        /// <summary>
        /// 获取列表视图控件中Item数量
        /// </summary>
        /// <returns></returns>
        public int GetItemCount()
        {
            return ListView_GetItemCount(Handle);
        }

        /// <summary>
        /// 获取列表视图控件中Item位置
        /// </summary>
        /// <param name="i">第i个Item</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        public bool GetItemPosition(int i, out int x, out int y)
        {
            Point point;
            bool ret;

            ret = GetItemPosition(i, out point);
            x = point.x;
            y = point.y;
            return ret;
        }

        /// <summary>
        /// 获取列表视图控件中Item位置
        /// </summary>
        /// <param name="i">第i个Item</param>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public bool GetItemPosition(int i, out Point point)
        {
            return Util.ReadStructRemote(Handle, out point, (IntPtr hProcess, IntPtr addr) => ListView_GetItemPosition(Handle, i, addr), null);
        }

        /// <summary>
        /// 获取列表视图控件中Item的文本
        /// </summary>
        /// <param name="i">The index of the list-view item.</param>
        /// <param name="iSubItem">The index of the subitem. To retrieve the item text, set iSubItem to zero.</param>
        /// <returns></returns>
        public unsafe string GetItemText(int i, int iSubItem)
        {
            LVITEM item;
            IntPtr pStr;
            string text;

            text = null;
            pStr = IntPtr.Zero;
            item = new LVITEM
            {
                iSubItem = iSubItem,
                cchTextMax = 0x1000
            };
            Util.WriteStructRemote(Handle, ref item, (IntPtr hProcess, IntPtr addr) =>
            {
                pStr = VirtualAllocEx(hProcess, IntPtr.Zero, 0x1000, MEM_COMMIT, PAGE_READWRITE);
                //分配内存用于写入字符串
                if (pStr == IntPtr.Zero)
                    return false;
                item.pszText = (char*)pStr;
                //设置缓存区地址
                return true;
            }, (IntPtr hProcess, IntPtr addr) =>
            {
                try
                {
                    if (ListView_GetItemText(Handle, i, addr, 0x1000) == 0)
                        return false;
                    return MemoryIO.ReadStringInternal(hProcess, (IntPtr)item.pszText, out text, 0x1000, true);
                }
                finally
                {
                    VirtualFreeEx(hProcess, pStr, 0, MEM_RELEASE);
                }
            });
            return text;
        }

        /// <summary>
        /// 重绘列表视图控件中所有Item
        /// </summary>
        /// <returns></returns>
        public bool RedrawItems()
        {
            int iLast;

            iLast = GetItemCount() - 1;
            //获取最大索引
            if (iLast < 0)
                return false;
            return RedrawItems(0, iLast);
        }

        /// <summary>
        /// 重绘列表视图控件中指定Item
        /// </summary>
        /// <param name="iFirst">索引起始</param>
        /// <param name="iLast">索引结束</param>
        /// <returns></returns>
        public bool RedrawItems(int iFirst, int iLast)
        {
            return ListView_RedrawItems(Handle, iFirst, iLast);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="style">扩展样式</param>
        public void SetExtendedStyle(ExtendedListViewStyles style)
        {
            ListView_SetExtendedListViewStyle(Handle, style);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="mask">（Google翻译，懒得自己翻译了）指定dwExStyle中哪些样式将受到影响的DWORD值。此参数可以是扩展列表视图控件样式的组合。只有dwExMask中的扩展样式将被更改。所有其他样式将保持原样。如果此参数为零，则dwExStyle中的所有样式将受到影响。</param>
        /// <param name="style">扩展样式</param>
        public void SetExtendedStyleEx(ExtendedListViewStyles mask, ExtendedListViewStyles style)
        {
            ListView_SetExtendedListViewStyleEx(Handle, mask, style);
        }

        /// <summary>
        /// 设置列表视图控件中Item位置
        /// </summary>
        /// <param name="i">第i个Item</param>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public bool SetItemPosition(int i, Point point)
        {
            return SetItemPosition(i, point.x, point.y);
        }

        /// <summary>
        /// 设置列表视图控件中Item位置
        /// </summary>
        /// <param name="i">第i个Item</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        public bool SetItemPosition(int i, int x, int y)
        {
            return ListView_SetItemPosition(Handle, i, x, y) && RedrawItems(i, i);
        }
    }
}
