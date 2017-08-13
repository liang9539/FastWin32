using System;
using static FastWin32.Macro.CommCtrl;

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
        /// 获取列表视图控件中Item数量
        /// </summary>
        /// <returns></returns>
        public int GetItemCount()
        {
            return ListView_GetItemCount(_handle);
        }

        /// <summary>
        /// 删除列表视图控件中指定Item
        /// </summary>
        /// <param name="i">指定Item的索引</param>
        /// <returns></returns>
        public bool DeleteItem(int i)
        {
            return ListView_DeleteItem(_handle, i);
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
            return ListView_SetItemPosition(_handle, i, x, y) && RedrawItems(i, i);
        }

        /// <summary>
        /// 获取列表视图控件中Item位置
        /// </summary>
        /// <param name="i">第i个Item</param>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public bool GetItemPosition(int i, out Point point)
        {
            return Util.ReadStructRemote(_handle, out point, (IntPtr addr) => ListView_GetItemPosition(_handle, i, addr));
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
            return ListView_RedrawItems(_handle, iFirst, iLast);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="style">扩展样式</param>
        public void SetExtendedStyle(ExtendedListViewStyles style)
        {
            ListView_SetExtendedListViewStyle(_handle, style);
        }

        /// <summary>
        /// 设置列表视图控件扩展样式
        /// </summary>
        /// <param name="mask">（Google翻译，懒得自己翻译了）指定dwExStyle中哪些样式将受到影响的DWORD值。此参数可以是扩展列表视图控件样式的组合。只有dwExMask中的扩展样式将被更改。所有其他样式将保持原样。如果此参数为零，则dwExStyle中的所有样式将受到影响。</param>
        /// <param name="style">扩展样式</param>
        public void SetExtendedStyle(ExtendedListViewStyles mask, ExtendedListViewStyles style)
        {
            ListView_SetExtendedListViewStyleEx(_handle, mask, style);
        }

        /// <summary>
        /// 获取列表视图控件扩展样式
        /// </summary>
        /// <returns></returns>
        public ExtendedListViewStyles GetExtendedListViewStyle()
        {
            return ListView_GetExtendedListViewStyle(_handle);
        }
    }
}
